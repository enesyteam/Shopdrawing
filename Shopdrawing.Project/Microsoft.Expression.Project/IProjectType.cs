using System;

namespace Microsoft.Expression.Project
{
	public interface IProjectType
	{
		string Identifier
		{
			get;
		}

		IProjectCreateError CanCreateProject(IProjectStore projectStore);

		INamedProject CreateProject(IProjectStore projectStore, ICodeDocumentType codeDocumentType, IServiceProvider serviceProvider);

		bool IsValidTypeForProject(IProjectStore projectStore);
	}
}