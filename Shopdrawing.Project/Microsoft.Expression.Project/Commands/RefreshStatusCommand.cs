using Microsoft.Expression.Project;
using System;

namespace Microsoft.Expression.Project.Commands
{
	internal class RefreshStatusCommand : SourceControlCommand
	{
		public override string DisplayName
		{
			get
			{
				return StringTable.CommandRefreshStatusName;
			}
		}

		public RefreshStatusCommand(IServiceProvider serviceProvider) : base(serviceProvider)
		{
		}

		protected override void InternalExectute()
		{
			SourceControlStatusCache.UpdateStatus(base.GetFileItemAndDescendants(this.Solution()), base.SourceControlProvider);
		}
	}
}