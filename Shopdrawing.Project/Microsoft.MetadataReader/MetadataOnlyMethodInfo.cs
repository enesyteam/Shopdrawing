using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Reflection.Adds;
using System.Text;

namespace Microsoft.MetadataReader
{
	internal class MetadataOnlyMethodInfo : MethodInfo
	{
		private readonly Token m_methodDef;

		private readonly string m_name;

		private Type m_tOwner;

		private MethodSignatureDescriptor m_descriptor;

		private ParameterInfo m_returnParameter;

		private MethodBody m_methodBody;

		private readonly MethodAttributes m_attrs;

		private readonly Type[] m_typeArgs;

		private readonly Type[] m_methodArgs;

		private GenericContext m_context;

		private readonly MetadataOnlyModule m_resolver;

		private readonly Token m_declaringTypeDef;

		private readonly SignatureBlob m_sigBlob;

		private bool m_fullyInitialized;

		public override MethodAttributes Attributes
		{
			get
			{
				return this.m_attrs;
			}
		}

		public override CallingConventions CallingConvention
		{
			get
			{
				CallingConventions callingConvention;
				if (!this.m_fullyInitialized)
				{
					this.Initialize();
				}
				Microsoft.MetadataReader.CorCallingConvention corCallingConvention = this.m_descriptor.CallingConvention;
				callingConvention = ((corCallingConvention & Microsoft.MetadataReader.CorCallingConvention.Mask) != Microsoft.MetadataReader.CorCallingConvention.VarArg ? CallingConventions.Standard : CallingConventions.VarArgs);
				if ((corCallingConvention & Microsoft.MetadataReader.CorCallingConvention.HasThis) != Microsoft.MetadataReader.CorCallingConvention.Default)
				{
					callingConvention = callingConvention | CallingConventions.HasThis;
				}
				if ((corCallingConvention & Microsoft.MetadataReader.CorCallingConvention.ExplicitThis) != Microsoft.MetadataReader.CorCallingConvention.Default)
				{
					callingConvention = callingConvention | CallingConventions.ExplicitThis;
				}
				return callingConvention;
			}
		}

		public override bool ContainsGenericParameters
		{
			get
			{
				if (this.DeclaringType.ContainsGenericParameters)
				{
					return true;
				}
				Type[] genericArguments = this.GetGenericArguments();
				for (int i = 0; i < (int)genericArguments.Length; i++)
				{
					if (genericArguments[i].ContainsGenericParameters)
					{
						return true;
					}
				}
				return false;
			}
		}

		public override Type DeclaringType
		{
			get
			{
				if (!this.m_fullyInitialized)
				{
					this.Initialize();
				}
				return this.m_tOwner;
			}
		}

		public override bool IsGenericMethod
		{
			get
			{
				if (!this.m_fullyInitialized)
				{
					this.Initialize();
				}
				return !GenericContext.IsNullOrEmptyMethodArgs(this.m_context);
			}
		}

		public override bool IsGenericMethodDefinition
		{
			get
			{
				bool flag;
				if (!this.m_fullyInitialized)
				{
					this.Initialize();
				}
				if ((this.m_descriptor.CallingConvention & Microsoft.MetadataReader.CorCallingConvention.Generic) == Microsoft.MetadataReader.CorCallingConvention.Default)
				{
					return false;
				}
				if (GenericContext.IsNullOrEmptyMethodArgs(this.m_context))
				{
					return true;
				}
				MethodInfo methodInfo = this.Resolver.Factory.CreateMethodOrConstructor(this.Resolver, this.m_methodDef, null, null) as MethodInfo;
				Type[] methodArgs = this.m_context.MethodArgs;
				int num = 0;
				while (true)
				{
					if (num >= (int)methodArgs.Length)
					{
						return true;
					}
					Type type = methodArgs[num];
					if (!type.IsGenericParameter)
					{
						flag = false;
						break;
					}
					else if (methodInfo.Equals(type.DeclaringMethod))
					{
						num++;
					}
					else
					{
						flag = false;
						break;
					}
				}
				return flag;
			}
		}

		public override MemberTypes MemberType
		{
			get
			{
				return MemberTypes.Method;
			}
		}

		public override int MetadataToken
		{
			get
			{
				return this.m_methodDef.Value;
			}
		}

		public override RuntimeMethodHandle MethodHandle
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public override System.Reflection.Module Module
		{
			get
			{
				return this.m_resolver;
			}
		}

		public override string Name
		{
			get
			{
				return this.m_name;
			}
		}

		public override Type ReflectedType
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		internal MetadataOnlyModule Resolver
		{
			get
			{
				return this.m_resolver;
			}
		}

		public override ParameterInfo ReturnParameter
		{
			get
			{
				if (this.m_returnParameter == null)
				{
					this.GetParameters();
				}
				if (this.m_returnParameter != null)
				{
					return this.m_returnParameter;
				}
				return this.Resolver.Policy.GetFakeParameterInfo(this, this.ReturnType, -1);
			}
		}

		public override Type ReturnType
		{
			get
			{
				if (!this.m_fullyInitialized)
				{
					this.Initialize();
				}
				return this.m_descriptor.ReturnParameter.Type;
			}
		}

		public override ICustomAttributeProvider ReturnTypeCustomAttributes
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public MetadataOnlyMethodInfo(MetadataOnlyMethodInfo method)
		{
			this.m_resolver = method.m_resolver;
			this.m_methodDef = method.m_methodDef;
			this.m_tOwner = method.m_tOwner;
			this.m_descriptor = method.m_descriptor;
			this.m_name = method.m_name;
			this.m_attrs = method.m_attrs;
			this.m_returnParameter = method.m_returnParameter;
			this.m_methodBody = method.m_methodBody;
			this.m_declaringTypeDef = method.m_declaringTypeDef;
			this.m_sigBlob = method.m_sigBlob;
			this.m_typeArgs = method.m_typeArgs;
			this.m_methodArgs = method.m_methodArgs;
			this.m_context = method.m_context;
			this.m_fullyInitialized = method.m_fullyInitialized;
		}

		public MetadataOnlyMethodInfo(MetadataOnlyModule resolver, Token methodDef, Type[] typeArgs, Type[] methodArgs)
		{
			this.m_resolver = resolver;
			this.m_methodDef = methodDef;
			this.m_typeArgs = typeArgs;
			this.m_methodArgs = methodArgs;
			resolver.GetMethodProps(methodDef, out this.m_declaringTypeDef, out this.m_name, out this.m_attrs, out this.m_sigBlob);
		}

		internal static string CommonToString(MethodInfo m)
		{
			StringBuilder stringBuilder = new StringBuilder();
			MetadataOnlyCommonType.TypeSigToString(m.ReturnType, stringBuilder);
			stringBuilder.Append(" ");
			MetadataOnlyMethodInfo.ConstructMethodString(m, stringBuilder);
			return stringBuilder.ToString();
		}

		private static void ConstructMethodString(MethodInfo m, StringBuilder sb)
		{
			sb.Append(m.Name);
			string str = "";
			if (m.IsGenericMethod)
			{
				sb.Append("[");
				Type[] genericArguments = m.GetGenericArguments();
				for (int i = 0; i < (int)genericArguments.Length; i++)
				{
					Type type = genericArguments[i];
					sb.Append(str);
					MetadataOnlyCommonType.TypeSigToString(type, sb);
					str = ",";
				}
				sb.Append("]");
			}
			sb.Append("(");
			MetadataOnlyMethodInfo.ConstructParameters(sb, m.GetParameters(), m.CallingConvention);
			sb.Append(")");
		}

		private static void ConstructParameters(StringBuilder sb, ParameterInfo[] parameters, CallingConventions callingConvention)
		{
			Type[] parameterType = new Type[(int)parameters.Length];
			for (int i = 0; i < (int)parameters.Length; i++)
			{
				parameterType[i] = parameters[i].ParameterType;
			}
			MetadataOnlyMethodInfo.ConstructParameters(sb, parameterType, callingConvention);
		}

		private static void ConstructParameters(StringBuilder sb, Type[] parameters, CallingConventions callingConvention)
		{
			string str = "";
			for (int i = 0; i < (int)parameters.Length; i++)
			{
				Type type = parameters[i];
				sb.Append(str);
				MetadataOnlyCommonType.TypeSigToString(type, sb);
				if (type.IsByRef)
				{
					StringBuilder length = sb;
					length.Length = length.Length - 1;
					sb.Append(" ByRef");
				}
				str = ", ";
			}
			if ((callingConvention & CallingConventions.VarArgs) == CallingConventions.VarArgs)
			{
				sb.Append(str);
				sb.Append("...");
			}
		}

		internal static MethodBase Create(MetadataOnlyModule resolver, Token methodDef, GenericContext context)
		{
			Type[] emptyTypes = Type.EmptyTypes;
			Type[] methodArgs = Type.EmptyTypes;
			if (context != null)
			{
				emptyTypes = context.TypeArgs;
				methodArgs = context.MethodArgs;
			}
			return resolver.Factory.CreateMethodOrConstructor(resolver, methodDef, emptyTypes, methodArgs);
		}

		public override bool Equals(object obj)
		{
			MetadataOnlyMethodInfo metadataOnlyMethodInfo = obj as MetadataOnlyMethodInfo;
			if (metadataOnlyMethodInfo == null)
			{
				return false;
			}
			if (!this.DeclaringType.Equals(metadataOnlyMethodInfo.DeclaringType))
			{
				return false;
			}
			if (!this.IsGenericMethod)
			{
				return metadataOnlyMethodInfo.GetHashCode() == this.GetHashCode();
			}
			if (!metadataOnlyMethodInfo.IsGenericMethod)
			{
				return false;
			}
			Type[] genericArguments = this.GetGenericArguments();
			Type[] typeArray = metadataOnlyMethodInfo.GetGenericArguments();
			if ((int)genericArguments.Length != (int)typeArray.Length)
			{
				return false;
			}
			for (int i = 0; i < (int)genericArguments.Length; i++)
			{
				if (!genericArguments[i].Equals(typeArray[i]))
				{
					return false;
				}
			}
			return true;
		}

		public override MethodInfo GetBaseDefinition()
		{
			if (!base.IsVirtual || base.IsStatic || this.DeclaringType == null || this.DeclaringType.IsInterface)
			{
				return this;
			}
			Type baseType = this.DeclaringType.BaseType;
			if (baseType == null)
			{
				return this;
			}
			List<Type> types = new List<Type>();
			ParameterInfo[] parameters = this.GetParameters();
			for (int i = 0; i < (int)parameters.Length; i++)
			{
				types.Add(parameters[i].ParameterType);
			}
			MethodInfo method = baseType.GetMethod(this.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy, null, this.CallingConvention, types.ToArray(), null);
			if (method == null)
			{
				return this;
			}
			return method.GetBaseDefinition();
		}

		public override object[] GetCustomAttributes(bool inherit)
		{
			throw new NotSupportedException();
		}

		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			throw new NotSupportedException();
		}

		public override IList<CustomAttributeData> GetCustomAttributesData()
		{
			return this.Resolver.GetCustomAttributeData(this.MetadataToken);
		}

		public override Type[] GetGenericArguments()
		{
			if (!this.m_fullyInitialized)
			{
				this.Initialize();
			}
			return (Type[])this.m_context.MethodArgs.Clone();
		}

		private Type[] GetGenericMethodArgs()
		{
			Type[] mMethodArgs = null;
			int num = this.m_resolver.CountGenericParams(this.m_methodDef);
			bool flag = (this.m_methodArgs == null ? false : (int)this.m_methodArgs.Length > 0);
			if (num > 0)
			{
				if (flag)
				{
					if (num != (int)this.m_methodArgs.Length)
					{
						throw new ArgumentException(MetadataStringTable.WrongNumberOfGenericArguments);
					}
					mMethodArgs = this.m_methodArgs;
				}
				else
				{
					mMethodArgs = new Type[num];
					int num1 = 0;
					foreach (int genericParameterToken in this.m_resolver.GetGenericParameterTokens(this.m_methodDef))
					{
						int num2 = num1;
						num1 = num2 + 1;
						mMethodArgs[num2] = this.m_resolver.Factory.CreateTypeVariable(this.m_resolver, new Token(genericParameterToken));
					}
				}
			}
			if (mMethodArgs != null)
			{
				return mMethodArgs;
			}
			return Type.EmptyTypes;
		}

		public override MethodInfo GetGenericMethodDefinition()
		{
			if (!this.IsGenericMethod)
			{
				throw new InvalidOperationException();
			}
			if (this.IsGenericMethodDefinition)
			{
				return this;
			}
			return this.Resolver.Factory.CreateMethodOrConstructor(this.Resolver, this.m_methodDef, this.m_context.TypeArgs, null) as MethodInfo;
		}

		public override int GetHashCode()
		{
			Token mMethodDef = this.m_methodDef;
			return this.m_resolver.GetHashCode() * 32767 + mMethodDef.GetHashCode();
		}

		public override MethodBody GetMethodBody()
		{
			if (this.m_methodBody == null)
			{
				this.m_methodBody = MetadataOnlyMethodBody.TryCreate(this);
			}
			return this.m_methodBody;
		}

		public override MethodImplAttributes GetMethodImplementationFlags()
		{
			return this.m_resolver.GetMethodImplFlags(this.m_methodDef);
		}

		private void GetOwnerTypeAndTypeArgs(out Type ownerType, out Type[] typeArgs)
		{
			Type genericType = this.m_resolver.ResolveTypeDefToken(this.m_declaringTypeDef);
			GenericContext genericContext = new GenericContext(this.m_typeArgs, this.m_methodArgs);
			if (genericType.IsGenericType && GenericContext.IsNullOrEmptyTypeArgs(genericContext))
			{
				genericContext = new GenericContext(genericType.GetGenericArguments(), this.m_methodArgs);
			}
			genericType = this.m_resolver.GetGenericType(new Token(genericType.MetadataToken), genericContext);
			ownerType = genericType;
			typeArgs = genericContext.TypeArgs;
		}

		public override ParameterInfo[] GetParameters()
		{
			unsafe
			{
				uint num;
				uint num1;
				uint num2;
				uint num3;
				uint num4;
				uint num5;
				int num6;
				UnusedIntPtr unusedIntPtr;
				if (!this.m_fullyInitialized)
				{
					this.Initialize();
				}
				int length = (int)this.m_descriptor.Parameters.Length;
				ParameterInfo[] fakeParameterInfo = new ParameterInfo[length];
				Type[] type = new Type[length];
				for (int i = 0; i < length; i++)
				{
					type[i] = this.m_descriptor.Parameters[i].Type;
				}
				int[] numArray = new int[length + 1];
				IMetadataImport rawImport = this.m_resolver.RawImport;
				HCORENUM hCORENUM = new HCORENUM();
				if (rawImport.EnumParams(ref hCORENUM, this.m_methodDef.Value, numArray, (int)numArray.Length, out num) == 1)
				{
					for (int j = 0; j < length; j++)
					{
						fakeParameterInfo[j] = this.Resolver.Policy.GetFakeParameterInfo(this, type[j], j);
					}
					return fakeParameterInfo;
				}
				hCORENUM.Close(rawImport);
				if (num == 0)
				{
					return fakeParameterInfo;
				}
				ParameterInfo metadataOnlyParameterInfo = null;
				for (int k = 0; (long)k < (ulong)num; k++)
				{
					int num7 = numArray[k];
					rawImport.GetParamProps(num7, out num6, out num3, null, 0, out num2, out num1, out num4, out unusedIntPtr, out num5);
					if (num3 != 0)
					{
						uint num8 = num3 - 1;
						fakeParameterInfo[num8] = new MetadataOnlyParameterInfo(this.m_resolver, new Token(num7), type[num8], this.m_descriptor.Parameters[num8].CustomModifiers);
					}
					else
					{
						metadataOnlyParameterInfo = new MetadataOnlyParameterInfo(this.m_resolver, new Token(num7), this.ReturnType, this.m_descriptor.ReturnParameter.CustomModifiers);
					}
				}
				if (metadataOnlyParameterInfo == null)
				{
					metadataOnlyParameterInfo = this.Resolver.Policy.GetFakeParameterInfo(this, this.ReturnType, -1);
				}
				this.m_returnParameter = metadataOnlyParameterInfo;
				for (int l = 0; l < length; l++)
				{
					if (fakeParameterInfo[l] == null)
					{
						fakeParameterInfo[l] = this.Resolver.Policy.GetFakeParameterInfo(this, type[l], l);
					}
				}
				return fakeParameterInfo;
			}
		}

		private void Initialize()
		{
			Type type = null;
			Type[] mTypeArgs = null;
			if (this.m_declaringTypeDef.IsNil)
			{
				mTypeArgs = this.m_typeArgs;
			}
			else
			{
				this.GetOwnerTypeAndTypeArgs(out type, out mTypeArgs);
			}
			GenericContext genericContext = new GenericContext(mTypeArgs, this.GetGenericMethodArgs());
			MethodSignatureDescriptor methodSignatureDescriptor = SignatureUtil.ExtractMethodSignature(this.m_sigBlob, this.m_resolver, genericContext);
			this.m_tOwner = type;
			this.m_context = genericContext;
			this.m_descriptor = methodSignatureDescriptor;
			this.m_fullyInitialized = true;
		}

		public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
		{
			throw new NotSupportedException();
		}

		public override bool IsDefined(Type attributeType, bool inherit)
		{
			throw new NotSupportedException();
		}

		public override MethodInfo MakeGenericMethod(params Type[] types)
		{
			if (!this.IsGenericMethodDefinition)
			{
				throw new InvalidOperationException();
			}
			GenericContext genericContext = new GenericContext(this.m_context.TypeArgs, types);
			return (MethodInfo)MetadataOnlyMethodInfo.Create(this.m_resolver, this.m_methodDef, genericContext);
		}

		public override string ToString()
		{
			return MetadataOnlyMethodInfo.CommonToString(this);
		}
	}
}