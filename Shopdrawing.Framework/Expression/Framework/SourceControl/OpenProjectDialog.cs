// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.SourceControl.OpenProjectDialog
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework.Controls;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Microsoft.Expression.Framework.SourceControl
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class OpenProjectDialog : Dialog, INotifyPropertyChanged, IComponentConnector
  {
    internal ComboBox ServerComboBox;
    internal Button AcceptButton;
    internal Button CancelButton;
    private bool _contentLoaded;

    public event PropertyChangedEventHandler PropertyChanged
    {
      add
      {
      }
      remove
      {
      }
    }

    public OpenProjectDialog(SourceControlProjectModel model)
    {
      this.InitializeComponent();
      foreach (object newItem in model.GetRegisteredServers())
        this.ServerComboBox.Items.Add(newItem);
      this.Title = StringTable.SourceControlOpenProjectDialogTitle;
    }

    protected override void OnActivated(EventArgs e)
    {
      base.OnActivated(e);
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.Framework;component/sourcecontrol/userinterface/openprojectdialog.xaml", UriKind.Relative));
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
          this.ServerComboBox = (ComboBox) target;
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
