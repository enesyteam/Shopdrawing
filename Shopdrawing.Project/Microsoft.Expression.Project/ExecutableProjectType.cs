using System;

namespace Microsoft.Expression.Project
{
	public class ExecutableProjectType : MSBuildBasedProjectType
	{
		public override string Identifier
		{
			get
			{
				return ProjectTypeNamesHelper.Executable;
			}
		}

		public ExecutableProjectType()
		{
		}

		public override INamedProject CreateProject(IProjectStore projectStore, ICodeDocumentType codeDocumentType, IServiceProvider serviceProvider)
		{
			return ExecutableProject.Create(projectStore, codeDocumentType, this, serviceProvider);
		}

		public override bool IsValidTypeForProject(IProjectStore projectStore)
		{
			string property = projectStore.GetProperty("OutputType");
			if (property == null || !property.Equals("Exe", StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			return base.IsValidTypeForProject(projectStore);
		}
	}
}