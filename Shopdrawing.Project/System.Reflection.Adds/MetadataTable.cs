using System;

namespace System.Reflection.Adds
{
	internal enum MetadataTable
	{
		Module = 0,
		TypeRef = 1,
		TypeDef = 2,
		FieldDef = 4,
		MethodDef = 6,
		ParamDef = 8,
		InterfaceImpl = 9,
		MemberRef = 10,
		CustomAttribute = 12,
		Permission = 14,
		Signature = 17,
		Event = 20,
		Property = 23,
		ModuleRef = 26,
		TypeSpec = 27,
		Assembly = 32,
		AssemblyRef = 35,
		File = 38,
		ExportedType = 39,
		ManifestResource = 40,
		GenericPar = 42,
		MethodSpec = 43
	}
}