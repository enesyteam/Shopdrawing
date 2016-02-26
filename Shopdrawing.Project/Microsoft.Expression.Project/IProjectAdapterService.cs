using Microsoft.Expression.Extensibility.Project;
using System;
using System.Runtime.Versioning;

namespace Microsoft.Expression.Project
{
	internal interface IProjectAdapterService
	{
		T FindAdapter<T>(Microsoft.Expression.Project.IProject project);

		T FindAdapter<T>(Microsoft.Expression.Project.IProject project, out Microsoft.Expression.Extensibility.Project.IProject extensionProject);

		bool IsTargetFrameworkSupported(FrameworkName targetFramework);
	}
}