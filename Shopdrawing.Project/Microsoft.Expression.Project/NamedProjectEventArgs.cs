using System;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project
{
	public sealed class NamedProjectEventArgs : EventArgs
	{
		public INamedProject NamedProject
		{
			get;
			private set;
		}

		public NamedProjectEventArgs(INamedProject namedProject)
		{
			this.NamedProject = namedProject;
		}
	}
}