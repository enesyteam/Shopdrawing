using System;
using System.Reflection;
using System.Reflection.Adds;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.MetadataReader
{
	[Guid("EE62470B-E94B-424e-9B7C-2F00C9249F93")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IMetadataAssemblyImport
	{
		int CloseEnum(IntPtr hEnum);

		int EnumAssemblyRefs(ref HCORENUM phEnum, out Token assemblyRefs, int cMax, out int cTokens);

		void EnumExportedTypes(ref HCORENUM phEnum, out int rExportedTypes, int cMax, out uint cTokens);

		void EnumFiles(ref HCORENUM phEnum, out int files, int cMax, out int cTokens);

		void EnumManifestResources(ref HCORENUM phEnum, out int rManifestResources, int cMax, out int cTokens);

		void FindExportedTypeByName_();

		int FindManifestResourceByName(string szName, out int ptkManifestResource);

		int GetAssemblyFromScope(out int assemblyToken);

		void GetAssemblyProps([In] Token assemblyToken, out EmbeddedBlobPointer pPublicKey, out int cbPublicKey, out int hashAlgId, [Out] StringBuilder szName, [In] int cchName, out int pchName, [In][Out] ref AssemblyMetaData pMetaData, out AssemblyNameFlags flags);

		void GetAssemblyRefProps([In] Token token, out EmbeddedBlobPointer pPublicKey, out int cbPublicKey, [Out] StringBuilder szName, [In] int cchName, out int pchName, [In][Out] ref AssemblyMetaData pMetaData, out UnusedIntPtr ppbHashValue, out uint pcbHashValue, out AssemblyNameFlags dwAssemblyRefFlags);

		void GetExportedTypeProps(int mdct, [Out] StringBuilder szName, int cchName, out int pchName, out int ptkImplementation, out int ptkTypeDef, out CorTypeAttr pdwExportedTypeFlags);

		void GetFileProps([In] int token, [Out] StringBuilder szName, [In] int cchName, out int pchName, out UnusedIntPtr ppbHashValue, out uint pcbHashValue, out CorFileFlags dwFileFlags);

		void GetManifestResourceProps([In] int mdmr, [Out] StringBuilder szName, [In] int cchName, out int pchName, [ComAliasName("mdToken*")] out int ptkImplementation, [ComAliasName("DWORD*")] out uint pdwOffset, out CorManifestResourceFlags pdwResourceFlags);
	}
}