using System;

namespace Microsoft.Expression.DesignModel.Metadata
{
	internal struct RoutedEventDescription
	{
		private string name;

		private object routedEvent;

		private Type handlerType;

		private Type ownerType;

		public Type HandlerType
		{
			get
			{
				return this.handlerType;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public Type OwnerType
		{
			get
			{
				return this.ownerType;
			}
		}

		public object RoutedEvent
		{
			get
			{
				return this.routedEvent;
			}
		}

		public RoutedEventDescription(string name, object routedEvent, Type handlerType, Type ownerType)
		{
			this.name = name;
			this.routedEvent = routedEvent;
			this.handlerType = handlerType;
			this.ownerType = ownerType;
		}
	}
}