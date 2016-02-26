using System;
using System.IO;
using System.Reflection;
using System.Reflection.Adds;

namespace Microsoft.MetadataReader
{
	internal class Loader
	{
		private readonly IMutableTypeUniverse m_universe;

		private readonly MetadataDispenser m_dispenser = new MetadataDispenser();

		private IReflectionFactory m_factory;

		public IReflectionFactory Factory
		{
			get
			{
				if (this.m_factory == null)
				{
					this.m_factory = new DefaultFactory();
				}
				return this.m_factory;
			}
			set
			{
				this.m_factory = value;
			}
		}

		public Loader(IMutableTypeUniverse universe)
		{
			this.m_universe = universe;
		}

		public Assembly LoadAssemblyFromByteArray(byte[] data)
		{
			MetadataFile metadataFile = this.m_dispenser.OpenFromByteArray(data);
			Assembly assembly = AssemblyFactory.CreateAssembly(this.m_universe, metadataFile, this.Factory, string.Empty);
			this.m_universe.AddAssembly(assembly);
			return assembly;
		}

		public Assembly LoadAssemblyFromFile(string file)
		{
			MetadataFile metadataFile = this.OpenMetadataFile(file);
			Assembly assembly = AssemblyFactory.CreateAssembly(this.m_universe, metadataFile, this.Factory, metadataFile.FilePath);
			this.m_universe.AddAssembly(assembly);
			return assembly;
		}

		public Assembly LoadAssemblyFromFile(string manifestFile, string[] netModuleFiles)
		{
			MetadataFile metadataFile = this.OpenMetadataFile(manifestFile);
			MetadataFile[] metadataFileArray = null;
			if (netModuleFiles != null && (int)netModuleFiles.Length > 0)
			{
				metadataFileArray = new MetadataFile[(int)netModuleFiles.Length];
				for (int i = 0; i < (int)netModuleFiles.Length; i++)
				{
					metadataFileArray[i] = this.OpenMetadataFile(netModuleFiles[i]);
				}
			}
			Assembly assembly = AssemblyFactory.CreateAssembly(this.m_universe, metadataFile, metadataFileArray, this.Factory, metadataFile.FilePath, netModuleFiles);
			this.m_universe.AddAssembly(assembly);
			return assembly;
		}

		public MetadataOnlyModule LoadModuleFromFile(string moduleFileName)
		{
			MetadataFile metadataFile = this.m_dispenser.OpenFileAsFileMapping(moduleFileName);
			return new MetadataOnlyModule(this.m_universe, metadataFile, this.Factory, moduleFileName);
		}

		private MetadataFile OpenMetadataFile(string filename)
		{
			return this.m_dispenser.OpenFileAsFileMapping(filename);
		}

		public Module ResolveModule(Assembly containingAssembly, string moduleName)
		{
			if (containingAssembly == null || string.IsNullOrEmpty(containingAssembly.Location))
			{
				throw new ArgumentException("manifestModule needs to be associated with an assembly with valid location");
			}
			string directoryName = Path.GetDirectoryName(containingAssembly.Location);
			string str = Path.Combine(directoryName, moduleName);
			MetadataFile metadataFile = this.m_dispenser.OpenFileAsFileMapping(str);
			MetadataOnlyModule metadataOnlyModule = new MetadataOnlyModule(this.m_universe, metadataFile, this.Factory, str);
			metadataOnlyModule.SetContainingAssembly(containingAssembly);
			return metadataOnlyModule;
		}
	}
}