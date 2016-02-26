// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.UserInterface.MessageBubbleContent
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

namespace Microsoft.Expression.Framework.UserInterface
{
  public sealed class MessageBubbleContent
  {
    private MessageBubbleType messageBubbleType;
    private string message;

    public MessageBubbleType MessageBubbleType
    {
      get
      {
        return this.messageBubbleType;
      }
      set
      {
        this.messageBubbleType = value;
      }
    }

    public string Message
    {
      get
      {
        return this.message;
      }
      set
      {
        this.message = value;
      }
    }

    public MessageBubbleContent(string message, MessageBubbleType messageBubbleType)
    {
      this.message = message;
      this.messageBubbleType = messageBubbleType;
    }
  }
}
