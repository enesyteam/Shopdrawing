// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.TrialExpiredDialog
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
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Microsoft.Expression.Framework
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class TrialExpiredDialog : LicensingDialogBase, IComponentConnector
  {
    internal StackPanel DocumentRoot;
    internal TextBlock TrialExpiredText;
    internal ProductKeyEditControl KeyValueEditor;
    internal Button AcceptButton;
    internal Image ShieldImage;
    internal Button CancelButton;
    private bool _contentLoaded;

    public bool CanContinue
    {
      get
      {
        return this.KeyValueEditor.IsWellFormed;
      }
    }

    public override ProductKeyEditControl KeyValueEditorInternal
    {
      get
      {
        return this.KeyValueEditor;
      }
    }

    public TrialExpiredDialog(IServices services)
      : base(services)
    {
      this.InitializeComponent();
    }

    public override void InitializeDialog()
    {
      this.Title = this.ExpressionInformationService.DefaultDialogTitle;
      this.ShowInTaskbar = false;
      this.WindowStyle = WindowStyle.None;
      this.AllowsTransparency = true;
      this.Background = (Brush) new SolidColorBrush(Colors.Transparent);
      this.SizeToContent = SizeToContent.WidthAndHeight;
      this.TrialExpiredText.Text = StringTable.LicensingTrialExpiredText;
      this.KeyValueEditor.PropertyChanged += new PropertyChangedEventHandler(this.KeyValueEditor_PropertyChanged);
      IntPtr icon = UnsafeNativeMethods.LoadIcon(IntPtr.Zero, LicensingDialogBase.IDI_SHIELD);
      if (icon != IntPtr.Zero)
        this.ShieldImage.Source = (ImageSource) Imaging.CreateBitmapSourceFromHIcon(icon, Int32Rect.Empty, (BitmapSizeOptions) null);
      else
        this.ShieldImage.Visibility = Visibility.Collapsed;
      this.DataContext = (object) this;
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

    protected override void OnAcceptButtonExecute()
    {
      this.IsLicensed = false;
      this.ProcessLicenseServiceResponse(this.InsertKeyIntoLicenseStore(this.KeyValueEditor.Text));
      if (!this.IsLicensed)
        return;
      base.OnAcceptButtonExecute();
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.Framework;component/licensing/trialexpireddialog.xaml", UriKind.Relative));
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
          this.TrialExpiredText = (TextBlock) target;
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
          this.AcceptButton = (Button) target;
          break;
        case 7:
          this.ShieldImage = (Image) target;
          break;
        case 8:
          this.CancelButton = (Button) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
