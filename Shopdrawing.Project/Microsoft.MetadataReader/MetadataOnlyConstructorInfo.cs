using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace Microsoft.MetadataReader
{
	internal class MetadataOnlyConstructorInfo : ConstructorInfo
	{
		private readonly MethodBase m_method;

		public override MethodAttributes Attributes
		{
			get
			{
				return this.m_method.Attributes;
			}
		}

		public override CallingConventions CallingConvention
		{
			get
			{
				return this.m_method.CallingConvention;
			}
		}

		public override Type DeclaringType
		{
			get
			{
				return this.m_method.DeclaringType;
			}
		}

		public override bool IsGenericMethodDefinition
		{
			get
			{
				return this.m_method.IsGenericMethodDefinition;
			}
		}

		public override MemberTypes MemberType
		{
			get
			{
				return MemberTypes.Constructor;
			}
		}

		public override int MetadataToken
		{
			get
			{
				return this.m_method.MetadataToken;
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
				return this.m_method.Module;
			}
		}

		public override string Name
		{
			get
			{
				return this.m_method.Name;
			}
		}

		public override Type ReflectedType
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public MetadataOnlyConstructorInfo(MethodBase method)
		{
			this.m_method = method;
		}

		public override bool Equals(object obj)
		{
			MetadataOnlyConstructorInfo metadataOnlyConstructorInfo = obj as MetadataOnlyConstructorInfo;
			if (metadataOnlyConstructorInfo == null)
			{
				return false;
			}
			return this.m_method.Equals(metadataOnlyConstructorInfo.m_method);
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
			return this.m_method.GetCustomAttributesData();
		}

		public override int GetHashCode()
		{
			return this.m_method.GetHashCode();
		}

		public override MethodBody GetMethodBody()
		{
			return this.m_method.GetMethodBody();
		}

		public override MethodImplAttributes GetMethodImplementationFlags()
		{
			return this.m_method.GetMethodImplementationFlags();
		}

		public override ParameterInfo[] GetParameters()
		{
			return this.m_method.GetParameters();
		}

		public override object Invoke(BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
		{
			throw new NotSupportedException();
		}

		public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
		{
			throw new NotSupportedException();
		}

		public override bool IsDefined(Type attributeType, bool inherit)
		{
			throw new NotSupportedException();
		}

		public override string ToString()
		{
			return this.m_method.ToString();
		}
	}
}