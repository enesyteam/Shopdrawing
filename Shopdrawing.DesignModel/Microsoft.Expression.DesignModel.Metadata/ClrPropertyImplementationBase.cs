using System;
using System.Reflection;

namespace Microsoft.Expression.DesignModel.Metadata
{
	public abstract class ClrPropertyImplementationBase : PropertyImplementationBase
	{
		private IPlatformMetadata platformMetadata;

		public abstract object DefaultValue
		{
			get;
		}

		public virtual MethodInfo GetMethod
		{
			get
			{
				return null;
			}
		}

		public abstract bool HasDefault
		{
			get;
		}

		public override Microsoft.Expression.DesignModel.Metadata.MemberType MemberType
		{
			get
			{
				return Microsoft.Expression.DesignModel.Metadata.MemberType.LocalProperty;
			}
		}

		public override ITypeId MemberTypeId
		{
			get
			{
				return PlatformTypes.PropertyInfo;
			}
		}

		public IPlatformMetadata PlatformMetadata
		{
			get
			{
				return this.platformMetadata;
			}
		}

		public virtual System.Reflection.PropertyInfo PropertyInfo
		{
			get
			{
				return null;
			}
		}

		public virtual MethodInfo SetMethod
		{
			get
			{
				return null;
			}
		}

		protected ClrPropertyImplementationBase(IPlatformMetadata platformMetadata)
		{
			this.platformMetadata = platformMetadata;
		}

		public override void ClearValue(object target)
		{
			this.SetValue(target, this.DefaultValue);
		}

		public override object GetDefaultValue(Type targetType)
		{
			return this.DefaultValue;
		}

		public override bool HasDefaultValue(Type targetType)
		{
			return this.HasDefault;
		}
	}
}