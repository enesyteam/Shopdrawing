using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.UserInterface;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.UserInterface;
using System;
using System.Globalization;
using System.IO;

namespace Microsoft.Expression.Project.Commands
{
	internal sealed class SaveSolutionCopyCommand : BaseSaveCommand
	{
		public override string DisplayName
		{
			get
			{
				return this.FormatWithSolutionTypeSubtext(StringTable.CommandSaveCopyName);
			}
		}

		public override bool IsEnabled
		{
			get
			{
				if (!base.IsEnabled)
				{
					return false;
				}
				return this.Solution() != null;
			}
		}

		public SaveSolutionCopyCommand(IServiceProvider serviceProvider) : base(serviceProvider)
		{
		}

		public override void Execute()
		{
			this.HandleBasicExceptions(new Action(this.InternalExecute));
		}

		public override object GetProperty(string propertyName)
		{
			if (propertyName == "Text")
			{
				return this.FormatWithSolutionTypeSubtext(StringTable.MenuBarFileMenuSaveSolutionCopy);
			}
			return base.GetProperty(propertyName);
		}

		private void InternalExecute()
		{
			ISolutionManagement solutionManagement = this.Solution() as ISolutionManagement;
			if (solutionManagement == null)
			{
				return;
			}
			if (!this.SaveSolution(true))
			{
				return;
			}
			string parentDirectory = Microsoft.Expression.Framework.Documents.PathHelper.GetParentDirectory(Microsoft.Expression.Framework.Documents.PathHelper.GetDirectory(solutionManagement.DocumentReference.Path));
			CultureInfo currentCulture = CultureInfo.CurrentCulture;
			string newNameCopyTemplate = StringTable.NewNameCopyTemplate;
			object[] fileNameWithoutExtension = new object[] { Path.GetFileNameWithoutExtension(solutionManagement.DocumentReference.DisplayName) };
			string availableFileOrDirectoryName = string.Format(currentCulture, newNameCopyTemplate, fileNameWithoutExtension);
			availableFileOrDirectoryName = Microsoft.Expression.Framework.Documents.PathHelper.GetAvailableFileOrDirectoryName(availableFileOrDirectoryName, null, parentDirectory, true);
			if (base.PromptUserForNewNameAndPath(ref parentDirectory, ref availableFileOrDirectoryName, false) == ProjectDialog.ProjectDialogResult.Ok)
			{
				using (IDisposable disposable = TemporaryCursor.SetWaitCursor())
				{
					Directory.CreateDirectory(parentDirectory);
					solutionManagement.SaveCopy(parentDirectory, availableFileOrDirectoryName);
				}
			}
		}
	}
}