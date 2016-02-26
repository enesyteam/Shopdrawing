using System;

namespace Microsoft.Expression.Project
{
	public interface IProjectTypeManager
	{
		IProjectTypeCollection ProjectTypes
		{
			get;
		}

		IProjectType UnknownProjectType
		{
			get;
		}

		IProjectType GetProjectTypeForProject(IProjectStore projectStore);

		void Register(IProjectType projectType);

		void Unregister(IProjectType projectType);
	}
}