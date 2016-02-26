using System;

namespace Microsoft.Expression.Project
{
	public interface IPrototypingProjectService
	{
		bool CanCutOrCopy(IProjectItem projectItem, bool isCut);

		bool CanDelete(IProjectItem projectItem);

		bool CanRemove(IProjectItem projectItem);

		bool CanRename(IProjectItem projectItem);
	}
}