using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Project.ServiceExtensions.View;
using System;
using System.Collections;

namespace Microsoft.Expression.Project.Commands
{
	internal sealed class CloseAllDocumentsCommand : ProjectCommand
	{
		private bool closeActiveDocument;

		public override string DisplayName
		{
			get
			{
				return StringTable.CommandCloseAllDocumentsName;
			}
		}

		public override bool IsEnabled
		{
			get
			{
				if (this.closeActiveDocument)
				{
					if (!base.IsEnabled)
					{
						return false;
					}
					return base.Services.Views().Count > 0;
				}
				if (!base.IsEnabled)
				{
					return false;
				}
				return base.Services.Views().Count > 1;
			}
		}

		public CloseAllDocumentsCommand(IServiceProvider serviceProvider, bool closeActiveDocument) : base(serviceProvider)
		{
			this.closeActiveDocument = closeActiveDocument;
		}

		public override void Execute()
		{
			if (this.IsEnabled && this.SaveSolution(true, this.closeActiveDocument))
			{
				base.Services.CloseAllViews(this.closeActiveDocument);
			}
		}
	}
}