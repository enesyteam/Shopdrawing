using System;

namespace Microsoft.Expression.Project
{
	public sealed class SolutionEventArgs : EventArgs
	{
		private ISolution solution;

		public ISolution Solution
		{
			get
			{
				return this.solution;
			}
		}

		public SolutionEventArgs(ISolution solution)
		{
			this.solution = solution;
		}
	}
}