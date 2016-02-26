using System;
using System.Reflection;

namespace Microsoft.Expression.DesignModel.Metadata
{
	internal sealed class LocalEventImplementation : EventImplementationBase
	{
		private System.Reflection.EventInfo eventInfo;

		private MethodInfo addMethod;

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
				return this.eventInfo.DeclaringType;
			}
		}

		public override System.Reflection.EventInfo EventInfo
		{
			get
			{
				return this.eventInfo;
			}
		}

		public override Type HandlerType
		{
			get
			{
				return PlatformTypes.GetHandlerType(this.eventInfo);
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
				return this.eventInfo.Name;
			}
		}

		public override Type TargetType
		{
			get
			{
				return this.eventInfo.DeclaringType;
			}
		}

		public LocalEventImplementation(System.Reflection.EventInfo eventInfo)
		{
			this.eventInfo = eventInfo;
			this.addMethod = this.eventInfo.GetAddMethod(true);
			this.access = (this.addMethod != null ? PlatformTypeHelper.GetMemberAccess(this.addMethod) : MemberAccessType.Private);
		}
	}
}