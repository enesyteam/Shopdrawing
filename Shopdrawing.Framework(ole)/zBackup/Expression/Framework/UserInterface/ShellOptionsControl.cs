// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.UserInterface.ShellOptionsControl
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

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
  internal sealed class ShellOptionsControl : StackPanel, IComponentConnector
  {
    internal ShellOptionsControl shellOptionsControl;
    internal ComboBox ThemeTextBox;
    private bool _contentLoaded;

    public ShellOptionsControl(ShellOptionsModel shellOptionsModel)
    {
      this.DataContext = (object) shellOptionsModel;
      this.InitializeComponent();
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.Framework;component/userinterface/shelloptionscontrol.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.shellOptionsControl = (ShellOptionsControl) target;
          break;
        case 2:
          this.ThemeTextBox = (ComboBox) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
