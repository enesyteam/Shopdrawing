using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;

namespace Microsoft.Expression.DesignModel.Metadata
{
	public abstract class BaseMethod : Member, IMethod, IMember, IMemberId
	{
		private ITypeResolver typeResolver;

		private IList<IParameter> arguments;

		private string nameWithParameters;

		public override MemberAccessType Access
		{
			get
			{
				System.Reflection.MethodBase methodBase = this.MethodBase;
				if (methodBase == null)
				{
					return MemberAccessType.Public;
				}
				return PlatformTypeHelper.GetMemberAccess(methodBase);
			}
		}

		public override bool IsResolvable
		{
			get
			{
				return this.MethodBase != null;
			}
		}

		protected abstract System.Reflection.MethodBase MethodBase
		{
			get;
		}

		protected override string NameWithParameters
		{
			get
			{
				return this.nameWithParameters;
			}
		}

		public IList<IParameter> Parameters
		{
			get
			{
				return this.arguments;
			}
		}

		public override string UniqueName
		{
			get
			{
				return this.nameWithParameters;
			}
		}

		protected BaseMethod(ITypeResolver typeResolver, IType declaringType, System.Reflection.MethodBase method, string name, string nameWithParameters) : base(declaringType, name)
		{
			this.typeResolver = typeResolver;
			this.nameWithParameters = nameWithParameters;
			this.arguments = this.GetArguments(method);
		}

		private IList<IParameter> GetArguments(System.Reflection.MethodBase method)
		{
			ParameterInfo[] parameters = method.GetParameters();
			IParameter[] parameter = new IParameter[(int)parameters.Length];
			for (int i = 0; i < (int)parameters.Length; i++)
			{
				ParameterInfo parameterInfo = parameters[i];
				Type valueType = PlatformTypeHelper.GetValueType(parameterInfo);
				IType type = null;
				if (valueType != null)
				{
					type = this.typeResolver.GetType(valueType);
				}
				if (type == null)
				{
					type = this.typeResolver.ResolveType(PlatformTypes.Object);
				}
				parameter[i] = new BaseMethod.Parameter(type, parameterInfo.Name);
			}
			return new ReadOnlyCollection<IParameter>(parameter);
		}

		private sealed class Parameter : IParameter
		{
			private IType parameterType;

			private string name;

			public string Name
			{
				get
				{
					return this.name;
				}
			}

			public IType ParameterType
			{
				get
				{
					return this.parameterType;
				}
			}

			public Parameter(IType parameterType, string name)
			{
				this.parameterType = parameterType;
				this.name = name;
			}

			public override string ToString()
			{
				CultureInfo invariantCulture = CultureInfo.InvariantCulture;
				object[] name = new object[] { this.parameterType.Name, this.name };
				return string.Format(invariantCulture, "{0} {1}", name);
			}
		}
	}
}