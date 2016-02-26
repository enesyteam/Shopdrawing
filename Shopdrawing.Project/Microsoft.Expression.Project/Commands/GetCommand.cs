using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.Extensions.Enumerable;
using Microsoft.Expression.Framework.SourceControl;
using Microsoft.Expression.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project.Commands
{
	internal class GetCommand : SourceControlCommand
	{
		private bool targetSelection;

		private SourceControlGetOption typeOfGet;

		public override string DisplayName
		{
			get
			{
				if (this.typeOfGet == SourceControlGetOption.UserSpecifiedVersion)
				{
					return StringTable.CommandGetSpecificVersionName;
				}
				return StringTable.CommandGetLatestVersionName;
			}
		}

		public override bool IsAvailable
		{
			get
			{
				if (!this.targetSelection)
				{
					return true;
				}
				IDocumentItem documentItem = this.Selection().SingleOrNull<IDocumentItem>();
				if (!base.IsAvailable || documentItem == null)
				{
					return false;
				}
				return base.GetFileItemAndDescendants(documentItem).Any<IDocumentItem>(new Func<IDocumentItem, bool>(this.IsValidStatusForGet));
			}
		}

		public GetCommand(IServiceProvider serviceProvider, bool targetSelection, SourceControlGetOption typeOfGet) : base(serviceProvider)
		{
			this.targetSelection = targetSelection;
			this.typeOfGet = typeOfGet;
		}

		protected override void InternalExectute()
		{
			HashSet<IDocumentItem> documentItems = new HashSet<IDocumentItem>(new DocumentItemUrlComparer<IDocumentItem>());
			HashSet<IWebsiteProject> websiteProjects = new HashSet<IWebsiteProject>(new DocumentItemUrlComparer<IWebsiteProject>());
			List<IDocumentItem> list = null;
			List<IWebsiteProject> list1 = null;
			IEnumerable<IWebsiteProject> websiteProjects1 = null;
			IDocumentItem documentItem = null;
			string empty = string.Empty;
			int num = 0;
			Func<IDocumentItem, IEnumerable<IDocumentItem>> array = null;
			if (this.targetSelection)
			{
				documentItem = this.Selection().SingleOrNull<IDocumentItem>();
			}
			else
			{
				documentItem = this.Solution();
			}
			if (documentItem == null)
			{
				return;
			}
			if (!this.SaveSolution(true))
			{
				return;
			}
			ISolutionManagement solutionManagement = (ISolutionManagement)this.Solution();
			if (documentItem != this.Solution())
			{
				array = (IDocumentItem IDocumentItem) => base.GetFileItemAndDescendants(documentItem).ToArray<IDocumentItem>();
				if (documentItem is IWebsiteProject)
				{
					websiteProjects1 = new IWebsiteProject[] { (IWebsiteProject)documentItem };
				}
			}
			else
			{
				array = (IDocumentItem IDocumentItem) => solutionManagement.AllProjects.SelectMany<INamedProject, IDocumentItem>((INamedProject project) => base.GetFileItemAndDescendants(project).AppendItem<IDocumentItem>(this.Solution()));
				websiteProjects1 = solutionManagement.AllProjects.OfType<IWebsiteProject>();
			}
			using (SolutionBase.DisableReloadPromptToken disableReloadPromptToken = new SolutionBase.DisableReloadPromptToken())
			{
				do
				{
					if (websiteProjects1 == null)
					{
						list1 = null;
					}
					else
					{
						list1 = websiteProjects1.Except<IWebsiteProject>(websiteProjects, new DocumentItemUrlComparer<IWebsiteProject>()).ToList<IWebsiteProject>();
						if (list1 != null && list1.Count<IWebsiteProject>() > 0)
						{
							base.SourceControlProvider.GetFiles((
								from item in list1
								select item.DocumentReference.Path).ToArray<string>(), ref empty, (SourceControlGetOption)((int)((num == 0 ? this.typeOfGet : this.typeOfGet & (SourceControlGetOption.All | SourceControlGetOption.Latest | SourceControlGetOption.Recursive))) | 256));
							foreach (IWebsiteProject websiteProject in list1)
							{
								websiteProject.RefreshChildren(websiteProject, true);
								websiteProjects.Add(websiteProject);
							}
							num = websiteProjects.Count<IWebsiteProject>() + documentItems.Count<IDocumentItem>();
						}
					}
					list = array(documentItem).Except<IDocumentItem>(documentItems, new DocumentItemUrlComparer<IDocumentItem>()).ToList<IDocumentItem>();
					if (list != null && list.Count<IDocumentItem>() > 0)
					{
						base.SourceControlProvider.GetFiles((
							from item in list
							select item.DocumentReference.Path).ToArray<string>(), ref empty, (num == 0 ? this.typeOfGet : this.typeOfGet & (SourceControlGetOption.All | SourceControlGetOption.Latest | SourceControlGetOption.Recursive)));
						SourceControlStatusCache.UpdateStatus(list, base.SourceControlProvider);
					}
					documentItems.UnionWith(list);
					num = websiteProjects.Count<IWebsiteProject>() + documentItems.Count<IDocumentItem>();
					try
					{
						solutionManagement.ReactivateWatchers();
					}
					finally
					{
						solutionManagement.DeactivateWatchers();
					}
				}
				while (list.Any<IDocumentItem>() || list1 != null && list1.Any<IWebsiteProject>());
			}
		}

		private bool IsValidStatusForGet(IDocumentItem documentItem)
		{
			SourceControlStatus cachedStatus = SourceControlStatusCache.GetCachedStatus(documentItem);
			if (cachedStatus == SourceControlStatus.None)
			{
				return false;
			}
			return cachedStatus != SourceControlStatus.Add;
		}
	}
}