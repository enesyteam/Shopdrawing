using System;
using System.Reflection;

namespace Microsoft.Expression.DesignModel.Metadata
{
	internal sealed class MissingClrEventImplementation : RoutedEventImplementationBase
	{
		public override MemberAccessType Access
		{
			get
			{
				return MemberAccessType.Public;
			}
		}

		public override Type DeclaringType
		{
			get
			{
				return base.RoutedEventDescription.OwnerType;
			}
		}

		public override System.Reflection.EventInfo EventInfo
		{
			get
			{
				return null;
			}
		}

		public override Type HandlerType
		{
			get
			{
				return base.RoutedEventDescription.HandlerType;
			}
		}

		public override Microsoft.Expression.DesignModel.Metadata.MemberType MemberType
		{
			get
			{
				return Microsoft.Expression.DesignModel.Metadata.MemberType.RoutedEvent;
			}
		}

		public override Type TargetType
		{
			get
			{
				return typeof(object);
			}
		}

		public MissingClrEventImplementation(Microsoft.Expression.DesignModel.Metadata.RoutedEventDescription routedEvent) : base(routedEvent)
		{
		}
	}
}