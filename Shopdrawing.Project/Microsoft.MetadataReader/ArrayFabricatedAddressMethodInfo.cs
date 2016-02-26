using System;
using System.Reflection;

namespace Microsoft.MetadataReader
{
	internal class ArrayFabricatedAddressMethodInfo : ArrayFabricatedMethodInfo
	{
		public override string Name
		{
			get
			{
				return "Address";
			}
		}

		public override Type ReturnType
		{
			get
			{
				return base.GetElementType().MakeByRefType();
			}
		}

		public ArrayFabricatedAddressMethodInfo(Type arrayType) : base(arrayType)
		{
		}

		public override ParameterInfo[] GetParameters()
		{
			return base.MakeParameterHelper(0);
		}
	}
}