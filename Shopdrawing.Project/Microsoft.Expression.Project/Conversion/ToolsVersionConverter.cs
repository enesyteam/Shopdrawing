using Microsoft.Expression.Project;
using System;

namespace Microsoft.Expression.Project.Conversion
{
	internal sealed class ToolsVersionConverter : ProjectConverter
	{
		private const string Key = "ToolsVersionConverter";

		public override string Identifier
		{
			get
			{
				return "ToolsVersionConverter";
			}
		}

		public ToolsVersionConverter(ISolutionManagement solution, IServiceProvider serviceProvider) : base(solution, serviceProvider)
		{
		}

		public override ConversionType GetVersion(ConversionTarget project)
		{
			if (project.IsProject && ProjectStoreHelper.DoesLanguageSupportXaml(project.ProjectStore))
			{
				Version storeVersion = project.ProjectStore.StoreVersion;
				if (storeVersion == CommonVersions.Version2_0)
				{
					return ConversionType.BuildToolsVersion20;
				}
				if (storeVersion == CommonVersions.Version3_5)
				{
					return ConversionType.BuildToolsVersion35;
				}
				if (storeVersion == CommonVersions.Version4_0)
				{
					return ConversionType.BuildToolsVersion40;
				}
				if (storeVersion == null)
				{
					return ConversionType.BuildToolsVersionNone;
				}
			}
			return ConversionType.Unknown;
		}

		protected override bool UpgradeProject(IProjectStore projectStore, ConversionType initialVersion, ConversionType targetVersion)
		{
			if (targetVersion != ConversionType.BuildToolsVersion40)
			{
				return false;
			}
			Version silverlightVersion = ProjectStoreHelper.GetSilverlightVersion(projectStore);
			if (silverlightVersion == null)
			{
				return projectStore.SetStoreVersion(CommonVersions.Version4_0);
			}
			return SilverlightProjectConverter.ChangeImportPath(projectStore, silverlightVersion.ToString());
		}
	}
}