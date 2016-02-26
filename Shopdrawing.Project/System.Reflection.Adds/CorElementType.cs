using System;

namespace System.Reflection.Adds
{
	internal enum CorElementType
	{
		End = 0,
		Void = 1,
		Bool = 2,
		Char = 3,
		SByte = 4,
		Byte = 5,
		Short = 6,
		UShort = 7,
		Int = 8,
		UInt = 9,
		Long = 10,
		ULong = 11,
		Float = 12,
		Double = 13,
		String = 14,
		Pointer = 15,
		Byref = 16,
		ValueType = 17,
		Class = 18,
		TypeVar = 19,
		Array = 20,
		GenericInstantiation = 21,
		TypedByRef = 22,
		IntPtr = 24,
		UIntPtr = 25,
		FnPtr = 27,
		Object = 28,
		SzArray = 29,
		MethodVar = 30,
		CModReqd = 31,
		CModOpt = 32,
		Internal = 33,
		Max = 34,
		Modifier = 64,
		Sentinel = 65,
		Pinned = 69,
		Type = 80,
		Enum = 85
	}
}