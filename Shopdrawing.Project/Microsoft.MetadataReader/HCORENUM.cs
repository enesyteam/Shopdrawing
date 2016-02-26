using System;

namespace Microsoft.MetadataReader
{
	internal struct HCORENUM
	{
		private IntPtr hEnum;

		public void Close(IMetadataImport import)
		{
			if (this.hEnum != IntPtr.Zero)
			{
				import.CloseEnum(this.hEnum);
				this.hEnum = IntPtr.Zero;
			}
		}

		public void Close(IMetadataImport2 import)
		{
			if (this.hEnum != IntPtr.Zero)
			{
				import.CloseEnum(this.hEnum);
				this.hEnum = IntPtr.Zero;
			}
		}

		public void Close(IMetadataAssemblyImport import)
		{
			if (this.hEnum != IntPtr.Zero)
			{
				import.CloseEnum(this.hEnum);
				this.hEnum = IntPtr.Zero;
			}
		}
	}
}