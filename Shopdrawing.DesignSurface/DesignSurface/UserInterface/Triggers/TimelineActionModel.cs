// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Triggers.TimelineActionModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.UserInterface.Timeline.UI;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Data;

namespace Microsoft.Expression.DesignSurface.UserInterface.Triggers
{
  public class TimelineActionModel : TriggerActionModel
  {
    private static List<TimelineOperation> Operations = new List<TimelineOperation>();
    private ListCollectionView operationsView;
    private StoryboardOption currentStoryboardOption;

    public ListCollectionView OperationsView
    {
      get
      {
        return this.operationsView;
      }
    }

    public bool IsValid
    {
      get
      {
        return TimelineActionModel.ShouldExposeTriggerToUser(this.TimelineAction);
      }
    }

    public StoryboardOption Storyboard
    {
      get
      {
        if (this.currentStoryboardOption == null || this.currentStoryboardOption.Storyboard != this.TimelineAction.TargetTimeline)
          this.currentStoryboardOption = new StoryboardOption(this.TimelineAction.TargetTimeline);
        return this.currentStoryboardOption;
      }
      set
      {
        if (value == null || value.Storyboard == this.TimelineAction.TargetTimeline)
          return;
        using (SceneEditTransaction editTransaction = this.SceneNode.ViewModel.CreateEditTransaction(StringTable.TriggerChangeUndoUnit))
        {
          StoryboardTimelineSceneNode storyboard = value.Storyboard;
          if (storyboard == null)
          {
            bool createAsResource;
            string timelineName = TimelinePane.GetTimelineName(this.SceneNode.StoryboardContainer, TriggersHelper.GetDefaultStoryboardName(this.SceneNode.FindSceneNodeTypeAncestor<TriggerBaseNode>()), out createAsResource);
            if (timelineName != null)
              storyboard = this.SceneNode.ViewModel.AnimationEditor.CreateNewTimeline(this.SceneNode.StoryboardContainer, timelineName, TriggerCreateBehavior.DoNotCreate, createAsResource);
          }
          if (storyboard != null)
          {
            StoryboardTimelineSceneNode targetTimeline = this.TimelineAction.TargetTimeline;
            this.TimelineAction.TargetTimeline = storyboard;
            this.EnsureProperActions(this.TimelineAction.FindSceneNodeTypeAncestor<TriggerBaseNode>(), targetTimeline);
            this.EnsureProperActions(this.TimelineAction.FindSceneNodeTypeAncestor<TriggerBaseNode>(), storyboard);
            storyboard.UpdateActionNames();
          }
          else
            this.OnPropertyChanged("Storyboard");
          editTransaction.Commit();
        }
      }
    }

    public IEnumerable AvailableStoryboards
    {
      get
      {
        List<StoryboardOption> list = new List<StoryboardOption>();
        SceneViewModel viewModel = this.SceneNode.ViewModel;
        IStoryboardContainer storyboardContainer = this.SceneNode.StoryboardContainer;
        if (storyboardContainer != null && viewModel.Document != null)
        {
          StoryboardTimelineSceneNode targetTimeline = this.TimelineAction.TargetTimeline;
          if (targetTimeline.IsInResourceDictionary)
          {
            foreach (StoryboardTimelineSceneNode storyboard in viewModel.AnimationEditor.EnumerateStoryboardsForContainer(storyboardContainer))
            {
              if (storyboard.IsInResourceDictionary && TimelinePane.ShouldExposeStoryboardToUser((SceneNode) storyboard))
                list.Add(new StoryboardOption(storyboard));
            }
            list.Add(new StoryboardOption((StoryboardTimelineSceneNode) null));
          }
          else
            list.Add(new StoryboardOption(targetTimeline));
        }
        return (IEnumerable) list;
      }
    }

    protected TimelineActionNode TimelineAction
    {
      get
      {
        return (TimelineActionNode) this.SceneNode;
      }
    }

    static TimelineActionModel()
    {
      TimelineActionModel.Operations.Add(TimelineOperation.Begin);
      TimelineActionModel.Operations.Add(TimelineOperation.Stop);
      TimelineActionModel.Operations.Add(TimelineOperation.Pause);
      TimelineActionModel.Operations.Add(TimelineOperation.SkipToFill);
      TimelineActionModel.Operations.Add(TimelineOperation.Resume);
      TimelineActionModel.Operations.Add(TimelineOperation.Remove);
    }

    internal TimelineActionModel(TimelineActionNode action)
      : base((TriggerActionNode) action)
    {
      this.operationsView = (ListCollectionView) CollectionViewSource.GetDefaultView((object) new ReadOnlyCollection<TimelineOperation>((IList<TimelineOperation>) TimelineActionModel.Operations));
      this.operationsView.MoveCurrentTo((object) action.TimelineOperation);
      this.operationsView.CurrentChanged += new EventHandler(this.OperationsView_CurrentChanged);
    }

    public override void Update()
    {
      this.OnPropertyChanged("AvailableStoryboards");
      this.OnPropertyChanged("Storyboard");
      this.OnPropertyChanged("IsValid");
      if (this.currentStoryboardOption == null)
        return;
      this.currentStoryboardOption.Update();
    }

    protected override void Delete()
    {
      if (!this.SceneNode.IsAttached)
        return;
      if (!TriggersHelper.CanDeleteAction(this.TimelineAction))
      {
        this.SceneNode.ViewModel.DesignerContext.MessageDisplayService.ShowError(StringTable.TriggerDeleteBeginAction);
      }
      else
      {
        using (SceneEditTransaction editTransaction = this.SceneNode.ViewModel.CreateEditTransaction(StringTable.TriggerChangeUndoUnit))
        {
          StoryboardTimelineSceneNode targetTimeline = this.TimelineAction.TargetTimeline;
          this.SceneNode.Remove();
          targetTimeline.UpdateActionNames();
          editTransaction.Commit();
        }
      }
    }

    private void OperationsView_CurrentChanged(object sender, EventArgs e)
    {
      TimelineOperation timelineOperation = (TimelineOperation) this.operationsView.CurrentItem;
      if (timelineOperation == this.TimelineAction.TimelineOperation)
        return;
      using (SceneEditTransaction editTransaction = this.SceneNode.ViewModel.CreateEditTransaction(StringTable.TriggerChangeUndoUnit))
      {
        TimelineActionNode actionForOperation = TimelineActionNode.CreateActionForOperation(timelineOperation, this.SceneNode.ViewModel);
        actionForOperation.TargetTimeline = this.TimelineAction.TargetTimeline;
        TriggersHelper.ReplaceSceneNode((SceneNode) this.TimelineAction, (SceneNode) actionForOperation);
        actionForOperation.TargetTimeline.UpdateActionNames();
        this.EnsureProperActions(actionForOperation.FindSceneNodeTypeAncestor<TriggerBaseNode>(), actionForOperation.TargetTimeline);
        editTransaction.Commit();
      }
    }

    private void EnsureProperActions(TriggerBaseNode trigger, StoryboardTimelineSceneNode storyboard)
    {
      if (!TriggersHelper.NeedsBeginAction(storyboard))
        return;
      TimelineActionNode timelineActionNode = (TimelineActionNode) BeginActionNode.Factory.Instantiate(storyboard.ViewModel);
      timelineActionNode.TargetTimeline = storyboard;
      TriggersHelper.DefaultAddAction(trigger, (TriggerActionNode) timelineActionNode);
      storyboard.UpdateActionNames();
    }

    internal static bool ShouldExposeTriggerToUser(TimelineActionNode timelineActionNode)
    {
      if (timelineActionNode != null)
        return TimelinePane.ShouldExposeStoryboardToUser((SceneNode) timelineActionNode.TargetTimeline);
      return false;
    }
  }
}
