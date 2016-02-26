using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Microsoft.Expression.DesignModel.Metadata
{
	public static class RuntimeGeneratedTypesHelper
	{
		private static AssemblyBuilder runtimeGeneratedTypesBuilder;

		private static ModuleBuilder controlEditingDesignTypeAssembly;

		private static List<IAssembly> blendAssemblies;

		private static List<IAssembly> controlEditingAssemblies;

		private static Dictionary<IPlatformTypes, HashSet<IAssembly>> assembliesInitialized;

		public static IEnumerable<IAssembly> BlendAssemblies
		{
			get
			{
				return RuntimeGeneratedTypesHelper.blendAssemblies;
			}
		}

		public static IAssembly BlendControlEditingAssembly
		{
			get;
			private set;
		}

		public static IAssembly BlendDefaultAssembly
		{
			get;
			private set;
		}

		public static ModuleBuilder ControlEditingDesignTypeAssembly
		{
			get
			{
				return RuntimeGeneratedTypesHelper.controlEditingDesignTypeAssembly;
			}
		}

		public static ModuleBuilder RuntimeGeneratedTypesAssembly
		{
			get;
			private set;
		}

		static RuntimeGeneratedTypesHelper()
		{
			RuntimeGeneratedTypesHelper.blendAssemblies = new List<IAssembly>();
			RuntimeGeneratedTypesHelper.controlEditingAssemblies = new List<IAssembly>();
			RuntimeGeneratedTypesHelper.assembliesInitialized = new Dictionary<IPlatformTypes, HashSet<IAssembly>>();
			RuntimeGeneratedTypesHelper.RuntimeGeneratedTypesAssembly = RuntimeGeneratedTypesHelper.CreateNewAssembly();
			RuntimeGeneratedTypesHelper.BlendDefaultAssembly = new Microsoft.Expression.DesignModel.Metadata.RuntimeAssembly(RuntimeGeneratedTypesHelper.RuntimeGeneratedTypesAssembly.Assembly, AssemblySource.Unknown, false);
			RuntimeGeneratedTypesHelper.blendAssemblies.Add(RuntimeGeneratedTypesHelper.BlendDefaultAssembly);
		}

		public static void ClearControlEditingDesignTypeAssembly()
		{
			RuntimeGeneratedTypesHelper.BlendControlEditingAssembly = null;
			RuntimeGeneratedTypesHelper.controlEditingDesignTypeAssembly = null;
		}

		private static ModuleBuilder CreateNewAssembly()
		{
			string str = Guid.NewGuid().ToString();
			AssemblyName assemblyName = new AssemblyName()
			{
				Name = string.Concat("Blend_RuntimeGeneratedTypesAssembly", str)
			};
			RuntimeGeneratedTypesHelper.runtimeGeneratedTypesBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
			return RuntimeGeneratedTypesHelper.runtimeGeneratedTypesBuilder.DefineDynamicModule(assemblyName.Name, string.Concat("RuntimeGeneratedTypes", str, ".dll"), true);
		}

		public static void EnsureControlEditingDesignTypeAssembly(IPlatformTypes platformMetadata)
		{
			if (RuntimeGeneratedTypesHelper.controlEditingDesignTypeAssembly == null)
			{
				RuntimeGeneratedTypesHelper.controlEditingDesignTypeAssembly = RuntimeGeneratedTypesHelper.CreateNewAssembly();
				RuntimeGeneratedTypesHelper.BlendControlEditingAssembly = new Microsoft.Expression.DesignModel.Metadata.RuntimeAssembly(RuntimeGeneratedTypesHelper.controlEditingDesignTypeAssembly.Assembly, AssemblySource.Unknown, false);
				RuntimeGeneratedTypesHelper.blendAssemblies.Add(RuntimeGeneratedTypesHelper.BlendControlEditingAssembly);
				RuntimeGeneratedTypesHelper.controlEditingAssemblies.Add(RuntimeGeneratedTypesHelper.BlendControlEditingAssembly);
				RuntimeGeneratedTypesHelper.RegisterRuntimeAssemblies(platformMetadata);
			}
		}

		public static bool IsControlEditingAssembly(IAssembly assembly)
		{
			return RuntimeGeneratedTypesHelper.controlEditingAssemblies.Contains(assembly);
		}

		public static void RegisterRuntimeAssemblies(IPlatformTypes platformMetadata)
		{
			HashSet<IAssembly> assemblies;
			if (!RuntimeGeneratedTypesHelper.assembliesInitialized.TryGetValue(platformMetadata, out assemblies))
			{
				assemblies = new HashSet<IAssembly>();
			}
			foreach (IAssembly blendAssembly in RuntimeGeneratedTypesHelper.BlendAssemblies)
			{
				if (assemblies.Contains(blendAssembly))
				{
					continue;
				}
				platformMetadata.RegisterAssembly(((Microsoft.Expression.DesignModel.Metadata.RuntimeAssembly)blendAssembly).ReflectionAssembly);
				assemblies.Add(blendAssembly);
			}
		}

		[Conditional("DEBUG")]
		public static void SaveToDisk(bool showClipboardMessage)
		{
			try
			{
				string fullyQualifiedName = RuntimeGeneratedTypesHelper.RuntimeGeneratedTypesAssembly.FullyQualifiedName;
				if (!File.Exists(fullyQualifiedName))
				{
					RuntimeGeneratedTypesHelper.runtimeGeneratedTypesBuilder.Save(Path.GetFileName(fullyQualifiedName), PortableExecutableKinds.Required32Bit, ImageFileMachine.I386);
				}
				Clipboard.SetDataObject(new DataObject(DataFormats.UnicodeText, fullyQualifiedName));
				if (showClipboardMessage)
				{
					MessageBox.Show(string.Concat("File name copied to clipboard:\n", fullyQualifiedName));
				}
			}
			catch (Exception exception)
			{
			}
			RuntimeGeneratedTypesHelper.RuntimeGeneratedTypesAssembly = null;
			RuntimeGeneratedTypesHelper.runtimeGeneratedTypesBuilder = null;
		}
	}
}