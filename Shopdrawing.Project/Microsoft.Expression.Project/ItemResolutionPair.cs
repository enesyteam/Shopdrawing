using Microsoft.Build.Execution;
using System;

namespace Microsoft.Expression.Project
{
	internal class ItemResolutionPair : Tuple<IProjectItemData, ProjectItemInstance>
	{
		public ProjectItemInstance ResolvedItem
		{
			get
			{
				return base.Item2;
			}
		}

		public IProjectItemData SourceItem
		{
			get
			{
				return base.Item1;
			}
		}

		public ItemResolutionPair(IProjectItemData sourceItem, ProjectItemInstance resolvedItem) : base(sourceItem, resolvedItem)
		{
		}
	}
}