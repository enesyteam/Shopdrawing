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
        internal static Point TransformFromDevice(Window window, Point point)
        {
            return WindowHelper.TransformFromDevice((new WindowInteropHelper(window)).Handle, point);
        }

        private static Point TransformFromDevice(IntPtr hwnd, Point point)
        {
            double deviceCaps = 96;
            double num = 96;
            IntPtr dC = UnsafeNativeMethods.GetDC(hwnd);
            if (dC != IntPtr.Zero)
            {
                deviceCaps = (double)UnsafeNativeMethods.GetDeviceCaps(dC, 88);
                num = (double)UnsafeNativeMethods.GetDeviceCaps(dC, 90);
                UnsafeNativeMethods.ReleaseDC(hwnd, dC);
            }
            Matrix identity = Matrix.Identity;
            identity.Scale(96 / deviceCaps, 96 / num);
            return identity.Transform(point);
        }

        internal static Point TransformToDevice(Window window, Point point)
        {
            return WindowHelper.TransformToDevice((new WindowInteropHelper(window)).Handle, point);
        }

        private static Point TransformToDevice(IntPtr hwnd, Point point)
        {
            double deviceCaps = 96;
            double num = 96;
            IntPtr dC = UnsafeNativeMethods.GetDC(hwnd);
            if (dC != IntPtr.Zero)
            {
                deviceCaps = (double)UnsafeNativeMethods.GetDeviceCaps(dC, 88);
                num = (double)UnsafeNativeMethods.GetDeviceCaps(dC, 90);
                UnsafeNativeMethods.ReleaseDC(hwnd, dC);
            }
            Matrix identity = Matrix.Identity;
            identity.Scale(deviceCaps / 96, num / 96);
            return identity.Transform(point);
        }

        [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
        public static void UpdateWindowPlacement(Window window)
        {
            IntPtr handle = (new WindowInteropHelper(window)).Handle;
            UnsafeNativeMethods.WINDOWPLACEMENT wINDOWPLACEMENT = new UnsafeNativeMethods.WINDOWPLACEMENT()
            {
                Length = Marshal.SizeOf(typeof(UnsafeNativeMethods.WINDOWPLACEMENT))
            };
            if (UnsafeNativeMethods.GetWindowPlacement(handle, out wINDOWPLACEMENT))
            {
                UnsafeNativeMethods.SetWindowPlacement(handle, ref wINDOWPLACEMENT);
            }
        }
    }
}