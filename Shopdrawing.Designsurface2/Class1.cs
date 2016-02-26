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
using System.Runtime.CompilerServices;
using System.Threading;

namespace Microsoft.Expression.DesignSurface
{
    internal sealed class DesignerContext
    {
        private IServiceProvider serviceProvider;

        private ArtboardOptionsModel artboardOptionsModel;

        private ReferenceChecker referenceChecker;

        public SceneDocument ActiveDocument
        {
            get
            {
                if (this.DocumentService == null)
                {
                    return null;
                }
                return this.DocumentService.get_ActiveDocument() as SceneDocument;
            }
        }

        public IProject ActiveProject
        {
            get
            {
                IProject project = null;
                if (this.ActiveView != null && this.ActiveView.Document != null)
                {
                    project = ProjectHelper.GetProject(this.ProjectManager, this.ActiveView.Document.DocumentContext);
                }
                return project;
            }
        }

        public IProjectContext ActiveProjectContext
        {
            get
            {
                SceneDocument activeDocument = this.ActiveDocument;
                if (activeDocument == null)
                {
                    return null;
                }
                ProjectXamlContext projectXamlContext = ProjectXamlContext.FromProjectContext(activeDocument.ProjectContext);
                if (projectXamlContext != null)
                {
                    return projectXamlContext;
                }
                return activeDocument.ProjectContext;
            }
        }

        public SceneViewModel ActiveSceneViewModel
        {
            get
            {
                SceneView activeView = this.ActiveView;
                if (activeView == null)
                {
                    return null;
                }
                return activeView.ViewModel;
            }
        }

        public SceneView ActiveView
        {
            get
            {
                if (this.ViewService == null)
                {
                    return null;
                }
                return this.ViewService.get_ActiveView() as SceneView;
            }
        }

        public IAmbientPropertyManager AmbientPropertyManager
        {
            get;
            private set;
        }

        public AnnotationService AnnotationService
        {
            get
            {
                return (AnnotationService)this.serviceProvider.GetService(typeof(AnnotationService));
            }
        }

        internal AnnotationsOptionsModel AnnotationsOptionsModel
        {
            get;
            private set;
        }

        public ArtboardOptionsModel ArtboardOptionsModel
        {
            get
            {
                return this.artboardOptionsModel;
            }
            set
            {
                if (this.artboardOptionsModel != value)
                {
                    this.artboardOptionsModel = value;
                    this.NotifyArtboardOptionsChanged();
                }
            }
        }

        public IAssemblyService AssemblyService
        {
            get
            {
                return this.serviceProvider.GetService(typeof(IAssemblyService)) as IAssemblyService;
            }
        }

        internal IAssetLibrary AssetLibrary
        {
            get;
            private set;
        }

        internal BreadcrumbBarModel BreadcrumbBarModel
        {
            get;
            private set;
        }

        internal CodeAidProvider CodeAidProvider
        {
            get;
            private set;
        }

        public ICodeModelService CodeModelService
        {
            get
            {
                return (ICodeModelService)this.serviceProvider.GetService(typeof(ICodeModelService));
            }
        }

        public ICommandBarService CommandBarService
        {
            get
            {
                return (ICommandBarService)this.serviceProvider.GetService(typeof(ICommandBarService));
            }
        }

        public ICommandService CommandService
        {
            get
            {
                return (ICommandService)this.serviceProvider.GetService(typeof(ICommandService));
            }
        }

        public IConfigurationObject Configuration
        {
            get
            {
                IConfigurationService service = (IConfigurationService)this.serviceProvider.GetService(typeof(IConfigurationService));
                if (service == null)
                {
                    return null;
                }
                return service.get_Item("DesignerPackage");
            }
        }

        public IDesignerDefaultPlatformService DesignerDefaultPlatformService
        {
            get
            {
                return (IDesignerDefaultPlatformService)this.serviceProvider.GetService(typeof(IDesignerDefaultPlatformService));
            }
        }

        public IDocumentService DocumentService
        {
            get
            {
                return (IDocumentService)this.serviceProvider.GetService(typeof(IDocumentService));
            }
        }

        public IDocumentTypeManager DocumentTypeManager
        {
            get
            {
                return (IDocumentTypeManager)this.serviceProvider.GetService(typeof(IDocumentTypeManager));
            }
        }

        public IErrorService ErrorManager
        {
            get;
            set;
        }

        public IExpressionInformationService ExpressionInformationService
        {
            get;
            private set;
        }

        public IExternalChanges ExternalChanges
        {
            get
            {
                return (IExternalChanges)this.serviceProvider.GetService(typeof(IExternalChanges));
            }
        }

        internal GradientToolSelectionService GradientToolSelectionService
        {
            get;
            private set;
        }

        public IHelpService HelpService
        {
            get
            {
                return (IHelpService)this.serviceProvider.GetService(typeof(IHelpService));
            }
        }

        public IImporterService ImportService
        {
            get
            {
                return (IImporterService)this.serviceProvider.GetService(typeof(IImporterService));
            }
        }

        public ILicenseFileManager LicenseManager
        {
            get;
            private set;
        }

        public IMessageDisplayService MessageDisplayService
        {
            get
            {
                return (IMessageDisplayService)this.serviceProvider.GetService(typeof(IMessageDisplayService));
            }
        }

        public IMessageLoggingService MessageLoggingService
        {
            get;
            set;
        }

        internal ElementToPathEditorTargetMap PathEditorTargetMap
        {
            get;
            private set;
        }

        internal PlatformContextChanger PlatformContextChanger
        {
            get;
            private set;
        }

        public PlatformConverter PlatformConverter
        {
            get;
            private set;
        }

        public IPlatformService PlatformService
        {
            get
            {
                return (IPlatformService)this.serviceProvider.GetService(typeof(IPlatformService));
            }
        }

        public IProjectManager ProjectManager
        {
            get
            {
                return (IProjectManager)this.serviceProvider.GetService(typeof(IProjectManager));
            }
        }

        internal PropertyInspectorModel PropertyInspectorModel
        {
            get;
            private set;
        }

        public IPropertyManager PropertyManager
        {
            get;
            private set;
        }

        public IPrototypingService PrototypingService
        {
            get
            {
                return (IPrototypingService)this.serviceProvider.GetService(typeof(IPrototypingService));
            }
        }

        public ResourceManager ResourceManager
        {
            get;
            private set;
        }

        public ISchedulingService SchedulingService
        {
            get
            {
                return (ISchedulingService)this.serviceProvider.GetService(typeof(ISchedulingService));
            }
        }

        public SelectionManager SelectionManager
        {
            get;
            private set;
        }

        public IServices Services
        {
            get;
            private set;
        }

        public SnappingEngine SnappingEngine
        {
            get;
            private set;
        }

        public ITextBufferService TextBufferService
        {
            get
            {
                return (ITextBufferService)this.serviceProvider.GetService(typeof(ITextBufferService));
            }
        }

        public ITextEditorService TextEditorService
        {
            get
            {
                return (ITextEditorService)this.serviceProvider.GetService(typeof(ITextEditorService));
            }
        }

        internal ThemeContentProvider ThemeContentProvider
        {
            get;
            private set;
        }

        public ToolContext ToolContext
        {
            get;
            private set;
        }

        public ToolManager ToolManager
        {
            get;
            private set;
        }

        internal UnitsOptionsModel UnitsOptionsModel
        {
            get;
            private set;
        }

        internal ViewOptionsModel ViewOptionsModel
        {
            get;
            private set;
        }

        internal ViewRootResolver ViewRootResolver
        {
            get;
            private set;
        }

        public IViewService ViewService
        {
            get
            {
                return (IViewService)this.serviceProvider.GetService(typeof(IViewService));
            }
        }

        public ViewUpdateManager ViewUpdateManager
        {
            get;
            private set;
        }

        public IWindowService WindowService
        {
            get
            {
                return (IWindowService)this.serviceProvider.GetService(typeof(IWindowService));
            }
        }

        public IWorkspaceService WorkspaceService
        {
            get
            {
                return (IWorkspaceService)this.serviceProvider.GetService(typeof(IWorkspaceService));
            }
        }

        public DesignerContext(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            this.Services = this.serviceProvider as IServices;
            if (this.Services == null)
            {
                this.Services = (IServices)serviceProvider.GetService(typeof(IServices));
            }
            this.ExpressionInformationService = (IExpressionInformationService)this.serviceProvider.GetService(typeof(IExpressionInformationService));
            this.MessageLoggingService = (IMessageLoggingService)this.serviceProvider.GetService(typeof(IMessageLoggingService));
            this.ErrorManager = (IErrorService)this.serviceProvider.GetService(typeof(IErrorService));
            this.PathEditorTargetMap = new ElementToPathEditorTargetMap();
            this.referenceChecker = new ReferenceChecker(this);
        }

        public void Initialize()
        {
            this.SelectionManager = new SelectionManager(this);
            this.ToolManager = new ToolManager(this.ViewService, this.Configuration);
            this.PlatformConverter = new PlatformConverter(this);
            this.PropertyManager = new PropertyManager(this);
            this.AmbientPropertyManager = new AmbientPropertyManager(this);
            this.ArtboardOptionsModel = new ArtboardOptionsModel(null, false);
            this.SnappingEngine = new SnappingEngine(this);
            this.UnitsOptionsModel = new UnitsOptionsModel(true);
            this.ViewOptionsModel = new ViewOptionsModel(true);
            this.AnnotationsOptionsModel = new AnnotationsOptionsModel(true);
            this.ResourceManager = new ResourceManager(this);
            this.ViewUpdateManager = new ViewUpdateManager(this);
            this.LicenseManager = new LicenseFileManager(this);
            this.AssetLibrary = new AssetLibrary(this);
            this.ThemeContentProvider = new ThemeContentProvider(this);
            this.ViewRootResolver = new ViewRootResolver(this);
            this.BreadcrumbBarModel = new BreadcrumbBarModel(this);
            this.CodeAidProvider = new CodeAidProvider(this);
            this.PropertyInspectorModel = new PropertyInspectorModel(this);
            this.GradientToolSelectionService = new GradientToolSelectionService();
            this.PlatformContextChanger = new PlatformContextChanger(this.ViewService, this.ProjectManager);
            this.ToolContext = new ToolContext(this);
        }

        internal void InitializeForUnittests()
        {
            this.ToolManager = (ToolManager)this.serviceProvider.GetService(typeof(ToolManager));
            this.SelectionManager = new SelectionManager(this);
            this.PropertyManager = new PropertyManager(this);
            this.AssetLibrary = new AssetLibrary(this);
            this.ResourceManager = new ResourceManager(this);
            this.ViewRootResolver = new ViewRootResolver(this);
            this.ThemeContentProvider = new ThemeContentProvider(this);
            this.PlatformConverter = new PlatformConverter(this);
            this.ToolContext = new ToolContext(this);
            this.ViewUpdateManager = new ViewUpdateManager(this);
        }

        public void NotifyArtboardOptionsChanged()
        {
            if (this.ArtboardOptionsChanged != null)
            {
                this.ArtboardOptionsChanged(this, EventArgs.Empty);
            }
        }

        public void Uninitialize()
        {
            this.PropertyInspectorModel.Dispose();
            this.PropertyInspectorModel = null;
            this.ResourceManager.Unload();
            this.ResourceManager = null;
            this.BreadcrumbBarModel.Dispose();
            this.BreadcrumbBarModel = null;
            this.SelectionManager.Dispose();
            this.SelectionManager = null;
            this.ViewUpdateManager = null;
            this.PlatformContextChanger.Dispose();
            this.PlatformContextChanger = null;
            this.PropertyManager.Dispose();
            this.PropertyManager = null;
            this.PlatformConverter = null;
            this.AmbientPropertyManager.Dispose();
            this.AmbientPropertyManager = null;
            this.AssetLibrary.Dispose();
            this.AssetLibrary = null;
            this.ThemeContentProvider = null;
            this.SnappingEngine = null;
            this.CodeAidProvider = null;
            this.ViewOptionsModel = null;
            this.UnitsOptionsModel = null;
            this.AnnotationsOptionsModel = null;
            this.referenceChecker.Unhook();
            this.referenceChecker = null;
            this.ToolContext = null;
            this.ToolManager.Dispose();
            this.ToolManager = null;
            this.GradientToolSelectionService = null;
        }

        internal void UninitializeForUnittests()
        {
            this.ViewUpdateManager = null;
            this.ToolContext = null;
            this.PlatformConverter = null;
            this.ThemeContentProvider = null;
            this.ViewRootResolver.Dispose();
            this.ViewRootResolver = null;
            this.ResourceManager.Unload();
            this.ResourceManager = null;
            this.AssetLibrary.Dispose();
            this.AssetLibrary = null;
            this.PropertyManager.Dispose();
            this.PropertyManager = null;
            this.SelectionManager.Dispose();
            this.SelectionManager = null;
            this.ToolManager = null;
        }

        public event EventHandler ArtboardOptionsChanged;
    }
}