// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.ActivationDialog.Views.ChooseActivationTypePageView
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.ActivationDialog.ViewModel;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;

namespace Microsoft.Expression.Framework.ActivationDialog.Views
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  internal class ChooseActivationTypePageView : UserControl, IComponentConnector
  {
    internal ComboBox LocationComboBox;
    private bool _contentLoaded;

    public ChooseActivationTypePageView()
    {
      this.InitializeComponent();
    }

    private void NavigateToNavigateUri(object sender, RoutedEventArgs e)
    {
      ChooseActivationTypePageViewModel typePageViewModel = (ChooseActivationTypePageViewModel) this.DataContext;
      LicensingDialogBase.NavigateToLink(sender, typePageViewModel.Services);
    }

    private void OverlayComboBoxText_MouseDown(object sender, MouseButtonEventArgs e)
    {
      this.LocationComboBox.IsDropDownOpen = true;
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.Framework;component/licensing/activationdialog/view/chooseactivationtypepageview.xaml", UriKind.Relative));
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
          this.LocationComboBox = (ComboBox) target;
          break;
        case 2:
          ((Hyperlink) target).Click += new RoutedEventHandler(this.NavigateToNavigateUri);
          break;
        case 3:
          ((Hyperlink) target).Click += new RoutedEventHandler(this.NavigateToNavigateUri);
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
