using System;

namespace Microsoft.Expression.DesignModel.Metadata
{
	internal sealed class ProxyClrPropertyImplementation : ClrPropertyImplementationBase
	{
		private Type declaringType;

		private string name;

		private Type targetType;

		private Type valueType;

		private object defaultValue;

		private string constructorArgument;

		public override string ConstructorArgument
		{
			get
			{
				return this.constructorArgument;
			}
		}

		public override Type DeclaringType
		{
			get
			{
				return this.declaringType;
			}
		}

		public override object DefaultValue
		{
			get
			{
				return this.defaultValue;
			}
		}

		public override bool HasDefault
		{
			get
			{
				return true;
			}
		}

		public override bool IsProxy
		{
			get
			{
				return true;
			}
		}

		public override bool IsResolved
		{
			get
			{
				return true;
			}
		}

		public override string Name
		{
			get
			{
				return this.name;
			}
		}

		public override MemberAccessType ReadAccess
		{
			get
			{
				return MemberAccessType.Public;
			}
		}

		public override Type TargetType
		{
			get
			{
				return this.targetType;
			}
		}

		public override Type ValueType
		{
			get
			{
				return this.valueType;
			}
		}

		public override MemberAccessType WriteAccess
		{
			get
			{
				return MemberAccessType.Public;
			}
		}

		public ProxyClrPropertyImplementation(PlatformTypes platformTypes, Type declaringType, string name, Type targetType, Type valueType, object defaultValue, string constructorArgument) : base(platformTypes)
		{
			this.declaringType = declaringType;
			this.name = name;
			this.targetType = targetType;
			this.valueType = valueType;
			this.defaultValue = defaultValue;
			this.constructorArgument = constructorArgument;
		}

		public override object GetValue(object target)
		{
			return this.defaultValue;
		}

		public override void SetValue(object target, object valueToSet)
		{
		}
	}
}