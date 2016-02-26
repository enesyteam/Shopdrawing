using System;
using System.Reflection;
using System.Reflection.Adds;

namespace Microsoft.MetadataReader
{
	internal class OpenGenericContext : GenericContext
	{
		private readonly MetadataOnlyModule m_resolver;

		private readonly Token m_ownerMethod;

		public OpenGenericContext(Type[] typeArgs, Type[] methodArgs) : base(typeArgs, methodArgs)
		{
		}

		public OpenGenericContext(MetadataOnlyModule resolver, Type ownerType, Token ownerMethod) : base(null, null)
		{
			this.m_resolver = resolver;
			this.m_ownerMethod = ownerMethod;
			int length = (int)ownerType.GetGenericArguments().Length;
			Type[] metadataOnlyTypeVariableRef = new Type[length];
			Token token = new Token(ownerType.MetadataToken);
			for (int i = 0; i < length; i++)
			{
				metadataOnlyTypeVariableRef[i] = new MetadataOnlyTypeVariableRef(resolver, token, i);
			}
			base.TypeArgs = metadataOnlyTypeVariableRef;
		}

		public override GenericContext VerifyAndUpdateMethodArguments(int expectedNumberOfMethodArgs)
		{
			if (expectedNumberOfMethodArgs == (int)base.MethodArgs.Length)
			{
				return this;
			}
			Type[] metadataOnlyTypeVariableRef = new Type[expectedNumberOfMethodArgs];
			for (int i = 0; i < expectedNumberOfMethodArgs; i++)
			{
				metadataOnlyTypeVariableRef[i] = new MetadataOnlyTypeVariableRef(this.m_resolver, this.m_ownerMethod, i);
			}
			return new OpenGenericContext(base.TypeArgs, metadataOnlyTypeVariableRef);
		}
	}
}