// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.FloatingWindowManager
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.PlatformUI.Shell.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Microsoft.VisualStudio.PlatformUI.Shell
{
  public class FloatingWindowManager
  {
    private Dictionary<FloatSite, FloatingWindow> floatingWindows = new Dictionary<FloatSite, FloatingWindow>();
    private List<FloatingWindow> delayShownElements = new List<FloatingWindow>();
    private IntPtr ownerWindow;

    public IntPtr OwnerWindow
    {
      get
      {
        return this.ownerWindow;
      }
      set
      {
        if (!(this.ownerWindow != value))
          return;
        this.ownerWindow = value;
        this.UpdateFloatingOwners();
        if (!this.CanShowFloatingWindows)
          return;
        this.ShowDelayedElements();
      }
    }

    private bool CanShowFloatingWindows
    {
      get
      {
        return this.OwnerWindow != IntPtr.Zero;
      }
    }

    public int MonitorPadding { get; set; }

    public FloatingWindowManager()
    {
      this.MonitorPadding = 20;
    }

    private void DelayShowElement(FloatingWindow element)
    {
      this.delayShownElements.Add(element);
    }

    private void ShowDelayedElements()
    {
      foreach (Window window in this.delayShownElements)
        window.Show();
      this.delayShownElements.Clear();
    }

    public void RemoveAllFloats(WindowProfile profile)
    {
      foreach (ViewElement viewElement in (IEnumerable<ViewElement>) profile.Children)
      {
        FloatSite floatSite = viewElement as FloatSite;
        if (floatSite != null)
          this.RemoveFloat(floatSite);
      }
    }

    public void AddFloat(FloatSite floatSite)
    {
      floatSite.IsVisibleChanged += new EventHandler(this.OnFloatSiteIsVisibleChanged);
      if (!floatSite.IsVisible)
        return;
      this.ShowFloat(floatSite);
    }

    public void RemoveFloat(FloatSite floatSite)
    {
      floatSite.IsVisibleChanged -= new EventHandler(this.OnFloatSiteIsVisibleChanged);
      this.CloseFloat(floatSite);
    }

    private void ShowFloat(FloatSite floatSite)
    {
      FloatingWindow floatingControl = this.GetFloatingControl(floatSite);
      floatingControl.DataContext = (object) floatSite;
      this.EnsureFloatingSiteInVisibleBounds(floatSite);
      if (this.CanShowFloatingWindows)
        floatingControl.Show();
      else
        this.DelayShowElement(floatingControl);
    }

    private void CloseFloat(FloatSite floatSite)
    {
      FloatingWindow floatingWindow;
      if (!this.floatingWindows.TryGetValue(floatSite, out floatingWindow))
        return;
      floatingWindow.ForceClose();
      this.floatingWindows.Remove(floatSite);
      this.delayShownElements.Remove(floatingWindow);
    }

    protected virtual FloatingWindow CreateFloatingWindow(FloatSite floatSite)
    {
      return new FloatingWindow();
    }

    private FloatingWindow GetFloatingControl(FloatSite floatSite)
    {
      FloatingWindow floatingWindow;
      if (!this.floatingWindows.TryGetValue(floatSite, out floatingWindow))
      {
        floatingWindow = this.CreateFloatingWindow(floatSite);
        floatingWindow.Closing += new CancelEventHandler(this.OnFloatingControlClosing);
        new WindowInteropHelper((Window) floatingWindow).Owner = this.OwnerWindow;
        this.floatingWindows[floatSite] = floatingWindow;
      }
      return floatingWindow;
    }

    private void OnFloatSiteIsVisibleChanged(object sender, EventArgs args)
    {
      FloatSite floatSite = (FloatSite) sender;
      if (floatSite.IsVisible)
        this.ShowFloat(floatSite);
      else
        this.CloseFloat(floatSite);
    }

    public void ActivateFloatingControl(ViewElement element)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      FloatSite key = ViewElement.FindRootElement(element) as FloatSite;
      FloatingWindow floatingWindow;
      if (key == null || !this.floatingWindows.TryGetValue(key, out floatingWindow))
        return;
      floatingWindow.Activate();
    }

    private void OnFloatingControlClosing(object sender, CancelEventArgs args)
    {
      FloatSite floatSite = ((FrameworkElement) sender).DataContext as FloatSite;
      if (floatSite != null)
      {
        foreach (View view in floatSite.FindAll((Predicate<ViewElement>) (element => element is View)))
          view.Hide();
      }
      args.Cancel = floatSite.IsVisible;
    }

    private void UpdateFloatingOwners()
    {
      foreach (Window window in this.floatingWindows.Values)
        new WindowInteropHelper(window).Owner = this.OwnerWindow;
    }

    public void EnsureFloatingSiteInVisibleBounds(FloatSite site)
    {
      Rect logicalRect = new Rect(site.FloatingLeft, site.FloatingTop, site.FloatingWidth, site.FloatingHeight);
      RECT lprc = new RECT(DpiHelper.LogicalToDeviceUnits(logicalRect));
      IntPtr hMonitor = Microsoft.VisualStudio.PlatformUI.NativeMethods.MonitorFromRect(ref lprc, 1U);
      MONITORINFO monitorInfo = new MONITORINFO();
      monitorInfo.cbSize = (uint) Marshal.SizeOf((object) monitorInfo);
      if (!Microsoft.VisualStudio.PlatformUI.NativeMethods.GetMonitorInfo(hMonitor, ref monitorInfo))
        return;
      Rect rect = DpiHelper.DeviceToLogicalUnits(monitorInfo.rcWork.WPFRectValue);
      bool flag1 = logicalRect.Right < rect.Left + (double) this.MonitorPadding;
      bool flag2 = logicalRect.Left > rect.Right - (double) this.MonitorPadding;
      bool flag3 = logicalRect.Top < rect.Top + (double) this.MonitorPadding;
      bool flag4 = logicalRect.Top > rect.Bottom - (double) this.MonitorPadding;
      if (!flag1 && !flag2 && (!flag3 && !flag4))
        return;
      if (flag1)
        site.FloatingLeft = rect.Left;
      else if (flag2)
        site.FloatingLeft = rect.Right - site.FloatingWidth;
      if (flag3)
      {
        site.FloatingTop = rect.Top;
      }
      else
      {
        if (!flag4)
          return;
        site.FloatingTop = rect.Bottom - site.FloatingHeight;
      }
    }
  }
}
