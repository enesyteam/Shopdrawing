// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Timeline.UI.VerticalScrollManager
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Expression.DesignSurface.UserInterface.Timeline.UI
{
  public class VerticalScrollManager : ScrollManager
  {
    private double valueOld;

    protected override void OnScrollBarValueChange(RoutedPropertyChangedEventArgs<double> e)
    {
      this.valueOld = Math.Floor(e.NewValue);
      this.SyncViewers(e.NewValue);
    }

    protected override void OnViewerLayoutUpdated()
    {
      this.UpdateScrollExtents();
      this.SyncViewers(this.valueOld);
    }

    protected override void OnViewerScrollChange(ScrollChangedEventArgs e)
    {
      double verticalOffset = e.VerticalOffset;
      if (e.VerticalChange == 0.0 || verticalOffset == this.valueOld)
        return;
      this.ScrollBar.Value = verticalOffset;
      this.valueOld = verticalOffset;
    }

    private void SyncViewers(double value)
    {
      foreach (ScrollViewer scrollViewer in this.Viewers)
      {
        if (scrollViewer.VerticalOffset != value)
          scrollViewer.ScrollToVerticalOffset(value);
      }
    }

    private void UpdateScrollExtents()
    {
      ScrollViewer scrollViewer = (ScrollViewer) null;
      for (int index = 0; index < this.Viewers.Count; ++index)
      {
        scrollViewer = (ScrollViewer) this.Viewers[index];
        if (scrollViewer.IsVisible)
          break;
      }
      if (scrollViewer == null)
        return;
      double num = Math.Max(0.0, scrollViewer.ExtentHeight - scrollViewer.ViewportHeight);
      if (num != this.ScrollBar.Maximum)
        this.ScrollBar.Maximum = num;
      bool flag = this.ScrollBar.Maximum != this.ScrollBar.Minimum;
      if (this.ScrollBar.IsEnabled != flag)
        this.ScrollBar.IsEnabled = flag;
      if (this.ScrollBar.LargeChange != scrollViewer.ViewportHeight)
        this.ScrollBar.LargeChange = scrollViewer.ViewportHeight;
      if (this.ScrollBar.ViewportSize == scrollViewer.ViewportHeight)
        return;
      this.ScrollBar.ViewportSize = scrollViewer.ViewportHeight;
    }
  }
}
