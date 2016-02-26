using System;

namespace Microsoft.Expression.DesignModel.Metadata
{
	internal abstract class RoutedEventImplementationBase : EventImplementationBase
	{
		private Microsoft.Expression.DesignModel.Metadata.RoutedEventDescription routedEventDescription;

		public override bool IsAttached
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

		public override ITypeId MemberTypeId
		{
			get
			{
				return PlatformTypes.RoutedEvent;
			}
		}

		public override string Name
		{
			get
			{
				return this.routedEventDescription.Name;
			}
		}

		public override object RoutedEvent
		{
			get
			{
				return this.routedEventDescription.RoutedEvent;
			}
		}

		protected Microsoft.Expression.DesignModel.Metadata.RoutedEventDescription RoutedEventDescription
		{
			get
			{
				return this.routedEventDescription;
			}
		}

		protected RoutedEventImplementationBase(Microsoft.Expression.DesignModel.Metadata.RoutedEventDescription routedEventDescription)
		{
			this.routedEventDescription = routedEventDescription;
		}
	}
}