using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Microsoft.Expression.DesignModel.Metadata
{
	public static class SystemThemeAssemblies
	{
		private readonly static ICollection<IAssembly> themeAssemblyReferences;

		private readonly static Dictionary<string, string> knownStrongNames;

		private static IAssembly PresentationFrameworkAeroAssembly;

		private static IAssembly PresentationFrameworkClassicAssembly;

		private static IAssembly PresentationFrameworkLunaAssembly;

		private static IAssembly PresentationFrameworkRoyaleAssembly;

		private static IAssembly PresentationUIAssembly;

		public static ICollection<IAssembly> ThemeAssemblyReferences
		{
			get
			{
				return SystemThemeAssemblies.themeAssemblyReferences;
			}
		}

		static SystemThemeAssemblies()
		{
			SystemThemeAssemblies.PresentationFrameworkAeroAssembly = new Microsoft.Expression.DesignModel.Metadata.RuntimeAssembly("PresentationFramework.Aero", AssemblySource.Unknown);
			SystemThemeAssemblies.PresentationFrameworkClassicAssembly = new Microsoft.Expression.DesignModel.Metadata.RuntimeAssembly("PresentationFramework.Classic", AssemblySource.Unknown);
			SystemThemeAssemblies.PresentationFrameworkLunaAssembly = new Microsoft.Expression.DesignModel.Metadata.RuntimeAssembly("PresentationFramework.Luna", AssemblySource.Unknown);
			SystemThemeAssemblies.PresentationFrameworkRoyaleAssembly = new Microsoft.Expression.DesignModel.Metadata.RuntimeAssembly("PresentationFramework.Royale", AssemblySource.Unknown);
			SystemThemeAssemblies.PresentationUIAssembly = new Microsoft.Expression.DesignModel.Metadata.RuntimeAssembly("PresentationUI", AssemblySource.Unknown);
			SystemThemeAssemblies.knownStrongNames = new Dictionary<string, string>();
			SystemThemeAssemblies.knownStrongNames["PresentationFramework.Aero"] = "PresentationFramework.Aero, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35";
			SystemThemeAssemblies.knownStrongNames["PresentationFramework.Classic"] = "PresentationFramework.Classic, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35";
			SystemThemeAssemblies.knownStrongNames["PresentationFramework.Luna"] = "PresentationFramework.Luna, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35";
			SystemThemeAssemblies.knownStrongNames["PresentationFramework.Royale"] = "PresentationFramework.Royale, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35";
			SystemThemeAssemblies.knownStrongNames["PresentationUI"] = "PresentationUI, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35";
			List<IAssembly> assemblies = new List<IAssembly>()
			{
				SystemThemeAssemblies.PresentationFrameworkAeroAssembly,
				SystemThemeAssemblies.PresentationFrameworkClassicAssembly,
				SystemThemeAssemblies.PresentationFrameworkLunaAssembly,
				SystemThemeAssemblies.PresentationFrameworkRoyaleAssembly,
				SystemThemeAssemblies.PresentationUIAssembly
			};
			SystemThemeAssemblies.themeAssemblyReferences = new ReadOnlyCollection<IAssembly>(assemblies);
		}

		public static void LoadAssemblies()
		{
			foreach (IAssembly themeAssemblyReference in SystemThemeAssemblies.ThemeAssemblyReferences)
			{
				if (themeAssemblyReference.IsLoaded)
				{
					continue;
				}
				AssemblyName assemblyName = new AssemblyName(SystemThemeAssemblies.knownStrongNames[themeAssemblyReference.Name]);
				((Microsoft.Expression.DesignModel.Metadata.RuntimeAssembly)themeAssemblyReference).ReflectionAssembly = AppDomain.CurrentDomain.Load(assemblyName);
			}
		}
	}
}