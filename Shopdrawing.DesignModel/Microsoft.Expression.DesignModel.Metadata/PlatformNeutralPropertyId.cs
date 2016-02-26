using System;
using System.Globalization;

namespace Microsoft.Expression.DesignModel.Metadata
{
	internal sealed class PlatformNeutralPropertyId : IPropertyId, IMemberId
	{
		private FullNameTypeId declaringTypeId;

		private Microsoft.Expression.DesignModel.Metadata.MemberType memberTypes;

		private string name;

		private Microsoft.Expression.DesignModel.Metadata.MemberAccessTypes memberAccessTypes;

		private int hashCode;

		public ITypeId DeclaringTypeId
		{
			get
			{
				return this.declaringTypeId;
			}
		}

		public string FullName
		{
			get
			{
				return string.Concat(this.declaringTypeId.FullName, ".", this.name);
			}
		}

		public bool IsResolvable
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public Microsoft.Expression.DesignModel.Metadata.MemberAccessTypes MemberAccessTypes
		{
			get
			{
				return this.memberAccessTypes;
			}
		}

		public Microsoft.Expression.DesignModel.Metadata.MemberType MemberType
		{
			get
			{
				return this.memberTypes;
			}
		}

		public ITypeId MemberTypeId
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public int SortValue
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public Type TargetType
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public string UniqueName
		{
			get
			{
				return this.Name;
			}
		}

		public PlatformNeutralPropertyId(FullNameTypeId declaringTypeId, Microsoft.Expression.DesignModel.Metadata.MemberType memberTypes, string name, Microsoft.Expression.DesignModel.Metadata.MemberAccessTypes memberAccessTypes)
		{
			this.declaringTypeId = declaringTypeId;
			this.memberTypes = memberTypes;
			this.name = name;
			this.memberAccessTypes = memberAccessTypes;
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			IMemberId memberId = obj as IMemberId;
			if (memberId == null || !(memberId.UniqueName == this.UniqueName))
			{
				return false;
			}
			return memberId.DeclaringTypeId.Equals(this.DeclaringTypeId);
		}

		public override int GetHashCode()
		{
			if (this.hashCode == 0)
			{
				this.hashCode = this.DeclaringTypeId.GetHashCode() ^ this.MemberType.GetHashCode() ^ this.UniqueName.GetHashCode();
			}
			return this.hashCode;
		}

		public override string ToString()
		{
			CultureInfo invariantCulture = CultureInfo.InvariantCulture;
			object[] name = new object[] { this.declaringTypeId.Name, this.Name };
			return string.Format(invariantCulture, "{0}.{1}", name);
		}
	}
}