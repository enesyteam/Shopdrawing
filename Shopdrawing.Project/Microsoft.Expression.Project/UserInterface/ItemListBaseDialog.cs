using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.ServiceExtensions;
using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Expression.Project.UserInterface
{
	public abstract class ItemListBaseDialog : ProjectDialog
	{
		public abstract string AcceptText
		{
			get;
		}

		public abstract string AutomationId
		{
			get;
		}

		public abstract string CancelText
		{
			get;
		}

		public bool CheckBoxEnabled
		{
			get;
			private set;
		}

		public string CheckBoxMessage
		{
			get;
			private set;
		}

		public bool CheckBoxResult
		{
			get;
			set;
		}

		public abstract string DiscardText
		{
			get;
		}

		public abstract object FileList
		{
			get;
		}

		public abstract string Message
		{
			get;
		}

		protected IServiceProvider Services
		{
			get;
			private set;
		}

		protected ItemListBaseDialog(IServiceProvider serviceProvider) : this(serviceProvider, null)
		{
		}

		protected ItemListBaseDialog(IServiceProvider serviceProvider, string checkBoxMessage)
		{
			this.Services = serviceProvider;
			if (!string.IsNullOrEmpty(checkBoxMessage))
			{
				this.CheckBoxEnabled = true;
				this.CheckBoxMessage = checkBoxMessage;
			}
		}

		public virtual void InitializeDialog()
		{
			base.DialogContent = Microsoft.Expression.Project.FileTable.GetElement("Resources\\ItemsListBaseDialog.xaml");
			base.Title = this.Services.ExpressionInformationService().DefaultDialogTitle;
			base.SetValue(AutomationElement.IdProperty, this.AutomationId);
			base.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
			Button button = (Button)LogicalTreeHelper.FindLogicalNode(this, "DiscardButton");
			if (button != null)
			{
				button.Click += new RoutedEventHandler(this.OnNoCommand);
			}
			base.DataContext = this;
		}

		protected override void OnAcceptButtonExecute()
		{
			base.Result = ProjectDialog.ProjectDialogResult.Ok;
			base.OnAcceptButtonExecute();
		}

		protected override void OnCancelButtonExecute()
		{
			base.Result = ProjectDialog.ProjectDialogResult.Cancel;
			base.OnCancelButtonExecute();
		}

		private void OnNoCommand(object sender, RoutedEventArgs args)
		{
			base.Result = ProjectDialog.ProjectDialogResult.Discard;
			base.Close();
		}
	}
}