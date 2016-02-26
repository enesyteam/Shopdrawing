// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.UserInterface.WindowHelper
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace Microsoft.Expression.Framework.UserInterface
{
  public static class WindowHelper
  {
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    public static void UpdateWindowPlacement(Window window)
    {
      IntPtr handle = new WindowInteropHelper(window).Handle;
      UnsafeNativeMethods.WINDOWPLACEMENT lpwndpl = new UnsafeNativeMethods.WINDOWPLACEMENT();
      lpwndpl.Length = Marshal.SizeOf(typeof (UnsafeNativeMethods.WINDOWPLACEMENT));
      if (!UnsafeNativeMethods.GetWindowPlacement(handle, out lpwndpl))
        return;
      UnsafeNativeMethods.SetWindowPlacement(handle, ref lpwndpl);
    }

    internal static Point TransformToDevice(Window window, Point point)
    {
      return WindowHelper.TransformToDevice(new WindowInteropHelper(window).Handle, point);
    }

    private static Point TransformToDevice(IntPtr hwnd, Point point)
    {
      double num1 = 96.0;
      double num2 = 96.0;
      IntPtr dc = UnsafeNativeMethods.GetDC(hwnd);
      if (dc != IntPtr.Zero)
      {
        num1 = (double) UnsafeNativeMethods.GetDeviceCaps(dc, 88);
        num2 = (double) UnsafeNativeMethods.GetDeviceCaps(dc, 90);
        UnsafeNativeMethods.ReleaseDC(hwnd, dc);
      }
      Matrix identity = Matrix.Identity;
      identity.Scale(num1 / 96.0, num2 / 96.0);
      return identity.Transform(point);
    }

    internal static Point TransformFromDevice(Window window, Point point)
    {
      return WindowHelper.TransformFromDevice(new WindowInteropHelper(window).Handle, point);
    }

    private static Point TransformFromDevice(IntPtr hwnd, Point point)
    {
      double num1 = 96.0;
      double num2 = 96.0;
      IntPtr dc = UnsafeNativeMethods.GetDC(hwnd);
      if (dc != IntPtr.Zero)
      {
        num1 = (double) UnsafeNativeMethods.GetDeviceCaps(dc, 88);
        num2 = (double) UnsafeNativeMethods.GetDeviceCaps(dc, 90);
        UnsafeNativeMethods.ReleaseDC(hwnd, dc);
      }
      Matrix identity = Matrix.Identity;
      identity.Scale(96.0 / num1, 96.0 / num2);
      return identity.Transform(point);
    }
  }
}
