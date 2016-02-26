using System;
using System.Reflection;

namespace Microsoft.MetadataReader
{
	internal class MetadataOnlyVectorType : MetadataOnlyCommonArrayType
	{
		public override string FullName
		{
			get
			{
				string fullName = this.GetElementType().FullName;
				if (fullName == null || this.GetElementType().IsGenericTypeDefinition)
				{
					return null;
				}
				return string.Concat(fullName, "[]");
			}
		}

		public override string Name
		{
			get
			{
				return string.Concat(this.GetElementType().Name, "[]");
			}
		}

		public MetadataOnlyVectorType(MetadataOnlyCommonType elementType) : base(elementType)
		{
		}

		public override bool Equals(Type t)
		{
			if (t == null)
			{
				return false;
			}
			if (!(t is MetadataOnlyVectorType) || t.GetArrayRank() != 1)
			{
				return false;
			}
			return this.GetElementType().Equals(t.GetElementType());
		}

		public override int GetArrayRank()
		{
			return 1;
		}

		protected override bool IsArrayImpl()
		{
			return true;
		}

		public override bool IsAssignableFrom(Type c)
		{
			if (c == null)
			{
				return false;
			}
			if (!c.IsArray || c.GetArrayRank() != 1 || !(c is MetadataOnlyVectorType))
			{
				return MetadataOnlyTypeDef.IsAssignableFromHelper(this, c);
			}
			Type elementType = c.GetElementType();
			if (elementType.IsValueType)
			{
				return this.GetElementType().Equals(elementType);
			}
			return this.GetElementType().IsAssignableFrom(elementType);
		}

		public override string ToString()
		{
			return string.Concat(this.GetElementType().ToString(), "[]");
		}
	}
}