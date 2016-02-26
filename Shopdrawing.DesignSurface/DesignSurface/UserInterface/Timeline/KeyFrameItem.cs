// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Timeline.KeyFrameItem
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Selection;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.UserInterface.Timeline.UI;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.UserInterface.Timeline
{
  public abstract class KeyFrameItem : INotifyPropertyChanged, IComparable
  {
    private double localTime = double.NaN;
    private static Dictionary<KeyFrameItem.KeyFrameLocation, KeyFrameItem> locationToItemTable = new Dictionary<KeyFrameItem.KeyFrameLocation, KeyFrameItem>();
    private KeyFrameItem.KeyFrameLocation location;
    private bool isSelected;
    private TimelineItemManager timelineItemManager;

    public ICommand SelectCommand
    {
      get
      {
        return (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.Select));
      }
    }

    public ICommand ToggleSelectCommand
    {
      get
      {
        return (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.ToggleSelect));
      }
    }

    public ICommand ExtendSelectCommand
    {
      get
      {
        return (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.ExtendSelect));
      }
    }

    public ICommand EnsureSelectCommand
    {
      get
      {
        return (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.EnsureSelect));
      }
    }

    public ICommand BeginEditCommand
    {
      get
      {
        return (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.BeginEdit));
      }
    }

    public RoutedCommand EditCommand
    {
      get
      {
        return TimelineCommands.EditSelectedKeyFramesCommand;
      }
    }

    public RoutedCommand EndEditCommand
    {
      get
      {
        return TimelineCommands.ApplyKeyFrameEditsCommand;
      }
    }

    public TimelineItem TimelineItem
    {
      get
      {
        return this.location.TimelineItem;
      }
    }

    public double Time
    {
      get
      {
        if (double.IsNaN(this.localTime))
          return this.location.ModelTime;
        return this.localTime;
      }
      set
      {
        value = TimelineView.SnapSeconds(value);
        if (this.Time == value && this.IsOldTimeSet)
          return;
        this.localTime = value;
        this.OnPropertyChanged("Time");
      }
    }

    public bool IsOldTimeSet
    {
      get
      {
        return !double.IsNaN(this.localTime);
      }
    }

    public double OldTime
    {
      get
      {
        if (double.IsNaN(this.localTime))
          return double.NaN;
        return this.location.ModelTime;
      }
    }

    public virtual bool IsSelected
    {
      get
      {
        return this.isSelected;
      }
      set
      {
        this.isSelected = value;
        this.OnPropertyChanged("IsSelected");
        IKeyFramedTimelineItem framedTimelineItem = this.TimelineItem as IKeyFramedTimelineItem;
        if (framedTimelineItem == null)
          return;
        framedTimelineItem.OnKeyFrameSelectionChanged();
      }
    }

    public TimelinePane TimelinePane
    {
      get
      {
        return this.timelineItemManager.TimelinePane;
      }
    }

    public TimelineItemManager TimelineItemManager
    {
      get
      {
        return this.timelineItemManager;
      }
    }

    public abstract IList<KeyFrameSceneNode> KeyFrameSceneNodesToSelect { get; }

    protected virtual bool IsSelectable
    {
      get
      {
        TimelineItem timelineItem = this.TimelineItem;
        ElementTimelineItem elementTimelineItem = timelineItem as ElementTimelineItem;
        for (StyleTimelineItem styleTimelineItem = timelineItem as StyleTimelineItem; elementTimelineItem == null && styleTimelineItem == null && timelineItem != null; styleTimelineItem = timelineItem as StyleTimelineItem)
        {
          timelineItem = timelineItem.Parent;
          elementTimelineItem = timelineItem as ElementTimelineItem;
        }
        return timelineItem.IsSelectable;
      }
    }

    protected abstract SimpleKeyFrameItem KeyFrameItemToEdit { get; }

    public event PropertyChangedEventHandler PropertyChanged;

    protected KeyFrameItem(double time, TimelineItem timelineItem)
    {
      this.timelineItemManager = timelineItem.TimelineItemManager;
      this.location = new KeyFrameItem.KeyFrameLocation(time, timelineItem);
      KeyFrameItem.locationToItemTable.Add(this.location, this);
    }

    public static bool operator ==(KeyFrameItem x, KeyFrameItem y)
    {
      return object.Equals((object) x, (object) y);
    }

    public static bool operator !=(KeyFrameItem x, KeyFrameItem y)
    {
      return !object.Equals((object) x, (object) y);
    }

    public static bool operator >(KeyFrameItem x, KeyFrameItem y)
    {
      if (x != (KeyFrameItem) null)
        return x.CompareTo((object) y) > 0;
      return false;
    }

    public static bool operator <(KeyFrameItem x, KeyFrameItem y)
    {
      if (x != (KeyFrameItem) null)
        return x.CompareTo((object) y) < 0;
      return false;
    }

    public int CompareTo(object obj)
    {
      int num = 0;
      KeyFrameItem keyFrameItem = obj as KeyFrameItem;
      if (keyFrameItem != (KeyFrameItem) null && this.Time != keyFrameItem.Time)
        num = this.Time > keyFrameItem.Time ? 1 : -1;
      return num;
    }

    public override bool Equals(object obj)
    {
      KeyFrameItem keyFrameItem = obj as KeyFrameItem;
      if (keyFrameItem != (KeyFrameItem) null)
        return object.Equals((object) keyFrameItem.location, (object) this.location);
      return false;
    }

    public override int GetHashCode()
    {
      return this.location.GetHashCode();
    }

    public bool SetTimeFromModel(double time)
    {
      bool flag = false;
      KeyFrameItem.locationToItemTable.Remove(this.location);
      this.localTime = double.NaN;
      this.location.ModelTime = time;
      if (!KeyFrameItem.locationToItemTable.ContainsKey(this.location))
      {
        KeyFrameItem.locationToItemTable.Add(this.location, this);
        flag = true;
        this.OnPropertyChanged("Time");
      }
      return flag;
    }

    public static KeyFrameItem FindItem(double time, TimelineItem timelineItem)
    {
      KeyFrameItem keyFrameItem = (KeyFrameItem) null;
      KeyFrameItem.locationToItemTable.TryGetValue(new KeyFrameItem.KeyFrameLocation(time, timelineItem), out keyFrameItem);
      return keyFrameItem;
    }

    public void RemoveFromModel()
    {
      KeyFrameItem.locationToItemTable.Remove(this.location);
    }

    protected void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }

    private void Select()
    {
      KeyFrameSelectionSet frameSelectionSet = this.TimelineItemManager.ViewModel.KeyFrameSelectionSet;
      if (this.IsSelectable)
        frameSelectionSet.SetSelection((ICollection<KeyFrameSceneNode>) this.KeyFrameSceneNodesToSelect, (KeyFrameSceneNode) null);
      else
        frameSelectionSet.Clear();
    }

    private void ToggleSelect()
    {
      if (!this.IsSelectable)
        return;
      KeyFrameSelectionSet frameSelectionSet = this.TimelineItemManager.ViewModel.KeyFrameSelectionSet;
      if (this.IsSelected)
        frameSelectionSet.RemoveSelection((ICollection<KeyFrameSceneNode>) this.KeyFrameSceneNodesToSelect);
      else
        frameSelectionSet.ExtendSelection((ICollection<KeyFrameSceneNode>) this.KeyFrameSceneNodesToSelect);
    }

    private void ExtendSelect()
    {
      if (!this.IsSelectable)
        return;
      this.TimelineItemManager.ViewModel.KeyFrameSelectionSet.ExtendSelection((ICollection<KeyFrameSceneNode>) this.KeyFrameSceneNodesToSelect);
    }

    private void EnsureSelect()
    {
      KeyFrameSelectionSet frameSelectionSet = this.timelineItemManager.ViewModel.KeyFrameSelectionSet;
      if (this.IsSelectable)
      {
        if (this.IsSelected)
          return;
        frameSelectionSet.SetSelection((ICollection<KeyFrameSceneNode>) this.KeyFrameSceneNodesToSelect, (KeyFrameSceneNode) null);
      }
      else
        frameSelectionSet.Clear();
    }

    private void BeginEdit()
    {
      this.TimelinePane.TimelineView.EditingKeyFrameItem = (KeyFrameItem) this.KeyFrameItemToEdit;
    }

    private struct KeyFrameLocation
    {
      private double modelTime;
      private TimelineItem timelineItem;

      public double ModelTime
      {
        get
        {
          return this.modelTime;
        }
        set
        {
          this.modelTime = value;
        }
      }

      public TimelineItem TimelineItem
      {
        get
        {
          return this.timelineItem;
        }
      }

      public KeyFrameLocation(double modelTime, TimelineItem timelineItem)
      {
        this.modelTime = modelTime;
        this.timelineItem = timelineItem;
      }

      public static bool operator ==(KeyFrameItem.KeyFrameLocation lhs, KeyFrameItem.KeyFrameLocation rhs)
      {
        if (lhs.modelTime == rhs.modelTime)
          return lhs.timelineItem == rhs.timelineItem;
        return false;
      }

      public static bool operator !=(KeyFrameItem.KeyFrameLocation lhs, KeyFrameItem.KeyFrameLocation rhs)
      {
        return !(lhs == rhs);
      }

      public override bool Equals(object obj)
      {
        KeyFrameItem.KeyFrameLocation? nullable = obj as KeyFrameItem.KeyFrameLocation?;
        if (nullable.HasValue && nullable.Value.modelTime == this.modelTime)
          return nullable.Value.timelineItem == this.timelineItem;
        return false;
      }

      public override int GetHashCode()
      {
        return this.timelineItem.GetHashCode() ^ this.modelTime.GetHashCode();
      }
    }
  }
}
