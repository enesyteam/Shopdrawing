using System;

namespace Microsoft.Expression.Project
{
	internal class WebApplicationProjectType : MSBuildBasedProjectType
	{
		public override string Identifier
		{
			get
			{
				return ProjectTypeNamesHelper.WebApplication;
			}
		}

		public WebApplicationProjectType()
		{
		}

		public override INamedProject CreateProject(IProjectStore projectStore, ICodeDocumentType codeDocumentType, IServiceProvider serviceProvider)
		{
			return WebApplicationProject.Create(projectStore, codeDocumentType, this, serviceProvider);
		}

		public override bool IsValidTypeForProject(IProjectStore projectStore)
		{
			string property = projectStore.GetProperty("ProjectTypeGuids");
			string str = projectStore.GetProperty("OutputType");
			if (!base.IsValidTypeForProject(projectStore) || str == null || !str.Equals("Library", StringComparison.OrdinalIgnoreCase) || string.IsNullOrEmpty(property))
			{
				return false;
			}
			return property.IndexOf("{349c5851-65df-11da-9384-00065b846f21}", StringComparison.OrdinalIgnoreCase) != -1;
		}
	}
}