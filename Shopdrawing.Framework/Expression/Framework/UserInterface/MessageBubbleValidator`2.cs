// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.UserInterface.MessageBubbleValidator`2
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Windows;

namespace Microsoft.Expression.Framework.UserInterface
{
  public sealed class MessageBubbleValidator<TData, TEventArgs> : IMessageBubbleValidator
  {
    private Func<TData> validatedMethod;
    private Func<TData, string> validatorMethod;
    private IMessageBubbleHelper helper;

    public MessageBubbleValidator(Func<TData> validatedMethod, Func<TData, string> validatorMethod)
    {
      if (validatedMethod == null)
        throw new ArgumentNullException("validatedMethod");
      if (validatorMethod == null)
        throw new ArgumentNullException("validatorMethod");
      this.validatedMethod = validatedMethod;
      this.validatorMethod = validatorMethod;
    }

    public void EventHook(object sender, TEventArgs eventArgs)
    {
      this.Validate();
    }

    public bool Validate()
    {
      string message = this.validatorMethod(this.validatedMethod());
      MessageBubbleContent content = (MessageBubbleContent) null;
      if (!string.IsNullOrEmpty(message))
        content = new MessageBubbleContent(message, MessageBubbleType.Error);
      this.helper.SetContent(content);
      return content == null;
    }

    void IMessageBubbleValidator.Initialize(UIElement targetElement, IMessageBubbleHelper helper)
    {
      this.helper = helper;
    }
  }
}
