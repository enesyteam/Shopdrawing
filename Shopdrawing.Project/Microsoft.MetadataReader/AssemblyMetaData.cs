using Microsoft.Win32.SafeHandles;
using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Microsoft.MetadataReader
{
	internal struct AssemblyMetaData : IDisposable
	{
		public ushort majorVersion;

		public ushort minorVersion;

		public ushort buildNumber;

		public ushort revisionNumber;

		public UnmanagedStringMemoryHandle szLocale;

		public uint cbLocale;

		public UnusedIntPtr rdwProcessor;

		public uint ulProcessor;

		public UnusedIntPtr rOS;

		public uint ulOS;

		public CultureInfo Locale
		{
			get
			{
				if (this.szLocale == null)
				{
					return CultureInfo.InvariantCulture;
				}
				return new CultureInfo(this.LocaleString);
			}
		}

		public string LocaleString
		{
			get
			{
				if (this.szLocale == null)
				{
					return null;
				}
				if (this.szLocale.IsInvalid)
				{
					return string.Empty;
				}
				if (this.cbLocale <= 0)
				{
					return string.Empty;
				}
				int num = (int)this.cbLocale;
				return this.szLocale.GetAsString(num - 1);
			}
		}

		public System.Version Version
		{
			get
			{
				return new System.Version((int)this.majorVersion, (int)this.minorVersion, (int)this.buildNumber, (int)this.revisionNumber);
			}
		}

		public void Dispose()
		{
			if (this.szLocale != null)
			{
				this.szLocale.Dispose();
			}
		}

		public void Init()
		{
			this.szLocale = new UnmanagedStringMemoryHandle();
			this.cbLocale = 0;
			this.ulProcessor = 0;
			this.ulOS = 0;
		}
	}
}