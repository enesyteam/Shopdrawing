using Microsoft.Win32.SafeHandles;
using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;

namespace System.Reflection.Adds
{
	internal class FileMapping : IDisposable
	{
		private readonly string m_fileName;

		private readonly string m_filePath;

		private readonly long m_fileLength;

		private readonly SafeFileHandle m_fileHandle;

		private readonly System.Reflection.Adds.NativeMethods.SafeWin32Handle m_fileMapping;

		private readonly System.Reflection.Adds.NativeMethods.SafeMapViewHandle m_View;

		public IntPtr BaseAddress
		{
			get
			{
				return this.m_View.BaseAddress;
			}
		}

		public long Length
		{
			get
			{
				return this.m_fileLength;
			}
		}

		public string Path
		{
			get
			{
				return this.m_filePath;
			}
		}

		public FileMapping(string fileName)
		{
			this.m_fileName = fileName;
			this.m_fileHandle = System.Reflection.Adds.NativeMethods.SafeOpenFile(fileName);
			this.m_fileLength = System.Reflection.Adds.NativeMethods.FileSize(this.m_fileHandle);
			this.m_filePath = System.IO.Path.GetFullPath(this.m_fileName);
			this.m_fileMapping = System.Reflection.Adds.NativeMethods.CreateFileMapping(this.m_fileHandle, IntPtr.Zero, System.Reflection.Adds.NativeMethods.PageProtection.Readonly, 0, 0, null);
			if (this.m_fileMapping.IsInvalid)
			{
				Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
			}
			this.m_View = System.Reflection.Adds.NativeMethods.MapViewOfFile(this.m_fileMapping, 4, 0, 0, IntPtr.Zero);
			if (this.m_View.IsInvalid)
			{
				Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
			}
		}

		public void Dispose()
		{
			if (this.m_View != null)
			{
				this.m_View.Close();
			}
			if (this.m_fileMapping != null)
			{
				this.m_fileMapping.Close();
			}
			if (this.m_fileHandle != null)
			{
				this.m_fileHandle.Close();
			}
			GC.SuppressFinalize(this);
		}

		public override string ToString()
		{
			if (this.m_View == null || this.m_fileMapping == null)
			{
				return this.m_fileName;
			}
			if (this.m_View.IsInvalid)
			{
				return string.Concat(this.m_fileName, " (closed)");
			}
			CultureInfo invariantCulture = CultureInfo.InvariantCulture;
			object[] mFileName = new object[] { this.m_fileName, null, null };
			mFileName[1] = this.BaseAddress.ToString("x");
			long length = this.Length;
			mFileName[2] = length.ToString("x", CultureInfo.InvariantCulture);
			return string.Format(invariantCulture, "{0} Addr=0x{1}, Length=0x{2}", mFileName);
		}
	}
}