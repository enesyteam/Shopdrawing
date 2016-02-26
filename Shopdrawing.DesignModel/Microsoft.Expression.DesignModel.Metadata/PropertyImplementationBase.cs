using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace Microsoft.Expression.DesignModel.Metadata
{
	public abstract class PropertyImplementationBase
	{
		public MemberAccessType Access
		{
			get
			{
				MemberAccessType readAccess = this.ReadAccess;
				MemberAccessType writeAccess = this.WriteAccess;
				if (readAccess > writeAccess)
				{
					return readAccess;
				}
				return writeAccess;
			}
		}

		public virtual AttributeCollection Attributes
		{
			get
			{
				return null;
			}
		}

		public virtual string ConstructorArgument
		{
			get
			{
				return null;
			}
		}

		public abstract Type DeclaringType
		{
			get;
		}

		public virtual bool IsAttachable
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsProxy
		{
			get
			{
				return false;
			}
		}

		public abstract bool IsResolved
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

		public abstract string Name
		{
			get;
		}

		public abstract MemberAccessType ReadAccess
		{
			get;
		}

		public virtual bool ShouldSerialize
		{
			get
			{
				return true;
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

		public abstract Type ValueType
		{
			get;
		}

		public abstract MemberAccessType WriteAccess
		{
			get;
		}

		protected PropertyImplementationBase()
		{
		}

		public virtual void ClearValue(object target)
		{
		}

		public virtual object[] GetCustomAttributes(Type attributeType, bool inherits)
		{
			return null;
		}

		public abstract object GetDefaultValue(Type targetType);

		public abstract object GetValue(object target);

		public abstract bool HasDefaultValue(Type targetType);

		public virtual void SetValue(object target, object valueToSet)
		{
		}

		public override string ToString()
		{
			string name = this.Name;
			Type declaringType = this.DeclaringType;
			if (declaringType != null)
			{
				CultureInfo invariantCulture = CultureInfo.InvariantCulture;
				object[] objArray = new object[] { declaringType.Name, name };
				name = string.Format(invariantCulture, "{0}.{1}", objArray);
			}
			return name;
		}
	}
}