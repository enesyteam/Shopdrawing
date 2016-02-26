using Microsoft.Expression.Framework;
using Shopdrawing.Framework.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
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

		public string AutomationId
		{
			get
			{
				return this.automationId;
			}
		}

		public string CheckBoxMessage
		{
			get
			{
				return this.checkBoxMessage;
			}
		}

		public ICollection<ICommand> Choices
		{
			get
			{
				return this.choices;
			}
		}

		public string Content
		{
			get
			{
				return this.content;
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

		public string HelpMessage
		{
			get
			{
				return this.helpMessage;
			}
		}

		public string HyperlinkMessage
		{
			get;
			set;
		}

		public Uri HyperlinkUri
		{
			get;
			set;
		}

		public MessageIcon Icon
		{
			get
			{
				return this.icon;
			}
		}

		public MessageChoice Result
		{
			get
			{
				return this.result;
			}
		}

		public bool ShowHyperlink
		{
			get;
			set;
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

		public MessageWindow(Window parent, MessageIcon icon, string title, string content, MessageChoice choices, string helpMessage) : this(parent, icon, title, content, choices | MessageChoice.Help)
		{
			this.helpMessage = helpMessage;
		}

		public MessageWindow(IMessageDisplayService messageDisplayService, string checkBoxMessage, Window parent, MessageIcon icon, string title, string content, string automationId, MessageChoice choices, IDictionary<MessageChoice, string> textOverrides) : this(messageDisplayService, parent, icon, title, content, automationId, choices, textOverrides)
		{
			this.checkBoxMessage = checkBoxMessage;
		}

		public MessageWindow(IMessageDisplayService messageDisplayService, string checkBoxMessage, Window parent, MessageIcon icon, string title, string content, string automationId, MessageChoice choices, IDictionary<MessageChoice, string> textOverrides, MessageChoice defaultChoice) : this(messageDisplayService, parent, icon, title, content, automationId, choices, textOverrides, defaultChoice)
		{
			this.checkBoxMessage = checkBoxMessage;
		}

		public MessageWindow(Window parent, MessageIcon icon, string title, string content, MessageChoice choices) : this(null, parent, icon, title, content, null, choices, new Dictionary<MessageChoice, string>())
		{
		}

		public MessageWindow(IMessageDisplayService messageDisplayService, Window parent, MessageIcon icon, string title, string content, string automationId, MessageChoice choices, IDictionary<MessageChoice, string> textOverrides) : this(messageDisplayService, parent, icon, title, content, automationId, choices, textOverrides, MessageChoice.OK | MessageChoice.Yes)
		{
		}

		private MessageWindow(IMessageDisplayService messageDisplayService, Window parent, MessageIcon icon, string title, string content, string automationId, MessageChoice choices, IDictionary<MessageChoice, string> textOverrides, MessageChoice defaultChoice)
		{
			string item;
			bool flag = (MessageWindow.NumberOfChoices(defaultChoice) <= 1 || defaultChoice == (MessageChoice.OK | MessageChoice.Yes) ? true : defaultChoice == (MessageChoice.Cancel | MessageChoice.No));
			if (MessageWindow.IsSet(choices, MessageChoice.OK | MessageChoice.Yes) || choices == MessageChoice.Help || !flag)
			{
				throw new ArgumentException("Invalid set of choices for MessageWindow");
			}
			this.window = new MessageWindowDialog(messageDisplayService)
			{
				Owner = parent,
				WindowStartupLocation = (parent == null || parent.WindowState == WindowState.Maximized ? WindowStartupLocation.CenterScreen : WindowStartupLocation.CenterOwner),
				DataContext = this
			};
			this.icon = icon;
			this.title = title;
			this.content = content;
			this.automationId = automationId;
			this.choices = new List<ICommand>();
			if (MessageWindow.IsSet(choices, MessageChoice.Yes))
			{
				string messageWindowYesLabel = StringTable.MessageWindowYesLabel;
				if (textOverrides != null && textOverrides.ContainsKey(MessageChoice.Yes))
				{
					messageWindowYesLabel = textOverrides[MessageChoice.Yes];
				}
				bool flag1 = MessageWindow.IsSet(defaultChoice, MessageChoice.Yes);
				this.choices.Add(new MessageWindow.ChoiceCommand(new MessageWindow.ButtonCommandHandler(this.CloseCommand), flag1, false, messageWindowYesLabel, "Yes", MessageChoice.Yes));
			}
			if (MessageWindow.IsSet(choices, MessageChoice.No))
			{
				string messageWindowNoLabel = StringTable.MessageWindowNoLabel;
				if (textOverrides != null && textOverrides.ContainsKey(MessageChoice.No))
				{
					messageWindowNoLabel = textOverrides[MessageChoice.No];
				}
				bool flag2 = MessageWindow.IsSet(defaultChoice, MessageChoice.No);
				this.choices.Add(new MessageWindow.ChoiceCommand(new MessageWindow.ButtonCommandHandler(this.CloseCommand), flag2, !MessageWindow.IsSet(choices, MessageChoice.Cancel), messageWindowNoLabel, "No", MessageChoice.No));
			}
			if (MessageWindow.IsSet(choices, MessageChoice.OK))
			{
				item = (choices != MessageChoice.OK || icon != MessageIcon.Error && icon != MessageIcon.Warning ? StringTable.MessageWindowOKLabel : StringTable.MessageWindowCloseLabel);
				if (textOverrides != null && textOverrides.ContainsKey(MessageChoice.OK))
				{
					item = textOverrides[MessageChoice.OK];
				}
				bool flag3 = MessageWindow.IsSet(defaultChoice, MessageChoice.OK);
				this.choices.Add(new MessageWindow.ChoiceCommand(new MessageWindow.ButtonCommandHandler(this.CloseCommand), flag3, !MessageWindow.IsSet(choices, MessageChoice.Cancel), item, "OK", MessageChoice.OK));
			}
			if (MessageWindow.IsSet(choices, MessageChoice.Cancel))
			{
				string messageWindowCancelLabel = StringTable.MessageWindowCancelLabel;
				if (textOverrides != null && textOverrides.ContainsKey(MessageChoice.Cancel))
				{
					messageWindowCancelLabel = textOverrides[MessageChoice.Cancel];
				}
				bool flag4 = MessageWindow.IsSet(defaultChoice, MessageChoice.Cancel);
				this.choices.Add(new MessageWindow.ChoiceCommand(new MessageWindow.ButtonCommandHandler(this.CloseCommand), (flag4 ? true : choices == MessageChoice.Cancel), true, messageWindowCancelLabel, "Cancel", MessageChoice.Cancel));
			}
			if (MessageWindow.IsSet(choices, MessageChoice.Help))
			{
				string messageWindowHelpLabel = StringTable.MessageWindowHelpLabel;
				if (textOverrides != null && textOverrides.ContainsKey(MessageChoice.Help))
				{
					messageWindowHelpLabel = textOverrides[MessageChoice.Help];
				}
				bool flag5 = MessageWindow.IsSet(defaultChoice, MessageChoice.Help);
				this.choices.Add(new MessageWindow.ChoiceCommand(new MessageWindow.ButtonCommandHandler(this.HelpCommand), (flag5 ? true : choices == MessageChoice.Help), false, messageWindowHelpLabel, "Help", MessageChoice.Help));
			}
			this.result = MessageChoice.None;
			this.checkBoxMessage = "sss";
			ItemsControl itemsControl = (ItemsControl)this.window.FindName("ButtonsControl");
			itemsControl.ItemContainerGenerator.StatusChanged += new EventHandler(this.ItemContainerGenerator_StatusChanged);
		}

		public void Close(MessageChoice result)
		{
			this.result = result;
			this.window.Close();
			this.window.Owner = null;
		}

		private void CloseCommand(object target)
		{
			this.Close((target as MessageWindow.ChoiceCommand).Result);
		}

		private void HelpCommand(object target)
		{
			MessageWindow.Show(this.window, MessageIcon.None, this.title, this.helpMessage, MessageChoice.OK);
		}

		private static bool IsSet(MessageChoice choices, MessageChoice bits)
		{
			return (choices & bits) == bits;
		}

		private void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
		{
			ItemsControl itemsControl = (ItemsControl)this.window.FindName("ButtonsControl");
			if (itemsControl.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
			{
				foreach (MessageWindow.ChoiceCommand item in (IEnumerable)itemsControl.Items)
				{
					ContentPresenter contentPresenter = (ContentPresenter)itemsControl.ItemContainerGenerator.ContainerFromItem(item);
					contentPresenter.ApplyTemplate();
					if (VisualTreeHelper.GetChildrenCount(contentPresenter) <= 0)
					{
						continue;
					}
					Button child = (Button)VisualTreeHelper.GetChild(contentPresenter, 0);
					if (!child.IsDefault)
					{
						continue;
					}
					Keyboard.Focus(child);
					break;
				}
			}
		}

		private static int NumberOfChoices(MessageChoice choices)
		{
			int num = 0;
			for (int i = (int)choices; i > 0; i = i >> 1)
			{
				num = num + (i % 2 == 1 ? 1 : 0);
			}
			return num;
		}

		private void OnPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		public static MessageChoice Show(Window parent, MessageIcon icon, string title, string content, MessageChoice choices)
		{
			MessageWindow messageWindow = new MessageWindow(parent, icon, title, content, choices);
			messageWindow.ShowDialog();
			return messageWindow.Result;
		}

		public void Show()
		{
			this.window.Show();
		}

		public void ShowDialog()
		{
			try
			{
				this.window.ShowDialog();
			}
			catch (Win32Exception win32Exception1)
			{
				Win32Exception win32Exception = win32Exception1;
				if (win32Exception.ErrorCode != -2147467259 || win32Exception.NativeErrorCode != 1400 || !(win32Exception.TargetSite.Name == "PostMessage"))
				{
					throw;
				}
			}
		}

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

		public event PropertyChangedEventHandler PropertyChanged;

		public delegate void ButtonCommandHandler(object target);

		private sealed class ChoiceCommand : ICommand
		{
			private bool isDefault;

			private bool isCancel;

			private string label;

			private string automationId;

			private MessageWindow.ButtonCommandHandler commandHandler;

			private MessageChoice result;

			public string AutomationId
			{
				get
				{
					return this.automationId;
				}
			}

			public bool IsCancel
			{
				get
				{
					return this.isCancel;
				}
			}

			public bool IsDefault
			{
				get
				{
					return this.isDefault;
				}
			}

			public string Label
			{
				get
				{
					return this.label;
				}
			}

			public MessageChoice Result
			{
				get
				{
					return this.result;
				}
			}

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
				this.commandHandler(this);
			}

			public event EventHandler CanExecuteChanged;
		}
	}
}