// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.ActivationDialog.ActivationDialog
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Extensibility;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.ActivationDialog.DataAccess;
using Microsoft.Expression.Framework.ActivationDialog.ViewModel;
using Microsoft.Expression.Framework.Controls;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace Microsoft.Expression.Framework.ActivationDialog
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  internal class ActivationDialog : LicensingDialogBase, IComponentConnector
  {
    private ActivationWizardViewModel activationWizardViewModel;
    private ActivationWizardAction wizardStartAction;
    internal Button PreviousButton;
    internal AccessText PreviousButtonText;
    internal Button ContinueButton;
    internal AccessText ContinueButtonText;
    internal Button CancelButton;
    internal AccessText CancelButtonText;
    private bool _contentLoaded;

    public override ProductKeyEditControl KeyValueEditorInternal
    {
      get
      {
        return (ProductKeyEditControl) null;
      }
    }

    public ActivationDialog(IServices services, ActivationWizardAction wizardStartAction)
      : base(services)
    {
      this.wizardStartAction = wizardStartAction;
      this.InitializeComponent();
      InputMethod.SetIsInputMethodEnabled((DependencyObject) this, false);
    }

    public override void InitializeDialog()
    {
      this.Title = this.ExpressionInformationService.DefaultDialogTitle;
      this.ShowInTaskbar = false;
      this.WindowStyle = WindowStyle.None;
      this.AllowsTransparency = true;
      this.Background = (Brush) new SolidColorBrush(Colors.Transparent);
      this.SizeToContent = SizeToContent.WidthAndHeight;
      this.activationWizardViewModel = new ActivationWizardViewModel(new ActivationDataRepository(this.Services.GetService<ILicenseService>()), this.Services, this.wizardStartAction);
      this.activationWizardViewModel.ActivationComplete += new EventHandler(this.ActivationWizardViewModel_ActivationComplete);
      this.DataContext = (object) this.activationWizardViewModel;
    }

    private void ActivationWizardViewModel_ActivationComplete(object sender, EventArgs e)
    {
      this.Close(new bool?(true));
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.Framework;component/licensing/activationdialog/activationdialog.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    internal Delegate _CreateDelegate(Type delegateType, string handler)
    {
      return Delegate.CreateDelegate(delegateType, (object) this, handler);
    }

    [DebuggerNonUserCode]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.PreviousButton = (Button) target;
          break;
        case 2:
          this.PreviousButtonText = (AccessText) target;
          break;
        case 3:
          this.ContinueButton = (Button) target;
          break;
        case 4:
          this.ContinueButtonText = (AccessText) target;
          break;
        case 5:
          this.CancelButton = (Button) target;
          break;
        case 6:
          this.CancelButtonText = (AccessText) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
