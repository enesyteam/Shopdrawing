using System;
using System.Reflection;

namespace Microsoft.Expression.DesignModel.Metadata
{
	internal sealed class AttachedEventImplementation : EventImplementationBase
	{
		private string name;

		private MethodInfo addMethod;

		private Type targetType;

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
				return this.addMethod.DeclaringType;
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
				return Microsoft.Expression.DesignModel.Metadata.MemberType.AttachedEvent;
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
				return this.targetType;
			}
		}

		public AttachedEventImplementation(string name, MethodInfo addMethod)
		{
			this.name = name;
			this.addMethod = addMethod;
			ParameterInfo[] parameters = PlatformTypeHelper.GetParameters(this.addMethod);
			if (parameters != null && (int)parameters.Length == 2)
			{
				this.targetType = parameters[0].ParameterType;
				this.handlerType = parameters[1].ParameterType;
			}
			this.access = PlatformTypeHelper.GetMemberAccess(this.addMethod);
		}
	}
}