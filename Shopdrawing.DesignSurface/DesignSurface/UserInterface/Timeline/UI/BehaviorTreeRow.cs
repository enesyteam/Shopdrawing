// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Timeline.UI.BehaviorTreeRow
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.UserInterface.Timeline;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.UserInterface.Timeline.UI
{
  public sealed class BehaviorTreeRow : TimelineTreeRow
  {
    private BehaviorTimelineItem behaviorTimelineItem;
    private bool hasHookedPropertyChanged;

    public override TimelineItem TimelineItem
    {
      get
      {
        return base.TimelineItem;
      }
      set
      {
        base.TimelineItem = value;
        BehaviorTimelineItem behaviorTimelineItem = this.TimelineItem as BehaviorTimelineItem;
        if (this.behaviorTimelineItem != null && this.hasHookedPropertyChanged)
        {
          this.behaviorTimelineItem.PropertyChanged -= new PropertyChangedEventHandler(this.OnBehaviorTimelineItemPropertyChanged);
          this.hasHookedPropertyChanged = false;
        }
        this.behaviorTimelineItem = behaviorTimelineItem;
        if (this.behaviorTimelineItem == null)
          return;
        this.behaviorTimelineItem.RenameCommand = this.RenameCommand;
        this.behaviorTimelineItem.PropertyChanged += new PropertyChangedEventHandler(this.OnBehaviorTimelineItemPropertyChanged);
        this.hasHookedPropertyChanged = true;
      }
    }

    protected override bool CustomLayout
    {
      get
      {
        return this.TimelineItem is BehaviorTimelineItem;
      }
    }

    public BehaviorTreeRow()
    {
      this.Loaded += new RoutedEventHandler(this.OnBehaviorTreeRowLoaded);
      this.Unloaded += new RoutedEventHandler(this.OnBehaviorTreeRowUnloaded);
    }

    private void OnBehaviorTreeRowLoaded(object sender, RoutedEventArgs e)
    {
      if (this.hasHookedPropertyChanged || this.behaviorTimelineItem == null)
        return;
      this.behaviorTimelineItem.PropertyChanged += new PropertyChangedEventHandler(this.OnBehaviorTimelineItemPropertyChanged);
      this.hasHookedPropertyChanged = true;
    }

    private void OnBehaviorTreeRowUnloaded(object sender, RoutedEventArgs e)
    {
      if (!this.hasHookedPropertyChanged || this.behaviorTimelineItem == null)
        return;
      this.behaviorTimelineItem.PropertyChanged -= new PropertyChangedEventHandler(this.OnBehaviorTimelineItemPropertyChanged);
      this.hasHookedPropertyChanged = false;
    }

    private void OnBehaviorTimelineItemPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == "ViewportWidth") && !(e.PropertyName == "ViewportOffset"))
        return;
      this.InvalidateMeasure();
      this.InvalidateArrange();
    }

    protected override void OnRenameCommandChanged(ICommand newCommand)
    {
      BehaviorTimelineItem behaviorTimelineItem = this.TimelineItem as BehaviorTimelineItem;
      if (behaviorTimelineItem == null)
        return;
      behaviorTimelineItem.RenameCommand = newCommand;
    }
  }
}
