using System;

namespace Microsoft.Expression.Project
{
	public sealed class ProjectEventArgs : EventArgs
	{
		private IProject project;

		public IProject Project
		{
			get
			{
				return this.project;
			}
		}

		public ProjectEventArgs(IProject project)
		{
			this.project = project;
		}
	}
}