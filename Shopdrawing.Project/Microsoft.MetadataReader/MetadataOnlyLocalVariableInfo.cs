using System;
using System.Reflection;

namespace Microsoft.MetadataReader
{
	internal class MetadataOnlyLocalVariableInfo : LocalVariableInfo
	{
		private readonly Type m_type;

		private readonly int m_index;

		private readonly bool m_fPinned;

		public override bool IsPinned
		{
			get
			{
				return this.m_fPinned;
			}
		}

		public override int LocalIndex
		{
			get
			{
				return this.m_index;
			}
		}

		public override Type LocalType
		{
			get
			{
				return this.m_type;
			}
		}

		public MetadataOnlyLocalVariableInfo(int index, Type type, bool fPinned)
		{
			this.m_type = type;
			this.m_index = index;
			this.m_fPinned = fPinned;
		}
	}
}