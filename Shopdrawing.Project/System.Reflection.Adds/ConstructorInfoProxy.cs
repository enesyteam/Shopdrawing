using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace System.Reflection.Adds
{
	internal abstract class ConstructorInfoProxy : ConstructorInfo
	{
		private ConstructorInfo m_cachedResolved;

		public override MethodAttributes Attributes
		{
			get
			{
				return this.GetResolvedConstructor().Attributes;
			}
		}

		public override CallingConventions CallingConvention
		{
			get
			{
				return this.GetResolvedConstructor().CallingConvention;
			}
		}

		public override bool ContainsGenericParameters
		{
			get
			{
				return this.GetResolvedConstructor().ContainsGenericParameters;
			}
		}

		public override Type DeclaringType
		{
			get
			{
				return this.GetResolvedConstructor().DeclaringType;
			}
		}

		public override bool IsGenericMethodDefinition
		{
			get
			{
				return this.GetResolvedConstructor().IsGenericMethodDefinition;
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
				return this.GetResolvedConstructor().MetadataToken;
			}
		}

		public override RuntimeMethodHandle MethodHandle
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override System.Reflection.Module Module
		{
			get
			{
				return this.GetResolvedConstructor().Module;
			}
		}

		public override string Name
		{
			get
			{
				return this.GetResolvedConstructor().Name;
			}
		}

		public override Type ReflectedType
		{
			get
			{
				return this.GetResolvedConstructor().ReflectedType;
			}
		}

		protected ConstructorInfoProxy()
		{
		}

		public override object[] GetCustomAttributes(bool inherit)
		{
			return this.GetResolvedConstructor().GetCustomAttributes(inherit);
		}

		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			return this.GetResolvedConstructor().GetCustomAttributes(attributeType, inherit);
		}

		public override IList<CustomAttributeData> GetCustomAttributesData()
		{
			return this.GetResolvedConstructor().GetCustomAttributesData();
		}

		public override Type[] GetGenericArguments()
		{
			return this.GetResolvedConstructor().GetGenericArguments();
		}

		public override MethodBody GetMethodBody()
		{
			return this.GetResolvedConstructor().GetMethodBody();
		}

		public override MethodImplAttributes GetMethodImplementationFlags()
		{
			return this.GetResolvedConstructor().GetMethodImplementationFlags();
		}

		public override ParameterInfo[] GetParameters()
		{
			return this.GetResolvedConstructor().GetParameters();
		}

		public ConstructorInfo GetResolvedConstructor()
		{
			if (this.m_cachedResolved == null)
			{
				this.m_cachedResolved = this.GetResolvedWorker();
			}
			return this.m_cachedResolved;
		}

		protected abstract ConstructorInfo GetResolvedWorker();

		public override object Invoke(BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
		{
			return this.GetResolvedConstructor().Invoke(invokeAttr, binder, parameters, culture);
		}

		public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
		{
			return this.GetResolvedConstructor().Invoke(obj, invokeAttr, binder, parameters, culture);
		}

		public override bool IsDefined(Type attributeType, bool inherit)
		{
			return this.GetResolvedConstructor().IsDefined(attributeType, inherit);
		}
	}
}