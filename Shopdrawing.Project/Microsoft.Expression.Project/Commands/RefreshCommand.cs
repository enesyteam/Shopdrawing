using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Framework.Extensions.Enumerable;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.Build;
using Microsoft.Expression.Project.ServiceExtensions;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project.Commands
{
	internal sealed class RefreshCommand : ProjectCommand
	{
		public override string DisplayName
		{
			get
			{
				return StringTable.CommandRefreshName;
			}
		}

		public override bool IsAvailable
		{
			get
			{
				if (!base.IsAvailable)
				{
					return false;
				}
				return this.SelectedProjectOrNull() is IWebsiteProject;
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
				return this.Selection().CountIs<IDocumentItem>(1);
			}
		}

		public RefreshCommand(IServiceProvider serviceProvider) : base(serviceProvider)
		{
		}

		public override void Execute()
		{
			this.HandleBasicExceptions(() => {
				IWebsiteProject websiteProject = this.SelectedProjectOrNull() as IWebsiteProject;
				if (websiteProject != null)
				{
					IDocumentItem documentItem = this.Selection().First<IDocumentItem>();
					websiteProject.RefreshChildren(documentItem, true);
					SourceControlStatusCache.UpdateStatus(documentItem.Descendants.AppendItem<IDocumentItem>(documentItem), base.Services.SourceControlProvider());
				}
			});
		}
	}
}