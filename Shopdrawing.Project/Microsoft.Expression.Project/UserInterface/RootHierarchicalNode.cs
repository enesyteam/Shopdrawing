using System;

namespace Microsoft.Expression.Project.UserInterface
{
	public class RootHierarchicalNode : HierarchicalNode
	{
		public override string DisplayName
		{
			get
			{
				return string.Empty;
			}
			set
			{
			}
		}

		public RootHierarchicalNode(Microsoft.Expression.Project.UserInterface.ProjectPane projectPane) : base(null, projectPane)
		{
		}

		public override int CompareTo(HierarchicalNode treeItem)
		{
			return 0;
		}

		protected override void ExpandAllParents()
		{
			throw new NotImplementedException();
		}

		protected override void OnCreateContextMenuCommand()
		{
			throw new NotImplementedException();
		}
	}
}