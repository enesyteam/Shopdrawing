using System;

namespace Microsoft.Expression.Project
{
	internal sealed class WatcherSuspender : IDisposable
	{
		private ISolutionManagement solution;

		public WatcherSuspender(ISolutionManagement solution)
		{
			this.solution = solution;
			solution.DeactivateWatchers();
		}

		public void Dispose()
		{
			this.solution.ReactivateWatchers();
		}
	}
}