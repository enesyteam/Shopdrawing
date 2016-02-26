using Microsoft.Expression.Framework;
using Shopdrawing.Framework.Controls;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;

namespace Microsoft.Expression.Framework.Controls
{
	public sealed class MessageWindowDialog : Dialog
	{
		private IMessageDisplayService messageDisplayService;

		internal OnDemandControl HyperlinkOnDemand;

		internal ItemsControl ButtonsControl;

		public MessageWindowDialog(IMessageDisplayService messageDisplayService)
		{
			Version version = base.GetType().Assembly.GetName().Version;
            Uri uri = new Uri(string.Concat("/Shopdrawing.Controls", ";component/MessageBox/UserControl1.xaml"), UriKind.Relative);
			Application.LoadComponent(this, uri);
			this.messageDisplayService = messageDisplayService;
		}

		private void HyperlinkClick(object sender, RoutedEventArgs e)
		{
			Hyperlink hyperlink = (Hyperlink)sender;
			if (hyperlink.NavigateUri != null)
			{
				WebPageLauncher.Navigate(hyperlink.NavigateUri, this.messageDisplayService);
			}
		}
	}
}