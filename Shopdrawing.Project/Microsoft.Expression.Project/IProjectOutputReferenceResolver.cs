using System;

namespace Microsoft.Expression.Project
{
	public interface IProjectOutputReferenceResolver
	{
		Uri GetDeploymentResolvedRoot(IProject sourceProject);

		Uri GetDeploymentResolvedRoot(IProject sourceProject, out Uri targetProjectRoot);

		IProjectOutputReferenceInformation GetProjectOutputReferenceInfo(IProject targetProject, IProject sourceProject);
	}
}