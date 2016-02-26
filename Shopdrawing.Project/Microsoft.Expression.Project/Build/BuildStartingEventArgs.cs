using System;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project.Build
{
	public sealed class BuildStartingEventArgs : EventArgs
	{
		public IProjectBuildContext BuildTarget
		{
			get;
			private set;
		}

		public bool DisplayOutput
		{
			get;
			private set;
		}

		public BuildStartingEventArgs(IProjectBuildContext buildTarget, bool displayOutput)
		{
			this.BuildTarget = buildTarget;
			this.DisplayOutput = displayOutput;
		}
	}
}