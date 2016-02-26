// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.Controls.WindowResizeFrame
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace Microsoft.VisualStudio.PlatformUI.Shell.Controls
{
  public class WindowResizeFrame : Decorator
  {
    public static readonly DependencyProperty GripThicknessProperty = DependencyProperty.Register("GripThickness", typeof (double), typeof (WindowResizeFrame), (PropertyMetadata) new FrameworkPropertyMetadata((object) 4.0));
    public static readonly DependencyProperty CornerGripThicknessProperty = DependencyProperty.Register("CornerGripThickness", typeof (double), typeof (WindowResizeFrame), (PropertyMetadata) new FrameworkPropertyMetadata((object) 12.0));

    public double GripThickness
    {
      get
      {
        return (double) this.GetValue(WindowResizeFrame.GripThicknessProperty);
      }
      set
      {
        this.SetValue(WindowResizeFrame.GripThicknessProperty, (object) value);
      }
    }

    public double CornerGripThickness
    {
      get
      {
        return (double) this.GetValue(WindowResizeFrame.CornerGripThicknessProperty);
      }
      set
      {
        this.SetValue(WindowResizeFrame.CornerGripThicknessProperty, (object) value);
      }
    }

    private WindowResizeGripDirection HitTest(Point point)
    {
      double gripThickness = this.GripThickness;
      double cornerGripThickness = this.CornerGripThickness;
      double width1 = this.ActualWidth - 2.0 * cornerGripThickness;
      double height1 = this.ActualHeight - 2.0 * cornerGripThickness;
      double width2 = this.ActualWidth - 2.0 * gripThickness;
      double height2 = this.ActualHeight - 2.0 * gripThickness;
      if (this.RectContainsPointExclusive(point.X, point.Y, gripThickness, gripThickness, width2, height2))
        return WindowResizeGripDirection.None;
      if (this.RectContainsPointInclusive(point.X, point.Y, 0.0, cornerGripThickness, gripThickness, height1))
        return WindowResizeGripDirection.Left;
      if (this.RectContainsPointInclusive(point.X, point.Y, gripThickness + width2, cornerGripThickness, gripThickness, height1))
        return WindowResizeGripDirection.Right;
      if (this.RectContainsPointInclusive(point.X, point.Y, cornerGripThickness, 0.0, width1, gripThickness))
        return WindowResizeGripDirection.Top;
      if (this.RectContainsPointInclusive(point.X, point.Y, cornerGripThickness, gripThickness + height2, width1, gripThickness))
        return WindowResizeGripDirection.Bottom;
      if (this.RectContainsPointInclusive(point.X, point.Y, 0.0, 0.0, cornerGripThickness, cornerGripThickness))
        return WindowResizeGripDirection.TopLeft;
      if (this.RectContainsPointInclusive(point.X, point.Y, cornerGripThickness + width1, 0.0, cornerGripThickness, cornerGripThickness))
        return WindowResizeGripDirection.TopRight;
      if (this.RectContainsPointInclusive(point.X, point.Y, cornerGripThickness + width1, cornerGripThickness + height1, cornerGripThickness, cornerGripThickness))
        return WindowResizeGripDirection.BottomRight;
      return this.RectContainsPointInclusive(point.X, point.Y, 0.0, cornerGripThickness + height1, cornerGripThickness, cornerGripThickness) ? WindowResizeGripDirection.BottomLeft : WindowResizeGripDirection.None;
    }

    private bool RectContainsPointInclusive(double x, double y, double left, double top, double width, double height)
    {
      if (x >= left && x - width <= left && y >= top)
        return y - height <= top;
      return false;
    }

    private bool RectContainsPointExclusive(double x, double y, double left, double top, double width, double height)
    {
      if (x > left && x - width < left && y > top)
        return y - height < top;
      return false;
    }

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
      base.OnMouseDown(e);
      WindowResizeGripDirection direction = this.HitTest(e.GetPosition((IInputElement) this));
      if (direction == WindowResizeGripDirection.None)
        return;
      this.BeginResize(direction);
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
      base.OnMouseMove(e);
      this.UpdateCursor(this.HitTest(e.GetPosition((IInputElement) this)));
    }

    protected override void OnMouseEnter(MouseEventArgs e)
    {
      base.OnMouseEnter(e);
      this.UpdateCursor(this.HitTest(e.GetPosition((IInputElement) this)));
    }

    private void UpdateCursor(WindowResizeGripDirection direction)
    {
      switch (direction)
      {
        case WindowResizeGripDirection.None:
          this.Cursor = Cursors.Arrow;
          break;
        case WindowResizeGripDirection.Left:
        case WindowResizeGripDirection.Right:
          this.Cursor = Cursors.SizeWE;
          break;
        case WindowResizeGripDirection.Top:
        case WindowResizeGripDirection.Bottom:
          this.Cursor = Cursors.SizeNS;
          break;
        case WindowResizeGripDirection.TopLeft:
        case WindowResizeGripDirection.BottomRight:
          this.Cursor = Cursors.SizeNWSE;
          break;
        case WindowResizeGripDirection.TopRight:
        case WindowResizeGripDirection.BottomLeft:
          this.Cursor = Cursors.SizeNESW;
          break;
      }
    }

    private void BeginResize(WindowResizeGripDirection direction)
    {
      HwndSource hwndSource = (HwndSource) PresentationSource.FromVisual((Visual) this);
      if (hwndSource == null)
        return;
      NativeMethods.SendMessage(hwndSource.Handle, 274, (IntPtr) ((int) (61440 + direction)), IntPtr.Zero);
    }

    protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
    {
      if (this.HitTest(hitTestParameters.HitPoint) != WindowResizeGripDirection.None)
        return (HitTestResult) new PointHitTestResult((Visual) this, hitTestParameters.HitPoint);
      return (HitTestResult) null;
    }
  }
}
