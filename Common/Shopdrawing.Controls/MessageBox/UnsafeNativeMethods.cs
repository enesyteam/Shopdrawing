using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows.Interop;

namespace Microsoft.Expression.Framework.UserInterface
{
    [SuppressUnmanagedCodeSecurity]
    internal sealed class UnsafeNativeMethods
    {
        internal const int GWL_STYLE = -16;

        internal const int GWL_EXSTYLE = -20;

        internal const uint WS_POPUP = 2147483648;

        internal const uint WS_MINIMIZEBOX = 131072;

        internal const uint WS_MAXIMIZEBOX = 65536;

        internal const uint WS_EX_WINDOWEDGE = 256;

        internal const uint WS_EX_TOOLWINDOW = 128;

        internal const uint WS_EX_TOPMOST = 8;

        internal const uint WS_EX_TRANSPARENT = 32;

        internal const uint SRCCOPY = 13369376;

        internal const int SW_HIDE = 0;

        internal const int SW_SHOWNORMAL = 1;

        internal const int SW_SHOWMINIMIZED = 2;

        internal const int SW_SHOWMAXIMIZED = 3;

        internal const int SW_SHOWNOACTIVE = 4;

        internal const int SW_SHOW = 5;

        internal const int SM_CXSCREEN = 0;

        internal const int SM_CYSCREEN = 1;

        internal const int WM_CLOSE = 16;

        internal const uint MB_OK = 0;

        internal const uint MB_OKCANCEL = 1;

        internal const uint MB_YESNOCANCEL = 3;

        internal const uint MB_YESNO = 4;

        internal const uint MB_ICONHAND = 16;

        internal const uint MB_ICONQUESTION = 32;

        internal const uint MB_ICONEXCLAMATION = 48;

        internal const uint MB_ICONASTERISK = 64;

        internal const uint MB_ICONWARNING = 48;

        internal const uint MB_ICONERROR = 16;

        internal const uint MB_ICONINFORMATION = 64;

        internal const uint MB_SETFOREGROUND = 65536;

        public const int SPI_GETWORKAREA = 48;

        public const int LOGPIXELSX = 88;

        public const int LOGPIXELSY = 90;

        public UnsafeNativeMethods()
        {
        }

        [DllImport("user32.dll", CharSet = CharSet.None, ExactSpelling = false)]
        internal static extern IntPtr BeginPaint(IntPtr hwnd, out UnsafeNativeMethods.PAINTSTRUCT lpPaint);

        [DllImport("gdi32.dll", CharSet = CharSet.None, ExactSpelling = false)]
        internal static extern bool BitBlt(IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, uint dwRop);

        [DllImport("gdi32.dll", CharSet = CharSet.None, ExactSpelling = false)]
        internal static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "CreateWindowExW", ExactSpelling = false)]
        internal static extern IntPtr CreateWindowEx(uint dwExStyle, string lpClassName, string lpWindowName, uint dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);

        [DllImport("user32.dll", CharSet = CharSet.None, ExactSpelling = false)]
        internal static extern IntPtr DefWindowProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("gdi32.dll", CharSet = CharSet.None, ExactSpelling = false)]
        internal static extern bool DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll", CharSet = CharSet.None, ExactSpelling = false)]
        internal static extern bool DeleteObject(IntPtr hObject);

        [DllImport("user32.dll", CharSet = CharSet.None, ExactSpelling = false)]
        internal static extern IntPtr DispatchMessage([In] ref MSG lpmsg);

        [DllImport("user32.dll", CharSet = CharSet.None, ExactSpelling = false)]
        internal static extern bool EndPaint(IntPtr hWnd, ref UnsafeNativeMethods.PAINTSTRUCT lpPaint);

        [DllImport("user32.dll", CharSet = CharSet.None, ExactSpelling = false)]
        internal static extern IntPtr GetDC(HandleRef hWnd);

        [DllImport("user32.dll", CharSet = CharSet.None, ExactSpelling = false)]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.None, ExactSpelling = false)]
        internal static extern IntPtr GetDesktopWindow();

        [DllImport("gdi32.dll", CharSet = CharSet.None, ExactSpelling = false)]
        public static extern int GetDeviceCaps(IntPtr hDC, int index);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "GetMessageW", ExactSpelling = true)]
        internal static extern int GetMessage([In][Out] ref MSG msg, IntPtr hWnd, int uMsgFilterMin, int uMsgFilterMax);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, EntryPoint = "GetModuleHandleW", ExactSpelling = false)]
        internal static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("gdi32.dll", CharSet = CharSet.None, ExactSpelling = false)]
        internal static extern int GetObject(IntPtr hgdiobj, int cbBuffer, IntPtr lpvObject);

        [DllImport("user32.dll", CharSet = CharSet.None, ExactSpelling = false)]
        internal static extern int GetSystemMetrics(int nIndex);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = false)]
        internal static extern int GetWindowLong(IntPtr handle, int index);

        [DllImport("user32.dll", CharSet = CharSet.None, ExactSpelling = false)]
        public static extern bool GetWindowPlacement(IntPtr hWnd, out UnsafeNativeMethods.WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "LoadBitmapW", ExactSpelling = false)]
        internal static extern IntPtr LoadBitmap(IntPtr hInstance, IntPtr lpBitmapName);

        [DllImport("user32.dll", CharSet = CharSet.None, ExactSpelling = false)]
        internal static extern bool PostMessage(HandleRef hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.None, ExactSpelling = false)]
        internal static extern short RegisterClassExW([In] ref UnsafeNativeMethods.WNDCLASSEX lpwcx);

        [DllImport("user32.dll", CharSet = CharSet.None, ExactSpelling = false)]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll", CharSet = CharSet.None, ExactSpelling = false)]
        internal static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("user32.dll", CharSet = CharSet.None, ExactSpelling = false)]
        internal static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = false)]
        internal static extern int SetWindowLong(IntPtr handle, int index, int dwNewLong);

        [DllImport("user32.dll", CharSet = CharSet.None, ExactSpelling = false)]
        public static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref UnsafeNativeMethods.WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll", CharSet = CharSet.None, ExactSpelling = false)]
        internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", CharSet = CharSet.None, ExactSpelling = false)]
        public static extern bool SystemParametersInfo(int nAction, int nParam, ref UnsafeNativeMethods.RECT rc, int nUpdate);

        [DllImport("user32.dll", CharSet = CharSet.None, ExactSpelling = false)]
        internal static extern bool TranslateMessage([In] ref MSG lpMsg);

        internal struct BITMAP
        {
            public int bmType;

            public int bmWidth;

            public int bmHeight;

            public int bmWidthBytes;

            public ushort bmPlanes;

            public ushort bmBitsPixel;

            public IntPtr bmBits;
        }

        internal struct PAINTSTRUCT
        {
            public IntPtr hdc;

            public bool fErase;

            public UnsafeNativeMethods.RECT rcPaint;

            public bool fRestore;

            public bool fIncUpdate;

            public byte[] rgbReserved;
        }

        public struct POINT
        {
            public int X;

            public int Y;
        }

        internal struct RECT
        {
            public int Left;

            public int Top;

            public int Right;

            public int Bottom;
        }

        public struct WINDOWPLACEMENT
        {
            public int Length;

            public int Flags;

            public int ShowCmd;

            public UnsafeNativeMethods.POINT MinPosition;

            public UnsafeNativeMethods.POINT MaxPosition;

            public UnsafeNativeMethods.RECT NormalPosition;
        }

        internal struct WNDCLASSEX
        {
            public uint cbSize;

            public uint style;

            public UnsafeNativeMethods.WndProc lpfnWndProc;

            public int cbClsExtra;

            public int cbWndExtra;

            public IntPtr hInstance;

            public IntPtr hIcon;

            public IntPtr hCursor;

            public IntPtr hbrBackground;

            public string lpszMenuName;

            public string lpszClassName;

            public IntPtr hIconSm;
        }

        internal delegate IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
    }
}