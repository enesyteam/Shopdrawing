using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.ServiceExtensions.View;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project.Commands
{
	internal sealed class InsertIntoActiveDocumentCommand : ItemCollectionCommand
	{
		public override string DisplayName
		{
			get
			{
				return StringTable.CommandInsertName;
			}
		}

		public InsertIntoActiveDocumentCommand(IServiceProvider serviceProvider) : base(serviceProvider)
		{
		}

		protected override void Execute(IDocumentItem item)
		{
			if (this.IsEnabled)
			{
				this.HandleBasicExceptions(() => {
					PerformanceUtility.StartPerformanceSequence(PerformanceEvent.InsertIntoActiveDocument);
					IView view = this.Services.ActiveView();
					if (view != null)
					{
						IProjectItem projectItem = item as IProjectItem;
						if (projectItem != null)
						{
							projectItem.DocumentType.AddToDocument(projectItem, view);
						}
					}
					PerformanceUtility.EndPerformanceSequence(PerformanceEvent.InsertIntoActiveDocument);
				});
			}
		}

		protected override bool ShouldAddItem(IDocumentItem item)
		{
			bool flag = false;
			IView view = base.Services.ActiveView();
			if (view != null)
			{
				IProjectItem projectItem = item as IProjectItem;
				flag = (projectItem == null ? false : projectItem.DocumentType.CanInsertTo(projectItem, view));
			}
			return flag;
		}
	}
}