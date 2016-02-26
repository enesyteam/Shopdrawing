using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.Build;
using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project.Commands
{
	internal sealed class CleanCommand : ProjectCommand
	{
		public override string DisplayName
		{
			get
			{
				return this.FormatWithSolutionTypeSubtext(StringTable.CommandCleanName);
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

		public CleanCommand(IServiceProvider serviceProvider) : base(serviceProvider)
		{
		}

		public override void Execute()
		{
			this.HandleBasicExceptions(() => {
				IProjectBuildContext activeBuildTarget = this.ProjectManager().ActiveBuildTarget;
				IExecutable startupProject = this.Solution().StartupProject;
				if (startupProject == null || !startupProject.IsExecuting)
				{
					this.ProjectManager().BuildManager.Clean(activeBuildTarget);
					return;
				}
				this.DisplayCommandFailedMessage(string.Format(CultureInfo.CurrentCulture, StringTable.BuildAndRunCommandUnableToBuildDialogMessage, new object[] { activeBuildTarget.DisplayName }));
			});
		}

		public override object GetProperty(string propertyName)
		{
			if (propertyName == "Text")
			{
				return this.FormatWithSolutionTypeSubtext(StringTable.MenuBarProjectMenuClean);
			}
			return base.GetProperty(propertyName);
		}
	}
}