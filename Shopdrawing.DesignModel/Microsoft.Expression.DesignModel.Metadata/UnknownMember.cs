using System;
using System.ComponentModel;

namespace Microsoft.Expression.DesignModel.Metadata
{
	internal sealed class UnknownMember : Member, IProperty, IMember, IPropertyId, IMemberId, ICachedMemberInfo
	{
		private static System.ComponentModel.TypeConverter ObjectTypeConverter;

		private Microsoft.Expression.DesignModel.Metadata.MemberType memberType;

		private int sortValue = PropertySortValue.NoValue;

		public AttributeCollection Attributes
		{
			get
			{
				return AttributeCollection.Empty;
			}
		}

		public bool IsAttachable
		{
			get
			{
				return false;
			}
		}

		public bool IsProxy
		{
			get
			{
				return true;
			}
		}

		public override bool IsResolvable
		{
			get
			{
				return false;
			}
		}

		public override Microsoft.Expression.DesignModel.Metadata.MemberType MemberType
		{
			get
			{
				return this.memberType;
			}
		}

		public override ITypeId MemberTypeId
		{
			get
			{
				return base.DeclaringType.PlatformMetadata.ResolveType(PlatformTypes.MemberInfo);
			}
		}

		public IType PropertyType
		{
			get
			{
				return base.DeclaringType.PlatformMetadata.ResolveType(PlatformTypes.Object);
			}
		}

		public MemberAccessType ReadAccess
		{
			get
			{
				return MemberAccessType.Public;
			}
		}

		public bool ShouldSerialize
		{
			get
			{
				return true;
			}
		}

		public int SortValue
		{
			get
			{
				return this.sortValue;
			}
		}

		public Type TargetType
		{
			get
			{
				return null;
			}
		}

		public System.ComponentModel.TypeConverter TypeConverter
		{
			get
			{
				return UnknownMember.ObjectTypeConverter;
			}
		}

		public MemberAccessType WriteAccess
		{
			get
			{
				return MemberAccessType.Public;
			}
		}

		static UnknownMember()
		{
			UnknownMember.ObjectTypeConverter = MetadataStore.GetTypeConverter(typeof(object));
		}

		public UnknownMember(IType declaringType, Microsoft.Expression.DesignModel.Metadata.MemberType memberType, string propertyName) : base(declaringType, propertyName)
		{
			this.memberType = memberType;
			this.sortValue = PropertySortValue.RegisterProperty(this);
		}

		public object GetDefaultValue(Type targetType)
		{
			throw new NotSupportedException();
		}

		public bool HasDefaultValue(Type targetType)
		{
			return false;
		}

		public bool Refresh()
		{
			return false;
		}
	}
}