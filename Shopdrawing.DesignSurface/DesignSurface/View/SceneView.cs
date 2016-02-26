// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.View.SceneView
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Code;
using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.InstanceBuilders;
using Microsoft.Expression.DesignModel.Markup;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.Text;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Annotations.Commands;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.SampleData;
using Microsoft.Expression.DesignSurface.SceneCommands;
using Microsoft.Expression.DesignSurface.Selection;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.Tools.Assets;
using Microsoft.Expression.DesignSurface.Tools.Text;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.UserInterface.DataPane;
using Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Framework.Commands.Undo;
using Microsoft.Expression.Framework.Data;
using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.UserInterface;
using Microsoft.Expression.Project.Build;
using Microsoft.Expression.Project.Documents;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using System.Windows.Threading;

namespace Microsoft.Expression.DesignSurface.View
{
  [DebuggerDisplay("{DebuggerDisplayValue}")]
  public abstract class SceneView : DocumentView, IElementProvider, INotifyPropertyChanged, IAttachViewRoot, ISetCaretPosition
  {
    private static int invisiblePanelEdgeTolerance = 4;
    private string buildMessage = string.Empty;
    private SceneView.MessageBubbleHost messageBubbleHost = new SceneView.MessageBubbleHost(15000);
    private bool allowInvalidatingCode = true;
    private Visibility? storedAdornersVisibility = new Visibility?();
    private List<SceneView.AppliedStoryboardData> appliedStoryboards = new List<SceneView.AppliedStoryboardData>();
    private List<DocumentNode> lastExceptionNodes = new List<DocumentNode>();
    private List<DocumentNode> lastWarningNodes = new List<DocumentNode>();
    private List<ViewContentValueProvider> currentValueProviders = new List<ViewContentValueProvider>();
    private Dictionary<SceneNode, Size> valueProviderSizeCache = new Dictionary<SceneNode, Size>();
    protected bool shouldRebuildAdornerSets = true;
    protected const double FallbackArtboardWidthAndHeight = 20.0;
    protected ISceneViewHost viewHost;
    protected SceneViewModel viewModel;
    protected INameScope rootNameScope;
    protected SceneView.HitTestHelper hitTestHelper;
    protected IViewVisual hitTestRoot;
    protected bool allowViewScoping;
    protected Microsoft.Expression.DesignSurface.ViewModel.InstanceBuilderContext instanceBuilderContext;
    protected object currentScopedInstance;
    protected ViewContentType currentContentType;
    protected SceneView.PostponedUpdate postponedUpdate;
    private SceneXamlDocument xamlDocument;
    private IDocumentRoot documentRoot;
    private ViewMode viewMode;
    private SceneEditTransaction currentTextEditTransaction;
    private SceneView.NautilusUndoUnit lastTextEditUndoUnit;
    private ViewOptionsModel viewOptionsModel;
    private FocusedEditor focusedEditor;
    private ErrorTaskCollection errors;
    private bool errorsInvalidated;
    private SceneViewTabControl rootElement;
    private SceneSplitView sceneSplitView;
    private object messageContent;
    private bool isDefaultMessageDisplayed;
    private bool isUpdating;
    private bool isQueryingForParentContextValue;
    private bool pendingSynchronousUpdate;
    private Microsoft.Expression.DesignSurface.UserInterface.SceneScrollViewer scrollViewer;
    private EventRouter eventRouter;
    private ExceptionFormatter exceptionFormatter;
    private SceneView.SelectionSynchronizer selectionSynchronizer;
    private BuildManager buildManager;
    private bool building;
    private DispatcherTimer textCommitTimeoutTimer;
    private bool initialized;
    private SceneView.LimitedFrequencyAutoScroller autoScroller;
    private SceneNode lastViewScope;
    private bool noInvisiblePanelStrokeHitTesting;
    private bool invisiblePanelContainerOfSelectionDisable;
    private bool pendingNeedUpdate;
    private bool pendingChangeRoot;
    private bool candidateEditingContainerChanged;
    private bool missingResourcesResolved;
    private bool needToUpdateViewContent;
    private bool suspendUpdates;
    private SceneView rootView;
    private SceneView.RefreshParentErrors refreshParentErrorsDelegate;
    private bool isDisposed;
    private DocumentNodeMarker lastRelativeViewNodeTargetMarker;
    private ITextEditor codeEditor;
    private SceneView.AppliedStoryboardData transitionFromStateStoryboardData;

    protected ExceptionFormatter ExceptionFormatter
    {
      get
      {
        return this.exceptionFormatter;
      }
    }

    protected abstract object ScopedInstance { get; }

    public abstract FrameworkElement ViewRootContainer { get; }

    public abstract IViewObject ViewRoot { get; }

    public abstract IViewVisual HitTestRoot { get; }

    public IPlatform Platform
    {
      get
      {
        return this.ProjectContext.Platform;
      }
    }

    protected SceneView RootView
    {
      get
      {
        return this.rootView;
      }
    }

    public bool IsEditingOutOfPlace
    {
      get
      {
        return this.rootView != null;
      }
    }

    public bool DeferViewStoryboard { get; set; }

    internal ViewStoryboardApplyOptions StoryboardApplyOptions
    {
      get
      {
        ViewStoryboardApplyOptions storyboardApplyOptions = ViewStoryboardApplyOptions.None;
        if (this.InstanceBuilderContext == null || this.ProjectContext == null || this.ProjectContext.IsCapabilitySet(PlatformCapability.IsWpf))
          return storyboardApplyOptions;
        IInstanceBuilderContext instanceBuilderContext = this.InstanceBuilderContext;
        if (instanceBuilderContext.EffectManager.DisableEffects)
          storyboardApplyOptions |= ViewStoryboardApplyOptions.DisableEffects;
        if (instanceBuilderContext.DisableProjectionTransforms)
          storyboardApplyOptions |= ViewStoryboardApplyOptions.DisableTransforms;
        return storyboardApplyOptions;
      }
    }

    public bool IsRootElementSizeFixed
    {
      get
      {
        if (this.ViewRoot != null && this.ViewRoot.PlatformSpecificObject != null && this.viewModel != null)
          return (bool) ((ReferenceStep) this.viewModel.ProjectContext.ResolveProperty(DesignTimeProperties.IsEnhancedOutOfPlaceRootProperty)).GetValue(this.ViewRoot.PlatformSpecificObject);
        return false;
      }
    }

    private DocumentNode FirstStaleUserControls
    {
      get
      {
        ICollection<ViewNode> controlInstances = this.InstanceBuilderContext.UserControlInstances;
        if (controlInstances == null || controlInstances.Count == 0)
          return (DocumentNode) null;
        foreach (ViewNode viewNode in (IEnumerable<ViewNode>) controlInstances)
        {
          if (viewNode.ChildContext != null)
            return viewNode.DocumentNode;
        }
        return (DocumentNode) null;
      }
    }

    public override object ActiveEditor
    {
      get
      {
        if (this.FocusedEditor == FocusedEditor.Code)
          return (object) this.CodeEditor;
        return base.ActiveEditor;
      }
    }

    public SceneDocument Document
    {
      get
      {
        return this.viewModel.Document;
      }
    }

    public IDocumentRoot DocumentRoot
    {
      get
      {
        return this.documentRoot;
      }
    }

    public bool IsClosing
    {
      get
      {
        return this.suspendUpdates;
      }
    }

    public SceneViewModel ViewModel
    {
      get
      {
        return this.viewModel;
      }
    }

    public FrameworkElement Element
    {
      get
      {
        return (FrameworkElement) this.rootElement;
      }
    }

    public SceneElementSelectionSet ElementSelectionSet
    {
      get
      {
        return this.viewModel.ElementSelectionSet;
      }
    }

    public IInstanceDictionary InstanceDictionary
    {
      get
      {
        return this.instanceBuilderContext.InstanceDictionary;
      }
    }

    public IInstanceBuilderContext RootInstanceBuilderContext
    {
      get
      {
        if (this.rootView == null)
          return (IInstanceBuilderContext) this.instanceBuilderContext;
        return (IInstanceBuilderContext) this.rootView.instanceBuilderContext;
      }
    }

    public IInstanceBuilderContext InstanceBuilderContext
    {
      get
      {
        return (IInstanceBuilderContext) this.instanceBuilderContext;
      }
    }

    public DocumentNodePath CandidateEditingContainer
    {
      get
      {
        return this.ViewNodeManager.CandidateEditingContainer;
      }
      set
      {
        if (this.ViewNodeManager.CandidateEditingContainer == value)
          return;
        this.ViewNodeManager.CandidateEditingContainer = value;
        this.candidateEditingContainerChanged = true;
      }
    }

    public virtual IViewPanel OverlayLayer
    {
      get
      {
        return this.Platform.ViewObjectFactory.Instantiate((object) this.Artboard.OverlayLayer) as IViewPanel;
      }
    }

    public Artboard Artboard
    {
      get
      {
        return this.scrollViewer.Artboard;
      }
    }

    public FeedbackLayer FeedbackLayer
    {
      get
      {
        return this.Artboard.FeedbackLayer;
      }
    }

    public Canvas LiveControlLayer
    {
      get
      {
        return this.Artboard.LiveControlLayer;
      }
    }

    public IProjectContext ProjectContext
    {
      get
      {
        return this.Document.ProjectContext;
      }
    }

    public bool UpdatesPostponed
    {
      get
      {
        if (this.ViewUpdateManager != null)
          return this.ViewUpdateManager.UpdatesPostponed;
        return false;
      }
    }

    internal DesignerContext DesignerContext
    {
      get
      {
        return this.viewModel.DesignerContext;
      }
    }

    internal ViewUpdateManager ViewUpdateManager
    {
      get
      {
        return this.DesignerContext.ViewUpdateManager;
      }
    }

    internal ViewNodeManager ViewNodeManager
    {
      get
      {
        return this.instanceBuilderContext.ViewNodeManager;
      }
    }

    private IExceptionDictionary ExceptionDictionary
    {
      get
      {
        return this.instanceBuilderContext.ExceptionDictionary;
      }
    }

    private IWarningDictionary WarningDictionary
    {
      get
      {
        return this.instanceBuilderContext.WarningDictionary;
      }
    }

    internal ICollection<SceneDocument> RelatedDocuments
    {
      get
      {
        return this.ViewUpdateManager.GetRelatedDocuments(this);
      }
    }

    public FrameworkElement SceneScrollViewer
    {
      get
      {
        return (FrameworkElement) this.scrollViewer;
      }
    }

    public IErrorTaskCollection Errors
    {
      get
      {
        return (IErrorTaskCollection) this.errors;
      }
    }

    public object MessageContent
    {
      get
      {
        SceneXamlDocument sceneXamlDocument = this.xamlDocument;
        if (!sceneXamlDocument.IsEditable)
        {
          if (sceneXamlDocument.AllErrorsFromMissingAssemblies)
            return (object) new DesignViewMessage(string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.DocumentRefersToUnbuiltProjectTypesHeader, new object[1]
            {
              (object) this.DesignerContext.ProjectManager.CurrentSolution.SolutionTypeDescription
            }), StringTable.DocumentRefersToUnbuiltProjectTypesDetails);
          if (sceneXamlDocument.ParseErrorsCount > 0)
            return (object) this.errors;
          if (sceneXamlDocument.UnresolvedTypes.Count > 0)
            return (object) new DesignViewMessage(string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.DocumentRefersToUnresolvedTypesHeader, new object[1]
            {
              (object) this.Document.DocumentReference.DisplayName
            }), string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.DocumentRefersToUnresolvedTypesDetails, new object[1]
            {
              (object) SceneView.GetUnresolvedTypesList(sceneXamlDocument.UnresolvedTypes)
            }));
        }
        if (!this.isDefaultMessageDisplayed)
          return this.messageContent;
        return (object) new DesignViewMessage(string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.CannotEditXamlContentsDirectlyHeader, new object[1]
        {
          (object) this.Document.DocumentReference.DisplayName
        }), StringTable.CannotEditXamlContentsDirectlyDetails);
      }
      set
      {
        object messageContent = this.MessageContent;
        this.messageContent = value;
        if (object.Equals(messageContent, this.MessageContent))
          return;
        this.OnMessageContentChanged();
      }
    }

    protected bool IsDefaultMessageDisplayed
    {
      get
      {
        return this.isDefaultMessageDisplayed;
      }
      set
      {
        if (this.isDefaultMessageDisplayed == value)
          return;
        object messageContent = this.MessageContent;
        this.isDefaultMessageDisplayed = value;
        if (object.Equals(messageContent, this.MessageContent))
          return;
        this.OnMessageContentChanged();
      }
    }

    public bool IsValid
    {
      get
      {
        if (this.Document.IsEditable)
          return this.messageContent == null;
        return false;
      }
    }

    public bool IsUpdating
    {
      get
      {
        return this.isUpdating;
      }
    }

    public bool IsEditable
    {
      get
      {
        return this.MessageContent == null;
      }
    }

    public FocusedEditor FocusedEditor
    {
      get
      {
        return this.focusedEditor;
      }
      set
      {
        if (this.focusedEditor == value)
          return;
        this.focusedEditor = value;
        if (this.focusedEditor != FocusedEditor.Design || this.IsEditable)
          return;
        this.OnMessageContentChanged();
      }
    }

    public ViewMode ViewMode
    {
      get
      {
        return this.viewMode;
      }
      set
      {
        if (this.viewMode == value)
          return;
        PerformanceUtility.MeasurePerformanceUntilRender(PerformanceEvent.ChangeViewMode);
        using (TemporaryCursor.SetWaitCursor())
        {
          DesignerContext designerContext = this.DesignerContext;
          bool flag = designerContext.ViewService.ActiveView == this && (this.viewMode != ViewMode.Design || value != ViewMode.Split);
          if (flag)
            designerContext.ViewService.ActiveView = (IView) null;
          this.viewMode = value;
          this.OnPropertyChanged("ViewMode");
          this.sceneSplitView.UpdateSplitViewState(this.rootElement.VerticalScrollBarMargin);
          if (this.viewMode == ViewMode.Code)
          {
            this.FocusedEditor = FocusedEditor.Code;
            this.RefreshCodeArea();
            UIThreadDispatcher.Instance.BeginInvoke(DispatcherPriority.Render, (Action) (() => this.CodeEditor.EnsureCaretVisible()));
          }
          else if (this.viewMode == ViewMode.Design)
          {
            this.FocusedEditor = FocusedEditor.Design;
            this.RefreshDesignArea();
            this.RebuildAdornerSetsAsync();
          }
          else if (this.viewMode == ViewMode.Split)
          {
            this.FocusedEditor = FocusedEditor.Design;
            this.RefreshDesignArea();
            this.RebuildAdornerSetsAsync();
            this.RefreshCodeArea();
            this.selectionSynchronizer.EnsureXamlSynchronizedToScene(true);
          }
          if (flag)
            designerContext.ViewService.ActiveView = (IView) this;
          this.RestoreFocusAsync();
          this.AddCommands();
        }
      }
    }

    public bool IsDesignSurfaceEnabled
    {
      get
      {
        if (this.IsDesignSurfaceVisible)
          return this.IsEditable;
        return false;
      }
    }

    public bool IsDesignSurfaceVisible
    {
      get
      {
        if (this.ViewMode != ViewMode.Split)
          return this.ViewMode == ViewMode.Design;
        return true;
      }
    }

    public System.Windows.Input.ICommand ViewCodeCommand
    {
      get
      {
        return (System.Windows.Input.ICommand) new DelegateCommand((DelegateCommand.SimpleEventHandler) (() =>
        {
          this.EnsureXamlEditorVisible();
          if (this.errors.Count <= 0)
            return;
          (this.errors[0] as SceneView.ISceneErrorTask).InvokeCommand.Execute((object) null);
        }));
      }
    }

    public object DesignSurfaceRoot
    {
      get
      {
        if (this.ViewNodeManager.Root != null)
          return this.ViewNodeManager.Root.Instance;
        return (object) null;
      }
    }

    public double CenterX
    {
      get
      {
        return this.Artboard.CenterX;
      }
      set
      {
        this.Artboard.CenterX = value;
      }
    }

    public double CenterY
    {
      get
      {
        return this.Artboard.CenterY;
      }
      set
      {
        this.Artboard.CenterY = value;
      }
    }

    public double Zoom
    {
      get
      {
        return this.Artboard.Zoom;
      }
      set
      {
        this.Artboard.Zoom = value;
      }
    }

    public bool CanZoomIn
    {
      get
      {
        return this.Artboard.CanZoomIn;
      }
    }

    public bool CanZoomOut
    {
      get
      {
        return this.Artboard.CanZoomOut;
      }
    }

    public EventRouter EventRouter
    {
      get
      {
        return this.eventRouter;
      }
    }

    public AdornerLayer AdornerLayer
    {
      get
      {
        return this.Artboard.AdornerLayer;
      }
    }

    public AdornerService AdornerService
    {
      get
      {
        return this.Artboard.AdornerService;
      }
    }

    public FrameworkElement ServiceRoot
    {
      get
      {
        return (FrameworkElement) this.Artboard;
      }
    }

    public ITextEditor CodeEditor
    {
      get
      {
        if (this.codeEditor == null && this.allowViewScoping)
        {
          if (this.Document.IsPreviewDocument)
          {
            this.codeEditor = (ITextEditor) new SceneView.EmptyCodeEditor();
          }
          else
          {
            this.codeEditor = this.DesignerContext.TextEditorService.CreateTextEditor(this.xamlDocument.TextBuffer);
            if (this.codeEditor != null)
            {
              this.codeEditor.GotFocus += new EventHandler(this.CodeEditor_GotFocus);
              this.codeEditor.LostFocus += new EventHandler(this.CodeEditor_LostFocus);
              this.codeEditor.CaretPositionChanged += new EventHandler(this.CodeEditor_CaretPositionChanged);
            }
          }
        }
        return this.codeEditor;
      }
    }

    public bool IsBuilding
    {
      get
      {
        return this.building;
      }
    }

    public string BuildMessage
    {
      get
      {
        return this.buildMessage;
      }
    }

    public Visibility? OverrideAdornerLayerVisibility
    {
      get
      {
        if (!this.storedAdornersVisibility.HasValue)
          return new Visibility?();
        return new Visibility?(this.AdornerLayer.Visibility);
      }
      set
      {
        if (!value.HasValue)
        {
          if (!this.storedAdornersVisibility.HasValue)
            return;
          this.AdornerLayer.Visibility = this.storedAdornersVisibility.Value;
          this.storedAdornersVisibility = new Visibility?();
        }
        else
        {
          if (!this.storedAdornersVisibility.HasValue)
            this.storedAdornersVisibility = new Visibility?(this.AdornerLayer.Visibility);
          this.AdornerLayer.Visibility = value.Value;
        }
      }
    }

    public bool NoInvisiblePanelStrokeHitTesting
    {
      get
      {
        return this.noInvisiblePanelStrokeHitTesting;
      }
      set
      {
        this.noInvisiblePanelStrokeHitTesting = value;
      }
    }

    public bool InvisiblePanelContainerOfSelectionDisable
    {
      get
      {
        return this.invisiblePanelContainerOfSelectionDisable;
      }
      set
      {
        this.invisiblePanelContainerOfSelectionDisable = value;
      }
    }

    internal bool ShuttingDown
    {
      get
      {
        if (!this.IsClosing && this.viewModel != null)
          return this.DesignerContext == null;
        return true;
      }
    }

    private ViewNode ScopedViewNode
    {
      get
      {
        if (this.viewModel == null)
          return (ViewNode) null;
        SceneNode sceneNode = this.allowViewScoping ? this.viewModel.ViewRoot : this.viewModel.RootNode;
        if (sceneNode == null)
          return (ViewNode) null;
        ViewNode viewNode = (ViewNode) null;
        this.ViewNodeManager.TryGetCorrespondingViewNode(sceneNode.DocumentNodePath, out viewNode);
        return viewNode;
      }
    }

    private IViewVisual StoryboardContainerTargetViewVisual
    {
      get
      {
        IStoryboardContainer storyboardContainer = this.viewModel.ActiveStoryboardContainer;
        FrameworkTemplateElement frameworkTemplateElement = storyboardContainer as FrameworkTemplateElement;
        if (frameworkTemplateElement != null)
          return frameworkTemplateElement.ViewTargetElement as IViewVisual;
        SceneNode sceneNode = (SceneNode) storyboardContainer.TargetElement;
        if (sceneNode != null)
          return (IViewVisual) sceneNode.ViewObject;
        return (IViewVisual) null;
      }
    }

    ViewNode IAttachViewRoot.ViewRoot
    {
      get
      {
        return this.ScopedViewNode;
      }
    }

    private object CurrentRootException
    {
      get
      {
        Exception exception = this.ViewNodeManager.Root == null || this.ViewNodeManager.Root.InstanceState == InstanceState.Valid ? (Exception) null : this.ExceptionDictionary.GetException(this.ViewNodeManager.Root);
        if (exception != null)
          return (object) this.exceptionFormatter.Format(exception);
        return (object) null;
      }
    }

    protected XamlDocument ApplicationDocumentRoot
    {
      get
      {
        SceneDocument applicationSceneDocument = this.Document.ApplicationSceneDocument;
        if (applicationSceneDocument == null)
          return (XamlDocument) null;
        return (XamlDocument) applicationSceneDocument.XamlDocument;
      }
    }

    protected bool ShouldActivate
    {
      get
      {
        if (this.IsClosing)
          return false;
        if (this.DesignerContext.PlatformContextChanger != null)
          return this.DesignerContext.PlatformContextChanger.AllowViewActivation;
        return true;
      }
    }

    private string DebuggerDisplayValue
    {
      get
      {
        return string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}:{1}{2}", (object) this.ProjectContext.ProjectName, (object) DocumentReference.Create(this.ProjectContext.ProjectPath).GetRelativePath(this.Document.DocumentReference), this.IsEditingOutOfPlace ? (object) " [OOP]" : (object) string.Empty);
      }
    }

    public SampleDataCollection SampleData
    {
      get
      {
        return ProjectXamlContext.FromProjectContext(this.ProjectContext).SampleData;
      }
    }

    private event EventHandler updated;

    public event EventHandler Updated
    {
      add
      {
        this.updated += value;
      }
      remove
      {
        this.updated -= value;
      }
    }

    protected SceneView(ISceneViewHost viewHost, SceneViewModel viewModel, bool isRootView)
      : base((IDocument) viewModel.Document)
    {
      PerformanceUtility.StartPerformanceSequence(PerformanceEvent.SceneViewConstructor);
      this.viewModel = viewModel;
      this.xamlDocument = this.Document.XamlDocument;
      this.documentRoot = this.Document.DocumentRoot;
      this.allowViewScoping = isRootView;
      this.viewHost = viewHost;
      this.viewOptionsModel = this.DesignerContext.ViewOptionsModel;
      this.viewMode = this.viewOptionsModel == null ? ViewMode.Split : this.viewOptionsModel.ViewMode;
      this.focusedEditor = this.viewMode == ViewMode.Code ? FocusedEditor.Code : FocusedEditor.Design;
      this.exceptionFormatter = new ExceptionFormatter(this.ProjectContext);
      if (isRootView)
      {
        if (this.DesignerContext.WindowService != null)
          this.DesignerContext.WindowService.StateChanged += new EventHandler(this.WindowService_StateChanged);
        this.hitTestHelper = new SceneView.HitTestHelper(this);
        if (this.viewModel != null && this.DesignerContext != null)
          this.DesignerContext.ArtboardOptionsChanged += new EventHandler(this.OnArtboardOptionsChanged);
      }
      this.scrollViewer = new Microsoft.Expression.DesignSurface.UserInterface.SceneScrollViewer(this, this.CreateArtboard());
      this.scrollViewer.Focusable = true;
      if (isRootView)
      {
        this.scrollViewer.GotFocus += new RoutedEventHandler(this.DesignSurface_GotFocus);
        this.Artboard.GotFocus += new RoutedEventHandler(this.DesignSurface_GotFocus);
        this.Artboard.ZoomChanged += new EventHandler(this.Artboard_ZoomChanged);
        this.sceneSplitView = new SceneSplitView(this, this.viewOptionsModel, this.scrollViewer, this.CodeEditor);
        this.rootElement = new SceneViewTabControl((FrameworkElement) this.sceneSplitView);
        this.rootElement.DataContext = (object) this;
        this.rootElement.PropertyChanged += new PropertyChangedEventHandler(this.OnRootElementPropertyChanged);
        if (this.viewOptionsModel != null)
          this.viewOptionsModel.PropertyChanged += new PropertyChangedEventHandler(this.OnViewOptionsModelPropertyChanged);
        this.sceneSplitView.UpdateSplitViewState(this.rootElement.VerticalScrollBarMargin);
        this.eventRouter = new EventRouter(this);
      }
      this.AdornerLayer.SceneView = this;
      this.scrollViewer.SetValue(AutomationElement.IdProperty, (object) this.Document.DocumentReference.Path);
      PerformanceUtility.MarkInterimStep(PerformanceEvent.SceneViewConstructor, "Created scene scroll viewer.");
      this.ViewUpdateManager.Register(this);
      PerformanceUtility.MarkInterimStep(PerformanceEvent.SceneViewConstructor, "Added handlers.");
      this.rootNameScope = this.CreateNameScope();
      IDocumentContext documentContext = this.Document.DocumentContext;
      this.instanceBuilderContext = new Microsoft.Expression.DesignSurface.ViewModel.InstanceBuilderContext(this.Platform, (IDocumentRootResolver) new SceneView.DocumentRootResolver(this), this.Platform.Metadata.TypeMetadataFactory, this.viewModel, this.rootNameScope, (Microsoft.Expression.DesignSurface.ViewModel.InstanceBuilderContext) null, true, this.DesignerContext.TextBufferService, (DocumentNode) null);
      if (this.DesignerContext.ArtboardOptionsModel != null)
        this.instanceBuilderContext.EffectManager.DisableEffects = !this.DesignerContext.ArtboardOptionsModel.EffectsEnabled;
      if (isRootView && this.viewModel.DefaultView == null)
        this.viewModel.DefaultView = this;
      this.errors = new ErrorTaskCollection();
      if (isRootView)
      {
        this.AddCommands();
        PerformanceUtility.MarkInterimStep(PerformanceEvent.SceneViewConstructor, "Added commands.");
      }
      this.buildManager = this.DesignerContext.ProjectManager.BuildManager;
      if (this.buildManager != null)
      {
        this.buildManager.BuildStarting += new EventHandler<BuildStartingEventArgs>(this.BuildManager_BuildStarting);
        this.buildManager.BuildCompleted += new EventHandler<BuildCompletedEventArgs>(this.BuildManager_BuildCompleted);
      }
      this.xamlDocument.PropertyChanged += new PropertyChangedEventHandler(this.XamlDocument_PropertyChanged);
      if (isRootView)
      {
        this.xamlDocument.TextChanged += new EventHandler(this.XamlDocument_TextChanged);
        ITextBufferUndo textBufferUndo = this.xamlDocument.TextBuffer as ITextBufferUndo;
        if (textBufferUndo != null)
          textBufferUndo.UndoUnitAdded += new EventHandler<TextUndoCompletedEventArgs>(this.OnXamlDocumentTextBufferUndoUnitAdded);
      }
      this.selectionSynchronizer = new SceneView.SelectionSynchronizer(this);
      PropertyCacheHelper.PopulateDocumentSpecificPropertyCache(this.ViewModel);
      PerformanceUtility.EndPerformanceSequence(PerformanceEvent.SceneViewConstructor);
    }

    protected abstract Artboard CreateArtboard();

    protected abstract INameScope CreateNameScope();

    protected abstract SceneView CreateSceneView();

    protected abstract void SetApplicationResourceDictionary(ResourceDictionary resources);

    public abstract void InvalidateAndUpdateApplicationResourceDictionary();

    public abstract void RefreshApplicationResourceDictionary();

    protected abstract bool SetViewContentInternal(ViewContentType contentType, ViewNode target, object content);

    public abstract void EnsureVisible(IViewObject element, bool scrollNow);

    public abstract void EnsureVisible(IAdorner adorner, bool scrollNow);

    public abstract object ConvertToWpfValue(object obj);

    public abstract object ConvertFromWpfValue(object obj);

    public abstract DocumentNode ConvertToWpfValueAsDocumentNode(object obj);

    public abstract DocumentNode ConvertFromWpfValueAsDocumentNode(object obj);

    public abstract System.Windows.Media.Geometry GetRenderedGeometryAsWpf(SceneElement shapeElement);

    public abstract bool IsMatrixTransform(IViewObject from, IViewObject to);

    public abstract Point TransformPoint(IViewObject from, IViewObject to, Point point);

    public abstract Rect TransformBounds(IViewObject from, IViewObject to, Rect bounds);

    public abstract Matrix GetComputedTransformToRootVerified(IViewObject element);

    public abstract Matrix GetComputedTransformToRoot(IViewObject element);

    public abstract Matrix GetComputedTransformFromRoot(IViewObject element);

    public abstract GeneralTransform ComputeTransformToVisual(IViewObject from, IViewObject to);

    public abstract GeneralTransform ComputeTransformToVisual(IViewObject from, Visual to);

    public abstract GeneralTransform GetComputedTransformToHitArea();

    public abstract double GetDefinitionActualSize(IViewObject definition);

    public abstract Rect GetActualBounds(IViewObject element);

    public abstract Rect GetActualBoundsInParent(IViewObject element);

    public abstract Size GetRenderSize(IViewObject element);

    public abstract Point PointToScreen(IViewObject relative, Point point);

    public abstract Point PointFromScreen(IViewObject relative, Point point);

    public abstract Viewport3DElement GetFirstHitViewport3D(Point point);

    public abstract ImageCapturer GetCapturer(Size targetSize);

    public override void Initialize()
    {
      if (this.initialized)
      {
        this.viewOptionsModel = this.DesignerContext.ViewOptionsModel;
        this.ViewMode = this.viewOptionsModel == null ? ViewMode.Split : this.viewOptionsModel.ViewMode;
      }
      else
      {
        this.initialized = true;
        this.InvalidateForRootChanged();
        this.postponedUpdate = SceneView.PostponedUpdate.UpdateInvalidated;
        base.Initialize();
      }
    }

    private void InitializeInternal(bool shouldInvalidateAndUpdate)
    {
      this.initialized = true;
      this.RefreshErrors();
      if (!shouldInvalidateAndUpdate)
        return;
      this.InvalidateAndUpdate(true);
      this.InvalidateAndUpdateApplicationResourceDictionary();
      this.UpdateLayout();
      this.Artboard.CenterAll();
    }

    private void ResolveMissingResourcesAsync()
    {
      if (this.missingResourcesResolved || this.IsClosing || this != this.DesignerContext.ActiveView)
        return;
      this.ResolveMissingResourcesOnce(new DesignTimeResourceResolverContext(this));
    }

    internal bool ResolveMissingResourcesOnce(DesignTimeResourceResolverContext resourceResolverContext)
    {
      if (this.missingResourcesResolved || this.IsClosing || resourceResolverContext.IsCanceled)
        return false;
      this.missingResourcesResolved = true;
      bool flag = false;
      while (this.ResolveMissingResourcesOnceInternal(resourceResolverContext))
      {
        flag = true;
        if (resourceResolverContext.IsCanceled)
          break;
      }
      if (!resourceResolverContext.IsCanceled && this.ViewUpdateManager.ResolveRelatedMissingResources(this, resourceResolverContext))
      {
        this.UpdateReferences();
        flag = true;
      }
      return flag;
    }

    private bool ResolveMissingResourcesOnceInternal(DesignTimeResourceResolverContext resourceResolverContext)
    {
      IInstanceBuilderContext instanceBuilderContext = this.InstanceBuilderContext;
      if (instanceBuilderContext.WarningDictionary.Count == 0)
        return false;
      bool flag1 = false;
      List<DocumentNode> list = new List<DocumentNode>(instanceBuilderContext.WarningDictionary.Keys);
      ExpressionEvaluator expressionEvaluator = new ExpressionEvaluator(this.InstanceBuilderContext.DocumentRootResolver);
      for (int index = 0; index < list.Count && !resourceResolverContext.IsCanceled; ++index)
      {
        DocumentCompositeNode documentCompositeNode = list[index] as DocumentCompositeNode;
        if (documentCompositeNode != null && documentCompositeNode.Type.IsResource)
        {
          if (documentCompositeNode.DocumentRoot == null)
          {
            flag1 = true;
          }
          else
          {
            DocumentPrimitiveNode documentPrimitiveNode = documentCompositeNode.Properties[ResourceNodeHelper.GetResourceProperty((DocumentNode) documentCompositeNode)] as DocumentPrimitiveNode;
            if (documentPrimitiveNode != null)
            {
              DocumentNodePath nodePath = new DocumentNodePath(documentCompositeNode.DocumentRoot.RootNode, (DocumentNode) documentCompositeNode);
              bool flag2;
              for (flag2 = expressionEvaluator.EvaluateResource(nodePath, (DocumentNode) documentPrimitiveNode) == null; flag2; flag2 = expressionEvaluator.EvaluateResource(nodePath, (DocumentNode) documentPrimitiveNode) == null)
              {
                string missingResourceName = documentPrimitiveNode.GetValue<string>();
                if (!resourceResolverContext.ResourceResolver.Resolve(instanceBuilderContext.DocumentContext, missingResourceName))
                {
                  resourceResolverContext.IsCanceled = true;
                  break;
                }
                flag1 = true;
              }
              if (!flag2)
                flag1 = true;
              else
                break;
            }
          }
        }
      }
      if (flag1)
        this.InvalidateAndUpdate();
      return flag1;
    }

    public void Reset()
    {
      this.suspendUpdates = false;
      this.ViewModel.Reset();
      this.Zoom = 1.0;
      this.ClearAdornerSets();
      this.Document.XamlDocument.UndoService.Clear();
    }

    public void SuspendUpdatesForViewShutdown()
    {
      this.suspendUpdates = true;
    }

    public IViewStoryboard CreateViewStoryboard(TimelineSceneNode timeline)
    {
      using (this.ViewModel.ForceUseShadowProperties())
        return this.DeferViewStoryboard ? (IViewStoryboard) null : this.Platform.ViewObjectFactory.Instantiate(timeline.CreateInstance()) as IViewStoryboard;
    }

    public void StopAllStateTransitions()
    {
      IViewVisual targetViewVisual = this.StoryboardContainerTargetViewVisual;
      if (targetViewVisual == null)
        return;
      SceneElement sceneElement = VisualStateManagerSceneNode.GetHostNode(this.ViewModel.ActiveEditingContainer) as SceneElement;
      ViewStoryboardApplyOptions storyboardApplyOptions = this.StoryboardApplyOptions;
      targetViewVisual.StopAllStateTransitions(sceneElement.ViewObject as IViewVisual, VisualStateManagerSceneNode.CanOnlySupportExtendedVisualStateManager(this.ViewModel.ActiveEditingContainer), (ITypeResolver) this.ProjectContext, storyboardApplyOptions);
    }

    public bool SimulateGoToState(VisualStateSceneNode state, bool playTransition)
    {
      return this.SimulateGoToState(state, playTransition, (TransitionCompletedCallback) (() =>
      {
        if (this.viewModel == null)
          return;
        this.viewModel.ActivateState(state);
      }));
    }

    public bool SimulateGoToState(VisualStateSceneNode state, bool playTransition, TransitionCompletedCallback completedCallback)
    {
      IViewVisual targetViewVisual = this.StoryboardContainerTargetViewVisual;
      if (state == null || targetViewVisual == null)
        return false;
      StateTransitionPreviewInfo info = completedCallback == null ? (StateTransitionPreviewInfo) null : new StateTransitionPreviewInfo(completedCallback);
      SceneElement sceneElement = VisualStateManagerSceneNode.GetHostNode(this.ViewModel.ActiveEditingContainer) as SceneElement;
      ViewStoryboardApplyOptions storyboardApplyOptions = this.StoryboardApplyOptions;
      return targetViewVisual.SimulateGoToState(sceneElement.ViewObject as IViewVisual, state.Name, state.Parent.Name, playTransition, VisualStateManagerSceneNode.CanOnlySupportExtendedVisualStateManager(this.ViewModel.ActiveEditingContainer), info, (ITypeResolver) this.Document.ProjectContext, storyboardApplyOptions);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (this.isDisposed)
          return;
        this.isDisposed = true;
        this.suspendUpdates = true;
        if (this.viewModel != null)
        {
          if (this.viewModel.DefaultView == this)
            this.viewModel.NotifyClosing();
          if (this.refreshParentErrorsDelegate != null)
            this.refreshParentErrorsDelegate = (SceneView.RefreshParentErrors) null;
          if (this.rootView != null)
            this.rootView.Dispose();
          this.rootView = (SceneView) null;
          if (this.codeEditor != null)
          {
            this.codeEditor.GotFocus -= new EventHandler(this.CodeEditor_GotFocus);
            this.codeEditor.LostFocus -= new EventHandler(this.CodeEditor_LostFocus);
            this.codeEditor.CaretPositionChanged -= new EventHandler(this.CodeEditor_CaretPositionChanged);
            this.codeEditor.Dispose();
            this.codeEditor = (ITextEditor) null;
          }
          this.ClearCommands();
          if (this.eventRouter != null)
          {
            this.eventRouter.TearDown();
            this.eventRouter = (EventRouter) null;
          }
          if (this.viewOptionsModel != null)
          {
            this.viewOptionsModel.PropertyChanged -= new PropertyChangedEventHandler(this.OnViewOptionsModelPropertyChanged);
            this.viewOptionsModel = (ViewOptionsModel) null;
          }
          if (this.allowViewScoping)
          {
            if (this.DesignerContext.WindowService != null)
              this.DesignerContext.WindowService.StateChanged -= new EventHandler(this.WindowService_StateChanged);
            this.hitTestHelper = (SceneView.HitTestHelper) null;
            if (this.viewModel != null && this.DesignerContext != null)
              this.DesignerContext.ArtboardOptionsChanged -= new EventHandler(this.OnArtboardOptionsChanged);
            this.scrollViewer.GotFocus -= new RoutedEventHandler(this.DesignSurface_GotFocus);
            this.Artboard.GotFocus -= new RoutedEventHandler(this.DesignSurface_GotFocus);
            this.Artboard.ZoomChanged -= new EventHandler(this.Artboard_ZoomChanged);
          }
          this.xamlDocument.PropertyChanged -= new PropertyChangedEventHandler(this.XamlDocument_PropertyChanged);
          if (this.allowViewScoping)
          {
            this.xamlDocument.TextChanged -= new EventHandler(this.XamlDocument_TextChanged);
            ITextBufferUndo textBufferUndo = this.xamlDocument.TextBuffer as ITextBufferUndo;
            if (textBufferUndo != null)
              textBufferUndo.UndoUnitAdded -= new EventHandler<TextUndoCompletedEventArgs>(this.OnXamlDocumentTextBufferUndoUnitAdded);
          }
          if (this.buildManager != null)
          {
            this.buildManager.BuildStarting -= new EventHandler<BuildStartingEventArgs>(this.BuildManager_BuildStarting);
            this.buildManager.BuildCompleted -= new EventHandler<BuildCompletedEventArgs>(this.BuildManager_BuildCompleted);
            this.buildManager = (BuildManager) null;
          }
          if (this.sceneSplitView != null)
          {
            this.sceneSplitView.Dispose();
            this.sceneSplitView = (SceneSplitView) null;
          }
          this.selectionSynchronizer.Unhook();
          this.selectionSynchronizer = (SceneView.SelectionSynchronizer) null;
          this.ViewUpdateManager.Unregister(this);
          this.SetApplicationResourceDictionary((ResourceDictionary) null);
          this.AdornerLayer.TearDown();
          this.scrollViewer.TearDown();
          if (this.viewModel.DefaultView == this)
          {
            this.viewModel.Close();
            this.viewModel = (SceneViewModel) null;
          }
          if (this.rootElement != null)
          {
            this.rootElement.PropertyChanged -= new PropertyChangedEventHandler(this.OnRootElementPropertyChanged);
            this.rootElement.DataContext = (object) null;
            this.rootElement = (SceneViewTabControl) null;
          }
          if (this.instanceBuilderContext != null && this.ViewNodeManager != null)
            this.instanceBuilderContext.Dispose();
          this.lastExceptionNodes = (List<DocumentNode>) null;
          this.lastWarningNodes = (List<DocumentNode>) null;
          this.instanceBuilderContext = (Microsoft.Expression.DesignSurface.ViewModel.InstanceBuilderContext) null;
          this.scrollViewer = (Microsoft.Expression.DesignSurface.UserInterface.SceneScrollViewer) null;
          this.documentRoot = (IDocumentRoot) null;
          this.xamlDocument = (SceneXamlDocument) null;
        }
      }
      base.Dispose(disposing);
    }

    private void RefreshDesignArea()
    {
      if (this.ShuttingDown)
        return;
      this.CommitTextEdits();
    }

    private void RefreshCodeArea()
    {
      int num = this.ShuttingDown ? true : false;
    }

    protected void RefreshErrors()
    {
      this.RefreshErrors(false);
    }

    private void RefreshErrors(bool forceShowErrorsWhenXamlFocused)
    {
      if (this.ShuttingDown || this.isDisposed || this.DesignerContext.ViewRootResolver.IsViewHidden(this))
        return;
      if (!this.allowViewScoping)
      {
        if (this.refreshParentErrorsDelegate == null)
          return;
        this.refreshParentErrorsDelegate(forceShowErrorsWhenXamlFocused);
      }
      else
      {
        int count = this.errors.Count;
        this.lastExceptionNodes = new List<DocumentNode>(this.instanceBuilderContext.ExceptionDictionary.Keys);
        this.lastWarningNodes = new List<DocumentNode>(this.instanceBuilderContext.WarningDictionary.Keys);
        this.errors.Clear();
        this.errorsInvalidated = false;
        ProjectXamlContext projectContext = ProjectXamlContext.FromProjectContext(this.ProjectContext);
        foreach (XamlParseError error in (IEnumerable<XamlParseError>) this.xamlDocument.ParseErrorsAndWarnings)
          this.errors.Add((IErrorTask) new SceneView.ErrorTask(projectContext, this.ProjectContext.ProjectName, this.Document.DocumentReference.Path, error));
        foreach (SceneDocument sceneDocument in (IEnumerable<SceneDocument>) this.RelatedDocuments)
        {
          if (sceneDocument.XamlDocument != this.xamlDocument)
          {
            foreach (XamlParseError error in (IEnumerable<XamlParseError>) sceneDocument.XamlDocument.ParseErrorsAndWarnings)
              this.errors.Add((IErrorTask) new SceneView.ErrorTask(projectContext, sceneDocument.ProjectContext.ProjectName, sceneDocument.DocumentContext.DocumentUrl, error));
          }
        }
        int num = 0;
        if (this.Document.IsEditable)
        {
          this.AddExceptionsToDictionary(this);
          if (this.rootView != null)
            this.AddExceptionsToDictionary(this.rootView);
          if (this.currentContentType == ViewContentType.Error && this.xamlDocument.RootNode != null && this.messageContent != null)
          {
            FormattedException formattedException = this.messageContent as FormattedException;
            string description = formattedException != null ? formattedException.SourceException.Message : this.messageContent.ToString();
            InstanceBuilderException builderException = formattedException != null ? formattedException.SourceException as InstanceBuilderException : (InstanceBuilderException) null;
            this.errors.Add((IErrorTask) new SceneView.DocumentErrorTask((builderException != null ? builderException.ExceptionSource : (DocumentNode) null) ?? this.xamlDocument.RootNode, description, this.messageContent.ToString(), ErrorSeverity.Error));
          }
          if (this.FirstStaleUserControls != null)
          {
            ++num;
            this.errors.Add((IErrorTask) new SceneView.WarningTask(StringTable.StaleTypeDescription, this.ProjectContext.ProjectName, this.Document.DocumentReference.Path, ErrorSeverity.Warning));
          }
        }
        bool flag = false;
        if (this.CodeEditor != null)
        {
          List<TextEditorErrorInformation> errorRanges = new List<TextEditorErrorInformation>(this.errors.Count);
          foreach (IErrorTask errorTask in new List<IErrorTask>((IEnumerable<IErrorTask>) this.errors))
          {
            SceneView.ISceneErrorTask sceneErrorTask = errorTask as SceneView.ISceneErrorTask;
            if (sceneErrorTask != null && sceneErrorTask.Document == this.xamlDocument)
            {
              ITextRange span = sceneErrorTask.Span;
              errorRanges.Add(new TextEditorErrorInformation(span.Offset, span.Length, sceneErrorTask.Description));
            }
            else
              flag = true;
          }
          this.CodeEditor.UpdateErrorRanges(errorRanges);
        }
        Microsoft.Expression.Framework.UserInterface.IWindowService windowService = this.DesignerContext.WindowService;
        if (windowService == null || windowService.PaletteRegistry == null)
          return;
        PaletteRegistryEntry paletteRegistryEntry = this.DesignerContext.WindowService.PaletteRegistry["Designer_ResultsPane"];
        if (this.FocusedEditor != FocusedEditor.Code || forceShowErrorsWhenXamlFocused || flag || (paletteRegistryEntry != null && paletteRegistryEntry.IsVisible || this.CodeEditor == null))
        {
          if (count > 0 || this.errors.Count > 0)
            this.errors.Timestamp = DateTime.Now;
          if (this.errors.Count > num)
            this.ViewModel.DesignerContext.ErrorManager.DisplayErrors();
        }
        ResultsPane resultsPane = paletteRegistryEntry != null ? paletteRegistryEntry.Content as ResultsPane : (ResultsPane) null;
        if (resultsPane == null || count <= 0 || (this.errors.Count != 0 || resultsPane.View != ResultsView.Errors) || resultsPane.ErrorManager.Errors.Count != 0)
          return;
        paletteRegistryEntry.IsVisible = false;
      }
    }

    private void AddExceptionsToDictionary(SceneView activeView)
    {
      foreach (KeyValuePair<DocumentNode, Exception> keyValuePair in (IEnumerable<KeyValuePair<DocumentNode, Exception>>) activeView.ExceptionDictionary)
      {
        if (keyValuePair.Key.DocumentRoot != null)
        {
          string message = this.UnwrapException(keyValuePair.Value).Message;
          this.errors.Add((IErrorTask) new SceneView.DocumentErrorTask(keyValuePair.Key, message, message, ErrorSeverity.Error));
        }
      }
      foreach (KeyValuePair<DocumentNode, string> keyValuePair in (IEnumerable<KeyValuePair<DocumentNode, string>>) activeView.WarningDictionary)
      {
        if (keyValuePair.Key.DocumentRoot != null)
          this.errors.Add((IErrorTask) new SceneView.DocumentErrorTask(keyValuePair.Key, keyValuePair.Value, keyValuePair.Value, DocumentNodeUtilities.IsStaticResource(keyValuePair.Key) ? ErrorSeverity.Error : ErrorSeverity.Warning));
      }
    }

    private void RefreshErrorsAsync()
    {
      if (this.errorsInvalidated)
        return;
      this.errorsInvalidated = true;
      UIThreadDispatcher.Instance.BeginInvoke(DispatcherPriority.Background, new Action(this.RefreshErrors));
    }

    private void XamlDocument_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (this.isDisposed)
        return;
      this.RefreshErrorsAsync();
      this.OnMessageContentChanged(this.FocusedEditor != FocusedEditor.Code || this.viewModel.IsEditable);
    }

    private bool CommitTextEdits()
    {
      bool hasTextEdits = this.xamlDocument.HasTextEdits;
      bool flag = this.allowInvalidatingCode;
      bool shouldBroadcast = false;
      if (this.xamlDocument.HasTextEdits)
      {
        PerformanceUtility.StartPerformanceSequence(PerformanceEvent.XamlIncrementalParse);
        this.allowInvalidatingCode = false;
        using (SceneEditTransaction editTransaction = this.Document.CreateEditTransaction(StringTable.CommitTextEditsUndo, true))
        {
          shouldBroadcast = this.xamlDocument.CommitTextEdits();
          editTransaction.Commit(false);
        }
        PerformanceUtility.EndPerformanceSequence(PerformanceEvent.XamlIncrementalParse);
      }
      this.CommitCurrentTextEditTransaction(shouldBroadcast);
      this.allowInvalidatingCode = flag;
      return hasTextEdits;
    }

    private bool SynchronizeText()
    {
      bool flag = this.xamlDocument.SynchronizeText();
      this.selectionSynchronizer.EnsureXamlSynchronizedToScene(false);
      return flag;
    }

    private void AddCommands()
    {
      this.ClearCommands();
      Dictionary<string, Command> dictionary1 = new Dictionary<string, Command>();
      Dictionary<string, Command> dictionary2 = new Dictionary<string, Command>();
      dictionary1.Add("ExtensibilityCommand", (Command) new SceneView.ExtensibilityCommand());
      dictionary1.Add("Edit_Cut", (Command) new CutCommand(this.viewModel));
      dictionary1.Add("Edit_Copy", (Command) new CopyCommand(this.viewModel));
      dictionary1.Add("Edit_Paste", (Command) new PasteCommand(this.viewModel));
      dictionary1.Add("Edit_Delete", (Command) new DeleteCommand(this.viewModel, this.DesignerContext));
      dictionary1.Add("Edit_Group", (Command) new GroupCommand(this.viewModel));
      dictionary1.Add("Edit_Ungroup", (Command) new UngroupCommand(this.viewModel));
      dictionary1.Add("Edit_AddItem", (Command) new AddItemCommand(this.viewModel));
      dictionary1.Add("Edit_AddSeparator", (Command) new AddSeparatorCommand(this.viewModel));
      dictionary1.Add("Edit_ExpandControl", (Command) new ExpandControlCommand(this.viewModel));
      dictionary1.Add("Edit_BindToData", (Command) new BindToDataCommand(this.viewModel));
      dictionary1.Add("Edit_Order_BringToFront", (Command) new BringToFrontCommand(this.viewModel));
      dictionary1.Add("Edit_Order_BringForward", (Command) new BringForwardCommand(this.viewModel));
      dictionary1.Add("Edit_Order_SendBackward", (Command) new SendBackwardCommand(this.viewModel));
      dictionary1.Add("Edit_Order_SendToBack", (Command) new SendToBackCommand(this.viewModel));
      dictionary1.Add("Edit_Align_AlignLeft", (Command) new AlignLeftCommand(this.viewModel));
      dictionary1.Add("Edit_Align_AlignCenter", (Command) new AlignCenterCommand(this.viewModel));
      dictionary1.Add("Edit_Align_AlignRight", (Command) new AlignRightCommand(this.viewModel));
      dictionary1.Add("Edit_Align_AlignTop", (Command) new AlignTopCommand(this.viewModel));
      dictionary1.Add("Edit_Align_AlignMiddle", (Command) new AlignMiddleCommand(this.viewModel));
      dictionary1.Add("Edit_Align_AlignBottom", (Command) new AlignBottomCommand(this.viewModel));
      dictionary1.Add("Edit_SelectAll", (Command) new SelectAllCommand(this.viewModel));
      dictionary1.Add("Edit_SelectNone", (Command) new SelectNoneCommand(this.viewModel));
      dictionary1.Add("Edit_SetCurrentSelection", (Command) new SetCurrentSelectionCommand(this));
      dictionary1.Add("Edit_LockInsertionPoint", (Command) new ToggleLockInsertionPointCommand(this.viewModel));
      dictionary1.Add("Edit_Format_LockObjects", (Command) new LockObjectsCommand(this.viewModel, true));
      dictionary1.Add("Edit_Format_LockAllObjects", (Command) new LockAllObjectsCommand(this.viewModel, true));
      dictionary1.Add("Edit_Format_UnlockAllObjects", (Command) new LockAllObjectsCommand(this.viewModel, false));
      dictionary1.Add("Edit_Format_ShowObjects", (Command) new ShowObjectsCommand(this.viewModel, true));
      dictionary1.Add("Edit_Format_ShowAllObjects", (Command) new ShowAllObjectsCommand(this.viewModel, true));
      dictionary1.Add("Edit_Format_HideObjects", (Command) new ShowObjectsCommand(this.viewModel, false));
      dictionary1.Add("Edit_Format_HideAllObjects", (Command) new ShowAllObjectsCommand(this.viewModel, false));
      dictionary1.Add("Edit_AutoSize_Horizontal", (Command) new ToggleAutoSizeHorizontalCommand(this.viewModel));
      dictionary1.Add("Edit_AutoSize_Vertical", (Command) new ToggleAutoSizeVerticalCommand(this.viewModel));
      dictionary1.Add("Edit_AutoSize_Both", (Command) new ToggleAutoSizeBothCommand(this.viewModel));
      dictionary1.Add("Edit_AutoSize_Fill", (Command) new AutoSizeFillCommand(this.viewModel));
      dictionary1.Add("Edit_Flip_Horizontal", (Command) new FlipHorizontalCommand(this.viewModel));
      dictionary1.Add("Edit_Flip_Vertical", (Command) new FlipVerticalCommand(this.viewModel));
      dictionary1.Add("Edit_Size_MakeSameWidth", (Command) new MakeSameWidthCommand(this.viewModel));
      dictionary1.Add("Edit_Size_MakeSameHeight", (Command) new MakeSameHeightCommand(this.viewModel));
      dictionary1.Add("Edit_Size_MakeSameSize", (Command) new MakeSameSizeCommand(this.viewModel));
      dictionary1.Add("Edit_MakeClippingPath", (Command) new MakeClippingPathCommand(this.viewModel));
      dictionary1.Add("Edit_RemoveClippingPath", (Command) new RemoveClippingPathCommand(this.viewModel));
      dictionary1.Add("Edit_MakeLayoutPath", (Command) new MakeLayoutPathCommand(this.viewModel));
      dictionary1.Add("Edit_MakeCompoundPath", (Command) new MakeCompoundPathCommand(this.viewModel));
      dictionary1.Add("Edit_ReleaseCompoundPath", (Command) new ReleaseCompoundPathCommand(this.viewModel));
      dictionary1.Add("Edit_ConvertToPath", (Command) new ConvertToPathCommand(this.viewModel));
      dictionary1.Add("Edit_ConvertToMotionPath", (Command) new ConvertToMotionPathCommand(this.viewModel));
      dictionary1.Add("Edit_AddDataGridColumns", (Command) new AddDataGridColumnsCommand(this.viewModel));
      dictionary1.Add("Tools_Combine_Unite", (Command) new BooleanUniteCommand(this.viewModel));
      dictionary1.Add("Tools_Combine_Divide", (Command) new BooleanDivideCommand(this.viewModel));
      dictionary1.Add("Tools_Combine_Intersect", (Command) new BooleanIntersectCommand(this.viewModel));
      dictionary1.Add("Tools_Combine_Subtract", (Command) new BooleanSubtractCommand(this.viewModel));
      dictionary1.Add("Tools_Combine_ExcludeOverlap", (Command) new BooleanExcludeOverlapCommand(this.viewModel));
      dictionary1.Add("Tools_SetupFontEmbedding", (Command) new SetupFontEmbeddingCommand(this.viewModel));
      dictionary1.Add("Tools_NameInteractiveElements", (Command) new NameInteractiveElementsCommand(this.viewModel));
      dictionary1.Add("Edit_MakeImage3D", (Command) new MakeImage3DCommand(this.viewModel));
      dictionary1.Add("Edit_EditText", (Command) new EditTextCommand(this.viewModel));
      dictionary1.Add("Edit_EditControl", (Command) new EditControlCommand(this.viewModel));
      dictionary1.Add("Edit_Style_EditExisting", (Command) new EditExistingStyleTemplateCommand(this.viewHost, this.viewModel, BaseFrameworkElement.StyleProperty));
      dictionary1.Add("Edit_Style_EditNew", (Command) new EditNewStyleCommand(this.viewHost, this.viewModel, BaseFrameworkElement.StyleProperty));
      dictionary1.Add("Edit_Style_EditCopy", (Command) new EditCopyOfStyleCommand(this.viewHost, this.viewModel, BaseFrameworkElement.StyleProperty));
      dictionary1.Add("Edit_Style_LocalResource", (Command) new ResourceListCommand(this.viewModel, BaseFrameworkElement.StyleProperty));
      dictionary1.Add("Edit_EditStyles", (Command) new EditStylesListCommand(this.viewHost, this.viewModel));
      dictionary1.Add("Edit_Template_EditExisting", (Command) new EditExistingStyleTemplateCommand(this.viewHost, this.viewModel));
      dictionary1.Add("Edit_Template_EditNew", (Command) new EditNewTemplateCommand(this.viewHost, this.viewModel));
      dictionary1.Add("Edit_Template_EditCopy", (Command) new EditCopyOfStyleTemplateCommand(this.viewHost, this.viewModel));
      dictionary1.Add("Edit_Template_LocalResource", (Command) new StyleTemplateResourceListCommand(this.viewModel));
      dictionary1.Add("Edit_EditTemplates", (Command) new EditTemplatesListCommand(this.viewHost, this.viewModel));
      dictionary1.Add("Edit_MakeButton", (Command) new MakeControlCommand(this.viewModel));
      dictionary1.Add("Edit_MakeUserControl", (Command) new MakeUserControlCommand(this.viewModel));
      dictionary1.Add("Edit_MakeCompositionScreen", (Command) new MakeCompositionScreenCommand(this.viewModel));
      dictionary1.Add("Edit_MakePart", (Command) new PartsListCommand(this.viewModel));
      dictionary1.Add("Edit_ClearPart", (Command) new ClearPartAssignmentCommand(this.viewModel));
      dictionary1.Add("Edit_CopyAllStateProperties", (Command) new CopyAllStatePropertiesCommand(this.viewModel, (VisualStateSceneNode) null));
      dictionary1.Add("Edit_CopySelectedStateProperties", (Command) new CopySelectedStatePropertiesCommand(this.viewModel, (VisualStateSceneNode) null));
      dictionary1.Add("Edit_MakeTileBrush_MakeImageBrush", (Command) new MakeImageBrushCommand(this.viewModel));
      dictionary1.Add("Edit_MakeTileBrush_MakeVisualBrush", (Command) new MakeVisualBrushCommand(this.viewModel));
      dictionary1.Add("Edit_MakeTileBrush_MakeDrawingBrush", (Command) new MakeDrawingBrushCommand(this.viewModel));
      dictionary1.Add("Edit_MakeTileBrush_MakeVideoBrush", (Command) new MakeVideoBrushCommand(this.viewModel));
      dictionary1.Add("Edit_MakeTileBrush_CopyToResource", (Command) new CopyToResourceCommand(this.viewModel));
      dictionary1.Add("Edit_MakeTileBrush_MoveToResource", (Command) new MoveToResourceCommand(this.viewModel));
      dictionary1.Add("Edit_ChangeLayoutTypes", (Command) new ChangeLayoutTypeFlyoutCommand(this.viewModel));
      dictionary1.Add("Edit_GroupInto", (Command) new GroupIntoLayoutTypeFlyoutCommand(this.viewModel));
      dictionary1.Add("Timeline_Navigate_Down", (Command) new NavigateDownCommand(this.viewModel));
      dictionary1.Add("Timeline_Navigate_Up", (Command) new NavigateUpCommand(this.viewModel));
      dictionary1.Add("View_GoToXaml", (Command) new GoToXamlCommand(this.viewModel));
      dictionary1.Add("View_3D_ToggleLights", (Command) new ToggleLights(this));
      dictionary1.Add("Application_Import_Illustrator", (Command) new ImportCommand(this.viewModel, new string[1]
      {
        "ai"
      }));
      dictionary1.Add("Application_Import_Photoshop", (Command) new ImportCommand(this.viewModel, new string[1]
      {
        "psd"
      }));
      dictionary1.Add("View_ActualSize", (Command) new ActualSizeCommand(this));
      dictionary1.Add("View_FitToAll", (Command) new FitToScreenCommand(this, false));
      dictionary1.Add("View_FitToSelection", (Command) new FitToScreenCommand(this, true));
      dictionary1.Add("View_ZoomIn", (Command) new ZoomInCommand(this));
      dictionary1.Add("View_ZoomOut", (Command) new ZoomOutCommand(this));
      dictionary1.Add("View_ToggleAdorners", (Command) new ToggleAdornersCommand(this));
      dictionary1.Add("View_ShowSelectionPreview", (Command) new ToggleShowSelectionPreviewCommand(this));
      dictionary1.Add("View_ShowActiveContainer", (Command) new ToggleShowActiveContainerCommand(this));
      dictionary1.Add("View_ShowBoundaries", (Command) new ShowBoundariesCommand(this));
      dictionary1.Add("View_ShowAlignmentAdorners", (Command) new ShowAlignmentAdornersCommand(this));
      dictionary1.Add("View_ShowAnnotations", (Command) new ShowAnnotationsCommand(this));
      dictionary1.Add("View_ApplyProjections", (Command) new ApplyProjectionsCommand(this));
      dictionary1.Add("Annotations_CreateAnnotationCommand", (Command) new CreateAnnotationCommand(this));
      dictionary1.Add("Annotations_CopyAnnotationsInDocumentAsText", (Command) new CopyAnnotationsInDocumentAsText(this));
      dictionary1.Add("Annotations_DeleteAnnotationsInDocument", (Command) new DeleteAnnotationsInDocument(this));
      dictionary1.Add("Edit_Find", (Command) new FindAnnotationCommand(this));
      dictionary1.Add("Edit_FindNext", (Command) new FindNextAnnotationCommand(this));
      dictionary1.Add("Assets_SetAsDefaultTheme", (Command) new SetUserThemeCommand(this.DesignerContext.AssetLibrary));
      dictionary1.Add("Assets_RestoreToDefaultContent", (Command) new RestoreThemeContentCommand(this.DesignerContext.AssetLibrary));
      dictionary1.Add("Assets_ResetSystemTheme", (Command) new ResetSystemThemeCommand(this.DesignerContext.AssetLibrary));
      dictionary1.Add("Debug_CreateDocumentNodeView", (Command) new CreateDocumentNodeViewCommand(this.viewModel));
      dictionary1.Add("Debug_UndoManager_Dump", (Command) new SceneView.DumpUndoStackCommand(this.Document));
      dictionary1.Add("Debug_UndoManager_Clear", (Command) new SceneView.ClearUndoStackCommand(this.Document));
      dictionary1.Add("Edit_Undo", (Command) new SceneView.SceneViewUndoCommand(this.Document));
      dictionary1.Add("Edit_Redo", (Command) new SceneView.SceneViewRedoCommand(this.Document));
      dictionary1.Add("View_ShowDesign", (Command) new SwitchViewModeCommand(this, ViewMode.Design));
      dictionary1.Add("View_ShowXaml", (Command) new SwitchViewModeCommand(this, ViewMode.Code));
      dictionary1.Add("View_ShowSplit", (Command) new SwitchViewModeCommand(this, ViewMode.Split));
      dictionary1.Add("View_CycleView", (Command) new CycleViewModeCommand(this));
      if (this.viewOptionsModel != null)
      {
        dictionary1.Add("View_SplitView_SplitHorizontally", (Command) new SwitchSplitViewOrientationCommand(this, this.viewOptionsModel, false));
        dictionary1.Add("View_SplitView_SplitVertically", (Command) new SwitchSplitViewOrientationCommand(this, this.viewOptionsModel, true));
        dictionary1.Add("View_SplitView_DesignOnTop", (Command) new ToggleDesignOnTopCommand(this, this.viewOptionsModel));
      }
      if (this.ViewMode == ViewMode.Code || this.ViewMode == ViewMode.Split)
      {
        foreach (DictionaryEntry dictionaryEntry in this.CodeEditor.GetEditCommands())
          dictionary2.Add((string) dictionaryEntry.Key, (Command) dictionaryEntry.Value);
      }
      foreach (KeyValuePair<string, Command> keyValuePair in dictionary1)
      {
        Command xamlCommand;
        if (dictionary2.TryGetValue(keyValuePair.Key, out xamlCommand))
          this.AddCommand(keyValuePair.Key, (Microsoft.Expression.Framework.Commands.ICommand) new XamlDesignChooserCommand(this, keyValuePair.Value, xamlCommand));
        else
          this.AddCommand(keyValuePair.Key, (Microsoft.Expression.Framework.Commands.ICommand) keyValuePair.Value);
      }
      foreach (KeyValuePair<string, Command> keyValuePair in dictionary2)
      {
        if (!dictionary1.ContainsKey(keyValuePair.Key))
          this.AddCommand(keyValuePair.Key, (Microsoft.Expression.Framework.Commands.ICommand) keyValuePair.Value);
      }
      if (this.DesignerContext.CommandService == null || this.DesignerContext.CommandService.GetCommandProperty("Timeline_Navigate_Down", "Shortcuts") != null)
        return;
      this.DesignerContext.CommandService.SetCommandPropertyDefault("Timeline_Navigate_Down", "Shortcuts", (object) SceneView.CreateKeyBinding(Key.Oem6, ModifierKeys.Alt));
      this.DesignerContext.CommandService.SetCommandPropertyDefault("Timeline_Navigate_Up", "Shortcuts", (object) SceneView.CreateKeyBinding(Key.Oem4, ModifierKeys.Alt));
    }

    public DocumentNodePath GetDocumentNodePath(ViewNode viewNode)
    {
      return this.ViewNodeManager.GetCorrespondingNodePath(viewNode);
    }

    public void AddLiveControl(IViewControl control)
    {
      this.Artboard.AddLiveControl(control);
    }

    public void RemoveLiveControl(IViewControl control)
    {
      this.Artboard.RemoveLiveControl(control);
    }

    public void FocusLiveControlLayer()
    {
      this.Artboard.FocusLiveControlLayer();
    }

    private void DesignSurface_GotFocus(object sender, EventArgs e)
    {
      this.FocusedEditor = FocusedEditor.Design;
      if (this.xamlDocument.IsEditable)
        return;
      this.OnMessageContentChanged();
    }

    private void CodeEditor_CaretPositionChanged(object sender, EventArgs e)
    {
      if (!this.selectionSynchronizer.IsUpdatingSelectionFromScene)
        this.FocusedEditor = FocusedEditor.Code;
      this.selectionSynchronizer.SynchronizeSceneToXaml(false);
    }

    private void CodeEditor_GotFocus(object sender, EventArgs e)
    {
      this.FocusedEditor = FocusedEditor.Code;
      this.RefreshCodeArea();
    }

    private void CodeEditor_LostFocus(object sender, EventArgs e)
    {
      if (this.isDisposed)
        return;
      this.CommitTextEdits();
      this.RefreshErrorsAsync();
    }

    private void OnArtboardOptionsChanged(object sender, EventArgs e)
    {
      if (this.isDisposed)
        return;
      this.AdornerLayer.InvalidateAdornerVisuals();
      this.UpdateEffectEnabledState();
    }

    private void UpdateEffectEnabledState()
    {
      bool flag1 = this.DesignerContext.ArtboardOptionsModel.EffectsEnabled;
      if (flag1 && this.Artboard.Zoom > this.DesignerContext.ArtboardOptionsModel.EffectsEnabledZoomThreshold)
        flag1 = false;
      bool flag2 = this.InstanceBuilderContext.EffectManager.DisableEffects == flag1;
      this.InstanceBuilderContext.EffectManager.DisableEffects = !flag1;
      if (!flag2)
        return;
      this.ViewModel.TimelineItemManager.UpdateEffectTimelineItemLockState();
      if (this.ProjectContext.IsCapabilitySet(PlatformCapability.IsWpf) || this.appliedStoryboards.Count == 0 && this.ViewModel.ActiveStoryboardTimeline == null)
        return;
      this.OnUpdated();
    }

    private void BuildManager_BuildStarting(object sender, BuildStartingEventArgs args)
    {
      if (this.viewModel.DesignerContext.ActiveView == this)
        this.ReturnFocus();
      if (!args.DisplayOutput)
        return;
      this.buildMessage = StringTable.SceneBuildNotificationBuilding;
      this.OnPropertyChanged("BuildMessage");
      this.building = true;
      this.OnPropertyChanged("IsBuilding");
    }

    private void BuildManager_BuildCompleted(object sender, BuildCompletedEventArgs args)
    {
      if (!args.DisplayOutput)
        return;
      switch (args.BuildResult)
      {
        case BuildResult.Succeeded:
          this.buildMessage = StringTable.SceneBuildNotificationBuildSucceeded;
          break;
        case BuildResult.Failed:
          this.buildMessage = StringTable.SceneBuildNotificationBuildFailed;
          break;
        case BuildResult.Canceled:
          this.buildMessage = StringTable.SceneBuildNotificationBuildCanceled;
          break;
        default:
          this.buildMessage = string.Empty;
          break;
      }
      this.OnPropertyChanged("BuildMessage");
      this.building = false;
      this.OnPropertyChanged("IsBuilding");
    }

    public double GetBaseline(SceneNode sceneNode)
    {
      if (sceneNode.ViewObject == null)
        return double.NaN;
      return sceneNode.ViewObject.GetBaseline();
    }

    public bool IsInArtboard(SceneElement element)
    {
      IViewVisual visual = element.Visual as IViewVisual;
      if (this.Artboard == null || visual == null)
        return false;
      return this.Artboard.IsInArtboard(visual);
    }

    public void ZoomIn()
    {
      this.Artboard.ZoomIn();
    }

    public void ZoomOut()
    {
      this.Artboard.ZoomOut();
    }

    public void ZoomToFitRectangle(Rect rectangle)
    {
      this.Artboard.ZoomToFitRectangle(rectangle);
    }

    public void ZoomToFitScreen(bool considerSelections)
    {
      if (!considerSelections || this.ElementSelectionSet.IsEmpty)
        this.Artboard.ZoomToFitAll();
      else
        this.Artboard.ZoomToFitElementList(Enumerable.Select<SceneElement, SceneElement>((IEnumerable<SceneElement>) this.ElementSelectionSet.Selection, (Func<SceneElement, SceneElement>) (e => SceneView.GetZoomableElement(e))));
    }

    private static SceneElement GetZoomableElement(SceneElement element)
    {
      if (PlatformTypes.FrameworkElement.IsAssignableFrom((ITypeId) element.Type) || PlatformTypes.FrameworkTemplate.IsAssignableFrom((ITypeId) element.Type) || PlatformTypes.Style.IsAssignableFrom((ITypeId) element.Type))
        return element;
      return element.FindTargetTypeAncestor(PlatformTypes.FrameworkElement) as SceneElement ?? element;
    }

    public void CenterSelection()
    {
      if (this.Artboard == null)
        return;
      if (this.ElementSelectionSet.IsEmpty)
        this.Artboard.CenterAll();
      else
        this.Artboard.CenterElementList((IEnumerable<SceneElement>) this.ElementSelectionSet.Selection);
    }

    public bool TryEnterTextEditMode(bool textElementOnly)
    {
      bool flag = false;
      TextToolBehavior textToolBehavior = this.eventRouter.ActiveBehavior as TextToolBehavior;
      if (textToolBehavior == null)
      {
        if (this.ElementSelectionSet.Count == 1 && TextEditProxyFactory.IsEditableElement(this.ElementSelectionSet.PrimarySelection, textElementOnly))
        {
          this.eventRouter.PushBehavior((ToolBehavior) new TextToolBehavior(this.eventRouter.ActiveBehavior.ToolBehaviorContext, (ToolBehavior) null));
          flag = true;
        }
      }
      else
        flag = textToolBehavior.TryEnterTextEditMode();
      if (this.ViewModel.TextSelectionSet.TextEditProxy != null)
      {
        this.ViewModel.TextSelectionSet.TextEditProxy.EditingElement.Focus();
        this.ViewModel.TextSelectionSet.TextEditProxy.EditingElement.SelectAll();
      }
      return flag;
    }

    public bool TryExitTextEditMode()
    {
      TextToolBehavior textToolBehavior = this.EventRouter.ActiveBehavior as TextToolBehavior;
      if (textToolBehavior == null)
        return false;
      textToolBehavior.PopSelfOrExitEditMode();
      return true;
    }

    public PropertyReference ConvertToWpfPropertyReference(PropertyReference propertyReference)
    {
      IPlatformMetadata destinationWpfPlatform = (IPlatformMetadata) this.Platform.Metadata;
      if (!this.Platform.Metadata.IsCapabilitySet(PlatformCapability.IsWpf))
        destinationWpfPlatform = (IPlatformMetadata) this.DesignerContext.DesignerDefaultPlatformService.DefaultPlatform.Metadata;
      return this.DesignerContext.PlatformConverter.ConvertToWpfPropertyReference(propertyReference, destinationWpfPlatform);
    }

    public PropertyReference ConvertFromWpfPropertyReference(PropertyReference propertyReference)
    {
      return this.DesignerContext.PlatformConverter.ConvertFromWpfPropertyReference(propertyReference, (IPlatformMetadata) this.Platform.Metadata);
    }

    public Point TransformPointToRoot(SceneElement element, Point point)
    {
      return this.TransformPoint(element.Visual, (IViewObject) this.HitTestRoot, point);
    }

    public Point TransformPointFromRoot(SceneElement element, Point point)
    {
      return this.TransformPoint((IViewObject) this.HitTestRoot, element.Visual, point);
    }

    public Vector TransformVectorToRoot(SceneElement element, Vector vector)
    {
      return this.TransformVector(element.Visual, (IViewObject) this.HitTestRoot, vector);
    }

    public Vector TransformVectorFromRoot(SceneElement element, Vector vector)
    {
      return this.TransformVector((IViewObject) this.HitTestRoot, element.Visual, vector);
    }

    public Vector TransformVector(IViewObject from, IViewObject to, Vector vector)
    {
      return this.TransformPoint(from, to, new Point(vector.X, vector.Y)) - this.TransformPoint(from, to, new Point(0.0, 0.0));
    }

    public Matrix GetComputedTransformToRoot(SceneElement element)
    {
      return this.GetComputedTransformToRoot(element.Visual);
    }

    public Matrix GetComputedTransformFromRoot(SceneElement element)
    {
      return this.GetComputedTransformFromRoot(element.Visual);
    }

    public Point GetMousePosition(MouseDevice mouseDevice, IViewObject relative)
    {
      return this.PointFromScreen(relative, this.Artboard.PointToScreen(mouseDevice.GetPosition((IInputElement) this.Artboard)));
    }

    public void EnsureVisible(IViewObject element)
    {
      this.EnsureVisible(element, false);
    }

    public void EnsureVisible(IAdorner adorner)
    {
      this.EnsureVisible(adorner, false);
    }

    public void EnsureVisible(Point point)
    {
      this.EnsureVisible(point, false);
    }

    public void EnsureVisible(Point point, bool scrollNow)
    {
      this.EnsureVisibleInternal((Visual) this.ViewRootContainer, new Rect(point.X, point.Y, 0.0, 0.0), scrollNow);
    }

    protected void EnsureVisibleInternal(Visual visual, Rect rect, bool scrollNow)
    {
      if (scrollNow)
      {
        if (this.autoScroller != null)
          this.autoScroller.StopScroll();
        this.Artboard.MakeVisible(visual, rect);
      }
      else
      {
        if (this.autoScroller == null)
          this.autoScroller = new SceneView.LimitedFrequencyAutoScroller(this);
        this.autoScroller.StartScroll(visual, rect);
      }
    }

    public void UpdateLayout()
    {
      try
      {
        this.UpdateLayoutInternal();
      }
      catch (Exception ex)
      {
        this.OnExceptionWithUnknownSource(ex);
      }
    }

    void IAttachViewRoot.UpdateLayout()
    {
      this.UpdateLayoutInternal();
    }

    protected virtual void UpdateLayoutInternal()
    {
      this.Artboard.UpdateLayout();
    }

    public void OnExceptionWithUnknownSource(Exception exception)
    {
      IViewVisual elementForException = this.Platform.TryGetElementForException(exception);
      ViewNode viewNode = elementForException != null ? this.GetCorrespondingViewNode((IViewObject) elementForException, false) : (ViewNode) null;
      if (viewNode != null)
      {
        viewNode.ViewNodeManager.OnException(viewNode, exception);
        if (this.isUpdating)
          this.pendingSynchronousUpdate = true;
        else
          this.Update();
      }
      else
        this.SetViewException(exception);
      ExceptionHandler.SafelyForceLayoutArrange();
    }

    public void InvalidateAndRebuildAdornerSets()
    {
      this.shouldRebuildAdornerSets = true;
      this.RebuildAdornerSets();
    }

    public IList<DocumentNodePath> GenerateAllPaths(DocumentNode target)
    {
      return this.RootInstanceBuilderContext.ViewNodeManager.GetCorrespondingNodePaths(target);
    }

    public IList<DocumentNodePath> GenerateVisiblePaths(DocumentNode target)
    {
      if (this.instanceBuilderContext == null)
        return (IList<DocumentNodePath>) null;
      return this.ViewNodeManager.GetCorrespondingNodePaths(target);
    }

    public DocumentNodePath GenerateSinglePath(DocumentNode target)
    {
      if (this.instanceBuilderContext == null)
        return (DocumentNodePath) null;
      return this.ViewNodeManager.GetSingleCorrespondingNodePath(target);
    }

    public ViewState GetViewState(SceneNode sceneElement)
    {
      if (this.ExceptionDictionary.Count == 0)
        return ViewState.Valid;
      return this.GetViewState<SceneNode>((IEnumerable<SceneNode>) new List<SceneNode>()
      {
        sceneElement
      });
    }

    public ViewState GetViewState<T>(IEnumerable<T> elements) where T : SceneNode
    {
      ViewState viewState1 = ViewState.Valid;
      if (this.ExceptionDictionary.Count != 0)
      {
        ViewState viewState2 = viewState1 & ~ViewState.SceneValid;
        Dictionary<SceneNode, bool> invalidSceneNodes = new Dictionary<SceneNode, bool>();
        Dictionary<SceneNode, bool> queryNodes = new Dictionary<SceneNode, bool>();
        foreach (T obj in elements)
        {
          SceneNode index = (SceneNode) obj;
          queryNodes[index] = true;
        }
        viewState1 = viewState2 & this.GetViewStateForSubtree(invalidSceneNodes, queryNodes);
        foreach (T obj in elements)
        {
          for (SceneNode parent = obj.Parent; parent != null; parent = parent.Parent)
          {
            if (invalidSceneNodes.ContainsKey(parent))
            {
              viewState1 &= ~ViewState.AncestorValid;
              break;
            }
          }
        }
      }
      return viewState1;
    }

    public ViewState GetViewStateForSelection()
    {
      return this.GetViewState<SceneElement>((IEnumerable<SceneElement>) this.ElementSelectionSet.Selection);
    }

    private ViewState GetViewStateForSubtree(Dictionary<SceneNode, bool> invalidSceneNodes, Dictionary<SceneNode, bool> queryNodes)
    {
      ViewState viewState = ViewState.Valid;
      foreach (DocumentNode node in this.ExceptionDictionary.Keys)
      {
        if (node != null && node.DocumentRoot == this.documentRoot)
        {
          SceneNode key = this.viewModel.GetSceneNode(node);
          invalidSceneNodes[key] = true;
          if (queryNodes.ContainsKey(key))
            viewState &= ~ViewState.ElementValid;
          else if ((viewState & ViewState.SubtreeValid) == ViewState.SubtreeValid)
          {
            for (; key != null; key = key.Parent)
            {
              if (queryNodes.ContainsKey(key))
              {
                viewState &= ~ViewState.SubtreeValid;
                break;
              }
            }
          }
        }
      }
      return viewState;
    }

    public IViewObject GetCorrespondingViewObject(DocumentNodePath nodePath)
    {
      IViewObject viewObject = (IViewObject) null;
      ViewNode viewNode;
      if (this.instanceBuilderContext != null && this.ViewNodeManager.TryGetCorrespondingViewNode(nodePath, out viewNode))
        viewObject = this.GetCorrespondingViewObject(viewNode);
      if (viewObject == null && this.rootView != null && this.rootView.ViewNodeManager.TryGetCorrespondingViewNode(nodePath, out viewNode))
        viewObject = this.rootView.GetCorrespondingViewObject(viewNode);
      return viewObject;
    }

    public IViewObject GetCorrespondingViewObject(ViewNode viewNode)
    {
      IViewObject viewObject = (IViewObject) null;
      IInstantiatedElementViewNode instantiatedElementViewNode = viewNode as IInstantiatedElementViewNode;
      if (instantiatedElementViewNode != null && !PlatformTypes.Style.IsAssignableFrom((ITypeId) viewNode.Type) && !PlatformTypes.FrameworkTemplate.IsAssignableFrom((ITypeId) viewNode.Type))
      {
        object first = instantiatedElementViewNode.InstantiatedElements.First;
        if (first != null)
          viewObject = this.Platform.ViewObjectFactory.Instantiate(first);
      }
      if (viewObject == null)
        viewObject = this.Platform.ViewObjectFactory.Instantiate(viewNode.Instance);
      return viewObject;
    }

    public ICollection<IViewObject> GetInstantiatedElements(DocumentNodePath nodePath)
    {
      List<IViewObject> list = new List<IViewObject>();
      ViewNode viewNode;
      if (this.instanceBuilderContext != null && this.ViewNodeManager.TryGetCorrespondingViewNode(nodePath, out viewNode))
      {
        IInstantiatedElementViewNode instantiatedElementViewNode = viewNode as IInstantiatedElementViewNode;
        if (instantiatedElementViewNode != null && instantiatedElementViewNode.InstantiatedElements != null)
        {
          foreach (object platformObject in instantiatedElementViewNode.InstantiatedElements)
            list.Add(this.Platform.ViewObjectFactory.Instantiate(platformObject));
        }
      }
      return (ICollection<IViewObject>) list;
    }

    public IList<SceneElement> GetInvalidElements()
    {
      List<SceneElement> list = new List<SceneElement>();
      if (this.IsValid && this.instanceBuilderContext != null && this.ExceptionDictionary.Count != 0)
      {
        foreach (DocumentNode node in this.ExceptionDictionary.Keys)
        {
          if (node != null && node.DocumentRoot == this.documentRoot)
          {
            SceneElement sceneElement = this.viewModel.GetSceneNode(node) as SceneElement;
            if (sceneElement != null)
              list.Add(sceneElement);
          }
        }
      }
      return (IList<SceneElement>) list;
    }

    public IList<SceneElement> GetUserControlInstances()
    {
      List<SceneElement> list = new List<SceneElement>();
      if (this.IsValid && this.instanceBuilderContext != null)
      {
        foreach (ViewNode viewNode in (IEnumerable<ViewNode>) this.instanceBuilderContext.UserControlInstances)
        {
          if (viewNode.DocumentNode.DocumentRoot == this.documentRoot)
          {
            SceneElement sceneElement = this.viewModel.GetSceneNode(viewNode.DocumentNode) as SceneElement;
            if (sceneElement != null)
              list.Add(sceneElement);
          }
        }
      }
      return (IList<SceneElement>) list;
    }

    public Exception GetException(DocumentNodePath invalidElement)
    {
      ViewNode viewNode;
      if (this.instanceBuilderContext != null && this.ViewNodeManager.TryGetCorrespondingViewNode(invalidElement, out viewNode))
        return this.ExceptionDictionary.GetException(viewNode);
      return (Exception) null;
    }

    public DocumentNode GetExceptionTarget(DocumentNodePath invalidElement)
    {
      ViewNode viewNode;
      if (this.instanceBuilderContext != null && this.ViewNodeManager.TryGetCorrespondingViewNode(invalidElement, out viewNode))
        return this.ExceptionDictionary.GetExceptionSource(viewNode);
      return (DocumentNode) null;
    }

    public ViewNode GetCorrespondingViewNode(IViewObject instance, bool allowCrossView)
    {
      ViewNode viewNode = (ViewNode) null;
      if (this.instanceBuilderContext != null)
      {
        viewNode = this.instanceBuilderContext.InstanceDictionary.GetViewNode(instance.PlatformSpecificObject, allowCrossView);
        if (viewNode == null && this.RootInstanceBuilderContext != null)
          viewNode = this.RootInstanceBuilderContext.InstanceDictionary.GetViewNode(instance.PlatformSpecificObject, allowCrossView);
        if (viewNode == null && allowCrossView)
        {
          foreach (SceneView sceneView in (IEnumerable<SceneView>) this.DesignerContext.ViewRootResolver.ResolvedViews)
          {
            IInstanceBuilderContext viewContext = this.DesignerContext.ViewRootResolver.GetViewContext(sceneView.DocumentRoot);
            if (viewContext != null)
            {
              viewNode = viewContext.InstanceDictionary.GetViewNode(instance.PlatformSpecificObject, true);
              if (viewNode != null)
                break;
            }
          }
        }
      }
      if (!allowCrossView && viewNode != null)
      {
        IDocumentRoot documentRoot = viewNode.DocumentNode.DocumentRoot;
        ViewNode parent = viewNode.Parent;
        if (this.documentRoot != documentRoot)
        {
          viewNode = (ViewNode) null;
        }
        else
        {
          for (; parent != null; parent = parent.Parent)
          {
            if (parent.DocumentNode.DocumentRoot != documentRoot)
            {
              viewNode = (ViewNode) null;
              break;
            }
          }
        }
      }
      return viewNode;
    }

    public DocumentNode GetCorrespondingDocumentNode(IViewObject instance, bool allowCrossView)
    {
      ViewNode correspondingViewNode = this.GetCorrespondingViewNode(instance, allowCrossView);
      if (correspondingViewNode != null)
        return correspondingViewNode.DocumentNode;
      return (DocumentNode) null;
    }

    public DocumentNodePath GetCorrespondingNodePath(IViewObject instance, bool allowCrossView)
    {
      ViewNode correspondingViewNode = this.GetCorrespondingViewNode(instance, allowCrossView);
      if (correspondingViewNode != null)
        return correspondingViewNode.ViewNodeManager.GetCorrespondingNodePath(correspondingViewNode);
      return (DocumentNodePath) null;
    }

    public SceneElement GetSelectableElement(DocumentNodePath nodePath)
    {
      IStoryboardContainer storyboardContainer = this.viewModel.ActiveStoryboardContainer;
      if (storyboardContainer != null)
      {
        DocumentNodePath targetElementPath = storyboardContainer.TargetElement != null ? storyboardContainer.TargetElement.DocumentNodePath : ((SceneNode) storyboardContainer).DocumentNodePath;
        DocumentNodePath editingContainer = this.viewModel.GetAncestorInEditingContainer(nodePath, this.viewModel.ActiveEditingContainerPath, targetElementPath);
        if (editingContainer != null)
          return this.viewModel.GetUnlockedAncestor(editingContainer);
      }
      return (SceneElement) null;
    }

    public void SetFocusToRoot()
    {
      if (this.scrollViewer == null)
        return;
      this.FocusedEditor = FocusedEditor.Design;
      if (this.Artboard != null && PresentationSource.FromVisual((Visual) this.Artboard) != null && this.IsEditable)
      {
        if (this.Artboard.Focus())
          ;
      }
      else
      {
        if (!this.scrollViewer.IsVisible)
          return;
        this.scrollViewer.Focus();
      }
    }

    public override void ReturnFocus()
    {
      if (this.isDisposed)
        return;
      using (this.DesignerContext.WindowService.SuppressViewActivationOnGotFocus())
      {
        switch (this.FocusedEditor)
        {
          case FocusedEditor.Design:
            if (this.viewModel.TextSelectionSet.IsActive)
            {
              this.viewModel.TextSelectionSet.TextEditProxy.EditingElement.Focus();
              break;
            }
            this.SetFocusToRoot();
            break;
          case FocusedEditor.Code:
            if (this.CodeEditor == null)
              break;
            this.CodeEditor.Focus();
            break;
        }
      }
    }

    public void ShowErrors()
    {
      this.RefreshErrors(true);
    }

    public void EnsureDesignSurfaceVisible()
    {
      if (this.ViewMode != ViewMode.Code)
        return;
      this.ViewMode = ViewMode.Split;
    }

    public void EnsureXamlEditorVisible()
    {
      if (this.ViewMode != ViewMode.Design)
        return;
      this.ViewMode = ViewMode.Split;
    }

    public static SceneView.AnimationChangeResult HandleAnimationChanges(SceneViewModel viewModel, DocumentNodeChange documentNodeChange, SceneView.HandleAnimationChange animationChange)
    {
      SceneNode sceneNode1 = viewModel.GetSceneNode((DocumentNode) documentNodeChange.ParentNode);
      if (sceneNode1 == null)
        return SceneView.AnimationChangeResult.NotAnimationChange;
      if (documentNodeChange.Action == DocumentNodeChangeAction.Remove && documentNodeChange.OldChildNode != null)
      {
        SceneNode sceneNode2 = viewModel.GetSceneNode(documentNodeChange.OldChildNode);
        AnimationSceneNode animationSceneNode;
        if ((animationSceneNode = sceneNode2 as AnimationSceneNode) != null)
          return animationSceneNode.TargetProperty != null && DesignTimeProperties.ExplicitAnimationProperty.Equals((object) animationSceneNode.TargetProperty.LastStep) ? SceneView.AnimationChangeResult.NotAnimationChange : SceneView.AnimationChangeResult.InvalidateAll;
        if (sceneNode2 is StoryboardTimelineSceneNode || sceneNode2 is SetterSceneNode)
          return SceneView.AnimationChangeResult.InvalidateAll;
      }
      if (documentNodeChange.NewChildNode != null && viewModel.GetSceneNode(documentNodeChange.NewChildNode) is SetterSceneNode)
        return SceneView.AnimationChangeResult.InvalidateAll;
      if (typeof (TimelineCollection).IsAssignableFrom(sceneNode1.TargetType))
        sceneNode1 = sceneNode1.Parent;
      if (sceneNode1.Parent != null && typeof (AnimationSceneNode).IsAssignableFrom(sceneNode1.Parent.GetType()))
        sceneNode1 = sceneNode1.Parent;
      KeyFrameSceneNode keyFrameSceneNode = sceneNode1 as KeyFrameSceneNode;
      AnimationSceneNode animationSceneNode1 = sceneNode1 as AnimationSceneNode;
      StoryboardTimelineSceneNode timelineSceneNode1 = sceneNode1 as StoryboardTimelineSceneNode;
      if (keyFrameSceneNode != null)
      {
        SceneElement element = keyFrameSceneNode.TargetElement as SceneElement;
        if (element != null)
        {
          animationChange(element, keyFrameSceneNode.TargetProperty);
          return SceneView.AnimationChangeResult.AnimationChange;
        }
      }
      else if (animationSceneNode1 != null)
      {
        TimelineSceneNode.PropertyNodePair elementAndProperty = animationSceneNode1.TargetElementAndProperty;
        SceneElement element = elementAndProperty.SceneNode as SceneElement;
        if (element != null && elementAndProperty.PropertyReference != null)
        {
          animationChange(element, elementAndProperty.PropertyReference);
          return SceneView.AnimationChangeResult.AnimationChange;
        }
      }
      else if (timelineSceneNode1 != null)
      {
        bool flag = false;
        foreach (TimelineSceneNode timelineSceneNode2 in (IEnumerable<TimelineSceneNode>) timelineSceneNode1.Children)
        {
          AnimationSceneNode animationSceneNode2 = timelineSceneNode2 as AnimationSceneNode;
          if (animationSceneNode2 != null)
          {
            TimelineSceneNode.PropertyNodePair elementAndProperty = animationSceneNode2.TargetElementAndProperty;
            SceneElement element = elementAndProperty.SceneNode as SceneElement;
            if (element != null && elementAndProperty.PropertyReference != null)
            {
              animationChange(element, elementAndProperty.PropertyReference);
              flag = true;
            }
          }
        }
        if (flag)
          return SceneView.AnimationChangeResult.AnimationChange;
      }
      return keyFrameSceneNode != null || animationSceneNode1 != null || timelineSceneNode1 != null ? SceneView.AnimationChangeResult.InvalidateAll : SceneView.AnimationChangeResult.NotAnimationChange;
    }

    internal static string GetUnresolvedTypesList(ICollection<IType> types)
    {
      string str = string.Empty;
      int num = 0;
      foreach (IMemberId memberId in (IEnumerable<IType>) types)
      {
        string fullName = memberId.FullName;
        if (num == 0)
          str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.DocumentInitialUnresolvedType, new object[1]
          {
            (object) fullName
          });
        else
          str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.DocumentSubsequentUnresolvedType, new object[2]
          {
            (object) str,
            (object) fullName
          });
        ++num;
        if (num == 5)
          break;
      }
      if (types.Count > 5)
        str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.DocumentMoreUnreportedUnresolvedTypes, new object[1]
        {
          (object) str
        });
      return str;
    }

    public Point PointToScreen(Visual relative, Point point)
    {
      return relative.PointToScreen(point);
    }

    public Point PointFromScreen(Visual relative, Point point)
    {
      return relative.PointFromScreen(point);
    }

    public Rect GetBounds(FrameworkElement element)
    {
      return ElementUtilities.GetActualBounds(element);
    }

    public void ShowBubble(string message, MessageBubbleType messageBubbleType)
    {
      MessageBubble messageBubble = new MessageBubble((UIElement) this.Artboard, new MessageBubbleContent(message, messageBubbleType));
      messageBubble.Initialize();
      messageBubble.Placement = PlacementMode.Relative;
      messageBubble.PlacementRectangle = new Rect(60.0, 0.0, 0.0, 0.0);
      this.messageBubbleHost.Show(messageBubble);
    }

    public virtual SceneElement GetElementAtPoint(Point point, HitTestModifier hitTestModifier, InvisibleObjectHitTestModifier invisibleObjectHitTestModifier, ICollection<BaseFrameworkElement> ignoredElements)
    {
      PointHitTestParameters hitTestParameters = new PointHitTestParameters(point);
      bool stopAfterFirstHit = true;
      bool skipFullyContainedSelectionInObject = false;
      IList<SceneElement> list = this.hitTestHelper.PerformHitTest((HitTestParameters) hitTestParameters, hitTestModifier, invisibleObjectHitTestModifier, ignoredElements, stopAfterFirstHit, skipFullyContainedSelectionInObject);
      if (list.Count <= 0)
        return (SceneElement) null;
      return list[0];
    }

    public virtual IList<SceneElement> GetElementsInRectangle(Rect rectangle, HitTestModifier hitTestModifier, InvisibleObjectHitTestModifier invisibleObjectHitTestModifier, bool skipFullyContainedSelectionInObject)
    {
      GeometryHitTestParameters hitTestParameters = new GeometryHitTestParameters((System.Windows.Media.Geometry) new RectangleGeometry(rectangle));
      bool stopAfterFirstHit = false;
      ICollection<BaseFrameworkElement> ignoredElements = (ICollection<BaseFrameworkElement>) null;
      return this.hitTestHelper.PerformHitTest((HitTestParameters) hitTestParameters, hitTestModifier, invisibleObjectHitTestModifier, ignoredElements, stopAfterFirstHit, skipFullyContainedSelectionInObject);
    }

    public SceneElement GetSelectableElementAtPoint(Point point, SelectionFor3D selectionFor3D, bool selectedOnly)
    {
      return this.GetSelectableElementAtPoint(point, selectionFor3D, selectedOnly, true);
    }

    public SceneElement GetSelectableElementAtPoint(Point point, SelectionFor3D selectionFor3D, bool selectedOnly, bool smartInvisiblePanelSelect)
    {
      return new SceneView.HitElementHelper(this, selectionFor3D, selectedOnly, smartInvisiblePanelSelect).GetSelectableElementAtPoint(point);
    }

    public IList<SceneElement> GetSelectableElementsInRectangle(Rect rectangle, SelectionFor3D selectionFor3D, bool selectedOnly, bool smartInvisiblePanelSelect)
    {
      return new SceneView.HitElementHelper(this, selectionFor3D, selectedOnly, smartInvisiblePanelSelect).GetSelectableElementsInRectangle(rectangle);
    }

    public static InvisibleObjectHitTestResult SmartInvisiblePanelSelect(IHitTestHelperService hitTestHelperService, IViewObject hitElement, IViewObject invisibleElement)
    {
      bool flag = false;
      IViewPanel viewPanel = invisibleElement as IViewPanel;
      if (viewPanel == null || viewPanel.GetCurrentValue(hitTestHelperService.SceneView.ProjectContext.ResolveProperty(PanelElement.BackgroundProperty)) != null)
        return InvisibleObjectHitTestResult.Fill;
      Rect actualBounds = hitTestHelperService.SceneView.GetActualBounds((IViewObject) viewPanel);
      actualBounds.Inflate((double) -SceneView.invisiblePanelEdgeTolerance, (double) -SceneView.invisiblePanelEdgeTolerance);
      IntersectionDetail intersectionDetail = hitTestHelperService.HitTestBounds(actualBounds, (IViewVisual) viewPanel);
      if (intersectionDetail == IntersectionDetail.Empty)
        flag = true;
      hitElement = hitTestHelperService.QueryHitTestModifier(hitElement);
      if (hitTestHelperService.HitElements.Count == 0 && hitElement == null)
        return InvisibleObjectHitTestResult.Fill;
      if (intersectionDetail == IntersectionDetail.FullyInside)
        return InvisibleObjectHitTestResult.None;
      return flag ? InvisibleObjectHitTestResult.Stroke : InvisibleObjectHitTestResult.Fill;
    }

    private void ToolManager_ToolChanged(object sender, ToolEventArgs e)
    {
      if (this.viewModel == null)
        return;
      Tool activeTool = this.DesignerContext.ToolManager.ActiveTool;
      Tool overrideTool = this.DesignerContext.ToolManager.OverrideTool;
      this.Artboard.ShowExtensibleAdorners = activeTool != null && (activeTool.ShowExtensibleAdorners || overrideTool != null && overrideTool.ShowExtensibleAdorners);
    }

    private void Artboard_ZoomChanged(object sender, EventArgs e)
    {
      this.UpdateEffectEnabledState();
    }

    private void OnPropertyChanged(string propertyName)
    {
      this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
    }

    private void OnMessageContentChanged()
    {
      this.OnMessageContentChanged(true);
    }

    private void OnMessageContentChanged(bool shouldSendPropertyChanges)
    {
      if (shouldSendPropertyChanges)
      {
        this.OnPropertyChanged("MessageContent");
        this.OnPropertyChanged("IsEditable");
      }
      this.viewModel.OnIsEditableChanged();
    }

    void IAttachViewRoot.EnsureRootSited()
    {
      ViewNode scopedViewNode = this.ScopedViewNode;
      if (scopedViewNode == null)
      {
        this.SetViewContentInternal(ViewContentType.Unknown, (ViewNode) null, (object) null);
      }
      else
      {
        if (this.ScopedInstance == null)
          return;
        this.SetViewContentInternal(ViewContentType.Content, scopedViewNode, this.ScopedInstance);
      }
    }

    public virtual IDisposable EnsureCrossDocumentUpdateContext()
    {
      return (IDisposable) null;
    }

    private void EnsureViewContent()
    {
      this.SetViewContentInternal(ViewContentType.Content, this.ScopedViewNode, this.ScopedInstance);
    }

    public void SetViewException(Exception exception)
    {
      Exception exception1 = this.UnwrapException(exception);
      if (this.ShouldFilterViewException(exception))
        return;
      object content = (object) this.exceptionFormatter.Format(exception1);
      if (content == null)
        return;
      this.SetViewContentInternal(ViewContentType.Unknown, (ViewNode) null, (object) null);
      if (this.rootView != null)
        this.rootView.SetViewContentInternal(ViewContentType.Unknown, (ViewNode) null, (object) null);
      this.InvalidateForRootChanged();
      this.SetViewContentInternal(ViewContentType.Error, (ViewNode) null, content);
    }

    protected virtual bool ShouldFilterViewException(Exception exception)
    {
      return false;
    }

    private Exception UnwrapException(Exception exception)
    {
      if (exception == null)
        return (Exception) null;
      while (exception.InnerException != null)
        exception = exception.InnerException;
      return exception;
    }

    internal void ChangedScope()
    {
      if (this.ViewNodeManager.Root == null || !this.ViewNodeManager.Root.DocumentNode.IsInDocument)
        return;
      this.EnsureViewContent();
      this.ViewNodeManager.OnViewScopeChanged(this.ScopedViewNode, (IAttachViewRoot) this);
    }

    public void InvalidateAndUpdate()
    {
      this.InvalidateAndUpdate(true);
    }

    public void InvalidateAndUpdate(bool updateReferences)
    {
      this.InvalidateForRootChanged();
      if (this.rootView != null)
        this.rootView.UpdateInternal(true, updateReferences);
      this.UpdateInternal(true, updateReferences);
    }

    public void Update(bool updateReferences)
    {
      this.UpdateInternal(true, updateReferences);
    }

    private void InvalidateForRootChanged()
    {
      XamlDocument xamlDocument = (XamlDocument) this.xamlDocument;
      if (!xamlDocument.IsEditable)
        return;
      SceneNode viewRoot = this.viewModel.ViewRoot;
      if (viewRoot == null)
        return;
      DocumentNodePath documentNodePath1 = viewRoot.DocumentNodePath;
      DocumentNodePath documentNodePath2 = new DocumentNodePath(xamlDocument.RootNode, xamlDocument.RootNode);
      this.shouldRebuildAdornerSets = true;
      if (this.allowViewScoping && this.rootView == null && documentNodePath1.Node != xamlDocument.RootNode)
      {
        this.rootView = this.CreateSceneView();
        this.rootView.Attach(this);
      }
      else if (this.rootView != null && documentNodePath1.Node == xamlDocument.RootNode)
      {
        bool flag = this.rootView.Detach(this);
        this.rootView.Dispose();
        this.rootView = (SceneView) null;
        if (flag)
          return;
      }
      DocumentNodePath editingContainer = this.ViewNodeManager.CandidateEditingContainer;
      this.ViewNodeManager.RootNodePath = !this.allowViewScoping ? documentNodePath2 : documentNodePath1;
      this.ViewNodeManager.EditingContainer = this.viewModel.ActiveEditingContainerPath;
      if (editingContainer != null && editingContainer.IsValid())
        this.ViewNodeManager.CandidateEditingContainer = editingContainer;
      this.pendingChangeRoot = false;
    }

    internal void RebuildUserControls(IProjectDocument changedDocument)
    {
      if (this.IsClosing || !this.ViewNodeManager.InvalidateUserControls(changedDocument.Path))
        return;
      this.Update();
      this.InvalidateAndRebuildAdornerSets();
    }

    private void Attach(SceneView parent)
    {
      if (new DocumentNodePath(this.documentRoot.RootNode, this.documentRoot.RootNode).Equals((object) parent.instanceBuilderContext.ViewNodeManager.RootNodePath))
        this.AttachInternal(parent);
      else
        this.Initialize();
      this.refreshParentErrorsDelegate = new SceneView.RefreshParentErrors(parent.RefreshErrors);
    }

    protected virtual void AttachInternal(SceneView parent)
    {
      this.SwapInstanceBuilderContexts(parent);
      parent.SetViewContentInternal(ViewContentType.Unknown, (ViewNode) null, (object) null);
      this.InitializeInternal(false);
      this.Update();
    }

    private void SwapInstanceBuilderContexts(SceneView parent)
    {
      Microsoft.Expression.DesignSurface.ViewModel.InstanceBuilderContext instanceBuilderContext = this.instanceBuilderContext;
      this.instanceBuilderContext = parent.instanceBuilderContext;
      parent.instanceBuilderContext = instanceBuilderContext;
      IDocumentRootResolver documentRootResolver = this.instanceBuilderContext.DocumentRootResolver;
      this.instanceBuilderContext.UpdateDocumentRootResolver(parent.instanceBuilderContext.DocumentRootResolver);
      parent.instanceBuilderContext.UpdateDocumentRootResolver(documentRootResolver);
      this.ViewUpdateManager.ExchangeRelatedDocumentHandlers(this, parent);
    }

    private bool Detach(SceneView parent)
    {
      SceneNode viewRoot = this.viewModel.ViewRoot;
      if (viewRoot != null)
      {
        if (this.refreshParentErrorsDelegate != null)
          this.refreshParentErrorsDelegate = (SceneView.RefreshParentErrors) null;
        if (viewRoot.DocumentNodePath.Equals((object) this.ViewNodeManager.RootNodePath))
        {
          this.DetachInternal(parent);
          return true;
        }
      }
      return false;
    }

    protected virtual void DetachInternal(SceneView parent)
    {
      this.SwapInstanceBuilderContexts(parent);
    }

    protected void Invalidate(DocumentNode target, InstanceState state)
    {
      if (this.ShuttingDown)
        return;
      PerformanceUtility.StartPerformanceSequence(PerformanceEvent.SceneViewInvalidate);
      DocumentNode documentNode = this.ViewNodeManager.Root.DocumentNode;
      if (target.IsAncestorOf(documentNode))
        target = documentNode;
      this.ViewNodeManager.Invalidate(target, state);
      PerformanceUtility.EndPerformanceSequence(PerformanceEvent.SceneViewInvalidate);
    }

    private void Invalidate(IDocumentRoot relatedRoot)
    {
      if (this.ShuttingDown)
        return;
      PerformanceUtility.StartPerformanceSequence(PerformanceEvent.SceneViewInvalidate);
      this.ViewNodeManager.Invalidate(relatedRoot);
      PerformanceUtility.EndPerformanceSequence(PerformanceEvent.SceneViewInvalidate);
    }

    private bool Invalidate(IDocumentRoot relatedRoot, DocumentNode targetNode, InstanceState state)
    {
      if (this.ShuttingDown)
        return false;
      PerformanceUtility.StartPerformanceSequence(PerformanceEvent.SceneViewInvalidate);
      bool flag = this.ViewNodeManager.Invalidate(relatedRoot, targetNode, state);
      PerformanceUtility.EndPerformanceSequence(PerformanceEvent.SceneViewInvalidate);
      return flag;
    }

    protected bool IsValidToUpdate()
    {
      if (this.ShuttingDown)
        return false;
      if (this.xamlDocument.IsEditable)
        return true;
      this.pendingNeedUpdate = true;
      return false;
    }

    protected void Update()
    {
      this.UpdateInternal(true, true);
    }

    internal void UpdateReferences()
    {
      this.UpdateInternal(false, true);
    }

    protected virtual void UpdateInternal(bool updateInstances, bool updateReferences)
    {
      if (this.IsClosing)
        return;
      if (this.isUpdating)
        return;
      try
      {
        this.isUpdating = true;
        if (!this.IsValidToUpdate())
          return;
        if (updateInstances)
        {
          this.pendingNeedUpdate = false;
          PerformanceUtility.StartPerformanceSequence(PerformanceEvent.SceneViewUpdate, this.Document.DocumentReference.Path);
          this.UpdateInstancesInternal();
        }
        if (updateReferences)
        {
          this.UpdateReferencesInternal();
          if (this.IsDesignSurfaceVisible)
            this.RebuildAdornerSets();
          this.UpdateLayout();
          if (!this.pendingSynchronousUpdate)
            this.OnUpdated();
          PerformanceUtility.EndPerformanceSequence(PerformanceEvent.SceneViewUpdate, this.Document.DocumentReference.Path);
        }
        this.currentValueProviders.Clear();
      }
      finally
      {
        this.isUpdating = false;
      }
      if (!this.pendingSynchronousUpdate)
        return;
      this.pendingSynchronousUpdate = false;
      this.UpdateInternal(true, updateReferences);
    }

    private void UpdateInstancesInternal()
    {
      this.postponedUpdate = SceneView.PostponedUpdate.None;
      object obj = (object) null;
      ViewContentType oldContentType = this.currentContentType;
      object rootInstance = this.ViewNodeManager.Root != null ? this.ViewNodeManager.Root.Instance : (object) null;
      DocumentNode rootNode = this.documentRoot.RootNode;
      object error;
      if (rootNode == null)
      {
        error = (object) StringTable.SceneViewNoDocumentRoot;
      }
      else
      {
        PerformanceUtility.MarkInterimStep(PerformanceEvent.SceneViewUpdate, "builder.Update");
        this.InstanceBuilderContext.RootTargetTypeReplacement = (InstanceTypeReplacement) null;
        if (this.ViewNodeManager.Root != null && this.ViewNodeManager.Root.DocumentNode != null)
        {
          IType sourceType = (IType) null;
          if (DocumentNodeUtilities.IsStyleOrTemplate(this.ViewNodeManager.Root.DocumentNode.Type))
            sourceType = DocumentNodeUtilities.GetStyleOrTemplateTargetType(this.ViewNodeManager.Root.DocumentNode);
          else if (rootNode == this.ViewNodeManager.Root.DocumentNode)
            sourceType = rootNode.Type;
          if (sourceType != null && this.allowViewScoping)
          {
            ViewContentValueProvider valueProviderObject = this.GetParentContextValueProviderObject((ICollection<IProperty>) null);
            IType replacementType = (IType) null;
            if (valueProviderObject != null)
            {
              IType type = this.ProjectContext.GetType(valueProviderObject.ObjectType);
              if (sourceType.IsAssignableFrom((ITypeId) type))
                replacementType = type;
            }
            if (replacementType == null)
              replacementType = this.ViewNodeManager.GetInstantiatableType((ITypeResolver) this.ProjectContext, sourceType.RuntimeType);
            if (replacementType != null && !replacementType.Equals((object) sourceType))
              this.InstanceBuilderContext.RootTargetTypeReplacement = new InstanceTypeReplacement(sourceType, replacementType);
          }
        }
        if (this.ViewNodeManager.Root == null || !this.ViewNodeManager.UpdateInstances((IAttachViewRoot) this))
          this.ClearAdornerSets();
        error = this.CurrentRootException ?? obj;
      }
      this.PostViewUpdate(rootInstance, oldContentType, error);
    }

    private void PostViewUpdate(object rootInstance, ViewContentType oldContentType, object error)
    {
      bool flag = false;
      this.errorsInvalidated = this.errorsInvalidated || !Enumerable.SequenceEqual<DocumentNode>(this.instanceBuilderContext.ExceptionDictionary.Keys, (IEnumerable<DocumentNode>) this.lastExceptionNodes);
      this.errorsInvalidated = this.errorsInvalidated || !Enumerable.SequenceEqual<DocumentNode>(this.instanceBuilderContext.WarningDictionary.Keys, (IEnumerable<DocumentNode>) this.lastWarningNodes);
      if (error != null)
        this.ViewNodeManager.RootNodePath = (DocumentNodePath) null;
      if (error != null)
        flag = this.SetViewContentInternal(ViewContentType.Error, (ViewNode) null, error);
      else if (this.ViewNodeManager.Root != null)
        flag = this.ViewNodeManager.Root.Instance != rootInstance;
      if (this.errorsInvalidated || this.FirstStaleUserControls == null && oldContentType == this.currentContentType && (error == null || !flag) && (this.errors.Count == 0 || !flag))
        return;
      this.errorsInvalidated = true;
    }

    protected virtual void UpdateReferencesInternal()
    {
      ViewContentType oldContentType = this.currentContentType;
      object rootInstance = this.ViewNodeManager.Root != null ? this.ViewNodeManager.Root.Instance : (object) null;
      if (!this.ViewNodeManager.UpdateReferences((IAttachViewRoot) this, true))
        this.ClearAdornerSets();
      this.PostViewUpdate(rootInstance, oldContentType, this.CurrentRootException);
      if (!this.errorsInvalidated)
        return;
      this.RefreshErrors();
    }

    protected virtual void ActivateHost()
    {
    }

    protected virtual void DeactivateHost()
    {
    }

    protected bool ProcessPostponedUpdate(bool onlyIfActive)
    {
      if (this.postponedUpdate == SceneView.PostponedUpdate.None)
        return false;
      SceneView activeView = this.DesignerContext.ActiveView;
      bool flag1 = activeView != null && activeView.Document == this.Document;
      bool flag2 = !onlyIfActive || flag1;
      if (!flag2 && this.Document.ProjectDocumentType == ProjectDocumentType.Page)
        return false;
      if (flag2 && !flag1)
        this.ActivateHost();
      bool flag3 = this.ProcessPostponedUpdateInternal();
      if (flag2 && !flag1)
        this.DeactivateHost();
      return flag3;
    }

    protected virtual bool ProcessPostponedUpdateInternal()
    {
      if (this.IsClosing)
        return false;
      bool flag = false;
      if (this.postponedUpdate == SceneView.PostponedUpdate.Update)
      {
        this.UpdateInternal(true, false);
        flag = true;
      }
      else if (this.postponedUpdate == SceneView.PostponedUpdate.InvalidateAndUpdate)
      {
        this.InvalidateAndUpdate(false);
        flag = true;
      }
      else if (this.postponedUpdate == SceneView.PostponedUpdate.UpdateInvalidated)
      {
        this.UpdateInternal(true, true);
        flag = true;
      }
      this.postponedUpdate = SceneView.PostponedUpdate.None;
      return flag;
    }

    protected void ClearAdornerSets()
    {
      Tool activeTool = this.viewModel.DesignerContext.ToolManager.ActiveTool;
      if (activeTool == null || !this.allowViewScoping || activeTool.ActiveView != this)
        return;
      activeTool.ClearAdornerSets();
      this.shouldRebuildAdornerSets = true;
    }

    private void RebuildAdornerSetsAsync()
    {
      UIThreadDispatcher.Instance.BeginInvoke(DispatcherPriority.Normal, new Action(this.RebuildAdornerSets));
    }

    private void RebuildAdornerSets()
    {
      if (this.ShuttingDown || this.isDisposed)
        return;
      Tool activeTool = this.DesignerContext.ToolManager.ActiveTool;
      if (activeTool == null || activeTool.ActiveView != this || (!this.shouldRebuildAdornerSets || !this.allowViewScoping))
        return;
      PerformanceUtility.MarkInterimStep(PerformanceEvent.SceneViewUpdate, "UpdateLayout");
      this.UpdateLayout();
      activeTool.RebuildAdornerSets();
      this.shouldRebuildAdornerSets = false;
    }

    private void OnUpdated()
    {
      this.ViewModel.AnimationEditor.Invalidate();
      foreach (SceneView.AppliedStoryboardData appliedStoryboardData in this.appliedStoryboards)
      {
        if (appliedStoryboardData != null && appliedStoryboardData.ViewStoryboard != null)
          appliedStoryboardData.ViewStoryboard.Remove();
      }
      this.appliedStoryboards.Clear();
      this.transitionFromStateStoryboardData = (SceneView.AppliedStoryboardData) null;
      this.viewModel.EditContextManager.SingleViewModelEditContextWalker.Walk(false, (SingleHistoryCallback) ((context, isGhosted) =>
      {
        IList<VisualStateSceneNode> pinnedStates = context.PinnedStates;
        StoryboardTimelineSceneNode timelineSceneNode = context.StateEditTarget != null ? context.StateEditTarget.Storyboard : (StoryboardTimelineSceneNode) null;
        if (pinnedStates != null && pinnedStates.Count != 0)
        {
          foreach (VisualStateSceneNode visualStateSceneNode in (IEnumerable<VisualStateSceneNode>) pinnedStates)
          {
            if (visualStateSceneNode.Storyboard != null && visualStateSceneNode.Storyboard != timelineSceneNode)
              this.appliedStoryboards.Add(new SceneView.AppliedStoryboardData(context.StoryboardContainer as SceneElement, this.CreateViewStoryboard((TimelineSceneNode) visualStateSceneNode.Storyboard)));
          }
        }
        if (timelineSceneNode != null && timelineSceneNode != this.ViewModel.AnimationEditor.ActiveStoryboardTimeline)
          this.appliedStoryboards.Add(new SceneView.AppliedStoryboardData(context.StoryboardContainer as SceneElement, this.CreateViewStoryboard((TimelineSceneNode) timelineSceneNode)));
        return false;
      }));
      VisualStateTransitionSceneNode transitionEditTarget = this.ViewModel.TransitionEditTarget;
      if (transitionEditTarget != null)
      {
        StoryboardTimelineSceneNode timelineSceneNode = !this.ProjectContext.IsCapabilitySet(PlatformCapability.IsWpf) ? transitionEditTarget.BuildHandoffStoryboardNode() : (transitionEditTarget.FromState != null ? transitionEditTarget.FromState.Storyboard : (StoryboardTimelineSceneNode) null);
        if (timelineSceneNode != null)
        {
          using (this.viewModel.ForceAlternateSiteNode(transitionEditTarget.DocumentNode))
            this.transitionFromStateStoryboardData = new SceneView.AppliedStoryboardData(this.ViewModel.ActiveStoryboardContainer as SceneElement, this.CreateViewStoryboard((TimelineSceneNode) timelineSceneNode));
          this.appliedStoryboards.Add(this.transitionFromStateStoryboardData);
        }
      }
      foreach (SceneView.AppliedStoryboardData appliedStoryboardData in this.appliedStoryboards)
      {
        if (!this.TryApplyStoryboard(appliedStoryboardData.ViewStoryboard, appliedStoryboardData.TargetElement, 0.0, true))
          break;
      }
      if (this.viewModel.AnimationEditor != null)
        this.viewModel.AnimationEditor.UpdateActiveTimeline();
      if (this.updated != null)
        this.updated((object) this, EventArgs.Empty);
      this.Artboard.ClearContentBounds();
    }

    public void OnActiveStoryboardTimelineInstanceUpdated(IViewStoryboard viewStoryboard)
    {
      if (this.ProjectContext.IsCapabilitySet(PlatformCapability.IsWpf) || this.transitionFromStateStoryboardData == null)
        return;
      viewStoryboard.SetValue((ITypeResolver) this.ProjectContext, this.ProjectContext.ResolveProperty(DesignTimeProperties.HandoffStoryboardProperty), (object) this.transitionFromStateStoryboardData.ViewStoryboard);
    }

    private void XamlDocument_TextChanged(object sender, EventArgs e)
    {
      if (this.textCommitTimeoutTimer == null)
      {
        this.textCommitTimeoutTimer = new DispatcherTimer();
        this.textCommitTimeoutTimer.Interval = TimeSpan.FromMilliseconds(500.0);
        this.textCommitTimeoutTimer.Tick += new EventHandler(this.OnTextCommitTimeoutTimerTick);
      }
      if (this.textCommitTimeoutTimer.IsEnabled)
        this.textCommitTimeoutTimer.Stop();
      this.textCommitTimeoutTimer.Start();
      this.selectionSynchronizer.DelaySelectionIfActive();
    }

    private void OnTextCommitTimeoutTimerTick(object sender, EventArgs e)
    {
      if (!this.ShuttingDown)
      {
        this.DisposeLastTextEditUndoUnit();
        this.CommitTextEdits();
        this.selectionSynchronizer.SynchronizeSceneToXaml(true);
      }
      this.textCommitTimeoutTimer.Stop();
    }

    public void TransactionCanceled()
    {
      this.xamlDocument.ClearDocumentChanges();
    }

    public void InvalidateCodeEditor()
    {
      if (this.ShuttingDown || !this.allowInvalidatingCode)
        return;
      this.SynchronizeText();
    }

    public void UpdateFromDamage(SceneViewModel.ViewStateBits viewStateBits, DocumentNodeChangeList damage, SceneUpdateTypeFlags updateFlags)
    {
      if (this.IsClosing || this.ViewUpdateManager.UpdatesPostponed || this.Document.XamlDocument.RootNode == null)
        return;
      bool flag1 = false;
      if (this.rootView != null)
      {
        this.rootView.UpdateFromDamage(viewStateBits, damage, updateFlags);
        flag1 = true;
      }
      bool flag2 = this.pendingNeedUpdate;
      if (this.ViewNodeManager.Root != null && this.ViewNodeManager.Root.DocumentNode.IsInDocument)
      {
        int count1 = this.ExceptionDictionary.Count;
        int count2 = this.WarningDictionary.Count;
        bool flag3 = this.pendingChangeRoot;
        if (this.allowViewScoping && (viewStateBits & SceneViewModel.ViewStateBits.ActiveViewScope) != SceneViewModel.ViewStateBits.None && this.viewModel != null)
        {
          SceneNode viewRoot = this.viewModel.ViewRoot;
          if (viewRoot != null)
          {
            if (!viewRoot.DocumentNodePath.Equals((object) this.ViewNodeManager.RootNodePath))
              flag3 = true;
          }
          else
          {
            this.pendingChangeRoot = true;
            return;
          }
        }
        if (flag3)
        {
          this.InvalidateForRootChanged();
          if (this == this.DesignerContext.ActiveView)
            this.ActivateHost();
          if (this.rootView != null && !flag1)
          {
            this.rootView.UpdateFromDamage(viewStateBits, damage, updateFlags);
            this.rootView.UpdateInternal(true, true);
          }
          this.ViewNodeManager.InvalidateTemplates(true);
          this.UpdateInternal(true, true);
          this.Artboard.UpdateImageHostLocation();
          return;
        }
        if (damage.Count > 0)
        {
          using (this.instanceBuilderContext.ForceAllowIncrementalTemplateUpdates((updateFlags & SceneUpdateTypeFlags.Updated) != SceneUpdateTypeFlags.None))
          {
            for (int index = 0; index < damage.Count; ++index)
            {
              DocumentNodeChange args = damage.ValueAt(index);
              if ((!damage.MarkerAt(index).IsDeleted || args.Action != DocumentNodeChangeAction.Add) && !args.IsRootNodeChange)
              {
                this.Invalidate((DocumentNode) args.ParentNode, new InstanceState(args));
                flag2 = true;
              }
            }
          }
        }
        if ((updateFlags & SceneUpdateTypeFlags.Completing) != SceneUpdateTypeFlags.None && this.ViewNodeManager.InvalidateTemplates(false))
          flag2 = true;
        if (this.allowViewScoping && (viewStateBits & SceneViewModel.ViewStateBits.ActiveEditingContainer) != SceneViewModel.ViewStateBits.None || (viewStateBits & SceneViewModel.ViewStateBits.ActiveTrigger) != SceneViewModel.ViewStateBits.None)
        {
          if (this.lastRelativeViewNodeTargetMarker != null && !this.lastRelativeViewNodeTargetMarker.IsDeleted)
          {
            this.Invalidate(this.lastRelativeViewNodeTargetMarker.Node, InstanceState.Invalid);
            this.ViewNodeManager.InvalidateTemplates(true);
            flag2 = true;
          }
          DocumentNode target = (DocumentNode) null;
          if (this.viewModel.ActiveStoryboardContainer != null && (this.viewModel.ActiveStoryboardContainer is FrameworkTemplateElement || this.viewModel.ActiveStoryboardContainer is StyleNode))
          {
            target = this.GetRelativeViewNode(this.viewModel.ActiveStoryboardContainer, this.viewModel.ActiveEditingContainerPath);
            if (target != null && target.Marker != this.lastRelativeViewNodeTargetMarker)
            {
              this.Invalidate(target, InstanceState.Invalid);
              flag2 = true;
            }
          }
          this.lastRelativeViewNodeTargetMarker = target == null ? (DocumentNodeMarker) null : target.Marker;
        }
        if ((viewStateBits & SceneViewModel.ViewStateBits.ActivePinnedStates) != SceneViewModel.ViewStateBits.None || (viewStateBits & SceneViewModel.ViewStateBits.ActiveTimeline) != SceneViewModel.ViewStateBits.None)
          flag2 = true;
        if (this.candidateEditingContainerChanged)
        {
          this.candidateEditingContainerChanged = false;
          flag2 = true;
        }
        if ((viewStateBits & SceneViewModel.ViewStateBits.ActiveEditingContainer) != SceneViewModel.ViewStateBits.None)
        {
          this.ViewNodeManager.EditingContainer = this.viewModel.ActiveEditingContainerPath;
          this.shouldRebuildAdornerSets = true;
          flag2 = true;
        }
        if (count1 != this.ExceptionDictionary.Count || count2 != this.WarningDictionary.Count)
        {
          this.shouldRebuildAdornerSets = true;
          this.errorsInvalidated = true;
        }
        if (flag2)
          this.Update();
      }
      else
        this.InvalidateAndUpdate();
      if ((viewStateBits & SceneViewModel.ViewStateBits.ActiveViewScope) != SceneViewModel.ViewStateBits.None)
        this.ChangedScope();
      if ((viewStateBits & SceneViewModel.ViewStateBits.ElementSelection) == SceneViewModel.ViewStateBits.None)
        return;
      this.OverrideAdornerLayerVisibility = new Visibility?();
    }

    public override void Activated()
    {
      if (!this.ShouldActivate)
        return;
      this.OverrideAdornerLayerVisibility = new Visibility?();
      this.ViewModel.AnnotationEditor.UpdateAnnotationsVisibility();
      try
      {
        this.Artboard.DesignerView.Context = this.viewModel.ExtensibilityManager.EditingContext;
      }
      catch (Exception ex)
      {
        this.DesignerContext.MessageLoggingService.Write(ex.Message);
      }
      if (this.ViewNodeManager.InvalidateUserControls((string) null) && this.postponedUpdate == SceneView.PostponedUpdate.None)
        this.postponedUpdate = SceneView.PostponedUpdate.Update;
      this.EnsureActiveViewUpdated();
      this.DesignerContext.ToolManager.ActiveToolChanged += new ToolEventHandler(this.ToolManager_ToolChanged);
      this.DesignerContext.ToolManager.OverrideToolChanged += new ToolEventHandler(this.ToolManager_ToolChanged);
      UIThreadDispatcher.Instance.BeginInvoke(DispatcherPriority.Loaded, (Action) (() =>
      {
        if (this.viewModel != null && this.ViewModel == this.DesignerContext.ActiveSceneViewModel && (this.viewModel.EditContextManager.ActiveEditContext != null && this.viewModel.EditContextManager.ActiveEditContext.ViewScope != this.lastViewScope))
        {
          this.CenterSelection();
          this.Artboard.ResetZoom();
        }
        this.lastViewScope = this.viewModel == null || this.viewModel.EditContextManager.ActiveEditContext == null ? (SceneNode) null : this.viewModel.EditContextManager.ActiveEditContext.ViewScope;
      }));
      this.valueProviderSizeCache.Clear();
      this.UpdateViewContentProxy();
      base.Activated();
      if (this.missingResourcesResolved)
        return;
      UIThreadDispatcher.Instance.BeginInvoke(DispatcherPriority.ContextIdle, (Action) (() => this.ResolveMissingResourcesAsync()));
    }

    public virtual void EnsureActiveViewUpdated()
    {
      if (this.IsClosing)
        return;
      this.ViewUpdateManager.UpdateInvalidRelatedDocuments(this);
      if (this.rootView != null)
      {
        this.rootView.InvalidateTemplates();
        this.rootView.ProcessPostponedUpdate(false);
        this.rootView.UpdateReferences();
      }
      this.InvalidateTemplates();
      this.ProcessPostponedUpdate(false);
      this.UpdateReferences();
    }

    public virtual void ShutdownVisualTree()
    {
      if (this.rootView != null)
        this.rootView.ShutdownVisualTree();
      this.ViewNodeManager.RootNodePath = (DocumentNodePath) null;
      this.InstanceDictionary.Clear();
      this.currentScopedInstance = (object) null;
      this.hitTestRoot = (IViewVisual) null;
      this.InvalidateForRootChanged();
      this.postponedUpdate = SceneView.PostponedUpdate.UpdateInvalidated;
    }

    protected ViewContentValueProvider GetParentContextValueProviderObject(ICollection<IProperty> ancestorProperties)
    {
      if (this.IsClosing || !this.allowViewScoping)
        return (ViewContentValueProvider) null;
      if (!this.IsEditingOutOfPlace || this.viewModel.EditContextManager == null || (this.viewModel.ActiveEditingContainer == null || !DocumentNodeUtilities.IsStyleOrTemplate(this.viewModel.ActiveEditingContainer.TrueTargetTypeId)))
        return (ViewContentValueProvider) null;
      if (this.isQueryingForParentContextValue)
        return (ViewContentValueProvider) null;
      try
      {
        this.isQueryingForParentContextValue = true;
        SceneNode providerSceneNode = this.GetProviderSceneNode(ancestorProperties);
        if (providerSceneNode == null)
          return (ViewContentValueProvider) null;
        SceneView defaultView = providerSceneNode.ViewModel.DefaultView;
        if (defaultView == this)
          return (ViewContentValueProvider) null;
        bool updatedProvider = false;
        ViewContentValueProvider specifiedValueProvider = defaultView.GetSpecifiedValueProvider(providerSceneNode, this.ViewModel.PreferredSize, out updatedProvider);
        if (updatedProvider)
          this.needToUpdateViewContent = true;
        return specifiedValueProvider;
      }
      finally
      {
        this.isQueryingForParentContextValue = false;
      }
    }

    private ViewContentValueProvider GetSpecifiedValueProvider(SceneNode providerNode, Size preferredSize, out bool updatedProvider)
    {
      updatedProvider = false;
      if (this.isQueryingForParentContextValue || this.IsUpdating)
        return (ViewContentValueProvider) null;
      if (!providerNode.IsViewObjectValid || (this.postponedUpdate & SceneView.PostponedUpdate.Update) != SceneView.PostponedUpdate.None)
        this.Update();
      else if (this.ViewNodeManager.ContainsInvalidReferences)
        this.UpdateReferences();
      if (!providerNode.IsViewObjectValid)
        return (ViewContentValueProvider) null;
      IViewVisual viewVisual = providerNode.ViewObject as IViewVisual;
      ViewContentValueProvider contentValueProvider = Enumerable.FirstOrDefault<ViewContentValueProvider>((IEnumerable<ViewContentValueProvider>) this.currentValueProviders, (Func<ViewContentValueProvider, bool>) (provider => provider.Object == providerNode.ViewObject.PlatformSpecificObject));
      if (contentValueProvider == null)
      {
        contentValueProvider = new ViewContentValueProvider(providerNode.ViewObject.PlatformSpecificObject, new Size?());
        this.currentValueProviders.Add(contentValueProvider);
        updatedProvider = true;
      }
      Size? nullable = new Size?();
      if (viewVisual != null)
      {
        Size renderSize = viewVisual.RenderSize;
        if (renderSize.Width > 0.0 && renderSize.Height > 0.0)
          nullable = new Size?(renderSize);
      }
      if (nullable.HasValue && nullable.Value.Height > 0.0 && nullable.Value.Width > 0.0)
        this.valueProviderSizeCache[providerNode] = nullable.Value;
      if (!nullable.HasValue && !contentValueProvider.OverriddenSize.HasValue)
      {
        Size size;
        nullable = !this.valueProviderSizeCache.TryGetValue(providerNode, out size) ? (preferredSize.Width <= 0.0 || preferredSize.Height <= 0.0 ? new Size?(new Size(20.0, 20.0)) : new Size?(preferredSize)) : new Size?(size);
      }
      if (nullable.HasValue && !object.Equals((object) nullable, (object) contentValueProvider.OverriddenSize))
      {
        contentValueProvider.OverriddenSize = nullable;
        updatedProvider = true;
      }
      return contentValueProvider;
    }

    private SceneNode GetProviderSceneNode(ICollection<IProperty> ancestorProperties)
    {
      SceneNode providerNode = (SceneNode) null;
      bool foundCurrentViewScope = false;
      EditContext activeEditContext = this.ViewModel.ActiveEditContext;
      this.viewModel.EditContextManager.MultiViewModelEditContextWalker.Walk(false, (MultiHistoryCallback) ((context, selectedElementPath, ownerPropertyKey, isGhosted) =>
      {
        if (isGhosted)
          return true;
        if (context.ParentElement != null && context.ParentElement.NodePath != null)
        {
          DocumentNode node = context.ParentElement.NodePath.Node;
          if (!DocumentNodeUtilities.IsStyleOrTemplate(node.Type))
          {
            if (foundCurrentViewScope)
              return true;
            providerNode = context.ParentElement.View.ViewModel.GetSceneNode(node);
            if (ancestorProperties != null)
              ancestorProperties.Clear();
          }
        }
        if (!foundCurrentViewScope && context.ViewScope == activeEditContext.ViewScope)
          foundCurrentViewScope = true;
        else if (ancestorProperties != null)
          SceneView.AddAncestorProperty(ancestorProperties, context);
        return false;
      }));
      return providerNode;
    }

    private static void AddAncestorProperty(ICollection<IProperty> ancestorProperties, EditContext context)
    {
      IProperty property = (IProperty) null;
      DocumentNodePath documentNodePath = context.ParentElement != null ? context.ParentElement.NodePath : (DocumentNodePath) null;
      if (documentNodePath != null)
        property = documentNodePath.Node.TypeResolver.ResolveProperty(context.ParentElement.PropertyKey);
      DocumentNodePath editingContainerPath = context.EditingContainerPath;
      if (property == null && editingContainerPath != null && editingContainerPath.Node.PlatformMetadata.KnownProperties.DictionaryEntryValue.Equals((object) editingContainerPath.ContainerOwnerProperty))
      {
        DocumentCompositeNode entryNode = editingContainerPath.ContainerOwner as DocumentCompositeNode;
        if (entryNode != null)
        {
          DocumentNode resourceEntryKey = DocumentNodeHelper.GetResourceEntryKey(entryNode);
          if (resourceEntryKey == null || DocumentPrimitiveNode.GetValueAsMember(resourceEntryKey) is IType)
            property = entryNode.TypeResolver.ResolveProperty(BaseFrameworkElement.StyleProperty);
        }
      }
      if (property == null && editingContainerPath != null && editingContainerPath.ContainerOwner != null)
      {
        property = editingContainerPath.ContainerOwnerProperty;
        IPropertyValueTypeMetadata valueTypeMetadata = editingContainerPath.ContainerOwner.Type.Metadata as IPropertyValueTypeMetadata;
        if (valueTypeMetadata != null && valueTypeMetadata.PropertyProperty != null)
        {
          DocumentCompositeNode valueNode = editingContainerPath.ContainerOwner as DocumentCompositeNode;
          if (valueNode != null)
            property = DocumentNodeHelper.GetValueAsMember(valueNode, valueTypeMetadata.PropertyProperty) as IProperty;
        }
      }
      if (property == null)
        return;
      ancestorProperties.Add(property);
    }

    protected ViewContent ProvideViewContent(ViewNode target, object content)
    {
      object templateDataContext = DataContextEvaluator.GetTemplateDataContext(target.DocumentNode, this.viewModel);
      HashSet<IProperty> overriddenProperties = new HashSet<IProperty>();
      ViewContentValueProvider valueProviderObject = this.GetParentContextValueProviderObject((ICollection<IProperty>) overriddenProperties);
      if (this.viewModel.ActiveEditContext != null && this.viewModel.ActiveEditContext.OutOfPlaceOverriddenProperties != null)
      {
        foreach (IProperty property in (IEnumerable<IProperty>) this.viewModel.ActiveEditContext.OutOfPlaceOverriddenProperties)
          overriddenProperties.Add(property);
      }
      this.CollectStyledTemplateStyleProperties(target, valueProviderObject, overriddenProperties);
      ViewContent viewContent = this.Platform.ViewContentProvider.ProvideContent(new ContentProviderParameters()
      {
        InstanceBuilderContext = this.InstanceBuilderContext,
        Target = target,
        ValueProviderObject = valueProviderObject,
        Instance = content,
        TemplateDataContext = templateDataContext,
        ExistingContent = (object) null,
        OverriddenProperties = (ICollection<IProperty>) overriddenProperties,
        ActivePropertyTrigger = this.viewModel.ActivePropertyTriggerNode
      });
      this.needToUpdateViewContent = false;
      return viewContent;
    }

    private void CollectStyledTemplateStyleProperties(ViewNode scopeNode, ViewContentValueProvider valueProvider, HashSet<IProperty> overriddenProperties)
    {
      if (valueProvider == null || valueProvider.Object == null)
        return;
      object instance = scopeNode.Instance;
      if (instance == null || !PlatformTypes.Style.IsAssignableFrom((ITypeId) this.ProjectContext.GetType(instance.GetType())))
        return;
      ReferenceStep referenceStep1 = (ReferenceStep) this.ProjectContext.ResolveProperty(StyleNode.SettersProperty);
      DocumentCompositeNode documentCompositeNode1 = (DocumentCompositeNode) ((DocumentCompositeNode) scopeNode.DocumentNode).Properties[(IPropertyId) referenceStep1];
      if (documentCompositeNode1 == null)
        return;
      for (int index = 0; index < documentCompositeNode1.Children.Count; ++index)
      {
        DocumentCompositeNode documentCompositeNode2 = documentCompositeNode1.Children[index] as DocumentCompositeNode;
        if (documentCompositeNode2 != null && PlatformTypes.Setter.IsAssignableFrom((ITypeId) documentCompositeNode2.Type))
        {
          ReferenceStep referenceStep2 = DocumentPrimitiveNode.GetValueAsMember(documentCompositeNode2.Properties[SetterSceneNode.PropertyProperty]) as ReferenceStep;
          if (referenceStep2 != null && (PlatformTypes.FrameworkTemplate.IsAssignableFrom((ITypeId) referenceStep2.PropertyType) || PlatformTypes.Style.IsAssignableFrom((ITypeId) referenceStep2.PropertyType)))
          {
            IndexedClrPropertyReferenceStep referenceStep3 = IndexedClrPropertyReferenceStep.GetReferenceStep((ITypeResolver) this.ProjectContext, PlatformTypes.SetterBaseCollection, index);
            ReferenceStep referenceStep4 = (ReferenceStep) this.ProjectContext.ResolveProperty(SetterSceneNode.ValueProperty);
            object platformObject1 = new PropertyReference(new List<ReferenceStep>()
            {
              referenceStep1,
              (ReferenceStep) referenceStep3,
              referenceStep4
            }).GetValue(instance);
            object platformObject2 = referenceStep2.GetValue(valueProvider.Object);
            if (platformObject2 != null && platformObject1 != null)
            {
              ViewNode correspondingViewNode1 = this.GetCorrespondingViewNode(this.Platform.ViewObjectFactory.Instantiate(platformObject1), true);
              ViewNode correspondingViewNode2 = this.GetCorrespondingViewNode(this.Platform.ViewObjectFactory.Instantiate(platformObject2), true);
              if (correspondingViewNode2 != null && correspondingViewNode1 != null && correspondingViewNode2.DocumentNode == correspondingViewNode1.DocumentNode)
                overriddenProperties.Add((IProperty) referenceStep2);
            }
          }
        }
      }
    }

    public override void Deactivated()
    {
      this.DesignerContext.ToolManager.ActiveToolChanged -= new ToolEventHandler(this.ToolManager_ToolChanged);
      this.DesignerContext.ToolManager.OverrideToolChanged -= new ToolEventHandler(this.ToolManager_ToolChanged);
      bool flag = false;
      if (this.rootView != null)
      {
        this.rootView.InvalidateTemplates();
        if (this.rootView.ProcessPostponedUpdate(false))
          flag = true;
      }
      if (this.InvalidateTemplates())
        this.ViewUpdateManager.InvalidateRelatedDocumentTemplates(this);
      if (this.ProcessPostponedUpdate(false))
        flag = true;
      if (flag)
        this.ViewUpdateManager.UpdateRelatedViews(this.Document, false);
      base.Deactivated();
    }

    internal bool InvalidateTemplates()
    {
      if (this.ShuttingDown)
        return false;
      PerformanceUtility.StartPerformanceSequence(PerformanceEvent.SceneViewInvalidate);
      bool flag = this.ViewNodeManager.InvalidateTemplates(true);
      PerformanceUtility.EndPerformanceSequence(PerformanceEvent.SceneViewInvalidate);
      if (flag)
        this.postponedUpdate = SceneView.PostponedUpdate.UpdateInvalidated;
      return flag;
    }

    private void UpdateViewContentProxy()
    {
      if (this.viewModel != null && this.viewModel.ActiveEditContext != null)
        this.viewModel.ActiveEditContext.OutOfPlaceOverriddenProperties = (ICollection<IProperty>) null;
      if (this.currentContentType != ViewContentType.Content || this.ScopedViewNode == null || this.ScopedInstance == null)
        return;
      this.UpdateViewContentIfNecessary(true);
    }

    protected void UpdateViewContentIfNecessary(bool forceUpdate)
    {
      ICollection<IProperty> ancestorProperties = (ICollection<IProperty>) new HashSet<IProperty>();
      ViewContentValueProvider valueProviderObject = this.GetParentContextValueProviderObject(ancestorProperties);
      if (!forceUpdate && !this.needToUpdateViewContent)
        return;
      this.Platform.ViewContentProvider.UpdateContent(new ContentProviderParameters()
      {
        InstanceBuilderContext = this.InstanceBuilderContext,
        ExistingContent = this.ViewRoot.PlatformSpecificObject,
        Target = this.ScopedViewNode,
        ValueProviderObject = valueProviderObject,
        OverriddenProperties = ancestorProperties,
        Instance = this.ScopedInstance,
        ActivePropertyTrigger = this.viewModel != null ? this.viewModel.ActivePropertyTriggerNode : (DocumentCompositeNode) null
      });
      this.needToUpdateViewContent = false;
    }

    public bool UpdateFromRelatedDocument(SceneDocument document, RelatedDocumentInfo documentInfo, bool verifyChangeStamp, bool majorChange)
    {
      if (this.IsClosing || this.ViewUpdateManager.UpdatesPostponed || this.ViewNodeManager.IsRootInvalid || verifyChangeStamp && (int) documentInfo.ChangeStamp == (int) document.XamlDocument.ChangeStamp)
        return false;
      documentInfo.UpdateChangeStamp();
      if (this.viewModel.Damage.Count == 0)
        Enumerable.Contains<SceneDocument>(this.Document.DesignTimeResourceDocuments, document);
      if (!majorChange && document.IsDesignDataDocument)
        majorChange = true;
      if (this.ViewNodeManager.Root != null && this.ViewNodeManager.Root.DocumentNode.IsInDocument)
      {
        bool flag = false;
        if (!majorChange)
        {
          DocumentNodeChangeList damage = documentInfo.Damage;
          if (damage.Count > 0)
          {
            for (int index = 0; index < damage.Count; ++index)
            {
              DocumentNodeChange documentNodeChange = damage.ValueAt(index);
              if (!damage.MarkerAt(index).IsDeleted || documentNodeChange.Action != DocumentNodeChangeAction.Add)
              {
                if (documentNodeChange.IsRootNodeChange)
                {
                  majorChange = true;
                  break;
                }
                if (this.Invalidate(documentInfo.DocumentRoot, (DocumentNode) documentNodeChange.ParentNode, new InstanceState(documentNodeChange)))
                {
                  flag = true;
                  BrushTool brushTool = this.viewModel.DesignerContext.ToolManager.ActiveTool as BrushTool;
                  if (brushTool != null && brushTool.ShouldRebuildAdorner(documentNodeChange))
                    this.shouldRebuildAdornerSets = true;
                }
              }
            }
          }
        }
        documentInfo.ClearDamage();
        if (majorChange)
        {
          this.Invalidate((IDocumentRoot) document.XamlDocument);
          flag = true;
        }
        if (flag)
          this.postponedUpdate = SceneView.PostponedUpdate.Update;
      }
      else
        this.postponedUpdate = SceneView.PostponedUpdate.InvalidateAndUpdate;
      return this.ProcessPostponedUpdate(true);
    }

    protected IList<ViewNode> GetUpdatedApplicationResourceDictionaries()
    {
      CrossDocumentUpdateContext documentUpdateContext = new CrossDocumentUpdateContext((IViewRootResolver) this.DesignerContext.ViewRootResolver);
      using (this.InstanceBuilderContext.ChangeCrossDocumentUpdateContext((ICrossDocumentUpdateContext) documentUpdateContext))
      {
        List<ViewNode> list = new List<ViewNode>();
        SceneDocument applicationSceneDocument = this.Document.ApplicationSceneDocument;
        foreach (SceneDocument document in this.Document.DesignTimeResourceDocuments)
        {
          if (!this.IsEditingOutOfPlace && document == this.Document)
          {
            if (document != applicationSceneDocument)
            {
              list.Clear();
              break;
            }
          }
          else
          {
            ViewNode resourceDictionaryNode = this.GetResourceDictionaryNode(document);
            if (resourceDictionaryNode != null)
              list.Add(resourceDictionaryNode);
          }
        }
        documentUpdateContext.EndUpdate();
        return (IList<ViewNode>) list;
      }
    }

    private ViewNode GetResourceDictionaryNode(SceneDocument document)
    {
      XamlDocument xamlDocument = (XamlDocument) document.XamlDocument;
      if (xamlDocument == null || !xamlDocument.IsEditable)
        return (ViewNode) null;
      IInstanceBuilderContext viewContext = this.InstanceBuilderContext.GetViewContext((IDocumentRoot) xamlDocument);
      if (viewContext == null)
        return (ViewNode) null;
      ViewNode root = viewContext.ViewNodeManager.Root;
      if (root == null)
        return (ViewNode) null;
      if (PlatformTypes.ResourceDictionary.IsAssignableFrom((ITypeId) root.Type))
        return root;
      IPropertyId resourcesProperty = root.DocumentNode.Type.Metadata.ResourcesProperty;
      if (resourcesProperty != null)
      {
        IProperty index = root.TypeResolver.ResolveProperty(resourcesProperty);
        if (index != null)
          return root.Properties[index];
      }
      return (ViewNode) null;
    }

    private void InvalidateAdornersFromAnimationChange(SceneElement sceneElement, PropertyReference propertyReference)
    {
      this.AdornerLayer.InvalidateAdornerVisuals(sceneElement);
    }

    public void UpdateAdorners(DocumentNodeChangeList damage)
    {
      if (this.IsClosing)
        return;
      if ((this.viewModel.DirtyState & SceneViewModel.ViewStateBits.CurrentValues) == SceneViewModel.ViewStateBits.None)
      {
        foreach (DocumentNodeChange documentNodeChange in damage.DistinctChanges)
        {
          switch (SceneView.HandleAnimationChanges(this.viewModel, documentNodeChange, new SceneView.HandleAnimationChange(this.InvalidateAdornersFromAnimationChange)))
          {
            case SceneView.AnimationChangeResult.InvalidateAll:
              this.AdornerLayer.InvalidateAdornerVisuals();
              goto label_27;
            case SceneView.AnimationChangeResult.NotAnimationChange:
              List<SceneChange> list = documentNodeChange.Annotation as List<SceneChange>;
              if (list != null)
              {
                using (List<SceneChange>.Enumerator enumerator = list.GetEnumerator())
                {
                  while (enumerator.MoveNext())
                  {
                    PropertySceneChange propertySceneChange = enumerator.Current as PropertySceneChange;
                    if (propertySceneChange != null && propertySceneChange.Target != null)
                    {
                      SceneElement sceneElement1 = propertySceneChange.Target as SceneElement;
                      if (sceneElement1 != null)
                      {
                        this.AdornerLayer.InvalidateAdornerVisuals(sceneElement1);
                        if (sceneElement1 is GridElement || sceneElement1 is ControlTemplateElement)
                        {
                          foreach (SceneElement sceneElement2 in this.ElementSelectionSet.Selection)
                            this.AdornerLayer.InvalidateAdornerVisuals(sceneElement2);
                        }
                      }
                    }
                  }
                  continue;
                }
              }
              else
                continue;
            default:
              continue;
          }
        }
      }
      else
        this.AdornerLayer.InvalidateAdornerVisuals();
label_27:
      this.AdornerLayer.InvalidateVisual();
    }

    protected void RestoreFocusAsync()
    {
      UIThreadDispatcher.Instance.BeginInvoke(DispatcherPriority.Background, new Action(this.RestoreFocusAsyncImpl));
    }

    private void WindowService_StateChanged(object sender, EventArgs e)
    {
      if (this.isDisposed || this.Artboard == null)
        return;
      this.Artboard.ResetPositionLock();
    }

    private void RestoreFocusAsyncImpl()
    {
      switch (this.FocusedEditor)
      {
        case FocusedEditor.Design:
          this.SetFocusToRoot();
          break;
        case FocusedEditor.Code:
          if (this.codeEditor == null)
            break;
          this.codeEditor.Focus();
          break;
      }
    }

    private DocumentNode GetRelativeViewNode(IStoryboardContainer storyboardContainer, DocumentNodePath editingContainer)
    {
      SceneElement sceneElement = (SceneElement) storyboardContainer.GetRelativeTargetElement(editingContainer);
      if (sceneElement != null)
        return sceneElement.DocumentNode;
      DocumentNode viewRootNode = this.viewModel.ViewRootNode;
      if (viewRootNode != null)
      {
        if (editingContainer.Node == viewRootNode)
          return viewRootNode;
        SceneNode sceneNode = (SceneNode) storyboardContainer.ScopeElement;
        if (sceneNode != null && sceneNode.DocumentNode == viewRootNode)
          return viewRootNode;
      }
      return (DocumentNode) null;
    }

    private void OnViewOptionsModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (this.isDisposed || !(e.PropertyName == "IsVerticalSplit") && !(e.PropertyName == "IsDesignOnTop") && !(e.PropertyName == "SplitRatio"))
        return;
      this.sceneSplitView.UpdateSplitViewState(this.rootElement.VerticalScrollBarMargin);
    }

    private void OnRootElementPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (this.isDisposed || !(e.PropertyName == "VerticalScrollBarMargin"))
        return;
      this.sceneSplitView.UpdateSplitViewState(this.rootElement.VerticalScrollBarMargin);
    }

    private static KeyBinding[] CreateKeyBinding(Key key, ModifierKeys modifierKeys)
    {
      return new KeyBinding[1]
      {
        new KeyBinding()
        {
          Modifiers = modifierKeys,
          Key = key
        }
      };
    }

    internal void CommitCurrentTextEditTransaction(bool shouldBroadcast)
    {
      if (this.currentTextEditTransaction == null)
        return;
      SceneEditTransaction sceneEditTransaction = this.currentTextEditTransaction;
      this.currentTextEditTransaction = (SceneEditTransaction) null;
      sceneEditTransaction.Commit(shouldBroadcast);
    }

    internal IDisposable DisableSelectionSynchronization()
    {
      return this.selectionSynchronizer.DisableSelectionSynchronization();
    }

    internal void DisposeLastTextEditUndoUnit()
    {
      this.lastTextEditUndoUnit.DisallowMerge();
      this.lastTextEditUndoUnit = (SceneView.NautilusUndoUnit) null;
    }

    internal void OnXamlDocumentTextBufferUndoUnitAdded(object sender, TextUndoCompletedEventArgs e)
    {
      if (e == null || e.Transaction.IsHidden || this.xamlDocument.IsSettingTextDuringUndo)
        return;
      if (e.Result == TextUndoCompletionResult.Added)
      {
        SceneView.NautilusUndoUnit nautilusUndoUnit = new SceneView.NautilusUndoUnit((XamlDocument) this.xamlDocument, e.Transaction);
        if (!this.xamlDocument.IsSerializing)
        {
          this.lastTextEditUndoUnit = nautilusUndoUnit;
          if (this.currentTextEditTransaction == null)
            this.currentTextEditTransaction = this.Document.CreateEditTransaction(StringTable.TextEditUndo, false, SceneEditTransactionType.NestedInAutoClosing);
          this.currentTextEditTransaction.AddUndoUnit((IUndoUnit) nautilusUndoUnit);
        }
        else
        {
          SceneEditTransaction editTransaction = this.Document.CreateEditTransaction(StringTable.TextEditUndo, true, SceneEditTransactionType.NestedInAutoClosing);
          editTransaction.AddUndoUnit((IUndoUnit) nautilusUndoUnit);
          editTransaction.Commit();
        }
      }
      else
      {
        if (e.Result != TextUndoCompletionResult.Merged)
          return;
        this.lastTextEditUndoUnit.SetUndoTransaction(e.Transaction);
      }
    }

    public bool TryApplyStoryboard(IViewStoryboard storyboard, SceneElement targetElement, double animationTime, bool disableMedia)
    {
      IViewObject target = targetElement != null ? targetElement.ViewTargetElement : (IViewObject) null;
      if (target != null && storyboard != null)
      {
        IViewObject context = (IViewObject) null;
        FrameworkTemplateElement frameworkTemplateElement = targetElement as FrameworkTemplateElement;
        if (frameworkTemplateElement != null)
          context = frameworkTemplateElement.ViewObject;
        try
        {
          ViewStoryboardApplyOptions flags = (disableMedia ? ViewStoryboardApplyOptions.RemoveMedia : ViewStoryboardApplyOptions.None) | this.StoryboardApplyOptions;
          storyboard.Apply(context, target, flags);
          storyboard.Play();
          storyboard.Pause();
          storyboard.Seek(animationTime);
        }
        catch (Exception ex)
        {
          this.SetViewException(ex);
          return false;
        }
      }
      return true;
    }

    private static bool ChangeCaretPositionInternal(SceneView view, int line, int column)
    {
      view.EnsureXamlEditorVisible();
      ITextEditor textEditor = view.CodeEditor;
      textEditor.ClearSelection();
      ITextRange span = SceneView.GetSpan((IReadableTextBuffer) view.ViewModel.XamlDocument.TextBuffer, textEditor, line, column);
      textEditor.CaretPosition = span.Offset;
      textEditor.Select(span.Offset, span.Length);
      textEditor.Focus();
      UIThreadDispatcher.Instance.BeginInvoke(DispatcherPriority.Render, (Action) (() =>
      {
        textEditor.EnsureCaretVisible();
        textEditor.MoveLineToCenterOfView(Math.Max(0, line - 1));
      }));
      return true;
    }

    public void SetCaretPosition(int line, int column)
    {
      SceneView.ChangeCaretPositionInternal(this, line, column);
    }

    private static ITextRange GetSpan(IReadableTextBuffer textBuffer, ITextEditor textEditor, int line, int column)
    {
      int start = Math.Min(textBuffer.Length, Math.Max(0, textEditor.GetStartOfLineFromLineNumber(Math.Max(0, line - 1)) + column - 1));
      int num = Math.Min(textBuffer.Length, start + 1);
      while (num < textBuffer.Length && (!char.IsWhiteSpace(textBuffer.GetText(num, 1)[0]) || num == start + 1))
        ++num;
      return (ITextRange) new Microsoft.Expression.DesignModel.Code.TextRange(start, num);
    }

    public class HitTestHelper : IHitTestHelperService
    {
      private readonly SceneView.HitTestHelper.InvisibleHitTestTypeHandlerFactory invisibleHitTestTypeHandlerFactory = new SceneView.HitTestHelper.InvisibleHitTestTypeHandlerFactory();
      private SceneView sceneView;
      private HitTestModifier hitTestModifier;
      private InvisibleObjectHitTestModifier invisibleObjectHitTestModifier;
      private ICollection<BaseFrameworkElement> ignoredElements;
      private ICollection<BaseFrameworkElement> overlayLayerIgnoredElements;
      private bool stopAfterFirstHit;
      private IList<SceneElement> hitElements;
      private Dictionary<SceneElement, bool> hitElementCache;
      private IEnumerator<IViewVisual> treeEnumerator;
      private HitTestParameters hitTestParameters;
      private bool deepHitTestViewport3D;
      private bool skipFullyContainedSelectionInObject;

      public SceneView SceneView
      {
        get
        {
          return this.sceneView;
        }
      }

      public IList<SceneElement> HitElements
      {
        get
        {
          return this.hitElements;
        }
      }

      public HitTestHelper(SceneView sceneView)
      {
        if (sceneView == null)
          throw new ArgumentNullException("sceneView");
        this.sceneView = sceneView;
      }

      public IList<SceneElement> PerformHitTest(HitTestParameters hitTestParameters, HitTestModifier hitTestModifier, InvisibleObjectHitTestModifier invisibleObjectHitTestModifier, ICollection<BaseFrameworkElement> ignoredElements, bool stopAfterFirstHit, bool skipFullyContainedSelectionInObject)
      {
        if (hitTestParameters == null)
          throw new ArgumentNullException("hitTestParameters");
        this.hitTestModifier = hitTestModifier;
        this.invisibleObjectHitTestModifier = invisibleObjectHitTestModifier;
        this.ignoredElements = ignoredElements;
        this.stopAfterFirstHit = stopAfterFirstHit;
        this.hitElements = (IList<SceneElement>) new List<SceneElement>();
        this.hitElementCache = new Dictionary<SceneElement, bool>();
        this.deepHitTestViewport3D = true;
        this.skipFullyContainedSelectionInObject = skipFullyContainedSelectionInObject;
        PointHitTestParameters hitTestParameters1 = hitTestParameters as PointHitTestParameters;
        this.hitTestParameters = hitTestParameters1 == null ? (HitTestParameters) new GeometryHitTestParameters(((GeometryHitTestParameters) hitTestParameters).HitGeometry.Clone()) : (HitTestParameters) new PointHitTestParameters(hitTestParameters1.HitPoint);
        if (this.ignoredElements != null && this.ignoredElements.Count > 0 && (this.sceneView.OverlayLayer != null && this.sceneView.OverlayLayer.VisualChildrenCount > 0))
        {
          this.overlayLayerIgnoredElements = (ICollection<BaseFrameworkElement>) new List<BaseFrameworkElement>();
          IViewVisual viewVisual = (IViewVisual) this.sceneView.OverlayLayer;
          for (int index = 0; index < viewVisual.VisualChildrenCount; ++index)
          {
            BaseFrameworkElement frameworkElement = this.GetCorrespondingSceneElement((IViewObject) viewVisual.GetVisualChild(index), (IParentChainProvider) null) as BaseFrameworkElement;
            if (frameworkElement != null)
            {
              foreach (SceneNode sceneNode in (IEnumerable<BaseFrameworkElement>) this.ignoredElements)
              {
                if (sceneNode.IsAncestorOf((SceneNode) frameworkElement))
                  this.overlayLayerIgnoredElements.Add(frameworkElement);
              }
            }
          }
        }
        else
          this.overlayLayerIgnoredElements = (ICollection<BaseFrameworkElement>) null;
        IViewVisual viewVisual1 = (IViewVisual) this.sceneView.OverlayLayer;
        if (viewVisual1 != null && viewVisual1.VisualChildrenCount > 0)
        {
          this.treeEnumerator = (IEnumerator<IViewVisual>) new SceneView.HitTestHelper.PostfixVisualEnumerator(this, viewVisual1);
          viewVisual1.HitTest(new ViewHitTestFilterCallback(this.FilterPotentialHit), new ViewHitTestResultCallback(this.ProcessHitTestResult), hitTestParameters);
          if (this.hitElements.Count == 0 || !this.stopAfterFirstHit)
          {
            int num = (int) this.LookForInvisibleObjectsUntil(viewVisual1);
          }
        }
        if (this.hitElements.Count == 0 || !this.stopAfterFirstHit)
        {
          IViewVisual hitTestRoot = this.sceneView.HitTestRoot;
          this.treeEnumerator = (IEnumerator<IViewVisual>) new SceneView.HitTestHelper.PostfixVisualEnumerator(this, hitTestRoot);
          hitTestRoot.HitTest(new ViewHitTestFilterCallback(this.FilterPotentialHit), new ViewHitTestResultCallback(this.ProcessHitTestResult), hitTestParameters);
          if (this.hitElements.Count == 0 || !this.stopAfterFirstHit)
          {
            int num = (int) this.LookForInvisibleObjectsUntil(hitTestRoot);
          }
        }
        this.treeEnumerator = (IEnumerator<IViewVisual>) null;
        return this.hitElements;
      }

      public Viewport3DElement PerformHitTestForViewport3D(PointHitTestParameters hitTestParameters)
      {
        if (hitTestParameters == null)
          throw new ArgumentNullException("hitTestParameters");
        this.hitTestModifier = new HitTestModifier(this.sceneView.GetSelectableElement);
        this.ignoredElements = (ICollection<BaseFrameworkElement>) null;
        this.stopAfterFirstHit = true;
        this.hitElements = (IList<SceneElement>) new List<SceneElement>();
        this.hitElementCache = new Dictionary<SceneElement, bool>();
        this.deepHitTestViewport3D = false;
        this.hitTestParameters = (HitTestParameters) new PointHitTestParameters(hitTestParameters.HitPoint);
        ViewHitTestResultCallback resultCallback = (ViewHitTestResultCallback) (result => HitTestResultBehavior.Stop);
        IViewVisual viewVisual = (IViewVisual) this.sceneView.OverlayLayer;
        if (viewVisual != null)
          viewVisual.HitTest(new ViewHitTestFilterCallback(this.FilterPotentialHit), resultCallback, (HitTestParameters) hitTestParameters);
        if (this.hitElements.Count != 0)
          return this.hitElements[0] as Viewport3DElement;
        this.sceneView.HitTestRoot.HitTest(new ViewHitTestFilterCallback(this.FilterPotentialHit), resultCallback, (HitTestParameters) hitTestParameters);
        if (this.hitElements.Count == 0)
          return (Viewport3DElement) null;
        return this.hitElements[0] as Viewport3DElement;
      }

      private HitTestFilterBehavior EarlyFilterPotentialHit(IViewObject viewObject, IParentChainProvider parentChain)
      {
        SceneElement correspondingSceneElement = this.GetCorrespondingSceneElement(viewObject, parentChain);
        bool flag = false;
        if (correspondingSceneElement != null)
        {
          object localValue = correspondingSceneElement.GetLocalValue(DesignTimeProperties.IsHiddenProperty);
          if (localValue != null)
            flag = (bool) localValue;
        }
        if (correspondingSceneElement != null && !this.IsValidChild(correspondingSceneElement))
          return HitTestFilterBehavior.ContinueSkipSelfAndChildren;
        IType type = this.sceneView.ProjectContext.ResolveType(PlatformTypes.UIElement);
        if (correspondingSceneElement != null && flag || type.RuntimeType.IsAssignableFrom(viewObject.TargetType) && !((IViewVisual) viewObject).GetIsVisible(parentChain))
          return HitTestFilterBehavior.ContinueSkipSelfAndChildren;
        BaseFrameworkElement frameworkElement = correspondingSceneElement as BaseFrameworkElement;
        return frameworkElement != null && this.ignoredElements != null && this.ignoredElements.Contains(frameworkElement) || this.overlayLayerIgnoredElements != null && this.overlayLayerIgnoredElements.Contains(frameworkElement) ? HitTestFilterBehavior.ContinueSkipSelfAndChildren : HitTestFilterBehavior.Continue;
      }

      private bool IsValidChild(SceneElement element)
      {
        IProperty sitePropertyKey = element.DocumentNode.SitePropertyKey;
        if (sitePropertyKey == null || element is FrameworkTemplateElement)
          return true;
        SceneElement parentElement = element.ParentElement;
        if (parentElement == null)
          return true;
        foreach (IPropertyId propertyId in parentElement.Metadata.ContentProperties)
        {
          if (parentElement.ProjectContext.ResolveProperty(propertyId) == sitePropertyKey)
            return true;
        }
        return false;
      }

      private HitTestFilterBehavior FilterPotentialHit(IViewObject viewObject, IParentChainProvider parentChain)
      {
        HitTestFilterBehavior testFilterBehavior = this.EarlyFilterPotentialHit(viewObject, parentChain);
        Viewport3D viewport3D = viewObject.PlatformSpecificObject as Viewport3D;
        if (testFilterBehavior == HitTestFilterBehavior.Continue && viewport3D != null)
        {
          bool flag = true;
          if (viewport3D.ClipToBounds)
            flag = this.HitTestSingleInvisibleElement(viewObject as IViewVisual);
          if (flag)
          {
            if (this.deepHitTestViewport3D)
            {
              GeneralTransform viewportTransform = (GeneralTransform) new MatrixTransform(ElementUtilities.GetComputedTransform((Visual) viewport3D, (Visual) this.sceneView.ViewRootContainer));
              foreach (RectangleHitTestResult summary in new Viewport3DHitTestHelper(viewport3D, viewportTransform).HitTest(this.hitTestParameters).ClosestFirstHitTestResults)
              {
                if (this.Record3DHitTestResult(viewport3D, summary) == HitTestResultBehavior.Stop)
                  return HitTestFilterBehavior.Stop;
              }
              return HitTestFilterBehavior.ContinueSkipSelfAndChildren;
            }
            int num = (int) this.RecordHitTestResult(viewObject, false);
            return HitTestFilterBehavior.Stop;
          }
        }
        return testFilterBehavior;
      }

      private HitTestResultBehavior ProcessHitTestResult(IViewHitTestResult hitTestResult)
      {
        return this.RecordHitTestResult(hitTestResult.HitObject);
      }

      private HitTestResultBehavior Record3DHitTestResult(Viewport3D rootViewport, RectangleHitTestResult summary)
      {
        ViewNode viewNode1 = this.sceneView.InstanceBuilderContext.InstanceDictionary.GetViewNode((object) rootViewport, false);
        if (viewNode1 != null)
        {
          ViewNode viewNode2 = this.WalkHitTestPath(rootViewport, viewNode1, new List<DependencyObject>(summary.HitPath));
          if (viewNode2 != null)
          {
            if (this.sceneView.ViewModel.IsExternal(viewNode2.DocumentNode))
              return this.RecordHitTestResult(this.sceneView.Platform.ViewObjectFactory.Instantiate((object) rootViewport));
            for (; viewNode2 != null; viewNode2 = viewNode2.Parent)
            {
              Base3DElement base3Delement = this.sceneView.ViewModel.GetSceneNode(viewNode2.DocumentNode) as Base3DElement;
              if (base3Delement != null && base3Delement.Viewport != null)
                break;
            }
            if (viewNode2 != null)
            {
              IViewObject correspondingViewObject = this.sceneView.GetCorrespondingViewObject(viewNode2);
              if (correspondingViewObject != null)
                return this.RecordHitTestResult(correspondingViewObject);
              return HitTestResultBehavior.Continue;
            }
          }
        }
        return this.RecordHitTestResult(this.sceneView.Platform.ViewObjectFactory.Instantiate((object) summary.ObjectHit));
      }

      private ViewNode WalkHitTestPath(Viewport3D rootViewport, ViewNode currentViewNode, List<DependencyObject> path)
      {
        Visual3D visual3D = (Visual3D) path[path.Count - 1];
        Model3D model3D = (Model3D) null;
        IPlatformMetadata platformMetadata = currentViewNode.DocumentNode.TypeResolver.PlatformMetadata;
        int index1 = rootViewport.Children.IndexOf(visual3D);
        currentViewNode.Properties.TryGetValue(platformMetadata.ResolveProperty(Viewport3DElement.ChildrenProperty), out currentViewNode);
        if (currentViewNode != null)
          currentViewNode = currentViewNode.Children[index1];
        if (currentViewNode == null)
          return (ViewNode) null;
        ViewNodeManager viewNodeManager = this.sceneView.InstanceBuilderContext.ViewNodeManager;
        for (int index2 = path.Count - 2; index2 >= 0; --index2)
        {
          if (path[index2] is Model3D)
          {
            if (typeof (Visual3D).IsAssignableFrom(currentViewNode.TargetType))
            {
              ModelVisual3D modelVisual3D = visual3D as ModelVisual3D;
              ModelUIElement3D modelUiElement3D = visual3D as ModelUIElement3D;
              if (modelVisual3D != null)
              {
                model3D = modelVisual3D.Content;
                currentViewNode.Properties.TryGetValue(platformMetadata.ResolveProperty(ModelVisual3DElement.ContentProperty), out currentViewNode);
              }
              else if (modelUiElement3D != null)
              {
                model3D = modelUiElement3D.Model;
                currentViewNode.Properties.TryGetValue(platformMetadata.ResolveProperty(ModelUIElement3DElement.ModelProperty), out currentViewNode);
              }
            }
            else
            {
              Model3DGroup model3Dgroup = (Model3DGroup) model3D;
              model3D = (Model3D) path[index2];
              index1 = model3Dgroup.Children.IndexOf(model3D);
              currentViewNode.Properties.TryGetValue(platformMetadata.ResolveProperty(Model3DGroupElement.ChildrenProperty), out currentViewNode);
              if (currentViewNode != null)
                currentViewNode = currentViewNode.Children[index1];
            }
          }
          else if (path[index2] is Visual3D)
          {
            ModelVisual3D modelVisual3D = visual3D as ModelVisual3D;
            ContainerUIElement3D containerUiElement3D = visual3D as ContainerUIElement3D;
            visual3D = (Visual3D) path[index2];
            if (modelVisual3D != null)
            {
              index1 = modelVisual3D.Children.IndexOf(visual3D);
              currentViewNode.Properties.TryGetValue(platformMetadata.ResolveProperty(ModelVisual3DElement.ChildrenProperty), out currentViewNode);
            }
            else if (containerUiElement3D != null)
            {
              index1 = containerUiElement3D.Children.IndexOf(visual3D);
              currentViewNode.Properties.TryGetValue(platformMetadata.ResolveProperty(ContainerUIElement3DElement.ChildrenProperty), out currentViewNode);
            }
            if (currentViewNode != null)
              currentViewNode = currentViewNode.Children[index1];
          }
          if (currentViewNode == null)
            return (ViewNode) null;
          if (viewNodeManager.IsContainerRoot(currentViewNode))
            break;
        }
        return currentViewNode;
      }

      private HitTestResultBehavior RecordHitTestResult(IViewObject viewObject)
      {
        return this.RecordHitTestResult(viewObject, true);
      }

      private HitTestResultBehavior RecordHitTestResult(IViewObject viewObject, bool lookForInvisibleObjects)
      {
        if (this.IsNonHitTestableExternalViewObject(viewObject))
          return HitTestResultBehavior.Continue;
        DocumentNodePath correspondingNodePath = this.GetCorrespondingNodePath(viewObject);
        if (correspondingNodePath != null)
        {
          SceneNode editingContainer = this.sceneView.ViewModel.ActiveEditingContainer;
          if (editingContainer != null && correspondingNodePath.ContainerNode != editingContainer.DocumentNode && !(bool) viewObject.GetCurrentValue(this.sceneView.ProjectContext.ResolveProperty(DesignTimeProperties.RuntimeIsHitTestVisibleProperty)))
            return HitTestResultBehavior.Continue;
          SceneElement key = this.sceneView.ViewModel.GetSceneNode(correspondingNodePath.Node) as SceneElement;
          if (key != null)
          {
            if (lookForInvisibleObjects)
            {
              IViewVisual hitElement = viewObject as IViewVisual;
              if (hitElement == null)
              {
                Base3DElement base3Delement = key as Base3DElement;
                if (base3Delement != null)
                {
                  Viewport3DElement viewport = base3Delement.Viewport;
                  if (viewport != null)
                  {
                    DocumentNode documentNode = viewport.DocumentNode;
                    correspondingNodePath.GetContainerNodePath();
                    hitElement = (IViewVisual) this.sceneView.GetCorrespondingViewObject(correspondingNodePath.GetPathInContainer(documentNode));
                  }
                }
              }
              if (hitElement == null)
                return HitTestResultBehavior.Continue;
              HitTestResultBehavior testResultBehavior = this.LookForInvisibleObjectsUntil(hitElement);
              if (testResultBehavior == HitTestResultBehavior.Stop)
                return testResultBehavior;
            }
            if (this.SkipFullyContainedSelectionInObject(viewObject))
              return HitTestResultBehavior.Continue;
            if (this.hitTestModifier != null)
              key = this.hitTestModifier(correspondingNodePath);
          }
          if (key != null && !this.hitElementCache.ContainsKey(key))
          {
            this.hitElementCache.Add(key, true);
            this.hitElements.Add(key);
            if (this.stopAfterFirstHit)
              return HitTestResultBehavior.Stop;
          }
        }
        return HitTestResultBehavior.Continue;
      }

      private bool SkipFullyContainedSelectionInObject(IViewObject viewObject)
      {
        if (!this.skipFullyContainedSelectionInObject)
          return false;
        IViewVisual element = viewObject as IViewVisual;
        return element != null && this.HitTestBounds(this.sceneView.GetActualBounds(viewObject), element) == IntersectionDetail.FullyInside && this.HitElements.Count != 0;
      }

      private HitTestResultBehavior LookForInvisibleObjectsUntil(IViewVisual hitElement)
      {
        HitTestResultBehavior testResultBehavior = HitTestResultBehavior.Continue;
        if (hitElement != null)
        {
          IPlatformMetadata platformMetadata = (IPlatformMetadata) this.sceneView.Platform.Metadata;
          IProjectContext projectContext = this.SceneView.ProjectContext;
          while (this.treeEnumerator.MoveNext() && !this.treeEnumerator.Current.Equals((object) hitElement))
          {
            IType itype = this.treeEnumerator.Current.GetIType((ITypeResolver) projectContext);
            if (!platformMetadata.IsNullType((ITypeId) itype) && this.invisibleHitTestTypeHandlerFactory.ShouldInvisibleHitTest((ITypeResolver) projectContext, itype))
            {
              IViewVisual current = this.treeEnumerator.Current;
              bool flag = this.HitTestSingleInvisibleElement(current);
              IViewVisual element = current;
              if (JoltHelper.TypeSupported((ITypeResolver) projectContext, PlatformTypes.ScrollViewer))
              {
                while (flag && element != this.sceneView.ViewRoot && element != null)
                {
                  element = element.VisualParent as IViewVisual;
                  if (element != null && element.GetIType((ITypeResolver) projectContext).IsAssignableFrom((ITypeId) projectContext.ResolveType(PlatformTypes.ScrollViewer)))
                    flag = this.HitTestSingleInvisibleElement(element);
                }
              }
              if ((!flag || this.invisibleObjectHitTestModifier == null || this.invisibleObjectHitTestModifier((IHitTestHelperService) this, (IViewObject) hitElement, (IViewObject) current) != InvisibleObjectHitTestResult.None) && flag)
              {
                DocumentNodePath correspondingNodePath = this.GetCorrespondingNodePath((IViewObject) current);
                if (correspondingNodePath != null)
                {
                  SceneElement sceneElement = this.sceneView.ViewModel.GetSceneNode(correspondingNodePath.Node) as SceneElement;
                  if (sceneElement != null && sceneElement.ViewTargetElement != null && !((IViewVisual) sceneElement.ViewTargetElement).IsAncestorOf(hitElement))
                  {
                    testResultBehavior = this.RecordHitTestResult((IViewObject) current, false);
                    if (testResultBehavior == HitTestResultBehavior.Stop)
                      return testResultBehavior;
                  }
                }
              }
            }
          }
        }
        return testResultBehavior;
      }

      private bool HitTestSingleInvisibleElement(IViewVisual element)
      {
        if (element.Visibility == Visibility.Collapsed)
          return false;
        IViewShape shape = element as IViewShape;
        return shape == null ? this.HitTestBounds(this.sceneView.GetActualBounds((IViewObject) element), element) != IntersectionDetail.Empty : this.HitTestInvisibleShape(shape);
      }

      private bool HitTestInvisibleShape(IViewShape shape)
      {
        if (shape.Platform.Metadata.IsCapabilitySet(PlatformCapability.IsWpf) && shape.GetValue(this.sceneView.ProjectContext.ResolveProperty(ShapeElement.FillProperty)) != null || this.HitTestBounds(this.sceneView.GetActualBounds((IViewObject) shape), (IViewVisual) shape) == IntersectionDetail.Empty)
          return false;
        System.Windows.Media.Geometry geometry1 = shape.RenderedGeometry;
        if (geometry1 == null && !this.sceneView.Document.ProjectContext.IsCapabilitySet(PlatformCapability.IsWpf))
        {
          DocumentNodePath correspondingNodePath = this.GetCorrespondingNodePath((IViewObject) shape);
          if (correspondingNodePath != null)
          {
            ShapeElement shapeElement = this.sceneView.ViewModel.GetSceneNode(correspondingNodePath.Node) as ShapeElement;
            if (shapeElement != null && shapeElement.IsViewObjectValid)
              geometry1 = this.sceneView.GetRenderedGeometryAsWpf((SceneElement) shapeElement);
          }
        }
        if (geometry1 == null)
          return false;
        if (this.hitTestParameters is PointHitTestParameters)
        {
          Point hitPoint = this.TransformHitPoint((IViewVisual) shape);
          return geometry1.FillContains(hitPoint);
        }
        System.Windows.Media.Geometry geometry2 = this.TransformHitGeometry((IViewVisual) shape);
        return geometry1.FillContainsWithDetail(geometry2) != IntersectionDetail.Empty;
      }

      private SceneElement GetCorrespondingSceneElement(IViewObject visualOrModel3D, IParentChainProvider parentChain)
      {
        DocumentNode correspondingDocumentNode = this.GetCorrespondingDocumentNode(visualOrModel3D, parentChain);
        if (correspondingDocumentNode != null)
        {
          SceneViewModel viewModel = this.sceneView.ViewModel;
          if (!viewModel.IsExternal(correspondingDocumentNode))
            return viewModel.GetSceneNode(correspondingDocumentNode) as SceneElement;
        }
        return (SceneElement) null;
      }

      private DocumentNode GetCorrespondingDocumentNode(IViewObject visualOrModel3D, IParentChainProvider parentChain)
      {
        int index = 0;
        while (visualOrModel3D != null)
        {
          DocumentNode correspondingDocumentNode = this.sceneView.GetCorrespondingDocumentNode(visualOrModel3D, false);
          if (correspondingDocumentNode != null && !this.sceneView.ViewModel.IsExternal(correspondingDocumentNode))
            return correspondingDocumentNode;
          IViewVisual viewVisual = visualOrModel3D as IViewVisual;
          if (viewVisual != null)
          {
            if (parentChain == null)
            {
              visualOrModel3D = viewVisual != null ? viewVisual.VisualParent : (IViewObject) null;
            }
            else
            {
              visualOrModel3D = (IViewObject) parentChain.GetParent(index);
              ++index;
            }
          }
          else
          {
            Visual3D visual3D = visualOrModel3D.PlatformSpecificObject as Visual3D;
            visualOrModel3D = visual3D != null ? this.sceneView.Platform.ViewObjectFactory.Instantiate((object) VisualTreeHelper.GetParent((DependencyObject) visual3D)) : (IViewObject) null;
          }
        }
        return (DocumentNode) null;
      }

      private bool IsNonHitTestableExternalViewObject(IViewObject viewObject)
      {
        ViewNode viewNode = this.sceneView.InstanceDictionary.GetViewNode(viewObject.PlatformSpecificObject, true);
        return viewNode != null && this.sceneView.ViewModel.IsExternal(viewNode.DocumentNode) && !(bool) viewObject.GetCurrentValue(this.sceneView.ProjectContext.ResolveProperty(DesignTimeProperties.RuntimeIsHitTestVisibleProperty));
      }

      private DocumentNodePath GetCorrespondingNodePath(IViewObject visualOrModel3D)
      {
        while (visualOrModel3D != null)
        {
          DocumentNodePath correspondingNodePath = this.sceneView.GetCorrespondingNodePath(visualOrModel3D, false);
          if (correspondingNodePath != null && !this.sceneView.ViewModel.IsExternal(correspondingNodePath.Node))
            return correspondingNodePath;
          IViewVisual viewVisual = visualOrModel3D as IViewVisual;
          if (viewVisual != null)
          {
            visualOrModel3D = viewVisual != null ? viewVisual.VisualParent : (IViewObject) null;
          }
          else
          {
            Visual3D visual3D = visualOrModel3D.PlatformSpecificObject as Visual3D;
            visualOrModel3D = visual3D != null ? this.sceneView.Platform.ViewObjectFactory.Instantiate((object) VisualTreeHelper.GetParent((DependencyObject) visual3D)) : (IViewObject) null;
          }
        }
        return (DocumentNodePath) null;
      }

      private Point TransformHitPoint(IViewVisual element)
      {
        Point point = ((PointHitTestParameters) this.hitTestParameters).HitPoint;
        GeneralTransform transformToHitArea = this.sceneView.GetComputedTransformToHitArea();
        if (transformToHitArea != null)
          point = transformToHitArea.Inverse.Transform(point);
        return this.sceneView.TransformPoint((IViewObject) this.sceneView.HitTestRoot, (IViewObject) element, point);
      }

      private System.Windows.Media.Geometry TransformHitGeometry(IViewVisual element)
      {
        Rect rect = ((GeometryHitTestParameters) this.hitTestParameters).HitGeometry.Clone().Bounds;
        GeneralTransform transformToHitArea = this.sceneView.GetComputedTransformToHitArea();
        if (transformToHitArea != null)
          rect = transformToHitArea.Inverse.TransformBounds(rect);
        return (System.Windows.Media.Geometry) new RectangleGeometry(this.sceneView.TransformBounds((IViewObject) this.sceneView.HitTestRoot, (IViewObject) element, rect));
      }

      public IntersectionDetail HitTestBounds(Rect bounds, IViewVisual element)
      {
        if (!(this.hitTestParameters is PointHitTestParameters))
          return this.TransformHitGeometry(element).FillContainsWithDetail((System.Windows.Media.Geometry) new RectangleGeometry(bounds));
        Point point = this.TransformHitPoint(element);
        return bounds.Contains(point) ? IntersectionDetail.FullyInside : IntersectionDetail.Empty;
      }

      public IViewObject QueryHitTestModifier(IViewObject hitVisual)
      {
        if (hitVisual == null)
          return (IViewObject) null;
        DocumentNodePath correspondingNodePath = this.GetCorrespondingNodePath(hitVisual);
        if (correspondingNodePath == null)
          return (IViewObject) null;
        BaseFrameworkElement frameworkElement = this.sceneView.ViewModel.GetSceneNode(correspondingNodePath.Node) as BaseFrameworkElement;
        if (frameworkElement != null && this.ignoredElements != null && this.ignoredElements.Contains(frameworkElement))
          return (IViewObject) null;
        if (this.hitTestModifier == null)
          return hitVisual;
        SceneElement sceneElement = this.hitTestModifier(correspondingNodePath);
        if (sceneElement != null)
          return sceneElement.ViewTargetElement;
        return (IViewObject) null;
      }

      private sealed class InvisibleHitTestTypeHandlerFactory : TypeIdHandlerFactory<SceneView.HitTestHelper.InvisibleHitTestTypeHandler>
      {
        public bool ShouldInvisibleHitTest(ITypeResolver typeResolver, IType type)
        {
          SceneView.HitTestHelper.InvisibleHitTestTypeHandler handler = this.GetHandler((IMetadataResolver) typeResolver, type);
          if (handler != null)
            return handler.InvisibleHitTest;
          return false;
        }

        protected override void Initialize()
        {
          base.Initialize();
          this.RegisterHandler(new SceneView.HitTestHelper.InvisibleHitTestTypeHandler(PlatformTypes.UIElement, false));
          this.RegisterHandler(new SceneView.HitTestHelper.InvisibleHitTestTypeHandler(PlatformTypes.Shape, true));
          this.RegisterHandler(new SceneView.HitTestHelper.InvisibleHitTestTypeHandler(PlatformTypes.Panel, true));
          this.RegisterHandler(new SceneView.HitTestHelper.InvisibleHitTestTypeHandler(PlatformTypes.Decorator, true));
          this.RegisterHandler(new SceneView.HitTestHelper.InvisibleHitTestTypeHandler(PlatformTypes.Border, true));
          this.RegisterHandler(new SceneView.HitTestHelper.InvisibleHitTestTypeHandler(PlatformTypes.Control, true));
          this.RegisterHandler(new SceneView.HitTestHelper.InvisibleHitTestTypeHandler(PlatformTypes.TextBlock, true));
          this.RegisterHandler(new SceneView.HitTestHelper.InvisibleHitTestTypeHandler(PlatformTypes.StandInPopup, false));
          this.RegisterHandler(new SceneView.HitTestHelper.InvisibleHitTestTypeHandler(PlatformTypes.Viewport3D, true));
          this.RegisterHandler(new SceneView.HitTestHelper.InvisibleHitTestTypeHandler(PlatformTypes.Image, true));
          this.RegisterHandler(new SceneView.HitTestHelper.InvisibleHitTestTypeHandler(PlatformTypes.MediaElement, true));
          this.RegisterHandler(new SceneView.HitTestHelper.InvisibleHitTestTypeHandler(PlatformTypes.ContentPresenter, true));
          this.RegisterHandler(new SceneView.HitTestHelper.InvisibleHitTestTypeHandler(PlatformTypes.ItemsPresenter, true));
          this.RegisterHandler(new SceneView.HitTestHelper.InvisibleHitTestTypeHandler(PlatformTypes.TickBar, true));
          this.RegisterHandler(new SceneView.HitTestHelper.InvisibleHitTestTypeHandler(PlatformTypes.MultiScaleImage, true));
          this.RegisterHandler(new SceneView.HitTestHelper.InvisibleHitTestTypeHandler(ProjectNeutralTypes.Viewbox, true));
        }

        protected override ITypeId GetBaseType(SceneView.HitTestHelper.InvisibleHitTestTypeHandler handler)
        {
          return handler.BaseType;
        }
      }

      private sealed class InvisibleHitTestTypeHandler
      {
        private ITypeId baseType;
        private bool invisibleHitTest;

        public ITypeId BaseType
        {
          get
          {
            return this.baseType;
          }
        }

        public bool InvisibleHitTest
        {
          get
          {
            return this.invisibleHitTest;
          }
        }

        public InvisibleHitTestTypeHandler(ITypeId baseType, bool invisibleHitTest)
        {
          this.baseType = baseType;
          this.invisibleHitTest = invisibleHitTest;
        }
      }

      private sealed class PostfixVisualEnumerator : IEnumerator<IViewVisual>, IDisposable, IEnumerator, IParentChainProvider
      {
        private bool isBeforeFirstElement = true;
        private List<SceneView.HitTestHelper.PostfixVisualEnumerator.Node> stack = new List<SceneView.HitTestHelper.PostfixVisualEnumerator.Node>();
        private IViewVisual root;
        private SceneView.HitTestHelper helper;

        public IViewVisual Current
        {
          get
          {
            if (this.stack.Count == 0)
              throw new InvalidOperationException(ExceptionStringTable.ElementUtilitiesEnumeratorOutOfRangeError);
            return this.stack[this.stack.Count - 1].Current;
          }
        }

        object IEnumerator.Current
        {
          get
          {
            return (object) this.Current;
          }
        }

        public PostfixVisualEnumerator(SceneView.HitTestHelper helper, IViewVisual root)
        {
          if (root == null)
            throw new ArgumentNullException("root");
          this.root = root;
          this.helper = helper;
        }

        public void Dispose()
        {
          this.Reset();
        }

        public void Reset()
        {
          this.isBeforeFirstElement = true;
          this.stack.Clear();
        }

        public bool MoveNext()
        {
          if (this.isBeforeFirstElement)
          {
            if (this.root == null)
              return false;
            this.isBeforeFirstElement = false;
            this.stack.Add(new SceneView.HitTestHelper.PostfixVisualEnumerator.Node(this.root));
          }
          while (this.stack.Count > 0)
          {
            SceneView.HitTestHelper.PostfixVisualEnumerator.Node node = this.stack[this.stack.Count - 1];
            if (node.MoveNext() && node.Current != null)
            {
              this.stack[this.stack.Count - 1] = node;
              switch (this.helper.EarlyFilterPotentialHit((IViewObject) node.Current, (IParentChainProvider) this))
              {
                case HitTestFilterBehavior.ContinueSkipSelfAndChildren:
                  if (node.Current is Viewport3D)
                  {
                    this.stack.Add(new SceneView.HitTestHelper.PostfixVisualEnumerator.Node(node.Current));
                    continue;
                  }
                  continue;
                case HitTestFilterBehavior.Stop:
                  return false;
                default:
                  this.stack.Add(new SceneView.HitTestHelper.PostfixVisualEnumerator.Node(node.Current));
                  continue;
              }
            }
            else
            {
              this.stack.RemoveAt(this.stack.Count - 1);
              return this.stack.Count > 0;
            }
          }
          return false;
        }

        public IViewVisual GetParent(int index)
        {
          if (index >= this.stack.Count)
            return (IViewVisual) null;
          return this.stack[this.stack.Count - 1 - index].Parent;
        }

        private struct Node
        {
          private IViewVisual visual;
          private IViewVisual currentChild;
          private int index;

          public IViewVisual Current
          {
            get
            {
              if (this.index >= 0 && this.currentChild == null)
                this.currentChild = this.visual.GetVisualChild(this.index);
              return this.currentChild;
            }
          }

          public IViewVisual Parent
          {
            get
            {
              return this.visual;
            }
          }

          public Node(IViewVisual visual)
          {
            this.visual = visual;
            this.currentChild = (IViewVisual) null;
            int visualChildrenCount = visual.VisualChildrenCount;
            this.index = visualChildrenCount > 0 ? visualChildrenCount : -1;
          }

          public bool MoveNext()
          {
            this.currentChild = (IViewVisual) null;
            --this.index;
            return this.index >= 0;
          }
        }
      }
    }

    protected enum PostponedUpdate
    {
      None,
      Update,
      InvalidateAndUpdate,
      UpdateInvalidated,
    }

    private delegate void RefreshParentErrors(bool forceShowErrorsWhenXamlFocused);

    private enum DocumentRelation
    {
      Unrelated,
      Related,
      OfflineSampleData,
    }

    internal class ExtensibilityCommand : Command
    {
      public override void Execute()
      {
      }
    }

    private class LimitedFrequencyAutoScroller : AutoScroller
    {
      private Visual ensureVisibleVisual;
      private Rect ensureVisibleRect;
      private SceneView hostSceneView;

      public LimitedFrequencyAutoScroller(SceneView sceneView)
      {
        this.hostSceneView = sceneView;
      }

      public void StartScroll(Visual visual, Rect rect)
      {
        if (visual == null)
          return;
        this.ensureVisibleRect = rect;
        this.ensureVisibleVisual = visual;
        this.ScrollTimer.Start();
      }

      protected override bool DoScroll()
      {
        if (this.hostSceneView.Artboard != null && this.hostSceneView.IsValid && this.ensureVisibleVisual != null)
          this.hostSceneView.Artboard.MakeVisible(this.ensureVisibleVisual, this.ensureVisibleRect);
        return false;
      }
    }

    public delegate void HandleAnimationChange(SceneElement element, PropertyReference propertyReference);

    private class HitElementHelper
    {
      private SceneView sceneView;
      private SelectionFor3D selectionFor3D;
      private bool selectedOnly;
      private bool smartInvisiblePanelSelect;

      public HitElementHelper(SceneView sceneView, SelectionFor3D selectionFor3D, bool selectedOnly, bool smartInvisiblePanelSelect)
      {
        this.sceneView = sceneView;
        this.selectionFor3D = selectionFor3D;
        this.selectedOnly = selectedOnly;
        this.smartInvisiblePanelSelect = smartInvisiblePanelSelect;
      }

      public SceneElement GetSelectableElementAtPoint(Point point)
      {
        return this.sceneView.GetElementAtPoint(point, new HitTestModifier(this.GetSelectableElement), new InvisibleObjectHitTestModifier(this.GetInvisibleElementReturned), (ICollection<BaseFrameworkElement>) null);
      }

      public IList<SceneElement> GetSelectableElementsInRectangle(Rect rectangle)
      {
        return this.sceneView.GetElementsInRectangle(rectangle, new HitTestModifier(this.GetSelectableElement), new InvisibleObjectHitTestModifier(this.GetInvisibleElementReturned), true);
      }

      private InvisibleObjectHitTestResult GetInvisibleElementReturned(IHitTestHelperService hitTestHelperService, IViewObject hitElement, IViewObject invisibleElement)
      {
        if (!this.smartInvisiblePanelSelect)
          return InvisibleObjectHitTestResult.Fill;
        InvisibleObjectHitTestResult objectHitTestResult = SceneView.SmartInvisiblePanelSelect(hitTestHelperService, hitElement, invisibleElement);
        if (objectHitTestResult == InvisibleObjectHitTestResult.Stroke && this.sceneView.NoInvisiblePanelStrokeHitTesting)
          return InvisibleObjectHitTestResult.None;
        IViewPanel viewPanel = invisibleElement as IViewPanel;
        IViewVisual viewVisual = this.sceneView.ViewModel.ElementSelectionSet.PrimarySelection != null ? (this.sceneView.ViewModel.ElementSelectionSet.PrimarySelection.ViewObject != null ? this.sceneView.ViewModel.ElementSelectionSet.PrimarySelection.ViewObject as IViewVisual : (IViewVisual) null) : (IViewVisual) null;
        if (this.sceneView.InvisiblePanelContainerOfSelectionDisable && objectHitTestResult == InvisibleObjectHitTestResult.None && (viewPanel != null && viewVisual != null) && (viewVisual.VisualParent != null && viewVisual.VisualParent.Equals((object) viewPanel)))
          return InvisibleObjectHitTestResult.Fill;
        return objectHitTestResult;
      }

      private static bool IsSelectable(ISceneInsertionPoint activeInsertionPoint, SceneElement element, SceneElement effectiveParent)
      {
        return activeInsertionPoint != null && (activeInsertionPoint.SceneElement == element || effectiveParent != null && activeInsertionPoint.SceneElement == effectiveParent);
      }

      private SceneElement GetSelectableElement(DocumentNodePath nodePath)
      {
        SceneElement element = this.sceneView.GetSelectableElement(nodePath);
        SceneElement effectiveParent;
        if (element is Base3DElement && this.selectionFor3D == SelectionFor3D.TopLevel)
        {
          for (; element != null; element = effectiveParent)
          {
            effectiveParent = element.EffectiveParent;
            if (element.GetIsVisuallySelectable(false) && (SceneView.HitElementHelper.IsSelectable(this.sceneView.ViewModel.ActiveSceneInsertionPoint, element, effectiveParent) || element == this.sceneView.ViewModel.ActiveEditingContainer))
              break;
          }
        }
        if (element != null && !element.IsVisuallySelectable)
          element = (SceneElement) null;
        if (element != null && this.selectionFor3D == SelectionFor3D.None && (element is Base3DElement || element is Viewport3DElement))
          element = (SceneElement) null;
        if (element != null && this.selectedOnly && !this.sceneView.ViewModel.ElementSelectionSet.IsSelected(element))
          element = (SceneElement) null;
        return element;
      }
    }

    private class AppliedStoryboardData
    {
      public SceneElement TargetElement { get; private set; }

      public IViewStoryboard ViewStoryboard { get; private set; }

      public AppliedStoryboardData(SceneElement targetElement, IViewStoryboard viewStoryboard)
      {
        this.TargetElement = targetElement;
        this.ViewStoryboard = viewStoryboard;
      }
    }

    private sealed class EmptyCodeEditor : ITextEditor, IDisposable
    {
      private FrameworkElement element = (FrameworkElement) new Canvas();
      private Dictionary<string, Command> emptyDictionary = new Dictionary<string, Command>();

      public int CaretPosition
      {
        get
        {
          return 0;
        }
        set
        {
        }
      }

      public string Text
      {
        get
        {
          return string.Empty;
        }
      }

      public int LineCount
      {
        get
        {
          return 0;
        }
      }

      public bool IsSelectionEmpty
      {
        get
        {
          return true;
        }
      }

      public int SelectionStart
      {
        get
        {
          return 0;
        }
      }

      public int SelectionLength
      {
        get
        {
          return 0;
        }
      }

      public bool CanUndo
      {
        get
        {
          return false;
        }
      }

      public bool CanRedo
      {
        get
        {
          return false;
        }
      }

      public int SquiggleCount
      {
        get
        {
          return 0;
        }
      }

      public FrameworkElement Element
      {
        get
        {
          return this.element;
        }
      }

      public Thickness VerticalScrollBarMargin
      {
        get
        {
          return new Thickness();
        }
        set
        {
        }
      }

      public string FontName
      {
        get
        {
          return "Arial";
        }
      }

      public double FontSize
      {
        get
        {
          return 10.0;
        }
      }

      public bool ConvertTabsToSpace
      {
        get
        {
          return false;
        }
      }

      public int TabSize
      {
        get
        {
          return 1;
        }
      }

      public bool WordWrap
      {
        get
        {
          return false;
        }
      }

      public event EventHandler GotFocus
      {
        add
        {
        }
        remove
        {
        }
      }

      public event EventHandler LostFocus
      {
        add
        {
        }
        remove
        {
        }
      }

      public event EventHandler CaretPositionChanged
      {
        add
        {
        }
        remove
        {
        }
      }

      public void Activated()
      {
      }

      public void Focus()
      {
      }

      public int GetLengthOfLineFromLineNumber(int value)
      {
        return 0;
      }

      public int GetLineNumberFromPosition(int value)
      {
        return 0;
      }

      public int GetLineOffsetFromPosition(int value)
      {
        return 0;
      }

      public int GetStartOfLineFromLineNumber(int value)
      {
        return 0;
      }

      public int GetStartOfNextLineFromPosition(int value)
      {
        return 0;
      }

      public string GetText(int offset, int length)
      {
        return string.Empty;
      }

      public void Select(int start, int length)
      {
      }

      public void ClearSelection()
      {
      }

      public void EnsureSpanVisible(int start, int length)
      {
      }

      public void EnsureCaretVisible()
      {
      }

      public void MoveLineToCenterOfView(int lineNumber)
      {
      }

      public void GotoLine(int lineNumber)
      {
      }

      public void SelectCurrentWord()
      {
      }

      public int Find(string findText, bool matchCase, bool searchReverse)
      {
        return 0;
      }

      public int Replace(string findText, string replaceText, bool matchCase)
      {
        return 0;
      }

      public bool ReplaceAll(string findText, string replaceText, bool matchCase)
      {
        return false;
      }

      public TextEditorErrorInformation GetSquiggleInformation(int squiggleIndex)
      {
        return new TextEditorErrorInformation(0, 0, string.Empty);
      }

      public void UpdateErrorRanges(List<TextEditorErrorInformation> errorRanges)
      {
      }

      public IDictionary GetEditCommands()
      {
        return (IDictionary) this.emptyDictionary;
      }

      public IDictionary GetUndoCommands()
      {
        return (IDictionary) this.emptyDictionary;
      }

      public void Dispose()
      {
        GC.SuppressFinalize((object) this);
      }
    }

    public enum AnimationChangeResult
    {
      NotAnimationChange,
      AnimationChange,
      InvalidateAll,
    }

    internal interface ISceneErrorTask : IErrorTask
    {
      ITextRange Span { get; }

      IDocumentRoot Document { get; }
    }

    internal sealed class ErrorTask : SceneView.ISceneErrorTask, IErrorTask
    {
      private ProjectXamlContext projectContext;
      private string project;
      private string file;
      private XamlParseError error;

      public ErrorSeverity Severity
      {
        get
        {
          return this.error.Severity != XamlErrorSeverity.Error ? ErrorSeverity.Warning : ErrorSeverity.Error;
        }
      }

      public string Description
      {
        get
        {
          return this.error.Message;
        }
      }

      public string ExtendedDescription
      {
        get
        {
          return this.error.Message;
        }
      }

      public string Project
      {
        get
        {
          return this.project;
        }
      }

      public string File
      {
        get
        {
          return Path.GetFileName(this.file);
        }
      }

      public string FullFileName
      {
        get
        {
          return Microsoft.Expression.Framework.Documents.PathHelper.ResolvePath(this.file);
        }
      }

      public int? Line
      {
        get
        {
          return new int?(this.error.Line);
        }
      }

      public int? Column
      {
        get
        {
          return new int?(this.error.Column);
        }
      }

      public ITextRange Span
      {
        get
        {
          SceneView view = this.GetView(false);
          if (view != null)
            return SceneView.GetSpan((IReadableTextBuffer) view.ViewModel.XamlDocument.TextBuffer, view.CodeEditor, this.error.Line, this.error.Column + 1);
          return Microsoft.Expression.DesignModel.Code.TextRange.Null;
        }
      }

      public IDocumentRoot Document
      {
        get
        {
          if (this.projectContext != null)
          {
            IProjectDocument projectDocument = this.projectContext.OpenDocument(this.file);
            if (projectDocument != null)
              return projectDocument.DocumentRoot;
          }
          return (IDocumentRoot) null;
        }
      }

      public System.Windows.Input.ICommand InvokeCommand
      {
        get
        {
          return (System.Windows.Input.ICommand) new DelegateCommand((DelegateCommand.SimpleEventHandler) (() => SceneView.ChangeCaretPositionInternal(this.GetView(true), this.error.Line, this.error.Column + 1)));
        }
      }

      public ErrorTask(ProjectXamlContext projectContext, string project, string file, XamlParseError error)
      {
        this.projectContext = projectContext;
        this.project = project;
        this.file = file;
        this.error = error;
      }

      private SceneView GetView(bool makeActive)
      {
        IDocumentRoot document = this.Document;
        if (document != null)
        {
          SceneView sceneView = this.projectContext.OpenView(document, makeActive);
          if (sceneView != null)
            return sceneView;
        }
        return (SceneView) null;
      }
    }

    internal sealed class WarningTask : IErrorTask
    {
      private string message;
      private string project;
      private string file;
      private ErrorSeverity severity;

      public ErrorSeverity Severity
      {
        get
        {
          return this.severity;
        }
      }

      public string Description
      {
        get
        {
          return this.message;
        }
      }

      public string ExtendedDescription
      {
        get
        {
          return this.message;
        }
      }

      public string Project
      {
        get
        {
          return this.project;
        }
      }

      public string File
      {
        get
        {
          return Path.GetFileName(this.file);
        }
      }

      public string FullFileName
      {
        get
        {
          return Microsoft.Expression.Framework.Documents.PathHelper.ResolvePath(this.file);
        }
      }

      public int? Line
      {
        get
        {
          return new int?();
        }
      }

      public int? Column
      {
        get
        {
          return new int?();
        }
      }

      public System.Windows.Input.ICommand InvokeCommand
      {
        get
        {
          return (System.Windows.Input.ICommand) null;
        }
      }

      public WarningTask(string message, string project, string file, ErrorSeverity severity)
      {
        this.message = message;
        this.severity = severity;
        this.project = project;
        this.file = file;
      }
    }

    private class MessageBubbleHost
    {
      private MessageBubble messageBubble;
      private DispatcherTimer timer;
      private int milliseconds;

      public MessageBubbleHost(int milliseconds)
      {
        this.milliseconds = milliseconds;
      }

      public void Show(MessageBubble messageBubble)
      {
        if (this.timer != null)
          this.timer.Stop();
        this.timer = new DispatcherTimer();
        this.timer.Interval = TimeSpan.FromMilliseconds((double) this.milliseconds);
        this.timer.Tick += new EventHandler(this.OnTimerTick);
        this.timer.Start();
        if (this.messageBubble != null)
          this.messageBubble.IsOpen = false;
        this.messageBubble = messageBubble;
        this.messageBubble.IsOpen = true;
      }

      private void OnTimerTick(object sender, EventArgs e)
      {
        this.timer.Stop();
        this.timer = (DispatcherTimer) null;
        this.messageBubble.IsOpen = false;
        this.messageBubble = (MessageBubble) null;
      }
    }

    internal sealed class SelectionSynchronizer
    {
      private SceneView sceneView;
      private bool isUpdatingSelectionFromCursor;
      private int isUpdatingSelectionFromSceneCount;
      private DispatcherTimer caretPositionChangedTimeoutTimer;
      private bool isDirtyStateRelevant;

      public bool IsUpdatingSelectionFromScene
      {
        get
        {
          return this.isUpdatingSelectionFromSceneCount != 0;
        }
      }

      public SelectionSynchronizer(SceneView sceneView)
      {
        this.sceneView = sceneView;
        this.sceneView.ViewModel.LateSceneUpdatePhase += new SceneUpdatePhaseEventHandler(this.ViewModel_LateSceneUpdatePhase);
      }

      internal IDisposable DisableSelectionSynchronization()
      {
        return (IDisposable) new SceneView.SelectionSynchronizer.SelectionSynchronizingToken(this);
      }

      public void Unhook()
      {
        this.sceneView.ViewModel.LateSceneUpdatePhase -= new SceneUpdatePhaseEventHandler(this.ViewModel_LateSceneUpdatePhase);
        this.sceneView = (SceneView) null;
        if (this.caretPositionChangedTimeoutTimer == null)
          return;
        this.caretPositionChangedTimeoutTimer.Stop();
        this.caretPositionChangedTimeoutTimer = (DispatcherTimer) null;
      }

      public void EnsureXamlSynchronizedToScene(bool forceUpdate)
      {
        if (!forceUpdate && !this.isDirtyStateRelevant || !this.sceneView.ViewModel.XamlDocument.IsTextUpToDate)
          return;
        this.SynchronizeXamlToScene();
        this.isDirtyStateRelevant = false;
      }

      private void SynchronizeXamlToScene()
      {
        if (this.sceneView.ViewMode != ViewMode.Split || this.sceneView.FocusedEditor != FocusedEditor.Design || (this.isUpdatingSelectionFromCursor || !this.sceneView.ViewModel.XamlDocument.IsEditable))
          return;
        PerformanceUtility.StartPerformanceSequence(PerformanceEvent.SelectionSynchronize);
        DesignerContext designerContext = this.sceneView.ViewModel.DesignerContext;
        SceneNode sceneNode = (SceneNode) null;
        if (designerContext.SelectionManager.ElementSelectionSet != null && designerContext.SelectionManager.ElementSelectionSet.PrimarySelection != null)
        {
          sceneNode = (SceneNode) designerContext.SelectionManager.ElementSelectionSet.PrimarySelection;
        }
        else
        {
          SceneNode[] selectedNodes = designerContext.SelectionManager.SelectedNodes;
          if (selectedNodes != null && selectedNodes.Length > 0)
            sceneNode = selectedNodes[0];
        }
        if (sceneNode != null && sceneNode.DocumentNode.DocumentRoot == this.sceneView.ViewModel.XamlDocument)
          GoToXamlCommand.GoToXaml(this.sceneView, this.sceneView.ViewModel.XamlDocument, new List<DocumentNode>(1)
          {
            sceneNode.DocumentNode
          }, true, false);
        PerformanceUtility.EndPerformanceSequence(PerformanceEvent.SelectionSynchronize);
      }

      public void SynchronizeSceneToXaml(bool updateImmediately)
      {
        if (this.sceneView.ViewMode != ViewMode.Split || this.sceneView.FocusedEditor != FocusedEditor.Code || (this.sceneView.Document.IsUndoingOrRedoing || this.IsUpdatingSelectionFromScene))
          return;
        if (updateImmediately)
        {
          this.OnCaretPositionChangedTimeoutTimerTick((object) null, EventArgs.Empty);
        }
        else
        {
          if (this.caretPositionChangedTimeoutTimer == null)
          {
            this.caretPositionChangedTimeoutTimer = new DispatcherTimer();
            this.caretPositionChangedTimeoutTimer.Interval = TimeSpan.FromMilliseconds(500.0);
            this.caretPositionChangedTimeoutTimer.Tick += new EventHandler(this.OnCaretPositionChangedTimeoutTimerTick);
          }
          if (this.caretPositionChangedTimeoutTimer.IsEnabled)
            this.caretPositionChangedTimeoutTimer.Stop();
          this.caretPositionChangedTimeoutTimer.Start();
        }
      }

      public void DelaySelectionIfActive()
      {
        if (this.caretPositionChangedTimeoutTimer == null || !this.caretPositionChangedTimeoutTimer.IsEnabled)
          return;
        this.caretPositionChangedTimeoutTimer.Stop();
        this.caretPositionChangedTimeoutTimer.Start();
      }

      private void ViewModel_LateSceneUpdatePhase(object sender, SceneUpdatePhaseEventArgs args)
      {
        this.isDirtyStateRelevant |= args.IsDirtyViewState(SceneViewModel.ViewStateBits.ElementSelection | SceneViewModel.ViewStateBits.KeyFrameSelection | SceneViewModel.ViewStateBits.AnimationSelection | SceneViewModel.ViewStateBits.StoryboardSelection | SceneViewModel.ViewStateBits.GridColumnSelection | SceneViewModel.ViewStateBits.GridRowSelection | SceneViewModel.ViewStateBits.ChildPropertySelection | SceneViewModel.ViewStateBits.BehaviorSelection | SceneViewModel.ViewStateBits.AnnotationSelection);
        this.EnsureXamlSynchronizedToScene(false);
      }

      private void OnCaretPositionChangedTimeoutTimerTick(object sender, EventArgs e)
      {
        if (this.sceneView != null && !this.sceneView.ShuttingDown && this.sceneView.ViewModel.XamlDocument.IsEditable)
        {
          PerformanceUtility.MeasurePerformanceUntilRender(PerformanceEvent.SelectionSynchronize);
          ITextEditor codeEditor = this.sceneView.CodeEditor;
          ITextRange nodeRange;
          DocumentNode nodeContainingRange = this.sceneView.ViewModel.XamlDocument.GetLowestNodeContainingRange((ITextRange) new Microsoft.Expression.DesignModel.Code.TextRange(codeEditor.CaretPosition, codeEditor.CaretPosition), true, false, out nodeRange);
          DocumentNode documentNode1 = (DocumentNode) null;
          if (this.sceneView.ViewModel.ActiveEditingContainer != null)
            documentNode1 = this.sceneView.ViewModel.ActiveEditingContainer.DocumentNode;
          DocumentNode node = (DocumentNode) null;
          if (nodeContainingRange != null)
          {
            List<DocumentNode> list = new List<DocumentNode>(nodeContainingRange.AncestorNodes);
            list.Reverse();
            list.Add(nodeContainingRange);
            bool isContainingTemplateSelectable = true;
            foreach (DocumentNode documentNode2 in list)
            {
              SceneNode targetSceneNode = this.sceneView.ViewModel.GetSceneNode(documentNode2);
              if (documentNode1 == null || documentNode1.IsAncestorOf(documentNode2))
              {
                if (targetSceneNode is StyleNode || targetSceneNode is FrameworkTemplateElement)
                {
                  isContainingTemplateSelectable = false;
                  this.sceneView.ViewModel.EditContextManager.SingleViewModelEditContextWalker.Walk(true, (SingleHistoryCallback) ((context, isGhosted) =>
                  {
                    if (context.EditingContainer != targetSceneNode)
                      return false;
                    isContainingTemplateSelectable = true;
                    return true;
                  }));
                }
                if (isContainingTemplateSelectable)
                {
                  SceneElement sceneElement = targetSceneNode as SceneElement;
                  if (sceneElement != null)
                  {
                    if (sceneElement.IsSelectable)
                      node = documentNode2;
                    else
                      break;
                  }
                  else if (targetSceneNode is StoryboardTimelineSceneNode || targetSceneNode is KeyFrameSceneNode)
                  {
                    if (this.sceneView.ViewModel.ActiveStoryboardTimeline != null && this.sceneView.ViewModel.ActiveStoryboardTimeline.DocumentNode.IsAncestorOf(documentNode2))
                      node = documentNode2;
                  }
                  else if (targetSceneNode is BehaviorBaseNode && !(targetSceneNode is ConditionBehaviorNode) || targetSceneNode is AnnotationSceneNode)
                    node = documentNode2;
                  else if (targetSceneNode is EffectNode)
                    node = documentNode2;
                }
              }
            }
          }
          if (node != null)
          {
            List<SceneNode> list = new List<SceneNode>(1);
            list.Add(this.sceneView.ViewModel.GetSceneNode(node));
            SceneNode[] selectedNodes = this.sceneView.DesignerContext.SelectionManager.SelectedNodes;
            if (selectedNodes == null || selectedNodes.Length != 1 || selectedNodes[0] != list[0])
            {
              this.isUpdatingSelectionFromCursor = true;
              using (SceneEditTransaction editTransaction = this.sceneView.ViewModel.CreateEditTransaction("Switch selection", true))
              {
                this.sceneView.ViewModel.ClearSelections();
                this.sceneView.ViewModel.SelectNodes((ICollection<SceneNode>) list);
                editTransaction.Commit();
              }
              this.isUpdatingSelectionFromCursor = false;
            }
          }
        }
        if (this.caretPositionChangedTimeoutTimer == null)
          return;
        this.caretPositionChangedTimeoutTimer.Stop();
      }

      private class SelectionSynchronizingToken : IDisposable
      {
        private SceneView.SelectionSynchronizer synchronizer;

        public SelectionSynchronizingToken(SceneView.SelectionSynchronizer synchronizer)
        {
          this.synchronizer = synchronizer;
          ++this.synchronizer.isUpdatingSelectionFromSceneCount;
        }

        public void Dispose()
        {
          --this.synchronizer.isUpdatingSelectionFromSceneCount;
          this.synchronizer = (SceneView.SelectionSynchronizer) null;
        }
      }
    }

    internal sealed class DocumentRootResolver : IDocumentRootResolver
    {
      private SceneView view;
      private ViewUpdateManager viewUpdateManager;

      public IDocumentRoot ApplicationRoot
      {
        get
        {
          SceneDocument applicationSceneDocument = this.view.Document.ApplicationSceneDocument;
          if (applicationSceneDocument == null)
            return (IDocumentRoot) null;
          this.viewUpdateManager.EnsureViewUpdatesForRelatedDocument(this.view, applicationSceneDocument);
          return applicationSceneDocument.DocumentRoot;
        }
      }

      public DocumentRootResolver(SceneView view)
      {
        this.view = view;
        this.viewUpdateManager = view.DesignerContext.ViewUpdateManager;
      }

      public IDocumentRoot GetDocumentRoot(string path)
      {
        SceneDocument sceneDocument = this.view.Document.GetSceneDocument(path);
        if (sceneDocument == null)
          return (IDocumentRoot) null;
        this.viewUpdateManager.EnsureViewUpdatesForRelatedDocument(this.view, sceneDocument);
        return sceneDocument.DocumentRoot;
      }
    }

    private class SceneViewUndoCommand : Command
    {
      private SceneDocument document;

      public override bool IsEnabled
      {
        get
        {
          return this.document.CanUndo;
        }
      }

      public SceneViewUndoCommand(SceneDocument document)
      {
        this.document = document;
      }

      public override void Execute()
      {
        this.document.Undo();
      }

      public override object GetProperty(string propertyName)
      {
        if (!(propertyName == "Text"))
          return base.GetProperty(propertyName);
        string str = string.Empty;
        if (this.document.CanUndo)
          str = this.document.UndoDescription;
        return (object) string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.UndoMenuText, new object[1]
        {
          (object) str
        });
      }
    }

    private class SceneViewRedoCommand : Command
    {
      private SceneDocument document;

      public override bool IsEnabled
      {
        get
        {
          return this.document.CanRedo;
        }
      }

      public SceneViewRedoCommand(SceneDocument document)
      {
        this.document = document;
      }

      public override void Execute()
      {
        this.document.Redo();
      }

      public override object GetProperty(string propertyName)
      {
        if (!(propertyName == "Text"))
          return base.GetProperty(propertyName);
        string str = string.Empty;
        if (this.document.CanRedo)
          str = this.document.RedoDescription;
        return (object) string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.RedoMenuText, new object[1]
        {
          (object) str
        });
      }
    }

    private class NautilusUndoUnit : UndoUnit
    {
      private bool isFirstRedo = true;
      private XamlDocument xamlDocument;
      private ITextUndoTransaction undoTransaction;

      public NautilusUndoUnit(XamlDocument xamlDocument, ITextUndoTransaction undoTransaction)
      {
        this.xamlDocument = xamlDocument;
        this.undoTransaction = undoTransaction;
      }

      public void SetUndoTransaction(ITextUndoTransaction transaction)
      {
        this.undoTransaction = transaction;
      }

      public override void Undo()
      {
        this.xamlDocument.IsUndoingTextChange = true;
        this.undoTransaction.Undo();
        this.xamlDocument.IsUndoingTextChange = false;
        base.Undo();
      }

      public override void Redo()
      {
        if (!this.isFirstRedo)
        {
          this.xamlDocument.IsUndoingTextChange = true;
          this.undoTransaction.Redo();
          this.xamlDocument.IsUndoingTextChange = false;
          base.Redo();
        }
        else
          this.isFirstRedo = false;
      }

      public void DisallowMerge()
      {
        this.undoTransaction.DisallowMerge();
      }
    }

    internal sealed class DocumentErrorTask : SceneView.ISceneErrorTask, IErrorTask
    {
      private SceneXamlDocument document;
      private DocumentNode source;
      private string description;
      private string fullDescription;
      private ErrorSeverity errorSeverity;

      public ErrorSeverity Severity
      {
        get
        {
          return this.errorSeverity;
        }
      }

      public string Description
      {
        get
        {
          return this.description;
        }
      }

      public string ExtendedDescription
      {
        get
        {
          return this.fullDescription;
        }
      }

      public string Project
      {
        get
        {
          if (this.document == null || this.document.ProjectContext == null)
            return string.Empty;
          return this.document.ProjectContext.ProjectName;
        }
      }

      public string File
      {
        get
        {
          if (this.document == null || this.document.DocumentContext == null)
            return string.Empty;
          return Path.GetFileName(this.document.DocumentContext.DocumentUrl);
        }
      }

      public string FullFileName
      {
        get
        {
          if (this.document == null || this.document.DocumentContext == null)
            return string.Empty;
          return Microsoft.Expression.Framework.Documents.PathHelper.ResolvePath(this.document.DocumentContext.DocumentUrl);
        }
      }

      public int? Line
      {
        get
        {
          if (this.document != null && this.document.TextBuffer != null)
          {
            ITextRange span = this.Span;
            if (!Microsoft.Expression.DesignModel.Code.TextRange.IsNull(span))
              return new int?(this.document.TextBuffer.GetLocation(span.Offset).Line);
          }
          return new int?();
        }
      }

      public int? Column
      {
        get
        {
          if (this.document != null && this.document.TextBuffer != null)
          {
            ITextRange span = this.Span;
            if (!Microsoft.Expression.DesignModel.Code.TextRange.IsNull(span))
              return new int?(this.document.TextBuffer.GetLocation(span.Offset).Column);
          }
          return new int?();
        }
      }

      public ITextRange Span
      {
        get
        {
          if (this.source.DocumentRoot != this.document || this.document.DocumentContext == null)
            return Microsoft.Expression.DesignModel.Code.TextRange.Null;
          ITextRange textRange = Microsoft.Expression.DesignModel.Code.TextRange.Null;
          for (DocumentNode node = this.source; Microsoft.Expression.DesignModel.Code.TextRange.IsNull(textRange) && node != null; node = (DocumentNode) node.Parent)
          {
            textRange = DocumentNodeHelper.GetNodeSpan(node);
            if (node == this.source.DocumentRoot.RootNode)
              textRange = GoToXamlCommand.GetElementNameSelectionSpan((IReadableSelectableTextBuffer) this.document.TextBuffer, textRange);
          }
          return textRange;
        }
      }

      public IDocumentRoot Document
      {
        get
        {
          return (IDocumentRoot) this.document;
        }
      }

      public System.Windows.Input.ICommand InvokeCommand
      {
        get
        {
          return (System.Windows.Input.ICommand) new DelegateCommand((DelegateCommand.SimpleEventHandler) (() =>
          {
            if (this.source.DocumentRoot != this.document || this.document.DocumentContext == null)
              return;
            List<DocumentNode> targetNodes = new List<DocumentNode>();
            if (this.source != this.source.DocumentRoot.RootNode)
              targetNodes.Add(this.source);
            GoToXamlCommand.GoToXaml(this.document, targetNodes);
          }));
        }
      }

      public DocumentErrorTask(DocumentNode source, string description, string fullDescription, ErrorSeverity errorSeverity)
      {
        this.document = (SceneXamlDocument) source.DocumentRoot;
        this.source = source;
        this.description = description;
        this.fullDescription = fullDescription;
        this.errorSeverity = errorSeverity;
      }
    }

    [Prototype]
    private class DumpUndoStackCommand : Command
    {
      private SceneDocument document;

      public DumpUndoStackCommand(SceneDocument document)
      {
        this.document = document;
      }

      public override void Execute()
      {
        this.document.DumpUndoService();
      }
    }

    [Prototype]
    private class ClearUndoStackCommand : Command
    {
      private SceneDocument document;

      public ClearUndoStackCommand(SceneDocument document)
      {
        this.document = document;
      }

      public override void Execute()
      {
        this.document.ClearUndoService();
      }
    }
  }
}
