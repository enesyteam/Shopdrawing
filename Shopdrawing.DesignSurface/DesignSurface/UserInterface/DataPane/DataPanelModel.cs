// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.DataPanelModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Markup;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.SampleData;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface.ResourcePane;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.DesignSurface.ViewModel.SceneNodeSearch;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Configuration;
using Microsoft.Expression.Framework.Data;
using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.Templates;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  [DebuggerDisplay("{ViewModel.DocumentRoot}")]
  public class DataPanelModel : NotifyingObject
  {
    private static readonly string SharedDataFileName = "ProjectDataSources";
    private static readonly string SharedDataTag = "ContainsDataSources";
    private static readonly string DefaultXmlDataSourceName = "XMLDataSource";
    private static readonly string DataSourceSuffix = "DataSource";
    private List<DataSourceItem> delayedRefreshDataSources = new List<DataSourceItem>();
    private Dictionary<string, bool> dataContextExpansion = new Dictionary<string, bool>();
    private SceneViewModel viewModel;
    private ResourceDictionaryContentProvider sharedContentProvider;
    private bool shouldRefreshSharedDocument;
    private ObservableCollection<DataHostItem> dataHosts;
    private FrameworkElement addDataSourceMenuHost;
    private SpecialFolderManager xmlDataSourceFolderManager;
    private SceneNodeSubscription<object, object> basisDocumentSubscription;
    private SceneNodeSubscription<DataHostItem, DataSourceItem> documentSubscription;
    private Microsoft.Expression.DesignSurface.UserInterface.DataPane.SelectionContext<DataModelItemBase> selectionContext;
    private Microsoft.Expression.DesignSurface.UserInterface.DataPane.SelectionContext<DataModelItemBase> activeDataContextSelectionContext;
    private KeyValuePair<DataSourceItem, DataSchemaItem>? activeDataSource;
    private DocumentNode pendingSelectedDataSource;
    private DocumentNode pendingRenameSchemaItemDataSource;
    private string pendingRenameSchemaItemNodePath;
    private DocumentNode pendingSelectedSchemaItemDataSource;
    private string pendingSelectedSchemaItemNodePath;
    private Dictionary<SampleDataSet, Dictionary<SampleCompositeType, Dictionary<string, string>>> pendingRenamesBySampleDataSet;
    private SchemaItem activeDataContext;

    public FrameworkElement AddDataSourceMenuHost
    {
      get
      {
        return this.addDataSourceMenuHost;
      }
      set
      {
        this.addDataSourceMenuHost = value;
      }
    }

    public SceneViewModel ViewModel
    {
      get
      {
        return this.viewModel;
      }
    }

    public SpecialFolderManager XmlDataSourceFolderManager
    {
      get
      {
        return this.xmlDataSourceFolderManager;
      }
    }

    public IList DataHosts
    {
      get
      {
        return (IList) this.dataHosts;
      }
    }

    public bool HasDataHosts
    {
      get
      {
        if (this.DataHosts != null)
          return this.DataHosts.Count > 0;
        return false;
      }
    }

    public IEnumerable<DataSourceItem> DataSources
    {
      get
      {
        foreach (DataHostItem dataHostItem in (Collection<DataHostItem>) this.dataHosts)
        {
          foreach (DataSourceItem dataSourceItem in (IEnumerable) dataHostItem.DataSources)
            yield return dataSourceItem;
        }
      }
    }

    public KeyValuePair<DataSourceItem, DataSchemaItem>? ActiveDataSourceAndSchema
    {
      get
      {
        return this.activeDataSource;
      }
      set
      {
        if (this.activeDataSource.Equals((object) value))
          return;
        if (this.activeDataSource.HasValue)
          this.activeDataSource.Value.Key.ActiveChildItem = (DataSchemaItem) null;
        this.activeDataSource = value;
        if (this.activeDataSource.HasValue)
          this.activeDataSource.Value.Key.ActiveChildItem = this.activeDataSource.Value.Value;
        this.OnPropertyChanged("ActiveDataSourceAndSchema");
      }
    }

    public DataHostItem ClrPreferredDataHost
    {
      get
      {
        return (this.selectionContext != null ? Enumerable.FirstOrDefault<DataHostItem>(Enumerable.OfType<DataHostItem>((IEnumerable) this.selectionContext)) : (DataHostItem) null) ?? this.DocumentHost ?? this.SharedHost;
      }
    }

    internal Microsoft.Expression.DesignSurface.UserInterface.DataPane.SelectionContext<DataModelItemBase> SelectionContext
    {
      get
      {
        return this.selectionContext;
      }
    }

    public bool IsActive
    {
      get
      {
        if (this.viewModel != null)
          return JoltHelper.DatabindingSupported(this.ProjectContext);
        return false;
      }
    }

    public bool IsXmlEnabled
    {
      get
      {
        if (this.IsActive)
          return JoltHelper.TypeSupported((ITypeResolver) this.ProjectContext, PlatformTypes.XmlDataProvider);
        return false;
      }
    }

    public bool CanInsertDataSource
    {
      get
      {
        if (!this.IsDocumentEditable)
          return this.IsSharedEditable;
        return true;
      }
    }

    public bool CanInsertXmlDataSource
    {
      get
      {
        if (this.CanInsertDataSource)
          return this.IsXmlEnabled;
        return false;
      }
    }

    public bool CanInsertDataStore
    {
      get
      {
        return this.ProjectContext.IsCapabilitySet(PlatformCapability.SupportsDataStore);
      }
    }

    public bool IsAllXamlValid
    {
      get
      {
        if (this.IsDocumentEditable)
          return this.IsSharedEditable;
        return false;
      }
    }

    public ICommand AddXmlDataSourceCommand
    {
      get
      {
        return (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.AddXmlDataSourceCommandImpl));
      }
    }

    public ICommand CreateSampleDataCommand
    {
      get
      {
        return (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.CreateSampleData));
      }
    }

    public ICommand CreateDataStoreCommand
    {
      get
      {
        return (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.CreateDataStore));
      }
    }

    public ICommand CreateSampleDataFromXmlCommand
    {
      get
      {
        return (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.CreateSampleDataFromXml));
      }
    }

    public ICommand AddClrObjectDataSourceCommand
    {
      get
      {
        return (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.AddClrObjectDataSourceCommandImpl));
      }
    }

    public ICommand InvokeAddDataSourceMenuCommand
    {
      get
      {
        return (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.InvokeAddDataSourceMenuCommandImpl));
      }
    }

    public ICommand CreateDesignDataCommand
    {
      get
      {
        return (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.CreateDesignDataCommandImpl));
      }
    }

    public DataHostItem SharedHost { get; private set; }

    public DataHostItem DocumentHost { get; private set; }

    private DataSourcesCollection<DataSourceItem> SharedResourceDataSources
    {
      get
      {
        return this.SharedHost.ResourceDataSources;
      }
    }

    private DataSourcesCollection<FileBasedDataSourceItem> SharedFileDataSources
    {
      get
      {
        return this.SharedHost.FileDataSources;
      }
    }

    public DataHostItem CurrentDocumentHost
    {
      get
      {
        if (this.DocumentHost != null)
          return this.DocumentHost;
        if (this.sharedContentProvider != null && this.viewModel != null && (this.viewModel.Document != null && this.viewModel.Document.XamlDocument == this.sharedContentProvider.Document))
          return this.SharedHost;
        return (DataHostItem) null;
      }
    }

    public bool IsDetailsModeSet
    {
      get
      {
        return !this.IsMasterModeSet;
      }
      set
      {
        this.IsMasterModeSet = !value;
      }
    }

    public bool IsMasterModeSet
    {
      get
      {
        return DataBindingModeModel.Instance.Mode == DataBindingMode.Default;
      }
      set
      {
        if (value)
          DataBindingModeModel.Instance.SetMode(DataBindingMode.Default, true);
        else
          DataBindingModeModel.Instance.SetMode(DataBindingMode.StickyDetails, true);
        this.OnPropertyChanged("IsMasterModeSet");
        this.OnPropertyChanged("IsDetailsModeSet");
      }
    }

    private bool IsDocumentEditable
    {
      get
      {
        if (this.ViewModel != null && this.ViewModel.XamlDocument != null)
          return this.ViewModel.XamlDocument.IsEditable;
        return false;
      }
    }

    private bool IsSharedEditable
    {
      get
      {
        if (this.sharedContentProvider == null)
          return this.viewModel != null;
        if (this.sharedContentProvider.Document != null)
          return this.sharedContentProvider.Document.IsEditable;
        return false;
      }
    }

    private bool IsActiveViewModel
    {
      get
      {
        if (this.viewModel != null)
          return this.viewModel.IsActiveSceneViewModel;
        return false;
      }
    }

    public SceneDocument SharedDocument { get; private set; }

    internal IProjectContext ProjectContext
    {
      get
      {
        return this.viewModel.ProjectContext;
      }
    }

    internal IProject Project
    {
      get
      {
        return (IProject) this.ProjectContext.GetService(typeof (IProject));
      }
    }

    internal DesignerContext DesignerContext
    {
      get
      {
        return this.viewModel.DesignerContext;
      }
    }

    internal ResourceManager ResourceManager
    {
      get
      {
        return this.DesignerContext.ResourceManager;
      }
    }

    internal SchemaManager SchemaManager
    {
      get
      {
        return ProjectXamlContext.FromProjectContext(this.ProjectContext).SchemaManager;
      }
    }

    internal SampleDataCollection SampleData
    {
      get
      {
        return ProjectXamlContext.FromProjectContext(this.ProjectContext).SampleData;
      }
    }

    public bool HasActiveDataContext
    {
      get
      {
        return this.activeDataContext != null;
      }
    }

    public SchemaItem ActiveDataContext
    {
      get
      {
        return this.activeDataContext;
      }
    }

    internal DataPanelModel(SceneViewModel viewModel, bool initializeOnCreation)
    {
      this.viewModel = viewModel;
      if (this.viewModel == null)
        return;
      this.pendingRenamesBySampleDataSet = new Dictionary<SampleDataSet, Dictionary<SampleCompositeType, Dictionary<string, string>>>();
      if (!initializeOnCreation)
        this.viewModel.EarlySceneUpdatePhase += new SceneUpdatePhaseEventHandler(this.ViewModel_EarlySceneUpdatePhase);
      else
        this.Initialize();
    }

    private void ViewModel_EarlySceneUpdatePhase(object sender, SceneUpdatePhaseEventArgs args)
    {
      this.Initialize();
      this.viewModel.EarlySceneUpdatePhase -= new SceneUpdatePhaseEventHandler(this.ViewModel_EarlySceneUpdatePhase);
    }

    internal void Initialize()
    {
      if (this.viewModel == null)
        return;
      this.xmlDataSourceFolderManager = new SpecialFolderManager(Environment.SpecialFolder.Personal.ToString(), this.DesignerContext.Configuration);
      this.selectionContext = (Microsoft.Expression.DesignSurface.UserInterface.DataPane.SelectionContext<DataModelItemBase>) new MultipleSelectionContext<DataModelItemBase>();
      this.activeDataContextSelectionContext = (Microsoft.Expression.DesignSurface.UserInterface.DataPane.SelectionContext<DataModelItemBase>) new MultipleSelectionContext<DataModelItemBase>();
      this.dataHosts = new ObservableCollection<DataHostItem>();
      this.SharedHost = new DataHostItem(new DataSourceHost(), this, StringTable.ProjectDataHostName, "Project");
      this.SharedHost.DocumentRequired = false;
      this.dataHosts.Add(this.SharedHost);
      this.InitializeSharedDocument();
      this.InitializeSharedResourceDataSources();
      this.InitializeDesignDataSources();
      if (this.SharedDocument != this.viewModel.Document)
      {
        this.DocumentHost = new DataHostItem((DataSourceHost) null, this, StringTable.LocalDocumentDataHostName, "Local Document");
        this.DocumentHost.DocumentContext = this.ViewModel.Document.DocumentContext;
        this.DocumentHost.DocumentRequired = true;
        this.dataHosts.Add(this.DocumentHost);
        this.InitializeDocumentSubscription();
        this.EnsureDocumentSubscriptionUpToDate(this.viewModel.Damage, this.viewModel.Document.XamlDocument.ChangeStamp);
        this.viewModel.Document.DocumentRoot.TypesChanged += new EventHandler(this.DocumentRoot_ModelChanged);
      }
      this.viewModel.LateSceneUpdatePhase += new SceneUpdatePhaseEventHandler(this.ViewModel_LateSceneUpdatePhase);
      this.viewModel.DesignerContext.ViewService.ActiveViewChanging += new ViewChangedEventHandler(this.ActiveView_Changing);
      this.viewModel.DesignerContext.ProjectManager.ProjectClosing += new EventHandler<ProjectEventArgs>(this.ProjectManager_ProjectClosing);
      this.OnPropertyChanged((string) null);
      this.UpdateActveDataContext();
    }

    private FileBasedDataSourceItem AddDesignDataSchema(IProjectItem designDataFile, bool select)
    {
      FileBasedDataSourceItem dataSource = new FileBasedDataSourceItem(this.ProvideSchemaForDesingDataFile(designDataFile) ?? (ISchema) new EmptySchema(), this, designDataFile);
      this.AddDesignDataSchema(dataSource, select);
      this.RegisterForDocumentChanges(designDataFile.Document as SceneDocument);
      return dataSource;
    }

    private void AddDesignDataSchema(FileBasedDataSourceItem dataSource, bool select)
    {
      int index;
      for (index = this.SharedFileDataSources.Count - 1; index >= 0; --index)
      {
        FileBasedDataSourceItem basedDataSourceItem = this.SharedFileDataSources[index];
        if (basedDataSourceItem == null || string.Compare(dataSource.DisplayName, basedDataSourceItem.DisplayName, StringComparison.OrdinalIgnoreCase) >= 0)
        {
          this.SharedFileDataSources.InsertDataSource(index + 1, dataSource);
          break;
        }
      }
      if (index < 0)
        this.SharedFileDataSources.InsertDataSource(0, dataSource);
      if (!select)
        return;
      this.SharedHost.ExpandAncestors();
      dataSource.IsExpanded = true;
      this.selectionContext.Clear();
      this.selectionContext.Add((DataModelItemBase) dataSource);
    }

    private FileBasedDataSourceItem GetDesignDataSchema(IProjectItem designDataFile)
    {
      return Enumerable.FirstOrDefault<FileBasedDataSourceItem>((IEnumerable<FileBasedDataSourceItem>) this.SharedFileDataSources, (Func<FileBasedDataSourceItem, bool>) (dataSource => dataSource.DesignDataFile == designDataFile));
    }

    private void RemoveDesignDataSchema(IProjectItem designDataFile)
    {
      FileBasedDataSourceItem designDataSchema = this.GetDesignDataSchema(designDataFile);
      if (designDataSchema == null)
        return;
      this.selectionContext.Remove((DataModelItemBase) designDataSchema);
      this.delayedRefreshDataSources.Remove((DataSourceItem) designDataSchema);
      this.SharedFileDataSources.RemoveDataSource(designDataSchema);
    }

    private bool RefreshDesignDataSchema(IProjectItem designDataFile)
    {
      for (int index = 0; index < this.SharedFileDataSources.Count; ++index)
      {
        FileBasedDataSourceItem basedDataSourceItem = this.SharedFileDataSources[index];
        if (basedDataSourceItem != null && basedDataSourceItem.DesignDataFile == designDataFile)
        {
          this.RefreshSchemaIfNeeded((DataSourceItem) basedDataSourceItem, false);
          return true;
        }
      }
      return false;
    }

    private void ProjectItem_Added(object sender, ProjectItemEventArgs args)
    {
      if (string.Compare(Path.GetExtension(args.ProjectItem.DocumentReference.Path), ".xaml", StringComparison.OrdinalIgnoreCase) != 0)
        return;
      this.MarkSharedDocumentForRefresh();
      if (DocumentContextHelper.GetDesignDataMode(args.ProjectItem) == DesignDataMode.None)
        return;
      this.AddDesignDataSchema(args.ProjectItem, true);
    }

    private void ProjectItem_Removed(object sender, ProjectItemEventArgs args)
    {
      if (string.Compare(Path.GetExtension(args.ProjectItem.DocumentReference.Path), ".xaml", StringComparison.OrdinalIgnoreCase) != 0)
        return;
      this.MarkSharedDocumentForRefresh();
      this.RemoveDesignDataSchema(args.ProjectItem);
    }

    private void ProjectItem_Renamed(object sender, ProjectItemRenamedEventArgs args)
    {
      if (string.Compare(Path.GetExtension(args.OldName.Path), ".xaml", StringComparison.OrdinalIgnoreCase) != 0 && string.Compare(Path.GetExtension(args.ProjectItem.DocumentReference.Path), ".xaml", StringComparison.OrdinalIgnoreCase) != 0)
        return;
      this.MarkSharedDocumentForRefresh();
      if (DocumentContextHelper.GetDesignDataMode(args.ProjectItem) == DesignDataMode.None)
        return;
      FileBasedDataSourceItem designDataSchema = this.GetDesignDataSchema(args.ProjectItem);
      if (designDataSchema == null)
        return;
      this.SharedFileDataSources.RemoveDataSource(designDataSchema);
      this.AddDesignDataSchema(designDataSchema, false);
    }

    private void ProjectItem_Changed(object sender, ProjectItemEventArgs args)
    {
      this.UpdateProjectItem(args.ProjectItem);
    }

    private void UpdateProjectItem(IProjectItem projectItem)
    {
      if (projectItem == null || this.viewModel == null)
        return;
      if (DocumentContextHelper.GetDesignDataMode(projectItem) != DesignDataMode.None)
      {
        if (this.RefreshDesignDataSchema(projectItem))
          return;
        this.AddDesignDataSchema(projectItem, true);
      }
      else
        this.RemoveDesignDataSchema(projectItem);
    }

    private IProjectItem FromSceneDocument(SceneDocument document)
    {
      if (document == null || document.ProjectContext != this.ProjectContext)
        return (IProjectItem) null;
      return this.Project.FindItem(document.DocumentReference);
    }

    private void ActiveView_Changing(object sender, ViewChangedEventArgs e)
    {
      SceneView sceneView = e.NewView as SceneView;
      if (sceneView == null || sceneView.ViewModel != this.viewModel)
        return;
      this.RefreshSharedDocumentIfNeeded();
      if (this.delayedRefreshDataSources.Count <= 0)
        return;
      List<DataSourceItem> list = this.delayedRefreshDataSources;
      this.delayedRefreshDataSources = new List<DataSourceItem>();
      foreach (DataSourceItem dataSourceItem in list)
        this.RefreshSchemaIfNeeded(dataSourceItem, true);
      this.OnPropertyChanged("DataSources");
    }

    private void ProjectManager_ProjectClosing(object sender, ProjectEventArgs args)
    {
      if (this.viewModel == null || this.viewModel.DesignerContext == null || args.Project != this.Project)
        return;
      this.UnregisterFromDesignDataChanges();
    }

    private void Context_DocumentClosedEarly(object sender, ProjectDocumentEventArgs args)
    {
      if (this.viewModel == null)
        return;
      if (this.viewModel.DocumentRoot == args.Document.DocumentRoot)
        this.UnregisterFromDesignDataChanges();
      else
        this.UnregisterFromDocumentChanges(args.Document.ProjectItem.Document as SceneDocument);
    }

    private void Context_DocumentClosedLate(object sender, ProjectDocumentEventArgs args)
    {
      this.UpdateProjectItem(args.Document.ProjectItem);
    }

    private void Context_DocumentOpened(object sender, ProjectDocumentEventArgs args)
    {
      if (this.GetDesignDataSchema(args.Document.ProjectItem) == null)
        return;
      this.RegisterForDocumentChanges(args.Document.ProjectItem.Document as SceneDocument);
    }

    private void RegisterForDocumentChanges(SceneDocument sceneDocument)
    {
      if (sceneDocument == null)
        return;
      this.UnregisterFromDocumentChanges(sceneDocument);
      sceneDocument.EditTransactionCompleted += new EventHandler(this.DesignDataChanged);
      sceneDocument.EditTransactionUndoRedo += new EventHandler(this.DesignDataChanged);
    }

    private void UnregisterFromDocumentChanges(SceneDocument sceneDocument)
    {
      if (sceneDocument == null)
        return;
      sceneDocument.EditTransactionCompleted -= new EventHandler(this.DesignDataChanged);
      sceneDocument.EditTransactionUndoRedo -= new EventHandler(this.DesignDataChanged);
    }

    private void RegisterForDesignDataChanges()
    {
      if (this.viewModel == null)
        return;
      this.ProjectContext.DocumentClosing += new EventHandler<ProjectDocumentEventArgs>(this.Context_DocumentClosedEarly);
      this.ProjectContext.DocumentClosed += new EventHandler<ProjectDocumentEventArgs>(this.Context_DocumentClosedLate);
      this.ProjectContext.DocumentOpened += new EventHandler<ProjectDocumentEventArgs>(this.Context_DocumentOpened);
      this.SchemaManager.ClrObjectSchemasInvalidated += new EventHandler(this.SchemaManager_ClrObjectSchemasChanged);
      this.SchemaManager.SampleTypesChanging += new SampleDataChangedEventHandler(this.SchemaManager_SampleTypesChanging);
      IProject project = this.Project;
      project.ItemAdded += new EventHandler<ProjectItemEventArgs>(this.ProjectItem_Added);
      project.ItemRemoved += new EventHandler<ProjectItemEventArgs>(this.ProjectItem_Removed);
      project.ItemRenamed += new EventHandler<ProjectItemRenamedEventArgs>(this.ProjectItem_Renamed);
      project.ItemChanged += new EventHandler<ProjectItemEventArgs>(this.ProjectItem_Changed);
    }

    private void UnregisterFromDesignDataChanges()
    {
      if (this.viewModel == null || this.viewModel.DesignerContext == null)
        return;
      this.ProjectContext.DocumentClosing -= new EventHandler<ProjectDocumentEventArgs>(this.Context_DocumentClosedEarly);
      this.ProjectContext.DocumentClosed -= new EventHandler<ProjectDocumentEventArgs>(this.Context_DocumentClosedLate);
      this.ProjectContext.DocumentOpened -= new EventHandler<ProjectDocumentEventArgs>(this.Context_DocumentOpened);
      this.SchemaManager.ClrObjectSchemasInvalidated -= new EventHandler(this.SchemaManager_ClrObjectSchemasChanged);
      this.SchemaManager.SampleTypesChanging -= new SampleDataChangedEventHandler(this.SchemaManager_SampleTypesChanging);
      IProject project = this.Project;
      project.ItemAdded -= new EventHandler<ProjectItemEventArgs>(this.ProjectItem_Added);
      project.ItemRemoved -= new EventHandler<ProjectItemEventArgs>(this.ProjectItem_Removed);
      project.ItemRenamed -= new EventHandler<ProjectItemRenamedEventArgs>(this.ProjectItem_Renamed);
      project.ItemChanged -= new EventHandler<ProjectItemEventArgs>(this.ProjectItem_Changed);
      if (this.SharedHost == null)
        return;
      foreach (FileBasedDataSourceItem basedDataSourceItem in (Collection<FileBasedDataSourceItem>) this.SharedFileDataSources)
        this.UnregisterFromDocumentChanges(basedDataSourceItem.DesignDataFile.Document as SceneDocument);
    }

    private void DesignDataChanged(object sender, EventArgs e)
    {
      this.UpdateProjectItem(this.FromSceneDocument(sender as SceneDocument));
    }

    private void MarkSharedDocumentForRefresh()
    {
      this.shouldRefreshSharedDocument = true;
      if (!this.IsActiveViewModel || !this.RefreshSharedDocumentIfNeeded())
        return;
      this.OnPropertyChanged("DataSources");
    }

    private bool RefreshSharedDocumentIfNeeded()
    {
      if (this.shouldRefreshSharedDocument)
      {
        this.shouldRefreshSharedDocument = false;
        SceneDocument sharedDocument = this.SharedDocument;
        this.InitializeSharedDocument();
        if (sharedDocument != this.SharedDocument || this.SharedDocument != null && this.sharedContentProvider != this.ResourceManager.FindContentProviderForResourceDictionary(this.SharedDocument))
        {
          this.InitializeSharedResourceDataSources();
          return true;
        }
      }
      return false;
    }

    private void InitializeSharedResourceDataSources()
    {
      if (this.SharedDocument != null)
      {
        this.SharedHost.DocumentRequired = true;
        this.SharedHost.DocumentContext = this.SharedDocument.DocumentContext;
      }
      else
      {
        this.SharedHost.DocumentRequired = false;
        this.SharedHost.DocumentContext = (IDocumentContext) null;
      }
      ResourceDictionaryContentProvider dictionaryContentProvider = (ResourceDictionaryContentProvider) null;
      if (this.SharedDocument != null)
        dictionaryContentProvider = this.ResourceManager.FindContentProviderForResourceDictionary(this.SharedDocument);
      if (dictionaryContentProvider == this.sharedContentProvider && this.SharedResourceDataSources.Count > 0)
        return;
      bool flag = this.selectionContext.Remove((DataModelItemBase) this.SharedHost);
      for (int index = this.SharedResourceDataSources.Count - 1; index >= 0; --index)
      {
        DataSourceItem dataSource = this.SharedResourceDataSources[index];
        if (!(dataSource is FileBasedDataSourceItem))
          this.RemoveResourceDataSourceInternal(this.SharedHost, dataSource);
      }
      if (this.sharedContentProvider != null)
      {
        this.sharedContentProvider.ItemsChanged -= new CollectionChangeEventHandler(this.Shared_ItemsChanged);
        this.sharedContentProvider.KeyChanged -= new EventHandler<EventArgs<DocumentNode>>(this.Shared_KeyChanged);
      }
      this.sharedContentProvider = dictionaryContentProvider;
      if (this.sharedContentProvider != null)
      {
        this.sharedContentProvider.ItemsChanged += new CollectionChangeEventHandler(this.Shared_ItemsChanged);
        this.sharedContentProvider.KeyChanged += new EventHandler<EventArgs<DocumentNode>>(this.Shared_KeyChanged);
        this.UpdateSharedDataHost();
      }
      if (!flag)
        return;
      this.selectionContext.Add((DataModelItemBase) this.SharedHost);
    }

    private void InitializeDesignDataSources()
    {
      foreach (IProjectItem designDataFile in Enumerable.Where<IProjectItem>((IEnumerable<IProjectItem>) this.Project.Items, (Func<IProjectItem, bool>) (item => DocumentContextHelper.GetDesignDataMode(item) != DesignDataMode.None)))
        this.AddDesignDataSchema(designDataFile, false);
      this.RegisterForDesignDataChanges();
    }

    internal void Unload()
    {
      if (this.viewModel == null)
        return;
      this.viewModel.LateSceneUpdatePhase -= new SceneUpdatePhaseEventHandler(this.ViewModel_LateSceneUpdatePhase);
      this.viewModel.DesignerContext.ViewService.ActiveViewChanging -= new ViewChangedEventHandler(this.ActiveView_Changing);
      this.viewModel.DesignerContext.ProjectManager.ProjectClosing -= new EventHandler<ProjectEventArgs>(this.ProjectManager_ProjectClosing);
      this.UnregisterFromDesignDataChanges();
      if (this.dataHosts != null)
      {
        foreach (DataHostItem dataHostItem in (Collection<DataHostItem>) this.dataHosts)
        {
          if (dataHostItem.DataSourceHost != null)
            dataHostItem.DataSourceHost.Unload();
        }
        this.dataHosts = (ObservableCollection<DataHostItem>) null;
      }
      if (this.documentSubscription != null)
      {
        this.documentSubscription.SetBasisNodeInsertedHandler((SceneNodeSubscription<DataHostItem, DataSourceItem>.BasisNodeInsertedHandler) null);
        this.documentSubscription.BasisNodeRemoved -= new SceneNodeSubscription<DataHostItem, DataSourceItem>.BasisNodeRemovedListener(this.DocumentHostSubscription_DataHostItemRemoved);
        this.documentSubscription.SetPathNodeInsertedHandler((SceneNodeSubscription<DataHostItem, DataSourceItem>.PathNodeInsertedHandler) null);
        this.documentSubscription.PathNodeRemoved -= new SceneNodeSubscription<DataHostItem, DataSourceItem>.PathNodeRemovedListener(this.DocumentHostSubscription_ResourceDataSourceRemoved);
        this.documentSubscription.PathNodeContentChanged -= new SceneNodeSubscription<DataHostItem, DataSourceItem>.PathNodeContentChangedListener(this.DocumentHostSubscription_ResourceDataSourceContentChanged);
        this.documentSubscription.CurrentViewModel = (SceneViewModel) null;
        this.documentSubscription = (SceneNodeSubscription<DataHostItem, DataSourceItem>) null;
      }
      if (this.basisDocumentSubscription != null)
      {
        this.basisDocumentSubscription.CurrentViewModel = (SceneViewModel) null;
        this.basisDocumentSubscription = (SceneNodeSubscription<object, object>) null;
      }
      this.SchemaManager.ClrObjectSchemasInvalidated -= new EventHandler(this.SchemaManager_ClrObjectSchemasChanged);
      this.SchemaManager.SampleTypesChanging -= new SampleDataChangedEventHandler(this.SchemaManager_SampleTypesChanging);
      if (this.sharedContentProvider != null)
      {
        this.sharedContentProvider.ItemsChanged -= new CollectionChangeEventHandler(this.Shared_ItemsChanged);
        this.sharedContentProvider.KeyChanged -= new EventHandler<EventArgs<DocumentNode>>(this.Shared_KeyChanged);
        this.sharedContentProvider = (ResourceDictionaryContentProvider) null;
      }
      this.selectionContext = (Microsoft.Expression.DesignSurface.UserInterface.DataPane.SelectionContext<DataModelItemBase>) null;
      this.viewModel.Document.DocumentRoot.TypesChanged -= new EventHandler(this.DocumentRoot_ModelChanged);
      this.viewModel = (SceneViewModel) null;
    }

    private void InitializeSharedDocument()
    {
      this.SharedDocument = (SceneDocument) null;
      if (this.viewModel == null)
        return;
      foreach (IProjectItem projectItem in (IEnumerable<IProjectItem>) this.Project.Items)
      {
        IMSBuildItem msBuildItem = projectItem as IMSBuildItem;
        string str = msBuildItem != null ? msBuildItem.GetMetadata(DataPanelModel.SharedDataTag) : (string) null;
        bool result;
        if (!string.IsNullOrEmpty(str) && bool.TryParse(str, out result) && result)
        {
          this.SharedDocument = this.viewModel.Document.GetSceneDocument(projectItem.DocumentReference.Path);
          return;
        }
      }
      this.SharedDocument = this.viewModel.Document.ApplicationSceneDocument;
      if (this.SharedDocument == null || this.SharedDocument.ProjectContext == this.ProjectContext)
        return;
      this.SharedDocument = (SceneDocument) null;
    }

    private bool EnsureSharedDocument()
    {
      if (this.SharedDocument != null)
        return true;
      IProject project = this.Project;
      TemplateItemHelper templateItemHelper = new TemplateItemHelper(project, (IList<string>) null, (IServiceProvider) this.DesignerContext.Services);
      IProjectItemTemplate templateItem = Enumerable.FirstOrDefault<IProjectItemTemplate>(templateItemHelper.TemplateItems, (Func<IProjectItemTemplate, bool>) (item => item.DefaultName == "ResourceDictionary.xaml"));
      if (templateItem == null)
        return false;
      string directoryName = Path.GetDirectoryName(this.ProjectContext.ProjectPath);
      string str = DataPanelModel.SharedDataFileName + ".xaml";
      for (int index = 1; index < 1073741823; ++index)
      {
        string path = Path.Combine(directoryName, str);
        if (Microsoft.Expression.Framework.Documents.PathHelper.FileExists(path) || project.FindItem(DocumentReference.Create(path)) != null)
          str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}.xaml", new object[2]
          {
            (object) DataPanelModel.SharedDataFileName,
            (object) index
          });
        else
          break;
      }
      IProjectItem projectItem = (IProjectItem) null;
      try
      {
        this.ResourceManager.ListenForProjectChanges = false;
        List<IProjectItem> itemsToOpen;
        projectItem = Enumerable.FirstOrDefault<IProjectItem>(templateItemHelper.AddProjectItemsForTemplateItem(templateItem, str, directoryName, CreationOptions.DoNotSelectCreatedItems | CreationOptions.DoNotSetDefaultImportPath, out itemsToOpen));
        if (projectItem == null)
          return false;
        ((IMSBuildItem) projectItem).SetMetadata(DataPanelModel.SharedDataTag, "True");
      }
      finally
      {
        this.ResourceManager.ListenForProjectChanges = true;
      }
      this.SharedDocument = this.viewModel.Document.GetSceneDocument(projectItem.DocumentReference.Path);
      this.ResourceManager.GetContentProviderForResourceDictionary(projectItem);
      this.InitializeSharedResourceDataSources();
      return this.SharedDocument != null;
    }

    private XmlDataSourceDialogModel ShowDataSourceDialog(DataSourceDialogMode mode, string dataSourceName)
    {
      XmlDataSourceDialog dataSourceDialog = new XmlDataSourceDialog(this, dataSourceName, mode);
      try
      {
        bool? nullable = dataSourceDialog.ShowDialog();
        if (nullable.HasValue)
        {
          if (nullable.Value)
            goto label_5;
        }
        return (XmlDataSourceDialogModel) null;
      }
      catch (Exception ex)
      {
        return (XmlDataSourceDialogModel) null;
      }
label_5:
      dataSourceName = dataSourceDialog.Model.DataSourceName;
      if (dataSourceName != null)
        dataSourceName = dataSourceName.Trim();
      if (string.IsNullOrEmpty(dataSourceName))
      {
        this.DesignerContext.MessageDisplayService.ShowError(string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.DataSourceNameErrorDescription, new object[1]
        {
          (object) dataSourceName
        }));
        return (XmlDataSourceDialogModel) null;
      }
      dataSourceDialog.Model.DataSourceName = dataSourceName;
      if (mode == DataSourceDialogMode.DefineNewDataStore)
        dataSourceDialog.Model.IsEnabledAtRuntime = true;
      PerformanceUtility.MeasurePerformanceUntilRender(PerformanceEvent.AddXmlDataSource);
      return dataSourceDialog.Model;
    }

    private void AddXmlDataSourceCommandImpl()
    {
      XmlDataSourceDialogModel sourceDialogModel = this.ShowDataSourceDialog(DataSourceDialogMode.LiveXmlData, DataPanelModel.DefaultXmlDataSourceName);
      if (sourceDialogModel == null)
        return;
      DocumentNode xmlDataSource = sourceDialogModel.XmlDataSource;
      if (xmlDataSource == null)
      {
        this.DesignerContext.MessageDisplayService.ShowError(StringTable.DataSourceUriFormatErrorDescription);
      }
      else
      {
        try
        {
          if (sourceDialogModel.IsDefinedInProjectRoot && !this.EnsureSharedDocument())
            return;
          this.AddXmlDataSource((XmlDataProviderSceneNode) this.viewModel.GetSceneNode(xmlDataSource), sourceDialogModel.IsDefinedInProjectRoot ? this.SharedHost : this.CurrentDocumentHost, sourceDialogModel.DataSourceName);
        }
        catch (Exception ex)
        {
          this.DisplayException(ex, StringTable.AddXmlDataSourceDialogTitle);
        }
      }
    }

    private void DisplayException(Exception e, string title)
    {
      this.DesignerContext.MessageDisplayService.ShowError(new ErrorArgs()
      {
        Message = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.CommandFailedDialogMessage, new object[1]
        {
          (object) title
        }),
        Exception = e
      });
    }

    private void AddClrObjectDataSourceCommandImpl()
    {
      string dataSourceName;
      Type dataSourceType;
      SceneNode objectDataSource = ClrObjectDataSourceDialog.CreateClrObjectDataSource(out dataSourceName, out dataSourceType, this);
      if (objectDataSource == null)
        return;
      if (dataSourceName == ClrObjectDataSourceDialog.DefaultClrObjectDataSourceName && dataSourceType != (Type) null)
        dataSourceName = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.DataSourceNameFormatString, new object[1]
        {
          (object) dataSourceType.Name
        });
      if (this.ClrPreferredDataHost.DataSourceHost.DocumentNode == null && !this.EnsureSharedDocument())
        return;
      this.AddDataSourceToHost(this.ClrPreferredDataHost, dataSourceName, objectDataSource, StringTable.UndoUnitCreateClrObjectDataSource);
    }

    private void InvokeAddDataSourceMenuCommandImpl()
    {
      if (this.addDataSourceMenuHost == null)
        return;
      this.addDataSourceMenuHost.ContextMenu.DataContext = (object) this;
      this.addDataSourceMenuHost.ContextMenu.IsOpen = true;
    }

    private void CreateDesignDataCommandImpl()
    {
      DesignDataHelper.PromptAndCreateDesignData(this.ViewModel);
    }

    public string AddDataSourceToHost(DataHostItem dataSourceHost, string dataSourceName, SceneNode dataSource, string undoName)
    {
      DocumentNode documentNode = dataSourceHost.DataSourceHost.DocumentNode;
      SceneViewModel viewModel = this.viewModel.GetViewModel(documentNode.DocumentRoot, false);
      SceneNode sceneNode = viewModel.GetSceneNode(documentNode);
      DocumentCompositeNode documentCompositeNode = (dataSource.DocumentNode as DocumentCompositeNode).Clone(viewModel.Document.DocumentContext) as DocumentCompositeNode;
      using (SceneEditTransaction editTransaction = viewModel.CreateEditTransaction(undoName))
      {
        IDocumentContext context = documentCompositeNode.Context;
        if (documentCompositeNode != null)
          documentCompositeNode.Properties[DesignTimeProperties.IsDataSourceProperty] = context.CreateNode(typeof (bool), (object) true);
        ResourceSite resourceSite = new ResourceSite(sceneNode.DocumentNode);
        dataSourceName = resourceSite.GetUniqueResourceKey(dataSourceName);
        DocumentNode keyNode = (DocumentNode) resourceSite.DocumentContext.CreateNode(dataSourceName);
        resourceSite.CreateResource(keyNode, (DocumentNode) documentCompositeNode, 0);
        editTransaction.Commit();
      }
      this.pendingSelectedDataSource = (DocumentNode) documentCompositeNode;
      dataSourceHost.ExpandAncestors();
      return dataSourceName;
    }

    public void RemoveDataSource(DataSourceItem dataSource, bool confirmDelete)
    {
      if (confirmDelete)
      {
        if (this.DesignerContext.MessageDisplayService.ShowMessage(new MessageBoxArgs()
        {
          Message = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.ConfirmDeleteDataSourceMessage, new object[1]
          {
            (object) dataSource.DisplayName
          }),
          Button = MessageBoxButton.YesNo,
          Image = MessageBoxImage.Exclamation
        }) != MessageBoxResult.Yes)
          return;
      }
      FileBasedDataSourceItem basedDataSourceItem = dataSource as FileBasedDataSourceItem;
      if (basedDataSourceItem != null)
      {
        DesignDataHelper.RemoveDesignData(basedDataSourceItem.DesignDataFile, this.ProjectContext, this.DesignerContext.ExpressionInformationService);
      }
      else
      {
        SampleDataSet sampleData = dataSource.DataSourceNode.SampleData;
        if (sampleData != null)
          this.viewModel.DefaultView.SampleData.DeleteSampleDataSet(sampleData);
        else
          new LiveDataRemovalProcessor((IAsyncMechanism) new CurrentDispatcherAsyncMechanism(DispatcherPriority.Background), (DocumentCompositeNode) dataSource.DataSourceNode.DocumentNode, ChangeProcessingModes.CollectChanges | ChangeProcessingModes.ApplyChanges).Process(this.ViewModel.DesignerContext.ExpressionInformationService);
      }
    }

    public void AddXmlDataSourceFromDrop(string[] files)
    {
      if (files.Length <= 0)
        return;
      IEnumerable<IProjectItem> source = this.DesignerContext.ActiveProject.AddItems(Enumerable.Select<string, DocumentCreationInfo>((IEnumerable<string>) files, (Func<string, DocumentCreationInfo>) (file => new DocumentCreationInfo()
      {
        SourcePath = file
      })));
      if (source == null || !Enumerable.Any<IProjectItem>(source))
        return;
      XmlDataSourceDialogModel sourceDialogModel = new XmlDataSourceDialogModel(this, DataPanelModel.DefaultXmlDataSourceName, DataSourceDialogMode.LiveXmlData);
      if (sourceDialogModel == null || sourceDialogModel.IsDefinedInProjectRoot && !this.EnsureSharedDocument())
        return;
      using (SceneEditTransaction editTransaction = this.DesignerContext.ActiveSceneViewModel.CreateEditTransaction(StringTable.DropXMLFilesIntoDataPaneUndo))
      {
        foreach (IProjectItem projectItem in source)
        {
          string dataSourceName = Path.GetFileNameWithoutExtension(projectItem.DocumentReference.Path) + DataPanelModel.DataSourceSuffix;
          sourceDialogModel.DataSourceUrl = projectItem.Project.DocumentReference.GetRelativePath(projectItem.DocumentReference);
          DocumentNode xmlDataSource = sourceDialogModel.XmlDataSource;
          if (xmlDataSource != null)
            this.AddXmlDataSource((XmlDataProviderSceneNode) this.viewModel.GetSceneNode(xmlDataSource), sourceDialogModel.IsDefinedInProjectRoot ? this.SharedHost : this.CurrentDocumentHost, dataSourceName);
        }
        editTransaction.Commit();
      }
    }

    public string GetDefaultDesignerDataSetName(DataSetContext context)
    {
      return this.viewModel.DefaultView.SampleData.GetUniqueSampleDataSetName(context.DataSetType == DataSetType.SampleDataSet ? "SampleDataSource" : "DataStore");
    }

    public void CreateSampleData()
    {
      this.CreateDesignerDataSet(DataSetContext.SampleData);
    }

    public void CreateDataStore()
    {
      this.CreateDesignerDataSet(DataSetContext.DataStore);
    }

    private void CreateDesignerDataSet(DataSetContext context)
    {
      XmlDataSourceDialogModel sourceDialogModel = this.ShowDataSourceDialog(context.DataSetType == DataSetType.SampleDataSet ? DataSourceDialogMode.DefineNewSampleData : DataSourceDialogMode.DefineNewDataStore, this.GetDefaultDesignerDataSetName(context));
      if (sourceDialogModel == null)
        return;
      try
      {
        if (sourceDialogModel.IsDefinedInProjectRoot && !this.EnsureSharedDocument())
          return;
        this.AddSampleDataSource(this.viewModel.DefaultView.SampleData.CreateDefaultNewSampleDataSource(context, sourceDialogModel.DataSourceName, sourceDialogModel.IsEnabledAtRuntime), sourceDialogModel.IsDefinedInProjectRoot ? this.SharedHost : this.CurrentDocumentHost);
      }
      catch (Exception ex)
      {
        this.DisplayException(ex, StringTable.DefineNewSampleData);
      }
    }

    public void CreateSampleDataFromXml()
    {
      XmlDataSourceDialogModel sourceDialogModel = this.ShowDataSourceDialog(DataSourceDialogMode.SampleDataFromXml, this.GetDefaultDesignerDataSetName(DataSetContext.SampleData));
      if (sourceDialogModel == null)
        return;
      string normalizedDataSourceUrl = sourceDialogModel.NormalizedDataSourceUrl;
      if (!sourceDialogModel.IsNameModified)
      {
        string withoutExtension = Path.GetFileNameWithoutExtension(normalizedDataSourceUrl);
        if (!string.IsNullOrEmpty(withoutExtension))
          sourceDialogModel.DataSourceName = withoutExtension;
      }
      try
      {
        if (sourceDialogModel.IsDefinedInProjectRoot && !this.EnsureSharedDocument())
          return;
        this.AddSampleDataSource(this.viewModel.DefaultView.SampleData.ImportSampleDataFromXmlFile(sourceDialogModel.DataSourceName, sourceDialogModel.IsEnabledAtRuntime, normalizedDataSourceUrl), sourceDialogModel.IsDefinedInProjectRoot ? this.SharedHost : this.CurrentDocumentHost);
      }
      catch (Exception ex)
      {
        this.DisplayException(ex, StringTable.ImportSampleDataFromXMLSource);
      }
    }

    public void ReimportSampleDataFromXml(SampleDataSet dataSet)
    {
      try
      {
        string path = this.xmlDataSourceFolderManager.Path;
        string xmlFileName = XmlDataSourceDialogModel.BrowseForXmlFile(StringTable.ReImportSampleDataFromXmlDialogTitle, ref path);
        this.xmlDataSourceFolderManager.Path = path;
        if (string.IsNullOrEmpty(xmlFileName))
          return;
        dataSet.ImportFromXml(xmlFileName, true);
      }
      catch (Exception ex)
      {
        string shortApplicationName = this.DesignerContext.ExpressionInformationService.ShortApplicationName;
        this.DesignerContext.MessageDisplayService.ShowError(new ErrorArgs()
        {
          Message = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.ImportSampleDataFromXMLSourceErrorMessage, new object[1]
          {
            (object) shortApplicationName
          }),
          Exception = ex
        });
      }
    }

    public void AddDataStoreDataSource(SampleDataSet dataSet, bool sharedDocument)
    {
      if (sharedDocument)
        this.EnsureSharedDocument();
      this.AddSampleDataSource(dataSet, sharedDocument ? this.SharedHost : this.CurrentDocumentHost);
    }

    private void AddSampleDataSource(SampleDataSet dataSet, DataHostItem dataHost)
    {
      if (dataSet == null)
        return;
      SceneNode sceneNode = this.viewModel.GetSceneNode((DocumentNode) this.viewModel.Document.DocumentContext.CreateNode(dataSet.RootType.DesignTimeType));
      this.AddDataSourceToHost(dataHost, dataSet.Name, sceneNode, StringTable.UndoUnitCreateClrObjectDataSource);
    }

    public void AddXmlDataSource(XmlDataProviderSceneNode dataSource, DataHostItem dataHost, string dataSourceName)
    {
      if (dataSource == null)
        return;
      string errorMessage = string.Empty;
      ISchema schemaForDataSource = SchemaManager.GetSchemaForDataSource(dataSource.DocumentNode, out errorMessage);
      if (errorMessage.Length > 0)
      {
        this.DesignerContext.MessageDisplayService.ShowError(errorMessage);
      }
      else
      {
        if (dataSourceName == DataPanelModel.DefaultXmlDataSourceName && schemaForDataSource.Root != null)
          dataSourceName = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.DataSourceNameFormatString, new object[1]
          {
            (object) schemaForDataSource.Root.PathName
          });
        XmlSchema xmlSchema = schemaForDataSource as XmlSchema;
        if (xmlSchema != null && xmlSchema.NamespaceManager != null && xmlSchema.NamespaceManager.Count > 0)
          dataSource.XmlNamespaceManager = (XmlNamespaceManager) xmlSchema.NamespaceManager;
        this.AddDataSourceToHost(dataHost, dataSourceName, (SceneNode) dataSource, StringTable.UndoUnitCreateXmlDataSource);
      }
    }

    public void RenameSampleDataSchemaItemUponRebuild(DataSourceNode dataSourceNode, string schemaNodePath)
    {
      this.pendingRenameSchemaItemNodePath = schemaNodePath;
      this.pendingRenameSchemaItemDataSource = dataSourceNode.DocumentNode;
    }

    public void ExtendSelectionUponRebuild(DataSourceNode dataSourceNode, string schemaNodePath)
    {
      this.pendingSelectedSchemaItemNodePath = schemaNodePath;
      this.pendingSelectedSchemaItemDataSource = dataSourceNode.DocumentNode;
    }

    private void ViewModel_LateSceneUpdatePhase(object sender, SceneUpdatePhaseEventArgs args)
    {
      if (this.DocumentHost != null)
      {
        this.EnsureDocumentSubscriptionUpToDate(args.DocumentChanges, args.DocumentChangeStamp);
        this.DocumentHost.UpdateDocumentHasErrors();
      }
      bool flag = args.IsDirtyViewState(SceneViewModel.ViewStateBits.IsEditable | SceneViewModel.ViewStateBits.ActiveEditingContainer | SceneViewModel.ViewStateBits.ElementSelection);
      if (!flag)
      {
        foreach (DocumentNodeChange documentNodeChange in args.DocumentChanges.DistinctChanges)
        {
          if (documentNodeChange.IsPropertyChange && (BaseFrameworkElement.DataContextProperty.Equals((object) documentNodeChange.PropertyKey) || DesignTimeProperties.DesignDataContextProperty.Equals((object) documentNodeChange.PropertyKey) || DataTemplateElement.DataTypeProperty.Equals((object) documentNodeChange.PropertyKey)))
          {
            flag = true;
            break;
          }
        }
      }
      if (flag)
        this.UpdateActveDataContext();
      this.UpdatePendingSelectedDataSource();
      this.UpdatePendingSelectedSchemaItem();
      this.UpdatePendingSchemaItemRename();
    }

    private void SaveOldDataContextExpansion()
    {
      if (!this.HasActiveDataContext || this.activeDataContext.Root == null)
        return;
      this.CacheExpandedDescendants(this.activeDataContext.Root, this.dataContextExpansion);
    }

    private void CacheExpandedDescendants(DataSchemaItem dataSchemaItem, Dictionary<string, bool> isExpanded)
    {
      if (!dataSchemaItem.IsExpanded)
        return;
      string index = dataSchemaItem.SampleDataSet != null ? this.CalculateNodePathWithRenames(dataSchemaItem.SampleDataSet, dataSchemaItem.DataSchemaNodePath) : dataSchemaItem.DataSchemaNodePath.AbsolutePath.Path;
      isExpanded[index] = dataSchemaItem.IsExpanded;
      foreach (DataSchemaItem dataSchemaItem1 in Enumerable.OfType<DataSchemaItem>((IEnumerable) dataSchemaItem.Children))
        this.CacheExpandedDescendants(dataSchemaItem1, isExpanded);
    }

    private string CalculateNodePathWithRenames(SampleDataSet sampleDataSet, DataSchemaNodePath nodePath)
    {
      Dictionary<SampleCompositeType, Dictionary<string, string>> renamesForSampleDataSet;
      if (!this.pendingRenamesBySampleDataSet.TryGetValue(sampleDataSet, out renamesForSampleDataSet))
        return nodePath.AbsolutePath.Path;
      if (nodePath == null || nodePath.Node == nodePath.Schema.Root)
        return string.Empty;
      DataSchemaNode node = nodePath.Node;
      bool isCollectionItem = node.IsCollectionItem;
      string str = this.GetPathNameForNode(renamesForSampleDataSet, node);
      for (DataSchemaNode parent = node.Parent; parent != nodePath.Schema.Root && parent != null; parent = parent.Parent)
      {
        str = isCollectionItem ? parent.PathName + str : this.GetPathNameForNode(renamesForSampleDataSet, parent) + "." + str;
        isCollectionItem = parent.IsCollectionItem;
      }
      return str;
    }

    private string GetPathNameForNode(Dictionary<SampleCompositeType, Dictionary<string, string>> renamesForSampleDataSet, DataSchemaNode node)
    {
      SampleCompositeType effectiveParentType = node.EffectiveParentType;
      Dictionary<string, string> dictionary;
      string str;
      return effectiveParentType == null || !renamesForSampleDataSet.TryGetValue(effectiveParentType, out dictionary) || !dictionary.TryGetValue(node.PathName, out str) ? node.PathName : str;
    }

    private void UpdateExpandedDescendants(DataSchemaItem dataSchemaItem, Dictionary<string, bool> isExpanded)
    {
      string path = dataSchemaItem.DataSchemaNodePath.AbsolutePath.Path;
      if (!isExpanded.ContainsKey(path))
        return;
      dataSchemaItem.IsExpanded = isExpanded[path];
      if (!dataSchemaItem.IsExpanded)
        return;
      foreach (DataSchemaItem dataSchemaItem1 in Enumerable.OfType<DataSchemaItem>((IEnumerable) dataSchemaItem.Children))
        this.UpdateExpandedDescendants(dataSchemaItem1, isExpanded);
    }

    private void UpdatePendingSchemaItemRename()
    {
      DataSchemaItem schemaItem = this.FindSchemaItem(this.pendingRenameSchemaItemDataSource, this.pendingRenameSchemaItemNodePath);
      if (schemaItem != null)
        schemaItem.IsPendingRename = true;
      this.pendingRenameSchemaItemNodePath = (string) null;
      this.pendingRenameSchemaItemDataSource = (DocumentNode) null;
    }

    private void UpdatePendingSelectedSchemaItem()
    {
      DataSchemaItem schemaItem = this.FindSchemaItem(this.pendingSelectedSchemaItemDataSource, this.pendingSelectedSchemaItemNodePath);
      if (schemaItem != null)
      {
        schemaItem.ExpandAncestors();
        this.SelectionContext.Add((DataModelItemBase) schemaItem);
      }
      this.pendingSelectedSchemaItemNodePath = (string) null;
      this.pendingSelectedSchemaItemDataSource = (DocumentNode) null;
    }

    private void UpdatePendingSelectedDataSource()
    {
      if (this.pendingSelectedDataSource == null)
        return;
      DataSourceItem dataSource = this.FindDataSource(this.pendingSelectedDataSource);
      if (dataSource != null)
      {
        this.selectionContext.SetSelection((DataModelItemBase) dataSource);
        if (dataSource.SchemaItem != null && dataSource.SchemaItem.Root != null)
        {
          DataSchemaItem root = dataSource.SchemaItem.Root;
          if (!root.IsSampleBasicType)
          {
            root.IsExpanded = true;
            foreach (DataSchemaItem dataSchemaItem in Enumerable.OfType<DataSchemaItem>((IEnumerable) root.Children))
            {
              if (!dataSchemaItem.IsSampleBasicType)
                dataSchemaItem.IsExpanded = true;
            }
          }
        }
      }
      this.pendingSelectedDataSource = (DocumentNode) null;
    }

    private DataSchemaItem FindSchemaItem(DocumentNode dataSource, string nodePath)
    {
      if (nodePath != null && dataSource != null)
      {
        DataSourceItem dataSourceItem = Enumerable.FirstOrDefault<DataSourceItem>(this.DataSources, (Func<DataSourceItem, bool>) (item => DesignDataHelper.CompareDataSources(item.DataSourceNode.DocumentNode, dataSource)));
        if (dataSourceItem != null)
          return dataSourceItem.SchemaItem.GetItemFromPath(nodePath);
      }
      return (DataSchemaItem) null;
    }

    private void SetActiveDataContext(ISchema schema)
    {
      if (this.activeDataContext == null && schema == null || this.activeDataContext != null && schema != null && this.activeDataContext.Schema.Root == schema.Root)
        return;
      if (this.HasActiveDataContext)
        this.SaveOldDataContextExpansion();
      if (schema != null)
      {
        SchemaItem schemaItem = new SchemaItem(schema, this, this.activeDataContextSelectionContext);
        this.UpdateExpandedDescendants(schemaItem.Root, this.dataContextExpansion);
        if (this.activeDataContext != schemaItem)
          this.activeDataContext = schemaItem;
      }
      else if (this.activeDataContext != null)
        this.activeDataContext = (SchemaItem) null;
      this.OnPropertyChanged("HasActiveDataContext");
      this.OnPropertyChanged("ActiveDataContext");
    }

    private void UpdateActveDataContext()
    {
      SceneNode target1 = (SceneNode) null;
      if (this.viewModel.ElementSelectionSet != null && this.viewModel.ElementSelectionSet.Count > 0)
        target1 = (SceneNode) this.viewModel.ElementSelectionSet.Selection[0];
      else if (this.viewModel.DependencyObjectSelectionSet != null && this.viewModel.DependencyObjectSelectionSet.Count > 0)
        target1 = this.viewModel.DependencyObjectSelectionSet.Selection[0];
      if (target1 == null)
        target1 = (SceneNode) this.viewModel.FindPanelClosestToActiveEditingContainer() ?? this.viewModel.ActiveEditingContainer;
      if (target1 is DataTemplateElement)
        target1 = target1.GetLocalValueAsSceneNode(FrameworkTemplateElement.VisualTreeProperty);
      else if (target1 is ControlTemplateElement)
        target1 = target1.GetLocalValueAsSceneNode(FrameworkTemplateElement.VisualTreeProperty);
      if (target1 == null)
        return;
      DataContextInfo target2 = new DataContextEvaluator().Evaluate(target1);
      if (target2 != null)
      {
        for (int index = 1; index < this.viewModel.ElementSelectionSet.Count; ++index)
        {
          DataContextInfo dataContextInfo = new DataContextEvaluator().Evaluate((SceneNode) this.viewModel.ElementSelectionSet.Selection[index]);
          if (dataContextInfo == null || dataContextInfo.Owner != target2.Owner)
            return;
        }
      }
      this.SetActiveDataContext(target2);
    }

    private void SetActiveDataContext(DataContextInfo target)
    {
      DocumentNode targetDataSource = (DocumentNode) null;
      ISchema schema = (ISchema) null;
      if (target != null && target.Owner != null && (target.Property != null && target.DataSource != null) && target.DataSource.IsValidWithSource)
      {
        ISchema schemaForDataSource = SchemaManager.GetSchemaForDataSource(target.RawDataSource.SourceNode);
        DataSchemaNodePath nodePathFromPath = schemaForDataSource.GetNodePathFromPath(target.DataSource.Path);
        if (nodePathFromPath != null)
        {
          targetDataSource = DesignDataHelper.GetRootDesignDataNode(target.DataSource.SourceNode) ?? target.DataSource.SourceNode;
          DataSchemaNode node = nodePathFromPath.Node;
          if (node.IsCollectionItem)
          {
            while (!node.IsCollection)
              node = node.Parent;
          }
          schema = schemaForDataSource.MakeRelativeToNode(node);
        }
        else
          schema = (ISchema) null;
      }
      if (targetDataSource != null)
      {
        DataSourceItem dataSource = this.FindDataSource(targetDataSource);
        if (dataSource != null)
        {
          DataSchemaItem dataSchemaItem1 = dataSource.SchemaItem.Root;
          if (schema != null && schema.Root != null)
          {
            DataSchemaItem dataSchemaItem2 = dataSchemaItem1;
            bool flag = true;
            while (flag)
            {
              flag = false;
              foreach (DataSchemaItem dataSchemaItem3 in Enumerable.OfType<DataSchemaItem>((IEnumerable) dataSchemaItem2.Children))
              {
                if (dataSchemaItem3.DataSchemaNode.IsAncestorOf(schema.Root))
                {
                  dataSchemaItem2.IsExpanded = true;
                  dataSchemaItem2 = dataSchemaItem3;
                  flag = true;
                  break;
                }
              }
            }
            dataSchemaItem1 = dataSchemaItem2;
          }
          this.ActiveDataSourceAndSchema = new KeyValuePair<DataSourceItem, DataSchemaItem>?(new KeyValuePair<DataSourceItem, DataSchemaItem>(dataSource, dataSchemaItem1));
        }
      }
      else
        this.ActiveDataSourceAndSchema = new KeyValuePair<DataSourceItem, DataSchemaItem>?();
      this.SetActiveDataContext(schema);
    }

    private DataSourceItem FindDataSource(DocumentNode targetDataSource)
    {
      return Enumerable.FirstOrDefault<DataSourceItem>(this.DataSources, (Func<DataSourceItem, bool>) (item =>
      {
        if (item.DataSourceNode != null)
          return DesignDataHelper.CompareDataSources(targetDataSource, item.DataSourceNode.DocumentNode);
        return false;
      }));
    }

    private void InitializeDocumentSubscription()
    {
      this.basisDocumentSubscription = new SceneNodeSubscription<object, object>();
      this.basisDocumentSubscription.Path = new SearchPath(new SearchStep[1]
      {
        new SearchStep(SearchAxis.Root)
      });
      this.documentSubscription = new SceneNodeSubscription<DataHostItem, DataSourceItem>();
      this.documentSubscription.Path = new SearchPath(new SearchStep[1]
      {
        new SearchStep((SearchAxis) new DelegateAxis(new DelegateAxis.EnumerationHandler(BindingEditor.DataSourceItemEnumerator), SearchScope.NodeTreeDescendant))
      });
      this.documentSubscription.SetBasisNodeInsertedHandler(new SceneNodeSubscription<DataHostItem, DataSourceItem>.BasisNodeInsertedHandler(this.DocumentHostSubscription_DataHostItemInsertedHandler));
      this.documentSubscription.BasisNodeRemoved += new SceneNodeSubscription<DataHostItem, DataSourceItem>.BasisNodeRemovedListener(this.DocumentHostSubscription_DataHostItemRemoved);
      this.documentSubscription.SetPathNodeInsertedHandler(new SceneNodeSubscription<DataHostItem, DataSourceItem>.PathNodeInsertedHandler(this.DocumentHostSubscription_ResourceDataSourceInsertedHandler));
      this.documentSubscription.PathNodeRemoved += new SceneNodeSubscription<DataHostItem, DataSourceItem>.PathNodeRemovedListener(this.DocumentHostSubscription_ResourceDataSourceRemoved);
      this.documentSubscription.PathNodeContentChanged += new SceneNodeSubscription<DataHostItem, DataSourceItem>.PathNodeContentChangedListener(this.DocumentHostSubscription_ResourceDataSourceContentChanged);
    }

    public void EnsureDocumentSubscriptionUpToDate(DocumentNodeChangeList documentChanges, uint documentChangeStamp)
    {
      if (this.basisDocumentSubscription == null)
        this.InitializeDocumentSubscription();
      List<SceneNode> list = new List<SceneNode>(1);
      if (this.viewModel.RootNode is BaseFrameworkElement)
        list.Add(this.viewModel.RootNode);
      this.basisDocumentSubscription.SetSceneRootNodeAsTheBasisNode(this.viewModel);
      this.basisDocumentSubscription.Update(this.viewModel, documentChanges, documentChangeStamp);
      this.documentSubscription.ChainUpdate(this.viewModel, this.basisDocumentSubscription.PathNodeList, (IEnumerable<SceneNode>) list, documentChanges, documentChangeStamp);
      this.OnPropertyChanged("IsActive");
    }

    private DataHostItem DocumentHostSubscription_DataHostItemInsertedHandler(object sender, SceneNode newBasisNode)
    {
      this.DocumentHost.RefreshDataSource(new DataSourceHost(newBasisNode));
      this.OnPropertyChanged("DataSources");
      return this.DocumentHost;
    }

    private void DocumentHostSubscription_DataHostItemRemoved(object sender, SceneNode oldBasisNode, DataHostItem oldDataSourceHost)
    {
      if (this.DocumentHost == null || this.DocumentHost.DataSourceHost.DocumentNode != oldBasisNode.DocumentNode)
        return;
      this.DocumentHost.ClearDataSourceHost();
      this.OnPropertyChanged("DataSources");
    }

    private DataSourceItem DocumentHostSubscription_ResourceDataSourceInsertedHandler(object sender, SceneNode basisNode, DataHostItem basisHost, SceneNode newPathNode)
    {
      DataSourceItem dataSourceItem = (DataSourceItem) null;
      DocumentCompositeNode resourceEntryNode = newPathNode.DocumentNode as DocumentCompositeNode;
      if (resourceEntryNode != null)
      {
        dataSourceItem = this.BuildDataSourceItem(resourceEntryNode);
        DataSourceItem.Comparer comparer = new DataSourceItem.Comparer();
        int index = 0;
        foreach (DataSourceItem y in (IEnumerable) basisHost.DataSources)
        {
          if (comparer.Compare(dataSourceItem, y) >= 0)
            ++index;
          else
            break;
        }
        if (index < basisHost.ResourceDataSources.Count)
          basisHost.ResourceDataSources.InsertDataSource(index, dataSourceItem);
        else
          basisHost.ResourceDataSources.AddDataSource(dataSourceItem);
        this.OnPropertyChanged("DataSources");
      }
      return dataSourceItem;
    }

    private void DocumentHostSubscription_ResourceDataSourceRemoved(object sender, SceneNode basisNode, DataHostItem basisHost, SceneNode oldPathNode, DataSourceItem oldDataSource)
    {
      this.RemoveResourceDataSourceInternal(basisHost, oldDataSource);
      this.OnPropertyChanged("DataSources");
    }

    private void DocumentHostSubscription_ResourceDataSourceContentChanged(object sender, SceneNode pathNode, DataSourceItem item, DocumentNodeMarker damageMarker, DocumentNodeChange damage)
    {
      if (!damage.IsPropertyChange)
        return;
      DictionaryEntryNode dictionaryEntryNode = pathNode as DictionaryEntryNode;
      if (dictionaryEntryNode == null)
        return;
      if (dictionaryEntryNode.DocumentNode is DocumentCompositeNode && dictionaryEntryNode.Value != null && (damage.ParentNode == dictionaryEntryNode.Value.DocumentNode && DesignTimeProperties.SampleDataTagProperty.Equals((object) damage.PropertyKey)))
        this.RefreshSchemaIfNeeded(item, false);
      bool flag = DictionaryEntryNode.KeyProperty.Equals((object) damage.PropertyKey);
      if (!flag && damage.ParentNode != null && (damage.ParentNode == dictionaryEntryNode.Value.DocumentNode && this.ProjectContext.IsCapabilitySet(PlatformCapability.NameSupportedAsKey)) && damage.PropertyKey == damage.ParentNode.NameProperty)
        flag = true;
      if (!flag)
        return;
      item.OnDisplayNameChanged();
    }

    private void SchemaManager_ClrObjectSchemasChanged(object sender, EventArgs e)
    {
      foreach (DataHostItem dataHostItem in (Collection<DataHostItem>) this.dataHosts)
      {
        dataHostItem.UpdateDocumentHasErrors();
        if (!dataHostItem.DocumentHasErrors)
        {
          foreach (DataSourceItem dataSourceItem in (Collection<DataSourceItem>) dataHostItem.ResourceDataSources)
            this.RefreshSchemaIfNeeded(dataSourceItem, false);
        }
        if (dataHostItem.FileDataSources.Count > 0)
        {
          foreach (DataSourceItem dataSourceItem in new List<FileBasedDataSourceItem>((IEnumerable<FileBasedDataSourceItem>) dataHostItem.FileDataSources))
            this.RefreshSchemaIfNeeded(dataSourceItem, false);
        }
      }
      this.UpdatePendingSelectedSchemaItem();
      this.UpdatePendingSchemaItemRename();
      this.pendingRenamesBySampleDataSet.Clear();
    }

    private void Shared_ItemsChanged(object sender, CollectionChangeEventArgs e)
    {
      ResourceDictionaryContentProvider dictionaryContentProvider = e.Element as ResourceDictionaryContentProvider;
      if (dictionaryContentProvider == null)
        return;
      XamlDocument xamlDocument = this.SharedDocument != null ? (XamlDocument) this.SharedDocument.XamlDocument : (XamlDocument) null;
      if (dictionaryContentProvider.Document != xamlDocument)
      {
        this.shouldRefreshSharedDocument = true;
      }
      else
      {
        this.UpdateSharedDataHost();
        this.UpdatePendingSelectedDataSource();
      }
    }

    private void Shared_KeyChanged(object sender, EventArgs<DocumentNode> e)
    {
      foreach (DataSourceItem dataSourceItem in (Collection<DataSourceItem>) this.SharedResourceDataSources)
      {
        DocumentNode documentNode = dataSourceItem.DataSourceNode == null || dataSourceItem.DataSourceNode.DocumentNode == null ? (DocumentNode) null : (DocumentNode) dataSourceItem.DataSourceNode.DocumentNode.Parent;
        if (documentNode != null && documentNode == e.Value)
        {
          dataSourceItem.OnDisplayNameChanged();
          break;
        }
      }
    }

    private void UpdateSharedDataHost()
    {
      List<DocumentNode> list1 = new List<DocumentNode>(this.sharedContentProvider.Items);
      list1.RemoveAll((Predicate<DocumentNode>) (target =>
      {
        DocumentCompositeNode documentCompositeNode = target as DocumentCompositeNode;
        if (documentCompositeNode != null && PlatformTypes.DictionaryEntry.Equals((object) documentCompositeNode.Type))
          return !BindingEditor.IsDataSource(documentCompositeNode.Properties[DictionaryEntryNode.ValueProperty]);
        return true;
      }));
      List<DataSourceItem> list2 = new List<DataSourceItem>((IEnumerable<DataSourceItem>) this.SharedResourceDataSources);
      this.SharedHost.DataSourceHost.DocumentNode = this.sharedContentProvider.Document != null ? this.sharedContentProvider.Document.RootNode : (DocumentNode) null;
      bool flag = list2.Count == list1.Count;
      for (int index = 0; flag && index < list1.Count; ++index)
      {
        DocumentNode documentNode = ((DocumentCompositeNode) list1[index]).Properties[DictionaryEntryNode.ValueProperty];
        flag = flag && list2[index].DataSourceNode.DocumentNode == documentNode;
      }
      if (!flag)
      {
        this.SharedResourceDataSources.ClearDataSources();
        foreach (DocumentNode documentNode in list1)
        {
          DocumentCompositeNode resourceEntryNode = documentNode as DocumentCompositeNode;
          if (resourceEntryNode != null)
            this.SharedResourceDataSources.AddDataSource(this.BuildDataSourceItem(resourceEntryNode));
        }
        this.OnPropertyChanged("DataSources");
      }
      this.SharedHost.UpdateDocumentHasErrors();
    }

    private void DocumentRoot_ModelChanged(object sender, EventArgs e)
    {
      this.UpdateThisDocumentDataHost();
    }

    private void UpdateThisDocumentDataHost()
    {
      if (this.DocumentHost == null)
        return;
      List<DataSourceItem> list = new List<DataSourceItem>((IEnumerable<DataSourceItem>) this.DocumentHost.ResourceDataSources);
      for (int index = 0; index < list.Count; ++index)
        this.RefreshSchemaIfNeeded(list[index], false);
      this.DocumentHost.UpdateDocumentHasErrors();
    }

    private void RefreshSchemaIfNeeded(DataSourceItem item, bool force)
    {
      if (!force && !this.IsActiveViewModel)
      {
        if (this.delayedRefreshDataSources.Contains(item))
          return;
        this.delayedRefreshDataSources.Add(item);
      }
      else
      {
        FileBasedDataSourceItem basedDataSourceItem = item as FileBasedDataSourceItem;
        ISchema schema = basedDataSourceItem == null ? this.ProvideSchemaForDataSource(item.DataSourceNode.DocumentNode) : this.ProvideSchemaForDesingDataFile(basedDataSourceItem.DesignDataFile);
        if (schema != item.SchemaItem.Schema)
        {
          DataSchemaItem dataSchemaItem = Enumerable.FirstOrDefault<DataSchemaItem>(Enumerable.OfType<DataSchemaItem>((IEnumerable) this.SelectionContext), (Func<DataSchemaItem, bool>) (i => i.DataSourceNode == item.DataSourceNode));
          string path = (string) null;
          if (dataSchemaItem != null)
          {
            path = dataSchemaItem.SampleDataSet != null ? this.CalculateNodePathWithRenames(dataSchemaItem.SampleDataSet, dataSchemaItem.DataSchemaNodePath) : dataSchemaItem.DataSchemaNodePath.Path;
            this.SelectionContext.Remove((DataModelItemBase) dataSchemaItem);
          }
          item = this.RefreshSchema(item, schema);
          if (path != null)
          {
            DataSchemaNodePath nodePathFromPath = item.SchemaItem.Schema.GetNodePathFromPath(path);
            if (nodePathFromPath != null)
            {
              DataSchemaItem schemaItemForNode = item.SchemaItem.FindDataSchemaItemForNode(nodePathFromPath);
              if (schemaItemForNode != null)
                this.SelectionContext.Add((DataModelItemBase) schemaItemForNode);
            }
          }
          if (this.ActiveDataSourceAndSchema.HasValue && this.ActiveDataSourceAndSchema.Value.Key == item)
            this.UpdateActveDataContext();
        }
        item.OnDisplayNameChanged();
      }
    }

    private DataSourceItem RefreshSchema(DataSourceItem item, ISchema schema)
    {
      Dictionary<string, bool> isExpanded = new Dictionary<string, bool>();
      if (item.SchemaItem.Root != null)
        this.CacheExpandedDescendants(item.SchemaItem.Root, isExpanded);
      FileBasedDataSourceItem dataSource = item as FileBasedDataSourceItem;
      if (dataSource != null)
      {
        this.SharedFileDataSources.RemoveDataSource(dataSource);
        item = (DataSourceItem) this.AddDesignDataSchema(dataSource.DesignDataFile, false);
      }
      else
        item.RefreshSchema(schema);
      if (item.SchemaItem.Root != null)
        this.UpdateExpandedDescendants(item.SchemaItem.Root, isExpanded);
      return item;
    }

    private DataSourceItem BuildDataSourceItem(DocumentCompositeNode resourceEntryNode)
    {
      DataSourceItem dataSourceItem = (DataSourceItem) null;
      DocumentNode dataSourceNode1 = resourceEntryNode.Properties[DictionaryEntryNode.ValueProperty];
      if (dataSourceNode1 != null)
      {
        dataSourceItem = new DataSourceItem(this.ProvideSchemaForDataSource(dataSourceNode1), this);
        DataSourceNode dataSourceNode2 = dataSourceItem.DataSourceNode;
      }
      return dataSourceItem;
    }

    private void RemoveResourceDataSourceInternal(DataHostItem basisHost, DataSourceItem dataSource)
    {
      this.selectionContext.Remove((DataModelItemBase) dataSource);
      this.delayedRefreshDataSources.Remove(dataSource);
      basisHost.ResourceDataSources.RemoveDataSource(dataSource);
    }

    private void SchemaManager_SampleTypesChanging(SampleDataSet sender, EventArgs e)
    {
      Dictionary<SampleCompositeType, Dictionary<string, string>> dictionary1 = new Dictionary<SampleCompositeType, Dictionary<string, string>>();
      foreach (SampleDataChange sampleDataChange in sender.Changes)
      {
        SamplePropertyRenamed samplePropertyRenamed = sampleDataChange as SamplePropertyRenamed;
        if (samplePropertyRenamed != null)
        {
          SampleCompositeType declaringSampleType = samplePropertyRenamed.SampleProperty.DeclaringSampleType;
          Dictionary<string, string> dictionary2;
          if (!dictionary1.TryGetValue(declaringSampleType, out dictionary2))
          {
            dictionary2 = new Dictionary<string, string>();
            dictionary1.Add(declaringSampleType, dictionary2);
          }
          dictionary2.Add(samplePropertyRenamed.OldName, samplePropertyRenamed.NewName);
        }
      }
      if (dictionary1.Count <= 0)
        return;
      this.pendingRenamesBySampleDataSet.Add(sender, dictionary1);
    }

    private ISchema ProvideSchemaForDataSource(DocumentNode dataSourceNode)
    {
      return SchemaManager.GetSchemaForDataSource(dataSourceNode);
    }

    private ISchema ProvideSchemaForDesingDataFile(IProjectItem designDataFile)
    {
      return this.SchemaManager.GetSchemaForDesignDataFile(designDataFile);
    }
  }
}
