// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.CopyStatePropertiesCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.UserInterface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal abstract class CopyStatePropertiesCommand : SingleTargetDynamicMenuCommandBase
  {
    private VisualStateSceneNode sourceState;

    public override bool IsAvailable
    {
      get
      {
        if (base.IsAvailable)
          return this.ViewModel.ProjectContext.IsCapabilitySet(PlatformCapability.SupportsVisualStateManager);
        return false;
      }
    }

    public override bool IsEnabled
    {
      get
      {
        if (base.IsEnabled && this.IsAvailable && (this.SourceState != null && this.SourceState.Storyboard != null) && (this.SourceState.Storyboard.Children != null && this.SourceState.Storyboard.Children.Count > 0 && this.SourceState.StateGroup != null))
          return !this.SourceState.StateGroup.IsSketchFlowAnimation;
        return false;
      }
    }

    public override IEnumerable Items
    {
      get
      {
        List<object> list = new List<object>();
        if (this.SourceState == null || this.SourceState.StateGroup == null)
          return (IEnumerable) list;
        bool flag = false;
        foreach (VisualStateSceneNode visualStateSceneNode in (IEnumerable<VisualStateSceneNode>) this.SourceState.StateGroup.States)
        {
          if (visualStateSceneNode != this.SourceState)
          {
            MenuItem menuItem = this.CreateMenuItem(visualStateSceneNode.Name.Replace("_", "__"), visualStateSceneNode.Name, (ICommand) new CopyStatePropertiesCommand.CopyStatePropertiesInternalCommand(this, (SceneNode) visualStateSceneNode));
            list.Add((object) menuItem);
            flag = true;
          }
        }
        if (this.CanAddNewState)
        {
          if (flag)
            list.Add((object) new Separator());
          list.Add((object) this.CreateMenuItem(StringTable.CopyPropertiesToNewState, StringTable.CopyPropertiesToNewState, (ICommand) new CopyStatePropertiesCommand.CopyStatePropertiesInternalCommand(this, (SceneNode) this.SourceState.StateGroup)));
        }
        return (IEnumerable) list;
      }
    }

    protected VisualStateSceneNode SourceState
    {
      get
      {
        if (this.sourceState != null)
          return this.sourceState;
        if (this.ViewModel != null)
          return this.ViewModel.StateEditTarget;
        return (VisualStateSceneNode) null;
      }
    }

    protected abstract string UndoString { get; }

    public abstract string CommandName { get; }

    private bool CanAddNewState
    {
      get
      {
        if (this.ViewModel == null || this.SourceState == null || this.SourceState.StateGroup == null)
          return false;
        ControlTemplateElement controlTemplateElement = this.ViewModel.ActiveEditingContainer as ControlTemplateElement;
        if (controlTemplateElement == null)
          return true;
        foreach (DefaultStateRecord defaultStateRecord in ProjectAttributeHelper.GetDefaultStateRecords(this.ViewModel.ProjectContext.ResolveType(controlTemplateElement.ControlTemplateTargetTypeId), (ITypeResolver) this.ViewModel.ProjectContext))
        {
          if (defaultStateRecord.GroupName.Equals(this.SourceState.StateGroup.Name, StringComparison.Ordinal))
            return false;
        }
        return true;
      }
    }

    public CopyStatePropertiesCommand(SceneViewModel viewModel, VisualStateSceneNode sourceState)
      : base(viewModel)
    {
      this.sourceState = sourceState;
    }

    public override void Execute()
    {
      throw new InvalidOperationException();
    }

    protected virtual void ExecuteInternal(SceneNode targetStateOrStateGroup)
    {
      VisualStateSceneNode sourceState = this.SourceState;
      using (SceneEditTransaction editTransaction = this.ViewModel.CreateEditTransaction(this.UndoString, false))
      {
        VisualStateSceneNode visualStateSceneNode = targetStateOrStateGroup as VisualStateSceneNode;
        VisualStateGroupSceneNode stateGroupSceneNode = targetStateOrStateGroup as VisualStateGroupSceneNode;
        if (stateGroupSceneNode != null)
          visualStateSceneNode = stateGroupSceneNode.AddState(this.ViewModel.ActiveEditingContainer, sourceState.Name + StringTable.SceneModelDuplicateLabelSuffix);
        if (visualStateSceneNode.Storyboard == null)
          visualStateSceneNode.Storyboard = StoryboardTimelineSceneNode.Factory.Instantiate(this.ViewModel);
        Dictionary<TimelineSceneNode.PropertyNodePair, List<TimelineSceneNode>> dictionary = new Dictionary<TimelineSceneNode.PropertyNodePair, List<TimelineSceneNode>>();
        foreach (TimelineSceneNode timelineSceneNode in (IEnumerable<TimelineSceneNode>) visualStateSceneNode.Storyboard.Children)
        {
          if (!dictionary.ContainsKey(timelineSceneNode.TargetElementAndProperty))
            dictionary.Add(timelineSceneNode.TargetElementAndProperty, new List<TimelineSceneNode>(1));
          dictionary[timelineSceneNode.TargetElementAndProperty].Add(timelineSceneNode);
        }
        List<TimelineSceneNode> animationsToCopy = this.GetAnimationsToCopy();
        bool flag = false;
        foreach (SceneNode sceneNode in animationsToCopy)
        {
          TimelineSceneNode timelineSceneNode1 = this.ViewModel.GetSceneNode(sceneNode.DocumentNode.Clone(this.ViewModel.DocumentRoot.DocumentContext)) as TimelineSceneNode;
          visualStateSceneNode.Storyboard.Children.Add(timelineSceneNode1);
          if (dictionary.ContainsKey(timelineSceneNode1.TargetElementAndProperty))
          {
            flag = true;
            foreach (TimelineSceneNode timelineSceneNode2 in dictionary[timelineSceneNode1.TargetElementAndProperty])
              visualStateSceneNode.Storyboard.Children.Remove(timelineSceneNode2);
          }
        }
        if (flag && this.ViewModel.DefaultView != null)
          this.ViewModel.DefaultView.ShowBubble(StringTable.AnimationAutoDeletedWarningMessage, MessageBubbleType.Warning);
        editTransaction.Commit();
      }
    }

    protected abstract List<TimelineSceneNode> GetAnimationsToCopy();

    private class CopyStatePropertiesInternalCommand : ICommand
    {
      private CopyStatePropertiesCommand owner;
      private SceneNode targetStateOrStateGroup;

      public event EventHandler CanExecuteChanged
      {
        add
        {
        }
        remove
        {
        }
      }

      public CopyStatePropertiesInternalCommand(CopyStatePropertiesCommand owner, SceneNode targetStateOrStateGroup)
      {
        this.owner = owner;
        this.targetStateOrStateGroup = targetStateOrStateGroup;
      }

      public void Execute(object arg)
      {
        this.owner.ExecuteInternal(this.targetStateOrStateGroup);
      }

      public bool CanExecute(object arg)
      {
        return true;
      }
    }
  }
}
