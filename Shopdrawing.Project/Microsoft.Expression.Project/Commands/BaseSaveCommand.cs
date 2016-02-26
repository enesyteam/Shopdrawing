using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Project.Build;
using System;

namespace Microsoft.Expression.Project.Commands
{
	internal abstract class BaseSaveCommand : ProjectCommand
	{
		public override bool IsEnabled
		{
			get
			{
				if (!base.IsEnabled || BuildManager.Building)
				{
					return false;
				}
				return this.Solution() != null;
			}
		}

		public BaseSaveCommand(IServiceProvider serviceProvider) : base(serviceProvider)
		{
		}
	}
}