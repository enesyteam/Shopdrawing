using System;
using System.Runtime.CompilerServices;

namespace Microsoft.MetadataReader
{
	internal class MethodSignatureDescriptor
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

		public TypeSignatureDescriptor[] Parameters
		{
			get;
			set;
		}

		public TypeSignatureDescriptor ReturnParameter
		{
			get;
			set;
		}

		public MethodSignatureDescriptor()
		{
		}
	}
}