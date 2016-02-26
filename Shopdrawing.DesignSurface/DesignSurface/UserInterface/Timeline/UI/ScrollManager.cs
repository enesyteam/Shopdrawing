// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Timeline.UI.ScrollManager
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Microsoft.Expression.DesignSurface.UserInterface.Timeline.UI
{
  public abstract class ScrollManager : INotifyPropertyChanged
  {
    private ArrayList viewers = new ArrayList();
    private ScrollBar scrollBar;

    public ScrollBar ScrollBar
    {
      get
      {
        return this.scrollBar;
      }
      set
      {
        this.scrollBar = value;
        this.scrollBar.ValueChanged += new RoutedPropertyChangedEventHandler<double>(this.ScrollBar_ValueChanged);
      }
    }

    protected ArrayList Viewers
    {
      get
      {
        return this.viewers;
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    internal void RegisterViewer(ScrollViewer viewer)
    {
      if (viewer == null || this.viewers.Contains((object) viewer))
        return;
      this.viewers.Add((object) viewer);
      viewer.ScrollChanged += new ScrollChangedEventHandler(this.ScrollViewer_ScrollChanged);
      if (this.viewers.Count != 1)
        return;
      viewer.LayoutUpdated += new EventHandler(this.ScrollViewer_LayoutUpdated);
    }

    protected void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }

    private void ScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
      this.OnScrollBarValueChange(e);
    }

    private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
    {
      if (!this.viewers.Contains(e.OriginalSource))
        return;
      this.OnViewerScrollChange(e);
    }

    private void ScrollViewer_LayoutUpdated(object sender, EventArgs e)
    {
      this.OnViewerLayoutUpdated();
    }

    protected abstract void OnScrollBarValueChange(RoutedPropertyChangedEventArgs<double> e);

    protected abstract void OnViewerScrollChange(ScrollChangedEventArgs e);

    protected abstract void OnViewerLayoutUpdated();
  }
}
