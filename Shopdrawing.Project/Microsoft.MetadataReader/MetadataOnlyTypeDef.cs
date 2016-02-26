using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Reflection.Adds;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.MetadataReader
{
	internal class MetadataOnlyTypeDef : MetadataOnlyCommonType
	{
		private readonly MetadataOnlyModule m_resolver;

		private readonly Token m_tokenTypeDef;

		private readonly Type[] m_typeParameters;

		private readonly Token m_tokenExtends;

		private readonly string m_fullName;

		private readonly TypeAttributes m_typeAttributes;

		private Type m_baseType;

		private MetadataOnlyTypeDef.TriState m_fIsValueType = MetadataOnlyTypeDef.TriState.Maybe;

		private readonly static string[] PrimitiveTypeNames;

		public override System.Reflection.Assembly Assembly
		{
			get
			{
				return this.m_resolver.Assembly;
			}
		}

		public override Type BaseType
		{
			get
			{
				if (this.m_baseType == null)
				{
					if (this.m_tokenExtends.IsNil)
					{
						return null;
					}
					this.m_baseType = this.m_resolver.ResolveTypeTokenInternal(this.m_tokenExtends, this.GenericContext);
				}
				return this.m_baseType;
			}
		}

		public override Type DeclaringType
		{
			get
			{
				Type enclosingType = this.m_resolver.GetEnclosingType(new Token(this.MetadataToken));
				if (enclosingType != null)
				{
					return enclosingType;
				}
				return null;
			}
		}

		public override string FullName
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				this.GetSimpleName(stringBuilder);
				if (!this.IsGenericType || this.IsGenericTypeDefinition)
				{
					return stringBuilder.ToString();
				}
				stringBuilder.Append("[");
				Type[] genericArguments = this.GetGenericArguments();
				for (int i = 0; i < (int)genericArguments.Length; i++)
				{
					if (i > 0)
					{
						stringBuilder.Append(",");
					}
					stringBuilder.Append('[');
					if (genericArguments[i].FullName == null || genericArguments[i].IsGenericTypeDefinition)
					{
						return null;
					}
					stringBuilder.Append(genericArguments[i].AssemblyQualifiedName);
					stringBuilder.Append(']');
				}
				stringBuilder.Append("]");
				return stringBuilder.ToString();
			}
		}

		public override System.Reflection.GenericParameterAttributes GenericParameterAttributes
		{
			get
			{
				throw new InvalidOperationException(MetadataStringTable.ValidOnGenericParameterTypeOnly);
			}
		}

		public override Guid GUID
		{
			get
			{
				Guid guid;
				using (IEnumerator<CustomAttributeData> enumerator = this.GetCustomAttributesData().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						CustomAttributeData current = enumerator.Current;
						if (!current.Constructor.DeclaringType.FullName.Equals("System.Runtime.InteropServices.GuidAttribute"))
						{
							continue;
						}
						CustomAttributeTypedArgument item = current.ConstructorArguments[0];
						guid = new Guid((string)item.Value);
						return guid;
					}
					return Guid.Empty;
				}
				return guid;
			}
		}

		public override bool IsEnum
		{
			get
			{
				Type typeXFromName = this.m_resolver.AssemblyResolver.GetTypeXFromName("System.Enum");
				return typeXFromName.Equals(this.BaseType);
			}
		}

		public override bool IsGenericType
		{
			get
			{
				return (int)this.m_typeParameters.Length > 0;
			}
		}

		public override bool IsGenericTypeDefinition
		{
			get
			{
				bool flag;
				if (!this.IsGenericType)
				{
					return false;
				}
				Type[] genericArguments = this.GetGenericArguments();
				int num = 0;
				while (true)
				{
					if (num >= (int)genericArguments.Length)
					{
						return true;
					}
					Type type = genericArguments[num];
					if (!type.IsGenericParameter)
					{
						flag = false;
						break;
					}
					else if (type.DeclaringType.Equals(this))
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
				if (base.IsNested)
				{
					return MemberTypes.NestedType;
				}
				return MemberTypes.TypeInfo;
			}
		}

		public override int MetadataToken
		{
			get
			{
				return this.m_tokenTypeDef.Value;
			}
		}

		public override string Name
		{
			get
			{
				return Utility.GetTypeNameFromFullNameHelper(this.m_fullName, base.IsNested);
			}
		}

		public override string Namespace
		{
			get
			{
				if (this.DeclaringType != null)
				{
					return this.DeclaringType.Namespace;
				}
				return Utility.GetNamespaceHelper(this.m_fullName);
			}
		}

		internal override MetadataOnlyModule Resolver
		{
			get
			{
				return this.m_resolver;
			}
		}

		public override System.Runtime.InteropServices.StructLayoutAttribute StructLayoutAttribute
		{
			get
			{
				uint num;
				LayoutKind layoutKind;
				if (base.IsInterface)
				{
					return null;
				}
				uint num1 = 0;
				this.m_resolver.RawImport.GetClassLayout(this.m_tokenTypeDef, out num, UnusedIntPtr.Zero, 0, UnusedIntPtr.Zero, ref num1);
				if (num == 0)
				{
					num = 8;
				}
				TypeAttributes mTypeAttributes = this.m_typeAttributes & TypeAttributes.LayoutMask;
				if (mTypeAttributes == TypeAttributes.NotPublic)
				{
					layoutKind = LayoutKind.Auto;
				}
				else if (mTypeAttributes == TypeAttributes.SequentialLayout)
				{
					layoutKind = LayoutKind.Sequential;
				}
				else
				{
					if (mTypeAttributes != TypeAttributes.ExplicitLayout)
					{
						throw new InvalidOperationException(MetadataStringTable.IllegalLayoutMask);
					}
					layoutKind = LayoutKind.Explicit;
				}
				CharSet charSet = CharSet.None;
				TypeAttributes typeAttribute = this.m_typeAttributes & TypeAttributes.StringFormatMask;
				if (typeAttribute == TypeAttributes.NotPublic)
				{
					charSet = CharSet.Ansi;
				}
				else if (typeAttribute == TypeAttributes.UnicodeClass)
				{
					charSet = CharSet.Unicode;
				}
				else if (typeAttribute == TypeAttributes.AutoClass)
				{
					charSet = CharSet.Auto;
				}
				System.Runtime.InteropServices.StructLayoutAttribute structLayoutAttribute = new System.Runtime.InteropServices.StructLayoutAttribute(layoutKind)
				{
					Size = (int)num1,
					Pack = (int)num,
					CharSet = charSet
				};
				return structLayoutAttribute;
			}
		}

		public override Type UnderlyingSystemType
		{
			get
			{
				return this;
			}
		}

		static MetadataOnlyTypeDef()
		{
			string[] strArrays = new string[] { "System.Boolean", "System.Char", "System.SByte", "System.Byte", "System.Int16", "System.UInt16", "System.Int32", "System.UInt32", "System.Int64", "System.UInt64", "System.Single", "System.Double", "System.IntPtr", "System.UIntPtr" };
			MetadataOnlyTypeDef.PrimitiveTypeNames = strArrays;
		}

		public MetadataOnlyTypeDef(MetadataOnlyModule scope, Token tokenTypeDef) : this(scope, tokenTypeDef, null)
		{
		}

		public MetadataOnlyTypeDef(MetadataOnlyModule scope, Token tokenTypeDef, Type[] typeParameters)
		{
			MetadataOnlyTypeDef.ValidateConstructorArguments(scope, tokenTypeDef);
			this.m_resolver = scope;
			this.m_tokenTypeDef = tokenTypeDef;
			this.m_typeParameters = null;
			this.m_resolver.GetTypeProps(this.m_tokenTypeDef, out this.m_tokenExtends, out this.m_fullName, out this.m_typeAttributes);
			int num = this.m_resolver.CountGenericParams(this.m_tokenTypeDef);
			bool flag = (typeParameters == null ? false : (int)typeParameters.Length > 0);
			if (num <= 0)
			{
				this.m_typeParameters = Type.EmptyTypes;
			}
			else
			{
				if (flag)
				{
					if (num != (int)typeParameters.Length)
					{
						throw new ArgumentException(MetadataStringTable.WrongNumberOfGenericArguments);
					}
					this.m_typeParameters = typeParameters;
					return;
				}
				this.m_typeParameters = new Type[num];
				int num1 = 0;
				foreach (int genericParameterToken in this.m_resolver.GetGenericParameterTokens(this.m_tokenTypeDef))
				{
					int num2 = num1;
					num1 = num2 + 1;
					this.m_typeParameters[num2] = this.m_resolver.Factory.CreateTypeVariable(this.m_resolver, new Token(genericParameterToken));
				}
			}
		}

		public override bool Equals(Type other)
		{
			if (other == null)
			{
				return false;
			}
			if (!this.Module.Equals(other.Module))
			{
				return false;
			}
			bool isGenericType = this.IsGenericType;
			bool flag = other.IsGenericType;
			if (isGenericType != flag)
			{
				return false;
			}
			if (this.MetadataToken != other.MetadataToken)
			{
				return false;
			}
			if (!isGenericType && !flag)
			{
				return true;
			}
			Type[] genericArguments = this.GetGenericArguments();
			Type[] typeArray = other.GetGenericArguments();
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

		internal static Type[] GetAllInterfacesHelper(MetadataOnlyCommonType type)
		{
			HashSet<Type> types = new HashSet<Type>();
			if (type.BaseType != null)
			{
				types.UnionWith(type.BaseType.GetInterfaces());
			}
			foreach (Type interfacesOnType in type.Resolver.GetInterfacesOnType(type))
			{
				if (types.Contains(interfacesOnType))
				{
					continue;
				}
				types.Add(interfacesOnType);
				types.UnionWith(interfacesOnType.GetInterfaces());
			}
			Type[] typeArray = new Type[types.Count];
			types.CopyTo(typeArray);
			return typeArray;
		}

		protected override TypeAttributes GetAttributeFlagsImpl()
		{
			return this.m_typeAttributes;
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

		internal override IEnumerable<MethodBase> GetDeclaredConstructors()
		{
			return this.Resolver.GetMethodBasesOnDeclaredTypeOnly(this.m_tokenTypeDef, this.GenericContext, MetadataOnlyModule.EMethodKind.Constructor);
		}

		internal override IEnumerable<MethodBase> GetDeclaredMethods()
		{
			return this.Resolver.GetMethodBasesOnDeclaredTypeOnly(this.m_tokenTypeDef, this.GenericContext, MetadataOnlyModule.EMethodKind.Methods);
		}

		internal override IEnumerable<PropertyInfo> GetDeclaredProperties()
		{
			return this.Resolver.GetPropertiesOnDeclaredTypeOnly(this.m_tokenTypeDef, this.GenericContext);
		}

		public override Type GetElementType()
		{
			return null;
		}

		public override EventInfo GetEvent(string name, BindingFlags flags)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			StringComparison stringComparison = SignatureUtil.GetStringComparison(flags);
			EventInfo[] events = this.GetEvents(flags);
			for (int i = 0; i < (int)events.Length; i++)
			{
				EventInfo eventInfo = events[i];
				if (eventInfo.Name.Equals(name, stringComparison))
				{
					return eventInfo;
				}
			}
			return null;
		}

		public override EventInfo[] GetEvents(BindingFlags flags)
		{
			return MetadataOnlyModule.GetEventsOnType(this, flags);
		}

		public override FieldInfo GetField(string name, BindingFlags bindingAttr)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			StringComparison stringComparison = SignatureUtil.GetStringComparison(bindingAttr);
			FieldInfo[] fields = this.GetFields(bindingAttr);
			for (int i = 0; i < (int)fields.Length; i++)
			{
				FieldInfo fieldInfo = fields[i];
				if (fieldInfo.Name.Equals(name, stringComparison))
				{
					return fieldInfo;
				}
			}
			return null;
		}

		public override FieldInfo[] GetFields(BindingFlags flags)
		{
			return MetadataOnlyModule.GetFieldsOnType(this, flags);
		}

		public override Type[] GetGenericArguments()
		{
			return (Type[])this.m_typeParameters.Clone();
		}

		public override Type GetGenericTypeDefinition()
		{
			if (!this.IsGenericType)
			{
				throw new InvalidOperationException();
			}
			if (this.IsGenericTypeDefinition)
			{
				return this;
			}
			return this.Resolver.Factory.CreateSimpleType(this.Resolver, this.m_tokenTypeDef);
		}

		public override Type GetInterface(string name, bool ignoreCase)
		{
			return MetadataOnlyModule.GetInterfaceHelper(this.GetInterfaces(), name, ignoreCase);
		}

		public override Type[] GetInterfaces()
		{
			return MetadataOnlyTypeDef.GetAllInterfacesHelper(this);
		}

		public override MemberInfo[] GetMember(string name, MemberTypes type, BindingFlags bindingAttr)
		{
			MemberInfo[] members = this.GetMembers(bindingAttr);
			List<MemberInfo> memberInfos = new List<MemberInfo>();
			StringComparison stringComparison = SignatureUtil.GetStringComparison(bindingAttr);
			MemberInfo[] memberInfoArray = members;
			for (int i = 0; i < (int)memberInfoArray.Length; i++)
			{
				MemberInfo memberInfo = memberInfoArray[i];
				if (name.Equals(memberInfo.Name, stringComparison) && (type == memberInfo.MemberType || type == MemberTypes.All))
				{
					memberInfos.Add(memberInfo);
				}
			}
			return memberInfos.ToArray();
		}

		internal static MemberInfo[] GetMembersHelper(Type type, BindingFlags bindingAttr)
		{
			List<MemberInfo> memberInfos = new List<MemberInfo>(type.GetMethods(bindingAttr));
			memberInfos.AddRange(type.GetConstructors(bindingAttr));
			memberInfos.AddRange(type.GetFields(bindingAttr));
			memberInfos.AddRange(type.GetProperties(bindingAttr));
			memberInfos.AddRange(type.GetEvents(bindingAttr));
			memberInfos.AddRange(type.GetNestedTypes(bindingAttr));
			return memberInfos.ToArray();
		}

		public override Type GetNestedType(string name, BindingFlags bindingAttr)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			StringComparison stringComparison = SignatureUtil.GetStringComparison(bindingAttr);
			Type[] nestedTypes = this.GetNestedTypes(bindingAttr);
			for (int i = 0; i < (int)nestedTypes.Length; i++)
			{
				Type type = nestedTypes[i];
				if (type.Name.Equals(name, stringComparison))
				{
					return type;
				}
			}
			return null;
		}

		public override Type[] GetNestedTypes(BindingFlags bindingAttr)
		{
			List<Type> types = new List<Type>(this.m_resolver.GetNestedTypesOnType(this, bindingAttr));
			return types.ToArray();
		}

		internal static PropertyInfo GetPropertyImplHelper(MetadataOnlyCommonType type, string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
		{
			if (binder != null)
			{
				throw new NotSupportedException();
			}
			if (modifiers != null && (int)modifiers.Length != 0)
			{
				throw new NotSupportedException();
			}
			StringComparison stringComparison = SignatureUtil.GetStringComparison(bindingAttr);
			PropertyInfo[] properties = type.GetProperties(bindingAttr);
			for (int i = 0; i < (int)properties.Length; i++)
			{
				PropertyInfo propertyInfo = properties[i];
				if (propertyInfo.Name.Equals(name, stringComparison) && (!(returnType != null) || propertyInfo.PropertyType.Equals(returnType)) && MetadataOnlyTypeDef.PropertyParamTypesMatch(propertyInfo, types))
				{
					return propertyInfo;
				}
			}
			return null;
		}

		private void GetSimpleName(StringBuilder sb)
		{
			Type declaringType = this.DeclaringType;
			if (declaringType != null)
			{
				sb.Append(declaringType.FullName);
				sb.Append('+');
			}
			sb.Append(this.m_fullName);
		}

		protected override TypeCode GetTypeCodeImpl()
		{
			return this.m_resolver.GetTypeCode(this);
		}

		protected override bool HasElementTypeImpl()
		{
			return false;
		}

		public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
		{
			throw new NotSupportedException();
		}

		public override bool IsAssignableFrom(Type c)
		{
			return MetadataOnlyTypeDef.IsAssignableFromHelper(this, c);
		}

		internal static bool IsAssignableFromHelper(Type current, Type target)
		{
			if (target == null)
			{
				return false;
			}
			if (current.Equals(target))
			{
				return true;
			}
			if (target.IsSubclassOf(current))
			{
				return true;
			}
			Type[] interfaces = target.GetInterfaces();
			for (int i = 0; i < (int)interfaces.Length; i++)
			{
				if (interfaces[i].Equals(current))
				{
					return true;
				}
				if (current.IsAssignableFrom(interfaces[i]))
				{
					return true;
				}
			}
			if (target.IsGenericParameter)
			{
				Type[] genericParameterConstraints = target.GetGenericParameterConstraints();
				for (int j = 0; j < (int)genericParameterConstraints.Length; j++)
				{
					if (MetadataOnlyTypeDef.IsAssignableFromHelper(current, genericParameterConstraints[j]))
					{
						return true;
					}
				}
			}
			ITypeUniverse typeUniverse = Helpers.Universe(current);
			if (typeUniverse == null || !current.Equals(typeUniverse.GetTypeXFromName("System.Object")))
			{
				return false;
			}
			if (target.IsPointer || target.IsInterface)
			{
				return true;
			}
			return target.IsArray;
		}

		protected override bool IsCOMObjectImpl()
		{
			throw new NotImplementedException();
		}

		public override bool IsDefined(Type attributeType, bool inherit)
		{
			throw new NotSupportedException();
		}

		protected override bool IsPrimitiveImpl()
		{
			if (!this.m_resolver.IsSystemModule())
			{
				return false;
			}
			string fullName = this.FullName;
			string[] primitiveTypeNames = MetadataOnlyTypeDef.PrimitiveTypeNames;
			for (int i = 0; i < (int)primitiveTypeNames.Length; i++)
			{
				if (primitiveTypeNames[i].Equals(fullName, StringComparison.Ordinal))
				{
					return true;
				}
			}
			return false;
		}

		private bool IsValueTypeHelper()
		{
			MetadataOnlyModule resolver = this.Resolver;
			if (this.Equals(resolver.AssemblyResolver.GetTypeXFromName("System.Enum")))
			{
				return false;
			}
			if (resolver.AssemblyResolver.GetTypeXFromName("System.ValueType").Equals(this.BaseType))
			{
				return true;
			}
			return this.IsEnum;
		}

		protected override bool IsValueTypeImpl()
		{
			if (this.m_fIsValueType == MetadataOnlyTypeDef.TriState.Maybe)
			{
				if (!this.IsValueTypeHelper())
				{
					this.m_fIsValueType = MetadataOnlyTypeDef.TriState.No;
				}
				else
				{
					this.m_fIsValueType = MetadataOnlyTypeDef.TriState.Yes;
				}
			}
			if (this.m_fIsValueType == MetadataOnlyTypeDef.TriState.Yes)
			{
				return true;
			}
			if (this.m_fIsValueType == MetadataOnlyTypeDef.TriState.No)
			{
				return false;
			}
			return false;
		}

		public override Type MakeGenericType(params Type[] argTypes)
		{
			if (argTypes == null)
			{
				throw new ArgumentNullException("argTypes");
			}
			if (!this.IsGenericTypeDefinition)
			{
				throw new InvalidOperationException();
			}
			if ((int)argTypes.Length != (int)this.m_typeParameters.Length)
			{
				throw new ArgumentException(MetadataStringTable.WrongNumberOfGenericArguments);
			}
			return this.Resolver.Factory.CreateGenericType(this.Resolver, this.m_tokenTypeDef, argTypes);
		}

		private static bool PropertyParamTypesMatch(PropertyInfo p, Type[] types)
		{
			if (types == null)
			{
				return true;
			}
			ParameterInfo[] indexParameters = p.GetIndexParameters();
			if ((int)indexParameters.Length != (int)types.Length)
			{
				return false;
			}
			int length = (int)indexParameters.Length;
			for (int i = 0; i < length; i++)
			{
				if (!indexParameters[i].ParameterType.Equals(types[i]))
				{
					return false;
				}
			}
			return true;
		}

		public override string ToString()
		{
			if (!this.IsGenericType)
			{
				return this.FullName;
			}
			StringBuilder stringBuilder = new StringBuilder();
			this.GetSimpleName(stringBuilder);
			stringBuilder.Append("[");
			Type[] genericArguments = this.GetGenericArguments();
			for (int i = 0; i < (int)genericArguments.Length; i++)
			{
				if (i != 0)
				{
					stringBuilder.Append(",");
				}
				stringBuilder.Append(genericArguments[i].ToString());
			}
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		}

		private static void ValidateConstructorArguments(MetadataOnlyModule scope, Token tokenTypeDef)
		{
			if (scope == null)
			{
				throw new ArgumentNullException("scope");
			}
			if (!tokenTypeDef.IsType(System.Reflection.Adds.TokenType.TypeDef))
			{
				CultureInfo invariantCulture = CultureInfo.InvariantCulture;
				string expectedTokenType = MetadataStringTable.ExpectedTokenType;
				object[] str = new object[] { System.Reflection.Adds.TokenType.TypeDef.ToString() };
				throw new ArgumentException(string.Format(invariantCulture, expectedTokenType, str));
			}
		}

		private enum TriState
		{
			Yes,
			No,
			Maybe
		}
	}
}