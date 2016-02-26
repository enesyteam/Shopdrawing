using Microsoft.Expression.Framework.Documents;
using System;
using System.Collections;
using System.Configuration.Assemblies;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.VisualStudio.Silverlight
{
	internal class AssemblyMetadataHelper
	{
		private static Guid CLSID_CorMetaDataDispenserRuntime;

		private static Guid IID_IMetaDataImport;

		private static int ENUM_TOKEN_BUF_SIZE;

		static AssemblyMetadataHelper()
		{
			AssemblyMetadataHelper.CLSID_CorMetaDataDispenserRuntime = new Guid("1EC2DE53-75CC-11d2-9775-00A0C9B4D50C");
			AssemblyMetadataHelper.IID_IMetaDataImport = typeof(AssemblyMetadataHelper.IMetaDataImport).GUID;
			AssemblyMetadataHelper.ENUM_TOKEN_BUF_SIZE = 4;
		}

		public AssemblyMetadataHelper()
		{
		}

		public static AssemblyNameVersion GetAssemblyNameVersion(AssemblyMetadataHelper.IMetaDataAssemblyImport assemblyImport)
		{
			IntPtr intPtr;
			uint num;
			uint num1;
			int num2;
			int num3;
			if (assemblyImport == null)
			{
				return null;
			}
			int assemblyFromScope = assemblyImport.GetAssemblyFromScope();
			AssemblyMetadataHelper.AssemblyMetadataInfo assemblyMetadataInfo = new AssemblyMetadataHelper.AssemblyMetadataInfo();
			assemblyImport.GetAssemblyProps(assemblyFromScope, out intPtr, out num, out num1, null, 0, out num2, ref assemblyMetadataInfo, out num3);
			if (num2 <= 1)
			{
				return null;
			}
			char[] chrArray = new char[num2 + 1];
			assemblyImport.GetAssemblyProps(assemblyFromScope, out intPtr, out num, out num1, chrArray, (int)chrArray.Length, out num2, ref assemblyMetadataInfo, out num3);
			string str = new string(chrArray, 0, num2 - 1);
			Version version = new Version((int)assemblyMetadataInfo.usMajorVersion, (int)assemblyMetadataInfo.usMinorVersion, (int)assemblyMetadataInfo.usBuildNumber, (int)assemblyMetadataInfo.usRevisionNumber);
			byte[] numArray = new byte[num];
			Marshal.Copy(intPtr, numArray, 0, (int)num);
			AssemblyName assemblyName = new AssemblyName()
			{
				HashAlgorithm = (AssemblyHashAlgorithm)num1
			};
			if ((num3 & 1) == 0)
			{
				assemblyName.SetPublicKeyToken(numArray);
			}
			else
			{
				assemblyName.SetPublicKey(numArray);
			}
			return new AssemblyNameVersion(str, version, assemblyName.GetPublicKeyToken());
		}

		public static AssemblyNameVersion[] GetAssemblyReferenceNameVersion(AssemblyMetadataHelper.IMetaDataAssemblyImport assemblyImport)
		{
			IntPtr intPtr;
			IntPtr intPtr1;
			int num;
			int num1;
			int num2;
			int num3;
			if (assemblyImport == null)
			{
				return null;
			}
			IntPtr zero = IntPtr.Zero;
			int[] numArray = new int[AssemblyMetadataHelper.ENUM_TOKEN_BUF_SIZE];
			int num4 = 0;
			ArrayList arrayLists = new ArrayList();
			try
			{
				do
				{
					assemblyImport.EnumAssemblyRefs(ref zero, numArray, (int)numArray.Length, out num4);
					for (int i = 0; i < num4; i++)
					{
						AssemblyMetadataHelper.AssemblyMetadataInfo assemblyMetadataInfo = new AssemblyMetadataHelper.AssemblyMetadataInfo();
						assemblyImport.GetAssemblyRefProps(numArray[i], out intPtr1, out num1, null, 0, out num2, ref assemblyMetadataInfo, out intPtr, out num, out num3);
						if (num2 > 1)
						{
							char[] chrArray = new char[num2 + 1];
							assemblyImport.GetAssemblyRefProps(numArray[i], out intPtr1, out num1, chrArray, (int)chrArray.Length, out num2, ref assemblyMetadataInfo, out intPtr, out num, out num3);
							byte[] numArray1 = new byte[num1];
							Marshal.Copy(intPtr1, numArray1, 0, num1);
							string str = new string(chrArray, 0, num2 - 1);
							Version version = new Version((int)assemblyMetadataInfo.usMajorVersion, (int)assemblyMetadataInfo.usMinorVersion, (int)assemblyMetadataInfo.usBuildNumber, (int)assemblyMetadataInfo.usRevisionNumber);
							arrayLists.Add(new AssemblyNameVersion(str, version, numArray1));
						}
					}
				}
				while (num4 > 0);
			}
			finally
			{
				if (zero != IntPtr.Zero)
				{
					assemblyImport.CloseEnum(zero);
				}
			}
			return (AssemblyNameVersion[])arrayLists.ToArray(typeof(AssemblyNameVersion));
		}

		public static AssemblyMetadataHelper.IMetaDataDispenserEx GetDispenser()
		{
			return Activator.CreateInstance(Type.GetTypeFromCLSID(AssemblyMetadataHelper.CLSID_CorMetaDataDispenserRuntime)) as AssemblyMetadataHelper.IMetaDataDispenserEx;
		}

		public static AssemblyMetadataHelper.IMetaDataAssemblyImport OpenScope(AssemblyMetadataHelper.IMetaDataDispenserEx dispenser, string path)
		{
			if (dispenser == null || path == null || !PathHelper.FileExists(path))
			{
				return null;
			}
			AssemblyMetadataHelper.IMetaDataImport metaDataImport = dispenser.OpenScope(path, 18, ref AssemblyMetadataHelper.IID_IMetaDataImport);
			AssemblyMetadataHelper.IMetaDataAssemblyImport metaDataAssemblyImport = metaDataImport as AssemblyMetadataHelper.IMetaDataAssemblyImport;
			if (metaDataImport != null && metaDataAssemblyImport == null)
			{
				Marshal.ReleaseComObject(metaDataImport);
			}
			return metaDataAssemblyImport;
		}

		public static void ReleaseAssemblyImport(AssemblyMetadataHelper.IMetaDataAssemblyImport assemblyImport)
		{
			if (assemblyImport != null)
			{
				Marshal.ReleaseComObject(assemblyImport);
			}
		}

		public static void ReleaseDispenser(AssemblyMetadataHelper.IMetaDataDispenserEx dispenser)
		{
			if (dispenser != null)
			{
				Marshal.ReleaseComObject(dispenser);
			}
		}

		[DllImport("mscoree.dll", CharSet=CharSet.None, ExactSpelling=true, PreserveSig=false)]
		private static extern void StrongNameTokenFromPublicKey(IntPtr publicKeyBlob, uint publicKeyBlobCount, ref IntPtr strongNameTokenArray, ref uint strongNameTokenCount);

		[ComVisible(true)]
		internal struct AssemblyMetadataInfo
		{
			public ushort usMajorVersion;

			public ushort usMinorVersion;

			public ushort usBuildNumber;

			public ushort usRevisionNumber;

			public string szLocale;

			private uint cbLocale;

			private IntPtr rProcessor;

			private uint ulProcessor;

			private IntPtr rOS;

			private uint ulOS;
		}

		private enum CorElementType
		{
			ELEMENT_TYPE_END = 0,
			ELEMENT_TYPE_VOID = 1,
			ELEMENT_TYPE_BOOLEAN = 2,
			ELEMENT_TYPE_CHAR = 3,
			ELEMENT_TYPE_I1 = 4,
			ELEMENT_TYPE_U1 = 5,
			ELEMENT_TYPE_I2 = 6,
			ELEMENT_TYPE_U2 = 7,
			ELEMENT_TYPE_I4 = 8,
			ELEMENT_TYPE_U4 = 9,
			ELEMENT_TYPE_I8 = 10,
			ELEMENT_TYPE_U8 = 11,
			ELEMENT_TYPE_R4 = 12,
			ELEMENT_TYPE_R8 = 13,
			ELEMENT_TYPE_STRING = 14,
			ELEMENT_TYPE_PTR = 15,
			ELEMENT_TYPE_BYREF = 16,
			ELEMENT_TYPE_VALUETYPE = 17,
			ELEMENT_TYPE_CLASS = 18,
			ELEMENT_TYPE_VAR = 19,
			ELEMENT_TYPE_ARRAY = 20,
			ELEMENT_TYPE_GENERICINST = 21,
			ELEMENT_TYPE_TYPEDBYREF = 22,
			ELEMENT_TYPE_I = 24,
			ELEMENT_TYPE_U = 25,
			ELEMENT_TYPE_FNPTR = 27,
			ELEMENT_TYPE_OBJECT = 28,
			ELEMENT_TYPE_SZARRAY = 29,
			ELEMENT_TYPE_MVAR = 30,
			ELEMENT_TYPE_CMOD_REQD = 31,
			ELEMENT_TYPE_CMOD_OPT = 32,
			ELEMENT_TYPE_INTERNAL = 33,
			ELEMENT_TYPE_MAX = 34,
			ELEMENT_TYPE_MODIFIER = 64,
			ELEMENT_TYPE_SENTINEL = 65,
			ELEMENT_TYPE_PINNED = 69
		}

		private enum CorOpenFlags : uint
		{
			ofRead = 0,
			ofReadWriteMask = 1,
			ofWrite = 1,
			ofCopyMemory = 2,
			ofCacheImage = 4,
			ofManifestMetadata = 8,
			ofReadOnly = 16,
			ofTakeOwnership = 32,
			ofNoTypeLib = 128,
			ofReserved1 = 256,
			ofReserved2 = 512,
			ofReserved = 4294967104
		}

		private enum CorSerializationType
		{
			SERIALIZATION_TYPE_BOOLEAN = 2,
			SERIALIZATION_TYPE_CHAR = 3,
			SERIALIZATION_TYPE_I1 = 4,
			SERIALIZATION_TYPE_U1 = 5,
			SERIALIZATION_TYPE_I2 = 6,
			SERIALIZATION_TYPE_U2 = 7,
			SERIALIZATION_TYPE_I4 = 8,
			SERIALIZATION_TYPE_U4 = 9,
			SERIALIZATION_TYPE_I8 = 10,
			SERIALIZATION_TYPE_U8 = 11,
			SERIALIZATION_TYPE_R4 = 12,
			SERIALIZATION_TYPE_R8 = 13,
			SERIALIZATION_TYPE_STRING = 14,
			SERIALIZATION_TYPE_SZARRAY = 29,
			SERIALIZATION_TYPE_TYPE = 80,
			SERIALIZATION_TYPE_TAGGED_OBJECT = 81,
			SERIALIZATION_TYPE_FIELD = 83,
			SERIALIZATION_TYPE_PROPERTY = 84,
			SERIALIZATION_TYPE_ENUM = 85
		}

		private enum CorTokenType
		{
			mdtModule = 0,
			mdtTypeRef = 16777216,
			mdtTypeDef = 33554432,
			mdtFieldDef = 67108864,
			mdtMethodDef = 100663296,
			mdtParamDef = 134217728,
			mdtInterfaceImpl = 150994944,
			mdtMemberRef = 167772160,
			mdtCustomAttribute = 201326592,
			mdtPermission = 234881024,
			mdtSignature = 285212672,
			mdtEvent = 335544320,
			mdtProperty = 385875968,
			mdtModuleRef = 436207616,
			mdtTypeSpec = 452984832,
			mdtAssembly = 536870912,
			mdtAssemblyRef = 587202560,
			mdtFile = 637534208,
			mdtExportedType = 654311424,
			mdtManifestResource = 671088640,
			mdtGenericParam = 704643072,
			mdtBaseType = 1912602624
		}

		private enum CorTypeAttr
		{
			tdNotPublic = 0,
			tdPublic = 1,
			tdNestedPublic = 2,
			tdVisibilityMask = 7,
			tdInterface = 32,
			tdAbstract = 128
		}

		[Guid("EE62470B-E94B-424e-9B7C-2F00C9249F93")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		internal interface IMetaDataAssemblyImport
		{
			void CloseEnum([In] IntPtr phEnum);

			void EnumAssemblyRefs([In][Out] ref IntPtr phEnum, [Out] int[] asmRefs, int fileRefCount, out int iFetched);

			void EnumExportedTypes();

			void EnumFiles([In][Out] ref IntPtr phEnum, [Out] int[] fileRefs, int fileRefCount, out int iFetched);

			void EnumManifestResources();

			void FindAssembliesByName();

			void FindExportedTypeByName();

			void FindManifestResourceByName();

			int GetAssemblyFromScope();

			void GetAssemblyProps(int mdAsm, out IntPtr pPublicKeyPtr, out uint ucbPublicKeyBytes, out uint uHashAlg, char[] strName, int cchNameIn, out int cchNameRequired, ref AssemblyMetadataHelper.AssemblyMetadataInfo amdInfo, out int dwFlags);

			void GetAssemblyRefProps(int mdAsm, out IntPtr pPublicKeyPtr, out int ucbPublicKeyPtr, char[] strName, int cchNameIn, out int cchNameRequired, ref AssemblyMetadataHelper.AssemblyMetadataInfo amdInfo, out IntPtr ppbHashValue, out int pcbHashLength, out int dwFlags);

			void GetExportedTypeProps();

			void GetFileProps(int mdFile, StringBuilder strName, int cchNameIn, out int cchNameRequired, IntPtr bHashData, IntPtr cchHashBytes, IntPtr dwFileFlags);

			void GetManifestResourceProps();
		}

		[Guid("31BCFCE2-DAFB-11D2-9F81-00C04F79A0A3")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		public interface IMetaDataDispenserEx
		{
			object DefineScope();

			AssemblyMetadataHelper.IMetaDataImport OpenScope([In] string szScope, int dwOpenFlags, ref Guid riid);

			object OpenScopeOnMemory();
		}

		[Guid("fce5efa0-8bba-4f8e-a036-8f2022b08466")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		public interface IMetaDataImport
		{
			void CloseEnum(IntPtr hEnum);

			uint CountEnum(IntPtr hEnum);

			int EnumCustomAttributes([In][Out] ref IntPtr pEnum, int tkScope, int tkType, [Out] int[] tkAttributes, int nTokens);

			void EnumGenericParams([In][Out] ref IntPtr hEnum, int mdToken, [Out] int[] genparms, int numparms, out uint numGenericParams);

			int EnumInterfaceImpls([In][Out] ref IntPtr hEnum, int mdTypeDef, [Out] int[] pMdInterfaceTokens, int nTokens);

			int EnumTypeDefs([In][Out] ref IntPtr hEnum, [Out] int[] pMdTokens, int nTokens);

			void FindTypeDefByName(string name, int enclosingToken, out int mdTypeToken);

			int GetCustomAttributeByName(int mdTokenObj, string szName, out IntPtr ppData, out uint pDataSize);

			uint GetCustomAttributeProps(int mdToken, out int pTkObj, out int ptkType, out IntPtr ppData);

			int GetInterfaceImplProps(int mdInterfaceImpl, out int mdTypeClass);

			void GetMemberRefProps(int mdMemberRef, out int mdType, StringBuilder szMember, int cchMember, out int pchMember, out IntPtr ppvSigBlob, out int pbSig);

			void GetMethodProps(int mdMethodDef, out int mdTypeDefClass, StringBuilder szMethod, int cchMethod, out int pchMethod, out int pdwAttrFlags, out IntPtr ppvSigBlob, out int pcbSigBlob, out int pulCodeRVA, out int pdwImplFlags);

			int GetModuleFromScope();

			int GetModuleRefProps(int mdModuleRef, [In][Out] StringBuilder sb, int capacity, out int pNameLen);

			Guid GetScopeProps(StringBuilder szName, int cchName, out int pchName);

			int GetTypeDefProps(int mdTypeDef, StringBuilder defName, int nMaxChars, out int nameSize, out int dwFlags);

			int GetTypeRefProps(int mdTypeDef, out int mdScope, StringBuilder defName, int nMaxChars);

			void GetTypeSpecFromToken(int mdToken, out IntPtr ppvSigBlob, out int corsigSize);

			bool IsValidToken(int mdToken);

			void ResetEnum(IntPtr hEnum, uint pos);

			int ResolveTypeRef(int mdTypeDef, ref Guid riid, out AssemblyMetadataHelper.IMetaDataImport ppScope);

			void Stub_EnumEvents();

			void Stub_EnumFields();

			void Stub_EnumFieldsWithName();

			void Stub_EnumGenericParamConstraints();

			void Stub_EnumMemberRefs();

			void Stub_EnumMembers();

			void Stub_EnumMembersWithName();

			void Stub_EnumMethodImpls();

			void Stub_EnumMethods();

			void Stub_EnumMethodSemantics();

			void Stub_EnumMethodSpecs();

			void Stub_EnumMethodsWithName();

			void Stub_EnumModuleRefs();

			void Stub_EnumParams();

			void Stub_EnumPermissionSets();

			void Stub_EnumProperties();

			void Stub_EnumSignatures();

			void Stub_EnumTypeRefs();

			void Stub_EnumTypeSpecs();

			void Stub_EnumUnresolvedMethods();

			void Stub_EnumUserStrings();

			void Stub_FindField();

			void Stub_FindMember();

			void Stub_FindMemberRef();

			void Stub_FindMethod();

			void Stub_FindTypeRef();

			void Stub_GetClassLayout();

			void Stub_GetEventProps();

			void Stub_GetFieldMarshal();

			void Stub_GetFieldProps();

			void Stub_GetGenericParamConstraintProps();

			void Stub_GetGenericParamProps();

			void Stub_GetMemberProps();

			void Stub_GetMethodSemantics();

			void Stub_GetMethodSpecProps();

			void Stub_GetNameFromToken();

			void Stub_GetNativeCallConvFromSig();

			void Stub_GetNestedClassProps();

			void Stub_GetParamForMethodIndex();

			void Stub_GetParamProps();

			void Stub_GetPEKind();

			void Stub_GetPermissionSetProps();

			void Stub_GetPinvokeMap();

			void Stub_GetPropertyProps();

			void Stub_GetRVA();

			void Stub_GetSigFromToken();

			void Stub_GetUserString();

			void Stub_GetVersionString();

			void Stub_IsGlobal();
		}
	}
}