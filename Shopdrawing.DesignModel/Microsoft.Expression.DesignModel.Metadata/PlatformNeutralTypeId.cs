using System;

namespace Microsoft.Expression.DesignModel.Metadata
{
	internal sealed class PlatformNeutralTypeId : FullNameTypeId
	{
		public PlatformNeutralTypeId(string fullName) : base(fullName)
		{
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			ITypeId typeId = obj as ITypeId;
			if (typeId != null && !(obj is ProjectContextType))
			{
				return base.FullName == typeId.FullName;
			}
			if (this == PlatformTypes.XData && obj is XDataType)
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool IsAssignableFrom(ITypeId type)
		{
			if (type == null || !type.IsResolvable)
			{
				return false;
			}
			IType type1 = (IType)type;
			return type1.PlatformMetadata.ResolveType(this).IsAssignableFrom(type);
		}
	}
}