// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Timeline.StyleTimelineItem
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Selection;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace Microsoft.Expression.DesignSurface.UserInterface.Timeline
{
  public sealed class StyleTimelineItem : CompoundKeyFrameTimelineItem
  {
    private bool isShown;
    private StyleNode styleNode;

    public override bool ShouldBubbleKeyFrames
    {
      get
      {
        return false;
      }
    }

    public StyleNode StyleNode
    {
      get
      {
        return this.styleNode;
      }
    }

    public override string DisplayName
    {
      get
      {
        return this.StyleNode.DisplayName;
      }
      set
      {
      }
    }

    public override string FullName
    {
      get
      {
        return this.styleNode.UniqueID;
      }
    }

    public override bool IsSelectable
    {
      get
      {
        if (this.StyleNode != null)
          return this.StyleNode.IsSelectable;
        return false;
      }
    }

    public bool IsShown
    {
      get
      {
        return this.isShown;
      }
      set
      {
        if (this.IsShown != value)
          this.isShown = value;
        this.OnPropertyChanged("IsShown");
      }
    }

    protected override bool IsActiveCore
    {
      get
      {
        return this.StyleNode == this.TimelineItemManager.ViewModel.ActiveEditingContainer;
      }
    }

    public StyleTimelineItem(TimelineItemManager timelineItemManager, StyleNode styleNode)
      : base(timelineItemManager)
    {
      this.isShown = true;
      this.IsExpanded = true;
      this.styleNode = styleNode;
      this.TimelineItemManager.RegisterTimelineItem((TimelineItem) this);
    }

    public override int CompareTo(TimelineItem timelineItem)
    {
      return 0;
    }

    public void RefreshIsLocked()
    {
      this.RefreshIsAncestorLocked();
    }

    public void RefreshIsAncestorLocked()
    {
      this.OnPropertyChanged("IsSelectable");
      foreach (TimelineItem timelineItem in (Collection<TimelineItem>) this.Children)
      {
        ElementTimelineItem elementTimelineItem = timelineItem as ElementTimelineItem;
        if (elementTimelineItem != null)
          elementTimelineItem.RefreshIsAncestorLocked();
      }
    }

    protected override void OnRemoved()
    {
      base.OnRemoved();
    }

    public override void Select()
    {
      SceneElementSelectionSet elementSelectionSet = this.TimelineItemManager.ViewModel.ElementSelectionSet;
      if (!this.IsSelectable)
        elementSelectionSet.Clear();
      else
        elementSelectionSet.SetSelection((SceneElement) this.StyleNode);
    }

    public override void ToggleSelect()
    {
      if (!this.IsSelectable)
        return;
      this.TimelineItemManager.ViewModel.ElementSelectionSet.ToggleSelection((SceneElement) this.StyleNode);
    }

    public override void ExtendSelect()
    {
      if (!this.IsSelectable)
        return;
      this.TimelineItemManager.ViewModel.ElementSelectionSet.ExtendSelection((SceneElement) this.StyleNode);
    }

    protected override ContextMenu BuildContextMenu()
    {
      return ContextMenuHelper.CreateContextMenu((ISelectionSet<SceneElement>) this.TimelineItemManager.ViewModel.ElementSelectionSet, this.TimelineItemManager.ViewModel, false);
    }
  }
}
