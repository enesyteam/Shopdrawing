// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.UserInterface.FindDialog
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using Microsoft.Expression.DesignModel.Code;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Data;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Microsoft.Expression.Code.UserInterface
{
  internal sealed class FindDialog : Dialog, INotifyPropertyChanged
  {
    private FindReplaceModel findReplaceModel;
    private ITextEditor textEditor;
    private IMessageDisplayService messageManager;
    private DelegateCommand findNextCommand;
    private bool shouldFocusTextEditor;
    private static FindDialog findDialog;

    public string FindText
    {
      get
      {
        return this.findReplaceModel.FindText;
      }
      set
      {
        this.findReplaceModel.FindText = value;
        this.findNextCommand.IsEnabled = !string.IsNullOrEmpty(value);
        this.OnPropertyChanged("FindText");
      }
    }

    public bool MatchCase
    {
      get
      {
        return this.findReplaceModel.MatchCase;
      }
      set
      {
        this.findReplaceModel.MatchCase = value;
        this.OnPropertyChanged("MatchCase");
      }
    }

    public bool SearchReverse
    {
      get
      {
        return this.findReplaceModel.SearchReverse;
      }
      set
      {
        this.findReplaceModel.SearchReverse = value;
        this.OnPropertyChanged("SearchReverse");
      }
    }

    public ICommand FindNextCommand
    {
      get
      {
        return (ICommand) this.findNextCommand;
      }
    }

    private Window MessageBoxOwner
    {
      get
      {
        if (!this.IsVisible)
          return this.Owner;
        return (Window) this;
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private FindDialog(FindReplaceModel findReplaceModel, ITextEditor textEditor, IMessageDisplayService messageManager)
    {
      this.textEditor = textEditor;
      this.messageManager = messageManager;
      this.findReplaceModel = findReplaceModel;
      this.DialogContent = (UIElement) Microsoft.Expression.Code.FileTable.GetElement("UserInterface\\FindDialog.xaml");
      this.Title = StringTable.FindDialogTitle;
      this.SizeToContent = SizeToContent.WidthAndHeight;
      this.DataContext = (object) this;
      this.findNextCommand = new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.OnFindNext));
      this.FindText = this.FindText;
    }

    public static FindDialog GetFindDialog(FindReplaceModel findReplaceModel, ITextEditor textEditor, IMessageDisplayService messageManager)
    {
      if (FindDialog.findDialog == null)
        FindDialog.findDialog = new FindDialog(findReplaceModel, textEditor, messageManager);
      return FindDialog.findDialog;
    }

    public static void CloseIfOpen()
    {
      if (FindDialog.findDialog == null)
        return;
      FindDialog.findDialog.Close();
    }

    public new void Show()
    {
      base.Show();
      this.Activate();
      UIThreadDispatcher.Instance.Invoke(DispatcherPriority.Background, (Action) (() =>
      {
        TextBox textBox = (TextBox) LogicalTreeHelper.FindLogicalNode((DependencyObject) this, "FindText");
        textBox.Focus();
        textBox.SelectAll();
      }));
    }

    public bool FindNext()
    {
      int matchOffset = this.textEditor.Find(this.FindText, this.MatchCase, this.SearchReverse);
      if (matchOffset == -1)
      {
        int num = (int) this.messageManager.ShowMessage(new MessageBoxArgs()
        {
          Owner = this.MessageBoxOwner,
          Message = StringTable.FindDialogCannotFind,
          Button = MessageBoxButton.OK,
          Image = MessageBoxImage.Asterisk
        });
        return false;
      }
      if (this.findReplaceModel.MatchOffset == matchOffset)
      {
        int num1 = (int) this.messageManager.ShowMessage(new MessageBoxArgs()
        {
          Owner = this.MessageBoxOwner,
          Message = StringTable.FindDialogReachedStart,
          Button = MessageBoxButton.OK,
          Image = MessageBoxImage.Asterisk
        });
      }
      this.findReplaceModel.SetMatchIfFirst(matchOffset);
      return true;
    }

    protected override void OnClosing(CancelEventArgs e)
    {
      FindDialog.findDialog = (FindDialog) null;
      base.OnClosing(e);
      this.Owner.Activate();
      if (!this.shouldFocusTextEditor)
        return;
      this.shouldFocusTextEditor = false;
      this.textEditor.Focus();
    }

    protected override void OnCancelButtonExecute()
    {
      this.Close();
    }

    protected override void OnDeactivated(EventArgs e)
    {
      this.findReplaceModel.ClearFirstMatch();
      base.OnDeactivated(e);
    }

    private void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }

    private void OnFindNext()
    {
      if (!this.FindNext())
        return;
      this.shouldFocusTextEditor = true;
    }
  }
}
