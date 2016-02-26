// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Timeline.UI.HorizontalScrollManager
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.Framework.Controls;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Expression.DesignSurface.UserInterface.Timeline.UI
{
  public class HorizontalScrollManager : ScrollManager
  {
    private double visibleWidth = 1.0;
    private const double AdditionalScrollSeconds = 1.0;
    private const double MinimumThumbWidth = 20.0;
    private TimelineView timelineView;
    private double valueOld;
    private bool wasCurrentTimeVisible;
    private double centerTime;
    private TimeMarkerControl timeMarkerControl;

    public double VisibleWidth
    {
      get
      {
        return this.visibleWidth;
      }
    }

    public HorizontalScrollManager(TimelineView timelineView)
    {
      this.timelineView = timelineView;
      this.timelineView.PropertyChanged += new PropertyChangedEventHandler(this.TimelineView_PropertyChanged);
      this.timeMarkerControl = (TimeMarkerControl) ElementUtilities.FindElement((FrameworkElement) timelineView, "TimeMarkerControl");
    }

    protected override void OnScrollBarValueChange(RoutedPropertyChangedEventArgs<double> e)
    {
      this.valueOld = e.NewValue;
      foreach (ScrollViewer scrollViewer in this.Viewers)
      {
        if (scrollViewer.HorizontalOffset != e.NewValue)
          scrollViewer.ScrollToHorizontalOffset(e.NewValue);
      }
      this.UpdateCenter();
      this.timeMarkerControl.UpdateMarkers(false);
    }

    protected override void OnViewerScrollChange(ScrollChangedEventArgs e)
    {
      double horizontalOffset = e.HorizontalOffset;
      if (e.HorizontalChange == 0.0 || horizontalOffset == this.valueOld)
        return;
      this.ScrollBar.Value = horizontalOffset;
      this.valueOld = horizontalOffset;
    }

    protected override void OnViewerLayoutUpdated()
    {
      ScrollViewer scrollViewer = (ScrollViewer) this.Viewers[0];
      if (scrollViewer.RenderSize.Width != this.visibleWidth)
      {
        this.visibleWidth = scrollViewer.RenderSize.Width;
        this.UpdateCenter();
        this.OnPropertyChanged("VisibleWidth");
        this.UpdateScrollExtents(false);
      }
      if (this.ScrollBar.ViewportSize == this.visibleWidth)
        return;
      this.ScrollBar.ViewportSize = this.visibleWidth;
    }

    private void UpdateScrollExtents(bool isZooming)
    {
      ScrollViewer scrollViewer = (ScrollViewer) this.Viewers[0];
      ExtendedScrollBar extendedScrollBar = (ExtendedScrollBar) this.ScrollBar;
      double num1 = TimelineView.PositionFromSeconds(Math.Max(this.timelineView.LastSignificantTime, TimelineView.SecondsFromPosition(scrollViewer.RenderSize.Width)) + 1.0) - scrollViewer.RenderSize.Width;
      if (isZooming)
      {
        if (this.wasCurrentTimeVisible)
          this.centerTime = this.timelineView.CurrentTime;
        double num2 = Math.Max(0.0, TimelineView.PositionFromSeconds(this.centerTime) - this.visibleWidth / 2.0);
        extendedScrollBar.ContentMaximum = num1;
        extendedScrollBar.Value = num2;
      }
      else
        extendedScrollBar.ContentMaximum = num1;
    }

    private void UpdateCenter()
    {
      double num = TimelineView.PositionFromSeconds(this.timelineView.CurrentTime);
      this.wasCurrentTimeVisible = num >= this.ScrollBar.Value && num <= this.ScrollBar.Value + this.visibleWidth;
      this.centerTime = TimelineView.SecondsFromPosition(this.ScrollBar.Value + this.visibleWidth / 2.0);
    }

    private void TimelineView_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      switch (e.PropertyName)
      {
        case "UnitsPerSecond":
          this.UpdateScrollExtents(true);
          break;
        case "LastSignificantTime":
          this.UpdateScrollExtents(false);
          break;
        case "CurrentTime":
          this.UpdateCenter();
          break;
      }
    }
  }
}
