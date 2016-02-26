using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.ServiceExtensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Microsoft.Expression.Project.UserInterface
{
	internal sealed class RenameProjectItemCommand : Command
	{
		private ProjectPane projectPane;

		private ProjectItemNode ActiveProjectItemNode
		{
			get
			{
				ProjectItemNode projectItemNode;
				if (this.projectPane.Services.ProjectManager().ItemSelectionSet.Count != 1)
				{
					return null;
				}
				IProjectItem projectItem = this.projectPane.Services.ProjectManager().ItemSelectionSet.Selection.First<IDocumentItem>() as IProjectItem;
				if (projectItem != null)
				{
					using (IEnumerator<HierarchicalNode> enumerator = this.projectPane.Projects.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							ProjectNode current = (ProjectNode)enumerator.Current;
							if (current.Project != projectItem.Project)
							{
								continue;
							}
							projectItemNode = current.FindItemNode(projectItem);
							return projectItemNode;
						}
						return null;
					}
					return projectItemNode;
				}
				return null;
			}
		}

		public override bool IsEnabled
		{
			get
			{
				bool flag = false;
				if (this.ActiveProjectItemNode != null && this.ActiveProjectItemNode.CanBeRenamed)
				{
					flag = true;
				}
				return flag;
			}
		}

		public RenameProjectItemCommand(ProjectPane projectPane)
		{
			this.projectPane = projectPane;
		}

		public override void Execute()
		{
			ProjectItemNode activeProjectItemNode = this.ActiveProjectItemNode;
			if (this.IsEnabled)
			{
				activeProjectItemNode.BeginRename();
			}
		}
	}
}