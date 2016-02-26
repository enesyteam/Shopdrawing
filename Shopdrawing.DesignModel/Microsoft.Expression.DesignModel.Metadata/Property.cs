using System;
using System.ComponentModel;

namespace Microsoft.Expression.DesignModel.Metadata
{
	public abstract class Property : Member, IProperty, IMember, IPropertyId, IMemberId
	{
		private IType propertyType;

		private int sortValue = PropertySortValue.NoValue;

		public virtual AttributeCollection Attributes
		{
			get
			{
				return AttributeCollection.Empty;
			}
		}

		public abstract bool IsAttachable
		{
			get;
		}

		public virtual bool IsProxy
		{
			get
			{
				return false;
			}
		}

		public IType PropertyType
		{
			get
			{
				return this.propertyType;
			}
		}

		public virtual MemberAccessType ReadAccess
		{
			get
			{
				return MemberAccessType.Public;
			}
		}

		public virtual bool ShouldSerialize
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

		public abstract Type TargetType
		{
			get;
		}

		public virtual System.ComponentModel.TypeConverter TypeConverter
		{
			get
			{
				return null;
			}
		}

		public virtual MemberAccessType WriteAccess
		{
			get
			{
				return MemberAccessType.Public;
			}
		}

		protected Property(IType declaringType, string name, IType propertyType, int sortValue) : base(declaringType, name)
		{
			this.propertyType = propertyType;
			this.sortValue = (sortValue == PropertySortValue.NoValue ? PropertySortValue.RegisterProperty(this) : sortValue);
		}

		public abstract object GetDefaultValue(Type targetType);

		public abstract bool HasDefaultValue(Type targetType);
	}
}