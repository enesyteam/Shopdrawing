using System;
using System.Reflection;

namespace Microsoft.Expression.DesignModel.Metadata
{
	internal sealed class RoutedEventImplementation : RoutedEventImplementationBase
	{
		private EventImplementationBase clrImplementation;

		public override MemberAccessType Access
		{
			get
			{
				return this.clrImplementation.Access;
			}
		}

		public override Type DeclaringType
		{
			get
			{
				return this.clrImplementation.DeclaringType;
			}
		}

		public override System.Reflection.EventInfo EventInfo
		{
			get
			{
				return this.clrImplementation.EventInfo;
			}
		}

		public override Type HandlerType
		{
			get
			{
				return this.clrImplementation.HandlerType;
			}
		}

		public override Microsoft.Expression.DesignModel.Metadata.MemberType MemberType
		{
			get
			{
				return this.clrImplementation.MemberType;
			}
		}

		public override Type TargetType
		{
			get
			{
				AttachedEventImplementation attachedEventImplementation = this.clrImplementation as AttachedEventImplementation;
				if (attachedEventImplementation != null)
				{
					return attachedEventImplementation.TargetType;
				}
				return typeof(object);
			}
		}

		public RoutedEventImplementation(Microsoft.Expression.DesignModel.Metadata.RoutedEventDescription routedEvent, EventImplementationBase clrImplementation) : base(routedEvent)
		{
			if (clrImplementation == null)
			{
				throw new ArgumentNullException("clrImplementation");
			}
			this.clrImplementation = clrImplementation;
		}
	}
}