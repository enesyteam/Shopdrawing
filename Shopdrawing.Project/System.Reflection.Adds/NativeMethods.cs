using Microsoft.Win32.SafeHandles;
using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;

namespace System.Reflection.Adds
{
	internal static class NativeMethods
	{
		private const string Kernel32LibraryName = "kernel32.dll";

		private const int FILE_TYPE_DISK = 1;

		private const int GENERIC_READ = -2147483648;

		[DllImport("kernel32.dll", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern bool CloseHandle(IntPtr handle);

		[DllImport("kernel32.dll", BestFitMapping=false, CharSet=CharSet.Auto, ExactSpelling=false, SetLastError=true)]
		private static extern SafeFileHandle CreateFile(string lpFileName, int dwDesiredAccess, FileShare dwShareMode, IntPtr securityAttrs, FileMode dwCreationDisposition, int dwFlagsAndAttributes, IntPtr hTemplateFile);

		[DllImport("kernel32.dll", BestFitMapping=false, CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern System.Reflection.Adds.NativeMethods.SafeWin32Handle CreateFileMapping(SafeFileHandle hFile, IntPtr lpFileMappingAttributes, System.Reflection.Adds.NativeMethods.PageProtection flProtect, uint dwMaximumSizeHigh, uint dwMaximumSizeLow, string lpName);

		internal static long FileSize(SafeFileHandle handle)
		{
			int num = 0;
			int fileSize = 0;
			fileSize = System.Reflection.Adds.NativeMethods.GetFileSize(handle, out num);
			if (fileSize == -1)
			{
				Marshal.ThrowExceptionForHR(Marshal.GetLastWin32Error());
			}
			return (long)((long)num << 32 | (ulong)fileSize);
		}

		[DllImport("kernel32.dll", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		private static extern int GetFileSize(SafeFileHandle hFile, out int highSize);

		[DllImport("kernel32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		private static extern int GetFileType(SafeFileHandle handle);

		[DllImport("kernel32.dll", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern System.Reflection.Adds.NativeMethods.SafeMapViewHandle MapViewOfFile(System.Reflection.Adds.NativeMethods.SafeWin32Handle hFileMappingObject, uint dwDesiredAccess, uint dwFileOffsetHigh, uint dwFileOffsetLow, IntPtr dwNumberOfBytesToMap);

		internal static SafeFileHandle SafeOpenFile(string fileName)
		{
			if (fileName == null)
			{
				throw new ArgumentNullException("fileName");
			}
			if (fileName.Length == 0 || fileName.StartsWith("\\\\.\\", StringComparison.Ordinal))
			{
				CultureInfo invariantCulture = CultureInfo.InvariantCulture;
				string invalidFileName = MetadataStringTable.InvalidFileName;
				object[] objArray = new object[] { fileName };
				throw new ArgumentException(string.Format(invariantCulture, invalidFileName, objArray));
			}
			SafeFileHandle safeFileHandle = System.Reflection.Adds.NativeMethods.CreateFile(fileName, -2147483648, FileShare.Read, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);
			if (safeFileHandle.IsInvalid)
			{
				Marshal.ThrowExceptionForHR(Marshal.GetLastWin32Error());
			}
			else if (System.Reflection.Adds.NativeMethods.GetFileType(safeFileHandle) != 1)
			{
				safeFileHandle.Dispose();
				throw new ArgumentException(MetadataStringTable.UnsupportedImageType);
			}
			return safeFileHandle;
		}

		[DllImport("kernel32.dll", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern bool UnmapViewOfFile(IntPtr baseAddress);

		[Flags]
		public enum PageProtection : uint
		{
			NoAccess = 1,
			Readonly = 2,
			ReadWrite = 4,
			WriteCopy = 8,
			Execute = 16,
			ExecuteRead = 32,
			ExecuteReadWrite = 64,
			ExecuteWriteCopy = 128,
			Guard = 256,
			NoCache = 512,
			WriteCombine = 1024
		}

		public sealed class SafeMapViewHandle : SafeHandleZeroOrMinusOneIsInvalid
		{
			public IntPtr BaseAddress
			{
				get
				{
					return this.handle;
				}
			}

			private SafeMapViewHandle() : base(true)
			{
			}

			protected override bool ReleaseHandle()
			{
				return System.Reflection.Adds.NativeMethods.UnmapViewOfFile(this.handle);
			}
		}

		public sealed class SafeWin32Handle : SafeHandleZeroOrMinusOneIsInvalid
		{
			private SafeWin32Handle() : base(true)
			{
			}

			protected override bool ReleaseHandle()
			{
				return System.Reflection.Adds.NativeMethods.CloseHandle(this.handle);
			}
		}
	}
}