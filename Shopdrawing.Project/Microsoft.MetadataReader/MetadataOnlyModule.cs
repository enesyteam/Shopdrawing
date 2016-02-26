using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Reflection.Adds;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Microsoft.MetadataReader
{
	internal class MetadataOnlyModule : Module, IModule2, IDisposable
	{
		private const BindingFlags DefaultLookup = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;

		private const BindingFlags MembersDeclaredOnTypeOnly = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

		private readonly IMetadataExtensionsPolicy m_policy;

		private readonly IReflectionFactory m_factory;

		private readonly string m_modulePath;

		private readonly MetadataFile m_metadata;

		private IMetadataImport m_cachedThreadAffinityImporter;

		private string m_scopeName;

		private Token[] m_typeCodeMapping;

		private readonly ITypeUniverse m_assemblyResolver;

		private MetadataOnlyModule.NestedTypeCache m_nestedTypeInfo;

		private System.Reflection.Assembly m_assembly;

		public override System.Reflection.Assembly Assembly
		{
			get
			{
				return this.m_assembly;
			}
		}

		public ITypeUniverse AssemblyResolver
		{
			get
			{
				return this.m_assemblyResolver;
			}
		}

		internal IReflectionFactory Factory
		{
			get
			{
				return this.m_factory;
			}
		}

		public override string FullyQualifiedName
		{
			get
			{
				return this.m_modulePath;
			}
		}

		public override int MDStreamVersion
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override int MetadataToken
		{
			get
			{
				int num;
				this.RawImport.GetModuleFromScope(out num);
				return num;
			}
		}

		public override Guid ModuleVersionId
		{
			get
			{
				int num;
				Guid guid;
				this.RawImport.GetScopeProps(null, 0, out num, out guid);
				return guid;
			}
		}

		public override string Name
		{
			get
			{
				return Path.GetFileName(this.m_modulePath);
			}
		}

		internal IMetadataExtensionsPolicy Policy
		{
			get
			{
				return this.m_policy;
			}
		}

		internal IMetadataImport RawImport
		{
			get
			{
				return this.GetThreadSafeImporter();
			}
		}

		internal MetadataFile RawMetadata
		{
			get
			{
				return this.m_metadata;
			}
		}

		public override string ScopeName
		{
			get
			{
				int num;
				Guid guid;
				if (this.m_scopeName == null)
				{
					IMetadataImport threadSafeImporter = this.GetThreadSafeImporter();
					threadSafeImporter.GetScopeProps(null, 0, out num, out guid);
					StringBuilder stringBuilder = new StringBuilder(num);
					threadSafeImporter.GetScopeProps(stringBuilder, stringBuilder.Capacity, out num, out guid);
					stringBuilder.Length = num - 1;
					this.m_scopeName = stringBuilder.ToString();
				}
				return this.m_scopeName;
			}
		}

		public MetadataOnlyModule(ITypeUniverse universe, MetadataFile import, string modulePath) : this(universe, import, new DefaultFactory(), modulePath)
		{
		}

		public MetadataOnlyModule(ITypeUniverse universe, MetadataFile import, IReflectionFactory factory, string modulePath)
		{
			this.m_assemblyResolver = universe;
			this.m_metadata = import;
			this.m_factory = factory;
			this.m_policy = new MetadataExtensionsPolicy20(universe);
			this.m_modulePath = modulePath;
			object uniqueObjectForIUnknown = Marshal.GetUniqueObjectForIUnknown(this.m_metadata.RawPtr);
			this.m_cachedThreadAffinityImporter = (IMetadataImport)uniqueObjectForIUnknown;
		}

		private static void CheckBinderAndModifiersforLMR(Binder binder, ParameterModifier[] modifiers)
		{
			if (binder != null)
			{
				throw new NotSupportedException();
			}
			if (modifiers != null && (int)modifiers.Length != 0)
			{
				throw new NotSupportedException();
			}
		}

		private static void CheckBindingFlagsInMethod(BindingFlags flags, string methodName)
		{
			if ((flags | BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.InvokeMethod | BindingFlags.CreateInstance | BindingFlags.GetField | BindingFlags.SetField | BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.ExactBinding) != (BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.InvokeMethod | BindingFlags.CreateInstance | BindingFlags.GetField | BindingFlags.SetField | BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.ExactBinding))
			{
				CultureInfo invariantCulture = CultureInfo.InvariantCulture;
				string methodIsUsingUnsupportedBindingFlags = MetadataStringTable.MethodIsUsingUnsupportedBindingFlags;
				object[] objArray = new object[] { methodName, flags };
				throw new NotSupportedException(string.Format(invariantCulture, methodIsUsingUnsupportedBindingFlags, objArray));
			}
		}

		private static void CheckIsStaticAndIsPublic(MethodInfo methodInfo, ref bool isStatic, ref bool isPublic)
		{
			if (methodInfo == null)
			{
				return;
			}
			if (methodInfo.IsStatic)
			{
				isStatic = true;
			}
			if (methodInfo.IsPublic)
			{
				isPublic = true;
			}
		}

		private static void CheckIsStaticAndIsPublicOnEvent(EventInfo eventInfo, ref bool isStatic, ref bool isPublic)
		{
			bool flag = true;
			MetadataOnlyModule.CheckIsStaticAndIsPublic(eventInfo.GetAddMethod(flag), ref isStatic, ref isPublic);
			MetadataOnlyModule.CheckIsStaticAndIsPublic(eventInfo.GetRemoveMethod(flag), ref isStatic, ref isPublic);
			MetadataOnlyModule.CheckIsStaticAndIsPublic(eventInfo.GetRaiseMethod(flag), ref isStatic, ref isPublic);
		}

		private static void CheckIsStaticAndIsPublicOnProperty(PropertyInfo propertyInfo, ref bool isStatic, ref bool isPublic)
		{
			bool flag = true;
			MetadataOnlyModule.CheckIsStaticAndIsPublic(propertyInfo.GetGetMethod(flag), ref isStatic, ref isPublic);
			MetadataOnlyModule.CheckIsStaticAndIsPublic(propertyInfo.GetSetMethod(flag), ref isStatic, ref isPublic);
		}

		internal int CountGenericParams(Token token)
		{
			int num;
			uint num1;
			int num2;
			IMetadataImport2 rawImport = this.RawImport as IMetadataImport2;
			if (rawImport == null)
			{
				return 0;
			}
			HCORENUM hCORENUM = new HCORENUM();
			rawImport.EnumGenericParams(ref hCORENUM, token.Value, out num, 1, out num1);
			try
			{
				rawImport.CountEnum(hCORENUM, out num2);
			}
			finally
			{
				hCORENUM.Close(rawImport);
			}
			return num2;
		}

		private Token[] CreateTypeCodeMapping()
		{
			Token[] tokenArray = new Token[19];
			Token token = new Token();
			tokenArray[0] = token;
			tokenArray[1] = this.LookupTypeToken("System.Object");
			tokenArray[2] = this.LookupTypeToken("System.DBNull");
			tokenArray[3] = this.LookupTypeToken("System.Boolean");
			tokenArray[4] = this.LookupTypeToken("System.Char");
			tokenArray[5] = this.LookupTypeToken("System.SByte");
			tokenArray[6] = this.LookupTypeToken("System.Byte");
			tokenArray[7] = this.LookupTypeToken("System.Int16");
			tokenArray[8] = this.LookupTypeToken("System.UInt16");
			tokenArray[9] = this.LookupTypeToken("System.Int32");
			tokenArray[10] = this.LookupTypeToken("System.UInt32");
			tokenArray[11] = this.LookupTypeToken("System.Int64");
			tokenArray[12] = this.LookupTypeToken("System.UInt64");
			tokenArray[13] = this.LookupTypeToken("System.Single");
			tokenArray[14] = this.LookupTypeToken("System.Double");
			tokenArray[15] = this.LookupTypeToken("System.Decimal");
			tokenArray[16] = this.LookupTypeToken("System.DateTime");
			Token token1 = new Token();
			tokenArray[17] = token1;
			tokenArray[18] = this.LookupTypeToken("System.String");
			return tokenArray;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.m_cachedThreadAffinityImporter != null)
				{
					Marshal.ReleaseComObject(this.m_cachedThreadAffinityImporter);
					this.m_cachedThreadAffinityImporter = null;
				}
				if (this.m_metadata != null)
				{
					this.m_metadata.Dispose();
				}
			}
		}

		private void EnsureNestedTypeCacheExists()
		{
			if (this.m_nestedTypeInfo == null)
			{
				this.m_nestedTypeInfo = new MetadataOnlyModule.NestedTypeCache(this);
			}
		}

		private void EnsureValidToken(Token token)
		{
			if (!this.IsValidToken(token))
			{
				CultureInfo currentCulture = CultureInfo.CurrentCulture;
				string invalidMetadataToken = MetadataStringTable.InvalidMetadataToken;
				object[] str = new object[] { token.ToString() };
				throw new ArgumentException(string.Format(currentCulture, invalidMetadataToken, str));
			}
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			MetadataOnlyModule metadataOnlyModule = obj as MetadataOnlyModule;
			if (metadataOnlyModule == null)
			{
				return false;
			}
			if (!this.m_assemblyResolver.Equals(metadataOnlyModule.AssemblyResolver))
			{
				return false;
			}
			return this.ScopeName == metadataOnlyModule.ScopeName;
		}

		private static IList<EventInfo> FilterInheritedEvents(IList<EventInfo> inheritedEvents, IList<EventInfo> events)
		{
			if (events == null || events.Count == 0)
			{
				return inheritedEvents;
			}
			List<EventInfo> eventInfos = new List<EventInfo>();
			foreach (EventInfo inheritedEvent in inheritedEvents)
			{
				bool flag = false;
				foreach (EventInfo @event in events)
				{
					if (!inheritedEvent.Name.Equals(@event.Name, StringComparison.Ordinal))
					{
						continue;
					}
					flag = true;
					break;
				}
				if (flag)
				{
					continue;
				}
				eventInfos.Add(inheritedEvent);
			}
			return eventInfos;
		}

		private static IList<PropertyInfo> FilterInheritedProperties(IList<PropertyInfo> inheritedProperties, IList<PropertyInfo> properties, BindingFlags flags)
		{
			if (properties == null || properties.Count == 0)
			{
				return inheritedProperties;
			}
			List<PropertyInfo> propertyInfos = new List<PropertyInfo>();
			List<MethodInfo> methodInfos = new List<MethodInfo>();
			List<MethodInfo> methodInfos1 = new List<MethodInfo>();
			foreach (PropertyInfo property in properties)
			{
				MethodInfo getMethod = property.GetGetMethod();
				if (getMethod != null)
				{
					methodInfos.Add(getMethod);
				}
				MethodInfo setMethod = property.GetSetMethod();
				if (setMethod == null)
				{
					continue;
				}
				methodInfos1.Add(setMethod);
			}
			foreach (PropertyInfo inheritedProperty in inheritedProperties)
			{
				MethodInfo methodInfo = inheritedProperty.GetGetMethod();
				if (methodInfo != null && !MetadataOnlyModule.IncludeInheritedAccessor(methodInfo, methodInfos, flags))
				{
					continue;
				}
				MethodInfo setMethod1 = inheritedProperty.GetSetMethod();
				if (setMethod1 != null && !MetadataOnlyModule.IncludeInheritedAccessor(setMethod1, methodInfos1, flags))
				{
					continue;
				}
				propertyInfos.Add(inheritedProperty);
			}
			return propertyInfos;
		}

		private static MethodInfo FilterMethod(MethodInfo[] methods, string name, BindingFlags bindingAttr, CallingConventions callConv, Type[] types)
		{
			bool flag = false;
			MethodInfo methodInfo = null;
			StringComparison stringComparison = SignatureUtil.GetStringComparison(bindingAttr);
			MethodInfo[] methodInfoArray = methods;
			for (int i = 0; i < (int)methodInfoArray.Length; i++)
			{
				MethodInfo methodInfo1 = methodInfoArray[i];
				if (flag && methodInfo.DeclaringType != null && !methodInfo.DeclaringType.Equals(methodInfo1.DeclaringType))
				{
					break;
				}
				if (methodInfo1.Name.Equals(name, stringComparison) && SignatureUtil.IsCallingConventionMatch(methodInfo1, callConv) && SignatureUtil.IsParametersTypeMatch(methodInfo1, types))
				{
					if (flag)
					{
						throw new AmbiguousMatchException();
					}
					methodInfo = methodInfo1;
					flag = true;
				}
			}
			return methodInfo;
		}

		internal Token FindTypeDefByName(Type outerType, string className, bool fThrow)
		{
			Token token = new Token(0);
			if (outerType != null)
			{
				if (outerType.Module != this)
				{
					throw new InvalidOperationException(MetadataStringTable.DifferentTokenResolverForOuterType);
				}
				token = new Token(outerType.MetadataToken);
			}
			return this.FindTypeDefByName(token, className, fThrow);
		}

		internal Token FindTypeDefByName(Token outerTypeDefToken, string className, bool fThrow)
		{
			int num;
			bool isNil = outerTypeDefToken.IsNil;
			int num1 = this.RawImport.FindTypeDefByName(className, outerTypeDefToken, out num);
			if (!fThrow && num1 == -2146234064)
			{
				return Token.Nil;
			}
			if (num1 != 0)
			{
				throw Marshal.GetExceptionForHR(num1);
			}
			return new Token(num);
		}

		public override Type[] FindTypes(TypeFilter filter, object filterCriteria)
		{
			List<Type> types = new List<Type>();
			foreach (Type typeList in this.GetTypeList())
			{
				if (!filter(typeList, filterCriteria))
				{
					continue;
				}
				types.Add(typeList);
			}
			return types.ToArray();
		}

		public AssemblyName GetAssemblyNameFromAssemblyRef(Token assemblyRefToken)
		{
			return AssemblyNameHelper.GetAssemblyNameFromRef(assemblyRefToken, this, (IMetadataAssemblyImport)this.RawImport);
		}

		internal IEnumerable<Type> GetConstraintTypes(int gpToken)
		{
			int num;
			uint num1;
			int num2;
			int num3;
			Token token = new Token(gpToken);
			IMetadataImport2 rawImport = this.RawImport as IMetadataImport2;
			if (rawImport != null)
			{
				HCORENUM hCORENUM = new HCORENUM();
				try
				{
					while (true)
					{
						rawImport.EnumGenericParamConstraints(ref hCORENUM, gpToken, out num, 1, out num1);
						if (num1 != 1)
						{
							break;
						}
						rawImport.GetGenericParamConstraintProps(num, out num2, out num3);
						yield return this.ResolveTypeTokenInternal(new Token(num3), null);
					}
				}
				finally
				{
					hCORENUM.Close(rawImport);
				}
			}
		}

		private IList<CustomAttributeTypedArgument> GetConstructorArguments(ConstructorInfo constructorInfo, byte[] customAttributeBlob, ref int index)
		{
			System.Reflection.Adds.CorElementType corElementType;
			ParameterInfo[] parameters = constructorInfo.GetParameters();
			IList<CustomAttributeTypedArgument> customAttributeTypedArguments = new List<CustomAttributeTypedArgument>((int)parameters.Length);
			for (int i = 0; i < (int)parameters.Length; i++)
			{
				Type parameterType = parameters[i].ParameterType;
				System.Reflection.Adds.CorElementType typeId = SignatureUtil.GetTypeId(parameterType);
				object customAttributeArgumentValue = null;
				Type type = null;
				if (typeId == System.Reflection.Adds.CorElementType.Object)
				{
					SignatureUtil.ExtractCustomAttributeArgumentType(this.AssemblyResolver, this, customAttributeBlob, ref index, out corElementType, out type);
					customAttributeArgumentValue = this.GetCustomAttributeArgumentValue(corElementType, type, customAttributeBlob, ref index);
				}
				else
				{
					customAttributeArgumentValue = this.GetCustomAttributeArgumentValue(typeId, parameterType, customAttributeBlob, ref index);
					type = parameterType;
				}
				customAttributeTypedArguments.Add(new CustomAttributeTypedArgument(type, customAttributeArgumentValue));
			}
			return customAttributeTypedArguments;
		}

		internal static ConstructorInfo GetConstructorOnType(MetadataOnlyCommonType type, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			MetadataOnlyModule.CheckBinderAndModifiersforLMR(binder, modifiers);
			ConstructorInfo[] constructorsOnType = MetadataOnlyModule.GetConstructorsOnType(type, bindingAttr);
			for (int i = 0; i < (int)constructorsOnType.Length; i++)
			{
				ConstructorInfo constructorInfo = constructorsOnType[i];
				if (SignatureUtil.IsCallingConventionMatch(constructorInfo, callConvention) && SignatureUtil.IsParametersTypeMatch(constructorInfo, types))
				{
					return constructorInfo;
				}
			}
			return null;
		}

		internal static ConstructorInfo[] GetConstructorsOnType(MetadataOnlyCommonType type, BindingFlags flags)
		{
			MetadataOnlyModule.CheckBindingFlagsInMethod(flags, "GetConstructorsOnType");
			List<ConstructorInfo> constructorInfos = new List<ConstructorInfo>();
			foreach (ConstructorInfo declaredConstructor in type.GetDeclaredConstructors())
			{
				if (!Utility.IsBindingFlagsMatching(declaredConstructor, false, flags))
				{
					continue;
				}
				constructorInfos.Add(declaredConstructor);
			}
			return constructorInfos.ToArray();
		}

		private object GetCustomAttributeArgumentValue(System.Reflection.Adds.CorElementType typeId, Type type, byte[] customAttributeBlob, ref int index)
		{
			object obj = null;
			System.Reflection.Adds.CorElementType corElementType = typeId;
			if (corElementType == System.Reflection.Adds.CorElementType.SzArray)
			{
				uint num = SignatureUtil.ExtractUIntValue(customAttributeBlob, ref index);
				if (num != -1)
				{
					obj = SignatureUtil.ExtractListOfValues(type.GetElementType(), this.AssemblyResolver, this, num, customAttributeBlob, ref index);
				}
			}
			else if (corElementType == System.Reflection.Adds.CorElementType.Type)
			{
				obj = SignatureUtil.ExtractTypeValue(this.AssemblyResolver, this, customAttributeBlob, ref index);
			}
			else if (corElementType == System.Reflection.Adds.CorElementType.Enum)
			{
				System.Reflection.Adds.CorElementType corElementType1 = SignatureUtil.GetTypeId(MetadataOnlyModule.GetUnderlyingType(type));
				obj = SignatureUtil.ExtractValue(corElementType1, customAttributeBlob, ref index);
			}
			else
			{
				obj = SignatureUtil.ExtractValue(typeId, customAttributeBlob, ref index);
			}
			return obj;
		}

		public IList<CustomAttributeData> GetCustomAttributeData(int memberTokenValue)
		{
			Token token;
			Token token1;
			Token token2;
			EmbeddedBlobPointer embeddedBlobPointer;
			int num;
			uint num1;
			List<CustomAttributeData> customAttributeDatas = new List<CustomAttributeData>();
			HCORENUM hCORENUM = new HCORENUM();
			IMetadataImport rawImport = this.RawImport;
			try
			{
				while (true)
				{
					rawImport.EnumCustomAttributes(ref hCORENUM, memberTokenValue, 0, out token, 1, out num1);
					if (num1 == 0)
					{
						break;
					}
					rawImport.GetCustomAttributeProps(token, out token1, out token2, out embeddedBlobPointer, out num);
					ConstructorInfo constructorInfo = this.ResolveCustomAttributeConstructor(token2);
					customAttributeDatas.Add(new MetadataOnlyCustomAttributeData(this, token, constructorInfo));
				}
			}
			finally
			{
				hCORENUM.Close(rawImport);
			}
			IEnumerable<CustomAttributeData> pseudoCustomAttributes = this.m_policy.GetPseudoCustomAttributes(this, new Token(memberTokenValue));
			customAttributeDatas.AddRange(pseudoCustomAttributes);
			return customAttributeDatas;
		}

		public override IList<CustomAttributeData> GetCustomAttributesData()
		{
			return this.GetCustomAttributeData(this.MetadataToken);
		}

		internal Type GetEnclosingType(Token tokenTypeDef)
		{
			Token token = new Token(this.GetNestedClassProps(tokenTypeDef));
			if (token.IsNil)
			{
				return null;
			}
			return this.ResolveTypeTokenInternal(token, null);
		}

		internal System.Reflection.Adds.CorElementType GetEnumUnderlyingType(Token tokenTypeDef)
		{
			int num;
			uint num1;
			FieldAttributes fieldAttribute;
			int num2;
			int num3;
			IntPtr intPtr;
			int num4;
			EmbeddedBlobPointer embeddedBlobPointer;
			int num5;
			int num6;
			System.Reflection.Adds.CorElementType corElementType;
			IMetadataImport rawImport = this.RawImport;
			HCORENUM hCORENUM = new HCORENUM();
			try
			{
				rawImport.EnumFields(ref hCORENUM, tokenTypeDef.Value, out num, 1, out num1);
				while (num1 != 0)
				{
					rawImport.GetFieldProps(num, out num6, null, 0, out num2, out fieldAttribute, out embeddedBlobPointer, out num5, out num3, out intPtr, out num4);
					if ((fieldAttribute & FieldAttributes.Static) != FieldAttributes.PrivateScope)
					{
						rawImport.EnumFields(ref hCORENUM, tokenTypeDef.Value, out num, 1, out num1);
					}
					else
					{
						byte[] numArray = this.ReadEmbeddedBlob(embeddedBlobPointer, num5);
						int num7 = 0;
						SignatureUtil.ExtractCallingConvention(numArray, ref num7);
						corElementType = SignatureUtil.ExtractElementType(numArray, ref num7);
						return corElementType;
					}
				}
				throw new ArgumentException(MetadataStringTable.OperationValidOnEnumOnly);
			}
			finally
			{
				hCORENUM.Close(rawImport);
			}
			return corElementType;
		}

		private IEnumerable<EventInfo> GetEventsOnDeclaredTypeOnly(Token tokenTypeDef, GenericContext context)
		{
			int num;
			uint num1;
			HCORENUM hCORENUM = new HCORENUM();
			IMetadataImport rawImport = this.RawImport;
			try
			{
				while (true)
				{
					rawImport.EnumEvents(ref hCORENUM, tokenTypeDef.Value, out num, 1, out num1);
					if (num1 == 0)
					{
						break;
					}
					EventInfo eventInfo = this.Factory.CreateEventInfo(this, new Token(num), context.TypeArgs, context.MethodArgs);
					yield return eventInfo;
				}
			}
			finally
			{
				hCORENUM.Close(rawImport);
			}
		}

		internal static EventInfo[] GetEventsOnType(MetadataOnlyCommonType type, BindingFlags flags)
		{
			MetadataOnlyModule.CheckBindingFlagsInMethod(flags, "GetEventsOnType");
			List<EventInfo> eventInfos = new List<EventInfo>();
			foreach (EventInfo eventsOnDeclaredTypeOnly in type.Resolver.GetEventsOnDeclaredTypeOnly(new Token(type.MetadataToken), type.GenericContext))
			{
				bool flag = false;
				bool flag1 = false;
				MetadataOnlyModule.CheckIsStaticAndIsPublicOnEvent(eventsOnDeclaredTypeOnly, ref flag, ref flag1);
				if (!Utility.IsBindingFlagsMatching(eventsOnDeclaredTypeOnly, flag, flag1, false, flags))
				{
					continue;
				}
				eventInfos.Add(eventsOnDeclaredTypeOnly);
			}
			if (MetadataOnlyModule.WalkInheritanceChain(flags) && type.BaseType != null)
			{
				EventInfo[] events = type.BaseType.GetEvents(flags);
				eventInfos.AddRange(MetadataOnlyModule.FilterInheritedEvents(events, eventInfos));
			}
			return eventInfos.ToArray();
		}

		public override FieldInfo GetField(string name, BindingFlags bindingAttr)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			FieldInfo[] fields = this.GetFields(bindingAttr);
			for (int i = 0; i < (int)fields.Length; i++)
			{
				FieldInfo fieldInfo = fields[i];
				if (fieldInfo.Name.Equals(name))
				{
					return fieldInfo;
				}
			}
			return null;
		}

		public override FieldInfo[] GetFields(BindingFlags bindingFlags)
		{
			int num;
			MetadataOnlyModule.CheckBindingFlagsInMethod(bindingFlags, "GetFields");
			IMetadataImport rawImport = this.RawImport;
			HCORENUM hCORENUM = new HCORENUM();
			List<FieldInfo> fieldInfos = new List<FieldInfo>();
			try
			{
				uint num1 = 1;
				while (true)
				{
					rawImport.EnumFields(ref hCORENUM, this.MetadataToken, out num, 1, out num1);
					if (num1 != 1)
					{
						break;
					}
					FieldInfo fieldInfo = base.ResolveField(num);
					if (Utility.IsBindingFlagsMatching(fieldInfo, false, bindingFlags))
					{
						fieldInfos.Add(fieldInfo);
					}
				}
			}
			finally
			{
				hCORENUM.Close(rawImport);
			}
			return fieldInfos.ToArray();
		}

		private IEnumerable<FieldInfo> GetFieldsOnDeclaredTypeOnly(Token typeDefToken, GenericContext context)
		{
			int num;
			uint num1;
			HCORENUM hCORENUM = new HCORENUM();
			IMetadataImport rawImport = this.RawImport;
			Type[] emptyTypes = Type.EmptyTypes;
			Type[] typeArray = Type.EmptyTypes;
			if (context != null)
			{
				emptyTypes = context.TypeArgs;
				typeArray = context.MethodArgs;
			}
			try
			{
				while (true)
				{
					rawImport.EnumFields(ref hCORENUM, typeDefToken, out num, 1, out num1);
					if (num1 == 0)
					{
						break;
					}
					FieldInfo fieldInfo = this.Factory.CreateField(this, new Token(num), emptyTypes, typeArray);
					yield return fieldInfo;
				}
			}
			finally
			{
				hCORENUM.Close(rawImport);
			}
		}

		internal static FieldInfo[] GetFieldsOnType(MetadataOnlyCommonType type, BindingFlags flags)
		{
			MetadataOnlyModule.CheckBindingFlagsInMethod(flags, "GetFieldsOnType");
			List<FieldInfo> fieldInfos = new List<FieldInfo>();
			foreach (FieldInfo fieldsOnDeclaredTypeOnly in type.Resolver.GetFieldsOnDeclaredTypeOnly(new Token(type.MetadataToken), type.GenericContext))
			{
				if (!Utility.IsBindingFlagsMatching(fieldsOnDeclaredTypeOnly, false, flags))
				{
					continue;
				}
				fieldInfos.Add(fieldsOnDeclaredTypeOnly);
			}
			if (MetadataOnlyModule.WalkInheritanceChain(flags) && type.BaseType != null)
			{
				FieldInfo[] fields = type.BaseType.GetFields(flags);
				List<FieldInfo> fieldInfos1 = new List<FieldInfo>();
				FieldInfo[] fieldInfoArray = fields;
				for (int i = 0; i < (int)fieldInfoArray.Length; i++)
				{
					FieldInfo fieldInfo = fieldInfoArray[i];
					if (MetadataOnlyModule.IncludeInheritedField(fieldInfo, flags))
					{
						fieldInfos1.Add(fieldInfo);
					}
				}
				fieldInfos.AddRange(fieldInfos1);
			}
			return fieldInfos.ToArray();
		}

		internal MethodBase GetGenericMethodBase(Token methodToken, GenericContext genericContext)
		{
			if (genericContext != null && (genericContext.TypeArgs == null || (int)genericContext.TypeArgs.Length == 0) && (genericContext.MethodArgs == null || (int)genericContext.MethodArgs.Length == 0))
			{
				genericContext = null;
			}
			return MetadataOnlyMethodInfo.Create(this, methodToken, genericContext);
		}

		internal MethodInfo GetGenericMethodInfo(Token methodToken, GenericContext genericContext)
		{
			return (MethodInfo)this.GetGenericMethodBase(methodToken, genericContext);
		}

		internal void GetGenericParameterProps(int mdGenericParam, out int ownerTypeToken, out int ownerMethodToken, out string name, out GenericParameterAttributes attributes, out uint genIndex)
		{
			int num;
			int num1;
			int num2;
			ulong num3;
			IMetadataImport2 rawImport = this.RawImport as IMetadataImport2;
			HCORENUM hCORENUM = new HCORENUM();
			try
			{
				rawImport.GetGenericParamProps(mdGenericParam, out genIndex, out num, out num1, out num2, null, (ulong)0, out num3);
				attributes = (GenericParameterAttributes)num;
				StringBuilder stringBuilder = new StringBuilder((int)num3);
				rawImport.GetGenericParamProps(mdGenericParam, out genIndex, out num, out num1, out num2, stringBuilder, (ulong)stringBuilder.Capacity, out num3);
				name = stringBuilder.ToString();
				if (!(new Token(num1)).IsType(System.Reflection.Adds.TokenType.MethodDef))
				{
					ownerTypeToken = num1;
					ownerMethodToken = 0;
				}
				else
				{
					ownerMethodToken = num1;
					ownerTypeToken = 0;
				}
			}
			finally
			{
				hCORENUM.Close(rawImport);
			}
		}

		internal IEnumerable<int> GetGenericParameterTokens(int typeOrMethodToken)
		{
			int num;
			uint num1;
			Token token = new Token(typeOrMethodToken);
			IMetadataImport2 rawImport = this.RawImport as IMetadataImport2;
			if (rawImport != null)
			{
				HCORENUM hCORENUM = new HCORENUM();
				try
				{
					while (true)
					{
						rawImport.EnumGenericParams(ref hCORENUM, typeOrMethodToken, out num, 1, out num1);
						if (num1 != 1)
						{
							break;
						}
						yield return num;
					}
				}
				finally
				{
					hCORENUM.Close(rawImport);
				}
			}
		}

		internal Type GetGenericType(Token token, GenericContext context)
		{
			Type[] typeArgs = null;
			Type[] methodArgs = null;
			if (context != null)
			{
				typeArgs = context.TypeArgs;
				methodArgs = context.MethodArgs;
			}
			if (token.IsType(System.Reflection.Adds.TokenType.TypeDef))
			{
				if (typeArgs == null || (int)typeArgs.Length <= 0)
				{
					return this.m_factory.CreateSimpleType(this, token);
				}
				return this.m_factory.CreateGenericType(this, token, typeArgs);
			}
			if (!token.IsType(System.Reflection.Adds.TokenType.TypeRef))
			{
				if (!token.IsType(System.Reflection.Adds.TokenType.TypeSpec))
				{
					throw new ArgumentException(MetadataStringTable.TypeTokenExpected);
				}
				return this.m_factory.CreateTypeSpec(this, token, typeArgs, methodArgs);
			}
			Type type = this.m_factory.CreateTypeRef(this, token);
			if (typeArgs != null && (int)typeArgs.Length > 0)
			{
				type = type.MakeGenericType(typeArgs);
			}
			return type;
		}

		public override int GetHashCode()
		{
			IntPtr rawPtr = this.m_metadata.RawPtr;
			return rawPtr.GetHashCode() + this.m_assemblyResolver.GetHashCode();
		}

		public static Type GetInterfaceHelper(Type[] interfaces, string name, bool ignoreCase)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			Type type = null;
			Type[] typeArray = interfaces;
			for (int i = 0; i < (int)typeArray.Length; i++)
			{
				Type type1 = typeArray[i];
				if (Utility.Compare(name, type1.Name, ignoreCase))
				{
					if (type != null)
					{
						throw new AmbiguousMatchException();
					}
					type = type1;
				}
			}
			return type;
		}

		internal IEnumerable<Type> GetInterfacesOnType(Type type)
		{
			int num;
			int num1;
			int num2;
			IMetadataImport rawImport = this.RawImport;
			if (!type.IsGenericParameter)
			{
				HCORENUM hCORENUM = new HCORENUM();
				int num3 = 1;
				while (true)
				{
					rawImport.EnumInterfaceImpls(ref hCORENUM, type.MetadataToken, out num, 1, ref num3);
					if (num3 != 1)
					{
						break;
					}
					Token token = new Token(num);
					rawImport.GetInterfaceImplProps(token.Value, out num1, out num2);
					Token token1 = new Token(num1);
					Token token2 = new Token(num2);
					Type type1 = this.ResolveTypeTokenInternal(token2, new GenericContext(type.GetGenericArguments(), null));
					yield return type1;
				}
				hCORENUM.Close(rawImport);
			}
			else
			{
				foreach (Type constraintType in this.GetConstraintTypes(type.MetadataToken))
				{
					if (!constraintType.IsInterface)
					{
						continue;
					}
					yield return constraintType;
				}
			}
		}

		internal void GetMemberRefData(Token token, out Token declaringTypeToken, out string nameMember, out SignatureBlob sig)
		{
			uint num;
			EmbeddedBlobPointer embeddedBlobPointer;
			uint num1;
			if (!this.IsValidToken(token))
			{
				CultureInfo invariantCulture = CultureInfo.InvariantCulture;
				string invalidMetadataToken = MetadataStringTable.InvalidMetadataToken;
				object[] str = new object[] { token.ToString() };
				throw new ArgumentException(string.Format(invariantCulture, invalidMetadataToken, str));
			}
			IMetadataImport rawImport = this.RawImport;
			rawImport.GetMemberRefProps(token, out declaringTypeToken, null, 0, out num, out embeddedBlobPointer, out num1);
			StringBuilder stringBuilder = new StringBuilder((int)num);
			rawImport.GetMemberRefProps(token, out declaringTypeToken, stringBuilder, stringBuilder.Capacity, out num, out embeddedBlobPointer, out num1);
			nameMember = stringBuilder.ToString();
			sig = SignatureBlob.ReadSignature(this.RawMetadata, embeddedBlobPointer, (int)num1);
		}

		internal IEnumerable<MethodBase> GetMethodBasesOnDeclaredTypeOnly(Token tokenTypeDef, GenericContext context, MetadataOnlyModule.EMethodKind kind)
		{
			int num;
			int num1;
			IMetadataImport rawImport = this.RawImport;
			HCORENUM hCORENUM = new HCORENUM();
			try
			{
				while (true)
				{
					rawImport.EnumMethods(ref hCORENUM, tokenTypeDef.Value, out num, 1, out num1);
					if (num1 == 0)
					{
						break;
					}
					List<Type> typeParameters = this.GetTypeParameters(num);
					GenericContext genericContext = new GenericContext(context.TypeArgs, typeParameters.ToArray());
					MethodBase genericMethodBase = this.GetGenericMethodBase(new Token(num), genericContext);
					if (genericMethodBase is ConstructorInfo == kind == MetadataOnlyModule.EMethodKind.Constructor)
					{
						yield return genericMethodBase;
					}
				}
			}
			finally
			{
				hCORENUM.Close(rawImport);
			}
		}

		protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			MetadataOnlyModule.CheckBinderAndModifiersforLMR(binder, modifiers);
			MethodInfo[] methods = this.GetMethods(bindingAttr);
			return MetadataOnlyModule.FilterMethod(methods, name, bindingAttr, callConvention, types);
		}

		internal MethodImplAttributes GetMethodImplFlags(int methodToken)
		{
			uint num;
			uint num1;
			this.RawImport.GetRVA(methodToken, out num, out num1);
			return (MethodImplAttributes)num1;
		}

		internal static MethodInfo GetMethodImplHelper(Type type, string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConv, Type[] types, ParameterModifier[] modifiers)
		{
			MetadataOnlyModule.CheckBinderAndModifiersforLMR(binder, modifiers);
			MethodInfo[] methods = type.GetMethods(bindingAttr);
			return MetadataOnlyModule.FilterMethod(methods, name, bindingAttr, callConv, types);
		}

		internal void GetMethodProps(Token methodDef, out Token declaringTypeDef, out string name, out MethodAttributes attrs, out SignatureBlob signature)
		{
			uint num;
			MethodAttributes methodAttribute;
			EmbeddedBlobPointer embeddedBlobPointer;
			uint num1;
			uint num2;
			uint num3;
			int num4;
			if (!this.IsValidToken(methodDef))
			{
				CultureInfo invariantCulture = CultureInfo.InvariantCulture;
				string invalidMetadataToken = MetadataStringTable.InvalidMetadataToken;
				object[] str = new object[] { methodDef.ToString() };
				throw new ArgumentException(string.Format(invariantCulture, invalidMetadataToken, str));
			}
			uint value = (uint)methodDef.Value;
			IMetadataImport rawImport = this.RawImport;
			rawImport.GetMethodProps(value, out num4, null, 0, out num, out methodAttribute, out embeddedBlobPointer, out num3, out num1, out num2);
			StringBuilder stringBuilder = new StringBuilder((int)num);
			rawImport.GetMethodProps(value, out num4, stringBuilder, stringBuilder.Capacity, out num, out methodAttribute, out embeddedBlobPointer, out num3, out num1, out num2);
			name = stringBuilder.ToString();
			attrs = methodAttribute;
			signature = SignatureBlob.ReadSignature(this.RawMetadata, embeddedBlobPointer, (int)num3);
			declaringTypeDef = new Token(num4);
		}

		internal uint GetMethodRva(int methodDef)
		{
			uint num;
			MethodAttributes methodAttribute;
			EmbeddedBlobPointer embeddedBlobPointer;
			uint num1;
			uint num2;
			uint num3;
			int num4;
			IMetadataImport rawImport = this.RawImport;
			rawImport.GetMethodProps((uint)methodDef, out num4, null, 0, out num, out methodAttribute, out embeddedBlobPointer, out num3, out num1, out num2);
			return num1;
		}

		public override MethodInfo[] GetMethods(BindingFlags bindingFlags)
		{
			int num;
			MetadataOnlyModule.CheckBindingFlagsInMethod(bindingFlags, "GetMethods");
			IMetadataImport rawImport = this.RawImport;
			HCORENUM hCORENUM = new HCORENUM();
			List<MethodInfo> methodInfos = new List<MethodInfo>();
			try
			{
				int num1 = 1;
				while (true)
				{
					rawImport.EnumMethods(ref hCORENUM, this.MetadataToken, out num, 1, out num1);
					if (num1 != 1)
					{
						break;
					}
					MethodBase methodBase = this.ResolveMethodTokenInternal(new Token(num), null);
					if (Utility.IsBindingFlagsMatching(methodBase, false, bindingFlags))
					{
						MethodInfo methodInfo = methodBase as MethodInfo;
						if (methodInfo != null)
						{
							methodInfos.Add(methodInfo);
						}
					}
				}
			}
			finally
			{
				hCORENUM.Close(rawImport);
			}
			return methodInfos.ToArray();
		}

		internal static MethodInfo[] GetMethodsOnType(MetadataOnlyCommonType type, BindingFlags flags)
		{
			MetadataOnlyModule.CheckBindingFlagsInMethod(flags, "GetMethodsOnType");
			List<MethodInfo> methodInfos = new List<MethodInfo>();
			foreach (MethodInfo declaredMethod in type.GetDeclaredMethods())
			{
				if (!Utility.IsBindingFlagsMatching(declaredMethod, false, flags))
				{
					continue;
				}
				methodInfos.Add(declaredMethod);
			}
			if (MetadataOnlyModule.WalkInheritanceChain(flags) && type.BaseType != null)
			{
				MethodInfo[] methods = type.BaseType.GetMethods(flags);
				List<MethodInfo> methodInfos1 = new List<MethodInfo>();
				MethodInfo[] methodInfoArray = methods;
				for (int i = 0; i < (int)methodInfoArray.Length; i++)
				{
					MethodInfo methodInfo = methodInfoArray[i];
					if (MetadataOnlyModule.IncludeInheritedMethod(methodInfo, methodInfos, flags))
					{
						methodInfos1.Add(methodInfo);
					}
				}
				methodInfos.AddRange(methodInfos1);
			}
			return methodInfos.ToArray();
		}

		private IList<CustomAttributeNamedArgument> GetNamedArguments(ConstructorInfo constructorInfo, byte[] customAttributeBlob, ref int index)
		{
			System.Reflection.Adds.CorElementType corElementType;
			Type type;
			MemberInfo property;
			ushort num = BitConverter.ToUInt16(customAttributeBlob, index);
			index = index + 2;
			IList<CustomAttributeNamedArgument> customAttributeNamedArguments = new List<CustomAttributeNamedArgument>((int)num);
			if (num == 0 && index != (int)customAttributeBlob.Length)
			{
				throw new ArgumentException(MetadataStringTable.InvalidCustomAttributeFormat);
			}
			for (int i = 0; i < num; i++)
			{
				NamedArgumentType namedArgumentType = SignatureUtil.ExtractNamedArgumentType(customAttributeBlob, ref index);
				SignatureUtil.ExtractCustomAttributeArgumentType(this.AssemblyResolver, this, customAttributeBlob, ref index, out corElementType, out type);
				string str = SignatureUtil.ExtractStringValue(customAttributeBlob, ref index);
				if (type == null)
				{
					SignatureUtil.ExtractCustomAttributeArgumentType(this.AssemblyResolver, this, customAttributeBlob, ref index, out corElementType, out type);
				}
				object customAttributeArgumentValue = this.GetCustomAttributeArgumentValue(corElementType, type, customAttributeBlob, ref index);
				if (namedArgumentType != NamedArgumentType.Field)
				{
					property = constructorInfo.DeclaringType.GetProperty(str);
				}
				else
				{
					property = constructorInfo.DeclaringType.GetField(str, BindingFlags.Instance | BindingFlags.Public);
				}
				CustomAttributeTypedArgument customAttributeTypedArgument = new CustomAttributeTypedArgument(type, customAttributeArgumentValue);
				customAttributeNamedArguments.Add(new CustomAttributeNamedArgument(property, customAttributeTypedArgument));
			}
			if (index != (int)customAttributeBlob.Length)
			{
				throw new ArgumentException(MetadataStringTable.InvalidCustomAttributeFormat);
			}
			return customAttributeNamedArguments;
		}

		internal Token GetNestedClassProps(Token tokenTypeDef)
		{
			int num;
			int nestedClassProps = this.RawImport.GetNestedClassProps(tokenTypeDef, out num);
			if (nestedClassProps == 0)
			{
				return new Token(num);
			}
			if (nestedClassProps != -2146234064)
			{
				throw Marshal.GetExceptionForHR(nestedClassProps);
			}
			return new Token(0);
		}

		internal IEnumerable<Type> GetNestedTypesOnType(MetadataOnlyCommonType type, BindingFlags flags)
		{
			return this.GetNestedTypesOnType(new Token(type.MetadataToken), flags);
		}

		internal IEnumerable<Type> GetNestedTypesOnType(Token tokenTypeDef, BindingFlags flags)
		{
			bool flag;
			MetadataOnlyModule.CheckBindingFlagsInMethod(flags, "GetNestedTypesOnType");
			this.EnsureNestedTypeCacheExists();
			IEnumerable<int> nestedTokens = this.m_nestedTypeInfo.GetNestedTokens(tokenTypeDef);
			if (nestedTokens != null)
			{
				foreach (int nestedToken in nestedTokens)
				{
					Type type = base.ResolveType(nestedToken);
					flag = (type.IsPublic ? true : type.IsNestedPublic);
					if (!Utility.IsBindingFlagsMatching(type, false, flag, false, flags))
					{
						continue;
					}
					yield return type;
				}
			}
		}

		public override void GetPEKind(out PortableExecutableKinds peKind, out ImageFileMachine machine)
		{
			((IMetadataImport2)this.RawImport).GetPEKind(out peKind, out machine);
		}

		internal IEnumerable<PropertyInfo> GetPropertiesOnDeclaredTypeOnly(Token tokenTypeDef, GenericContext context)
		{
			int num;
			uint num1;
			HCORENUM hCORENUM = new HCORENUM();
			IMetadataImport rawImport = this.RawImport;
			try
			{
				while (true)
				{
					rawImport.EnumProperties(ref hCORENUM, tokenTypeDef.Value, out num, 1, out num1);
					if (num1 == 0)
					{
						break;
					}
					PropertyInfo propertyInfo = this.Factory.CreatePropertyInfo(this, new Token(num), context.TypeArgs, context.MethodArgs);
					yield return propertyInfo;
				}
			}
			finally
			{
				hCORENUM.Close(rawImport);
			}
		}

		internal static PropertyInfo[] GetPropertiesOnType(MetadataOnlyCommonType type, BindingFlags flags)
		{
			MetadataOnlyModule.CheckBindingFlagsInMethod(flags, "GetPropertiesOnType");
			List<PropertyInfo> propertyInfos = new List<PropertyInfo>();
			bool flag = false;
			foreach (PropertyInfo declaredProperty in type.GetDeclaredProperties())
			{
				bool flag1 = false;
				bool flag2 = false;
				MetadataOnlyModule.CheckIsStaticAndIsPublicOnProperty(declaredProperty, ref flag1, ref flag2);
				if (!Utility.IsBindingFlagsMatching(declaredProperty, flag1, flag2, flag, flags))
				{
					continue;
				}
				propertyInfos.Add(declaredProperty);
			}
			if (MetadataOnlyModule.WalkInheritanceChain(flags) && type.BaseType != null)
			{
				PropertyInfo[] properties = type.BaseType.GetProperties(flags);
				propertyInfos.AddRange(MetadataOnlyModule.FilterInheritedProperties(properties, propertyInfos, flags));
			}
			return propertyInfos.ToArray();
		}

		private IMetadataImport GetThreadSafeImporter()
		{
			return this.m_cachedThreadAffinityImporter;
		}

		public override Type GetType(string className, bool throwOnError, bool ignoreCase)
		{
			if (ignoreCase)
			{
				throw new NotImplementedException(MetadataStringTable.CaseInsensitiveTypeLookupNotImplemented);
			}
			Func<AssemblyName, System.Reflection.Assembly> func = (AssemblyName assemblyName) => this.AssemblyResolver.ResolveAssembly(assemblyName);
			Func<System.Reflection.Assembly, string, bool, Type> func1 = (System.Reflection.Assembly assembly, string simpleTypeName, bool ignoreCaseInCallback) => {
				bool flag = false;
				if (assembly != null)
				{
					return assembly.GetType(simpleTypeName, flag, ignoreCaseInCallback);
				}
				Token token = this.FindTypeDefByName(null, simpleTypeName, false);
				if (token.IsNil)
				{
					return null;
				}
				return base.ResolveType(token.Value);
			};
			return Type.GetType(className, func, func1, throwOnError);
		}

		internal TypeCode GetTypeCode(Type type)
		{
			if (type.IsEnum)
			{
				type = MetadataOnlyModule.GetUnderlyingType(type);
				return Type.GetTypeCode(type);
			}
			if (!this.IsSystemModule())
			{
				return TypeCode.Object;
			}
			Token token = new Token(type.MetadataToken);
			if (this.m_typeCodeMapping == null)
			{
				this.m_typeCodeMapping = this.CreateTypeCodeMapping();
			}
			for (int i = 0; i < (int)this.m_typeCodeMapping.Length; i++)
			{
				if (token == this.m_typeCodeMapping[i])
				{
					return (TypeCode)i;
				}
			}
			return TypeCode.Object;
		}

		public IEnumerable<Type> GetTypeList()
		{
			foreach (int typeTokenList in this.GetTypeTokenList())
			{
				yield return this.ResolveTypeTokenInternal(new Token(typeTokenList), null);
			}
		}

		private List<Type> GetTypeParameters(int token)
		{
			List<Type> types = new List<Type>();
			foreach (int genericParameterToken in this.GetGenericParameterTokens(token))
			{
				Token token1 = new Token(genericParameterToken);
				if (!token1.IsType(System.Reflection.Adds.TokenType.GenericPar))
				{
					continue;
				}
				types.Add(this.Factory.CreateTypeVariable(this, token1));
			}
			return types;
		}

		internal void GetTypeProps(Token tokenTypeDef, out Token tokenExtends, out string name, out TypeAttributes attr)
		{
			int num;
			int num1;
			IMetadataImport rawImport = this.RawImport;
			rawImport.GetTypeDefProps(tokenTypeDef.Value, null, 0, out num, out attr, out num1);
			StringBuilder stringBuilder = new StringBuilder(num);
			rawImport.GetTypeDefProps(tokenTypeDef.Value, stringBuilder, stringBuilder.Capacity, out num, out attr, out num1);
			name = TypeNameQuoter.GetQuotedTypeName(stringBuilder.ToString());
			tokenExtends = new Token(num1);
		}

		public override Type[] GetTypes()
		{
			return (new List<Type>(this.GetTypeList())).ToArray();
		}

		private IEnumerable<int> GetTypeTokenList()
		{
			int num;
			IMetadataImport rawImport = this.RawImport;
			HCORENUM hCORENUM = new HCORENUM();
			try
			{
				uint num1 = 1;
				while (true)
				{
					rawImport.EnumTypeDefs(ref hCORENUM, out num, 1, out num1);
					if (num1 != 1)
					{
						break;
					}
					yield return num;
				}
			}
			finally
			{
				hCORENUM.Close(rawImport);
			}
		}

		internal static Type GetUnderlyingType(Type enumType)
		{
			return enumType.GetFields(BindingFlags.Instance | BindingFlags.Public)[0].FieldType;
		}

		private static bool IncludeInheritedAccessor(MethodInfo inheritedMethod, IEnumerable<MethodInfo> methods, BindingFlags flags)
		{
			if (!inheritedMethod.IsStatic)
			{
				return !MetadataOnlyModule.IsOverride(methods, inheritedMethod);
			}
			if ((flags & BindingFlags.FlattenHierarchy) == BindingFlags.Default)
			{
				return false;
			}
			return !MetadataOnlyModule.IsOverride(methods, inheritedMethod);
		}

		private static bool IncludeInheritedField(FieldInfo inheritedField, BindingFlags flags)
		{
			if (inheritedField.IsPrivate)
			{
				return false;
			}
			if (!inheritedField.IsStatic)
			{
				return true;
			}
			if ((flags & BindingFlags.FlattenHierarchy) != BindingFlags.Default)
			{
				return true;
			}
			return false;
		}

		private static bool IncludeInheritedMethod(MethodInfo inheritedMethod, IEnumerable<MethodInfo> methods, BindingFlags flags)
		{
			if (inheritedMethod.IsStatic)
			{
				if ((flags & BindingFlags.FlattenHierarchy) != BindingFlags.Default)
				{
					return true;
				}
				return false;
			}
			if (!inheritedMethod.IsVirtual)
			{
				return true;
			}
			return !MetadataOnlyModule.IsOverride(methods, inheritedMethod);
		}

		private static bool IsOverride(IEnumerable<MethodInfo> methods, MethodInfo m)
		{
			bool flag;
			using (IEnumerator<MethodInfo> enumerator = methods.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!MetadataOnlyModule.IsOverride(enumerator.Current, m))
					{
						continue;
					}
					flag = true;
					return flag;
				}
				return false;
			}
			return flag;
		}

		private static bool IsOverride(MethodInfo m1, MethodInfo m2)
		{
			return MetadataOnlyModule.MatchSignatures(m1, m2);
		}

		public override bool IsResource()
		{
			return false;
		}

		internal bool IsSystemModule()
		{
			return this.AssemblyResolver.GetSystemAssembly().Equals(this.Assembly);
		}

		internal bool IsValidToken(int token)
		{
			return this.RawImport.IsValidToken((uint)token);
		}

		internal bool IsValidToken(Token token)
		{
			return this.IsValidToken(token.Value);
		}

		internal void LazyAttributeParse(Token token, ConstructorInfo constructorInfo, out IList<CustomAttributeTypedArgument> constructorArguments, out IList<CustomAttributeNamedArgument> namedArguments)
		{
			Token token1;
			Token token2;
			EmbeddedBlobPointer embeddedBlobPointer;
			int num;
			IMetadataImport rawImport = this.RawImport;
			rawImport.GetCustomAttributeProps(token, out token1, out token2, out embeddedBlobPointer, out num);
			byte[] numArray = this.RawMetadata.ReadEmbeddedBlob(embeddedBlobPointer, num);
			int num1 = 0;
			if (BitConverter.ToInt16(numArray, num1) != 1)
			{
				throw new ArgumentException(MetadataStringTable.InvalidCustomAttributeFormat);
			}
			num1 = num1 + 2;
			constructorArguments = this.GetConstructorArguments(constructorInfo, numArray, ref num1);
			namedArguments = this.GetNamedArguments(constructorInfo, numArray, ref num1);
		}

		internal Token LookupTypeToken(string className)
		{
			return this.FindTypeDefByName(null, className, true);
		}

		private static bool MatchSignatures(MethodBase m1, MethodBase methodCandidate)
		{
			if (m1.Name != methodCandidate.Name && !m1.Name.EndsWith(string.Concat(".", methodCandidate.Name), StringComparison.Ordinal))
			{
				return false;
			}
			if (m1.IsStatic != methodCandidate.IsStatic)
			{
				return false;
			}
			ParameterInfo[] parameters = m1.GetParameters();
			ParameterInfo[] parameterInfoArray = methodCandidate.GetParameters();
			if ((int)parameters.Length != (int)parameterInfoArray.Length)
			{
				return false;
			}
			if (m1.IsGenericMethodDefinition)
			{
				m1 = (m1 as MethodInfo).MakeGenericMethod(methodCandidate.GetGenericArguments());
				parameters = m1.GetParameters();
			}
			for (int i = 0; i < (int)parameters.Length; i++)
			{
				if (!parameters[i].ParameterType.Equals(parameterInfoArray[i].ParameterType))
				{
					return false;
				}
			}
			MethodInfo methodInfo = m1 as MethodInfo;
			MethodInfo methodInfo1 = methodCandidate as MethodInfo;
			if (methodInfo != null && methodInfo1 == null || methodInfo == null && methodInfo1 != null)
			{
				return false;
			}
			if (methodInfo != null && !methodInfo.ReturnType.Equals(methodInfo1.ReturnType))
			{
				return false;
			}
			return true;
		}

		public byte[] ReadEmbeddedBlob(EmbeddedBlobPointer pointer, int countBytes)
		{
			return this.m_metadata.ReadEmbeddedBlob(pointer, countBytes);
		}

		private ConstructorInfo ResolveCustomAttributeConstructor(Token customAttributeConstructorTokenValue)
		{
			string str;
			SignatureBlob signatureBlob;
			Token token;
			Token token1 = customAttributeConstructorTokenValue;
			this.EnsureValidToken(token1);
			if (token1.IsType(System.Reflection.Adds.TokenType.MethodDef))
			{
				return (ConstructorInfo)this.ResolveMethodDef(token1);
			}
			if (!token1.IsType(System.Reflection.Adds.TokenType.MemberRef))
			{
				throw new ArgumentException(MetadataStringTable.MethodTokenExpected);
			}
			this.GetMemberRefData(token1, out token, out str, out signatureBlob);
			return new ConstructorInfoRef(this.ResolveTypeTokenInternal(token, null), this, token1);
		}

		public override FieldInfo ResolveField(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
		{
			return this.ResolveFieldTokenInternal(new Token(metadataToken), new GenericContext(genericTypeArguments, genericMethodArguments));
		}

		internal FieldInfo ResolveFieldRef(Token memberRef, GenericContext context)
		{
			string str;
			SignatureBlob signatureBlob;
			Token token;
			this.GetMemberRefData(memberRef, out token, out str, out signatureBlob);
			return this.ResolveTypeTokenInternal(token, context).GetField(str, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		}

		internal FieldInfo ResolveFieldTokenInternal(Token fieldToken, GenericContext context)
		{
			if (fieldToken.IsType(System.Reflection.Adds.TokenType.FieldDef))
			{
				return this.Factory.CreateField(this, fieldToken, null, null);
			}
			if (!fieldToken.IsType(System.Reflection.Adds.TokenType.MemberRef))
			{
				CultureInfo invariantCulture = CultureInfo.InvariantCulture;
				string invalidMetadataToken = MetadataStringTable.InvalidMetadataToken;
				object[] str = new object[] { fieldToken.ToString() };
				throw new ArgumentException(string.Format(invariantCulture, invalidMetadataToken, str));
			}
			return this.ResolveFieldRef(fieldToken, context);
		}

		public override MemberInfo ResolveMember(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
		{
			throw new NotImplementedException();
		}

		public override MethodBase ResolveMethod(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
		{
			return this.ResolveMethodTokenInternal(new Token(metadataToken), new GenericContext(genericTypeArguments, genericMethodArguments));
		}

		private MethodBase ResolveMethodDef(Token methodToken)
		{
			List<Type> typeParameters = this.GetTypeParameters(methodToken.Value);
			GenericContext genericContext = null;
			if (typeParameters.Count > 0)
			{
				genericContext = new GenericContext(null, typeParameters.ToArray());
			}
			return MetadataOnlyMethodInfo.Create(this, methodToken, genericContext);
		}

		internal MethodBase ResolveMethodRef(Token memberRef, GenericContext context, Type[] genericMethodParameters)
		{
			string str;
			SignatureBlob signatureBlob;
			Token token;
			this.GetMemberRefData(memberRef, out token, out str, out signatureBlob);
			byte[] signatureAsByteArray = signatureBlob.GetSignatureAsByteArray();
			int num = 0;
			if (SignatureUtil.ExtractCallingConvention(signatureAsByteArray, ref num) == Microsoft.MetadataReader.CorCallingConvention.VarArg)
			{
				throw new NotImplementedException(MetadataStringTable.VarargSignaturesNotImplemented);
			}
			Type type = this.ResolveTypeTokenInternal(token, context);
			GenericContext openGenericContext = new OpenGenericContext(this, type, memberRef);
			MethodSignatureDescriptor methodSignatureDescriptor = SignatureUtil.ExtractMethodSignature(signatureBlob, this, openGenericContext);
			GenericContext genericContext = new GenericContext(type.GetGenericArguments(), genericMethodParameters);
			MethodBase methodBase = SignatureComparer.FindMatchingMethod(str, type, methodSignatureDescriptor, genericContext);
			if (methodBase == null)
			{
				throw new MissingMethodException(type.Name, str);
			}
			return methodBase;
		}

		private MethodInfo ResolveMethodSpec(Token methodToken, GenericContext context)
		{
			Token token;
			EmbeddedBlobPointer embeddedBlobPointer;
			int num;
			MethodInfo genericMethodInfo;
			((IMetadataImport2)this.RawImport).GetMethodSpecProps(methodToken, out token, out embeddedBlobPointer, out num);
			byte[] numArray = this.ReadEmbeddedBlob(embeddedBlobPointer, num);
			int num1 = 0;
			SignatureUtil.ExtractCallingConvention(numArray, ref num1);
			int num2 = SignatureUtil.ExtractInt(numArray, ref num1);
			Type[] typeArray = new Type[num2];
			for (int i = 0; i < num2; i++)
			{
				typeArray[i] = SignatureUtil.ExtractType(numArray, ref num1, this, context);
			}
			Token token1 = new Token(token);
			System.Reflection.Adds.TokenType tokenType = token1.TokenType;
			if (tokenType == System.Reflection.Adds.TokenType.MethodDef)
			{
				genericMethodInfo = this.GetGenericMethodInfo(token1, new GenericContext(null, typeArray));
			}
			else
			{
				if (tokenType != System.Reflection.Adds.TokenType.MemberRef)
				{
					throw new InvalidOperationException();
				}
				genericMethodInfo = (MethodInfo)this.ResolveMethodRef(token1, context, typeArray);
			}
			return genericMethodInfo;
		}

		private MethodBase ResolveMethodTokenInternal(Token methodToken, GenericContext context)
		{
			this.EnsureValidToken(methodToken);
			if (methodToken.IsType(System.Reflection.Adds.TokenType.MethodDef))
			{
				return this.ResolveMethodDef(methodToken);
			}
			if (methodToken.IsType(System.Reflection.Adds.TokenType.MemberRef))
			{
				return this.ResolveMethodRef(methodToken, context, null);
			}
			if (!methodToken.IsType(System.Reflection.Adds.TokenType.MethodSpec))
			{
				throw new ArgumentException(MetadataStringTable.MethodTokenExpected);
			}
			return this.ResolveMethodSpec(methodToken, context);
		}

		internal Module ResolveModuleRef(Token moduleRefToken)
		{
			int num;
			if (this.Assembly == null)
			{
				throw new InvalidOperationException(MetadataStringTable.CannotResolveModuleRefOnNetModule);
			}
			StringBuilder stringBuilder = new StringBuilder();
			IMetadataImport rawImport = this.RawImport;
			rawImport.GetModuleRefProps(moduleRefToken.Value, null, 0, out num);
			stringBuilder.Capacity = num;
			rawImport.GetModuleRefProps(moduleRefToken.Value, stringBuilder, num, out num);
			return this.Assembly.GetModule(stringBuilder.ToString());
		}

		public override byte[] ResolveSignature(int metadataToken)
		{
			throw new NotImplementedException();
		}

		public override string ResolveString(int metadataToken)
		{
			int num;
			Token token = new Token(metadataToken);
			IMetadataImport rawImport = this.RawImport;
			rawImport.GetUserString(token, null, 0, out num);
			char[] chrArray = new char[num];
			rawImport.GetUserString(token, chrArray, (int)chrArray.Length, out num);
			return new string(chrArray);
		}

		public override Type ResolveType(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
		{
			Type type = this.ResolveTypeTokenInternal(new Token(metadataToken), new GenericContext(genericTypeArguments, genericMethodArguments));
			Helpers.EnsureResolve(type);
			return type;
		}

		internal MetadataOnlyCommonType ResolveTypeDefToken(Token token)
		{
			return this.m_factory.CreateSimpleType(this, token);
		}

		internal Type ResolveTypeRef(ITypeReference typeReference)
		{
			Token resolutionScope = typeReference.ResolutionScope;
			string rawName = typeReference.RawName;
			System.Reflection.Adds.TokenType tokenType = resolutionScope.TokenType;
			if (tokenType > System.Reflection.Adds.TokenType.TypeRef)
			{
				if (tokenType == System.Reflection.Adds.TokenType.ModuleRef)
				{
					return this.ResolveModuleRef(resolutionScope).GetType(typeReference.FullName);
				}
				if (tokenType == System.Reflection.Adds.TokenType.AssemblyRef)
				{
					System.Reflection.Assembly assembly = this.m_assemblyResolver.ResolveAssembly(this, resolutionScope);
					if (assembly == null)
					{
						throw new InvalidOperationException(MetadataStringTable.ResolverMustResolveToValidAssembly);
					}
					if (((IAssembly2)assembly).TypeUniverse != this.m_assemblyResolver)
					{
						throw new InvalidOperationException(MetadataStringTable.ResolvedAssemblyMustBeWithinSameUniverse);
					}
					return assembly.GetType(rawName, true);
				}
			}
			else
			{
				if (tokenType == System.Reflection.Adds.TokenType.Module)
				{
					return this.GetType(typeReference.FullName);
				}
				if (tokenType == System.Reflection.Adds.TokenType.TypeRef)
				{
					Type type = this.Factory.CreateTypeRef(this, resolutionScope);
					return type.GetNestedType(rawName, BindingFlags.Public | BindingFlags.NonPublic);
				}
			}
			throw new InvalidOperationException(MetadataStringTable.InvalidMetadata);
		}

		internal Type ResolveTypeTokenInternal(Token token, GenericContext context)
		{
			this.EnsureValidToken(token);
			if (token.IsType(System.Reflection.Adds.TokenType.TypeDef))
			{
				return this.ResolveTypeDefToken(token);
			}
			if (token.IsType(System.Reflection.Adds.TokenType.TypeRef))
			{
				return this.Factory.CreateTypeRef(this, token);
			}
			if (!token.IsType(System.Reflection.Adds.TokenType.TypeSpec))
			{
				throw new ArgumentException(MetadataStringTable.TypeTokenExpected);
			}
			Type[] typeArgs = null;
			Type[] methodArgs = null;
			if (context != null)
			{
				typeArgs = context.TypeArgs;
				methodArgs = context.MethodArgs;
			}
			return this.Factory.CreateTypeSpec(this, token, typeArgs, methodArgs);
		}

		public int RowCount(System.Reflection.Adds.MetadataTable metadataTableIndex)
		{
			int num;
			int num1;
			UnusedIntPtr unusedIntPtr;
			IMetadataTables rawImport = (IMetadataTables)this.RawImport;
			rawImport.GetTableInfo(metadataTableIndex, out num1, out num, out num1, out num1, out unusedIntPtr);
			return num;
		}

		internal void SetContainingAssembly(System.Reflection.Assembly assembly)
		{
			this.m_assembly = assembly;
		}

		public override string ToString()
		{
			if (this.m_metadata == null)
			{
				return "uninitialized";
			}
			return this.ScopeName;
		}

		private static bool WalkInheritanceChain(BindingFlags flags)
		{
			if ((flags & BindingFlags.DeclaredOnly) != BindingFlags.Default)
			{
				return false;
			}
			return true;
		}

		internal enum EMethodKind
		{
			Constructor,
			Methods
		}

		private class NestedTypeCache
		{
			private readonly Dictionary<int, List<int>> m_cache;

			public NestedTypeCache(MetadataOnlyModule outer)
			{
				this.m_cache = new Dictionary<int, List<int>>();
				foreach (int typeTokenList in outer.GetTypeTokenList())
				{
					int nestedClassProps = outer.GetNestedClassProps(new Token(typeTokenList));
					if (nestedClassProps == 0)
					{
						continue;
					}
					if (!this.m_cache.ContainsKey(nestedClassProps))
					{
						List<int> nums = new List<int>()
						{
							typeTokenList
						};
						this.m_cache.Add(nestedClassProps, nums);
					}
					else
					{
						this.m_cache[nestedClassProps].Add(typeTokenList);
					}
				}
			}

			public IEnumerable<int> GetNestedTokens(Token tokenTypeDef)
			{
				List<int> nums;
				if (this.m_cache.TryGetValue(tokenTypeDef, out nums))
				{
					return nums;
				}
				return null;
			}
		}
	}
}