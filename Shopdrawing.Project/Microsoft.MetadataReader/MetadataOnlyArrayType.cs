using System;
using System.Reflection;
using System.Text;

namespace Microsoft.MetadataReader
{
	internal class MetadataOnlyArrayType : MetadataOnlyCommonArrayType
	{
		private readonly int m_rank;

		public override string FullName
		{
			get
			{
				string fullName = this.GetElementType().FullName;
				if (fullName == null || this.GetElementType().IsGenericTypeDefinition)
				{
					return null;
				}
				return string.Concat(fullName, "[", MetadataOnlyArrayType.GetDimensionString(this.m_rank), "]");
			}
		}

		public override string Name
		{
			get
			{
				return string.Concat(this.GetElementType().Name, "[", MetadataOnlyArrayType.GetDimensionString(this.m_rank), "]");
			}
		}

		public MetadataOnlyArrayType(MetadataOnlyCommonType elementType, int rank) : base(elementType)
		{
			this.m_rank = rank;
		}

		public override bool Equals(Type t)
		{
			if (t == null)
			{
				return false;
			}
			if (!(t is MetadataOnlyArrayType) || t.GetArrayRank() != this.GetArrayRank())
			{
				return false;
			}
			return this.GetElementType().Equals(t.GetElementType());
		}

		public override int GetArrayRank()
		{
			return this.m_rank;
		}

		private static string GetDimensionString(int rank)
		{
			if (rank == 1)
			{
				return "*";
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 1; i < rank; i++)
			{
				stringBuilder.Append(',');
			}
			return stringBuilder.ToString();
		}

		public override bool IsAssignableFrom(Type c)
		{
			if (c == null)
			{
				return false;
			}
			if (!c.IsArray)
			{
				return MetadataOnlyTypeDef.IsAssignableFromHelper(this, c);
			}
			if (c.GetArrayRank() != this.m_rank)
			{
				return false;
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
			return string.Concat(this.GetElementType().ToString(), "[", MetadataOnlyArrayType.GetDimensionString(this.m_rank), "]");
		}
	}
}