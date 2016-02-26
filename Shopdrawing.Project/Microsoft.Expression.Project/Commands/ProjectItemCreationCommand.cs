using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Framework.Extensions.Enumerable;
using Microsoft.Expression.Project.Build;
using Microsoft.Expression.Project.ServiceExtensions;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project.Commands
{
	public abstract class ProjectItemCreationCommand : ProjectCommand
	{
		private bool skipProjectPaneActivation;

		public override bool IsEnabled
		{
			get
			{
				if (!base.IsEnabled || !this.SelectedProjects().CountIs<IProject>(1))
				{
					return false;
				}
				return !BuildManager.Building;
			}
		}

		protected ProjectItemCreationCommand(IServiceProvider serviceProvider) : base(serviceProvider)
		{
		}

		protected abstract bool CreateProjectItem();

		public sealed override void Execute()
		{
			base.Services.SuspendNotificationsWhile(() => this.HandleBasicExceptions(() => {
				if (this.CreateProjectItem())
				{
					if (!this.skipProjectPaneActivation)
					{
						base.ActivateProjectPane();
					}
					this.skipProjectPaneActivation = false;
				}
			}));
		}

		public override object GetProperty(string propertyName)
		{
			if (propertyName == "TemporarilyStopProjectPaneActivation")
			{
				return this.skipProjectPaneActivation;
			}
			return base.GetProperty(propertyName);
		}

		public override void SetProperty(string propertyName, object propertyValue)
		{
			base.SetProperty(propertyName, propertyValue);
			if (propertyName == "TemporarilyStopProjectPaneActivation" && propertyValue is bool)
			{
				this.skipProjectPaneActivation = (bool)propertyValue;
			}
		}
	}
}