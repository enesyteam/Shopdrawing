using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Diagnostics;
using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Expression.Project.UserInterface
{
	public class NewProjectDialog : Dialog
	{
		private ProjectDialogResult result = ProjectDialogResult.Cancel;

		public virtual string AcceptButtonText
		{
			get
			{
				return StringTable.AcceptButtonYesText;
			}
		}

		protected string ApplicationName
		{
			get;
			private set;
		}

		public virtual string CancelButtonText
		{
			get
			{
				return StringTable.CancelButtonText;
			}
		}

		public virtual string DiscardButtonText
		{
			get
			{
				return StringTable.DiscardButtonNoText;
			}
		}

		public ProjectDialogResult Result
		{
			get
			{
				return this.result;
			}
			protected set
			{
				this.result = value;
			}
		}

		public NewProjectDialog(string automationId, IExpressionInformationService expressionInformationService)
		{
			base.Title = expressionInformationService.DefaultDialogTitle;
			this.ApplicationName = expressionInformationService.ShortApplicationName;
			base.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
			base.SetValue(AutomationElement.IdProperty, automationId);
		}

		protected override void OnAcceptButtonExecute()
		{
			this.result = ProjectDialogResult.Yes;
			base.OnAcceptButtonExecute();
		}

		protected override void OnCancelButtonExecute()
		{
			this.result = ProjectDialogResult.Cancel;
			base.OnCancelButtonExecute();
		}

		protected virtual void OnDiscardButtonExecute(object sender, RoutedEventArgs args)
		{
			this.result = ProjectDialogResult.No;
			base.Close();
		}

		protected override void OnInitialized(EventArgs e)
		{
			base.DataContext = this;
			Button button = LogicalTreeHelper.FindLogicalNode(this, "DiscardButton") as Button;
			if (button != null)
			{
				NewProjectDialog newProjectDialog = this;
				button.Click += new RoutedEventHandler(newProjectDialog.OnDiscardButtonExecute);
			}
			base.OnInitialized(e);
		}

		public ProjectDialogResult ShowProjectDialog()
		{
			base.ShowDialog();
			return this.Result;
		}
	}
}