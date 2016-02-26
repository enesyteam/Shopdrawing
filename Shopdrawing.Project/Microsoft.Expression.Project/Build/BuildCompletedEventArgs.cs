using System;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project.Build
{
	public sealed class BuildCompletedEventArgs : EventArgs
	{
		public Microsoft.Expression.Project.Build.BuildResult BuildResult
		{
			get;
			private set;
		}

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

		public IExecutable Executable
		{
			get;
			private set;
		}

		public BuildCompletedEventArgs(IProjectBuildContext buildTarget, IExecutable executable, Microsoft.Expression.Project.Build.BuildResult buildResult, bool displayOutput)
		{
			this.BuildTarget = buildTarget;
			this.Executable = executable;
			this.BuildResult = buildResult;
			this.DisplayOutput = displayOutput;
		}
	}
}