// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Timeline.PropertyTimelineItem
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.UserInterface.Timeline
{
  public class PropertyTimelineItem : KeyFramedTimelineItem
  {
    private TimelineItem targetTimelineItem;

    public override string DisplayName
    {
      get
      {
        if (this.TargetProperty == null)
          return string.Empty;
        return this.TargetProperty.LastStep.Name;
      }
      set
      {
      }
    }

    public override string FullName
    {
      get
      {
        if (this.TargetProperty == null)
          return this.targetTimelineItem.FullName;
        return this.targetTimelineItem.FullName + "." + this.TargetProperty.ShortPath;
      }
    }

    public KeyFrameAnimationSceneNode PropertyAnimation
    {
      get
      {
        return this.KeyFrameAnimationSceneNode;
      }
    }

    public override bool IsSelectable
    {
      get
      {
        if (this.Parent != null && this.Parent.IsSelectable)
          return this.TimelinePane.HasUnlockedActiveStoryboardContainer;
        return false;
      }
    }

    protected override bool IsActiveCore
    {
      get
      {
        if (base.IsActiveCore && this.TargetProperty != null)
          return !DesignTimeProperties.ExplicitAnimationProperty.Equals((object) this.TargetProperty[0]);
        return false;
      }
    }

    private ICollection<KeyFrameSceneNode> KeyFrameSceneNodes
    {
      get
      {
        List<KeyFrameSceneNode> list = new List<KeyFrameSceneNode>();
        foreach (SimpleKeyFrameItem simpleKeyFrameItem in (Collection<KeyFrameItem>) this.KeyFrameItems)
          list.Add(simpleKeyFrameItem.KeyFrameSceneNode);
        return (ICollection<KeyFrameSceneNode>) list;
      }
    }

    public override bool ShouldBubbleKeyFrames
    {
      get
      {
        return this.IsInActiveTimeline;
      }
    }

    public override bool ShouldBubbleTimes
    {
      get
      {
        return this.IsInActiveTimeline;
      }
    }

    public override ICommand EditRepeatCountCommand
    {
      get
      {
        return (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(((ScheduledTimelineItem) this).EditRepeatCount));
      }
    }

    public PropertyTimelineItem(TimelineItemManager timelineItemManager, TimelineItem targetTimelineItem, SceneElement targetElement, StoryboardTimelineSceneNode parentTimeline, KeyFrameAnimationSceneNode propertyAnimation)
      : base(timelineItemManager, targetElement, parentTimeline, propertyAnimation)
    {
      this.targetTimelineItem = targetTimelineItem;
      this.targetTimelineItem.PropertyChanged += new PropertyChangedEventHandler(this.TargetTimelineItem_PropertyChanged);
    }

    protected override ContextMenu BuildContextMenu()
    {
      ContextMenu contextMenu = new ContextMenu();
      MenuItem menuItem = new MenuItem();
      menuItem.Command = this.EditRepeatCountCommand;
      menuItem.Header = (object) StringTable.EditRepeatCountCommandName;
      menuItem.Name = "EditRepeatCount";
      contextMenu.Items.Add((object) menuItem);
      return contextMenu;
    }

    public override void Select()
    {
      if (this.IsSelectable)
        this.TimelineItemManager.ViewModel.KeyFrameSelectionSet.SetSelection(this.KeyFrameSceneNodes, (KeyFrameSceneNode) null);
      else
        this.TimelineItemManager.ViewModel.KeyFrameSelectionSet.Clear();
    }

    public override void ToggleSelect()
    {
      if (!this.IsSelectable)
        return;
      if (this.IsSelected)
        this.TimelineItemManager.ViewModel.KeyFrameSelectionSet.RemoveSelection(this.KeyFrameSceneNodes);
      else
        this.TimelineItemManager.ViewModel.KeyFrameSelectionSet.ExtendSelection(this.KeyFrameSceneNodes);
    }

    public override void ExtendSelect()
    {
      if (!this.IsSelectable)
        return;
      this.TimelineItemManager.ViewModel.KeyFrameSelectionSet.ExtendSelection(this.KeyFrameSceneNodes);
    }

    protected override void OnRemoved()
    {
      base.OnRemoved();
      if (this.targetTimelineItem == null)
        return;
      this.targetTimelineItem.PropertyChanged -= new PropertyChangedEventHandler(this.TargetTimelineItem_PropertyChanged);
      this.targetTimelineItem = (TimelineItem) null;
    }

    private void TargetTimelineItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == "FullName"))
        return;
      this.OnPropertyChanged("FullName");
    }
  }
}
