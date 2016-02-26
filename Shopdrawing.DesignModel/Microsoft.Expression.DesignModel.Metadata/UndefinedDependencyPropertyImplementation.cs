using System;
using System.ComponentModel;
using System.Windows;

namespace Microsoft.Expression.DesignModel.Metadata
{
	internal sealed class UndefinedDependencyPropertyImplementation : DependencyPropertyImplementationBase
	{
		public readonly static UndefinedDependencyPropertyImplementation Instance;

		public override AttributeCollection Attributes
		{
			get
			{
				throw new InvalidOperationException();
			}
		}

		public override Type DeclaringType
		{
			get
			{
				return null;
			}
		}

		public override object DependencyProperty
		{
			get
			{
				throw new InvalidOperationException();
			}
		}

		public override bool IsResolved
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
				return Microsoft.Expression.DesignModel.Metadata.MemberType.DependencyProperty;
			}
		}

		public override ITypeId MemberTypeId
		{
			get
			{
				return PlatformTypes.DependencyProperty;
			}
		}

		public override string Name
		{
			get
			{
				return "[Undefined]";
			}
		}

		public override Type TargetType
		{
			get
			{
				return typeof(object);
			}
		}

		public override Type ValueType
		{
			get
			{
				return null;
			}
		}

		static UndefinedDependencyPropertyImplementation()
		{
			UndefinedDependencyPropertyImplementation.Instance = new UndefinedDependencyPropertyImplementation();
		}

		private UndefinedDependencyPropertyImplementation()
		{
		}

		public override bool BindsTwoWayByDefault(Type targetType)
		{
			throw new InvalidOperationException();
		}

		public override object GetDefaultValue(Type targetType)
		{
			throw new InvalidOperationException();
		}

		public override object GetValue(object target)
		{
			throw new InvalidOperationException();
		}

		public override BaseValueSource GetValueSource(object target)
		{
			return BaseValueSource.Unknown;
		}

		public override bool HasDefaultValue(Type targetType)
		{
			return false;
		}

		public override bool Inherits(Type targetType)
		{
			throw new InvalidOperationException();
		}

		public override bool IsAnimated(object target)
		{
			return false;
		}

		public override bool IsAnimationProhibited(Type targetType)
		{
			throw new InvalidOperationException();
		}

		public override bool IsDefaultValue(object instance)
		{
			throw new NotImplementedException();
		}

		public override bool IsSet(object target)
		{
			return false;
		}

		public override void SetBinding(object target, object value)
		{
			throw new InvalidOperationException();
		}

		public override void SetResourceReference(object target, object value)
		{
			throw new InvalidOperationException();
		}
	}
}