using System;
using System.Reflection;

namespace Microsoft.Expression.DesignModel.Metadata
{
	internal sealed class Event : Property, IEventImplementation, IEvent, IProperty, IMember, IPropertyId, IMemberId
	{
		private EventImplementationBase implementation;

		public override MemberAccessType Access
		{
			get
			{
				return this.implementation.Access;
			}
		}

		public IType EventHandlerType
		{
			get
			{
				return base.PropertyType;
			}
		}

		public System.Reflection.EventInfo EventInfo
		{
			get
			{
				return this.implementation.EventInfo;
			}
		}

		public bool IncludesClrEvent
		{
			get
			{
				return this.EventInfo != null;
			}
		}

		public bool IncludesRoutedEvent
		{
			get
			{
				return this.RoutedEvent != null;
			}
		}

		public override bool IsAttachable
		{
			get
			{
				return this.implementation.IsAttached;
			}
		}

		public override bool IsProxy
		{
			get
			{
				return this.implementation.IsProxy;
			}
		}

		public override bool IsResolvable
		{
			get
			{
				return this.implementation.IsResolved;
			}
		}

		public override Microsoft.Expression.DesignModel.Metadata.MemberType MemberType
		{
			get
			{
				return this.implementation.MemberType;
			}
		}

		public override ITypeId MemberTypeId
		{
			get
			{
				return this.implementation.MemberTypeId;
			}
		}

		EventImplementationBase Microsoft.Expression.DesignModel.Metadata.IEventImplementation.Implementation
		{
			get
			{
				return this.implementation;
			}
			set
			{
				this.implementation = value;
			}
		}

		public object RoutedEvent
		{
			get
			{
				return this.implementation.RoutedEvent;
			}
		}

		public override Type TargetType
		{
			get
			{
				return this.implementation.TargetType;
			}
		}

		internal Event(IType declaringType, IType handlerType, EventImplementationBase implementation) : base(declaringType, implementation.Name, handlerType, PropertySortValue.NoValue)
		{
			this.implementation = implementation;
		}

		public override object GetDefaultValue(Type targetType)
		{
			throw new NotSupportedException();
		}

		public override bool HasDefaultValue(Type targetType)
		{
			return false;
		}

		void Microsoft.Expression.DesignModel.Metadata.IEventImplementation.Invalidate()
		{
			this.implementation = Event.UndefinedEventImplementation.Instance;
		}

		private sealed class UndefinedEventImplementation : EventImplementationBase
		{
			public readonly static Event.UndefinedEventImplementation Instance;

			public override MemberAccessType Access
			{
				get
				{
					return MemberAccessType.Private;
				}
			}

			public override Type DeclaringType
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
					return typeof(EventHandler);
				}
			}

			public override bool IsAttached
			{
				get
				{
					return false;
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
					return Microsoft.Expression.DesignModel.Metadata.MemberType.LocalEvent;
				}
			}

			public override ITypeId MemberTypeId
			{
				get
				{
					return PlatformTypes.Delegate;
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

			static UndefinedEventImplementation()
			{
				Event.UndefinedEventImplementation.Instance = new Event.UndefinedEventImplementation();
			}

			private UndefinedEventImplementation()
			{
			}
		}
	}
}