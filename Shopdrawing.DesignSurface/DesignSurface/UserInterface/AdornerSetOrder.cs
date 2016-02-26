// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.AdornerSetOrder
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Windows.Design;
using System;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  public sealed class AdornerSetOrder : OrderToken
  {
    private static AdornerSetOrder middleOrder;
    private static AdornerSetOrder bottomOrder;
    private static AdornerSetOrder topOrder;

    public static AdornerSetOrder MiddleOrder
    {
      get
      {
        if ((OrderToken) AdornerSetOrder.middleOrder == (OrderToken) null)
          AdornerSetOrder.middleOrder = new AdornerSetOrder(OrderTokenPrecedence.After, (OrderToken) null, OrderTokenConflictResolution.Win);
        return AdornerSetOrder.middleOrder;
      }
    }

    public static AdornerSetOrder BottomOrder
    {
      get
      {
        if ((OrderToken) AdornerSetOrder.bottomOrder == (OrderToken) null)
          AdornerSetOrder.bottomOrder = new AdornerSetOrder(OrderTokenPrecedence.Before, (OrderToken) AdornerSetOrder.MiddleOrder, OrderTokenConflictResolution.Win);
        return AdornerSetOrder.bottomOrder;
      }
    }

    public static AdornerSetOrder TopOrder
    {
      get
      {
        if ((OrderToken) AdornerSetOrder.topOrder == (OrderToken) null)
          AdornerSetOrder.topOrder = new AdornerSetOrder(OrderTokenPrecedence.After, (OrderToken) AdornerSetOrder.MiddleOrder, OrderTokenConflictResolution.Win);
        return AdornerSetOrder.topOrder;
      }
    }

    private AdornerSetOrder(OrderTokenPrecedence precedence, OrderToken reference, OrderTokenConflictResolution conflictResolution)
      : base(precedence, reference, conflictResolution)
    {
    }

    public static AdornerSetOrder CreateAbove(AdornerSetOrder reference)
    {
      if ((OrderToken) reference == (OrderToken) null)
        throw new ArgumentNullException("reference");
      return new AdornerSetOrder(OrderTokenPrecedence.After, (OrderToken) reference, OrderTokenConflictResolution.Lose);
    }

    public static AdornerSetOrder CreateBelow(AdornerSetOrder reference)
    {
      if ((OrderToken) reference == (OrderToken) null)
        throw new ArgumentNullException("reference");
      return new AdornerSetOrder(OrderTokenPrecedence.Before, (OrderToken) reference, OrderTokenConflictResolution.Lose);
    }

    protected override int ResolveConflict(OrderToken left, OrderToken right)
    {
      return 0;
    }
  }
}
