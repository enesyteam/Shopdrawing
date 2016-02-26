using System;
using System.Windows;

namespace Microsoft.Expression.DesignModel.Metadata
{
	internal abstract class DependencyPropertyImplementationBase : PropertyImplementationBase
	{
		public virtual ClrPropertyImplementationBase ClrImplementation
		{
			get
			{
				return null;
			}
		}

		public abstract object DependencyProperty
		{
			get;
		}

		public override MemberAccessType ReadAccess
		{
			get
			{
				ClrPropertyImplementationBase clrImplementation = this.ClrImplementation;
				if (clrImplementation == null)
				{
					return MemberAccessType.Public;
				}
				return clrImplementation.ReadAccess;
			}
		}

		public override MemberAccessType WriteAccess
		{
			get
			{
				ClrPropertyImplementationBase clrImplementation = this.ClrImplementation;
				if (clrImplementation == null)
				{
					return MemberAccessType.Public;
				}
				return clrImplementation.WriteAccess;
			}
		}

		protected DependencyPropertyImplementationBase()
		{
		}

		public abstract bool BindsTwoWayByDefault(Type targetType);

		public virtual object GetBaseValue(object target)
		{
			return this.GetValue(target);
		}

		public virtual object GetCurrentValue(object target)
		{
			return this.GetValue(target);
		}

		public override object[] GetCustomAttributes(Type attributeType, bool inherits)
		{
			if (this.ClrImplementation == null)
			{
				return base.GetCustomAttributes(attributeType, inherits);
			}
			return this.ClrImplementation.GetCustomAttributes(attributeType, inherits);
		}

		public abstract BaseValueSource GetValueSource(object target);

		public abstract bool Inherits(Type targetType);

		public abstract bool IsAnimated(object target);

		public abstract bool IsAnimationProhibited(Type targetType);

		public abstract bool IsDefaultValue(object instance);

		public abstract bool IsSet(object target);

		public abstract void SetBinding(object target, object value);

		public abstract void SetResourceReference(object target, object value);
	}
}