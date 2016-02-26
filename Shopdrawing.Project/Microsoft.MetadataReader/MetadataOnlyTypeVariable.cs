using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Reflection.Adds;

namespace Microsoft.MetadataReader
{
	internal class MetadataOnlyTypeVariable : MetadataOnlyCommonType
	{
		private readonly int m_ownerMethodToken;

		private readonly int m_ownerTypeToken;

		private readonly string m_name;

		private readonly uint m_position;

		private readonly MetadataOnlyModule m_resolver;

		private readonly int m_Token;

		private readonly System.Reflection.GenericParameterAttributes m_gpAttributes;

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
				Type[] genericParameterConstraints = this.GetGenericParameterConstraints();
				for (int i = 0; i < (int)genericParameterConstraints.Length; i++)
				{
					Type type = genericParameterConstraints[i];
					if (type.IsClass)
					{
						return type;
					}
				}
				return this.m_resolver.AssemblyResolver.GetBuiltInType(System.Reflection.Adds.CorElementType.Object);
			}
		}

		public override MethodBase DeclaringMethod
		{
			get
			{
				if (this.m_ownerMethodToken == 0)
				{
					return null;
				}
				return this.m_resolver.ResolveMethod(this.m_ownerMethodToken);
			}
		}

		public override Type DeclaringType
		{
			get
			{
				if (this.m_ownerTypeToken != 0)
				{
					return this.m_resolver.ResolveType(this.m_ownerTypeToken);
				}
				if (this.DeclaringMethod == null)
				{
					return null;
				}
				return this.DeclaringMethod.DeclaringType;
			}
		}

		public override string FullName
		{
			get
			{
				return null;
			}
		}

		public override System.Reflection.GenericParameterAttributes GenericParameterAttributes
		{
			get
			{
				return this.m_gpAttributes;
			}
		}

		public override int GenericParameterPosition
		{
			get
			{
				return (int)this.m_position;
			}
		}

		public override Guid GUID
		{
			get
			{
				return Guid.Empty;
			}
		}

		public override bool IsGenericParameter
		{
			get
			{
				return true;
			}
		}

		public override MemberTypes MemberType
		{
			get
			{
				return MemberTypes.TypeInfo;
			}
		}

		public override int MetadataToken
		{
			get
			{
				return this.m_Token;
			}
		}

		public override string Name
		{
			get
			{
				return this.m_name;
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
				if (this.DeclaringMethod == null)
				{
					return null;
				}
				return this.DeclaringMethod.DeclaringType.Namespace;
			}
		}

		public override Type ReflectedType
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		internal override MetadataOnlyModule Resolver
		{
			get
			{
				return this.m_resolver;
			}
		}

		public override Type UnderlyingSystemType
		{
			get
			{
				return this;
			}
		}

		internal MetadataOnlyTypeVariable(MetadataOnlyModule resolver, Token token)
		{
			this.m_Token = token.Value;
			this.m_resolver = resolver;
			this.m_resolver.GetGenericParameterProps(this.m_Token, out this.m_ownerTypeToken, out this.m_ownerMethodToken, out this.m_name, out this.m_gpAttributes, out this.m_position);
		}

		public override bool Equals(Type txOther)
		{
			if (txOther is MetadataOnlyTypeVariableRef)
			{
				if (this.m_ownerMethodToken == 0)
				{
					return false;
				}
				return (ulong)this.m_position == (long)txOther.GenericParameterPosition;
			}
			MetadataOnlyTypeVariable metadataOnlyTypeVariable = txOther as MetadataOnlyTypeVariable;
			if (metadataOnlyTypeVariable == null)
			{
				return false;
			}
			if (this.Name != metadataOnlyTypeVariable.Name)
			{
				return false;
			}
			if (this.m_ownerTypeToken != metadataOnlyTypeVariable.m_ownerTypeToken || this.m_ownerMethodToken != metadataOnlyTypeVariable.m_ownerMethodToken)
			{
				return false;
			}
			return this.Module.Equals(metadataOnlyTypeVariable.Module);
		}

		protected override TypeAttributes GetAttributeFlagsImpl()
		{
			return TypeAttributes.Public;
		}

		protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			return null;
		}

		public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
		{
			return new ConstructorInfo[0];
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

		public override Type GetElementType()
		{
			return null;
		}

		public override EventInfo GetEvent(string name, BindingFlags flags)
		{
			return null;
		}

		public override EventInfo[] GetEvents(BindingFlags flags)
		{
			return new EventInfo[0];
		}

		public override FieldInfo GetField(string name, BindingFlags flags)
		{
			return null;
		}

		public override FieldInfo[] GetFields(BindingFlags flags)
		{
			return new FieldInfo[0];
		}

		public override Type[] GetGenericArguments()
		{
			return Type.EmptyTypes;
		}

		public override Type[] GetGenericParameterConstraints()
		{
			List<Type> types = new List<Type>(this.m_resolver.GetConstraintTypes(this.m_Token));
			return types.ToArray();
		}

		public override Type GetGenericTypeDefinition()
		{
			throw new InvalidOperationException();
		}

		public override Type GetInterface(string name, bool ignoreCase)
		{
			return MetadataOnlyModule.GetInterfaceHelper(this.GetInterfaces(), name, ignoreCase);
		}

		public override Type[] GetInterfaces()
		{
			return MetadataOnlyTypeDef.GetAllInterfacesHelper(this);
		}

		public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
		{
			return MetadataOnlyTypeDef.GetMembersHelper(this, bindingAttr);
		}

		protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			return null;
		}

		public override MethodInfo[] GetMethods(BindingFlags flags)
		{
			return new MethodInfo[0];
		}

		public override Type GetNestedType(string name, BindingFlags bindingAttr)
		{
			return null;
		}

		public override Type[] GetNestedTypes(BindingFlags bindingAttr)
		{
			return Type.EmptyTypes;
		}

		public override PropertyInfo[] GetProperties(BindingFlags flags)
		{
			return new PropertyInfo[0];
		}

		protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
		{
			return null;
		}

		protected override TypeCode GetTypeCodeImpl()
		{
			return TypeCode.Object;
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

		public override bool IsDefined(Type attributeType, bool inherit)
		{
			throw new NotSupportedException();
		}

		public override Type MakeGenericType(params Type[] argTypes)
		{
			throw new InvalidOperationException();
		}

		public override string ToString()
		{
			return this.Name;
		}
	}
}