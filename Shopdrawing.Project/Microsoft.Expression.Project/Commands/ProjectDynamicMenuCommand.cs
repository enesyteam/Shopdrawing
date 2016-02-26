using Microsoft.Expression.Framework.Commands;
using System;

namespace Microsoft.Expression.Project.Commands
{
	internal abstract class ProjectDynamicMenuCommand : DynamicMenuCommand, IProjectCommand, ICommand
	{
		private IServiceProvider serviceProvider;

		public abstract string DisplayName
		{
			get;
		}

		public IServiceProvider Services
		{
			get
			{
				return this.serviceProvider;
			}
		}

		public ProjectDynamicMenuCommand(IServiceProvider serviceProvider)
		{
			this.serviceProvider = serviceProvider;
		}
	}
}