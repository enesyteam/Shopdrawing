using Microsoft.Expression.Project;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;

namespace Microsoft.Expression.Project.Conversion
{
	internal sealed class WpfProjectConverter : ProjectConverter
	{
		private const string Key = "WpfProject";

		public override string Identifier
		{
			get
			{
				return "WpfProject";
			}
		}

		public WpfProjectConverter(ISolutionManagement solution, IServiceProvider serviceProvider) : base(solution, serviceProvider)
		{
		}

		public override ConversionType GetVersion(ConversionTarget file)
		{
			if (!file.IsProject)
			{
				return ConversionType.Unknown;
			}
			FrameworkName targetFrameworkName = ProjectStoreHelper.GetTargetFrameworkName(file.ProjectStore);
			if (targetFrameworkName == null || targetFrameworkName.Identifier != ".NETFramework")
			{
				return ConversionType.Unknown;
			}
			if (ProjectStoreHelper.UsesWpf(file.ProjectStore))
			{
				if (targetFrameworkName.Version == CommonVersions.Version3_0)
				{
					return ConversionType.ProjectWpf30;
				}
				if (targetFrameworkName.Version == CommonVersions.Version3_5)
				{
					return ConversionType.ProjectWpf35;
				}
				if (targetFrameworkName.Version == CommonVersions.Version4_0)
				{
					return ConversionType.ProjectWpf40;
				}
			}
			return ConversionType.Unknown;
		}

		private bool HandleVersion4Upgrade(IProjectStore projectStore)
		{
			if (!projectStore.GetItems("Reference").Any<IProjectItemData>((IProjectItemData reference) => {
				if (string.Equals(reference.Value, "System.Xaml", StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
				if (string.IsNullOrEmpty(reference.Value) || reference.Value.IndexOf("System.Xaml", StringComparison.OrdinalIgnoreCase) == -1)
				{
					return false;
				}
				AssemblyName assemblyName = ProjectConverterBase.GetAssemblyName(reference.Value);
				if (assemblyName == null)
				{
					return false;
				}
				if (assemblyName.Name.Equals("System.Xaml", StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
				return false;
			}))
			{
				IProjectItemData projectItemDatum = projectStore.AddItem("Reference", "System.Xaml");
				if (projectItemDatum == null)
				{
					return false;
				}
				if (!projectItemDatum.SetItemMetadata("RequiredTargetFramework", "v4.0"))
				{
					return false;
				}
			}
			return true;
		}

		protected override bool UpgradeProject(IProjectStore projectStore, ConversionType initialVersion, ConversionType targetVersion)
		{
			ProjectLanguage projectLanguage = ProjectStoreHelper.GetProjectLanguage(projectStore);
			if (projectLanguage != ProjectLanguage.CSharp && projectLanguage != ProjectLanguage.VisualBasic)
			{
				return false;
			}
			string str = null;
			ConversionType conversionType = targetVersion;
			switch (conversionType)
			{
				case ConversionType.ProjectWpf30:
				{
					str = "v3.0";
					break;
				}
				case ConversionType.ProjectWpf35:
				{
					str = "v3.5";
					break;
				}
				case ConversionType.ProjectWpf40:
				{
					str = "v4.0";
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
			WpfToolkitRemover.Update(projectStore, base.Context, initialVersion, targetVersion);
			if (!projectStore.SetProperty("TargetFrameworkVersion", str))
			{
				return false;
			}
			if (!projectStore.SetStoreVersion(CommonVersions.Version4_0))
			{
				return false;
			}
			if (targetVersion == ConversionType.ProjectWpf40 && !this.HandleVersion4Upgrade(projectStore))
			{
				return false;
			}
			AssemblyReferenceHelper.RepairAssemblyReferences(projectStore);
			return true;
		}
	}
}