using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.Build;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project.Commands
{
	internal sealed class SetStartupProjectCommand : ProjectCommand
	{
		public override string DisplayName
		{
			get
			{
				return StringTable.CommandSetStartupProjectName;
			}
		}

		public override bool IsAvailable
		{
			get
			{
				IProject project = this.SelectedProjectOrNull();
				bool flag = (project != null ? project.GetCapability<bool>("CanBeStartupProject") : false);
				if (base.IsAvailable && (flag || (bool)this.GetProperty("IsChecked")))
				{
					return true;
				}
				return false;
			}
		}

		public override bool IsEnabled
		{
			get
			{
				IProject project = this.SelectedProjectOrNull();
				bool flag = (project != null ? project.GetCapability<bool>("CanBeStartupProject") : false);
				if (base.IsEnabled)
				{
					return flag;
				}
				return false;
			}
		}

		public SetStartupProjectCommand(IServiceProvider serviceProvider) : base(serviceProvider)
		{
		}

		public override void Execute()
		{
			this.HandleBasicExceptions(() => {
				IExecutable executable = this.SelectedProjectOrNull() as IExecutable;
				ISolutionManagement solutionManagement = this.Solution() as ISolutionManagement;
				if (executable != null && solutionManagement != null)
				{
					solutionManagement.StartupProject = executable;
				}
			});
		}

		public override object GetProperty(string propertyName)
		{
			if (propertyName != "IsChecked")
			{
				if (propertyName == "Text")
				{
					return this.DisplayName;
				}
				return base.GetProperty(propertyName);
			}
			IProject startupProject = this.Solution().StartupProject as IProject;
			return (startupProject == null ? false : startupProject == this.SelectedProjectOrNull());
		}

		public override void SetProperty(string propertyName, object propertyValue)
		{
			if (propertyName != "IsChecked")
			{
				base.SetProperty(propertyName, propertyValue);
			}
			else if ((bool)propertyValue)
			{
				this.Execute();
				return;
			}
		}
	}
}