using Microsoft.Expression.Project;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project.Build
{
	public sealed class PartialXamlBuildFinishedEventArgs : EventArgs
	{
		public Microsoft.Expression.Project.Build.BuildResult BuildResult
		{
			get;
			private set;
		}

		public string OriginalXamlText
		{
			get;
			private set;
		}

		public IProjectItem ProjectItem
		{
			get;
			private set;
		}

		public string XamlFileLocation
		{
			get;
			private set;
		}

		public PartialXamlBuildFinishedEventArgs(IProjectItem projectItem, Microsoft.Expression.Project.Build.BuildResult buildResult, string xamlFileLocation, string originalXamlText)
		{
			this.ProjectItem = projectItem;
			this.BuildResult = buildResult;
			this.XamlFileLocation = xamlFileLocation;
			this.OriginalXamlText = originalXamlText;
		}
	}
}