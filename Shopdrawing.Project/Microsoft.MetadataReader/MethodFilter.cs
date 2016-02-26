using System;
using System.Runtime.CompilerServices;

namespace Microsoft.MetadataReader
{
	internal class MethodFilter
	{
		public CorCallingConvention CallingConvention
		{
			get;
			set;
		}

		public int GenericParameterCount
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public int ParameterCount
		{
			get;
			set;
		}

		public MethodFilter(string name, int genericParameterCount, int parameterCount, CorCallingConvention callingConvention)
		{
			this.Name = name;
			this.GenericParameterCount = genericParameterCount;
			this.ParameterCount = parameterCount;
			this.CallingConvention = callingConvention;
		}
	}
}