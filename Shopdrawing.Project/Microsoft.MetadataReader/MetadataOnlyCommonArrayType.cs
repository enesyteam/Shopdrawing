using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Reflection.Adds;

namespace Microsoft.MetadataReader
{
	internal class MetadataOnlyCommonArrayType : MetadataOnlyCommonType
	{
		private readonly MetadataOnlyCommonType m_elementType;

		private readonly Type m_baseType;

		public override System.Reflection.Assembly Assembly
		{
			get
			{
				return this.m_elementType.Assembly;
			}
		}

		public override Type BaseType
		{
			get
			{
				return this.m_baseType;
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
				throw new InvalidOperationException();
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
				throw new InvalidOperationException();
			}
		}

		public override string Namespace
		{
			get
			{
				return this.m_elementType.Namespace;
			}
		}

		internal override MetadataOnlyModule Resolver
		{
			get
			{
				return this.m_elementType.Resolver;
			}
		}

		public override Type UnderlyingSystemType
		{
			get
			{
				return this;
			}
		}

		public MetadataOnlyCommonArrayType(MetadataOnlyCommonType elementType)
		{
			this.m_baseType = Helpers.Universe(elementType).GetTypeXFromName("System.Array");
			this.m_elementType = elementType;
		}

		public override bool Equals(Type o)
		{
			throw new InvalidOperationException();
		}

		protected override TypeAttributes GetAttributeFlagsImpl()
		{
			return 257 | 8192;
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

		internal override IEnumerable<MethodBase> GetDeclaredConstructors()
		{
			return this.Resolver.Policy.GetExtraArrayConstructors(this);
		}

		internal override IEnumerable<MethodBase> GetDeclaredMethods()
		{
			return this.Resolver.Policy.GetExtraArrayMethods(this);
		}

		public override Type GetElementType()
		{
			return this.m_elementType;
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
			return this.m_elementType.GetGenericArguments();
		}

		public override Type GetGenericTypeDefinition()
		{
			throw new InvalidOperationException();
		}

		public override int GetHashCode()
		{
			return this.m_elementType.GetHashCode();
		}

		public override Type GetInterface(string name, bool ignoreCase)
		{
			return MetadataOnlyModule.GetInterfaceHelper(this.GetInterfaces(), name, ignoreCase);
		}

		public override Type[] GetInterfaces()
		{
			List<Type> types = new List<Type>(this.m_baseType.GetInterfaces());
			types.AddRange(this.Resolver.Policy.GetExtraArrayInterfaces(this.m_elementType));
			return types.ToArray();
		}

		public override Type GetNestedType(string name, BindingFlags bindingAttr)
		{
			return null;
		}

		public override Type[] GetNestedTypes(BindingFlags bindingAttr)
		{
			return new Type[0];
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

		protected override bool IsArrayImpl()
		{
			return true;
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
	}
}