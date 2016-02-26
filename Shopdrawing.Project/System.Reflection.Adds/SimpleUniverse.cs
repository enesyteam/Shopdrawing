using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace System.Reflection.Adds
{
	internal class SimpleUniverse : IMutableTypeUniverse, ITypeUniverse, IDisposable
	{
		private Dictionary<string, Type> m_hash = new Dictionary<string, Type>();

		private List<Assembly> m_loadedAssemblies = new List<Assembly>();

		private object m_loadedAssembliesLock = new object();

		private Assembly m_systemAssembly;

		public IEnumerable<Assembly> Assemblies
		{
			get
			{
				IEnumerable<Assembly> list;
				lock (this.m_loadedAssembliesLock)
				{
					list = this.m_loadedAssemblies.ToList<Assembly>();
				}
				return list;
			}
		}

		public SimpleUniverse()
		{
		}

		public void AddAssembly(Assembly assembly)
		{
			IAssembly2 assembly2 = (IAssembly2)assembly;
			lock (this.m_loadedAssembliesLock)
			{
				if (!this.m_loadedAssemblies.Contains(assembly))
				{
					this.m_loadedAssemblies.Add(assembly);
				}
			}
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
				lock (this.m_loadedAssembliesLock)
				{
					if (this.m_loadedAssemblies != null)
					{
						foreach (Assembly mLoadedAssembly in this.m_loadedAssemblies)
						{
							IDisposable disposable = mLoadedAssembly as IDisposable;
							if (disposable == null)
							{
								continue;
							}
							disposable.Dispose();
						}
						this.m_loadedAssemblies = null;
					}
				}
			}
		}

		protected Assembly FindSystemAssembly()
		{
			Assembly assembly;
			lock (this.m_loadedAssembliesLock)
			{
				foreach (Assembly mLoadedAssembly in this.m_loadedAssemblies)
				{
					if ((int)mLoadedAssembly.GetReferencedAssemblies().Length != 0)
					{
						continue;
					}
					assembly = mLoadedAssembly;
					return assembly;
				}
				return null;
			}
			return assembly;
		}

		public virtual Type GetBuiltInType(System.Reflection.Adds.CorElementType elementType)
		{
			return this.GetTypeXFromName(ElementTypeUtility.GetNameForPrimitive(elementType));
		}

		public virtual Assembly GetSystemAssembly()
		{
			if (this.m_systemAssembly == null)
			{
				Assembly assembly = this.FindSystemAssembly();
				if (assembly != null)
				{
					this.SetSystemAssembly(assembly);
				}
			}
			if (this.m_systemAssembly == null)
			{
				throw new UnresolvedAssemblyException(string.Format(CultureInfo.InvariantCulture, MetadataStringTable.CannotDetermineSystemAssembly, new object[0]));
			}
			return this.m_systemAssembly;
		}

		public virtual Type GetTypeXFromName(string fullName)
		{
			Type type;
			if (!this.m_hash.TryGetValue(fullName, out type))
			{
				type = this.GetSystemAssembly().GetType(fullName, true, false);
				this.m_hash[fullName] = type;
			}
			return type;
		}

		public virtual Assembly ResolveAssembly(AssemblyName name)
		{
			Assembly target = this.TryResolveAssembly(name);
			if (target != null)
			{
				return target;
			}
			if (this.OnResolveEvent != null)
			{
				ResolveAssemblyNameEventArgs resolveAssemblyNameEventArg = new ResolveAssemblyNameEventArgs(name);
				this.OnResolveEvent(this, resolveAssemblyNameEventArg);
				target = resolveAssemblyNameEventArg.Target;
			}
			if (target == null)
			{
				CultureInfo invariantCulture = CultureInfo.InvariantCulture;
				string universeCannotResolveAssembly = MetadataStringTable.UniverseCannotResolveAssembly;
				object[] objArray = new object[] { name };
				throw new UnresolvedAssemblyException(string.Format(invariantCulture, universeCannotResolveAssembly, objArray));
			}
			IAssembly2 assembly2 = target as IAssembly2;
			if (assembly2 == null)
			{
				throw new InvalidOperationException(MetadataStringTable.ResolverMustResolveToValidAssembly);
			}
			if (assembly2.TypeUniverse != this)
			{
				throw new InvalidOperationException(MetadataStringTable.ResolvedAssemblyMustBeWithinSameUniverse);
			}
			return target;
		}

		public virtual Assembly ResolveAssembly(Module scope, Token tokenAssemblyRef)
		{
			return this.ResolveAssembly(((IModule2)scope).GetAssemblyNameFromAssemblyRef(tokenAssemblyRef));
		}

		public virtual Module ResolveModule(Assembly containingAssembly, string moduleName)
		{
			throw new NotImplementedException();
		}

		public void SetSystemAssembly(Assembly systemAssembly)
		{
			if (systemAssembly == null)
			{
				throw new ArgumentNullException("systemAssembly");
			}
			this.m_systemAssembly = systemAssembly;
		}

		protected Assembly TryResolveAssembly(AssemblyName name)
		{
			Assembly assembly;
			lock (this.m_loadedAssembliesLock)
			{
				foreach (Assembly mLoadedAssembly in this.m_loadedAssemblies)
				{
					if (!AssemblyName.ReferenceMatchesDefinition(name, mLoadedAssembly.GetName()))
					{
						continue;
					}
					assembly = mLoadedAssembly;
					return assembly;
				}
				return null;
			}
			return assembly;
		}

		public event EventHandler<ResolveAssemblyNameEventArgs> OnResolveEvent;
	}
}