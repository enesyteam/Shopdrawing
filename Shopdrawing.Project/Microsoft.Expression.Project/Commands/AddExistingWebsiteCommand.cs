using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.UserInterface;
using System;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project.Commands
{
	internal sealed class AddExistingWebsiteCommand : ProjectCommand
	{
		public override string DisplayName
		{
			get
			{
				return StringTable.CommandAddExistingWebsiteName;
			}
		}

		public override bool IsEnabled
		{
			get
			{
				ISolution solution = this.Solution();
				if (!base.IsEnabled || solution == null)
				{
					return false;
				}
				return solution.CanAddProjects;
			}
		}

		public AddExistingWebsiteCommand(IServiceProvider serviceProvider) : base(serviceProvider)
		{
		}

		public override void Execute()
		{
			this.HandleBasicExceptions(() => {
				string str = this.SelectProject();
				if (!string.IsNullOrEmpty(str))
				{
					this.ProjectManager().AddProject(FileSystemProjectStore.CreateInstance(DocumentReference.Create(str), base.Services));
					base.ActivateProjectPane();
				}
			});
		}

		private string SelectProject()
		{
			string defaultOpenProjectPath = this.ProjectManager().DefaultOpenProjectPath;
			NewNamePathDialog newNamePathDialog = null;
			if (!Microsoft.Expression.Framework.Documents.PathHelper.DirectoryExists(defaultOpenProjectPath))
			{
				defaultOpenProjectPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			}
			if (!Microsoft.Expression.Framework.Documents.PathHelper.DirectoryExists(defaultOpenProjectPath))
			{
				defaultOpenProjectPath = Path.GetPathRoot(Environment.CurrentDirectory);
			}
			newNamePathDialog = new NewNamePathDialog(this.DisplayName, defaultOpenProjectPath, string.Empty, StringTable.SelectWebsiteFolderDialogDescription, StringTable.SelectProjectFolderDialogDescriptionVista)
			{
				ShowDiscardButton = false,
				ShowName = false
			};
			newNamePathDialog.ShowDialog();
			if (newNamePathDialog.Result == ProjectDialog.ProjectDialogResult.Ok)
			{
				if (Microsoft.Expression.Framework.Documents.PathHelper.DirectoryExists(newNamePathDialog.NewPath))
				{
					return newNamePathDialog.NewPath;
				}
				CultureInfo currentCulture = CultureInfo.CurrentCulture;
				string directoryDoesNotExistErrorMessage = StringTable.DirectoryDoesNotExistErrorMessage;
				object[] newPath = new object[] { newNamePathDialog.NewPath };
				this.DisplayCommandFailedMessage(string.Format(currentCulture, directoryDoesNotExistErrorMessage, newPath));
			}
			return string.Empty;
		}
	}
}