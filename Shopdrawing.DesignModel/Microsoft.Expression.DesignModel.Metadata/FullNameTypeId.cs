using System;

namespace Microsoft.Expression.DesignModel.Metadata
{
	public abstract class FullNameTypeId : ITypeId, IMemberId, IMutableMembers, IResolvableRuntimeType, IReflectionType
	{
		public const uint MaxUniqueIds = 700;

		private static uint uniqueIdCounter;

		private uint uniqueId;

		private string fullName;

		public ITypeId DeclaringTypeId
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public string FullName
		{
			get
			{
				return this.fullName;
			}
		}

		public bool IsBuilt
		{
			get
			{
				return true;
			}
		}

		public bool IsResolvable
		{
			get
			{
				return true;
			}
		}

		public Microsoft.Expression.DesignModel.Metadata.MemberType MemberType
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		Type Microsoft.Expression.DesignModel.Metadata.IReflectionType.ReflectionType
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public string Name
		{
			get
			{
				string[] strArrays = this.fullName.Split(new char[] { '.' });
				return strArrays[(int)strArrays.Length - 1];
			}
		}

		public string Namespace
		{
			get
			{
				int num = this.fullName.LastIndexOf('.');
				if (num == -1)
				{
					return string.Empty;
				}
				return this.fullName.Substring(0, num);
			}
		}

		public uint UniqueId
		{
			get
			{
				return this.uniqueId;
			}
		}

		public string UniqueName
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public IXmlNamespace XmlNamespace
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		static FullNameTypeId()
		{
		}

		protected FullNameTypeId(string fullName)
		{
			this.fullName = fullName;
			uint num = FullNameTypeId.uniqueIdCounter;
			FullNameTypeId.uniqueIdCounter = num + 1;
			this.uniqueId = num;
		}

		public override int GetHashCode()
		{
			return (int)this.uniqueId;
		}

		public IMemberId GetMember(Microsoft.Expression.DesignModel.Metadata.MemberType memberTypes, string memberName, MemberAccessTypes access)
		{
			if ((memberTypes & Microsoft.Expression.DesignModel.Metadata.MemberType.Property) == Microsoft.Expression.DesignModel.Metadata.MemberType.None)
			{
				return null;
			}
			return new PlatformNeutralPropertyId(this, memberTypes, memberName, access);
		}

		public abstract bool IsAssignableFrom(ITypeId type);

		void Microsoft.Expression.DesignModel.Metadata.IMutableMembers.AddMember(IMember memberId)
		{
			throw new NotSupportedException();
		}

		IMember Microsoft.Expression.DesignModel.Metadata.IMutableMembers.GetMember(Microsoft.Expression.DesignModel.Metadata.MemberType memberTypes, string uniqueName)
		{
			throw new NotSupportedException();
		}

		Type Microsoft.Expression.DesignModel.Metadata.IResolvableRuntimeType.GetRuntimeType()
		{
			throw new NotSupportedException();
		}

		public override string ToString()
		{
			return this.FullName;
		}
	}
}