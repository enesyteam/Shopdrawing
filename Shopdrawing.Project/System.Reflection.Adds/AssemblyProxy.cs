using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace System.Reflection.Adds
{
	[DebuggerDisplay("AssemblyProxy")]
	internal abstract class AssemblyProxy : Assembly, IAssembly2, IDisposable
	{
		private readonly ITypeUniverse m_universe;

		private Assembly m_assembly;

		public override string CodeBase
		{
			get
			{
				return this.GetResolvedAssembly().CodeBase;
			}
		}

		public override MethodInfo EntryPoint
		{
			get
			{
				return this.GetResolvedAssembly().EntryPoint;
			}
		}

		public override string EscapedCodeBase
		{
			get
			{
				return this.GetResolvedAssembly().EscapedCodeBase;
			}
		}

		public override string FullName
		{
			get
			{
				return this.GetResolvedAssembly().FullName;
			}
		}

		public override bool GlobalAssemblyCache
		{
			get
			{
				return this.GetResolvedAssembly().GlobalAssemblyCache;
			}
		}

		public override long HostContext
		{
			get
			{
				return this.GetResolvedAssembly().HostContext;
			}
		}

		public override string Location
		{
			get
			{
				return this.GetResolvedAssembly().Location;
			}
		}

		public override Module ManifestModule
		{
			get
			{
				return this.GetResolvedAssembly().ManifestModule;
			}
		}

		public ITypeUniverse TypeUniverse
		{
			get
			{
				return this.m_universe;
			}
		}

		protected AssemblyProxy(ITypeUniverse universe)
		{
			this.m_universe = universe;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing && this.m_assembly != null)
			{
				IDisposable mAssembly = this.m_assembly as IDisposable;
				if (mAssembly != null)
				{
					mAssembly.Dispose();
				}
			}
		}

		public override bool Equals(object obj)
		{
			return this.GetResolvedAssembly().Equals(obj);
		}

		public override IList<CustomAttributeData> GetCustomAttributesData()
		{
			return this.GetResolvedAssembly().GetCustomAttributesData();
		}

		public override Type[] GetExportedTypes()
		{
			return this.GetResolvedAssembly().GetExportedTypes();
		}

		public override FileStream GetFile(string name)
		{
			return this.GetResolvedAssembly().GetFile(name);
		}

		public override FileStream[] GetFiles(bool getResourceModules)
		{
			return this.GetResolvedAssembly().GetFiles(getResourceModules);
		}

		public override FileStream[] GetFiles()
		{
			return this.GetResolvedAssembly().GetFiles();
		}

		public override int GetHashCode()
		{
			return this.GetResolvedAssembly().GetHashCode();
		}

		public override Module[] GetLoadedModules(bool getResourceModules)
		{
			return this.GetResolvedAssembly().GetLoadedModules(getResourceModules);
		}

		public override ManifestResourceInfo GetManifestResourceInfo(string resourceName)
		{
			return this.GetResolvedAssembly().GetManifestResourceInfo(resourceName);
		}

		public override string[] GetManifestResourceNames()
		{
			return this.GetResolvedAssembly().GetManifestResourceNames();
		}

		public override Stream GetManifestResourceStream(Type type, string name)
		{
			return this.GetResolvedAssembly().GetManifestResourceStream(type, name);
		}

		public override Stream GetManifestResourceStream(string name)
		{
			return this.GetResolvedAssembly().GetManifestResourceStream(name);
		}

		public override Module GetModule(string name)
		{
			return this.GetResolvedAssembly().GetModule(name);
		}

		public override Module[] GetModules(bool getResourceModules)
		{
			return this.GetResolvedAssembly().GetModules(getResourceModules);
		}

		public override AssemblyName GetName()
		{
			return this.GetResolvedAssembly().GetName();
		}

		public override AssemblyName GetName(bool copiedName)
		{
			return this.GetResolvedAssembly().GetName(copiedName);
		}

		public override AssemblyName[] GetReferencedAssemblies()
		{
			return this.GetResolvedAssembly().GetReferencedAssemblies();
		}

		public Assembly GetResolvedAssembly()
		{
			if (this.m_assembly == null)
			{
				this.m_assembly = this.GetResolvedAssemblyWorker();
			}
			return this.m_assembly;
		}

		protected abstract Assembly GetResolvedAssemblyWorker();

		public override Assembly GetSatelliteAssembly(CultureInfo culture)
		{
			return this.GetResolvedAssembly().GetSatelliteAssembly(culture);
		}

		public override Assembly GetSatelliteAssembly(CultureInfo culture, Version version)
		{
			return this.GetResolvedAssembly().GetSatelliteAssembly(culture, version);
		}

		public override Type GetType(string name, bool throwOnError, bool ignoreCase)
		{
			return this.GetResolvedAssembly().GetType(name, throwOnError, ignoreCase);
		}

		public override Type[] GetTypes()
		{
			return this.GetResolvedAssembly().GetTypes();
		}

		public override string ToString()
		{
			return this.GetResolvedAssembly().ToString();
		}
	}
}