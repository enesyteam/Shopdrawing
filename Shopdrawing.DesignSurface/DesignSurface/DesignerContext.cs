// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.DesignerContext
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Code;
using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Text;
using Microsoft.Expression.DesignSurface.Annotations;
using Microsoft.Expression.DesignSurface.CodeAid;
using Microsoft.Expression.DesignSurface.DocumentProcessors;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.Selection;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.Tools.Assets;
using Microsoft.Expression.DesignSurface.Tools.Path;
using Microsoft.Expression.DesignSurface.Tools.Transforms;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector;
using Microsoft.Expression.DesignSurface.UserInterface.ResourcePane;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Extensibility;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Framework.Configuration;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.Importers;
using Microsoft.Expression.Framework.Scheduler;
using Microsoft.Expression.Framework.UserInterface;
using Microsoft.Expression.Project;
using System;

namespace Microsoft.Expression.DesignSurface
{
  internal sealed class DesignerContext
  {
    private IServiceProvider serviceProvider;
    private ArtboardOptionsModel artboardOptionsModel;
    private ReferenceChecker referenceChecker;

    public IAssemblyService AssemblyService
    {
      get
      {
        return this.serviceProvider.GetService(typeof (IAssemblyService)) as IAssemblyService;
      }
    }

    public IServices Services { get; private set; }

    public IExpressionInformationService ExpressionInformationService { get; private set; }

    public IDocumentService DocumentService
    {
      get
      {
        return (IDocumentService) this.serviceProvider.GetService(typeof (IDocumentService));
      }
    }

    public IWindowService WindowService
    {
      get
      {
        return (IWindowService) this.serviceProvider.GetService(typeof (IWindowService));
      }
    }

    public IWorkspaceService WorkspaceService
    {
      get
      {
        return (IWorkspaceService) this.serviceProvider.GetService(typeof (IWorkspaceService));
      }
    }

    public IViewService ViewService
    {
      get
      {
        return (IViewService) this.serviceProvider.GetService(typeof (IViewService));
      }
    }

    public ICommandService CommandService
    {
      get
      {
        return (ICommandService) this.serviceProvider.GetService(typeof (ICommandService));
      }
    }

    public ICommandBarService CommandBarService
    {
      get
      {
        return (ICommandBarService) this.serviceProvider.GetService(typeof (ICommandBarService));
      }
    }

    public IProjectManager ProjectManager
    {
      get
      {
        return (IProjectManager) this.serviceProvider.GetService(typeof (IProjectManager));
      }
    }

    public IDocumentTypeManager DocumentTypeManager
    {
      get
      {
        return (IDocumentTypeManager) this.serviceProvider.GetService(typeof (IDocumentTypeManager));
      }
    }

    public ISchedulingService SchedulingService
    {
      get
      {
        return (ISchedulingService) this.serviceProvider.GetService(typeof (ISchedulingService));
      }
    }

    public IExternalChanges ExternalChanges
    {
      get
      {
        return (IExternalChanges) this.serviceProvider.GetService(typeof (IExternalChanges));
      }
    }

    public IMessageDisplayService MessageDisplayService
    {
      get
      {
        return (IMessageDisplayService) this.serviceProvider.GetService(typeof (IMessageDisplayService));
      }
    }

    public ITextBufferService TextBufferService
    {
      get
      {
        return (ITextBufferService) this.serviceProvider.GetService(typeof (ITextBufferService));
      }
    }

    public ITextEditorService TextEditorService
    {
      get
      {
        return (ITextEditorService) this.serviceProvider.GetService(typeof (ITextEditorService));
      }
    }

    public ICodeModelService CodeModelService
    {
      get
      {
        return (ICodeModelService) this.serviceProvider.GetService(typeof (ICodeModelService));
      }
    }

    public AnnotationService AnnotationService
    {
      get
      {
        return (AnnotationService) this.serviceProvider.GetService(typeof (AnnotationService));
      }
    }

    public IPrototypingService PrototypingService
    {
      get
      {
        return (IPrototypingService) this.serviceProvider.GetService(typeof (IPrototypingService));
      }
    }

    public IPlatformService PlatformService
    {
      get
      {
        return (IPlatformService) this.serviceProvider.GetService(typeof (IPlatformService));
      }
    }

    public IDesignerDefaultPlatformService DesignerDefaultPlatformService
    {
      get
      {
        return (IDesignerDefaultPlatformService) this.serviceProvider.GetService(typeof (IDesignerDefaultPlatformService));
      }
    }

    public IHelpService HelpService
    {
      get
      {
        return (IHelpService) this.serviceProvider.GetService(typeof (IHelpService));
      }
    }

    public IMessageLoggingService MessageLoggingService { get; set; }

    public IErrorService ErrorManager { get; set; }

    public IConfigurationObject Configuration
    {
      get
      {
        IConfigurationService configurationService = (IConfigurationService) this.serviceProvider.GetService(typeof (IConfigurationService));
        if (configurationService != null)
          return configurationService["DesignerPackage"];
        return (IConfigurationObject) null;
      }
    }

    public IProject ActiveProject
    {
      get
      {
        IProject project = (IProject) null;
        if (this.ActiveView != null && this.ActiveView.Document != null)
          project = ProjectHelper.GetProject(this.ProjectManager, this.ActiveView.Document.DocumentContext);
        return project;
      }
    }

    public SceneDocument ActiveDocument
    {
      get
      {
        if (this.DocumentService == null)
          return (SceneDocument) null;
        return this.DocumentService.ActiveDocument as SceneDocument;
      }
    }

    public IProjectContext ActiveProjectContext
    {
      get
      {
        SceneDocument activeDocument = this.ActiveDocument;
        if (activeDocument == null)
          return (IProjectContext) null;
        return (IProjectContext) ProjectXamlContext.FromProjectContext(activeDocument.ProjectContext) ?? activeDocument.ProjectContext;
      }
    }

    public SceneView ActiveView
    {
      get
      {
        if (this.ViewService == null)
          return (SceneView) null;
        return this.ViewService.ActiveView as SceneView;
      }
    }

    public SceneViewModel ActiveSceneViewModel
    {
      get
      {
        SceneView activeView = this.ActiveView;
        if (activeView != null)
          return activeView.ViewModel;
        return (SceneViewModel) null;
      }
    }

    public IImporterService ImportService
    {
      get
      {
        return (IImporterService) this.serviceProvider.GetService(typeof (IImporterService));
      }
    }

    public ToolContext ToolContext { get; private set; }

    public ToolManager ToolManager { get; private set; }

    public ILicenseFileManager LicenseManager { get; private set; }

    public PlatformConverter PlatformConverter { get; private set; }

    public SelectionManager SelectionManager { get; private set; }

    public IPropertyManager PropertyManager { get; private set; }

    public IAmbientPropertyManager AmbientPropertyManager { get; private set; }

    public SnappingEngine SnappingEngine { get; private set; }

    public ResourceManager ResourceManager { get; private set; }

    public ViewUpdateManager ViewUpdateManager { get; private set; }

    internal IAssetLibrary AssetLibrary { get; private set; }

    internal ViewRootResolver ViewRootResolver { get; private set; }

    internal ThemeContentProvider ThemeContentProvider { get; private set; }

    internal BreadcrumbBarModel BreadcrumbBarModel { get; private set; }

    internal CodeAidProvider CodeAidProvider { get; private set; }

    internal PropertyInspectorModel PropertyInspectorModel { get; private set; }

    internal ViewOptionsModel ViewOptionsModel { get; private set; }

    internal UnitsOptionsModel UnitsOptionsModel { get; private set; }

    internal AnnotationsOptionsModel AnnotationsOptionsModel { get; private set; }

    internal GradientToolSelectionService GradientToolSelectionService { get; private set; }

    internal ElementToPathEditorTargetMap PathEditorTargetMap { get; private set; }

    internal PlatformContextChanger PlatformContextChanger { get; private set; }

    public ArtboardOptionsModel ArtboardOptionsModel
    {
      get
      {
        return this.artboardOptionsModel;
      }
      set
      {
        if (this.artboardOptionsModel == value)
          return;
        this.artboardOptionsModel = value;
        this.NotifyArtboardOptionsChanged();
      }
    }

    public event EventHandler ArtboardOptionsChanged;

    public DesignerContext(IServiceProvider serviceProvider)
    {
      this.serviceProvider = serviceProvider;
      this.Services = this.serviceProvider as IServices;
      if (this.Services == null)
        this.Services = (IServices) serviceProvider.GetService(typeof (IServices));
      this.ExpressionInformationService = (IExpressionInformationService) this.serviceProvider.GetService(typeof (IExpressionInformationService));
      this.MessageLoggingService = (IMessageLoggingService) this.serviceProvider.GetService(typeof (IMessageLoggingService));
      this.ErrorManager = (IErrorService) this.serviceProvider.GetService(typeof (IErrorService));
      this.PathEditorTargetMap = new ElementToPathEditorTargetMap();
      this.referenceChecker = new ReferenceChecker(this);
    }

    public void NotifyArtboardOptionsChanged()
    {
      if (this.ArtboardOptionsChanged == null)
        return;
      this.ArtboardOptionsChanged((object) this, EventArgs.Empty);
    }

    public void Initialize()
    {
      this.SelectionManager = new SelectionManager(this);
      this.ToolManager = new ToolManager(this.ViewService, this.Configuration);
      this.PlatformConverter = new PlatformConverter(this);
      this.PropertyManager = (IPropertyManager) new Microsoft.Expression.DesignSurface.Properties.PropertyManager(this);
      this.AmbientPropertyManager = (IAmbientPropertyManager) new Microsoft.Expression.DesignSurface.Properties.AmbientPropertyManager(this);
      this.ArtboardOptionsModel = new ArtboardOptionsModel((IConfigurationObject) null, false);
      this.SnappingEngine = new SnappingEngine(this);
      this.UnitsOptionsModel = new UnitsOptionsModel(true);
      this.ViewOptionsModel = new ViewOptionsModel(true);
      this.AnnotationsOptionsModel = new AnnotationsOptionsModel(true);
      this.ResourceManager = new ResourceManager(this);
      this.ViewUpdateManager = new ViewUpdateManager(this);
      this.LicenseManager = (ILicenseFileManager) new LicenseFileManager(this);
      this.AssetLibrary = (IAssetLibrary) new Microsoft.Expression.DesignSurface.Tools.Assets.AssetLibrary(this);
      this.ThemeContentProvider = new ThemeContentProvider(this);
      this.ViewRootResolver = new ViewRootResolver(this);
      this.BreadcrumbBarModel = new BreadcrumbBarModel(this);
      this.CodeAidProvider = new CodeAidProvider(this);
      this.PropertyInspectorModel = new PropertyInspectorModel(this);
      this.GradientToolSelectionService = new GradientToolSelectionService();
      this.PlatformContextChanger = new PlatformContextChanger(this.ViewService, this.ProjectManager);
      this.ToolContext = new ToolContext(this);
    }

    public void Uninitialize()
    {
      this.PropertyInspectorModel.Dispose();
      this.PropertyInspectorModel = (PropertyInspectorModel) null;
      this.ResourceManager.Unload();
      this.ResourceManager = (ResourceManager) null;
      this.BreadcrumbBarModel.Dispose();
      this.BreadcrumbBarModel = (BreadcrumbBarModel) null;
      this.SelectionManager.Dispose();
      this.SelectionManager = (SelectionManager) null;
      this.ViewUpdateManager = (ViewUpdateManager) null;
      this.PlatformContextChanger.Dispose();
      this.PlatformContextChanger = (PlatformContextChanger) null;
      this.PropertyManager.Dispose();
      this.PropertyManager = (IPropertyManager) null;
      this.PlatformConverter = (PlatformConverter) null;
      this.AmbientPropertyManager.Dispose();
      this.AmbientPropertyManager = (IAmbientPropertyManager) null;
      this.AssetLibrary.Dispose();
      this.AssetLibrary = (IAssetLibrary) null;
      this.ThemeContentProvider = (ThemeContentProvider) null;
      this.SnappingEngine = (SnappingEngine) null;
      this.CodeAidProvider = (CodeAidProvider) null;
      this.ViewOptionsModel = (ViewOptionsModel) null;
      this.UnitsOptionsModel = (UnitsOptionsModel) null;
      this.AnnotationsOptionsModel = (AnnotationsOptionsModel) null;
      this.referenceChecker.Unhook();
      this.referenceChecker = (ReferenceChecker) null;
      this.ToolContext = (ToolContext) null;
      this.ToolManager.Dispose();
      this.ToolManager = (ToolManager) null;
      this.GradientToolSelectionService = (GradientToolSelectionService) null;
    }

    internal void InitializeForUnittests()
    {
      this.ToolManager = (ToolManager) this.serviceProvider.GetService(typeof (ToolManager));
      this.SelectionManager = new SelectionManager(this);
      this.PropertyManager = (IPropertyManager) new Microsoft.Expression.DesignSurface.Properties.PropertyManager(this);
      this.AssetLibrary = (IAssetLibrary) new Microsoft.Expression.DesignSurface.Tools.Assets.AssetLibrary(this);
      this.ResourceManager = new ResourceManager(this);
      this.ViewRootResolver = new ViewRootResolver(this);
      this.ThemeContentProvider = new ThemeContentProvider(this);
      this.PlatformConverter = new PlatformConverter(this);
      this.ToolContext = new ToolContext(this);
      this.ViewUpdateManager = new ViewUpdateManager(this);
    }

    internal void UninitializeForUnittests()
    {
      this.ViewUpdateManager = (ViewUpdateManager) null;
      this.ToolContext = (ToolContext) null;
      this.PlatformConverter = (PlatformConverter) null;
      this.ThemeContentProvider = (ThemeContentProvider) null;
      this.ViewRootResolver.Dispose();
      this.ViewRootResolver = (ViewRootResolver) null;
      this.ResourceManager.Unload();
      this.ResourceManager = (ResourceManager) null;
      this.AssetLibrary.Dispose();
      this.AssetLibrary = (IAssetLibrary) null;
      this.PropertyManager.Dispose();
      this.PropertyManager = (IPropertyManager) null;
      this.SelectionManager.Dispose();
      this.SelectionManager = (SelectionManager) null;
      this.ToolManager = (ToolManager) null;
    }
  }
}
