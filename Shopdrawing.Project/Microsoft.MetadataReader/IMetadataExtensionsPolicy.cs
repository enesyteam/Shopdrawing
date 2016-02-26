using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Adds;

namespace Microsoft.MetadataReader
{
	internal interface IMetadataExtensionsPolicy
	{
		ConstructorInfo[] GetExtraArrayConstructors(Type arrayType);

		Type[] GetExtraArrayInterfaces(Type elementType);

		MethodInfo[] GetExtraArrayMethods(Type arrayType);

		ParameterInfo GetFakeParameterInfo(MemberInfo member, Type paramType, int position);

		IEnumerable<CustomAttributeData> GetPseudoCustomAttributes(MetadataOnlyModule module, Token token);

		Type TryTypeForwardResolution(MetadataOnlyAssembly assembly, string fullname, bool ignoreCase);
	}
}