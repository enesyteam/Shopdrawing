// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.UserInterface.MessageDisplayService
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Windows;
using System.Windows.Input;

namespace Microsoft.Expression.Framework.UserInterface
{
  [Export(typeof (IMessageDisplayService))]
  public class MessageDisplayService : IMessageDisplayService
  {
    private bool useWin32MessageBox;
    private string defaultCaption;

    public bool UseWin32MessageBox
    {
      get
      {
        return this.useWin32MessageBox;
      }
      set
      {
        this.useWin32MessageBox = value;
      }
    }

    public MessageDisplayService()
    {
      this.defaultCaption = StringTable.DefaultMessageBoxCaption;
    }

    public MessageDisplayService(IExpressionInformationService expressionInformationService)
    {
      this.defaultCaption = expressionInformationService.DefaultDialogTitle;
    }

    private MessageChoice GetMessageChoicesFromMessageBoxButton(MessageBoxButton button)
    {
      MessageChoice messageChoice;
      switch (button)
      {
        case MessageBoxButton.OK:
          messageChoice = MessageChoice.OK;
          break;
        case MessageBoxButton.OKCancel:
          messageChoice = MessageChoice.OK | MessageChoice.Cancel;
          break;
        case MessageBoxButton.YesNoCancel:
          messageChoice = MessageChoice.Cancel | MessageChoice.Yes | MessageChoice.No;
          break;
        case MessageBoxButton.YesNo:
          messageChoice = MessageChoice.Yes | MessageChoice.No;
          break;
        default:
          messageChoice = MessageChoice.None;
          break;
      }
      return messageChoice;
    }

    private MessageIcon GetMessageIconFromMessageBoxImage(MessageBoxImage image)
    {
      MessageIcon messageIcon;
      switch (image)
      {
        case MessageBoxImage.Exclamation:
          messageIcon = MessageIcon.Warning;
          break;
        case MessageBoxImage.Asterisk:
          messageIcon = MessageIcon.Information;
          break;
        case MessageBoxImage.Hand:
          messageIcon = MessageIcon.Error;
          break;
        case MessageBoxImage.Question:
          messageIcon = MessageIcon.Question;
          break;
        default:
          messageIcon = MessageIcon.None;
          break;
      }
      return messageIcon;
    }

    private MessageBoxResult GetMessageBoxResultFromMessageChoice(MessageChoice choice)
    {
      MessageBoxResult messageBoxResult = MessageBoxResult.None;
      switch (choice)
      {
        case MessageChoice.OK:
          messageBoxResult = MessageBoxResult.OK;
          break;
        case MessageChoice.Cancel:
          messageBoxResult = MessageBoxResult.Cancel;
          break;
        case MessageChoice.Yes:
          messageBoxResult = MessageBoxResult.Yes;
          break;
        case MessageChoice.No:
          messageBoxResult = MessageBoxResult.No;
          break;
      }
      return messageBoxResult;
    }

    public MessageBoxResult ShowMessage(string message)
    {
      return this.ShowMessage(message, (string) null);
    }

    public MessageBoxResult ShowMessage(string message, string caption)
    {
      return this.ShowMessage(message, caption, MessageBoxButton.OK);
    }

    public MessageBoxResult ShowMessage(string message, string caption, MessageBoxButton button)
    {
      return this.ShowMessage(message, caption, button, MessageBoxImage.None);
    }

    public MessageBoxResult ShowMessage(string message, string caption, MessageBoxButton button, MessageBoxImage image)
    {
      return this.ShowMessage((Window) null, message, caption, button, image);
    }

    public MessageBoxResult ShowMessage(string message, string caption, MessageBoxButton button, MessageBoxImage image, MessageChoice defaultChoice)
    {
      return this.ShowMessage((Window) null, message, caption, button, image, defaultChoice);
    }

    public MessageBoxResult ShowMessage(Window owner, string message, string caption, MessageBoxButton button, MessageBoxImage image)
    {
      return this.ShowMessage(owner, message, caption, button, image, MessageChoice.OK | MessageChoice.Yes);
    }

    public MessageBoxResult ShowMessage(Window owner, string message, string caption, MessageBoxButton button, MessageBoxImage image, MessageChoice defaultChoice)
    {
      bool dontAskAgain;
      return this.ShowInternalMessage(owner, (string) null, message, caption, (string) null, button, image, (string) null, (Uri) null, (IDictionary<MessageChoice, string>) null, defaultChoice, out dontAskAgain);
    }

    public MessageBoxResult ShowMessage(MessageBoxArgs args)
    {
      bool dontAskAgain;
      return this.ShowInternalMessage(args.Owner, (string) null, args.Message, (string) null, args.AutomationId, args.Button, args.Image, args.HyperlinkMessage, args.HyperlinkUri, args.TextOverrides, out dontAskAgain);
    }

    public MessageBoxResult ShowMessage(MessageBoxArgs args, out bool dontAskAgain)
    {
      string checkBoxMessage = args.CheckBoxMessage ?? StringTable.DefaultMessageWindowCheckboxMessage;
      return this.ShowInternalMessage(args.Owner, checkBoxMessage, args.Message, (string) null, args.AutomationId, args.Button, args.Image, args.HyperlinkMessage, args.HyperlinkUri, args.TextOverrides, out dontAskAgain);
    }

    public void ShowError(string message)
    {
      this.ShowError(message, (string) null);
    }

    public void ShowError(string message, string caption)
    {
      int num = (int) this.ShowMessage(message, caption, MessageBoxButton.OK, MessageBoxImage.Hand);
    }

    public void ShowError(string message, Exception exception, string caption)
    {
      this.ShowErrorInternal(message, exception, caption, (string) null);
    }

    public void ShowError(ErrorArgs args)
    {
      this.ShowErrorInternal(args.Message, args.Exception, (string) null, args.AutomationId);
    }

    private void ShowErrorInternal(string message, Exception exception, string caption, string automationId)
    {
      if (exception != null)
      {
        if (exception.InnerException != null)
          message = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.ErrorMessageWithInnerException, (object) message, (object) exception.Message, (object) exception.InnerException.Message);
        else
          message = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.ErrorMessageWithException, new object[2]
          {
            (object) message,
            (object) exception.Message
          });
      }
      bool dontAskAgain;
      int num = (int) this.ShowInternalMessage((Window) null, (string) null, message, caption, automationId, MessageBoxButton.OK, MessageBoxImage.Hand, (string) null, (Uri) null, (IDictionary<MessageChoice, string>) null, out dontAskAgain);
    }

    private MessageBoxResult ShowInternalMessage(Window owner, string checkBoxMessage, string message, string caption, string automationId, MessageBoxButton button, MessageBoxImage image, string hyperlinkMessage, Uri hyperlinkUri, IDictionary<MessageChoice, string> textOverrides, out bool dontAskAgain)
    {
      return this.ShowInternalMessage(owner, checkBoxMessage, message, caption, automationId, button, image, hyperlinkMessage, hyperlinkUri, textOverrides, MessageChoice.OK | MessageChoice.Yes, out dontAskAgain);
    }

    private MessageBoxResult ShowInternalMessage(Window owner, string checkBoxMessage, string message, string caption, string automationId, MessageBoxButton button, MessageBoxImage image, string hyperlinkMessage, Uri hyperlinkUri, IDictionary<MessageChoice, string> textOverrides, MessageChoice defaultChoice, out bool dontAskAgain)
    {
      if (owner == null)
        owner = this.useWin32MessageBox ? (Window) null : Dialog.ActiveModalWindow;
      if (caption == null)
        caption = this.defaultCaption;
      bool flag1 = !string.IsNullOrEmpty(checkBoxMessage);
      int num1 = flag1 ? 1 : 0;
      bool flag2 = !string.IsNullOrEmpty(hyperlinkMessage);
      int num2 = flag2 ? 1 : 0;
      if (this.useWin32MessageBox || owner == null)
      {
        dontAskAgain = false;
        return Win32MessageBox.Show(message, caption, button, image);
      }
      MessageIcon fromMessageBoxImage = this.GetMessageIconFromMessageBoxImage(image);
      MessageChoice messageBoxButton = this.GetMessageChoicesFromMessageBoxButton(button);
      MessageWindow messageWindow = new MessageWindow((IMessageDisplayService) this, checkBoxMessage, owner, fromMessageBoxImage, caption, message, automationId, messageBoxButton, textOverrides, defaultChoice);
      messageWindow.EnableDontAskAgain = flag1;
      messageWindow.ShowHyperlink = flag2;
      messageWindow.HyperlinkMessage = hyperlinkMessage;
      messageWindow.HyperlinkUri = hyperlinkUri;
      using (TemporaryCursor.SetCursor(Cursors.Arrow))
        messageWindow.ShowDialog();
      dontAskAgain = messageWindow.DontAskAgain;
      return this.GetMessageBoxResultFromMessageChoice(messageWindow.Result);
    }
  }
}
