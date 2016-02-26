using System;
using System.Reflection;
using System.Reflection.Adds;

namespace Microsoft.MetadataReader
{
	internal interface IReflectionFactory
	{
		MetadataOnlyCommonType CreateArrayType(MetadataOnlyCommonType elementType, int rank);

		MetadataOnlyCommonType CreateByRefType(MetadataOnlyCommonType type);

		MetadataOnlyEventInfo CreateEventInfo(MetadataOnlyModule resolver, Token eventToken, Type[] typeArgs, Type[] methodArgs);

		MetadataOnlyFieldInfo CreateField(MetadataOnlyModule resolver, Token fieldDefToken, Type[] typeArgs, Type[] methodArgs);

		MetadataOnlyCommonType CreateGenericType(MetadataOnlyModule scope, Token tokenTypeDef, Type[] typeArgs);

		MethodBase CreateMethodOrConstructor(MetadataOnlyModule resolver, Token methodToken, Type[] typeArgs, Type[] methodArgs);

		MetadataOnlyCommonType CreatePointerType(MetadataOnlyCommonType type);

		MetadataOnlyPropertyInfo CreatePropertyInfo(MetadataOnlyModule resolver, Token propToken, Type[] typeArgs, Type[] methodArgs);

		MetadataOnlyCommonType CreateSimpleType(MetadataOnlyModule scope, Token tokenTypeDef);

		Type CreateTypeRef(MetadataOnlyModule scope, Token tokenTypeRef);

		Type CreateTypeSpec(MetadataOnlyModule scope, Token tokenTypeRef, Type[] typeArgs, Type[] methodArgs);

		MetadataOnlyTypeVariable CreateTypeVariable(MetadataOnlyModule resolver, Token typeVariableToken);

		MetadataOnlyCommonType CreateVectorType(MetadataOnlyCommonType elementType);

		bool TryCreateMethodBody(MetadataOnlyMethodInfo method, ref MethodBody body);
	}
}