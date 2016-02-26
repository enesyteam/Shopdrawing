using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security;

namespace Microsoft.Expression.Project
{
	internal static class NativeMethods
	{
		internal const uint WM_CLOSE = 16;

		internal const uint WM_QUERYENDSESSION = 17;

		internal const int S_OK = 0;

		internal const int S_FALSE = 1;

		internal const int STG_E_FILENOTFOUND = -2147287038;

		internal const int STG_E_TOOMANYOPENFILES = -2147287036;

		internal const int STG_E_ACCESSDENIED = -2147287035;

		internal const int STG_E_SHAREVIOLATION = -2147287008;

		internal const int STG_E_REVERTED = -2147286782;

		internal const int STG_E_INVALIDNAME = -2147286788;

		internal const int STG_E_INSUFFICIENTMEMORY = -2147287032;

		internal const int STG_E_LOCKVIOLATION = -2147287007;

		internal const int STG_E_OLDDLL = -2147286779;

		[DllImport("user32.Dll", CharSet=CharSet.Auto, ExactSpelling=false, SetLastError=true)]
		internal static extern int EnumWindows(Microsoft.Expression.Project.NativeMethods.EnumWindowsCallBack callback, IntPtr lParam);

		[DllImport("user32.dll", CharSet=CharSet.Auto, ExactSpelling=false, SetLastError=true)]
		internal static extern int GetWindowThreadProcessId(IntPtr hwnd, out uint lpdwProcessId);

		[DllImport("user32.Dll", CharSet=CharSet.Auto, ExactSpelling=false, SetLastError=true)]
		internal static extern int IsWindow(IntPtr hwnd);

		[DllImport("user32.dll", CharSet=CharSet.Auto, ExactSpelling=false, SetLastError=true)]
		internal static extern int SendMessageTimeout(IntPtr hwnd, uint msg, UIntPtr wParam, IntPtr lParam, Microsoft.Expression.Project.NativeMethods.SendMessageTimeoutFlags fuFlags, uint timeout, out IntPtr result);

		[DllImport("shell32.dll", CharSet=CharSet.Auto, ExactSpelling=false, SetLastError=true)]
		internal static extern void SHChangeNotify(uint wEventId, uint uFlags, string dwItem1, IntPtr dwItem2);

		[DllImport("shell32.dll", CharSet=CharSet.Auto, EntryPoint="SHFileOperation", ExactSpelling=false, SetLastError=true)]
		internal static extern int SHFileOperation32(ref Microsoft.Expression.Project.NativeMethods.SHFILEOPSTRUCT32 lpFileOp);

		[DllImport("shell32.dll", CharSet=CharSet.Auto, EntryPoint="SHFileOperation", ExactSpelling=false, SetLastError=true)]
		internal static extern int SHFileOperation64(ref Microsoft.Expression.Project.NativeMethods.SHFILEOPSTRUCT64 lpFileOp);

		[DllImport("ole32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		internal static extern int StgCreateDocfile(string pwcsName, Microsoft.Expression.Project.NativeMethods.STGM grfMode, uint reserved, out Microsoft.Expression.Project.NativeMethods.IStorage iStorage);

		[DllImport("ole32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		internal static extern int StgIsStorageFile(string pcwsName);

		[DllImport("ole32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		internal static extern int StgOpenStorage(string pcwsName, Microsoft.Expression.Project.NativeMethods.IStorage ptsgPriority, Microsoft.Expression.Project.NativeMethods.STGM grfMode, IntPtr snbExclude, uint reserved, out Microsoft.Expression.Project.NativeMethods.IStorage ppstgOpen);

		public delegate bool EnumWindowsCallBack(IntPtr hwnd, IntPtr lParam);

		[Guid("0000000d-0000-0000-C000-000000000046")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		[SuppressUnmanagedCodeSecurity]
		internal interface IEnumSTATSTG
		{
			void Clone(out Microsoft.Expression.Project.NativeMethods.IEnumSTATSTG ppenum);

			void Next(uint celt, out System.Runtime.InteropServices.ComTypes.STATSTG rgelt, out uint pceltFetched);

			void Reset();

			void Skip(uint celt);
		}

		[Guid("0000000b-0000-0000-C000-000000000046")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		[SuppressUnmanagedCodeSecurity]
		internal interface IStorage
		{
			void Commit(int grfCommitFlags);

			void CopyTo(int ciidExclude, Guid[] rgiidExclude, IntPtr snbExclude, Microsoft.Expression.Project.NativeMethods.IStorage ppstg);

			int CreateStorage([In] string pwcsName, Microsoft.Expression.Project.NativeMethods.STGM grfMode, int reserved1, int reserved2, out Microsoft.Expression.Project.NativeMethods.IStorage ppstg);

			int CreateStream([In] string pwcsName, Microsoft.Expression.Project.NativeMethods.STGM grfMode, int reserved1, int reserved2, out Microsoft.Expression.Project.NativeMethods.IStream ppstm);

			void DestroyElement([In] string pwcsName);

			void EnumElements(int reserved1, IntPtr reserved2, int reserved3, out Microsoft.Expression.Project.NativeMethods.IEnumSTATSTG ppEnum);

			void MoveElementTo([In] string pwcsName, Microsoft.Expression.Project.NativeMethods.IStorage pstgDest, [In] string pwcsNewName, int grfFlags);

			int OpenStorage([In] string pwcsName, Microsoft.Expression.Project.NativeMethods.IStorage pstgPriority, Microsoft.Expression.Project.NativeMethods.STGM grfMode, IntPtr snbExclude, int reserved, out Microsoft.Expression.Project.NativeMethods.IStorage ppstg);

			int OpenStream([In] string pwcsName, IntPtr reserved1, Microsoft.Expression.Project.NativeMethods.STGM grfMode, int reserved2, out Microsoft.Expression.Project.NativeMethods.IStream ppstm);

			void RenameElement([In] string pwcsOldName, [In] string pwcsNewName);

			void Revert();

			void SetClass(ref Guid clsid);

			void SetElementTimes([In] string pwcsName, System.Runtime.InteropServices.ComTypes.FILETIME pctime, System.Runtime.InteropServices.ComTypes.FILETIME patime, System.Runtime.InteropServices.ComTypes.FILETIME pmtime);

			void SetStateBits(int grfStateBits, int grfMask);

			void Stat(out System.Runtime.InteropServices.ComTypes.STATSTG pstatstg, int grfStatFlag);
		}

		[Guid("0000000c-0000-0000-C000-000000000046")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		[SuppressUnmanagedCodeSecurity]
		internal interface IStream
		{
			void Clone(out Microsoft.Expression.Project.NativeMethods.IStream ppstm);

			void Commit(Microsoft.Expression.Project.NativeMethods.STGC grfCommitFlags);

			void CopyTo(Microsoft.Expression.Project.NativeMethods.IStream pstm, ulong cb, out ulong pcbRead, out ulong pcbWritten);

			void LockRegion(ulong libOffset, ulong cb, uint dwLockType);

			void Read([Out] byte[] pv, uint cb, out uint pcbRead);

			void Revert();

			void Seek(long dlibMove, Microsoft.Expression.Project.NativeMethods.STREAM_SEEK dwOrigin, out ulong plibNewPosition);

			void SetSize(ulong libNewSize);

			void Stat(out System.Runtime.InteropServices.ComTypes.STATSTG pstatstg, Microsoft.Expression.Project.NativeMethods.STATFLAG grfStatFlag);

			void UnlockRegion(ulong libOffset, ulong cb, uint dwLockType);

			void Write(byte[] pv, uint cb, out uint pcbWritten);
		}

		[Flags]
		internal enum SendMessageTimeoutFlags : uint
		{
			SMTO_NORMAL = 0,
			SMTO_BLOCK = 1,
			SMTO_ABORTIFHUNG = 2,
			SMTO_NOTIMEOUTIFNOTHUNG = 8
		}

		internal struct SHFILEOPSTRUCT32
		{
			internal IntPtr hwnd;

			internal uint wFunc;

			internal string pFrom;

			internal string pTo;

			internal ushort fFlags;

			internal bool fAnyOperationsAborted;

			internal IntPtr hNameMappings;

			internal string lpszProgressTitle;
		}

		internal struct SHFILEOPSTRUCT64
		{
			internal IntPtr hwnd;

			internal uint wFunc;

			internal string pFrom;

			internal string pTo;

			internal ushort fFlags;

			internal bool fAnyOperationsAborted;

			internal IntPtr hNameMappings;

			internal string lpszProgressTitle;
		}

		internal enum STATFLAG
		{
			STATFLAG_DEFAULT,
			STATFLAG_NONAME
		}

		[Flags]
		internal enum STGC : uint
		{
			STGC_DEFAULT = 0,
			STGC_OVERWRITE = 1,
			STGC_ONLYIFCURRENT = 2,
			STGC_DANGEROUSLYCOMMITMERELYTODISKCACHE = 4,
			STGC_CONSOLIDATE = 8
		}

		[Flags]
		internal enum STGFMT
		{
			STORAGE = 0,
			FILE = 3,
			ANY = 4,
			DOCFILE = 5
		}

		[Flags]
		internal enum STGM
		{
			DIRECT = 0,
			FAILIFTHERE = 0,
			READ = 0,
			WRITE = 1,
			READWRITE = 2,
			SHARE_EXCLUSIVE = 16,
			SHARE_DENY_WRITE = 32,
			SHARE_DENY_READ = 48,
			SHARE_DENY_NONE = 64,
			CREATE = 4096,
			TRANSACTED = 65536,
			CONVERT = 131072,
			PRIORITY = 262144,
			NOSCRATCH = 1048576,
			NOSNAPSHOT = 2097152,
			DIRECT_SWMR = 4194304,
			DELETEONRELEASE = 67108864,
			SIMPLE = 134217728
		}

		internal enum STREAM_SEEK : uint
		{
			STREAM_SEEK_SET,
			STREAM_SEEK_CUR,
			STREAM_SEEK_END
		}

		internal enum VariantTypes : uint
		{
			VT_EMPTY = 0,
			VT_NULL = 1,
			VT_I2 = 2,
			VT_I4 = 3,
			VT_R4 = 4,
			VT_R8 = 5,
			VT_CY = 6,
			VT_DATE = 7,
			VT_BSTR = 8,
			VT_DISPATCH = 9,
			VT_ERROR = 10,
			VT_BOOL = 11,
			VT_VARIANT = 12,
			VT_UNKNOWN = 13,
			VT_DECIMAL = 14,
			VT_I1 = 16,
			VT_UI1 = 17,
			VT_UI2 = 18,
			VT_UI4 = 19,
			VT_INT = 22,
			VT_UINT = 23,
			VT_RECORD = 36,
			VT_TYPEMASK = 4095,
			VT_ARRAY = 8192,
			VT_BYREF = 16384
		}
	}
}