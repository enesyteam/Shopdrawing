using System;

namespace Microsoft.Expression.Project
{
	public abstract class ProjectType : IProjectType
	{
		public abstract string Identifier
		{
			get;
		}

		protected ProjectType()
		{
		}

		public virtual IProjectCreateError CanCreateProject(IProjectStore projectStore)
		{
			return null;
		}

		public abstract INamedProject CreateProject(IProjectStore projectStore, ICodeDocumentType codeDocumentType, IServiceProvider serviceProvider);

		public virtual ReferenceAssemblyMode GetReferenceAssemblyMode(IProject project)
		{
			return ReferenceAssemblyMode.None;
		}

		public abstract bool IsValidTypeForProject(IProjectStore projectStore);
	}
}