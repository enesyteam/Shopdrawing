using System;
using System.Reflection;
using System.Reflection.Adds;

namespace Microsoft.MetadataReader
{
	internal class ConstructorInfoRef : ConstructorInfoProxy
	{
		private readonly Type m_declaringType;

		private readonly Token m_token;

		private readonly MetadataOnlyModule m_scope;

		public override Type DeclaringType
		{
			get
			{
				return this.m_declaringType;
			}
		}

		public ConstructorInfoRef(Type declaringType, MetadataOnlyModule scope, Token token)
		{
			this.m_declaringType = declaringType;
			this.m_token = token;
			this.m_scope = scope;
		}

		protected override ConstructorInfo GetResolvedWorker()
		{
			return (ConstructorInfo)this.m_scope.ResolveMethod(this.m_token);
		}
	}
}