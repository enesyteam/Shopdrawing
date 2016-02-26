using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.Extensions.Enumerable;
using Microsoft.Expression.Framework.SourceControl;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.Build;
using Microsoft.Expression.Project.ServiceExtensions;
using Microsoft.Expression.Project.ServiceExtensions.Messaging;
using Microsoft.Expression.Project.ServiceExtensions.Selection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project.Commands
{
	internal sealed class DeleteProjectItemCommand : ProjectCommand
	{
		private bool AllSelectedItemsAreDeleteable
		{
			get
			{
				IPrototypingProjectService prototypingProjectService = base.Services.PrototypingProjectService();
				IEnumerable<IDocumentItem> documentItems = (
					from item in this.Selection()
					select new { item = item, projectItem = item as IProjectItem }).Where((argument0) => {
					if (argument0.projectItem == null || argument0.projectItem.IsVirtual || argument0.projectItem.IsLinkedFile)
					{
						return true;
					}
					if (prototypingProjectService == null)
					{
						return false;
					}
					return !prototypingProjectService.CanDelete(argument0.projectItem);
				}).Select((argument1) => argument1.item);
				return !documentItems.Any<IDocumentItem>();
			}
		}

		public override string DisplayName
		{
			get
			{
				return StringTable.CommandDeleteName;
			}
		}

		public override bool IsEnabled
		{
			get
			{
				if (!base.IsEnabled || BuildManager.Building || !this.Selection().Any<IDocumentItem>())
				{
					return false;
				}
				return this.AllSelectedItemsAreDeleteable;
			}
		}

		public DeleteProjectItemCommand(IServiceProvider serviceProvider) : base(serviceProvider)
		{
		}

		public override void Execute()
		{
			if (this.IsEnabled)
			{
				this.HandleBasicExceptions(new Action(this.InternalExecute));
			}
		}

		private bool FileHasPendingChangeThatIsNotAdd(IDocumentItem item)
		{
			SourceControlStatus cachedStatus = SourceControlStatusCache.GetCachedStatus(item);
			switch (cachedStatus)
			{
				case SourceControlStatus.None:
				case SourceControlStatus.Add:
				{
					return false;
				}
				default:
				{
					if (cachedStatus == SourceControlStatus.CheckedIn)
					{
						return false;
					}
					return true;
				}
			}
		}

		private void InternalExecute()
		{
			string str;
			string str1;
			IProjectItem[] array = this.Selection().OfType<IProjectItem>().Where<IProjectItem>((IProjectItem selectedItem) => {
				if (selectedItem.IsVirtual)
				{
					return false;
				}
				return !selectedItem.IsLinkedFile;
			}).ToArray<IProjectItem>();
			IProjectItem[] projectItemArray = array;
			IProjectItem[] projectItemArray1 = array;
			Func<IProjectItem, IEnumerable<IProjectItem>> func = (IProjectItem selectedItem) => selectedItem.Descendants.OfType<IProjectItem>();
			var collection = ((IEnumerable<IProjectItem>)projectItemArray1).SelectMany(func, (IProjectItem selectedItem, IProjectItem child) => new { selectedItem = selectedItem, child = child }).Where((argument0) => {
				if (argument0.child.IsVirtual)
				{
					return false;
				}
				return !argument0.child.IsLinkedFile;
			});
			IProjectItem[] array1 = ((IEnumerable<IProjectItem>)projectItemArray).Union<IProjectItem>(
				from <>h__TransparentIdentifier0 in collection
				select <>h__TransparentIdentifier0.child).ToArray<IProjectItem>();
			if ((int)array1.Length == 0)
			{
				return;
			}
			if (this.Solution().IsSourceControlActive)
			{
				if (((IEnumerable<IProjectItem>)array1).Any<IProjectItem>((IProjectItem item) => this.FileHasPendingChangeThatIsNotAdd(item)))
				{
					this.DisplayCommandFailedMessage(StringTable.DeletePendingChangesErrorMessage);
					return;
				}
			}
			IProjectItem[] projectItemArray2 = array1;
			Func<IProjectItem, IEnumerable<IProjectItem>> referencingProjectItems = (IProjectItem item) => item.ReferencingProjectItems;
			IEnumerable<string> strs = ((IEnumerable<IProjectItem>)projectItemArray2).SelectMany<IProjectItem, IProjectItem, string>(referencingProjectItems, (IProjectItem item, IProjectItem referencedItem) => referencedItem.DocumentReference.DisplayName).Distinct<string>();
			bool flag = ((IEnumerable<IProjectItem>)array1).Any<IProjectItem>((IProjectItem item) => item.IsReadOnly);
			if (strs.Any<string>())
			{
				if ((int)array.Length <= 1)
				{
					CultureInfo currentCulture = CultureInfo.CurrentCulture;
					str1 = (flag ? StringTable.ProjectItemDeleteConfirmationMessageReadOnlySingularInUse : StringTable.ProjectItemDeleteConfirmationMessageSingularInUse);
					object[] objArray = new object[] { string.Join(", ", strs.ToArray<string>()) };
					str = string.Format(currentCulture, str1, objArray);
				}
				else
				{
					str = (flag ? StringTable.ProjectItemDeleteConfirmationMessageReadOnlyPluralSomeInUse : StringTable.ProjectItemDeleteConfirmationMessagePluralInUse);
				}
			}
			else if ((int)array.Length <= 1)
			{
				str = (flag ? StringTable.ProjectItemDeleteConfirmationMessageReadOnlySingular : StringTable.ProjectItemDeleteConfirmationMessageSingular);
			}
			else
			{
				str = (flag ? StringTable.ProjectItemDeleteConfirmationMessageReadOnlyPlural : StringTable.ProjectItemDeleteConfirmationMessagePlural);
			}
			if (base.Services.PromptUserYesNo(str))
			{
				IEnumerable<IGrouping<IProject, IProjectItem>> project = 
					from selectedItem in (IEnumerable<IProjectItem>)array1
					group selectedItem by selectedItem.Project;
				foreach (IGrouping<IProject, IProjectItem> projects in project)
				{
					IProject key = projects.Key;
					key.RemoveItems(true, projects.ToArray<IProjectItem>());
				}
				if (this.Selection().CountIs<IDocumentItem>(0))
				{
					base.Services.SetSelection(project.First<IGrouping<IProject, IProjectItem>>().Key);
				}
			}
		}
	}
}