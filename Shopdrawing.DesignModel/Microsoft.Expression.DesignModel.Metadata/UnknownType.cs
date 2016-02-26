using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Microsoft.Expression.DesignModel.Metadata
{
	internal sealed class UnknownType : IType, IMember, ITypeId, IMemberId, ICachedMemberInfo
	{
		private readonly ITypeResolver typeResolver;

		private readonly IXmlNamespace xmlNamespace;

		private readonly IAssembly assembly;

		private readonly string clrNamespace;

		private readonly string typeName;

		private readonly int hashCode;

		public MemberAccessType Access
		{
			get
			{
				return MemberAccessType.Public;
			}
		}

		public IType BaseType
		{
			get
			{
				return this.typeResolver.ResolveType(PlatformTypes.Object);
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
				return TypeHelper.CombineNamespaceAndTypeName(this.clrNamespace, this.typeName);
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
				return false;
			}
		}

		public bool IsArray
		{
			get
			{
				return false;
			}
		}

		public bool IsBinding
		{
			get
			{
				return false;
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
				return false;
			}
		}

		public bool IsGenericType
		{
			get
			{
				return false;
			}
		}

		public bool IsInterface
		{
			get
			{
				return false;
			}
		}

		public bool IsResolvable
		{
			get
			{
				return false;
			}
		}

		public bool IsResource
		{
			get
			{
				return false;
			}
		}

		public IType ItemType
		{
			get
			{
				return null;
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
				return this.typeResolver.ResolveType(PlatformTypes.Type);
			}
		}

		public ITypeMetadata Metadata
		{
			get
			{
				return this.BaseType.Metadata;
			}
		}

		public string Name
		{
			get
			{
				return this.typeName;
			}
		}

		public string Namespace
		{
			get
			{
				return this.clrNamespace;
			}
		}

		public IType NearestResolvedType
		{
			get
			{
				return this.typeResolver.ResolveType(PlatformTypes.Object);
			}
		}

		public IType NullableType
		{
			get
			{
				return null;
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
				return null;
			}
		}

		public bool SupportsNullValues
		{
			get
			{
				return false;
			}
		}

		public System.ComponentModel.TypeConverter TypeConverter
		{
			get
			{
				return this.BaseType.TypeConverter;
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
				return this.xmlNamespace;
			}
		}

		public UnknownType(ITypeResolver typeResolver, IXmlNamespace xmlNamespace, string typeName) : this(typeResolver, typeName)
		{
			if (xmlNamespace == null)
			{
				throw new ArgumentNullException("xmlNamespace");
			}
			this.xmlNamespace = xmlNamespace;
			this.hashCode = this.xmlNamespace.GetHashCode() ^ this.typeName.GetHashCode();
		}

		public UnknownType(ITypeResolver typeResolver, IAssembly assembly, string clrNamespace, string typeName) : this(typeResolver, typeName)
		{
			if (assembly == null)
			{
				throw new ArgumentNullException("assembly");
			}
			if (clrNamespace == null)
			{
				clrNamespace = string.Empty;
			}
			this.assembly = assembly;
			this.clrNamespace = clrNamespace;
			this.hashCode = this.FullName.GetHashCode();
		}

		private UnknownType(ITypeResolver typeResolver, string typeName)
		{
			if (typeResolver == null)
			{
				throw new ArgumentNullException("typeResolver");
			}
			if (string.IsNullOrEmpty(typeName))
			{
				throw new ArgumentNullException("typeName");
			}
			this.typeResolver = typeResolver;
			this.typeName = typeName;
		}

		public IMember Clone(ITypeResolver typeResolver)
		{
			if (this.xmlNamespace != null)
			{
				return new UnknownType(this.typeResolver, this.xmlNamespace, this.typeName);
			}
			return new UnknownType(this.typeResolver, this.assembly, this.clrNamespace, this.typeName);
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			UnknownType unknownType = obj as UnknownType;
			if (unknownType == null || !(this.typeName == unknownType.typeName) || !(this.clrNamespace == unknownType.clrNamespace) || !object.Equals(this.assembly, unknownType.assembly))
			{
				return false;
			}
			return object.Equals(unknownType.xmlNamespace, this.xmlNamespace);
		}

		public IConstructorArgumentProperties GetConstructorArgumentProperties()
		{
			return PlatformTypeHelper.EmptyConstructorArgumentProperties;
		}

		public IList<IConstructor> GetConstructors()
		{
			return ReadOnlyCollections<IConstructor>.Empty;
		}

		public IEnumerable<IEvent> GetEvents(MemberAccessTypes access)
		{
			yield break;
		}

		public IList<IType> GetGenericTypeArguments()
		{
			return ReadOnlyCollections<IType>.Empty;
		}

		public override int GetHashCode()
		{
			return this.hashCode;
		}

		public IMemberId GetMember(Microsoft.Expression.DesignModel.Metadata.MemberType memberTypes, string memberName, MemberAccessTypes access)
		{
			return this.BaseType.GetMember(memberTypes, memberName, access);
		}

		public IEnumerable<IProperty> GetProperties(MemberAccessTypes access)
		{
			yield break;
		}

		public bool HasDefaultConstructor(bool supportInternal)
		{
			return true;
		}

		public void InitializeClass()
		{
		}

		public bool IsAssignableFrom(ITypeId type)
		{
			return ((PlatformTypes)this.typeResolver.PlatformMetadata).IsAssignableFrom(this, type);
		}

		public bool IsInProject(ITypeResolver typeResolver)
		{
			if (this.assembly == null)
			{
				return true;
			}
			return typeResolver.AssemblyReferences.Contains(this.assembly);
		}

		public bool Refresh()
		{
			return false;
		}

		public override string ToString()
		{
			return this.FullName;
		}
	}
}