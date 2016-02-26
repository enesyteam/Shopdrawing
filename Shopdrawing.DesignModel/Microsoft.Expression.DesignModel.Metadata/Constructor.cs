using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Expression.DesignModel.Metadata
{
	internal sealed class Constructor : BaseMethod, ICachedMemberInfo, IConstructor, IMethod, IMember, IMemberId
	{
		private ConstructorInfo constructorInfo;

		public override Microsoft.Expression.DesignModel.Metadata.MemberType MemberType
		{
			get
			{
				return Microsoft.Expression.DesignModel.Metadata.MemberType.Constructor;
			}
		}

		public override ITypeId MemberTypeId
		{
			get
			{
				return PlatformTypes.ConstructorInfo;
			}
		}

		protected override System.Reflection.MethodBase MethodBase
		{
			get
			{
				return this.constructorInfo;
			}
		}

		public Constructor(ITypeResolver typeResolver, IType declaringType, ConstructorInfo constructorInfo, string nameWithParameters) : base(typeResolver, declaringType, constructorInfo, ".ctor", nameWithParameters)
		{
			this.Initialize(constructorInfo);
		}

		private static ConstructorInfo GetConstructor(Type type, IList<IParameter> arguments)
		{
			ConstructorInfo[] constructors = PlatformTypeHelper.GetConstructors(type);
			if (constructors != null && (int)constructors.Length > 0)
			{
				ConstructorInfo[] constructorInfoArray = constructors;
				for (int i = 0; i < (int)constructorInfoArray.Length; i++)
				{
					ConstructorInfo constructorInfo = constructorInfoArray[i];
					if ((int)constructorInfo.GetParameters().Length == arguments.Count)
					{
						return constructorInfo;
					}
				}
			}
			return null;
		}

		private void Initialize(ConstructorInfo constructorInfo)
		{
			this.constructorInfo = constructorInfo;
		}

		public object Invoke(object[] arguments)
		{
			return this.constructorInfo.Invoke(arguments);
		}

		bool Microsoft.Expression.DesignModel.Metadata.ICachedMemberInfo.Refresh()
		{
			ConstructorInfo constructor = null;
			Type runtimeType = base.DeclaringType.RuntimeType;
			if (runtimeType != null)
			{
				constructor = Constructor.GetConstructor(runtimeType, base.Parameters);
			}
			if (constructor == null)
			{
				return false;
			}
			this.Initialize(constructor);
			return true;
		}

		IMember Microsoft.Expression.DesignModel.Metadata.IMember.Clone(ITypeResolver typeResolver)
		{
			IType type = base.DeclaringType.Clone(typeResolver) as IType;
			if (base.DeclaringType == type)
			{
				return this;
			}
			if (type == null)
			{
				return null;
			}
			IList<IConstructor> constructors = type.GetConstructors();
			for (int i = 0; i < constructors.Count; i++)
			{
				Constructor item = constructors[i] as Constructor;
				if (item != null && item.constructorInfo == this.constructorInfo)
				{
					return item;
				}
			}
			return null;
		}
	}
}