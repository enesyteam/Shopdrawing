using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Data;

namespace Microsoft.Expression.DesignModel.Metadata
{
	public abstract class CollectionAdapterDescription
	{
		private System.Type type;

		private System.Type itemType;

		private static Dictionary<System.Type, CollectionAdapterDescription> collectionAdapterDescriptionCache;

		public System.Type ItemType
		{
			get
			{
				return this.itemType;
			}
		}

		public System.Type Type
		{
			get
			{
				return this.type;
			}
		}

		static CollectionAdapterDescription()
		{
			CollectionAdapterDescription.collectionAdapterDescriptionCache = new Dictionary<System.Type, CollectionAdapterDescription>();
		}

		protected CollectionAdapterDescription(System.Type type, System.Type itemType)
		{
			this.type = type;
			this.itemType = itemType;
		}

		public static CollectionAdapterDescription GetAdapterDescription(System.Type type)
		{
			return CollectionAdapterDescription.GetAdapterDescription(type, null);
		}

		public static CollectionAdapterDescription GetAdapterDescription(System.Type type, System.Type referenceType)
		{
			CollectionAdapterDescription descriptionInternal = null;
			if (!CollectionAdapterDescription.collectionAdapterDescriptionCache.TryGetValue(type, out descriptionInternal))
			{
				descriptionInternal = CollectionAdapterDescription.GetDescriptionInternal(type, referenceType);
				CollectionAdapterDescription.collectionAdapterDescriptionCache[type] = descriptionInternal;
			}
			return descriptionInternal;
		}

		public abstract ICollection GetCollectionAdapter(object instance);

		private static CollectionAdapterDescription GetDescriptionInternal(System.Type type, System.Type referenceType)
		{
			if (typeof(IDictionary).IsAssignableFrom(type) || typeof(IDictionary<object, object>).IsAssignableFrom(type) && type.Name == "ResourceDictionary")
			{
				return new CollectionAdapterDescription.CollectionDescription(type, typeof(DictionaryEntry));
			}
			if (!typeof(ICollection).IsAssignableFrom(type))
			{
				if (!typeof(XmlNamespaceMappingCollection).IsAssignableFrom(type))
				{
					return null;
				}
				return new CollectionAdapterDescription.CollectionDescription(type, typeof(XmlNamespaceMapping));
			}
			if (type.IsArray)
			{
				return new CollectionAdapterDescription.CollectionDescription(type, type.GetElementType());
			}
			System.Type genericCollectionType = CollectionAdapterDescription.GetGenericCollectionType(type);
			MemberInfo[] memberInfoArray = type.FindMembers(MemberTypes.Method, BindingFlags.Instance | BindingFlags.Public, (MemberInfo member, object arg) => member.Name == "Add", null);
			MethodInfo methodInfo = null;
			MemberInfo[] memberInfoArray1 = memberInfoArray;
			for (int i = 0; i < (int)memberInfoArray1.Length; i++)
			{
				MethodInfo methodInfo1 = memberInfoArray1[i] as MethodInfo;
				if (methodInfo1 != null && MemberCollection.ReferenceMethodExists(referenceType, "Add", BindingFlags.Instance | BindingFlags.Public))
				{
					ParameterInfo[] parameters = methodInfo1.GetParameters();
					if ((int)parameters.Length == 1)
					{
						if (genericCollectionType == null)
						{
							methodInfo = methodInfo1;
							genericCollectionType = parameters[0].ParameterType;
							break;
						}
						else if (parameters[0].ParameterType == genericCollectionType)
						{
							methodInfo = methodInfo1;
							break;
						}
					}
				}
			}
			if (genericCollectionType == null)
			{
				return null;
			}
			if (typeof(IList).IsAssignableFrom(type))
			{
				return new CollectionAdapterDescription.CollectionDescription(type, genericCollectionType);
			}
			if (methodInfo != null)
			{
				memberInfoArray = type.FindMembers(MemberTypes.Method, BindingFlags.Instance | BindingFlags.Public, (MemberInfo member, object arg) => member.Name == "Clear", null);
				MethodInfo methodInfo2 = null;
				MemberInfo[] memberInfoArray2 = memberInfoArray;
				int num = 0;
				while (num < (int)memberInfoArray2.Length)
				{
					MethodInfo methodInfo3 = memberInfoArray2[num] as MethodInfo;
					if (!(methodInfo3 != null) || !MemberCollection.ReferenceMethodExists(referenceType, "Clear", BindingFlags.Instance | BindingFlags.Public) || (int)methodInfo3.GetParameters().Length != 0)
					{
						num++;
					}
					else
					{
						methodInfo2 = methodInfo3;
						break;
					}
				}
				memberInfoArray = type.FindMembers(MemberTypes.Method, BindingFlags.Instance | BindingFlags.Public, (MemberInfo member, object arg) => member.Name == "Insert", null);
				MethodInfo methodInfo4 = null;
				MemberInfo[] memberInfoArray3 = memberInfoArray;
				for (int j = 0; j < (int)memberInfoArray3.Length; j++)
				{
					MethodInfo methodInfo5 = memberInfoArray3[j] as MethodInfo;
					if (methodInfo5 != null && MemberCollection.ReferenceMethodExists(referenceType, "Insert", BindingFlags.Instance | BindingFlags.Public))
					{
						ParameterInfo[] parameterInfoArray = methodInfo5.GetParameters();
						if ((int)parameterInfoArray.Length == 2 && parameterInfoArray[0].ParameterType == typeof(int) && parameterInfoArray[1].ParameterType == genericCollectionType)
						{
							methodInfo4 = methodInfo5;
							break;
						}
					}
				}
				memberInfoArray = type.FindMembers(MemberTypes.Method, BindingFlags.Instance | BindingFlags.Public, (MemberInfo member, object arg) => member.Name == "RemoveAt", null);
				MethodInfo methodInfo6 = null;
				MemberInfo[] memberInfoArray4 = memberInfoArray;
				for (int k = 0; k < (int)memberInfoArray4.Length; k++)
				{
					MethodInfo methodInfo7 = memberInfoArray4[k] as MethodInfo;
					if (methodInfo7 != null && MemberCollection.ReferenceMethodExists(referenceType, "RemoveAt", BindingFlags.Instance | BindingFlags.Public))
					{
						ParameterInfo[] parameters1 = methodInfo7.GetParameters();
						if ((int)parameters1.Length == 1 && parameters1[0].ParameterType == typeof(int))
						{
							methodInfo6 = methodInfo7;
							break;
						}
					}
				}
				memberInfoArray = type.FindMembers(MemberTypes.Property, BindingFlags.Instance | BindingFlags.Public, (MemberInfo member, object arg) => member.Name == "Item", null);
				PropertyInfo propertyInfo = null;
				MemberInfo[] memberInfoArray5 = memberInfoArray;
				for (int l = 0; l < (int)memberInfoArray5.Length; l++)
				{
					PropertyInfo propertyInfo1 = memberInfoArray5[l] as PropertyInfo;
					if (propertyInfo1 != null && propertyInfo1.CanRead && propertyInfo1.CanWrite && (!(referenceType != null) || !(referenceType.GetProperty("Item", BindingFlags.Instance | BindingFlags.Public) == null)))
					{
						ParameterInfo[] indexParameters = propertyInfo1.GetIndexParameters();
						if ((int)indexParameters.Length == 1 && indexParameters[0].ParameterType == typeof(int))
						{
							propertyInfo = propertyInfo1;
							break;
						}
					}
				}
				if (methodInfo2 != null && methodInfo4 != null && methodInfo6 != null && propertyInfo != null)
				{
					return new ReflectionCollectionDescription(type, genericCollectionType, methodInfo, methodInfo2, methodInfo4, methodInfo6, propertyInfo);
				}
			}
			return new CollectionAdapterDescription.CollectionDescription(type, genericCollectionType);
		}

		public static System.Type GetGenericCollectionType(System.Type type)
		{
			if (type == null)
			{
				return null;
			}
			System.Type[] interfaces = PlatformTypeHelper.GetInterfaces(type);
			if (interfaces != null)
			{
				System.Type[] typeArray = interfaces;
				for (int i = 0; i < (int)typeArray.Length; i++)
				{
					System.Type genericCollectionTypeFromInterface = CollectionAdapterDescription.GetGenericCollectionTypeFromInterface(typeArray[i]);
					if (genericCollectionTypeFromInterface != null)
					{
						return genericCollectionTypeFromInterface;
					}
				}
			}
			if (!type.IsInterface)
			{
				return null;
			}
			return CollectionAdapterDescription.GetGenericCollectionTypeFromInterface(type);
		}

		private static System.Type GetGenericCollectionTypeFromInterface(System.Type type)
		{
			System.Type genericTypeDefinition = PlatformTypeHelper.GetGenericTypeDefinition(type);
			if (genericTypeDefinition != null && typeof(ICollection<>).IsAssignableFrom(genericTypeDefinition))
			{
				System.Type[] genericTypeArguments = PlatformTypeHelper.GetGenericTypeArguments(type);
				if (genericTypeArguments != null)
				{
					return genericTypeArguments[0];
				}
			}
			return null;
		}

		private sealed class CollectionDescription : CollectionAdapterDescription
		{
			public CollectionDescription(System.Type type, System.Type itemType) : base(type, itemType)
			{
			}

			public override ICollection GetCollectionAdapter(object instance)
			{
				return instance as ICollection;
			}
		}
	}
}