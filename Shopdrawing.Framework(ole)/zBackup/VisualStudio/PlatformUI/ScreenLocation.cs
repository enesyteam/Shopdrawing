// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.ScreenLocation
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace Microsoft.VisualStudio.PlatformUI
{
  public static class ScreenLocation
  {
    private static Dictionary<UIElement, ScreenLocation.Monitor> monitors = new Dictionary<UIElement, ScreenLocation.Monitor>();
    private static readonly DependencyPropertyKey LeftPropertyKey = DependencyProperty.RegisterAttachedReadOnly("Left", typeof (double), typeof (ScreenLocation), (PropertyMetadata) new FrameworkPropertyMetadata((object) 0.0));
    public static readonly DependencyProperty LeftProperty = ScreenLocation.LeftPropertyKey.DependencyProperty;
    private static readonly DependencyPropertyKey TopPropertyKey = DependencyProperty.RegisterAttachedReadOnly("Top", typeof (double), typeof (ScreenLocation), (PropertyMetadata) new FrameworkPropertyMetadata((object) 0.0));
    public static readonly DependencyProperty TopProperty = ScreenLocation.TopPropertyKey.DependencyProperty;
    private static readonly DependencyPropertyKey RightPropertyKey = DependencyProperty.RegisterAttachedReadOnly("Right", typeof (double), typeof (ScreenLocation), (PropertyMetadata) new FrameworkPropertyMetadata((object) 0.0));
    public static readonly DependencyProperty RightProperty = ScreenLocation.RightPropertyKey.DependencyProperty;
    private static readonly DependencyPropertyKey BottomPropertyKey = DependencyProperty.RegisterAttachedReadOnly("Bottom", typeof (double), typeof (ScreenLocation), (PropertyMetadata) new FrameworkPropertyMetadata((object) 0.0));
    public static readonly DependencyProperty BottomProperty = ScreenLocation.BottomPropertyKey.DependencyProperty;

    public static void AttachMonitor(UIElement element)
    {
      ScreenLocation.Monitor monitor1;
      if (ScreenLocation.monitors.TryGetValue(element, out monitor1))
        return;
      ScreenLocation.Monitor monitor2 = new ScreenLocation.Monitor(element);
      ScreenLocation.monitors.Add(element, monitor2);
    }

    public static void DetachMonitor(UIElement element)
    {
      ScreenLocation.Monitor monitor;
      if (!ScreenLocation.monitors.TryGetValue(element, out monitor))
        return;
      monitor.Dispose();
      ScreenLocation.monitors.Remove(element);
    }

    public static HwndSource FindTopLevelHwndSource(UIElement element)
    {
      HwndSource hwndSource = (HwndSource) PresentationSource.FromVisual((Visual) element);
      if (hwndSource != null && ScreenLocation.IsChildWindow(hwndSource.Handle))
        hwndSource = HwndSource.FromHwnd(ScreenLocation.FindTopLevelWindow(hwndSource.Handle));
      return hwndSource;
    }

    private static bool IsChildWindow(IntPtr hWnd)
    {
      return (NativeMethods.GetWindowLong(hWnd, NativeMethods.GWL.STYLE) & 1073741824) == 1073741824;
    }

    private static IntPtr FindTopLevelWindow(IntPtr hWnd)
    {
      while (hWnd != IntPtr.Zero && ScreenLocation.IsChildWindow(hWnd))
        hWnd = NativeMethods.GetParent(hWnd);
      return hWnd;
    }

    public static double GetLeft(UIElement element)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      return (double) element.GetValue(ScreenLocation.LeftProperty);
    }

    private static void SetLeft(UIElement element, double value)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      element.SetValue(ScreenLocation.LeftPropertyKey, (object) value);
    }

    public static double GetTop(UIElement element)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      return (double) element.GetValue(ScreenLocation.TopProperty);
    }

    private static void SetTop(UIElement element, double value)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      element.SetValue(ScreenLocation.TopPropertyKey, (object) value);
    }

    public static double GetRight(UIElement element)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      return (double) element.GetValue(ScreenLocation.RightProperty);
    }

    private static void SetRight(UIElement element, double value)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      element.SetValue(ScreenLocation.RightPropertyKey, (object) value);
    }

    public static double GetBottom(UIElement element)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      return (double) element.GetValue(ScreenLocation.BottomProperty);
    }

    private static void SetBottom(UIElement element, double value)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      element.SetValue(ScreenLocation.BottomPropertyKey, (object) value);
    }

    private sealed class Monitor : DisposableObject
    {
      private HwndSource hwndSource;

      public UIElement Element { get; set; }

      public HwndSource HwndSource
      {
        get
        {
          return this.hwndSource;
        }
        set
        {
          if (this.hwndSource != null)
            this.hwndSource.RemoveHook(new HwndSourceHook(this.HostingHwndProc));
          this.hwndSource = value;
          if (this.hwndSource == null)
            return;
          this.hwndSource.AddHook(new HwndSourceHook(this.HostingHwndProc));
        }
      }

      public Monitor(UIElement element)
      {
        this.Element = element;
        this.HwndSource = ScreenLocation.FindTopLevelHwndSource(element);
        this.Element.LayoutUpdated += new EventHandler(this.OnLayoutUpdated);
        PresentationSource.AddSourceChangedHandler((IInputElement) element, new SourceChangedEventHandler(this.OnPresentationSourceChanged));
      }

      private void OnLayoutUpdated(object sender, EventArgs e)
      {
        this.TryUpdateLocation();
      }

      public IntPtr HostingHwndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
      {
        if (msg == 71)
          this.TryUpdateLocation();
        return IntPtr.Zero;
      }

      private void OnPresentationSourceChanged(object sender, SourceChangedEventArgs args)
      {
        this.HwndSource = ScreenLocation.FindTopLevelHwndSource((UIElement) sender);
        this.TryUpdateLocation();
      }

      private void TryUpdateLocation()
      {
        if (this.HwndSource == null || !this.Element.IsArrangeValid)
          return;
        Point point = DpiHelper.DeviceToLogicalUnits(this.Element.PointToScreen(new Point(0.0, 0.0)));
        ScreenLocation.SetLeft(this.Element, point.X);
        ScreenLocation.SetTop(this.Element, point.Y);
        ScreenLocation.SetRight(this.Element, point.X + this.Element.RenderSize.Width);
        ScreenLocation.SetBottom(this.Element, point.Y + this.Element.RenderSize.Height);
      }

      protected override void DisposeManagedResources()
      {
        if (this.Element == null)
          return;
        PresentationSource.RemoveSourceChangedHandler((IInputElement) this.Element, new SourceChangedEventHandler(this.OnPresentationSourceChanged));
        this.Element.LayoutUpdated -= new EventHandler(this.OnLayoutUpdated);
        this.HwndSource = (HwndSource) null;
        this.Element = (UIElement) null;
      }
    }
  }
}
