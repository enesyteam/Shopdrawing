using System;

namespace Microsoft.Expression.Project
{
	internal class WebsiteProjectType : ProjectType
	{
		public override string Identifier
		{
			get
			{
				return ProjectTypeNamesHelper.Website;
			}
		}

		public WebsiteProjectType()
		{
		}

		public override INamedProject CreateProject(IProjectStore projectStore, ICodeDocumentType codeDocumentType, IServiceProvider serviceProvider)
		{
			return WebsiteProject.Create(projectStore, this, serviceProvider);
		}

		public override bool IsValidTypeForProject(IProjectStore projectStore)
		{
			return projectStore is FileSystemProjectStore;
		}
	}
}