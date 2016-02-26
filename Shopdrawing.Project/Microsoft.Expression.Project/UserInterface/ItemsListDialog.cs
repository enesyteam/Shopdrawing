using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Documents;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace Microsoft.Expression.Project.UserInterface
{
	public class ItemsListDialog : ItemListBaseDialog
	{
		private string message;

		private string automationId;

		private List<BasicItemModel> items;

		public override string AcceptText
		{
			get
			{
				return StringTable.ItemsListYesButton;
			}
		}

		public override string AutomationId
		{
			get
			{
				return this.automationId;
			}
		}

		public override string CancelText
		{
			get
			{
				return StringTable.CancelButtonText;
			}
		}

		public override string DiscardText
		{
			get
			{
				return StringTable.DiscardButtonNoText;
			}
		}

		public override object FileList
		{
			get
			{
				ICollectionView defaultView = CollectionViewSource.GetDefaultView(this.items);
				defaultView.SortDescriptions.Add(new SortDescription("IsHeaderItem", ListSortDirection.Descending));
				defaultView.SortDescriptions.Add(new SortDescription("DisplayName", ListSortDirection.Ascending));
				defaultView.Refresh();
				return defaultView;
			}
		}

		public override string Message
		{
			get
			{
				return this.message;
			}
		}

		public ItemsListDialog(IServiceProvider serviceProvider, string message, string automationId, string checkBoxMessage, IEnumerable<DocumentReference> items) : base(serviceProvider, checkBoxMessage)
		{
			this.message = message;
			this.automationId = automationId;
			this.items = new List<BasicItemModel>();
			foreach (DocumentReference item in items)
			{
				this.items.Add(new BasicItemModel(item));
			}
		}

		public override void InitializeDialog()
		{
			base.InitializeDialog();
			((FrameworkElement)base.DialogContent).Width = 350;
		}
	}
}