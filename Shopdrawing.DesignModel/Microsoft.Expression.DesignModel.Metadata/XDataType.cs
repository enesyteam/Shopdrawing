using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Microsoft.Expression.DesignModel.Metadata
{
	internal sealed class XDataType : IType, IMember, ITypeId, IMemberId
	{
		private readonly static ReadOnlyCollection<IProperty> emptyProperties;

		private readonly static ReadOnlyCollection<IPropertyId> emptyPropertyIds;

		private XDataType.InlineXmlTypeMetadata metadata;

		private IPlatformTypes platformTypes;

		private System.ComponentModel.TypeConverter typeConverter;

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
				return this.platformTypes.ResolveType(PlatformTypes.Object);
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
				return this.Name;
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
				return true;
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
				return Microsoft.Expression.DesignModel.Metadata.MemberType.Other;
			}
		}

		public ITypeId MemberTypeId
		{
			get
			{
				return this.platformTypes.ResolveType(PlatformTypes.Type);
			}
		}

		public ITypeMetadata Metadata
		{
			get
			{
				return this.metadata;
			}
		}

		public string Name
		{
			get
			{
				return "XData";
			}
		}

		public string Namespace
		{
			get
			{
				return null;
			}
		}

		public IType NearestResolvedType
		{
			get
			{
				return this.platformTypes.ResolveType(PlatformTypes.Object);
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
				return this.platformTypes;
			}
		}

		public IAssembly RuntimeAssembly
		{
			get
			{
				return this.platformTypes.DefaultAssemblyReferences[0];
			}
		}

		public Type RuntimeType
		{
			get
			{
				return this.platformTypes.ResolveType(PlatformTypes.Object).RuntimeType;
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
				return this.typeConverter;
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
				return Microsoft.Expression.DesignModel.Metadata.XmlNamespace.XamlXmlNamespace;
			}
		}

		static XDataType()
		{
			XDataType.emptyProperties = new ReadOnlyCollection<IProperty>(new List<IProperty>());
			XDataType.emptyPropertyIds = new ReadOnlyCollection<IPropertyId>(new List<IPropertyId>());
		}

		public XDataType(IPlatformTypes platformTypes)
		{
			this.metadata = new XDataType.InlineXmlTypeMetadata(platformTypes);
			this.platformTypes = platformTypes;
			this.typeConverter = this.platformTypes.GetTypeConverter(typeof(object));
		}

		public IMember Clone(ITypeResolver typeResolver)
		{
			if (this.PlatformMetadata == typeResolver.PlatformMetadata)
			{
				return this;
			}
			return typeResolver.GetType(this.XmlNamespace, this.Name);
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
			return ReadOnlyCollections<IEvent>.Empty;
		}

		public IList<IType> GetGenericTypeArguments()
		{
			return ReadOnlyCollections<IType>.Empty;
		}

		public IMemberId GetMember(Microsoft.Expression.DesignModel.Metadata.MemberType memberTypes, string memberName, MemberAccessTypes access)
		{
			return null;
		}

		public IEnumerable<IProperty> GetProperties(MemberAccessTypes access)
		{
			return ReadOnlyCollections<IProperty>.Empty;
		}

		public bool HasDefaultConstructor(bool supportInternal)
		{
			return false;
		}

		public void InitializeClass()
		{
		}

		public bool IsAssignableFrom(ITypeId type)
		{
			return type == this;
		}

		public bool IsInProject(ITypeResolver typeResolver)
		{
			return true;
		}

		private sealed class InlineXmlTypeMetadata : ITypeMetadata
		{
			private ITypeResolver typeResolver;

			public ReadOnlyCollection<IPropertyId> ContentProperties
			{
				get
				{
					return null;
				}
			}

			public IProperty DefaultContentProperty
			{
				get
				{
					return null;
				}
			}

			public IPropertyId ImplicitDictionaryKeyProperty
			{
				get
				{
					return null;
				}
			}

			public bool IsNameScope
			{
				get
				{
					return false;
				}
			}

			public bool IsWhitespaceSignificant
			{
				get
				{
					return false;
				}
			}

			public IProperty NameProperty
			{
				get
				{
					return DesignTimeProperties.ResolveDesignTimeReferenceStep(DesignTimeProperties.XNameProperty, this.TypeResolver.PlatformMetadata);
				}
			}

			public ReadOnlyCollection<IProperty> Properties
			{
				get
				{
					return XDataType.emptyProperties;
				}
			}

			public IPropertyId ResourcesProperty
			{
				get
				{
					return null;
				}
			}

			public bool ShouldTrimSurroundingWhitespace
			{
				get
				{
					return false;
				}
			}

			public ReadOnlyCollection<IPropertyId> StyleProperties
			{
				get
				{
					return XDataType.emptyPropertyIds;
				}
			}

			public bool SupportsInlineXml
			{
				get
				{
					return false;
				}
			}

			public ITypeResolver TypeResolver
			{
				get
				{
					return this.typeResolver;
				}
				set
				{
				}
			}

			public InlineXmlTypeMetadata(IPlatformTypes platformTypes)
			{
				this.typeResolver = platformTypes.DefaultTypeResolver;
			}

			public Type GetStylePropertyTargetType(IPropertyId propertyKey)
			{
				return null;
			}

			public bool IsNameProperty(IPropertyId propertyKey)
			{
				return DesignTimeProperties.XNameProperty.Equals(propertyKey);
			}
		}
	}
}