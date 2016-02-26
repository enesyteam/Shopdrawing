// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.LicensingDialog
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Extensibility;
using Microsoft.Expression.Framework.Controls;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Microsoft.Expression.Framework
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public abstract class LicensingDialog : LicensingDialogBase, IComponentConnector
  {
    internal StackPanel DocumentRoot;
    internal RadioButton HaveProductKeyButton;
    internal ProductKeyEditControl KeyValueEditor;
    internal CheckBox ActivateCheckBox;
    internal RadioButton ContinueTrialButton;
    internal TextBlock ContinueTrialButtonText;
    internal ProgressAnimationControl ProgressAnimation;
    internal Button AcceptButton;
    internal Image ShieldImage;
    internal AccessText ContinueButtonText;
    internal Button CancelButton;
    internal AccessText CancelButtonText;
    private bool _contentLoaded;

    public abstract bool ShowActivationCheckBox { get; }

    public abstract bool HasCancelButton { get; }

    public abstract string FallbackOptionText { get; }

    public virtual string HeaderText
    {
      get
      {
        return string.Empty;
      }
    }

    private bool CanContinue
    {
      get
      {
        if (this.ContinueTrialButton.IsChecked.GetValueOrDefault(false))
          return true;
        if (this.HaveProductKeyButton.IsChecked.GetValueOrDefault(false))
          return this.KeyValueEditor.IsWellFormed;
        return false;
      }
    }

    public override ProductKeyEditControl KeyValueEditorInternal
    {
      get
      {
        return this.KeyValueEditor;
      }
    }

    protected override bool ShouldActivate
    {
      get
      {
        if (this.ShowActivationCheckBox && this.ActivateCheckBox.IsChecked.HasValue)
          return this.ActivateCheckBox.IsChecked.Value;
        return false;
      }
    }

    public LicensingDialog(IServices services)
      : base(services)
    {
      this.InitializeComponent();
      FocusManager.SetIsFocusScope((DependencyObject) this, true);
    }

    public override void InitializeDialog()
    {
      this.Title = this.ExpressionInformationService.DefaultDialogTitle;
      this.ShowInTaskbar = false;
      this.WindowStyle = WindowStyle.None;
      this.AllowsTransparency = true;
      this.Background = (Brush) new SolidColorBrush(Colors.Transparent);
      this.SizeToContent = SizeToContent.WidthAndHeight;
      this.HaveProductKeyButton.Checked += new RoutedEventHandler(this.HaveProductKeyButton_IsChecked);
      this.HaveProductKeyButton.Unchecked += new RoutedEventHandler(this.HaveProductKeyButton_IsChecked);
      this.KeyValueEditor.PropertyChanged += new PropertyChangedEventHandler(this.KeyValueEditor_PropertyChanged);
      this.ShieldImage.Source = (ImageSource) LicensingDialog.CreateShieldImage();
      this.ShieldImage.Visibility = Visibility.Collapsed;
      this.ProgressAnimation.Visibility = Visibility.Collapsed;
      this.ActivateCheckBox.IsEnabled = false;
      this.ActivateCheckBox.IsChecked = new bool?(true);
      this.DataContext = (object) this;
    }

    internal static BitmapSource CreateShieldImage()
    {
      IntPtr icon = UnsafeNativeMethods.LoadIcon(IntPtr.Zero, LicensingDialogBase.IDI_SHIELD);
      if (icon != IntPtr.Zero)
        return Imaging.CreateBitmapSourceFromHIcon(icon, Int32Rect.Empty, (BitmapSizeOptions) null);
      return (BitmapSource) null;
    }

    private void NavigateToNavigateUri(object sender, RoutedEventArgs e)
    {
      LicensingDialogBase.NavigateToLink(sender, this.Services);
    }

    private void KeyValueEditor_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == "IsWellFormed"))
        return;
      this.AcceptButton.IsEnabled = this.CanContinue;
    }

    private void HaveProductKeyButton_IsChecked(object sender, RoutedEventArgs e)
    {
      this.AcceptButton.IsEnabled = this.CanContinue;
      if (this.ShieldImage.Source != null)
        this.ShieldImage.Visibility = this.HaveProductKeyButton.IsChecked.GetValueOrDefault(false) ? Visibility.Visible : Visibility.Collapsed;
      this.ActivateCheckBox.IsEnabled = this.HaveProductKeyButton.IsChecked.HasValue && this.HaveProductKeyButton.IsChecked.Value;
    }

    protected override void OnAcceptButtonExecute()
    {
      bool flag = false;
      if (this.ContinueTrialButton.IsChecked.GetValueOrDefault(false))
        flag = this.ProcessFallbackOption();
      else if (this.HaveProductKeyButton.IsChecked.GetValueOrDefault(false))
      {
        if (this.KeyValueEditor.IsWellFormed)
        {
          this.ProcessEnteredProductKeyAsync();
        }
        else
        {
          int num = (int) this.Services.GetService<IMessageDisplayService>().ShowMessage(new MessageBoxArgs()
          {
            Owner = (Window) this,
            Message = StringTable.LicensingInvalidKeyMessage,
            Button = MessageBoxButton.OK,
            Image = MessageBoxImage.Hand
          });
        }
      }
      if (!flag)
        return;
      base.OnAcceptButtonExecute();
    }

    protected abstract bool ProcessFallbackOption();

    private void ProcessEnteredProductKeyAsync()
    {
      BackgroundWorker backgroundWorker = new BackgroundWorker();
      backgroundWorker.DoWork += new DoWorkEventHandler(this.ProcessProductKeyWorker_Work);
      backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.ProcessProductKeyWorker_Completed);
      backgroundWorker.RunWorkerAsync((object) this.KeyValueEditor.Text);
      this.ChangeUIEnabledState(false);
    }

    private void ChangeUIEnabledState(bool enabled)
    {
      this.ProgressAnimation.Visibility = enabled ? Visibility.Collapsed : Visibility.Visible;
      this.AcceptButton.IsEnabled = enabled;
      this.HaveProductKeyButton.IsEnabled = enabled;
      this.KeyValueEditor.IsEnabled = enabled;
      this.ContinueTrialButton.IsEnabled = enabled;
      this.ActivateCheckBox.IsEnabled = enabled;
    }

    private void ProcessProductKeyWorker_Completed(object sender, RunWorkerCompletedEventArgs e)
    {
      this.ChangeUIEnabledState(true);
      this.ProcessLicenseServiceResponse((IEnterKeyResponse) e.Result);
      if (!this.IsLicensed)
        return;
      base.OnAcceptButtonExecute();
    }

    private void ProcessProductKeyWorker_Work(object sender, DoWorkEventArgs e)
    {
      IEnterKeyResponse enterKeyResponse = this.InsertKeyIntoLicenseStore((string) e.Argument);
      e.Result = (object) enterKeyResponse;
    }

    protected override void OnClosing(CancelEventArgs e)
    {
      if (!this.DialogResult.GetValueOrDefault(false))
      {
        if (MessageBoxResult.No == this.Services.GetService<IMessageDisplayService>().ShowMessage(new MessageBoxArgs()
        {
          Owner = (Window) this,
          Message = StringTable.LicensingNotLicensedWarningMessage,
          Button = MessageBoxButton.YesNo,
          Image = MessageBoxImage.Exclamation
        }))
          e.Cancel = true;
      }
      base.OnClosing(e);
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.Framework;component/licensing/licensingdialog.xaml", UriKind.Relative));
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
          this.DocumentRoot = (StackPanel) target;
          break;
        case 2:
          this.HaveProductKeyButton = (RadioButton) target;
          break;
        case 3:
          this.KeyValueEditor = (ProductKeyEditControl) target;
          break;
        case 4:
          ((Hyperlink) target).Click += new RoutedEventHandler(this.NavigateToNavigateUri);
          break;
        case 5:
          ((Hyperlink) target).Click += new RoutedEventHandler(this.NavigateToNavigateUri);
          break;
        case 6:
          this.ActivateCheckBox = (CheckBox) target;
          break;
        case 7:
          this.ContinueTrialButton = (RadioButton) target;
          break;
        case 8:
          this.ContinueTrialButtonText = (TextBlock) target;
          break;
        case 9:
          ((Hyperlink) target).Click += new RoutedEventHandler(this.NavigateToNavigateUri);
          break;
        case 10:
          ((Hyperlink) target).Click += new RoutedEventHandler(this.NavigateToNavigateUri);
          break;
        case 11:
          this.ProgressAnimation = (ProgressAnimationControl) target;
          break;
        case 12:
          this.AcceptButton = (Button) target;
          break;
        case 13:
          this.ShieldImage = (Image) target;
          break;
        case 14:
          this.ContinueButtonText = (AccessText) target;
          break;
        case 15:
          this.CancelButton = (Button) target;
          break;
        case 16:
          this.CancelButtonText = (AccessText) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
