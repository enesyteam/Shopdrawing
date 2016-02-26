// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Controls.MessageWindow
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.Framework.Controls
{
  public sealed class MessageWindow : INotifyPropertyChanged
  {
    private readonly Dialog window;
    private readonly MessageIcon icon;
    private readonly string title;
    private readonly string content;
    private readonly string automationId;
    private readonly List<ICommand> choices;
    private MessageChoice result;
    private bool enableDontAskAgain;
    private bool dontAskAgain;
    private readonly string helpMessage;
    private string checkBoxMessage;

    public MessageChoice Result
    {
      get
      {
        return this.result;
      }
    }

    public MessageIcon Icon
    {
      get
      {
        return this.icon;
      }
    }

    public bool ShowIcon
    {
      get
      {
        return this.icon != MessageIcon.None;
      }
    }

    public string Title
    {
      get
      {
        return this.title;
      }
    }

    public string Content
    {
      get
      {
        return this.content;
      }
    }

    public string AutomationId
    {
      get
      {
        return this.automationId;
      }
    }

    public ICollection<ICommand> Choices
    {
      get
      {
        return (ICollection<ICommand>) this.choices;
      }
    }

    public string CheckBoxMessage
    {
      get
      {
        return this.checkBoxMessage;
      }
    }

    public bool EnableDontAskAgain
    {
      get
      {
        return this.enableDontAskAgain;
      }
      set
      {
        this.enableDontAskAgain = value;
        this.OnPropertyChanged("EnableDontAskAgain");
      }
    }

    public bool DontAskAgain
    {
      get
      {
        return this.dontAskAgain;
      }
      set
      {
        this.dontAskAgain = value;
        this.OnPropertyChanged("DontAskAgain");
      }
    }

    public bool ShowHyperlink { get; set; }

    public string HyperlinkMessage { get; set; }

    public Uri HyperlinkUri { get; set; }

    public string HelpMessage
    {
      get
      {
        return this.helpMessage;
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public event EventHandler Closed
    {
      add
      {
        this.window.Closed += value;
      }
      remove
      {
        this.window.Closed -= value;
      }
    }

    public MessageWindow(Window parent, MessageIcon icon, string title, string content, MessageChoice choices, string helpMessage)
      : this(parent, icon, title, content, choices | MessageChoice.Help)
    {
      this.helpMessage = helpMessage;
    }

    public MessageWindow(IMessageDisplayService messageDisplayService, string checkBoxMessage, Window parent, MessageIcon icon, string title, string content, string automationId, MessageChoice choices, IDictionary<MessageChoice, string> textOverrides)
      : this(messageDisplayService, parent, icon, title, content, automationId, choices, textOverrides)
    {
      this.checkBoxMessage = checkBoxMessage;
    }

    public MessageWindow(IMessageDisplayService messageDisplayService, string checkBoxMessage, Window parent, MessageIcon icon, string title, string content, string automationId, MessageChoice choices, IDictionary<MessageChoice, string> textOverrides, MessageChoice defaultChoice)
      : this(messageDisplayService, parent, icon, title, content, automationId, choices, textOverrides, defaultChoice)
    {
      this.checkBoxMessage = checkBoxMessage;
    }

    public MessageWindow(Window parent, MessageIcon icon, string title, string content, MessageChoice choices)
      : this((IMessageDisplayService) null, parent, icon, title, content, (string) null, choices, (IDictionary<MessageChoice, string>) new Dictionary<MessageChoice, string>())
    {
    }

    public MessageWindow(IMessageDisplayService messageDisplayService, Window parent, MessageIcon icon, string title, string content, string automationId, MessageChoice choices, IDictionary<MessageChoice, string> textOverrides)
      : this(messageDisplayService, parent, icon, title, content, automationId, choices, textOverrides, MessageChoice.OK | MessageChoice.Yes)
    {
    }

    private MessageWindow(IMessageDisplayService messageDisplayService, Window parent, MessageIcon icon, string title, string content, string automationId, MessageChoice choices, IDictionary<MessageChoice, string> textOverrides, MessageChoice defaultChoice)
    {
      bool flag = MessageWindow.NumberOfChoices(defaultChoice) <= 1 || defaultChoice == (MessageChoice.OK | MessageChoice.Yes) || defaultChoice == (MessageChoice.Cancel | MessageChoice.No);
      if (MessageWindow.IsSet(choices, MessageChoice.OK | MessageChoice.Yes) || choices == MessageChoice.Help || !flag)
        throw new ArgumentException("Invalid set of choices for MessageWindow");
      this.window = (Dialog) new MessageWindowDialog(messageDisplayService);
      this.window.Owner = parent;
      this.window.WindowStartupLocation = parent == null || parent.WindowState == WindowState.Maximized ? WindowStartupLocation.CenterScreen : WindowStartupLocation.CenterOwner;
      this.window.DataContext = (object) this;
      this.icon = icon;
      this.title = title;
      this.content = content;
      this.automationId = automationId;
      this.choices = new List<ICommand>();
      if (MessageWindow.IsSet(choices, MessageChoice.Yes))
      {
        string label = StringTable.MessageWindowYesLabel;
        if (textOverrides != null && textOverrides.ContainsKey(MessageChoice.Yes))
          label = textOverrides[MessageChoice.Yes];
        this.choices.Add((ICommand) new MessageWindow.ChoiceCommand(new MessageWindow.ButtonCommandHandler(this.CloseCommand), MessageWindow.IsSet(defaultChoice, MessageChoice.Yes), false, label, "Yes", MessageChoice.Yes));
      }
      if (MessageWindow.IsSet(choices, MessageChoice.No))
      {
        string label = StringTable.MessageWindowNoLabel;
        if (textOverrides != null && textOverrides.ContainsKey(MessageChoice.No))
          label = textOverrides[MessageChoice.No];
        this.choices.Add((ICommand) new MessageWindow.ChoiceCommand(new MessageWindow.ButtonCommandHandler(this.CloseCommand), MessageWindow.IsSet(defaultChoice, MessageChoice.No), !MessageWindow.IsSet(choices, MessageChoice.Cancel), label, "No", MessageChoice.No));
      }
      if (MessageWindow.IsSet(choices, MessageChoice.OK))
      {
        string label = choices != MessageChoice.OK || icon != MessageIcon.Error && icon != MessageIcon.Warning ? StringTable.MessageWindowOKLabel : StringTable.MessageWindowCloseLabel;
        if (textOverrides != null && textOverrides.ContainsKey(MessageChoice.OK))
          label = textOverrides[MessageChoice.OK];
        this.choices.Add((ICommand) new MessageWindow.ChoiceCommand(new MessageWindow.ButtonCommandHandler(this.CloseCommand), MessageWindow.IsSet(defaultChoice, MessageChoice.OK), !MessageWindow.IsSet(choices, MessageChoice.Cancel), label, "OK", MessageChoice.OK));
      }
      if (MessageWindow.IsSet(choices, MessageChoice.Cancel))
      {
        string label = StringTable.MessageWindowCancelLabel;
        if (textOverrides != null && textOverrides.ContainsKey(MessageChoice.Cancel))
          label = textOverrides[MessageChoice.Cancel];
        this.choices.Add((ICommand) new MessageWindow.ChoiceCommand(new MessageWindow.ButtonCommandHandler(this.CloseCommand), MessageWindow.IsSet(defaultChoice, MessageChoice.Cancel) || choices == MessageChoice.Cancel, true, label, "Cancel", MessageChoice.Cancel));
      }
      if (MessageWindow.IsSet(choices, MessageChoice.Help))
      {
        string label = StringTable.MessageWindowHelpLabel;
        if (textOverrides != null && textOverrides.ContainsKey(MessageChoice.Help))
          label = textOverrides[MessageChoice.Help];
        this.choices.Add((ICommand) new MessageWindow.ChoiceCommand(new MessageWindow.ButtonCommandHandler(this.HelpCommand), MessageWindow.IsSet(defaultChoice, MessageChoice.Help) || choices == MessageChoice.Help, false, label, "Help", MessageChoice.Help));
      }
      this.result = MessageChoice.None;
      this.checkBoxMessage = StringTable.DefaultMessageWindowCheckboxMessage;
      ((ItemsControl) this.window.FindName("ButtonsControl")).ItemContainerGenerator.StatusChanged += new EventHandler(this.ItemContainerGenerator_StatusChanged);
    }

    public static MessageChoice Show(Window parent, MessageIcon icon, string title, string content, MessageChoice choices)
    {
      MessageWindow messageWindow = new MessageWindow(parent, icon, title, content, choices);
      messageWindow.ShowDialog();
      return messageWindow.Result;
    }

    private void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
    {
      ItemsControl itemsControl = (ItemsControl) this.window.FindName("ButtonsControl");
      if (itemsControl.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
        return;
      foreach (MessageWindow.ChoiceCommand choiceCommand in (IEnumerable) itemsControl.Items)
      {
        ContentPresenter contentPresenter = (ContentPresenter) itemsControl.ItemContainerGenerator.ContainerFromItem((object) choiceCommand);
        contentPresenter.ApplyTemplate();
        if (VisualTreeHelper.GetChildrenCount((DependencyObject) contentPresenter) > 0)
        {
          Button button = (Button) VisualTreeHelper.GetChild((DependencyObject) contentPresenter, 0);
          if (button.IsDefault)
          {
            Keyboard.Focus((IInputElement) button);
            break;
          }
        }
      }
    }

    public void ShowDialog()
    {
      try
      {
        this.window.ShowDialog();
      }
      catch (Win32Exception ex)
      {
        if (ex.ErrorCode == -2147467259 && ex.NativeErrorCode == 1400 && ex.TargetSite.Name == "PostMessage")
          return;
        throw;
      }
    }

    public void Show()
    {
      this.window.Show();
    }

    public void Close(MessageChoice result)
    {
      this.result = result;
      this.window.Close();
      this.window.Owner = (Window) null;
    }

    private static int NumberOfChoices(MessageChoice choices)
    {
      int num1 = 0;
      int num2 = (int) choices;
      while (num2 > 0)
      {
        num1 += num2 % 2 == 1 ? 1 : 0;
        num2 >>= 1;
      }
      return num1;
    }

    private static bool IsSet(MessageChoice choices, MessageChoice bits)
    {
      return (choices & bits) == bits;
    }

    private void CloseCommand(object target)
    {
      this.Close((target as MessageWindow.ChoiceCommand).Result);
    }

    private void HelpCommand(object target)
    {
      int num = (int) MessageWindow.Show((Window) this.window, MessageIcon.None, this.title, this.helpMessage, MessageChoice.OK);
    }

    private void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }

    public delegate void ButtonCommandHandler(object target);

    private sealed class ChoiceCommand : ICommand
    {
      private bool isDefault;
      private bool isCancel;
      private string label;
      private string automationId;
      private MessageWindow.ButtonCommandHandler commandHandler;
      private MessageChoice result;

      public MessageChoice Result
      {
        get
        {
          return this.result;
        }
      }

      public bool IsDefault
      {
        get
        {
          return this.isDefault;
        }
      }

      public bool IsCancel
      {
        get
        {
          return this.isCancel;
        }
      }

      public string Label
      {
        get
        {
          return this.label;
        }
      }

      public string AutomationId
      {
        get
        {
          return this.automationId;
        }
      }

      public event EventHandler CanExecuteChanged;

      public ChoiceCommand(MessageWindow.ButtonCommandHandler commandHandler, bool isDefault, bool isCancel, string label, string automationId, MessageChoice result)
      {
        this.commandHandler += commandHandler;
        this.isDefault = isDefault;
        this.isCancel = isCancel;
        this.label = label;
        this.automationId = automationId;
        this.result = result;
      }

      public bool CanExecute(object parameter)
      {
        return true;
      }

      public void Execute(object parameter)
      {
        this.commandHandler((object) this);
      }
    }
  }
}
