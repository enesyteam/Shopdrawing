using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Threading;
using System.Windows.Interop;

namespace Microsoft.Expression.Framework.UserInterface
{
    public class SplashScreen : IDisposable
    {
        private static string windowClassName;

        private static bool isClassRegistered;

        private IntPtr windowHandle = IntPtr.Zero;

        private IntPtr bitmapHandle = IntPtr.Zero;

        private int bitmapWidth;

        private int bitmapHeight;

        private UnsafeNativeMethods.WndProc splashWindowProcedureCallback;

        private bool isClosed;

        static SplashScreen()
        {
            SplashScreen.windowClassName = "ExpressionSplashScreenWindowClass";
        }

        [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
        public SplashScreen(Module module, int resourceId)
        {
            IntPtr hINSTANCE = Marshal.GetHINSTANCE(module);
            this.bitmapHandle = UnsafeNativeMethods.LoadBitmap(hINSTANCE, new IntPtr(resourceId));
            this.splashWindowProcedureCallback = new UnsafeNativeMethods.WndProc(this.SplashWindowProcedure);
        }

        public void Close()
        {
            if (this.isClosed)
            {
                return;
            }
            this.isClosed = true;
            UnsafeNativeMethods.PostMessage(new HandleRef(this, this.windowHandle), 16, IntPtr.Zero, IntPtr.Zero);
            if (this.bitmapHandle != IntPtr.Zero)
            {
                UnsafeNativeMethods.DeleteObject(this.bitmapHandle);
                this.bitmapHandle = IntPtr.Zero;
            }
        }

        public void Close(IntPtr handle)
        {
            if (this.windowHandle != IntPtr.Zero)
            {
                UnsafeNativeMethods.SetForegroundWindow(handle);
            }
            this.Close();
        }

        private void CreateWindow()
        {
            if (!SplashScreen.isClassRegistered)
            {
                this.RegisterClass();
            }
            if (SplashScreen.isClassRegistered)
            {
                this.ReallyCreateWindow();
                if (this.windowHandle != IntPtr.Zero)
                {
                    UnsafeNativeMethods.ShowWindow(this.windowHandle, 5);
                }
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            this.Close(IntPtr.Zero);
            GC.SuppressFinalize(this);
        }

        private void GetBitmapDimensions()
        {
            int num = Marshal.SizeOf(typeof(UnsafeNativeMethods.BITMAP));
            IntPtr intPtr = Marshal.AllocCoTaskMem(num);
            UnsafeNativeMethods.GetObject(this.bitmapHandle, num, intPtr);
            UnsafeNativeMethods.BITMAP structure = (UnsafeNativeMethods.BITMAP)Marshal.PtrToStructure(intPtr, typeof(UnsafeNativeMethods.BITMAP));
            this.bitmapWidth = structure.bmWidth;
            this.bitmapHeight = structure.bmHeight;
            Marshal.FreeCoTaskMem(intPtr);
        }

        private void OnPaint(IntPtr hdc)
        {
            if (this.bitmapHandle != IntPtr.Zero)
            {
                IntPtr intPtr = UnsafeNativeMethods.CreateCompatibleDC(hdc);
                IntPtr intPtr1 = UnsafeNativeMethods.SelectObject(intPtr, this.bitmapHandle);
                UnsafeNativeMethods.BitBlt(hdc, 0, 0, this.bitmapWidth, this.bitmapHeight, intPtr, 0, 0, 13369376);
                UnsafeNativeMethods.SelectObject(intPtr, intPtr1);
                UnsafeNativeMethods.DeleteDC(intPtr);
            }
        }

        private void ReallyCreateWindow()
        {
            int num = this.bitmapWidth;
            int num1 = this.bitmapHeight;
            int systemMetrics = UnsafeNativeMethods.GetSystemMetrics(0);
            int systemMetrics1 = UnsafeNativeMethods.GetSystemMetrics(1);
            uint num2 = 2147483648; //-2147483648
            uint num3 = 392;
            IntPtr moduleHandle = UnsafeNativeMethods.GetModuleHandle(null);
            IntPtr desktopWindow = UnsafeNativeMethods.GetDesktopWindow();
            this.windowHandle = UnsafeNativeMethods.CreateWindowEx(num3, SplashScreen.windowClassName, "", num2, (systemMetrics - num) / 2, (systemMetrics1 - num1) / 2, num, num1, desktopWindow, IntPtr.Zero, moduleHandle, IntPtr.Zero);
        }

        private void RegisterClass()
        {
            IntPtr moduleHandle = UnsafeNativeMethods.GetModuleHandle(null);
            UnsafeNativeMethods.WNDCLASSEX wNDCLASSEX = new UnsafeNativeMethods.WNDCLASSEX()
            {
                cbSize = (uint)Marshal.SizeOf(typeof(UnsafeNativeMethods.WNDCLASSEX)),
                cbClsExtra = 0,
                cbWndExtra = 0,
                hbrBackground = IntPtr.Zero,
                hCursor = IntPtr.Zero,
                hIcon = IntPtr.Zero,
                hIconSm = IntPtr.Zero,
                hInstance = moduleHandle,
                lpfnWndProc = this.splashWindowProcedureCallback,
                lpszClassName = SplashScreen.windowClassName,
                lpszMenuName = null,
                style = 0
            };
            if (UnsafeNativeMethods.RegisterClassExW(ref wNDCLASSEX) != 0)
            {
                SplashScreen.isClassRegistered = true;
            }
        }

        public void Show()
        {
            if (this.windowHandle != IntPtr.Zero)
            {
                return;
            }
            Thread thread = new Thread(new ThreadStart(this.ThreadMethod))
            {
                IsBackground = true
            };
            thread.Start();
        }

        [SuppressUnmanagedCodeSecurity]
        private IntPtr SplashWindowProcedure(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            if (msg != 15)
            {
                return UnsafeNativeMethods.DefWindowProc(hWnd, msg, wParam, lParam);
            }
            UnsafeNativeMethods.PAINTSTRUCT pAINTSTRUCT = new UnsafeNativeMethods.PAINTSTRUCT();
            this.OnPaint(UnsafeNativeMethods.BeginPaint(hWnd, out pAINTSTRUCT));
            UnsafeNativeMethods.EndPaint(hWnd, ref pAINTSTRUCT);
            return IntPtr.Zero;
        }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        private void ThreadMethod()
        {
            if (this.bitmapHandle == IntPtr.Zero)
            {
                return;
            }
            this.GetBitmapDimensions();
            this.CreateWindow();
            MSG mSG = new MSG();
            while (UnsafeNativeMethods.GetMessage(ref mSG, this.windowHandle, 0, 0) > 0)
            {
                UnsafeNativeMethods.TranslateMessage(ref mSG);
                UnsafeNativeMethods.DispatchMessage(ref mSG);
            }
            this.windowHandle = IntPtr.Zero;
            GC.KeepAlive(this);
        }
    }
}