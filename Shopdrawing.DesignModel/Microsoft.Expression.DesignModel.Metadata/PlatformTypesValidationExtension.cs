using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.DesignModel.Metadata
{
	public class PlatformTypesValidationExtension : IDesignTypeGeneratorExtension
	{
		private static ITypeId[] CollectionTypes;

		private static ITypeId[] ObjectTypes;

		private List<Type> collectionTypes;

		private Type baseCollectionType;

		private Type replacementCollectionType;

		private List<Type> objectTypes;

		private Type objectType;

		private IPlatformMetadata platformMetadata;

		static PlatformTypesValidationExtension()
		{
			ITypeId[] collection = new ITypeId[] { PlatformTypes.ICollection, PlatformTypes.ICollectionT, PlatformTypes.IEnumerable, PlatformTypes.IEnumerableT, PlatformTypes.IList, PlatformTypes.IListT };
			PlatformTypesValidationExtension.CollectionTypes = collection;
			ITypeId[] uIElement = new ITypeId[] { PlatformTypes.UIElement, PlatformTypes.Delegate };
			PlatformTypesValidationExtension.ObjectTypes = uIElement;
		}

		public PlatformTypesValidationExtension(IPlatformMetadata platformMetadata)
		{
			this.platformMetadata = platformMetadata;
			this.collectionTypes = new List<Type>((int)PlatformTypesValidationExtension.CollectionTypes.Length);
			for (int i = 0; i < (int)PlatformTypesValidationExtension.CollectionTypes.Length; i++)
			{
				Type runtimeType = platformMetadata.ResolveType(PlatformTypesValidationExtension.CollectionTypes[i]).RuntimeType;
				if (runtimeType != null)
				{
					this.collectionTypes.Add(runtimeType);
				}
			}
			this.objectTypes = new List<Type>((int)PlatformTypesValidationExtension.ObjectTypes.Length);
			for (int j = 0; j < (int)PlatformTypesValidationExtension.ObjectTypes.Length; j++)
			{
				Type type = platformMetadata.ResolveType(PlatformTypesValidationExtension.ObjectTypes[j]).RuntimeType;
				if (type != null)
				{
					this.objectTypes.Add(type);
				}
			}
			this.replacementCollectionType = this.platformMetadata.ResolveType(PlatformTypes.ObservableCollection).RuntimeType;
			this.baseCollectionType = this.platformMetadata.ResolveType(PlatformTypes.IList).RuntimeType;
			this.objectType = this.platformMetadata.ResolveType(PlatformTypes.Object).RuntimeType;
		}

		public Type GetDesignType(IDesignTypeGeneratorContext context, Type runtimeType)
		{
			if (runtimeType == null)
			{
				return runtimeType;
			}
			if (PlatformTypes.IsExpressionInteractiveType(runtimeType))
			{
				return runtimeType;
			}
			if (runtimeType.IsPrimitive || runtimeType.IsGenericParameter)
			{
				return runtimeType;
			}
			if (!this.IsCorePlatformType(runtimeType))
			{
				return null;
			}
			Type replacementForPlatformType = this.GetReplacementForPlatformType(runtimeType);
			if (replacementForPlatformType != null)
			{
				return replacementForPlatformType;
			}
			if (runtimeType.IsInterface)
			{
				return null;
			}
			if (!runtimeType.IsGenericType)
			{
				if (runtimeType.IsArray && this.GetDesignType(context, DesignTypeGenerator.GetArrayItemType(runtimeType)) == null)
				{
					return null;
				}
				return runtimeType;
			}
			Type[] genericArguments = runtimeType.GetGenericArguments();
			for (int i = 0; i < (int)genericArguments.Length; i++)
			{
				if (this.GetDesignType(context, genericArguments[i]) == null)
				{
					return null;
				}
			}
			return runtimeType;
		}

		private Type GetReplacementCollectionType(Type type)
		{
			if (type.IsArray)
			{
				if (type.GetArrayRank() == 1)
				{
					Type arrayItemType = DesignTypeGenerator.GetArrayItemType(type);
					if (arrayItemType != null)
					{
						return this.replacementCollectionType.MakeGenericType(new Type[] { arrayItemType });
					}
				}
				return null;
			}
			Type genericTypeDefinition = type;
			if (type.IsGenericType && !type.IsGenericTypeDefinition)
			{
				genericTypeDefinition = type.GetGenericTypeDefinition();
			}
			if (!this.collectionTypes.Contains(genericTypeDefinition))
			{
				if (!this.baseCollectionType.IsAssignableFrom(genericTypeDefinition))
				{
					return null;
				}
				ConstructorInfo constructor = genericTypeDefinition.GetConstructor(Type.EmptyTypes);
				if (constructor != null && constructor.IsPublic)
				{
					return null;
				}
			}
			Type type1 = this.replacementCollectionType;
			if (!type.IsGenericType)
			{
				Type type2 = this.replacementCollectionType;
				Type[] typeArray = new Type[] { this.objectType };
				type1 = type2.MakeGenericType(typeArray);
			}
			return type1;
		}

		private Type GetReplacementForPlatformType(Type type)
		{
			if (this.objectTypes.Find((Type t) => t.IsAssignableFrom(type)) != null)
			{
				return this.objectType;
			}
			return this.GetReplacementCollectionType(type);
		}

		public Type GetReplacementType(Type type)
		{
			if (!type.IsArray && !this.IsCorePlatformType(type))
			{
				return null;
			}
			return this.GetReplacementForPlatformType(type);
		}

		private bool IsComplexTypeWithCollectionInterfaces(Type originalType)
		{
			Type baseType = originalType.BaseType;
			if (baseType == null || baseType == this.objectType)
			{
				return false;
			}
			Type genericTypeDefinition = baseType;
			if (baseType.IsGenericType && !baseType.IsGenericTypeDefinition)
			{
				genericTypeDefinition = baseType.GetGenericTypeDefinition();
			}
			for (int i = 0; i < this.collectionTypes.Count; i++)
			{
				Type item = this.collectionTypes[i];
				if (item.IsAssignableFrom(genericTypeDefinition) || item.IsAssignableFrom(baseType))
				{
					return false;
				}
			}
			Type[] implementedInterfaces = DesignTypeGenerator.GetImplementedInterfaces(originalType);
			for (int j = 0; j < (int)implementedInterfaces.Length; j++)
			{
				Type type = implementedInterfaces[j];
				Type genericTypeDefinition1 = type;
				if (type.IsGenericType && !type.IsGenericTypeDefinition)
				{
					genericTypeDefinition1 = type.GetGenericTypeDefinition();
				}
				for (int k = 0; k < this.collectionTypes.Count; k++)
				{
					Type item1 = this.collectionTypes[k];
					if (item1.IsAssignableFrom(genericTypeDefinition1) || item1.IsAssignableFrom(type))
					{
						return true;
					}
				}
			}
			return false;
		}

		private bool IsCorePlatformType(Type sourceType)
		{
			AssemblyName assemblyName = AssemblyHelper.GetAssemblyName(sourceType.Assembly);
			if (((PlatformTypes)this.platformMetadata).GetPlatformAssembly(assemblyName.Name) != null)
			{
				return true;
			}
			return false;
		}

		private bool IsSpecialCollection(BuildingTypeInfo typeInfo)
		{
			object obj;
			bool flag = false;
			if (typeInfo.DesignType is TypeBuilder)
			{
				if (!typeInfo.Extensions.TryGetValue(this, out obj))
				{
					flag = this.IsComplexTypeWithCollectionInterfaces(typeInfo.SourceType);
					typeInfo.Extensions[this] = flag;
				}
				else
				{
					flag = (bool)obj;
				}
			}
			return flag;
		}

		public void OnPropertySet(IDesignTypeGeneratorContext context, TypeBuilder designTimeType, PropertyInfo propertyInfo, ILGenerator setMethod)
		{
		}

		public void OnTypeCloned(IDesignTypeGeneratorContext context, BuildingTypeInfo typeInfo)
		{
			if (this.IsSpecialCollection(typeInfo))
			{
				(new CollectionGenerator(this.platformMetadata, typeInfo)).EmitCollectionImplementation(context);
			}
		}

		public bool ShouldReflectMethod(IDesignTypeGeneratorContext context, BuildingTypeInfo typeInfo, MethodInfo sourceMethod)
		{
			if (sourceMethod.Name == "Clear" && sourceMethod.ReturnType == typeof(void) && this.IsSpecialCollection(typeInfo) && (int)sourceMethod.GetParameters().Length == 0)
			{
				return false;
			}
			return true;
		}

		public bool ShouldReflectProperty(IDesignTypeGeneratorContext context, BuildingTypeInfo typeInfo, PropertyInfo sourceProperty)
		{
			if (!this.IsSpecialCollection(typeInfo))
			{
				return true;
			}
			PropertyInfo property = typeof(IList<>).GetProperty(sourceProperty.Name);
			if (property == null)
			{
				property = typeof(IList).GetProperty(sourceProperty.Name);
				if (property == null)
				{
					return true;
				}
			}
			if (sourceProperty.Name == "Items")
			{
				ParameterInfo[] indexParameters = property.GetIndexParameters();
				if ((int)indexParameters.Length != 1 || indexParameters[0].ParameterType != typeof(int))
				{
					return true;
				}
			}
			return false;
		}
	}
}