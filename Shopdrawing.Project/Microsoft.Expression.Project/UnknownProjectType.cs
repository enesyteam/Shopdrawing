using System;

namespace Microsoft.Expression.Project
{
	public class UnknownProjectType : ProjectType
	{
		public override string Identifier
		{
			get
			{
				return ProjectTypeNamesHelper.Unknown;
			}
		}

		public UnknownProjectType()
		{
		}

		public override INamedProject CreateProject(IProjectStore projectStore, ICodeDocumentType codeDocumentType, IServiceProvider serviceProvider)
		{
			return new UnknownProject(projectStore, serviceProvider);
		}

		public override bool IsValidTypeForProject(IProjectStore projectStore)
		{
			return true;
		}
	}
}