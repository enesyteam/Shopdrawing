// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.Controls.HwndWrapper
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.VisualStudio.PlatformUI;
using System;

namespace Microsoft.VisualStudio.PlatformUI.Shell.Controls
{
  public abstract class HwndWrapper : DisposableObject
  {
    private IntPtr handle;
    private ushort wndClassAtom;
    private Delegate wndProc;

    [CLSCompliant(false)]
    protected ushort WindowClassAtom
    {
      get
      {
        if ((int) this.wndClassAtom == 0)
          this.wndClassAtom = this.CreateWindowClassCore();
        return this.wndClassAtom;
      }
    }

    public IntPtr Handle
    {
      get
      {
        if (this.handle == IntPtr.Zero)
          this.handle = this.CreateWindowCore();
        return this.handle;
      }
    }

    [CLSCompliant(false)]
    protected virtual ushort CreateWindowClassCore()
    {
        return this.RegisterClass(Guid.NewGuid().ToString());
    }

    protected virtual void DestroyWindowClassCore()
    {
      if ((int) this.wndClassAtom == 0)
        return;
      NativeMethods.UnregisterClass(new IntPtr((int) this.wndClassAtom), NativeMethods.GetModuleHandle((string) null));
      this.wndClassAtom = (ushort) 0;
    }

    [CLSCompliant(false)]
    protected ushort RegisterClass(string className)
    {
        WNDCLASS wNDCLAss = new WNDCLASS()
        {
            cbClsExtra = 0,
            cbWndExtra = 0,
            hbrBackground = IntPtr.Zero,
            hCursor = IntPtr.Zero,
            hIcon = IntPtr.Zero
        };
        HwndWrapper hwndWrapper = this;
        NativeMethods.WndProc wndProc = new NativeMethods.WndProc(hwndWrapper.WndProc);
        Delegate @delegate = wndProc;
        this.wndProc = wndProc;
        wNDCLAss.lpfnWndProc = @delegate;
        wNDCLAss.lpszClassName = className;
        wNDCLAss.lpszMenuName = null;
        wNDCLAss.style = 0;
        return NativeMethods.RegisterClass(ref wNDCLAss);
    }

    protected abstract IntPtr CreateWindowCore();

    protected virtual void DestroyWindowCore()
    {
      if (!(this.handle != IntPtr.Zero))
        return;
      NativeMethods.DestroyWindow(this.handle);
      this.handle = IntPtr.Zero;
    }

    protected virtual IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam)
    {
      return NativeMethods.DefWindowProc(hwnd, msg, wParam, lParam);
    }

    protected override void DisposeNativeResources()
    {
      this.DestroyWindowCore();
      this.DestroyWindowClassCore();
    }
  }
}
