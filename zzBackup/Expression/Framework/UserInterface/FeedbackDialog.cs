// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.UserInterface.FeedbackDialog
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Extensibility;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Feedback;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;

namespace Microsoft.Expression.Framework.UserInterface
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class FeedbackDialog : Dialog, IComponentConnector
  {
    private IServices services;
    private FeedbackDialogModel model;
    internal Hyperlink PART_WebPageLink;
    internal Button AcceptButton;
    internal Button CancelButton;
    private bool _contentLoaded;

    public FeedbackDialogModel Model
    {
      get
      {
        return this.model;
      }
    }

    public FeedbackDialog(IServices services)
    {
      this.InitializeComponent();
      this.services = services;
      this.model = new FeedbackDialogModel(this.services.GetService<IExpressionInformationService>().ShortApplicationName, this.services.GetService<IFeedbackService>());
      this.DataContext = (object) this.model;
      this.Title = StringTable.FeedbackDialogTitle;
      this.PART_WebPageLink.Click += new RoutedEventHandler(this.Hyperlink_Click);
    }

    private void Hyperlink_Click(object sender, RoutedEventArgs e)
    {
      WebPageLauncher.Navigate(((Hyperlink) sender).NavigateUri, this.services.GetService<IMessageDisplayService>());
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.Framework;component/userinterface/feedbackdialog.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    internal Delegate _CreateDelegate(Type delegateType, string handler)
    {
      return Delegate.CreateDelegate(delegateType, (object) this, handler);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DebuggerNonUserCode]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.PART_WebPageLink = (Hyperlink) target;
          break;
        case 2:
          this.AcceptButton = (Button) target;
          break;
        case 3:
          this.CancelButton = (Button) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
