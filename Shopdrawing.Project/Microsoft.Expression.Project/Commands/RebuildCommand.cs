using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.Build;
using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project.Commands
{
	internal sealed class RebuildCommand : ProjectCommand
	{
		public override string DisplayName
		{
			get
			{
				return this.FormatWithSolutionTypeSubtext(StringTable.CommandRebuildName);
			}
		}

		public override bool IsAvailable
		{
			get
			{
				if (this.Solution() is WebProjectSolution)
				{
					return false;
				}
				return base.IsAvailable;
			}
		}

		public override bool IsEnabled
		{
			get
			{
				IProjectBuildContext activeBuildTarget = this.ProjectManager().ActiveBuildTarget;
				if (!base.IsEnabled || activeBuildTarget == null)
				{
					return false;
				}
				return !BuildManager.Building;
			}
		}

		public RebuildCommand(IServiceProvider serviceProvider) : base(serviceProvider)
		{
		}

		private void BuildFinished(object sender, BuildCompletedEventArgs args)
		{
			(sender as BuildManager).BuildCompleted -= new EventHandler<BuildCompletedEventArgs>(this.BuildFinished);
			if (args.BuildResult == BuildResult.Failed)
			{
				this.DisplayCommandFailedMessage(StringTable.BuildAndRunCommandBuildFailedDialogMessage);
			}
		}

		public override void Execute()
		{
			if (this.IsEnabled)
			{
				this.HandleBasicExceptions(() => {
					BuildManager buildManager = this.ProjectManager().BuildManager;
					IProjectBuildContext activeBuildTarget = this.ProjectManager().ActiveBuildTarget;
					IExecutable startupProject = this.Solution().StartupProject;
					if (startupProject != null && startupProject.IsExecuting)
					{
						this.DisplayCommandFailedMessage(string.Format(CultureInfo.CurrentCulture, StringTable.BuildAndRunCommandUnableToBuildDialogMessage, new object[] { activeBuildTarget.DisplayName }));
					}
					else if (this.SaveSolution(false))
					{
						buildManager.BuildCompleted += new EventHandler<BuildCompletedEventArgs>(this.BuildFinished);
						buildManager.Rebuild(activeBuildTarget, startupProject, false);
						return;
					}
				});
			}
		}

		public override object GetProperty(string propertyName)
		{
			if (propertyName == "Text")
			{
				return this.FormatWithSolutionTypeSubtext(StringTable.MenuBarProjectMenuRebuild);
			}
			return base.GetProperty(propertyName);
		}
	}
}