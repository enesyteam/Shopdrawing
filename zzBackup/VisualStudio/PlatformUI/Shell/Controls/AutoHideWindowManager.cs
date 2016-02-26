// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.Controls.AutoHideWindowManager
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.PlatformUI.Shell;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace Microsoft.VisualStudio.PlatformUI.Shell.Controls
{
  internal class AutoHideWindowManager
  {
    private View autoHideWindowElement;

    private AutoHideWindow AutoHideWindow { get; set; }

    private AutoHideChannelControl AutoHideChannelControl { get; set; }

    private AutoHideWindowManager.MouseVirtualCaptureObserver MouseObserver { get; set; }

    public View AutoHideWindowElement
    {
      get
      {
        return this.autoHideWindowElement;
      }
      private set
      {
        if (this.autoHideWindowElement != null)
        {
          this.autoHideWindowElement.ParentChanged -= new EventHandler(this.OnElementParentOrVisibilityChanged);
          this.autoHideWindowElement.IsVisibleChanged -= new EventHandler(this.OnElementParentOrVisibilityChanged);
          this.autoHideWindowElement.IsSelectedChanged -= new EventHandler(this.OnElementParentOrVisibilityChanged);
          this.autoHideWindowElement.IsSelected = false;
        }
        this.autoHideWindowElement = value;
        if (this.autoHideWindowElement == null)
          return;
        this.autoHideWindowElement.IsSelected = true;
        this.autoHideWindowElement.IsVisibleChanged += new EventHandler(this.OnElementParentOrVisibilityChanged);
        this.autoHideWindowElement.ParentChanged += new EventHandler(this.OnElementParentOrVisibilityChanged);
        this.autoHideWindowElement.IsSelectedChanged += new EventHandler(this.OnElementParentOrVisibilityChanged);
      }
    }

    public bool IsAutoHideWindowShown
    {
      get
      {
        return this.AutoHideWindow != null;
      }
    }

    public AutoHideWindowManager()
    {
      ViewManager.Instance.ActiveViewChanged += new EventHandler(this.OnActiveViewChanged);
    }

    private void OnActiveViewChanged(object sender, EventArgs e)
    {
      if (!this.IsAutoHideWindowShown)
        return;
      if (ViewManager.Instance.ActiveView != this.AutoHideWindowElement)
        this.CloseAutoHideWindow();
      else
        this.StopMouseObserver();
    }

    public void ShowAutoHideWindow(AutoHideTabItem item, View view)
    {
      if (view == this.AutoHideWindowElement)
        return;
      this.CloseAutoHideWindow();
      AutoHideChannelControl ancestor1 = Microsoft.VisualStudio.PlatformUI.ExtensionMethods.FindAncestor<AutoHideChannelControl>((Visual) item);
      AutoHideRootControl ancestor2 = Microsoft.VisualStudio.PlatformUI.ExtensionMethods.FindAncestor<AutoHideRootControl>((Visual) item);
      if (ancestor1 == null || ancestor2 == null)
        return;
      this.AutoHideWindowElement = view;
      this.AutoHideChannelControl = ancestor1;
      this.AutoHideWindow = new AutoHideWindow();
      this.AutoHideWindow.DataContext = (object) view;
      this.AutoHideWindow.DockRootElement = ancestor2.DockRoot;
      this.AutoHideChannelControl.AutoHideSlideout = (object) this.AutoHideWindow;
      LayoutSynchronizer.Update((Visual) this.AutoHideWindow);
      if (!this.AutoHideWindowElement.IsActive)
        this.StartMouseObserver(item);
      this.AutoHideWindow.RaiseEvent((RoutedEventArgs) new ViewEventArgs(AutoHideWindow.SlideoutStartedEvent, this.AutoHideWindowElement));
      this.AutoHideWindow.RaiseEvent((RoutedEventArgs) new ViewEventArgs(AutoHideWindow.SlideoutCompletedEvent, this.AutoHideWindowElement));
    }

    private void StartMouseObserver(AutoHideTabItem item)
    {
      this.MouseObserver = new AutoHideWindowManager.MouseVirtualCaptureObserver(item, this.AutoHideWindow);
      this.MouseObserver.LostVirtualMouseCapture += new EventHandler(this.OnLostVirtualMouseCapture);
    }

    private void StopMouseObserver()
    {
      if (this.MouseObserver == null)
        return;
      this.MouseObserver.Dispose();
      this.MouseObserver = (AutoHideWindowManager.MouseVirtualCaptureObserver) null;
    }

    private void OnLostVirtualMouseCapture(object sender, EventArgs args)
    {
      this.CloseAutoHideWindow();
    }

    private void OnElementParentOrVisibilityChanged(object sender, EventArgs args)
    {
      this.CloseAutoHideWindow();
    }

    public void CloseAutoHideWindow()
    {
      if (!this.IsAutoHideWindowShown)
        return;
      this.StopMouseObserver();
      this.AutoHideChannelControl.AutoHideSlideout = (object) null;
      this.AutoHideWindow.Dispose();
      this.AutoHideWindow = (AutoHideWindow) null;
      this.AutoHideWindowElement = (View) null;
      this.AutoHideChannelControl = (AutoHideChannelControl) null;
    }

    private class MouseVirtualCaptureObserver : DisposableObject
    {
      private const int VirtualCaptureTimerIntervalMilliseconds = 50;

      private AutoHideWindow Window { get; set; }

      private AutoHideTabItem TabItem { get; set; }

      private DispatcherTimer IsMouseOverTimer { get; set; }

      private DispatcherTimer MouseExitedTimer { get; set; }

      public event EventHandler LostVirtualMouseCapture;

      public MouseVirtualCaptureObserver(AutoHideTabItem tabItem, AutoHideWindow window)
      {
        this.Window = window;
        this.TabItem = tabItem;
        ApplicationActivationMonitor.Instance.Activated += new EventHandler(this.OnApplicationActivated);
        ApplicationActivationMonitor.Instance.Deactivated += new EventHandler(this.OnApplicationDeactivated);
        this.StartIsMouseOverTimer();
      }

      private void StartIsMouseOverTimer()
      {
        if (this.IsMouseOverTimer != null)
          return;
        this.IsMouseOverTimer = new DispatcherTimer(TimeSpan.FromMilliseconds(50.0), DispatcherPriority.ApplicationIdle, new EventHandler(this.OnUpdateIsMouseOver), this.Window.Dispatcher);
      }

      private void StopIsMouseOverTimer()
      {
        if (this.IsMouseOverTimer == null)
          return;
        this.IsMouseOverTimer.Stop();
        this.IsMouseOverTimer = (DispatcherTimer) null;
      }

      private void OnUpdateIsMouseOver(object sender, EventArgs args)
      {
        Point cursorPos = NativeMethods.GetCursorPos();
        Point point1 = this.Window.PointFromScreen(cursorPos);
        Point point2 = this.TabItem.PointFromScreen(cursorPos);
        bool hitWindowOrTabItem = false;
        VisualTreeHelper.HitTest((Visual) this.Window, new HitTestFilterCallback(this.ExcludeNonVisualElements), (HitTestResultCallback) (result =>
        {
          hitWindowOrTabItem = true;
          return HitTestResultBehavior.Stop;
        }), (HitTestParameters) new PointHitTestParameters(point1));
        if (!hitWindowOrTabItem)
          VisualTreeHelper.HitTest((Visual) this.TabItem, new HitTestFilterCallback(this.ExcludeNonVisualElements), (HitTestResultCallback) (result =>
          {
            hitWindowOrTabItem = true;
            return HitTestResultBehavior.Stop;
          }), (HitTestParameters) new PointHitTestParameters(point2));
        if (!hitWindowOrTabItem)
          this.StartMouseExitedTimer();
        else
          this.StopMouseExitedTimer();
      }

      private void StartMouseExitedTimer()
      {
        if (this.MouseExitedTimer != null)
          return;
        this.MouseExitedTimer = new DispatcherTimer(ViewManager.Instance.Preferences.AutoHideMouseExitGracePeriod, DispatcherPriority.Input, new EventHandler(this.OnMouseExited), this.Window.Dispatcher);
      }

      private void StopMouseExitedTimer()
      {
        if (this.MouseExitedTimer == null)
          return;
        this.MouseExitedTimer.Stop();
        this.MouseExitedTimer = (DispatcherTimer) null;
      }

      private void OnMouseExited(object sender, EventArgs args)
      {
        this.StopMouseExitedTimer();
        Microsoft.VisualStudio.PlatformUI.ExtensionMethods.RaiseEvent(this.LostVirtualMouseCapture, (object) this);
      }

      private HitTestFilterBehavior ExcludeNonVisualElements(DependencyObject potentialHitTestTarget)
      {
        Visual visual = potentialHitTestTarget as Visual;
        UIElement uiElement = potentialHitTestTarget as UIElement;
        bool flag = uiElement == null || uiElement.IsVisible;
        return visual == null || !flag ? HitTestFilterBehavior.ContinueSkipSelfAndChildren : HitTestFilterBehavior.Continue;
      }

      private void OnApplicationDeactivated(object sender, EventArgs args)
      {
        this.StopMouseExitedTimer();
        this.StopIsMouseOverTimer();
      }

      private void OnApplicationActivated(object sender, EventArgs args)
      {
        this.StartIsMouseOverTimer();
      }

      protected override void DisposeManagedResources()
      {
        this.StopMouseExitedTimer();
        this.StopIsMouseOverTimer();
        ApplicationActivationMonitor.Instance.Activated -= new EventHandler(this.OnApplicationActivated);
        ApplicationActivationMonitor.Instance.Deactivated -= new EventHandler(this.OnApplicationDeactivated);
      }
    }
  }
}
