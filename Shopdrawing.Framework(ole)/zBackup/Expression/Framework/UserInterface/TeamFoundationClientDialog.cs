// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.UserInterface.TeamFoundationClientDialog
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Diagnostics;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Microsoft.Expression.Framework.UserInterface
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class TeamFoundationClientDialog : Dialog, IComponentConnector
  {
    private IServiceProvider serviceProvider;
    internal Button AcceptButton;
    private bool _contentLoaded;

    public bool DoNotShowAgain { get; set; }

    public TeamFoundationClientDialog(IServiceProvider serviceProvider)
    {
      this.InitializeComponent();
      this.serviceProvider = serviceProvider;
      this.DataContext = (object) this;
      this.Title = (this.serviceProvider.GetService(typeof (IExpressionInformationService)) as IExpressionInformationService).DefaultDialogTitle;
      this.SetValue(AutomationElement.IdProperty, (object) "SourceControlDialog");
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.Framework;component/resources/teamfoundationclientdialog.xaml", UriKind.Relative));
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
      if (connectionId == 1)
        this.AcceptButton = (Button) target;
      else
        this._contentLoaded = true;
    }
  }
}
