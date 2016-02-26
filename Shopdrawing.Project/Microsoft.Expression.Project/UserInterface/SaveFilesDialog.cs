using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.ServiceExtensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace Microsoft.Expression.Project.UserInterface
{
	internal class SaveFilesDialog : ItemListBaseDialog
	{
		private List<DirtyProjectItem> dirtyProjectItems = new List<DirtyProjectItem>();

		public override string AcceptText
		{
			get
			{
				return StringTable.SaveFilesDialogAcceptText;
			}
		}

		public override string AutomationId
		{
			get
			{
				return "SaveFilesDialog";
			}
		}

		public override string CancelText
		{
			get
			{
				return StringTable.SaveFilesDialogCancelText;
			}
		}

		public override string DiscardText
		{
			get
			{
				return StringTable.SaveFilesDialogDiscardText;
			}
		}

		public override object FileList
		{
			get
			{
				ICollectionView defaultView = CollectionViewSource.GetDefaultView(this.dirtyProjectItems);
				defaultView.SortDescriptions.Add(new SortDescription("OwningProject", ListSortDirection.Ascending));
				defaultView.SortDescriptions.Add(new SortDescription("IsHeaderItem", ListSortDirection.Descending));
				defaultView.SortDescriptions.Add(new SortDescription("DisplayName", ListSortDirection.Ascending));
				defaultView.Refresh();
				return defaultView;
			}
		}

		public bool IsSolutionOpen
		{
			get
			{
				return base.Services.ProjectManager().CurrentSolution != null;
			}
		}

		public override string Message
		{
			get
			{
				return StringTable.SaveFilesDialogMessageText;
			}
		}

		public SaveFilesDialog(IServiceProvider serviceProvider, IEnumerable<IProjectItem> dirtyProjectItems) : base(serviceProvider)
		{
			List<IProject> projects = new List<IProject>();
			foreach (IProjectItem dirtyProjectItem in dirtyProjectItems)
			{
				DirtyProjectItem dirtyProjectItem1 = new DirtyProjectItem(dirtyProjectItem.Project.DocumentReference.GetRelativePath(dirtyProjectItem.DocumentReference), dirtyProjectItem.Project.DocumentReference.DisplayName, false);
				this.dirtyProjectItems.Add(dirtyProjectItem1);
				if (projects.Contains(dirtyProjectItem.Project))
				{
					continue;
				}
				projects.Add(dirtyProjectItem.Project);
			}
			foreach (INamedProject project in projects)
			{
				DirtyProjectItem dirtyProjectItem2 = new DirtyProjectItem(project.DocumentReference.DisplayName, project.DocumentReference.DisplayName, true);
				this.dirtyProjectItems.Add(dirtyProjectItem2);
			}
		}
	}
}