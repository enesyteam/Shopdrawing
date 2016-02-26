using System;

namespace System.Reflection.Adds
{
	internal struct EmbeddedBlobPointer
	{
		private IntPtr m_data;

		internal IntPtr GetDangerousLivePointer
		{
			get
			{
				return this.m_data;
			}
		}
	}
}