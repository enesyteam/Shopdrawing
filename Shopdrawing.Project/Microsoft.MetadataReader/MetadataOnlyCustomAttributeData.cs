using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Adds;

namespace Microsoft.MetadataReader
{
	internal class MetadataOnlyCustomAttributeData : CustomAttributeData
	{
		private readonly ConstructorInfo m_ctor;

		private readonly MetadataOnlyModule m_module;

		private readonly Token m_token;

		private IList<CustomAttributeTypedArgument> m_typedArguments;

		private IList<CustomAttributeNamedArgument> m_namedArguments;

		public override ConstructorInfo Constructor
		{
			get
			{
				return this.m_ctor;
			}
		}

		public override IList<CustomAttributeTypedArgument> ConstructorArguments
		{
			get
			{
				if (this.m_typedArguments == null)
				{
					this.InitArgumentData();
				}
				return this.m_typedArguments;
			}
		}

		public override IList<CustomAttributeNamedArgument> NamedArguments
		{
			get
			{
				if (this.m_namedArguments == null)
				{
					this.InitArgumentData();
				}
				return this.m_namedArguments;
			}
		}

		public MetadataOnlyCustomAttributeData(MetadataOnlyModule module, Token token, ConstructorInfo ctor)
		{
			this.m_ctor = ctor;
			this.m_token = token;
			this.m_module = module;
		}

		public MetadataOnlyCustomAttributeData(ConstructorInfo ctor, IList<CustomAttributeTypedArgument> typedArguments, IList<CustomAttributeNamedArgument> namedArguments)
		{
			this.m_ctor = ctor;
			this.m_typedArguments = typedArguments;
			this.m_namedArguments = namedArguments;
		}

		private void InitArgumentData()
		{
			IList<CustomAttributeTypedArgument> customAttributeTypedArguments;
			IList<CustomAttributeNamedArgument> customAttributeNamedArguments;
			this.m_module.LazyAttributeParse(this.m_token, this.m_ctor, out customAttributeTypedArguments, out customAttributeNamedArguments);
			this.m_typedArguments = customAttributeTypedArguments;
			this.m_namedArguments = customAttributeNamedArguments;
		}
	}
}