using System;

namespace Microsoft.Expression.Project
{
	public interface IAssemblyLoggingService
	{
		bool IsEnabled
		{
			get;
			set;
		}

		void Log(AssemblyLoggingEvent assemblyLoadingEvent);
	}
}