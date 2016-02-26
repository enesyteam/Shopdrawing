using Microsoft.Expression.Framework.SourceControl;
using Microsoft.Expression.Project;
using System;

namespace Microsoft.Expression.Project.Commands
{
	internal class GoOnlineCommand : SourceControlCommand
	{
		public override string DisplayName
		{
			get
			{
				return StringTable.CommandGoOnlineName;
			}
		}

		public override bool IsAvailable
		{
			get
			{
				if (this.Solution() == null || !this.Solution().IsUnderSourceControl || base.SourceControlProvider == null)
				{
					return false;
				}
				return base.SourceControlProvider.GetOnlineStatus() != SourceControlOnlineStatus.Online;
			}
		}

		public GoOnlineCommand(IServiceProvider serviceProvider) : base(serviceProvider)
		{
		}

		protected override void InternalExectute()
		{
			base.SourceControlProvider.SetOnlineStatus(SourceControlOnlineStatus.Online);
		}
	}
}