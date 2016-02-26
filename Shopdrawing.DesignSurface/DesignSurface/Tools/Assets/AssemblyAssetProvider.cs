// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Assets.AssemblyAssetProvider
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.Expression.Framework.Extensions.Enumerable;
using Microsoft.Expression.Project;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Metadata;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;

namespace Microsoft.Expression.DesignSurface.Tools.Assets
{
  internal class AssemblyAssetProvider : AssetProvider
  {
    private static ITypeId[] AllDerivedTypesExcludedList = new ITypeId[2]
    {
      PlatformTypes.Window,
      PlatformTypes.Page
    };
    private static string[] ExcludedTypesList = new string[6]
    {
      "UIElement",
      "FrameworkElement",
      "AdornedElementPlaceholder",
      "ScrollBarPanel",
      "ContextMenu",
      "ToolTip"
    };
    private IList<Asset> asyncAssets = (IList<Asset>) new List<Asset>();
    private IProjectContext projectContext;
    private IAssembly assembly;
    private string assemblyFilename;
    private string assemblyFullName;
    private string assemblyName;
    private bool isMainAssembly;
    private IAssemblyLoggingService assemblyLoggingService;
    private string assemblyLoadingContext;
    private AssetTypeHelper typeHelper;

    public bool LoadingInSeparateAppDomain { get; private set; }

    public string AssemblyName
    {
      get
      {
        return this.assemblyName;
      }
    }

    public string AssemblyFullName
    {
      get
      {
        return this.assemblyFullName;
      }
    }

    public AssemblyAssetProvider(IProjectContext projectContext, IAssembly assembly, bool isMainAssembly)
    {
      this.projectContext = projectContext;
      this.assembly = assembly;
      if (this.assembly != null && this.assembly.IsLoaded)
      {
        this.assemblyFullName = this.assembly.FullName;
        this.assemblyName = AssemblyHelper.GetAssemblyName(this.assembly).Name;
      }
      this.isMainAssembly = isMainAssembly;
      this.InitializeTypes();
    }

    public AssemblyAssetProvider(IProjectContext projectContext, IAssemblyLoggingService assemblyLoggingService, string filename)
    {
      this.projectContext = projectContext;
      this.assemblyLoggingService = assemblyLoggingService;
      this.assemblyFilename = filename;
      this.LoadingInSeparateAppDomain = true;
      ProjectXamlContext projectXamlContext = projectContext as ProjectXamlContext;
      if (projectXamlContext != null)
        this.assemblyLoadingContext = projectXamlContext.ProjectXml;
      this.InitializeTypes();
    }

    private void InitializeTypes()
    {
      this.typeHelper = new AssetTypeHelper(this.projectContext, (IPrototypingService) null);
      this.typeHelper.EnforceTypeCaches();
    }

    internal bool AsynchronizedInitialize(AppDomain typeLoadingAppDomain, string workingFolder, IBackgroundWorkerWrapper worker)
    {
      bool keepInAggregator = true;
      using (PerformanceUtility.PerformanceSequence(PerformanceEvent.AssetProviderAsyncUpdateAssets))
      {
        AssemblyAssetProvider.AssemblyLocationMap mainAppdomainAssemblies = new AssemblyAssetProvider.AssemblyLocationMap(AppDomain.CurrentDomain);
        AssemblyAssetProvider.AppDomainIsolatedAssembly isolatedAssembly = (AssemblyAssetProvider.AppDomainIsolatedAssembly) typeLoadingAppDomain.CreateInstanceAndUnwrap(typeof (AssemblyAssetProvider.AppDomainIsolatedAssembly).Assembly.FullName, typeof (AssemblyAssetProvider.AppDomainIsolatedAssembly).FullName);
        string[] referencedAssemblies = Enumerable.ToArray<string>(Enumerable.Select<IAssembly, string>(Enumerable.Where<IAssembly>((IEnumerable<IAssembly>) this.projectContext.AssemblyReferences, (Func<IAssembly, bool>) (assembly => assembly.IsLoaded)), (Func<IAssembly, string>) (assembly => assembly.FullName)));
        string[] referencedAssemblyNames = Enumerable.ToArray<string>(Enumerable.Select<IAssembly, string>(Enumerable.Where<IAssembly>((IEnumerable<IAssembly>) this.projectContext.AssemblyReferences, (Func<IAssembly, bool>) (assembly =>
        {
          if (assembly.IsLoaded)
            return !assembly.IsResolvedImplicitAssembly;
          return false;
        })), (Func<IAssembly, string>) (assembly => assembly.Name)));
        AssemblyAssetProvider.FakeType[] fakeTypeArray = isolatedAssembly.Initialize(this.assemblyFilename, referencedAssemblies, mainAppdomainAssemblies, this.assemblyLoadingContext, workingFolder, worker, new AssemblyAssetProvider.BuildManagerProxy());
        if (fakeTypeArray != null)
        {
          string name = isolatedAssembly.Name;
          this.assemblyFullName = isolatedAssembly.FullName;
          this.assemblyName = isolatedAssembly.Name;
          AssemblyNameAndLocation[] assemblyReferences = isolatedAssembly.ResolvableAssemblyReferences;
          if (this.typeHelper.IsAssemblySupported(this.assemblyFullName, true))
          {
            foreach (AssemblyAssetProvider.FakeType fakeType in fakeTypeArray)
            {
              IType type1 = (IType) new AssemblyAssetProvider.UnreferencedType((ITypeResolver) this.projectContext, this.projectContext.Platform.Metadata.CreateAssembly(name), fakeType);
              if (this.DefaultTypeFilter(type1))
              {
                AssetTypeHelper assetTypeHelper = this.typeHelper;
                bool flag1 = false;
                IType type2 = type1;
                int num = flag1 ? true : false;
                if (assetTypeHelper.IsTypeSupported(type2, num != 0))
                {
                  bool flag2 = false;
                  if (fakeType.Examples != null)
                  {
                    foreach (ExampleAssetInfo exampleInfo in fakeType.Examples)
                    {
                      if (this.ShouldCreateAsset(type1, exampleInfo))
                      {
                        flag2 |= string.IsNullOrEmpty(exampleInfo.DisplayName);
                        this.asyncAssets.Add((Asset) new TypeAsset(type1, fakeType.DisplayName, exampleInfo, this.assemblyFilename, assemblyReferences));
                      }
                    }
                  }
                  if (!flag2 && this.ShouldCreateAsset(type1, (ExampleAssetInfo) null))
                    this.asyncAssets.Add((Asset) AssemblyAssetProvider.CreateTypeAsset(type1, fakeType.DisplayName, (ExampleAssetInfo) null, this.assemblyFilename, assemblyReferences));
                }
              }
            }
          }
        }
        if (worker != null && worker.CancellationPending)
          return false;
        worker.InvokeUIThread(DispatcherPriority.Background, (Action) (() =>
        {
          if (this.asyncAssets.Count == 0 || referencedAssemblyNames == null || Enumerable.Contains<string>((IEnumerable<string>) referencedAssemblyNames, this.assemblyName))
            keepInAggregator = false;
          if (this.asyncAssets.Count > 0)
            EnumerableExtensions.ForEach<Asset>((IEnumerable<Asset>) this.asyncAssets, (Action<Asset>) (asset => this.Assets.Add(asset)));
          this.NotifyAssetsChanged();
          this.NeedsUpdate = false;
          this.asyncAssets = (IList<Asset>) null;
        }));
        if (this.assemblyLoggingService != null)
        {
          foreach (AssemblyNameAndLocation assemblyNameAndLocation in isolatedAssembly.AssembliesResolved)
            this.assemblyLoggingService.Log((AssemblyLoggingEvent) new AssemblyResolveEvent(assemblyNameAndLocation.AssemblyName, assemblyNameAndLocation.Location != null, AssemblyLoadingAppDomain.AssetTool));
          if (this.assemblyFullName != null)
            this.assemblyLoggingService.Log((AssemblyLoggingEvent) new AssemblyLoadedEvent(this.assemblyFullName, this.assemblyFilename, AssemblyLoadingAppDomain.AssetTool));
        }
        return keepInAggregator;
      }
    }

    internal void SynchronizedInitialize()
    {
      Type[] typeArray = Type.EmptyTypes;
      IAssembly assembly = this.assembly;
      if (assembly != null && assembly.IsLoaded)
      {
        if (this.typeHelper.IsAssemblySupported(assembly.FullName, false))
        {
          try
          {
            typeArray = AssemblyHelper.GetTypes(assembly);
          }
          catch (ReflectionTypeLoadException ex)
          {
          }
        }
      }
      foreach (Type type1 in typeArray)
      {
        if (this.typeHelper.IsRuntimeTypeSupportedByAssetTool(type1, this.isMainAssembly))
        {
          IType type2;
          try
          {
            type2 = this.projectContext.GetType(type1);
          }
          catch (IOException ex)
          {
            continue;
          }
          if (type2 != null && this.DefaultTypeFilter(type2))
          {
            AssetTypeHelper assetTypeHelper = this.typeHelper;
            bool flag1 = true;
            IType type3 = type2;
            int num = flag1 ? true : false;
            if (assetTypeHelper.IsTypeSupported(type3, num != 0))
            {
              bool flag2 = false;
              string displayName = AssetTypeHelper.GetDisplayName(type2.RuntimeType) ?? type2.Name;
              ToolboxExampleAttribute attribute = TypeUtilities.GetAttribute<ToolboxExampleAttribute>(type2.RuntimeType);
              if (attribute != null)
              {
                IToolboxExampleFactory toolboxExampleFactory = Activator.CreateInstance(attribute.ToolboxExampleFactoryType) as IToolboxExampleFactory;
                if (toolboxExampleFactory != null && toolboxExampleFactory.Examples != null)
                {
                  IToolboxExample[] toolboxExampleArray = Enumerable.ToArray<IToolboxExample>(toolboxExampleFactory.Examples);
                  for (int index = 0; index < toolboxExampleArray.Length; ++index)
                  {
                    ExampleAssetInfo exampleInfo = ExampleAssetInfo.FromToolboxExample(toolboxExampleArray[index], index);
                    if (this.ShouldCreateAsset(type2, exampleInfo))
                    {
                      flag2 |= string.IsNullOrEmpty(exampleInfo.DisplayName);
                      this.Assets.Add((Asset) AssemblyAssetProvider.CreateTypeAsset(type2, displayName, exampleInfo, (string) null, (AssemblyNameAndLocation[]) null));
                    }
                  }
                }
              }
              if (!flag2 && this.ShouldCreateAsset(type2, (ExampleAssetInfo) null))
                this.Assets.Add((Asset) AssemblyAssetProvider.CreateTypeAsset(type2, displayName, (ExampleAssetInfo) null, (string) null, (AssemblyNameAndLocation[]) null));
            }
          }
        }
      }
      this.NotifyAssetsChanged();
      this.NeedsUpdate = false;
    }

    protected override bool UpdateAssets()
    {
      if (this.LoadingInSeparateAppDomain)
        return false;
      this.SynchronizedInitialize();
      return true;
    }

    private bool ShouldCreateAsset(IType type, ExampleAssetInfo exampleInfo)
    {
      IUnreferencedType unreferencedType = type as IUnreferencedType;
      if (unreferencedType != null)
      {
        if (unreferencedType.IsCreatable)
        {
          if (exampleInfo != null)
            return exampleInfo.IsBrowable;
          return unreferencedType.IsBrowsable;
        }
      }
      else if (AssetTypeHelper.IsTypeCreatable(type.NearestResolvedType.RuntimeType, this.isMainAssembly))
      {
        if (exampleInfo != null)
          return exampleInfo.IsBrowable;
        return AssetTypeHelper.IsTypeBrowsable(type.NearestResolvedType.RuntimeType);
      }
      return false;
    }

    private bool DefaultTypeFilter(IType type)
    {
      if (Array.IndexOf<string>(AssemblyAssetProvider.ExcludedTypesList, type.Name) >= 0)
        return false;
      foreach (ITypeId typeId in AssemblyAssetProvider.AllDerivedTypesExcludedList)
      {
        if (typeId.IsAssignableFrom((ITypeId) type))
          return false;
      }
      if (!this.typeHelper.IsVisualType((ITypeId) type) && !AssetTypeHelper.IsBehaviorType((ITypeId) type) && (!AssetTypeHelper.IsTriggerActionType((ITypeId) type) && !this.typeHelper.IsEffectType((ITypeId) type)) || !JoltHelper.TypeSupported((ITypeResolver) this.projectContext, (ITypeId) type))
        return false;
      IUnreferencedType unreferencedType = type as IUnreferencedType;
      if (unreferencedType != null)
        return unreferencedType.IsCreatable;
      return AssetTypeHelper.IsTypeCreatable(type.NearestResolvedType.RuntimeType, this.isMainAssembly);
    }

    private static TypeAsset CreateTypeAsset(IType type, string displayName, ExampleAssetInfo exampleInfo, string onDemandAssemblyFilename, AssemblyNameAndLocation[] resolvableAssemblyReferences)
    {
      if (AssetTypeHelper.IsTriggerActionType((ITypeId) type))
        return (TypeAsset) new TriggerActionAsset(type, displayName, exampleInfo, onDemandAssemblyFilename, resolvableAssemblyReferences);
      if (AssetTypeHelper.IsBehaviorType((ITypeId) type))
        return (TypeAsset) new BehaviorAsset(type, displayName, exampleInfo, onDemandAssemblyFilename, resolvableAssemblyReferences);
      if (PlatformTypes.Effect.IsAssignableFrom((ITypeId) type))
        return (TypeAsset) new EffectAsset(type, displayName, exampleInfo, onDemandAssemblyFilename, resolvableAssemblyReferences);
      return new TypeAsset(type, displayName, exampleInfo, onDemandAssemblyFilename, resolvableAssemblyReferences);
    }

    private class BuildManagerProxy : MarshalByRefObject
    {
      public void BlockUntilDoneBuild()
      {
        while (Microsoft.Expression.Project.Build.BuildManager.Building || Microsoft.Expression.Project.Build.BuildManager.Finalizing)
          Thread.Sleep(10);
      }
    }

    private class UnreferencedType : IType, IMember, ITypeId, IMemberId, IUnreferencedType, IUnreferencedTypeId
    {
      private ITypeResolver typeResolver;
      private IAssembly assembly;
      private Type type;
      private string name;
      private string fullName;
      private bool isCreatable;
      private bool isBrowsable;
      private string description;
      private DrawingBrush smallIcon;
      private DrawingBrush largeIcon;
      private KnownUnreferencedType[] knownUnreferencedTypes;

      public IAssembly RuntimeAssembly
      {
        get
        {
          return this.assembly;
        }
      }

      public string AssemblyLocation { get; private set; }

      public byte[] AssemblyPublicKey { get; private set; }

      public IEnumerable<CustomAssetCategoryPath> Categories { get; private set; }

      public IType BaseType
      {
        get
        {
          return this.typeResolver.GetType(this.type);
        }
      }

      public Exception InitializationException
      {
        get
        {
          throw new NotImplementedException();
        }
      }

      public bool IsAbstract
      {
        get
        {
          throw new NotImplementedException();
        }
      }

      public bool IsArray
      {
        get
        {
          throw new NotImplementedException();
        }
      }

      public bool IsBinding
      {
        get
        {
          throw new NotImplementedException();
        }
      }

      public bool IsExpression
      {
        get
        {
          throw new NotImplementedException();
        }
      }

      public bool IsGenericType
      {
        get
        {
          throw new NotImplementedException();
        }
      }

      public bool IsInterface
      {
        get
        {
          throw new NotImplementedException();
        }
      }

      public bool IsResource
      {
        get
        {
          throw new NotImplementedException();
        }
      }

      public IType ItemType
      {
        get
        {
          throw new NotImplementedException();
        }
      }

      public ITypeMetadata Metadata
      {
        get
        {
          throw new NotImplementedException();
        }
      }

      public IType NearestResolvedType
      {
        get
        {
          return (IType) this;
        }
      }

      public IType NullableType
      {
        get
        {
          throw new NotImplementedException();
        }
      }

      public IPlatformMetadata PlatformMetadata
      {
        get
        {
          return this.typeResolver.PlatformMetadata;
        }
      }

      public Type RuntimeType
      {
        get
        {
          return this.type;
        }
      }

      public bool SupportsNullValues
      {
        get
        {
          throw new NotImplementedException();
        }
      }

      public TypeConverter TypeConverter
      {
        get
        {
          throw new NotImplementedException();
        }
      }

      public string XamlSourcePath
      {
        get
        {
          throw new NotImplementedException();
        }
      }

      public MemberAccessType Access
      {
        get
        {
          throw new NotImplementedException();
        }
      }

      public IType DeclaringType
      {
        get
        {
          throw new NotImplementedException();
        }
      }

      public ITypeId MemberTypeId
      {
        get
        {
          throw new NotImplementedException();
        }
      }

      public ITypeId DeclaringTypeId
      {
        get
        {
          throw new NotImplementedException();
        }
      }

      public string FullName
      {
        get
        {
          return this.fullName;
        }
      }

      public bool IsResolvable
      {
        get
        {
          return true;
        }
      }

      public MemberType MemberType
      {
        get
        {
          throw new NotImplementedException();
        }
      }

      public string Name
      {
        get
        {
          return this.name;
        }
      }

      public string UniqueName
      {
        get
        {
          throw new NotImplementedException();
        }
      }

      public bool IsBuilt
      {
        get
        {
          throw new NotImplementedException();
        }
      }

      public string Namespace { get; private set; }

      public IXmlNamespace XmlNamespace
      {
        get
        {
          throw new NotImplementedException();
        }
      }

      public bool IsCreatable
      {
        get
        {
          return this.isCreatable;
        }
      }

      public bool IsBrowsable
      {
        get
        {
          return this.isBrowsable;
        }
      }

      public string Description
      {
        get
        {
          return this.description;
        }
      }

      public DrawingBrush SmallIcon
      {
        get
        {
          return this.smallIcon;
        }
      }

      public DrawingBrush LargeIcon
      {
        get
        {
          return this.largeIcon;
        }
      }

      public UnreferencedType(ITypeResolver typeResolver, IAssembly assembly, AssemblyAssetProvider.FakeType fakeType)
      {
        this.typeResolver = typeResolver;
        this.assembly = assembly;
        Assembly assembly1 = (Assembly) null;
        foreach (Assembly assembly2 in AppDomain.CurrentDomain.GetAssemblies())
        {
          if (assembly2.FullName == fakeType.NearestResolvedTypeAssemblyFullName)
          {
            assembly1 = assembly2;
            break;
          }
        }
        this.type = !(assembly1 != (Assembly) null) ? typeof (object) : assembly1.GetType(fakeType.NearestResolvedTypeFullName);
        this.name = fakeType.Name;
        this.Namespace = fakeType.Namespace;
        this.fullName = fakeType.FullName;
        this.isCreatable = fakeType.IsCreatable;
        this.isBrowsable = fakeType.IsBrowsable;
        this.description = fakeType.Description;
        this.AssemblyLocation = fakeType.AssemblyLocation;
        this.AssemblyPublicKey = fakeType.AssemblyPublicKey;
        this.Categories = (IEnumerable<CustomAssetCategoryPath>) Enumerable.ToList<CustomAssetCategoryPath>((IEnumerable<CustomAssetCategoryPath>) fakeType.Categories);
        this.knownUnreferencedTypes = fakeType.KnownUnreferencedTypes;
        this.smallIcon = AssemblyAssetProvider.UnreferencedType.InitializeIcon(fakeType.SmallIcon, fakeType.SmallIconName);
        this.largeIcon = AssemblyAssetProvider.UnreferencedType.InitializeIcon(fakeType.LargeIcon, fakeType.LargeIconName);
      }

      private static DrawingBrush InitializeIcon(Stream stream, string name)
      {
        if (stream == null)
          return (DrawingBrush) null;
        byte[] buffer = new byte[stream.Length];
        stream.Read(buffer, 0, (int) stream.Length);
        return IconMapper.CreateDrawingBrushFromStream((Stream) new MemoryStream(buffer), name);
      }

      public IConstructorArgumentProperties GetConstructorArgumentProperties()
      {
        throw new NotImplementedException();
      }

      public IList<IConstructor> GetConstructors()
      {
        throw new NotImplementedException();
      }

      public IEnumerable<IEvent> GetEvents(MemberAccessTypes access)
      {
        throw new NotImplementedException();
      }

      public IList<IType> GetGenericTypeArguments()
      {
        throw new NotImplementedException();
      }

      public IEnumerable<Microsoft.Expression.DesignModel.Metadata.IProperty> GetProperties(MemberAccessTypes access)
      {
        throw new NotImplementedException();
      }

      public bool HasDefaultConstructor(bool supportInternal)
      {
        throw new NotImplementedException();
      }

      public void InitializeClass()
      {
        throw new NotImplementedException();
      }

      public bool IsInProject(ITypeResolver typeResolver)
      {
        throw new NotImplementedException();
      }

      public IMember Clone(ITypeResolver typeResolver)
      {
        throw new NotImplementedException();
      }

      public IMemberId GetMember(MemberType memberTypes, string memberName, MemberAccessTypes access)
      {
        throw new NotImplementedException();
      }

      public bool IsAssignableFrom(ITypeId type)
      {
        throw new NotImplementedException();
      }

      public bool IsKnownUnreferencedType(KnownUnreferencedType knownUnreferencedType)
      {
        if (this.knownUnreferencedTypes != null)
          return Enumerable.Contains<KnownUnreferencedType>((IEnumerable<KnownUnreferencedType>) this.knownUnreferencedTypes, knownUnreferencedType);
        return false;
      }
    }

    private class AppDomainIsolatedAssembly : MarshalByRefObject
    {
      private List<AssemblyResolveEvent> resolveEvents = new List<AssemblyResolveEvent>();
      private AssemblyAssetProvider.AssemblyLocationMap mainAppdomainAssemblies;
      private Microsoft.Build.Evaluation.Project assemblyLoadingProject;
      private string assemblyLoadingContext;
      private string workingDirectory;
      private Assembly assembly;
      private HashSet<string> directories;
      private HashSet<string> referencedAssemblies;
      private AssemblyAssetProvider.BuildManagerProxy buildManagerProxy;

      public string Name
      {
        get
        {
          return AssemblyHelper.GetAssemblyName(this.assembly).Name;
        }
      }

      public string FullName
      {
        get
        {
          return this.assembly.FullName;
        }
      }

      public AssemblyNameAndLocation[] ResolvableAssemblyReferences { get; private set; }

      public AssemblyNameAndLocation[] AssembliesResolved
      {
        get
        {
          List<AssemblyNameAndLocation> list = new List<AssemblyNameAndLocation>();
          foreach (AssemblyResolveEvent assemblyResolveEvent in this.resolveEvents)
            list.Add(new AssemblyNameAndLocation()
            {
              AssemblyName = assemblyResolveEvent.AssemblyName,
              Location = assemblyResolveEvent.Success ? "" : (string) null
            });
          return list.ToArray();
        }
      }

      public AssemblyAssetProvider.FakeType[] Initialize(string filename, string[] referencedAssemblies, AssemblyAssetProvider.AssemblyLocationMap mainAppdomainAssemblies, string assemblyLoadingContext, string workingDirectory, IBackgroundWorkerWrapper worker, AssemblyAssetProvider.BuildManagerProxy buildManagerProxy)
      {
        this.referencedAssemblies = new HashSet<string>((IEnumerable<string>) referencedAssemblies, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        this.mainAppdomainAssemblies = mainAppdomainAssemblies;
        this.assemblyLoadingContext = assemblyLoadingContext;
        this.workingDirectory = workingDirectory;
        this.buildManagerProxy = buildManagerProxy;
        List<AssemblyAssetProvider.FakeType> list = new List<AssemblyAssetProvider.FakeType>();
        try
        {
          AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(this.OnCurrentDomainAssemblyResolve);
          this.directories = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          this.directories.Add(Microsoft.Expression.Framework.Documents.PathHelper.GetDirectoryNameOrRoot(filename));
          this.assembly = Assembly.LoadFile(filename);
          if (worker != null && worker.CancellationPending)
            return (AssemblyAssetProvider.FakeType[]) null;
          foreach (string str in AssemblyAssetAggregator.GetDesignFileNames(filename, false))
          {
            string directoryNameOrRoot = Microsoft.Expression.Framework.Documents.PathHelper.GetDirectoryNameOrRoot(str);
            this.directories.Add(directoryNameOrRoot);
            Assembly assembly = Assembly.LoadFile(str);
            foreach (string path in ProjectAssemblyHelper.GetSatelliteAssemblyNames(assembly, directoryNameOrRoot))
              Assembly.LoadFile(path);
            this.RegisterAssemblyMetadata(assembly);
          }
          Type[] types = this.assembly.GetTypes();
          DesignSurfaceMetadata.InitializeMetadata();
          if (worker != null && worker.CancellationPending)
            return (AssemblyAssetProvider.FakeType[]) null;
          foreach (Type type in types)
            list.Add(this.CreateType(type));
          this.InitializeResolvableAssemblyReferences();
        }
        catch (Exception ex)
        {
          return (AssemblyAssetProvider.FakeType[]) null;
        }
        finally
        {
          AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(this.OnCurrentDomainAssemblyResolve);
          if (this.assemblyLoadingProject != null)
            Microsoft.Expression.Project.Build.BuildManager.ProjectCollection.UnloadProject(this.assemblyLoadingProject);
        }
        return list.ToArray();
      }

      private void RegisterAssemblyMetadata(Assembly assembly)
      {
        object[] customAttributes;
        try
        {
          customAttributes = assembly.GetCustomAttributes(typeof (ProvideMetadataAttribute), false);
        }
        catch (Exception ex)
        {
          return;
        }
        foreach (object obj in customAttributes)
        {
          ProvideMetadataAttribute metadataAttribute = obj as ProvideMetadataAttribute;
          if (metadataAttribute != null)
          {
            if (typeof (IProvideAttributeTable).IsAssignableFrom(metadataAttribute.MetadataProviderType))
            {
              try
              {
                Microsoft.Expression.DesignModel.Metadata.MetadataStore.AddAttributeTable(((IProvideAttributeTable) Activator.CreateInstance(metadataAttribute.MetadataProviderType)).AttributeTable);
              }
              catch (Exception ex)
              {
              }
            }
          }
        }
      }

      private Assembly OnCurrentDomainAssemblyResolve(object sender, ResolveEventArgs args)
      {
        System.Reflection.AssemblyName name = new System.Reflection.AssemblyName(args.Name);
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
          if (assembly.FullName.Equals(name.FullName, StringComparison.OrdinalIgnoreCase))
          {
            this.resolveEvents.Add(new AssemblyResolveEvent(args.Name, true, AssemblyLoadingAppDomain.AssetTool));
            return assembly;
          }
        }
        foreach (string path1 in this.directories)
        {
          foreach (string path2 in Directory.GetFiles(path1, "*.dll"))
          {
            if (name.Name == System.IO.Path.GetFileNameWithoutExtension(path2))
            {
              this.resolveEvents.Add(new AssemblyResolveEvent(args.Name, true, AssemblyLoadingAppDomain.AssetTool));
              return Assembly.LoadFile(path2);
            }
          }
        }
        Assembly assembly1 = (Assembly) null;
        string assemblyLocation = this.GetAssemblyLocation(name);
        if (!string.IsNullOrEmpty(assemblyLocation))
          assembly1 = Assembly.LoadFile(assemblyLocation);
        this.resolveEvents.Add(new AssemblyResolveEvent(args.Name, assembly1 != (Assembly) null, AssemblyLoadingAppDomain.AssetTool));
        return assembly1;
      }

      private string GetAssemblyLocation(System.Reflection.AssemblyName name)
      {
        if (this.assemblyLoadingContext != null || this.assemblyLoadingProject != null)
        {
          if (this.assemblyLoadingProject == null)
          {
            string s = this.assemblyLoadingContext;
            this.assemblyLoadingContext = (string) null;
            this.assemblyLoadingProject = Microsoft.Expression.Project.Build.BuildManager.ProjectCollection.LoadProject((XmlReader) new XmlTextReader((TextReader) new StringReader(s)));
          }
          if (this.assemblyLoadingProject != null)
          {
            this.assemblyLoadingProject.FullPath = this.workingDirectory;
            ProjectInstance projectInstance = this.assemblyLoadingProject.CreateProjectInstance();
            projectInstance.AddItem("Reference", name.FullName);
            IEnumerable<ProjectItemInstance> source = (IEnumerable<ProjectItemInstance>) projectInstance.GetItems("ReferencePath");
            if (Enumerable.Count<ProjectItemInstance>(source) <= 0)
            {
              this.buildManagerProxy.BlockUntilDoneBuild();
              projectInstance.Build(new string[2]
              {
                "ResolveComReferences",
                "ResolveAssemblyReferences"
              }, (IEnumerable<ILogger>) null);
              source = (IEnumerable<ProjectItemInstance>) projectInstance.GetItems("ReferencePath");
            }
            foreach (ProjectItemInstance projectItemInstance in source)
            {
              if (name.FullName.Equals(projectItemInstance.GetMetadataValue("FusionName"), StringComparison.OrdinalIgnoreCase))
              {
                string evaluatedInclude = projectItemInstance.EvaluatedInclude;
                if (evaluatedInclude != null && File.Exists(evaluatedInclude))
                  return evaluatedInclude;
              }
            }
          }
        }
        return (string) null;
      }

      private void InitializeResolvableAssemblyReferences()
      {
        List<AssemblyNameAndLocation> result = new List<AssemblyNameAndLocation>();
        HashSet<string> visited = new HashSet<string>((IEnumerable<string>) this.referencedAssemblies);
        this.AddReferencedAssemblies(this.assembly, (System.Reflection.AssemblyName) null, result, visited);
        this.ResolvableAssemblyReferences = result.ToArray();
      }

      private void AddReferencedAssemblies(Assembly assembly, System.Reflection.AssemblyName name, List<AssemblyNameAndLocation> result, HashSet<string> visited)
      {
        if (visited.Contains(assembly.FullName))
          return;
        visited.Add(assembly.FullName);
        foreach (System.Reflection.AssemblyName name1 in assembly.GetReferencedAssemblies())
        {
          Assembly assembly1 = (Assembly) null;
          try
          {
            assembly1 = Assembly.Load(name1.FullName);
            string directoryNameOrRoot = Microsoft.Expression.Framework.Documents.PathHelper.GetDirectoryNameOrRoot(typeof (AssemblyAssetProvider.AppDomainIsolatedAssembly).Assembly.Location);
            if (!assembly1.Location.StartsWith(directoryNameOrRoot, StringComparison.OrdinalIgnoreCase))
            {
              Version version = assembly1.GetName().Version;
              if (version.Major == name1.Version.Major)
              {
                if (version.Minor == name1.Version.Minor)
                  goto label_12;
              }
              string assemblyLocation = this.GetAssemblyLocation(name1);
              if (!string.IsNullOrEmpty(assemblyLocation))
              {
                if (assembly1 != this.assembly)
                {
                  result.Add(new AssemblyNameAndLocation()
                  {
                    AssemblyName = name1.Name,
                    Location = assemblyLocation
                  });
                  continue;
                }
              }
            }
            else
              continue;
          }
          catch (Exception ex)
          {
          }
label_12:
          if (assembly1 != (Assembly) null)
            this.AddReferencedAssemblies(assembly1, name1, result, visited);
        }
        if (!(assembly != this.assembly))
          return;
        result.Add(new AssemblyNameAndLocation()
        {
          AssemblyName = name.Name,
          Location = assembly.Location
        });
      }

      private AssemblyAssetProvider.FakeType CreateType(Type type)
      {
        if (type == (Type) null)
          return (AssemblyAssetProvider.FakeType) null;
        return new AssemblyAssetProvider.FakeType(this.GetResolvedType(type), type);
      }

      private Type GetResolvedType(Type type)
      {
        return this.InternalGetResolvedType(type, false);
      }

      private bool IsResolvableType(Type type)
      {
        return this.InternalGetResolvedType(type, true) != (Type) null;
      }

      private Type InternalGetResolvedType(Type type, bool sameOrNull)
      {
        for (; type != (Type) null; type = type.BaseType)
        {
          if (this.mainAppdomainAssemblies[type.Assembly.FullName] != null && type.FullName != null)
          {
            if (!type.IsGenericType)
              return type;
            Type[] genericTypeArguments = PlatformTypeHelper.GetGenericTypeArguments(type);
            if (genericTypeArguments != null)
            {
              bool flag = true;
              foreach (Type type1 in genericTypeArguments)
              {
                if (!this.IsResolvableType(type1))
                {
                  flag = false;
                  break;
                }
              }
              if (flag)
                return type;
            }
          }
          if (sameOrNull)
            return (Type) null;
        }
        return typeof (object);
      }
    }

    [Serializable]
    private class FakeType
    {
      private string displayName;
      private string name;
      private string fullName;
      private string nearestResolvedTypeFullName;
      private string nearestResolvedTypeAssemblyFullName;
      private bool isCreatable;
      private bool isBrowsable;
      private string description;
      private Stream smallIcon;
      private string smallIconName;
      private Stream largeIcon;
      private string largeIconName;

      public ExampleAssetInfo[] Examples { get; private set; }

      public KnownUnreferencedType[] KnownUnreferencedTypes { get; private set; }

      public string NearestResolvedTypeFullName
      {
        get
        {
          return this.nearestResolvedTypeFullName;
        }
      }

      public string NearestResolvedTypeAssemblyFullName
      {
        get
        {
          return this.nearestResolvedTypeAssemblyFullName;
        }
      }

      public byte[] AssemblyPublicKey { get; private set; }

      public string FullName
      {
        get
        {
          return this.fullName;
        }
      }

      public string Name
      {
        get
        {
          return this.name;
        }
      }

      public string DisplayName
      {
        get
        {
          return this.displayName;
        }
      }

      public bool IsCreatable
      {
        get
        {
          return this.isCreatable;
        }
      }

      public bool IsBrowsable
      {
        get
        {
          return this.isBrowsable;
        }
      }

      public string Description
      {
        get
        {
          return this.description;
        }
      }

      public Stream SmallIcon
      {
        get
        {
          return this.smallIcon;
        }
      }

      public string SmallIconName
      {
        get
        {
          return this.smallIconName;
        }
      }

      public Stream LargeIcon
      {
        get
        {
          return this.largeIcon;
        }
      }

      public string LargeIconName
      {
        get
        {
          return this.largeIconName;
        }
      }

      public string Namespace { get; private set; }

      public string AssemblyLocation { get; private set; }

      public CustomAssetCategoryPath[] Categories { get; private set; }

      public FakeType(Type nearestResolvedType, Type realType)
      {
        this.nearestResolvedTypeFullName = nearestResolvedType.FullName;
        this.nearestResolvedTypeAssemblyFullName = nearestResolvedType.Assembly.FullName;
        this.name = realType.Name;
        this.fullName = realType.FullName;
        this.AssemblyPublicKey = AssemblyHelper.GetAssemblyName(realType.Assembly).GetPublicKeyToken();
        this.isCreatable = AssetTypeHelper.IsTypeCreatable(realType, false);
        this.isBrowsable = AssetTypeHelper.IsTypeBrowsable(realType);
        this.description = AssetInfoModel.GetDescription(realType);
        this.displayName = AssetTypeHelper.GetDisplayName(realType);
        this.smallIcon = IconMapper.GetExtensibleDrawingBrushStream(realType, 12, 12, out this.smallIconName);
        this.largeIcon = IconMapper.GetExtensibleDrawingBrushStream(realType, 24, 24, out this.largeIconName);
        this.Namespace = realType.Namespace;
        this.AssemblyLocation = realType.Assembly.Location;
        this.Categories = Enumerable.ToArray<CustomAssetCategoryPath>(AssetTypeHelper.GetCustomAssetCategoryPaths(realType));
        ToolboxExampleAttribute attribute = TypeUtilities.GetAttribute<ToolboxExampleAttribute>(realType);
        if (attribute != null)
        {
          IToolboxExampleFactory toolboxExampleFactory = Activator.CreateInstance(attribute.ToolboxExampleFactoryType) as IToolboxExampleFactory;
          if (toolboxExampleFactory != null && toolboxExampleFactory.Examples != null)
          {
            IToolboxExample[] toolboxExampleArray = Enumerable.ToArray<IToolboxExample>(toolboxExampleFactory.Examples);
            List<ExampleAssetInfo> list = new List<ExampleAssetInfo>();
            for (int index = 0; index < toolboxExampleArray.Length; ++index)
              list.Add(ExampleAssetInfo.FromToolboxExample(toolboxExampleArray[index], index));
            this.Examples = list.ToArray();
          }
        }
        Attribute[] attributes = TypeUtilities.GetAttributes((MemberInfo) realType, typeof (KnownUnreferencedTypeAttribute), true);
        if (attributes == null || attributes.Length <= 0)
          return;
        this.KnownUnreferencedTypes = Enumerable.ToArray<KnownUnreferencedType>(Enumerable.Select<KnownUnreferencedTypeAttribute, KnownUnreferencedType>(Enumerable.Cast<KnownUnreferencedTypeAttribute>((IEnumerable) attributes), (Func<KnownUnreferencedTypeAttribute, KnownUnreferencedType>) (a => a.KnownUnreferencedType)));
      }
    }

    [Serializable]
    private class AssemblyLocationMap
    {
      private Dictionary<string, string> names = new Dictionary<string, string>();

      public string this[string fullName]
      {
        get
        {
          string str;
          if (this.names.TryGetValue(fullName, out str))
            return str;
          return (string) null;
        }
      }

      public AssemblyLocationMap(AppDomain appDomain)
      {
        foreach (Assembly assembly in appDomain.GetAssemblies())
        {
          if (!assembly.IsDynamic && !this.names.ContainsKey(assembly.FullName))
            this.names.Add(assembly.FullName, assembly.Location);
        }
      }
    }
  }
}
