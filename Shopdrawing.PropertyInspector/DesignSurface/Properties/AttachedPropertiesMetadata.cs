// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Properties.AttachedPropertiesMetadata
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Data;
using Microsoft.Expression.Project;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Microsoft.Expression.DesignSurface.Properties
{
  public sealed class AttachedPropertiesMetadata : IAttachedPropertiesMetadata, IDisposable
  {
    private static Dictionary<IPlatform, AttachedPropertiesMetadata.AttachedPropertyMetadataBackgroundWorker> platformWorkerTable = new Dictionary<IPlatform, AttachedPropertiesMetadata.AttachedPropertyMetadataBackgroundWorker>();
    private static object platformTypesWorkerLock = new object();
    private IProjectContext projectContext;
    private IPlatformService platformService;
    private IAssemblyService assemblyService;
    private AttachedPropertiesMetadata.AttachedPropertyMetadataBackgroundWorker projectTypesWorker;
    private AttachedPropertiesMetadata.AttachedPropertyMetadataBackgroundWorker platformTypesWorker;

    public bool IsFinished { get; private set; }

    public event EventHandler FinishedScanning;

    public AttachedPropertiesMetadata(IProjectContext projectContext, IPlatformService platformService, IAssemblyService assemblyService)
      : this(projectContext, projectContext.Platform.Metadata.RuntimeContext, projectContext.Platform.Metadata.ReferenceContext, platformService, assemblyService)
    {
    }

    public AttachedPropertiesMetadata(IProjectContext projectContext, IPlatformRuntimeContext runtimeContext, IPlatformReferenceContext referenceContext, IPlatformService platformService, IAssemblyService assemblyService)
    {
      this.projectContext = projectContext;
      this.platformService = platformService;
      this.platformService.PlatformDisposing += new EventHandler<PlatformEventArgs>(this.AttachedPropertiesMetadata_PlatformDisposing);
      this.assemblyService = assemblyService;
      this.projectTypesWorker = new AttachedPropertiesMetadata.AttachedPropertyMetadataBackgroundWorker(this.projectContext, runtimeContext, referenceContext, true);
      this.projectTypesWorker.Complete += new EventHandler(this.OnAttachedPropertiesScanningComplete);
      lock (AttachedPropertiesMetadata.platformTypesWorkerLock)
      {
        if (AttachedPropertiesMetadata.platformWorkerTable.TryGetValue(this.projectContext.Platform, out this.platformTypesWorker))
          return;
        this.platformTypesWorker = new AttachedPropertiesMetadata.AttachedPropertyMetadataBackgroundWorker(this.projectContext, runtimeContext, referenceContext, false);
        this.platformTypesWorker.Complete += new EventHandler(this.OnAttachedPropertiesScanningComplete);
        AttachedPropertiesMetadata.platformWorkerTable.Add(this.projectContext.Platform, this.platformTypesWorker);
      }
    }

    public IAttachedPropertiesAccessToken Access()
    {
      return (IAttachedPropertiesAccessToken) new AttachedPropertiesMetadata.AccessToken(this);
    }

    public void OnTypesChanged(TypesChangedEventArgs e)
    {
      lock (this.projectTypesWorker.properties)
      {
        IList<TypeChangedInfo> local_0;
        IList<TypeChangedInfo> local_1;
        this.SeparateAssemblies((IEnumerable<TypeChangedInfo>) e.InvalidatedAssemblies, out local_0, out local_1);
        this.platformTypesWorker.BeginScanAssembliesAsync((IEnumerable<TypeChangedInfo>) local_0);
        this.projectTypesWorker.BeginScanAssembliesAsync((IEnumerable<TypeChangedInfo>) local_1);
      }
    }

    public void CancelScanAssembliesAsync()
    {
      this.projectTypesWorker.Cancel();
    }

    public void BeginScanAssemblies(IEnumerable<TypeChangedInfo> assemblyInfosToScan)
    {
      IList<TypeChangedInfo> platformAssemblies;
      IList<TypeChangedInfo> userAssemblies;
      this.SeparateAssemblies(assemblyInfosToScan, out platformAssemblies, out userAssemblies);
      this.IsFinished = false;
      this.platformTypesWorker.BeginScanAssembliesAsync((IEnumerable<TypeChangedInfo>) platformAssemblies);
      this.projectTypesWorker.BeginScanAssembliesAsync((IEnumerable<TypeChangedInfo>) userAssemblies);
    }

    private void SeparateAssemblies(IEnumerable<TypeChangedInfo> assemblyInfos, out IList<TypeChangedInfo> platformAssemblies, out IList<TypeChangedInfo> userAssemblies)
    {
      platformAssemblies = (IList<TypeChangedInfo>) new List<TypeChangedInfo>();
      userAssemblies = (IList<TypeChangedInfo>) new List<TypeChangedInfo>();
      foreach (TypeChangedInfo typeChangedInfo in assemblyInfos)
      {
        if (this.IsPlatformAssembly(typeChangedInfo.Assembly))
          platformAssemblies.Add(typeChangedInfo);
        else
          userAssemblies.Add(typeChangedInfo);
      }
    }

    private bool IsPlatformAssembly(IAssembly assembly)
    {
      if (assembly != null && assembly.IsLoaded && !assembly.IsDynamic)
      {
        AssemblyName assemblyName = AssemblyHelper.GetAssemblyName(assembly);
        if (this.assemblyService.ResolvePlatformAssembly(assemblyName) != (Assembly) null || this.assemblyService.ResolveLibraryAssembly(assemblyName) != (Assembly) null)
          return true;
        string path = (string) null;
        try
        {
          path = assembly.Location;
        }
        catch (Exception ex)
        {
        }
        if (!string.IsNullOrEmpty(path))
          return this.assemblyService.IsInstalledAssembly(path);
      }
      return false;
    }

    private void AttachedPropertiesMetadata_PlatformDisposing(object sender, PlatformEventArgs e)
    {
      lock (AttachedPropertiesMetadata.platformTypesWorkerLock)
      {
        AttachedPropertiesMetadata.AttachedPropertyMetadataBackgroundWorker local_0;
        if (!AttachedPropertiesMetadata.platformWorkerTable.TryGetValue(e.Platform, out local_0))
          return;
        local_0.Cancel();
        AttachedPropertiesMetadata.platformWorkerTable.Remove(e.Platform);
        this.platformService.PlatformDisposing -= new EventHandler<PlatformEventArgs>(this.AttachedPropertiesMetadata_PlatformDisposing);
      }
    }

    private void OnAttachedPropertiesScanningComplete(object sender, EventArgs e)
    {
      if (this.projectTypesWorker.IsActive || this.platformTypesWorker.IsActive)
        return;
      this.IsFinished = true;
      if (this.FinishedScanning == null)
        return;
      this.FinishedScanning(this, EventArgs.Empty);
    }

    public void Dispose()
    {
      if (this.projectTypesWorker != null)
        this.projectTypesWorker.Complete -= new EventHandler(this.OnAttachedPropertiesScanningComplete);
      if (this.platformService != null)
        this.platformService.PlatformDisposing -= new EventHandler<PlatformEventArgs>(this.AttachedPropertiesMetadata_PlatformDisposing);
      GC.SuppressFinalize(this);
    }

    private class AccessToken : IAttachedPropertiesAccessToken, IDisposable
    {
      private AttachedPropertiesMetadata owner;
      private IAttachedPropertyMetadata[] cachedAllAttachedProperties;

      public AccessToken(AttachedPropertiesMetadata owner)
      {
        bool lockTaken = false;
        this.owner = owner;
        Monitor.Enter((object) owner.projectTypesWorker.properties, ref lockTaken);
      }

      public void Dispose()
      {
        Monitor.Exit(this.owner.projectTypesWorker.properties);
      }

      public IAttachedPropertyMetadata[] AllAttachedProperties()
      {
        if (this.cachedAllAttachedProperties == null || this.owner.platformTypesWorker.IsDirty || this.owner.projectTypesWorker.IsDirty)
        {
          List<IAttachedPropertyMetadata> list;
          lock (AttachedPropertiesMetadata.platformTypesWorkerLock)
            list = Enumerable.ToList<IAttachedPropertyMetadata>(Enumerable.Cast<IAttachedPropertyMetadata>((IEnumerable) this.owner.platformTypesWorker.properties));
          list.AddRange(Enumerable.Cast<IAttachedPropertyMetadata>((IEnumerable) this.owner.projectTypesWorker.properties));
          this.owner.platformTypesWorker.IsDirty = false;
          this.owner.projectTypesWorker.IsDirty = false;
          this.cachedAllAttachedProperties = list.ToArray();
        }
        return this.cachedAllAttachedProperties;
      }

      public IAttachedPropertyMetadata[] AttachedPropertiesForAssembly(string assemblyName)
      {
        int hi;
        int lo;
        lock (AttachedPropertiesMetadata.platformTypesWorkerLock)
        {
          if (AttachedPropertiesMetadata.AttachedPropertyMetadataBackgroundWorker.FindArrayBounds(this.owner.platformTypesWorker.properties, assemblyName, (string) null, (string) null, out hi, out lo))
            return Enumerable.ToArray<IAttachedPropertyMetadata>(Enumerable.Cast<IAttachedPropertyMetadata>((IEnumerable) this.owner.platformTypesWorker.properties.EnumerateRange(lo, hi - lo + 1)));
        }
        if (AttachedPropertiesMetadata.AttachedPropertyMetadataBackgroundWorker.FindArrayBounds(this.owner.projectTypesWorker.properties, assemblyName, (string) null, (string) null, out hi, out lo))
          return Enumerable.ToArray<IAttachedPropertyMetadata>(Enumerable.Cast<IAttachedPropertyMetadata>((IEnumerable) this.owner.projectTypesWorker.properties.EnumerateRange(lo, hi - lo + 1)));
        return new IAttachedPropertyMetadata[0];
      }

      public IAttachedPropertyMetadata[] AttachedPropertiesForAssemblyAndNamespace(string assemblyName, string clrNamespaceName)
      {
        int hi;
        int lo;
        lock (AttachedPropertiesMetadata.platformTypesWorkerLock)
        {
          if (AttachedPropertiesMetadata.AttachedPropertyMetadataBackgroundWorker.FindArrayBounds(this.owner.platformTypesWorker.properties, assemblyName, clrNamespaceName, (string) null, out hi, out lo))
            return Enumerable.ToArray<IAttachedPropertyMetadata>(Enumerable.Cast<IAttachedPropertyMetadata>((IEnumerable) this.owner.platformTypesWorker.properties.EnumerateRange(lo, hi - lo + 1)));
        }
        if (AttachedPropertiesMetadata.AttachedPropertyMetadataBackgroundWorker.FindArrayBounds(this.owner.projectTypesWorker.properties, assemblyName, clrNamespaceName, (string) null, out hi, out lo))
          return Enumerable.ToArray<IAttachedPropertyMetadata>(Enumerable.Cast<IAttachedPropertyMetadata>((IEnumerable) this.owner.projectTypesWorker.properties.EnumerateRange(lo, hi - lo + 1)));
        return new IAttachedPropertyMetadata[0];
      }

      public IAttachedPropertyMetadata[] AttachedPropertiesForType(IType type)
      {
        int hi;
        int lo;
        lock (AttachedPropertiesMetadata.platformTypesWorkerLock)
        {
          if (AttachedPropertiesMetadata.AttachedPropertyMetadataBackgroundWorker.FindArrayBounds(this.owner.platformTypesWorker.properties, type.RuntimeAssembly.Name, type.Namespace, type.Name, out hi, out lo))
            return Enumerable.ToArray<IAttachedPropertyMetadata>(Enumerable.Cast<IAttachedPropertyMetadata>((IEnumerable) this.owner.platformTypesWorker.properties.EnumerateRange(lo, hi - lo + 1)));
        }
        if (AttachedPropertiesMetadata.AttachedPropertyMetadataBackgroundWorker.FindArrayBounds(this.owner.projectTypesWorker.properties, type.RuntimeAssembly.Name, type.Namespace, type.Name, out hi, out lo))
          return Enumerable.ToArray<IAttachedPropertyMetadata>(Enumerable.Cast<IAttachedPropertyMetadata>((IEnumerable) this.owner.projectTypesWorker.properties.EnumerateRange(lo, hi - lo + 1)));
        return new IAttachedPropertyMetadata[0];
      }

      public IAttachedPropertyMetadata[] AttachedPropertiesForType(Type type)
      {
        int hi;
        int lo;
        lock (AttachedPropertiesMetadata.platformTypesWorkerLock)
        {
          if (AttachedPropertiesMetadata.AttachedPropertyMetadataBackgroundWorker.FindArrayBounds(this.owner.platformTypesWorker.properties, AttachedPropertiesMetadata.AttachedPropertyMetadataBackgroundWorker.GetAssemblyName(type), type.Namespace, type.Name, out hi, out lo))
            return Enumerable.ToArray<IAttachedPropertyMetadata>(Enumerable.Cast<IAttachedPropertyMetadata>((IEnumerable) this.owner.platformTypesWorker.properties.EnumerateRange(lo, hi - lo + 1)));
        }
        if (AttachedPropertiesMetadata.AttachedPropertyMetadataBackgroundWorker.FindArrayBounds(this.owner.projectTypesWorker.properties, AttachedPropertiesMetadata.AttachedPropertyMetadataBackgroundWorker.GetAssemblyName(type), type.Namespace, type.Name, out hi, out lo))
          return Enumerable.ToArray<IAttachedPropertyMetadata>(Enumerable.Cast<IAttachedPropertyMetadata>((IEnumerable) this.owner.projectTypesWorker.properties.EnumerateRange(lo, hi - lo + 1)));
        return new IAttachedPropertyMetadata[0];
      }

      public void BeginInitializeAsync()
      {
        this.owner.BeginScanAssemblies(Enumerable.Select<IAssembly, TypeChangedInfo>((IEnumerable<IAssembly>) this.owner.projectContext.AssemblyReferences, (Func<IAssembly, TypeChangedInfo>) (assembly => new TypeChangedInfo(assembly, ModificationType.Modified))));
      }

      public void PreloadType(IType type)
      {
        lock (AttachedPropertiesMetadata.platformTypesWorkerLock)
        {
          int local_0;
          int local_1;
          if (AttachedPropertiesMetadata.AttachedPropertyMetadataBackgroundWorker.FindArrayBounds(this.owner.platformTypesWorker.properties, type.RuntimeAssembly.Name, type.Namespace, type.Name, out local_0, out local_1) || !(type.RuntimeType != (Type) null))
            return;
          this.owner.platformTypesWorker.UpdateAttachedProperties(type.RuntimeType);
        }
      }
    }

    internal class AttachedPropertyMetadataBackgroundWorker
    {
      private static HashSet<string> blacklistedAssemblies = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
      {
        "mscorlib",
        "System",
        "System.Core",
        "System.Net",
        "System.Xml"
      };
      private object initializeListSyncLock = new object();
      private object initializeTypeSyncLock = new object();
      private object typeDictionaryLock = new object();
      private HashSet<string> scannedAssemblies = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      private IProjectContext projectContext;
      private IPlatformRuntimeContext runtimeContext;
      private IPlatformReferenceContext referenceContext;
      private AsyncQueueProcess initializeListProcess;
      private AsyncQueueProcess initializeTypeProcess;
      private Dictionary<AttachedPropertiesMetadata.AttachedPropertyMetadataBackgroundWorker.AttachedPropertyReferenceAssembly, Type[]> typeDictionary;
      private bool shouldRescanDuplicateAssemblies;

      internal bool IsActive { get; private set; }

      internal GapList<AttachedPropertyMetadata> properties { get; private set; }

      internal bool IsDirty { get; set; }

      public event EventHandler Complete;

      public AttachedPropertyMetadataBackgroundWorker(IProjectContext projectContext, IPlatformRuntimeContext runtimeContext, IPlatformReferenceContext referenceContext, bool shouldRescanDuplicateAssemblies)
      {
        this.projectContext = projectContext;
        this.runtimeContext = runtimeContext;
        this.referenceContext = referenceContext;
        this.properties = new GapList<AttachedPropertyMetadata>();
        this.shouldRescanDuplicateAssemblies = shouldRescanDuplicateAssemblies;
      }

      internal static string GetAssemblyName(Type type)
      {
        return AssemblyHelper.GetAssemblyName(type.Assembly).Name;
      }

      internal void Cancel()
      {
        this.IsActive = false;
        lock (this.initializeTypeSyncLock)
        {
          if (this.initializeTypeProcess != null)
          {
            this.initializeTypeProcess.Complete -= new EventHandler(this.InitializeTypeProcess_Complete);
            this.initializeTypeProcess.Kill();
            this.initializeTypeProcess = (AsyncQueueProcess) null;
            lock (this.typeDictionaryLock)
              this.typeDictionary = (Dictionary<AttachedPropertiesMetadata.AttachedPropertyMetadataBackgroundWorker.AttachedPropertyReferenceAssembly, Type[]>) null;
          }
        }
        lock (this.initializeListSyncLock)
        {
          if (this.initializeListProcess == null)
            return;
          this.initializeListProcess.Complete -= new EventHandler(this.OnInitializeListProcessComplete);
          this.initializeListProcess.Kill();
          this.initializeListProcess = (AsyncQueueProcess) null;
          lock (this.properties)
          {
            this.IsDirty = true;
            this.properties.Clear();
          }
        }
      }

      private void ReplacePropertiesForType(Type type, IEnumerable<AttachedPropertyMetadata> attachedProperties)
      {
        string namespaceName = type.Namespace ?? string.Empty;
        int hi;
        int lo;
        if (AttachedPropertiesMetadata.AttachedPropertyMetadataBackgroundWorker.FindArrayBounds(this.properties, AttachedPropertiesMetadata.AttachedPropertyMetadataBackgroundWorker.GetAssemblyName(type), namespaceName, type.Name, out hi, out lo))
        {
          this.IsDirty = true;
          this.properties.RemoveRange(lo, hi - lo + 1);
        }
        else
          lo = ~lo;
        foreach (AttachedPropertyMetadata propertyMetadata in attachedProperties)
        {
          this.IsDirty = true;
          this.properties.Insert(lo, propertyMetadata);
          ++lo;
        }
      }

      internal void BeginScanAssembliesAsync(IEnumerable<TypeChangedInfo> assemblyInfosToScan)
      {
        lock (this.initializeTypeSyncLock)
        {
          if (this.initializeTypeProcess == null)
          {
            this.initializeTypeProcess = new AsyncQueueProcess((IAsyncMechanism) new BackgroundWorkerAsyncMechanism(BackgroundWorkMode.ThreadHog));
            this.initializeTypeProcess.Complete += new EventHandler(this.InitializeTypeProcess_Complete);
          }
        }
        List<AttachedPropertiesMetadata.AttachedPropertyMetadataBackgroundWorker.AttachedPropertyReferenceAssembly> list = new List<AttachedPropertiesMetadata.AttachedPropertyMetadataBackgroundWorker.AttachedPropertyReferenceAssembly>();
        foreach (TypeChangedInfo typeChangedInfo in assemblyInfosToScan)
        {
          if (this.ShouldScanAssembly(typeChangedInfo.Assembly.Name))
          {
            int hi;
            int lo;
            if (AttachedPropertiesMetadata.AttachedPropertyMetadataBackgroundWorker.FindArrayBounds(this.properties, typeChangedInfo.Assembly.Name, (string) null, (string) null, out hi, out lo))
            {
              this.IsDirty = true;
              this.properties.RemoveRange(lo, hi - lo + 1);
            }
            if (typeChangedInfo.ModificationAction != ModificationType.Removed && typeChangedInfo.Assembly.IsLoaded)
              list.Add(new AttachedPropertiesMetadata.AttachedPropertyMetadataBackgroundWorker.AttachedPropertyReferenceAssembly(AssemblyHelper.GetReflectionAssembly(typeChangedInfo.Assembly), this.runtimeContext, this.referenceContext));
            this.scannedAssemblies.Add(typeChangedInfo.Assembly.Name);
          }
        }
        lock (this.initializeTypeSyncLock)
        {
          lock (this.typeDictionaryLock)
            this.typeDictionary = new Dictionary<AttachedPropertiesMetadata.AttachedPropertyMetadataBackgroundWorker.AttachedPropertyReferenceAssembly, Type[]>();
          if (this.initializeTypeProcess == null)
            return;
          this.IsActive = true;
          this.initializeTypeProcess.Add((AsyncProcess) new EnumeratingAsyncProcess<AttachedPropertiesMetadata.AttachedPropertyMetadataBackgroundWorker.AttachedPropertyReferenceAssembly>((IAsyncMechanism) null, (IEnumerable<AttachedPropertiesMetadata.AttachedPropertyMetadataBackgroundWorker.AttachedPropertyReferenceAssembly>) list, (Action<AttachedPropertiesMetadata.AttachedPropertyMetadataBackgroundWorker.AttachedPropertyReferenceAssembly>) (assemblyTuple =>
          {
            try
            {
              if (assemblyTuple == null)
                return;
              lock (this.typeDictionaryLock)
                this.typeDictionary.Add(assemblyTuple, assemblyTuple.GetReflectionTypes());
            }
            catch (ReflectionTypeLoadException ex)
            {
            }
            catch (ArgumentException ex)
            {
            }
          })), true);
        }
      }

      private void InitializeTypeProcess_Complete(object sender, EventArgs e)
      {
        Dictionary<AttachedPropertiesMetadata.AttachedPropertyMetadataBackgroundWorker.AttachedPropertyReferenceAssembly, Type[]> dictionary = new Dictionary<AttachedPropertiesMetadata.AttachedPropertyMetadataBackgroundWorker.AttachedPropertyReferenceAssembly, Type[]>();
        lock (this.typeDictionaryLock)
        {
          foreach (KeyValuePair<AttachedPropertiesMetadata.AttachedPropertyMetadataBackgroundWorker.AttachedPropertyReferenceAssembly, Type[]> item_0 in this.typeDictionary)
            dictionary.Add(item_0.Key, item_0.Value);
        }
        lock (this.initializeListSyncLock)
        {
          if (dictionary.Count != 0)
          {
            if (this.initializeListProcess == null)
            {
              this.initializeListProcess = new AsyncQueueProcess((IAsyncMechanism) new BackgroundWorkerAsyncMechanism(BackgroundWorkMode.ThreadHog));
              this.initializeListProcess.Complete += new EventHandler(this.OnInitializeListProcessComplete);
            }
            foreach (KeyValuePair<AttachedPropertiesMetadata.AttachedPropertyMetadataBackgroundWorker.AttachedPropertyReferenceAssembly, Type[]> item_1 in dictionary)
            {
              AttachedPropertiesMetadata.AttachedPropertyMetadataBackgroundWorker.AttachedPropertyReferenceAssembly assemblies = item_1.Key;
              this.initializeListProcess.Add((AsyncProcess) new EnumeratingAsyncProcess<Type>((IAsyncMechanism) null, (IEnumerable<Type>) item_1.Value, (Action<Type>) (type =>
              {
                lock (this.properties)
                {
                  if (this.initializeListProcess == null || !this.initializeListProcess.IsAlive)
                    return;
                  this.UpdateAttachedProperties(type, assemblies);
                }
              })), true);
            }
          }
          else
            this.IsActive = false;
        }
      }

      private void OnInitializeListProcessComplete(object sender, EventArgs e)
      {
        if (this.initializeTypeProcess != null && this.initializeTypeProcess.IsAlive)
          return;
        this.IsActive = false;
        if (this.Complete == null)
          return;
        this.Complete(this, EventArgs.Empty);
      }

      internal void UpdateAttachedProperties(Type reflectionType)
      {
        this.UpdateAttachedProperties(reflectionType, new AttachedPropertiesMetadata.AttachedPropertyMetadataBackgroundWorker.AttachedPropertyReferenceAssembly(reflectionType.Assembly, this.runtimeContext, this.referenceContext));
      }

      private void UpdateAttachedProperties(Type reflectionType, AttachedPropertiesMetadata.AttachedPropertyMetadataBackgroundWorker.AttachedPropertyReferenceAssembly assemblyTuple)
      {
        Type runtimeType = assemblyTuple.RuntimeAssembly.GetType(reflectionType.FullName);
        if (runtimeType == (Type) null || !runtimeType.IsPublic && !this.IsTypeFromProjectAssembly(runtimeType))
          return;
        IEnumerable<AttachedPropertyMetadata> attachedProperties = Enumerable.Select(Enumerable.Where(Enumerable.Select(Enumerable.Where<MethodInfo>((IEnumerable<MethodInfo>) reflectionType.GetMethods(BindingFlags.Static | BindingFlags.Public), (Func<MethodInfo, bool>) (methodInfo => this.IsAttachedPropertyGetterMethod(reflectionType, methodInfo))), methodInfo => new
        {
          methodInfo = methodInfo,
          runtimeMethod = this.GetRuntimeMethodInfo(runtimeType, methodInfo)
        }), param0 => param0.runtimeMethod != (MethodInfo) null), param0 => new AttachedPropertyMetadata(param0.runtimeMethod, param0.methodInfo));
        this.ReplacePropertiesForType(reflectionType, attachedProperties);
      }

      private bool IsTypeFromProjectAssembly(Type type)
      {
        string assemblyName = AttachedPropertiesMetadata.AttachedPropertyMetadataBackgroundWorker.GetAssemblyName(type);
        if (this.projectContext.ProjectAssembly != null)
          return string.Equals(assemblyName, this.projectContext.ProjectAssembly.Name, StringComparison.OrdinalIgnoreCase);
        return false;
      }

      private MethodInfo GetRuntimeMethodInfo(Type runtimeType, MethodInfo referenceMethodInfo)
      {
        if (referenceMethodInfo.DeclaringType == runtimeType)
          return referenceMethodInfo;
        MethodInfo[] methodInfoArray = Enumerable.ToArray<MethodInfo>(Enumerable.OfType<MethodInfo>((IEnumerable) runtimeType.GetMember(referenceMethodInfo.Name, MemberTypes.Method, BindingFlags.Static | BindingFlags.Public)));
        if (methodInfoArray.Length == 0)
          return (MethodInfo) null;
        MethodInfo methodInfo1 = (MethodInfo) null;
        ParameterInfo[] parameters1 = referenceMethodInfo.GetParameters();
        foreach (MethodInfo methodInfo2 in methodInfoArray)
        {
          ParameterInfo[] parameters2 = methodInfo2.GetParameters();
          if (parameters2.Length == parameters1.Length)
          {
            if (methodInfo1 == (MethodInfo) null)
              methodInfo1 = methodInfo2;
            bool flag = true;
            for (int index = 0; index < parameters2.Length; ++index)
            {
              if (!parameters2[index].ParameterType.FullName.Equals(parameters1[index].ParameterType.FullName, StringComparison.Ordinal))
              {
                flag = false;
                break;
              }
            }
            if (flag)
              return methodInfo2;
          }
        }
        return methodInfo1;
      }

      private bool IsAttachedPropertyGetterMethod(Type declaringType, MethodInfo methodInfo)
      {
        if (methodInfo.Name.StartsWith("Get", StringComparison.Ordinal))
        {
          string propertyName = methodInfo.Name.Substring(3);
          ParameterInfo[] parameters = methodInfo.GetParameters();
          if (parameters.Length == 1)
          {
            Type returnType = methodInfo.ReturnType;
            if (this.VerifySetterMethod(declaringType, propertyName, parameters[0].ParameterType, returnType))
              return true;
          }
        }
        return false;
      }

      private bool VerifySetterMethod(Type declaringType, string propertyName, Type targetType, Type valueType)
      {
        if (string.Equals(valueType.FullName, "System.Collections.IList", StringComparison.OrdinalIgnoreCase) || valueType.GetInterface("System.Collections.IList") != (Type) null)
          return true;
        Type[] types = new Type[2]
        {
          targetType,
          valueType
        };
        MethodInfo method = declaringType.GetMethod("Set" + propertyName, BindingFlags.Static | BindingFlags.Public | BindingFlags.ExactBinding, (Binder) null, types, (ParameterModifier[]) null);
        return method != (MethodInfo) null && method.ReturnType.FullName == "System.Void" && method.ReturnType.Assembly.FullName.Contains("mscorlib");
      }

      private bool ShouldScanAssembly(string assemblyName)
      {
        if (AttachedPropertiesMetadata.AttachedPropertyMetadataBackgroundWorker.blacklistedAssemblies.Contains(assemblyName))
          return false;
        if (!this.shouldRescanDuplicateAssemblies)
          return !this.scannedAssemblies.Contains(assemblyName);
        return true;
      }

      internal static bool FindArrayBounds(GapList<AttachedPropertyMetadata> properties, string assemblyName, string namespaceName, string typeName, out int hi, out int lo)
      {
        return OrderedListExtensions.GetHiLoBounds((IList<AttachedPropertyMetadata>) properties, new
        {
          AssemblyName = assemblyName,
          NamespaceName = namespaceName,
          TypeName = typeName
        }, (searchCriteria, property) =>
        {
          int num = string.CompareOrdinal(searchCriteria.AssemblyName, AttachedPropertiesMetadata.AttachedPropertyMetadataBackgroundWorker.GetAssemblyName(property.OwnerType));
          if (num == 0 && searchCriteria.NamespaceName != null)
          {
            num = string.CompareOrdinal(searchCriteria.NamespaceName, property.OwnerType.Namespace ?? string.Empty);
            if (num == 0 && searchCriteria.TypeName != null)
              num = string.CompareOrdinal(searchCriteria.TypeName, property.OwnerType.Name);
          }
          return num;
        }, out hi, out lo);
      }

      private class AttachedPropertyReferenceAssembly
      {
        private Assembly runtimeAssembly;
        private Assembly referenceAssembly;

        public Assembly RuntimeAssembly
        {
          get
          {
            return this.runtimeAssembly;
          }
        }

        public AttachedPropertyReferenceAssembly(Assembly runtimeAssembly, IPlatformRuntimeContext runtimeContext, IPlatformReferenceContext referenceContext)
        {
          this.runtimeAssembly = runtimeAssembly;
          if (runtimeContext == null || !(runtimeContext.ResolveRuntimeAssembly(AssemblyHelper.GetAssemblyName(runtimeAssembly)) != (Assembly) null))
            return;
          this.referenceAssembly = referenceContext != null ? referenceContext.ResolveReferenceAssembly(runtimeAssembly) : (Assembly) null;
        }

        public Type[] GetReflectionTypes()
        {
          return (this.referenceAssembly ?? this.runtimeAssembly).GetTypes();
        }
      }
    }
  }
}
