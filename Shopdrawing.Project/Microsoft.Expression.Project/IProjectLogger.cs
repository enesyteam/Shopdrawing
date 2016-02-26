using System;

namespace Microsoft.Expression.Project
{
	public interface IProjectLogger
	{
		void LogError(string path, Exception exception, string messageTemplate, params object[] args);

		void LogError(string path, string errorMessage, string messageTemplate, params object[] args);

		void LogSuccess(string path, string messageTemplate, params object[] args);
	}
}