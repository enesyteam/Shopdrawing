// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Utility.Interop.UnsafeNativeMethods
// Assembly: Microsoft.Expression.Utility, Version=5.0.30709.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: B77F0E80-A3D7-4861-BF76-6A6A586443F3
// Assembly location: C:\Users\M4600\Documents\Project\4.5\Microsoft.Expression.ProjectReferences\Microsoft.Expression.Utility.dll

using Microsoft.Expression.Utility;
using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security;
using System.Security.Permissions;
using System.Text;

namespace Microsoft.Expression.Utility.Interop
{
  [SuppressUnmanagedCodeSecurity]
  internal static class UnsafeNativeMethods
  {
    internal static readonly IntPtr HWND_NOTOPMOST = (IntPtr) -2;
    internal static readonly IntPtr HWND_TOPMOST = (IntPtr) -1;
    private static bool dwmExists = true;
    internal const uint MB_ICONERROR = 16U;
    internal const uint MB_ABORTRETRYIGNORE = 2U;
    internal const uint MB_TASKMODAL = 8192U;
    internal const int IDABORT = 3;
    internal const int IDRETRY = 4;
    internal const int IDIGNORE = 5;
    internal const int GWL_EXSTYLE = -20;
    internal const int WS_EX_TOPMOST = 8;
    internal const int SWP_NOMOVE = 2;
    internal const int SWP_NOSIZE = 1;
    internal const int SWP_NOACTIVATE = 16;
    internal const int S_OK = 0;
    internal const int STRSAFE_E_INSUFFICIENT_BUFFER = -2147024774;

    [DllImport("user32.dll", EntryPoint = "MessageBoxW", CharSet = CharSet.Unicode)]
    internal static extern int MessageBox(IntPtr hWnd, string lpText, string lpCaption, uint uType);

    [DllImport("kernel32.dll", EntryPoint = "GetShortPathNameW", CharSet = CharSet.Unicode)]
    internal static extern uint GetShortPathName(string lpszLongPath, StringBuilder lpszShortPath, uint cchBuffer);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern uint GetFullPathName(string lpFileName, uint nBufferLength, StringBuilder lpBuffer, IntPtr lpFilePart);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool SetCursorPos(int x, int y);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool GetCursorPos(ref UnsafeNativeMethods.Win32Point pt);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern ushort GetKeyState(int nVirtKey);

    [DllImport("gdi32.dll", EntryPoint = "CreateDCW", CharSet = CharSet.Unicode)]
    internal static extern IntPtr CreateDC(string strDriver, string strDevice, string strOutput, IntPtr pData);

    [DllImport("gdi32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool DeleteDC(IntPtr hdc);

    [DllImport("gdi32.dll")]
    internal static extern int GetPixel(IntPtr hdc, int x, int y);

    [DllImport("mscoree.dll", CharSet = CharSet.Unicode)]
    internal static extern int GetFileVersion(string fileName, StringBuilder fileVersionString, int bufferSize, out int bufferLength);

    [DllImport("dwmapi.dll", EntryPoint = "DwmIsCompositionEnabled", PreserveSig = false)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool InternalDwmIsCompositionEnabled();

    internal static bool DwmIsCompositionEnabled()
    {
      try
      {
        return UnsafeNativeMethods.dwmExists && UnsafeNativeMethods.InternalDwmIsCompositionEnabled();
      }
      catch (DllNotFoundException ex)
      {
        return UnsafeNativeMethods.dwmExists = false;
      }
    }

    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    internal static UnsafeNativeMethods.Win32Point GetCursorPosition()
    {
      UnsafeNativeMethods.Win32Point pt = new UnsafeNativeMethods.Win32Point();
      UnsafeNativeMethods.GetCursorPos(ref pt);
      return pt;
    }

    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    internal static extern int GetCurrentThreadId();

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool EnumThreadWindows(int dwThreadId, UnsafeNativeMethods.EnumWindowsProc lpfn, IntPtr lParam);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool IsWindowVisible(IntPtr hWnd);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool IsWindowEnabled(IntPtr hWnd);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool IsWindow(IntPtr hWnd);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool EnableWindow(IntPtr hWnd, [MarshalAs(UnmanagedType.Bool)] bool enable);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    internal static extern int SetWindowLong(IntPtr handle, int index, int dwNewLong);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    internal static extern int GetWindowLong(IntPtr handle, int index);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool SetWindowPos(IntPtr handle, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint flags);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    internal static extern IntPtr GetCapture();

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool ReleaseCapture();

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool DestroyIcon(IntPtr hIcon);

    [DllImport("user32.dll")]
    internal static extern uint GetWindowThreadProcessId(IntPtr hwnd, out uint lpdwProcessId);

    internal struct Win32Point
    {
      public int x;
      public int y;

      public Win32Point(int x, int y)
      {
        this.x = x;
        this.y = y;
      }
    }

    internal static class Shell32
    {
      [DllImport("shell32.dll")]
      public static extern int SHILCreateFromPath([MarshalAs(UnmanagedType.LPWStr)] string pszPath, out IntPtr ppIdl, ref uint rgflnOut);

      [DllImport("shell32.dll")]
      public static extern int SHCreateShellItem(IntPtr pidlParent, IntPtr psfParent, IntPtr pidl, out IShellItem ppsi);

      [DllImport("shell32.dll")]
      public static extern int SHGetDesktopFolder(out IShellFolder ppshf);

      [DllImport("shell32.dll")]
      public static extern int SHParseDisplayName([MarshalAs(UnmanagedType.LPWStr)] string name, IBindCtx bindCtx, out IntPtr pidl, uint sfgaoIn, out uint psfgaoOut);
    }

    internal static class Ole32
    {
      [DllImport("ole32.dll")]
      public static extern int CreateBindCtx(uint reserved, out IBindCtx ppbc);

      public static class BindContextStringKeys
      {
        public static string STR_PARSE_PREFER_FOLDER_BROWSING
        {
          get
          {
            return "Parse Prefer Folder Browsing";
          }
        }
      }
    }

    internal delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
  }
}
