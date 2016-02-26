using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.Build;
using Microsoft.Expression.Project.ServiceExtensions;
using System;
using System.Globalization;
using System.Windows;

namespace Microsoft.Expression.Project.Commands
{
	internal sealed class OpenRecentProject : ProjectCommand
	{
		private int recentIndex;

		public override string DisplayName
		{
			get
			{
				return StringTable.CommandOpenRecentProjectName;
			}
		}

		private bool HasValidProject
		{
			get
			{
				return (int)this.ProjectManager().RecentProjects.Length > this.recentIndex;
			}
		}

		public override bool IsEnabled
		{
			get
			{
				if (!base.IsEnabled || !this.HasValidProject)
				{
					return false;
				}
				return !BuildManager.Building;
			}
		}

		private string Project
		{
			get
			{
				if (!this.HasValidProject)
				{
					return string.Empty;
				}
				return this.ProjectManager().RecentProjects[this.recentIndex];
			}
		}

		public OpenRecentProject(IServiceProvider serviceProvider, int recentIndex) : base(serviceProvider)
		{
			this.recentIndex = recentIndex;
		}

		public override void Execute()
		{
			string project = this.Project;
			if (!string.IsNullOrEmpty(project))
			{
				if (PathHelper.FileExists(project) || PathHelper.DirectoryExists(project))
				{
					if (this.Solution() != null && this.Solution().DocumentReference.Path == project)
					{
						return;
					}
					if (this.ProjectManager().CloseSolution())
					{
						this.ProjectManager().OpenSolution(DocumentReference.Create(project), true, true);
					}
				}
				else
				{
					MessageBoxArgs messageBoxArg = new MessageBoxArgs();
					CultureInfo currentCulture = CultureInfo.CurrentCulture;
					string openRecentProjectNotFoundDialogMessage = StringTable.OpenRecentProjectNotFoundDialogMessage;
					object[] objArray = new object[] { project };
					messageBoxArg.Message = string.Format(currentCulture, openRecentProjectNotFoundDialogMessage, objArray);
					messageBoxArg.Button = MessageBoxButton.YesNo;
					messageBoxArg.Image = MessageBoxImage.Hand;
					if (base.Services.MessageDisplayService().ShowMessage(messageBoxArg) == MessageBoxResult.Yes)
					{
						((ProjectManager)this.ProjectManager()).RemoveRecentProject(project);
						return;
					}
				}
			}
		}

		public override object GetProperty(string propertyName)
		{
			if (propertyName != "Text")
			{
				if (propertyName == "IsVisible")
				{
					return this.HasValidProject;
				}
				return base.GetProperty(propertyName);
			}
			int num = this.recentIndex + 1;
			string str = PathHelper.CompressPathForDisplay(this.Project, 50);
			if (num == 10)
			{
				CultureInfo invariantCulture = CultureInfo.InvariantCulture;
				object[] objArray = new object[] { str };
				return string.Format(invariantCulture, "1_0 {0}", objArray);
			}
			CultureInfo cultureInfo = CultureInfo.InvariantCulture;
			object[] objArray1 = new object[] { num, str };
			return string.Format(cultureInfo, "_{0} {1}", objArray1);
		}
	}
}