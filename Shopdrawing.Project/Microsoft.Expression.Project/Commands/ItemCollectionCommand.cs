using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Project;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Expression.Project.Commands
{
	internal abstract class ItemCollectionCommand : ProjectCommand
	{
		private IDocumentItem targetItem;

		public override bool IsEnabled
		{
			get
			{
				if (!base.IsEnabled)
				{
					return false;
				}
				return this.GetItems().FirstOrDefault<IDocumentItem>() != null;
			}
		}

		public ItemCollectionCommand(IServiceProvider serviceProvider) : base(serviceProvider)
		{
		}

		public override void Execute()
		{
			if (this.targetItem != null)
			{
				this.Execute(this.targetItem);
				this.targetItem = null;
				return;
			}
			foreach (IDocumentItem item in this.GetItems())
			{
				this.Execute(item);
			}
		}

		protected abstract void Execute(IDocumentItem item);

		protected IEnumerable<IDocumentItem> GetItems()
		{
			IEnumerable<IDocumentItem> documentItems = this.Selection();
			ItemCollectionCommand itemCollectionCommand = this;
			if (documentItems.All<IDocumentItem>(new Func<IDocumentItem, bool>(itemCollectionCommand.ShouldAddItem)))
			{
				return documentItems;
			}
			return Enumerable.Empty<IDocumentItem>();
		}

		public override void SetProperty(string propertyName, object propertyValue)
		{
			if (propertyName == "TargetDocument")
			{
				this.targetItem = propertyValue as IDocumentItem;
				return;
			}
			base.SetProperty(propertyName, propertyValue);
		}

		protected abstract bool ShouldAddItem(IDocumentItem item);
	}
}