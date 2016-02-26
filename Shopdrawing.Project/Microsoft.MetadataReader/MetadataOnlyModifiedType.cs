using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace Microsoft.MetadataReader
{
	internal class MetadataOnlyModifiedType : MetadataOnlyCommonType
	{
		private readonly MetadataOnlyCommonType m_type;

		private readonly string m_mod;

		public override System.Reflection.Assembly Assembly
		{
			get
			{
				return this.m_type.Assembly;
			}
		}

		public override Type BaseType
		{
			get
			{
				return null;
			}
		}

		public override Type DeclaringType
		{
			get
			{
				return null;
			}
		}

		public override string FullName
		{
			get
			{
				string fullName = this.m_type.FullName;
				if (fullName == null || this.m_type.IsGenericTypeDefinition)
				{
					return null;
				}
				return string.Concat(fullName, this.m_mod);
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
				return Guid.Empty;
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
				return 33554432;
			}
		}

		public override string Name
		{
			get
			{
				return string.Concat(this.m_type.Name, this.m_mod);
			}
		}

		public override string Namespace
		{
			get
			{
				return this.m_type.Namespace;
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
				return this.m_type.Resolver;
			}
		}

		public override Type UnderlyingSystemType
		{
			get
			{
				return this;
			}
		}

		public MetadataOnlyModifiedType(MetadataOnlyCommonType type, string mod)
		{
			this.m_type = type;
			this.m_mod = mod;
		}

		public override bool Equals(Type t)
		{
			if (t == null)
			{
				return false;
			}
			if (base.IsByRef)
			{
				if (!t.IsByRef)
				{
					return false;
				}
			}
			else if (base.IsPointer && !t.IsPointer)
			{
				return false;
			}
			Type elementType = t.GetElementType();
			return this.m_type.Equals(elementType);
		}

		protected override TypeAttributes GetAttributeFlagsImpl()
		{
			return TypeAttributes.NotPublic;
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
			return new CustomAttributeData[0];
		}

		public override Type GetElementType()
		{
			return this.m_type;
		}

		public override EventInfo GetEvent(string name, BindingFlags flags)
		{
			return null;
		}

		public override EventInfo[] GetEvents(BindingFlags flags)
		{
			return new EventInfo[0];
		}

		public override FieldInfo GetField(string name, BindingFlags bindingAttr)
		{
			return null;
		}

		public override FieldInfo[] GetFields(BindingFlags flags)
		{
			return new FieldInfo[0];
		}

		public override Type[] GetGenericArguments()
		{
			return this.m_type.GetGenericArguments();
		}

		public override Type GetGenericTypeDefinition()
		{
			throw new InvalidOperationException();
		}

		public override Type GetInterface(string name, bool ignoreCase)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			return null;
		}

		public override Type[] GetInterfaces()
		{
			return new Type[0];
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
			return true;
		}

		public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
		{
			throw new NotSupportedException();
		}

		public override bool IsAssignableFrom(Type c)
		{
			if (c == null)
			{
				return false;
			}
			if (base.IsPointer && c.IsPointer || base.IsByRef && c.IsByRef)
			{
				Type elementType = c.GetElementType();
				if (this.m_type.IsAssignableFrom(elementType) && !elementType.IsValueType)
				{
					return true;
				}
			}
			return MetadataOnlyTypeDef.IsAssignableFromHelper(this, c);
		}

		protected override bool IsByRefImpl()
		{
			return this.m_mod.Equals("&");
		}

		public override bool IsDefined(Type attributeType, bool inherit)
		{
			throw new NotSupportedException();
		}

		protected override bool IsPointerImpl()
		{
			return this.m_mod.Equals("*");
		}

		public override string ToString()
		{
			return string.Concat(this.m_type.ToString(), this.m_mod);
		}
	}
}