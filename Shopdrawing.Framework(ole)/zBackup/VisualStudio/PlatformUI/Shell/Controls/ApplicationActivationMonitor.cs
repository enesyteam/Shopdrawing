// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.Controls.ApplicationActivationMonitor
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Diagnostics;
using System.Windows.Interop;

namespace Microsoft.VisualStudio.PlatformUI.Shell.Controls
{
  internal sealed class ApplicationActivationMonitor
  {
    private HwndSource hwndSource;
    private bool isActive;
    private static ApplicationActivationMonitor instance;

    public static ApplicationActivationMonitor Instance
    {
      get
      {
        return ApplicationActivationMonitor.instance ?? (ApplicationActivationMonitor.instance = new ApplicationActivationMonitor());
      }
    }

    internal HwndSource HwndSource
    {
      get
      {
        return this.hwndSource;
      }
      set
      {
        if (this.hwndSource == value)
          return;
        if (this.hwndSource != null)
          this.hwndSource.RemoveHook(new HwndSourceHook(this.WndProcHook));
        this.hwndSource = value;
        if (this.hwndSource == null)
          return;
        this.hwndSource.AddHook(new HwndSourceHook(this.WndProcHook));
        this.IsActive = ApplicationActivationMonitor.IsApplicationActive();
      }
    }

    public bool IsActive
    {
      get
      {
        if (this.HwndSource == null)
          return ApplicationActivationMonitor.IsApplicationActive();
        return this.isActive;
      }
      private set
      {
        if (this.isActive == value)
          return;
        this.isActive = value;
        if (this.isActive)
          this.OnActivated();
        else
          this.OnDeactivated();
      }
    }

    public event EventHandler Activated;

    public event EventHandler Deactivated;

    private static bool IsApplicationActive()
    {
      IntPtr foregroundWindow = NativeMethods.GetForegroundWindow();
      if (foregroundWindow == IntPtr.Zero)
        return false;
      uint processId;
      int num = (int) NativeMethods.GetWindowThreadProcessId(foregroundWindow, out processId);
      uint currentProcessId = NativeMethods.GetCurrentProcessId();
      return (int) processId == (int) currentProcessId;
    }

    [DebuggerStepThrough]
    private IntPtr WndProcHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
      if (msg == 28)
        this.IsActive = wParam != IntPtr.Zero;
      return IntPtr.Zero;
    }

    private void OnActivated()
    {
        Microsoft.VisualStudio.PlatformUI.ExtensionMethods.RaiseEvent(this.Activated, (object)this);
    }

    private void OnDeactivated()
    {
        Microsoft.VisualStudio.PlatformUI.ExtensionMethods.RaiseEvent(this.Deactivated, (object)this);
    }
  }
}
