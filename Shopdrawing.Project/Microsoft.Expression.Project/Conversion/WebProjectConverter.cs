using Microsoft.Expression.Project;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;

namespace Microsoft.Expression.Project.Conversion
{
	internal sealed class WebProjectConverter : ProjectConverter
	{
		private const string Key = "WebAppHostProject";

		private const string ProjectTypeGuidsKey = "ProjectTypeGuids";

		private const string WebTargetImport = "$(MSBuildExtensionsPath32)\\Microsoft\\VisualStudio\\v10.0\\WebApplications\\Microsoft.WebApplication.targets";

		private static Regex webApplicationTargetVersion;

		public override string Identifier
		{
			get
			{
				return "WebAppHostProject";
			}
		}

		static WebProjectConverter()
		{
			WebProjectConverter.webApplicationTargetVersion = new Regex(".*\\\\(?<version>[^\\\\]*)\\\\WebApplications\\\\Microsoft.WebApplication.targets\\s*", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);
		}

		public WebProjectConverter(ISolutionManagement solution, IServiceProvider serviceProvider) : base(solution, serviceProvider)
		{
		}

		public override ConversionType GetVersion(ConversionTarget file)
		{
			ConversionType conversionType;
			if (!file.IsProject)
			{
				return ConversionType.Unknown;
			}
			FrameworkName targetFrameworkName = ProjectStoreHelper.GetTargetFrameworkName(file.ProjectStore);
			if (targetFrameworkName == null || targetFrameworkName.Identifier != ".NETFramework")
			{
				return ConversionType.Unknown;
			}
			string property = file.ProjectStore.GetProperty("ProjectTypeGuids");
			if (!string.IsNullOrEmpty(property) && property.IndexOf("{349c5851-65df-11da-9384-00065b846f21}", StringComparison.OrdinalIgnoreCase) != -1)
			{
				using (IEnumerator<string> enumerator = file.ProjectStore.ProjectImports.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string current = enumerator.Current;
						Match match = WebProjectConverter.webApplicationTargetVersion.Match(current);
						if (!match.Success || !string.Equals(match.Groups["version"].Value, "v9.0", StringComparison.OrdinalIgnoreCase))
						{
							continue;
						}
						conversionType = ConversionType.WebAppProject9;
						return conversionType;
					}
					return ConversionType.Unknown;
				}
				return conversionType;
			}
			return ConversionType.Unknown;
		}

		protected override bool UpgradeProject(IProjectStore projectStore, ConversionType initialVersion, ConversionType targetVersion)
		{
			ProjectLanguage projectLanguage = ProjectStoreHelper.GetProjectLanguage(projectStore);
			if (projectLanguage != ProjectLanguage.CSharp && projectLanguage != ProjectLanguage.VisualBasic)
			{
				return false;
			}
			if (targetVersion != ConversionType.WebAppProject10)
			{
				return false;
			}
			string[] array = projectStore.ProjectImports.ToArray<string>();
			for (int i = 0; i < (int)array.Length; i++)
			{
				string str = array[i];
				if (WebProjectConverter.webApplicationTargetVersion.Match(str).Success && !projectStore.ChangeImport(str, "$(MSBuildExtensionsPath32)\\Microsoft\\VisualStudio\\v10.0\\WebApplications\\Microsoft.WebApplication.targets"))
				{
					return false;
				}
			}
			if (!projectStore.SetStoreVersion(CommonVersions.Version4_0))
			{
				return false;
			}
			return true;
		}
	}
}