using System;

namespace Microsoft.MetadataReader
{
	[Flags]
	internal enum CorTypeAttr
	{
		tdAnsiClass = 0,
		tdAutoLayout = 0,
		tdClass = 0,
		tdNotPublic = 0,
		tdPublic = 1,
		tdNestedPublic = 2,
		tdNestedPrivate = 3,
		tdNestedFamily = 4,
		tdNestedAssembly = 5,
		tdNestedFamANDAssem = 6,
		tdNestedFamORAssem = 7,
		tdVisibilityMask = 7,
		tdSequentialLayout = 8,
		tdExplicitLayout = 16,
		tdLayoutMask = 24,
		tdClassSemanticsMask = 32,
		tdInterface = 32,
		tdAbstract = 128,
		tdSealed = 256,
		tdSpecialName = 1024,
		tdRTSpecialName = 2048,
		tdImport = 4096,
		tdSerializable = 8192,
		tdUnicodeClass = 65536,
		tdAutoClass = 131072,
		tdCustomFormatClass = 196608,
		tdStringFormatMask = 196608,
		tdHasSecurity = 262144,
		tdReservedMask = 264192,
		tdBeforeFieldInit = 1048576,
		tdForwarder = 2097152,
		tdCustomFormatMask = 12582912
	}
}