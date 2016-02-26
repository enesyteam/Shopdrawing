using System;
using System.Runtime.Versioning;

namespace Microsoft.Expression.Extensibility.Project
{
	public interface IProjectAdapter
	{
		FrameworkName RequiredTargetFramework
		{
			get;
		}

		bool AppliesToProject(IProject project);
	}
}