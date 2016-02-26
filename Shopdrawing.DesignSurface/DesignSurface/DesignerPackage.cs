using Microsoft.Expression.DesignModel.Code;
using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Annotations;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Documents.Commands;
using Microsoft.Expression.DesignSurface.Project;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.SceneCommands;
using Microsoft.Expression.DesignSurface.Selection;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.Tools.Assets;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.UserInterface.DataPane;
using Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector;
using Microsoft.Expression.DesignSurface.UserInterface.ResourcePane;
using Microsoft.Expression.DesignSurface.UserInterface.SkinEditing;
using Microsoft.Expression.DesignSurface.UserInterface.Timeline.UI;
using Microsoft.Expression.DesignSurface.UserInterface.Triggers;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Extensibility;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Framework.Configuration;
using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.Expression.Framework.Scheduler;
using Microsoft.Expression.Framework.UserInterface;
using Microsoft.Expression.Framework.ValueEditors.ColorEditor;
using Microsoft.Expression.Framework.Workspaces.Extension;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.UserInterface;
using Microsoft.Windows.Design.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Microsoft.Expression.DesignSurface
{
    public sealed class DesignerPackage : IPackage, IDisposable
    {
        private DesignerContext designerContext;

        private IServices services;

        private SilverlightAssemblyResolver silverLightAssemblyResolver;

        private IList<IDocumentType> documentTypes = new List<IDocumentType>();

        private IList<IProjectType> projectTypes = new List<IProjectType>();

        private IList<Tool> tools = new List<Tool>();

        private SharedColorSpaceManager sharedColorSpaceManager;

        private AnnotationService annotationService;

        private IOptionsPage unitsOptionsPage;

        private IOptionsPage viewOptionsPage;

        private IOptionsPage artboardOptionsPage;

        private IOptionsPage annotationsOptionsPage;

        private CommandTarget commandTarget;

        private ResourceDictionary designSurfaceIcons;

        public static ICollection<ITypeId> KnownGenericControlTypes
        {
            get
            {
                List<ITypeId> typeIds = new List<ITypeId>(9)
                {
                    PlatformTypes.Button,
                    PlatformTypes.CheckBox,
                    PlatformTypes.ComboBox,
                    PlatformTypes.ListBox,
                    PlatformTypes.RadioButton,
                    PlatformTypes.ScrollBar,
                    PlatformTypes.Slider,
                    ProjectNeutralTypes.TabControl,
                    ProjectNeutralTypes.GridSplitter
                };
                return typeIds;
            }
        }

        static DesignerPackage()
        {
            PropertyMetadata metadata = DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject));
            if (metadata == null || !(bool)metadata.DefaultValue)
            {
                FrameworkPropertyMetadata frameworkPropertyMetadatum = new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.Inherits);
                DesignerProperties.IsInDesignModeProperty.OverrideMetadata(typeof(DependencyObject), frameworkPropertyMetadatum);
            }
            DesignerPackage.RegisterAssembly("Microsoft.Expression.Framework");
            DesignerPackage.RegisterAssembly("Microsoft.Expression.DesignSurface");
            DesignerPackage.RegisterAssembly("Microsoft.Expression.Code");
            DesignerPackage.RegisterAssembly("Microsoft.Expression.Project");
        }

        public DesignerPackage()
        {
        }

        public void Dispose()
        {
            this.Unload();
        }

        public void Load(IServices services)
        {
            PerformanceUtility.StartPerformanceSequence(PerformanceEvent.DesignerPackageLoad);
            this.services = services;
            PropertyReference.RegisterAssemblyNamespace(typeof(DesignerPackage).Assembly, new string[] { "Microsoft.Expression.DesignSurface", "Microsoft.Expression.DesignSurface.Properties" });
            IWindowService service = this.services.GetService<IWindowService>();
            IProjectTypeManager projectTypeManager = this.services.GetService<IProjectTypeManager>();
            IDocumentTypeManager documentTypeManager = this.services.GetService<IDocumentTypeManager>();
            IOptionsDialogService optionsDialogService = this.services.GetService<IOptionsDialogService>();
            this.services.GetService<IProjectManager>();
            this.designSurfaceIcons = FileTable.GetResourceDictionary("Resources/Icons/DesignSurfaceIcons.xaml");
            service.AddResourceDictionary(this.designSurfaceIcons);
            SceneViewModel.RegisterPipelineTasks(this.services.GetService<ISchedulingService>());
            this.designerContext = new DesignerContext(this.services);
            this.designerContext.Initialize();
            this.services.AddService(typeof(SelectionManager), this.designerContext.SelectionManager);
            this.services.AddService(typeof(ToolManager), this.designerContext.ToolManager);
            this.services.AddService(typeof(SnappingEngine), this.designerContext.SnappingEngine);
            this.services.AddService(typeof(ICodeAidProvider), this.designerContext.CodeAidProvider);
            this.services.AddService(typeof(IAttachedPropertyMetadataFactory), new AttachedPropertyMetadataFactory());
            IOutOfBrowserDeploymentService outOfBrowserDeploymentService = new OutOfBrowserDeploymentService(this.services);
            this.services.AddService(typeof(IOutOfBrowserDeploymentService), outOfBrowserDeploymentService);
            this.services.AddService(typeof(IPlatformContextChanger), this.designerContext.PlatformContextChanger);
            PerformanceUtility.MarkInterimStep(PerformanceEvent.DesignerPackageLoad, "Registering Document Types");
            this.documentTypes.Add(new LicxDocumentType());
            this.documentTypes.Add(new SceneDocumentType(this.designerContext));
            this.documentTypes.Add(new ResourceDictionaryDocumentType(this.designerContext));
            this.documentTypes.Add(new ImageDocumentType(this.designerContext));
            this.documentTypes.Add(new IconImageDocumentType(this.designerContext));
            this.documentTypes.Add(new PngImageDocumentType(this.designerContext));
            this.documentTypes.Add(new JpgImageDocumentType(this.designerContext));
            this.documentTypes.Add(new GifImageDocumentType(this.designerContext));
            this.documentTypes.Add(new TifImageDocumentType(this.designerContext));
            this.documentTypes.Add(new FontDocumentType());
            this.documentTypes.Add(new WpfMediaDocumentType(this.designerContext));
            this.documentTypes.Add(new SilverlightAndWpfMediaDocumentType(this.designerContext));
            this.documentTypes.Add(new XapDocumentType());
            this.documentTypes.Add(new WavefrontObjAsset(this.designerContext));
            this.documentTypes.Add(new WavefrontMtlDocumentType(this.designerContext));
            this.documentTypes.Add(new ApplicationDefinitionDocumentType(this.designerContext));
            foreach (IDocumentType documentType in this.documentTypes)
            {
                documentTypeManager.Register(documentType);
            }
            PerformanceUtility.MarkInterimStep(PerformanceEvent.DesignerPackageLoad, "Initializing DesignTime Metadata Store");
            IDesignerDefaultPlatformService designerDefaultPlatformService = new DesignerDefaultPlatformService(this.services);
            this.services.AddService(typeof(IDesignerDefaultPlatformService), designerDefaultPlatformService);
            IHelpService helpService = new HelpService();
            this.services.AddService(typeof(IHelpService), helpService);
            CanonicalTransform3D.Initialize(this.designerContext);
            PerformanceUtility.MarkInterimStep(PerformanceEvent.DesignerPackageLoad, "Creating Managers");
            this.unitsOptionsPage = new UnitsOptionsPage(this.designerContext);
            optionsDialogService.OptionsPages.Add(this.unitsOptionsPage);
            this.artboardOptionsPage = new ArtboardOptionsPage(this.designerContext);
            optionsDialogService.OptionsPages.Add(this.artboardOptionsPage);
            this.viewOptionsPage = new ViewOptionsPage(this.designerContext);
            optionsDialogService.OptionsPages.Add(this.viewOptionsPage);
            this.annotationsOptionsPage = new AnnotationsOptionsPage(this.designerContext);
            optionsDialogService.OptionsPages.Add(this.annotationsOptionsPage);
            this.projectTypes.Add(new ExecutableProjectType());
            this.projectTypes.Add(new WindowsExecutableProjectType());
            this.projectTypes.Add(new WpfProjectType());
            this.projectTypes.Add(new SilverlightProjectType(services));
            foreach (IProjectType projectType in this.projectTypes)
            {
                projectTypeManager.Register(projectType);
            }
            IAssemblyService assemblyService = services.GetService<IAssemblyService>();
            this.silverLightAssemblyResolver = new SilverlightAssemblyResolver(AppDomain.CurrentDomain, services);
            assemblyService.RegisterPlatformResolver(".NETFramework", new ClrAssemblyResolver());
            assemblyService.RegisterPlatformResolver("Silverlight", this.silverLightAssemblyResolver);
            service.AddResourceDictionary(FileTable.GetResourceDictionary("Resources\\DesignSurfaceStyles.xaml"));
            PerformanceUtility.MarkInterimStep(PerformanceEvent.ApplicationStartup, "Opening Panels");
            PerformanceUtility.StartPerformanceSequence(PerformanceEvent.OpeningPanels);
            ToolContext toolContext = this.designerContext.ToolContext;
            PerformanceUtility.MarkInterimStep(PerformanceEvent.OpeningPanels, "Tools");
            service.RegisterPalette("Designer_ToolPane", new ToolPane(this.designerContext, toolContext), StringTable.ToolPaneTitle, null, new StandaloneViewProperties(false, true, false));
            ProjectPane projectPane = new ProjectPane(services);
            service.RegisterPalette("Designer_ProjectPane", projectPane, StringTable.ProjectPaneTitle);
            PerformanceUtility.MarkInterimStep(PerformanceEvent.OpeningPanels, "Data");
            service.RegisterPalette("Designer_DataPane", new DataPane(this.designerContext), StringTable.DataPaneTitle);
            PerformanceUtility.MarkInterimStep(PerformanceEvent.OpeningPanels, "Resources");
            service.RegisterPalette("Designer_ResourcePane", new ResourcePane(this.designerContext, this.designerContext.ProjectManager, this.designerContext.ResourceManager), StringTable.ResourcePaneTitle);
            PerformanceUtility.MarkInterimStep(PerformanceEvent.OpeningPanels, "PropertyInspector");
            service.RegisterPalette("Designer_PropertyInspector", new SceneNodePropertyInspectorPane(this.designerContext.PropertyInspectorModel), StringTable.PropertyInspectorPaneTitle);
            PerformanceUtility.MarkInterimStep(PerformanceEvent.OpeningPanels, "Timeline");
            service.RegisterPalette("Designer_TimelinePane", new TimelinePane(this.designerContext), StringTable.TimelinePaneTitle);
            PerformanceUtility.MarkInterimStep(PerformanceEvent.OpeningPanels, "Triggers");
            service.RegisterPalette("Designer_TriggersPane", new TriggersPane(this.designerContext), StringTable.TriggersPaneTitle);
            PerformanceUtility.MarkInterimStep(PerformanceEvent.OpeningPanels, "States");
            service.RegisterPalette("Interaction_Skin", new SkinView(this.designerContext), StringTable.SkinPaneTitle);
            PerformanceUtility.MarkInterimStep(PerformanceEvent.OpeningPanels, "Asset");
            service.RegisterPalette("Designer_AssetPane", new AssetPane(this.designerContext, toolContext), StringTable.AssetPaneTitle);
            PerformanceUtility.MarkInterimStep(PerformanceEvent.OpeningPanels, "Parts");
            service.RegisterPalette("Interaction_Parts", new PartsPane(this.designerContext), StringTable.PartsPaneTitle);
            PerformanceUtility.MarkInterimStep(PerformanceEvent.OpeningPanels, "Results");
            ResultsPane resultsPane = new ResultsPane(this.designerContext, new ProjectErrorTaskCollection(this.designerContext));
            KeyBinding keyBinding = new KeyBinding()
            {
                Key = Key.F12
            };
            resultsPane.Palette = service.RegisterPalette("Designer_ResultsPane", resultsPane, StringTable.ResultsPaneTitle, keyBinding);
            PerformanceUtility.EndPerformanceSequence(PerformanceEvent.OpeningPanels);
            this.services.RemoveService(typeof(IErrorService));
            this.services.RemoveService(typeof(IMessageLoggingService));
            this.services.AddService(typeof(IErrorService), resultsPane.ErrorManager);
            this.services.AddService(typeof(IMessageLoggingService), resultsPane.MessageLoggingService);
            this.designerContext.ErrorManager = resultsPane.ErrorManager;
            this.designerContext.MessageLoggingService = resultsPane.MessageLoggingService;
            PerformanceUtility.MarkInterimStep(PerformanceEvent.DesignerPackageLoad, "Adding Tools to ToolManager");
            this.tools.Add(new SelectionTool(toolContext));
            this.tools.Add(new SubselectionTool(toolContext));
            this.tools.Add(new RectangleTool(toolContext));
            this.tools.Add(new EllipseTool(toolContext));
            this.tools.Add(new LineTool(toolContext));
            this.tools.Add(new PenTool(toolContext));
            this.tools.Add(new PencilTool(toolContext));
            this.tools.Add(new PanTool(toolContext));
            this.tools.Add(new ZoomTool(toolContext));
            this.tools.Add(new EyedropperTool(toolContext));
            this.tools.Add(new PaintBucketTool(toolContext));
            this.tools.Add(new GradientBrushTool(toolContext));
            this.tools.Add(new BrushTransformTool(toolContext));
            this.tools.Add(new CameraOrbitTool(toolContext));
            foreach (ITypeId textToolType in TextTool.TextToolTypes)
            {
                this.tools.Add(new TextTool(toolContext, textToolType));
            }
            this.tools.Add(new GenericControlTool(toolContext, PlatformTypes.Grid, ToolCategory.LayoutPanels));
            this.tools.Add(new GenericControlTool(toolContext, PlatformTypes.Canvas, ToolCategory.LayoutPanels));
            this.tools.Add(new GenericControlTool(toolContext, PlatformTypes.StackPanel, ToolCategory.LayoutPanels));
            this.tools.Add(new GenericControlTool(toolContext, ProjectNeutralTypes.WrapPanel, ToolCategory.LayoutPanels));
            this.tools.Add(new GenericControlTool(toolContext, ProjectNeutralTypes.DockPanel, ToolCategory.LayoutPanels));
            this.tools.Add(new GenericControlTool(toolContext, PlatformTypes.ScrollViewer, ToolCategory.LayoutPanels));
            this.tools.Add(new GenericControlTool(toolContext, PlatformTypes.Border, ToolCategory.LayoutPanels));
            this.tools.Add(new GenericControlTool(toolContext, PlatformTypes.UniformGrid, ToolCategory.LayoutPanels));
            this.tools.Add(new GenericControlTool(toolContext, ProjectNeutralTypes.Viewbox, ToolCategory.LayoutPanels));
            foreach (ITypeId knownGenericControlType in DesignerPackage.KnownGenericControlTypes)
            {
                this.tools.Add(new GenericControlTool(toolContext, knownGenericControlType, ToolCategory.CommonControls));
            }
            foreach (Tool tool in this.tools)
            {
                this.designerContext.ToolManager.Add(tool);
            }
            this.sharedColorSpaceManager = new SharedColorSpaceManager(this.designerContext.Configuration);
            this.annotationService = new AnnotationService(this.designerContext);
            services.AddService(typeof(AnnotationService), this.annotationService);
            this.commandTarget = new CommandTarget();
            this.commandTarget.AddCommand("Application_AddNewItem", new AddNewItemCommand(this.designerContext, null));
            CommandTarget commandTarget = this.commandTarget;
            DesignerContext designerContext = this.designerContext;
            string[] strArrays = new string[] { "ResourceDictionary.xaml" };
            commandTarget.AddCommand("Application_AddNewResource", new AddNewItemCommand(designerContext, strArrays));
            this.commandTarget.AddCommand("Project_OpenView", new OpenViewCommand(this.designerContext));
            this.commandTarget.AddCommand("Project_EnablePlatformExtensions", new EnablePlatformExtensionsCommand(services));
            this.commandTarget.AddCommand("Project_EnableOutOfBrowser", new EnableApplicationOutsideBrowserCommand(services));
            this.commandTarget.AddCommand("Project_EnablePreviewOutOfBrowser", new EnablePreviewOutOfBrowserCommand(services));
            this.commandTarget.AddCommand("Project_EnableElevatedOutOfBrowser", new EnableElevatedOutOfBrowserCommand(services));
            this.commandTarget.AddCommand("Project_AddReference", new AddReferenceCommand(services));
            this.designerContext.CommandService.AddTarget(this.commandTarget);
            PerformanceUtility.EndPerformanceSequence(PerformanceEvent.DesignerPackageLoad);
            EventRouter.InitializeKeyboardHook(this.designerContext);
            XamlPerformanceEvents.RegisterEvents();
            UIThreadDispatcherHelper.Capture();
            ViewNodeManager.EnsureLayoutRequired += new EventHandler(this.ViewNodeManager_EnsureLayoutRequired);
            KnownProjectBase.MetadataStore = new DesignerPackage.MetadataStore();
            UIThreadDispatcher.Instance.BeginInvoke(DispatcherPriority.SystemIdle, new Action(FontResolver.CleanFontCache));
        }

        private static void RegisterAssembly(string assemblyName)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < (int)assemblies.Length; i++)
            {
                Assembly assembly = assemblies[i];
                if (AssemblyHelper.GetAssemblyName(assembly).Name == assemblyName)
                {
                    PlatformTypes.DesignToolAssemblies.Add(assembly);
                }
            }
        }

        public void Unload()
        {
            KnownProjectBase.MetadataStore = null;
            ViewNodeManager.EnsureLayoutRequired -= new EventHandler(this.ViewNodeManager_EnsureLayoutRequired);
            XamlPerformanceEvents.UnregisterEvents();
            this.services.RemoveService(typeof(AnnotationService));
            this.annotationService.Shutdown();
            IProjectTypeManager service = this.services.GetService<IProjectTypeManager>();
            IDocumentTypeManager documentTypeManager = this.services.GetService<IDocumentTypeManager>();
            IWindowService windowService = this.services.GetService<IWindowService>();
            IOptionsDialogService optionsDialogService = this.services.GetService<IOptionsDialogService>();
            this.services.GetService<ICommandService>().RemoveTarget(this.commandTarget);
            this.commandTarget = null;
            foreach (Tool tool in this.tools)
            {
                this.designerContext.ToolManager.Remove(tool);
            }
            this.services.RemoveService(typeof(ToolManager));
            this.services.RemoveService(typeof(SnappingEngine));
            this.services.RemoveService(typeof(IAttachedPropertyMetadataFactory));
            this.services.RemoveService(typeof(IOutOfBrowserDeploymentService));
            this.services.GetService<IDesignerDefaultPlatformService>().Dispose();
            this.services.RemoveService(typeof(IDesignerDefaultPlatformService));
            this.services.RemoveService(typeof(IHelpService));
            windowService.RemoveResourceDictionary(this.designSurfaceIcons);
            optionsDialogService.OptionsPages.Remove(this.unitsOptionsPage);
            optionsDialogService.OptionsPages.Remove(this.artboardOptionsPage);
            optionsDialogService.OptionsPages.Remove(this.viewOptionsPage);
            optionsDialogService.OptionsPages.Remove(this.annotationsOptionsPage);
            this.sharedColorSpaceManager.Unload();
            if (windowService.PaletteRegistry["Designer_ProjectPane"] != null)
            {
                ProjectPane content = (ProjectPane)windowService.PaletteRegistry["Designer_ProjectPane"].Content;
            }
            SceneViewModel.UnregisterPipelineTasks(this.services.GetService<ISchedulingService>());
            foreach (IProjectType projectType in this.projectTypes)
            {
                service.Unregister(projectType);
            }
            foreach (IDocumentType documentType in this.documentTypes)
            {
                documentTypeManager.Unregister(documentType);
            }
            this.projectTypes.Clear();
            this.documentTypes.Clear();
            this.designerContext.Uninitialize();
            this.designerContext = null;
            IAssemblyService assemblyService = this.services.GetService<IAssemblyService>();
            assemblyService.UnregisterPlatformResolver("Silverlight");
            assemblyService.UnregisterPlatformResolver(".NETFramework");
            this.silverLightAssemblyResolver.Dispose();
        }

        private void ViewNodeManager_EnsureLayoutRequired(object sender, EventArgs e)
        {
            ExceptionHandler.SafelyForceLayoutArrange();
        }

        private class MetadataStore : IMetadataStore
        {
            public void AddAttributeTable(AttributeTable value)
            {
                Microsoft.Expression.DesignModel.Metadata.MetadataStore.AddAttributeTable(value);
            }
        }
    }
}