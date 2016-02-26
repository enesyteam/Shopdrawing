using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Reflection.Adds;
using System.Runtime.InteropServices;

namespace Microsoft.MetadataReader
{
	[DebuggerDisplay("TypeProxy")]
	internal abstract class TypeProxy : MetadataOnlyCommonType, ITypeProxy
	{
		protected readonly MetadataOnlyModule m_resolver;

		private Type m_cachedResolvedType;

		public override System.Reflection.Assembly Assembly
		{
			get
			{
				return this.GetResolvedType().Assembly;
			}
		}

		public override string AssemblyQualifiedName
		{
			get
			{
				return this.GetResolvedType().AssemblyQualifiedName;
			}
		}

		public override Type BaseType
		{
			get
			{
				return this.GetResolvedType().BaseType;
			}
		}

		public override bool ContainsGenericParameters
		{
			get
			{
				return this.GetResolvedType().ContainsGenericParameters;
			}
		}

		public override MethodBase DeclaringMethod
		{
			get
			{
				return this.GetResolvedType().DeclaringMethod;
			}
		}

		public override Type DeclaringType
		{
			get
			{
				return this.GetResolvedType().DeclaringType;
			}
		}

		public override string FullName
		{
			get
			{
				return this.GetResolvedType().FullName;
			}
		}

		public override System.Reflection.GenericParameterAttributes GenericParameterAttributes
		{
			get
			{
				return this.GetResolvedType().GenericParameterAttributes;
			}
		}

		public override int GenericParameterPosition
		{
			get
			{
				return this.GetResolvedType().GenericParameterPosition;
			}
		}

		public override Guid GUID
		{
			get
			{
				return this.GetResolvedType().GUID;
			}
		}

		public override bool IsEnum
		{
			get
			{
				return this.GetResolvedType().IsEnum;
			}
		}

		public override bool IsGenericParameter
		{
			get
			{
				return this.GetResolvedType().IsGenericParameter;
			}
		}

		public override bool IsGenericType
		{
			get
			{
				return this.GetResolvedType().IsGenericType;
			}
		}

		public override bool IsGenericTypeDefinition
		{
			get
			{
				return this.GetResolvedType().IsGenericTypeDefinition;
			}
		}

		public override bool IsSerializable
		{
			get
			{
				return this.GetResolvedType().IsSerializable;
			}
		}

		public override MemberTypes MemberType
		{
			get
			{
				return this.GetResolvedType().MemberType;
			}
		}

		public override int MetadataToken
		{
			get
			{
				return this.GetResolvedType().MetadataToken;
			}
		}

		public override System.Reflection.Module Module
		{
			get
			{
				return this.GetResolvedType().Module;
			}
		}

		public override string Name
		{
			get
			{
				return this.GetResolvedType().Name;
			}
		}

		public override string Namespace
		{
			get
			{
				return this.GetResolvedType().Namespace;
			}
		}

		public override Type ReflectedType
		{
			get
			{
				return this.GetResolvedType().ReflectedType;
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
				return this.GetResolvedType().StructLayoutAttribute;
			}
		}

		public ITypeUniverse TypeUniverse
		{
			get
			{
				return this.m_resolver.AssemblyResolver;
			}
		}

		public override Type UnderlyingSystemType
		{
			get
			{
				return this.GetResolvedType().UnderlyingSystemType;
			}
		}

		protected TypeProxy(MetadataOnlyModule resolver)
		{
			this.m_resolver = resolver;
		}

		public override bool Equals(object objOther)
		{
			Type type = objOther as Type;
			if (type == null)
			{
				return false;
			}
			return this.Equals(type);
		}

		public override bool Equals(Type t)
		{
			if (t == null)
			{
				return false;
			}
			return this.GetResolvedType().Equals(t);
		}

		public override Type[] FindInterfaces(TypeFilter filter, object filterCriteria)
		{
			return this.GetResolvedType().FindInterfaces(filter, filterCriteria);
		}

		public override MemberInfo[] FindMembers(MemberTypes memberType, BindingFlags bindingAttr, MemberFilter filter, object filterCriteria)
		{
			return this.GetResolvedType().FindMembers(memberType, bindingAttr, filter, filterCriteria);
		}

		public override int GetArrayRank()
		{
			return this.GetResolvedType().GetArrayRank();
		}

		protected override TypeAttributes GetAttributeFlagsImpl()
		{
			return this.GetResolvedType().Attributes;
		}

		protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			return this.GetResolvedType().GetConstructor(bindingAttr, binder, callConvention, types, modifiers);
		}

		public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
		{
			return this.GetResolvedType().GetConstructors(bindingAttr);
		}

		public override object[] GetCustomAttributes(bool inherit)
		{
			return this.GetResolvedType().GetCustomAttributes(inherit);
		}

		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			return this.GetResolvedType().GetCustomAttributes(attributeType, inherit);
		}

		public override IList<CustomAttributeData> GetCustomAttributesData()
		{
			return this.GetResolvedType().GetCustomAttributesData();
		}

		public override MemberInfo[] GetDefaultMembers()
		{
			return this.GetResolvedType().GetDefaultMembers();
		}

		public override Type GetElementType()
		{
			return this.GetResolvedType().GetElementType();
		}

		public override EventInfo GetEvent(string name, BindingFlags flags)
		{
			return this.GetResolvedType().GetEvent(name, flags);
		}

		public override EventInfo[] GetEvents(BindingFlags flags)
		{
			return this.GetResolvedType().GetEvents(flags);
		}

		public override FieldInfo GetField(string name, BindingFlags flags)
		{
			return this.GetResolvedType().GetField(name, flags);
		}

		public override FieldInfo[] GetFields(BindingFlags flags)
		{
			return this.GetResolvedType().GetFields(flags);
		}

		public override Type[] GetGenericArguments()
		{
			return this.GetResolvedType().GetGenericArguments();
		}

		public override Type[] GetGenericParameterConstraints()
		{
			return this.GetResolvedType().GetGenericParameterConstraints();
		}

		public override Type GetGenericTypeDefinition()
		{
			return this.GetResolvedType().GetGenericTypeDefinition();
		}

		public override int GetHashCode()
		{
			return this.GetResolvedType().GetHashCode();
		}

		public override Type GetInterface(string name, bool ignoreCase)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			return this.GetResolvedType().GetInterface(name, ignoreCase);
		}

		public override InterfaceMapping GetInterfaceMap(Type interfaceType)
		{
			return this.GetResolvedType().GetInterfaceMap(interfaceType);
		}

		public override Type[] GetInterfaces()
		{
			return this.GetResolvedType().GetInterfaces();
		}

		public override MemberInfo[] GetMember(string name, MemberTypes type, BindingFlags bindingAttr)
		{
			return this.GetResolvedType().GetMember(name, type, bindingAttr);
		}

		public override MemberInfo[] GetMember(string name, BindingFlags bindingAttr)
		{
			return this.GetResolvedType().GetMember(name, bindingAttr);
		}

		public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
		{
			return this.GetResolvedType().GetMembers(bindingAttr);
		}

		protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			if (types == null && modifiers == null && binder == null && callConvention == CallingConventions.Any)
			{
				return this.GetResolvedType().GetMethod(name, bindingAttr);
			}
			return this.GetResolvedType().GetMethod(name, bindingAttr, binder, callConvention, types, modifiers);
		}

		public override MethodInfo[] GetMethods(BindingFlags flags)
		{
			return this.GetResolvedType().GetMethods(flags);
		}

		public override Type GetNestedType(string name, BindingFlags bindingAttr)
		{
			return this.GetResolvedType().GetNestedType(name, bindingAttr);
		}

		public override Type[] GetNestedTypes(BindingFlags bindingAttr)
		{
			return this.GetResolvedType().GetNestedTypes(bindingAttr);
		}

		public override PropertyInfo[] GetProperties(BindingFlags flags)
		{
			return this.GetResolvedType().GetProperties(flags);
		}

		protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
		{
			if (types != null || modifiers != null)
			{
				return this.GetResolvedType().GetProperty(name, bindingAttr, binder, returnType, types, modifiers);
			}
			if (returnType != null)
			{
				return this.GetResolvedType().GetProperty(name, returnType);
			}
			return this.GetResolvedType().GetProperty(name, bindingAttr);
		}

		public virtual Type GetResolvedType()
		{
			if (this.m_cachedResolvedType == null)
			{
				this.m_cachedResolvedType = this.GetResolvedTypeWorker();
			}
			return this.m_cachedResolvedType;
		}

		protected abstract Type GetResolvedTypeWorker();

		protected override TypeCode GetTypeCodeImpl()
		{
			return Type.GetTypeCode(this.GetResolvedType());
		}

		protected override bool HasElementTypeImpl()
		{
			if (base.IsArray || base.IsByRef)
			{
				return true;
			}
			return base.IsPointer;
		}

		public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
		{
			return this.GetResolvedType().InvokeMember(name, invokeAttr, binder, target, args, modifiers, culture, namedParameters);
		}

		protected override bool IsArrayImpl()
		{
			return this.GetResolvedType().IsArray;
		}

		public override bool IsAssignableFrom(Type c)
		{
			return this.GetResolvedType().IsAssignableFrom(c);
		}

		protected override bool IsByRefImpl()
		{
			return this.GetResolvedType().IsByRef;
		}

		protected override bool IsCOMObjectImpl()
		{
			return this.GetResolvedType().IsCOMObject;
		}

		protected override bool IsContextfulImpl()
		{
			return this.GetResolvedType().IsContextful;
		}

		public override bool IsDefined(Type attributeType, bool inherit)
		{
			return this.GetResolvedType().IsDefined(attributeType, inherit);
		}

		public override bool IsInstanceOfType(object o)
		{
			return this.GetResolvedType().IsInstanceOfType(o);
		}

		protected override bool IsMarshalByRefImpl()
		{
			return this.GetResolvedType().IsMarshalByRef;
		}

		protected override bool IsPointerImpl()
		{
			return this.GetResolvedType().IsPointer;
		}

		protected override bool IsPrimitiveImpl()
		{
			return this.GetResolvedType().IsPrimitive;
		}

		public override bool IsSubclassOf(Type c)
		{
			return this.GetResolvedType().IsSubclassOf(c);
		}

		protected override bool IsValueTypeImpl()
		{
			return this.GetResolvedType().IsValueType;
		}

		public override Type MakeArrayType()
		{
			return this.GetResolvedType().MakeArrayType();
		}

		public override Type MakeArrayType(int rank)
		{
			return this.GetResolvedType().MakeArrayType(rank);
		}

		public override Type MakeByRefType()
		{
			return this.Resolver.Factory.CreateByRefType(this);
		}

		public override Type MakeGenericType(params Type[] args)
		{
			return new ProxyGenericType(this, args);
		}

		public override Type MakePointerType()
		{
			return this.Resolver.Factory.CreatePointerType(this);
		}

		public override string ToString()
		{
			return this.GetResolvedType().ToString();
		}
	}
}