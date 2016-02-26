using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.DesignModel.Metadata
{
	internal sealed class PlatformType : IType, IMember, ITypeId, IMemberId, IMutableMembers, IResolvableRuntimeType, IReflectionType, IReferenceType
	{
		private IAssembly assembly;

		private Type type;

		private Type referenceType;

		private IType declaringType;

		private IType baseType;

		private IXmlNamespace xmlNamespace;

		private ITypeMetadata metadata;

		private IList<IType> genericTypeArguments;

		private IList<IConstructor> constructors;

		private IConstructorArgumentProperties constructorArgumentProperties;

		private MemberCollection members;

		private IType itemType;

		private IType nullableType;

		private System.ComponentModel.TypeConverter typeConverter;

		private PlatformTypes platformTypes;

		private string fullNameCache;

		private bool isExpression;

		private PlatformType.CachedFlags cachedFlags;

		private uint uniqueId;

		public MemberAccessType Access
		{
			get
			{
				return PlatformTypeHelper.GetMemberAccess(this.type);
			}
		}

		public IType BaseType
		{
			get
			{
				return this.GetCachedValue<IType>(PlatformType.CachedFlags.BaseType, ref this.baseType, () => {
					for (Type i = this.type.BaseType; i != null; i = i.BaseType)
					{
						IType type = this.platformTypes.GetType(i);
						if (!this.platformTypes.IsNullType(type))
						{
							return type;
						}
					}
					return null;
				});
			}
		}

		public IType DeclaringType
		{
			get
			{
				return this.declaringType;
			}
		}

		public ITypeId DeclaringTypeId
		{
			get
			{
				return this.declaringType;
			}
		}

		public string FullName
		{
			get
			{
				if (this.fullNameCache == null)
				{
					this.fullNameCache = this.type.FullName;
				}
				return this.fullNameCache;
			}
		}

		public Exception InitializationException
		{
			get
			{
				return null;
			}
		}

		public bool IsAbstract
		{
			get
			{
				return this.type.IsAbstract;
			}
		}

		public bool IsArray
		{
			get
			{
				return this.type.IsArray;
			}
		}

		public bool IsBinding
		{
			get
			{
				return this.platformTypes.IsBinding(this.type);
			}
		}

		public bool IsBuilt
		{
			get
			{
				return true;
			}
		}

		public bool IsExpression
		{
			get
			{
				return this.GetCachedValue<bool>(PlatformType.CachedFlags.IsExpression, ref this.isExpression, () => this.platformTypes.IsExpression(this.type));
			}
		}

		public bool IsGenericType
		{
			get
			{
				return this.GetGenericTypeArguments().Count > 0;
			}
		}

		public bool IsInterface
		{
			get
			{
				return this.type.IsInterface;
			}
		}

		public bool IsResolvable
		{
			get
			{
				return true;
			}
		}

		public bool IsResource
		{
			get
			{
				return this.platformTypes.IsResource(this.type);
			}
		}

		public IType ItemType
		{
			get
			{
				return this.GetCachedValue<IType>(PlatformType.CachedFlags.ItemType, ref this.itemType, () => {
					CollectionAdapterDescription adapterDescription = CollectionAdapterDescription.GetAdapterDescription(this.type, this.referenceType);
					if (adapterDescription != null)
					{
						Type itemType = adapterDescription.ItemType;
						if (itemType != null)
						{
							return this.platformTypes.GetType(itemType);
						}
					}
					return null;
				});
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
				return this.GetCachedValue<ITypeMetadata>(PlatformType.CachedFlags.Metadata, ref this.metadata, () => this.platformTypes.TypeMetadataFactory.GetMetadata(this.type));
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
				return this.type.Name;
			}
		}

		public string Namespace
		{
			get
			{
				return this.type.Namespace;
			}
		}

		public IType NearestResolvedType
		{
			get
			{
				return this;
			}
		}

		public IType NullableType
		{
			get
			{
				return this.GetCachedValue<IType>(PlatformType.CachedFlags.NullableType, ref this.nullableType, () => {
					Type nullableType = PlatformTypeHelper.GetNullableType(this.type);
					if (nullableType == null)
					{
						return null;
					}
					return this.platformTypes.GetType(nullableType);
				});
			}
		}

		public IPlatformMetadata PlatformMetadata
		{
			get
			{
				return this.platformTypes;
			}
		}

		public IAssembly ReferenceAssembly
		{
			get
			{
				if (this.referenceType == null)
				{
					return null;
				}
				return new Microsoft.Expression.DesignModel.Metadata.ReferenceAssembly(this.referenceType.Assembly);
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
				return this.type;
			}
		}

		public bool SupportsNullValues
		{
			get
			{
				if (!this.type.IsValueType)
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
				if (this.typeConverter == null)
				{
					this.typeConverter = this.platformTypes.GetTypeConverter(this.type);
				}
				return this.typeConverter;
			}
		}

		public uint UniqueId
		{
			get
			{
				return this.uniqueId;
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
				return null;
			}
		}

		public IXmlNamespace XmlNamespace
		{
			get
			{
				return this.GetCachedValue<IXmlNamespace>(PlatformType.CachedFlags.XmlNamespace, ref this.xmlNamespace, () => this.platformTypes.XmlnsMap.GetNamespace(this.assembly, this.type));
			}
		}

		public PlatformType(PlatformTypes platformTypes)
		{
			this.platformTypes = platformTypes;
			PlatformTypes platformType = platformTypes;
			uint platformTypeUniqueId = platformType.PlatformTypeUniqueId;
			uint num = platformTypeUniqueId;
			platformType.PlatformTypeUniqueId = platformTypeUniqueId + 1;
			this.uniqueId = num;
		}

		public IMember Clone(ITypeResolver typeResolver)
		{
			if (this.PlatformMetadata == typeResolver.PlatformMetadata)
			{
				return this;
			}
			return typeResolver.GetType(this.type);
		}

		public bool Equals(PlatformType other)
		{
			if (other.type != this.type)
			{
				return false;
			}
			if (this.referenceType == other.referenceType)
			{
				return true;
			}
			if (this.referenceType == null || other.referenceType == null)
			{
				return false;
			}
			if (string.Compare(this.referenceType.Assembly.Location, other.referenceType.Assembly.Location, StringComparison.OrdinalIgnoreCase) == 0)
			{
				return true;
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			PlatformType platformType = obj as PlatformType;
			if (platformType != null)
			{
				return this.Equals(platformType);
			}
			FullNameTypeId fullNameTypeId = obj as FullNameTypeId;
			if (fullNameTypeId == null)
			{
				return false;
			}
			if (this.FullName == fullNameTypeId.FullName)
			{
				return true;
			}
			return false;
		}

		private T GetCachedValue<T>(PlatformType.CachedFlags cachedFlag, ref T value, Func<T> result)
		{
			if ((int)(this.cachedFlags & cachedFlag) != 0)
			{
				return value;
			}
			PlatformType platformType = this;
			platformType.cachedFlags = platformType.cachedFlags | cachedFlag;
			T t = result();
			T t1 = t;
			value = t;
			return t1;
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
				this.constructors = PlatformTypeHelper.GetConstructors(this.platformTypes.DefaultTypeResolver, this);
			}
			return this.constructors;
		}

		public IEnumerable<IEvent> GetEvents(MemberAccessTypes access)
		{
			return this.members.GetEvents(access);
		}

		public IList<IType> GetGenericTypeArguments()
		{
			if (this.genericTypeArguments == null)
			{
				this.genericTypeArguments = PlatformTypeHelper.GetGenericTypeArguments(this.platformTypes.DefaultTypeResolver, this.type);
			}
			return this.genericTypeArguments;
		}

		public override int GetHashCode()
		{
			return (int)this.uniqueId;
		}

		public IMemberId GetMember(Microsoft.Expression.DesignModel.Metadata.MemberType memberTypes, string memberName, MemberAccessTypes access)
		{
			return this.members.GetMember(memberTypes, memberName, access);
		}

		public IEnumerable<IProperty> GetProperties(MemberAccessTypes access)
		{
			return this.members.GetProperties(access);
		}

		public bool HasDefaultConstructor(bool supportInternal)
		{
			return TypeUtilities.HasDefaultConstructor(this.type, supportInternal);
		}

		public void Initialize(IAssembly assembly, Type type, Type referenceType)
		{
			this.assembly = assembly;
			this.type = type;
			this.referenceType = referenceType;
			Type declaringType = this.type.DeclaringType;
			if (declaringType != null)
			{
				this.declaringType = this.platformTypes.GetType(declaringType);
			}
			this.members = new MemberCollection(this.platformTypes.DefaultTypeResolver, this, referenceType);
		}

		public void InitializeClass()
		{
			RuntimeHelpers.RunClassConstructor(this.type.TypeHandle);
		}

		public bool IsAssignableFrom(ITypeId type)
		{
			return this.platformTypes.IsAssignableFrom(this, type);
		}

		public bool IsInProject(ITypeResolver typeResolver)
		{
			return typeResolver.AssemblyReferences.Contains(this.assembly);
		}

		void Microsoft.Expression.DesignModel.Metadata.IMutableMembers.AddMember(IMember memberId)
		{
			this.members.AddMember(memberId);
		}

		IMember Microsoft.Expression.DesignModel.Metadata.IMutableMembers.GetMember(Microsoft.Expression.DesignModel.Metadata.MemberType memberTypes, string uniqueName)
		{
			return this.members.GetMemberByUniqueName(memberTypes, uniqueName);
		}

		Type Microsoft.Expression.DesignModel.Metadata.IResolvableRuntimeType.GetRuntimeType()
		{
			return this.type;
		}

		public override string ToString()
		{
			return this.FullName;
		}

		[Flags]
		private enum CachedFlags : uint
		{
			BaseType = 1,
			XmlNamespace = 2,
			Metadata = 4,
			ItemType = 8,
			NullableType = 16,
			IsExpression = 32
		}
	}
}