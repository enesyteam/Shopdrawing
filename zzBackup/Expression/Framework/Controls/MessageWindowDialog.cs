// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Controls.MessageWindowDialog
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;

namespace Microsoft.Expression.Framework.Controls
{
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public sealed class MessageWindowDialog : Dialog, IComponentConnector, IStyleConnector
    {
        private IMessageDisplayService messageDisplayService;
        internal OnDemandControl HyperlinkOnDemand;
        internal ItemsControl ButtonsControl;
        private bool _contentLoaded;

        public MessageWindowDialog(IMessageDisplayService messageDisplayService)
        {
            //Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.Framework;v" + (object) this.GetType().Assembly.GetName().Version + ";component/resources/controls/messagewindow.xaml", UriKind.Relative));
            Application.LoadComponent((object)this, new Uri("/Shopdrawing.Framework;component/Resources/Controls/UserControl1.xaml", UriKind.Relative));
            this.messageDisplayService = messageDisplayService;
        }

        private void HyperlinkClick(object sender, RoutedEventArgs e)
        {
            Hyperlink hyperlink = (Hyperlink)sender;
            if (!(hyperlink.NavigateUri != (Uri)null))
                return;
            WebPageLauncher.Navigate(hyperlink.NavigateUri, this.messageDisplayService);
        }

        [DebuggerNonUserCode]
        public void InitializeComponent()
        {
            if (this._contentLoaded)
                return;
            this._contentLoaded = true;
            Application.LoadComponent((object)this, new Uri("/Microsoft.Expression.Framework;component/resources/controls/messagewindow.xaml", UriKind.Relative));
        }

        [DebuggerNonUserCode]
        internal Delegate _CreateDelegate(Type delegateType, string handler)
        {
            return Delegate.CreateDelegate(delegateType, (object)this, handler);
        }

        [DebuggerNonUserCode]
        [EditorBrowsable(EditorBrowsableState.Never)]
        void IComponentConnector.Connect(int connectionId, object target)
        {
            switch (connectionId)
            {
                case 1:
                    this.HyperlinkOnDemand = (OnDemandControl)target;
                    break;
                case 3:
                    this.ButtonsControl = (ItemsControl)target;
                    break;
                default:
                    this._contentLoaded = true;
                    break;
            }
        }

        [DebuggerNonUserCode]
        [EditorBrowsable(EditorBrowsableState.Never)]
        void IStyleConnector.Connect(int connectionId, object target)
        {
            if (connectionId != 2)
                return;
            ((Hyperlink)target).Click += new RoutedEventHandler(this.HyperlinkClick);
        }
    }
}
