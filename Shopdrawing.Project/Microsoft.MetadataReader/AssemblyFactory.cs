using System;
using System.Reflection;
using System.Reflection.Adds;

namespace Microsoft.MetadataReader
{
	internal static class AssemblyFactory
	{
		public static Assembly CreateAssembly(MetadataOnlyModule manifestModule, string manifestFile)
		{
			return new MetadataOnlyAssembly(manifestModule, manifestFile);
		}

		public static Assembly CreateAssembly(ITypeUniverse typeUniverse, MetadataFile metadataImport, string manifestFile)
		{
			return AssemblyFactory.CreateAssembly(typeUniverse, metadataImport, new DefaultFactory(), manifestFile);
		}

		public static Assembly CreateAssembly(ITypeUniverse typeUniverse, MetadataFile metadataImport, IReflectionFactory factory, string manifestFile)
		{
			return AssemblyFactory.CreateAssembly(typeUniverse, metadataImport, null, factory, manifestFile, null);
		}

		public static Assembly CreateAssembly(ITypeUniverse typeUniverse, MetadataFile manifestModuleImport, MetadataFile[] netModuleImports, string manifestFile, string[] netModuleFiles)
		{
			return AssemblyFactory.CreateAssembly(typeUniverse, manifestModuleImport, netModuleImports, new DefaultFactory(), manifestFile, netModuleFiles);
		}

		public static Assembly CreateAssembly(ITypeUniverse typeUniverse, MetadataFile manifestModuleImport, MetadataFile[] netModuleImports, IReflectionFactory factory, string manifestFile, string[] netModuleFiles)
		{
			int length = 1;
			if (netModuleImports != null)
			{
				length = length + (int)netModuleImports.Length;
			}
			MetadataOnlyModule[] metadataOnlyModule = new MetadataOnlyModule[length];
			metadataOnlyModule[0] = new MetadataOnlyModule(typeUniverse, manifestModuleImport, factory, manifestFile);
			if (length > 1)
			{
				for (int i = 0; i < (int)netModuleImports.Length; i++)
				{
					metadataOnlyModule[i + 1] = new MetadataOnlyModule(typeUniverse, netModuleImports[i], factory, netModuleFiles[i]);
				}
			}
			return new MetadataOnlyAssembly(metadataOnlyModule, manifestFile);
		}
	}
}