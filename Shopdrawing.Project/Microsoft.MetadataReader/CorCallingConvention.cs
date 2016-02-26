using System;

namespace Microsoft.MetadataReader
{
	internal enum CorCallingConvention
	{
		Default = 0,
		VarArg = 5,
		Field = 6,
		LocalSig = 7,
		Property = 8,
		Unmanaged = 9,
		GenericInst = 10,
		NativeVarArg = 11,
		Mask = 15,
		Generic = 16,
		HasThis = 32,
		ExplicitThis = 64
	}
}