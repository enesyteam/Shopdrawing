using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.Extensions.Enumerable;
using Microsoft.Expression.Framework.SourceControl;
using Microsoft.Expression.Framework.UserInterface;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.ServiceExtensions;
using Microsoft.Expression.Project.UserInterface;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project.Commands
{
	public abstract class ProjectCommand : Command, IProjectCommand, ICommand
	{
		private IServiceProvider serviceProvider;

		public abstract string DisplayName
		{
			get;
		}

		public IServiceProvider Services
		{
			get
			{
				return this.serviceProvider;
			}
		}

		protected ProjectCommand(IServiceProvider serviceProvider)
		{
			this.serviceProvider = serviceProvider;
		}

		protected void ActivateProjectPane()
		{
			ProjectCommand.ActivateProjectPane(this.Services.WindowService());
		}

		public static void ActivateProjectPane(IWindowService windowService)
		{
			if (windowService != null && windowService.PaletteRegistry != null)
			{
				PaletteRegistryEntry item = windowService.PaletteRegistry["Designer_ProjectPane"];
				if (item != null)
				{
					item.IsVisibleAndSelected = true;
				}
			}
		}

		protected bool FileHasPendingChange(IDocumentItem item)
		{
			SourceControlStatus cachedStatus = SourceControlStatusCache.GetCachedStatus(item);
			if (cachedStatus == SourceControlStatus.CheckedIn)
			{
				return false;
			}
			return cachedStatus != SourceControlStatus.None;
		}

		protected IEnumerable<IDocumentItem> GetFileItemAndDescendants(IDocumentItem item)
		{
			IEnumerable<IDocumentItem> documentItems = item.Descendants.Where<IDocumentItem>((IDocumentItem descendant) => {
				if (descendant.IsDirectory || descendant.IsReference || descendant.IsVirtual)
				{
					return false;
				}
				return Microsoft.Expression.Framework.Documents.PathHelper.IsValidPath(descendant.DocumentReference.Path);
			});
			if (!item.IsDirectory && !item.IsReference && !item.IsVirtual && Microsoft.Expression.Framework.Documents.PathHelper.IsValidPath(item.DocumentReference.Path))
			{
				documentItems = documentItems.AppendItem<IDocumentItem>(item);
			}
			return documentItems;
		}

		protected ProjectDialog.ProjectDialogResult PromptUserForNewNameAndPath(ref string path, ref string name, bool allowDiscard)
		{
			NewNamePathDialog newNamePathDialog = null;
			do
			{
				newNamePathDialog = new NewNamePathDialog(this.DisplayName, path, name, StringTable.SelectProjectFolderDialogDescription, StringTable.SelectProjectFolderDialogDescriptionVista)
				{
					ShowDiscardButton = allowDiscard
				};
				newNamePathDialog.ShowDialog();
			}
			while (newNamePathDialog.Result == ProjectDialog.ProjectDialogResult.Ok && !this.ValidatePathIsNew(Path.Combine(newNamePathDialog.NewPath, newNamePathDialog.NewName)));
			if (newNamePathDialog.Result == ProjectDialog.ProjectDialogResult.Ok)
			{
				path = newNamePathDialog.NewPath;
				name = newNamePathDialog.NewName;
			}
			return newNamePathDialog.Result;
		}

		protected void UpdateSourceControl(IEnumerable<INamedProject> projects)
		{
			if (this.Solution().IsUnderSourceControl)
			{
				foreach (IProject project in projects.OfType<IProject>())
				{
					project.SetCapability<bool>("SourceControlBound", true);
					if (!this.Solution().IsSourceControlActive)
					{
						continue;
					}
					IEnumerable<IDocumentItem> list = this.GetFileItemAndDescendants(project).ToList<IDocumentItem>();
					SourceControlStatusCache.UpdateStatus(list, this.Services.SourceControlProvider());
					IEnumerable<IDocumentItem> documentItems = (
						from item in list
						where SourceControlStatusCache.GetCachedStatus(item) == SourceControlStatus.None
						select item).ToList<IDocumentItem>();
					this.Services.SourceControlProvider().Add((
						from item in documentItems
						select item.DocumentReference.Path).ToArray<string>());
					SourceControlStatusCache.UpdateStatus(documentItems, this.Services.SourceControlProvider());
				}
			}
		}

		private bool ValidatePathIsNew(string fullPath)
		{
			string directoryExistsErrorMessage = null;
			if (Microsoft.Expression.Framework.Documents.PathHelper.DirectoryExists(fullPath))
			{
				directoryExistsErrorMessage = StringTable.DirectoryExistsErrorMessage;
			}
			else if (Microsoft.Expression.Framework.Documents.PathHelper.FileExists(fullPath))
			{
				directoryExistsErrorMessage = StringTable.FileExistsErrorMessage;
			}
			if (directoryExistsErrorMessage == null)
			{
				return true;
			}
			CultureInfo currentCulture = CultureInfo.CurrentCulture;
			object[] objArray = new object[] { fullPath };
			this.DisplayCommandFailedMessage(string.Format(currentCulture, directoryExistsErrorMessage, objArray));
			return false;
		}
	}
}