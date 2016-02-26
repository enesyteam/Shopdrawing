using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.DesignModel.Metadata
{
	internal sealed class ProjectContextType : IType, IMember, ITypeId, IMemberId, IMutableMembers, ICachedMemberInfo, IResolvableRuntimeType, IReflectionType
	{
		private readonly Microsoft.Expression.DesignModel.Metadata.TypeResolver typeResolver;

		private readonly string internalName;

		private IXmlNamespace xmlNamespace;

		private string assemblyName;

		private string unbuiltClrNamespace;

		private IType baseType;

		private IPropertyId nameProperty;

		private IPropertyId defaultContentProperty;

		private ITypeId arrayItemType;

		private int arrayRank;

		private IList<IType> genericTypeArguments;

		private IList<IConstructor> constructors;

		private IConstructorArgumentProperties constructorArgumentProperties;

		private MemberCollection members;

		private IAssembly assembly;

		private Type type;

		private IType nearestSupportedType;

		private Type lastResolvedType;

		private string name;

		private ITypeMetadata metadata;

		private IType itemType;

		private IType nullableType;

		private System.ComponentModel.TypeConverter typeConverter;

		private Exception initializationException;

		private int hashCode;

		private bool isBuilt;

		private int genericDepth;

		public MemberAccessType Access
		{
			get
			{
				return PlatformTypeHelper.GetMemberAccess(this.lastResolvedType);
			}
		}

		public IType BaseType
		{
			get
			{
				return this.baseType;
			}
		}

		public IType DeclaringType
		{
			get
			{
				return null;
			}
		}

		public ITypeId DeclaringTypeId
		{
			get
			{
				return null;
			}
		}

		public string FullName
		{
			get
			{
				return TypeHelper.CombineNamespaceAndTypeName(this.Namespace, this.Name);
			}
		}

		public Exception InitializationException
		{
			get
			{
				return this.initializationException;
			}
		}

		public bool IsAbstract
		{
			get
			{
				Type runtimeType = this.RuntimeType;
				if (runtimeType == null)
				{
					return false;
				}
				return runtimeType.IsAbstract;
			}
		}

		public bool IsArray
		{
			get
			{
				Type runtimeType = this.RuntimeType;
				if (runtimeType == null)
				{
					return false;
				}
				return runtimeType.IsArray;
			}
		}

		public bool IsBinding
		{
			get
			{
				return ((PlatformTypes)this.PlatformMetadata).IsBinding(this.NearestResolvedType.RuntimeType);
			}
		}

		public bool IsBuilt
		{
			get
			{
				return this.isBuilt;
			}
		}

		public bool IsExpression
		{
			get
			{
				return ((PlatformTypes)this.PlatformMetadata).IsExpression(this.NearestResolvedType.RuntimeType);
			}
		}

		public bool IsGenericType
		{
			get
			{
				if (this.genericTypeArguments == null)
				{
					return false;
				}
				return this.genericTypeArguments.Count > 0;
			}
		}

		public bool IsInterface
		{
			get
			{
				Type runtimeType = this.RuntimeType;
				if (runtimeType == null)
				{
					return false;
				}
				return runtimeType.IsInterface;
			}
		}

		public bool IsResolvable
		{
			get
			{
				return this.type != null;
			}
		}

		public bool IsResource
		{
			get
			{
				return ((PlatformTypes)this.PlatformMetadata).IsResource(this.NearestResolvedType.RuntimeType);
			}
		}

		public IType ItemType
		{
			get
			{
				return this.itemType;
			}
		}

		public Microsoft.Expression.DesignModel.Metadata.MemberType MemberType
		{
			get
			{
				return Microsoft.Expression.DesignModel.Metadata.MemberType.Type;
			}
		}

		public ITypeId MemberTypeId
		{
			get
			{
				return PlatformTypes.Type;
			}
		}

		public ITypeMetadata Metadata
		{
			get
			{
				return this.metadata;
			}
		}

		Type Microsoft.Expression.DesignModel.Metadata.IReflectionType.ReflectionType
		{
			get
			{
				return this.type;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public string Namespace
		{
			get
			{
				string str;
				str = (this.isBuilt ? this.lastResolvedType.Namespace : this.unbuiltClrNamespace);
				if (string.IsNullOrEmpty(str))
				{
					return null;
				}
				return str;
			}
		}

		public IType NearestResolvedType
		{
			get
			{
				if (this.nearestSupportedType != null)
				{
					return this.nearestSupportedType;
				}
				if (this.type != null)
				{
					return this;
				}
				if (this.baseType == null)
				{
					return null;
				}
				return this.baseType.NearestResolvedType;
			}
		}

		public IType NullableType
		{
			get
			{
				return this.nullableType;
			}
		}

		public IPlatformMetadata PlatformMetadata
		{
			get
			{
				return this.typeResolver.PlatformMetadata;
			}
		}

		public IAssembly RuntimeAssembly
		{
			get
			{
				return this.assembly;
			}
		}

		public Type RuntimeType
		{
			get
			{
				if (this.nearestSupportedType != null)
				{
					return null;
				}
				return this.type;
			}
		}

		public bool SupportsNullValues
		{
			get
			{
				Type runtimeType = this.RuntimeType;
				if (runtimeType == null)
				{
					return true;
				}
				if (!runtimeType.IsValueType)
				{
					return true;
				}
				return this.NullableType != null;
			}
		}

		public System.ComponentModel.TypeConverter TypeConverter
		{
			get
			{
				return this.typeConverter;
			}
		}

		public ITypeResolver TypeResolver
		{
			get
			{
				return this.typeResolver;
			}
		}

		public string UniqueName
		{
			get
			{
				return this.Name;
			}
		}

		public string XamlSourcePath
		{
			get
			{
				return this.typeResolver.GetXamlSourcePath(this);
			}
		}

		public IXmlNamespace XmlNamespace
		{
			get
			{
				return this.xmlNamespace;
			}
		}

		public ProjectContextType(Microsoft.Expression.DesignModel.Metadata.TypeResolver typeResolver, string internalName)
		{
			if (typeResolver == null)
			{
				throw new ArgumentNullException("typeResolver");
			}
			this.typeResolver = typeResolver;
			this.internalName = internalName;
		}

		private void Cache()
		{
			Type runtimeType;
			ITypeMetadata metadata;
			if (this.isBuilt)
			{
				this.baseType = null;
				if (this.type != null)
				{
					Type baseType = this.type.BaseType;
					if (baseType != null)
					{
						this.baseType = this.typeResolver.GetType(baseType);
					}
				}
				else if (this.lastResolvedType != null && this.lastResolvedType.BaseType != null)
				{
					this.baseType = this.typeResolver.GetType(this.lastResolvedType.BaseType);
				}
				if (this.baseType == null && !this.IsInterface)
				{
					this.baseType = this.typeResolver.ResolveType(PlatformTypes.Object);
				}
			}
			if (this.NearestResolvedType != null)
			{
				runtimeType = this.NearestResolvedType.RuntimeType;
			}
			else
			{
				runtimeType = null;
			}
			Type type = runtimeType;
			if (type == null)
			{
				type = typeof(object);
			}
			if (this.typeResolver.MetadataFactory != null)
			{
				metadata = this.typeResolver.MetadataFactory.GetMetadata(type);
			}
			else
			{
				metadata = null;
			}
			this.metadata = metadata;
			if (this.metadata == null || this.genericDepth >= 10)
			{
				this.nameProperty = null;
				this.defaultContentProperty = null;
			}
			else
			{
				this.nameProperty = this.metadata.NameProperty;
				this.defaultContentProperty = this.metadata.DefaultContentProperty;
			}
			this.itemType = ProjectContextType.GetItemType(this.typeResolver, type);
			this.nullableType = null;
			Type nullableType = PlatformTypeHelper.GetNullableType(type);
			if (nullableType != null)
			{
				this.nullableType = this.typeResolver.GetType(nullableType);
			}
			if (this.PlatformMetadata != null)
			{
				this.typeConverter = this.PlatformMetadata.GetTypeConverter(type);
			}
		}

		[Conditional("DEBUG")]
		internal void CheckAllInvariants()
		{
		}

		[Conditional("DEBUG")]
		private void CheckBaseTypeInvariants()
		{
			string name = this.Name;
			if (!this.IsInterface)
			{
				if (this.baseType == null && this.type != null)
				{
					Type baseType = this.type.BaseType;
				}
				IType type = this.baseType;
			}
			if (this.type != null && this.baseType != null)
			{
				IReflectionType reflectionType = this.baseType as IReflectionType;
				if (reflectionType == null)
				{
					return;
				}
				Type reflectionType1 = reflectionType.ReflectionType;
			}
		}

		[Conditional("DEBUG")]
		private void CheckInvariants()
		{
			string name = this.Name;
			IAssembly projectAssembly = this.typeResolver.ProjectAssembly;
			string str = (projectAssembly != null ? projectAssembly.Name : string.Empty);
			if (this.assemblyName != null)
			{
			}
			if (this.type != null && this.isBuilt)
			{
				string @namespace = this.Namespace;
			}
			IType nearestResolvedType = this.NearestResolvedType;
		}

		[Conditional("DEBUG")]
		private static void CheckSupportedType(Type type)
		{
		}

		public IMember Clone(ITypeResolver typeResolver)
		{
			string name;
			string str;
			if (typeResolver == this.typeResolver)
			{
				return this;
			}
			if (this.IsGenericType)
			{
				if (this.RuntimeType == null)
				{
					return null;
				}
				return typeResolver.GetType(this.RuntimeType);
			}
			if (this.xmlNamespace != null)
			{
				return typeResolver.GetType(this.xmlNamespace, this.Name);
			}
			if (this.typeResolver.ProjectAssembly != null)
			{
				name = this.typeResolver.ProjectAssembly.Name;
			}
			else
			{
				name = null;
			}
			string str1 = name;
			if (typeResolver.ProjectAssembly != null)
			{
				str = typeResolver.ProjectAssembly.Name;
			}
			else
			{
				str = null;
			}
			string str2 = str;
			string str3 = (this.assemblyName == null ? str1 : this.assemblyName);
			if (str2 == str3)
			{
				str3 = null;
			}
			return typeResolver.GetType(str3, TypeHelper.CombineNamespaceAndTypeName(this.Namespace, this.Name));
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			ITypeId typeId = obj as ITypeId;
			if (typeId == null)
			{
				return false;
			}
			ProjectContextType projectContextType = this.typeResolver.ResolveType(typeId) as ProjectContextType;
			if (projectContextType == null)
			{
				return false;
			}
			return projectContextType.internalName == this.internalName;
		}

		private IAssembly GetAssembly(ITypeResolver typeResolver, string assemblyName)
		{
			IAssembly assembly = null;
			if (!string.IsNullOrEmpty(assemblyName))
			{
				assembly = typeResolver.GetAssembly(assemblyName);
			}
			else
			{
				assembly = typeResolver.ProjectAssembly;
				assemblyName = "__Blend_Unknown_Assembly";
			}
			if (assembly == null)
			{
				assembly = ((IPlatformTypes)this.PlatformMetadata).CreateAssembly(assemblyName);
			}
			return assembly;
		}

		public IConstructorArgumentProperties GetConstructorArgumentProperties()
		{
			if (this.constructorArgumentProperties == null)
			{
				this.constructorArgumentProperties = PlatformTypeHelper.GetConstructorArgumentProperties(this);
			}
			return this.constructorArgumentProperties;
		}

		public IList<IConstructor> GetConstructors()
		{
			if (this.constructors == null)
			{
				this.constructors = PlatformTypeHelper.GetConstructors(this.typeResolver, this);
			}
			return this.constructors;
		}

		public IEnumerable<IEvent> GetEvents(MemberAccessTypes access)
		{
			return this.members.GetEvents(access);
		}

		public IList<IType> GetGenericTypeArguments()
		{
			return this.genericTypeArguments;
		}

		public override int GetHashCode()
		{
			return this.hashCode;
		}

		private static IType GetItemType(ITypeResolver typeResolver, Type type)
		{
			CollectionAdapterDescription adapterDescription = CollectionAdapterDescription.GetAdapterDescription(type);
			IType type1 = null;
			if (adapterDescription != null)
			{
				type1 = typeResolver.GetType(adapterDescription.ItemType);
			}
			return type1;
		}

		public IMemberId GetMember(Microsoft.Expression.DesignModel.Metadata.MemberType memberTypes, string memberName, MemberAccessTypes access)
		{
			return this.members.GetMember(memberTypes, memberName, access);
		}

		internal static string GetNameIncludingAnyDeclaringTypes(Type type)
		{
			string name = type.Name;
			while (type.DeclaringType != null)
			{
				type = type.DeclaringType;
				CultureInfo invariantCulture = CultureInfo.InvariantCulture;
				object[] objArray = new object[] { type.Name, name };
				name = string.Format(invariantCulture, "{0}+{1}", objArray);
			}
			return name;
		}

		public IEnumerable<IProperty> GetProperties(MemberAccessTypes access)
		{
			return this.members.GetProperties(access);
		}

		public Type GetRuntimeType()
		{
			return this.GetRuntimeType(this.typeResolver, this);
		}

		private Type GetRuntimeType(ITypeResolver typeResolver, ProjectContextType typeId)
		{
			Type type;
			if (RuntimeGeneratedTypesHelper.IsControlEditingAssembly(typeId.assembly))
			{
				Type sourceType = ControlEditingDesignTypeGenerator.GetSourceType(typeId.RuntimeType);
				if (sourceType != null)
				{
					ProjectContextType projectContextType = typeResolver.GetType(sourceType) as ProjectContextType;
					if (projectContextType != null)
					{
						Type runtimeType = projectContextType.GetRuntimeType();
						if (runtimeType != sourceType && runtimeType != null)
						{
							Type type1 = (new ControlEditingDesignTypeGenerator(typeResolver)).DefineType(runtimeType);
							if (type1 != typeId.RuntimeType)
							{
								this.assemblyName = AssemblyHelper.GetAssemblyName(type1.Assembly).Name;
								this.assembly = this.GetAssembly(this.typeResolver, this.assemblyName);
								return type1;
							}
						}
					}
				}
			}
			if (typeId.arrayItemType == null)
			{
				IAssembly runtimeAssembly = typeId.RuntimeAssembly;
				if (runtimeAssembly != null)
				{
					runtimeAssembly = this.GetAssembly(typeResolver, runtimeAssembly.Name);
				}
				if (runtimeAssembly != null)
				{
					Type type2 = PlatformTypeHelper.GetType(runtimeAssembly, typeId.FullName);
					if (type2 != null)
					{
						if (!typeId.IsGenericType)
						{
							return type2;
						}
						IList<IType> genericTypeArguments = typeId.GetGenericTypeArguments();
						int count = genericTypeArguments.Count;
						if (count > 0)
						{
							Type[] typeArray = new Type[count];
							for (int i = 0; i < count; i++)
							{
								Type runtimeType1 = this.GetRuntimeType(genericTypeArguments[i]);
								if (runtimeType1 == null)
								{
									return null;
								}
								typeArray[i] = runtimeType1;
							}
							try
							{
								type = type2.MakeGenericType(typeArray);
							}
							catch (ArgumentException argumentException)
							{
								return null;
							}
							return type;
						}
					}
				}
			}
			else
			{
				Type runtimeType2 = this.GetRuntimeType(typeId.arrayItemType);
				if (runtimeType2 != null)
				{
					if (typeId.arrayRank <= 1)
					{
						return runtimeType2.MakeArrayType();
					}
					return runtimeType2.MakeArrayType(typeId.arrayRank);
				}
			}
			return null;
		}

		private Type GetRuntimeType(ITypeId typeId)
		{
			IResolvableRuntimeType resolvableRuntimeType = typeId as IResolvableRuntimeType;
			if (resolvableRuntimeType == null)
			{
				return null;
			}
			return resolvableRuntimeType.GetRuntimeType();
		}

		private static string GetTypeConverterId(System.ComponentModel.TypeConverter typeConverter)
		{
			if (typeConverter == null)
			{
				return string.Empty;
			}
			return typeConverter.GetType().FullName;
		}

		public bool HasDefaultConstructor(bool supportInternal)
		{
			if (this.type == null)
			{
				return true;
			}
			return TypeUtilities.HasDefaultConstructor(this.type, supportInternal);
		}

		public void Initialize(IXmlNamespace xmlNamespace, Type type, string key)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			PlatformTypes platformMetadata = (PlatformTypes)this.PlatformMetadata;
			this.isBuilt = true;
			this.xmlNamespace = xmlNamespace;
			this.type = type;
			Type nearestSupportedType = platformMetadata.GetNearestSupportedType(this.type);
			if (nearestSupportedType != null)
			{
				this.nearestSupportedType = platformMetadata.GetType(nearestSupportedType);
			}
			this.lastResolvedType = this.type;
			this.name = ProjectContextType.GetNameIncludingAnyDeclaringTypes(this.type);
			Assembly assembly = this.type.Assembly;
			IAssembly projectAssembly = this.typeResolver.ProjectAssembly;
			if (projectAssembly == null || !projectAssembly.CompareTo(assembly))
			{
				this.assemblyName = AssemblyHelper.GetAssemblyName(assembly).Name;
				if (projectAssembly != null)
				{
					bool name = this.assemblyName == projectAssembly.Name;
				}
			}
			this.members = new MemberCollection(this.typeResolver, this);
			this.assembly = this.GetAssembly(this.typeResolver, this.assemblyName);
			if (type.IsArray)
			{
				this.arrayItemType = ProjectContextType.GetItemType(this.typeResolver, type);
				this.arrayRank = type.GetArrayRank();
			}
			this.genericTypeArguments = PlatformTypeHelper.GetGenericTypeArguments(this.typeResolver, this.type);
			if (this.genericTypeArguments.Count > 0)
			{
				foreach (IType genericTypeArgument in this.genericTypeArguments)
				{
					ProjectContextType projectContextType = genericTypeArgument as ProjectContextType;
					if (projectContextType == null)
					{
						continue;
					}
					this.genericDepth = Math.Max(projectContextType.genericDepth + 1, this.genericDepth);
				}
			}
			this.Cache();
			this.hashCode = key.GetHashCode();
		}

		public void Initialize(UnbuiltTypeDescription typeInfo, string key)
		{
			this.isBuilt = false;
			this.baseType = this.typeResolver.ResolveType(typeInfo.BaseType);
			this.type = this.baseType.RuntimeType;
			this.xmlNamespace = typeInfo.XmlNamespace;
			this.name = typeInfo.TypeName;
			this.unbuiltClrNamespace = typeInfo.ClrNamespace;
			this.assembly = typeInfo.AssemblyReference;
			if (this.assembly != this.typeResolver.ProjectAssembly)
			{
				this.assemblyName = this.assembly.Name;
			}
			this.lastResolvedType = this.type;
			this.members = new MemberCollection(this.typeResolver, this);
			this.genericTypeArguments = PlatformTypeHelper.GetGenericTypeArguments(this.typeResolver, null);
			this.Cache();
			this.hashCode = key.GetHashCode();
		}

		public void InitializeClass()
		{
			if (this.type != null)
			{
				try
				{
					RuntimeHelpers.RunClassConstructor(this.type.TypeHandle);
				}
				catch (Exception exception)
				{
					this.initializationException = exception;
				}
			}
		}

		public bool IsAssignableFrom(ITypeId type)
		{
			return ((PlatformTypes)this.PlatformMetadata).IsAssignableFrom(this, type);
		}

		public bool IsInProject(ITypeResolver typeResolver)
		{
			return this.typeResolver == typeResolver;
		}

		void Microsoft.Expression.DesignModel.Metadata.IMutableMembers.AddMember(IMember memberId)
		{
			this.members.AddMember(memberId);
		}

		IMember Microsoft.Expression.DesignModel.Metadata.IMutableMembers.GetMember(Microsoft.Expression.DesignModel.Metadata.MemberType memberTypes, string uniqueName)
		{
			return this.members.GetMemberByUniqueName(memberTypes, uniqueName);
		}

		public bool Refresh()
		{
			ITypeId typeId = this.baseType;
			IPropertyId propertyId = this.nameProperty;
			IPropertyId propertyId1 = this.defaultContentProperty;
			ITypeId typeId1 = this.itemType;
			ITypeId typeId2 = this.nullableType;
			System.ComponentModel.TypeConverter typeConverter = this.typeConverter;
			IAssembly assembly = this.GetAssembly(this.typeResolver, this.assemblyName);
			if (!assembly.IsLoaded)
			{
				return true;
			}
			this.assembly = assembly;
			this.type = this.GetRuntimeType();
			this.typeConverter = null;
			this.initializationException = null;
			if (this.type != null)
			{
				this.lastResolvedType = this.type;
				this.isBuilt = true;
			}
			this.Cache();
			bool flag = true;
			if (this.constructors != null)
			{
				foreach (Constructor constructor in this.constructors)
				{
					ICachedMemberInfo cachedMemberInfo = constructor;
					if (cachedMemberInfo == null || cachedMemberInfo.Refresh())
					{
						continue;
					}
					flag = false;
				}
				if (flag && this.type != null)
				{
					int num = 0;
					ConstructorInfo[] constructors = PlatformTypeHelper.GetConstructors(this.type);
					if (constructors != null)
					{
						ConstructorInfo[] constructorInfoArray = constructors;
						for (int i = 0; i < (int)constructorInfoArray.Length; i++)
						{
							if (PlatformTypeHelper.IsAccessibleConstructor(constructorInfoArray[i]))
							{
								num++;
							}
						}
					}
					if (num != this.constructors.Count)
					{
						flag = false;
					}
				}
			}
			if (!this.members.Refresh())
			{
				flag = false;
			}
			if (flag && (typeId != this.baseType || typeId1 != this.itemType || typeId2 != this.nullableType))
			{
				flag = false;
			}
			if (flag && (propertyId != this.nameProperty || propertyId1 != this.defaultContentProperty))
			{
				flag = false;
			}
			if (flag && ProjectContextType.GetTypeConverterId(typeConverter) != ProjectContextType.GetTypeConverterId(this.typeConverter))
			{
				flag = false;
			}
			if (flag && this.constructorArgumentProperties != null)
			{
				IConstructorArgumentProperties constructorArgumentProperties = PlatformTypeHelper.GetConstructorArgumentProperties(this);
				if (constructorArgumentProperties.Count == this.constructorArgumentProperties.Count)
				{
					foreach (string constructorArgumentProperty in constructorArgumentProperties)
					{
						if (constructorArgumentProperties[constructorArgumentProperty] == this.constructorArgumentProperties[constructorArgumentProperty])
						{
							continue;
						}
						flag = false;
						break;
					}
				}
				else
				{
					flag = false;
				}
			}
			return flag;
		}

		public override string ToString()
		{
			return this.FullName;
		}
	}
}