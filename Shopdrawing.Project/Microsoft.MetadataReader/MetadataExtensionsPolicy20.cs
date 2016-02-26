using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Adds;

namespace Microsoft.MetadataReader
{
	internal class MetadataExtensionsPolicy20 : IMetadataExtensionsPolicy
	{
		protected ITypeUniverse m_universe;

		public MetadataExtensionsPolicy20(ITypeUniverse u)
		{
			this.m_universe = u;
		}

		public virtual ConstructorInfo[] GetExtraArrayConstructors(Type arrayType)
		{
			int arrayRank = arrayType.GetArrayRank();
			ConstructorInfo[] arrayFabricatedConstructorInfo = new ConstructorInfo[] { new ArrayFabricatedConstructorInfo(arrayType, arrayRank), new ArrayFabricatedConstructorInfo(arrayType, arrayRank * 2) };
			return arrayFabricatedConstructorInfo;
		}

		public virtual Type[] GetExtraArrayInterfaces(Type elementType)
		{
			if (elementType.IsPointer)
			{
				return Type.EmptyTypes;
			}
			Type[] typeArray = new Type[] { elementType };
			Type[] typeArray1 = new Type[] { this.m_universe.GetTypeXFromName("System.Collections.Generic.IList`1").MakeGenericType(typeArray), this.m_universe.GetTypeXFromName("System.Collections.Generic.ICollection`1").MakeGenericType(typeArray), this.m_universe.GetTypeXFromName("System.Collections.Generic.IEnumerable`1").MakeGenericType(typeArray) };
			return typeArray1;
		}

		public virtual MethodInfo[] GetExtraArrayMethods(Type arrayType)
		{
			MethodInfo[] arrayFabricatedGetMethodInfo = new MethodInfo[] { new ArrayFabricatedGetMethodInfo(arrayType), new ArrayFabricatedSetMethodInfo(arrayType), new ArrayFabricatedAddressMethodInfo(arrayType) };
			return arrayFabricatedGetMethodInfo;
		}

		public virtual ParameterInfo GetFakeParameterInfo(MemberInfo member, Type paramType, int position)
		{
			return new SimpleParameterInfo(member, paramType, position);
		}

		public virtual IEnumerable<CustomAttributeData> GetPseudoCustomAttributes(MetadataOnlyModule module, Token token)
		{
			List<CustomAttributeData> customAttributeDatas = new List<CustomAttributeData>();
			customAttributeDatas.AddRange(PseudoCustomAttributes.GetTypeForwardedToAttributes(module, token));
			CustomAttributeData serializableAttribute = PseudoCustomAttributes.GetSerializableAttribute(module, token);
			if (serializableAttribute != null)
			{
				customAttributeDatas.Add(serializableAttribute);
			}
			return customAttributeDatas;
		}

		public virtual Type TryTypeForwardResolution(MetadataOnlyAssembly assembly, string fullname, bool ignoreCase)
		{
			Type type;
			using (IEnumerator<UnresolvedTypeName> enumerator = PseudoCustomAttributes.GetRawTypeForwardedToAttributes(assembly).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					UnresolvedTypeName current = enumerator.Current;
					if (!Utility.Compare(current.TypeName, fullname, ignoreCase))
					{
						continue;
					}
					type = current.ConvertToType(assembly.TypeUniverse, assembly.ManifestModuleInternal);
					return type;
				}
				return null;
			}
			return type;
		}
	}
}