using System;
using System.Globalization;
using System.Reflection;

namespace Microsoft.Expression.DesignModel.Metadata
{
	public abstract class Member : IMember, IMemberId
	{
		private IType declaringType;

		private string name;

		private int hashCode;

		private string fullName;

		public virtual MemberAccessType Access
		{
			get
			{
				return MemberAccessType.Public;
			}
		}

		public IType DeclaringType
		{
			get
			{
				return this.declaringType;
			}
		}

		public ITypeId DeclaringTypeId
		{
			get
			{
				return this.declaringType;
			}
		}

		public virtual string FullName
		{
			get
			{
				string str = this.fullName;
				if (str == null)
				{
					string str1 = this.ComputeFullName();
					string str2 = str1;
					this.fullName = str1;
					str = str2;
				}
				return str;
			}
		}

		public abstract bool IsResolvable
		{
			get;
		}

		public abstract Microsoft.Expression.DesignModel.Metadata.MemberType MemberType
		{
			get;
		}

		public abstract ITypeId MemberTypeId
		{
			get;
		}

		public virtual string Name
		{
			get
			{
				return this.name;
			}
		}

		protected virtual string NameWithParameters
		{
			get
			{
				return this.Name;
			}
		}

		public virtual string UniqueName
		{
			get
			{
				return this.Name;
			}
		}

		protected Member(IType declaringType, string name)
		{
			if (declaringType == null)
			{
				throw new ArgumentNullException("declaringType");
			}
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			this.declaringType = declaringType;
			this.name = name;
		}

		public IMember Clone(ITypeResolver typeResolver)
		{
			IType type = (IType)this.declaringType.Clone(typeResolver);
			if (type == null)
			{
				return null;
			}
			if (type == this.declaringType)
			{
				return this;
			}
			Microsoft.Expression.DesignModel.Metadata.MemberType memberType = this.MemberType;
			IMember member = (IMember)type.GetMember(memberType, this.Name, (MemberAccessTypes)this.Access);
			if (member == null)
			{
				member = new UnknownMember(type, memberType, this.Name);
			}
			return member;
		}

		private string ComputeFullName()
		{
			CultureInfo invariantCulture = CultureInfo.InvariantCulture;
			object[] fullName = new object[] { this.declaringType.FullName, this.NameWithParameters };
			return string.Format(invariantCulture, "{0}.{1}", fullName);
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			IMemberId memberId = obj as IMemberId;
			if (memberId == null)
			{
				return false;
			}
			PlatformNeutralPropertyId platformNeutralPropertyId = memberId as PlatformNeutralPropertyId;
			if (platformNeutralPropertyId != null)
			{
				if (!string.Equals(platformNeutralPropertyId.Name, this.Name, StringComparison.Ordinal))
				{
					return false;
				}
				return platformNeutralPropertyId.DeclaringTypeId.Equals(this.DeclaringTypeId);
			}
			if (!string.Equals(memberId.UniqueName, this.UniqueName, StringComparison.Ordinal) || !memberId.DeclaringTypeId.Equals(this.DeclaringTypeId))
			{
				return false;
			}
			return memberId.MemberType == this.MemberType;
		}

		public override int GetHashCode()
		{
			if (this.hashCode == 0)
			{
				this.hashCode = this.DeclaringTypeId.GetHashCode() ^ this.MemberType.GetHashCode() ^ this.UniqueName.GetHashCode();
			}
			return this.hashCode;
		}

		protected static T GetMemberAs<T>(IMemberId member)
		where T : Member
		{
			if (member == null)
			{
				throw new ArgumentNullException("member");
			}
			T t = (T)(member as T);
			if (t == null)
			{
				CultureInfo currentCulture = CultureInfo.CurrentCulture;
				string cannotAccessMemberUsingDifferentType = ExceptionStringTable.CannotAccessMemberUsingDifferentType;
				object[] name = new object[] { member.Name, typeof(T).Name, member.GetType().Name };
				throw new InvalidOperationException(string.Format(currentCulture, cannotAccessMemberUsingDifferentType, name));
			}
			return t;
		}

		public override string ToString()
		{
			CultureInfo invariantCulture = CultureInfo.InvariantCulture;
			object[] name = new object[] { this.declaringType.Name, this.NameWithParameters };
			return string.Format(invariantCulture, "{0}.{1}", name);
		}
	}
}