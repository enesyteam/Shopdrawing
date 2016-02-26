using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Microsoft.MetadataReader
{
	internal class SimpleParameterInfo : ParameterInfo
	{
		private readonly MemberInfo m_member;

		private readonly Type m_paramType;

		private readonly int m_position;

		public override ParameterAttributes Attributes
		{
			get
			{
				return ParameterAttributes.None;
			}
		}

		public override object DefaultValue
		{
			get
			{
				return null;
			}
		}

		public override MemberInfo Member
		{
			get
			{
				return this.m_member;
			}
		}

		public override int MetadataToken
		{
			get
			{
				return 134217728;
			}
		}

		public override string Name
		{
			get
			{
				return string.Empty;
			}
		}

		public override Type ParameterType
		{
			get
			{
				return this.m_paramType;
			}
		}

		public override int Position
		{
			get
			{
				return this.m_position;
			}
		}

		public override object RawDefaultValue
		{
			get
			{
				return null;
			}
		}

		internal SimpleParameterInfo(MemberInfo member, Type paramType, int position)
		{
			this.m_member = member;
			this.m_paramType = paramType;
			this.m_position = position;
		}

		public override IList<CustomAttributeData> GetCustomAttributesData()
		{
			return new CustomAttributeData[0];
		}

		public override Type[] GetOptionalCustomModifiers()
		{
			return Type.EmptyTypes;
		}

		public override Type[] GetRequiredCustomModifiers()
		{
			return Type.EmptyTypes;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			MetadataOnlyCommonType.TypeSigToString(this.ParameterType, stringBuilder);
			stringBuilder.Append(' ');
			return stringBuilder.ToString();
		}
	}
}