using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.Build;
using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project.Commands
{
	internal sealed class BuildCommand : ProjectCommand
	{
		public override string DisplayName
		{
			get
			{
				return this.FormatWithSolutionTypeSubtext(StringTable.CommandBuildName);
			}
		}

		public override bool IsAvailable
		{
			get
			{
				if (!base.IsAvailable)
				{
					return false;
				}
				return !(this.Solution() is WebProjectSolution);
			}
		}

		public override bool IsEnabled
		{
			get
			{
				ISolution solution = this.Solution();
				if (solution == null)
				{
					return false;
				}
				IProjectBuildContext projectBuildContext = solution.ProjectBuildContext;
				if (!base.IsEnabled || projectBuildContext == null)
				{
					return false;
				}
				return !BuildManager.Building;
			}
		}

		public BuildCommand(IServiceProvider serviceProvider) : base(serviceProvider)
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
					IExecutable startupProject = this.Solution().StartupProject;
					if (startupProject != null && startupProject.IsExecuting)
					{
						this.DisplayCommandFailedMessage(string.Format(CultureInfo.CurrentCulture, StringTable.BuildAndRunCommandUnableToBuildDialogMessage, new object[] { this.Solution().DocumentReference.DisplayName }));
					}
					else if (this.SaveSolution(false))
					{
						IProjectBuildContext projectBuildContext = this.Solution().ProjectBuildContext;
						BuildManager buildManager = this.ProjectManager().BuildManager;
						buildManager.BuildCompleted += new EventHandler<BuildCompletedEventArgs>(this.BuildFinished);
						buildManager.Build(projectBuildContext, startupProject, false);
						return;
					}
				});
			}
		}

		public override object GetProperty(string propertyName)
		{
			if (propertyName == "Text")
			{
				return this.FormatWithSolutionTypeSubtext(StringTable.MenuBarProjectMenuBuild);
			}
			return base.GetProperty(propertyName);
		}
	}
}