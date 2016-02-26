using System;
using System.Runtime.Versioning;

namespace Microsoft.Expression.Project
{
	public abstract class MSBuildBasedProjectType : ProjectType
	{
		protected MSBuildBasedProjectType()
		{
		}

		private bool IsValidIfSketchFlow(IProjectStore projectStore)
		{
			if (!ProjectStoreHelper.IsSketchFlowProject(projectStore))
			{
				return true;
			}
			FrameworkName targetFrameworkName = ProjectStoreHelper.GetTargetFrameworkName(projectStore);
			if (targetFrameworkName == null)
			{
				return false;
			}
			return targetFrameworkName.Version == CommonVersions.Version4_0;
		}

		public override bool IsValidTypeForProject(IProjectStore projectStore)
		{
			Version storeVersion = projectStore.StoreVersion;
			if (!(projectStore is MSBuildBasedProjectStore) || !(storeVersion != null) || !(storeVersion == CommonVersions.Version4_0) || !ProjectStoreHelper.IsKnownLanguage(projectStore))
			{
				return false;
			}
			return this.IsValidIfSketchFlow(projectStore);
		}
	}
}