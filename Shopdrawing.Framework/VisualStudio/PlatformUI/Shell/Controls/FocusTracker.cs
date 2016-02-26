// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.Controls.FocusTracker
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.PlatformUI.Shell.Controls
{
  public sealed class FocusTracker : DisposableObject
  {
    [ThreadStatic]
    private static FocusTracker _instance;
    private FocusTracker.WindowsHookHandle _windowsHookHandle;
    private Microsoft.VisualStudio.PlatformUI.NativeMethods.WindowsHookProc _cbtHookProc;

    public static FocusTracker Instance
    {
      get
      {
        return FocusTracker._instance ?? (FocusTracker._instance = new FocusTracker());
      }
    }

    private event EventHandler<TrackFocusEventArgs> _trackFocusDelegate;

    public event EventHandler<TrackFocusEventArgs> TrackFocus
    {
      [MethodImpl(MethodImplOptions.Synchronized)] add
      {
        this._trackFocusDelegate += value;
        if (this._windowsHookHandle != null && !this._windowsHookHandle.IsInvalid)
          return;
        this._windowsHookHandle = new FocusTracker.WindowsHookHandle(Microsoft.VisualStudio.PlatformUI.NativeMethods.WindowsHookType.WH_CBT, this._cbtHookProc, IntPtr.Zero, Microsoft.VisualStudio.PlatformUI.NativeMethods.GetCurrentThreadId());
      }
      [MethodImpl(MethodImplOptions.Synchronized)] remove
      {
        this._trackFocusDelegate -= value;
        if (this._trackFocusDelegate != null || this._windowsHookHandle == null)
          return;
        this._windowsHookHandle.Close();
        this._windowsHookHandle = (FocusTracker.WindowsHookHandle) null;
      }
    }

    private FocusTracker()
    {
      this._cbtHookProc = new Microsoft.VisualStudio.PlatformUI.NativeMethods.WindowsHookProc(this.CbtWindowsHookProc);
    }

    private IntPtr CbtWindowsHookProc(Microsoft.VisualStudio.PlatformUI.NativeMethods.CbtHookAction code, IntPtr wParam, IntPtr lParam)
    {
      if (code == Microsoft.VisualStudio.PlatformUI.NativeMethods.CbtHookAction.HCBT_SETFOCUS)
        this.SendTrackFocusEvent(new TrackFocusEventArgs(wParam, lParam));
      return Microsoft.VisualStudio.PlatformUI.NativeMethods.CallNextHookEx(IntPtr.Zero, code, wParam, lParam);
    }

    private void SendTrackFocusEvent(TrackFocusEventArgs e)
    {
      EventHandler<TrackFocusEventArgs> eventHandler = this._trackFocusDelegate;
      if (eventHandler == null)
        return;
      eventHandler((object) this, e);
    }

    protected override void DisposeNativeResources()
    {
      if (this._windowsHookHandle == null)
        return;
      this._windowsHookHandle.Close();
      this._windowsHookHandle = (FocusTracker.WindowsHookHandle) null;
    }

    private sealed class WindowsHookHandle : CriticalHandle
    {
      public override bool IsInvalid
      {
        get
        {
          return this.handle == IntPtr.Zero;
        }
      }

      public WindowsHookHandle(Microsoft.VisualStudio.PlatformUI.NativeMethods.WindowsHookType hookType, Microsoft.VisualStudio.PlatformUI.NativeMethods.WindowsHookProc hookProc, IntPtr module, uint threadId)
        : base(IntPtr.Zero)
      {
        this.SetHandle(Microsoft.VisualStudio.PlatformUI.NativeMethods.SetWindowsHookEx(hookType, hookProc, module, threadId));
      }

      protected override bool ReleaseHandle()
      {
        return Microsoft.VisualStudio.PlatformUI.NativeMethods.UnhookWindowsHookEx(this.handle);
      }
    }
  }
}
