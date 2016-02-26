// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.SkinEditing.StateModelManager
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.DesignSurface.ViewModel.SceneNodeSearch;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Microsoft.Expression.DesignSurface.UserInterface.SkinEditing
{
  public class StateModelManager : ModelBase
  {
    private ObservableCollection<StateGroupModel> groupModels = new ObservableCollection<StateGroupModel>();
    private Dictionary<string, string> statesFromAttributes = new Dictionary<string, string>();
    private Dictionary<TimelineSceneNode.PropertyNodePair, Dictionary<StateGroupModel, int>> conflictsCollection = new Dictionary<TimelineSceneNode.PropertyNodePair, Dictionary<StateGroupModel, int>>();
    private Dictionary<TimelineSceneNode.PropertyNodePair, bool> candidateConflictAnimations = new Dictionary<TimelineSceneNode.PropertyNodePair, bool>();
    private HashSet<VisualStateTransitionSceneNode> dirtyTransitions = new HashSet<VisualStateTransitionSceneNode>();
    private DesignerContext designerContext;
    private bool isStructureEditable;
    private SceneNodeSubscription<VisualStateGroupSceneNode, StateGroupModel> visualStateGroupsSubscription;
    private SceneNodeSubscription<VisualStateSceneNode, StateModel> visualStatesSubscription;
    private SceneNodeSubscription<VisualStateTransitionSceneNode, TransitionModel> transitionsSubscription;
    private SceneNodeSubscription<TimelineSceneNode, TimelineReferenceModel> timelineSceneNodeSubscription;
    private VisualStateSceneNode lastStateAdded;
    private VisualStateGroupSceneNode lastStateGroupAdded;
    private StateModel selectedState;
    private BaseStateModel baseState;
    private TransitionModel selectedTransition;
    private DelegateCommand addStateGroupCommand;
    private DelegateCommand beginEditTransitionDurationCommand;
    private DelegateCommand commitEditCommand;
    private DelegateCommand cancelEditCommand;
    private SceneEditTransaction openEditTransaction;
    private bool shouldPreviewTransitions;
    private VisualStateSceneNode stateAnimatingTo;
    private TransitionModel pendingTransitionModel;
    private int postNotifyHandlerCount;
    private static Timer fallbackTimer;

    public SceneViewModel ActiveSceneViewModel
    {
      get
      {
        SceneViewModel activeSceneViewModel = this.designerContext.ActiveSceneViewModel;
        if (activeSceneViewModel != null && activeSceneViewModel.IsEditable && activeSceneViewModel.DefaultView.IsDesignSurfaceEnabled)
          return activeSceneViewModel;
        return (SceneViewModel) null;
      }
    }

    public ObservableCollection<StateGroupModel> GroupModels
    {
      get
      {
        return this.groupModels;
      }
    }

    public BaseStateModel BaseState
    {
      get
      {
        return this.baseState;
      }
    }

    public SceneNode TargetNode
    {
      get
      {
        SceneViewModel activeSceneViewModel = this.ActiveSceneViewModel;
        if (activeSceneViewModel != null && activeSceneViewModel.ProjectContext.IsCapabilitySet(PlatformCapability.SupportsVisualStateManager))
          return VisualStateManagerSceneNode.GetHostNode(activeSceneViewModel.ActiveEditingContainer);
        return (SceneNode) null;
      }
    }

    public bool IsStructureEditable
    {
      get
      {
        return this.isStructureEditable;
      }
      set
      {
        if (this.isStructureEditable == value)
          return;
        this.isStructureEditable = value;
        this.NotifyPropertyChanged("IsStructureEditable");
        this.NotifyChildrenIsStructureEditableChanged();
      }
    }

    public bool CanEditStates
    {
      get
      {
        SceneViewModel activeSceneViewModel = this.ActiveSceneViewModel;
        if (activeSceneViewModel != null && activeSceneViewModel.IsEditable && (activeSceneViewModel.ProjectContext.IsCapabilitySet(PlatformCapability.SupportsVisualStateManager) && VisualStateManagerSceneNode.CanSupportVisualStateManager(activeSceneViewModel.ActiveEditingContainer)))
          return this.TargetNode != null;
        return false;
      }
    }

    public bool ShouldPreviewTransitions
    {
      get
      {
        return this.shouldPreviewTransitions;
      }
      set
      {
        if (this.shouldPreviewTransitions == value)
          return;
        this.shouldPreviewTransitions = value;
        this.NotifyPropertyChanged("ShouldPreviewTransitions");
      }
    }

    public VisualStateSceneNode LastStateAdded
    {
      get
      {
        return this.lastStateAdded;
      }
      set
      {
        this.lastStateAdded = value;
      }
    }

    public VisualStateGroupSceneNode LastStateGroupAdded
    {
      get
      {
        return this.lastStateGroupAdded;
      }
      set
      {
        this.lastStateGroupAdded = value;
      }
    }

    public ICommand AddStateGroupCommand
    {
      get
      {
        if (this.addStateGroupCommand == null)
          this.addStateGroupCommand = new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.AddStateGroup));
        return (ICommand) this.addStateGroupCommand;
      }
    }

    public ICommand BeginEditTransitionDurationCommand
    {
      get
      {
        if (this.beginEditTransitionDurationCommand == null)
          this.beginEditTransitionDurationCommand = new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.BeginEditTransitionDuration));
        return (ICommand) this.beginEditTransitionDurationCommand;
      }
    }

    public ICommand CommitOpenEditTransactionCommand
    {
      get
      {
        if (this.commitEditCommand == null)
          this.commitEditCommand = new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.CommitOpenEditTransaction));
        return (ICommand) this.commitEditCommand;
      }
    }

    public ICommand CancelOpenEditTransactionCommand
    {
      get
      {
        if (this.cancelEditCommand == null)
          this.cancelEditCommand = new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.CancelOpenEditTransaction));
        return (ICommand) this.cancelEditCommand;
      }
    }

    public bool IsEditTransactionOpen
    {
      get
      {
        return this.openEditTransaction != null;
      }
    }

    private bool IsSimulatingStateTransition
    {
      get
      {
        return this.stateAnimatingTo != null;
      }
    }

    internal StateModelManager(DesignerContext designerContext)
    {
      this.designerContext = designerContext;
      this.baseState = new BaseStateModel();
      this.baseState.ParentModel = (ModelBase) this;
      this.baseState.IsSelected = true;
    }

    public void Attach()
    {
      if (this.visualStateGroupsSubscription != null || this.visualStatesSubscription != null || (this.transitionsSubscription != null || this.timelineSceneNodeSubscription != null))
        this.Detach();
      this.designerContext.SelectionManager.LateActiveSceneUpdatePhase += new SceneUpdatePhaseEventHandler(this.SelectionManager_LateActiveSceneUpdatePhase);
      this.designerContext.SelectionManager.ActiveSceneSwitched += new EventHandler(this.SelectionManager_ActiveSceneSwitched);
      this.visualStateGroupsSubscription = new SceneNodeSubscription<VisualStateGroupSceneNode, StateGroupModel>();
      SearchStep searchStep1 = new SearchStep(new SearchAxis(VisualStateManagerSceneNode.VisualStateGroupsProperty));
      SearchStep searchStep2 = new SearchStep(SearchAxis.DocumentChild, (ISearchPredicate) new DelegatePredicate((Predicate<SceneNode>) (node =>
      {
        VisualStateGroupSceneNode stateGroupSceneNode = node as VisualStateGroupSceneNode;
        if (stateGroupSceneNode != null)
          return !stateGroupSceneNode.IsSketchFlowAnimation;
        return false;
      }), SearchScope.NodeTreeSelf));
      this.visualStateGroupsSubscription.Path = new SearchPath(new SearchStep[2]
      {
        searchStep1,
        searchStep2
      });
      this.visualStateGroupsSubscription.SetPathNodeInsertedHandler(new SceneNodeSubscription<VisualStateGroupSceneNode, StateGroupModel>.PathNodeInsertedHandler(this.VisualStateGroupsSubscription_NodeInserted));
      this.visualStateGroupsSubscription.PathNodeRemoved += new SceneNodeSubscription<VisualStateGroupSceneNode, StateGroupModel>.PathNodeRemovedListener(this.VisualStateGroupsSubscription_NodeRemoved);
      this.visualStateGroupsSubscription.PathNodeContentChanged += new SceneNodeSubscription<VisualStateGroupSceneNode, StateGroupModel>.PathNodeContentChangedListener(this.VisualStateGroupsSubscription_NodeContentChanged);
      this.visualStatesSubscription = new SceneNodeSubscription<VisualStateSceneNode, StateModel>();
      this.visualStatesSubscription.Path = new SearchPath(new SearchStep[3]
      {
        searchStep1,
        searchStep2,
        new SearchStep(SearchAxis.DocumentDescendant, (ISearchPredicate) new SceneNodeTypePredicate(typeof (VisualStateSceneNode)))
      });
      this.visualStatesSubscription.SetPathNodeInsertedHandler(new SceneNodeSubscription<VisualStateSceneNode, StateModel>.PathNodeInsertedHandler(this.VisualStatesSubscription_NodeInserted));
      this.visualStatesSubscription.PathNodeRemoved += new SceneNodeSubscription<VisualStateSceneNode, StateModel>.PathNodeRemovedListener(this.VisualStatesSubscription_NodeRemoved);
      this.visualStatesSubscription.PathNodeContentChanged += new SceneNodeSubscription<VisualStateSceneNode, StateModel>.PathNodeContentChangedListener(this.VisualStatesSubscription_NodeContentChanged);
      this.transitionsSubscription = new SceneNodeSubscription<VisualStateTransitionSceneNode, TransitionModel>();
      this.transitionsSubscription.Path = new SearchPath(new SearchStep[3]
      {
        searchStep1,
        searchStep2,
        new SearchStep(SearchAxis.DocumentDescendant, (ISearchPredicate) new SceneNodeTypePredicate(typeof (VisualStateTransitionSceneNode)))
      });
      this.transitionsSubscription.SetPathNodeInsertedHandler(new SceneNodeSubscription<VisualStateTransitionSceneNode, TransitionModel>.PathNodeInsertedHandler(this.TransitionsSubscription_NodeInserted));
      this.transitionsSubscription.PathNodeRemoved += new SceneNodeSubscription<VisualStateTransitionSceneNode, TransitionModel>.PathNodeRemovedListener(this.TransitionsSubscription_NodeRemoved);
      this.transitionsSubscription.PathNodeContentChanged += new SceneNodeSubscription<VisualStateTransitionSceneNode, TransitionModel>.PathNodeContentChangedListener(this.TransitionsSubscription_NodeContentChanged);
      this.timelineSceneNodeSubscription = new SceneNodeSubscription<TimelineSceneNode, TimelineReferenceModel>();
      this.timelineSceneNodeSubscription.Path = new SearchPath(new SearchStep[3]
      {
        searchStep1,
        searchStep2,
        new SearchStep(SearchAxis.DocumentDescendant, (ISearchPredicate) new SceneNodeTypePredicate(typeof (TimelineSceneNode)))
      });
      this.timelineSceneNodeSubscription.SetPathNodeInsertedHandler(new SceneNodeSubscription<TimelineSceneNode, TimelineReferenceModel>.PathNodeInsertedHandler(this.TimelineSceneNodeSubscription_NodeInserted));
      this.timelineSceneNodeSubscription.PathNodeRemoved += new SceneNodeSubscription<TimelineSceneNode, TimelineReferenceModel>.PathNodeRemovedListener(this.TimelineSceneNodeSubscription_NodeRemoved);
      this.timelineSceneNodeSubscription.PathNodeContentChanged += new SceneNodeSubscription<TimelineSceneNode, TimelineReferenceModel>.PathNodeContentChangedListener(this.TimelineSceneNodeSubscription_NodeContentChanged);
      this.UpdateBasisNode();
      this.AddMissingStates();
      this.NotifyPropertyChanged("CanEditStates");
    }

    public void Detach()
    {
      if (this.designerContext.SelectionManager != null)
      {
        this.designerContext.SelectionManager.LateActiveSceneUpdatePhase -= new SceneUpdatePhaseEventHandler(this.SelectionManager_LateActiveSceneUpdatePhase);
        this.designerContext.SelectionManager.ActiveSceneSwitched -= new EventHandler(this.SelectionManager_ActiveSceneSwitched);
      }
      if (this.timelineSceneNodeSubscription != null)
      {
        using (StateGroupModel.UpdateConflicts((IEnumerable<StateGroupModel>) this.groupModels, this.candidateConflictAnimations, this.conflictsCollection))
          this.timelineSceneNodeSubscription.CurrentViewModel = (SceneViewModel) null;
        this.timelineSceneNodeSubscription.SetPathNodeInsertedHandler((SceneNodeSubscription<TimelineSceneNode, TimelineReferenceModel>.PathNodeInsertedHandler) null);
        this.timelineSceneNodeSubscription.PathNodeRemoved -= new SceneNodeSubscription<TimelineSceneNode, TimelineReferenceModel>.PathNodeRemovedListener(this.TimelineSceneNodeSubscription_NodeRemoved);
        this.timelineSceneNodeSubscription.PathNodeContentChanged -= new SceneNodeSubscription<TimelineSceneNode, TimelineReferenceModel>.PathNodeContentChangedListener(this.TimelineSceneNodeSubscription_NodeContentChanged);
        this.timelineSceneNodeSubscription = (SceneNodeSubscription<TimelineSceneNode, TimelineReferenceModel>) null;
      }
      if (this.visualStateGroupsSubscription != null)
      {
        this.visualStateGroupsSubscription.CurrentViewModel = (SceneViewModel) null;
        this.visualStateGroupsSubscription.SetPathNodeInsertedHandler((SceneNodeSubscription<VisualStateGroupSceneNode, StateGroupModel>.PathNodeInsertedHandler) null);
        this.visualStateGroupsSubscription.PathNodeRemoved -= new SceneNodeSubscription<VisualStateGroupSceneNode, StateGroupModel>.PathNodeRemovedListener(this.VisualStateGroupsSubscription_NodeRemoved);
        this.visualStateGroupsSubscription.PathNodeContentChanged -= new SceneNodeSubscription<VisualStateGroupSceneNode, StateGroupModel>.PathNodeContentChangedListener(this.VisualStateGroupsSubscription_NodeContentChanged);
        this.visualStateGroupsSubscription.CurrentViewModel = (SceneViewModel) null;
      }
      if (this.visualStatesSubscription != null)
      {
        this.visualStatesSubscription.CurrentViewModel = (SceneViewModel) null;
        this.visualStatesSubscription.SetPathNodeInsertedHandler((SceneNodeSubscription<VisualStateSceneNode, StateModel>.PathNodeInsertedHandler) null);
        this.visualStatesSubscription.PathNodeRemoved -= new SceneNodeSubscription<VisualStateSceneNode, StateModel>.PathNodeRemovedListener(this.VisualStatesSubscription_NodeRemoved);
        this.visualStatesSubscription.PathNodeContentChanged -= new SceneNodeSubscription<VisualStateSceneNode, StateModel>.PathNodeContentChangedListener(this.VisualStatesSubscription_NodeContentChanged);
        this.visualStatesSubscription = (SceneNodeSubscription<VisualStateSceneNode, StateModel>) null;
      }
      if (this.transitionsSubscription != null)
      {
        this.transitionsSubscription.CurrentViewModel = (SceneViewModel) null;
        this.transitionsSubscription.SetPathNodeInsertedHandler((SceneNodeSubscription<VisualStateTransitionSceneNode, TransitionModel>.PathNodeInsertedHandler) null);
        this.transitionsSubscription.PathNodeRemoved -= new SceneNodeSubscription<VisualStateTransitionSceneNode, TransitionModel>.PathNodeRemovedListener(this.TransitionsSubscription_NodeRemoved);
        this.transitionsSubscription.PathNodeContentChanged -= new SceneNodeSubscription<VisualStateTransitionSceneNode, TransitionModel>.PathNodeContentChangedListener(this.TransitionsSubscription_NodeContentChanged);
        this.transitionsSubscription = (SceneNodeSubscription<VisualStateTransitionSceneNode, TransitionModel>) null;
      }
      this.NotifyPropertyChanged("CanEditStates");
      this.selectedState = (StateModel) null;
      this.selectedTransition = (TransitionModel) null;
    }

    private void UpdateBasisNode()
    {
      SceneViewModel activeSceneViewModel = this.ActiveSceneViewModel;
      List<SceneNode> list = new List<SceneNode>(1);
      this.RecacheStatesAttributesFromMetadata();
      if (activeSceneViewModel != null && activeSceneViewModel.ActiveEditingContainer != null && this.TargetNode != null)
      {
        list.Add(this.TargetNode);
        this.IsStructureEditable = true;
      }
      else
        this.IsStructureEditable = false;
      this.visualStateGroupsSubscription.SetBasisNodes(activeSceneViewModel, (IEnumerable<SceneNode>) list);
      this.visualStatesSubscription.SetBasisNodes(activeSceneViewModel, (IEnumerable<SceneNode>) list);
      this.transitionsSubscription.SetBasisNodes(activeSceneViewModel, (IEnumerable<SceneNode>) list);
      using (StateGroupModel.UpdateConflicts((IEnumerable<StateGroupModel>) this.groupModels, this.candidateConflictAnimations, this.conflictsCollection))
        this.timelineSceneNodeSubscription.SetBasisNodes(activeSceneViewModel, (IEnumerable<SceneNode>) list);
      this.UpdateModelSelection();
      this.UpdateModelPinning();
      this.UpdateModelTransitionDurations();
      this.NotifyPropertyChanged("CanEditStates");
    }

    private void AddMissingStates()
    {
      if (this.ActiveSceneViewModel == null || this.TargetNode == null)
        return;
      using (SceneEditTransaction editTransaction = this.ActiveSceneViewModel.CreateEditTransaction("Adding hidden states", true, SceneEditTransactionType.NestedInAutoClosing))
      {
        foreach (KeyValuePair<string, string> keyValuePair in this.statesFromAttributes)
        {
          VisualStateGroupSceneNode stateGroupSceneNode = VisualStateManagerSceneNode.FindGroupByName(this.TargetNode, keyValuePair.Value);
          if (stateGroupSceneNode == null)
          {
            VisualStateManagerSceneNode.EnsureNameAvailable(this.TargetNode, keyValuePair.Value);
            stateGroupSceneNode = VisualStateManagerSceneNode.AddStateGroup(this.TargetNode, this.TargetNode, keyValuePair.Value);
            if (stateGroupSceneNode != null)
              stateGroupSceneNode.ShouldSerialize = false;
          }
          if (stateGroupSceneNode != null && stateGroupSceneNode.FindStateByName(keyValuePair.Key) == null)
          {
            VisualStateManagerSceneNode.EnsureNameAvailable(this.TargetNode, keyValuePair.Key);
            VisualStateSceneNode visualStateSceneNode = stateGroupSceneNode.AddState(this.TargetNode, keyValuePair.Key);
            if (visualStateSceneNode.Name != keyValuePair.Key)
            {
              editTransaction.Cancel();
              return;
            }
            visualStateSceneNode.ShouldSerialize = false;
          }
        }
        editTransaction.Commit();
      }
    }

    private void RecacheStatesAttributesFromMetadata()
    {
      this.statesFromAttributes.Clear();
      if (this.ActiveSceneViewModel == null || this.TargetNode == null)
        return;
      ControlTemplateElement controlTemplateElement = this.ActiveSceneViewModel.ActiveEditingContainer as ControlTemplateElement;
      if (controlTemplateElement == null)
        return;
      foreach (DefaultStateRecord defaultStateRecord in ProjectAttributeHelper.GetDefaultStateRecords(this.TargetNode.ViewModel.ProjectContext.ResolveType(controlTemplateElement.ControlTemplateTargetTypeId), (ITypeResolver) ProjectContext.GetProjectContext(this.TargetNode.ViewModel.ProjectContext)))
      {
        if (!this.statesFromAttributes.ContainsKey(defaultStateRecord.StateName))
          this.statesFromAttributes.Add(defaultStateRecord.StateName, defaultStateRecord.GroupName);
      }
    }

    public bool IsStateGroupFromMetadata(string stateGroupName)
    {
      return this.statesFromAttributes != null && this.statesFromAttributes.ContainsValue(stateGroupName);
    }

    public bool IsStateFromMetadata(string stateName, string stateGroupName)
    {
      return this.statesFromAttributes != null && this.statesFromAttributes.ContainsKey(stateName) && this.statesFromAttributes[stateName].Equals(stateGroupName, StringComparison.Ordinal);
    }

    private void SelectionManager_ActiveSceneSwitched(object sender, EventArgs e)
    {
      this.UpdateBasisNode();
    }

    private void SelectionManager_LateActiveSceneUpdatePhase(object sender, SceneUpdatePhaseEventArgs args)
    {
      if (args.IsRadicalChange || args.IsDirtyViewState(SceneViewModel.ViewStateBits.IsEditable) || args.IsDirtyViewState(SceneViewModel.ViewStateBits.ActiveEditingContainer) || (this.visualStateGroupsSubscription.BasisNodeCount == 0 && this.TargetNode != null || this.visualStateGroupsSubscription.BasisNodeCount == 1 && this.TargetNode == null) || this.visualStateGroupsSubscription.BasisNodeCount == 1 && this.TargetNode != this.visualStateGroupsSubscription.BasisNodeAt(0).Node)
      {
        if (args.IsDirtyViewState(SceneViewModel.ViewStateBits.ActiveEditingContainer))
          this.UpdateModelForSelectedStateAndTransition((StateModel) null, (TransitionModel) null);
        this.UpdateBasisNode();
        this.NotifyPropertyChanged("CanEditStates");
      }
      this.visualStateGroupsSubscription.Update(args.ViewModel, args.DocumentChanges, args.DocumentChangeStamp);
      this.visualStatesSubscription.Update(args.ViewModel, args.DocumentChanges, args.DocumentChangeStamp);
      this.transitionsSubscription.Update(args.ViewModel, args.DocumentChanges, args.DocumentChangeStamp);
      using (StateGroupModel.UpdateConflicts((IEnumerable<StateGroupModel>) this.groupModels, this.candidateConflictAnimations, this.conflictsCollection))
        this.timelineSceneNodeSubscription.Update(args.ViewModel, args.DocumentChanges, args.DocumentChangeStamp);
      this.UpdateModelTransitionDurations();
      if (args.IsDirtyViewState(SceneViewModel.ViewStateBits.ActiveStateContext) || args.IsDirtyViewState(SceneViewModel.ViewStateBits.ActiveTransitionContext))
        this.UpdateModelSelection();
      if (args.IsDirtyViewState(SceneViewModel.ViewStateBits.ActivePinnedStates))
        this.UpdateModelPinning();
      if (args.IsDirtyViewState(SceneViewModel.ViewStateBits.RecordMode) && this.selectedState != null)
        this.selectedState.NotifyIsRecordingChanged();
      UIThreadDispatcher.Instance.BeginInvoke(DispatcherPriority.Normal, (Action) (() => this.AddMissingStates()));
    }

    private void UpdateModelSelection()
    {
      if (this.ActiveSceneViewModel != null)
      {
        StateModel newState = (StateModel) null;
        VisualStateTransitionSceneNode transitionSceneNode = this.selectedTransition != null ? this.selectedTransition.TransitionSceneNode : (VisualStateTransitionSceneNode) null;
        VisualStateTransitionSceneNode transitionEditTarget = this.ActiveSceneViewModel.TransitionEditTarget;
        TransitionModel newTransition = transitionEditTarget == transitionSceneNode ? this.selectedTransition : this.FindTransitionModelForNode(transitionEditTarget);
        VisualStateSceneNode visualStateSceneNode = this.selectedState != null ? this.selectedState.SceneNode : (VisualStateSceneNode) null;
        VisualStateSceneNode stateEditTarget = this.ActiveSceneViewModel.StateEditTarget;
        if (stateEditTarget != visualStateSceneNode)
        {
          if (stateEditTarget != null && stateEditTarget.IsAttached && !stateEditTarget.StateGroup.IsSketchFlowAnimation)
            newState = this.FindStateModelForNode(stateEditTarget);
        }
        else
          newState = this.selectedState;
        this.UpdateModelForSelectedStateAndTransition(newState, newTransition);
        if (stateEditTarget == null || !stateEditTarget.IsAttached || (stateEditTarget.StateGroup == null || !stateEditTarget.StateGroup.IsSketchFlowAnimation))
          return;
        this.BaseState.IsSelected = false;
      }
      else
        this.UpdateModelForSelectedStateAndTransition((StateModel) null, (TransitionModel) null);
    }

    private void UpdateModelPinning()
    {
      if (this.ActiveSceneViewModel == null)
        return;
      IList<VisualStateSceneNode> pinnedStates = this.ActiveSceneViewModel.PinnedStates;
      using (IEnumerator<StateGroupModel> enumerator = this.GroupModels.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          StateGroupModel groupModel = enumerator.Current;
          VisualStateSceneNode stateNode = (VisualStateSceneNode) null;
          if (pinnedStates != null)
            stateNode = Enumerable.FirstOrDefault<VisualStateSceneNode>((IEnumerable<VisualStateSceneNode>) pinnedStates, (Func<VisualStateSceneNode, bool>) (state => state.StateGroup == groupModel.SceneNode));
          if (stateNode != null)
          {
            StateModel stateModelForNode = this.FindStateModelForNode(stateNode);
            this.UpdateModelForStatePin(groupModel, stateModelForNode, true);
          }
          else
            this.UpdateModelForStatePin(groupModel, (StateModel) null, false);
        }
      }
    }

    private void UpdateModelTransitionDurations()
    {
      if (this.ActiveSceneViewModel == null)
        return;
      foreach (VisualStateTransitionSceneNode transitionNode in this.dirtyTransitions)
      {
        if (transitionNode.DocumentNode.Marker != null && this.ActiveSceneViewModel != null && transitionNode.DocumentNode.Marker.DocumentContext == this.ActiveSceneViewModel.Document.DocumentContext)
        {
          TransitionModel transitionModelForNode = this.FindTransitionModelForNode(transitionNode);
          if (transitionModelForNode != null)
            transitionModelForNode.UpdateDuration();
        }
      }
      this.dirtyTransitions.Clear();
    }

    private StateGroupModel VisualStateGroupsSubscription_NodeInserted(object sender, SceneNode basisNode, object basisContent, SceneNode newPathNode)
    {
      VisualStateGroupSceneNode groupNode = newPathNode as VisualStateGroupSceneNode;
      if (groupNode != null)
        return this.AddStateGroupModel(groupNode);
      return (StateGroupModel) null;
    }

    private void VisualStateGroupsSubscription_NodeRemoved(object sender, SceneNode basisNode, object basisContent, SceneNode oldPathNode, StateGroupModel oldContent)
    {
      if (oldContent == null)
        return;
      this.RemoveStateGroupModel(oldContent);
    }

    private void VisualStateGroupsSubscription_NodeContentChanged(object sender, SceneNode pathNode, StateGroupModel content, DocumentNodeMarker damageMarker, DocumentNodeChange damage)
    {
      if (damage.IsPropertyChange && damage.ParentNode == pathNode.DocumentNode && damage.PropertyKey.Name == "UseFluidLayout")
        this.ActiveSceneViewModel.RefreshSelection();
      if (content == null)
        return;
      content.OnContentChanged();
    }

    public StateGroupModel AddStateGroupModel(VisualStateGroupSceneNode groupNode)
    {
      StateGroupModel stateGroupModel = new StateGroupModel(groupNode);
      stateGroupModel.ParentModel = (ModelBase) this;
      this.groupModels.Insert(ModelBase.GetInsertionIndex<StateGroupModel>(stateGroupModel, (Collection<StateGroupModel>) this.GroupModels, new List<string>((IEnumerable<string>) ModelBase.stateAndStateGroupsPredefinedOrder.Keys), this.TargetNode), stateGroupModel);
      if (groupNode == this.LastStateGroupAdded)
      {
        this.LastStateGroupAdded = (VisualStateGroupSceneNode) null;
        stateGroupModel.IsEditingName = true;
      }
      return stateGroupModel;
    }

    public void RemoveStateGroupModel(StateGroupModel model)
    {
      this.groupModels.Remove(model);
    }

    private StateModel VisualStatesSubscription_NodeInserted(object sender, SceneNode basisNode, object basisContent, SceneNode newPathNode)
    {
      VisualStateSceneNode node = newPathNode as VisualStateSceneNode;
      if (node == null || node.StateGroup == null)
        return (StateModel) null;
      StateModel stateModel = new StateModel(node);
      this.AddStateModel(stateModel);
      return stateModel;
    }

    private void VisualStatesSubscription_NodeRemoved(object sender, SceneNode basisNode, object basisContent, SceneNode oldPathNode, StateModel oldContent)
    {
      if (oldContent == null)
        return;
      this.RemoveStateModel(oldContent);
    }

    private void VisualStatesSubscription_NodeContentChanged(object sender, SceneNode pathNode, StateModel content, DocumentNodeMarker damageMarker, DocumentNodeChange damage)
    {
      if (content == null)
        return;
      content.OnContentChanged((IPropertyId) damage.PropertyKey);
    }

    private void AddStateModel(StateModel stateModel)
    {
      StateGroupModel groupModelForNode = this.GetStateGroupModelForNode(stateModel.SceneNode.StateGroup);
      if (groupModelForNode == null)
        return;
      groupModelForNode.AddStateModel(stateModel);
      if (stateModel.SceneNode != this.LastStateAdded)
        return;
      this.LastStateAdded = (VisualStateSceneNode) null;
      UIThreadDispatcher.Instance.BeginInvoke(DispatcherPriority.Normal, (Action) (() =>
      {
        this.SelectState(stateModel.GroupModel, stateModel);
        stateModel.IsEditingName = true;
      }));
    }

    private void RemoveStateModel(StateModel model)
    {
      StateGroupModel groupModel = model.GroupModel;
      if (groupModel == null)
        return;
      groupModel.RemoveStateModel(model);
    }

    private TimelineReferenceModel TimelineSceneNodeSubscription_NodeInserted(object sender, SceneNode basisNode, TimelineSceneNode basisContent, SceneNode newPathNode)
    {
      TimelineSceneNode basisContent1 = newPathNode as TimelineSceneNode;
      if (basisContent1 != null)
      {
        StateGroupModel stateGroupForNode = this.FindStateGroupForNode(basisContent1);
        if (stateGroupForNode != null && basisContent1.TargetElementAndProperty.SceneNode != null && basisContent1.TargetElementAndProperty.PropertyReference != null)
        {
          SceneNode stateOrTransition = (SceneNode) basisContent1.ControllingState ?? (SceneNode) basisContent1.ControllingTransition;
          TimelineReferenceModel timelineReferenceModel = new TimelineReferenceModel(stateGroupForNode, stateOrTransition, basisContent1.TargetElementAndProperty);
          this.UpdateConflictsOnAdd(timelineReferenceModel);
          this.DirtyTransitionDurationsIfNeeded(timelineReferenceModel);
          return timelineReferenceModel;
        }
      }
      return (TimelineReferenceModel) null;
    }

    private void TimelineSceneNodeSubscription_NodeRemoved(object sender, SceneNode basisNode, TimelineSceneNode basisContent, SceneNode oldPathNode, TimelineReferenceModel oldContent)
    {
      TimelineReferenceModel timelineReferenceModel = oldContent;
      if (timelineReferenceModel == null)
        return;
      this.UpdateConflictsOnRemove(timelineReferenceModel);
      this.DirtyTransitionDurationsIfNeeded(timelineReferenceModel);
    }

    private void TimelineSceneNodeSubscription_NodeContentChanged(object sender, SceneNode pathNode, TimelineReferenceModel content, DocumentNodeMarker damageMarker, DocumentNodeChange damage)
    {
      this.DirtyTransitionDurationsIfNeeded(content);
    }

    private TransitionModel TransitionsSubscription_NodeInserted(object sender, SceneNode basisNode, object basisContent, SceneNode newPathNode)
    {
      VisualStateTransitionSceneNode sceneNode = newPathNode as VisualStateTransitionSceneNode;
      if (sceneNode == null)
        return (TransitionModel) null;
      StateGroupModel groupModelForNode = this.GetStateGroupModelForNode(sceneNode.StateGroup);
      if (groupModelForNode.DefaultTransition.TransitionSceneNode == sceneNode)
        return groupModelForNode.DefaultTransition;
      TransitionModel model = new TransitionModel(sceneNode, groupModelForNode);
      this.AddTransitionModel(model);
      return model;
    }

    private void TransitionsSubscription_NodeRemoved(object sender, SceneNode basisNode, object basisContent, SceneNode oldPathNode, TransitionModel oldContent)
    {
      if (oldContent == null)
        return;
      this.RemoveTransitionModel(oldContent);
    }

    private void TransitionsSubscription_NodeContentChanged(object sender, SceneNode pathNode, TransitionModel content, DocumentNodeMarker damageMarker, DocumentNodeChange damage)
    {
      if (content == null)
        return;
      IPropertyId propertyKey = (IPropertyId) damageMarker.Property;
      if (VisualStateTransitionSceneNode.FromStateNameProperty.Equals((object) propertyKey) || VisualStateTransitionSceneNode.ToStateNameProperty.Equals((object) propertyKey))
      {
        StateModel stateModel = content.StateModel;
        StateGroupModel stateGroupModel = (StateGroupModel) null;
        if (stateModel != null)
        {
          if (!stateModel.DoesTransitionMatch(content))
          {
            stateModel.RemoveTransitionModel(content);
            content.GroupModel.AddTransitionModel(content);
          }
        }
        else
        {
          stateGroupModel = this.GetStateGroupModelForNode(content.GroupNode);
          if (stateGroupModel != null)
            stateGroupModel.AddTransitionModel(content);
        }
        if (stateGroupModel != null)
          stateGroupModel.OnContentChanged();
      }
      else if (VisualStateTransitionSceneNode.GeneratedDurationProperty.Equals((object) propertyKey))
        this.dirtyTransitions.Add(content.TransitionSceneNode);
      content.OnContentChanged(propertyKey);
    }

    private void AddTransitionModel(TransitionModel model)
    {
      if (model.GroupModel == null)
        return;
      model.GroupModel.AddTransitionModel(model);
    }

    private void RemoveTransitionModel(TransitionModel model)
    {
      model.GroupModel.RemoveTransitionModel(model);
    }

    private void DirtyTransitionDurationsIfNeeded(TimelineReferenceModel timelineModel)
    {
      if (timelineModel == null)
        return;
      VisualStateTransitionSceneNode transitionSceneNode1 = timelineModel.StateOrTransition as VisualStateTransitionSceneNode;
      VisualStateSceneNode visualStateSceneNode = timelineModel.StateOrTransition as VisualStateSceneNode;
      if (transitionSceneNode1 != null)
      {
        this.dirtyTransitions.Add(transitionSceneNode1);
      }
      else
      {
        if (visualStateSceneNode == null)
          return;
        foreach (VisualStateTransitionSceneNode transitionSceneNode2 in (IEnumerable<VisualStateTransitionSceneNode>) timelineModel.Group.SceneNode.Transitions)
        {
          if (transitionSceneNode2.FromState == visualStateSceneNode || transitionSceneNode2.ToState == visualStateSceneNode)
            this.dirtyTransitions.Add(transitionSceneNode2);
        }
      }
    }

    private StateGroupModel GetStateGroupModelForNode(VisualStateGroupSceneNode sceneNode)
    {
      foreach (StateGroupModel stateGroupModel in (Collection<StateGroupModel>) this.groupModels)
      {
        if (stateGroupModel.SceneNode == sceneNode)
          return stateGroupModel;
      }
      return (StateGroupModel) null;
    }

    private void UpdateModelForSelectedStateAndTransition(StateModel newState, TransitionModel newTransition)
    {
      if (this.selectedState != null)
      {
        this.selectedState.GroupModel.SelectedState = (StateModel) null;
        this.selectedState = (StateModel) null;
      }
      if (this.selectedTransition != null)
      {
        this.selectedTransition.IsSelected = false;
        this.selectedTransition = (TransitionModel) null;
      }
      if (newTransition != null)
      {
        this.selectedTransition = newTransition;
        this.selectedTransition.IsSelected = true;
        this.baseState.IsSelected = false;
      }
      else if (newState != null)
      {
        this.baseState.IsSelected = false;
        this.selectedState = newState;
        this.selectedState.GroupModel.SelectedState = newState;
      }
      else
      {
        foreach (StateGroupModel groupModel in (Collection<StateGroupModel>) this.GroupModels)
        {
          if (groupModel.PinnedState != null)
            this.UpdateModelForStatePin(groupModel, groupModel.PinnedState, false);
        }
        this.baseState.IsSelected = true;
        this.selectedState = (StateModel) null;
      }
    }

    private void UpdateModelForStatePin(StateGroupModel groupModel, StateModel state, bool newPin)
    {
      if (newPin)
        groupModel.PinnedState = state;
      else
        groupModel.PinnedState = (StateModel) null;
    }

    public void SelectState(StateGroupModel groupModel, StateModel newState)
    {
      if (this.ActiveSceneViewModel != null)
      {
        using (SceneEditTransaction editTransaction = this.ActiveSceneViewModel.CreateEditTransaction(StringTable.ActivateStateUndoUnit, true))
        {
          this.ActiveSceneViewModel.DefaultView.TryExitTextEditMode();
          if (groupModel != null && groupModel.PinnedState != null && groupModel.PinnedState != newState)
            this.UnpinState(groupModel, groupModel.PinnedState);
          VisualStateSceneNode stateAnimatingFrom = this.selectedState != null ? this.selectedState.SceneNode : (VisualStateSceneNode) null;
          bool flag1 = this.selectedState != null;
          bool flag2 = flag1 && newState != null && this.selectedState.GroupNode != newState.GroupNode;
          this.UpdateModelForSelectedStateAndTransition(newState, (TransitionModel) null);
          if (this.ActiveSceneViewModel.StoryboardSelectionSet.Count > 0)
            this.ActiveSceneViewModel.StoryboardSelectionSet.Clear();
          VisualStateSceneNode currentStateNode = newState != null ? newState.SceneNode : (VisualStateSceneNode) null;
          if (this.shouldPreviewTransitions)
          {
            if (flag1 && !flag2 && (currentStateNode != null && stateAnimatingFrom != currentStateNode))
              this.SimulateStateTransition(stateAnimatingFrom, currentStateNode);
            else if (stateAnimatingFrom != currentStateNode || currentStateNode == null)
            {
              if (StateModelManager.fallbackTimer != null)
                StateModelManager.fallbackTimer.Stop();
              this.ActiveSceneViewModel.DefaultView.StopAllStateTransitions();
              this.ActiveSceneViewModel.DefaultView.DeferViewStoryboard = false;
              if (currentStateNode != null)
              {
                UIThreadDispatcher.Instance.BeginInvoke(DispatcherPriority.Background, (Action) (() =>
                {
                  if (this.ActiveSceneViewModel == null)
                    return;
                  this.ActiveSceneViewModel.ActivateState(currentStateNode);
                }));
              }
              else
              {
                if (this.stateAnimatingTo != null)
                {
                  this.stateAnimatingTo.ViewModel.DefaultView.OverrideAdornerLayerVisibility = new Visibility?();
                  this.stateAnimatingTo = (VisualStateSceneNode) null;
                  --this.postNotifyHandlerCount;
                  InputManager.Current.PostNotifyInput -= new NotifyInputEventHandler(this.Current_PostNotifyInput);
                }
                this.ActiveSceneViewModel.ActivateState((VisualStateSceneNode) null);
              }
            }
          }
          else
            this.ActiveSceneViewModel.ActivateState(currentStateNode);
          if (newState != null)
            newState.NotifyIsRecordingChanged();
          editTransaction.Commit();
        }
      }
      else
      {
        if (newState != null)
          return;
        this.selectedState = (StateModel) null;
      }
    }

    private VisualStateTransitionSceneNode GetTransition(VisualStateSceneNode stateAnimatingFrom, VisualStateSceneNode stateAnimatingTo)
    {
      VisualStateGroupSceneNode stateGroupSceneNode = stateAnimatingFrom.Parent as VisualStateGroupSceneNode;
      VisualStateTransitionSceneNode transitionSceneNode1 = (VisualStateTransitionSceneNode) null;
      int num1 = -1;
      foreach (VisualStateTransitionSceneNode transitionSceneNode2 in (IEnumerable<VisualStateTransitionSceneNode>) stateGroupSceneNode.Transitions)
      {
        int num2 = 0;
        if (transitionSceneNode2.FromState == stateAnimatingFrom)
          ++num2;
        else if (transitionSceneNode2.FromState != null)
          continue;
        if (transitionSceneNode2.ToState == stateAnimatingTo)
          num2 += 2;
        else if (transitionSceneNode2.ToState != null)
          continue;
        if (num2 > num1)
        {
          num1 = num2;
          transitionSceneNode1 = transitionSceneNode2;
        }
      }
      return transitionSceneNode1;
    }

    private void SimulateStateTransition(VisualStateSceneNode stateAnimatingFrom, VisualStateSceneNode stateAnimatingTo)
    {
      VisualStateTransitionSceneNode transition = this.GetTransition(stateAnimatingFrom, stateAnimatingTo);
      TransitionModel transitionModel = transition == null ? (TransitionModel) null : this.FindTransitionModelForNode(transition);
      if (StateModelManager.fallbackTimer != null)
      {
        StateModelManager.fallbackTimer.Stop();
      }
      else
      {
        StateModelManager.fallbackTimer = new Timer();
        StateModelManager.fallbackTimer.Elapsed += new ElapsedEventHandler(this.timer_Elapsed);
      }
      StateModelManager.fallbackTimer.Interval = (transitionModel == null ? 0.0 : transitionModel.Duration) * 1000.0 + 100.0;
      bool simulatingStateTransition = this.IsSimulatingStateTransition;
      this.stateAnimatingTo = stateAnimatingTo;
      if (!simulatingStateTransition)
      {
        this.ActiveSceneViewModel.AnimationEditor.Invalidate();
        this.ActiveSceneViewModel.DefaultView.OverrideAdornerLayerVisibility = new Visibility?(Visibility.Collapsed);
        this.ActiveSceneViewModel.DefaultView.DeferViewStoryboard = true;
        this.ActiveSceneViewModel.DefaultView.SimulateGoToState(stateAnimatingFrom, false, (TransitionCompletedCallback) null);
        UIThreadDispatcher.Instance.BeginInvoke(DispatcherPriority.Background, (Action) (() =>
        {
          StateModelManager.fallbackTimer.Start();
          ++this.postNotifyHandlerCount;
          InputManager.Current.PostNotifyInput += new NotifyInputEventHandler(this.Current_PostNotifyInput);
          this.ActiveSceneViewModel.DefaultView.SimulateGoToState(stateAnimatingTo, true, (TransitionCompletedCallback) null);
        }));
      }
      else
      {
        StateModelManager.fallbackTimer.Start();
        this.ActiveSceneViewModel.DefaultView.SimulateGoToState(stateAnimatingTo, true, (TransitionCompletedCallback) null);
      }
    }

    private void timer_Elapsed(object sender, ElapsedEventArgs e)
    {
      StateModelManager.fallbackTimer.Stop();
      UIThreadDispatcher.Instance.BeginInvoke(DispatcherPriority.Normal, new Action(this.OnSimulationCompleted));
    }

    private void CancelTransitionPreview()
    {
      UIThreadDispatcher.Instance.BeginInvoke(DispatcherPriority.Background, (Action) (() => this.StopSimulatingStateTransition()));
    }

    private void StopSimulatingStateTransition()
    {
      if (StateModelManager.fallbackTimer != null)
        StateModelManager.fallbackTimer.Stop();
      if (this.stateAnimatingTo == null)
        return;
      if (this.stateAnimatingTo.ViewModel != null && this.stateAnimatingTo.ViewModel.DefaultView != null)
        this.stateAnimatingTo.ViewModel.DefaultView.StopAllStateTransitions();
      UIThreadDispatcher.Instance.BeginInvoke(DispatcherPriority.Normal, (Action) (() =>
      {
        if (this.stateAnimatingTo == null)
          return;
        if (this.stateAnimatingTo.ViewModel != null && this.stateAnimatingTo.ViewModel.DefaultView != null)
        {
          this.stateAnimatingTo.ViewModel.DefaultView.OverrideAdornerLayerVisibility = new Visibility?();
          this.stateAnimatingTo.ViewModel.DefaultView.DeferViewStoryboard = false;
          if (this.pendingTransitionModel != null)
          {
            this.stateAnimatingTo = (VisualStateSceneNode) null;
            this.SelectTransition(this.pendingTransitionModel);
            this.pendingTransitionModel = (TransitionModel) null;
          }
          else
            this.stateAnimatingTo.ViewModel.ActivateState(this.stateAnimatingTo);
        }
        this.stateAnimatingTo = (VisualStateSceneNode) null;
        --this.postNotifyHandlerCount;
        InputManager.Current.PostNotifyInput -= new NotifyInputEventHandler(this.Current_PostNotifyInput);
      }));
    }

    private void OnSimulationCompleted()
    {
      using (Microsoft.Expression.DesignModel.Core.DispatcherHelper.DeferDoEvents())
        UIThreadDispatcher.Instance.BeginInvoke(DispatcherPriority.Background, (Action) (() => this.StopSimulatingStateTransition()));
    }

    private void Current_PostNotifyInput(object sender, NotifyInputEventArgs e)
    {
      if (this.postNotifyHandlerCount <= 0 || e.StagingItem == null)
        return;
      MouseButtonEventArgs mouseButtonEventArgs = e.StagingItem.Input as MouseButtonEventArgs;
      KeyEventArgs keyEventArgs = e.StagingItem.Input as KeyEventArgs;
      KeyboardFocusChangedEventArgs changedEventArgs = e.StagingItem.Input as KeyboardFocusChangedEventArgs;
      if (mouseButtonEventArgs != null)
      {
        FrameworkElement frameworkElement1 = mouseButtonEventArgs.OriginalSource as FrameworkElement;
        if (mouseButtonEventArgs.ButtonState != MouseButtonState.Pressed || frameworkElement1 == null)
          return;
        StateModel stateModel = frameworkElement1.DataContext as StateModel;
        FrameworkElement frameworkElement2;
        while (true)
        {
          frameworkElement2 = VisualTreeHelper.GetParent((DependencyObject) frameworkElement1) as FrameworkElement;
          if (frameworkElement2 != null && frameworkElement2.DataContext == stateModel)
          {
            if (!(frameworkElement2 is Control))
              frameworkElement1 = frameworkElement2;
            else
              break;
          }
          else
            goto label_10;
        }
        ClickControl clickControl = frameworkElement2 as ClickControl;
        if (clickControl == null || clickControl.LeftClickCommand != stateModel.SelectCommand)
          stateModel = (StateModel) null;
label_10:
        if (stateModel != null && this.stateAnimatingTo != null && (!Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.RightShift)) && stateModel.GroupModel == this.FindStateModelForNode(this.stateAnimatingTo).GroupModel)
          return;
        this.CancelTransitionPreview();
      }
      else if (keyEventArgs != null && keyEventArgs.IsDown)
      {
        this.CancelTransitionPreview();
      }
      else
      {
        if (changedEventArgs == null)
          return;
        FrameworkElement frameworkElement = changedEventArgs.NewFocus as FrameworkElement;
        if (frameworkElement == null)
          return;
        StateModel stateModel = frameworkElement.DataContext as StateModel;
        if (stateModel != null && this.stateAnimatingTo != null && stateModel.GroupModel == this.FindStateModelForNode(this.stateAnimatingTo).GroupModel)
          return;
        this.CancelTransitionPreview();
      }
    }

    public void SelectBaseState()
    {
      if (this.ActiveSceneViewModel == null)
        return;
      using (SceneEditTransaction editTransaction = this.ActiveSceneViewModel.CreateEditTransaction(StringTable.ActivateStateUndoUnit, true))
      {
        this.ActiveSceneViewModel.UnpinAllStates();
        this.SelectState((StateGroupModel) null, (StateModel) null);
        editTransaction.Commit();
      }
    }

    public void SelectTransition(TransitionModel transitionModel)
    {
      if (this.stateAnimatingTo != null)
      {
        this.pendingTransitionModel = transitionModel;
      }
      else
      {
        if (this.ActiveSceneViewModel == null)
          return;
        this.UpdateModelForSelectedStateAndTransition((StateModel) null, transitionModel);
        this.ActiveSceneViewModel.ActivateTransition(transitionModel != null ? transitionModel.TransitionSceneNode : (VisualStateTransitionSceneNode) null);
      }
    }

    public void PinState(StateGroupModel groupModel, StateModel stateToPin)
    {
      if (this.ActiveSceneViewModel == null)
        return;
      using (SceneEditTransaction editTransaction = this.ActiveSceneViewModel.CreateEditTransaction(StringTable.PinStateUndoUnit))
      {
        this.UpdateModelForStatePin(groupModel, stateToPin, true);
        if (this.selectedState != null && this.selectedState.GroupModel == groupModel)
        {
          bool flag = this.shouldPreviewTransitions;
          this.shouldPreviewTransitions = false;
          this.SelectState(groupModel, stateToPin);
          this.shouldPreviewTransitions = flag;
        }
        VisualStateSceneNode state = stateToPin != null ? stateToPin.SceneNode : (VisualStateSceneNode) null;
        if (state != null)
          this.ActiveSceneViewModel.PinState(state);
        editTransaction.Commit();
      }
    }

    public void UnpinState(StateGroupModel groupModel, StateModel stateToUnpin)
    {
      if (this.ActiveSceneViewModel == null)
        return;
      using (SceneEditTransaction editTransaction = this.ActiveSceneViewModel.CreateEditTransaction(StringTable.UnpinStateUndoUnit))
      {
        this.UpdateModelForStatePin(groupModel, stateToUnpin, false);
        VisualStateSceneNode state = stateToUnpin != null ? stateToUnpin.SceneNode : (VisualStateSceneNode) null;
        if (state != null && state.IsInDocument)
          this.ActiveSceneViewModel.UnpinState(state);
        editTransaction.Commit();
      }
    }

    private void AddStateGroup()
    {
      SceneNode targetNode = this.TargetNode;
      if (targetNode == null || this.ActiveSceneViewModel == null)
        return;
      using (SceneEditTransaction editTransaction = this.ActiveSceneViewModel.CreateEditTransaction(StringTable.AddNewStateGroupUndoUnit))
      {
        VisualStateGroupSceneNode stateGroupSceneNode = VisualStateManagerSceneNode.AddStateGroup(targetNode, targetNode.ViewModel.ActiveEditingContainer, ProjectNeutralTypes.VisualStateGroup.Name);
        if (stateGroupSceneNode == null)
        {
          editTransaction.Cancel();
        }
        else
        {
          editTransaction.Update();
          if (this.ActiveSceneViewModel != null)
            VisualStateManagerSceneNode.UpdateHasExtendedVisualStateManager(this.ActiveSceneViewModel.ActiveEditingContainer);
          editTransaction.Commit();
          this.LastStateGroupAdded = stateGroupSceneNode;
        }
      }
    }

    private void BeginEditTransitionDuration()
    {
      this.CreateNewEditTransaction(StringTable.EditTransitionDurationUndoUnit);
    }

    private void CreateNewEditTransaction(string description)
    {
      if (this.openEditTransaction != null || this.ActiveSceneViewModel == null)
        return;
      this.openEditTransaction = this.ActiveSceneViewModel.CreateEditTransaction(description);
    }

    private void CommitOpenEditTransaction()
    {
      if (this.openEditTransaction == null)
        return;
      this.openEditTransaction.Commit();
      this.openEditTransaction.Dispose();
      this.openEditTransaction = (SceneEditTransaction) null;
    }

    private void CancelOpenEditTransaction()
    {
      if (this.openEditTransaction == null)
        return;
      this.openEditTransaction.Cancel();
      this.openEditTransaction.Dispose();
      this.openEditTransaction = (SceneEditTransaction) null;
    }

    private void NotifyChildrenIsStructureEditableChanged()
    {
      foreach (StateGroupModel stateGroupModel in (Collection<StateGroupModel>) this.GroupModels)
        stateGroupModel.NotifyIsStructureEditableChanged();
    }

    private TransitionModel FindTransitionModelForNode(VisualStateTransitionSceneNode transitionNode)
    {
      if (transitionNode == null)
        return (TransitionModel) null;
      return this.transitionsSubscription.FindPathNode((SceneNode) transitionNode).Info;
    }

    private StateModel FindStateModelForNode(VisualStateSceneNode stateNode)
    {
      if (stateNode == null)
        return (StateModel) null;
      return this.visualStatesSubscription.FindPathNode((SceneNode) stateNode).Info;
    }

    private StateGroupModel FindStateGroupForNode(TimelineSceneNode basisContent)
    {
      if (!basisContent.CanAccessProperties)
        return (StateGroupModel) null;
      VisualStateGroupSceneNode sceneNode = (VisualStateGroupSceneNode) null;
      for (SceneNode parent = basisContent.Parent; parent != null && sceneNode == null; parent = parent.Parent)
        sceneNode = parent as VisualStateGroupSceneNode;
      return this.GetStateGroupModelForNode(sceneNode);
    }

    private void UpdateConflictsOnAdd(TimelineReferenceModel timelineReferenceModel)
    {
      Dictionary<StateGroupModel, int> dictionary1;
      if (!this.conflictsCollection.TryGetValue(timelineReferenceModel.TargetElementAndProperty, out dictionary1))
      {
        dictionary1 = new Dictionary<StateGroupModel, int>();
        this.conflictsCollection.Add(timelineReferenceModel.TargetElementAndProperty, dictionary1);
      }
      if (!dictionary1.ContainsKey(timelineReferenceModel.Group))
      {
        dictionary1.Add(timelineReferenceModel.Group, 0);
        if (dictionary1.Count > 1)
          this.candidateConflictAnimations[timelineReferenceModel.TargetElementAndProperty] = true;
      }
      Dictionary<StateGroupModel, int> dictionary2;
      StateGroupModel group;
      (dictionary2 = dictionary1)[group = timelineReferenceModel.Group] = dictionary2[group] + 1;
    }

    private void UpdateConflictsOnRemove(TimelineReferenceModel timelineReferenceModel)
    {
      Dictionary<StateGroupModel, int> dictionary1 = this.conflictsCollection[timelineReferenceModel.TargetElementAndProperty];
      StateGroupModel group = timelineReferenceModel.Group;
      Dictionary<StateGroupModel, int> dictionary2;
      StateGroupModel index;
      if (((dictionary2 = dictionary1)[index = group] = dictionary2[index] - 1) <= 0)
      {
        if (dictionary1.Count > 1)
          this.candidateConflictAnimations[timelineReferenceModel.TargetElementAndProperty] = true;
        dictionary1.Remove(group);
      }
      if (dictionary1.Count != 0)
        return;
      this.conflictsCollection.Remove(timelineReferenceModel.TargetElementAndProperty);
    }
  }
}
