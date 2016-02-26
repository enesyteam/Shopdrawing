using System;
using System.Reflection;
using System.Reflection.Adds;

namespace Microsoft.MetadataReader
{
	internal class ArrayFabricatedSetMethodInfo : ArrayFabricatedMethodInfo
	{
		public override string Name
		{
			get
			{
				return "Set";
			}
		}

		public override Type ReturnType
		{
			get
			{
				return base.Universe.GetBuiltInType(System.Reflection.Adds.CorElementType.Void);
			}
		}

		public ArrayFabricatedSetMethodInfo(Type arrayType) : base(arrayType)
		{
		}

		public override ParameterInfo[] GetParameters()
		{
			ParameterInfo[] parameterInfoArray = base.MakeParameterHelper(1);
			int rank = base.Rank;
			Type elementType = base.GetElementType();
			parameterInfoArray[rank] = base.MakeParameterInfo(elementType, rank);
			return parameterInfoArray;
		}
	}
}