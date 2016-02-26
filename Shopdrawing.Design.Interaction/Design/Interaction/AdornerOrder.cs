// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Interaction.AdornerOrder
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design;
using System;

namespace Microsoft.Windows.Design.Interaction
{
  public sealed class AdornerOrder : OrderToken
  {
    private static AdornerOrder _foreground;
    private static AdornerOrder _content;
    private static AdornerOrder _background;

    public static AdornerOrder Background
    {
      get
      {
        if ((OrderToken) AdornerOrder._background == (OrderToken) null)
          AdornerOrder._background = new AdornerOrder(OrderTokenPrecedence.After, (OrderToken) AdornerOrder.Content, OrderTokenConflictResolution.Win);
        return AdornerOrder._background;
      }
    }

    public static AdornerOrder Content
    {
      get
      {
        if ((OrderToken) AdornerOrder._content == (OrderToken) null)
          AdornerOrder._content = new AdornerOrder(OrderTokenPrecedence.After, (OrderToken) null, OrderTokenConflictResolution.Win);
        return AdornerOrder._content;
      }
    }

    public static AdornerOrder Foreground
    {
      get
      {
        if ((OrderToken) AdornerOrder._foreground == (OrderToken) null)
          AdornerOrder._foreground = new AdornerOrder(OrderTokenPrecedence.Before, (OrderToken) AdornerOrder.Content, OrderTokenConflictResolution.Win);
        return AdornerOrder._foreground;
      }
    }

    private AdornerOrder(OrderTokenPrecedence precedence, OrderToken reference, OrderTokenConflictResolution conflictResolution)
      : base(precedence, reference, conflictResolution)
    {
    }

    public static AdornerOrder CreateAbove(AdornerOrder reference)
    {
      if ((OrderToken) reference == (OrderToken) null)
        throw new ArgumentNullException("reference");
      return new AdornerOrder(OrderTokenPrecedence.Before, (OrderToken) reference, OrderTokenConflictResolution.Lose);
    }

    public static AdornerOrder CreateBelow(AdornerOrder reference)
    {
      if ((OrderToken) reference == (OrderToken) null)
        throw new ArgumentNullException("reference");
      return new AdornerOrder(OrderTokenPrecedence.After, (OrderToken) reference, OrderTokenConflictResolution.Lose);
    }
  }
}
