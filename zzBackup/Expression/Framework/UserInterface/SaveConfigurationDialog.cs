// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.UserInterface.SaveConfigurationDialog
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

namespace Microsoft.Expression.Framework.UserInterface
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class SaveConfigurationDialog : Dialog, INotifyPropertyChanged, IComponentConnector
  {
    private string workspaceName;
    private WorkspaceService workspaceService;
    private MessageBubbleValidator<string, TextChangedEventArgs> nameValidator;
    private MessageBubbleHelper workspaceNameMessageBubble;
    internal SaveConfigurationDialog SaveConfigurationDialogRoot;
    internal TextBox WorkspaceNameTextBox;
    internal Button AcceptButton;
    internal Button CancelButton;
    private bool _contentLoaded;

    public string WorkspaceName
    {
      get
      {
        return this.workspaceName;
      }
      set
      {
        this.workspaceName = value;
        this.OnPropertyChanged("WorkspaceName");
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public SaveConfigurationDialog(WorkspaceService workspaceService)
    {
      this.workspaceService = workspaceService;
      this.InitializeComponent();
      this.nameValidator = new MessageBubbleValidator<string, TextChangedEventArgs>((Func<string>) (() => this.WorkspaceNameTextBox.Text), new Func<string, string>(this.ValidateWorkspaceName));
      this.workspaceNameMessageBubble = new MessageBubbleHelper((UIElement) this.WorkspaceNameTextBox, (IMessageBubbleValidator) this.nameValidator);
      this.WorkspaceNameTextBox.TextChanged += new TextChangedEventHandler(this.nameValidator.EventHook);
      string customWorkspaceName = StringTable.DefaultCustomWorkspaceName;
      int num = 1;
      while (this.workspaceService.IsExistingWorkspaceName(customWorkspaceName + (object) num))
        ++num;
      this.WorkspaceName = customWorkspaceName + (object) num;
      this.Title = StringTable.SaveAsNewWorkspaceDialogTitle;
    }

    private string ValidateWorkspaceName(string name)
    {
      string error = (string) null;
      this.AcceptButton.IsEnabled = this.workspaceService.IsValidNameForNewWorkspace(name, out error);
      return error;
    }

    private void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.Framework;component/userinterface/saveconfigurationdialog.xaml", UriKind.Relative));
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
          this.SaveConfigurationDialogRoot = (SaveConfigurationDialog) target;
          break;
        case 2:
          this.WorkspaceNameTextBox = (TextBox) target;
          break;
        case 3:
          this.AcceptButton = (Button) target;
          break;
        case 4:
          this.CancelButton = (Button) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
