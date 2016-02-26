// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Timeline.TimelineItem
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Timeline.DragDrop;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.UserInterface.Timeline.UI;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.UserInterface;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.UserInterface.Timeline
{
  public abstract class TimelineItem : VirtualizingTreeItem<TimelineItem>, IDragSourceProvider
  {
    private bool isAnimatable;
    private bool isActive;
    private bool isValid;
    private TimelineItemManager timelineItemManager;
    private ContextMenu contextMenu;
    private double rowHeight;
    private string extendedTooltip;

    public ContextMenu ContextMenu
    {
      get
      {
        return this.contextMenu;
      }
    }

    public string ExtendedTooltip
    {
      get
      {
        return this.extendedTooltip;
      }
      set
      {
        this.extendedTooltip = value;
        this.OnPropertyChanged("ExtendedTooltip");
      }
    }

    public virtual DrawingBrush IconBrush
    {
      get
      {
        return IconMapper.GetDrawingBrushForType(PlatformTypes.DependencyProperty, false, 12, 12);
      }
    }

    public bool IsActive
    {
      get
      {
        return this.isActive;
      }
      protected set
      {
        if (this.isActive == value)
          return;
        this.isActive = value;
        this.OnPropertyChanged("IsActive");
      }
    }

    protected abstract bool IsActiveCore { get; }

    public virtual bool HasActiveChild
    {
      get
      {
        bool flag = false;
        foreach (TimelineItem timelineItem in (Collection<TimelineItem>) this.Children)
        {
          if (timelineItem.IsActive)
          {
            flag = true;
            break;
          }
        }
        return flag;
      }
    }

    public TimelinePane TimelinePane
    {
      get
      {
        return this.timelineItemManager.TimelinePane;
      }
    }

    public bool IsAnimatable
    {
      get
      {
        return this.isAnimatable;
      }
      set
      {
        this.isAnimatable = value;
        this.OnPropertyChanged("IsAnimatable");
      }
    }

    public bool IsValid
    {
      get
      {
        return this.isValid;
      }
    }

    public TimelineItemManager TimelineItemManager
    {
      get
      {
        return this.timelineItemManager;
      }
    }

    public double ViewportWidth
    {
      get
      {
        return this.timelineItemManager.ViewportWidth;
      }
    }

    public double ViewportOffset
    {
      get
      {
        return this.timelineItemManager.ViewportOffset;
      }
    }

    public double RowHeight
    {
      get
      {
        return this.rowHeight;
      }
      set
      {
        if (value == this.rowHeight)
          return;
        this.rowHeight = value;
        this.OnPropertyChanged("RowHeight");
      }
    }

    public bool CanLockAndHide
    {
      get
      {
        if (this.CanLock)
          return this.CanHide;
        return false;
      }
    }

    public virtual bool CanLock
    {
      get
      {
        return false;
      }
    }

    public virtual bool CanHide
    {
      get
      {
        return false;
      }
    }

    public override IComparer<TimelineItem> TreeItemComparer
    {
      get
      {
        return (IComparer<TimelineItem>) new TimelineItemComparer();
      }
    }

    public virtual SceneNode SceneNode
    {
      get
      {
        return (SceneNode) null;
      }
    }

    public virtual object DragData
    {
      get
      {
        return (object) null;
      }
    }

    protected TimelineItem(TimelineItemManager timelineItemManager)
    {
      this.timelineItemManager = timelineItemManager;
    }

    public override void Initialize()
    {
      this.UpdateIsActive();
    }

    private void UpdateIsActive()
    {
      this.IsActive = this.IsActiveCore;
    }

    public virtual void RefreshName()
    {
      this.OnPropertyChanged("DisplayName");
      this.OnPropertyChanged("FullName");
      this.OnPropertyChanged("DisplayNameNoTextContent");
    }

    public void Invalidate()
    {
      TimelineItem timelineItem = this;
      do
      {
        timelineItem.isValid = false;
        timelineItem = timelineItem.Parent;
      }
      while (timelineItem != null && timelineItem.IsValid);
    }

    public void Validate()
    {
      this.ValidateChildren();
    }

    public virtual void OnActiveTimelineContextChanged()
    {
      this.UpdateIsActive();
      for (int index = 0; index < this.Children.Count; ++index)
        this.Children[index].OnActiveTimelineContextChanged();
    }

    public virtual ISceneInsertionPoint GetInsertionPoint(object data, DragDropContext context)
    {
      return (ISceneInsertionPoint) null;
    }

    public virtual bool CanDrag()
    {
      return false;
    }

    public virtual void OnZoomChanged()
    {
    }

    public void OnViewportChanged()
    {
      this.OnPropertyChanged("ViewportWidth");
      this.OnPropertyChanged("ViewportOffset");
    }

    public virtual void UpdateContextMenu()
    {
      this.contextMenu = this.BuildContextMenu();
      if (this.contextMenu != null)
        FocusScopeManager.SetAllowedFocus((DependencyObject) this.contextMenu, true);
      this.OnPropertyChanged("ContextMenu");
    }

    protected virtual ContextMenu BuildContextMenu()
    {
      return (ContextMenu) null;
    }

    protected override void ChildAdded(TimelineItem child)
    {
      this.ChildAdded(child);
      this.OnPropertyChanged("HasActiveChild");
    }

    protected override void ChildRemoved(TimelineItem child)
    {
      this.ChildRemoved(child);
      this.OnPropertyChanged("HasActiveChild");
    }

    protected override void ChildPropertyChanged(string propertyName)
    {
      if (propertyName == "IsActive")
        this.OnPropertyChanged("HasActiveChild");
      base.ChildPropertyChanged(propertyName);
    }

    protected void ValidateChildren()
    {
      if (this.IsValid)
        return;
      for (int index = 0; index < this.Children.Count; ++index)
      {
        TimelineItem timelineItem = this.Children[index];
        if (timelineItem != null)
          timelineItem.ValidateChildren();
      }
      this.ValidateCore();
    }

    protected virtual void ValidateCore()
    {
      this.isValid = true;
    }

    public virtual void CommitScheduledPropertyChanges(double originalRegionBegin, double originalRegionEnd, double finalRegionBegin, double finalRegionEnd)
    {
    }
  }
}
