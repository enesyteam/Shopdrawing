// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.SceneViewModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.InstanceBuilders;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Annotations;
using Microsoft.Expression.DesignSurface.Designers;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.Selection;
using Microsoft.Expression.DesignSurface.Tools.Path;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.UserInterface.DataPane;
using Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector;
using Microsoft.Expression.DesignSurface.UserInterface.SkinEditing;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel.Extensibility;
using Microsoft.Expression.DesignSurface.ViewModel.SceneNodeSearch;
using Microsoft.Expression.Framework.Commands.Undo;
using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.Expression.Framework.Scheduler;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  [DebuggerDisplay("{System.IO.Path.GetFileName(this.defaultView.InstanceBuilderContext.DocumentContext.DocumentUrl)}")]
  public sealed class SceneViewModel : IDisposable
  {
    private static IPropertyId SelectedIndexProperty = (IPropertyId) ProjectNeutralTypes.TabControl.GetMember(MemberType.LocalProperty, "SelectedIndex", MemberAccessTypes.Public);
    private DocumentNodeMarkerSortedList currentExpansionSet = new DocumentNodeMarkerSortedList();
    private ReadOnlyCollection<PathPart> lastPathPartList = new ReadOnlyCollection<PathPart>((IList<PathPart>) new List<PathPart>());
    private DesignerContext designerContext;
    private SceneDocument sceneDocument;
    private SceneNodeFactory sceneNodeFactory;
    private int forcingBaseValueCount;
    private int forcingDefaultSetValueCount;
    private int disablingDrawIntoStateCount;
    private int forcingUseShadowPropertiesCount;
    private int disableUpdateChildrenOnAddAndRemoveCount;
    private int enforceGridDesignModeCount;
    private double storedSeekTime;
    private SceneView defaultView;
    private EditContextManager editContextManager;
    private SceneNodeSubscription<object, object> triggerSubscription;
    private SceneElementSelectionSet elementSelectionSet;
    private TextSelectionSet textSelectionSet;
    private PathPartSelectionSet pathPartSelectionSet;
    private KeyFrameSelectionSet keyFrameSelectionSet;
    private AnimationSelectionSet animationSelectionSet;
    private StoryboardSelectionSet storyboardSelectionSet;
    private BehaviorSelectionSet behaviorSelectionSet;
    private AnnotationSelectionSet annotationSelectionSet;
    private DependencyObjectSelectionSet dependencyObjectSelectionSet;
    private TransitionSelectionSet transitionSelectionSet;
    private ISceneInsertionPoint activeInsertionPoint;
    private GridColumnSelectionSet gridColumnSelectionSet;
    private GridRowSelectionSet gridRowSelectionSet;
    private PropertySelectionSet propertySelectionSet;
    private ChildPropertySelectionSet childPropertySelectionSet;
    private SceneElementSubpartSelectionSet<SetterSceneNode> setterSelectionSet;
    private AnimationEditor animationEditor;
    private AnnotationEditor annotationEditor;
    private TimelineItemManager timelineItemManager;
    private BindingEditor bindingEditor;
    private DataPanelModel dataPanelModel;
    private PartsModel partsModel;
    private DocumentNodeChangeList damage;
    private FontEmbedder fontEmbedder;
    private ExtensibilityManager extensibilityManager;
    private DocumentNode alternateSiteNode;
    private SceneViewModel.ViewStateBits dirtyState;
    private uint changeStamp;
    private SceneViewModel.PipelineCalcBits dirtyPipelineState;
    private uint pipelineBitsChangeStamp;
    private static SceneViewUpdateScheduleTask earlySceneViewUpdateScheduleTask;
    private static SceneViewUpdateScheduleTask lateSceneViewUpdateScheduleTask;
    private AnimationProxyManager animationProxyManager;
    private int viewObjectCacheScopeCount;
    private List<SceneNode> viewObjectCachedNodes;

    public SceneView DefaultView
    {
      get
      {
        return this.defaultView;
      }
      internal set
      {
        if (this.defaultView != null)
        {
          Microsoft.Expression.DesignModel.Markup.XamlDocument xamlDocument = (Microsoft.Expression.DesignModel.Markup.XamlDocument) this.sceneDocument.XamlDocument;
          xamlDocument.RootNodeChangingOutsideUndo -= new EventHandler(this.DocumentRoot_RootNodeChangingOutsideUndo);
          xamlDocument.RootNodeChangedOutsideUndo -= new EventHandler(this.DocumentRoot_RootNodeChangedOutsideUndo);
          xamlDocument.RootNodeChanging -= new EventHandler(this.DocumentRoot_RootNodeChanging);
          xamlDocument.RootNodeChanged -= new EventHandler(this.DocumentRoot_RootNodeChanged);
        }
        this.defaultView = value;
        if (this.defaultView == null)
          return;
        Microsoft.Expression.DesignModel.Markup.XamlDocument xamlDocument1 = (Microsoft.Expression.DesignModel.Markup.XamlDocument) this.sceneDocument.XamlDocument;
        xamlDocument1.RootNodeChanged += new EventHandler(this.DocumentRoot_RootNodeChanged);
        xamlDocument1.RootNodeChanging += new EventHandler(this.DocumentRoot_RootNodeChanging);
        xamlDocument1.RootNodeChangedOutsideUndo += new EventHandler(this.DocumentRoot_RootNodeChangedOutsideUndo);
        xamlDocument1.RootNodeChangingOutsideUndo += new EventHandler(this.DocumentRoot_RootNodeChangingOutsideUndo);
      }
    }

    public SceneNode RootNode
    {
      get
      {
        IDocumentRoot documentRoot = this.DocumentRoot;
        if (documentRoot != null && documentRoot.IsEditable)
        {
          DocumentNode rootNode = documentRoot.RootNode;
          if (rootNode != null)
            return this.GetSceneNode(rootNode);
        }
        return (SceneNode) null;
      }
    }

    public SceneNode ViewRoot
    {
      get
      {
        SceneNode viewRoot = (SceneNode) null;
        this.EditContextManager.SingleViewModelEditContextWalker.Walk(true, (SingleHistoryCallback) ((context, isGhosted) =>
        {
          viewRoot = context.ViewScope;
          return viewRoot != null;
        }));
        if (viewRoot == null)
          return this.RootNode;
        return viewRoot;
      }
    }

    public DocumentNode ViewRootNode
    {
      get
      {
        SceneNode viewRoot = this.ViewRoot;
        if (viewRoot == null)
          return (DocumentNode) null;
        return viewRoot.DocumentNode;
      }
    }

    public SceneDocument Document
    {
      get
      {
        return this.sceneDocument;
      }
    }

    public IProjectContext ProjectContext
    {
      get
      {
        return this.sceneDocument.ProjectContext;
      }
    }

    public DocumentNodeChangeList Damage
    {
      get
      {
        return this.damage;
      }
    }

    public Size PreferredSize
    {
      get
      {
        return this.EditContextManager.PreferredSize;
      }
    }

    internal bool IsDisposed
    {
      get
      {
        return this.Document == null;
      }
    }

    public SceneXamlDocument XamlDocument
    {
      get
      {
        return this.sceneDocument.XamlDocument;
      }
    }

    public AnimationEditor AnimationEditor
    {
      get
      {
        return this.animationEditor;
      }
    }

    public AnimationProxyManager AnimationProxyManager
    {
      get
      {
        return this.animationProxyManager;
      }
    }

    public AnnotationEditor AnnotationEditor
    {
      get
      {
        return this.annotationEditor;
      }
    }

    public TimelineItemManager TimelineItemManager
    {
      get
      {
        if (this.timelineItemManager == null && this.designerContext.WindowService != null)
          this.timelineItemManager = new TimelineItemManager(this);
        return this.timelineItemManager;
      }
    }

    public BindingEditor BindingEditor
    {
      get
      {
        return this.bindingEditor;
      }
    }

    public DataPanelModel DataPanelModel
    {
      get
      {
        return this.dataPanelModel;
      }
    }

    public PartsModel PartsModel
    {
      get
      {
        return this.partsModel;
      }
    }

    public ExtensibilityManager ExtensibilityManager
    {
      get
      {
        return this.extensibilityManager;
      }
    }

    public FontEmbedder FontEmbedder
    {
      get
      {
        return this.fontEmbedder;
      }
    }

    public IDocumentRootResolver DocumentRootResolver
    {
      get
      {
        return (IDocumentRootResolver) this.sceneDocument.ProjectContext;
      }
    }

    public IDocumentRoot DocumentRoot
    {
      get
      {
        if (this.sceneDocument == null)
          return (IDocumentRoot) null;
        return this.sceneDocument.DocumentRoot;
      }
    }

    public bool IsEditable
    {
      get
      {
        if (this.defaultView == null)
          return false;
        return this.defaultView.IsEditable;
      }
    }

    public SceneElementSelectionSet ElementSelectionSet
    {
      get
      {
        return this.elementSelectionSet;
      }
    }

    public DependencyObjectSelectionSet DependencyObjectSelectionSet
    {
      get
      {
        return this.dependencyObjectSelectionSet;
      }
    }

    public TransitionSelectionSet TransitionSelectionSet
    {
      get
      {
        return this.transitionSelectionSet;
      }
    }

    public TextSelectionSet TextSelectionSet
    {
      get
      {
        return this.textSelectionSet;
      }
    }

    public PathPartSelectionSet PathPartSelectionSet
    {
      get
      {
        return this.pathPartSelectionSet;
      }
    }

    public KeyFrameSelectionSet KeyFrameSelectionSet
    {
      get
      {
        return this.keyFrameSelectionSet;
      }
    }

    public AnimationSelectionSet AnimationSelectionSet
    {
      get
      {
        return this.animationSelectionSet;
      }
    }

    public StoryboardSelectionSet StoryboardSelectionSet
    {
      get
      {
        return this.storyboardSelectionSet;
      }
    }

    public BehaviorSelectionSet BehaviorSelectionSet
    {
      get
      {
        return this.behaviorSelectionSet;
      }
    }

    public AnnotationSelectionSet AnnotationSelectionSet
    {
      get
      {
        return this.annotationSelectionSet;
      }
    }

    public GridColumnSelectionSet GridColumnSelectionSet
    {
      get
      {
        return this.gridColumnSelectionSet;
      }
    }

    public GridRowSelectionSet GridRowSelectionSet
    {
      get
      {
        return this.gridRowSelectionSet;
      }
    }

    public PropertySelectionSet PropertySelectionSet
    {
      get
      {
        return this.propertySelectionSet;
      }
    }

    public ChildPropertySelectionSet ChildPropertySelectionSet
    {
      get
      {
        return this.childPropertySelectionSet;
      }
    }

    public SceneElementSubpartSelectionSet<SetterSceneNode> SetterSelectionSet
    {
      get
      {
        return this.setterSelectionSet;
      }
    }

    public SceneViewModel.ViewStateBits DirtyState
    {
      get
      {
        return this.dirtyState;
      }
    }

    public uint ChangeStamp
    {
      get
      {
        return this.changeStamp;
      }
    }

    public SceneViewModel.PipelineCalcBits DirtyPipelineCalcState
    {
      get
      {
        return this.dirtyPipelineState;
      }
    }

    public uint PipelineCalcChangeStamp
    {
      get
      {
        return this.pipelineBitsChangeStamp;
      }
    }

    public double StoredSeekTime
    {
      get
      {
        return this.storedSeekTime;
      }
      set
      {
        this.storedSeekTime = value;
      }
    }

    internal EditContextManager EditContextManager
    {
      get
      {
        return this.editContextManager;
      }
    }

    internal EditContext NextActiveNonHiddenParentContext
    {
      get
      {
        return this.EditContextManager.NextActiveNonHiddenParentContext;
      }
    }

    internal bool CanPopActiveEditingContainer
    {
      get
      {
        return this.EditContextManager.CanPopActiveEditingContainer;
      }
    }

    internal EditContext ActiveEditContext
    {
      get
      {
        return this.EditContextManager.ActiveEditContext;
      }
    }

    public ISceneInsertionPoint LockedInsertionPoint
    {
      get
      {
        EditContext activeEditContext = this.ActiveEditContext;
        if (activeEditContext != null)
          return activeEditContext.LockedInsertionPoint;
        return (ISceneInsertionPoint) null;
      }
      set
      {
        if (object.Equals((object) this.ActiveEditContext.LockedInsertionPoint, (object) value))
          return;
        using (SceneEditTransaction editTransaction = this.CreateEditTransaction(StringTable.UndoUnitSetSceneInsertionPoint, true))
        {
          if (value != null)
          {
            IStoryboardContainer storyboardContainer = value.SceneElement.StoryboardContainer;
            if (storyboardContainer != null && storyboardContainer != this.ActiveStoryboardContainer)
              this.ActiveEditingContainerPath = ((SceneNode) storyboardContainer).DocumentNodePath;
          }
          this.EditContextManager.SetLockedInsertionPoint(value);
          this.GridColumnSelectionSet.Clear();
          this.GridRowSelectionSet.Clear();
          editTransaction.Commit();
        }
      }
    }

    public ISceneInsertionPoint ActiveSceneInsertionPoint
    {
      get
      {
        return this.GetActiveSceneInsertionPoint(new InsertionPointContext());
      }
    }

    public DocumentNodePath ActiveEditingContainerPath
    {
      get
      {
        return this.EditContextManager.ActiveEditingContainerPath;
      }
      set
      {
        this.EditContextManager.ActiveEditingContainerPath = value;
      }
    }

    public SceneNode ActiveEditingContainer
    {
      get
      {
        if (this.EditContextManager == null)
          return (SceneNode) null;
        return this.EditContextManager.ActiveEditingContainer;
      }
    }

    public IStoryboardContainer ActiveStoryboardContainer
    {
      get
      {
        return this.EditContextManager.ActiveStoryboardContainer;
      }
    }

    public TriggerBaseNode ActiveVisualTrigger
    {
      get
      {
        return this.EditContextManager.ActiveVisualTrigger;
      }
    }

    public StoryboardTimelineSceneNode ActiveStoryboardTimeline
    {
      get
      {
        return this.EditContextManager.ActiveStoryboardTimeline;
      }
    }

    public VisualStateSceneNode StateEditTarget
    {
      get
      {
        return this.EditContextManager.StateEditTarget;
      }
    }

    public VisualStateTransitionSceneNode TransitionEditTarget
    {
      get
      {
        return this.EditContextManager.TransitionEditTarget;
      }
    }

    public StoryboardTimelineSceneNode StateStoryboardEditTarget
    {
      get
      {
        return this.EditContextManager.StateStoryboardEditTarget;
      }
    }

    public StoryboardTimelineSceneNode TransitionStoryboardEditTarget
    {
      get
      {
        return this.EditContextManager.TransitionStoryboardEditTarget;
      }
    }

    public DocumentCompositeNode ActivePropertyTriggerNode
    {
      get
      {
        if (!(this.ActiveVisualTrigger is TriggerNode))
          return (DocumentCompositeNode) null;
        return this.ActiveVisualTrigger.DocumentNode as DocumentCompositeNode;
      }
    }

    public IList<VisualStateSceneNode> PinnedStates
    {
      get
      {
        return this.EditContextManager.PinnedStates;
      }
    }

    public bool IsForcingBaseValue
    {
      get
      {
        return this.forcingBaseValueCount > 0;
      }
    }

    public bool IsForcingDefaultSetValue
    {
      get
      {
        return this.forcingDefaultSetValueCount > 0;
      }
    }

    public bool IsDisablingDrawIntoState
    {
      get
      {
        return this.disablingDrawIntoStateCount > 0;
      }
    }

    public bool IsForcingUseShadowProperties
    {
      get
      {
        return this.forcingUseShadowPropertiesCount > 0;
      }
    }

    public bool CanDeleteSelection
    {
      get
      {
        return !this.AnimationSelectionSet.IsEmpty || (!this.StoryboardSelectionSet.IsEmpty || (!this.behaviorSelectionSet.IsEmpty || (!this.annotationSelectionSet.IsEmpty || (!this.ChildPropertySelectionSet.IsEmpty || (!this.KeyFrameSelectionSet.IsEmpty || (this.GridColumnSelectionSet.GridLineMode && !this.GridColumnSelectionSet.IsEmpty || (this.GridRowSelectionSet.GridLineMode && !this.GridRowSelectionSet.IsEmpty || (!this.DefaultView.EventRouter.IsEditingText ? this.CanDeleteElementSelection : this.TextSelectionSet.IsActive && this.TextSelectionSet.CanDelete))))))));
      }
    }

    public bool CanDeleteElementSelection
    {
      get
      {
        bool flag = false;
        if (!this.ElementSelectionSet.IsEmpty)
        {
          SceneElement sceneElement1 = this.RootNode as SceneElement;
          if (sceneElement1 == null || !this.ElementSelectionSet.Selection.Contains(sceneElement1))
          {
            flag = true;
            foreach (SceneElement sceneElement2 in this.ElementSelectionSet.Selection)
            {
              StyleNode styleNode = sceneElement2 as StyleNode;
              FrameworkTemplateElement frameworkTemplateElement = sceneElement2 as FrameworkTemplateElement;
              if (sceneElement2 is CameraElement && sceneElement2.Parent is Viewport3DElement || (styleNode != null || frameworkTemplateElement != null) && (sceneElement2.DocumentNodePath != null && sceneElement2.DocumentNodePath.ContainerOwner == null))
              {
                flag = false;
                break;
              }
            }
          }
        }
        return flag;
      }
    }

    internal DesignerContext DesignerContext
    {
      get
      {
        return this.designerContext;
      }
    }

    internal bool IsActiveSceneViewModel
    {
      get
      {
        return this.designerContext.ActiveSceneViewModel == this;
      }
    }

    public bool UsingEffectDesigner
    {
      get
      {
        return this.AnimationEditor.IsKeyFraming && this.AnimationEditor.IsRecording && !this.AnimationEditor.CanAnimateLayout;
      }
    }

    public IDisposable EnforceGridDesignMode
    {
      get
      {
        return (IDisposable) new SceneViewModel.EnforceGridDesignModeToken(this);
      }
    }

    public bool OverrideGridDesignMode
    {
      get
      {
        return this.enforceGridDesignModeCount > 0;
      }
    }

    public bool IsInGridDesignMode
    {
      get
      {
        if (this.designerContext.ArtboardOptionsModel != null)
          return this.designerContext.ArtboardOptionsModel.IsInGridDesignMode;
        return true;
      }
      set
      {
        if (this.designerContext.ArtboardOptionsModel == null)
          return;
        this.designerContext.ArtboardOptionsModel.IsInGridDesignMode = value;
      }
    }

    public bool InViewObjectCacheScope
    {
      get
      {
        return this.viewObjectCacheScopeCount != 0;
      }
    }

    public event SceneUpdatePhaseEventHandler EarlySceneUpdatePhase;

    public event SceneUpdatePhaseEventHandler LateSceneUpdatePhase;

    public event EventHandler Closing;

    internal SceneViewModel(DesignerContext designerContext, SceneDocument sceneDocument)
    {
      this.designerContext = designerContext;
      this.sceneDocument = sceneDocument;
      this.damage = new DocumentNodeChangeList();
      this.XamlDocument.RegisterChangeList(this.damage);
      this.XamlDocument.ParseCompleted += new EventHandler(this.OnXamlDocumentParseCompleted);
      this.sceneDocument.EditTransactionCompleted += new EventHandler(this.SceneDocument_EditTransactionCompleted);
      this.sceneDocument.EditTransactionUpdated += new EventHandler(this.SceneDocument_EditTransactionUpdated);
      this.sceneDocument.EditTransactionCanceled += new EventHandler(this.SceneDocument_EditTransactionCanceled);
      this.sceneDocument.EditTransactionUndoRedo += new EventHandler(this.SceneDocument_EditTransactionUndoRedo);
      this.sceneNodeFactory = new SceneNodeFactory();
      this.editContextManager = new EditContextManager(this);
      this.elementSelectionSet = new SceneElementSelectionSet(this);
      this.textSelectionSet = new TextSelectionSet(this);
      this.pathPartSelectionSet = new PathPartSelectionSet(this.elementSelectionSet);
      this.keyFrameSelectionSet = new KeyFrameSelectionSet(this);
      this.animationSelectionSet = new AnimationSelectionSet(this);
      this.storyboardSelectionSet = new StoryboardSelectionSet(this);
      this.behaviorSelectionSet = new BehaviorSelectionSet(this);
      this.annotationSelectionSet = new AnnotationSelectionSet(this);
      this.gridColumnSelectionSet = new GridColumnSelectionSet(this);
      this.gridRowSelectionSet = new GridRowSelectionSet(this);
      this.dependencyObjectSelectionSet = new DependencyObjectSelectionSet(this);
      this.transitionSelectionSet = new TransitionSelectionSet(this);
      this.elementSelectionSet.Changed += new EventHandler(this.ElementSelectionSet_SelectionChanged);
      this.dependencyObjectSelectionSet.Changed += new EventHandler(this.DependencyObjectSelectionSet_SelectionChanged);
      this.elementSelectionSet.SelectionChangedOutsideUndo += new EventHandler(this.ElementSelectionSet_SelectionChangedOutsideUndo);
      this.dependencyObjectSelectionSet.SelectionChangedOutsideUndo += new EventHandler(this.ElementSelectionSet_SelectionChangedOutsideUndo);
      this.pathPartSelectionSet.Changed += new EventHandler(this.PathPartSelectionSet_SelectionChanged);
      this.propertySelectionSet = new PropertySelectionSet(this);
      this.childPropertySelectionSet = new ChildPropertySelectionSet(this);
      this.setterSelectionSet = new SceneElementSubpartSelectionSet<SetterSceneNode>(this.elementSelectionSet, this, (ISelectionSetNamingHelper) new SetterNamingHelper());
      this.animationEditor = new AnimationEditor(this.KeyFrameSelectionSet, this.AnimationSelectionSet, this);
      this.annotationEditor = new AnnotationEditor(this);
      this.bindingEditor = new BindingEditor(this);
      if (!this.sceneDocument.IsPreviewDocument)
        this.dataPanelModel = new DataPanelModel(this, false);
      this.partsModel = new PartsModel(this.designerContext, this);
      this.fontEmbedder = new FontEmbedder(this);
      this.extensibilityManager = new ExtensibilityManager(this);
      this.textSelectionSet.Changed += new EventHandler(this.GeneralSelectionSet_SelectionChanged);
      this.keyFrameSelectionSet.Changed += new EventHandler(this.GeneralSelectionSet_SelectionChanged);
      this.animationSelectionSet.Changed += new EventHandler(this.GeneralSelectionSet_SelectionChanged);
      this.storyboardSelectionSet.Changed += new EventHandler(this.GeneralSelectionSet_SelectionChanged);
      this.behaviorSelectionSet.Changed += new EventHandler(this.GeneralSelectionSet_SelectionChanged);
      this.annotationSelectionSet.Changed += new EventHandler(this.GeneralSelectionSet_SelectionChanged);
      this.gridColumnSelectionSet.Changed += new EventHandler(this.GeneralSelectionSet_SelectionChanged);
      this.gridRowSelectionSet.Changed += new EventHandler(this.GeneralSelectionSet_SelectionChanged);
      this.propertySelectionSet.Changed += new EventHandler(this.GeneralSelectionSet_SelectionChanged);
      this.childPropertySelectionSet.Changed += new EventHandler(this.GeneralSelectionSet_SelectionChanged);
      this.dependencyObjectSelectionSet.Changed += new EventHandler(this.GeneralSelectionSet_SelectionChanged);
      this.transitionSelectionSet.Changed += new EventHandler(this.GeneralSelectionSet_SelectionChanged);
      this.setterSelectionSet.Changed += new EventHandler(this.GeneralSelectionSet_SelectionChanged);
      this.animationEditor.RecordModeChanged += new EventHandler(this.AnimationEditor_RecordModeChanged);
      this.sceneDocument.EditTransactionCompleting += new EventHandler(this.SceneDocument_EditTransactionCompleting);
      this.triggerSubscription = new SceneNodeSubscription<object, object>();
      this.triggerSubscription.Path = new SearchPath(new SearchStep[1]
      {
        new SearchStep(SearchAxis.DocumentDescendant, (ISearchPredicate) new DelegatePredicate((Predicate<SceneNode>) (sceneNode =>
        {
          if (!typeof (TriggerBase).IsAssignableFrom(sceneNode.TargetType) && !typeof (ConditionCollection).IsAssignableFrom(sceneNode.TargetType))
            return typeof (Condition).IsAssignableFrom(sceneNode.TargetType);
          return true;
        }), SearchScope.NodeTreeSelf))
      });
      this.triggerSubscription.PathNodeInserted += new SceneNodeSubscription<object, object>.PathNodeInsertedListener(this.TriggerSubscription_Inserted);
      this.triggerSubscription.PathNodeRemoved += new SceneNodeSubscription<object, object>.PathNodeRemovedListener(this.TriggerSubscription_Removed);
      this.triggerSubscription.PathNodeContentChanged += new SceneNodeSubscription<object, object>.PathNodeContentChangedListener(this.TriggerSubscription_ContentChanged);
      if (this.ProjectContext.PlatformMetadata.IsCapabilitySet(PlatformCapability.IsWpf))
        return;
      this.animationProxyManager = new AnimationProxyManager();
      this.animationProxyManager.Attach(this);
    }

    public static void RegisterPipelineTasks(ISchedulingService scheduleManager)
    {
      SceneViewModel.earlySceneViewUpdateScheduleTask = new SceneViewUpdateScheduleTask(scheduleManager, true);
      SceneViewModel.lateSceneViewUpdateScheduleTask = new SceneViewUpdateScheduleTask(scheduleManager, false);
    }

    public static void UnregisterPipelineTasks(ISchedulingService scheduleManager)
    {
      SceneViewModel.earlySceneViewUpdateScheduleTask.Unregister(scheduleManager);
      SceneViewModel.lateSceneViewUpdateScheduleTask.Unregister(scheduleManager);
    }

    internal void Reset()
    {
      while (this.EditContextManager.CanPopActiveEditingContainer)
        this.EditContextManager.PopActiveEditingContainer();
      this.elementSelectionSet.Clear();
      this.textSelectionSet.Clear();
      this.pathPartSelectionSet.Clear();
      this.keyFrameSelectionSet.Clear();
      this.animationSelectionSet.Clear();
      this.storyboardSelectionSet.Clear();
      this.behaviorSelectionSet.Clear();
      this.annotationSelectionSet.Clear();
      this.dependencyObjectSelectionSet.Clear();
      this.transitionSelectionSet.Clear();
      this.gridColumnSelectionSet.Clear();
      this.gridRowSelectionSet.Clear();
      this.propertySelectionSet.Clear();
      this.childPropertySelectionSet.Clear();
      this.setterSelectionSet.Clear();
      this.activeInsertionPoint = (ISceneInsertionPoint) null;
      this.lastPathPartList = new ReadOnlyCollection<PathPart>((IList<PathPart>) new List<PathPart>());
      this.editContextManager = new EditContextManager(this);
    }

    public void Dispose()
    {
      this.XamlDocument.UnregisterChangeList(this.damage);
      this.XamlDocument.ParseCompleted -= new EventHandler(this.OnXamlDocumentParseCompleted);
      this.sceneDocument.EditTransactionCompleting -= new EventHandler(this.SceneDocument_EditTransactionCompleting);
      this.sceneDocument.EditTransactionCompleted -= new EventHandler(this.SceneDocument_EditTransactionCompleted);
      this.sceneDocument.EditTransactionUpdated -= new EventHandler(this.SceneDocument_EditTransactionUpdated);
      this.sceneDocument.EditTransactionCanceled -= new EventHandler(this.SceneDocument_EditTransactionCanceled);
      this.sceneDocument.EditTransactionUndoRedo -= new EventHandler(this.SceneDocument_EditTransactionUndoRedo);
      this.triggerSubscription.PathNodeInserted -= new SceneNodeSubscription<object, object>.PathNodeInsertedListener(this.TriggerSubscription_Inserted);
      this.triggerSubscription.PathNodeRemoved -= new SceneNodeSubscription<object, object>.PathNodeRemovedListener(this.TriggerSubscription_Removed);
      this.triggerSubscription.PathNodeContentChanged -= new SceneNodeSubscription<object, object>.PathNodeContentChangedListener(this.TriggerSubscription_ContentChanged);
      if (this.timelineItemManager != null)
      {
        this.timelineItemManager.Dispose();
        this.timelineItemManager = (TimelineItemManager) null;
      }
      if (this.animationEditor != null)
      {
        this.animationEditor.Dispose();
        this.animationEditor = (AnimationEditor) null;
      }
      if (this.annotationEditor != null)
      {
        this.annotationEditor.Dispose();
        this.annotationEditor = (AnnotationEditor) null;
      }
      if (this.dataPanelModel != null)
      {
        this.dataPanelModel.Unload();
        this.dataPanelModel = (DataPanelModel) null;
      }
      if (this.partsModel != null)
      {
        this.partsModel.Unload();
        this.partsModel = (PartsModel) null;
      }
      this.fontEmbedder = (FontEmbedder) null;
      this.bindingEditor = (BindingEditor) null;
      if (this.extensibilityManager != null)
      {
        this.extensibilityManager.Dispose();
        this.extensibilityManager = (ExtensibilityManager) null;
      }
      this.DefaultView = (SceneView) null;
      this.designerContext = (DesignerContext) null;
      this.sceneDocument = (SceneDocument) null;
      this.defaultView = (SceneView) null;
      this.editContextManager = (EditContextManager) null;
      this.elementSelectionSet = (SceneElementSelectionSet) null;
      this.textSelectionSet = (TextSelectionSet) null;
      this.dependencyObjectSelectionSet = (DependencyObjectSelectionSet) null;
      this.transitionSelectionSet = (TransitionSelectionSet) null;
      this.triggerSubscription = (SceneNodeSubscription<object, object>) null;
      if (this.damage != null)
      {
        this.damage.Clear();
        this.damage = (DocumentNodeChangeList) null;
      }
      SceneViewModel.earlySceneViewUpdateScheduleTask.Remove(this);
      SceneViewModel.lateSceneViewUpdateScheduleTask.Remove(this);
      if (this.animationProxyManager == null)
        return;
      this.animationProxyManager.Detach(this);
      this.animationProxyManager = (AnimationProxyManager) null;
    }

    public void Close()
    {
      this.Dispose();
    }

    internal void NotifyClosing()
    {
      if (this.Closing == null)
        return;
      this.Closing((object) this, EventArgs.Empty);
    }

    public void SetViewRoot(SceneView callingView, SceneElement ancestorElement, IPropertyId ancestorPropertyKey, DocumentNode node, Size preferredSize)
    {
      this.EditContextManager.SetViewRoot(callingView, ancestorElement, ancestorPropertyKey, node, preferredSize);
    }

    public void SelectNodes(ICollection<SceneNode> nodes)
    {
      List<SceneElement> list = (List<SceneElement>) null;
      foreach (SceneNode selectionToExtend in (IEnumerable<SceneNode>) nodes)
      {
        if (selectionToExtend is SceneElement)
        {
          if (list == null)
            list = new List<SceneElement>(nodes.Count);
          list.Add((SceneElement) selectionToExtend);
        }
        else if (selectionToExtend is KeyFrameSceneNode)
          this.KeyFrameSelectionSet.ExtendSelection((KeyFrameSceneNode) selectionToExtend);
        else if (selectionToExtend is AnimationSceneNode)
          this.AnimationSelectionSet.ExtendSelection((AnimationSceneNode) selectionToExtend);
        else if (selectionToExtend is StoryboardTimelineSceneNode)
          this.StoryboardSelectionSet.ExtendSelection((StoryboardTimelineSceneNode) selectionToExtend);
        else if (selectionToExtend is BehaviorBaseNode)
          this.BehaviorSelectionSet.SetSelection((BehaviorBaseNode) selectionToExtend);
        else if (selectionToExtend is AnnotationSceneNode)
          this.AnnotationSelectionSet.SetSelection((AnnotationSceneNode) selectionToExtend);
        else if (selectionToExtend is ColumnDefinitionNode)
          this.GridColumnSelectionSet.ExtendSelection((ColumnDefinitionNode) selectionToExtend);
        else if (selectionToExtend is RowDefinitionNode)
          this.GridRowSelectionSet.ExtendSelection((RowDefinitionNode) selectionToExtend);
        else if (selectionToExtend is SetterSceneNode)
          this.SetterSelectionSet.ExtendSelection((SetterSceneNode) selectionToExtend);
        else if (selectionToExtend is EffectNode)
          this.ChildPropertySelectionSet.ExtendSelection(selectionToExtend);
        else if (selectionToExtend is VisualStateTransitionSceneNode)
          this.TransitionSelectionSet.SetSelection((VisualStateTransitionSceneNode) selectionToExtend);
      }
      if (list == null)
        return;
      this.ElementSelectionSet.ExtendSelection((ICollection<SceneElement>) list);
    }

    public void ClearSelections()
    {
      this.ElementSelectionSet.Clear();
      this.TextSelectionSet.Clear();
      this.PathPartSelectionSet.Clear();
      this.KeyFrameSelectionSet.Clear();
      this.AnimationSelectionSet.Clear();
      this.StoryboardSelectionSet.Clear();
      this.BehaviorSelectionSet.Clear();
      this.AnnotationSelectionSet.Clear();
      this.GridColumnSelectionSet.Clear();
      this.GridRowSelectionSet.Clear();
      this.PropertySelectionSet.Clear();
      this.ChildPropertySelectionSet.Clear();
      this.SetterSelectionSet.Clear();
      this.DependencyObjectSelectionSet.Clear();
      this.TransitionSelectionSet.Clear();
    }

    internal void EnforceExclusiveSelection(ISelectionSet newWinner)
    {
      if (this.ElementSelectionSet != newWinner)
        this.ElementSelectionSet.Clear();
      if (this.StoryboardSelectionSet != newWinner)
        this.StoryboardSelectionSet.Clear();
      if (this.BehaviorSelectionSet != newWinner)
        this.behaviorSelectionSet.Clear();
      if (this.AnnotationSelectionSet != newWinner)
        this.annotationSelectionSet.Clear();
      if (this.AnimationSelectionSet != newWinner)
        this.AnimationSelectionSet.Clear();
      if (this.KeyFrameSelectionSet != newWinner)
        this.KeyFrameSelectionSet.Clear();
      if (this.GridColumnSelectionSet != newWinner)
        this.GridColumnSelectionSet.Clear();
      if (this.GridRowSelectionSet != newWinner)
        this.GridRowSelectionSet.Clear();
      if (this.ChildPropertySelectionSet != newWinner)
        this.ChildPropertySelectionSet.Clear();
      if (this.DependencyObjectSelectionSet == newWinner)
        return;
      this.DependencyObjectSelectionSet.Clear();
    }

    public void SetDirtyPipelineCalcBits(SceneViewModel.PipelineCalcBits bit)
    {
      ++this.pipelineBitsChangeStamp;
      this.dirtyPipelineState |= bit;
    }

    internal void OnIsEditableChanged()
    {
      this.SetDirtyViewStateBits(SceneViewModel.ViewStateBits.IsEditable);
    }

    public void SetLockedInsertionPoint(SceneElement sceneElement)
    {
      ISceneInsertionPoint sceneInsertionPoint = (ISceneInsertionPoint) null;
      if (sceneElement != null)
        sceneInsertionPoint = sceneElement.DefaultInsertionPoint;
      this.LockedInsertionPoint = sceneInsertionPoint;
    }

    public ISceneInsertionPoint GetActiveSceneInsertionPoint(InsertionPointContext insertionPointContext)
    {
      ISceneInsertionPoint sceneInsertionPoint = this.activeInsertionPoint;
      if (sceneInsertionPoint == null || !insertionPointContext.CheckContainerCapacity)
      {
        sceneInsertionPoint = this.GetActiveSceneInsertionPointFromPosition(insertionPointContext);
        if ((this.DirtyState & SceneViewModel.ViewStateBits.ActiveSceneInsertionPoint) == SceneViewModel.ViewStateBits.None)
          this.activeInsertionPoint = sceneInsertionPoint;
      }
      return sceneInsertionPoint;
    }

    public ISceneInsertionPoint GetActiveSceneInsertionPointFromPosition(InsertionPointContext insertPointContext)
    {
      if (insertPointContext.TypeToInsertIsSingleProperty(this))
        return this.GetActiveSinglePropertySceneInsertionPointFromPosition(insertPointContext);
      return this.GetActiveContentSceneInsertionPointFromPosition(insertPointContext);
    }

    public ISceneInsertionPoint GetActiveSinglePropertySceneInsertionPointFromPosition(InsertionPointContext insertPointContext)
    {
      return this.GetInsertionPointFromPosition(insertPointContext);
    }

    public ISceneInsertionPoint GetActiveContentSceneInsertionPointFromPosition(InsertionPointContext insertPointContext)
    {
      ISceneInsertionPoint sceneInsertionPoint = this.LockedInsertionPoint ?? this.GetInsertionPointFromSelectionAndPosition(insertPointContext);
      if (sceneInsertionPoint == null && !(this.ActiveEditingContainer is StyleNode))
      {
        SceneElement sceneElement = (SceneElement) null;
        BaseFrameworkElement editingContainer = this.FindPanelClosestToActiveEditingContainer();
        if (editingContainer != null)
          sceneElement = (SceneElement) editingContainer;
        if (sceneElement == null)
          sceneElement = this.ActiveEditingContainer as SceneElement;
        if (sceneElement != null)
          sceneInsertionPoint = sceneElement.DefaultInsertionPoint;
      }
      return sceneInsertionPoint;
    }

    private ISceneInsertionPoint GetInsertionPointFromSelection(InsertionPointContext insertPointContext)
    {
      ISceneInsertionPoint sceneInsertionPoint = (ISceneInsertionPoint) null;
      SceneElement element = this.ElementSelectionSet.PrimarySelection;
      if (!(element is DataGridColumnNode) && !(element is BaseFrameworkElement))
        element = (SceneElement) null;
      if (element == null)
      {
        ColumnDefinitionNode primarySelection = this.GridColumnSelectionSet.PrimarySelection;
        if (primarySelection != null)
          element = (SceneElement) (primarySelection.Parent as BaseFrameworkElement);
      }
      if (element == null)
      {
        RowDefinitionNode primarySelection = this.GridRowSelectionSet.PrimarySelection;
        if (primarySelection != null)
          element = (SceneElement) (primarySelection.Parent as BaseFrameworkElement);
      }
      if (element != null)
      {
        ISceneInsertionPoint elementAndPosition = this.GetInsertionPointFromElementAndPosition(element, insertPointContext);
        if (elementAndPosition != null && (!insertPointContext.ValidPosition || element.IsAncestorOf((SceneNode) elementAndPosition.SceneElement)))
          sceneInsertionPoint = elementAndPosition;
      }
      return sceneInsertionPoint;
    }

    private ISceneInsertionPoint GetInsertionPointFromPosition(InsertionPointContext insertPointContext)
    {
      ISceneInsertionPoint sceneInsertionPoint = (ISceneInsertionPoint) null;
      if (insertPointContext.ValidPosition)
      {
        BaseFrameworkElement frameworkElement = this.DefaultView.GetSelectableElementAtPoint(insertPointContext.Position, SelectionFor3D.None, false) as BaseFrameworkElement;
        if (frameworkElement != null)
          sceneInsertionPoint = this.GetInsertionPointFromElementAndPosition((SceneElement) frameworkElement, insertPointContext);
      }
      return sceneInsertionPoint;
    }

    private ISceneInsertionPoint GetInsertionPointFromSelectionAndPosition(InsertionPointContext insertPointContext)
    {
      ISceneInsertionPoint sceneInsertionPoint = (ISceneInsertionPoint) null;
      ISceneInsertionPoint pointFromSelection = this.GetInsertionPointFromSelection(insertPointContext);
      if (pointFromSelection == null)
        sceneInsertionPoint = this.GetInsertionPointFromPosition(insertPointContext);
      if (sceneInsertionPoint == null || pointFromSelection != null && !pointFromSelection.SceneElement.IsAncestorOf((SceneNode) sceneInsertionPoint.SceneElement))
        return pointFromSelection;
      return sceneInsertionPoint;
    }

    private ISceneInsertionPoint GetInsertionPointFromElementAndPosition(SceneElement element, InsertionPointContext insertionPointContext)
    {
      if (element != null && element.InsertionTargetProperty != null)
      {
        BaseFrameworkElement frameworkElement = element as BaseFrameworkElement;
        while (frameworkElement != null)
        {
          if (ProjectNeutralTypes.TabControl.IsAssignableFrom((ITypeId) frameworkElement.Type))
          {
            if (frameworkElement.IsViewObjectValid)
            {
              ReferenceStep referenceStep = (ReferenceStep) frameworkElement.ProjectContext.ResolveProperty(SceneViewModel.SelectedIndexProperty);
              if (referenceStep != null)
              {
                int index = (int) referenceStep.GetValue(frameworkElement.ViewObject.PlatformSpecificObject);
                ISceneNodeCollection<SceneNode> collectionForProperty = frameworkElement.GetCollectionForProperty(ItemsControlElement.ItemsProperty);
                if (collectionForProperty != null && index >= 0 && index < collectionForProperty.Count)
                  frameworkElement = collectionForProperty[index] as BaseFrameworkElement;
                else
                  break;
              }
              else
                break;
            }
            else
              break;
          }
          else
          {
            int? fixedCapacity1 = frameworkElement.DefaultContent.FixedCapacity;
            if ((fixedCapacity1.GetValueOrDefault() != 1 ? 0 : (fixedCapacity1.HasValue ? true : false)) != 0 && frameworkElement.DefaultContent.Count == 1 && frameworkElement.DefaultContent[0].DefaultContentProperty != null)
            {
              frameworkElement = frameworkElement.DefaultContent[0] as BaseFrameworkElement;
            }
            else
            {
              if (frameworkElement.DefaultContent.FixedCapacity.HasValue)
              {
                int count = frameworkElement.DefaultContent.Count;
                int? fixedCapacity2 = frameworkElement.DefaultContent.FixedCapacity;
                if ((count >= fixedCapacity2.GetValueOrDefault() ? 0 : (fixedCapacity2.HasValue ? true : false)) == 0)
                  break;
              }
              element = (SceneElement) frameworkElement;
              break;
            }
          }
        }
      }
      BaseFrameworkElement frameworkElement1 = (BaseFrameworkElement) null;
      if (element != null)
        frameworkElement1 = this.FindPanelClosestToRoot();
      SceneNode editingContainer = this.ActiveEditingContainer;
      SceneNode child = (SceneNode) null;
      for (; element != null && (element == editingContainer || !element.IsAncestorOf(editingContainer)) && (element == frameworkElement1 || !element.IsAncestorOf((SceneNode) frameworkElement1)); element = (SceneElement) (element.ParentElement as BaseFrameworkElement))
      {
        if (element.IsViewObjectValid && !ElementUtilities.HasVisualTreeAncestorOfType(element.ViewObject.PlatformSpecificObject as DependencyObject, typeof (Viewport2DVisual3D)))
        {
          ISceneInsertionPoint sceneInsertionPoint = insertionPointContext.GetSceneInsertionPoint(element, child);
          if (sceneInsertionPoint != null && !(element is Viewport3DElement) && element.Visual != null && (element.Type.XamlSourcePath == null && SceneInsertionPointHelper.IsDefaultContentProperty(sceneInsertionPoint) || !SceneInsertionPointHelper.IsDefaultContentProperty(sceneInsertionPoint)))
          {
            Point point = this.DefaultView.TransformPoint((IViewObject) this.DefaultView.HitTestRoot, element.Visual, insertionPointContext.Position);
            BaseFrameworkElement frameworkElement2 = element as BaseFrameworkElement;
            if (frameworkElement2 == null)
              return sceneInsertionPoint;
            bool flag = false;
            if (insertionPointContext.CheckContainerCapacity && SceneInsertionPointHelper.IsDefaultContentProperty(sceneInsertionPoint) && element.DefaultContent.FixedCapacity.HasValue)
            {
              int count = element.DefaultContent.Count;
              int? fixedCapacity = element.DefaultContent.FixedCapacity;
              if ((count < fixedCapacity.GetValueOrDefault() ? 0 : (fixedCapacity.HasValue ? true : false)) != 0)
                flag = true;
            }
            if (PlatformTypes.TextBlock.IsAssignableFrom((ITypeId) element.Type) && SceneInsertionPointHelper.IsDefaultContentProperty(sceneInsertionPoint) && !this.ProjectContext.IsCapabilitySet(PlatformCapability.SupportsTextBlockInlineUIContainer))
              flag = true;
            if (!flag && (!insertionPointContext.ValidPosition || frameworkElement2 != null && frameworkElement2.GetComputedBounds((Base2DElement) frameworkElement2).Contains(point)))
            {
              ITypeId typeToInsert = insertionPointContext.TypeToInsert ?? (!(sceneInsertionPoint.SceneElement is BaseTextElement) ? (ITypeId) (sceneInsertionPoint.Property.PropertyType.ItemType ?? sceneInsertionPoint.Property.PropertyType) : PlatformTypes.UIElement);
              if (sceneInsertionPoint.CanInsert(typeToInsert))
                return sceneInsertionPoint;
            }
          }
        }
        child = (SceneNode) element;
      }
      return (ISceneInsertionPoint) null;
    }

    public object FindResource(object key)
    {
      if (this.DefaultView != null)
      {
        try
        {
          return this.DefaultView.Artboard.ContentArea.TryFindResource(key);
        }
        catch (Exception ex)
        {
        }
      }
      return (object) null;
    }

    public void RefreshSelection()
    {
      this.SetDirtyViewStateBits(SceneViewModel.ViewStateBits.ElementSelection | SceneViewModel.ViewStateBits.TextSelection);
      this.FirePipelineUpdate(SceneUpdateTypeFlags.RefreshSelection);
    }

    public void RefreshCurrentValues()
    {
      this.SetDirtyViewStateBits(SceneViewModel.ViewStateBits.CurrentValues);
      this.FirePipelineUpdate(SceneUpdateTypeFlags.Updated);
    }

    public IDisposable DisableUpdateChildrenOnAddAndRemove()
    {
      return (IDisposable) new SceneViewModel.DisableUpdateChildrenOnAddAndRemoveToken(this);
    }

    public IDisposable ForceBaseValue()
    {
      return (IDisposable) new SceneViewModel.ForceBaseValueToken(this);
    }

    public IDisposable ForceDefaultSetValue()
    {
      return (IDisposable) new SceneViewModel.ForceDefaultSetValueToken(this);
    }

    public IDisposable DisableDrawIntoState()
    {
      return (IDisposable) new SceneViewModel.DisableDrawIntoStateToken(this);
    }

    public IDisposable ForceUseShadowProperties()
    {
      return (IDisposable) new SceneViewModel.ForceUseShadowPropertiesToken(this);
    }

    public IDisposable ForceAlternateSiteNode(DocumentNode node)
    {
      return (IDisposable) new SceneViewModel.ForceAlternateSiteNodeToken(this, node);
    }

    public IViewObject GetViewObject(DocumentNodePath path)
    {
      if (this.defaultView == null)
        throw new InvalidOperationException(ExceptionStringTable.NoViewCreated);
      return this.defaultView.GetCorrespondingViewObject(path);
    }

    public SceneViewModel GetViewModel(IDocumentRoot documentRoot, bool makeActive)
    {
      if (documentRoot == this.DocumentRoot)
        return this;
      return SceneViewModel.GetViewModel(this.XamlDocument.ProjectContext.GetService(typeof (ISceneViewHost)) as ISceneViewHost, documentRoot, makeActive);
    }

    internal static SceneViewModel GetViewModel(ISceneViewHost viewHost, IDocumentRoot documentRoot, bool makeActive)
    {
      if (viewHost != null)
      {
        SceneView sceneView = viewHost.OpenView(documentRoot, makeActive);
        if (sceneView != null)
          return sceneView.ViewModel;
      }
      return (SceneViewModel) null;
    }

    public void PopActiveEditingContainer()
    {
      this.EditContextManager.PopActiveEditingContainer();
    }

    public void MoveToEditContext(EditContext editContext)
    {
      this.EditContextManager.MoveToEditContext(editContext);
    }

    public void SetActiveStoryboardTimeline(IStoryboardContainer storyboardContainer, StoryboardTimelineSceneNode timeline, TriggerBaseNode visualTrigger)
    {
      PerformanceUtility.MeasurePerformanceUntilRender(PerformanceEvent.SwitchTimeline);
      if (this.ActiveStoryboardContainer == storyboardContainer && this.ActiveStoryboardTimeline == timeline && this.ActiveVisualTrigger == visualTrigger)
        return;
      using (SceneEditTransaction editTransaction = this.CreateEditTransaction(StringTable.UndoUnitSetActiveTimeline, true))
      {
        DocumentNodePath storyboardPath = (DocumentNodePath) null;
        if (storyboardContainer != null)
        {
          storyboardPath = ((SceneNode) storyboardContainer).DocumentNodePath;
          if (this.ActiveEditingContainerPath != storyboardPath)
            this.ActiveEditingContainerPath = storyboardPath;
        }
        this.KeyFrameSelectionSet.Clear();
        this.AnimationSelectionSet.Clear();
        this.StoryboardSelectionSet.Clear();
        ReadOnlyCollection<SceneElement> selection = this.ElementSelectionSet.Selection;
        SceneElement primarySelection = this.ElementSelectionSet.PrimarySelection;
        List<SceneElement> removedElements;
        List<SceneElement> editingContainer = this.GetSelectionForEditingContainer<SceneElement>((SceneNode) storyboardContainer, (ICollection<SceneElement>) selection, out removedElements);
        if (removedElements.Contains(primarySelection))
          primarySelection = (SceneElement) null;
        this.ElementSelectionSet.RemoveSelection((ICollection<SceneElement>) removedElements);
        if (this.animationProxyManager != null)
          this.animationProxyManager.AttachToStoryboard(timeline);
        this.EditContextManager.SetStoryboardTimelineTrigger(storyboardPath, timeline, visualTrigger);
        this.ElementSelectionSet.SetSelection((ICollection<SceneElement>) editingContainer, primarySelection);
        editTransaction.Commit();
      }
    }

    public void SetActiveTrigger(TriggerBaseNode trigger)
    {
      IStoryboardContainer storyboardContainer = this.ActiveStoryboardContainer;
      if (trigger != null)
        storyboardContainer = trigger.StoryboardContainer;
      if (trigger == this.ActiveVisualTrigger)
        return;
      this.SetActiveStoryboardTimeline(storyboardContainer, (StoryboardTimelineSceneNode) null, trigger);
    }

    public void ActivateState(VisualStateSceneNode state)
    {
      using (SceneEditTransaction editTransaction = this.CreateEditTransaction(StringTable.ActivateStateUndoUnit, true))
      {
        StoryboardTimelineSceneNode timelineSceneNode = (StoryboardTimelineSceneNode) null;
        if (state != null)
        {
          if (state.ViewModel.ActiveVisualTrigger != null)
            this.SetActiveStoryboardTimeline(this.ActiveStoryboardContainer, (StoryboardTimelineSceneNode) null, (TriggerBaseNode) null);
          if (state.Storyboard == null)
          {
            state.Storyboard = StoryboardTimelineSceneNode.Factory.Instantiate(this);
            state.Storyboard.ShouldSerialize = false;
          }
          timelineSceneNode = state.Storyboard;
        }
        this.EditContextManager.SetActiveVisualStateContext(state, timelineSceneNode);
        this.SetDirtyViewStateBits(SceneViewModel.ViewStateBits.ActiveTimeline);
        this.SetActiveStoryboardTimeline(this.ActiveStoryboardContainer, timelineSceneNode, (TriggerBaseNode) null);
        editTransaction.Commit();
      }
    }

    public void ActivateTransition(VisualStateTransitionSceneNode transition)
    {
      using (SceneEditTransaction editTransaction = this.CreateEditTransaction(StringTable.ActivateTransitionUndoUnit, true))
      {
        StoryboardTimelineSceneNode timelineSceneNode = (StoryboardTimelineSceneNode) null;
        if (transition != null)
        {
          if (transition.ViewModel.ActiveVisualTrigger != null)
            this.SetActiveStoryboardTimeline(this.ActiveStoryboardContainer, (StoryboardTimelineSceneNode) null, (TriggerBaseNode) null);
          transition.UpdateTransitionStoryboard(true, (Dictionary<TimelineSceneNode.PropertyNodePair, TransitionAnimationData>) null);
          timelineSceneNode = transition.Storyboard;
          this.TransitionSelectionSet.SetSelection(transition);
        }
        this.EditContextManager.SetActiveVisualStateTransitionContext(transition, timelineSceneNode);
        this.SetActiveStoryboardTimeline(this.ActiveStoryboardContainer, timelineSceneNode, (TriggerBaseNode) null);
        editTransaction.Commit();
      }
    }

    public void PinState(VisualStateSceneNode state)
    {
      using (SceneEditTransaction editTransaction = this.CreateEditTransaction(StringTable.PinStateUndoUnit, true))
      {
        this.EditContextManager.PinStateInContext(state);
        editTransaction.Commit();
      }
    }

    public void UnpinState(VisualStateSceneNode state)
    {
      using (SceneEditTransaction editTransaction = this.CreateEditTransaction(StringTable.UnpinStateUndoUnit, true))
      {
        this.EditContextManager.UnpinStateInContext(state);
        editTransaction.Commit();
      }
    }

    public void UnpinAllStates()
    {
      using (SceneEditTransaction editTransaction = this.CreateEditTransaction(StringTable.UnpinAllStatesUndoUnit, true))
      {
        this.EditContextManager.UnpinAllStatesInContext();
        editTransaction.Commit();
      }
    }

    public SceneEditTransaction CreateEditTransaction(string description)
    {
      return this.sceneDocument.CreateEditTransaction(description);
    }

    public SceneEditTransaction CreateEditTransaction(string description, bool hidden, SceneEditTransactionType sceneEditTransactionType)
    {
      return this.sceneDocument.CreateEditTransaction(description, hidden, sceneEditTransactionType);
    }

    public SceneEditTransaction CreateEditTransaction(string description, bool hidden)
    {
      return this.sceneDocument.CreateEditTransaction(description, hidden);
    }

    public void AddUndoUnit(IUndoUnit undoUnit)
    {
      this.sceneDocument.AddUndoUnit(undoUnit);
    }

    public ILayoutDesigner GetLayoutDesignerForChild(SceneElement childElement, bool forceNoEffectDesigner)
    {
      if (childElement.ViewObject == null)
      {
        if (!this.damage.Contains(childElement.DocumentNode))
          return (ILayoutDesigner) new InvalidElementLayoutDesigner();
        this.Document.OnUpdatedEditTransaction();
      }
      SceneElement parentElement = childElement.ParentElement;
      TextElementSceneElement elementSceneElement = parentElement as TextElementSceneElement;
      if (elementSceneElement != null)
        parentElement = (SceneElement) elementSceneElement.HostElement;
      return this.GetLayoutDesignerForParent(parentElement, forceNoEffectDesigner);
    }

    public ILayoutDesigner GetLayoutDesignerForParent(SceneElement parentElement, bool forceNoEffectDesigner)
    {
      return !this.UsingEffectDesigner || forceNoEffectDesigner ? (parentElement == null ? (ILayoutDesigner) new LayoutDesigner() : LayoutDesignerFactory.Instantiate((SceneNode) parentElement)) : (ILayoutDesigner) new RenderTransformDesigner();
    }

    public bool IsExternal(DocumentNode node)
    {
      if (node.DocumentRoot != null)
        return node.DocumentRoot != this.DocumentRoot;
      return false;
    }

    public SceneNode CreateSceneNode(Type targetType)
    {
      return this.CreateSceneNode((ITypeId) this.Document.ProjectContext.GetType(targetType));
    }

    public SceneNode CreateSceneNode(ITypeId targetType)
    {
      IDocumentContext documentContext = this.Document.DocumentContext;
      IType type1 = this.ProjectContext.ResolveType(targetType);
      Type type2 = type1 != null ? type1.RuntimeType : (Type) null;
      return this.sceneNodeFactory.Instantiate(this, !(type2 != (Type) null) || !type2.IsValueType ? (DocumentNode) documentContext.CreateNode(targetType) : documentContext.CreateNode(type2, Activator.CreateInstance(type2)));
    }

    public SceneNode CreateSceneNode(object exampleInstance)
    {
      return this.GetSceneNode(this.Document.DocumentContext.CreateNode(exampleInstance != null ? exampleInstance.GetType() : typeof (object), exampleInstance));
    }

    public SceneNode GetSceneNode(DocumentNode node)
    {
      SceneNode sceneNode = (SceneNode) null;
      if (node != null)
      {
        if (this.IsExternal(node))
          throw new InvalidOperationException();
        sceneNode = (SceneNode) node.SceneNode;
        if (sceneNode == null || sceneNode.ViewModel != this)
          sceneNode = this.sceneNodeFactory.Instantiate(this, node);
      }
      return sceneNode;
    }

    public object CreateInstance(DocumentNodePath nodePath)
    {
      using (InstanceBuilderContext instanceBuilderContext = new InstanceBuilderContext(this.Document.ProjectContext, this, this.IsForcingUseShadowProperties, this.alternateSiteNode))
      {
        instanceBuilderContext.ViewNodeManager.RootNodePath = nodePath;
        instanceBuilderContext.ViewNodeManager.Instantiate(instanceBuilderContext.ViewNodeManager.Root);
        instanceBuilderContext.ViewNodeManager.ResolveReferences();
        object obj = instanceBuilderContext.ViewNodeManager.ValidRootInstance;
        if (obj == null && nodePath.Node.TargetType.IsValueType)
          obj = InstanceBuilderOperations.InstantiateType(nodePath.Node.TargetType, true);
        return obj;
      }
    }

    public IViewStoryboard CreateViewStoryboard(TimelineSceneNode node)
    {
      return this.DefaultView.CreateViewStoryboard(node);
    }

    internal void UpdateViewModelStateForNodeRemoving(DocumentNodePath context, DocumentNode nodeBeingRemoved)
    {
      if (nodeBeingRemoved == null || !nodeBeingRemoved.IsInDocument)
        return;
      DocumentNode nodeBeingLookedFor = nodeBeingRemoved;
      if (nodeBeingRemoved.Type.IsExpression)
        nodeBeingLookedFor = new ExpressionEvaluator(this.DocumentRootResolver).EvaluateExpression(context, nodeBeingRemoved);
      if (nodeBeingRemoved.IsProperty && DictionaryEntryNode.KeyProperty.Equals((object) nodeBeingRemoved.SitePropertyKey))
        nodeBeingLookedFor = nodeBeingRemoved.Parent.Properties[DictionaryEntryNode.ValueProperty];
      if (nodeBeingRemoved.IsProperty && nodeBeingRemoved.Parent.NameProperty == nodeBeingRemoved.SitePropertyKey)
      {
        DocumentCompositeNode parent = nodeBeingRemoved.Parent;
        if (parent.Parent != null && parent.IsProperty && DictionaryEntryNode.ValueProperty.Equals((object) parent.SitePropertyKey))
          nodeBeingLookedFor = (DocumentNode) parent;
      }
      if (nodeBeingLookedFor == null || !(nodeBeingLookedFor is DocumentCompositeNode))
        return;
      List<SceneElement> list = new List<SceneElement>();
      foreach (SceneElement sceneElement in this.ElementSelectionSet.Selection)
      {
        if (sceneElement.DocumentNodePath.Contains(nodeBeingLookedFor))
          list.Add(sceneElement);
      }
      this.ElementSelectionSet.RemoveSelection((ICollection<SceneElement>) list);
      ISceneInsertionPoint lockedInsertionPoint = this.LockedInsertionPoint;
      if (lockedInsertionPoint == null || !lockedInsertionPoint.SceneElement.DocumentNodePath.Contains(nodeBeingLookedFor))
        return;
      this.SetLockedInsertionPoint((SceneElement) null);
    }

    public bool FindInternalResourceReferences(DocumentCompositeNode resourceNode, ICollection<SceneNode> references)
    {
      if (!PlatformTypes.PlatformsCompatible(this.ProjectContext.PlatformMetadata, resourceNode.PlatformMetadata))
        return false;
      int count = references.Count;
      DocumentNode searchKey = ResourceNodeHelper.GetResourceEntryKey(resourceNode);
      foreach (DocumentNode documentNode in this.RootNode.DocumentNode.SelectDescendantNodes((Predicate<DocumentNode>) (node =>
      {
        DocumentCompositeNode node1 = node as DocumentCompositeNode;
        return node1 != null && node1.Type.IsResource && ResourceNodeHelper.GetResourceKey(node1).Equals(searchKey);
      })))
      {
        foreach (DocumentNodePath context in (IEnumerable<DocumentNodePath>) this.defaultView.GenerateAllPaths(documentNode))
        {
          if (new ExpressionEvaluator(this.DocumentRootResolver).EvaluateExpression(context, documentNode) == resourceNode.Properties[DictionaryEntryNode.ValueProperty])
          {
            references.Add(this.GetSceneNode(documentNode));
            break;
          }
        }
      }
      return references.Count > count;
    }

    public SceneElement GetUnlockedAncestor(DocumentNodePath nodePath)
    {
      SceneElement sceneElement = (SceneElement) this.GetSceneNode(nodePath.Node);
label_4:
      while (sceneElement != null && (sceneElement.StoryboardContainer is StyleNode || sceneElement.StoryboardContainer is FrameworkTemplateElement) && (!((SceneNode) sceneElement.StoryboardContainer).IsSelectable || !((SceneNode) sceneElement.StoryboardContainer).IsAncestorOf(this.ActiveEditingContainer)))
      {
        SceneNode parent = ((SceneNode) sceneElement.StoryboardContainer).Parent;
        while (true)
        {
          if (parent != null && (sceneElement = parent as SceneElement) == null)
            parent = parent.Parent;
          else
            goto label_4;
        }
      }
      return sceneElement;
    }

    public DocumentNodePath GetAncestorInEditingContainer(DocumentNodePath nodePath, DocumentNodePath editingContainer, DocumentNodePath targetElementPath)
    {
      DocumentNodePath documentNodePath1;
      SceneElement sceneElement;
      for (; nodePath != null && !object.Equals((object) nodePath.GetContainerNodePath(), (object) editingContainer); nodePath = sceneElement != null ? documentNodePath1 : (DocumentNodePath) null)
      {
        DocumentNodePath documentNodePath2 = nodePath;
        documentNodePath1 = !object.Equals((object) documentNodePath2, (object) targetElementPath) ? (!DictionaryEntryNode.ValueProperty.Equals((object) documentNodePath2.ContainerOwnerProperty) || !PlatformTypes.Style.IsAssignableFrom((ITypeId) documentNodePath2.ContainerNode.Type) ? documentNodePath2.GetContainerOwnerPath() : (DocumentNodePath) null) : editingContainer;
        sceneElement = (SceneElement) null;
        for (; documentNodePath1 != null; documentNodePath1 = documentNodePath1.GetParent())
        {
          SceneNode sceneNode = this.GetSceneNode(documentNodePath1.Node);
          sceneElement = sceneNode as SceneElement;
          if (sceneElement == null)
          {
            SetterSceneNode setterSceneNode = sceneNode as SetterSceneNode;
            if (setterSceneNode != null)
            {
              sceneElement = setterSceneNode.StoryboardContainer.ResolveTargetName(setterSceneNode.Target) as SceneElement;
              if (sceneElement != null)
              {
                documentNodePath1 = documentNodePath1.GetContainerNodePath().GetPathInContainer(sceneElement.DocumentNode);
                break;
              }
            }
          }
          else
            break;
        }
      }
      return nodePath;
    }

    public void DeleteElementTree(SceneElement element)
    {
      if (element is StyleNode || element is FrameworkTemplateElement)
      {
        SceneElement sceneElement = (SceneElement) ((IStoryboardContainer) element).TargetElement;
        IPropertyId propertyId = (IPropertyId) element.DocumentNodePath.ContainerOwnerProperty;
        if (SetterSceneNode.ValueProperty.Equals((object) propertyId))
        {
          SetterSceneNode setterSceneNode = (SetterSceneNode) this.GetSceneNode(element.DocumentNodePath.ContainerOwner);
          setterSceneNode.Parent.GetCollectionForChild((SceneNode) setterSceneNode).Remove((SceneNode) setterSceneNode);
        }
        else
        {
          if (sceneElement == null)
            return;
          PropertyReference propertyReference = new PropertyReference((ReferenceStep) propertyId);
          sceneElement.ClearValue(propertyReference);
        }
      }
      else
      {
        if (element is Base3DElement && element.ParentElement != null)
          this.AnimationEditor.DeleteAllAnimationsInSubtree(element.ParentElement);
        else
          this.AnimationEditor.DeleteAllAnimationsInSubtree(element);
        this.DeleteCorrespondingLayoutPaths(element);
        if (element.Parent == null)
          return;
        element.Remove();
      }
    }

    private void DeleteCorrespondingLayoutPaths(SceneElement element)
    {
      if (!element.IsNamed)
        return;
      DocumentNode documentNode = element.DocumentNode;
      while (documentNode != null && documentNode.NameScope == null && documentNode.Parent != null)
        documentNode = (DocumentNode) documentNode.Parent;
      if (documentNode == null)
        return;
      string name = element.Name;
      foreach (DocumentNode node in documentNode.SelectDescendantNodes((Predicate<DocumentNode>) (dn =>
      {
        if (!ProjectNeutralTypes.PathListBox.IsAssignableFrom((ITypeId) dn.Type))
          return ProjectNeutralTypes.PathPanel.IsAssignableFrom((ITypeId) dn.Type);
        return true;
      })))
      {
        PathListBoxElement pathListBoxElement = this.GetSceneNode(node) as PathListBoxElement;
        PathPanelElement pathPanelElement = this.GetSceneNode(node) as PathPanelElement;
        ISceneNodeCollection<SceneNode> sceneNodeCollection = (ISceneNodeCollection<SceneNode>) null;
        List<LayoutPathNode> list = new List<LayoutPathNode>();
        if (pathListBoxElement != null)
          sceneNodeCollection = pathListBoxElement.LayoutPaths;
        else if (pathPanelElement != null)
          sceneNodeCollection = pathPanelElement.LayoutPaths;
        if (sceneNodeCollection != null && sceneNodeCollection.Count > 0)
        {
          foreach (LayoutPathNode layoutPathNode in (IEnumerable<SceneNode>) sceneNodeCollection)
          {
            BindingSceneNode binding = layoutPathNode.GetBinding(LayoutPathNode.SourceElementProperty);
            if (binding != null && binding.ElementName == name)
              list.Add(layoutPathNode);
          }
          foreach (LayoutPathNode layoutPathNode in list)
            sceneNodeCollection.Remove((SceneNode) layoutPathNode);
        }
      }
    }

    public void DeleteSelectedElements()
    {
      if (this.ElementSelectionSet.IsEmpty)
        return;
      using (SceneEditTransaction editTransaction = this.CreateEditTransaction(StringTable.UndoUnitDeleteSelectedElements))
      {
        List<SceneElement> elements = new List<SceneElement>((IEnumerable<SceneElement>) this.ElementSelectionSet.Selection);
        ElementUtilities.SortElementsByDepth(elements);
        this.ElementSelectionSet.Clear();
        foreach (SceneElement element in elements)
          this.DeleteElementTree(element);
        editTransaction.Commit();
      }
    }

    public void OnNodeAdding(SceneNode parent, SceneNode child)
    {
      DocumentNodeNameScope scopeForChildren = parent.DocumentNode.FindNameScopeForChildren();
      if (scopeForChildren == null)
        return;
      new SceneNodeIDHelper(this, scopeForChildren).FixNameConflicts(child);
    }

    public void OnNodeAdded(SceneNode parent, SceneNode child)
    {
      if (this.disableUpdateChildrenOnAddAndRemoveCount > 0)
        return;
      SceneElement sceneElement = child as SceneElement;
      if (sceneElement != null)
      {
        IPropertyId propertyId = (IPropertyId) child.Parent.GetPropertyForChild(child);
        if (parent.ContentProperties.Contains(propertyId))
        {
          Base3DElement base3Delement = sceneElement as Base3DElement;
          if (base3Delement != null)
          {
            Transform3D transform = sceneElement.GetLocalValue(base3Delement.TransformPropertyId) as Transform3D;
            if (transform != null && !(transform is Transform3DGroup))
            {
              CanonicalTransform3D canonicalTransform3D = new CanonicalTransform3D(transform);
              using (this.ForceBaseValue())
                sceneElement.SetValue(base3Delement.TransformPropertyId, (object) canonicalTransform3D.ToTransform());
            }
          }
        }
      }
      if (!this.ElementSelectionSet.IsEmpty || child != this.FindPanelClosestToActiveEditingContainer())
        return;
      this.OnActiveInsertionPointChanged();
    }

    public void OnNodeRemoving(SceneNode parent, SceneNode child)
    {
    }

    public void OnNodeRemoved(SceneNode parent, SceneNode child)
    {
      if (this.disableUpdateChildrenOnAddAndRemoveCount > 0)
        return;
      InlineUIContainerElement containerElement = parent as InlineUIContainerElement;
      if (containerElement == null)
        return;
      this.DeleteElementTree((SceneElement) containerElement);
    }

    public BaseFrameworkElement FindPanelClosestToActiveEditingContainer()
    {
      return this.FindPanelClosestToElement(this.ActiveEditingContainer as SceneElement);
    }

    public BaseFrameworkElement FindPanelClosestToRoot()
    {
      return this.FindPanelClosestToElement(this.ViewRoot as SceneElement);
    }

    private BaseFrameworkElement FindPanelClosestToElement(SceneElement topElement)
    {
      if (topElement != null)
      {
        foreach (SceneElement sceneElement in SceneElementHelper.GetElementTreeBreadthFirst(topElement))
        {
          if (sceneElement is PanelElement)
            return sceneElement as BaseFrameworkElement;
        }
      }
      return topElement as BaseFrameworkElement;
    }

    internal void FireEarlySceneUpdatePhase(SceneUpdatePhaseEventArgs args)
    {
      if (this.EarlySceneUpdatePhase == null)
        return;
      args.Refresh(false, false);
      this.EarlySceneUpdatePhase((object) this, args);
    }

    internal void FireLateSceneUpdatePhase(SceneUpdatePhaseEventArgs args)
    {
      if (this.LateSceneUpdatePhase == null)
        return;
      args.Refresh(false, false);
      this.LateSceneUpdatePhase((object) this, args);
    }

    public ICollection<IProperty> GetProperties(IInstanceBuilderContext context, ViewNode viewNode)
    {
      ICollection<IProperty> collection = (ICollection<IProperty>) ((DocumentCompositeNode) viewNode.DocumentNode).Properties.Keys;
      TriggerBaseNode activeTriggerForStoryboardContainer;
      if (this.defaultView != null && context == this.defaultView.InstanceBuilderContext && (this.ActiveStoryboardContainer != null && typeof (MultiTrigger).IsAssignableFrom(viewNode.TargetType)) && this.FindActiveStoryboardVisualTrigger(this.defaultView.GetDocumentNodePath(viewNode), out activeTriggerForStoryboardContainer) != null)
      {
        IProperty property = this.ProjectContext.ResolveProperty(MultiTriggerNode.ConditionsProperty);
        if (!collection.Contains(property))
        {
          collection = (ICollection<IProperty>) new List<IProperty>((IEnumerable<IProperty>) collection);
          collection.Add(property);
        }
      }
      if (this.defaultView != null && context == this.defaultView.InstanceBuilderContext && PlatformTypes.MediaElement.IsAssignableFrom((ITypeId) viewNode.Type))
      {
        collection = (ICollection<IProperty>) new List<IProperty>((IEnumerable<IProperty>) collection);
        DependencyPropertyReferenceStep propertyReferenceStep = DesignTimeProperties.ResolveDesignTimeReferenceStep(DesignTimeProperties.OwningTimelineSourceProperty, this.Document.ProjectContext.PlatformMetadata);
        collection.Add((IProperty) propertyReferenceStep);
      }
      return collection;
    }

    public DocumentNode GetPropertyValue(IInstanceBuilderContext context, ViewNode viewNode, IPropertyId propertyKey)
    {
      if (this.defaultView != null && context == this.defaultView.InstanceBuilderContext && this.ActiveStoryboardContainer != null)
      {
        TriggerBaseNode activeTriggerForStoryboardContainer = (TriggerBaseNode) null;
        TriggerBaseNode conditionOrTrigger = this.GetActiveStoryboardTriggerFromConditionOrTrigger(viewNode, out activeTriggerForStoryboardContainer);
        if (conditionOrTrigger != null)
        {
          if (TriggerNode.PropertyProperty.Equals((object) propertyKey) || ConditionNode.PropertyProperty.Equals((object) propertyKey))
          {
            DependencyPropertyReferenceStep propertyReferenceStep = DesignTimeProperties.ResolveDesignTimeReferenceStep(DesignTimeProperties.IsVisualTriggerActiveProperty, (IPlatformMetadata) this.Document.ProjectContext.Platform.Metadata);
            return (DocumentNode) this.Document.DocumentContext.CreateNode(PlatformTypes.DependencyProperty, (IDocumentNodeValue) new DocumentNodeMemberValue((IMember) propertyReferenceStep));
          }
          if (TriggerNode.ValueProperty.Equals((object) propertyKey) || ConditionNode.ValueProperty.Equals((object) propertyKey))
          {
            if (context.IsSerializationScope)
              return (DocumentNode) this.Document.DocumentContext.CreateNode(PlatformTypes.Boolean, (IDocumentNodeValue) new DocumentNodeStringValue(conditionOrTrigger == activeTriggerForStoryboardContainer ? bool.TrueString : bool.FalseString));
            return (DocumentNode) this.Document.DocumentContext.CreateNode(PlatformTypes.Boolean, (IDocumentNodeValue) new DocumentNodeStructValue<bool>(conditionOrTrigger == activeTriggerForStoryboardContainer));
          }
        }
        else if (typeof (MultiTrigger).IsAssignableFrom(viewNode.TargetType))
        {
          DocumentNodePath documentNodePath = this.defaultView.GetDocumentNodePath(viewNode);
          TriggerBaseNode storyboardVisualTrigger = this.FindActiveStoryboardVisualTrigger(documentNodePath, out activeTriggerForStoryboardContainer);
          if (storyboardVisualTrigger != null && propertyKey.Equals((object) MultiTriggerNode.ConditionsProperty))
          {
            DocumentCompositeNode documentCompositeNode = ((DocumentCompositeNode) documentNodePath.Node).Properties[propertyKey] as DocumentCompositeNode;
            if (documentCompositeNode == null || !documentCompositeNode.SupportsChildren || documentCompositeNode.Children.Count == 0)
            {
              IDocumentContext documentContext = this.Document.DocumentContext;
              IPlatformTypes metadata = ((IProjectContext) documentContext.TypeResolver).Platform.Metadata;
              DocumentCompositeNode node = documentContext.CreateNode((ITypeId) metadata.GetType(typeof (Condition)));
              DependencyPropertyReferenceStep propertyReferenceStep = DesignTimeProperties.ResolveDesignTimeReferenceStep(DesignTimeProperties.IsVisualTriggerActiveProperty, (IPlatformMetadata) metadata);
              node.Properties[ConditionNode.PropertyProperty] = (DocumentNode) documentContext.CreateNode(PlatformTypes.DependencyProperty, (IDocumentNodeValue) new DocumentNodeMemberValue((IMember) propertyReferenceStep));
              node.Properties[ConditionNode.ValueProperty] = documentContext.CreateNode(typeof (bool), (object) (bool) (storyboardVisualTrigger == activeTriggerForStoryboardContainer ? true : false));
              documentCompositeNode = documentContext.CreateNode(typeof (ConditionCollection));
              documentCompositeNode.Children.Add((DocumentNode) node);
            }
            return (DocumentNode) documentCompositeNode;
          }
        }
      }
      if (this.defaultView != null && context == this.defaultView.InstanceBuilderContext && (PlatformTypes.MediaElement.IsAssignableFrom((ITypeId) viewNode.Type) && DesignTimeProperties.OwningTimelineSourceProperty.Equals((object) propertyKey)))
      {
        MediaTimelineSceneNode owningTimeline = (this.GetSceneNode(viewNode.DocumentNode) as MediaSceneElement).OwningTimeline;
        if (owningTimeline != null)
          return ((DocumentCompositeNode) owningTimeline.DocumentNode).Properties[MediaTimelineSceneNode.SourceProperty];
      }
      return ((DocumentCompositeNode) viewNode.DocumentNode).Properties[propertyKey];
    }

    private TriggerBaseNode GetActiveStoryboardTriggerFromConditionOrTrigger(ViewNode conditionOrTrigger, out TriggerBaseNode activeTriggerForStoryboardContainer)
    {
      TriggerBaseNode triggerBaseNode = (TriggerBaseNode) null;
      activeTriggerForStoryboardContainer = (TriggerBaseNode) null;
      if (typeof (Condition).IsAssignableFrom(conditionOrTrigger.TargetType))
      {
        ViewNode parent1 = conditionOrTrigger.Parent;
        if (parent1 != null)
        {
          ViewNode parent2 = parent1.Parent;
          if (parent2 != null && typeof (MultiTrigger).IsAssignableFrom(parent2.TargetType))
            triggerBaseNode = this.FindActiveStoryboardVisualTrigger(this.defaultView.GetDocumentNodePath(parent2), out activeTriggerForStoryboardContainer);
        }
      }
      else if (typeof (Trigger).IsAssignableFrom(conditionOrTrigger.TargetType))
        triggerBaseNode = this.FindActiveStoryboardVisualTrigger(this.defaultView.GetDocumentNodePath(conditionOrTrigger), out activeTriggerForStoryboardContainer);
      return triggerBaseNode;
    }

    private TriggerBaseNode FindActiveStoryboardVisualTrigger(DocumentNodePath path, out TriggerBaseNode activeTriggerForStoryboardContainer)
    {
      TriggerBaseNode trigger = (TriggerBaseNode) null;
      EditContext triggerContext = (EditContext) null;
      this.EditContextManager.SingleViewModelEditContextWalker.Walk(false, (SingleHistoryCallback) ((context, isGhosted) =>
      {
        IStoryboardContainer storyboardContainer = context.StoryboardContainer;
        if (storyboardContainer != null)
        {
          foreach (TriggerBaseNode triggerBaseNode in (IEnumerable<TriggerBaseNode>) storyboardContainer.VisualTriggers)
          {
            if (path.Equals((object) triggerBaseNode.DocumentNodePath))
            {
              triggerContext = context;
              trigger = triggerBaseNode;
              return true;
            }
          }
        }
        return false;
      }));
      activeTriggerForStoryboardContainer = (TriggerBaseNode) null;
      if (trigger != null)
        activeTriggerForStoryboardContainer = triggerContext.Trigger;
      return trigger;
    }

    public bool ShouldDisableVisualStateManagerFor(IInstanceBuilderContext context, ViewNode viewNode)
    {
      bool shouldDisable = false;
      if (this.defaultView != null && context == this.defaultView.InstanceBuilderContext && this.ActiveStoryboardContainer != null)
      {
        DocumentNodePath path = this.defaultView.GetDocumentNodePath(viewNode);
        this.EditContextManager.SingleViewModelEditContextWalker.Walk(false, (SingleHistoryCallback) ((editContext, isGhosted) =>
        {
          SceneNode hostNode = VisualStateManagerSceneNode.GetHostNode(editContext.EditingContainer);
          if (hostNode == null || !path.Equals((object) hostNode.DocumentNodePath))
            return false;
          shouldDisable = true;
          return true;
        }));
      }
      return shouldDisable;
    }

    private void ElementSelectionSet_SelectionChangedOutsideUndo(object sender, EventArgs e)
    {
      if (this.elementSelectionSet.Selection.Count <= 0)
        return;
      this.GridColumnSelectionSet.Clear();
      this.GridRowSelectionSet.Clear();
      this.DependencyObjectSelectionSet.Clear();
    }

    private void ElementSelectionSet_SelectionChanged(object sender, EventArgs e)
    {
      this.SetDirtyViewStateBits(SceneViewModel.ViewStateBits.ElementSelection);
      if (this.LockedInsertionPoint != null)
        return;
      this.OnActiveInsertionPointChanged();
    }

    private void DependencyObjectSelectionSet_SelectionChanged(object sender, EventArgs e)
    {
      this.SetDirtyViewStateBits(SceneViewModel.ViewStateBits.DependencyObjectSelection);
      if (this.LockedInsertionPoint != null)
        return;
      this.OnActiveInsertionPointChanged();
    }

    private void PathPartSelectionSet_SelectionChanged(object sender, EventArgs e)
    {
      this.SetDirtyViewStateBits(SceneViewModel.ViewStateBits.PathPartSelection);
      ReadOnlyCollection<PathPart> selection = this.pathPartSelectionSet.Selection;
      Dictionary<SceneElement, bool> dictionary = new Dictionary<SceneElement, bool>();
      int index1 = 0;
      int index2 = 0;
      while (index1 < selection.Count && index2 < this.lastPathPartList.Count)
      {
        SceneElement key = (SceneElement) null;
        if (selection[index1] < this.lastPathPartList[index2])
        {
          key = selection[index1].SceneElement;
          ++index1;
        }
        else if (selection[index1] > this.lastPathPartList[index2])
        {
          key = this.lastPathPartList[index2].SceneElement;
          ++index2;
        }
        else
        {
          ++index1;
          ++index2;
        }
        if (key != null && !dictionary.ContainsKey(key))
          dictionary.Add(key, true);
      }
      for (; index1 < selection.Count; ++index1)
      {
        SceneElement sceneElement = selection[index1].SceneElement;
        if (!dictionary.ContainsKey(sceneElement))
          dictionary.Add(sceneElement, true);
      }
      for (; index2 < this.lastPathPartList.Count; ++index2)
      {
        SceneElement sceneElement = this.lastPathPartList[index2].SceneElement;
        if (!dictionary.ContainsKey(sceneElement))
          dictionary.Add(sceneElement, true);
      }
      foreach (SceneElement element in dictionary.Keys)
        this.UpdatePathAdornerSetsForElement(element);
      this.lastPathPartList = selection;
    }

    private void UpdatePathAdornerSetsForElement(SceneElement element)
    {
      SceneView defaultView = this.DefaultView;
      if (defaultView == null)
        return;
      foreach (AdornerSet adornerSet in (IEnumerable<AdornerSet>) defaultView.AdornerLayer.Get2DAdornerSets(element))
      {
        PathAdornerSet pathAdornerSet = adornerSet as PathAdornerSet;
        if (pathAdornerSet != null)
          pathAdornerSet.UpdateActiveStateFromSelection();
      }
    }

    private void GeneralSelectionSet_SelectionChanged(object sender, EventArgs e)
    {
      if (sender == this.animationSelectionSet)
        this.SetDirtyViewStateBits(SceneViewModel.ViewStateBits.AnimationSelection);
      else if (sender == this.dependencyObjectSelectionSet)
        this.SetDirtyViewStateBits(SceneViewModel.ViewStateBits.ElementSelection | SceneViewModel.ViewStateBits.DependencyObjectSelection);
      else if (sender == this.textSelectionSet)
        this.SetDirtyViewStateBits(SceneViewModel.ViewStateBits.TextSelection);
      else if (sender == this.keyFrameSelectionSet)
        this.SetDirtyViewStateBits(SceneViewModel.ViewStateBits.KeyFrameSelection);
      else if (sender == this.storyboardSelectionSet)
        this.SetDirtyViewStateBits(SceneViewModel.ViewStateBits.StoryboardSelection);
      else if (sender == this.behaviorSelectionSet)
        this.SetDirtyViewStateBits(SceneViewModel.ViewStateBits.BehaviorSelection);
      else if (sender == this.annotationSelectionSet)
        this.SetDirtyViewStateBits(SceneViewModel.ViewStateBits.AnnotationSelection);
      else if (sender == this.gridColumnSelectionSet)
      {
        this.SetDirtyViewStateBits(SceneViewModel.ViewStateBits.GridColumnSelection);
        if (this.LockedInsertionPoint != null)
          return;
        this.OnActiveInsertionPointChanged();
      }
      else if (sender == this.gridRowSelectionSet)
      {
        this.SetDirtyViewStateBits(SceneViewModel.ViewStateBits.GridRowSelection);
        if (this.LockedInsertionPoint != null)
          return;
        this.OnActiveInsertionPointChanged();
      }
      else if (sender == this.propertySelectionSet)
        this.SetDirtyViewStateBits(SceneViewModel.ViewStateBits.PropertySelection);
      else if (sender == this.childPropertySelectionSet)
      {
        this.SetDirtyViewStateBits(SceneViewModel.ViewStateBits.ChildPropertySelection);
      }
      else
      {
        if (sender != this.setterSelectionSet)
          return;
        this.SetDirtyViewStateBits(SceneViewModel.ViewStateBits.SetterSelection);
      }
    }

    private void AnimationEditor_RecordModeChanged(object sender, EventArgs e)
    {
      this.SetDirtyViewStateBits(SceneViewModel.ViewStateBits.RecordMode);
    }

    private void TriggerSubscription_Inserted(object sender, SceneNode basisNode, object basisContent, SceneNode newPathNode, object newContent)
    {
      this.SetDirtyViewStateBits(SceneViewModel.ViewStateBits.TriggersChanged);
    }

    private void TriggerSubscription_Removed(object sender, SceneNode basisNode, object basisContent, SceneNode oldPathNode, object oldContent)
    {
      this.SetDirtyViewStateBits(SceneViewModel.ViewStateBits.TriggersChanged);
    }

    private void TriggerSubscription_ContentChanged(object sender, SceneNode pathNode, object content, DocumentNodeMarker damageMarker, DocumentNodeChange damage)
    {
      this.SetDirtyViewStateBits(SceneViewModel.ViewStateBits.TriggersChanged);
    }

    public void RemoveElement(SceneNode child)
    {
      child.Remove();
    }

    private void OnActiveEditingContainerChanged()
    {
      this.SetDirtyViewStateBits(SceneViewModel.ViewStateBits.ActiveEditingContainer);
    }

    private void OnActiveInsertionPointChanged()
    {
      this.SetDirtyViewStateBits(SceneViewModel.ViewStateBits.ActiveSceneInsertionPoint);
      this.activeInsertionPoint = (ISceneInsertionPoint) null;
    }

    private void OnLockedInsertionPointChanged()
    {
      this.SetDirtyViewStateBits(SceneViewModel.ViewStateBits.LockedInsertionPoint);
      this.OnActiveInsertionPointChanged();
    }

    private void OnActiveTimelineChanged(IStoryboardContainer oldStoryboardContainer, IStoryboardContainer newStoryboardContainer, TriggerBaseNode oldVisualTrigger, TriggerBaseNode newVisualTrigger, StoryboardTimelineSceneNode oldTimeline, StoryboardTimelineSceneNode newTimeline, DocumentNodePath oldEditContainer, DocumentNodePath newEditContainer)
    {
      if (oldTimeline != newTimeline)
        this.SetDirtyViewStateBits(SceneViewModel.ViewStateBits.ActiveTimeline);
      if (oldVisualTrigger != newVisualTrigger)
        this.SetDirtyViewStateBits(SceneViewModel.ViewStateBits.ActiveTrigger);
      if (oldEditContainer != newEditContainer)
        this.SetDirtyViewStateBits(SceneViewModel.ViewStateBits.ActiveEditingContainer);
      this.animationEditor.OnViewModelActiveTimelineChanged();
    }

    private void OnInactiveEditingContainerChanged()
    {
      this.SetDirtyViewStateBits(SceneViewModel.ViewStateBits.InactiveEditingContainer);
    }

    private void OnEditContextHistoryChanged()
    {
      this.SetDirtyViewStateBits(SceneViewModel.ViewStateBits.EditContextHistory);
    }

    internal void NotifyActiveEditContextChanged(EditContext oldContext, EditContext newContext, ISceneInsertionPoint oldInsertionPoint, ISceneInsertionPoint newInsertionPoint)
    {
      if (oldContext.EditingContainerPath != newContext.EditingContainerPath)
        this.OnActiveEditingContainerChanged();
      if (oldContext.ParentElement != newContext.ParentElement)
        this.OnEditContextHistoryChanged();
      if (oldContext.StoryboardContainer != newContext.StoryboardContainer || oldContext.Timeline != newContext.Timeline || (oldContext.Trigger != newContext.Trigger || oldContext.EditingContainerPath != newContext.EditingContainerPath))
        this.OnActiveTimelineChanged(oldContext.StoryboardContainer, newContext.StoryboardContainer, oldContext.Trigger, newContext.Trigger, oldContext.Timeline, newContext.Timeline, oldContext.EditingContainerPath, newContext.EditingContainerPath);
      if (oldContext.StateEditTarget != newContext.StateEditTarget || oldContext.StateStoryboardEditTarget != newContext.StateStoryboardEditTarget)
        this.SetDirtyViewStateBits(SceneViewModel.ViewStateBits.ActiveStateContext);
      if (oldContext.TransitionEditTarget != newContext.TransitionEditTarget || oldContext.TransitionStoryboardEditTarget != newContext.TransitionStoryboardEditTarget)
        this.SetDirtyViewStateBits(SceneViewModel.ViewStateBits.ActiveTransitionContext);
      if (!oldContext.PinnedStatesEquals(newContext))
        this.SetDirtyViewStateBits(SceneViewModel.ViewStateBits.ActivePinnedStates);
      ISceneInsertionPoint lockedInsertionPoint = this.LockedInsertionPoint;
      if (!object.Equals((object) oldContext.LockedInsertionPoint, (object) lockedInsertionPoint))
        this.OnLockedInsertionPointChanged();
      if (this.defaultView == null || oldContext.ViewScope == newContext.ViewScope)
        return;
      this.SetDirtyViewStateBits(SceneViewModel.ViewStateBits.ActiveViewScope);
    }

    internal void NotifyInactiveEditContextChanged(EditContext oldContext, EditContext newContext)
    {
      if (oldContext.EditingContainerPath != newContext.EditingContainerPath)
        this.OnInactiveEditingContainerChanged();
      if (oldContext.ParentElement == newContext.ParentElement)
        return;
      this.OnEditContextHistoryChanged();
    }

    internal void NotifyEditContextHistoryChanged()
    {
      this.OnEditContextHistoryChanged();
    }

    internal SceneNode GetEditingContainerForSelection<T>(ICollection<T> selection)
    {
      SceneNode descendant = this.ActiveEditingContainer;
      foreach (T obj in (IEnumerable<T>) selection)
      {
        SceneNode sceneNode1 = (object) obj as SceneNode;
        if (sceneNode1 != null)
        {
          SceneNode sceneNode2 = (SceneNode) sceneNode1.StoryboardContainer;
          if (sceneNode2 != null && sceneNode2 != this.ActiveEditingContainer && (descendant == null || sceneNode2.IsAncestorOf(descendant)))
            descendant = sceneNode2;
        }
      }
      return descendant;
    }

    internal List<T> GetSelectionForEditingContainer<T>(SceneNode editingContainer, ICollection<T> selection, out List<T> removedElements) where T : SceneNode
    {
      List<T> list = new List<T>();
      removedElements = new List<T>();
      foreach (T obj in (IEnumerable<T>) selection)
      {
        SceneNode descendant = (SceneNode) obj;
        if (descendant != null)
        {
          if (descendant.IsSelectable)
          {
            if (editingContainer == descendant.StoryboardContainer)
            {
              list.Add((T) descendant);
            }
            else
            {
              removedElements.Add((T) descendant);
              if (editingContainer.IsAncestorOf(descendant))
              {
                IStoryboardContainer storyboardContainer = editingContainer as IStoryboardContainer;
                DocumentNodePath targetElementPath = storyboardContainer == null || storyboardContainer.TargetElement == null ? editingContainer.DocumentNodePath : storyboardContainer.TargetElement.DocumentNodePath;
                DocumentNodePath editingContainer1 = this.GetAncestorInEditingContainer(descendant.DocumentNodePath, editingContainer.DocumentNodePath, targetElementPath);
                if (editingContainer1 != null && this.GetSceneNode(editingContainer1.Node) is SceneElement && !list.Contains((T) descendant))
                  list.Add((T) descendant);
              }
            }
          }
          else
            removedElements.Add((T) descendant);
        }
        else
          break;
      }
      return list;
    }

    private void DocumentRoot_RootNodeChanging(object sender, EventArgs e)
    {
      this.DefaultView.AdornerLayer.ClearProxyGeometries();
      this.activeInsertionPoint = (ISceneInsertionPoint) null;
    }

    private void DocumentRoot_RootNodeChanged(object sender, EventArgs e)
    {
      this.FirePipelineUpdate(SceneUpdateTypeFlags.RootNodeChanged);
    }

    private void DocumentRoot_RootNodeChangingOutsideUndo(object sender, EventArgs e)
    {
      this.ClearSelections();
      this.EditContextManager.ClearEditContextList();
      this.activeInsertionPoint = (ISceneInsertionPoint) null;
    }

    private void DocumentRoot_RootNodeChangedOutsideUndo(object sender, EventArgs e)
    {
      this.EditContextManager.ResetEditContextListToDefault();
      this.FirePipelineUpdate(SceneUpdateTypeFlags.RootNodeChanged);
    }

    private void SceneDocument_EditTransactionCompleting(object sender, EventArgs e)
    {
      this.CanonicalizeView(SceneUpdateTypeFlags.Completing);
      this.CanonicalizeViewState(SceneUpdateTypeFlags.Completing);
      this.UpdateView(SceneUpdateTypeFlags.Completing);
      this.DefaultView.InvalidateCodeEditor();
    }

    private void SceneDocument_EditTransactionCompleted(object sender, EventArgs e)
    {
      this.ActivatePipeline(SceneUpdateTypeFlags.Completed);
    }

    private void SceneDocument_EditTransactionUpdated(object sender, EventArgs e)
    {
      this.FirePipelineUpdate(SceneUpdateTypeFlags.Updated);
    }

    private void SceneDocument_EditTransactionCanceled(object sender, EventArgs e)
    {
      this.FirePipelineUpdate(SceneUpdateTypeFlags.Canceled);
      this.DefaultView.TransactionCanceled();
    }

    private void SceneDocument_EditTransactionUndoRedo(object sender, EventArgs e)
    {
      this.FirePipelineUpdate(SceneUpdateTypeFlags.UndoRedo);
    }

    private void OnXamlDocumentParseCompleted(object sender, EventArgs e)
    {
      bool valid = this.LockedInsertionPoint == null || this.LockedInsertionPoint.SceneElement.DocumentNode.Marker != null && !this.LockedInsertionPoint.SceneElement.DocumentNode.Marker.IsDeleted;
      this.EditContextManager.SingleViewModelEditContextWalker.Walk(true, (SingleHistoryCallback) ((context, isGhosted) =>
      {
        if (context.EditingContainerPath != null && context.EditingContainerPath.IsValid())
          return false;
        valid = false;
        return true;
      }));
      if (valid)
        return;
      this.EditContextManager.ResetEditContextListToDefault();
    }

    private void FirePipelineUpdate(SceneUpdateTypeFlags flags)
    {
      this.CanonicalizeView(flags);
      this.CanonicalizeViewState(flags);
      this.ExtensibilityManager.UpdateForAddedTypes(this.damage);
      this.UpdateView(flags);
      this.ActivatePipeline(flags);
    }

    private void UpdateAdorners()
    {
      this.DefaultView.UpdateAdorners(this.damage);
    }

    private void UpdateView(SceneUpdateTypeFlags flags)
    {
      this.DefaultView.UpdateFromDamage(this.DirtyState, this.damage, flags);
    }

    private void CanonicalizeView(SceneUpdateTypeFlags flags)
    {
      if (this.animationProxyManager == null)
        return;
      this.animationProxyManager.Update(flags, this.damage, this.XamlDocument.ChangeStamp);
    }

    public void CanonicalizeViewState(SceneUpdateTypeFlags flags)
    {
      this.EditContextManager.Canonicalize(flags, this.damage);
      this.CanonicalizeActiveTrigger(flags);
      this.CanonicalizeActiveInsertionPoint(flags);
      this.elementSelectionSet.Canonicalize(flags);
      this.dependencyObjectSelectionSet.Canonicalize(flags);
      this.transitionSelectionSet.Canonicalize(flags);
      this.keyFrameSelectionSet.Canonicalize(flags);
      this.animationSelectionSet.Canonicalize(flags);
      this.storyboardSelectionSet.Canonicalize(flags);
      this.behaviorSelectionSet.Canonicalize(flags);
      this.annotationSelectionSet.Canonicalize(flags);
      this.gridColumnSelectionSet.Canonicalize(flags);
      this.gridRowSelectionSet.Canonicalize(flags);
      this.setterSelectionSet.Canonicalize(flags);
      this.childPropertySelectionSet.Canonicalize(flags);
      this.extensibilityManager.Canonicalize();
      List<SceneElement> list = new List<SceneElement>();
      if (this.elementSelectionSet.PrimarySelection != null)
        list.Add(this.elementSelectionSet.PrimarySelection);
      ISceneInsertionPoint lockedInsertionPoint = this.LockedInsertionPoint;
      if (lockedInsertionPoint != null && lockedInsertionPoint.SceneElement != null && lockedInsertionPoint.SceneElement.DocumentNode.Marker != null)
        list.Add(lockedInsertionPoint.SceneElement);
      this.EnsureDesignTimeExpansion(flags, new ReadOnlyCollection<SceneElement>((IList<SceneElement>) list));
    }

    private void CanonicalizeActiveTrigger(SceneUpdateTypeFlags flags)
    {
      if ((flags & SceneUpdateTypeFlags.Completing) == SceneUpdateTypeFlags.None || this.ActiveVisualTrigger == null || this.ActiveVisualTrigger.IsInDocument)
        return;
      this.SetActiveTrigger((TriggerBaseNode) null);
    }

    private void CanonicalizeActiveInsertionPoint(SceneUpdateTypeFlags flags)
    {
      if ((flags & SceneUpdateTypeFlags.Completing) == SceneUpdateTypeFlags.None || this.activeInsertionPoint == null || this.activeInsertionPoint.SceneElement.IsInDocument)
        return;
      this.activeInsertionPoint = (ISceneInsertionPoint) null;
    }

    private void EnsureDesignTimeExpansion(SceneUpdateTypeFlags flags, ReadOnlyCollection<SceneElement> selection)
    {
      if ((flags & SceneUpdateTypeFlags.Updated) == SceneUpdateTypeFlags.Updated)
        return;
      DocumentNodeMarkerSortedList markerList = SceneNode.GetMarkerList<SceneElement>((IEnumerable<SceneElement>) selection, true);
      if (this.ActiveEditingContainer != null)
      {
        for (DocumentNodePath documentNodePath = this.ActiveEditingContainerPath; documentNodePath != null; documentNodePath = documentNodePath.GetContainerOwnerPath())
        {
          DocumentCompositeNode documentCompositeNode = documentNodePath.ContainerOwner as DocumentCompositeNode;
          DocumentNode documentNode = documentCompositeNode != null ? documentCompositeNode.Properties[(IPropertyId) documentNodePath.ContainerOwnerProperty] : (DocumentNode) null;
          DocumentNodeMarker marker = documentNode != null ? documentNode.Marker : (DocumentNodeMarker) null;
          if (marker != null && markerList.FindPosition(marker) < 0)
            markerList.Add(marker);
        }
      }
      ISceneInsertionPoint sceneInsertionPoint = this.ActiveSceneInsertionPoint;
      if (sceneInsertionPoint != null && sceneInsertionPoint.SceneElement != null)
      {
        DocumentNodeMarker marker = sceneInsertionPoint.SceneElement.DocumentNode.Marker;
        if (marker != null && markerList.FindPosition(marker) < 0)
          markerList.Add(marker);
      }
      if ((flags & SceneUpdateTypeFlags.Completing) == SceneUpdateTypeFlags.Completing)
      {
        foreach (DocumentNodeMarkerSortedListBase.IntersectionResult intersectionResult in this.currentExpansionSet.UnionIdentity((DocumentNodeMarkerSortedListBase) markerList))
        {
          if (intersectionResult.LeftHandSideIndex == -1)
            this.UpdateAncestorsExpanded(SceneNode.FromMarker<SceneNode>(markerList.MarkerAt(intersectionResult.RightHandSideIndex), this), markerList);
          if (intersectionResult.RightHandSideIndex == -1)
            this.UpdateAncestorsExpanded(SceneNode.FromMarker<SceneNode>(this.currentExpansionSet.MarkerAt(intersectionResult.LeftHandSideIndex), this), markerList);
        }
      }
      this.currentExpansionSet = markerList;
    }

    private void UpdateAncestorsExpanded(SceneNode node, DocumentNodeMarkerSortedList selection)
    {
      if (node == null)
        return;
      int startIndex;
      int endIndex;
      bool flag = !selection.FindSelfAndDescendantRange(node.DocumentNode.Marker, out startIndex, out endIndex);
      bool descendantRange = selection.FindDescendantRange(node.DocumentNode.Marker, out startIndex, out endIndex);
      IExpandable expandable = node as IExpandable;
      PlatformTypes.UIElement.IsAssignableFrom((ITypeId) node.Type);
      if (descendantRange)
      {
        if (expandable != null)
        {
          expandable.EnsureExpandable();
          if (node.ProjectContext.ResolveProperty(expandable.DesignTimeExpansionProperty) != null)
            node.SetLocalValue(expandable.DesignTimeExpansionProperty, (object) true);
        }
        else if (ProjectNeutralTypes.TabControl.IsAssignableFrom((ITypeId) node.Type))
        {
          DocumentNodeMarkerSortedList markerList = SceneNode.GetMarkerList<SceneNode>((IEnumerable<SceneNode>) ((ItemsControlElement) node).Items, true);
          using (IEnumerator<DocumentNodeMarkerSortedListBase.IntersectionResult> enumerator = selection.Intersect((DocumentNodeMarkerSortedListBase) markerList, DocumentNodeMarkerSortedListBase.Flags.ContainedBy | DocumentNodeMarkerSortedListBase.Flags.Equals).GetEnumerator())
          {
            if (enumerator.MoveNext())
            {
              DocumentNodeMarkerSortedListBase.IntersectionResult current = enumerator.Current;
              SceneElement sceneElement = SceneNode.FromMarker<SceneElement>(markerList.MarkerAt(current.RightHandSideIndex), this);
              IProperty property = this.ProjectContext.ResolveProperty(DesignTimeProperties.DesignTimeSelectedIndexProperty);
              node.SetLocalValue((IPropertyId) property, (object) sceneElement.DocumentNode.SiteChildIndex);
            }
          }
        }
      }
      if (flag && expandable != null)
        node.ClearLocalValue(expandable.DesignTimeExpansionProperty);
      this.UpdateAncestorsExpanded(node.Parent, selection);
    }

    private void ActivatePipeline(SceneUpdateTypeFlags flags)
    {
      this.triggerSubscription.SetSceneRootNodeAsTheBasisNode(this);
      this.triggerSubscription.Update(this, this.damage, this.XamlDocument.ChangeStamp);
      this.AnimationEditor.UpdateStoryboardList(this.damage);
      if (this.damage.Count > 0 || this.dirtyState != SceneViewModel.ViewStateBits.None)
        this.SchedulePipelineTasks(false, flags == SceneUpdateTypeFlags.RootNodeChanged);
      this.UpdateAdorners();
      this.dirtyState = SceneViewModel.ViewStateBits.None;
      this.damage.Clear();
    }

    public void SchedulePipelineTasks(bool viewSwitched, bool rootNodeChanged)
    {
      PerformanceUtility.LogInfoEvent("SchedulePipelineTasks");
      SceneViewModel.earlySceneViewUpdateScheduleTask.ScheduleUpdate(this, viewSwitched, rootNodeChanged);
      SceneViewModel.lateSceneViewUpdateScheduleTask.ScheduleUpdate(this, viewSwitched, rootNodeChanged);
    }

    public void OnPipelineLatePhaseEnd()
    {
      this.dirtyPipelineState = SceneViewModel.PipelineCalcBits.None;
    }

    public void AnnotateDamage(DocumentNodeChangeList damage)
    {
      for (int index = 0; index < damage.Count; ++index)
        this.AnnotateDocumentNodeChange(damage.MarkerAt(index), damage.ValueAt(index));
    }

    private static bool IsValidTypeForAddedRemoved(ITypeId type)
    {
      if (!PlatformTypes.UIElement.IsAssignableFrom(type) && !PlatformTypes.FrameworkContentElement.IsAssignableFrom(type) && !PlatformTypes.Visual3D.IsAssignableFrom(type))
        return PlatformTypes.Model3D.IsAssignableFrom(type);
      return true;
    }

    private static bool IsSettersProperty(IPropertyId propertyKey, ITypeId type)
    {
      if (propertyKey == null)
        return false;
      if (propertyKey.Equals((object) DataTriggerProperties.SettersProperty) && PlatformTypes.DataTrigger.IsAssignableFrom(type) || propertyKey.Equals((object) MultiTriggerNode.SettersProperty) && PlatformTypes.MultiTrigger.IsAssignableFrom(type) || StyleNode.SettersProperty.Equals((object) propertyKey) && PlatformTypes.Style.IsAssignableFrom(type))
        return true;
      if (TriggerNode.SettersProperty.Equals((object) propertyKey))
        return PlatformTypes.Trigger.IsAssignableFrom(type);
      return false;
    }

    private void CheckStyleChanged(DocumentNode parentNode, IPropertyId id, List<SceneChange> list)
    {
      if (parentNode == null || !parentNode.Type.Metadata.StyleProperties.Contains(id) && !BaseFrameworkElement.OverridesDefaultStyleProperty.Equals((object) id) && !ControlElement.TemplateProperty.Equals((object) id))
        return;
      SceneElement parent = this.GetSceneNode(parentNode) as SceneElement;
      if (parent == null)
        return;
      PropertyReference propertyChanged = new PropertyReference(this.GetReferenceStep(parentNode, id));
      list.Add((SceneChange) new StyleSceneChange(parent, propertyChanged));
    }

    private void AnnotateDocumentNodeChange(DocumentNodeMarker marker, DocumentNodeChange e)
    {
      if (e.Annotation != null && ((List<SceneChange>) e.Annotation).Count > 0 && (e.ParentNode.Marker != null && !e.ParentNode.Marker.IsDeleted))
        return;
      if (e.Annotation != null)
        ((List<SceneChange>) e.Annotation).Clear();
      else
        e.Annotation = (object) new List<SceneChange>();
      if (e.IsRootNodeChange)
        return;
      List<SceneChange> list = (List<SceneChange>) e.Annotation;
      DocumentNode parentStyle = (DocumentNode) e.ParentNode.Parent;
      DocumentCompositeNode documentCompositeNode1 = e.NewChildNode as DocumentCompositeNode;
      DocumentCompositeNode documentCompositeNode2 = e.OldChildNode as DocumentCompositeNode;
      bool flag1 = false;
      bool flag2 = parentStyle != null && !PlatformTypes.SetterBaseCollection.IsAssignableFrom((ITypeId) e.ParentNode.Type) && e.ParentNode.IsProperty && parentStyle.Type.Metadata.ContentProperties.Contains((IPropertyId) e.ParentNode.SitePropertyKey);
      if (e.IsChildChange && parentStyle != null && !PlatformTypes.Style.IsAssignableFrom((ITypeId) parentStyle.Type) && (flag2 || SceneViewModel.IsValidTypeForAddedRemoved((ITypeId) e.ParentNode.Type)))
      {
        flag1 = true;
        DocumentNode node = (DocumentNode) e.ParentNode;
        if (flag2 && !SceneViewModel.IsValidTypeForAddedRemoved((ITypeId) e.ParentNode.Type))
          node = parentStyle;
        if (e.OldChildNode != null)
          list.Add((SceneChange) new ElementRemovedSceneChange(this.GetSceneNode(node), this.GetSceneNode(e.OldChildNode)));
        if (e.NewChildNode != null && !marker.IsDeleted)
          list.Add((SceneChange) new ElementAddedSceneChange(this.GetSceneNode(node), this.GetSceneNode(e.NewChildNode)));
      }
      else if (e.IsPropertyChange && (documentCompositeNode1 == null || !documentCompositeNode1.SupportsChildren || SceneViewModel.IsValidTypeForAddedRemoved((ITypeId) documentCompositeNode1.Type)) && ((documentCompositeNode2 == null || !documentCompositeNode2.SupportsChildren || SceneViewModel.IsValidTypeForAddedRemoved((ITypeId) documentCompositeNode2.Type)) && e.ParentNode.Type.Metadata.ContentProperties.Contains((IPropertyId) e.PropertyKey)))
      {
        flag1 = true;
        if (e.OldChildNode != null)
          list.Add((SceneChange) new ElementRemovedSceneChange(this.GetSceneNode((DocumentNode) e.ParentNode), this.GetSceneNode(e.OldChildNode)));
        if (e.NewChildNode != null && !marker.IsDeleted)
          list.Add((SceneChange) new ElementAddedSceneChange(this.GetSceneNode((DocumentNode) e.ParentNode), this.GetSceneNode(e.NewChildNode)));
      }
      else if (e.Action == DocumentNodeChangeAction.Add && e.ParentNode.Type.Metadata.ContentProperties.Contains((IPropertyId) e.PropertyKey) && SceneViewModel.IsValidTypeForAddedRemoved((ITypeId) e.ParentNode.Type))
      {
        foreach (DocumentNode node in e.NewChildNode.ChildNodes)
        {
          if (SceneViewModel.IsValidTypeForAddedRemoved((ITypeId) node.Type))
            list.Add((SceneChange) new ElementAddedSceneChange(this.GetSceneNode((DocumentNode) e.ParentNode), this.GetSceneNode(node)));
        }
      }
      if (!flag1 && e.IsPropertyChange)
      {
        this.CheckStyleChanged((DocumentNode) e.ParentNode, (IPropertyId) e.PropertyKey, list);
        if (e.ParentNode != null && e.ParentNode.IsInDocument)
        {
          foreach (DocumentNodeMarker documentNodeMarker in e.ParentNode.Marker.GetParentChain())
          {
            IPropertyId id = (IPropertyId) documentNodeMarker.Property;
            if (id != null && documentNodeMarker.Parent != null)
              this.CheckStyleChanged(documentNodeMarker.Parent.Node, id, list);
          }
        }
      }
      if (!flag1 && e.IsPropertyChange && e.PropertyKey.MemberType == MemberType.DesignTimeProperty)
      {
        if (DesignTimeProperties.EulerAnglesProperty.Equals((object) e.PropertyKey))
        {
          SceneNode sceneNode1 = this.GetSceneNode((DocumentNode) e.ParentNode);
          SceneNode sceneNode2 = sceneNode1;
          while (sceneNode2 != null && !(sceneNode2 is Base3DElement))
            sceneNode2 = sceneNode2.Parent;
          SceneNode parent = sceneNode2 ?? sceneNode1;
          list.Add((SceneChange) new DesignTimePropertySceneChange(parent, (IPropertyId) e.PropertyKey));
        }
        else
          list.Add((SceneChange) new DesignTimePropertySceneChange(this.GetSceneNode((DocumentNode) e.ParentNode), (IPropertyId) e.PropertyKey));
      }
      else if (!flag1 && e.IsChildChange && (e.OldChildNode != null && PlatformTypes.Setter.IsAssignableFrom((ITypeId) e.OldChildNode.Type) || e.NewChildNode != null && PlatformTypes.Setter.IsAssignableFrom((ITypeId) e.NewChildNode.Type)))
      {
        if (e.OldChildNode != null)
          this.AnnotateSetterChanged(e, e.OldChildNode, parentStyle);
        if (e.NewChildNode == null)
          return;
        this.AnnotateSetterChanged(e, e.NewChildNode, parentStyle);
      }
      else if (e.Action == DocumentNodeChangeAction.Add && SceneViewModel.IsSettersProperty((IPropertyId) e.PropertyKey, (ITypeId) e.ParentNode.Type) && PlatformTypes.SetterBaseCollection.IsAssignableFrom((ITypeId) e.NewChildNode.Type))
      {
        foreach (DocumentNode setterNode in e.NewChildNode.ChildNodes)
        {
          if (PlatformTypes.Setter.IsAssignableFrom((ITypeId) setterNode.Type))
            this.AnnotateSetterChanged(e, setterNode, (DocumentNode) e.ParentNode);
        }
      }
      else if (!flag1 && e.IsPropertyChange && PlatformTypes.Setter.IsAssignableFrom((ITypeId) e.ParentNode.Type))
      {
        if (parentStyle == null)
          return;
        this.AnnotateSetterChanged(e, (DocumentNode) e.ParentNode, (DocumentNode) parentStyle.Parent);
      }
      else
      {
        if (!this.DocumentRoot.RootNode.IsAncestorOf((DocumentNode) e.ParentNode))
          return;
        DocumentNodePath parentPath = this.defaultView.GenerateSinglePath((DocumentNode) e.ParentNode);
        if (parentPath.Node != null && this.IsExternal(parentPath.Node) || parentPath.Node.Parent != null && this.IsExternal((DocumentNode) parentPath.Node.Parent))
          return;
        List<ReferenceStep> steps = new List<ReferenceStep>();
        ReferenceStep referenceStep1 = !e.IsPropertyChange ? this.GetReferenceStep(ref parentPath, e.NewChildNode != null ? e.NewChildNode : e.OldChildNode, e.ChildIndex) : this.GetReferenceStep((DocumentNode) e.ParentNode, (IPropertyId) e.PropertyKey);
        if (referenceStep1 == null)
          return;
        steps.Add(referenceStep1);
        DocumentNodePath parent1;
        for (; !PlatformTypes.UIElement.IsAssignableFrom((ITypeId) parentPath.Node.Type) && !PlatformTypes.Style.IsAssignableFrom((ITypeId) parentPath.Node.Type) && (!PlatformTypes.Model3D.IsAssignableFrom((ITypeId) parentPath.Node.Type) && !PlatformTypes.Visual3D.IsAssignableFrom((ITypeId) parentPath.Node.Type)) && !ProjectNeutralTypes.DataGridColumn.IsAssignableFrom((ITypeId) parentPath.Node.Type); parentPath = parent1)
        {
          if (PlatformTypes.Setter.IsAssignableFrom((ITypeId) parentPath.Node.Type))
          {
            this.AnnotateSetterChanged(e, parentPath.Node, (DocumentNode) parentPath.Node.Parent);
            return;
          }
          DocumentNode node = parentPath.Node;
          DocumentCompositeNode parent2 = node.Parent;
          ReferenceStep referenceStep2 = (ReferenceStep) null;
          parent1 = parentPath.GetParent();
          if (parent2 != null)
            referenceStep2 = !node.IsProperty ? this.GetReferenceStep(ref parent1, node, parent2.Children.IndexOf(node)) : this.GetReferenceStep((DocumentNode) parent2, (IPropertyId) node.SitePropertyKey);
          if (referenceStep2 == null)
            return;
          steps.Add(referenceStep2);
        }
        steps.Reverse();
        PropertyReference propertyChanged = new PropertyReference(steps);
        list.Add((SceneChange) new PropertyReferenceSceneChange(this.GetSceneNode(parentPath == null || this.IsExternal(parentPath.Node) ? (DocumentNode) null : parentPath.Node), propertyChanged));
      }
    }

    private void AnnotateSetterChanged(DocumentNodeChange e, DocumentNode setterNode, DocumentNode parentStyle)
    {
      if (parentStyle == null)
        return;
      DocumentCompositeNode documentCompositeNode = setterNode as DocumentCompositeNode;
      List<SceneChange> list = (List<SceneChange>) e.Annotation;
      for (; !PlatformTypes.Style.IsAssignableFrom((ITypeId) parentStyle.Type) && !PlatformTypes.FrameworkTemplate.IsAssignableFrom((ITypeId) parentStyle.Type); parentStyle = (DocumentNode) parentStyle.Parent)
      {
        if (parentStyle.Parent == null)
          return;
      }
      if (parentStyle == null || this.IsExternal(parentStyle))
        return;
      if (documentCompositeNode != null)
      {
        DocumentPrimitiveNode documentPrimitiveNode = documentCompositeNode.Properties[SetterSceneNode.PropertyProperty] as DocumentPrimitiveNode;
        if (documentPrimitiveNode != null)
        {
          ReferenceStep singleStep = DocumentPrimitiveNode.GetValueAsMember((DocumentNode) documentPrimitiveNode) as ReferenceStep;
          if (singleStep != null)
            list.Add((SceneChange) new PropertyReferenceSceneChange(this.GetSceneNode(parentStyle), new PropertyReference(singleStep)));
        }
      }
      if (e.PropertyKey != SetterSceneNode.PropertyProperty)
        return;
      DocumentPrimitiveNode documentPrimitiveNode1 = e.OldChildNode as DocumentPrimitiveNode;
      if (documentPrimitiveNode1 == null)
        return;
      ReferenceStep singleStep1 = DocumentPrimitiveNode.GetValueAsMember((DocumentNode) documentPrimitiveNode1) as ReferenceStep;
      if (singleStep1 == null)
        return;
      list.Add((SceneChange) new PropertyReferenceSceneChange(this.GetSceneNode(parentStyle), new PropertyReference(singleStep1)));
    }

    private ReferenceStep GetReferenceStep(ref DocumentNodePath parentPath, DocumentNode child, int childIndex)
    {
      DocumentCompositeNode documentCompositeNode = parentPath.Node as DocumentCompositeNode;
      ReferenceStep referenceStep1 = (ReferenceStep) null;
      if (typeof (Setter).IsAssignableFrom(child.TargetType) && typeof (Trigger).IsAssignableFrom(documentCompositeNode.TargetType))
      {
        SetterSceneNode setterSceneNode = (SetterSceneNode) this.GetSceneNode(child);
        if (setterSceneNode != null)
        {
          DocumentNodePath parent = parentPath.GetParent();
          while (parent != null && PlatformTypes.Style.IsAssignableFrom((ITypeId) parent.Node.Type))
            parent = parent.GetParent();
          if (parent != null)
          {
            StyleNode styleNode = (StyleNode) this.GetSceneNode(parent.Node);
            if (styleNode != null)
            {
              SceneNode sceneNode = ((ITriggerContainer) styleNode).ResolveTargetName(setterSceneNode.Target);
              ReferenceStep referenceStep2 = (ReferenceStep) setterSceneNode.Property;
              if (sceneNode != null && referenceStep2 != null)
              {
                parentPath = sceneNode.DocumentNodePath;
                referenceStep1 = referenceStep2;
              }
            }
          }
        }
      }
      else
        referenceStep1 = this.GetReferenceStep((DocumentNode) documentCompositeNode, childIndex);
      return referenceStep1;
    }

    private ReferenceStep GetReferenceStep(DocumentNode parent, int childIndex)
    {
      return (ReferenceStep) IndexedClrPropertyReferenceStep.GetReferenceStep(parent.TypeResolver, parent.TargetType, childIndex, false);
    }

    private ReferenceStep GetReferenceStep(DocumentNode parent, IPropertyId propertyKey)
    {
      return propertyKey as ReferenceStep;
    }

    private void SetDirtyViewStateBits(SceneViewModel.ViewStateBits bit)
    {
      ++this.changeStamp;
      this.dirtyState |= bit;
    }

    public IDisposable ScopeViewObjectCache()
    {
      if (!this.InViewObjectCacheScope)
        this.viewObjectCachedNodes = new List<SceneNode>(this.elementSelectionSet.Count * 2);
      ++this.viewObjectCacheScopeCount;
      return (IDisposable) new SceneViewModel.ScopeViewObjectCacheToken(this);
    }

    public void AddViewObjectCachedNode(SceneNode node)
    {
      if (!this.InViewObjectCacheScope)
        throw new InvalidOperationException();
      this.viewObjectCachedNodes.Add(node);
    }

    private void EndScopeViewObjectCache()
    {
      --this.viewObjectCacheScopeCount;
      if (this.viewObjectCacheScopeCount != 0)
        return;
      for (int index = 0; index < this.viewObjectCachedNodes.Count; ++index)
        this.viewObjectCachedNodes[index].FlushViewObjectCache();
      this.viewObjectCachedNodes = (List<SceneNode>) null;
    }

    [System.Flags]
    public enum ViewStateBits
    {
      None = 0,
      IsEditable = 1,
      ActiveTrigger = 2,
      ActiveEditingContainer = 4,
      InactiveEditingContainer = 8,
      EditContextHistory = 16,
      ActiveViewScope = 32,
      ActiveTimeline = 64,
      ElementSelection = 128,
      TextSelection = 256,
      PathPartSelection = 512,
      KeyFrameSelection = 1024,
      AnimationSelection = 2048,
      StoryboardSelection = 4096,
      GridColumnSelection = 8192,
      GridRowSelection = 16384,
      PropertySelection = 32768,
      SetterSelection = 65536,
      ActiveSceneInsertionPoint = 131072,
      LockedInsertionPoint = 262144,
      TriggersChanged = 524288,
      RecordMode = 1048576,
      CurrentValues = 2097152,
      ChildPropertySelection = 4194304,
      BehaviorSelection = 8388608,
      ActiveStateContext = 16777216,
      AnnotationSelection = 33554432,
      ActivePinnedStates = 67108864,
      ActiveTransitionContext = 134217728,
      DependencyObjectSelection = 268435456,
      EntireScene = 2147483647,
    }

    [System.Flags]
    public enum PipelineCalcBits
    {
      None = 0,
      TimelineSortZOrder = 1,
      TimelineLastAnimationTime = 2,
      TimelineScopedTimelineItem = 4,
      AnimationMoveKeyFrame = 8,
      AnimationAddKeyFrame = 16,
      AnimationChangeKeyEase = 32,
      AnimationGeneralChange = 64,
      EntireScene = 16777215,
    }

    [System.Flags]
    public enum CanonicalizationFlags
    {
      None = 0,
      DescendantChanged = 1,
      ThisInserted = 2,
    }

    private abstract class FieldCountToken : IDisposable
    {
      private bool disposed;

      protected abstract int Value { get; }

      protected void Init()
      {
        this.AddToValue(1);
      }

      protected abstract void AddToValue(int increment);

      public void Dispose()
      {
        if (!this.disposed)
        {
          this.AddToValue(-1);
          this.disposed = true;
        }
        GC.SuppressFinalize((object) this);
      }
    }

    private sealed class ForceBaseValueToken : SceneViewModel.FieldCountToken, IDisposable
    {
      private SceneViewModel sceneViewModel;

      protected override int Value
      {
        get
        {
          return this.sceneViewModel.forcingBaseValueCount;
        }
      }

      public ForceBaseValueToken(SceneViewModel sceneViewModel)
      {
        this.sceneViewModel = sceneViewModel;
        this.Init();
      }

      protected override void AddToValue(int increment)
      {
        this.sceneViewModel.forcingBaseValueCount += increment;
      }
    }

    private sealed class ForceDefaultSetValueToken : SceneViewModel.FieldCountToken, IDisposable
    {
      private SceneViewModel sceneViewModel;

      protected override int Value
      {
        get
        {
          return this.sceneViewModel.forcingDefaultSetValueCount;
        }
      }

      public ForceDefaultSetValueToken(SceneViewModel sceneViewModel)
      {
        this.sceneViewModel = sceneViewModel;
        this.Init();
      }

      protected override void AddToValue(int increment)
      {
        this.sceneViewModel.forcingDefaultSetValueCount += increment;
      }
    }

    private sealed class ForceAlternateSiteNodeToken : IDisposable
    {
      private SceneViewModel sceneViewModel;
      private DocumentNode previousAlternateSiteNode;
      private bool isDisposed;

      public ForceAlternateSiteNodeToken(SceneViewModel sceneViewModel, DocumentNode node)
      {
        this.sceneViewModel = sceneViewModel;
        this.previousAlternateSiteNode = this.sceneViewModel.alternateSiteNode;
        this.sceneViewModel.alternateSiteNode = node;
      }

      ~ForceAlternateSiteNodeToken()
      {
        this.Dispose(false);
      }

      public void Dispose()
      {
        this.Dispose(true);
        GC.SuppressFinalize((object) this);
      }

      private void Dispose(bool isDisposing)
      {
        if (this.isDisposed || !isDisposing)
          return;
        this.sceneViewModel.alternateSiteNode = this.previousAlternateSiteNode;
        this.isDisposed = true;
      }
    }

    private sealed class ForceUseShadowPropertiesToken : SceneViewModel.FieldCountToken, IDisposable
    {
      private SceneViewModel sceneViewModel;

      protected override int Value
      {
        get
        {
          return this.sceneViewModel.forcingUseShadowPropertiesCount;
        }
      }

      public ForceUseShadowPropertiesToken(SceneViewModel sceneViewModel)
      {
        this.sceneViewModel = sceneViewModel;
        this.Init();
      }

      protected override void AddToValue(int increment)
      {
        this.sceneViewModel.forcingUseShadowPropertiesCount += increment;
      }
    }

    private sealed class DisableDrawIntoStateToken : SceneViewModel.FieldCountToken, IDisposable
    {
      private SceneViewModel sceneViewModel;

      protected override int Value
      {
        get
        {
          return this.sceneViewModel.disablingDrawIntoStateCount;
        }
      }

      public DisableDrawIntoStateToken(SceneViewModel sceneViewModel)
      {
        this.sceneViewModel = sceneViewModel;
        this.Init();
      }

      protected override void AddToValue(int increment)
      {
        this.sceneViewModel.disablingDrawIntoStateCount += increment;
      }
    }

    private sealed class DisableUpdateChildrenOnAddAndRemoveToken : SceneViewModel.FieldCountToken, IDisposable
    {
      private SceneViewModel sceneViewModel;

      protected override int Value
      {
        get
        {
          return this.sceneViewModel.disableUpdateChildrenOnAddAndRemoveCount;
        }
      }

      public DisableUpdateChildrenOnAddAndRemoveToken(SceneViewModel sceneViewModel)
      {
        this.sceneViewModel = sceneViewModel;
        this.Init();
      }

      protected override void AddToValue(int increment)
      {
        this.sceneViewModel.disableUpdateChildrenOnAddAndRemoveCount += increment;
      }
    }

    private class EnforceGridDesignModeToken : SceneViewModel.FieldCountToken, IDisposable
    {
      private SceneViewModel sceneViewModel;

      protected override int Value
      {
        get
        {
          return this.sceneViewModel.enforceGridDesignModeCount;
        }
      }

      public EnforceGridDesignModeToken(SceneViewModel sceneViewModel)
      {
        this.sceneViewModel = sceneViewModel;
        this.Init();
      }

      protected override void AddToValue(int increment)
      {
        this.sceneViewModel.enforceGridDesignModeCount += increment;
      }
    }

    private sealed class ScopeViewObjectCacheToken : IDisposable
    {
      private SceneViewModel owner;

      public ScopeViewObjectCacheToken(SceneViewModel viewModel)
      {
        this.owner = viewModel;
      }

      public void Dispose()
      {
        this.owner.EndScopeViewObjectCache();
      }
    }
  }
}
