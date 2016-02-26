// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Timeline.PathTimelineItem
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Data;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.UserInterface.Timeline
{
  public class PathTimelineItem : AnimationTimelineItem
  {
    private const double ZeroDurationEpsilon = 0.001;
    private DelegateCommand orientToPathCommand;

    public override bool IsSelectable
    {
      get
      {
        if (this.Parent != null && this.Parent.IsSelectable)
          return this.TimelinePane.HasUnlockedActiveStoryboardContainer;
        return false;
      }
    }

    public ReadOnlyCollection<TimelineSceneNode> PathAnimationSceneNodes
    {
      get
      {
        return this.ScheduledAnimations;
      }
    }

    public override string DisplayName
    {
      get
      {
        return StringTable.MotionPathTimelineItemDisplayName;
      }
      set
      {
      }
    }

    public ICommand OrientToPathCommand
    {
      get
      {
        return (ICommand) this.orientToPathCommand;
      }
    }

    public bool OrientToPath
    {
      get
      {
        return this.TimelineItemManager.AnimationEditor.IsMotionPathOrientedToPath((SceneNode) this.Element);
      }
      set
      {
        this.TimelineItemManager.AnimationEditor.SetMotionPathOrientToPath(this.Element, value);
      }
    }

    public SceneElement Element
    {
      get
      {
        return this.PrimaryScheduledAnimation.TargetElement as SceneElement;
      }
    }

    public override ICommand EditRepeatCountCommand
    {
      get
      {
        return (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(((ScheduledTimelineItem) this).EditRepeatCount));
      }
    }

    public override double AbsoluteClipEnd
    {
      get
      {
        return base.AbsoluteClipEnd;
      }
      set
      {
        if (value - this.AbsoluteClipBegin < 0.001)
          value = this.AbsoluteClipBegin + 0.001;
        base.AbsoluteClipEnd = value;
        this.Element.ViewModel.Document.OnUpdatedEditTransaction();
      }
    }

    public override double AbsoluteClipBegin
    {
      get
      {
        return base.AbsoluteClipBegin;
      }
      set
      {
        if (this.AbsoluteClipEnd - value < 0.001)
          value = this.AbsoluteClipEnd - 0.001;
        base.AbsoluteClipBegin = value;
        this.Element.ViewModel.Document.OnUpdatedEditTransaction();
      }
    }

    public PathTimelineItem(TimelineItemManager timelineItemManager, StoryboardTimelineSceneNode parentTimeline, PathAnimationSceneNode pathAnimationSceneNode)
      : base(timelineItemManager, parentTimeline, (AnimationSceneNode) pathAnimationSceneNode)
    {
      this.orientToPathCommand = new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.OrientToPathCommandBinding_Execute));
    }

    public void AddPathAnimationSceneNode(PathAnimationSceneNode pathAnimation)
    {
      if (pathAnimation.TargetProperty == pathAnimation.Platform.Metadata.CommonProperties.TranslationX)
        this.InsertScheduledAnimation(0, (TimelineSceneNode) pathAnimation);
      else
        this.AddScheduledAnimation((TimelineSceneNode) pathAnimation);
    }

    public void RemovePathAnimationSceneNode(PathAnimationSceneNode pathAnimation)
    {
      this.RemoveScheduledAnimation((TimelineSceneNode) pathAnimation);
    }

    protected override ContextMenu BuildContextMenu()
    {
      ContextMenu contextMenu = new ContextMenu();
      MenuItem menuItem1 = new MenuItem();
      menuItem1.Command = this.OrientToPathCommand;
      menuItem1.Header = (object) StringTable.OrientToPathCommandName;
      menuItem1.Name = "OrientToPath";
      menuItem1.IsChecked = this.OrientToPath;
      contextMenu.Items.Add((object) menuItem1);
      MenuItem menuItem2 = new MenuItem();
      menuItem2.Command = this.EditRepeatCountCommand;
      menuItem2.Header = (object) StringTable.EditRepeatCountCommandName;
      menuItem2.Name = "EditRepeatCount";
      contextMenu.Items.Add((object) menuItem2);
      return contextMenu;
    }

    public override void Select()
    {
      if (this.IsSelectable)
        this.TimelineItemManager.ViewModel.AnimationSelectionSet.SetSelection((AnimationSceneNode) this.PrimaryScheduledAnimation);
      else
        this.TimelineItemManager.ViewModel.AnimationSelectionSet.Clear();
    }

    public override void ToggleSelect()
    {
      if (!this.IsSelectable)
        return;
      if (this.IsSelected)
        this.TimelineItemManager.ViewModel.AnimationSelectionSet.RemoveSelection((AnimationSceneNode) this.PrimaryScheduledAnimation);
      else
        this.TimelineItemManager.ViewModel.AnimationSelectionSet.ExtendSelection((AnimationSceneNode) this.PrimaryScheduledAnimation);
    }

    public override void ExtendSelect()
    {
      if (!this.IsSelectable)
        return;
      this.TimelineItemManager.ViewModel.AnimationSelectionSet.ExtendSelection((AnimationSceneNode) this.PrimaryScheduledAnimation);
    }

    private void OrientToPathCommandBinding_Execute()
    {
      this.OrientToPath = !this.OrientToPath;
    }
  }
}
