// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.PropertyEditing.PropertyOrder
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design;
using System;

namespace Microsoft.Windows.Design.PropertyEditing
{
  public sealed class PropertyOrder : OrderToken
  {
    private static PropertyOrder _early;
    private static PropertyOrder _default;
    private static PropertyOrder _late;

    public static PropertyOrder Early
    {
      get
      {
        if ((OrderToken) PropertyOrder._early == (OrderToken) null)
          PropertyOrder._early = new PropertyOrder(OrderTokenPrecedence.Before, (OrderToken) PropertyOrder.Default, OrderTokenConflictResolution.Win);
        return PropertyOrder._early;
      }
    }

    public static PropertyOrder Default
    {
      get
      {
        if ((OrderToken) PropertyOrder._default == (OrderToken) null)
          PropertyOrder._default = new PropertyOrder(OrderTokenPrecedence.After, (OrderToken) null, OrderTokenConflictResolution.Win);
        return PropertyOrder._default;
      }
    }

    public static PropertyOrder Late
    {
      get
      {
        if ((OrderToken) PropertyOrder._late == (OrderToken) null)
          PropertyOrder._late = new PropertyOrder(OrderTokenPrecedence.After, (OrderToken) PropertyOrder.Default, OrderTokenConflictResolution.Win);
        return PropertyOrder._late;
      }
    }

    private PropertyOrder(OrderTokenPrecedence precedence, OrderToken reference, OrderTokenConflictResolution conflictResolution)
      : base(precedence, reference, conflictResolution)
    {
    }

    public static PropertyOrder CreateBefore(PropertyOrder reference)
    {
      if ((OrderToken) reference == (OrderToken) null)
        throw new ArgumentNullException("reference");
      return new PropertyOrder(OrderTokenPrecedence.Before, (OrderToken) reference, OrderTokenConflictResolution.Lose);
    }

    public static PropertyOrder CreateAfter(PropertyOrder reference)
    {
      if ((OrderToken) reference == (OrderToken) null)
        throw new ArgumentNullException("reference");
      return new PropertyOrder(OrderTokenPrecedence.After, (OrderToken) reference, OrderTokenConflictResolution.Lose);
    }

    protected override int ResolveConflict(OrderToken left, OrderToken right)
    {
      return 0;
    }
  }
}
