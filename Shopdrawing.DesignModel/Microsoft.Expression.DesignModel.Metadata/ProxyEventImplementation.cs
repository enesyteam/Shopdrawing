using System;

namespace Microsoft.Expression.DesignModel.Metadata
{
	internal sealed class ProxyEventImplementation : EventImplementationBase
	{
		private string name;

		private Type declaringType;

		private Type handlerType;

		private MemberAccessType access;

		public override MemberAccessType Access
		{
			get
			{
				return this.access;
			}
		}

		public override Type DeclaringType
		{
			get
			{
				return this.declaringType;
			}
		}

		public override Type HandlerType
		{
			get
			{
				return this.handlerType;
			}
		}

		public override bool IsAttached
		{
			get
			{
				return false;
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
				return this.name;
			}
		}

		public override Type TargetType
		{
			get
			{
				return this.declaringType;
			}
		}

		public ProxyEventImplementation(string name, Type declaringType, Type handlerType, MemberAccessType access)
		{
			this.name = name;
			this.declaringType = declaringType;
			this.handlerType = handlerType;
			this.access = access;
		}
	}
}