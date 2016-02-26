using System;

namespace Microsoft.Expression.Extensibility.Project
{
	public interface IProjectExecutionAdapter : IProjectAdapter
	{
		bool Execute(IProject project, ExecuteCompleteCallback executeComplete);
	}
}