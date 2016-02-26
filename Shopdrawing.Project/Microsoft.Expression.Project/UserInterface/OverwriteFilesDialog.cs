using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Expression.Project.UserInterface
{
	internal class OverwriteFilesDialog : ProjectDialog
	{
		private IExpressionInformationService expressionInformationService;

		private List<string> fileList;

		private bool? result = null;

		private bool allowOverwrite;

		private string labelText;

		private string acceptButtonText;

		public string AcceptButtonText
		{
			get
			{
				return this.acceptButtonText;
			}
		}

		public bool AllowOverwrite
		{
			get
			{
				return this.allowOverwrite;
			}
		}

		public string LabelText
		{
			get
			{
				return this.labelText;
			}
		}

		public List<string> OverwriteItems
		{
			get
			{
				return this.fileList;
			}
		}

		internal new bool? Result
		{
			get
			{
				return this.result;
			}
		}

		public OverwriteFilesDialog(IEnumerable<string> fileList, bool allowOverwrite, IExpressionInformationService expressionInformationService)
		{
			this.expressionInformationService = expressionInformationService;
			this.allowOverwrite = allowOverwrite;
			if (!this.allowOverwrite)
			{
				this.labelText = StringTable.OverwriteFilesDialogCannotContinueMessage;
				this.acceptButtonText = StringTable.AcceptButtonOkText;
			}
			else
			{
				this.labelText = StringTable.OverwriteFilesDialogContinueMessage;
				this.acceptButtonText = StringTable.AcceptButtonYesText;
			}
			this.fileList = (
				from file in fileList
				select PathHelper.GetFileOrDirectoryName(file)).ToList<string>();
			base.SetValue(AutomationElement.IdProperty, "OverwriteFilesYesNoDialog");
		}

		public void InitializeDialog()
		{
			base.DialogContent = Microsoft.Expression.Project.FileTable.GetElement("Resources\\OverwriteFilesDialog.xaml");
			base.Title = this.expressionInformationService.DefaultDialogTitle;
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
			this.result = new bool?(true);
			base.OnAcceptButtonExecute();
		}

		protected override void OnCancelButtonExecute()
		{
			this.result = null;
			base.OnCancelButtonExecute();
		}

		private void OnNoCommand(object sender, RoutedEventArgs args)
		{
			this.result = new bool?(false);
			base.Close();
		}
	}
}