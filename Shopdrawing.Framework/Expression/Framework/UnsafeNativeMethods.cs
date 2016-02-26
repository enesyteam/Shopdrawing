// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.UnsafeNativeMethods
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security;
using System.Security.Permissions;
using System.Text;

namespace Microsoft.Expression.Framework
{
  [SuppressUnmanagedCodeSecurity]
  internal static class UnsafeNativeMethods
  {
    private static bool dwmExists = true;
    internal const uint MB_ICONASTERISK = 64U;
    internal const uint MB_SETFOREGROUND = 65536U;

    [DllImport("kernel32.dll", EntryPoint = "GetShortPathNameW", CharSet = CharSet.Unicode)]
    public static extern uint GetShortPathName(string lpszLongPath, StringBuilder lpszShortPath, uint cchBuffer);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern uint GetFullPathName(string lpFileName, uint nBufferLength, StringBuilder lpBuffer, IntPtr lpFilePart);

    [DllImport("user32.dll", EntryPoint = "MessageBoxW", CharSet = CharSet.Unicode)]
    internal static extern int MessageBox(IntPtr hWnd, string lpText, string lpCaption, uint uType);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    internal static extern IntPtr LoadIcon(IntPtr hInstance, UIntPtr lpIconName);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool GetCursorPos(ref UnsafeNativeMethods.Win32Point pt);

    [DllImport("gdi32.dll", EntryPoint = "CreateDCW", CharSet = CharSet.Unicode)]
    internal static extern IntPtr CreateDC(string strDriver, string strDevice, string strOutput, IntPtr pData);

    [DllImport("gdi32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool DeleteDC(IntPtr hdc);

    [DllImport("gdi32.dll")]
    internal static extern int GetPixel(IntPtr hdc, int x, int y);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool SetCursorPos(int x, int y);

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
  }
}
