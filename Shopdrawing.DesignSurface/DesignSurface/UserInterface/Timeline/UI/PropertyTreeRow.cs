// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Timeline.UI.PropertyTreeRow
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.UserInterface.Timeline;
using System.ComponentModel;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.UserInterface.Timeline.UI
{
  public sealed class PropertyTreeRow : TimelineTreeRow
  {
    private ChildPropertyTimelineItem propertyTimelineItem;
    private bool hasHookedPropertyChanged;

    public override TimelineItem TimelineItem
    {
      get
      {
        return base.TimelineItem;
      }
      set
      {
        ChildPropertyTimelineItem propertyTimelineItem = value as ChildPropertyTimelineItem;
        if (this.propertyTimelineItem != null && this.hasHookedPropertyChanged)
        {
          this.propertyTimelineItem.PropertyChanged -= new PropertyChangedEventHandler(this.propertyTimelineItem_PropertyChanged);
          this.hasHookedPropertyChanged = false;
        }
        this.propertyTimelineItem = propertyTimelineItem;
        this.propertyTimelineItem.PropertyChanged += new PropertyChangedEventHandler(this.propertyTimelineItem_PropertyChanged);
        this.hasHookedPropertyChanged = true;
        base.TimelineItem = value;
      }
    }

    protected override bool CustomLayout
    {
      get
      {
        return this.TimelineItem is EffectTimelineItem;
      }
    }

    public PropertyTreeRow()
    {
      this.Loaded += new RoutedEventHandler(this.PropertyTreeRow_Loaded);
      this.Unloaded += new RoutedEventHandler(this.PropertyTreeRow_Unloaded);
    }

    private void PropertyTreeRow_Loaded(object sender, RoutedEventArgs e)
    {
      if (this.hasHookedPropertyChanged || this.propertyTimelineItem == null)
        return;
      this.propertyTimelineItem.PropertyChanged += new PropertyChangedEventHandler(this.propertyTimelineItem_PropertyChanged);
      this.hasHookedPropertyChanged = true;
    }

    private void PropertyTreeRow_Unloaded(object sender, RoutedEventArgs e)
    {
      if (!this.hasHookedPropertyChanged || this.propertyTimelineItem == null)
        return;
      this.propertyTimelineItem.PropertyChanged -= new PropertyChangedEventHandler(this.propertyTimelineItem_PropertyChanged);
      this.hasHookedPropertyChanged = false;
    }

    private void propertyTimelineItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == "ViewportWidth") && !(e.PropertyName == "ViewportOffset"))
        return;
      this.InvalidateMeasure();
      this.InvalidateArrange();
    }
  }
}
