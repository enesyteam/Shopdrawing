using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Reflection.Adds;

namespace Microsoft.MetadataReader
{
	internal class MetadataOnlyTypeVariableRef : MetadataOnlyCommonType
	{
		private readonly MetadataOnlyModule m_resolver;

		private readonly Token m_ownerToken;

		private readonly int m_position;

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
				throw new InvalidOperationException();
			}
		}

		public override MethodBase DeclaringMethod
		{
			get
			{
				if (!this.IsMethodVar)
				{
					return null;
				}
				return this.m_resolver.Factory.CreateMethodOrConstructor(this.m_resolver, this.m_ownerToken, null, null);
			}
		}

		public override Type DeclaringType
		{
			get
			{
				if (this.IsMethodVar)
				{
					return null;
				}
				if (this.m_ownerToken.IsType(System.Reflection.Adds.TokenType.TypeDef))
				{
					return this.m_resolver.Factory.CreateSimpleType(this.m_resolver, this.m_ownerToken);
				}
				return this.m_resolver.Factory.CreateTypeRef(this.m_resolver, this.m_ownerToken);
			}
		}

		public override string FullName
		{
			get
			{
				throw new InvalidOperationException();
			}
		}

		public override System.Reflection.GenericParameterAttributes GenericParameterAttributes
		{
			get
			{
				throw new InvalidOperationException();
			}
		}

		public override int GenericParameterPosition
		{
			get
			{
				return this.m_position;
			}
		}

		public override Guid GUID
		{
			get
			{
				throw new InvalidOperationException();
			}
		}

		public override bool IsGenericParameter
		{
			get
			{
				return true;
			}
		}

		private bool IsMethodVar
		{
			get
			{
				if (this.m_ownerToken.IsType(System.Reflection.Adds.TokenType.MemberRef))
				{
					return true;
				}
				return this.m_ownerToken.IsType(System.Reflection.Adds.TokenType.MethodDef);
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
				throw new InvalidOperationException();
			}
		}

		public override string Name
		{
			get
			{
				return null;
			}
		}

		public override string Namespace
		{
			get
			{
				return null;
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
				throw new InvalidOperationException();
			}
		}

		internal MetadataOnlyTypeVariableRef(MetadataOnlyModule resolver, Token ownerToken, int position)
		{
			this.m_resolver = resolver;
			this.m_ownerToken = ownerToken;
			this.m_position = position;
		}

		public override bool Equals(Type other)
		{
			MetadataOnlyTypeVariableRef metadataOnlyTypeVariableRef = other as MetadataOnlyTypeVariableRef;
			if (metadataOnlyTypeVariableRef != null)
			{
				if (!this.Resolver.Equals(metadataOnlyTypeVariableRef.Resolver) || this.m_ownerToken.Value != metadataOnlyTypeVariableRef.m_ownerToken.Value)
				{
					return false;
				}
				return this.m_position == metadataOnlyTypeVariableRef.m_position;
			}
			if (!other.IsGenericParameter)
			{
				return false;
			}
			bool isMethodVar = this.IsMethodVar == (other.DeclaringMethod != null);
			if (this.m_position == other.GenericParameterPosition)
			{
				return isMethodVar;
			}
			return false;
		}

		protected override TypeAttributes GetAttributeFlagsImpl()
		{
			throw new InvalidOperationException();
		}

		protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			throw new InvalidOperationException();
		}

		public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
		{
			throw new InvalidOperationException();
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
			throw new InvalidOperationException();
		}

		public override Type GetElementType()
		{
			throw new InvalidOperationException();
		}

		public override EventInfo GetEvent(string name, BindingFlags flags)
		{
			throw new InvalidOperationException();
		}

		public override EventInfo[] GetEvents(BindingFlags flags)
		{
			throw new InvalidOperationException();
		}

		public override FieldInfo GetField(string name, BindingFlags flags)
		{
			throw new InvalidOperationException();
		}

		public override FieldInfo[] GetFields(BindingFlags flags)
		{
			throw new InvalidOperationException();
		}

		public override Type[] GetGenericArguments()
		{
			throw new InvalidOperationException();
		}

		public override Type[] GetGenericParameterConstraints()
		{
			throw new InvalidOperationException();
		}

		public override Type GetGenericTypeDefinition()
		{
			throw new InvalidOperationException();
		}

		public override Type GetInterface(string name, bool ignoreCase)
		{
			throw new InvalidOperationException();
		}

		public override Type[] GetInterfaces()
		{
			throw new InvalidOperationException();
		}

		public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
		{
			throw new InvalidOperationException();
		}

		protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			throw new InvalidOperationException();
		}

		public override MethodInfo[] GetMethods(BindingFlags flags)
		{
			throw new InvalidOperationException();
		}

		public override Type GetNestedType(string name, BindingFlags bindingAttr)
		{
			throw new InvalidOperationException();
		}

		public override Type[] GetNestedTypes(BindingFlags bindingAttr)
		{
			throw new InvalidOperationException();
		}

		public override PropertyInfo[] GetProperties(BindingFlags flags)
		{
			throw new InvalidOperationException();
		}

		protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
		{
			throw new InvalidOperationException();
		}

		protected override TypeCode GetTypeCodeImpl()
		{
			throw new InvalidOperationException();
		}

		protected override bool HasElementTypeImpl()
		{
			throw new InvalidOperationException();
		}

		public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
		{
			throw new NotSupportedException();
		}

		public override bool IsAssignableFrom(Type c)
		{
			throw new InvalidOperationException();
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
			if (this.IsMethodVar)
			{
				int genericParameterPosition = this.GenericParameterPosition;
				return string.Concat("MVar!!", genericParameterPosition.ToString(CultureInfo.InvariantCulture));
			}
			int num = this.GenericParameterPosition;
			return string.Concat("Var!", num.ToString(CultureInfo.InvariantCulture));
		}
	}
}