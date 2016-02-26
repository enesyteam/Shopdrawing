using Microsoft.Expression.DesignModel.Markup;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace Microsoft.Expression.DesignModel.Metadata
{
	public abstract class TypeResolver : ITypeResolver, IMetadataResolver
	{
		private ITypeMetadataFactory typeMetadataFactory;

		private Dictionary<string, ProjectContextType> typeIds = new Dictionary<string, ProjectContextType>();

		private int versionStamp;

		private Dictionary<string, UnbuiltTypeDescription> unbuiltTypeInfo = new Dictionary<string, UnbuiltTypeDescription>();

		private IPlatformTypes platformTypes;

		private HashSet<IAssembly> platformAssemblies;

		private HashSet<IAssembly> libraryAssemblies;

		private HashSet<IAssembly> invalidatingAssemblies;

		public abstract ICollection<IAssembly> AssemblyReferences
		{
			get;
		}

		protected virtual ICollection<IAssembly> LibraryAssemblies
		{
			get
			{
				if (this.libraryAssemblies == null)
				{
					this.libraryAssemblies = new HashSet<IAssembly>();
				}
				return this.libraryAssemblies;
			}
		}

		public ITypeMetadataFactory MetadataFactory
		{
			get
			{
				return this.typeMetadataFactory;
			}
		}

		protected virtual ICollection<IAssembly> PlatformAssemblies
		{
			get
			{
				if (this.platformAssemblies == null)
				{
					this.platformAssemblies = new HashSet<IAssembly>(this.platformTypes.DefaultAssemblyReferences);
				}
				return this.platformAssemblies;
			}
		}

		public IPlatformMetadata PlatformMetadata
		{
			get
			{
				return this.platformTypes;
			}
		}

		public abstract IAssembly ProjectAssembly
		{
			get;
		}

		public abstract IXmlNamespaceTypeResolver ProjectNamespaces
		{
			get;
		}

		public abstract string ProjectPath
		{
			get;
		}

		public virtual string RootNamespace
		{
			get
			{
				return null;
			}
		}

		protected IEnumerable<ITypeId> Types
		{
			get
			{
				foreach (ITypeId value in this.typeIds.Values)
				{
					yield return value;
				}
			}
		}

		protected Dictionary<string, UnbuiltTypeDescription> UnbuiltTypeInfo
		{
			get
			{
				return this.unbuiltTypeInfo;
			}
		}

		public int VersionStamp
		{
			get
			{
				return this.versionStamp;
			}
		}

		protected TypeResolver()
		{
		}

		private static void BuildCurrentReachableAssemblies(Assembly assembly, HashSet<string> reachableAssemblyNames, IList<Assembly> appDomainAssemblies)
		{
			if (assembly == null || reachableAssemblyNames.Contains(assembly.FullName))
			{
				return;
			}
			reachableAssemblyNames.Add(assembly.FullName);
			AssemblyName[] referencedAssemblies = assembly.GetReferencedAssemblies();
			for (int i = 0; i < (int)referencedAssemblies.Length; i++)
			{
				string fullName = referencedAssemblies[i].FullName;
				Assembly assembly1 = null;
				foreach (Assembly appDomainAssembly in appDomainAssemblies)
				{
					if (!string.Equals(appDomainAssembly.FullName, fullName, StringComparison.OrdinalIgnoreCase))
					{
						continue;
					}
					assembly1 = appDomainAssembly;
					break;
				}
				if (assembly1 != null)
				{
					TypeResolver.BuildCurrentReachableAssemblies(assembly1, reachableAssemblyNames, appDomainAssemblies);
				}
			}
		}

		public virtual bool EnsureAssemblyReferenced(string assemblyPath)
		{
			return false;
		}

		public IAssembly GetAssembly(string assemblyName)
		{
			IAssembly assembly = this.AssemblyReferences.FirstOrDefault<IAssembly>((IAssembly assemblyReference) => string.Equals(assemblyReference.Name, assemblyName, StringComparison.OrdinalIgnoreCase)) ?? RuntimeGeneratedTypesHelper.BlendAssemblies.FirstOrDefault<IAssembly>((IAssembly designTypeAssembly) => designTypeAssembly.Name.Equals(assemblyName, StringComparison.Ordinal));
			return assembly;
		}

		protected IAssembly GetAssembly(Assembly assembly)
		{
			return this.GetAssembly(AssemblyHelper.GetAssemblyName(assembly).Name);
		}

		public virtual object GetCapabilityValue(PlatformCapability capability)
		{
			return this.PlatformMetadata.GetCapabilityValue(capability);
		}

		private string GetKey(IAssemblyId assembly, Type type)
		{
			string nameIncludingAnyDeclaringTypes = ProjectContextType.GetNameIncludingAnyDeclaringTypes(type);
			string key = this.GetKey(assembly, Microsoft.Expression.DesignModel.Metadata.TypeHelper.CombineNamespaceAndTypeName(type.Namespace, nameIncludingAnyDeclaringTypes));
			if (!type.IsGenericType)
			{
				return key;
			}
			StringBuilder stringBuilder = new StringBuilder(key);
			stringBuilder.Append('<');
			Type[] genericTypeArguments = PlatformTypeHelper.GetGenericTypeArguments(type);
			if (genericTypeArguments == null)
			{
				return null;
			}
			for (int i = 0; i < (int)genericTypeArguments.Length; i++)
			{
				Type type1 = genericTypeArguments[i];
				if (i > 0)
				{
					stringBuilder.Append(',');
				}
				IAssembly assembly1 = this.GetAssembly(type1.Assembly);
				if (assembly1 == null)
				{
					return null;
				}
				string str = this.GetKey(assembly1, type1);
				if (str == null)
				{
					return null;
				}
				stringBuilder.Append(str);
			}
			stringBuilder.Append('>');
			return stringBuilder.ToString();
		}

		protected string GetKey(IAssemblyId assembly, string typeName)
		{
			if (assembly == null || assembly == this.ProjectAssembly || this.ProjectAssembly != null && assembly.Name.Equals(this.ProjectAssembly.Name, StringComparison.OrdinalIgnoreCase))
			{
				return typeName;
			}
			return string.Concat(assembly.Name, ":", typeName);
		}

		public virtual IType GetType(IXmlNamespace xmlNamespace, string typeName)
		{
			string str;
			string str1;
			if (xmlNamespace == null)
			{
				throw new ArgumentNullException("xmlNamespace");
			}
			if (string.IsNullOrEmpty(typeName))
			{
				throw new ArgumentException(ExceptionStringTable.GetTypeCannotBeCalledWithNullOrEmptyTypeName, "typeName");
			}
			IType type = this.ProjectNamespaces.GetType(xmlNamespace, typeName);
			if (type != null)
			{
				return type;
			}
			if (!XamlParser.TryParseClrNamespaceUri(xmlNamespace.Value, out str, out str1))
			{
				return null;
			}
			return this.GetType(str1, Microsoft.Expression.DesignModel.Metadata.TypeHelper.CombineNamespaceAndTypeName(str, typeName));
		}

		public IType GetType(string assemblyName, string typeName)
		{
			IAssembly assembly;
			ProjectContextType projectContextType;
			UnbuiltTypeDescription unbuiltTypeDescription;
			IType type = null;
			if (string.IsNullOrEmpty(typeName))
			{
				throw new ArgumentException(ExceptionStringTable.GetTypeCannotBeCalledWithNullOrEmptyTypeName, "typeName");
			}
			assembly = (!string.IsNullOrEmpty(assemblyName) ? this.GetAssembly(assemblyName) : this.ProjectAssembly);
			if (assembly != null)
			{
				string key = this.GetKey(assembly, typeName);
				if (this.typeIds.TryGetValue(key, out projectContextType))
				{
					return projectContextType;
				}
				Type type1 = null;
				try
				{
					type1 = PlatformTypeHelper.GetType(assembly, typeName);
				}
				catch (Exception exception)
				{
				}
				if (type1 != null)
				{
					type = this.GetType(key, assembly, type1);
				}
				if (type == null && this.unbuiltTypeInfo.TryGetValue(key, out unbuiltTypeDescription))
				{
					projectContextType = new ProjectContextType(this, key);
					this.typeIds.Add(key, projectContextType);
					projectContextType.Initialize(unbuiltTypeDescription, key);
					type = projectContextType;
				}
			}
			return type;
		}

		public IType GetType(IAssembly assembly, string typeName)
		{
			return PlatformTypeHelper.GetType(this, assembly, typeName);
		}

		public virtual IType GetType(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			return this.GetType(null, null, type);
		}

		private IType GetType(string key, IAssembly assembly, Type type)
		{
			ProjectContextType projectContextType;
			IType type1 = this.platformTypes.GetType(type);
			if (type1 != null)
			{
				return type1;
			}
			if (assembly == null)
			{
				assembly = this.GetAssembly(type.Assembly);
				if (assembly == null)
				{
					IAssembly assembly1 = this.platformTypes.CreateAssembly(type.Assembly, AssemblySource.Unknown);
					foreach (IAssembly assemblyReference in this.AssemblyReferences)
					{
						if (!assembly1.Name.StartsWith(assemblyReference.Name, StringComparison.OrdinalIgnoreCase))
						{
							continue;
						}
						string str = assembly1.Name.Substring(assemblyReference.Name.Length);
						if (string.Compare(str, ".design", StringComparison.OrdinalIgnoreCase) != 0 && string.Compare(str, ".expression.design", StringComparison.OrdinalIgnoreCase) != 0)
						{
							continue;
						}
						assembly = assembly1;
					}
					if (assembly == null)
					{
						return null;
					}
				}
			}
			if (key == null)
			{
				key = this.GetKey(assembly, type);
				if (key == null)
				{
					return null;
				}
			}
			if (!this.typeIds.TryGetValue(key, out projectContextType))
			{
				projectContextType = new ProjectContextType(this, key);
				this.typeIds.Add(key, projectContextType);
				IXmlNamespace @namespace = this.ProjectNamespaces.GetNamespace(assembly, type);
				projectContextType.Initialize(@namespace, type, key);
			}
			return projectContextType;
		}

		public string GetXamlSourcePath(IType type)
		{
			UnbuiltTypeDescription unbuiltTypeDescription;
			string xamlSourcePath = null;
			string key = this.GetKey(type.RuntimeAssembly, type.FullName);
			if (this.UnbuiltTypeInfo.TryGetValue(key, out unbuiltTypeDescription))
			{
				xamlSourcePath = unbuiltTypeDescription.XamlSourcePath;
			}
			return xamlSourcePath;
		}

		private bool HasReferencedAssembly(AssemblyGroup assemblyGroup)
		{
			bool flag;
			IPlatformTypes platformMetadata = (IPlatformTypes)this.PlatformMetadata;
			using (IEnumerator<IAssemblyId> enumerator = platformMetadata.GetAssemblyGroup(assemblyGroup).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					IAssemblyId current = enumerator.Current;
					using (IEnumerator<IAssembly> enumerator1 = this.AssemblyReferences.GetEnumerator())
					{
						while (enumerator1.MoveNext())
						{
							IAssemblyId assemblyId = enumerator1.Current;
							if (current.Name != assemblyId.Name)
							{
								continue;
							}
							flag = true;
							return flag;
						}
					}
				}
				return false;
			}
			return flag;
		}

		protected void Initialize(IPlatformTypes platformTypes)
		{
			this.platformTypes = platformTypes;
			this.typeMetadataFactory = this.platformTypes.CreateTypeMetadataFactory(this);
			RuntimeGeneratedTypesHelper.ClearControlEditingDesignTypeAssembly();
		}

		public bool InTargetAssembly(IType typeId)
		{
			IAssembly projectAssembly = this.ProjectAssembly;
			if (projectAssembly == null)
			{
				return false;
			}
			return projectAssembly.Equals(typeId.RuntimeAssembly);
		}

		public virtual bool IsCapabilitySet(PlatformCapability capability)
		{
			return this.PlatformMetadata.IsCapabilitySet(capability);
		}

		protected virtual void OnAssemblyCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			TypeResolver typeResolver = this;
			typeResolver.versionStamp = typeResolver.versionStamp + 1;
			bool flag = false;
			ICollection<IAssembly> platformAssemblies = this.PlatformAssemblies;
			ICollection<IAssembly> libraryAssemblies = this.LibraryAssemblies;
			List<IAssembly> assemblies = new List<IAssembly>();
			List<IAssembly> assemblies1 = new List<IAssembly>();
			if (this.invalidatingAssemblies != null)
			{
				assemblies.AddRange(
					from assembly in this.invalidatingAssemblies
					where !this.AssemblyReferences.Contains(assembly)
					select assembly);
				assemblies1.AddRange(
					from assembly in this.AssemblyReferences
					where !this.invalidatingAssemblies.Contains(assembly)
					select assembly);
				this.invalidatingAssemblies = null;
			}
			HashSet<IAssembly> assemblies2 = new HashSet<IAssembly>((
				from libraryAssembly in libraryAssemblies
				where !TypeResolver.ShouldRefreshLibraryAssembly(assemblies, assemblies1, libraryAssembly)
				select libraryAssembly).Concat<IAssembly>(platformAssemblies));
			HashSet<ProjectContextType> projectContextTypes = new HashSet<ProjectContextType>(
				from type in this.typeIds.Values
				where TypeResolver.ShouldRefreshType(type, assemblies2)
				select type);
			if (e.Action == NotifyCollectionChangedAction.Reset)
			{
				RuntimeGeneratedTypesHelper.ClearControlEditingDesignTypeAssembly();
			}
			foreach (ProjectContextType projectContextType in projectContextTypes)
			{
				if (projectContextType.Refresh())
				{
					continue;
				}
				flag = true;
			}
			if (flag)
			{
				IList<string> list = (
					from keyValuePair in this.typeIds
					where projectContextTypes.Contains(keyValuePair.Value)
					select keyValuePair.Key).ToList<string>();
				for (int i = 0; i < list.Count; i++)
				{
					this.typeIds.Remove(list[i]);
				}
				this.typeMetadataFactory.Reset();
			}
			List<IAssembly> assemblies3 = new List<IAssembly>();
			if (flag)
			{
				assemblies3.AddRange(this.AssemblyReferences.Where<IAssembly>((IAssembly assembly) => {
					if (assemblies1.Contains(assembly) || assemblies.Contains(assembly) || platformAssemblies.Contains(assembly))
					{
						return false;
					}
					return !libraryAssemblies.Contains(assembly);
				}));
			}
			List<TypeChangedInfo> typeChangedInfos = new List<TypeChangedInfo>();
			typeChangedInfos.AddRange(
				from assembly in assemblies
				select new TypeChangedInfo(assembly, ModificationType.Removed));
			typeChangedInfos.AddRange(
				from assembly in assemblies1
				select new TypeChangedInfo(assembly, ModificationType.Added));
			typeChangedInfos.AddRange(
				from assembly in assemblies3
				select new TypeChangedInfo(assembly, ModificationType.Modified));
			this.OnTypesChanged(new TypesChangedEventArgs(new ReadOnlyCollection<TypeChangedInfo>(typeChangedInfos)));
		}

		protected virtual void OnAssemblyCollectionChanging(NotifyCollectionChangedEventArgs e)
		{
			this.invalidatingAssemblies = new HashSet<IAssembly>(this.AssemblyReferences);
		}

		public void OnTypesChanged(TypesChangedEventArgs e)
		{
			if (this.TypesChangedEarly != null)
			{
				this.TypesChangedEarly(this, e);
			}
			if (this.TypesChanged != null)
			{
				this.TypesChanged(this, e);
			}
		}

		public virtual IProperty ResolveProperty(IPropertyId propertyId)
		{
			IProperty property = this.PlatformMetadata.ResolveProperty(propertyId);
			if (property != null)
			{
				return property;
			}
			PlatformNeutralPropertyId platformNeutralPropertyId = propertyId as PlatformNeutralPropertyId;
			if (platformNeutralPropertyId == null)
			{
				return null;
			}
			IType type = this.ResolveType(platformNeutralPropertyId.DeclaringTypeId);
			if (this.PlatformMetadata.IsNullType(type))
			{
				return null;
			}
			return type.GetMember(platformNeutralPropertyId.MemberType, platformNeutralPropertyId.Name, platformNeutralPropertyId.MemberAccessTypes) as IProperty;
		}

		public virtual IType ResolveType(ITypeId typeId)
		{
			IType type;
			IPlatformTypes platformMetadata = (IPlatformTypes)this.PlatformMetadata;
			ProjectNeutralTypeId projectNeutralTypeId = typeId as ProjectNeutralTypeId;
			if (projectNeutralTypeId != null && projectNeutralTypeId.AssemblyGroup != AssemblyGroup.Interactivity && !this.HasReferencedAssembly(projectNeutralTypeId.AssemblyGroup))
			{
				return PlatformTypes.NullTypeInstance;
			}
			IType type1 = this.PlatformMetadata.ResolveType(typeId);
			if (!this.PlatformMetadata.IsNullType(type1))
			{
				return type1;
			}
			if (projectNeutralTypeId != null)
			{
				using (IEnumerator<IAssemblyId> enumerator = platformMetadata.GetAssemblyGroup(projectNeutralTypeId.AssemblyGroup).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						IAssemblyId current = enumerator.Current;
						type1 = this.GetType(current.Name, projectNeutralTypeId.FullName);
						if (type1 == null)
						{
							using (IEnumerator<string> enumerator1 = projectNeutralTypeId.PotentialFullNames.GetEnumerator())
							{
								while (enumerator1.MoveNext())
								{
									string str = enumerator1.Current;
									type1 = this.GetType(current.Name, str);
									if (type1 == null)
									{
										continue;
									}
									type = type1;
									return type;
								}
							}
						}
						else
						{
							type = type1;
							return type;
						}
					}
					return PlatformTypes.NullTypeInstance;
				}
				return type;
			}
			return PlatformTypes.NullTypeInstance;
		}

		private static bool ShouldRefreshLibraryAssembly(List<IAssembly> removedAssemblies, List<IAssembly> addedAssemblies, IAssembly libraryAssembly)
		{
			if (removedAssemblies.Contains(libraryAssembly) || addedAssemblies.Contains(libraryAssembly))
			{
				return true;
			}
			IReflectionAssembly reflectionAssembly = libraryAssembly as IReflectionAssembly;
			if (reflectionAssembly == null)
			{
				return false;
			}
			HashSet<string> strs = new HashSet<string>();
			TypeResolver.BuildCurrentReachableAssemblies(reflectionAssembly.ReflectionAssembly, strs, AppDomain.CurrentDomain.GetAssemblies());
			bool flag = removedAssemblies.Any<IAssembly>((IAssembly removedAssembly) => strs.Contains(removedAssembly.FullName));
			bool flag1 = addedAssemblies.Any<IAssembly>((IAssembly addedAssembly) => strs.Contains(addedAssembly.FullName));
			if (!flag)
			{
				return flag1;
			}
			return true;
		}

		private static bool ShouldRefreshType(IType type, ICollection<IAssembly> skipRefreshAssemblies)
		{
			bool flag;
			if (!skipRefreshAssemblies.Contains(type.RuntimeAssembly))
			{
				return true;
			}
			if (type.IsGenericType)
			{
				using (IEnumerator<IType> enumerator = type.GetGenericTypeArguments().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (!TypeResolver.ShouldRefreshType(enumerator.Current, skipRefreshAssemblies))
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
			return false;
		}

		public event EventHandler<TypesChangedEventArgs> TypesChanged;

		public event EventHandler<TypesChangedEventArgs> TypesChangedEarly;
	}
}