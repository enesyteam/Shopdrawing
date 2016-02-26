using System;
using System.Reflection;

namespace Microsoft.MetadataReader
{
	internal class ArrayFabricatedGetMethodInfo : ArrayFabricatedMethodInfo
	{
		public override string Name
		{
			get
			{
				return "Get";
			}
		}

		public override Type ReturnType
		{
			get
			{
				return base.GetElementType();
			}
		}

		public ArrayFabricatedGetMethodInfo(Type arrayType) : base(arrayType)
		{
		}

		public override ParameterInfo[] GetParameters()
		{
			return base.MakeParameterHelper(0);
		}
	}
}