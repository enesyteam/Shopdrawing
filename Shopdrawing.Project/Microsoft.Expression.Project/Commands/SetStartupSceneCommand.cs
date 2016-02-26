using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Project;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project.Commands
{
	internal sealed class SetStartupSceneCommand : ProjectCommand
	{
		public override string DisplayName
		{
			get
			{
				return StringTable.CommandSetAsStartupName;
			}
		}

		public bool IsChecked
		{
			get
			{
				IProjectItem projectItem = this.SelectedProjectItemOrNull();
				if (projectItem == null)
				{
					return false;
				}
				return projectItem.Project.StartupItem == projectItem;
			}
		}

		public override bool IsEnabled
		{
			get
			{
				IProjectItem projectItem = this.SelectedProjectItemOrNull();
				if (!base.IsEnabled || projectItem == null)
				{
					return false;
				}
				return projectItem.Project.IsValidStartupItem(projectItem);
			}
		}

		public bool IsVisible
		{
			get
			{
				IProjectItem projectItem = this.SelectedProjectItemOrNull();
				if (!base.IsEnabled || projectItem == null)
				{
					return false;
				}
				return projectItem.Project.GetCapability<bool>("CanHaveStartupItem");
			}
		}

		public SetStartupSceneCommand(IServiceProvider serviceProvider) : base(serviceProvider)
		{
		}

		private void ClearStartupScene()
		{
			IProject project = this.SelectedProjectOrNull();
			if (project != null)
			{
				project.StartupItem = null;
			}
		}

		public override void Execute()
		{
			if (this.IsEnabled)
			{
				this.HandleBasicExceptions(() => {
					IProjectItem projectItem = this.SelectedProjectItemOrNull();
					if (projectItem != null && projectItem.Project.IsValidStartupItem(projectItem))
					{
						projectItem.Project.StartupItem = projectItem;
					}
				});
			}
		}

		public override object GetProperty(string propertyName)
		{
			if (propertyName == "IsChecked")
			{
				return this.IsChecked;
			}
			if (propertyName == "IsVisible")
			{
				return this.IsVisible;
			}
			return base.GetProperty(propertyName);
		}

		public override void SetProperty(string propertyName, object propertyValue)
		{
			if (propertyName != "IsChecked")
			{
				base.SetProperty(propertyName, propertyValue);
				return;
			}
			if ((bool)propertyValue)
			{
				this.Execute();
				return;
			}
			this.ClearStartupScene();
		}
	}
}