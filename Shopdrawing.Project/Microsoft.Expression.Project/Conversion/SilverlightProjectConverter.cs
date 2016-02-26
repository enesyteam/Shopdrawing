using Microsoft.Expression.Framework.Interop;
using Microsoft.Expression.Project;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Expression.Project.Conversion
{
	internal sealed class SilverlightProjectConverter : ProjectConverter
	{
		private const string Key = "Silverlight2ToSilverlight3";

		private const string ProjectTypeGuidsKey = "ProjectTypeGuids";

		private const string WindowsCEEnabledKey = "WindowsCEEnabled";

		private const string SilverlightMobileKey = "SilverlightMobile";

		private const string RawSilverlightCSharpTargets = "$(MSBuildExtensionsPath32)\\Microsoft\\Silverlight\\$(SilverlightVersion)\\Microsoft.Silverlight.CSharp.targets";

		private const string RawSilverlightVisualBasicTargets = "$(MSBuildExtensionsPath32)\\Microsoft\\Silverlight\\$(SilverlightVersion)\\Microsoft.Silverlight.VisualBasic.targets";

		public override string Identifier
		{
			get
			{
				return "Silverlight2ToSilverlight3";
			}
		}

		public SilverlightProjectConverter(ISolutionManagement solution, IServiceProvider serviceProvider) : base(solution, serviceProvider)
		{
		}

		internal static bool ChangeImportPath(IProjectStore projectStore, string newVersion)
		{
			string str;
			ProjectLanguage projectLanguage = ProjectStoreHelper.GetProjectLanguage(projectStore);
			if (!ProjectStoreHelper.DoesLanguageSupportXaml(projectLanguage))
			{
				return false;
			}
			switch (projectLanguage)
			{
				case ProjectLanguage.CSharp:
				{
					str = "$(MSBuildExtensionsPath32)\\Microsoft\\Silverlight\\$(SilverlightVersion)\\Microsoft.Silverlight.CSharp.targets";
					break;
				}
				case ProjectLanguage.FSharp:
				{
					return false;
				}
				case ProjectLanguage.VisualBasic:
				{
					str = "$(MSBuildExtensionsPath32)\\Microsoft\\Silverlight\\$(SilverlightVersion)\\Microsoft.Silverlight.VisualBasic.targets";
					break;
				}
				default:
				{
					return false;
				}
			}
			string[] array = ProjectStoreHelper.GetSilverlightImports(projectStore).ToArray<string>();
			if ((int)array.Length != 0)
			{
				string[] strArrays = array;
				for (int i = 0; i < (int)strArrays.Length; i++)
				{
					if (!projectStore.ChangeImport(strArrays[i], str))
					{
						return false;
					}
				}
			}
			else
			{
				projectStore.AddImport(str);
			}
			projectStore.SetProperty("TargetFrameworkVersion", string.Concat("v", newVersion));
			projectStore.SetProperty("TargetFrameworkIdentifier", "Silverlight");
			projectStore.SetProperty("SilverlightVersion", "$(TargetFrameworkVersion)");
			projectStore.SetStoreVersion(CommonVersions.Version4_0);
			foreach (IProjectItemData item in projectStore.GetItems("Reference"))
			{
				string metadata = item.GetMetadata("RequiredTargetFramework");
				if (string.IsNullOrEmpty(metadata) || !metadata.Trim().Equals("3.5", StringComparison.Ordinal))
				{
					continue;
				}
				item.SetItemMetadata("RequiredTargetFramework", "3.0");
			}
			return true;
		}

		public override ConversionType GetVersion(ConversionTarget file)
		{
			if (!file.IsProject)
			{
				return ConversionType.Unknown;
			}
			IProjectStore projectStore = file.ProjectStore;
			if (Microsoft.Expression.Framework.Interop.TypeHelper.ConvertType<bool>(projectStore.GetProperty("WindowsCEEnabled")) || Microsoft.Expression.Framework.Interop.TypeHelper.ConvertType<bool>(projectStore.GetProperty("SilverlightMobile")))
			{
				return ConversionType.Unsupported;
			}
			Version silverlightVersion = ProjectStoreHelper.GetSilverlightVersion(projectStore);
			if (silverlightVersion == CommonVersions.Version2_0)
			{
				return ConversionType.ProjectSilverlight2;
			}
			if (silverlightVersion == CommonVersions.Version3_0)
			{
				return ConversionType.ProjectSilverlight3;
			}
			if (silverlightVersion == CommonVersions.Version4_0)
			{
				return ConversionType.ProjectSilverlight4;
			}
			return ConversionType.Unknown;
		}

		protected override bool UpgradeProject(IProjectStore projectStore, ConversionType initialVersion, ConversionType targetVersion)
		{
			string str = null;
			ConversionType conversionType = targetVersion;
			switch (conversionType)
			{
				case ConversionType.ProjectSilverlight3:
				{
					str = "3.0";
					break;
				}
				case ConversionType.ProjectSilverlight4:
				{
					str = "4.0";
					break;
				}
				default:
				{
					switch (conversionType)
					{
						case ConversionType.Unsupported:
						case ConversionType.DoNothing:
						case ConversionType.Unknown:
						{
							return true;
						}
						default:
						{
							return false;
						}
					}
					break;
				}
			}
			if (!SilverlightProjectConverter.ChangeImportPath(projectStore, str))
			{
				return false;
			}
			AssemblyReferenceHelper.RepairAssemblyReferences(projectStore);
			return true;
		}
	}
}