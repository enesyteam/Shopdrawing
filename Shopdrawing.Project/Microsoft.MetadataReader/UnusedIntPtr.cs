using System;

namespace Microsoft.MetadataReader
{
	internal struct UnusedIntPtr
	{
		private IntPtr zeroPtr;

		public static UnusedIntPtr Zero
		{
			get
			{
				return new UnusedIntPtr()
				{
					zeroPtr = IntPtr.Zero
				};
			}
		}
	}
}