using System;
using System.Reflection;
using System.Reflection.Adds;

namespace Microsoft.MetadataReader
{
	internal class DefaultUniverse : SimpleUniverse
	{
		private Microsoft.MetadataReader.Loader m_loader;

		public Microsoft.MetadataReader.Loader Loader
		{
			get
			{
				return this.m_loader;
			}
		}

		public DefaultUniverse()
		{
			this.m_loader = new Microsoft.MetadataReader.Loader(this);
		}

		internal Assembly LoadAssemblyFromByteArray(byte[] data)
		{
			return this.Loader.LoadAssemblyFromByteArray(data);
		}

		internal Assembly LoadAssemblyFromFile(string manifestFileName, string[] netModuleFileNames)
		{
			return this.Loader.LoadAssemblyFromFile(manifestFileName, netModuleFileNames);
		}

		internal Assembly LoadAssemblyFromFile(string manifestFileName)
		{
			return this.Loader.LoadAssemblyFromFile(manifestFileName);
		}

		internal MetadataOnlyModule LoadModuleFromFile(string netModulePath)
		{
			return this.Loader.LoadModuleFromFile(netModulePath);
		}

		public override Module ResolveModule(Assembly containingAssembly, string moduleName)
		{
			return this.Loader.ResolveModule(containingAssembly, moduleName);
		}
	}
}