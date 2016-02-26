using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Adds;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.MetadataReader
{
	[DebuggerDisplay("\\{Name = {Name} FullName = {FullName}\\}")]
	internal abstract class MetadataOnlyCommonType : Type
	{
		public override string AssemblyQualifiedName
		{
			get
			{
				string fullName = this.FullName;
				if (fullName == null)
				{
					return null;
				}
				System.Reflection.Assembly assembly = this.Assembly;
				return System.Reflection.Assembly.CreateQualifiedName(assembly.GetName().ToString(), fullName);
			}
		}

		public override bool ContainsGenericParameters
		{
			get
			{
				if (base.HasElementType)
				{
					return this.GetRootElementType().ContainsGenericParameters;
				}
				if (this.IsGenericParameter)
				{
					return true;
				}
				if (!this.IsGenericType)
				{
					return false;
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

		public override MethodBase DeclaringMethod
		{
			get
			{
				throw new InvalidOperationException(MetadataStringTable.ValidOnGenericParameterTypeOnly);
			}
		}

		internal virtual Microsoft.MetadataReader.GenericContext GenericContext
		{
			get
			{
				return new Microsoft.MetadataReader.GenericContext(this.GetGenericArguments(), null);
			}
		}

		public override bool IsEnum
		{
			get
			{
				return false;
			}
		}

		public override bool IsGenericParameter
		{
			get
			{
				return false;
			}
		}

		public override bool IsGenericType
		{
			get
			{
				return false;
			}
		}

		public override bool IsSerializable
		{
			get
			{
				if ((this.GetAttributeFlagsImpl() & TypeAttributes.Serializable) != TypeAttributes.NotPublic)
				{
					return true;
				}
				return this.QuickSerializationCastCheck();
			}
		}

		public override System.Reflection.Module Module
		{
			get
			{
				return this.Resolver;
			}
		}

		public override Type ReflectedType
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		internal abstract MetadataOnlyModule Resolver
		{
			get;
		}

		public override System.Runtime.InteropServices.StructLayoutAttribute StructLayoutAttribute
		{
			get
			{
				return null;
			}
		}

		protected MetadataOnlyCommonType()
		{
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

		public override int GetArrayRank()
		{
			throw new ArgumentException(MetadataStringTable.OperationValidOnArrayTypeOnly);
		}

		protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			return MetadataOnlyModule.GetConstructorOnType(this, bindingAttr, binder, callConvention, types, modifiers);
		}

		public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
		{
			return MetadataOnlyModule.GetConstructorsOnType(this, bindingAttr);
		}

		internal virtual IEnumerable<MethodBase> GetDeclaredConstructors()
		{
			return new MethodInfo[0];
		}

		internal virtual IEnumerable<MethodBase> GetDeclaredMethods()
		{
			return new MethodInfo[0];
		}

		internal virtual IEnumerable<PropertyInfo> GetDeclaredProperties()
		{
			return new PropertyInfo[0];
		}

		public override MemberInfo[] GetDefaultMembers()
		{
			Type typeXFromName = this.Resolver.AssemblyResolver.GetTypeXFromName("System.Reflection.DefaultMemberAttribute");
			if (typeXFromName == null)
			{
				return new MemberInfo[0];
			}
			CustomAttributeData item = null;
			for (Type i = this; i != null; i = i.BaseType)
			{
				IList<CustomAttributeData> customAttributesData = i.GetCustomAttributesData();
				int num = 0;
				while (num < customAttributesData.Count)
				{
					if (!customAttributesData[num].Constructor.DeclaringType.Equals(typeXFromName))
					{
						num++;
					}
					else
					{
						item = customAttributesData[num];
						break;
					}
				}
				if (item != null)
				{
					break;
				}
			}
			if (item == null)
			{
				return new MemberInfo[0];
			}
			string value = item.ConstructorArguments[0].Value as string;
			return base.GetMember(value) ?? new MemberInfo[0];
		}

		public override int GetHashCode()
		{
			return this.MetadataToken;
		}

		public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
		{
			return MetadataOnlyTypeDef.GetMembersHelper(this, bindingAttr);
		}

		protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			return MetadataOnlyModule.GetMethodImplHelper(this, name, bindingAttr, binder, callConvention, types, modifiers);
		}

		public override MethodInfo[] GetMethods(BindingFlags flags)
		{
			return MetadataOnlyModule.GetMethodsOnType(this, flags);
		}

		public override PropertyInfo[] GetProperties(BindingFlags flags)
		{
			return MetadataOnlyModule.GetPropertiesOnType(this, flags);
		}

		protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
		{
			return MetadataOnlyTypeDef.GetPropertyImplHelper(this, name, bindingAttr, binder, returnType, types, modifiers);
		}

		private new Type GetRootElementType()
		{
			Type elementType = this;
			while (elementType.HasElementType)
			{
				elementType = elementType.GetElementType();
			}
			return elementType;
		}

		protected override bool IsArrayImpl()
		{
			return false;
		}

		protected override bool IsByRefImpl()
		{
			return false;
		}

		protected override bool IsCOMObjectImpl()
		{
			throw new NotImplementedException();
		}

		protected override bool IsContextfulImpl()
		{
			Type typeXFromName = this.Resolver.AssemblyResolver.GetTypeXFromName("System.ContextBoundObject");
			if (typeXFromName == null)
			{
				return false;
			}
			return typeXFromName.IsAssignableFrom(this);
		}

		public override bool IsInstanceOfType(object o)
		{
			return false;
		}

		protected override bool IsMarshalByRefImpl()
		{
			Type typeXFromName = this.Resolver.AssemblyResolver.GetTypeXFromName("System.MarshalByRefObject");
			if (typeXFromName == null)
			{
				return false;
			}
			return typeXFromName.IsAssignableFrom(this);
		}

		protected override bool IsPointerImpl()
		{
			return false;
		}

		protected override bool IsPrimitiveImpl()
		{
			return false;
		}

		public override bool IsSubclassOf(Type c)
		{
			Type baseType = this;
			if (baseType.Equals(c))
			{
				return false;
			}
			while (baseType != null)
			{
				if (baseType.Equals(c))
				{
					return true;
				}
				baseType = baseType.BaseType;
			}
			return false;
		}

		public override Type MakeArrayType()
		{
			return this.Resolver.Factory.CreateVectorType(this);
		}

		public override Type MakeArrayType(int rank)
		{
			return this.MakeArrayTypeHelper(rank);
		}

		private Type MakeArrayTypeHelper(int rank)
		{
			if (rank <= 0)
			{
				throw new IndexOutOfRangeException();
			}
			return this.Resolver.Factory.CreateArrayType(this, rank);
		}

		public override Type MakeByRefType()
		{
			return this.Resolver.Factory.CreateByRefType(this);
		}

		public override Type MakeGenericType(params Type[] argTypes)
		{
			throw new InvalidOperationException();
		}

		public override Type MakePointerType()
		{
			return this.Resolver.Factory.CreatePointerType(this);
		}

		private bool QuickSerializationCastCheck()
		{
			ITypeUniverse typeUniverse = Helpers.Universe(this);
			Type typeXFromName = typeUniverse.GetTypeXFromName("System.Enum");
			Type type = typeUniverse.GetTypeXFromName("System.Delegate");
			for (Type i = this.UnderlyingSystemType; i != null; i = i.BaseType)
			{
				if (i.Equals(typeXFromName) || i.Equals(type))
				{
					return true;
				}
			}
			return false;
		}

		internal static string TypeSigToString(Type pThis)
		{
			StringBuilder stringBuilder = new StringBuilder();
			MetadataOnlyCommonType.TypeSigToString(pThis, stringBuilder);
			return stringBuilder.ToString();
		}

		internal static void TypeSigToString(Type pThis, StringBuilder sb)
		{
			Type elementType = pThis;
			while (elementType.HasElementType)
			{
				elementType = elementType.GetElementType();
			}
			if (elementType.IsNested)
			{
				sb.Append(pThis.Name);
				return;
			}
			string str = pThis.ToString();
			if (elementType.IsPrimitive || elementType.FullName == "System.Void")
			{
				str = str.Substring("System.".Length);
			}
			sb.Append(str);
		}
	}
}