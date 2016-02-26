// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.SkinEditing.StateGroupModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Data;
using Microsoft.Expression.Project;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.UserInterface.SkinEditing
{
  public class StateGroupModel : ModelBase
  {
    private ObservableCollection<StateModel> states = new ObservableCollection<StateModel>();
    private ObservableCollection<ConflictModel> conflicts = new ObservableCollection<ConflictModel>();
    private List<TransitionModel> parkedTransitions = new List<TransitionModel>();
    private bool isExpanded = true;
    private VisualStateGroupSceneNode sceneNode;
    private ICommand deleteStateGroupCommand;
    private ICommand addStateCommand;
    private StateModel selectedState;
    private StateModel pinnedState;

    public override string Name
    {
      get
      {
        if (!string.IsNullOrEmpty(this.sceneNode.Name))
          return this.sceneNode.Name;
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, StringTable.ElementTimelineItemBracketedName, new object[1]
        {
          (object) this.sceneNode.Type.Name
        });
      }
      set
      {
        if (!(this.sceneNode.Name != value) || string.IsNullOrEmpty(value))
          return;
        using (SceneEditTransaction editTransaction = this.sceneNode.ViewModel.CreateEditTransaction(StringTable.RenameStateGroupUndoUnit))
        {
          this.sceneNode.Name = new SceneNodeIDHelper(this.ActiveSceneViewModel, this.ActiveSceneViewModel.ActiveEditingContainer).GetValidElementID((Microsoft.Expression.DesignSurface.ViewModel.SceneNode) this.sceneNode, value);
          editTransaction.Commit();
        }
        this.NotifyPropertyChanged("Name");
      }
    }

    public bool IsEditingName { get; set; }

    public TransitionModel DefaultTransition { get; set; }

    public VisualStateGroupSceneNode SceneNode
    {
      get
      {
        return this.sceneNode;
      }
    }

    public ObservableCollection<StateModel> StateModels
    {
      get
      {
        return this.states;
      }
    }

    public ObservableCollection<ConflictModel> Conflicts
    {
      get
      {
        return this.conflicts;
      }
    }

    public bool IsUserDefined
    {
      get
      {
        if (this.ModelManager != null)
          return !this.ModelManager.IsStateGroupFromMetadata(this.Name);
        return false;
      }
    }

    public double DefaultDuration
    {
      get
      {
        if (this.DefaultTransition == null)
          return 0.0;
        return this.DefaultTransition.Duration;
      }
      set
      {
        this.sceneNode.DefaultTransitionDuration = new Duration(TimeSpan.FromSeconds(value));
        this.NotifyPropertyChanged("DefaultDuration");
      }
    }

    public bool FluidLayoutEnabled { get; private set; }

    public bool UseFluidLayout
    {
      get
      {
        if (this.ActiveSceneViewModel == null)
          return false;
        IProperty property = this.ActiveSceneViewModel.ProjectContext.ResolveProperty(VisualStateManagerSceneNode.UseFluidLayoutProperty);
        if (property == null)
          return false;
        return (bool) this.SceneNode.GetLocalOrDefaultValue((IPropertyId) property);
      }
      set
      {
        using (SceneEditTransaction editTransaction = this.sceneNode.ViewModel.CreateEditTransaction(StringTable.ToggleFluidLayoutUndoUnit))
        {
          if (value && !VisualStateManagerSceneNode.GetHasExtendedVisualStateManager(this.ModelManager.TargetNode))
            VisualStateManagerSceneNode.SetHasExtendedVisualStateManager(this.ModelManager.TargetNode, true);
          IProperty property = this.ActiveSceneViewModel.ProjectContext.ResolveProperty(VisualStateManagerSceneNode.UseFluidLayoutProperty);
          if (property != null)
          {
            if (value)
            {
              this.SceneNode.SetValue((IPropertyId) property, (object) (bool) (value ? true : false));
              VisualStateTransitionSceneNode transitionEditTarget = this.ActiveSceneViewModel.EditContextManager.TransitionEditTarget;
              if (transitionEditTarget != null && transitionEditTarget.StateGroup == this.SceneNode)
                this.ModelManager.SelectTransition((TransitionModel) null);
              foreach (StateModel stateModel in (Collection<StateModel>) this.StateModels)
              {
                if (stateModel.SceneNode.Storyboard != null)
                  this.ActiveSceneViewModel.AnimationEditor.EnsureNonAnimatablePropertiesAreObjectAnimations(stateModel.SceneNode.Storyboard);
              }
            }
            else
              this.SceneNode.ClearValue((IPropertyId) property);
            editTransaction.Update();
            VisualStateManagerSceneNode.UpdateHasExtendedVisualStateManager(this.ModelManager.TargetNode);
          }
          editTransaction.Commit();
          this.NotifyPropertyChanged("UseFluidLayout");
        }
      }
    }

    public StateModel SelectedState
    {
      get
      {
        return this.selectedState;
      }
      set
      {
        if (this.selectedState == value)
          return;
        if (this.selectedState != null)
          this.selectedState.IsSelected = false;
        this.selectedState = value;
        if (this.selectedState != null)
          this.selectedState.IsSelected = true;
        foreach (StateModel stateModel in (Collection<StateModel>) this.StateModels)
          stateModel.NotifySelectedStateChanged();
        this.NotifyPropertyChanged("SelectedState");
      }
    }

    public bool IsExpanded
    {
      get
      {
        return this.isExpanded;
      }
      set
      {
        if (this.isExpanded == value)
          return;
        this.isExpanded = value;
        this.NotifyPropertyChanged("IsExpanded");
      }
    }

    public ICommand ToggleExpandCommand
    {
      get
      {
        return (ICommand) new DelegateCommand((DelegateCommand.SimpleEventHandler) (() => this.IsExpanded = !this.IsExpanded));
      }
    }

    public StateModel PinnedState
    {
      get
      {
        return this.pinnedState;
      }
      set
      {
        if (this.pinnedState == value)
          return;
        StateModel stateModel = this.pinnedState;
        this.pinnedState = value;
        if (stateModel != null)
          stateModel.OnIsPinnedChanged();
        if (this.pinnedState != null)
          this.pinnedState.OnIsPinnedChanged();
        this.NotifyPropertyChanged("PinnedState");
      }
    }

    public bool IsStructureEditable
    {
      get
      {
        if (this.ModelManager.IsStructureEditable)
          return this.IsUserDefined;
        return false;
      }
    }

    public StateModelManager ModelManager
    {
      get
      {
        return this.ParentModel as StateModelManager;
      }
    }

    public ICommand DeleteCommand
    {
      get
      {
        if (this.deleteStateGroupCommand == null)
          this.deleteStateGroupCommand = (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.DeleteStateGroup));
        return this.deleteStateGroupCommand;
      }
    }

    public ICommand AddStateCommand
    {
      get
      {
        if (this.addStateCommand == null)
          this.addStateCommand = (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.AddState));
        return this.addStateCommand;
      }
    }

    public ObservableCollection<StateModel> States
    {
      get
      {
        return this.states;
      }
    }

    public SceneViewModel ActiveSceneViewModel
    {
      get
      {
        return this.ModelManager.ActiveSceneViewModel;
      }
    }

    protected override bool GetDirectChildrenOrSelfSelected
    {
      get
      {
        return Enumerable.Any<StateModel>((IEnumerable<StateModel>) this.states, (Func<StateModel, bool>) (state => state.IsSelectedWithin));
      }
    }

    public bool IsConflictListFull
    {
      get
      {
        return this.conflicts.Count >= 5;
      }
    }

    public StateGroupModel(VisualStateGroupSceneNode sceneNode)
    {
      this.sceneNode = sceneNode;
      this.FluidLayoutEnabled = BlendSdkHelper.IsSdkInstalled(sceneNode.ProjectContext.TargetFramework);
      this.DefaultTransition = new TransitionModel((VisualStateTransitionSceneNode) null, this);
      this.parkedTransitions.Add(this.DefaultTransition);
    }

    public void AddStateModel(StateModel model)
    {
      List<string> predefinedNames = new List<string>();
      if (ModelBase.stateAndStateGroupsPredefinedOrder.ContainsKey(this.Name))
        predefinedNames = ModelBase.stateAndStateGroupsPredefinedOrder[this.Name];
      this.states.Insert(ModelBase.GetInsertionIndex<StateModel>(model, (Collection<StateModel>) this.States, predefinedNames, (Microsoft.Expression.DesignSurface.ViewModel.SceneNode) this.SceneNode), model);
      model.ParentModel = (ModelBase) this;
      this.UpdateParkedTransitionsForState(model);
      foreach (StateModel stateModel in (Collection<StateModel>) this.StateModels)
        stateModel.NotifyHasPotentialTransitionsChanged();
    }

    public void RemoveStateModel(StateModel model)
    {
      if (this.states.Contains(model))
      {
        this.states.Remove(model);
        foreach (TransitionModel transition in (Collection<TransitionModel>) model.Transitions)
        {
          this.ParkTransition(transition);
          transition.ParentModel = (ModelBase) null;
        }
      }
      foreach (StateModel stateModel in (Collection<StateModel>) this.StateModels)
        stateModel.NotifyHasPotentialTransitionsChanged();
    }

    public void AddTransitionModel(TransitionModel model)
    {
      bool flag = false;
      foreach (StateModel stateModel in (Collection<StateModel>) this.states)
      {
        if (stateModel.DoesTransitionMatch(model))
        {
          stateModel.AddTransitionModel(model);
          flag = true;
          break;
        }
      }
      if (!flag)
        this.ParkTransition(model);
      if (this.DefaultTransition == model && !model.IsDefaultTransition)
      {
        TransitionModel transitionModel = this.parkedTransitions.Find((Predicate<TransitionModel>) (transition =>
        {
          if (string.IsNullOrEmpty(transition.FromStateName))
            return string.IsNullOrEmpty(transition.ToStateName);
          return false;
        }));
        if (transitionModel == null)
        {
          this.DefaultTransition = new TransitionModel((VisualStateTransitionSceneNode) null, this);
          this.parkedTransitions.Add(this.DefaultTransition);
        }
        else
          this.DefaultTransition = transitionModel;
        this.NotifyPropertyChanged("DefaultTransition");
      }
      if (!model.IsDefaultTransition || this.DefaultTransition.TransitionSceneNode != null && this.DefaultTransition.TransitionSceneNode.ShouldSerialize && this.DefaultTransition.TransitionSceneNode.IsAttached)
        return;
      if (this.DefaultTransition.TransitionSceneNode == null)
      {
        this.parkedTransitions.Remove(this.DefaultTransition);
        this.DefaultTransition.ParentModel = (ModelBase) null;
        this.DefaultTransition.GroupModel = (StateGroupModel) null;
      }
      this.DefaultTransition = model;
      this.NotifyPropertyChanged("DefaultTransition");
    }

    public void RemoveTransitionModel(TransitionModel model)
    {
      StateModel stateModel = model.StateModel;
      if (stateModel != null)
      {
        stateModel.RemoveTransitionModel(model);
        model.ParentModel = (ModelBase) null;
        model.GroupModel = (StateGroupModel) null;
      }
      else
        this.parkedTransitions.Remove(model);
      if (model.GroupModel == null || model != model.GroupModel.DefaultTransition || model.TransitionSceneNode != null && model.TransitionSceneNode.ShouldSerialize && model.TransitionSceneNode.IsAttached)
        return;
      TransitionModel transitionModel = this.parkedTransitions.Find((Predicate<TransitionModel>) (transition =>
      {
        if (string.IsNullOrEmpty(transition.FromStateName))
          return string.IsNullOrEmpty(transition.ToStateName);
        return false;
      }));
      if (transitionModel == null)
      {
        this.DefaultTransition = new TransitionModel((VisualStateTransitionSceneNode) null, this);
        this.parkedTransitions.Add(this.DefaultTransition);
      }
      else
        this.DefaultTransition = transitionModel;
      this.NotifyPropertyChanged("DefaultTransition");
    }

    public void SelectState(StateModel stateModel)
    {
      this.ModelManager.SelectState(this, stateModel);
    }

    public void SelectTransition(TransitionModel transitionModel)
    {
      this.ModelManager.SelectTransition(transitionModel);
    }

    public void TogglePinState(StateModel stateModel)
    {
      StateModel pinnedState = this.PinnedState;
      if (this.PinnedState != null)
        this.ModelManager.UnpinState(this, this.PinnedState);
      if (pinnedState == stateModel)
        return;
      this.ModelManager.PinState(this, stateModel);
    }

    private void ParkTransition(TransitionModel transition)
    {
      if (this.parkedTransitions == null)
        this.parkedTransitions = new List<TransitionModel>();
      this.parkedTransitions.Add(transition);
    }

    private void UpdateParkedTransitionsForState(StateModel model)
    {
      if (this.parkedTransitions == null)
        return;
      this.parkedTransitions.RemoveAll((Predicate<TransitionModel>) (transition =>
      {
        if (!model.DoesTransitionMatch(transition))
          return false;
        model.AddTransitionModel(transition);
        return true;
      }));
    }

    private void AddState()
    {
      using (SceneEditTransaction editTransaction = this.sceneNode.ViewModel.CreateEditTransaction(StringTable.AddNewVisualStateUndoUnit))
      {
        VisualStateSceneNode visualStateSceneNode = this.sceneNode.AddState(this.sceneNode.ViewModel.ActiveEditingContainer, ProjectNeutralTypes.VisualState.Name);
        editTransaction.Commit();
        this.ModelManager.LastStateAdded = visualStateSceneNode;
      }
    }

    private void DeleteStateGroup()
    {
      if (this.ActiveSceneViewModel == null)
        return;
      using (SceneEditTransaction editTransaction = this.sceneNode.ViewModel.CreateEditTransaction(StringTable.DeleteVisualStateGroupUndoUnit))
      {
        if (this.selectedState != null || this.IsSelectedWithin)
          this.SelectState((StateModel) null);
        VisualStateManagerSceneNode.DeleteStateGroup(this.ModelManager.TargetNode, this.sceneNode);
        editTransaction.Update();
        if (this.ActiveSceneViewModel != null)
          VisualStateManagerSceneNode.UpdateHasExtendedVisualStateManager(this.ActiveSceneViewModel.ActiveEditingContainer);
        editTransaction.Commit();
      }
    }

    internal void OnContentChanged()
    {
      this.NotifyPropertyChanged("DefaultDuration");
      this.NotifyPropertyChanged("UseFluidLayout");
      this.NotifyPropertyChanged("Name");
    }

    internal void NotifyIsStructureEditableChanged()
    {
      foreach (StateModel stateModel in (Collection<StateModel>) this.states)
        stateModel.NotifyIsStructureEditableChanged();
      this.NotifyPropertyChanged("IsStructureEditable");
    }

    internal void OnStateNameChanged(StateModel stateModel)
    {
      foreach (StateModel stateModel1 in (Collection<StateModel>) this.States)
        stateModel1.NotifyHasPotentialTransitionsChanged();
      this.UpdateParkedTransitionsForState(stateModel);
    }

    public static IDisposable UpdateConflicts(IEnumerable<StateGroupModel> groups, Dictionary<TimelineSceneNode.PropertyNodePair, bool> candidateConflictAnimations, Dictionary<TimelineSceneNode.PropertyNodePair, Dictionary<StateGroupModel, int>> conflictsCollection)
    {
      return (IDisposable) new StateGroupModel.UpdateConflictsToken(groups, candidateConflictAnimations, conflictsCollection);
    }

    private void UpdateConflicts(TimelineSceneNode.PropertyNodePair propertyNodePair, Dictionary<TimelineSceneNode.PropertyNodePair, Dictionary<StateGroupModel, int>> conflictsCollection)
    {
      ConflictModel conflictModel = Enumerable.FirstOrDefault<ConflictModel>((IEnumerable<ConflictModel>) this.conflicts, (Func<ConflictModel, bool>) (conflict => conflict.TargetElementAndProperty.Equals((object) propertyNodePair)));
      if (conflictModel != null)
        this.conflicts.Remove(conflictModel);
      Dictionary<StateGroupModel, int> dictionary;
      if (!this.IsConflictListFull && conflictsCollection.TryGetValue(propertyNodePair, out dictionary) && (dictionary.Keys.Count > 1 && dictionary.ContainsKey(this)))
        this.conflicts.Add(new ConflictModel(propertyNodePair, (IEnumerable<StateGroupModel>) dictionary.Keys));
      this.NotifyPropertyChanged("IsConflictListFull");
    }

    private class UpdateConflictsToken : IDisposable
    {
      private IEnumerable<StateGroupModel> groups;
      private Dictionary<TimelineSceneNode.PropertyNodePair, bool> candidateConflictAnimations;
      private Dictionary<TimelineSceneNode.PropertyNodePair, Dictionary<StateGroupModel, int>> conflictsCollection;

      public UpdateConflictsToken(IEnumerable<StateGroupModel> groups, Dictionary<TimelineSceneNode.PropertyNodePair, bool> candidateConflictAnimations, Dictionary<TimelineSceneNode.PropertyNodePair, Dictionary<StateGroupModel, int>> conflictsCollection)
      {
        this.groups = groups;
        this.candidateConflictAnimations = candidateConflictAnimations;
        this.conflictsCollection = conflictsCollection;
        this.candidateConflictAnimations.Clear();
      }

      public void Dispose()
      {
        foreach (TimelineSceneNode.PropertyNodePair propertyNodePair in this.candidateConflictAnimations.Keys)
        {
          foreach (StateGroupModel stateGroupModel in this.groups)
            stateGroupModel.UpdateConflicts(propertyNodePair, this.conflictsCollection);
        }
        this.candidateConflictAnimations.Clear();
      }
    }
  }
}
