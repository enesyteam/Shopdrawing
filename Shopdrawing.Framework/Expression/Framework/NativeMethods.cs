// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.NativeMethods
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.Expression.Framework
{
  internal static class NativeMethods
  {
    internal static IntPtr NO_PARENT = IntPtr.Zero;
    internal const uint INVALID_FILE_ATTRIBUTES = 4294967295U;
    internal const uint FILE_ATTRIBUTE_DIRECTORY = 16U;
    internal const uint FILE_ATTRIBUTE_VIRTUAL = 65536U;
    public const int WM_CLOSE = 16;
    public const int WM_QUERYENDSESSION = 17;
    public const int SMTO_ABORTIFHUNG = 2;
    internal const int SM_CMONITORS = 80;
    internal const int LOGPIXELSX = 88;
    internal const int LOGPIXELSY = 90;

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern int GetDoubleClickTime();

    [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
    public static extern int StrCmpLogicalW(string x, string y);

    [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SHAddToRecentDocs(NativeMethods.SHARD nFlags, string path);

    internal static bool ShellAddPathToRecentDocuments(string path)
    {
      return NativeMethods.SHAddToRecentDocs(NativeMethods.SHARD.SHARD_PATHW, path);
    }

    [SuppressUnmanagedCodeSecurity]
    [DllImport("kernel32.dll", EntryPoint = "GetFileAttributes", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern uint GetFileAttributesInternal(string lpFileName);

    internal static bool GetFileAttributes(string lplFileName, out FileAttributes attributes)
    {
      attributes = FileAttributes.Temporary;
      uint attributesInternal = NativeMethods.GetFileAttributesInternal(lplFileName);
      if ((int) attributesInternal == -1)
        return false;
      uint num = attributesInternal & 4294901759U;
      attributes = (FileAttributes) num;
      return true;
    }

    internal static bool PathExists(string path)
    {
      return (int) NativeMethods.GetFileAttributesInternal(path) != -1;
    }

    internal static bool FileExists(string path)
    {
      uint attributesInternal = NativeMethods.GetFileAttributesInternal(path);
      if ((int) attributesInternal == -1)
        return false;
      return ((int) attributesInternal & 16) != 16;
    }

    internal static bool DirectoryExists(string path)
    {
      uint attributesInternal = NativeMethods.GetFileAttributesInternal(path);
      if ((int) attributesInternal == -1)
        return false;
      return ((int) attributesInternal & 16) == 16;
    }

    [SuppressUnmanagedCodeSecurity]
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool SetFileAttributes(string lpFileName, [MarshalAs(UnmanagedType.U4)] FileAttributes dwFileAttributes);

    [DllImport("user32.Dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool EnumWindows(EnumWindowsCB callback, IntPtr lParam);

    [DllImport("user32")]
    public static extern int GetWindowThreadProcessId(HandleRef hwnd, out int lpdwProcessId);

    [DllImport("user32")]
    public static extern IntPtr SendMessageTimeout(HandleRef hWnd, int msg, IntPtr wParam, IntPtr lParam, int flags, int timeout, out IntPtr pdwResult);

    [DllImport("user32")]
    public static extern IntPtr PostMessage(HandleRef hWnd, int msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32")]
    public static extern int IsWindow(HandleRef hWnd);

    [DllImport("user32.dll")]
    internal static extern int GetSystemMetrics(int smIndex);

    [DllImport("user32.dll")]
    internal static extern IntPtr GetDC(IntPtr hWnd);

    [DllImport("user32.dll")]
    internal static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

    [DllImport("gdi32.dll")]
    internal static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool GlobalMemoryStatusEx([In, Out] NativeMethods.MEMORYSTATUSEX lpBuffer);

    [Flags]
    internal enum FOS : uint
    {
      FOS_OVERWRITEPROMPT = 2U,
      FOS_STRICTFILETYPES = 4U,
      FOS_NOCHANGEDIR = 8U,
      FOS_PICKFOLDERS = 32U,
      FOS_FORCEFILESYSTEM = 64U,
      FOS_ALLNONSTORAGEITEMS = 128U,
      FOS_NOVALIDATE = 256U,
      FOS_ALLOWMULTISELECT = 512U,
      FOS_PATHMUSTEXIST = 2048U,
      FOS_FILEMUSTEXIST = 4096U,
      FOS_CREATEPROMPT = 8192U,
      FOS_SHAREAWARE = 16384U,
      FOS_NOREADONLYRETURN = 32768U,
      FOS_NOTESTFILECREATE = 65536U,
      FOS_HIDEMRUPLACES = 131072U,
      FOS_HIDEPINNEDPLACES = 262144U,
      FOS_NODEREFERENCELINKS = 1048576U,
      FOS_DONTADDTORECENT = 33554432U,
      FOS_FORCESHOWHIDDEN = 268435456U,
      FOS_DEFAULTNOMINIMODE = 536870912U,
    }

    [Flags]
    internal enum SHCONTF : uint
    {
      SHCONTF_CHECKING_FOR_CHILDREN = 16U,
      SHCONTF_FOLDERS = 32U,
      SHCONTF_NONFOLDERS = 64U,
      SHCONTF_INCLUDEHIDDEN = 128U,
      SHCONTF_INIT_ON_FIRST_NEXT = 256U,
      SHCONTF_NETPRINTERSRCH = 512U,
      SHCONTF_SHAREABLE = 1024U,
      SHCONTF_STORAGE = 2048U,
      SHCONTF_NAVIGATION_ENUM = 4096U,
      SHCONTF_FASTITEMS = 8192U,
      SHCONTF_FLATLIST = 16384U,
      SHCONTF_ENABLE_ASYNC = 32768U,
    }

    [Flags]
    internal enum SFGAOF : uint
    {
      SFGAO_CANCOPY = 1U,
      SFGAO_CANMOVE = 2U,
      SFGAO_CANLINK = 4U,
      SFGAO_STORAGE = 8U,
      SFGAO_CANRENAME = 16U,
      SFGAO_CANDELETE = 32U,
      SFGAO_HASPROPSHEET = 64U,
      SFGAO_DROPTARGET = 256U,
      SFGAO_CAPABILITYMASK = SFGAO_DROPTARGET | SFGAO_HASPROPSHEET | SFGAO_CANDELETE | SFGAO_CANRENAME | SFGAO_CANLINK | SFGAO_CANMOVE | SFGAO_CANCOPY,
      SFGAO_SYSTEM = 4096U,
      SFGAO_ENCRYPTED = 8192U,
      SFGAO_ISSLOW = 16384U,
      SFGAO_GHOSTED = 32768U,
      SFGAO_LINK = 65536U,
      SFGAO_SHARE = 131072U,
      SFGAO_READONLY = 262144U,
      SFGAO_HIDDEN = 524288U,
      SFGAO_DISPLAYATTRMASK = SFGAO_HIDDEN | SFGAO_READONLY | SFGAO_SHARE | SFGAO_LINK | SFGAO_GHOSTED | SFGAO_ISSLOW,
      SFGAO_FILESYSANCESTOR = 268435456U,
      SFGAO_FOLDER = 536870912U,
      SFGAO_FILESYSTEM = 1073741824U,
      SFGAO_HASSUBFOLDER = 2147483648U,
      SFGAO_CONTENTSMASK = SFGAO_HASSUBFOLDER,
      SFGAO_VALIDATE = 16777216U,
      SFGAO_REMOVABLE = 33554432U,
      SFGAO_COMPRESSED = 67108864U,
      SFGAO_BROWSABLE = 134217728U,
      SFGAO_NONENUMERATED = 1048576U,
      SFGAO_NEWCONTENT = 2097152U,
      SFGAO_CANMONIKER = 4194304U,
      SFGAO_HASSTORAGE = SFGAO_CANMONIKER,
      SFGAO_STREAM = SFGAO_HASSTORAGE,
      SFGAO_STORAGEANCESTOR = 8388608U,
      SFGAO_STORAGECAPMASK = SFGAO_STORAGEANCESTOR | SFGAO_STREAM | SFGAO_FILESYSTEM | SFGAO_FOLDER | SFGAO_FILESYSANCESTOR | SFGAO_READONLY | SFGAO_LINK | SFGAO_STORAGE,
      SFGAO_PKEYSFGAOMASK = SFGAO_VALIDATE | SFGAO_CONTENTSMASK | SFGAO_READONLY | SFGAO_ISSLOW,
    }

    [Flags]
    internal enum SHGDNF : uint
    {
      SHGDN_NORMAL = 0U,
      SHGDN_INFOLDER = 1U,
      SHGDN_FOREDITING = 4096U,
      SHGDN_FORADDRESSBAR = 16384U,
      SHGDN_FORPARSING = 32768U,
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Auto)]
    internal struct COMDLG_FILTERSPEC
    {
      [MarshalAs(UnmanagedType.LPWStr)]
      internal string pszName;
      [MarshalAs(UnmanagedType.LPWStr)]
      internal string pszSpec;
    }

    internal enum FDAP
    {
      FDAP_BOTTOM,
      FDAP_TOP,
    }

    internal enum FDE_SHAREVIOLATION_RESPONSE
    {
      FDESVR_DEFAULT,
      FDESVR_ACCEPT,
      FDESVR_REFUSE,
    }

    internal enum FDE_OVERWRITE_RESPONSE
    {
      FDEOR_DEFAULT,
      FDEOR_ACCEPT,
      FDEOR_REFUSE,
    }

    internal enum SIGDN : uint
    {
      SIGDN_NORMALDISPLAY = 0U,
      SIGDN_PARENTRELATIVEPARSING = 2147581953U,
      SIGDN_DESKTOPABSOLUTEPARSING = 2147647488U,
      SIGDN_PARENTRELATIVEEDITING = 2147684353U,
      SIGDN_DESKTOPABSOLUTEEDITING = 2147794944U,
      SIGDN_FILESYSPATH = 2147844096U,
      SIGDN_URL = 2147909632U,
      SIGDN_PARENTRELATIVEFORADDRESSBAR = 2147991553U,
      SIGDN_PARENTRELATIVE = 2148007937U,
    }

    internal enum HRESULT : long
    {
      S_OK = 0L,
      S_FALSE = 1L,
      E_OUTOFMEMORY = 2147942414L,
      E_INVALIDARG = 2147942487L,
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct PROPERTYKEY
    {
      internal Guid fmtid;
      internal uint pid;
    }

    internal enum SIATTRIBFLAGS
    {
      SIATTRIBFLAGS_AND = 1,
      SIATTRIBFLAGS_OR = 2,
      SIATTRIBFLAGS_APPCOMPAT = 3,
    }

    private enum SHARD
    {
      SHARD_PATHW = 3,
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal class MEMORYSTATUSEX
    {
      public uint dwLength;
      public uint dwMemoryLoad;
      public ulong ullTotalPhys;
      public ulong ullAvailPhys;
      public ulong ullTotalPageFile;
      public ulong ullAvailPageFile;
      public ulong ullTotalVirtual;
      public ulong ullAvailVirtual;
      public ulong ullAvailExtendedVirtual;

      public MEMORYSTATUSEX()
      {
        this.dwLength = (uint) Marshal.SizeOf(typeof (NativeMethods.MEMORYSTATUSEX));
      }
    }

    internal enum WINSAT_ASSESSMENT_STATE
    {
      WINSAT_ASSESSMENT_STATE_MIN = 0,
      WINSAT_ASSESSMENT_STATE_UNKNOWN = 0,
      WINSAT_ASSESSMENT_STATE_VALID = 1,
      WINSAT_ASSESSMENT_STATE_INCOHERENT_WITH_HARDWARE = 2,
      WINSAT_ASSESSMENT_STATE_NOT_AVAILABLE = 3,
      WINSAT_ASSESSMENT_STATE_INVALID = 4,
      WINSAT_ASSESSMENT_STATE_MAX = 4,
    }

    internal enum WINSAT_ASSESSMENT_TYPE
    {
      WINSAT_ASSESSMENT_MEMORY,
      WINSAT_ASSESSMENT_CPU,
      WINSAT_ASSESSMENT_DISK,
      WINSAT_ASSESSMENT_D3D,
      WINSAT_ASSESSMENT_GRAPHICS,
    }

    [Guid("F8AD5D1F-3B47-4BDC-9375-7C6B1DA4ECA7")]
    [ComImport]
    internal interface IQueryRecentWinSATAssessment
    {
      object this[string xPath, string namespaces] { get; }

      NativeMethods.IProvideWinSATResultsInfo Info { get; }
    }

    [Guid("F8334D5D-568E-4075-875F-9DF341506640")]
    [ComImport]
    internal interface IProvideWinSATResultsInfo
    {
      NativeMethods.WINSAT_ASSESSMENT_STATE AssessmentState { get; }

      object AssessmentDateTime { get; }

      float SystemRating { get; }

      string RatingStateDesc { get; }

      [return: MarshalAs(UnmanagedType.Interface)]
      NativeMethods.IProvideWinSATAssessmentInfo GetAssessmentInfo(NativeMethods.WINSAT_ASSESSMENT_TYPE assessment);
    }

    [Guid("0CD1C380-52D3-4678-AC6F-E929E480BE9E")]
    [ComImport]
    internal interface IProvideWinSATAssessmentInfo
    {
      float Score { get; }

      string Title { get; }

      string Description { get; }
    }
  }
}
