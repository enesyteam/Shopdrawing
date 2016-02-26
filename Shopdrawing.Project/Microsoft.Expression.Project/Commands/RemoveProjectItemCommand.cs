using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.Extensions.Enumerable;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.Build;
using Microsoft.Expression.Project.ServiceExtensions;
using Microsoft.Expression.Project.ServiceExtensions.Messaging;
using Microsoft.Expression.Project.ServiceExtensions.Selection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project.Commands
{
	internal sealed class RemoveProjectItemCommand : ProjectCommand
	{
		private bool AllSelectedItemsAreRemoveable
		{
			get
			{
				IPrototypingProjectService prototypingProjectService = base.Services.PrototypingProjectService();
				IEnumerable<IDocumentItem> documentItems = (
					from item in this.Selection().OfType<IProjectItem>()
					select new { item = item, projectItem = item }).Where((argument0) => {
					if (argument0.projectItem == null || argument0.projectItem.IsVirtual || argument0.projectItem.Project is WebsiteProject)
					{
						return true;
					}
					if (prototypingProjectService == null)
					{
						return false;
					}
					return !prototypingProjectService.CanRemove(argument0.projectItem);
				}).Select((argument1) => argument1.item);
				return !documentItems.Any<IDocumentItem>();
			}
		}

		public override string DisplayName
		{
			get
			{
				return StringTable.CommandRemoveFromProjectName;
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
				if (!base.IsEnabled || BuildManager.Building || !this.Selection().Any<IDocumentItem>())
				{
					return false;
				}
				return this.AllSelectedItemsAreRemoveable;
			}
		}

		public RemoveProjectItemCommand(IServiceProvider serviceProvider) : base(serviceProvider)
		{
		}

		public override void Execute()
		{
			string projectItemRemoveConfirmationMessageSingular;
			if (!this.IsEnabled)
			{
				return;
			}
			IProjectItem[] array = this.Selection().OfType<IProjectItem>().ToArray<IProjectItem>();
			IProjectItem[] projectItemArray = array;
			Func<IProjectItem, IEnumerable<IProjectItem>> referencingProjectItems = (IProjectItem selectedItem) => selectedItem.ReferencingProjectItems;
			IEnumerable<string> strs = ((IEnumerable<IProjectItem>)projectItemArray).SelectMany<IProjectItem, IProjectItem, string>(referencingProjectItems, (IProjectItem selectedItem, IProjectItem referencingItem) => referencingItem.DocumentReference.DisplayName).Distinct<string>();
			if ((int)array.Length <= 1)
			{
				string str = string.Join(", ", strs);
				if (!string.IsNullOrEmpty(str))
				{
					CultureInfo currentCulture = CultureInfo.CurrentCulture;
					string projectItemRemoveConfirmationMessageSingularInUse = StringTable.ProjectItemRemoveConfirmationMessageSingularInUse;
					object[] fileName = new object[] { Path.GetFileName(array[0].DocumentReference.Path), str };
					projectItemRemoveConfirmationMessageSingular = string.Format(currentCulture, projectItemRemoveConfirmationMessageSingularInUse, fileName);
				}
				else
				{
					projectItemRemoveConfirmationMessageSingular = StringTable.ProjectItemRemoveConfirmationMessageSingular;
				}
			}
			else
			{
				projectItemRemoveConfirmationMessageSingular = (!strs.Any<string>() ? StringTable.ProjectItemRemoveConfirmationMessagePlural : StringTable.ProjectItemRemoveConfirmationMessagePluralInUse);
			}
			if (base.Services.PromptUserYesNo(projectItemRemoveConfirmationMessageSingular))
			{
				IEnumerable<IGrouping<IProject, IProjectItem>> isVirtual = 
					from selectedItem in (IEnumerable<IProjectItem>)array
					where !selectedItem.IsVirtual
					group selectedItem by selectedItem.Project;
				foreach (IGrouping<IProject, IProjectItem> projects in isVirtual)
				{
					IProject key = projects.Key;
					key.RemoveItems(false, projects.ToArray<IProjectItem>());
				}
				if (this.Selection().CountIs<IDocumentItem>(0))
				{
					base.Services.SetSelection(isVirtual.First<IGrouping<IProject, IProjectItem>>().Key);
				}
			}
		}
	}
}