using System;
using System.Collections.Generic;

namespace Microsoft.MetadataReader
{
	internal class CustomModifiers
	{
		private readonly List<Type> m_optional;

		private readonly List<Type> m_required;

		public Type[] OptionalCustomModifiers
		{
			get
			{
				if (this.m_optional == null)
				{
					return Type.EmptyTypes;
				}
				return this.m_optional.ToArray();
			}
		}

		public Type[] RequiredCustomModifiers
		{
			get
			{
				if (this.m_required == null)
				{
					return Type.EmptyTypes;
				}
				return this.m_required.ToArray();
			}
		}

		public CustomModifiers(List<Type> optModifiers, List<Type> reqModifiers)
		{
			this.m_optional = optModifiers;
			this.m_required = reqModifiers;
		}
	}
}