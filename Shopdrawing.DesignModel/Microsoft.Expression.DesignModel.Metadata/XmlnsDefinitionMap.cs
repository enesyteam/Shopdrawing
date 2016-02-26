using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Expression.DesignModel.Metadata
{
	public abstract class XmlnsDefinitionMap : IXmlNamespaceTypeResolver
	{
		private ITypeResolver typeResolver;

		private Dictionary<IXmlNamespace, XmlnsDefinitionMap.XmlNamespaceTypeMap> map;

		private IAssembly targetAssembly;

		private XmlNamespaceCanonicalization canonicalization;

		private Dictionary<string, Dictionary<string, string>> clrNamespaceWorkaround = new Dictionary<string, Dictionary<string, string>>();

		protected ITypeResolver TypeResolver
		{
			get
			{
				return this.typeResolver;
			}
		}

		protected XmlnsDefinitionMap(ITypeResolver typeResolver, IAssembly targetAssembly, XmlNamespaceCanonicalization canonicalization)
		{
			this.typeResolver = typeResolver;
			this.map = new Dictionary<IXmlNamespace, XmlnsDefinitionMap.XmlNamespaceTypeMap>();
			this.targetAssembly = targetAssembly;
			this.canonicalization = canonicalization;
		}

		protected void AddClrNamespaceWorkaround(IAssembly assembly, string clrNamespace, string prefix)
		{
			Dictionary<string, string> strs;
			XmlnsDefinitionMap.AssemblyNamespace assemblyNamespace = new XmlnsDefinitionMap.AssemblyNamespace(assembly, clrNamespace);
			if (!this.clrNamespaceWorkaround.TryGetValue(assemblyNamespace.Name, out strs))
			{
				strs = new Dictionary<string, string>();
				this.clrNamespaceWorkaround.Add(assemblyNamespace.Name, strs);
			}
			if (!strs.ContainsKey(clrNamespace))
			{
				strs.Add(clrNamespace, prefix);
			}
		}

		protected XmlNamespace AddNamespace(IAssembly assembly, string xamlNamespace, string clrNamespace)
		{
			XmlNamespace @namespace = XmlNamespace.ToNamespace(xamlNamespace, this.canonicalization);
			XmlnsDefinitionMap.XmlNamespaceTypeMap typeMapCreatingIfNecessary = this.GetTypeMapCreatingIfNecessary(@namespace);
			typeMapCreatingIfNecessary.AssemblyNamespaces.Add(new XmlnsDefinitionMap.AssemblyNamespace(assembly, clrNamespace));
			return @namespace;
		}

		public bool Contains(IXmlNamespace xmlNamespace)
		{
			return this.map.ContainsKey(xmlNamespace);
		}

		public string GetClrNamespacePrefixWorkaround(IAssembly assemblyReference, string clrNamespace)
		{
			Dictionary<string, string> strs;
			string str;
			if (this.clrNamespaceWorkaround.TryGetValue(assemblyReference.Name, out strs) && strs.TryGetValue(clrNamespace, out str))
			{
				return str;
			}
			return null;
		}

		public string GetDefaultPrefix(IXmlNamespace xmlNamespace)
		{
			XmlnsDefinitionMap.XmlNamespaceTypeMap xmlNamespaceTypeMap;
			if (!this.map.TryGetValue(xmlNamespace, out xmlNamespaceTypeMap))
			{
				return null;
			}
			return xmlNamespaceTypeMap.DefaultPrefix;
		}

		public virtual IXmlNamespace GetNamespace(IAssembly assembly, Type type)
		{
			return this.GetNamespace(assembly, type.Namespace);
		}

		public IXmlNamespace GetNamespace(IAssembly assemblyReference, string clrNamespace)
		{
			IXmlNamespace key;
			string name = assemblyReference.Name;
			IXmlNamespace xmlNamespace = null;
			Dictionary<IXmlNamespace, XmlnsDefinitionMap.XmlNamespaceTypeMap>.Enumerator enumerator = this.map.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<IXmlNamespace, XmlnsDefinitionMap.XmlNamespaceTypeMap> current = enumerator.Current;
					using (IEnumerator<XmlnsDefinitionMap.AssemblyNamespace> enumerator1 = current.Value.AssemblyNamespaces.GetEnumerator())
					{
						while (enumerator1.MoveNext())
						{
							XmlnsDefinitionMap.AssemblyNamespace assemblyNamespace = enumerator1.Current;
							if (!(assemblyNamespace.Name == name) || !(assemblyNamespace.ClrNamespace == clrNamespace))
							{
								continue;
							}
							if (current.Key == XmlNamespace.AvalonXmlNamespace || current.Key == XmlNamespace.JoltXmlNamespace)
							{
								key = current.Key;
								return key;
							}
							else
							{
								xmlNamespace = current.Key;
							}
						}
					}
				}
				return xmlNamespace;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return key;
		}

		public IType GetType(IXmlNamespace xmlNamespace, string typeName)
		{
			XmlnsDefinitionMap.XmlNamespaceTypeMap xmlNamespaceTypeMap;
			IType type;
			if (this.map.TryGetValue(xmlNamespace, out xmlNamespaceTypeMap))
			{
				Type type1 = xmlNamespaceTypeMap.GetType(typeName);
				if (type1 != null)
				{
					IType type2 = this.typeResolver.GetType(type1);
					if (!this.typeResolver.PlatformMetadata.IsSupported(this.typeResolver, type2))
					{
						return null;
					}
					return type2;
				}
				if (type1 == null)
				{
					IEnumerable<ITypeId> proxyTypes = ((PlatformTypes)this.typeResolver.PlatformMetadata).GetProxyTypes(this.typeResolver);
					if (proxyTypes != null)
					{
						using (IEnumerator<ITypeId> enumerator = proxyTypes.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								ITypeId current = enumerator.Current;
								if (!(typeName == current.Name) || current.XmlNamespace == null || !xmlNamespace.Equals(current.XmlNamespace) && (!XmlnsDefinitionMap.IsAvalonOrJolt(xmlNamespace) || !XmlnsDefinitionMap.IsAvalonOrJolt(current.XmlNamespace)))
								{
									continue;
								}
								type = this.typeResolver.ResolveType(current);
								return type;
							}
							return null;
						}
						return type;
					}
				}
			}
			return null;
		}

		private XmlnsDefinitionMap.XmlNamespaceTypeMap GetTypeMapCreatingIfNecessary(XmlNamespace xmlNamespace)
		{
			XmlnsDefinitionMap.XmlNamespaceTypeMap xmlNamespaceTypeMap;
			if (!this.map.TryGetValue(xmlNamespace, out xmlNamespaceTypeMap))
			{
				xmlNamespaceTypeMap = new XmlnsDefinitionMap.XmlNamespaceTypeMap(this.typeResolver, this.targetAssembly);
				this.map.Add(xmlNamespace, xmlNamespaceTypeMap);
			}
			return xmlNamespaceTypeMap;
		}

		private static bool IsAvalonOrJolt(IXmlNamespace xmlNamespace)
		{
			if (XmlNamespace.AvalonXmlNamespace.Equals(xmlNamespace))
			{
				return true;
			}
			return XmlNamespace.JoltXmlNamespace.Equals(xmlNamespace);
		}

		protected void RegisterAssemblies(IEnumerable<IAssembly> assemblies)
		{
			foreach (IAssembly assembly in assemblies)
			{
				try
				{
					Assembly reflectionAssembly = AssemblyHelper.GetReflectionAssembly(assembly);
					Assembly assembly1 = reflectionAssembly;
					if (reflectionAssembly != null)
					{
						IPlatformTypes platformMetadata = (IPlatformTypes)this.typeResolver.PlatformMetadata;
						if (platformMetadata.RuntimeContext != null && platformMetadata.ReferenceContext != null && platformMetadata.RuntimeContext.ResolveRuntimeAssembly(AssemblyHelper.GetAssemblyName(reflectionAssembly)) != null)
						{
							Assembly assembly2 = platformMetadata.ReferenceContext.ResolveReferenceAssembly(reflectionAssembly);
							if (assembly2 != null)
							{
								assembly1 = assembly2;
							}
						}
						this.RegisterAssemblyNamespaces(assembly, assembly1.GetCustomAttributesData());
					}
				}
				catch
				{
				}
			}
		}

		protected abstract void RegisterAssemblyNamespaces(IAssembly assembly, IEnumerable<CustomAttributeData> attributes);

		protected void SetNamespacePrefix(string xamlNamespace, string prefix)
		{
			XmlNamespace @namespace = XmlNamespace.ToNamespace(xamlNamespace, this.canonicalization);
			XmlnsDefinitionMap.XmlNamespaceTypeMap typeMapCreatingIfNecessary = this.GetTypeMapCreatingIfNecessary(@namespace);
			if (typeMapCreatingIfNecessary.DefaultPrefix == null)
			{
				typeMapCreatingIfNecessary.DefaultPrefix = prefix;
			}
		}

		private sealed class AssemblyNamespace
		{
			private IAssembly assembly;

			private string clrNamespace;

			private string assemblyName;

			public IAssembly Assembly
			{
				get
				{
					return this.assembly;
				}
			}

			public string ClrNamespace
			{
				get
				{
					return this.clrNamespace;
				}
			}

			public string Name
			{
				get
				{
					if (this.assemblyName == null)
					{
						this.assemblyName = AssemblyHelper.GetAssemblyName(this.assembly).Name;
					}
					return this.assemblyName;
				}
			}

			public AssemblyNamespace(IAssembly assembly, string clrNamespace)
			{
				this.assembly = assembly;
				this.clrNamespace = clrNamespace;
			}

			public override string ToString()
			{
				return string.Concat(this.Name, ", ", this.clrNamespace);
			}
		}

		private sealed class XmlNamespaceTypeMap
		{
			private ITypeResolver typeResolver;

			private IAssembly targetAssembly;

			private List<XmlnsDefinitionMap.AssemblyNamespace> assemblyNamespaces;

			private Dictionary<string, Type> typeNameCache;

			private string defaultPrefix;

			public IList<XmlnsDefinitionMap.AssemblyNamespace> AssemblyNamespaces
			{
				get
				{
					return this.assemblyNamespaces;
				}
			}

			public string DefaultPrefix
			{
				get
				{
					return this.defaultPrefix;
				}
				set
				{
					this.defaultPrefix = value;
				}
			}

			public XmlNamespaceTypeMap(ITypeResolver typeResolver, IAssembly targetAssembly)
			{
				this.typeResolver = typeResolver;
				this.targetAssembly = targetAssembly;
			}

			public Type GetType(string typeName)
			{
				Type type;
				Type type1 = null;
				if (this.typeNameCache.TryGetValue(typeName, out type1))
				{
					return type1;
				}
				using (IEnumerator<XmlnsDefinitionMap.AssemblyNamespace> enumerator = this.AssemblyNamespaces.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						XmlnsDefinitionMap.AssemblyNamespace current = enumerator.Current;
						IAssembly assembly = current.Assembly;
						string str = TypeHelper.CombineNamespaceAndTypeName(current.ClrNamespace, typeName);
						try
						{
							type1 = PlatformTypeHelper.GetType(assembly, str);
							if (type1 != null)
							{
								IType type2 = this.typeResolver.GetType(type1);
								if (!this.typeResolver.PlatformMetadata.IsNullType(type2))
								{
									if (TypeHelper.IsSet((assembly.CompareTo(this.targetAssembly) ? MemberAccessTypes.PublicOrInternal : MemberAccessTypes.Public), PlatformTypeHelper.GetMemberAccess(type1)))
									{
										this.typeNameCache.Add(typeName, type1);
										type = type1;
										return type;
									}
								}
							}
						}
						catch (ArgumentException argumentException)
						{
							type = null;
							return type;
						}
					}
					this.typeNameCache.Add(typeName, null);
					return null;
				}
				return type;
			}
		}
	}
}