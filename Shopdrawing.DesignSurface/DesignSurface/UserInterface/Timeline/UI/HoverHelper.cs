// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Timeline.UI.HoverHelper
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Microsoft.Expression.DesignSurface.UserInterface.Timeline.UI
{
  public class HoverHelper
  {
    private Point hoverStartPoint = new Point();
    private DispatcherTimer timer = new DispatcherTimer();
    private UIElement element;
    private bool isHovering;

    public UIElement Element
    {
      get
      {
        return this.element;
      }
      set
      {
        if (this.element == value)
          return;
        this.Stop();
        this.element = value;
        this.Start();
      }
    }

    public event EventHandler<EventArgs> HoverEnter;

    public event EventHandler<EventArgs> HoverLeave;

    public HoverHelper(UIElement element)
    {
      this.Element = element;
      this.timer.Interval = SystemParameters.MouseHoverTime.Add(SystemParameters.MouseHoverTime);
      this.timer.Tick += new EventHandler(this.Timer_Tick);
    }

    public void Stop()
    {
      this.timer.Stop();
      if (this.element == null)
        return;
      this.element.MouseMove -= new MouseEventHandler(this.Element_MouseMove);
      this.element.MouseEnter -= new MouseEventHandler(this.Element_MouseEnter);
      this.element.MouseLeave -= new MouseEventHandler(this.Element_MouseLeave);
    }

    public void Start()
    {
      if (this.element == null)
        return;
      this.element.MouseMove += new MouseEventHandler(this.Element_MouseMove);
      this.element.MouseEnter += new MouseEventHandler(this.Element_MouseEnter);
      this.element.MouseLeave += new MouseEventHandler(this.Element_MouseLeave);
    }

    public void HandleMouseMove(Point mousePosition)
    {
      this.HandleMouseMove(mousePosition, true);
    }

    public void HandleMouseMove(Point mousePosition, bool hoverCondition)
    {
      if (hoverCondition && Math.Abs(mousePosition.X - this.hoverStartPoint.X) <= SystemParameters.MouseHoverWidth && Math.Abs(mousePosition.Y - this.hoverStartPoint.Y) <= SystemParameters.MouseHoverHeight)
        return;
      if (this.isHovering)
      {
        this.isHovering = false;
        if (this.HoverLeave != null)
          this.HoverLeave((object) this.Element, EventArgs.Empty);
      }
      this.timer.Stop();
      this.timer.Start();
      this.hoverStartPoint = mousePosition;
    }

    public void HandleMouseEnter(Point mousePosition)
    {
      this.hoverStartPoint = mousePosition;
      this.isHovering = false;
      this.timer.Start();
    }

    public void HandleMouseLeave()
    {
      this.isHovering = false;
      this.timer.Stop();
    }

    private void Element_MouseMove(object sender, MouseEventArgs e)
    {
      this.HandleMouseMove(e.GetPosition((IInputElement) this.Element));
    }

    private void Element_MouseEnter(object sender, MouseEventArgs e)
    {
      this.HandleMouseEnter(e.GetPosition((IInputElement) this.Element));
    }

    private void Element_MouseLeave(object sender, MouseEventArgs e)
    {
      this.HandleMouseLeave();
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
      this.timer.Stop();
      this.isHovering = true;
      if (this.HoverEnter == null)
        return;
      this.HoverEnter((object) this.Element, EventArgs.Empty);
    }
  }
}
