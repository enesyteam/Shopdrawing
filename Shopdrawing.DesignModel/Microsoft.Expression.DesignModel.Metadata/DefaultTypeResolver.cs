using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignModel.Metadata
{
	internal sealed class DefaultTypeResolver : ITypeResolver, IMetadataResolver
	{
		private PlatformTypes platformTypes;

		public ICollection<IAssembly> AssemblyReferences
		{
			get
			{
				return this.platformTypes.DefaultAssemblyReferences;
			}
		}

		public IPlatformMetadata PlatformMetadata
		{
			get
			{
				return this.platformTypes;
			}
		}

		public IAssembly ProjectAssembly
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public IXmlNamespaceTypeResolver ProjectNamespaces
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public string ProjectPath
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public string RootNamespace
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public DefaultTypeResolver(PlatformTypes platformTypes)
		{
			this.platformTypes = platformTypes;
		}

		public bool EnsureAssemblyReferenced(string assemblyPath)
		{
			throw new NotSupportedException();
		}

		public IAssembly GetAssembly(string assemblyName)
		{
			IAssembly assembly;
			using (IEnumerator<IAssembly> enumerator = this.AssemblyReferences.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					IAssembly current = enumerator.Current;
					if (current.Name != assemblyName)
					{
						continue;
					}
					assembly = current;
					return assembly;
				}
				return null;
			}
			return assembly;
		}

		public object GetCapabilityValue(PlatformCapability capability)
		{
			return this.PlatformMetadata.GetCapabilityValue(capability);
		}

		public IType GetType(Type type)
		{
			return this.platformTypes.GetType(type);
		}

		public IType GetType(IXmlNamespace xmlNamespace, string typeName)
		{
			if (xmlNamespace == null)
			{
				throw new ArgumentNullException("xmlNamespace");
			}
			if (string.IsNullOrEmpty(typeName))
			{
				throw new ArgumentException(ExceptionStringTable.GetTypeCannotBeCalledWithNullOrEmptyTypeName, "typeName");
			}
			IType type = this.platformTypes.XmlnsMap.GetType(xmlNamespace, typeName);
			if (type != null)
			{
				return type;
			}
			return null;
		}

		public IType GetType(string assemblyName, string typeName)
		{
			if (string.IsNullOrEmpty(assemblyName))
			{
				return null;
			}
			if (string.IsNullOrEmpty(typeName))
			{
				throw new ArgumentException(ExceptionStringTable.GetTypeCannotBeCalledWithNullOrEmptyTypeName, "typeName");
			}
			IAssembly platformAssembly = this.platformTypes.GetPlatformAssembly(assemblyName);
			Type type = PlatformTypeHelper.GetType(platformAssembly, typeName);
			if (type == null)
			{
				return null;
			}
			return this.platformTypes.GetType(type);
		}

		public IType GetType(IAssembly assembly, string typeName)
		{
			return PlatformTypeHelper.GetType(this, assembly, typeName);
		}

		public bool InTargetAssembly(IType typeId)
		{
			return false;
		}

		public bool IsCapabilitySet(PlatformCapability capability)
		{
			return this.PlatformMetadata.IsCapabilitySet(capability);
		}

		public IProperty ResolveProperty(IPropertyId propertyId)
		{
			return this.platformTypes.ResolveProperty(propertyId);
		}

		public IType ResolveType(ITypeId typeId)
		{
			return this.platformTypes.ResolveType(typeId);
		}

		public event EventHandler<TypesChangedEventArgs> TypesChanged
		{
			add
			{
			}
			remove
			{
			}
		}

		public event EventHandler<TypesChangedEventArgs> TypesChangedEarly
		{
			add
			{
			}
			remove
			{
			}
		}
	}
}