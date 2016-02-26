using System;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignModel.Code
{
	[CLSCompliant(true)]
	public interface ICodeAidProvider
	{
		IEnumerable<ICodeAidTypeInfo> GetAttachedPropertyTypesInClrNamespace(string assembly, string namespaceName);

		IEnumerable<ICodeAidTypeInfo> GetAttachedPropertyTypesInXmlNamespace(string uri);

		IEnumerable<ICodeAidMarkupExtensionInfo> GetMarkupExtensions();

		IEnumerable<ICodeAidAssemblyInfo> GetReferenceAssemblies();

		IEnumerable<ICodeAidMemberInfo> GetRelativeSources();

		IEnumerable<ICodeAidMemberInfo> GetSystemBrushes();

		IEnumerable<ICodeAidMemberInfo> GetSystemColors();

		ICodeAidTypeInfo GetTypeByName(string uri, string typeName);

		ICodeAidTypeInfo GetTypeByName(string assembly, string namespaceName, string typeName);

		IEnumerable<ICodeAidTypeInfo> GetTypesInClrNamespace(string assembly, string namespaceName);

		IEnumerable<ICodeAidTypeInfo> GetTypesInXmlNamespace(string uri);
	}
}