using System;
using System.Reflection;

namespace Microsoft.Expression.DesignModel.Metadata
{
	public sealed class Method : BaseMethod, ICachedMemberInfo
	{
		private System.Reflection.MethodInfo methodInfo;

		public override Microsoft.Expression.DesignModel.Metadata.MemberType MemberType
		{
			get
			{
				return Microsoft.Expression.DesignModel.Metadata.MemberType.Method;
			}
		}

		public override ITypeId MemberTypeId
		{
			get
			{
				return PlatformTypes.MethodInfo;
			}
		}

		protected override System.Reflection.MethodBase MethodBase
		{
			get
			{
				return this.methodInfo;
			}
		}

		public System.Reflection.MethodInfo MethodInfo
		{
			get
			{
				return this.methodInfo;
			}
		}

		private Method(ITypeResolver typeResolver, IType declaringType, System.Reflection.MethodInfo methodInfo, string nameWithParameters) : base(typeResolver, declaringType, methodInfo, methodInfo.Name, nameWithParameters)
		{
			this.Initialize(methodInfo);
		}

		public static IMethod GetMethod(ITypeResolver typeResolver, IType declaringType, System.Reflection.MethodInfo methodInfo)
		{
			IMethod method;
			if (methodInfo == null)
			{
				throw new ArgumentNullException("methodInfo");
			}
			IMutableMembers mutableMember = (IMutableMembers)declaringType;
			string nameWithParameters = PlatformTypeHelper.GetNameWithParameters(methodInfo);
			IMemberId member = mutableMember.GetMember(Microsoft.Expression.DesignModel.Metadata.MemberType.Method, nameWithParameters);
			if (member == null)
			{
				method = new Method(typeResolver, declaringType, methodInfo, nameWithParameters);
				mutableMember.AddMember(method);
			}
			else
			{
				method = Member.GetMemberAs<Method>(member);
			}
			return method;
		}

		private void Initialize(System.Reflection.MethodInfo methodInfo)
		{
			this.methodInfo = methodInfo;
		}

		bool Microsoft.Expression.DesignModel.Metadata.ICachedMemberInfo.Refresh()
		{
			System.Reflection.MethodInfo method = PlatformTypeHelper.GetMethod(base.DeclaringType.NearestResolvedType.RuntimeType, this.Name);
			if (method == null)
			{
				return false;
			}
			this.Initialize(method);
			return true;
		}
	}
}