// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.UserInterface.ReplaceDialog
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
  internal sealed class ReplaceDialog : Dialog, INotifyPropertyChanged
  {
    private FindReplaceModel findReplaceModel;
    private ITextEditor textEditor;
    private IMessageDisplayService messageManager;
    private DelegateCommand findNextCommand;
    private DelegateCommand replaceCommand;
    private DelegateCommand replaceAllCommand;
    private bool shouldFocusTextEditor;
    private static ReplaceDialog replaceDialog;

    public string FindText
    {
      get
      {
        return this.findReplaceModel.FindText;
      }
      set
      {
        this.findReplaceModel.FindText = value;
        bool flag = !string.IsNullOrEmpty(value);
        this.findNextCommand.IsEnabled = flag;
        this.replaceCommand.IsEnabled = flag;
        this.replaceAllCommand.IsEnabled = flag;
        this.OnPropertyChanged("FindText");
      }
    }

    public string ReplaceText
    {
      get
      {
        return this.findReplaceModel.ReplaceText;
      }
      set
      {
        this.findReplaceModel.ReplaceText = value;
        this.OnPropertyChanged("ReplaceText");
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

    public ICommand FindNextCommand
    {
      get
      {
        return (ICommand) this.findNextCommand;
      }
    }

    public ICommand ReplaceCommand
    {
      get
      {
        return (ICommand) this.replaceCommand;
      }
    }

    public ICommand ReplaceAllCommand
    {
      get
      {
        return (ICommand) this.replaceAllCommand;
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

    public ReplaceDialog(FindReplaceModel findReplaceModel, ITextEditor textEditor, IMessageDisplayService messageManager)
    {
      this.textEditor = textEditor;
      this.messageManager = messageManager;
      this.findReplaceModel = findReplaceModel;
      this.DialogContent = (UIElement) Microsoft.Expression.Code.FileTable.GetElement("UserInterface\\ReplaceDialog.xaml");
      this.Title = StringTable.ReplaceDialogTitle;
      this.SizeToContent = SizeToContent.WidthAndHeight;
      this.DataContext = (object) this;
      this.findNextCommand = new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.OnFindNext));
      this.replaceCommand = new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.OnReplace));
      this.replaceAllCommand = new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.OnReplaceAll));
      this.FindText = this.FindText;
    }

    public static ReplaceDialog GetReplaceDialog(FindReplaceModel findReplaceModel, ITextEditor textEditor, IMessageDisplayService messageManager)
    {
      if (ReplaceDialog.replaceDialog == null)
        ReplaceDialog.replaceDialog = new ReplaceDialog(findReplaceModel, textEditor, messageManager);
      return ReplaceDialog.replaceDialog;
    }

    public static void CloseIfOpen()
    {
      if (ReplaceDialog.replaceDialog == null)
        return;
      ReplaceDialog.replaceDialog.Close();
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

    protected override void OnClosing(CancelEventArgs e)
    {
      ReplaceDialog.replaceDialog = (ReplaceDialog) null;
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
      this.FindReplaceWorker(false, (Func<int>) (() => this.textEditor.Find(this.FindText, this.MatchCase, false)));
    }

    private void OnReplace()
    {
      this.FindReplaceWorker(true, (Func<int>) (() => this.textEditor.Replace(this.FindText, this.ReplaceText, this.MatchCase)));
    }

    private void FindReplaceWorker(bool isReplacing, Func<int> findReplace)
    {
      int matchOffset = findReplace();
      if (matchOffset == -1)
      {
        int num1 = (int) this.messageManager.ShowMessage(new MessageBoxArgs()
        {
          Owner = this.MessageBoxOwner,
          Message = StringTable.FindDialogCannotFind,
          Button = MessageBoxButton.OK,
          Image = MessageBoxImage.Asterisk
        });
      }
      else
      {
        if (isReplacing && matchOffset < this.findReplaceModel.MatchOffset)
          this.findReplaceModel.UpdateMatchOffset(this.ReplaceText.Length - this.FindText.Length);
        if (this.findReplaceModel.MatchOffset == matchOffset)
        {
          int num2 = (int) this.messageManager.ShowMessage(new MessageBoxArgs()
          {
            Owner = this.MessageBoxOwner,
            Message = StringTable.FindDialogReachedStart,
            Button = MessageBoxButton.OK,
            Image = MessageBoxImage.Asterisk
          });
        }
        this.findReplaceModel.SetMatchIfFirst(matchOffset);
        this.shouldFocusTextEditor = true;
      }
    }

    private void OnReplaceAll()
    {
      if (!this.textEditor.ReplaceAll(this.FindText, this.ReplaceText, this.MatchCase))
      {
        int num = (int) this.messageManager.ShowMessage(new MessageBoxArgs()
        {
          Owner = (Window) this,
          Message = StringTable.FindDialogCannotFind,
          Button = MessageBoxButton.OK,
          Image = MessageBoxImage.Asterisk
        });
      }
      else
        this.shouldFocusTextEditor = true;
    }
  }
}
