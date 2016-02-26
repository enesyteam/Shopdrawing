using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Windows.Markup;

namespace Microsoft.Expression.DesignModel.Metadata
{
	public sealed class DesignTypeGenerator : IDesignTypeGeneratorContext
	{
		private Dictionary<Type, BuildingTypeInfo> buildingTypes = new Dictionary<Type, BuildingTypeInfo>();

		private List<BuildingTypeInfo> finalizedTypes = new List<BuildingTypeInfo>();

		private List<BuildingTypeInfo> orderedTypes = new List<BuildingTypeInfo>();

		private List<BuildingTypeInfo> recursionGuard = new List<BuildingTypeInfo>();

		private Stack typeContext = new Stack();

		private IPlatformMetadata platformMetadata;

		private Type objectType;

		private List<IDesignTypeGeneratorExtension> extensions = new List<IDesignTypeGeneratorExtension>();

		private static object _sync;

		private static Dictionary<Type, DesignTypeResult> designTimeTypes;

		private static Dictionary<Type, DesignTypeResult> xamlFirendlyListTypes;

		private static int nextUniqueId;

		private static char[] xamlUnfiendlyChars;

		public string CurrentContext
		{
			get
			{
				if (this.typeContext == null || this.typeContext.Count <= 0)
				{
					return string.Empty;
				}
				return this.typeContext.Peek() as string;
			}
		}

		static DesignTypeGenerator()
		{
			DesignTypeGenerator._sync = new object();
			DesignTypeGenerator.designTimeTypes = new Dictionary<Type, DesignTypeResult>();
			DesignTypeGenerator.xamlFirendlyListTypes = new Dictionary<Type, DesignTypeResult>();
			DesignTypeGenerator.xamlUnfiendlyChars = "`[]&+".ToCharArray();
		}

		public DesignTypeGenerator(IPlatformMetadata platformMetadata) : this(platformMetadata, new IDesignTypeGeneratorExtension[] { new PlatformTypesValidationExtension(platformMetadata) })
		{
		}

		public DesignTypeGenerator(IPlatformMetadata platformMetadata, params IDesignTypeGeneratorExtension[] extensions)
		{
			this.platformMetadata = platformMetadata;
			this.objectType = this.platformMetadata.ResolveType(PlatformTypes.Object).RuntimeType;
			this.extensions.AddRange(extensions);
		}

		private static void CacheResult(Type sourceType, Type designType)
		{
			if (sourceType == null || designType == null)
			{
				return;
			}
			if (sourceType == designType)
			{
				return;
			}
			DesignTypeResult designTypeResult = new DesignTypeResult(sourceType, designType);
			DesignTypeGenerator.designTimeTypes[sourceType] = designTypeResult;
			DesignTypeGenerator.designTimeTypes[designType] = designTypeResult;
		}

		private void CollectDescendantSourceTypes(BuildingTypeInfo typeInfo)
		{
			if (typeInfo.SourceType.IsNested)
			{
				this.CollectSourceTypes(typeInfo.SourceType.DeclaringType);
			}
			if (typeInfo.SourceType.IsArray)
			{
				Type arrayItemType = DesignTypeGenerator.GetArrayItemType(typeInfo.SourceType);
				this.CollectSourceTypes(arrayItemType);
				return;
			}
			if (typeInfo.SourceType.IsGenericType)
			{
				Type[] genericArguments = typeInfo.SourceType.GetGenericArguments();
				for (int i = 0; i < (int)genericArguments.Length; i++)
				{
					this.CollectSourceTypes(genericArguments[i]);
				}
			}
			if (typeInfo.IsReplacement)
			{
				return;
			}
			DesignTypeGenerator.BaseType sourceBaseType = this.GetSourceBaseType(typeInfo.SourceType);
			if (!sourceBaseType.IsReplacement || this.buildingTypes.ContainsKey(sourceBaseType.Type))
			{
				typeInfo.BaseTypeInfo = this.CollectSourceTypes(sourceBaseType.Type);
				if (sourceBaseType.IsReplacement)
				{
					typeInfo.BaseTypeInfo.IsReplacement = true;
				}
			}
			else
			{
				BuildingTypeInfo buildingTypeInfo = new BuildingTypeInfo()
				{
					SourceType = sourceBaseType.Type,
					DesignType = sourceBaseType.Type,
					IsReplacement = true
				};
				typeInfo.BaseTypeInfo = buildingTypeInfo;
				this.buildingTypes[sourceBaseType.Type] = typeInfo.BaseTypeInfo;
				this.CollectDescendantSourceTypes(typeInfo.BaseTypeInfo);
			}
			Type[] implementedInterfaces = DesignTypeGenerator.GetImplementedInterfaces(typeInfo.SourceType);
			for (int j = 0; j < (int)implementedInterfaces.Length; j++)
			{
				Type type = implementedInterfaces[j];
				if (this.GetReplacementType(type) == null)
				{
					this.CollectSourceTypes(type);
				}
			}
			Type[] nestedTypes = typeInfo.SourceType.GetNestedTypes(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			for (int k = 0; k < (int)nestedTypes.Length; k++)
			{
				this.CollectSourceTypes(nestedTypes[k]);
			}
			if (typeInfo.SourceType.IsInterface)
			{
				return;
			}
			foreach (PropertyInfo reflectableProperty in this.GetReflectableProperties(typeInfo))
			{
				if (DesignTypeGenerator.IsPropertyReplaced(typeInfo.BaseTypeInfo, reflectableProperty))
				{
					continue;
				}
				this.CollectSourceTypes(reflectableProperty.PropertyType);
				ParameterInfo[] indexParameters = reflectableProperty.GetIndexParameters();
				for (int l = 0; l < (int)indexParameters.Length; l++)
				{
					this.CollectSourceTypes(indexParameters[l].ParameterType);
				}
			}
		}

		private BuildingTypeInfo CollectSourceTypes(Type originalSourceType)
		{
			BuildingTypeInfo buildingTypeInfo;
			Type normalizedType = DesignTypeGenerator.GetNormalizedType(originalSourceType);
			if (normalizedType == null || normalizedType.IsGenericParameter || normalizedType.IsGenericTypeDefinition)
			{
				return null;
			}
			if (this.buildingTypes.TryGetValue(normalizedType, out buildingTypeInfo))
			{
				return buildingTypeInfo;
			}
			bool flag = true;
			Type replacementType = this.GetReplacementType(normalizedType);
			if (replacementType == null)
			{
				replacementType = this.GetExistingDesignType(normalizedType);
				flag = false;
			}
			BuildingTypeInfo buildingTypeInfo1 = new BuildingTypeInfo()
			{
				SourceType = normalizedType,
				DesignType = replacementType,
				IsReplacement = flag
			};
			buildingTypeInfo = buildingTypeInfo1;
			this.buildingTypes[normalizedType] = buildingTypeInfo;
			if (replacementType != null && !flag && !normalizedType.IsArray && !normalizedType.IsGenericType)
			{
				return buildingTypeInfo;
			}
			if (!normalizedType.IsEnum)
			{
				this.CollectDescendantSourceTypes(buildingTypeInfo);
			}
			else
			{
				this.typeContext.Push(normalizedType.FullName);
				buildingTypeInfo.DesignType = this.CopyEnumImpl(normalizedType);
				this.typeContext.Pop();
			}
			return buildingTypeInfo;
		}

		private static bool CompareMethods(MethodInfo left, MethodInfo right)
		{
			if (left == right)
			{
				return true;
			}
			if (left == null || right == null)
			{
				return false;
			}
			if (left.Name != right.Name || !object.Equals(left.ReturnType, right.ReturnType))
			{
				return false;
			}
			return DesignTypeGenerator.CompareParameters(left.GetParameters(), right.GetParameters());
		}

		internal static bool CompareParameters(ParameterInfo[] left, ParameterInfo[] right)
		{
			if (left == right)
			{
				return true;
			}
			if (left == null || right == null)
			{
				return false;
			}
			if ((int)left.Length != (int)right.Length)
			{
				return false;
			}
			for (int i = 0; i < (int)left.Length; i++)
			{
				if (!object.Equals(left[i].ParameterType, right[i].ParameterType))
				{
					return false;
				}
			}
			return true;
		}

		private void CopyClassImpl(BuildingTypeInfo typeInfo)
		{
			BuildingTypeInfo buildingTypeInfo;
			if (typeInfo.IsReplacement)
			{
				return;
			}
			TypeBuilder designType = typeInfo.DesignType as TypeBuilder;
			if (designType == null || typeInfo.SourceType.IsInterface)
			{
				return;
			}
			Type[] implementedInterfaces = DesignTypeGenerator.GetImplementedInterfaces(typeInfo.SourceType);
			for (int i = 0; i < (int)implementedInterfaces.Length; i++)
			{
				Type type = implementedInterfaces[i];
				if (!(this.GetReplacementType(type) != null) && this.buildingTypes.TryGetValue(type, out buildingTypeInfo))
				{
					designType.AddInterfaceImplementation(buildingTypeInfo.DesignType);
				}
			}
			ILGenerator lGenerator = ((ConstructorBuilder)typeInfo.Constructor).GetILGenerator();
			if (typeInfo.BaseTypeInfo != null)
			{
				ConstructorInfo designTypeConstructorInfo = this.GetDesignTypeConstructorInfo(typeInfo.BaseTypeInfo, true);
				if (designTypeConstructorInfo != null)
				{
					lGenerator.Emit(OpCodes.Ldarg_0);
					lGenerator.Emit(OpCodes.Call, designTypeConstructorInfo);
				}
			}
			foreach (IDesignTypeGeneratorExtension extension in this.extensions)
			{
				extension.OnTypeCloned(this, typeInfo);
			}
			foreach (PropertyInfo reflectableProperty in this.GetReflectableProperties(typeInfo))
			{
				if (typeInfo.PropertyExists(reflectableProperty) || DesignTypeGenerator.IsPropertyReplaced(typeInfo.BaseTypeInfo, reflectableProperty))
				{
					continue;
				}
				this.typeContext.Push(string.Concat(typeInfo.SourceType.FullName, ".", reflectableProperty.Name));
				this.DefineProperty(reflectableProperty, typeInfo, lGenerator, false);
				this.typeContext.Pop();
			}
			lGenerator.Emit(OpCodes.Ret);
		}

		private Type CopyEnumImpl(Type sourceType)
		{
			Type type;
			EnumBuilder enumBuilder = null;
			try
			{
				string str = DesignTypeGenerator.CreateTypeName(sourceType);
				Type underlyingType = Enum.GetUnderlyingType(sourceType);
				enumBuilder = RuntimeGeneratedTypesHelper.RuntimeGeneratedTypesAssembly.DefineEnum(str, TypeAttributes.Public, underlyingType);
				string[] names = Enum.GetNames(sourceType);
				for (int i = 0; i < (int)names.Length; i++)
				{
					object obj = Enum.Parse(sourceType, names[i]);
					object obj1 = null;
					switch (((Enum)obj).GetTypeCode())
					{
						case TypeCode.Boolean:
						{
							obj1 = (bool)obj;
							break;
						}
						case TypeCode.Char:
						{
							obj1 = (char)obj;
							break;
						}
						case TypeCode.SByte:
						{
							obj1 = (sbyte)obj;
							break;
						}
						case TypeCode.Byte:
						{
							obj1 = (byte)obj;
							break;
						}
						case TypeCode.Int16:
						{
							obj1 = (short)obj;
							break;
						}
						case TypeCode.UInt16:
						{
							obj1 = (ushort)obj;
							break;
						}
						case TypeCode.Int32:
						{
							obj1 = (int)obj;
							break;
						}
						case TypeCode.UInt32:
						{
							obj1 = (uint)obj;
							break;
						}
						case TypeCode.Int64:
						{
							obj1 = (long)obj;
							break;
						}
						case TypeCode.UInt64:
						{
							obj1 = (ulong)obj;
							break;
						}
						case TypeCode.Single:
						{
							obj1 = (float)obj;
							break;
						}
						case TypeCode.Double:
						{
							obj1 = (double)obj;
							break;
						}
						case TypeCode.Decimal:
						{
							obj1 = (decimal)obj;
							break;
						}
					}
					enumBuilder.DefineLiteral(names[i], obj1);
				}
				Type type1 = enumBuilder.CreateType();
				DesignTypeGenerator.CacheResult(sourceType, type1);
				Dictionary<Type, BuildingTypeInfo> types = this.buildingTypes;
				BuildingTypeInfo buildingTypeInfo = new BuildingTypeInfo()
				{
					SourceType = sourceType,
					DesignType = type1
				};
				types[sourceType] = buildingTypeInfo;
				type = type1;
			}
			catch (ArgumentException argumentException)
			{
				Type runtimeType = this.platformMetadata.ResolveType(PlatformTypes.String).RuntimeType;
				DesignTypeGenerator.CacheResult(sourceType, runtimeType);
				Dictionary<Type, BuildingTypeInfo> types1 = this.buildingTypes;
				BuildingTypeInfo buildingTypeInfo1 = new BuildingTypeInfo()
				{
					SourceType = sourceType,
					DesignType = runtimeType
				};
				types1[sourceType] = buildingTypeInfo1;
				type = runtimeType;
			}
			return type;
		}

		private static string CreateTypeName(Type originalType)
		{
			string str = DesignTypeGenerator.NormalizeTypeName(originalType, new List<Type>());
			string @namespace = originalType.Namespace;
			@namespace = (!string.IsNullOrEmpty(@namespace) ? string.Concat(@namespace, ".", str) : str);
			if (string.IsNullOrEmpty(@namespace))
			{
				@namespace = "__temp";
			}
			@namespace = DesignTypeGenerator.CreateUniqueTypeName(@namespace);
			return @namespace;
		}

		private static string CreateUniqueTypeName(string originalFullName)
		{
			string empty = string.Empty;
			int num = DesignTypeGenerator.nextUniqueId;
			while (num < 2147483646)
			{
				CultureInfo invariantCulture = CultureInfo.InvariantCulture;
				object[] objArray = new object[] { num, originalFullName };
				empty = string.Format(invariantCulture, "_.di{0}.{1}", objArray);
				if (RuntimeGeneratedTypesHelper.RuntimeGeneratedTypesAssembly.GetType(empty, false, false) != null)
				{
					num++;
				}
				else
				{
					DesignTypeGenerator.nextUniqueId = num;
					break;
				}
			}
			return empty;
		}

		internal static void DefineAbstractMethods(BuildingTypeInfo typeInfo)
		{
			if (typeInfo.DesignType.BaseType == null || !typeInfo.DesignType.BaseType.IsAbstract)
			{
				return;
			}
			TypeBuilder designType = (TypeBuilder)typeInfo.DesignType;
			MethodInfo[] methods = typeInfo.DesignType.BaseType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			for (int i = 0; i < (int)methods.Length; i++)
			{
				MethodInfo methodInfo = methods[i];
				if (methodInfo.IsAbstract && !typeInfo.MethodExists(methodInfo))
				{
					typeInfo.AddMethod(DesignTypeGenerator.DefineMethod(designType, methodInfo), methodInfo);
				}
			}
		}

		private Type DefineArrayType(BuildingTypeInfo typeInfo)
		{
			Type arrayItemType = DesignTypeGenerator.GetArrayItemType(typeInfo.SourceType);
			Type type = this.DefineOrGetExistingType(this.buildingTypes[arrayItemType]);
			typeInfo.DesignType = type.MakeArrayType(typeInfo.SourceType.GetArrayRank());
			return typeInfo.DesignType;
		}

		private void DefineCommandMethods(BuildingTypeInfo typeInfo)
		{
			Type designType;
			if (typeInfo.IsReplacement)
			{
				return;
			}
			MethodInfo[] commandMethods = DesignTypeGenerator.GetCommandMethods(typeInfo.SourceType, this.platformMetadata, false);
			if (commandMethods == null || (int)commandMethods.Length == 0)
			{
				return;
			}
			Type type = null;
			BuildingTypeInfo baseTypeInfo = typeInfo.BaseTypeInfo;
			while (true)
			{
				if (baseTypeInfo == null || !(baseTypeInfo.DesignType != null))
				{
					goto Label0;
				}
				if (!(baseTypeInfo.DesignType is TypeBuilder))
				{
					designType = baseTypeInfo.DesignType;
					if (!designType.IsGenericType)
					{
						break;
					}
					designType = designType.GetGenericTypeDefinition();
					if (!(designType is TypeBuilder))
					{
						break;
					}
				}
				baseTypeInfo = baseTypeInfo.BaseTypeInfo;
			}
			type = designType;
		Label0:
			MethodInfo[] methodInfoArray = null;
			if (type != null)
			{
				methodInfoArray = DesignTypeGenerator.GetCommandMethods(type, this.platformMetadata, true);
			}
			TypeBuilder typeBuilder = (TypeBuilder)typeInfo.DesignType;
			for (int i = 0; i < (int)commandMethods.Length; i++)
			{
				MethodInfo methodInfo = commandMethods[i];
				if (!typeInfo.MethodExists(methodInfo))
				{
					bool flag = true;
					foreach (IDesignTypeGeneratorExtension extension in this.extensions)
					{
						if (extension.ShouldReflectMethod(this, typeInfo, methodInfo))
						{
							continue;
						}
						flag = false;
						break;
					}
					if (flag && methodInfoArray != null)
					{
						int num = 0;
						while (num < (int)methodInfoArray.Length)
						{
							if (!DesignTypeGenerator.CompareMethods(methodInfoArray[num], methodInfo))
							{
								num++;
							}
							else
							{
								flag = false;
								break;
							}
						}
					}
					if (flag)
					{
						typeInfo.AddMethod(DesignTypeGenerator.DefineMethod(typeBuilder, methodInfo), methodInfo);
					}
				}
			}
		}

		private static void DefineCustomAttributes(PropertyInfo propertyInfo, PropertyBuilder propertyBuilder, bool hide)
		{
			foreach (CustomAttributeData customAttribute in CustomAttributeData.GetCustomAttributes(propertyInfo))
			{
				if (hide && customAttribute.Constructor.DeclaringType == typeof(BrowsableAttribute))
				{
					continue;
				}
				List<FieldInfo> fieldInfos = new List<FieldInfo>();
				List<object> objs = new List<object>();
				List<PropertyInfo> propertyInfos = new List<PropertyInfo>();
				List<object> objs1 = new List<object>();
				foreach (CustomAttributeNamedArgument namedArgument in customAttribute.NamedArguments)
				{
					if (!(namedArgument.MemberInfo is FieldInfo))
					{
						if (!(namedArgument.MemberInfo is PropertyInfo))
						{
							continue;
						}
						propertyInfos.Add(namedArgument.MemberInfo as PropertyInfo);
						objs1.Add(namedArgument.TypedValue.Value);
					}
					else
					{
						fieldInfos.Add(namedArgument.MemberInfo as FieldInfo);
						objs.Add(namedArgument.TypedValue.Value);
					}
				}
				List<object> objs2 = new List<object>();
				foreach (CustomAttributeTypedArgument constructorArgument in customAttribute.ConstructorArguments)
				{
					objs2.Add(constructorArgument.Value);
				}
				CustomAttributeBuilder customAttributeBuilder = new CustomAttributeBuilder(customAttribute.Constructor, objs2.ToArray(), propertyInfos.ToArray(), objs1.ToArray(), fieldInfos.ToArray(), objs.ToArray());
				propertyBuilder.SetCustomAttribute(customAttributeBuilder);
			}
			if (hide)
			{
				ConstructorInfo constructor = typeof(BrowsableAttribute).GetConstructor(new Type[] { typeof(bool) });
				object[] objArray = new object[] { false };
				propertyBuilder.SetCustomAttribute(new CustomAttributeBuilder(constructor, objArray));
			}
		}

		private Type DefineDesignType(BuildingTypeInfo typeInfo)
		{
			if (typeInfo.SourceType.IsGenericParameter)
			{
				return typeInfo.SourceType;
			}
			this.typeContext.Push(typeInfo.SourceType.FullName);
			typeInfo.DesignType = this.DefineDesignTypeInternal(typeInfo);
			this.typeContext.Pop();
			return typeInfo.DesignType;
		}

		private Type DefineDesignTypeInternal(BuildingTypeInfo typeInfo)
		{
			Type type;
			Type type1;
			if (typeInfo.SourceType.IsGenericTypeDefinition)
			{
				return this.objectType;
			}
			if (this.recursionGuard.Contains(typeInfo))
			{
				CultureInfo currentCulture = CultureInfo.CurrentCulture;
				string designTypeCannotBeRecursive = ExceptionStringTable.DesignTypeCannotBeRecursive;
				object[] fullName = new object[] { typeInfo.SourceType.FullName };
				throw new InvalidOperationException(string.Format(currentCulture, designTypeCannotBeRecursive, fullName));
			}
			this.recursionGuard.Add(typeInfo);
			try
			{
				if (typeInfo.IsReplacement)
				{
					type = this.DefineReplacementType(typeInfo);
				}
				else if (!typeInfo.SourceType.IsArray)
				{
					type = (!typeInfo.SourceType.IsGenericType ? this.DefinePlainType(typeInfo) : this.DefineGenericType(typeInfo));
				}
				else
				{
					type = this.DefineArrayType(typeInfo);
				}
				type1 = type;
			}
			finally
			{
				this.recursionGuard.Remove(typeInfo);
			}
			return type1;
		}

		private FieldBuilder DefineField(PropertyInfo propertyInfo, BuildingTypeInfo typeInfo, ILGenerator ilConstructor)
		{
			if (typeInfo.DesignType.IsInterface)
			{
				return null;
			}
			Type propertyType = propertyInfo.PropertyType;
			BuildingTypeInfo item = this.buildingTypes[propertyType];
			bool flag = DesignTypeGenerator.IsPropertyStatic(propertyInfo);
			FieldAttributes fieldAttribute = FieldAttributes.Private;
			if (flag)
			{
				fieldAttribute = fieldAttribute | FieldAttributes.Static;
			}
			TypeBuilder designType = (TypeBuilder)typeInfo.DesignType;
			FieldBuilder fieldBuilder = designType.DefineField(string.Concat("_fld_", propertyInfo.Name), item.DesignType, fieldAttribute);
			if (!flag && ilConstructor != null && this.ShouldInitializeProperty(propertyInfo, item.DesignType))
			{
				if (propertyType.IsEnum)
				{
					string[] enumNames = item.DesignType.GetEnumNames();
					if (enumNames != null && (int)enumNames.Length > 0)
					{
						ilConstructor.Emit(OpCodes.Ldarg_0);
						ilConstructor.Emit(OpCodes.Ldtoken, item.DesignType);
						OpCode call = OpCodes.Call;
						Type type = typeof(Type);
						Type[] typeArray = new Type[] { typeof(RuntimeTypeHandle) };
						ilConstructor.Emit(call, type.GetMethod("GetTypeFromHandle", typeArray));
						ilConstructor.Emit(OpCodes.Ldstr, enumNames[0]);
						OpCode opCode = OpCodes.Call;
						Type type1 = typeof(Enum);
						Type[] typeArray1 = new Type[] { typeof(Type), typeof(string) };
						ilConstructor.Emit(opCode, type1.GetMethod("Parse", typeArray1));
						ilConstructor.Emit(OpCodes.Unbox_Any, item.DesignType);
						ilConstructor.Emit(OpCodes.Stfld, fieldBuilder);
					}
				}
				else
				{
					ConstructorInfo designTypeConstructorInfo = this.GetDesignTypeConstructorInfo(item, false);
					if (designTypeConstructorInfo != null)
					{
						ilConstructor.Emit(OpCodes.Ldarg_0);
						ilConstructor.Emit(OpCodes.Newobj, designTypeConstructorInfo);
						ilConstructor.Emit(OpCodes.Stfld, fieldBuilder);
					}
				}
			}
			return fieldBuilder;
		}

		private Type DefineGenericType(BuildingTypeInfo typeInfo)
		{
			Type genericTypeDefinition = typeInfo.SourceType.GetGenericTypeDefinition();
			if (this.GetDesignTypeFromExtensions(genericTypeDefinition) == null)
			{
				return this.DefinePlainType(typeInfo);
			}
			Type[] genericArguments = typeInfo.SourceType.GetGenericArguments();
			Type[] typeArray = new Type[(int)genericArguments.Length];
			for (int i = 0; i < (int)genericArguments.Length; i++)
			{
				BuildingTypeInfo item = this.buildingTypes[genericArguments[i]];
				typeArray[i] = this.DefineOrGetExistingType(item);
			}
			typeInfo.DesignType = genericTypeDefinition.MakeGenericType(typeArray);
			return typeInfo.DesignType;
		}

		private static MethodBuilder DefineMethod(TypeBuilder typeBuilder, MethodInfo method)
		{
			MethodAttributes attributes = method.Attributes;
			attributes = attributes & (MethodAttributes.MemberAccessMask | MethodAttributes.Private | MethodAttributes.FamANDAssem | MethodAttributes.Assembly | MethodAttributes.Family | MethodAttributes.FamORAssem | MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.Final | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.CheckAccessOnOverride | MethodAttributes.SpecialName | MethodAttributes.PinvokeImpl | MethodAttributes.UnmanagedExport | MethodAttributes.RTSpecialName | MethodAttributes.ReservedMask | MethodAttributes.HasSecurity | MethodAttributes.RequireSecObject);
			attributes = attributes | MethodAttributes.Virtual;
			ParameterInfo[] parameters = method.GetParameters();
			Type[] parameterType = new Type[(int)parameters.Length];
			for (int i = 0; i < (int)parameters.Length; i++)
			{
				parameterType[i] = parameters[i].ParameterType;
			}
			MethodBuilder methodBuilder = typeBuilder.DefineMethod(method.Name, attributes, method.ReturnType, parameterType);
			for (int j = 0; j < (int)parameters.Length; j++)
			{
				ParameterInfo parameterInfo = parameters[j];
				methodBuilder.DefineParameter(parameterInfo.Position, parameterInfo.Attributes, parameterInfo.Name);
			}
			ILGenerator lGenerator = methodBuilder.GetILGenerator();
			if (method.ReturnType == typeof(void))
			{
				lGenerator.Emit(OpCodes.Ret);
			}
			else
			{
				lGenerator.Emit(OpCodes.Newobj, typeof(NotSupportedException).GetConstructor(Type.EmptyTypes));
				lGenerator.Emit(OpCodes.Throw);
			}
			return methodBuilder;
		}

		private Type DefineOrGetExistingType(BuildingTypeInfo typeInfo)
		{
			Type designType = typeInfo.DesignType;
			if (designType == null || typeInfo.IsReplacement && !typeInfo.IsReplacementCreated)
			{
				designType = this.DefineDesignType(typeInfo);
			}
			return designType;
		}

		private Type DefinePlainType(BuildingTypeInfo typeInfo)
		{
			TypeBuilder typeBuilder;
			TypeAttributes attributes = TypeAttributes.Public | typeInfo.SourceType.Attributes & (TypeAttributes.VisibilityMask | TypeAttributes.Public | TypeAttributes.NestedPublic | TypeAttributes.NestedPrivate | TypeAttributes.NestedFamily | TypeAttributes.NestedAssembly | TypeAttributes.NestedFamANDAssem | TypeAttributes.NestedFamORAssem | TypeAttributes.LayoutMask | TypeAttributes.SequentialLayout | TypeAttributes.ExplicitLayout | TypeAttributes.ClassSemanticsMask | TypeAttributes.Interface | TypeAttributes.Abstract | TypeAttributes.Sealed | TypeAttributes.SpecialName | TypeAttributes.Import | TypeAttributes.Serializable | TypeAttributes.WindowsRuntime | TypeAttributes.StringFormatMask | TypeAttributes.UnicodeClass | TypeAttributes.AutoClass | TypeAttributes.CustomFormatClass | TypeAttributes.CustomFormatMask | TypeAttributes.BeforeFieldInit) & (TypeAttributes.VisibilityMask | TypeAttributes.Public | TypeAttributes.NestedPublic | TypeAttributes.NestedPrivate | TypeAttributes.NestedFamily | TypeAttributes.NestedAssembly | TypeAttributes.NestedFamANDAssem | TypeAttributes.NestedFamORAssem | TypeAttributes.LayoutMask | TypeAttributes.SequentialLayout | TypeAttributes.ExplicitLayout | TypeAttributes.ClassSemanticsMask | TypeAttributes.Interface | TypeAttributes.Abstract | TypeAttributes.Sealed | TypeAttributes.SpecialName | TypeAttributes.Import | TypeAttributes.Serializable | TypeAttributes.WindowsRuntime | TypeAttributes.StringFormatMask | TypeAttributes.UnicodeClass | TypeAttributes.AutoClass | TypeAttributes.CustomFormatClass | TypeAttributes.CustomFormatMask | TypeAttributes.BeforeFieldInit | TypeAttributes.ReservedMask | TypeAttributes.RTSpecialName | TypeAttributes.HasSecurity);
			if (!typeInfo.SourceType.IsInterface)
			{
				attributes = attributes & (TypeAttributes.VisibilityMask | TypeAttributes.Public | TypeAttributes.NestedPublic | TypeAttributes.NestedPrivate | TypeAttributes.NestedFamily | TypeAttributes.NestedAssembly | TypeAttributes.NestedFamANDAssem | TypeAttributes.NestedFamORAssem | TypeAttributes.LayoutMask | TypeAttributes.SequentialLayout | TypeAttributes.ExplicitLayout | TypeAttributes.ClassSemanticsMask | TypeAttributes.Interface | TypeAttributes.Sealed | TypeAttributes.SpecialName | TypeAttributes.Import | TypeAttributes.Serializable | TypeAttributes.WindowsRuntime | TypeAttributes.StringFormatMask | TypeAttributes.UnicodeClass | TypeAttributes.AutoClass | TypeAttributes.CustomFormatClass | TypeAttributes.CustomFormatMask | TypeAttributes.BeforeFieldInit | TypeAttributes.ReservedMask | TypeAttributes.RTSpecialName | TypeAttributes.HasSecurity);
			}
			if (!typeInfo.SourceType.IsNested)
			{
				string str = DesignTypeGenerator.CreateTypeName(typeInfo.SourceType);
				typeBuilder = RuntimeGeneratedTypesHelper.RuntimeGeneratedTypesAssembly.DefineType(str, attributes);
			}
			else
			{
				Type declaringType = typeInfo.SourceType.DeclaringType;
				if (declaringType == null || declaringType.IsGenericParameter || declaringType.IsGenericTypeDefinition)
				{
					typeInfo.IsReplacement = true;
					typeInfo.IsReplacementCreated = true;
					return this.objectType;
				}
				BuildingTypeInfo item = this.buildingTypes[typeInfo.SourceType.DeclaringType];
				TypeBuilder typeBuilder1 = this.DefineOrGetExistingType(item) as TypeBuilder;
				if (typeBuilder1 == null)
				{
					return this.objectType;
				}
				typeBuilder = typeBuilder1.DefineNestedType(typeInfo.SourceType.Name, attributes);
			}
			typeInfo.DesignType = typeBuilder;
			if (typeInfo.BaseTypeInfo != null)
			{
				Type type = this.DefineOrGetExistingType(typeInfo.BaseTypeInfo);
				if (type != null)
				{
					typeBuilder.SetParent(type);
				}
			}
			if (!typeInfo.SourceType.IsInterface)
			{
				typeInfo.Constructor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, Type.EmptyTypes);
				DesignTypeGenerator.DefineAbstractMethods(typeInfo);
				this.DefineCommandMethods(typeInfo);
			}
			return typeBuilder;
		}

		private void DefineProperty(PropertyInfo propertyInfo, BuildingTypeInfo typeInfo, ILGenerator ilConstructor, bool hide)
		{
			if (typeInfo.DesignType.IsInterface)
			{
				return;
			}
			if (propertyInfo.PropertyType.IsGenericParameter)
			{
				return;
			}
			ParameterInfo[] indexParameters = propertyInfo.GetIndexParameters();
			Type[] designType = new Type[0];
			if ((indexParameters == null ? false : (int)indexParameters.Length > 0))
			{
				designType = new Type[(int)indexParameters.Length];
				for (int i = 0; i < (int)indexParameters.Length; i++)
				{
					ParameterInfo parameterInfo = indexParameters[i];
					BuildingTypeInfo item = this.buildingTypes[parameterInfo.ParameterType];
					designType[i] = item.DesignType;
				}
			}
			bool flag = DesignTypeGenerator.IsPropertyStatic(propertyInfo);
			bool flag1 = DesignTypeGenerator.IsPropertyVirtual(propertyInfo);
			CallingConventions callingConvention = CallingConventions.Standard;
			if (!flag)
			{
				callingConvention = callingConvention | CallingConventions.HasThis;
			}
			BuildingTypeInfo buildingTypeInfo = this.buildingTypes[propertyInfo.PropertyType];
			TypeBuilder typeBuilder = (TypeBuilder)typeInfo.DesignType;
			PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(propertyInfo.Name, PropertyAttributes.None, callingConvention, buildingTypeInfo.DesignType, null, null, designType, null, null);
			DesignTypeGenerator.DefineCustomAttributes(propertyInfo, propertyBuilder, hide);
			MethodAttributes methodAttribute = MethodAttributes.Public;
			if (flag)
			{
				methodAttribute = methodAttribute | MethodAttributes.Static;
			}
			else if (flag1)
			{
				methodAttribute = methodAttribute | MethodAttributes.Virtual;
			}
			FieldBuilder fieldBuilder = this.DefineField(propertyInfo, typeInfo, ilConstructor);
			MethodBuilder methodBuilder = this.GenerateGetMethod(typeBuilder, propertyInfo, fieldBuilder, methodAttribute);
			propertyBuilder.SetGetMethod(methodBuilder);
			typeInfo.AddMethod(methodBuilder, null);
			MethodBuilder methodBuilder1 = this.GenerateSetMethod(typeBuilder, propertyInfo, fieldBuilder, methodAttribute);
			propertyBuilder.SetSetMethod(methodBuilder1);
			typeInfo.AddMethod(methodBuilder1, null);
			typeInfo.AddProperty(propertyBuilder, propertyInfo);
		}

		private Type DefineReplacementType(BuildingTypeInfo typeInfo)
		{
			Type[] genericArguments;
			if (typeInfo.IsReplacementCreated)
			{
				return typeInfo.DesignType;
			}
			typeInfo.IsReplacementCreated = true;
			if (!typeInfo.DesignType.IsGenericType)
			{
				return typeInfo.DesignType;
			}
			if (!typeInfo.SourceType.IsGenericType && !typeInfo.SourceType.IsArray)
			{
				return typeInfo.DesignType;
			}
			if (!typeInfo.SourceType.IsArray)
			{
				genericArguments = typeInfo.SourceType.GetGenericArguments();
			}
			else
			{
				Type[] arrayItemType = new Type[] { DesignTypeGenerator.GetArrayItemType(typeInfo.SourceType) };
				genericArguments = arrayItemType;
			}
			Type[] typeArray = new Type[(int)genericArguments.Length];
			for (int i = 0; i < (int)genericArguments.Length; i++)
			{
				BuildingTypeInfo item = this.buildingTypes[genericArguments[i]];
				typeArray[i] = this.DefineOrGetExistingType(item);
			}
			Type genericTypeDefinition = typeInfo.DesignType.GetGenericTypeDefinition();
			typeInfo.DesignType = genericTypeDefinition.MakeGenericType(typeArray);
			return typeInfo.DesignType;
		}

		private void DefineTypes()
		{
			List<BuildingTypeInfo> buildingTypeInfos = new List<BuildingTypeInfo>(this.buildingTypes.Values);
			for (int i = 0; i < buildingTypeInfos.Count; i++)
			{
				this.DefineOrGetExistingType(buildingTypeInfos[i]);
			}
		}

		private void FailBuildingTypes(Exception e, string context)
		{
			foreach (BuildingTypeInfo value in this.buildingTypes.Values)
			{
				if (!(value.DesignType is TypeBuilder) || DesignTypeGenerator.designTimeTypes.ContainsKey(value.SourceType))
				{
					continue;
				}
				DesignTypeGenerator.designTimeTypes[value.SourceType] = new DesignTypeResult(value.SourceType, e, context);
			}
		}

		private void FillInTypes()
		{
			List<BuildingTypeInfo> buildingTypeInfos = new List<BuildingTypeInfo>(this.buildingTypes.Values);
			for (int i = 0; i < buildingTypeInfos.Count; i++)
			{
				BuildingTypeInfo item = buildingTypeInfos[i];
				this.typeContext.Push(item.SourceType.FullName);
				this.CopyClassImpl(item);
				this.typeContext.Pop();
			}
		}

		private bool FinalizeClass(BuildingTypeInfo typeInfo, bool createType)
		{
			if (typeInfo == null || typeInfo.DesignType == null || this.finalizedTypes.Contains(typeInfo))
			{
				return false;
			}
			TypeBuilder designType = typeInfo.DesignType as TypeBuilder;
			if (createType && designType == null)
			{
				return false;
			}
			this.finalizedTypes.Add(typeInfo);
			if (!typeInfo.DesignType.IsInterface || typeInfo.DesignType.IsGenericType)
			{
				if (!typeInfo.IsReplacement && typeInfo.BaseTypeInfo != null && !typeInfo.BaseTypeInfo.IsReplacement)
				{
					this.FinalizeClass(typeInfo.BaseTypeInfo, createType);
				}
				this.FinalizeGenericArguments(typeInfo.SourceType, createType);
				Type[] implementedInterfaces = DesignTypeGenerator.GetImplementedInterfaces(typeInfo.SourceType);
				for (int i = 0; i < (int)implementedInterfaces.Length; i++)
				{
					this.FinalizeGenericArguments(implementedInterfaces[i], createType);
				}
			}
			if (designType == null)
			{
				return false;
			}
			if (!createType)
			{
				this.orderedTypes.Add(typeInfo);
				return true;
			}
			try
			{
				Type type = designType.CreateType();
				if (type != null)
				{
					typeInfo.DesignType = type;
					DesignTypeGenerator.CacheResult(typeInfo.SourceType, typeInfo.DesignType);
					return true;
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				DesignTypeGenerator.designTimeTypes[typeInfo.SourceType] = new DesignTypeResult(typeInfo.SourceType, exception, typeInfo.SourceType.FullName);
			}
			return false;
		}

		private void FinalizeGenericArguments(Type sourceType, bool createType)
		{
			BuildingTypeInfo buildingTypeInfo;
			if (sourceType.IsGenericType && !sourceType.IsGenericTypeDefinition)
			{
				Type[] genericArguments = sourceType.GetGenericArguments();
				for (int i = 0; i < (int)genericArguments.Length; i++)
				{
					Type type = genericArguments[i];
					if (this.buildingTypes.TryGetValue(type, out buildingTypeInfo))
					{
						this.FinalizeClass(buildingTypeInfo, createType);
					}
				}
			}
		}

		private void FinalizeTypes()
		{
			List<BuildingTypeInfo> buildingTypeInfos = new List<BuildingTypeInfo>(this.buildingTypes.Values);
			this.PreorderTypes(buildingTypeInfos);
			this.PreorderTypes(buildingTypeInfos);
			do
			{
				this.finalizedTypes.Clear();
			}
			while (this.FinalizeTypes(buildingTypeInfos, true) > 0);
			List<BuildingTypeInfo> buildingTypeInfos1 = new List<BuildingTypeInfo>();
			foreach (BuildingTypeInfo value in this.buildingTypes.Values)
			{
				if (!(value.DesignType != null) || !(value.SourceType != value.DesignType) || !value.DesignType.IsGenericType && !value.DesignType.IsArray)
				{
					continue;
				}
				if (!value.IsReplacement)
				{
					value.DesignType = null;
				}
				else
				{
					value.IsReplacementCreated = false;
				}
				buildingTypeInfos1.Add(value);
			}
			foreach (BuildingTypeInfo buildingTypeInfo in buildingTypeInfos1)
			{
				buildingTypeInfo.DesignType = this.DefineDesignTypeInternal(buildingTypeInfo);
				DesignTypeGenerator.CacheResult(buildingTypeInfo.SourceType, buildingTypeInfo.DesignType);
			}
		}

		private int FinalizeTypes(List<BuildingTypeInfo> orderedTypes, bool createType)
		{
			int num = 0 + this.FinalizeTypes(orderedTypes, createType, (BuildingTypeInfo typeInfo) => typeInfo.DesignType.IsInterface);
			num = num + this.FinalizeTypes(orderedTypes, createType, (BuildingTypeInfo typeInfo) => typeInfo.DesignType.IsValueType);
			num = num + this.FinalizeTypes(orderedTypes, createType, (BuildingTypeInfo typeInfo) => typeInfo.DesignType.IsGenericType);
			return num + this.FinalizeTypes(orderedTypes, createType, (BuildingTypeInfo typeInfo) => true);
		}

		private int FinalizeTypes(List<BuildingTypeInfo> orderedTypes, bool createType, Predicate<BuildingTypeInfo> criteria)
		{
			int num = 0;
			for (int i = 0; i < orderedTypes.Count; i++)
			{
				BuildingTypeInfo item = orderedTypes[i];
				if (criteria(item) && this.FinalizeClass(item, createType))
				{
					num++;
				}
			}
			return num;
		}

		private DesignTypeResult FindFailedType()
		{
			DesignTypeResult designTypeResult;
			Dictionary<Type, BuildingTypeInfo>.KeyCollection.Enumerator enumerator = this.buildingTypes.Keys.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					DesignTypeResult cachedResult = DesignTypeGenerator.GetCachedResult(enumerator.Current);
					if (cachedResult == null || !cachedResult.IsFailed)
					{
						continue;
					}
					designTypeResult = cachedResult;
					return designTypeResult;
				}
				return null;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return designTypeResult;
		}

		private MethodBuilder GenerateGetMethod(TypeBuilder typeBuilder, PropertyInfo propertyInfo, FieldBuilder field, MethodAttributes attrs)
		{
			MethodInfo getMethod = propertyInfo.GetGetMethod(true);
			Type[] designType = new Type[0];
			if (getMethod != null)
			{
				ParameterInfo[] parameters = getMethod.GetParameters();
				if (parameters != null && (int)parameters.Length > 0)
				{
					designType = new Type[(int)parameters.Length];
					for (int i = 0; i < (int)parameters.Length; i++)
					{
						ParameterInfo parameterInfo = parameters[i];
						BuildingTypeInfo item = this.buildingTypes[parameterInfo.ParameterType];
						designType[i] = item.DesignType;
					}
				}
			}
			string str = string.Concat("get_", propertyInfo.Name);
			Type type = this.buildingTypes[propertyInfo.PropertyType].DesignType;
			MethodBuilder methodBuilder = typeBuilder.DefineMethod(str, attrs, type, designType);
			ILGenerator lGenerator = methodBuilder.GetILGenerator();
			if (!field.IsStatic)
			{
				lGenerator.Emit(OpCodes.Ldarg_0);
				lGenerator.Emit(OpCodes.Ldfld, field);
			}
			else
			{
				lGenerator.Emit(OpCodes.Ldsfld, field);
			}
			lGenerator.Emit(OpCodes.Ret);
			return methodBuilder;
		}

		private MethodBuilder GenerateSetMethod(TypeBuilder typeBuilder, PropertyInfo propertyInfo, FieldBuilder field, MethodAttributes attrs)
		{
			Type designType = this.buildingTypes[propertyInfo.PropertyType].DesignType;
			MethodInfo setMethod = propertyInfo.GetSetMethod(true);
			Type[] typeArray = null;
			if (setMethod == null)
			{
				typeArray = new Type[] { designType };
			}
			else
			{
				ParameterInfo[] parameters = setMethod.GetParameters();
				typeArray = new Type[0];
				if (parameters != null && (int)parameters.Length > 0)
				{
					typeArray = new Type[(int)parameters.Length];
					for (int i = 0; i < (int)parameters.Length; i++)
					{
						ParameterInfo parameterInfo = parameters[i];
						BuildingTypeInfo item = this.buildingTypes[parameterInfo.ParameterType];
						typeArray[i] = item.DesignType;
					}
				}
			}
			string str = string.Concat("set_", propertyInfo.Name);
			MethodBuilder methodBuilder = typeBuilder.DefineMethod(str, attrs, null, typeArray);
			ILGenerator lGenerator = methodBuilder.GetILGenerator();
			if (!field.IsStatic)
			{
				lGenerator.Emit(OpCodes.Ldarg_0);
				lGenerator.Emit(OpCodes.Ldarg_1);
				lGenerator.Emit(OpCodes.Stfld, field);
			}
			else
			{
				lGenerator.Emit(OpCodes.Ldarg_0);
				lGenerator.Emit(OpCodes.Stsfld, field);
			}
			foreach (IDesignTypeGeneratorExtension extension in this.extensions)
			{
				extension.OnPropertySet(this, typeBuilder, propertyInfo, lGenerator);
			}
			lGenerator.Emit(OpCodes.Ret);
			return methodBuilder;
		}

		public static Type GetArrayItemType(Type arrayType)
		{
			return arrayType.GetElementType();
		}

		private static Type GetCachedDesignType(Type sourceType)
		{
			DesignTypeResult cachedResult = DesignTypeGenerator.GetCachedResult(sourceType);
			if (cachedResult == null)
			{
				return null;
			}
			return cachedResult.DesignType;
		}

		private static DesignTypeResult GetCachedResult(Type sourceType)
		{
			DesignTypeResult designTypeResult;
			DesignTypeGenerator.designTimeTypes.TryGetValue(sourceType, out designTypeResult);
			return designTypeResult;
		}

		public static MethodInfo[] GetCommandMethods(Type type, IPlatformMetadata platformMetadata, bool includeInheritedMethods)
		{
			if (type == null)
			{
				return new MethodInfo[0];
			}
			BindingFlags bindingFlag = BindingFlags.Instance | BindingFlags.Public;
			if (!includeInheritedMethods)
			{
				bindingFlag = bindingFlag | BindingFlags.DeclaredOnly;
			}
			List<MethodInfo> methodInfos = new List<MethodInfo>();
			try
			{
				MethodInfo[] methods = type.GetMethods(bindingFlag);
				for (int i = 0; i < (int)methods.Length; i++)
				{
					MethodInfo methodInfo = methods[i];
					if (methodInfo.ReturnType == typeof(void))
					{
						ParameterInfo[] parameters = methodInfo.GetParameters();
						bool flag = false;
						if ((int)parameters.Length == 0)
						{
							flag = true;
						}
						else if ((int)parameters.Length == 2 && DesignTypeGenerator.VerifyParamType(PlatformTypes.Object, parameters[0], platformMetadata, true) && DesignTypeGenerator.VerifyParamType(PlatformTypes.EventArgs, parameters[1], platformMetadata, false))
						{
							flag = true;
						}
						if (flag)
						{
							methodInfos.Add(methodInfo);
						}
					}
				}
			}
			catch
			{
			}
			return methodInfos.ToArray();
		}

		private static ConstructorInfo GetDefaultConstructor(Type type, bool canUseProtectedConstructor)
		{
			BindingFlags bindingFlag = BindingFlags.Instance | BindingFlags.Public;
			if (canUseProtectedConstructor)
			{
				bindingFlag = bindingFlag | BindingFlags.NonPublic;
			}
			ConstructorInfo constructor = type.GetConstructor(bindingFlag, null, Type.EmptyTypes, null);
			if (constructor != null && !constructor.IsPublic && !constructor.IsFamily)
			{
				constructor = null;
			}
			return constructor;
		}

		public DesignTypeResult GetDesignType(Type targetType)
		{
			DesignTypeResult designTypeInternal;
			lock (DesignTypeGenerator._sync)
			{
				designTypeInternal = this.GetDesignTypeInternal(targetType);
			}
			return designTypeInternal;
		}

		private ConstructorInfo GetDesignTypeConstructorInfo(BuildingTypeInfo typeInfo, bool canUseProtectedConstructor)
		{
			ConstructorInfo constructorInfo;
			BuildingTypeInfo buildingTypeInfo;
			if (typeInfo.Constructor != null)
			{
				return typeInfo.Constructor;
			}
			if (typeInfo.DesignType.IsInterface || typeInfo.DesignType.IsGenericParameter)
			{
				return null;
			}
			try
			{
				typeInfo.Constructor = DesignTypeGenerator.GetDefaultConstructor(typeInfo.DesignType, canUseProtectedConstructor);
			}
			catch (NotSupportedException notSupportedException)
			{
				if (typeInfo.DesignType.IsGenericType)
				{
					Type genericTypeDefinition = typeInfo.DesignType.GetGenericTypeDefinition();
					constructorInfo = (this.buildingTypes.TryGetValue(genericTypeDefinition, out buildingTypeInfo) ? this.GetDesignTypeConstructorInfo(buildingTypeInfo, canUseProtectedConstructor) : DesignTypeGenerator.GetDefaultConstructor(genericTypeDefinition, canUseProtectedConstructor));
					typeInfo.Constructor = TypeBuilder.GetConstructor(typeInfo.DesignType, constructorInfo);
				}
			}
			return typeInfo.Constructor;
		}

		private Type GetDesignTypeFromExtensions(Type sourceType)
		{
			if (sourceType == this.objectType)
			{
				return sourceType;
			}
			Type designType = null;
			for (int i = 0; i < this.extensions.Count && designType == null; i++)
			{
				IDesignTypeGeneratorExtension item = this.extensions[i];
				designType = item.GetDesignType(this, sourceType);
			}
			return designType;
		}

		private DesignTypeResult GetDesignTypeInternal(Type targetType)
		{
			DesignTypeResult designTypeResult;
			DesignTypeResult cachedResult = DesignTypeGenerator.GetCachedResult(targetType);
			if (cachedResult != null)
			{
				return cachedResult;
			}
			if (targetType == null || typeof(MarkupExtension).IsAssignableFrom(targetType))
			{
				return new DesignTypeResult(targetType, targetType);
			}
			if (targetType.Assembly.Equals(RuntimeGeneratedTypesHelper.RuntimeGeneratedTypesAssembly.Assembly))
			{
				return new DesignTypeResult(targetType, targetType);
			}
			if (this.GetDesignTypeFromExtensions(targetType) != null)
			{
				return new DesignTypeResult(targetType, targetType);
			}
			this.Reset();
			try
			{
				this.CollectSourceTypes(targetType);
				DesignTypeResult designTypeResult1 = this.FindFailedType();
				if (designTypeResult1 == null)
				{
					this.DefineTypes();
					this.FillInTypes();
					this.FinalizeTypes();
					cachedResult = DesignTypeGenerator.GetCachedResult(targetType);
					return cachedResult;
				}
				else
				{
					cachedResult = new DesignTypeResult(targetType, designTypeResult1.TypeGenerationException, designTypeResult1.Context);
					DesignTypeGenerator.designTimeTypes[targetType] = cachedResult;
					designTypeResult = cachedResult;
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				this.FailBuildingTypes(exception, this.CurrentContext);
				cachedResult = DesignTypeGenerator.GetCachedResult(targetType) ?? new DesignTypeResult(targetType, exception, this.CurrentContext);
				designTypeResult = cachedResult;
			}
			return designTypeResult;
		}

		private Type GetExistingDesignType(Type sourceType)
		{
			if (sourceType == null)
			{
				return null;
			}
			if (!sourceType.IsGenericParameter)
			{
				Type cachedDesignType = DesignTypeGenerator.GetCachedDesignType(sourceType);
				if (cachedDesignType == null)
				{
					cachedDesignType = this.GetDesignTypeFromExtensions(sourceType);
				}
				return cachedDesignType;
			}
			Type existingDesignType = this.GetExistingDesignType(sourceType.DeclaringType);
			if (existingDesignType == null)
			{
				return null;
			}
			if (existingDesignType == sourceType.DeclaringType)
			{
				return sourceType;
			}
			return ((IEnumerable<Type>)existingDesignType.GetGenericArguments()).First<Type>((Type t) => t.Name == sourceType.Name);
		}

		public static Type[] GetImplementedInterfaces(Type type)
		{
			Type[] interfaces = type.GetInterfaces();
			Type[] typeArray = (type.BaseType != null ? type.BaseType.GetInterfaces() : Type.EmptyTypes);
			if ((int)typeArray.Length == 0)
			{
				return interfaces;
			}
			if ((int)typeArray.Length == (int)interfaces.Length)
			{
				return Type.EmptyTypes;
			}
			List<Type> types = new List<Type>(interfaces);
			Type[] typeArray1 = typeArray;
			for (int i = 0; i < (int)typeArray1.Length; i++)
			{
				types.Remove(typeArray1[i]);
			}
			return types.ToArray();
		}

		private static Type GetNormalizedType(Type sourceType)
		{
			if (sourceType == null)
			{
				return null;
			}
			Type type = sourceType;
			if (sourceType.IsByRef)
			{
				type = sourceType.Assembly.GetType(sourceType.FullName.Replace("&", ""), false, false);
			}
			return type;
		}

		private IEnumerable<PropertyInfo> GetReflectableProperties(BuildingTypeInfo typeInfo)
		{
			PropertyInfo[] properties = typeInfo.SourceType.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
			return 
				from prop in properties
				where this.IsPropertyReflectable(typeInfo, prop)
				select prop;
		}

		private Type GetReplacementType(Type sourceType)
		{
			Type replacementType = null;
			List<IDesignTypeGeneratorExtension>.Enumerator enumerator = this.extensions.GetEnumerator();
			try
			{
				do
				{
					if (!enumerator.MoveNext())
					{
						break;
					}
					replacementType = enumerator.Current.GetReplacementType(sourceType);
				}
				while (replacementType == null);
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			if (replacementType == null)
			{
				return null;
			}
			Type type = replacementType;
			if ((sourceType.IsGenericType || sourceType.IsArray) && replacementType.IsGenericType)
			{
				if (!replacementType.IsGenericTypeDefinition)
				{
					replacementType = replacementType.GetGenericTypeDefinition();
				}
				Type[] arrayItemType = null;
				if (sourceType.IsArray)
				{
					arrayItemType = new Type[] { DesignTypeGenerator.GetArrayItemType(sourceType) };
				}
				else if (sourceType.IsGenericType)
				{
					arrayItemType = sourceType.GetGenericArguments();
				}
				if (arrayItemType != null)
				{
					type = replacementType.MakeGenericType(arrayItemType);
				}
			}
			if (type.IsGenericTypeDefinition)
			{
				return null;
			}
			return type;
		}

		private DesignTypeGenerator.BaseType GetSourceBaseType(Type sourceType)
		{
			Type baseType = sourceType.BaseType;
			if (baseType == null)
			{
				return new DesignTypeGenerator.BaseType(baseType, false);
			}
			if (baseType != this.objectType)
			{
				if (!baseType.IsGenericTypeDefinition)
				{
					return new DesignTypeGenerator.BaseType(baseType, false);
				}
				return new DesignTypeGenerator.BaseType(this.objectType, false);
			}
			Type genericTypeDefinition = sourceType;
			if (sourceType.IsGenericType)
			{
				genericTypeDefinition = sourceType.GetGenericTypeDefinition();
			}
			genericTypeDefinition = this.GetDesignTypeFromExtensions(genericTypeDefinition);
			if (genericTypeDefinition != null)
			{
				return new DesignTypeGenerator.BaseType(this.objectType, false);
			}
			Type[] implementedInterfaces = DesignTypeGenerator.GetImplementedInterfaces(sourceType);
			for (int i = 0; i < (int)implementedInterfaces.Length; i++)
			{
				Type replacementType = this.GetReplacementType(implementedInterfaces[i]);
				if (replacementType != null && !replacementType.IsInterface)
				{
					return new DesignTypeGenerator.BaseType(replacementType, true);
				}
			}
			return new DesignTypeGenerator.BaseType(this.objectType, false);
		}

		public DesignTypeResult GetXamlFriendlyListType(Type sourceType)
		{
			DesignTypeResult designTypeResult;
			lock (DesignTypeGenerator._sync)
			{
				DesignTypeResult designTypeResult1 = null;
				if (!DesignTypeGenerator.xamlFirendlyListTypes.TryGetValue(sourceType, out designTypeResult1))
				{
					try
					{
						Type type = typeof(List<>).MakeGenericType(new Type[] { sourceType });
						string str = DesignTypeGenerator.CreateTypeName(type);
						TypeAttributes attributes = TypeAttributes.Public | sourceType.Attributes & (TypeAttributes.VisibilityMask | TypeAttributes.Public | TypeAttributes.NestedPublic | TypeAttributes.NestedPrivate | TypeAttributes.NestedFamily | TypeAttributes.NestedAssembly | TypeAttributes.NestedFamANDAssem | TypeAttributes.NestedFamORAssem | TypeAttributes.LayoutMask | TypeAttributes.SequentialLayout | TypeAttributes.ExplicitLayout | TypeAttributes.ClassSemanticsMask | TypeAttributes.Interface | TypeAttributes.Abstract | TypeAttributes.Sealed | TypeAttributes.SpecialName | TypeAttributes.Import | TypeAttributes.Serializable | TypeAttributes.WindowsRuntime | TypeAttributes.StringFormatMask | TypeAttributes.UnicodeClass | TypeAttributes.AutoClass | TypeAttributes.CustomFormatClass | TypeAttributes.CustomFormatMask | TypeAttributes.BeforeFieldInit) & (TypeAttributes.VisibilityMask | TypeAttributes.Public | TypeAttributes.NestedPublic | TypeAttributes.NestedPrivate | TypeAttributes.NestedFamily | TypeAttributes.NestedAssembly | TypeAttributes.NestedFamANDAssem | TypeAttributes.NestedFamORAssem | TypeAttributes.LayoutMask | TypeAttributes.SequentialLayout | TypeAttributes.ExplicitLayout | TypeAttributes.ClassSemanticsMask | TypeAttributes.Interface | TypeAttributes.Sealed | TypeAttributes.SpecialName | TypeAttributes.Import | TypeAttributes.Serializable | TypeAttributes.WindowsRuntime | TypeAttributes.StringFormatMask | TypeAttributes.UnicodeClass | TypeAttributes.AutoClass | TypeAttributes.CustomFormatClass | TypeAttributes.CustomFormatMask | TypeAttributes.BeforeFieldInit | TypeAttributes.ReservedMask | TypeAttributes.RTSpecialName | TypeAttributes.HasSecurity) & (TypeAttributes.VisibilityMask | TypeAttributes.Public | TypeAttributes.NestedPublic | TypeAttributes.NestedPrivate | TypeAttributes.NestedFamily | TypeAttributes.NestedAssembly | TypeAttributes.NestedFamANDAssem | TypeAttributes.NestedFamORAssem | TypeAttributes.LayoutMask | TypeAttributes.SequentialLayout | TypeAttributes.ExplicitLayout | TypeAttributes.ClassSemanticsMask | TypeAttributes.Interface | TypeAttributes.Abstract | TypeAttributes.Sealed | TypeAttributes.SpecialName | TypeAttributes.Import | TypeAttributes.Serializable | TypeAttributes.WindowsRuntime | TypeAttributes.StringFormatMask | TypeAttributes.UnicodeClass | TypeAttributes.AutoClass | TypeAttributes.CustomFormatClass | TypeAttributes.CustomFormatMask | TypeAttributes.BeforeFieldInit | TypeAttributes.ReservedMask | TypeAttributes.RTSpecialName | TypeAttributes.HasSecurity);
						TypeBuilder typeBuilder = RuntimeGeneratedTypesHelper.RuntimeGeneratedTypesAssembly.DefineType(str, attributes, type);
						designTypeResult1 = new DesignTypeResult(sourceType, typeBuilder.CreateType());
					}
					catch (Exception exception)
					{
						designTypeResult1 = new DesignTypeResult(sourceType, exception, string.Empty);
					}
					DesignTypeGenerator.xamlFirendlyListTypes[sourceType] = designTypeResult1;
					designTypeResult = designTypeResult1;
				}
				else
				{
					designTypeResult = designTypeResult1;
				}
			}
			return designTypeResult;
		}

		private bool IsPropertyReflectable(BuildingTypeInfo typeInfo, PropertyInfo propertyInfo)
		{
			bool flag;
			MethodInfo getMethod = propertyInfo.GetGetMethod();
			if (getMethod == null || !getMethod.IsPublic)
			{
				return false;
			}
			if (propertyInfo.Name.IndexOf('.') >= 0)
			{
				return false;
			}
			List<IDesignTypeGeneratorExtension>.Enumerator enumerator = this.extensions.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.ShouldReflectProperty(this, typeInfo, propertyInfo))
					{
						continue;
					}
					flag = false;
					return flag;
				}
				return true;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return flag;
		}

		private static bool IsPropertyReplaced(BuildingTypeInfo baseType, PropertyInfo sourceProperty)
		{
			if (baseType != null && baseType.IsReplacement)
			{
				PropertyInfo property = baseType.SourceType.GetProperty(sourceProperty.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (property != null && property.PropertyType == sourceProperty.PropertyType)
				{
					return true;
				}
			}
			return false;
		}

		private static bool IsPropertyStatic(PropertyInfo propertyInfo)
		{
			MethodInfo getMethod = propertyInfo.GetGetMethod();
			if (getMethod == null)
			{
				return false;
			}
			return getMethod.IsStatic;
		}

		private static bool IsPropertyVirtual(PropertyInfo propertyInfo)
		{
			MethodInfo getMethod = propertyInfo.GetGetMethod();
			if (getMethod == null)
			{
				return false;
			}
			return getMethod.IsVirtual;
		}

		public static DesignTypeResult LookupGetDesignTypeResult(Type sourceType)
		{
			DesignTypeResult designTypeResult;
			DesignTypeResult designTypeResult1;
			lock (DesignTypeGenerator._sync)
			{
				DesignTypeGenerator.designTimeTypes.TryGetValue(sourceType, out designTypeResult);
				designTypeResult1 = designTypeResult;
			}
			return designTypeResult1;
		}

		Type Microsoft.Expression.DesignModel.Metadata.IDesignTypeGeneratorContext.GetDesignType(Type type)
		{
			BuildingTypeInfo buildingTypeInfo;
			Type existingDesignType = this.GetExistingDesignType(type);
			if (existingDesignType == null && this.buildingTypes.TryGetValue(type, out buildingTypeInfo))
			{
				existingDesignType = buildingTypeInfo.DesignType;
			}
			return existingDesignType;
		}

		private static string NormalizeTypeName(Type type, List<Type> visitedTypes)
		{
			string name = type.Name;
			for (int i = name.IndexOfAny(DesignTypeGenerator.xamlUnfiendlyChars); i >= 0; i = name.IndexOfAny(DesignTypeGenerator.xamlUnfiendlyChars))
			{
				char chr = name[i];
				name = (chr != '\u0060' ? name.Replace(chr, '\u005F') : name.Substring(0, i));
			}
			if (type.IsGenericType)
			{
				Type[] genericArguments = type.GetGenericArguments();
				for (int j = 0; j < (int)genericArguments.Length; j++)
				{
					Type type1 = genericArguments[j];
					if (!visitedTypes.Contains(type1))
					{
						visitedTypes.Add(type1);
						name = string.Concat(name, "_", DesignTypeGenerator.NormalizeTypeName(type1, visitedTypes));
					}
				}
			}
			return name;
		}

		private void PreorderTypes(List<BuildingTypeInfo> types)
		{
			this.FinalizeTypes(types, false);
			types.Clear();
			types.AddRange(this.orderedTypes);
			this.orderedTypes.Clear();
			this.finalizedTypes.Clear();
		}

		private void Reset()
		{
			this.buildingTypes.Clear();
			this.finalizedTypes.Clear();
			this.orderedTypes.Clear();
			this.recursionGuard.Clear();
		}

		private bool ShouldInitializeProperty(PropertyInfo propertyInfo, Type designPropertyType)
		{
			BuildingTypeInfo buildingTypeInfo;
			bool flag;
			bool flag1 = false;
			try
			{
				Type propertyType = propertyInfo.PropertyType;
				if (propertyType.IsArray && designPropertyType.IsArray || propertyType.IsGenericTypeDefinition || propertyType.IsGenericParameter || propertyType.IsByRef)
				{
					flag = false;
				}
				else if (propertyType.IsEnum)
				{
					flag = true;
				}
				else if (!propertyType.IsInterface)
				{
					flag1 = true;
					for (Type i = propertyType; i != null && this.buildingTypes.TryGetValue(i, out buildingTypeInfo); i = i.BaseType)
					{
						if (buildingTypeInfo.DesignType is TypeBuilder)
						{
							if (!buildingTypeInfo.DesignType.Assembly.Equals(RuntimeGeneratedTypesHelper.RuntimeGeneratedTypesAssembly.Assembly))
							{
								break;
							}
							foreach (PropertyInfo reflectableProperty in this.GetReflectableProperties(buildingTypeInfo))
							{
								if (!this.buildingTypes[reflectableProperty.PropertyType].DesignType.Assembly.Equals(RuntimeGeneratedTypesHelper.RuntimeGeneratedTypesAssembly.Assembly))
								{
									continue;
								}
								flag1 = false;
								i = this.objectType;
								break;
							}
						}
					}
					return flag1;
				}
				else
				{
					flag = (designPropertyType.IsInterface ? false : true);
				}
			}
			catch (NotImplementedException notImplementedException)
			{
				return flag1;
			}
			catch (NotSupportedException notSupportedException)
			{
				return flag1;
			}
			return flag;
		}

		private static bool VerifyParamType(ITypeId expectedType, ParameterInfo parameter, IPlatformMetadata platformMetadata, bool exactMatch)
		{
			IType type = platformMetadata.ResolveType(expectedType);
			if (platformMetadata.IsNullType(expectedType))
			{
				return false;
			}
			if (exactMatch)
			{
				return type.RuntimeType.Equals(parameter.ParameterType);
			}
			return type.RuntimeType.IsAssignableFrom(parameter.ParameterType);
		}

		[DebuggerDisplay("{Type}")]
		private struct BaseType
		{
			public bool IsReplacement
			{
				get;
				set;
			}

			public Type Type
			{
				get;
				set;
			}

			public BaseType(Type type, bool isReplacement)
			{
				this = new DesignTypeGenerator.BaseType()
				{
					Type = type,
					IsReplacement = isReplacement
				};
			}
		}
	}
}