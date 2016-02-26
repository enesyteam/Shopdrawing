// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SampleData.SampleDataSet
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Markup;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.Text;
using Microsoft.Expression.DesignSurface.UserInterface.DataPane;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.UserInterface;
using Microsoft.Expression.Project;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Xml;

namespace Microsoft.Expression.DesignSurface.SampleData
{
  public sealed class SampleDataSet
  {
    public static readonly PrefixedNamespace BlendNS = new PrefixedNamespace("blend", "http://schemas.microsoft.com/expression/blend/2008");
    public static readonly PrefixedNamespace XsdNS = new PrefixedNamespace("xs", "http://www.w3.org/2001/XMLSchema");
    public static readonly string AssetsFolderSuffix = "_Files";
    public static readonly string DesignTimeBuildType = "DesignTimeOnly";
    public static readonly IPropertyId UriLocalPathProperty = (IPropertyId) PlatformTypes.Uri.GetMember(MemberType.LocalProperty, "LocalPath", MemberAccessTypes.Public);
    private static Dictionary<Guid, WeakReference> instanceDictionary = new Dictionary<Guid, WeakReference>();
    private List<SampleDataChange> changes = new List<SampleDataChange>();
    private Dictionary<string, SampleNonBasicType> types = new Dictionary<string, SampleNonBasicType>();
    private Dictionary<string, SampleDataSet.AssetFileAction> assetFiles = new Dictionary<string, SampleDataSet.AssetFileAction>();
    private DataSetContext dataSetContext;
    private string name;
    private Guid guid;
    private SampleDataFlags flags;
    private bool disableChangeTracking;
    private SampleCompositeType rootType;
    private IXmlNamespace xamlNamespace;
    private string actualClrNamespace;
    private string designTimeClrNamespace;
    private IProjectDocument projectDocument;
    private DocumentContext xamlDocumentContext;
    private ProjectItemAction codeItem;
    private ProjectItemAction xamlItem;
    private ProjectItemAction xsdItem;
    private ProjectContext projectContext;
    private IMSBuildProject msBuildProject;
    private IExpressionInformationService expressionInformationService;

    public string SampleDataFolder { get; private set; }

    public string AssetFilesFolder { get; private set; }

    public string CodeFilePath
    {
      get
      {
        return this.codeItem.ProjectItemPath;
      }
    }

    public string XamlFilePath
    {
      get
      {
        return this.xamlItem.ProjectItemPath;
      }
    }

    public string XsdFilePath
    {
      get
      {
        return this.xsdItem.ProjectItemPath;
      }
    }

    public DataSetType DataSetType
    {
      get
      {
        return this.dataSetContext.DataSetType;
      }
    }

    public DataSetContext Context
    {
      get
      {
        return this.dataSetContext;
      }
    }

    public string Name
    {
      get
      {
        return this.name;
      }
    }

    public Guid Guid
    {
      get
      {
        return this.guid;
      }
    }

    public string ClrNamespace
    {
      get
      {
        return this.actualClrNamespace;
      }
    }

    public IXmlNamespace XamlNamespace
    {
      get
      {
        return this.xamlNamespace;
      }
    }

    public SampleCompositeType RootType
    {
      get
      {
        return this.rootType;
      }
    }

    public ProjectContext ProjectContext
    {
      get
      {
        return this.projectContext;
      }
    }

    public IPlatform Platform
    {
      get
      {
        return this.projectContext.Platform;
      }
    }

    public IEnumerable<SampleNonBasicType> Types
    {
      get
      {
        return (IEnumerable<SampleNonBasicType>) this.types.Values;
      }
    }

    public IProjectDocument ProjectDocument
    {
      get
      {
        if (this.projectDocument == null)
        {
          this.projectDocument = this.ProjectContext.OpenDocument(this.XamlFilePath);
          if (this.projectDocument != null)
            this.projectContext.DocumentClosing += new EventHandler<ProjectDocumentEventArgs>(this.Context_DocumentClosing);
        }
        return this.projectDocument;
      }
    }

    public ReadOnlyCollection<SampleDataChange> Changes
    {
      get
      {
        return new ReadOnlyCollection<SampleDataChange>((IList<SampleDataChange>) this.changes);
      }
    }

    public IList<SampleDataChange> NormalizedChanges
    {
      get
      {
        return this.GetNormalizedChanges(0);
      }
    }

    public SampleDataFlags Flags
    {
      get
      {
        return this.flags;
      }
    }

    public bool IsEnabledAtRuntime
    {
      get
      {
        return !this.IsStateFlag(SampleDataFlags.RuntimeDisabled);
      }
      private set
      {
        this.SetStateFlag(SampleDataFlags.RuntimeDisabled, !value);
      }
    }

    public bool IsValid
    {
      get
      {
        return !this.IsStateFlag(SampleDataFlags.TypesLoadFailed | SampleDataFlags.ValuesLoadFailed);
      }
    }

    public bool IsDirtyDesignTimeTypes
    {
      get
      {
        return this.IsStateFlag(SampleDataFlags.DirtyDesignTimeTypes);
      }
      private set
      {
        this.SetStateFlag(SampleDataFlags.DirtyDesignTimeTypes, value);
      }
    }

    public bool IsDirtyTypes
    {
      get
      {
        return this.IsStateFlag(SampleDataFlags.DirtyTypes);
      }
      private set
      {
        this.SetStateFlag(SampleDataFlags.DirtyTypes, value);
      }
    }

    public bool IsDirtyTypeMetadata
    {
      get
      {
        return this.IsStateFlag(SampleDataFlags.DirtyTypeMetadata);
      }
      private set
      {
        this.SetStateFlag(SampleDataFlags.DirtyTypeMetadata, value);
      }
    }

    public bool IsDirtyValues
    {
      get
      {
        return this.IsStateFlag(SampleDataFlags.DirtyValues);
      }
      private set
      {
        this.SetStateFlag(SampleDataFlags.DirtyValues, value);
      }
    }

    public bool IsSaved
    {
      get
      {
        if (!this.IsDirtyTypes && !this.IsDirtyValues)
          return !this.IsDirtyTypeMetadata;
        return false;
      }
      private set
      {
        this.IsDirtyTypes = !value;
        this.IsDirtyTypeMetadata = !value;
        this.IsDirtyValues = !value;
      }
    }

    public bool IsSaveFailed
    {
      get
      {
        return this.IsStateFlag(SampleDataFlags.TypesSaveFailed | SampleDataFlags.ValuesSaveFailed | SampleDataFlags.UpdateProjectItemsFailed | SampleDataFlags.CodeGenFailed);
      }
      private set
      {
        this.SetStateFlag(SampleDataFlags.TypesSaveFailed | SampleDataFlags.ValuesSaveFailed | SampleDataFlags.UpdateProjectItemsFailed | SampleDataFlags.CodeGenFailed, value);
      }
    }

    public bool IsOnline
    {
      get
      {
        return !this.IsStateFlag(SampleDataFlags.Offline);
      }
      private set
      {
        this.SetStateFlag(SampleDataFlags.Offline, !value);
      }
    }

    private bool IsSaveInProgress
    {
      get
      {
        return this.IsStateFlag(SampleDataFlags.SaveInProgress);
      }
      set
      {
        this.SetStateFlag(SampleDataFlags.SaveInProgress, value);
      }
    }

    private string CodeDomLanguage
    {
      get
      {
        switch (this.msBuildProject.CodeLanguage)
        {
          case "C#":
            return "CSharp";
          case "VB":
            return "VB";
          default:
            return (string) null;
        }
      }
    }

    private string CodeFileExtension
    {
      get
      {
        switch (this.msBuildProject.CodeLanguage)
        {
          case "C#":
            return ".cs";
          case "VB":
            return ".vb";
          default:
            return (string) null;
        }
      }
    }

    public DocumentCompositeNode RootNode
    {
      get
      {
        return this.ValidRootNodeFromXamlDocument ?? this.CreateEmptyRootNode();
      }
      set
      {
        DocumentCompositeNode rootNode = value;
        if (rootNode == null || rootNode.Type != this.rootType)
          rootNode = this.CreateEmptyRootNode();
        this.IsDirtyValues = true;
        this.Save(rootNode);
      }
    }

    public string FallbackImageFolder
    {
      get
      {
        if (this.IsSampleImagesAvailable)
          return this.AssetFilesFolder;
        return Path.Combine(TemplateManager.TranslatedFolder("SampleDataResources"), "Images");
      }
    }

    private bool IsSampleImagesAvailable
    {
      get
      {
        if (Microsoft.Expression.Framework.Documents.PathHelper.DirectoryExists(this.AssetFilesFolder))
        {
          DirectoryInfo directoryInfo = new DirectoryInfo(this.AssetFilesFolder);
          foreach (string str in (IEnumerable<string>) ((IPlatformTypes) this.ProjectContext.PlatformMetadata).ImageFileExtensions)
          {
            FileInfo[] files = directoryInfo.GetFiles("*" + str);
            int index = 0;
            if (index < files.Length)
            {
              FileInfo fileInfo = files[index];
              return true;
            }
          }
        }
        return false;
      }
    }

    public DocumentCompositeNode ValidRootNodeFromXamlDocument
    {
      get
      {
        if (this.projectDocument == null && !Microsoft.Expression.Framework.Documents.PathHelper.FileExists(this.XamlFilePath))
          return (DocumentCompositeNode) null;
        if (this.ProjectDocument == null || this.ProjectDocument.DocumentRoot == null)
          return (DocumentCompositeNode) null;
        if (!this.ProjectDocument.DocumentRoot.IsEditable)
          return (DocumentCompositeNode) null;
        DocumentCompositeNode documentCompositeNode = this.ProjectDocument.DocumentRoot.RootNode as DocumentCompositeNode;
        if (documentCompositeNode == null || documentCompositeNode.Type != this.RootType)
          return (DocumentCompositeNode) null;
        return documentCompositeNode;
      }
    }

    public event SampleDataChangedEventHandler SampleTypesChanged;

    public event SampleDataChangedEventHandler SampleTypesChanging;

    private SampleDataSet()
    {
    }

    public SampleDataSet(DataSetContext dataSetContext, string name, bool enableAtRuntime, ProjectContext actualProjectContext, IMSBuildProject msBuildProject, IExpressionInformationService expressionInformationService)
    {
      this.dataSetContext = dataSetContext;
      this.disableChangeTracking = true;
      this.name = name;
      this.guid = Guid.NewGuid();
      SampleDataSet.instanceDictionary[this.guid] = new WeakReference((object) this);
      this.projectContext = actualProjectContext;
      this.msBuildProject = msBuildProject;
      this.expressionInformationService = expressionInformationService;
      this.rootType = this.CreateCompositeTypeInternal(this.name);
      this.SetDirty();
      if (!enableAtRuntime)
        this.SetStateFlag(SampleDataFlags.RuntimeDisabled, true);
      this.SampleDataFolder = Path.Combine(Path.Combine(Path.GetDirectoryName(this.projectContext.ProjectPath), this.Context.DataRootFolder), this.Name);
      string str = Path.Combine(this.SampleDataFolder, this.Name);
      this.xsdItem = new ProjectItemAction(str + ".xsd", false, (string) null);
      this.xamlItem = new ProjectItemAction(str + ".xaml", true, (string) null);
      this.codeItem = new ProjectItemAction(this.XamlFilePath + this.CodeFileExtension, true, (string) null);
      this.AssetFilesFolder = str + SampleDataSet.AssetsFolderSuffix;
      this.actualClrNamespace = this.Context.GetClrNamespacePrefix(this.ProjectContext.RootNamespace) + this.name;
      this.designTimeClrNamespace = string.Empty;
      this.xamlNamespace = (IXmlNamespace) XmlNamespace.ToNamespace("clr-namespace:" + this.actualClrNamespace, XmlNamespaceCanonicalization.None);
      this.xamlDocumentContext = new DocumentContext((IProjectContext) this.ProjectContext, (IDocumentLocator) null);
      this.RegisterForProjectItemChanges(true);
      SampleDataInstanceBuilder.EnsureRegistered(this.Platform);
      this.disableChangeTracking = false;
    }

    public static SampleDataSet SampleDataSetFromType(Type type)
    {
      string g = SampleDataDesignTimeTypeGenerator.SampleDataSetGuidFromType(type);
      if (string.IsNullOrEmpty(g))
        return (SampleDataSet) null;
      WeakReference weakReference = (WeakReference) null;
      Guid key = new Guid(g);
      if (!SampleDataSet.instanceDictionary.TryGetValue(key, out weakReference) || !weakReference.IsAlive)
        return (SampleDataSet) null;
      return (SampleDataSet) weakReference.Target;
    }

    public static SampleNonBasicType SampleDataTypeFromType(Type type)
    {
      if (type == (Type) null)
        return (SampleNonBasicType) null;
      SampleNonBasicType sampleNonBasicType = (SampleNonBasicType) null;
      SampleDataSet sampleDataSet = SampleDataSet.SampleDataSetFromType(type);
      if (sampleDataSet != null)
        sampleNonBasicType = sampleDataSet.GetSampleType(type.Name);
      return sampleNonBasicType;
    }

    public void Close()
    {
      if (this.msBuildProject == null)
        return;
      this.IsOnline = false;
      this.RegisterForProjectItemChanges(false);
      this.CloseProjectDocument();
      this.msBuildProject = (IMSBuildProject) null;
      this.xamlDocumentContext = (DocumentContext) null;
      this.types.Clear();
      this.assetFiles.Clear();
      this.changes.Clear();
      SampleDataSet.instanceDictionary.Remove(this.guid);
    }

    private void Context_DocumentClosing(object sender, ProjectDocumentEventArgs e)
    {
      if (e.Document != this.projectDocument)
        return;
      this.CloseProjectDocument();
    }

    public SampleNonBasicType GetSampleType(string name)
    {
      SampleNonBasicType sampleNonBasicType = (SampleNonBasicType) null;
      this.types.TryGetValue(name, out sampleNonBasicType);
      return sampleNonBasicType;
    }

    private void CloseProjectDocument()
    {
      if (this.projectDocument == null)
        return;
      this.ProjectContext.DocumentClosing -= new EventHandler<ProjectDocumentEventArgs>(this.Context_DocumentClosing);
      this.projectDocument = (IProjectDocument) null;
    }

    public string GetUniqueTypeName(string name)
    {
      return this.GetUniqueTypeName(name, (string) null);
    }

    public string GetUniqueTypeName(string name, string nameToIgnore)
    {
      return this.GetUniqueName(name, (IEnumerable<string>) this.types.Keys, nameToIgnore);
    }

    public string GetUniqueName(string name, IEnumerable<string> existingNames, string nameToIgnore)
    {
      return SampleDataNameHelper.GetUniqueName(name, this.msBuildProject, existingNames, nameToIgnore, false);
    }

    public string GetSafeName(string name)
    {
      return SampleDataNameHelper.GetSafeName(name, this.msBuildProject, false);
    }

    public bool IsSafeIdentifier(string name)
    {
      return this.msBuildProject.IsSafeIdentifier(name);
    }

    public SampleCollectionType CreateCollectionType(SampleNonBasicType itemType)
    {
      return this.CreateCollectionType(itemType.Name + "Collection", (SampleType) itemType);
    }

    public SampleCollectionType CreateCollectionType(string name, SampleType itemType)
    {
      return this.CreateCollectionTypeInternal(name, itemType);
    }

    internal bool RestoreDeletedType(SampleNonBasicType sampleType)
    {
      if (this.GetSampleType(sampleType.Name) != null)
        return false;
      this.types[sampleType.Name] = sampleType;
      return true;
    }

    public SampleCompositeType CreateCompositeType(string name)
    {
      return this.CreateCompositeTypeInternal(name);
    }

    private SampleCompositeType CreateCompositeTypeInternal(string name)
    {
      if (this.GetSampleType(name) != null)
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, ExceptionStringTable.NameIsAlreadyInUse, new object[1]
        {
          (object) name
        }));
      SampleCompositeType sampleCompositeType = new SampleCompositeType(name, this);
      this.OnTypeCreated((SampleNonBasicType) sampleCompositeType);
      return sampleCompositeType;
    }

    public bool DeleteType(SampleNonBasicType typeToRemove)
    {
      if (this.IsTypeInUse(typeToRemove, (List<SampleNonBasicType>) null))
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, ExceptionStringTable.CannotDeleteTypesBecauseOfDependencies, new object[1]
        {
          (object) typeToRemove.Name
        }));
      if (!this.types.Remove(typeToRemove.Name))
        return false;
      this.OnTypeDeleted(typeToRemove);
      return true;
    }

    private void RemoveUnusedTypes()
    {
      List<SampleNonBasicType> unusedTypes = this.GetUnusedTypes();
      for (int index = 0; index < unusedTypes.Count; ++index)
        this.types.Remove(unusedTypes[index].Name);
    }

    private List<SampleNonBasicType> GetUnusedTypes()
    {
      List<SampleNonBasicType> list = new List<SampleNonBasicType>((IEnumerable<SampleNonBasicType>) this.types.Values);
      list.Remove((SampleNonBasicType) this.rootType);
      List<SampleNonBasicType> unusedTypes = new List<SampleNonBasicType>();
      bool flag;
      do
      {
        flag = false;
        for (int index = list.Count - 1; index >= 0; --index)
        {
          SampleNonBasicType sampleType = list[index];
          if (!this.IsTypeInUse(sampleType, unusedTypes))
          {
            unusedTypes.Add(sampleType);
            list.RemoveAt(index);
            flag = true;
          }
        }
      }
      while (flag);
      return unusedTypes;
    }

    private bool IsTypeInUse(SampleNonBasicType sampleType, List<SampleNonBasicType> unusedTypes)
    {
      if (sampleType == this.rootType)
        return true;
      foreach (SampleNonBasicType sampleNonBasicType in this.types.Values)
      {
        if (sampleNonBasicType != sampleType && (unusedTypes == null || !unusedTypes.Contains(sampleNonBasicType)))
        {
          if (sampleNonBasicType.IsCollection)
          {
            if (((SampleCollectionType) sampleNonBasicType).ItemSampleType == sampleType)
              return true;
          }
          else
          {
            foreach (SampleProperty sampleProperty in (IEnumerable<SampleProperty>) ((SampleCompositeType) sampleNonBasicType).SampleProperties)
            {
              if (sampleProperty.PropertySampleType == sampleType)
                return true;
            }
          }
        }
      }
      return false;
    }

    internal void ThrowIfRecursiveRootType(SampleType type)
    {
      if (type == this.rootType)
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, ExceptionStringTable.RecursiveSampleDataRootTypeNotSupported, new object[1]
        {
          (object) this.rootType.Name
        }));
    }

    internal void OnTypeCreated(SampleNonBasicType sampleType)
    {
      this.types[sampleType.Name] = sampleType;
      this.SetDirty();
      if (this.disableChangeTracking)
        return;
      this.changes.Add((SampleDataChange) new SampleTypeCreated(sampleType));
    }

    internal void OnTypeRenamed(SampleNonBasicType sampleType, string oldName)
    {
      this.types.Remove(oldName);
      this.types[sampleType.Name] = sampleType;
      this.SetDirty();
      if (this.disableChangeTracking)
        return;
      this.changes.Add((SampleDataChange) new SampleTypeRenamed(sampleType, oldName));
    }

    internal void OnCollectionItemTypeChanged(SampleCollectionType sampleType, SampleType oldItemType)
    {
      this.SetDirty();
      if (this.disableChangeTracking)
        return;
      this.changes.Add((SampleDataChange) new SampleCollectionItemTypeChanged(sampleType, oldItemType));
    }

    internal void OnTypeDeleted(SampleNonBasicType sampleType)
    {
      this.types.Remove(sampleType.Name);
      this.SetDirty();
      if (this.disableChangeTracking)
        return;
      this.changes.Add((SampleDataChange) new SampleTypeDeleted(sampleType));
    }

    internal void OnPropertyAdded(SampleProperty sampleProperty)
    {
      this.SetDirty();
      if (this.disableChangeTracking)
        return;
      this.changes.Add((SampleDataChange) new SamplePropertyCreated(sampleProperty));
    }

    internal void OnPropertyRenamed(SampleProperty sampleProperty, string oldName)
    {
      this.SetDirty();
      if (this.disableChangeTracking)
        return;
      this.changes.Add((SampleDataChange) new SamplePropertyRenamed(sampleProperty, oldName));
    }

    internal void OnPropertyTypeOrFormatChanged(SampleProperty sampleProperty, SampleType oldType, string oldFormat, string oldFormatParameters)
    {
      if (sampleProperty.PropertySampleType != oldType)
        this.SetDirty();
      else
        this.IsDirtyTypeMetadata = true;
      if (this.disableChangeTracking)
        return;
      this.changes.Add((SampleDataChange) new SamplePropertyTypeOrFormatChanged(sampleProperty, oldType, oldFormat, oldFormatParameters));
    }

    internal void OnPropertyDeleted(SampleProperty sampleProperty)
    {
      this.SetDirty();
      if (this.disableChangeTracking)
        return;
      this.changes.Add((SampleDataChange) new SamplePropertyDeleted(sampleProperty));
    }

    private SampleCollectionType CreateCollectionTypeInternal(string name, SampleType itemType)
    {
      if (this.GetSampleType(name) != null)
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, ExceptionStringTable.NameIsAlreadyInUse, new object[1]
        {
          (object) name
        }));
      SampleCollectionType sampleCollectionType = new SampleCollectionType(name, itemType, this);
      this.OnTypeCreated((SampleNonBasicType) sampleCollectionType);
      return sampleCollectionType;
    }

    public void ClearChanges()
    {
      this.changes.Clear();
    }

    public IList<SampleDataChange> GetNormalizedChanges(int changesToSkip)
    {
      List<SampleDataChange> list = new List<SampleDataChange>(Enumerable.Skip<SampleDataChange>((IEnumerable<SampleDataChange>) this.changes, changesToSkip));
      List<SampleNonBasicType> unusedTypes = this.GetUnusedTypes();
      IEnumerable<SampleDataChange> source = Enumerable.Where<SampleDataChange>((IEnumerable<SampleDataChange>) list, (Func<SampleDataChange, bool>) (change =>
      {
        if (!(change is SampleTypeCreated))
          return change is SampleTypeDeleted;
        return true;
      }));
      unusedTypes.AddRange(Enumerable.Select<SampleDataChange, SampleNonBasicType>(source, (Func<SampleDataChange, SampleNonBasicType>) (change => ((SampleTypeChange) change).SampleType)));
      for (int index1 = list.Count - 1; index1 > 0; --index1)
      {
        SampleDataChange newerChange = list[index1];
        SamplePropertyChange samplePropertyChange = newerChange as SamplePropertyChange;
        if (samplePropertyChange != null && unusedTypes.Contains((SampleNonBasicType) samplePropertyChange.SampleProperty.DeclaringSampleType))
        {
          list.RemoveAt(index1);
        }
        else
        {
          for (int index2 = index1 - 1; index2 >= 0; --index2)
          {
            SampleDataChange sampleDataChange = list[index2];
            SampleDataChange mergedChange = (SampleDataChange) null;
            SampleDataChangeMergeResult changeMergeResult = sampleDataChange.MergeWith(newerChange, out mergedChange);
            if (changeMergeResult != SampleDataChangeMergeResult.CouldNotMerge)
            {
              list.RemoveAt(index1);
              if (changeMergeResult == SampleDataChangeMergeResult.MergedIntoOneUnit)
              {
                list[index2] = mergedChange;
                break;
              }
              if (changeMergeResult == SampleDataChangeMergeResult.MergedIntoNothing)
              {
                list.RemoveAt(index2);
                index1 = list.Count;
                break;
              }
              break;
            }
          }
        }
      }
      return (IList<SampleDataChange>) list;
    }

    public void SetDirty()
    {
      this.IsDirtyTypes = true;
      this.IsDirtyDesignTimeTypes = true;
      this.IsDirtyValues = true;
    }

    private void SetStateFlag(SampleDataFlags flag, bool on)
    {
      if (on)
        this.flags |= flag;
      else
        this.flags &= ~flag;
    }

    private bool IsStateFlag(SampleDataFlags flag)
    {
      return this.IsStateFlag(flag, false);
    }

    private bool IsStateFlag(SampleDataFlags flag, bool exact)
    {
      SampleDataFlags sampleDataFlags = this.flags & flag;
      return exact && sampleDataFlags == flag || !exact && sampleDataFlags != SampleDataFlags.None;
    }

    public bool Load()
    {
      PerformanceUtility.StartPerformanceSequence(PerformanceEvent.SampleDataSetLoad);
      bool flag = this.LoadInternal();
      PerformanceUtility.EndPerformanceSequence(PerformanceEvent.SampleDataSetLoad);
      return flag;
    }

    public IDisposable DisableChangeTracking()
    {
      return (IDisposable) new SampleDataSet.DisableChangeRecordingToken(this);
    }

    private bool LoadInternal()
    {
      using (this.DisableChangeTracking())
      {
        string name = this.RootType.Name;
        bool flag = this.LoadTypes();
        if (flag)
        {
          this.IsSaved = true;
          this.codeItem.OnActionCompleted();
          this.xsdItem.OnActionCompleted();
          this.xamlItem.OnActionCompleted();
          this.InitAssetFiles();
          this.IsOnline = true;
        }
        else
          this.RootType.Rename(name);
        this.RebuildDesignTimeTypesIfNeeded();
        this.NotifySampleTypesChanged();
        return flag;
      }
    }

    private void InitAssetFiles()
    {
      this.assetFiles.Clear();
      if (!Microsoft.Expression.Framework.Documents.PathHelper.DirectoryExists(this.AssetFilesFolder))
        return;
      foreach (string str in Directory.GetFiles(this.AssetFilesFolder))
        this.assetFiles[Path.GetFileName(str).ToUpperInvariant()] = new SampleDataSet.AssetFileAction(str, ProjectItemOperation.None, this.IsEnabledAtRuntime, (string) null);
    }

    private bool LoadTypes()
    {
      bool flag = false;
      try
      {
        SampleDataXsdParser.Parse(this, this.XsdFilePath);
        flag = true;
      }
      catch
      {
      }
      this.SetStateFlag(SampleDataFlags.TypesLoadFailed, !flag);
      return flag;
    }

    private void RegisterForProjectItemChanges(bool register)
    {
      if (this.msBuildProject.Project == null)
        return;
      if (register)
      {
        this.msBuildProject.Project.ItemChanged += new EventHandler<ProjectItemEventArgs>(this.ProjectItem_Changed);
        this.msBuildProject.Project.ItemAdded += new EventHandler<ProjectItemEventArgs>(this.ProjectItem_Added);
        this.msBuildProject.Project.ItemRemoved += new EventHandler<ProjectItemEventArgs>(this.ProjectItem_Removed);
        this.msBuildProject.Project.ItemRenamed += new EventHandler<ProjectItemRenamedEventArgs>(this.ProjectItem_Renamed);
      }
      else
      {
        this.msBuildProject.Project.ItemChanged -= new EventHandler<ProjectItemEventArgs>(this.ProjectItem_Changed);
        this.msBuildProject.Project.ItemAdded -= new EventHandler<ProjectItemEventArgs>(this.ProjectItem_Added);
        this.msBuildProject.Project.ItemRemoved -= new EventHandler<ProjectItemEventArgs>(this.ProjectItem_Removed);
        this.msBuildProject.Project.ItemRenamed -= new EventHandler<ProjectItemRenamedEventArgs>(this.ProjectItem_Renamed);
      }
    }

    private void ProjectItem_Changed(object sender, ProjectItemEventArgs args)
    {
      if (!this.ShouldHandleProjectItemEvent(args.ProjectItem) || !this.XsdFilePath.Equals(args.ProjectItem.DocumentReference.Path, StringComparison.OrdinalIgnoreCase))
        return;
      this.Reload();
    }

    private void ProjectItem_Added(object sender, ProjectItemEventArgs args)
    {
      if (!this.ShouldHandleProjectItemEvent(args.ProjectItem))
        return;
      string path = args.ProjectItem.Document.DocumentReference.Path;
      if (!this.IsAsetFile(path))
        return;
      this.assetFiles[Path.GetFileName(path).ToUpperInvariant()] = new SampleDataSet.AssetFileAction(path, ProjectItemOperation.None, this.IsEnabledAtRuntime, (string) null);
    }

    private void ProjectItem_Removed(object sender, ProjectItemEventArgs args)
    {
      if (!this.ShouldHandleProjectItemEvent(args.ProjectItem))
        return;
      string path = args.ProjectItem.Document.DocumentReference.Path;
      if (!this.IsAsetFile(path))
        return;
      this.assetFiles.Remove(Path.GetFileName(path).ToUpperInvariant());
    }

    private void ProjectItem_Renamed(object sender, ProjectItemRenamedEventArgs args)
    {
      if (!this.ShouldHandleProjectItemEvent(args.ProjectItem))
        return;
      string path1 = args.OldName.Path;
      if (this.IsAsetFile(path1))
        this.assetFiles.Remove(Path.GetFileName(path1).ToUpperInvariant());
      string path2 = args.ProjectItem.Document.DocumentReference.Path;
      if (!this.IsAsetFile(path2))
        return;
      this.assetFiles[Path.GetFileName(path2).ToUpperInvariant()] = new SampleDataSet.AssetFileAction(path2, ProjectItemOperation.None, this.IsEnabledAtRuntime, (string) null);
    }

    private bool ShouldHandleProjectItemEvent(IProjectItem projectItem)
    {
      return this.IsOnline && !this.IsSaveInProgress && (projectItem != null && projectItem.Document != null) && !(projectItem.Document.DocumentReference == (DocumentReference) null);
    }

    private bool IsAsetFile(string path)
    {
      return Microsoft.Expression.Framework.Documents.PathHelper.ArePathsEquivalent(Path.GetDirectoryName(path), this.AssetFilesFolder);
    }

    private void Reload()
    {
      try
      {
        using (new SampleDataSet.OfflineToken(this))
        {
          using (this.DisableChangeTracking())
          {
            using (TemporaryCursor.SetWaitCursor())
              this.ReloadInternal();
          }
        }
      }
      catch
      {
      }
    }

    private void ReloadInternal()
    {
      IProjectItem projectItem = this.msBuildProject.Project.FindItem(DocumentReference.Create(this.XamlFilePath));
      if (projectItem != null)
      {
        if (projectItem.IsDirty)
          projectItem.SaveDocumentFile();
        if (projectItem.IsOpen)
          projectItem.CloseDocument();
      }
      this.types.Clear();
      this.rootType = this.CreateCompositeTypeInternal(this.name);
      this.Load();
      this.FireTypesChangedEvent();
    }

    public bool ImportFromXml(string xmlFileName, bool reImport)
    {
      SampleDataSet.SampleDataSetState state = reImport ? this.CaptureState() : (SampleDataSet.SampleDataSetState) null;
      try
      {
        using (new SampleDataSet.OfflineToken(this))
        {
          using (this.DisableChangeTracking())
          {
            using (TemporaryCursor.SetWaitCursor())
              return this.ImportFromXmlInternal(xmlFileName, reImport, ref state);
          }
        }
      }
      finally
      {
        if (state != null)
          this.RestoreState(state);
      }
    }

    private bool ImportFromXmlInternal(string xmlFileName, bool reImport, ref SampleDataSet.SampleDataSetState state)
    {
      string expectedRootTypeName = (string) null;
      if (reImport)
      {
        expectedRootTypeName = this.rootType.Name;
        this.types.Clear();
        this.rootType = this.CreateCompositeTypeInternal(this.name);
      }
      XmlToSampleDataAdapter sampleDataAdapter = new XmlToSampleDataAdapter(this, xmlFileName, expectedRootTypeName);
      XmlToClrImporter.Import(xmlFileName, (IXmlToClrAdapter) sampleDataAdapter);
      state = (SampleDataSet.SampleDataSetState) null;
      if (reImport && this.IsSaved)
        this.FireTypesChangedEvent();
      return this.IsSaved;
    }

    private SampleDataSet.SampleDataSetState CaptureState()
    {
      return new SampleDataSet.SampleDataSetState()
      {
        RootType = this.RootType,
        Types = new List<SampleNonBasicType>(this.Types),
        Changes = this.changes,
        Flags = this.flags
      };
    }

    private void RestoreState(SampleDataSet.SampleDataSetState state)
    {
      this.types.Clear();
      this.rootType = state.RootType;
      foreach (SampleNonBasicType sampleNonBasicType in state.Types)
        this.types[sampleNonBasicType.Name] = sampleNonBasicType;
      this.changes = state.Changes;
      this.flags = state.Flags;
    }

    private void FireTypesChangedEvent()
    {
      this.ProjectContext.OnTypesChanged(new TypesChangedEventArgs((ICollection<TypeChangedInfo>) new List<TypeChangedInfo>()
      {
        new TypeChangedInfo(this.RootType.RuntimeAssembly, ModificationType.Modified)
      }));
    }

    public bool Save()
    {
      bool flag = true;
      if (!this.IsSaved)
        flag = this.Save(this.RootNode);
      return flag;
    }

    private bool Save(DocumentCompositeNode rootNode)
    {
      PerformanceUtility.StartPerformanceSequence(PerformanceEvent.SampleDataSetSave);
      this.IsSaveInProgress = true;
      try
      {
        using (ProjectPathHelper.TemporaryDirectory temporaryDirectory = new ProjectPathHelper.TemporaryDirectory())
          return this.SaveInternal(temporaryDirectory.Path, rootNode);
      }
      finally
      {
        this.IsSaveInProgress = false;
        PerformanceUtility.EndPerformanceSequence(PerformanceEvent.SampleDataSetSave);
      }
    }

    private bool SaveInternal(string tempFolder, DocumentCompositeNode rootNode)
    {
      this.RebuildDesignTimeTypesIfNeeded();
      if (!Microsoft.Expression.Framework.Documents.PathHelper.DirectoryExists(this.SampleDataFolder))
        Directory.CreateDirectory(this.SampleDataFolder);
      if (this.assetFiles.Count > 0 && !Microsoft.Expression.Framework.Documents.PathHelper.DirectoryExists(this.AssetFilesFolder))
        Directory.CreateDirectory(this.AssetFilesFolder);
      this.SetStateFlag(SampleDataFlags.TypesSaveFailed | SampleDataFlags.ValuesSaveFailed | SampleDataFlags.UpdateProjectItemsFailed | SampleDataFlags.CodeGenFailed, false);
      if (this.IsDirtyTypes || this.IsDirtyTypeMetadata)
      {
        string str = Path.Combine(tempFolder, Path.GetFileName(this.xsdItem.ProjectItemPath));
        if (!this.SaveTypes(str))
          return false;
        this.xsdItem.PrepareToUpdate(str, false);
      }
      if (this.IsDirtyTypes)
      {
        string str = Path.Combine(tempFolder, Path.GetFileName(this.codeItem.ProjectItemPath));
        if (!this.GenerateCode(str))
          return false;
        this.codeItem.PrepareToUpdate(str, true);
      }
      if (this.IsDirtyTypes || this.IsDirtyValues)
      {
        string str = Path.Combine(tempFolder, Path.GetFileName(this.xamlItem.ProjectItemPath));
        if (!this.SaveValues(rootNode, str))
          return false;
        this.xamlItem.PrepareToUpdate(str, this.IsEnabledAtRuntime);
      }
      this.IsSaved = this.UpdateProject((IEnumerable<ProjectItemAction>) new List<ProjectItemAction>(Enumerable.Select<SampleDataSet.AssetFileAction, ProjectItemAction>((IEnumerable<SampleDataSet.AssetFileAction>) this.assetFiles.Values, (Func<SampleDataSet.AssetFileAction, ProjectItemAction>) (a => (ProjectItemAction) a)))
      {
        this.xsdItem,
        this.codeItem,
        this.xamlItem
      }, false);
      if (this.IsSaved)
      {
        List<string> list = new List<string>();
        foreach (KeyValuePair<string, SampleDataSet.AssetFileAction> keyValuePair in this.assetFiles)
        {
          if (keyValuePair.Value.Operation == ProjectItemOperation.Delete)
            list.Add(keyValuePair.Key);
        }
        foreach (string key in list)
          this.assetFiles.Remove(key);
      }
      return this.IsSaved;
    }

    private bool SaveTypes(string xsdFile)
    {
      bool flag = false;
      try
      {
        SampleDataXsdSerializer.Serialize(this, xsdFile);
        flag = true;
      }
      catch (Exception ex)
      {
      }
      this.SetStateFlag(SampleDataFlags.TypesSaveFailed, !flag);
      return flag;
    }

    private bool UpdateProject(IEnumerable<ProjectItemAction> actions, bool ignoreFailure)
    {
      bool flag = this.msBuildProject.UpdateProject(actions);
      if (flag || !ignoreFailure)
        this.SetStateFlag(SampleDataFlags.UpdateProjectItemsFailed, !flag);
      return flag;
    }

    private bool GenerateCode(string codeFile)
    {
      bool flag = false;
      try
      {
        SampleCodeGenerator.GenerateSourceCodeFile(this, codeFile, this.CodeDomLanguage);
        flag = true;
      }
      catch (Exception ex)
      {
      }
      this.SetStateFlag(SampleDataFlags.CodeGenFailed, !flag);
      return flag;
    }

    private bool SaveValues(DocumentCompositeNode rootNode, string xamlFile)
    {
      bool flag = false;
      try
      {
        string contents = this.SerializeValues(rootNode);
        if (contents != null)
        {
          File.WriteAllText(xamlFile, contents, Encoding.UTF8);
          this.xamlItem.PrepareToUpdate(xamlFile, this.IsEnabledAtRuntime);
          flag = true;
        }
      }
      catch (Exception ex)
      {
      }
      this.SetStateFlag(SampleDataFlags.ValuesSaveFailed, !flag);
      return flag;
    }

    private string SerializeValues(DocumentCompositeNode rootNode)
    {
      string str1 = (string) null;
      string str2 = "<!--" + StringTable.SampleDataDoNotEditComment.TrimEnd(' ') + "-->";
      if (this.IsEndOfLineNeededAtTheEndOfHeader(rootNode))
        str2 += "\r\n";
      try
      {
        using (MemoryStream memoryStream = new MemoryStream())
        {
          using (StreamWriter streamWriter = new StreamWriter((Stream) memoryStream, Encoding.UTF8))
          {
            streamWriter.Write(str2);
            using (XamlDocument xamlDocument = new XamlDocument((IDocumentContext) this.xamlDocumentContext, (ITypeId) this.rootType, (ITextBuffer) new SimpleTextBuffer(), DocumentEncodingHelper.DefaultEncoding, (IXamlSerializerFilter) new DefaultXamlSerializerFilter()))
            {
              SampleDataSet.WorkaroundSerializerFilter serializerFilter = new SampleDataSet.WorkaroundSerializerFilter(this.projectContext.IsCapabilitySet(PlatformCapability.RequiresDefaultXmlns));
              new XamlSerializer((IDocumentRoot) xamlDocument, (IXamlSerializerFilter) serializerFilter).Serialize((DocumentNode) rootNode, (TextWriter) streamWriter);
              streamWriter.Flush();
              if (serializerFilter.DefaultNamespaceRequired && !serializerFilter.SeenDefaultNamespace)
              {
                memoryStream.Seek(0L, SeekOrigin.Begin);
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load((Stream) memoryStream);
                ((XmlElement) Enumerable.FirstOrDefault<XmlNode>(Enumerable.OfType<XmlNode>((IEnumerable) xmlDocument.ChildNodes), (Func<XmlNode, bool>) (node => node is XmlElement))).SetAttribute("xmlns", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
                memoryStream.Seek(0L, SeekOrigin.Begin);
                xmlDocument.Save((Stream) memoryStream);
                memoryStream.SetLength(memoryStream.Position);
              }
              memoryStream.Seek(0L, SeekOrigin.Begin);
              using (StreamReader streamReader = new StreamReader((Stream) memoryStream, Encoding.UTF8))
                str1 = streamReader.ReadToEnd();
            }
          }
        }
      }
      catch (Exception ex)
      {
      }
      return str1;
    }

    private bool IsEndOfLineNeededAtTheEndOfHeader(DocumentCompositeNode rootNode)
    {
      bool flag = true;
      if (rootNode.SourceContext != null)
      {
        ITextRange textRange = rootNode.SourceContext.TextRange;
        if (textRange != null && textRange.Offset > 0)
        {
          string text = rootNode.SourceContext.TextBuffer.GetText(0, textRange.Offset);
          for (int index = text.Length - 1; index >= 0; --index)
          {
            char c = text[index];
            if (char.IsWhiteSpace(c))
            {
              if ((int) c == 13)
              {
                flag = false;
                break;
              }
            }
            else
              break;
          }
        }
      }
      return flag;
    }

    public bool RebuildDesignTimeTypesIfNeeded()
    {
      if (!this.IsDirtyDesignTimeTypes && this.rootType.DesignTimeType != (Type) null)
        return false;
      SampleDataDesignTimeTypeGenerator.RebuildDesignTimeTypes(this);
      if (this.RootType.DesignTimeType != (Type) null)
      {
        this.designTimeClrNamespace = this.RootType.DesignTimeType.Namespace;
        this.IsDirtyDesignTimeTypes = false;
      }
      return true;
    }

    public void EnsureSampleImages()
    {
      if (this.IsSampleImagesAvailable)
        return;
      DirectoryInfo directoryInfo = new DirectoryInfo(Path.Combine(TemplateManager.TranslatedFolder("SampleDataResources"), "Images"));
      foreach (string str in (IEnumerable<string>) ((IPlatformTypes) this.ProjectContext.PlatformMetadata).ImageFileExtensions)
      {
        foreach (FileSystemInfo fileSystemInfo in directoryInfo.GetFiles("*" + str))
          this.AddOrUpdateAssetFile(fileSystemInfo.FullName);
      }
    }

    public DocumentCompositeNode CreateEmptyRootNode()
    {
      this.RebuildDesignTimeTypesIfNeeded();
      return this.xamlDocumentContext.CreateNode((ITypeId) this.rootType);
    }

    public SampleDataValueBuilder CreateValueBuilder()
    {
      return new SampleDataValueBuilder(this, (IDocumentContext) this.xamlDocumentContext);
    }

    private void UpdateValues(IList<SampleDataChange> normalizedChanges)
    {
      if (normalizedChanges.Count <= 0)
        return;
      DocumentCompositeNode fromXamlDocument = this.ValidRootNodeFromXamlDocument;
      if (fromXamlDocument != null)
      {
        DocumentCompositeNode rootNode = SampleDataValueChangeProcessor.ApplyChanges(normalizedChanges, fromXamlDocument, this.ProjectDocument, false);
        if (rootNode == fromXamlDocument)
          return;
        this.IsDirtyValues = true;
        this.Save(rootNode);
      }
      else
        this.AutoGenerateValues();
    }

    public void AutoGenerateValues()
    {
      using (SampleDataValueBuilder valueBuilder = this.CreateValueBuilder())
        valueBuilder.GenerateValue((SampleType) this.rootType);
    }

    public string GetAbsoluteAssetFilePathFromRelativePath(string relativePath)
    {
      if (string.IsNullOrEmpty(relativePath))
        return relativePath;
      string absolutePath;
      if (this.ProjectContext.IsCapabilitySet(PlatformCapability.UsePathRelativeToProject))
      {
        string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/{0};component/", new object[1]
        {
          (object) this.ProjectContext.ProjectAssembly.Name
        });
        if (relativePath.StartsWith(str, StringComparison.Ordinal))
          relativePath = relativePath.Substring(str.Length);
        absolutePath = Path.Combine(Path.GetDirectoryName(this.ProjectContext.ProjectPath), relativePath.Replace('/', '\\'));
      }
      else
        absolutePath = Path.Combine(Path.GetDirectoryName(this.XamlFilePath), relativePath);
      ProjectItemAction projectItemAction = (ProjectItemAction) Enumerable.FirstOrDefault<SampleDataSet.AssetFileAction>((IEnumerable<SampleDataSet.AssetFileAction>) this.assetFiles.Values, (Func<SampleDataSet.AssetFileAction, bool>) (a =>
      {
        if (!a.IsCompleted && a.Operation == ProjectItemOperation.Add)
          return Microsoft.Expression.Framework.Documents.PathHelper.ArePathsEquivalent(a.ProjectItemPath, absolutePath);
        return false;
      }));
      if (projectItemAction != null)
        absolutePath = projectItemAction.SourceFilePath;
      return absolutePath;
    }

    public void UnuseAssetFile(string relativePath)
    {
      if (string.IsNullOrEmpty(relativePath))
        return;
      string fileName;
      try
      {
        fileName = Path.GetFileName(relativePath);
      }
      catch (ArgumentException ex)
      {
        return;
      }
      string key = fileName.ToUpperInvariant();
      SampleDataSet.AssetFileAction assetFileAction;
      if (!this.assetFiles.TryGetValue(key, out assetFileAction) || assetFileAction.Operation != ProjectItemOperation.Add && assetFileAction.Operation != ProjectItemOperation.Update)
        return;
      --assetFileAction.UsageCount;
      if (assetFileAction.UsageCount > 0)
        return;
      if (assetFileAction.Operation == ProjectItemOperation.Add)
        this.assetFiles.Remove(key);
      else
        assetFileAction.OnActionCompleted();
    }

    public string AddOrUpdateAssetFile(string sourcePath)
    {
      string fileName = Path.GetFileName(sourcePath);
      string key = fileName.ToUpperInvariant();
      string projectItemPath;
      string path2_1;
      if (this.ProjectContext.IsCapabilitySet(PlatformCapability.UsePathRelativeToProject))
      {
        string path2_2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}{2}{1}{2}{3}{1}{4}", (object) this.Context.DataRootFolder, (object) Path.AltDirectorySeparatorChar, (object) this.Name, (object) SampleDataSet.AssetsFolderSuffix, (object) fileName);
        projectItemPath = Path.Combine(Path.GetDirectoryName(this.ProjectContext.ProjectPath), path2_2);
        path2_1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/{0};component/{1}", new object[2]
        {
          (object) this.ProjectContext.ProjectAssembly.Name,
          (object) path2_2
        });
      }
      else
      {
        path2_1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}{2}{3}", (object) this.Name, (object) SampleDataSet.AssetsFolderSuffix, (object) Path.DirectorySeparatorChar, (object) fileName);
        projectItemPath = Path.Combine(this.SampleDataFolder, path2_1);
      }
      SampleDataSet.AssetFileAction assetFileAction = (SampleDataSet.AssetFileAction) null;
      if (!this.assetFiles.TryGetValue(key, out assetFileAction))
      {
        assetFileAction = new SampleDataSet.AssetFileAction(projectItemPath, this.IsEnabledAtRuntime, sourcePath);
        this.assetFiles[key] = assetFileAction;
      }
      else if (!sourcePath.Equals(assetFileAction.SourceFilePath, StringComparison.OrdinalIgnoreCase) && !sourcePath.Equals(assetFileAction.ProjectItemPath, StringComparison.OrdinalIgnoreCase))
      {
        bool flag = true;
        if (Microsoft.Expression.Framework.Documents.PathHelper.FileExists(assetFileAction.ProjectItemPath))
        {
          FileInfo fileInfo1 = new FileInfo(assetFileAction.ProjectItemPath);
          FileInfo fileInfo2 = new FileInfo(assetFileAction.ProjectItemPath);
          if (fileInfo1.LastWriteTime == fileInfo2.LastWriteTime && fileInfo1.Length == fileInfo2.Length)
            flag = false;
        }
        if (flag)
          assetFileAction.PrepareToUpdate(sourcePath, this.IsEnabledAtRuntime);
      }
      ++assetFileAction.UsageCount;
      return path2_1;
    }

    public void EnableAtRuntime(bool enable)
    {
      if (enable == this.IsEnabledAtRuntime)
        return;
      this.xamlItem.PrepareToUpdate(enable);
      foreach (ProjectItemAction projectItemAction in this.assetFiles.Values)
        projectItemAction.PrepareToUpdate(enable);
      List<ProjectItemAction> list = new List<ProjectItemAction>()
      {
        this.xamlItem
      };
      list.AddRange(Enumerable.Select<SampleDataSet.AssetFileAction, ProjectItemAction>((IEnumerable<SampleDataSet.AssetFileAction>) this.assetFiles.Values, (Func<SampleDataSet.AssetFileAction, ProjectItemAction>) (a => (ProjectItemAction) a)));
      if (!this.UpdateProject((IEnumerable<ProjectItemAction>) list, true))
        return;
      this.IsEnabledAtRuntime = enable;
      PerformanceUtility.StartPerformanceSequence(PerformanceEvent.SampleDataEnableDisableAtRuntime);
      this.ApplyEnableAtRuntimeInXaml();
      PerformanceUtility.EndPerformanceSequence(PerformanceEvent.SampleDataEnableDisableAtRuntime);
    }

    private void ApplyEnableAtRuntimeInXaml()
    {
      new SampleDataEnableAtRuntimeProcessor((IAsyncMechanism) new CurrentDispatcherAsyncMechanism(DispatcherPriority.Background), this, ChangeProcessingModes.CollectChanges | ChangeProcessingModes.ApplyChanges).Process(this.expressionInformationService);
    }

    public void CommitChanges(IMessageDisplayService messageService)
    {
      this.CommitChanges((DocumentCompositeNode) null, messageService);
    }

    public void CommitChanges(DocumentCompositeNode newRootNode, IMessageDisplayService messageService)
    {
      if (messageService == null)
      {
        this.CommitChangesInternal(newRootNode);
      }
      else
      {
        try
        {
          this.CommitChangesInternal(newRootNode);
        }
        catch (Exception ex)
        {
          messageService.ShowError(ex.Message);
          try
          {
            this.RollbackChanges();
          }
          catch
          {
          }
        }
      }
    }

    private void CommitChangesInternal(DocumentCompositeNode newRootNode)
    {
      if (this.disableChangeTracking)
      {
        this.RebuildDesignTimeTypesIfNeeded();
        if (newRootNode == null)
          return;
        this.RootNode = newRootNode;
      }
      else
      {
        if (!this.IsValid)
          return;
        PerformanceUtility.StartPerformanceSequence(PerformanceEvent.SampleDataSetCommitChanges);
        this.NotifySampleTypesChanging();
        List<SampleDataChange> list = (List<SampleDataChange>) this.NormalizedChanges;
        this.RemoveUnusedTypes();
        bool flag = this.RebuildDesignTimeTypesIfNeeded();
        if (newRootNode != null)
        {
          newRootNode.SourceContext = (INodeSourceContext) null;
          this.RootNode = newRootNode;
        }
        else
          this.UpdateValues((IList<SampleDataChange>) list);
        if (!this.IsSaved && !this.IsSaveFailed)
          this.Save();
        if (this.IsSaveFailed)
        {
          if (!this.xsdItem.IsCompleted && !this.xamlItem.IsCompleted && !this.codeItem.IsCompleted)
            this.RollbackChangesInternal((IList<SampleDataChange>) list);
        }
        else
          this.CommitChangesToXaml((IList<SampleDataChange>) list);
        PerformanceUtility.EndPerformanceSequence(PerformanceEvent.SampleDataSetCommitChanges);
        this.changes.Clear();
        if (!flag)
          return;
        this.NotifySampleTypesChanged();
      }
    }

    public void RollbackChanges()
    {
      if (this.changes.Count <= 0)
        return;
      this.RollbackChangesInternal(this.NormalizedChanges);
      this.changes.Clear();
      this.NotifySampleTypesChanged();
    }

    private void NotifySampleTypesChanged()
    {
      if (this.SampleTypesChanged == null)
        return;
      this.SampleTypesChanged(this, EventArgs.Empty);
    }

    private void NotifySampleTypesChanging()
    {
      if (this.SampleTypesChanging == null)
        return;
      this.SampleTypesChanging(this, EventArgs.Empty);
    }

    private void RollbackChangesInternal(IList<SampleDataChange> normalizedChanges)
    {
      using (this.DisableChangeTracking())
      {
        for (int index = normalizedChanges.Count - 1; index >= 0; --index)
          normalizedChanges[index].Undo();
        this.InitAssetFiles();
      }
      this.RebuildDesignTimeTypesIfNeeded();
      this.IsSaveFailed = false;
      this.IsSaved = true;
    }

    private void CommitChangesToXaml(IList<SampleDataChange> normalizedChanges)
    {
      if (!SampleDataChangeProcessor.ShouldProcessChanges(normalizedChanges))
        return;
      new SampleDataChangeProcessor((IAsyncMechanism) new CurrentDispatcherAsyncMechanism(DispatcherPriority.Background), this, normalizedChanges, ChangeProcessingModes.CollectChanges | ChangeProcessingModes.ApplyChanges).Process(this.expressionInformationService);
    }

    public void RemoveFromProjectAndClose()
    {
      this.IsOnline = false;
      this.RemoveReferencesFromXaml();
      this.RemoveFromProject();
      this.Close();
    }

    private void RemoveReferencesFromXaml()
    {
      new SampleDataRemovalProcessor((IAsyncMechanism) new CurrentDispatcherAsyncMechanism(DispatcherPriority.Background), this, ChangeProcessingModes.CollectChanges | ChangeProcessingModes.ApplyChanges).Process(this.expressionInformationService);
    }

    private bool RemoveFromProject()
    {
      List<ProjectItemAction> list = new List<ProjectItemAction>(Enumerable.Select<SampleDataSet.AssetFileAction, ProjectItemAction>((IEnumerable<SampleDataSet.AssetFileAction>) this.assetFiles.Values, (Func<SampleDataSet.AssetFileAction, ProjectItemAction>) (a => (ProjectItemAction) a)));
      list.Add(this.xsdItem);
      list.Add(this.codeItem);
      list.Add(this.xamlItem);
      list.ForEach((Action<ProjectItemAction>) (action => action.Operation = ProjectItemOperation.Delete));
      list.Add(new ProjectItemAction(this.AssetFilesFolder, ProjectItemOperation.Delete, false, (string) null));
      list.Add(new ProjectItemAction(this.SampleDataFolder, ProjectItemOperation.Delete, false, (string) null));
      return this.UpdateProject((IEnumerable<ProjectItemAction>) list, false);
    }

    public override string ToString()
    {
      if (this.RootType == null)
        return "Uninitialized " + base.ToString();
      return this.Name + " - " + this.RootType.ToString();
    }

    public IType ResolveSampleType(SampleType sampleType)
    {
      IType type = sampleType as IType;
      if (type == null)
      {
        SampleBasicType sampleBasicType = sampleType as SampleBasicType;
        if (sampleBasicType != null)
          type = this.ProjectContext.ResolveType(sampleBasicType.TypeId);
      }
      return type;
    }

    public SampleNonBasicType SampleTypeFromDesignTimeType(Type type)
    {
      if (this.designTimeClrNamespace == null || type.FullName == null || !type.FullName.StartsWith(this.designTimeClrNamespace, StringComparison.Ordinal))
        return (SampleNonBasicType) null;
      return this.GetSampleType(type.Name);
    }

    public IType GetSampleType(IXmlNamespace xmlNamespace, string typeName)
    {
      if (xmlNamespace.Value == this.xamlNamespace.Value)
        return (IType) this.GetSampleType(typeName);
      return (IType) null;
    }

    public bool IsTypeOwner(IType type)
    {
      SampleNonBasicType sampleNonBasicType = type as SampleNonBasicType;
      return sampleNonBasicType != null && sampleNonBasicType.DeclaringDataSet == this;
    }

    private class SampleDataSetState
    {
      public List<SampleDataChange> Changes { get; set; }

      public List<SampleNonBasicType> Types { get; set; }

      public SampleCompositeType RootType { get; set; }

      public SampleDataFlags Flags { get; set; }
    }

    private class OfflineToken : IDisposable
    {
      private SampleDataSet dataSet;
      private bool isOnline;

      public OfflineToken(SampleDataSet dataSet)
      {
        this.dataSet = dataSet;
        this.isOnline = this.dataSet.IsOnline;
        this.dataSet.IsOnline = false;
      }

      public void Dispose()
      {
        if (!this.dataSet.IsOnline)
          this.dataSet.IsOnline = this.isOnline;
        GC.SuppressFinalize((object) this);
      }
    }

    private class DisableChangeRecordingToken : IDisposable
    {
      private SampleDataSet dataSet;
      private bool degenerated;

      public DisableChangeRecordingToken(SampleDataSet dataSet)
      {
        this.dataSet = dataSet;
        this.degenerated = this.dataSet.disableChangeTracking;
        this.dataSet.disableChangeTracking = true;
      }

      public void Dispose()
      {
        if (!this.degenerated)
          this.dataSet.disableChangeTracking = false;
        GC.SuppressFinalize((object) this);
      }
    }

    private class AssetFileAction : ProjectItemAction
    {
      public int UsageCount { get; set; }

      public AssetFileAction(string projectItemPath, bool enabled, string sourceFilePath)
        : this(projectItemPath, ProjectItemOperation.Add, enabled, sourceFilePath)
      {
      }

      public AssetFileAction(string projectItemPath, ProjectItemOperation operations, bool enabled, string sourceFilePath)
        : base(projectItemPath, operations, enabled, sourceFilePath)
      {
      }

      public override void OnActionCompleted()
      {
        base.OnActionCompleted();
        this.UsageCount = 0;
      }
    }

    private class WorkaroundSerializerFilter : DefaultXamlSerializerFilter
    {
      public bool DefaultNamespaceRequired { get; private set; }

      public bool SeenDefaultNamespace { get; private set; }

      public WorkaroundSerializerFilter(bool defaultNamespaceRequired)
      {
        this.DefaultNamespaceRequired = defaultNamespaceRequired;
      }

      public override IXmlNamespace GetReplacementNamespace(IXmlNamespace xmlNamespace)
      {
        IXmlNamespace xmlNamespace1 = xmlNamespace;
        if (this.DefaultNamespaceRequired && xmlNamespace1 != null)
        {
          if (xmlNamespace1.Value == "http://schemas.microsoft.com/client/2007")
          {
            xmlNamespace1 = (IXmlNamespace) XmlNamespace.AvalonXmlNamespace;
            this.SeenDefaultNamespace = true;
          }
          else if (!this.SeenDefaultNamespace)
            this.SeenDefaultNamespace = xmlNamespace1.Value == "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
        }
        return base.GetReplacementNamespace(xmlNamespace1);
      }
    }
  }
}
