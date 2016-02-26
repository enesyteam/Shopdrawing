// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.Controls.DockAdornerWindow
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.PlatformUI.Shell;
using System;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;

namespace Microsoft.VisualStudio.PlatformUI.Shell.Controls
{
  public class DockAdornerWindow : ContentControl
  {
    private IntPtr ownerHwnd;
    private DockTarget dockTarget;
    private HwndSource window;
    private DockAdorner innerContent;

    public bool IsDockGroup
    {
      get
      {
        return this.DockDirection == DockDirection.Fill;
      }
    }

    public FrameworkElement AdornedElement { get; set; }

    public DockDirection DockDirection { get; set; }

    public System.Windows.Controls.Orientation? Orientation { get; set; }

    public bool AreOuterTargetsEnabled { get; set; }

    public bool AreInnerTargetsEnabled { get; set; }

    public bool IsInnerCenterTargetEnabled { get; set; }

    public bool AreInnerSideTargetsEnabled { get; set; }

    public DockAdornerWindow(IntPtr ownerHwnd)
    {
      this.ownerHwnd = ownerHwnd;
    }

    public void PrepareAndShow(DockAdornerWindow insertAfter)
    {
      DockTarget dockTarget = this.AdornedElement as DockTarget;
      if (this.dockTarget != dockTarget)
      {
        this.PrepareAndHide();
        this.dockTarget = dockTarget;
      }
      if (this.window == null)
      {
        this.UpdateContent();
        this.window = new HwndSource(new HwndSourceParameters()
        {
          Width = 0,
          Height = 0,
          ParentWindow = this.ownerHwnd,
          UsesPerPixelOpacity = true,
          WindowName = "DockAdornerWindow",
          WindowStyle = -2013265880
        });
        this.window.SizeToContent = SizeToContent.WidthAndHeight;
        this.window.RootVisual = (Visual) this;
        DockManager.Instance.RegisterSite((Visual) this, this.window.Handle);
      }
      this.UpdatePositionAndVisibility(insertAfter);
    }

    public void PrepareAndHide()
    {
      if (this.window == null)
        return;
      DockManager.Instance.UnregisterSite((Visual) this);
      this.Content = (object) (this.innerContent = (DockAdorner) null);
      this.window.Dispose();
      this.window = (HwndSource) null;
    }

    private void UpdatePositionAndVisibility(DockAdornerWindow insertAfter)
    {
      if (!this.IsArrangeValid)
        this.UpdateLayout();
      double actualWidth = this.ActualWidth;
      double actualHeight = this.ActualHeight;
      double num1 = actualWidth - this.AdornedElement.ActualWidth;
      double num2 = actualHeight - this.AdornedElement.ActualHeight;
      Point point1 = DpiHelper.DeviceToLogicalUnits(this.AdornedElement.PointToScreen(new Point(0.0, 0.0)));
      RECT lpRect;
      NativeMethods.GetWindowRect(this.ownerHwnd, out lpRect);
      Point point2 = new Point((double) lpRect.Left, (double) lpRect.Top);
      Vector vector = Point.Subtract(point1, point2);
      double num3 = vector.X - num1 / 2.0;
      double num4 = vector.Y - num2 / 2.0;
      double x = vector.X;
      double num5 = vector.X - actualWidth + this.AdornedElement.ActualWidth;
      double y = vector.Y;
      double num6 = vector.Y - actualHeight + this.AdornedElement.ActualHeight;
      double offsetX = 0.0;
      double offsetY = 0.0;
      switch (this.DockDirection)
      {
        case DockDirection.FirstValue:
          offsetX = num3;
          offsetY = y;
          break;
        case DockDirection.Bottom:
          offsetX = num3;
          offsetY = num6;
          break;
        case DockDirection.Left:
          offsetX = x;
          offsetY = num4;
          break;
        case DockDirection.Right:
          offsetX = num5;
          offsetY = num4;
          break;
        case DockDirection.Fill:
          offsetX = num3;
          offsetY = num4;
          break;
      }
      IntPtr hWndInsertAfter = NativeMethods.HWND_TOPMOST;
      if (insertAfter != null)
        hWndInsertAfter = insertAfter.window.Handle;
      point2.Offset(offsetX, offsetY);
      Point point3 = DpiHelper.LogicalToDeviceUnits(point2);
      NativeMethods.SetWindowPos(this.window.Handle, hWndInsertAfter, (int) point3.X, (int) point3.Y, 0, 0, 593);
    }

    private void UpdateContent()
    {
      DockAdorner dockAdorner = !this.IsDockGroup ? (DockAdorner) new DockSiteAdorner() : (DockAdorner) new DockGroupAdorner();
      dockAdorner.AdornedElement = this.AdornedElement;
      dockAdorner.DockDirection = this.DockDirection;
      dockAdorner.Orientation = this.Orientation;
      dockAdorner.AreOuterTargetsEnabled = this.AreOuterTargetsEnabled;
      dockAdorner.AreInnerTargetsEnabled = this.AreInnerTargetsEnabled;
      dockAdorner.IsInnerCenterTargetEnabled = this.IsInnerCenterTargetEnabled;
      dockAdorner.AreInnerSideTargetsEnabled = this.AreInnerSideTargetsEnabled;
      this.Content = (object) (this.innerContent = dockAdorner);
      this.innerContent.UpdateContent();
    }

    protected override AutomationPeer OnCreateAutomationPeer()
    {
      return (AutomationPeer) new DockAdornerWindowAutomationPeer(this);
    }
  }
}
