using System;
using System.Reflection;
using System.Reflection.Adds;

namespace Microsoft.MetadataReader
{
	internal class DefaultFactory : IReflectionFactory
	{
		public DefaultFactory()
		{
		}

		public virtual MetadataOnlyCommonType CreateArrayType(MetadataOnlyCommonType elementType, int rank)
		{
			return new MetadataOnlyArrayType(elementType, rank);
		}

		public virtual MetadataOnlyCommonType CreateByRefType(MetadataOnlyCommonType type)
		{
			return new MetadataOnlyModifiedType(type, "&");
		}

		public virtual MetadataOnlyConstructorInfo CreateConstructorInfo(MethodBase method)
		{
			return new MetadataOnlyConstructorInfo(method);
		}

		public virtual MetadataOnlyEventInfo CreateEventInfo(MetadataOnlyModule resolver, Token eventToken, Type[] typeArgs, Type[] methodArgs)
		{
			return new MetadataOnlyEventInfo(resolver, eventToken, typeArgs, methodArgs);
		}

		public virtual MetadataOnlyFieldInfo CreateField(MetadataOnlyModule resolver, Token fieldDefToken, Type[] typeArgs, Type[] methodArgs)
		{
			return new MetadataOnlyFieldInfo(resolver, fieldDefToken, typeArgs, methodArgs);
		}

		public virtual MetadataOnlyCommonType CreateGenericType(MetadataOnlyModule scope, Token tokenTypeDef, Type[] typeArgs)
		{
			return new MetadataOnlyTypeDef(scope, tokenTypeDef, typeArgs);
		}

		public virtual MetadataOnlyMethodInfo CreateMethodInfo(MetadataOnlyMethodInfo method)
		{
			return new MetadataOnlyMethodInfo(method);
		}

		public virtual MethodBase CreateMethodOrConstructor(MetadataOnlyModule resolver, Token methodDef, Type[] typeArgs, Type[] methodArgs)
		{
			MetadataOnlyMethodInfo metadataOnlyMethodInfo = new MetadataOnlyMethodInfo(resolver, methodDef, typeArgs, methodArgs);
			if (DefaultFactory.IsRawConstructor(metadataOnlyMethodInfo))
			{
				return this.CreateConstructorInfo(metadataOnlyMethodInfo);
			}
			return this.CreateMethodInfo(metadataOnlyMethodInfo);
		}

		public virtual MetadataOnlyCommonType CreatePointerType(MetadataOnlyCommonType type)
		{
			return new MetadataOnlyModifiedType(type, "*");
		}

		public virtual MetadataOnlyPropertyInfo CreatePropertyInfo(MetadataOnlyModule resolver, Token propToken, Type[] typeArgs, Type[] methodArgs)
		{
			return new MetadataOnlyPropertyInfo(resolver, propToken, typeArgs, methodArgs);
		}

		public virtual MetadataOnlyCommonType CreateSimpleType(MetadataOnlyModule scope, Token tokenTypeDef)
		{
			return new MetadataOnlyTypeDef(scope, tokenTypeDef);
		}

		public virtual Type CreateTypeRef(MetadataOnlyModule scope, Token tokenTypeRef)
		{
			return new MetadataOnlyTypeReference(scope, tokenTypeRef);
		}

		public virtual Type CreateTypeSpec(MetadataOnlyModule scope, Token tokenTypeSpec, Type[] typeArgs, Type[] methodArgs)
		{
			return new TypeSpec(scope, tokenTypeSpec, typeArgs, methodArgs);
		}

		public virtual MetadataOnlyTypeVariable CreateTypeVariable(MetadataOnlyModule resolver, Token typeVariableToken)
		{
			return new MetadataOnlyTypeVariable(resolver, typeVariableToken);
		}

		public virtual MetadataOnlyCommonType CreateVectorType(MetadataOnlyCommonType elementType)
		{
			return new MetadataOnlyVectorType(elementType);
		}

		private static bool IsRawConstructor(MethodInfo m)
		{
			if ((m.Attributes & MethodAttributes.RTSpecialName) == MethodAttributes.PrivateScope)
			{
				return false;
			}
			string name = m.Name;
			if (name.Equals(ConstructorInfo.ConstructorName, StringComparison.Ordinal))
			{
				return true;
			}
			if (name.Equals(ConstructorInfo.TypeConstructorName, StringComparison.Ordinal))
			{
				return true;
			}
			return false;
		}

		public virtual bool TryCreateMethodBody(MetadataOnlyMethodInfo method, ref MethodBody body)
		{
			return false;
		}
	}
}