using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace Microsoft.MetadataReader
{
	[SecurityPermission(SecurityAction.Demand, UnmanagedCode=true)]
	internal sealed class UnmanagedStringMemoryHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		internal UnmanagedStringMemoryHandle() : base(true)
		{
		}

		internal UnmanagedStringMemoryHandle(int countBytes) : base(true)
		{
			if (countBytes == 0)
			{
				return;
			}
			base.SetHandle(Marshal.AllocHGlobal(countBytes));
		}

		public string GetAsString(int countCharsNoNull)
		{
			return Marshal.PtrToStringUni(this.handle, countCharsNoNull);
		}

		protected override bool ReleaseHandle()
		{
			if (this.handle == IntPtr.Zero)
			{
				return false;
			}
			Marshal.FreeHGlobal(this.handle);
			this.handle = IntPtr.Zero;
			return true;
		}
	}
}