using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Reflection.Adds;
using System.Text;

namespace Microsoft.MetadataReader
{
	internal class MetadataOnlyParameterInfo : ParameterInfo
	{
		private readonly MetadataOnlyModule m_resolver;

		private readonly int m_parameterToken;

		private readonly ParameterAttributes m_attrib;

		private readonly Type m_paramType;

		private readonly CustomModifiers m_customModifiers;

		private readonly string m_name;

		private readonly int m_position;

		private readonly int m_parentMemberToken;

		public override ParameterAttributes Attributes
		{
			get
			{
				return this.m_attrib;
			}
		}

		public override object DefaultValue
		{
			get
			{
				throw new InvalidOperationException();
			}
		}

		public override MemberInfo Member
		{
			get
			{
				return this.m_resolver.ResolveMethod(this.m_parentMemberToken);
			}
		}

		public override int MetadataToken
		{
			get
			{
				return this.m_parameterToken;
			}
		}

		public override string Name
		{
			get
			{
				return this.m_name;
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
				throw new NotImplementedException();
			}
		}

		internal MetadataOnlyParameterInfo(MetadataOnlyModule resolver, Token parameterToken, Type paramType, CustomModifiers customModifiers)
		{
			uint num;
			uint num1;
			uint num2;
			uint num3;
			uint num4;
			UnusedIntPtr unusedIntPtr;
			this.m_resolver = resolver;
			this.m_parameterToken = parameterToken;
			this.m_paramType = paramType;
			this.m_customModifiers = customModifiers;
			IMetadataImport rawImport = this.m_resolver.RawImport;
			rawImport.GetParamProps(this.m_parameterToken, out this.m_parentMemberToken, out num, null, 0, out num4, out num1, out num2, out unusedIntPtr, out num3);
			StringBuilder stringBuilder = new StringBuilder((int)num4);
			rawImport.GetParamProps(this.m_parameterToken, out this.m_parentMemberToken, out num, stringBuilder, (uint)stringBuilder.Capacity, out num4, out num1, out num2, out unusedIntPtr, out num3);
			this.m_name = stringBuilder.ToString();
			this.m_position = (int)(num - 1);
			this.m_attrib = (ParameterAttributes)num1;
		}

		public override bool Equals(object obj)
		{
			MetadataOnlyParameterInfo metadataOnlyParameterInfo = obj as MetadataOnlyParameterInfo;
			if (metadataOnlyParameterInfo == null)
			{
				return false;
			}
			if (!metadataOnlyParameterInfo.m_resolver.Equals(this.m_resolver))
			{
				return false;
			}
			return metadataOnlyParameterInfo.m_parameterToken.Equals(this.m_parameterToken);
		}

		public override IList<CustomAttributeData> GetCustomAttributesData()
		{
			return this.m_resolver.GetCustomAttributeData(this.MetadataToken);
		}

		public override int GetHashCode()
		{
			int mParameterToken = this.m_parameterToken;
			return this.m_resolver.GetHashCode() * 32767 + mParameterToken.GetHashCode();
		}

		public override Type[] GetOptionalCustomModifiers()
		{
			if (this.m_customModifiers == null)
			{
				return Type.EmptyTypes;
			}
			return this.m_customModifiers.OptionalCustomModifiers;
		}

		public override Type[] GetRequiredCustomModifiers()
		{
			if (this.m_customModifiers == null)
			{
				return Type.EmptyTypes;
			}
			return this.m_customModifiers.RequiredCustomModifiers;
		}

		public override string ToString()
		{
			CultureInfo invariantCulture = CultureInfo.InvariantCulture;
			object[] str = new object[] { MetadataOnlyCommonType.TypeSigToString(this.ParameterType), this.Name };
			return string.Format(invariantCulture, "{0} {1}", str);
		}
	}
}