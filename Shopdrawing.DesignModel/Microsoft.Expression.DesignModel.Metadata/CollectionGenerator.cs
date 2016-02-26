using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Microsoft.Expression.DesignModel.Metadata
{
	public class CollectionGenerator
	{
		private IPlatformMetadata platformMetadata;

		private BuildingTypeInfo typeInfo;

		private Type sourceType;

		private TypeBuilder designType;

		private ILGenerator ctorGenerator;

		private Type itemType;

		private Type observableCollectionType;

		private FieldBuilder collectionField;

		public CollectionGenerator(IPlatformMetadata platformMetadata, BuildingTypeInfo typeInfo)
		{
			this.platformMetadata = platformMetadata;
			this.typeInfo = typeInfo;
			this.designType = (TypeBuilder)typeInfo.DesignType;
			this.sourceType = typeInfo.SourceType;
			this.ctorGenerator = ((ConstructorBuilder)typeInfo.Constructor).GetILGenerator();
			this.observableCollectionType = this.platformMetadata.ResolveType(PlatformTypes.ObservableCollection).RuntimeType;
		}

		private void CreateCollectionField()
		{
			Type type = this.observableCollectionType;
			Type[] typeArray = new Type[] { this.itemType };
			Type type1 = type.MakeGenericType(typeArray);
			this.collectionField = this.designType.DefineField("__collection", type1, FieldAttributes.Private);
			ConstructorInfo constructor = null;
			try
			{
				constructor = type1.GetConstructor(Type.EmptyTypes);
			}
			catch (NotSupportedException notSupportedException)
			{
				ConstructorInfo constructorInfo = this.observableCollectionType.GetConstructor(Type.EmptyTypes);
				constructor = TypeBuilder.GetConstructor(type1, constructorInfo);
			}
			this.ctorGenerator.Emit(OpCodes.Ldarg_0);
			this.ctorGenerator.Emit(OpCodes.Newobj, constructor);
			this.ctorGenerator.Emit(OpCodes.Stfld, this.collectionField);
		}

		private MethodBuilder DefineMethod(string name, Type returnType, params Type[] paramTypes)
		{
			return this.DefineMethod(null, name, returnType, paramTypes);
		}

		private MethodBuilder DefineMethod(MethodInfo targetMethod, string name, Type returnType, params Type[] paramTypes)
		{
			MethodAttributes methodAttribute = (targetMethod != null ? targetMethod.Attributes : MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.VtableLayoutMask | MethodAttributes.NewSlot | MethodAttributes.SpecialName);
			CallingConventions callingConvention = (targetMethod != null ? targetMethod.CallingConvention : CallingConventions.Standard | CallingConventions.HasThis);
			MethodBuilder methodBuilder = this.designType.DefineMethod(name, methodAttribute, callingConvention, returnType, paramTypes);
			this.typeInfo.AddMethod(methodBuilder, this.GetSourceMethod(name));
			return methodBuilder;
		}

		private PropertyBuilder DefineProperty(string name, Type returnType, params Type[] paramTypes)
		{
			PropertyBuilder propertyBuilder = this.designType.DefineProperty(name, PropertyAttributes.None, returnType, paramTypes);
			this.typeInfo.AddProperty(propertyBuilder, this.GetSourceProperty(name));
			return propertyBuilder;
		}

		private void Emit()
		{
			Type type = typeof(int);
			Type[] typeArray = new Type[] { this.itemType };
			this.EmitPassThruCall("IndexOf", type, typeArray);
			Type type1 = typeof(void);
			Type[] typeArray1 = new Type[] { typeof(int), this.itemType };
			this.EmitPassThruCall("Insert", type1, typeArray1);
			Type type2 = typeof(void);
			Type[] typeArray2 = new Type[] { typeof(int) };
			this.EmitPassThruCall("RemoveAt", type2, typeArray2);
			Type type3 = typeof(bool);
			Type[] typeArray3 = new Type[] { this.itemType };
			this.EmitPassThruCall("Remove", type3, typeArray3);
			Type type4 = typeof(void);
			Type[] typeArray4 = new Type[] { this.itemType };
			this.EmitPassThruCall("Add", type4, typeArray4);
			this.EmitPassThruCall("Clear", typeof(void), new Type[0]);
			Type type5 = typeof(bool);
			Type[] typeArray5 = new Type[] { this.itemType };
			this.EmitPassThruCall("Contains", type5, typeArray5);
			Type type6 = typeof(void);
			Type[] typeArray6 = new Type[] { this.itemType.MakeArrayType(), typeof(int) };
			this.EmitPassThruCall("CopyTo", type6, typeArray6);
			Type runtimeType = this.platformMetadata.ResolveType(PlatformTypes.IEnumeratorT).RuntimeType;
			Type[] typeArray7 = new Type[] { this.itemType };
			this.EmitPassThruCall("GetEnumerator", runtimeType.MakeGenericType(typeArray7), new Type[0]);
			Type type7 = typeof(void);
			Type[] typeArray8 = new Type[] { typeof(Array), typeof(int) };
			MethodBuilder methodBuilder = this.DefineMethod("CopyTo", type7, typeArray8);
			MethodInfo method = this.platformMetadata.ResolveType(PlatformTypes.ICollection).RuntimeType.GetMethod("CopyTo");
			this.EmitPassThruCall(methodBuilder, method);
			Type runtimeType1 = this.platformMetadata.ResolveType(PlatformTypes.IEnumerator).RuntimeType;
			methodBuilder = this.DefineMethod("GetEnumerator", runtimeType1, new Type[0]);
			method = this.platformMetadata.ResolveType(PlatformTypes.IEnumerable).RuntimeType.GetMethod("GetEnumerator");
			this.EmitPassThruCall(methodBuilder, method);
			PropertyBuilder propertyBuilder = this.DefineProperty("Count", typeof(int), new Type[0]);
			methodBuilder = this.EmitPassThruCall("get_Count", typeof(int), new Type[0]);
			propertyBuilder.SetGetMethod(methodBuilder);
			propertyBuilder = this.DefineProperty("IsReadOnly", typeof(bool), new Type[0]);
			methodBuilder = this.DefineMethod("get_IsReadOnly", typeof(bool), new Type[0]);
			ILGenerator lGenerator = methodBuilder.GetILGenerator();
			lGenerator.Emit(OpCodes.Ldc_I4_0);
			lGenerator.Emit(OpCodes.Ret);
			propertyBuilder.SetGetMethod(methodBuilder);
			propertyBuilder = this.DefineProperty("IsSynchronized", typeof(bool), new Type[0]);
			methodBuilder = this.DefineMethod("get_IsSynchronized", typeof(bool), new Type[0]);
			lGenerator = methodBuilder.GetILGenerator();
			lGenerator.Emit(OpCodes.Ldc_I4_0);
			lGenerator.Emit(OpCodes.Ret);
			propertyBuilder.SetGetMethod(methodBuilder);
			propertyBuilder = this.DefineProperty("SyncRoot", typeof(object), new Type[0]);
			methodBuilder = this.DefineMethod("get_SyncRoot", typeof(object), new Type[0]);
			lGenerator = methodBuilder.GetILGenerator();
			lGenerator.Emit(OpCodes.Ldnull);
			lGenerator.Emit(OpCodes.Ret);
			propertyBuilder.SetGetMethod(methodBuilder);
			Type type8 = this.itemType;
			Type[] typeArray9 = new Type[] { typeof(int) };
			propertyBuilder = this.DefineProperty("Item", type8, typeArray9);
			Type type9 = this.itemType;
			Type[] typeArray10 = new Type[] { typeof(int) };
			methodBuilder = this.EmitPassThruCall("get_Item", type9, typeArray10);
			propertyBuilder.SetGetMethod(methodBuilder);
			Type type10 = typeof(void);
			Type[] typeArray11 = new Type[] { typeof(int), this.itemType };
			methodBuilder = this.EmitPassThruCall("set_Item", type10, typeArray11);
			propertyBuilder.SetSetMethod(methodBuilder);
			Type runtimeType2 = this.platformMetadata.ResolveType(PlatformTypes.IListT).RuntimeType;
			TypeBuilder typeBuilder = this.designType;
			Type[] typeArray12 = new Type[] { this.itemType };
			typeBuilder.AddInterfaceImplementation(runtimeType2.MakeGenericType(typeArray12));
			this.designType.AddInterfaceImplementation(this.platformMetadata.ResolveType(PlatformTypes.ICollection).RuntimeType);
			Type runtimeType3 = this.platformMetadata.ResolveType(PlatformTypes.INotifyCollectionChanged).RuntimeType;
			Type runtimeType4 = this.platformMetadata.ResolveType(PlatformTypes.NotifyCollectionChangedEventHandler).RuntimeType;
			EventBuilder eventBuilder = this.designType.DefineEvent("CollectionChanged", EventAttributes.None, runtimeType4);
			Type type11 = typeof(void);
			Type[] typeArray13 = new Type[] { runtimeType4 };
			methodBuilder = this.EmitPassThruCall("add_CollectionChanged", type11, typeArray13);
			eventBuilder.SetAddOnMethod(methodBuilder);
			Type type12 = typeof(void);
			Type[] typeArray14 = new Type[] { runtimeType4 };
			methodBuilder = this.EmitPassThruCall("remove_CollectionChanged", type12, typeArray14);
			eventBuilder.SetRemoveOnMethod(methodBuilder);
			this.designType.AddInterfaceImplementation(runtimeType3);
		}

		public void EmitCollectionImplementation(IDesignTypeGeneratorContext context)
		{
			this.InitializeItemType(context);
			this.CreateCollectionField();
			this.Emit();
		}

		private MethodBuilder EmitPassThruCall(string name, Type returnType, params Type[] paramTypes)
		{
			MethodInfo methodToInvoke = this.GetMethodToInvoke(name);
			MethodBuilder methodBuilder = this.DefineMethod(methodToInvoke, name, returnType, paramTypes);
			this.EmitPassThruCall(methodBuilder, methodToInvoke);
			return methodBuilder;
		}

		private void EmitPassThruCall(MethodBuilder method, MethodInfo targetMethod)
		{
			int length = (int)targetMethod.GetParameters().Length;
			ILGenerator lGenerator = method.GetILGenerator();
			lGenerator.Emit(OpCodes.Ldarg_0);
			lGenerator.Emit(OpCodes.Ldfld, this.collectionField);
			if (length >= 1)
			{
				lGenerator.Emit(OpCodes.Ldarg_1);
			}
			if (length >= 2)
			{
				lGenerator.Emit(OpCodes.Ldarg_2);
			}
			lGenerator.Emit(OpCodes.Callvirt, targetMethod);
			lGenerator.Emit(OpCodes.Ret);
		}

		private MethodInfo GetMethodToInvoke(string name)
		{
			MethodInfo method = null;
			try
			{
				method = this.collectionField.FieldType.GetMethod(name);
			}
			catch (NotSupportedException notSupportedException)
			{
				MethodInfo methodInfo = this.observableCollectionType.GetMethod(name);
				if (methodInfo != null)
				{
					Type declaringType = methodInfo.DeclaringType;
					if (!declaringType.IsGenericTypeDefinition)
					{
						declaringType = methodInfo.DeclaringType.GetGenericTypeDefinition();
						methodInfo = declaringType.GetMethod(name);
					}
					Type[] typeArray = new Type[] { this.itemType };
					method = TypeBuilder.GetMethod(declaringType.MakeGenericType(typeArray), methodInfo);
				}
			}
			return method;
		}

		private MethodInfo GetSourceMethod(string name)
		{
			MethodInfo method;
			try
			{
				method = this.sourceType.GetMethod(name);
			}
			catch (AmbiguousMatchException ambiguousMatchException)
			{
				method = null;
			}
			return method;
		}

		private PropertyInfo GetSourceProperty(string name)
		{
			PropertyInfo property;
			try
			{
				property = this.sourceType.GetProperty(name);
			}
			catch (AmbiguousMatchException ambiguousMatchException)
			{
				property = null;
			}
			return property;
		}

		private void InitializeItemType(IDesignTypeGeneratorContext context)
		{
			Type genericArguments = null;
			KeyValuePair<int, Type>[] keyValuePair = new KeyValuePair<int, Type>[] { new KeyValuePair<int, Type>(0, this.platformMetadata.ResolveType(PlatformTypes.IListT).RuntimeType), new KeyValuePair<int, Type>(1, this.platformMetadata.ResolveType(PlatformTypes.ICollectionT).RuntimeType), new KeyValuePair<int, Type>(2, this.platformMetadata.ResolveType(PlatformTypes.IEnumerableT).RuntimeType) };
			KeyValuePair<int, Type>[] keyValuePairArray = keyValuePair;
			int length = (int)keyValuePairArray.Length;
			Type[] interfaces = this.sourceType.GetInterfaces();
			for (int i = 0; i < (int)interfaces.Length; i++)
			{
				Type type = interfaces[i];
				if (type.IsGenericType && !type.IsGenericTypeDefinition)
				{
					Type genericTypeDefinition = type.GetGenericTypeDefinition();
					for (int j = 0; j < length; j++)
					{
						if (genericTypeDefinition.Equals(keyValuePairArray[j].Value))
						{
							genericArguments = type.GetGenericArguments()[0];
							if (genericArguments != typeof(object))
							{
								length = j;
								break;
							}
						}
					}
					if (length == 0)
					{
						break;
					}
				}
			}
			if (genericArguments == null)
			{
				this.itemType = typeof(object);
				return;
			}
			this.itemType = context.GetDesignType(genericArguments);
		}
	}
}