using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.Build;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project.Commands
{
	internal sealed class CloseProjectCommand : ProjectCommand
	{
		public override string DisplayName
		{
			get
			{
				return this.FormatWithSolutionTypeSubtext(StringTable.CommandCloseName);
			}
		}

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

		public CloseProjectCommand(IServiceProvider serviceProvider) : base(serviceProvider)
		{
		}

		public override void Execute()
		{
			if (this.IsEnabled)
			{
				this.HandleBasicExceptions(() => this.ProjectManager().CloseSolution());
			}
		}

		public override object GetProperty(string propertyName)
		{
			if (propertyName == "Text")
			{
				return this.FormatWithSolutionTypeSubtext(StringTable.MenuBarFileMenuCloseProject);
			}
			return base.GetProperty(propertyName);
		}
	}
}