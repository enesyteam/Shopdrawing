// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.UserInterface.ManageWorkspaceConfigurationsDialog
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Data;
using Microsoft.Expression.Framework.ValueEditors;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Threading;

namespace Microsoft.Expression.Framework.UserInterface
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class ManageWorkspaceConfigurationsDialog : Dialog, INotifyPropertyChanged, IComponentConnector
  {
    public static readonly DependencyProperty DoEditProperty = DependencyProperty.RegisterAttached("DoEdit", typeof (bool), typeof (ManageWorkspaceConfigurationsDialog), (PropertyMetadata) new FrameworkPropertyMetadata(new PropertyChangedCallback(ManageWorkspaceConfigurationsDialog.OnDoEdit)));
    private ObservableCollection<ManageWorkspaceConfigurationsDialog.WorkspaceEntry> workspaces = new ObservableCollection<ManageWorkspaceConfigurationsDialog.WorkspaceEntry>();
    private ObservableCollection<ManageWorkspaceConfigurationsDialog.IWorkspaceOperation> workspaceOperations = new ObservableCollection<ManageWorkspaceConfigurationsDialog.IWorkspaceOperation>();
    private bool renameError;
    private IMessageDisplayService messageService;
    private WorkspaceService workspaceService;
    internal ManageWorkspaceConfigurationsDialog ManageWorkspaceConfigurationsDialogRoot;
    internal ListBox WorkspaceListBox;
    internal Button AcceptButton;
    internal Button CancelButton;
    private bool _contentLoaded;

    public ManageWorkspaceConfigurationsDialog.WorkspaceEntry CurrentWorkspace
    {
      get
      {
        return CollectionViewSource.GetDefaultView((object) this.Workspaces).CurrentItem as ManageWorkspaceConfigurationsDialog.WorkspaceEntry;
      }
    }

    public bool HasSingleSelection
    {
      get
      {
        return this.WorkspaceListBox.SelectedItems.Count == 1;
      }
    }

    public bool CanRename
    {
      get
      {
        return this.HasSingleSelection;
      }
    }

    public bool CanDelete
    {
      get
      {
        if (this.WorkspaceListBox.SelectedItems.Count == 1)
          return !this.CurrentWorkspace.IsActiveWorkspace;
        return this.WorkspaceListBox.SelectedItems.Count > 1;
      }
    }

    public ObservableCollection<ManageWorkspaceConfigurationsDialog.WorkspaceEntry> Workspaces
    {
      get
      {
        return this.workspaces;
      }
    }

    public ICommand RenameCommand
    {
      get
      {
        return (ICommand) new DelegateCommand((DelegateCommand.SimpleEventHandler) (() => this.RenameSelectedWorkspace()));
      }
    }

    public ICommand DeleteCommand
    {
      get
      {
        return (ICommand) new DelegateCommand((DelegateCommand.SimpleEventHandler) (() => this.DeleteSelectedWorkspaces()));
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public ManageWorkspaceConfigurationsDialog(WorkspaceService workspaceService, IMessageDisplayService messageService)
    {
      this.workspaceService = workspaceService;
      this.messageService = messageService;
      this.InitializeComponent();
      ReadOnlyCollection<string> readOnlyCollection = (ReadOnlyCollection<string>) this.workspaceService.CustomWorkspaceNames;
      for (int index = 0; index < readOnlyCollection.Count; ++index)
        this.workspaces.Add(new ManageWorkspaceConfigurationsDialog.WorkspaceEntry(this, readOnlyCollection[index]));
      this.WorkspaceListBox.SelectionChanged += new SelectionChangedEventHandler(this.WorkspaceListBox_SelectionChanged);
      this.Title = StringTable.ManageWorkspaceConfigurationsDialogTitle;
      if (this.workspaces.Count <= 0)
        return;
      CollectionViewSource.GetDefaultView((object) this.Workspaces).MoveCurrentTo((object) this.workspaces[0]);
    }

    public static bool GetDoEdit(DependencyObject dependencyObject)
    {
      return (bool) dependencyObject.GetValue(ManageWorkspaceConfigurationsDialog.DoEditProperty);
    }

    public static void SetDoEdit(DependencyObject dependencyObject, bool value)
    {
        dependencyObject.SetValue(ManageWorkspaceConfigurationsDialog.DoEditProperty, value);
    }

    private static void OnDoEdit(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      InlineStringEditor inlineStringEditor = sender as InlineStringEditor;
      if (inlineStringEditor == null || !(bool) e.NewValue)
        return;
      inlineStringEditor.EditCommand.Execute((object) null);
      sender.SetValue(ManageWorkspaceConfigurationsDialog.DoEditProperty, (object) false);
    }

    private void WorkspaceListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      this.OnPropertyChanged("CurrentWorkspace");
      this.OnPropertyChanged("HasSingleSelection");
      this.OnPropertyChanged("CanDelete");
      this.OnPropertyChanged("CanRename");
    }

    private void DialogKeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.F2 && this.HasSingleSelection)
        this.CurrentWorkspace.DoEdit = true;
      if (e.Key != Key.Delete || !this.CanDelete)
        return;
      this.DeleteSelectedWorkspaces();
    }

    protected override void OnAcceptButtonExecute()
    {
      if (this.renameError)
      {
        this.renameError = false;
        this.CurrentWorkspace.DoEdit = true;
      }
      else
        base.OnAcceptButtonExecute();
    }

    private void DeleteSelectedWorkspaces()
    {
      string str = this.WorkspaceListBox.SelectedItems.Count > 1 ? StringTable.ConfirmMultipleWorkspaceDeletionDialogText : StringTable.ConfirmWorkspaceDeletionDialogText;
      if (this.messageService.ShowMessage(new MessageBoxArgs()
      {
        Message = str,
        Button = MessageBoxButton.YesNo,
        Image = MessageBoxImage.Exclamation
      }) != MessageBoxResult.Yes)
        return;
      foreach (ManageWorkspaceConfigurationsDialog.WorkspaceEntry workspaceEntry in new ArrayList((ICollection) this.WorkspaceListBox.SelectedItems))
      {
        if (workspaceEntry.IsActiveWorkspace)
        {
          this.messageService.ShowError(string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.DeleteWorkspaceInUseDialogText, new object[1]
          {
            (object) workspaceEntry.Name
          }));
        }
        else
        {
          this.workspaceOperations.Add((ManageWorkspaceConfigurationsDialog.IWorkspaceOperation) new ManageWorkspaceConfigurationsDialog.DeleteWorkspaceOperation(this.workspaceService, workspaceEntry.Name));
          this.workspaces.Remove(workspaceEntry);
        }
      }
    }

    private void RenameSelectedWorkspace()
    {
      this.CurrentWorkspace.DoEdit = true;
    }

    public void CommitChanges()
    {
      foreach (ManageWorkspaceConfigurationsDialog.IWorkspaceOperation workspaceOperation in (Collection<ManageWorkspaceConfigurationsDialog.IWorkspaceOperation>) this.workspaceOperations)
        workspaceOperation.Execute();
      this.workspaceOperations.Clear();
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
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.Framework;component/userinterface/manageworkspaceconfigurationsdialog.xaml", UriKind.Relative));
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
          this.ManageWorkspaceConfigurationsDialogRoot = (ManageWorkspaceConfigurationsDialog) target;
          break;
        case 2:
          this.WorkspaceListBox = (ListBox) target;
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

    public class WorkspaceEntry : NotifyingObject
    {
      private ManageWorkspaceConfigurationsDialog dialog;
      private string name;
      private bool doEdit;

      public string Name
      {
        get
        {
          return this.name;
        }
        set
        {
          if (!(this.name != value))
            return;
          string error;
          if (!this.dialog.workspaceService.IsValidNameForNewWorkspace(value, out error))
          {
            this.dialog.messageService.ShowError(error);
            this.dialog.renameError = true;
            this.Dialog.Dispatcher.BeginInvoke(DispatcherPriority.Input, (Delegate) new DispatcherOperationCallback(this.ClearRename), (object) null);
          }
          else
          {
            this.dialog.workspaceOperations.Add((ManageWorkspaceConfigurationsDialog.IWorkspaceOperation) new ManageWorkspaceConfigurationsDialog.RenameWorkspaceOperation(this.dialog.workspaceService, this, this.name, value));
            this.name = value;
            this.OnPropertyChanged("Name");
          }
        }
      }

      public bool IsActiveWorkspace { get; set; }

      public bool IsDeleted { get; set; }

      public bool DoEdit
      {
        get
        {
          return this.doEdit;
        }
        set
        {
          this.doEdit = value;
          this.OnPropertyChanged("DoEdit");
        }
      }

      public ManageWorkspaceConfigurationsDialog Dialog
      {
        get
        {
          return this.dialog;
        }
      }

      public WorkspaceEntry(ManageWorkspaceConfigurationsDialog dialog, string name)
      {
        this.dialog = dialog;
        this.name = name;
        this.IsActiveWorkspace = this.name == this.dialog.workspaceService.ActiveWorkspace.Name;
      }

      private object ClearRename(object sender)
      {
        this.dialog.renameError = false;
        return (object) null;
      }
    }

    private interface IWorkspaceOperation
    {
      bool Execute();
    }

    private class RenameWorkspaceOperation : ManageWorkspaceConfigurationsDialog.IWorkspaceOperation
    {
      private ManageWorkspaceConfigurationsDialog.WorkspaceEntry workspaceEntry;
      private string oldName;
      private string newName;
      private WorkspaceService workspaceService;

      public RenameWorkspaceOperation(WorkspaceService workspaceService, ManageWorkspaceConfigurationsDialog.WorkspaceEntry workspaceEntry, string oldName, string newName)
      {
        this.workspaceService = workspaceService;
        this.workspaceEntry = workspaceEntry;
        this.oldName = oldName;
        this.newName = newName;
      }

      public bool Execute()
      {
        if (!this.workspaceEntry.IsDeleted)
          return this.workspaceService.RenameWorkspace(this.oldName, this.newName);
        return true;
      }
    }

    private class DeleteWorkspaceOperation : ManageWorkspaceConfigurationsDialog.IWorkspaceOperation
    {
      private string workspaceName;
      private WorkspaceService workspaceService;

      public DeleteWorkspaceOperation(WorkspaceService workspaceService, string workspaceName)
      {
        this.workspaceService = workspaceService;
        this.workspaceName = workspaceName;
      }

      public bool Execute()
      {
        return this.workspaceService.DeleteWorkspace(this.workspaceName);
      }
    }
  }
}
