// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.MessageBoxArgs
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework.Controls;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Microsoft.Expression.Framework
{
  public class MessageBoxArgs
  {
    private MessageBoxButton button;
    private MessageBoxImage image;

    public MessageBoxButton Button
    {
      get
      {
        return this.button;
      }
      set
      {
        this.button = value;
      }
    }

    public MessageBoxImage Image
    {
      get
      {
        return this.image;
      }
      set
      {
        this.image = value;
      }
    }

    public Window Owner { get; set; }

    public string CheckBoxMessage { get; set; }

    public string Message { get; set; }

    public string AutomationId { get; set; }

    public string HyperlinkMessage { get; set; }

    public Uri HyperlinkUri { get; set; }

    public IDictionary<MessageChoice, string> TextOverrides { get; set; }
  }
}
