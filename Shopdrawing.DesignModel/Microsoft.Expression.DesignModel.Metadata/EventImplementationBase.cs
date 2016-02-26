using System;
using System.Reflection;

namespace Microsoft.Expression.DesignModel.Metadata
{
	internal abstract class EventImplementationBase
	{
		public abstract MemberAccessType Access
		{
			get;
		}

		public abstract Type DeclaringType
		{
			get;
		}

		public virtual System.Reflection.EventInfo EventInfo
		{
			get
			{
				return null;
			}
		}

		public abstract Type HandlerType
		{
			get;
		}

		public abstract bool IsAttached
		{
			get;
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

		public virtual object RoutedEvent
		{
			get
			{
				return null;
			}
		}

		public abstract Type TargetType
		{
			get;
		}

		protected EventImplementationBase()
		{
		}
	}
}