// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.ItemizedLicensingDialog
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Extensibility;
using Microsoft.Expression.Framework.Controls;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace Microsoft.Expression.Framework
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class ItemizedLicensingDialog : LicensingDialogBase, IComponentConnector
  {
    private IList<ItemizedLicensingComponentBase> licenseComponents;
    internal StackPanel DocumentRoot;
    internal Button AcceptButton;
    internal AccessText ContinueButtonText;
    private bool _contentLoaded;

    public IList<ItemizedLicensingComponentBase> LicensingComponents
    {
      get
      {
        return this.licenseComponents;
      }
    }

    public override ProductKeyEditControl KeyValueEditorInternal
    {
      get
      {
        return (ProductKeyEditControl) null;
      }
    }

    public ItemizedLicensingDialog(IServices services, ItemizedLicensingComponentBase[] components)
      : base(services)
    {
      this.licenseComponents = (IList<ItemizedLicensingComponentBase>) new List<ItemizedLicensingComponentBase>((IEnumerable<ItemizedLicensingComponentBase>) components);
      foreach (ItemizedLicensingComponentBase licensingComponentBase in (IEnumerable<ItemizedLicensingComponentBase>) this.licenseComponents)
        licensingComponentBase.StatusChanged += new EventHandler(this.Component_StatusChanged);
      this.InitializeComponent();
      FocusManager.SetIsFocusScope((DependencyObject) this, true);
    }

    private void Component_StatusChanged(object sender, EventArgs e)
    {
      ItemizedLicensingComponentBase licensingComponentBase1 = sender as ItemizedLicensingComponentBase;
      foreach (ItemizedLicensingComponentBase licensingComponentBase2 in (IEnumerable<ItemizedLicensingComponentBase>) this.licenseComponents)
      {
        if (licensingComponentBase2 != licensingComponentBase1)
          licensingComponentBase2.RefreshLicenseValues();
      }
    }

    public override void InitializeDialog()
    {
      this.Title = this.ExpressionInformationService.DefaultDialogTitle;
      this.ShowInTaskbar = false;
      this.WindowStyle = WindowStyle.None;
      this.AllowsTransparency = true;
      this.Background = (Brush) new SolidColorBrush(Colors.Transparent);
      this.SizeToContent = SizeToContent.WidthAndHeight;
      this.DataContext = (object) this;
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.Framework;component/licensing/itemizedlicensingdialog.xaml", UriKind.Relative));
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
          this.AcceptButton = (Button) target;
          break;
        case 3:
          this.ContinueButtonText = (AccessText) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
