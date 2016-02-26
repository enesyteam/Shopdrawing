using System;
using System.Reflection;
using System.Reflection.Adds;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.MetadataReader
{
	[Guid("FCE5EFA0-8BBA-4f8e-A036-8F2022B08466")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IMetadataImport2 : IMetadataImport
	{
		void CloseEnum(IntPtr hEnum);

		void CountEnum(HCORENUM hEnum, [ComAliasName("ULONG*")] out int pulCount);

		void EnumCustomAttributes(ref HCORENUM phEnum, int tk, int tkType, [ComAliasName("mdCustomAttribute*")] out Token mdCustomAttribute, uint cMax, [ComAliasName("ULONG*")] out uint pcTokens);

		void EnumEvents(ref HCORENUM phEnum, int td, [ComAliasName("mdEvent*")] out int mdFieldDef, int cMax, [ComAliasName("ULONG*")] out uint pcEvents);

		void EnumFields(ref HCORENUM phEnum, int cl, [ComAliasName("mdFieldDef*")] out int mdFieldDef, int cMax, [ComAliasName("ULONG*")] out uint pcTokens);

		void EnumFieldsWithName_();

		void EnumGenericParamConstraints(ref HCORENUM hEnum, int tk, [ComAliasName("mdGenericParamConstraint*")] out int rGenericParamConstraints, uint cMax, [ComAliasName("ULONG*")] out uint pcGenericParams);

		void EnumGenericParams(ref HCORENUM hEnum, int tk, [ComAliasName("mdGenericParam*")] out int rGenericParams, uint cMax, [ComAliasName("ULONG*")] out uint pcGenericParams);

		void EnumInterfaceImpls(ref HCORENUM phEnum, int td, out int rImpls, int cMax, ref int pcImpls);

		void EnumMemberRefs_();

		void EnumMembers_();

		void EnumMembersWithName_();

		void EnumMethodImpls(ref HCORENUM hEnum, Token typeDef, out Token methodBody, out Token methodDecl, int cMax, out int cTokens);

		void EnumMethods(ref HCORENUM phEnum, int cl, [ComAliasName("mdMethodDef*")] out int mdMethodDef, int cMax, [ComAliasName("ULONG*")] out int pcTokens);

		void EnumMethodSemantics_();

		void EnumMethodSpecs_();

		void EnumMethodsWithName(ref HCORENUM phEnum, int cl, [In] string szName, [ComAliasName("mdMethodDef*")] out int mdMethodDef, int cMax, [ComAliasName("ULONG*")] out int pcTokens);

		void EnumModuleRefs(ref HCORENUM phEnum, [ComAliasName("mdModuleRef*")] out int mdModuleRef, int cMax, [ComAliasName("ULONG*")] out uint pcModuleRefs);

		int EnumParams(ref HCORENUM phEnum, int mdMethodDef, int[] rParams, int cMax, [ComAliasName("ULONG*")] out uint pcTokens);

		void EnumPermissionSets_();

		void EnumProperties(ref HCORENUM phEnum, int td, [ComAliasName("mdProperty*")] out int mdFieldDef, int cMax, [ComAliasName("ULONG*")] out uint pcTokens);

		void EnumSignatures_();

		void EnumTypeDefs(ref HCORENUM phEnum, [ComAliasName("mdTypeDef*")] out int rTypeDefs, uint cMax, [ComAliasName("ULONG*")] out uint pcTypeDefs);

		void EnumTypeRefs_();

		void EnumTypeSpecs_();

		void EnumUnresolvedMethods_();

		void EnumUserStrings_();

		void FindField([In] int typeDef, [In] string szName, [In] byte[] pvSigBlob, [In] int cbSigBlob, out int fieldDef);

		void FindMember([In] int typeDefToken, [In] string szName, [In] byte[] pvSigBlob, [In] int cbSigBlob, out int memberDefToken);

		void FindMemberRef([In] int typeRef, [In] string szName, [In] byte[] pvSigBlob, [In] int cbSigBlob, out int result);

		void FindMethod([In] int typeDef, [In] string szName, [In] EmbeddedBlobPointer pvSigBlob, [In] int cbSigBlob, out int methodDef);

		void FindTypeDefByName([In] string szTypeDef, [In] int tkEnclosingClass, [ComAliasName("mdTypeDef*")] out int token);

		void FindTypeRef([In] int tkResolutionScope, [In] string szName, out int typeRef);

		uint GetClassLayout(int typeDef, out uint dwPackSize, UnusedIntPtr zeroPtr, uint zeroCount, UnusedIntPtr zeroPtr2, ref uint ulClassSize);

		int GetCustomAttributeByName(int tkObj, string szName, out EmbeddedBlobPointer ppData, out uint pcbData);

		void GetCustomAttributeProps([In] Token cv, out Token tkObj, out Token tkType, out EmbeddedBlobPointer blob, out int cbSize);

		void GetEventProps(int ev, [ComAliasName("mdTypeDef*")] out int pClass, [Out] StringBuilder szEvent, int cchEvent, [ComAliasName("ULONG*")] out int pchEvent, [ComAliasName("DWORD*")] out int pdwEventFlags, [ComAliasName("mdToken*")] out int ptkEventType, [ComAliasName("mdMethodDef*")] out int pmdAddOn, [ComAliasName("mdMethodDef*")] out int pmdRemoveOn, [ComAliasName("mdMethodDef*")] out int pmdFire, [ComAliasName("mdMethodDef*")] out int rmdOtherMethod, uint cMax, [ComAliasName("ULONG*")] out uint pcOtherMethod);

		void GetFieldMarshal_();

		void GetFieldProps(int mb, [ComAliasName("mdTypeDef*")] out int mdTypeDef, [Out] StringBuilder szField, int cchField, [ComAliasName("ULONG*")] out int pchField, [ComAliasName("DWORD*")] out FieldAttributes pdwAttr, [ComAliasName("PCCOR_SIGNATURE*")] out EmbeddedBlobPointer ppvSigBlob, [ComAliasName("ULONG*")] out int pcbSigBlob, [ComAliasName("DWORD*")] out int pdwCPlusTypeFlab, [ComAliasName("UVCP_CONSTANT*")] out IntPtr ppValue, [ComAliasName("ULONG*")] out int pcchValue);

		void GetGenericParamConstraintProps(int gpc, [ComAliasName("mdGenericParam*")] out int ptGenericParam, [ComAliasName("mdToken*")] out int ptkConstraintType);

		void GetGenericParamProps(int gp, [ComAliasName("ULONG*")] out uint pulParamSeq, [ComAliasName("DWORD*")] out int pdwParamFlags, [ComAliasName("mdToken*")] out int ptOwner, [ComAliasName("mdToken*")] out int ptkKind, [Out] StringBuilder wzName, ulong cchName, [ComAliasName("ULONG*")] out ulong pchName);

		void GetInterfaceImplProps(int iiImpl, out int pClass, out int ptkIface);

		void GetMemberProps_();

		void GetMemberRefProps([In] Token mr, [ComAliasName("mdMemberRef*")] out Token ptk, [Out] StringBuilder szMember, [In] int cchMember, [ComAliasName("ULONG*")] out uint pchMember, [ComAliasName("PCCOR_SIGNATURE*")] out EmbeddedBlobPointer ppvSigBlob, [ComAliasName("ULONG*")] out uint pbSig);

		void GetMethodProps([In] uint md, [ComAliasName("mdTypeDef*")] out int pClass, [Out] StringBuilder szMethod, [In] int cchMethod, [ComAliasName("ULONG*")] out uint pchMethod, [ComAliasName("DWORD*")] out MethodAttributes pdwAttr, [ComAliasName("PCCOR_SIGNATURE*")] out EmbeddedBlobPointer ppvSigBlob, [ComAliasName("ULONG*")] out uint pcbSigBlob, [ComAliasName("ULONG*")] out uint pulCodeRVA, [ComAliasName("DWORD*")] out uint pdwImplFlags);

		void GetMethodSemantics_();

		void GetMethodSpecProps([ComAliasName("mdMethodSpec")] Token mi, [ComAliasName("mdToken*")] out Token tkParent, [ComAliasName("PCCOR_SIGNATURE*")] out EmbeddedBlobPointer ppvSigBlob, [ComAliasName("ULONG*")] out int pcbSigBlob);

		void GetModuleFromScope(out int mdModule);

		void GetModuleRefProps(int mur, [Out] StringBuilder szName, int cchName, [ComAliasName("ULONG*")] out int pchName);

		void GetNameFromToken_();

		void GetNativeCallConvFromSig_();

		void GetNestedClassProps(int tdNestedClass, [ComAliasName("mdTypeDef*")] out int tdEnclosingClass);

		void GetParamForMethodIndex_();

		void GetParamProps(int tk, [ComAliasName("mdMethodDef*")] out int pmd, [ComAliasName("ULONG*")] out uint pulSequence, [Out] StringBuilder szName, uint cchName, [ComAliasName("ULONG*")] out uint pchName, [ComAliasName("DWORD*")] out uint pdwAttr, [ComAliasName("DWORD*")] out uint pdwCPlusTypeFlag, [ComAliasName("UVCP_CONSTANT*")] out UnusedIntPtr ppValue, [ComAliasName("ULONG*")] out uint pcchValue);

		void GetPEKind(out PortableExecutableKinds dwPEKind, out ImageFileMachine pdwMachine);

		void GetPermissionSetProps_();

		void GetPinvokeMap_();

		void GetPropertyProps(Token prop, [ComAliasName("mdTypeDef*")] out Token pClass, [Out] StringBuilder szProperty, int cchProperty, [ComAliasName("ULONG*")] out int pchProperty, [ComAliasName("DWORD*")] out PropertyAttributes pdwPropFlags, [ComAliasName("PCCOR_SIGNATURE*")] out EmbeddedBlobPointer ppvSig, [ComAliasName("ULONG*")] out int pbSig, [ComAliasName("DWORD*")] out int pdwCPlusTypeFlag, [ComAliasName("UVCP_CONSTANT*")] out UnusedIntPtr ppDefaultValue, [ComAliasName("ULONG*")] out int pcchDefaultValue, [ComAliasName("mdMethodDef*")] out Token pmdSetter, [ComAliasName("mdMethodDef*")] out Token pmdGetter, [ComAliasName("mdMethodDef*")] out Token rmdOtherMethod, uint cMax, [ComAliasName("ULONG*")] out uint pcOtherMethod);

		void GetRVA(int token, out uint rva, out uint flags);

		void GetScopeProps([Out] StringBuilder szName, [In] int cchName, [ComAliasName("ULONG*")] out int pchName, out Guid mvid);

		void GetSigFromToken(int token, out EmbeddedBlobPointer pSig, out int cbSig);

		void GetTypeDefProps([In] int td, [Out] StringBuilder szTypeDef, [In] int cchTypeDef, [ComAliasName("ULONG*")] out int pchTypeDef, out TypeAttributes pdwTypeDefFlags, [ComAliasName("mdToken*")] out int ptkExtends);

		void GetTypeRefProps(int tr, [ComAliasName("mdToken*")] out int ptkResolutionScope, [Out] StringBuilder szName, [In] int cchName, [ComAliasName("ULONG*")] out int pchName);

		int GetTypeSpecFromToken(Token typeSpec, out EmbeddedBlobPointer pSig, out int cbSig);

		void GetUserString([In] int stk, [Out] char[] szString, [In] int cchString, [ComAliasName("ULONG*")] out int pchString);

		void GetVersionString([Out] StringBuilder szName, [In] int cchName, out int pchName);

		void IsGlobal_();

		bool IsValidToken([In] uint tk);

		void ResetEnum(HCORENUM hEnum, int ulPos);

		void ResolveTypeRef_();
	}
}