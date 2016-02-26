// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.AdornerToolTipService
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.UserInterface;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Microsoft.Expression.DesignSurface.Tools
{
  internal class AdornerToolTipService
  {
    private DispatcherTimer timer;
    private IAdorner mouseOverAdorner;
    private Adorner adornerWithOpenToolTip;
    private ToolTip toolTip;

    public IAdorner MouseOverAdorner
    {
      get
      {
        return this.mouseOverAdorner;
      }
      set
      {
        Adorner adorner1 = this.mouseOverAdorner as Adorner;
        Adorner adorner2 = value as Adorner;
        if (adorner1 != adorner2)
        {
          if (adorner1 != null && PresentationSource.FromVisual((Visual) adorner1) != null)
            adorner1.OnMouseLeave();
          if (adorner2 != null)
            adorner2.OnMouseEnter();
        }
        this.mouseOverAdorner = value;
        if (this.adornerWithOpenToolTip != null && this.adornerWithOpenToolTip != adorner2)
        {
          bool flag = false;
          if (this.toolTip != null && this.toolTip.IsOpen)
            flag = new Rect(this.toolTip.RenderSize).Contains(Mouse.GetPosition((IInputElement) this.toolTip));
          if (!flag)
            this.Close();
        }
        this.StopTimer();
        if (adorner2 == null || this.adornerWithOpenToolTip == adorner2)
          return;
        this.timer = new DispatcherTimer(DispatcherPriority.Normal);
        this.timer.Interval = SystemParameters.MouseHoverTime;
        this.timer.Tick += new EventHandler(this.OpenToolTipTimer_Tick);
        this.timer.Start();
      }
    }

    private void Close()
    {
      if (this.toolTip == null)
        return;
      this.toolTip.Closed -= new RoutedEventHandler(this.ToolTip_Closed);
      this.toolTip.IsOpen = false;
      this.toolTip = (ToolTip) null;
      this.adornerWithOpenToolTip = (Adorner) null;
    }

    private void StopTimer()
    {
      if (this.timer == null)
        return;
      this.timer.Stop();
      this.timer = (DispatcherTimer) null;
    }

    private void OpenToolTipTimer_Tick(object sender, EventArgs e)
    {
      this.StopTimer();
      Adorner adorner = this.mouseOverAdorner as Adorner;
      if (adorner == null)
        return;
      object toolTip = adorner.ToolTip;
      if (toolTip == null)
        return;
      this.toolTip = toolTip as ToolTip;
      if (this.toolTip == null)
      {
        this.toolTip = new ToolTip();
        this.toolTip.Content = toolTip;
      }
      this.toolTip.Closed += new RoutedEventHandler(this.ToolTip_Closed);
      this.toolTip.IsOpen = true;
      this.adornerWithOpenToolTip = adorner;
    }

    private void ToolTip_Closed(object sender, RoutedEventArgs e)
    {
      if (this.toolTip == null)
        return;
      this.toolTip.Closed -= new RoutedEventHandler(this.ToolTip_Closed);
      this.toolTip.IsOpen = false;
      this.toolTip = (ToolTip) null;
      this.adornerWithOpenToolTip = (Adorner) null;
    }
  }
}
