using System;

namespace Microsoft.Expression.Project
{
	public interface IProjectOutputReferenceInformation
	{
		bool DeployToConfigurationSpecificFolder
		{
			get;
		}

		string RelativeOutputPath
		{
			get;
		}

		Guid SourceGuid
		{
			get;
		}

		string CreateDeploymentPath(IProject targetProject, IProject sourceProject);
	}
}