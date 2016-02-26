using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Reflection.Adds;

namespace Microsoft.MetadataReader
{
	internal abstract class ArrayFabricatedMethodInfo : MethodInfo
	{
		private Type m_arrayType;

		public override MethodAttributes Attributes
		{
			get
			{
				return MethodAttributes.Public;
			}
		}

		public override CallingConventions CallingConvention
		{
			get
			{
				return CallingConventions.Standard | CallingConventions.HasThis;
			}
		}

		public override bool ContainsGenericParameters
		{
			get
			{
				return this.GetElementType().IsGenericParameter;
			}
		}

		public override Type DeclaringType
		{
			get
			{
				return this.m_arrayType;
			}
		}

		public override bool IsGenericMethodDefinition
		{
			get
			{
				return false;
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
				return new Token(System.Reflection.Adds.TokenType.MethodDef, 0);
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
				return this.DeclaringType.Module;
			}
		}

		protected int Rank
		{
			get
			{
				return this.m_arrayType.GetArrayRank();
			}
		}

		public override Type ReflectedType
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public override ParameterInfo ReturnParameter
		{
			get
			{
				return this.MakeParameterInfo(this.ReturnType, -1);
			}
		}

		public override ICustomAttributeProvider ReturnTypeCustomAttributes
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		protected ITypeUniverse Universe
		{
			get
			{
				return Helpers.Universe(this.m_arrayType);
			}
		}

		protected ArrayFabricatedMethodInfo(Type arrayType)
		{
			this.m_arrayType = arrayType;
		}

		public override bool Equals(object obj)
		{
			ArrayFabricatedMethodInfo arrayFabricatedMethodInfo = obj as ArrayFabricatedMethodInfo;
			if (arrayFabricatedMethodInfo == null)
			{
				return false;
			}
			if (!this.DeclaringType.Equals(arrayFabricatedMethodInfo.DeclaringType))
			{
				return false;
			}
			return this.Name.Equals(arrayFabricatedMethodInfo.Name);
		}

		public override MethodInfo GetBaseDefinition()
		{
			return this;
		}

		public override object[] GetCustomAttributes(bool inherit)
		{
			return new object[0];
		}

		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			return new object[0];
		}

		public override IList<CustomAttributeData> GetCustomAttributesData()
		{
			return new CustomAttributeData[0];
		}

		protected Type GetElementType()
		{
			return this.m_arrayType.GetElementType();
		}

		public override Type[] GetGenericArguments()
		{
			return Type.EmptyTypes;
		}

		public override int GetHashCode()
		{
			return this.DeclaringType.GetHashCode() + this.Name.GetHashCode();
		}

		public override MethodBody GetMethodBody()
		{
			return null;
		}

		public override MethodImplAttributes GetMethodImplementationFlags()
		{
			return MethodImplAttributes.IL;
		}

		public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
		{
			throw new NotSupportedException();
		}

		public override bool IsDefined(Type attributeType, bool inherit)
		{
			throw new NotImplementedException();
		}

		public override MethodInfo MakeGenericMethod(params Type[] types)
		{
			throw new InvalidOperationException();
		}

		protected ParameterInfo[] MakeParameterHelper(int extra)
		{
			int rank = this.Rank;
			Type builtInType = this.Universe.GetBuiltInType(System.Reflection.Adds.CorElementType.Int);
			ParameterInfo[] parameterInfoArray = new ParameterInfo[rank + extra];
			for (int i = 0; i < rank; i++)
			{
				parameterInfoArray[i] = this.MakeParameterInfo(builtInType, i);
			}
			return parameterInfoArray;
		}

		protected ParameterInfo MakeParameterInfo(Type t, int position)
		{
			return new SimpleParameterInfo(this, t, position);
		}

		public override string ToString()
		{
			return MetadataOnlyMethodInfo.CommonToString(this);
		}
	}
}