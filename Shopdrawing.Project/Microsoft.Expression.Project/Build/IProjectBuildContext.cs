using Microsoft.Build.Execution;
using Microsoft.Expression.Framework;
using System;

namespace Microsoft.Expression.Project.Build
{
	public interface IProjectBuildContext
	{
		IErrorTaskCollection BuildErrors
		{
			get;
		}

		IBuildWorker BuildWorker
		{
			get;
		}

		string DisplayName
		{
			get;
		}

		Microsoft.Build.Execution.ProjectInstance ProjectInstance
		{
			get;
		}

		void BuildCompleted(Microsoft.Expression.Project.Build.BuildResult buildResult);
	}
}