using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.UserInterface;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.Build;
using Microsoft.Expression.Project.ServiceExtensions.Messaging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;

namespace Microsoft.Expression.Project.Commands
{
	internal sealed class TestProjectCommand : ProjectCommand
	{
		public override string DisplayName
		{
			get
			{
				return this.FormatWithSolutionTypeSubtext(StringTable.CommandTestName);
			}
		}

		public override bool IsEnabled
		{
			get
			{
				ISolution solution = this.Solution();
				if (!base.IsEnabled || solution == null || solution.StartupProject == null || !solution.StartupProject.CanExecute)
				{
					return false;
				}
				return !BuildManager.Building;
			}
		}

		public TestProjectCommand(IServiceProvider serviceProvider) : base(serviceProvider)
		{
		}

		private void BuildAndRun(IProjectBuildContext buildTarget, IExecutable executable)
		{
			if (buildTarget != null)
			{
				VisualStudioSolution visualStudioSolution = buildTarget as VisualStudioSolution;
				IProject startupProject = buildTarget as IProject;
				if (startupProject == null && visualStudioSolution != null)
				{
					startupProject = visualStudioSolution.StartupProject as IProject;
				}
				if (startupProject != null && startupProject.GetCapability<bool>("CanHaveStartupItem") && startupProject.StartupItem == null && !this.CapabilitiesAllowNullStartupItem(startupProject))
				{
					if (visualStudioSolution != null)
					{
						MessageBoxArgs messageBoxArg = new MessageBoxArgs()
						{
							Message = StringTable.RunCommandNoStartupSceneWarningMessage,
							Button = MessageBoxButton.YesNo,
							Image = MessageBoxImage.Exclamation
						};
						if (base.Services.ShowSuppressibleWarning(messageBoxArg, "ShowStartupSceneWarning", MessageBoxResult.Yes, visualStudioSolution.SolutionSettingsManager.SolutionSettings) == MessageBoxResult.No)
						{
							return;
						}
					}
					else if (!base.Services.PromptUserYesNo(StringTable.RunCommandNoStartupSceneWarningMessage))
					{
						return;
					}
				}
				ISolution solution = buildTarget as ISolution;
				IList<ProjectBase> list = null;
				if (solution != null)
				{
					list = (
						from project in solution.Projects.OfType<ProjectBase>()
						where project.DoesErrorConditionExist("PotentiallyInvalidTransparentCache", NetworkInterface.GetIsNetworkAvailable())
						select project).ToList<ProjectBase>();
				}
				if (list != null && list.Count > 0)
				{
					StringBuilder stringBuilder = new StringBuilder(128);
					foreach (IProject project1 in list)
					{
						stringBuilder.AppendLine(string.Concat("\t", project1.Name));
					}
					CultureInfo currentCulture = CultureInfo.CurrentCulture;
					string runCommandPlatformExtensionsWarningMessage = StringTable.RunCommandPlatformExtensionsWarningMessage;
					object[] str = new object[] { stringBuilder.ToString() };
					string str1 = string.Format(currentCulture, runCommandPlatformExtensionsWarningMessage, str);
					if (visualStudioSolution != null)
					{
						MessageBoxArgs messageBoxArg1 = new MessageBoxArgs()
						{
							Message = str1,
							Button = MessageBoxButton.YesNo,
							Image = MessageBoxImage.Exclamation
						};
						if (base.Services.ShowSuppressibleWarning(messageBoxArg1, "AllowPlatformExtension", MessageBoxResult.Yes, visualStudioSolution.SolutionSettingsManager.SolutionSettings) == MessageBoxResult.No)
						{
							return;
						}
					}
					else if (!base.Services.PromptUserYesNo(str1))
					{
						return;
					}
				}
			}
			if (this.SaveSolution(false))
			{
				if (buildTarget != null)
				{
					BuildManager buildManager = this.ProjectManager().BuildManager;
					buildManager.BuildCompleted += new EventHandler<BuildCompletedEventArgs>(this.BuildFinished);
					buildManager.Build(buildTarget, executable, false);
					return;
				}
				if (executable != null)
				{
					executable.Execute();
				}
			}
		}

		private void BuildFinished(object sender, BuildCompletedEventArgs args)
		{
			BuildResult buildResult = args.BuildResult;
			(sender as BuildManager).BuildCompleted -= new EventHandler<BuildCompletedEventArgs>(this.BuildFinished);
			IExecutable executable = args.Executable;
			if (executable != null)
			{
				if (buildResult == BuildResult.Succeeded)
				{
					executable.Execute();
					return;
				}
				if (buildResult == BuildResult.Failed)
				{
					this.DisplayCommandFailedMessage(StringTable.BuildAndRunCommandBuildFailedDialogMessage);
				}
			}
		}

		private bool CapabilitiesAllowNullStartupItem(IProject project)
		{
			if (project.GetCapability<bool>("ExpressionBlendPrototypeHarness"))
			{
				return true;
			}
			return project.GetCapability<bool>("ExpressionBlendPrototypingEnabled");
		}

		public override void Execute()
		{
			PerformanceUtility.StartPerformanceSequence(PerformanceEvent.ProjectRun);
			if (this.IsEnabled)
			{
				Microsoft.Expression.Framework.UserInterface.IWindowService service = (Microsoft.Expression.Framework.UserInterface.IWindowService)base.Services.GetService(typeof(Microsoft.Expression.Framework.UserInterface.IWindowService));
				if (service != null)
				{
					service.ReturnFocus();
				}
				ISolution solution = this.Solution();
				IProjectBuildContext projectBuildContext = solution.ProjectBuildContext;
				IExecutable startupProject = solution.StartupProject;
				string str = (projectBuildContext != null ? projectBuildContext.DisplayName : solution.DocumentReference.DisplayName);
				if (startupProject == null || !startupProject.IsExecuting)
				{
					this.HandleBasicExceptions(() => this.BuildAndRun(projectBuildContext, startupProject));
					return;
				}
				CultureInfo currentCulture = CultureInfo.CurrentCulture;
				string runCommandUnableToRunDialogMessage = StringTable.RunCommandUnableToRunDialogMessage;
				object[] objArray = new object[] { str };
				this.DisplayCommandFailedMessage(string.Format(currentCulture, runCommandUnableToRunDialogMessage, objArray));
			}
		}

		public override object GetProperty(string propertyName)
		{
			if (propertyName == "Text")
			{
				return this.FormatWithSolutionTypeSubtext(StringTable.MenuBarProjectMenuTestProject);
			}
			return base.GetProperty(propertyName);
		}
	}
}