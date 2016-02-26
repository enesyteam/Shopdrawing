using System;
using System.Reflection;
using System.Reflection.Adds;

namespace Microsoft.MetadataReader
{
	internal class ArrayFabricatedConstructorInfo : MetadataOnlyConstructorInfo
	{
		private readonly int m_numParams;

		public ArrayFabricatedConstructorInfo(Type arrayType, int numParams) : base(ArrayFabricatedConstructorInfo.MakeMethodInfo(arrayType, numParams))
		{
			this.m_numParams = numParams;
		}

		public override bool Equals(object obj)
		{
			ArrayFabricatedConstructorInfo arrayFabricatedConstructorInfo = obj as ArrayFabricatedConstructorInfo;
			if (arrayFabricatedConstructorInfo == null)
			{
				return false;
			}
			if (!this.DeclaringType.Equals(arrayFabricatedConstructorInfo.DeclaringType))
			{
				return false;
			}
			return this.ToString().Equals(arrayFabricatedConstructorInfo.ToString());
		}

		public override object[] GetCustomAttributes(bool inherit)
		{
			return new object[0];
		}

		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			return new object[0];
		}

		public override int GetHashCode()
		{
			return this.DeclaringType.GetHashCode() + this.m_numParams;
		}

		private static MethodInfo MakeMethodInfo(Type arrayType, int numParams)
		{
			return new ArrayFabricatedConstructorInfo.Adapter(arrayType, numParams);
		}

		private class Adapter : ArrayFabricatedMethodInfo
		{
			private readonly int m_numParams;

			public override MethodAttributes Attributes
			{
				get
				{
					return MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Public | MethodAttributes.RTSpecialName;
				}
			}

			public override string Name
			{
				get
				{
					return ".ctor";
				}
			}

			public override Type ReturnType
			{
				get
				{
					return base.Universe.GetBuiltInType(System.Reflection.Adds.CorElementType.Void);
				}
			}

			public Adapter(Type arrayType, int numParams) : base(arrayType)
			{
				this.m_numParams = numParams;
			}

			public override ParameterInfo[] GetParameters()
			{
				Type builtInType = base.Universe.GetBuiltInType(System.Reflection.Adds.CorElementType.Int);
				ParameterInfo[] parameterInfoArray = new ParameterInfo[this.m_numParams];
				for (int i = 0; i < this.m_numParams; i++)
				{
					parameterInfoArray[i] = base.MakeParameterInfo(builtInType, i);
				}
				return parameterInfoArray;
			}
		}
	}
}