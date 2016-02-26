// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Timeline.UI.ElementTreeRow
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.UserInterface.Timeline;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.UserInterface.Timeline.UI
{
  public sealed class ElementTreeRow : TimelineTreeRow
  {
    private ElementTimelineItem elementTimelineItem;
    private bool hasHookedPropertyChanged;

    protected override bool CustomLayout
    {
      get
      {
        return true;
      }
    }

    public override TimelineItem TimelineItem
    {
      get
      {
        return base.TimelineItem;
      }
      set
      {
        if (this.elementTimelineItem != null && this.hasHookedPropertyChanged)
        {
          this.elementTimelineItem.PropertyChanged -= new PropertyChangedEventHandler(this.elementTimelineItem_PropertyChanged);
          this.hasHookedPropertyChanged = false;
        }
        ElementTimelineItem elementTimelineItem = value as ElementTimelineItem;
        if (elementTimelineItem != null)
        {
          this.elementTimelineItem = elementTimelineItem;
          this.elementTimelineItem.RenameCommand = this.RenameCommand;
          this.elementTimelineItem.PropertyChanged += new PropertyChangedEventHandler(this.elementTimelineItem_PropertyChanged);
          this.hasHookedPropertyChanged = true;
        }
        base.TimelineItem = value;
      }
    }

    public ElementTreeRow()
    {
      this.Loaded += new RoutedEventHandler(this.ElementTreeRow_Loaded);
      this.Unloaded += new RoutedEventHandler(this.ElementTreeRow_Unloaded);
    }

    private void ElementTreeRow_Loaded(object sender, RoutedEventArgs e)
    {
      if (this.hasHookedPropertyChanged || this.elementTimelineItem == null)
        return;
      this.elementTimelineItem.PropertyChanged += new PropertyChangedEventHandler(this.elementTimelineItem_PropertyChanged);
      this.hasHookedPropertyChanged = true;
    }

    private void ElementTreeRow_Unloaded(object sender, RoutedEventArgs e)
    {
      if (!this.hasHookedPropertyChanged || this.elementTimelineItem == null)
        return;
      this.elementTimelineItem.PropertyChanged -= new PropertyChangedEventHandler(this.elementTimelineItem_PropertyChanged);
      this.hasHookedPropertyChanged = false;
    }

    protected override void OnInitialized(EventArgs e)
    {
      base.OnInitialized(e);
    }

    private void elementTimelineItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == "ViewportWidth") && !(e.PropertyName == "ViewportOffset"))
        return;
      this.InvalidateMeasure();
      this.InvalidateArrange();
    }

    protected override void OnRenameCommandChanged(ICommand newCommand)
    {
      if (this.elementTimelineItem == null)
        return;
      this.elementTimelineItem.RenameCommand = newCommand;
    }
  }
}
