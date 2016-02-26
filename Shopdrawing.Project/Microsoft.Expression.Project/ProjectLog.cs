using System;
using System.Collections.Generic;

namespace Microsoft.Expression.Project
{
	internal static class ProjectLog
	{
		private static List<IProjectLogger> loggers;

		static ProjectLog()
		{
			ProjectLog.loggers = new List<IProjectLogger>();
		}

		public static void LogError(string path, Exception e, string messageTemplate, params object[] messageParams)
		{
			foreach (IProjectLogger logger in ProjectLog.loggers)
			{
				logger.LogError(path, e, messageTemplate, messageParams);
			}
		}

		public static void LogError(string path, string error, string messageTemplate, params object[] messageParams)
		{
			foreach (IProjectLogger logger in ProjectLog.loggers)
			{
				logger.LogError(path, error, messageTemplate, messageParams);
			}
		}

		public static void LogSuccess(string path, string messageTemplate, params object[] messageParams)
		{
			foreach (IProjectLogger logger in ProjectLog.loggers)
			{
				logger.LogSuccess(path, messageTemplate, messageParams);
			}
		}

		public static void Register(IProjectLogger logger)
		{
			if (logger != null)
			{
				ProjectLog.loggers.Add(logger);
			}
		}

		public static void Unregister(IProjectLogger logger)
		{
			ProjectLog.loggers.Remove(logger);
		}
	}
}