// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.UserInterface.MessageBubbleHelper
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System.Windows;

namespace Microsoft.Expression.Framework.UserInterface
{
  public sealed class MessageBubbleHelper : IMessageBubbleHelper
  {
    private UIElement target;
    private MessageBubble messageBubble;

    public MessageBubbleHelper(UIElement target, IMessageBubbleValidator handler)
    {
      this.target = target;
      handler.Initialize(target, (IMessageBubbleHelper) this);
    }

    void IMessageBubbleHelper.SetContent(MessageBubbleContent content)
    {
      if (content != null)
      {
        if (this.messageBubble == null || !this.messageBubble.IsOpen)
        {
          this.messageBubble = new MessageBubble(this.target, content);
          this.messageBubble.Initialize();
          this.messageBubble.IsOpen = true;
        }
        else
        {
          if (!(this.messageBubble.Content.Message != content.Message) && this.messageBubble.Content.MessageBubbleType == content.MessageBubbleType)
            return;
          this.messageBubble.IsOpen = false;
          this.messageBubble = new MessageBubble(this.target, content);
          this.messageBubble.Initialize();
          this.messageBubble.IsOpen = true;
        }
      }
      else
      {
        if (this.messageBubble == null)
          return;
        this.messageBubble.IsOpen = false;
      }
    }
  }
}
