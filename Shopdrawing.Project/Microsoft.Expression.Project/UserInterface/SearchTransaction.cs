using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Project;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.Project.UserInterface
{
	public class SearchTransaction
	{
		private string filterString;

		private string previousFilterString;

		private bool blockExpansionRecording;

		private Dictionary<HierarchicalNode, bool> expansionStates;

		public string FilterString
		{
			get
			{
				if (this.filterString == null)
				{
					return string.Empty;
				}
				return this.filterString;
			}
		}

		private IEnumerable<HierarchicalNode> HierarchicalNodes
		{
			get
			{
				return this.expansionStates.Keys;
			}
		}

		public SearchTransaction(string filter, HierarchicalNode root)
		{
			this.blockExpansionRecording = true;
			this.expansionStates = new Dictionary<HierarchicalNode, bool>();
			this.GetExpansionStatesForProjectItems(root.Children);
			this.UpdateFilterString(filter);
		}

		public void CollapseNode(HierarchicalNode node)
		{
			this.ToggleNode(node, false);
		}

		public void EnsureNodeTracked(HierarchicalNode node)
		{
			if (!this.expansionStates.ContainsKey(node))
			{
				this.expansionStates.Add(node, node.IsExpanded);
			}
		}

		private void ExpandEverything()
		{
			if (this.blockExpansionRecording)
			{
				foreach (HierarchicalNode hierarchicalNode in this.HierarchicalNodes)
				{
					hierarchicalNode.IsExpanded = true;
				}
				this.blockExpansionRecording = false;
			}
		}

		public void ExpandNode(HierarchicalNode node)
		{
			this.ToggleNode(node, true);
		}

		public void ExpressInterestInNode(HierarchicalNode node)
		{
			ProjectItemNode projectItemNode = node as ProjectItemNode;
			if (projectItemNode != null)
			{
				HierarchicalNode parent = projectItemNode.Parent;
				if (parent == null)
				{
					if (!projectItemNode.ProjectItem.IsReference)
					{
						parent = projectItemNode.ProjectNode;
					}
					else
					{
						parent = projectItemNode.ProjectNode.ReferencesFolder;
					}
				}
				this.ExpandNode(parent);
			}
		}

		private void GetExpansionStatesForProjectItems(IEnumerable<HierarchicalNode> items)
		{
			foreach (HierarchicalNode item in items)
			{
				this.expansionStates.Add(item, item.IsExpanded);
				this.GetExpansionStatesForProjectItems(item.Children);
			}
		}

		private void ReleaseFilter()
		{
			this.blockExpansionRecording = true;
			this.ResetVisibility(true);
			foreach (HierarchicalNode hierarchicalNode in this.HierarchicalNodes)
			{
				hierarchicalNode.IsExpanded = this.expansionStates[hierarchicalNode];
			}
			this.blockExpansionRecording = false;
		}

		private void ResetVisibility(bool resetAll)
		{
			if (this.previousFilterString == null)
			{
				this.previousFilterString = string.Empty;
			}
			bool flag = (resetAll ? true : this.previousFilterString.ToUpperInvariant().Contains(this.FilterString.ToUpperInvariant()));
			bool flag1 = (resetAll ? true : this.FilterString.ToUpperInvariant().Contains(this.previousFilterString.ToUpperInvariant()));
			foreach (HierarchicalNode hierarchicalNode in this.HierarchicalNodes)
			{
				ProjectItemNode projectItemNode = hierarchicalNode as ProjectItemNode;
				if (projectItemNode == null)
				{
					continue;
				}
				projectItemNode.ResetVisibilityFilter(flag, flag1);
			}
		}

		private void ToggleNode(HierarchicalNode node, bool isExpanded)
		{
			if (!this.blockExpansionRecording)
			{
				this.expansionStates[node] = isExpanded;
				ProjectItemNode projectItemNode = node as ProjectItemNode;
				if (projectItemNode != null)
				{
					if (projectItemNode.Parent != null)
					{
						this.ToggleNode(projectItemNode.Parent, true);
						return;
					}
					this.ToggleNode(projectItemNode.ProjectNode, true);
				}
			}
		}

		public void UpdateFilterString(string filter)
		{
			this.previousFilterString = this.filterString;
			this.filterString = filter;
			if (string.IsNullOrEmpty(filter))
			{
				this.ReleaseFilter();
				return;
			}
			this.ExpandEverything();
			this.UpdateProjectPane();
		}

		private void UpdateProjectPane()
		{
			this.ResetVisibility(false);
		}
	}
}