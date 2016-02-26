using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;

namespace Microsoft.Expression.DesignModel.Metadata
{
	public class ControlEditingDesignTypeGenerator
	{
		private static Dictionary<Type, Type> cachedTypes;

		private static Dictionary<Type, Type> cachedTypesReverseLookup;

		private static int nextUniqueId;

		private ITypeResolver typeResolver;

		private Type DependencyObjectType;

		private Type DependencyPropertyType;

		private Type PropertyMetadataType;

		private Type TypeType;

		private Type FieldInfoType;

		static ControlEditingDesignTypeGenerator()
		{
			ControlEditingDesignTypeGenerator.cachedTypes = new Dictionary<Type, Type>();
			ControlEditingDesignTypeGenerator.cachedTypesReverseLookup = new Dictionary<Type, Type>();
		}

		public ControlEditingDesignTypeGenerator(ITypeResolver typeResolver)
		{
			this.typeResolver = typeResolver;
			this.DependencyObjectType = this.typeResolver.ResolveType(PlatformTypes.DependencyObject).RuntimeType;
			this.DependencyPropertyType = this.typeResolver.ResolveType(PlatformTypes.DependencyProperty).RuntimeType;
			this.PropertyMetadataType = this.typeResolver.ResolveType(PlatformTypes.PropertyMetadata).RuntimeType;
			this.TypeType = this.typeResolver.ResolveType(PlatformTypes.Type).RuntimeType;
			this.FieldInfoType = this.typeResolver.ResolveType(PlatformTypes.FieldInfo).RuntimeType;
		}

		private static void AddToCache(Type sourceType, Type generatedType)
		{
			ControlEditingDesignTypeGenerator.cachedTypes.Add(sourceType, generatedType);
			ControlEditingDesignTypeGenerator.cachedTypesReverseLookup.Add(generatedType, sourceType);
		}

		private string CreateUniqueTypeName(Type originalType)
		{
			string empty = string.Empty;
			CultureInfo invariantCulture = CultureInfo.InvariantCulture;
			object[] objArray = new object[] { originalType.FullName.Replace('.', '\u005F'), originalType.Name };
			empty = string.Format(invariantCulture, "_.ce.{1}", objArray);
			ControlEditingDesignTypeGenerator.nextUniqueId = ControlEditingDesignTypeGenerator.nextUniqueId + 1;
			return empty;
		}

		private bool DefineProperty(PropertyInfo propertyInfo, TypeBuilder designType, Type sourceType, ILGenerator ilStaticConstructor)
		{
			if (sourceType.GetField(string.Concat(propertyInfo.Name, "Property")) == null)
			{
				return false;
			}
			Type propertyType = propertyInfo.PropertyType;
			if (propertyType.IsGenericParameter || sourceType.IsInterface)
			{
				return false;
			}
			ParameterInfo[] indexParameters = propertyInfo.GetIndexParameters();
			if (indexParameters != null && (int)indexParameters.Length > 0)
			{
				return false;
			}
			FieldBuilder fieldBuilder = designType.DefineField(string.Concat(propertyInfo.Name, "Property"), this.DependencyPropertyType, FieldAttributes.FamANDAssem | FieldAttributes.Family | FieldAttributes.Public | FieldAttributes.Static | FieldAttributes.InitOnly);
			ilStaticConstructor.Emit(OpCodes.Ldstr, propertyInfo.Name);
			ilStaticConstructor.Emit(OpCodes.Ldtoken, propertyInfo.PropertyType);
			MethodInfo method = this.TypeType.GetMethod("GetTypeFromHandle");
			ilStaticConstructor.Emit(OpCodes.Call, method);
			ilStaticConstructor.Emit(OpCodes.Ldtoken, designType);
			ilStaticConstructor.Emit(OpCodes.Call, method);
			ilStaticConstructor.Emit(OpCodes.Ldtoken, sourceType);
			ilStaticConstructor.Emit(OpCodes.Call, method);
			ilStaticConstructor.Emit(OpCodes.Ldstr, string.Concat(propertyInfo.Name, "Property"));
			Type typeType = this.TypeType;
			Type[] typeArray = new Type[] { typeof(string) };
			MethodInfo methodInfo = typeType.GetMethod("GetField", typeArray);
			ilStaticConstructor.Emit(OpCodes.Call, methodInfo);
			ilStaticConstructor.Emit(OpCodes.Ldnull);
			Type fieldInfoType = this.FieldInfoType;
			Type[] typeArray1 = new Type[] { typeof(object) };
			MethodInfo method1 = fieldInfoType.GetMethod("GetValue", typeArray1);
			ilStaticConstructor.Emit(OpCodes.Callvirt, method1);
			ilStaticConstructor.Emit(OpCodes.Castclass, this.DependencyPropertyType);
			ilStaticConstructor.Emit(OpCodes.Ldtoken, sourceType);
			ilStaticConstructor.Emit(OpCodes.Call, method);
			Type dependencyPropertyType = this.DependencyPropertyType;
			Type[] typeType1 = new Type[] { this.TypeType };
			MethodInfo methodInfo1 = dependencyPropertyType.GetMethod("GetMetadata", typeType1);
			ilStaticConstructor.Emit(OpCodes.Callvirt, methodInfo1);
			MethodInfo method2 = this.PropertyMetadataType.GetMethod("get_DefaultValue", Type.EmptyTypes);
			ilStaticConstructor.Emit(OpCodes.Callvirt, method2);
			ConstructorInfo constructor = this.PropertyMetadataType.GetConstructor(new Type[] { typeof(object) });
			ilStaticConstructor.Emit(OpCodes.Newobj, constructor);
			Type type = this.DependencyPropertyType;
			Type[] typeType2 = new Type[] { typeof(string), this.TypeType, this.TypeType, this.PropertyMetadataType };
			MethodInfo methodInfo2 = type.GetMethod("Register", typeType2);
			ilStaticConstructor.Emit(OpCodes.Call, methodInfo2);
			ilStaticConstructor.Emit(OpCodes.Stsfld, fieldBuilder);
			PropertyBuilder propertyBuilder = designType.DefineProperty(propertyInfo.Name, PropertyAttributes.None, CallingConventions.Standard | CallingConventions.HasThis, propertyType, null, null, Type.EmptyTypes, null, null);
			MethodBuilder methodBuilder = designType.DefineMethod(string.Concat("get_", propertyInfo.Name), MethodAttributes.Public, propertyType, Type.EmptyTypes);
			if (fieldBuilder != null)
			{
				ILGenerator lGenerator = methodBuilder.GetILGenerator();
				lGenerator.Emit(OpCodes.Ldarg_0);
				lGenerator.Emit(OpCodes.Ldsfld, fieldBuilder);
				MethodInfo method3 = this.DependencyObjectType.GetMethod("GetValue");
				lGenerator.Emit(OpCodes.Call, method3);
				if (!propertyInfo.PropertyType.IsValueType)
				{
					lGenerator.Emit(OpCodes.Castclass, propertyType);
				}
				else
				{
					lGenerator.Emit(OpCodes.Unbox_Any, propertyType);
				}
				lGenerator.Emit(OpCodes.Ret);
			}
			propertyBuilder.SetGetMethod(methodBuilder);
			propertyInfo.GetSetMethod(true);
			string str = string.Concat("set_", propertyInfo.Name);
			Type[] typeArray2 = new Type[] { propertyType };
			MethodBuilder methodBuilder1 = designType.DefineMethod(str, MethodAttributes.Public, null, typeArray2);
			if (fieldBuilder != null)
			{
				ILGenerator lGenerator1 = methodBuilder1.GetILGenerator();
				lGenerator1.Emit(OpCodes.Ldarg_0);
				lGenerator1.Emit(OpCodes.Ldsfld, fieldBuilder);
				lGenerator1.Emit(OpCodes.Ldarg_1);
				if (propertyInfo.PropertyType.IsValueType)
				{
					lGenerator1.Emit(OpCodes.Box, propertyType);
				}
				Type dependencyObjectType = this.DependencyObjectType;
				Type[] dependencyPropertyType1 = new Type[] { this.DependencyPropertyType, typeof(object) };
				MethodInfo methodInfo3 = dependencyObjectType.GetMethod("SetValue", dependencyPropertyType1);
				lGenerator1.Emit(OpCodes.Call, methodInfo3);
				lGenerator1.Emit(OpCodes.Ret);
			}
			propertyBuilder.SetSetMethod(methodBuilder1);
			return true;
		}

		public Type DefineType(Type type)
		{
			Type type1;
			Type fromCache = ControlEditingDesignTypeGenerator.GetFromCache(type);
			if (fromCache != null)
			{
				return fromCache;
			}
			if (type == null || type.IsArray || type.IsGenericType || type.IsInterface || type.IsNested || !PlatformTypes.Control.IsAssignableFrom(this.typeResolver.GetType(type)))
			{
				return type;
			}
			try
			{
				Type baseType = type;
				ConstructorInfo constructor = null;
				while (baseType != null)
				{
					if (!baseType.IsSealed)
					{
						constructor = baseType.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
						if (constructor != null && (constructor.IsPublic || constructor.IsFamily))
						{
							break;
						}
					}
					baseType = baseType.BaseType;
				}
				TypeAttributes attributes = TypeAttributes.Public | type.Attributes & (TypeAttributes.VisibilityMask | TypeAttributes.Public | TypeAttributes.NestedPublic | TypeAttributes.NestedPrivate | TypeAttributes.NestedFamily | TypeAttributes.NestedAssembly | TypeAttributes.NestedFamANDAssem | TypeAttributes.NestedFamORAssem | TypeAttributes.LayoutMask | TypeAttributes.SequentialLayout | TypeAttributes.ExplicitLayout | TypeAttributes.ClassSemanticsMask | TypeAttributes.Interface | TypeAttributes.Abstract | TypeAttributes.Sealed | TypeAttributes.SpecialName | TypeAttributes.Import | TypeAttributes.Serializable | TypeAttributes.WindowsRuntime | TypeAttributes.StringFormatMask | TypeAttributes.UnicodeClass | TypeAttributes.AutoClass | TypeAttributes.CustomFormatClass | TypeAttributes.CustomFormatMask | TypeAttributes.BeforeFieldInit) & (TypeAttributes.VisibilityMask | TypeAttributes.Public | TypeAttributes.NestedPublic | TypeAttributes.NestedPrivate | TypeAttributes.NestedFamily | TypeAttributes.NestedAssembly | TypeAttributes.NestedFamANDAssem | TypeAttributes.NestedFamORAssem | TypeAttributes.LayoutMask | TypeAttributes.SequentialLayout | TypeAttributes.ExplicitLayout | TypeAttributes.ClassSemanticsMask | TypeAttributes.Interface | TypeAttributes.Abstract | TypeAttributes.Sealed | TypeAttributes.SpecialName | TypeAttributes.Import | TypeAttributes.Serializable | TypeAttributes.WindowsRuntime | TypeAttributes.StringFormatMask | TypeAttributes.UnicodeClass | TypeAttributes.AutoClass | TypeAttributes.CustomFormatClass | TypeAttributes.CustomFormatMask | TypeAttributes.BeforeFieldInit | TypeAttributes.ReservedMask | TypeAttributes.RTSpecialName | TypeAttributes.HasSecurity) & (TypeAttributes.VisibilityMask | TypeAttributes.Public | TypeAttributes.NestedPublic | TypeAttributes.NestedPrivate | TypeAttributes.NestedFamily | TypeAttributes.NestedAssembly | TypeAttributes.NestedFamANDAssem | TypeAttributes.NestedFamORAssem | TypeAttributes.LayoutMask | TypeAttributes.SequentialLayout | TypeAttributes.ExplicitLayout | TypeAttributes.ClassSemanticsMask | TypeAttributes.Interface | TypeAttributes.Sealed | TypeAttributes.SpecialName | TypeAttributes.Import | TypeAttributes.Serializable | TypeAttributes.WindowsRuntime | TypeAttributes.StringFormatMask | TypeAttributes.UnicodeClass | TypeAttributes.AutoClass | TypeAttributes.CustomFormatClass | TypeAttributes.CustomFormatMask | TypeAttributes.BeforeFieldInit | TypeAttributes.ReservedMask | TypeAttributes.RTSpecialName | TypeAttributes.HasSecurity);
				string str = this.CreateUniqueTypeName(type);
				RuntimeGeneratedTypesHelper.EnsureControlEditingDesignTypeAssembly(this.typeResolver.PlatformMetadata as IPlatformTypes);
				TypeBuilder typeBuilder = RuntimeGeneratedTypesHelper.ControlEditingDesignTypeAssembly.DefineType(str, attributes, baseType);
				BuildingTypeInfo buildingTypeInfo = new BuildingTypeInfo()
				{
					SourceType = type,
					DesignType = typeBuilder,
					Constructor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, Type.EmptyTypes)
				};
				ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Private | MethodAttributes.Static | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, CallingConventions.Standard, Type.EmptyTypes);
				buildingTypeInfo.IsReplacement = true;
				buildingTypeInfo.BaseTypeInfo = new BuildingTypeInfo()
				{
					SourceType = baseType,
					DesignType = baseType,
					IsReplacement = false
				};
				DesignTypeGenerator.DefineAbstractMethods(buildingTypeInfo);
				ILGenerator lGenerator = ((ConstructorBuilder)buildingTypeInfo.Constructor).GetILGenerator();
				lGenerator.Emit(OpCodes.Ldarg_0);
				lGenerator.Emit(OpCodes.Call, constructor);
				ILGenerator lGenerator1 = constructorBuilder.GetILGenerator();
				BindingFlags bindingFlag = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
				PropertyInfo[] properties = type.GetProperties(bindingFlag);
				for (int i = 0; i < (int)properties.Length; i++)
				{
					PropertyInfo propertyInfo = properties[i];
					PropertyInfo property = baseType.GetProperty(propertyInfo.Name, bindingFlag);
					if (!(property != null) || !(property.PropertyType == propertyInfo.PropertyType))
					{
						this.DefineProperty(propertyInfo, typeBuilder, type, lGenerator1);
					}
				}
				lGenerator1.Emit(OpCodes.Ret);
				lGenerator.Emit(OpCodes.Ret);
				Type type2 = typeBuilder.CreateType();
				ControlEditingDesignTypeGenerator.AddToCache(type, type2);
				type1 = type2;
			}
			catch (Exception exception)
			{
				type1 = type;
			}
			return type1;
		}

		public static Type GetFromCache(Type type)
		{
			Type type1;
			if (ControlEditingDesignTypeGenerator.cachedTypes.TryGetValue(type, out type1) && type1 != null)
			{
				return type1;
			}
			return null;
		}

		public static Type GetSourceType(Type generatedType)
		{
			Type type;
			if (ControlEditingDesignTypeGenerator.cachedTypesReverseLookup.TryGetValue(generatedType, out type))
			{
				return type;
			}
			return null;
		}
	}
}