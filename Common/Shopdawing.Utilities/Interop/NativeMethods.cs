// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Utility.Interop.NativeMethods
// Assembly: Microsoft.Expression.Utility, Version=5.0.30709.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: B77F0E80-A3D7-4861-BF76-6A6A586443F3
// Assembly location: C:\Users\M4600\Documents\Project\4.5\Microsoft.Expression.ProjectReferences\Microsoft.Expression.Utility.dll

using Microsoft.Expression.Utility.IO;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Microsoft.Expression.Utility.Interop
{
  internal static class NativeMethods
  {
    internal static IntPtr NO_PARENT = IntPtr.Zero;
    internal static int S_OK = 0;
    internal static Guid FOLDERID_LocalAppDataLow = new Guid(new byte[16]
    {
      (byte) 164,
      (byte) 161,
      (byte) 32,
      (byte) 165,
      (byte) sbyte.MinValue,
      (byte) 23,
      (byte) 246,
      (byte) 79,
      (byte) 189,
      (byte) 24,
      (byte) 22,
      (byte) 115,
      (byte) 67,
      (byte) 197,
      (byte) 175,
      (byte) 22
    });
    internal static uint GET_MODULE_HANDLE_EX_FLAG_PIN = 1U;
    internal static uint GET_MODULE_HANDLE_EX_FLAG_UNCHANGED_REFCOUNT = 2U;
    internal static uint GET_MODULE_HANDLE_EX_FLAG_FROM_ADDRESS = 4U;
    internal const ushort IMAGE_DOS_SIGNATURE = (ushort) 23117;
    internal const ushort IMAGE_NT_OPTIONAL_HDR32_MAGIC = (ushort) 267;
    internal const ushort IMAGE_NT_OPTIONAL_HDR64_MAGIC = (ushort) 523;
    public const int IMAGE_NUMBEROF_DIRECTORY_ENTRIES = 16;
    internal const int IMAGE_SIZEOF_SHORT_NAME = 8;
    internal const int IMAGE_DIRECTORY_ENTRY_EXPORT = 0;
    internal const short IMAGE_FILE_EXECUTABLE_IMAGE = (short) 2;
    internal const short IMAGE_FILE_DLL = (short) 8192;
    internal const short IMAGE_FILE_MACHINE_I386 = (short) 332;
    internal const short IMAGE_FILE_MACHINE_ARM = (short) 448;
    internal const short IMAGE_FILE_MACHINE_THUMB = (short) 450;
    internal const short IMAGE_FILE_MACHINE_ARMNT = (short) 452;
    internal const short IMAGE_FILE_MACHINE_AMD64 = (short) -31132;
    internal const uint INVALID_FILE_ATTRIBUTES = 4294967295U;
    internal const uint FILE_ATTRIBUTE_DIRECTORY = 16U;
    internal const uint FILE_ATTRIBUTE_VIRTUAL = 65536U;
    internal const uint FILE_ATTRIBUTE_HIDDEN = 2U;
    internal const int ERROR_INVALID_FUNCTION = 1;
    internal const int ERROR_FILE_NOT_FOUND = 2;
    internal const int ERROR_PATH_NOT_FOUND = 3;
    internal const int ERROR_ACCESS_DENIED = 5;
    internal const int ERROR_INVALID_DRIVE = 15;
    internal const int ERROR_NOT_READY = 21;
    internal const int ERROR_BAD_NETPATH = 53;
    internal const int ERROR_NETNAME_DELETED = 64;
    internal const int ERROR_NETWORK_ACCESS_DENIED = 65;
    internal const int ERROR_BAD_NET_NAME = 67;
    internal const int ERROR_INVALID_PARAMETER = 87;
    internal const int ERROR_INVALID_NAME = 123;
    internal const int ERROR_BAD_PATHNAME = 161;
    internal const int ERROR_NOT_FOUND = 1168;
    internal const int ERROR_DISK_CORRUPT = 1393;

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

    [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
    private static extern int SHGetKnownFolderPath([MarshalAs(UnmanagedType.LPStruct)] Guid rfid, uint dwFlags, IntPtr hToken, out string lpszPath);

    internal static string GetKnownFolderPath(Guid folderID)
    {
      string lpszPath;
      NativeMethods.SHGetKnownFolderPath(folderID, 0U, IntPtr.Zero, out lpszPath);
      int num = NativeMethods.S_OK;
      return lpszPath;
    }

    [SuppressUnmanagedCodeSecurity]
    [DllImport("kernel32.dll", EntryPoint = "GetFileAttributes", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern uint GetFileAttributesPrivate(string lpFileName);

    private static uint GetFileAttributesInternal(string path)
    {
      uint attributesPrivate = NativeMethods.GetFileAttributesPrivate(path);
      if ((int) attributesPrivate == -1)
      {
        int lastWin32Error = Marshal.GetLastWin32Error();
        switch (lastWin32Error)
        {
          case 161:
          case 1168:
          case 1393:
          case 87:
          case 123:
          case 21:
          case 53:
          case 67:
          case 1:
          case 2:
          case 3:
          case 15:
            return attributesPrivate;
          case 65:
          case 5:
            throw new UnauthorizedAccessException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} : '{1}'", new object[2]
            {
              (object) ErrorHandler.LastErrorToString(lastWin32Error),
              (object) path
            }));
          default:
            ErrorHandler.LastErrorToString(lastWin32Error);
            break;
        }
      }
      return attributesPrivate;
    }

    internal static bool GetFileAttributes(string path, out FileAttributes attributes)
    {
      attributes = FileAttributes.Temporary;
      uint attributesInternal = NativeMethods.GetFileAttributesInternal(path);
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

    internal static bool PathHidden(string path)
    {
      return ((int) NativeMethods.GetFileAttributesInternal(path) & 2) == -1;
    }

    [SuppressUnmanagedCodeSecurity]
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool SetFileAttributes(string lpFileName, [MarshalAs(UnmanagedType.U4)] FileAttributes dwFileAttributes);

    [SuppressUnmanagedCodeSecurity]
    [DllImport("shell32.dll", EntryPoint = "SHFileOperation", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern int SHFileOperation32(ref NativeMethods.SHFILEOPSTRUCT32 lpFileOp);

    [SuppressUnmanagedCodeSecurity]
    [DllImport("shell32.dll", EntryPoint = "SHFileOperation", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern int SHFileOperation64(ref NativeMethods.SHFILEOPSTRUCT64 lpFileOp);

    [SuppressUnmanagedCodeSecurity]
    [DllImport("shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern void SHChangeNotify(uint wEventId, uint uFlags, [MarshalAs(UnmanagedType.LPWStr)] string dwItem1, IntPtr dwItem2);

    internal static void DeleteWithUndo(string path)
    {
      NativeMethods.SHFILEOPSTRUCT32 lpFileOp = new NativeMethods.SHFILEOPSTRUCT32();
      lpFileOp.fFlags = (ushort) 13396;
      lpFileOp.hNameMappings = IntPtr.Zero;
      try
      {
        lpFileOp.hwnd = Process.GetCurrentProcess().MainWindowHandle;
      }
      catch (Exception ex)
      {
        if (!(ex is SecurityException) && !(ex is InvalidOperationException) && !(ex is NotSupportedException))
          throw;
        else
          lpFileOp.hwnd = IntPtr.Zero;
      }
      lpFileOp.lpszProgressTitle = string.Empty;
      lpFileOp.pFrom = path + "\0\0";
      lpFileOp.pTo = (string) null;
      lpFileOp.wFunc = 3U;
      lpFileOp.fAnyOperationsAborted = false;
      bool flag = NativeMethods.FileExists(path);
      if (IntPtr.Size == 4)
        NativeMethods.SHFileOperation32(ref lpFileOp);
      else if (IntPtr.Size == 8)
        NativeMethods.SHFileOperation64(ref new NativeMethods.SHFILEOPSTRUCT64()
        {
          fFlags = lpFileOp.fFlags,
          hNameMappings = lpFileOp.hNameMappings,
          hwnd = lpFileOp.hwnd,
          lpszProgressTitle = lpFileOp.lpszProgressTitle,
          pFrom = lpFileOp.pFrom,
          pTo = lpFileOp.pTo,
          wFunc = lpFileOp.wFunc,
          fAnyOperationsAborted = lpFileOp.fAnyOperationsAborted
        });
      if (flag)
        NativeMethods.SHChangeNotify(4U, 5U, path, IntPtr.Zero);
      else
        NativeMethods.SHChangeNotify(16U, 5U, path, IntPtr.Zero);
    }

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern SafeModuleHandle LoadLibraryEx(string lpFileName, IntPtr hFile, NativeMethods.LoadLibraryFlags dwFlags);

    internal static SafeModuleHandle LoadLibraryForResourceAccess(string path)
    {
      return NativeMethods.LoadLibraryEx(path, IntPtr.Zero, NativeMethods.LoadLibraryFlags.LOAD_LIBRARY_AS_DATAFILE | NativeMethods.LoadLibraryFlags.LOAD_LIBRARY_AS_IMAGE_RESOURCE);
    }

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern int LoadString(SafeModuleHandle hInstance, uint uID, out IntPtr lpBuffer, int nBufferMax);

    internal static string LoadStringResource(SafeModuleHandle module, uint identifier)
    {
      IntPtr lpBuffer;
      int len = NativeMethods.LoadString(module, identifier, out lpBuffer, 0);
      if (len == 0)
        return (string) null;
      return Marshal.PtrToStringUni(lpBuffer, len);
    }

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern IntPtr LoadIcon(SafeModuleHandle hInstance, string lpIconName);

    [DllImport("user32.dll", EntryPoint = "LoadIcon", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr LoadSystemIcon(IntPtr hInstance, UIntPtr lpIconName);

    internal static ImageSource LoadIcon(SystemIcon systemIcon)
    {
      IntPtr icon = NativeMethods.LoadSystemIcon(IntPtr.Zero, new UIntPtr((uint) systemIcon));
      if (icon != IntPtr.Zero)
        return (ImageSource) Imaging.CreateBitmapSourceFromHIcon(icon, Int32Rect.Empty, (BitmapSizeOptions) null);
      return (ImageSource) null;
    }

    internal static ImageSource LoadIcon(SafeModuleHandle module, string resourceIdentifier, int preferredWidth = 0, int preferredHeight = 0)
    {
      if (module == null)
        return (ImageSource) null;
      IntPtr handle = NativeMethods.LoadIcon(module, resourceIdentifier);
      if (handle != IntPtr.Zero)
      {
        Icon original = Icon.FromHandle(handle);
        if (original != null)
        {
          if (preferredHeight != 0 && preferredWidth != 0)
            original = new Icon(original, preferredWidth, preferredHeight);
          return (ImageSource) Imaging.CreateBitmapSourceFromHIcon(original.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }
      }
      return (ImageSource) null;
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool FreeLibrary(SafeModuleHandle hModule);

    [DllImport("User32", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern IntPtr SetFocus(IntPtr hWnd);

    [DllImport("User32", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern IntPtr GetFocus();

    [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
    public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref NativeMethods.SHFILEINFO psfi, uint cbFileInfo, NativeMethods.SHGFI uFlags);

    [DllImport("kernel32.dll", SetLastError = true, BestFitMapping = false)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool GetModuleHandleEx([In] uint dwFlags, [MarshalAs(UnmanagedType.LPStr), In] string lpModuleName, out IntPtr hModule);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, BestFitMapping = false)]
    internal static extern int WaitForSingleObject(SafeHandle hHandle, int dwMilliseconds);

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

    public struct IMAGE_DOS_HEADER
    {
      public ushort e_magic;
      public ushort e_cblp;
      public ushort e_cp;
      public ushort e_crlc;
      public ushort e_cparhdr;
      public ushort e_minalloc;
      public ushort e_maxalloc;
      public ushort e_ss;
      public ushort e_sp;
      public ushort e_csum;
      public ushort e_ip;
      public ushort e_cs;
      public ushort e_lfarlc;
      public ushort e_ovno;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      public ushort[] e_res1;
      public ushort e_oemid;
      public ushort e_oeminfo;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
      public ushort[] e_res2;
      public int e_lfanew;
    }

    public struct IMAGE_NT_HEADERS
    {
      public uint Signature;
      public NativeMethods.IMAGE_FILE_HEADER FileHeader;
      public NativeMethods.IMAGE_OPTIONAL_HEADER OptionalHeader;
    }

    public struct IMAGE_DATA_DIRECTORY
    {
      public uint VirtualAddress;
      public uint Size;
    }

    public struct IMAGE_OPTIONAL_HEADER
    {
      public ushort Magic;
      public byte MajorLinkerVersion;
      public byte MinorLinkerVersion;
      public uint SizeOfCode;
      public uint SizeOfInitializedData;
      public uint SizeOfUninitializedData;
      public uint AddressOfEntryPoint;
      public uint BaseOfCode;
      public uint BaseOfData;
      public uint ImageBase;
      public uint SectionAlignment;
      public uint FileAlignment;
      public ushort MajorOperatingSystemVersion;
      public ushort MinorOperatingSystemVersion;
      public ushort MajorImageVersion;
      public ushort MinorImageVersion;
      public ushort MajorSubsystemVersion;
      public ushort MinorSubsystemVersion;
      public uint Win32VersionValue;
      public uint SizeOfImage;
      public uint SizeOfHeaders;
      public uint CheckSum;
      public ushort Subsystem;
      public ushort DllCharacteristics;
      public uint SizeOfStackReserve;
      public uint SizeOfStackCommit;
      public uint SizeOfHeapReserve;
      public uint SizeOfHeapCommit;
      public uint LoaderFlags;
      public uint NumberOfRvaAndSizes;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
      public NativeMethods.IMAGE_DATA_DIRECTORY[] DataDirectory;
    }

    public struct IMAGE_FILE_HEADER
    {
      public ushort Machine;
      public ushort NumberOfSections;
      public uint TimeDateStamp;
      public uint PointerToSymbolTable;
      public uint NumberOfSymbols;
      public ushort SizeOfOptionalHeader;
      public ushort Characteristics;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct IMAGE_SECTION_HEADER_MISC
    {
      [FieldOffset(0)]
      public uint PhysicalAddress;
      [FieldOffset(0)]
      public uint VirtualSize;
    }

    public struct IMAGE_SECTION_HEADER
    {
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
      public byte[] Name;
      public NativeMethods.IMAGE_SECTION_HEADER_MISC Misc;
      public uint VirtualAddress;
      public uint SizeOfRawData;
      public uint PointerToRawData;
      public uint PointerToRelocations;
      public uint PointerToLinenumbers;
      public ushort NumberOfRelocations;
      public ushort NumberOfLinenumbers;
      public uint Characteristics;
    }

    public struct IMAGE_EXPORT_DIRECTORY
    {
      public uint Characteristics;
      public uint TimeDateStamp;
      public ushort MajorVersion;
      public ushort MinorVersion;
      public uint Name;
      public uint Base;
      public uint NumberOfFunctions;
      public uint NumberOfNames;
      public uint AddressOfFunctions;
      public uint AddressOfNames;
      public uint AddressOfNameOrdinals;
    }

    private enum SHARD
    {
      SHARD_PATHW = 3,
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Auto)]
    internal struct SHFILEOPSTRUCT32
    {
      internal IntPtr hwnd;
      internal uint wFunc;
      [MarshalAs(UnmanagedType.LPTStr)]
      internal string pFrom;
      [MarshalAs(UnmanagedType.LPTStr)]
      internal string pTo;
      internal ushort fFlags;
      internal bool fAnyOperationsAborted;
      internal IntPtr hNameMappings;
      [MarshalAs(UnmanagedType.LPTStr)]
      internal string lpszProgressTitle;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct SHFILEOPSTRUCT64
    {
      internal IntPtr hwnd;
      internal uint wFunc;
      [MarshalAs(UnmanagedType.LPTStr)]
      internal string pFrom;
      [MarshalAs(UnmanagedType.LPTStr)]
      internal string pTo;
      internal ushort fFlags;
      internal bool fAnyOperationsAborted;
      internal IntPtr hNameMappings;
      [MarshalAs(UnmanagedType.LPTStr)]
      internal string lpszProgressTitle;
    }

    [Flags]
    private enum LoadLibraryFlags : uint
    {
      LOAD_LIBRARY_AS_DATAFILE = 2U,
      LOAD_LIBRARY_AS_IMAGE_RESOURCE = 32U,
    }

    [Flags]
    public enum SHGFI : uint
    {
      Icon = 256U,
      DisplayName = 512U,
      TypeName = 1024U,
      Attributes = 2048U,
      IconLocation = 4096U,
      ExeType = 8192U,
      SysIconIndex = 16384U,
      LinkOverlay = 32768U,
      Selected = 65536U,
      Attr_Specified = 131072U,
      LargeIcon = 0U,
      SmallIcon = 1U,
      OpenIcon = 2U,
      ShellIconSize = 4U,
      PIDL = 8U,
      UseFileAttributes = 16U,
      AddOverlays = 32U,
      OverlayIndex = 64U,
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct SHFILEINFO
    {
      public IntPtr hIcon;
      public IntPtr iIcon;
      public uint dwAttributes;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
      public string szDisplayName;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
      public string szTypeName;
    }
  }
}
