// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.ResourcePane.ResourcePane
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Documents.Commands;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.SceneCommands;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Clipboard;
using Microsoft.Expression.Framework.Data;
using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.Extensions.Enumerable;
using Microsoft.Expression.Framework.UserInterface;
using Microsoft.Expression.Framework.Workspaces.Extension;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.UserInterface;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;

namespace Microsoft.Expression.DesignSurface.UserInterface.ResourcePane
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class ResourcePane : Border, INotifyPropertyChanged, IComponentConnector
  {
    private DesignerContext designerContext;
    private IProjectManager projectManager;
    private ResourceManager resourceManager;
    private ObservableCollectionAggregator resourceContainers;
    private bool resourceContainersEnabled;
    private Dictionary<ResourceItem, ResourceValueModel> valueModels;
    private DelegateCommand deleteCommand;
    private DelegateCommand viewXamlCommand;
    private DelegateCommand addNewResourceDictionaryCommand;
    private ActiveDocumentWrapper activeDocumentWrapper;
    private ObservableCollection<ActiveDocumentWrapper> wrapperList;
    private bool initialized;
    internal DragDropScrollViewer ResourceScrollViewer;
    internal ItemsControl ResourceItemSelector;
    private bool _contentLoaded;

    private DesignerContext DesignerContext
    {
      get
      {
        return this.designerContext;
      }
    }

    public IList ResourceContainers
    {
      get
      {
        return (IList) this.resourceContainers;
      }
    }

    public ICommand DeleteCommand
    {
      get
      {
        return (ICommand) this.deleteCommand;
      }
    }

    public ICommand ViewXamlCommand
    {
      get
      {
        return (ICommand) this.viewXamlCommand;
      }
    }

    public bool CanDelete
    {
      get
      {
        if (this.resourceManager.SelectedItems.Count > 0)
          return this.resourceManager.SelectedResourceItems.Count == this.resourceManager.SelectedItems.Count;
        return false;
      }
    }

    public bool HasTargetProject
    {
      get
      {
        if (this.resourceManager.TargetProjects != null)
          return Enumerable.Any<IProject>(this.resourceManager.TargetProjects);
        return false;
      }
    }

    public bool IsPaneEnabled
    {
      get
      {
        if (this.resourceManager.ActiveSceneViewModel != null)
          return this.resourceManager.ActiveSceneViewModel.DefaultView.IsDesignSurfaceVisible;
        return false;
      }
    }

    public bool IsEnabledNewDictionary
    {
      get
      {
        if (this.IsPaneEnabled)
          return this.HasTargetProject;
        return false;
      }
    }

    public bool IsFiltering
    {
      get
      {
        return this.resourceManager.IsFiltering;
      }
      set
      {
        this.resourceManager.IsFiltering = value;
        this.RefreshResourceContainers();
        this.OnPropertyChanged("IsFiltering");
      }
    }

    public ICommand AddNewResourceDictionaryCommand
    {
      get
      {
        return (ICommand) this.addNewResourceDictionaryCommand;
      }
    }

    public ICommand CutCommand
    {
      get
      {
        return (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.CutResource))
        {
          IsEnabled = (this.resourceManager.SelectedResourceContainers.Count == 0 && this.resourceManager.SelectedResourceItems.Count == 1)
        };
      }
    }

    public ICommand CopyCommand
    {
      get
      {
        return (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.CopyResource))
        {
          IsEnabled = (this.resourceManager.SelectedResourceContainers.Count == 0 && this.resourceManager.SelectedResourceItems.Count == 1)
        };
      }
    }

    public ICommand PasteCommand
    {
      get
      {
        DelegateCommand delegateCommand = new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.PasteResource));
        bool flag = false;
        if (this.resourceManager.SelectedItems.Count == 1)
        {
          ResourceEntryBase resourceEntryBase = this.resourceManager.SelectedItems.Selection[0];
          if (resourceEntryBase is NodeResourceContainer || resourceEntryBase is DocumentResourceContainer || resourceEntryBase is ResourceItem)
            flag = resourceEntryBase.DocumentNode != null && resourceEntryBase.DocumentNode.DocumentRoot != null && resourceEntryBase.DocumentNode.DocumentRoot.IsEditable;
        }
        if (flag)
        {
          SafeDataObject safeDataObject = SafeDataObject.FromClipboard();
          if (safeDataObject != null)
            flag = safeDataObject.GetDataPresent(Microsoft.Expression.Framework.Clipboard.Container.DataFormat.Name) || safeDataObject.GetDataPresent(DataFormats.Bitmap) || safeDataObject.GetDataPresent(DataFormats.FileDrop);
        }
        delegateCommand.IsEnabled = flag;
        return (ICommand) delegateCommand;
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    internal ResourcePane(DesignerContext designerContext, IProjectManager projectManager, ResourceManager resourceManager)
    {
      this.designerContext = designerContext;
      this.projectManager = projectManager;
      this.resourceManager = resourceManager;
      this.valueModels = new Dictionary<ResourceItem, ResourceValueModel>();
      this.activeDocumentWrapper = new ActiveDocumentWrapper(this.resourceManager, designerContext.ActiveSceneViewModel);
      this.wrapperList = new ObservableCollection<ActiveDocumentWrapper>();
      this.wrapperList.Add(this.activeDocumentWrapper);
      this.resourceContainers = new ObservableCollectionAggregator();
      this.resourceManager.ContainerRemoved += new EventHandler<EventArgs<ResourceContainer>>(this.ResourceManager_ContainerRemoved);
      this.resourceManager.ItemRemoved += new EventHandler<EventArgs<ResourceItem>>(this.ResourceManager_ItemRemoved);
      this.resourceManager.SelectedItems.Changed += new EventHandler(this.ResourceManager_SelectedItemsChanged);
      this.resourceManager.TargetProjectsChanged += new EventHandler<EventArgs>(this.ResourceManager_TargetProjectsChanged);
      this.designerContext.ViewService.ActiveViewChanged += new ViewChangedEventHandler(this.ViewService_ActiveViewChanged);
      this.designerContext.WorkspaceService.ActiveWorkspaceChanged += new EventHandler(this.WorkspaceService_ActiveWorkspaceChanged);
      this.designerContext.SelectionManager.LateActiveSceneUpdatePhase += new SceneUpdatePhaseEventHandler(this.SelectionManager_LateActiveSceneUpdatePhase);
      this.designerContext.PlatformContextChanger.HostPlatformRestarted += new EventHandler(this.PlatformContextChanger_HostPlatformRestarted);
      this.ContextMenu = new ContextMenu();
      this.deleteCommand = new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.OnDeleteCommand));
      this.deleteCommand.IsEnabled = false;
      this.viewXamlCommand = new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.OnViewXamlCommand));
      this.viewXamlCommand.IsEnabled = false;
      this.addNewResourceDictionaryCommand = new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.OnAddNewResourceDictionaryCommand));
      this.CommandBindings.Add(new CommandBinding((ICommand) PropertyValueEditorCommands.FinishEditing, new ExecutedRoutedEventHandler(this.OnPropertyValueFinishEditingCommand)));
      this.InitializeComponent();
    }

    protected override void OnInitialized(EventArgs e)
    {
      if (!this.initialized)
      {
        this.DataContext = (object) this;
        ((ResourceValueModelProvider) this.FindResource((object) "ResourceValueModelProvider")).ResourcePane = this;
        this.SetValue(PaletteRegistry.PaletteHeaderContentProperty, this.Resources[(object) "PaletteHeaderContent"]);
        this.initialized = true;
      }
      base.OnInitialized(e);
    }

    internal ResourceValueModel GetResourceValueModel(ResourceEntryItem item)
    {
      ResourceValueModel resourceValueModel = (ResourceValueModel) null;
      if (!this.valueModels.TryGetValue((ResourceItem) item, out resourceValueModel) && item.Container.DocumentNode != null)
      {
        resourceValueModel = new ResourceValueModel(item, this.designerContext);
        this.valueModels[(ResourceItem) item] = resourceValueModel;
      }
      return resourceValueModel;
    }

    private void OnPropertyValueFinishEditingCommand(object sender, ExecutedRoutedEventArgs eventArgs)
    {
      this.designerContext.ActiveView.ReturnFocus();
    }

    private void PlatformContextChanger_HostPlatformRestarted(object sender, EventArgs e)
    {
      if (this.valueModels == null)
        return;
      foreach (ResourceValueModel resourceValueModel in this.valueModels.Values)
        resourceValueModel.Recache();
    }

    private void ResourceManager_TargetProjectsChanged(object sender, EventArgs e)
    {
      this.OnPropertyChanged("HasTargetProject");
      this.OnPropertyChanged("IsEnabledNewDictionary");
    }

    private void SelectionManager_LateActiveSceneUpdatePhase(object sender, SceneUpdatePhaseEventArgs args)
    {
      if (!this.IsPaneEnabled)
        return;
      this.activeDocumentWrapper.Update(this.designerContext.ActiveSceneViewModel);
    }

    private void ResourceManager_ItemRemoved(object sender, EventArgs<ResourceItem> e)
    {
      this.OnItemRemoved(e.Value);
    }

    private void ResourceManager_ContainerRemoved(object sender, EventArgs<ResourceContainer> e)
    {
      foreach (ResourceItem resourceItem in (Collection<ResourceItem>) e.Value.ResourceItems)
        this.OnItemRemoved(resourceItem);
    }

    private void ViewService_ActiveViewChanged(object sender, ViewChangedEventArgs e)
    {
      this.activeDocumentWrapper.Update(this.designerContext.ActiveSceneViewModel);
      if (this.activeDocumentWrapper.IsEnabled && !this.wrapperList.Contains(this.activeDocumentWrapper))
        this.wrapperList.Add(this.activeDocumentWrapper);
      else if (!this.activeDocumentWrapper.IsEnabled && this.wrapperList.Contains(this.activeDocumentWrapper))
        this.wrapperList.Remove(this.activeDocumentWrapper);
      if (this.IsPaneEnabled != this.resourceContainersEnabled)
      {
        this.RefreshResourceContainers();
        this.OnPropertyChanged("IsPaneEnabled");
      }
      if (this.DesignerContext.ActiveSceneViewModel != null)
        this.UpdateResourcePaneSupported();
      this.OnPropertyChanged("IsEnabledNewDictionary");
    }

    private void WorkspaceService_ActiveWorkspaceChanged(object sender, EventArgs e)
    {
      this.UpdateResourcePaneSupported();
    }

    private void UpdateResourcePaneSupported()
    {
      IWorkspace activeWorkspace = this.designerContext.WorkspaceService.ActiveWorkspace;
      if (activeWorkspace == null)
        return;
      ExpressionView expressionView = activeWorkspace.FindPalette("Designer_ResourcePane") as ExpressionView;
      if (expressionView == null)
        return;
      expressionView.IsForcedInvisible = false;
    }

    private void ResourceManager_SelectedItemsChanged(object sender, EventArgs e)
    {
      this.deleteCommand.IsEnabled = this.CanDelete;
      List<DocumentNode> itemsDocumentNodes = this.GetSelectedItemsDocumentNodes();
      this.viewXamlCommand.IsEnabled = itemsDocumentNodes != null && itemsDocumentNodes.Count > 0;
    }

    private void OnPropertyChanged(string propertyName)
    {
      if (propertyName != null)
      {
        int length = propertyName.Length;
      }
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }

    private void OnDeleteCommand()
    {
      List<ResourceItem> list = new List<ResourceItem>();
      foreach (ResourceEntryBase resourceEntryBase in this.resourceManager.SelectedItems.Selection)
      {
        ResourceItem resourceItem = resourceEntryBase as ResourceItem;
        if (resourceItem != null)
          list.Add(resourceItem);
      }
      this.InteractiveDelete((ICollection<ResourceItem>) list);
    }

    private void OnAddNewResourceDictionaryCommand()
    {
      new AddNewItemCommand(this.DesignerContext, new string[1]
      {
        "ResourceDictionary.xaml"
      }).ExecuteWithProject(this.DesignerContext.ActiveProject);
    }

    private void OnViewXamlCommand()
    {
      List<DocumentNode> itemsDocumentNodes = this.GetSelectedItemsDocumentNodes();
      if (itemsDocumentNodes == null || itemsDocumentNodes.Count <= 0)
        return;
      IProjectItem projectItem = (IProjectItem) null;
      foreach (IProject project in this.projectManager.CurrentSolution.Projects)
      {
        projectItem = project.FindItem(DocumentReferenceLocator.GetDocumentReference(itemsDocumentNodes[0].Context));
        if (projectItem != null)
          break;
      }
      SceneView sceneView = projectItem.OpenView(true) as SceneView;
      if (sceneView == null)
        return;
      DocumentNode rootNode = itemsDocumentNodes[0].DocumentRoot.RootNode;
      if (itemsDocumentNodes.Count == 1 && itemsDocumentNodes[0] == rootNode)
      {
        sceneView.EnsureXamlEditorVisible();
      }
      else
      {
        if (itemsDocumentNodes.Contains(rootNode))
          itemsDocumentNodes.Remove(rootNode);
        GoToXamlCommand.GoToXaml(sceneView.ViewModel.XamlDocument, itemsDocumentNodes);
      }
    }

    private void OnItemRemoved(ResourceItem item)
    {
      ResourceValueModel resourceValueModel;
      if (!this.valueModels.TryGetValue(item, out resourceValueModel))
        return;
      resourceValueModel.OnRemoved();
      this.valueModels.Remove(item);
    }

    protected override void OnContextMenuOpening(ContextMenuEventArgs e)
    {
      base.OnContextMenuOpening(e);
      this.ContextMenu = (ContextMenu) null;
      ReadOnlyCollection<ResourceEntryBase> selection = this.resourceManager.SelectedItems.Selection;
      if (selection.Count <= 0)
        return;
      ContextMenu contextMenu = this.BuildResourceContextMenu(selection);
      contextMenu.IsOpen = true;
      contextMenu.Closed += new RoutedEventHandler(this.RestoreContextMenu);
    }

    private void RefreshResourceContainers()
    {
      this.resourceContainers.Clear();
      this.resourceContainersEnabled = false;
      if (!this.IsPaneEnabled)
        return;
      if (this.resourceManager.IsFiltering)
      {
        this.resourceContainers.AddCollection((IList) this.resourceManager.FilteredResourceContainers);
      }
      else
      {
        this.resourceContainers.AddCollection((IList) this.resourceManager.VisibleDocumentResourceContainers);
        this.resourceContainers.AddCollection((IList) this.wrapperList);
      }
      this.resourceContainersEnabled = true;
    }

    private void RestoreContextMenu(object sender, RoutedEventArgs args)
    {
      this.ContextMenu = new ContextMenu();
    }

    private ContextMenu BuildResourceContextMenu(ReadOnlyCollection<ResourceEntryBase> selection)
    {
      ContextMenu contextMenu = new ContextMenu();
      if (selection.Count == 1)
      {
        ResourceEntryBase resourceEntryBase = selection[0];
        ResourceEntryItem resourceEntryItem = resourceEntryBase as ResourceEntryItem;
        if (resourceEntryItem != null)
        {
          TypedResourceItem typedResourceItem = resourceEntryItem as TypedResourceItem;
          if (typedResourceItem != null)
            contextMenu.Items.Add((object) this.CreateItem(StringTable.ResourceItemEdit, "ResourceEntryItem_Edit", typedResourceItem.EditCommand));
        }
        ResourceDictionaryItem resourceDictionaryItem = resourceEntryBase as ResourceDictionaryItem;
        if (resourceDictionaryItem != null)
          contextMenu.Items.Add((object) this.CreateItem(StringTable.ResourceItemEdit, "ResourceDictionaryItem_Edit", resourceDictionaryItem.EditCommand));
        ResourceContainer targetContainer = resourceEntryBase as ResourceContainer;
        if (resourceEntryBase == this.activeDocumentWrapper)
          targetContainer = this.resourceManager.ActiveRootContainer;
        if (targetContainer != null && targetContainer.DocumentContext != null && resourceEntryBase.DocumentNode != null && (targetContainer is DocumentResourceContainer || targetContainer is NodeResourceContainer))
        {
          IProject project = ProjectHelper.GetProject(this.DesignerContext.ProjectManager, targetContainer.DocumentContext);
          if (project != null)
          {
            if (project.FindItem(targetContainer.DocumentReference).ContainsDesignTimeResources)
            {
              MenuItem menuItem = this.CreateItem(StringTable.DesignTimeResourcesAddDictionary, "ResourceContainer_LinkToDesignTimeResources", (ICommand) new ResourcePane.LinkToDesignTimeResourceCommand(this.DesignerContext, targetContainer));
              contextMenu.Items.Add((object) menuItem);
            }
            else
            {
              IProjectContext projectContext = (IProjectContext) ProjectXamlContext.GetProjectContext(project);
              if (projectContext != null)
              {
                MenuItem menuItem1 = this.CreateItem(StringTable.UndoUnitLinkToResourceDictionary, "ResourceContainer_LinkToResources", (ICommand) null);
                foreach (DocumentResourceContainer resourceContainer in (Collection<DocumentResourceContainer>) this.resourceManager.DocumentResourceContainers)
                {
                  if (resourceContainer.DocumentReference != targetContainer.DocumentReference && resourceContainer.Document != null && (resourceContainer.Document.DocumentRoot != null && resourceContainer.Document.DocumentRoot.RootNode != null) && (resourceContainer.ProjectContext != null && !resourceContainer.ProjectItem.ContainsDesignTimeResources && PlatformTypes.ResourceDictionary.IsAssignableFrom((ITypeId) resourceContainer.Document.DocumentRoot.RootNode.Type)) && (resourceContainer.ProjectContext == projectContext || ProjectHelper.DoesProjectReferencesContainTarget(project, resourceContainer.ProjectContext)))
                  {
                    ResourcePane.LinkToResourcesCommand resourcesCommand = new ResourcePane.LinkToResourcesCommand(this.resourceManager, targetContainer, resourceContainer.ProjectItem);
                    MenuItem menuItem2 = this.CreateItem(resourceContainer.Name, resourceContainer.Name, (ICommand) resourcesCommand);
                    menuItem2.IsCheckable = true;
                    menuItem2.SetBinding(MenuItem.IsCheckedProperty, (BindingBase) new Binding("IsChecked")
                    {
                      Source = (object) resourcesCommand,
                      Mode = BindingMode.OneTime
                    });
                    menuItem1.Items.Add((object) menuItem2);
                  }
                }
                if (menuItem1.Items.Count == 0)
                {
                  MenuItem menuItem2 = this.CreateItem(StringTable.ResourcePaneNoDictionaries, (string) null, (ICommand) null);
                  menuItem2.IsEnabled = false;
                  menuItem1.Items.Add((object) menuItem2);
                }
                contextMenu.Items.Add((object) menuItem1);
              }
            }
          }
        }
        if (contextMenu.Items.Count > 0)
          contextMenu.Items.Add((object) new Separator());
      }
      contextMenu.Items.Add((object) this.CreateItem(StringTable.ResourceItemCut, "ResourcePane_Cut", this.CutCommand));
      contextMenu.Items.Add((object) this.CreateItem(StringTable.ResourceItemCopy, "ResourcePane_Copy", this.CopyCommand));
      contextMenu.Items.Add((object) this.CreateItem(StringTable.ResourceItemPaste, "ResourcePane_Paste", this.PasteCommand));
      contextMenu.Items.Add((object) this.CreateItem(StringTable.ResourceItemDelete, "ResourcePane_Delete", this.DeleteCommand));
      contextMenu.Items.Add((object) new Separator());
      contextMenu.Items.Add((object) this.CreateItem(StringTable.ViewXamlCommandName, "ResourcePane_ViewXaml", this.ViewXamlCommand));
      return contextMenu;
    }

    private void InteractiveDelete(ICollection<ResourceItem> resources)
    {
      foreach (ResourceItem resourceItem in (IEnumerable<ResourceItem>) resources)
        resourceItem.InteractiveDelete();
    }

    private void CutResource()
    {
      if (this.resourceManager.SelectedResourceContainers.Count != 0 || this.resourceManager.SelectedResourceItems.Count != 1 || !this.CopyResourceCore())
        return;
      this.OnDeleteCommand();
    }

    private void CopyResource()
    {
      this.CopyResourceCore();
    }

    private bool CopyResourceCore()
    {
      if (this.resourceManager.SelectedResourceContainers.Count == 0 && this.resourceManager.SelectedResourceItems.Count == 1)
      {
        ResourceEntryItem resourceEntryItem = this.resourceManager.SelectedResourceItems[0] as ResourceEntryItem;
        if (resourceEntryItem != null)
        {
          SceneViewModel viewModel = resourceEntryItem.Container.ViewModel;
          PastePackage pastePackage = new PastePackage(viewModel);
          pastePackage.AddResource(viewModel.GetSceneNode((DocumentNode) resourceEntryItem.Resource.ResourceNode) as DictionaryEntryNode);
          pastePackage.SendToClipboard();
          return true;
        }
      }
      return false;
    }

    private void PasteResource()
    {
      int index = -1;
      ResourceContainer container = (ResourceContainer) null;
      if (this.resourceManager.SelectedResourceContainers.Count == 1)
      {
        container = this.resourceManager.SelectedResourceContainers[0];
        index = container.ResourceItems.Count;
      }
      else if (this.resourceManager.SelectedResourceItems.Count == 1)
      {
        ResourceItem resourceItem = this.resourceManager.SelectedResourceItems[0];
        container = resourceItem.Container;
        index = container.ResourceItems.IndexOf(resourceItem) + 1;
      }
      SafeDataObject dataObject = SafeDataObject.FromClipboard();
      if (index < 0)
        return;
      SceneViewModel viewModel = container.ViewModel;
      PastePackage pastePackage = PastePackage.FromData(viewModel, dataObject);
      if (pastePackage != null)
      {
        if (pastePackage.Elements.Count > 0)
        {
          viewModel.DesignerContext.MessageDisplayService.ShowError(StringTable.PasteResourcesFailedElementsFoundDialogMessage);
        }
        else
        {
          IDictionary<DocumentNode, string> imageMap = (IDictionary<DocumentNode, string>) new Dictionary<DocumentNode, string>();
          foreach (SceneNode sceneNode in pastePackage.Resources)
          {
            foreach (KeyValuePair<DocumentNode, string> keyValuePair in (IEnumerable<KeyValuePair<DocumentNode, string>>) Microsoft.Expression.DesignSurface.Utility.ResourceHelper.CreateImageReferenceMap(sceneNode.DocumentNode, pastePackage, viewModel))
              imageMap.Add(keyValuePair);
          }
          using (SceneEditTransaction editTransaction = viewModel.CreateEditTransaction(StringTable.UndoUnitPaste))
          {
            Microsoft.Expression.DesignSurface.Utility.ResourceHelper.PasteResources(pastePackage, imageMap, ResourceConflictResolution.RenameNew | ResourceConflictResolution.OverwriteOld, container.Node, index, true);
            editTransaction.Commit();
          }
        }
      }
      else if (dataObject.GetDataPresent(DataFormats.FileDrop))
      {
        DesignerContext designerContext = viewModel.DesignerContext;
        IDocumentType[] supportedDocumentTypes = new IDocumentType[1]
        {
          designerContext.DocumentTypeManager.DocumentTypes[DocumentTypeNamesHelper.Image]
        };
        string[] supportedFiles = new FileDropUtility(designerContext.ProjectManager, (FrameworkElement) null, supportedDocumentTypes).GetSupportedFiles(ClipboardService.GetDataObject());
        if (supportedFiles.Length > 0)
        {
          foreach (IProjectItem projectItem in designerContext.ActiveProject.AddItems(Enumerable.Select<string, DocumentCreationInfo>((IEnumerable<string>) supportedFiles, (Func<string, DocumentCreationInfo>) (file => new DocumentCreationInfo()
          {
            SourcePath = file
          }))))
            this.CreateImageBrushResource(container, projectItem);
        }
        else
          designerContext.MessageDisplayService.ShowError(StringTable.PasteElementsFailedDialogMessage);
      }
      else
      {
        if (!dataObject.GetDataPresent(DataFormats.Bitmap))
          return;
        IProject project = EnumerableExtensions.SingleOrNull<IProject>(this.projectManager.ItemSelectionSet.SelectedProjects);
        if (project == null)
          return;
        IProjectItem projectItem = CutBuffer.AddImageDataFromClipboard(this.projectManager, project);
        this.CreateImageBrushResource(container, projectItem);
      }
    }

    public void CreateImageBrushResource(ResourceContainer container, IProjectItem item)
    {
      SceneViewModel viewModel = container.ViewModel;
      string resourceReference = item.GetResourceReference(viewModel.Document.DocumentReference);
      ImageBrushNode imageBrushNode = (ImageBrushNode) viewModel.CreateSceneNode(PlatformTypes.ImageBrush);
      imageBrushNode.Source = resourceReference;
      using (SceneEditTransaction editTransaction = viewModel.CreateEditTransaction(StringTable.UndoUnitPaste, false))
      {
        IType type = viewModel.ProjectContext.ResolveType(PlatformTypes.ImageBrush);
        new CreateResourceModel(viewModel, viewModel.DesignerContext.ResourceManager, type.RuntimeType, (Type) null, (string) null, container.Node as SceneElement, (SceneNode) null, CreateResourceModel.ContextFlags.None)
        {
          KeyString = Path.GetFileNameWithoutExtension(resourceReference)
        }.CreateResource(imageBrushNode.DocumentNode, (IPropertyId) null, -1);
        editTransaction.Commit();
      }
    }

    private List<DocumentNode> GetSelectedItemsDocumentNodes()
    {
      List<DocumentNode> list = new List<DocumentNode>();
      IDocumentContext documentContext = (IDocumentContext) null;
      if (this.resourceManager.SelectedItems.Count <= 0)
        return (List<DocumentNode>) null;
      foreach (ResourceEntryBase resourceEntryBase in this.resourceManager.SelectedItems.Selection)
      {
        DocumentNode documentNode = resourceEntryBase.DocumentNode;
        if (resourceEntryBase == this.activeDocumentWrapper && this.designerContext.ActiveDocument != null)
          documentNode = this.designerContext.ActiveDocument.DocumentRoot.RootNode;
        if (documentNode != null)
        {
          list.Add(documentNode);
          if (documentContext == null)
            documentContext = documentNode.Context;
          else if (documentContext != documentNode.Context)
            return (List<DocumentNode>) null;
        }
      }
      return list;
    }

    private MenuItem CreateItem(string name, string id, ICommand command)
    {
      MenuItem menuItem = new MenuItem();
      menuItem.Header = (object) name;
      menuItem.SetValue(AutomationElement.IdProperty, (object) id);
      menuItem.Command = command;
      if (command != null)
        menuItem.SetBinding(UIElement.IsEnabledProperty, (BindingBase) new Binding("IsEnabled")
        {
          Source = (object) command,
          Mode = BindingMode.OneTime
        });
      return menuItem;
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/resources/resourcepane/resourcepane.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    internal Delegate _CreateDelegate(Type delegateType, string handler)
    {
      return Delegate.CreateDelegate(delegateType, (object) this, handler);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DebuggerNonUserCode]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.ResourceScrollViewer = (DragDropScrollViewer) target;
          break;
        case 2:
          this.ResourceItemSelector = (ItemsControl) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }

    private class LinkToResourcesCommand : ICommand
    {
      private ResourceManager resourceManager;
      private ResourceContainer targetContainer;
      private IProjectItem dictionaryItem;

      public bool IsChecked
      {
        get
        {
          return this.resourceManager.FindDictionaryItem(this.targetContainer, this.dictionaryItem.DocumentReference) != null;
        }
        set
        {
          if (value)
          {
            this.resourceManager.LinkToResource(this.targetContainer, this.dictionaryItem.DocumentReference);
          }
          else
          {
            DocumentNode node = this.resourceManager.FindDictionaryItem(this.targetContainer, this.dictionaryItem.DocumentReference);
            if (node == null)
              return;
            ResourceDictionaryItem resourceDictionaryItem = Enumerable.FirstOrDefault<ResourceItem>((IEnumerable<ResourceItem>) this.targetContainer.ResourceItems, (Func<ResourceItem, bool>) (i => i.DocumentNode == node)) as ResourceDictionaryItem;
            if (resourceDictionaryItem == null)
              return;
            resourceDictionaryItem.InteractiveDelete();
          }
        }
      }

      public event EventHandler CanExecuteChanged
      {
        add
        {
        }
        remove
        {
        }
      }

      public LinkToResourcesCommand(ResourceManager resourceManager, ResourceContainer targetContainer, IProjectItem dictionaryItem)
      {
        this.resourceManager = resourceManager;
        this.targetContainer = targetContainer;
        this.dictionaryItem = dictionaryItem;
      }

      public void Execute(object arg)
      {
        this.IsChecked = !this.IsChecked;
      }

      public bool CanExecute(object parameter)
      {
        return true;
      }
    }

    private class LinkToDesignTimeResourceCommand : ICommand
    {
      private DesignerContext context;
      private ResourceContainer targetContainer;
      private DesignTimeResourceResolver resolver;

      public event EventHandler CanExecuteChanged
      {
        add
        {
        }
        remove
        {
        }
      }

      public LinkToDesignTimeResourceCommand(DesignerContext context, ResourceContainer targetContainer)
      {
        this.context = context;
        this.targetContainer = targetContainer;
        this.resolver = new DesignTimeResourceResolver(this.context);
      }

      public bool CanExecute(object parameter)
      {
        return this.resolver.CanResolve(this.targetContainer.DocumentContext);
      }

      public void Execute(object parameter)
      {
        this.resolver.Enqueue((Action) (() => this.resolver.Resolve(this.targetContainer.DocumentContext, EditDesignTimeResourceModelMode.Manual, (string) null)));
      }
    }
  }
}
