// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.AdornerSetOrderTokens
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Windows.Design;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  public static class AdornerSetOrderTokens
  {
    private static AdornerSetOrder bottomPriority;
    private static AdornerSetOrder mediumPriority;
    private static AdornerSetOrder topPriority;
    private static AdornerSetOrder boundingBoxPriority;

    public static AdornerSetOrder BottomPriority
    {
      get
      {
        if ((OrderToken) AdornerSetOrderTokens.bottomPriority == (OrderToken) null)
          AdornerSetOrderTokens.bottomPriority = AdornerSetOrder.CreateAbove(AdornerSetOrder.BottomOrder);
        return AdornerSetOrderTokens.bottomPriority;
      }
    }

    public static AdornerSetOrder MediumPriority
    {
      get
      {
        if ((OrderToken) AdornerSetOrderTokens.mediumPriority == (OrderToken) null)
          AdornerSetOrderTokens.mediumPriority = AdornerSetOrder.CreateAbove(AdornerSetOrder.MiddleOrder);
        return AdornerSetOrderTokens.mediumPriority;
      }
    }

    public static AdornerSetOrder TopPriority
    {
      get
      {
        if ((OrderToken) AdornerSetOrderTokens.topPriority == (OrderToken) null)
          AdornerSetOrderTokens.topPriority = AdornerSetOrder.CreateAbove(AdornerSetOrder.TopOrder);
        return AdornerSetOrderTokens.topPriority;
      }
    }

    public static AdornerSetOrder BoundingBoxPriority
    {
      get
      {
        if ((OrderToken) AdornerSetOrderTokens.boundingBoxPriority == (OrderToken) null)
          AdornerSetOrderTokens.boundingBoxPriority = AdornerSetOrder.CreateBelow(AdornerSetOrderTokens.BottomPriority);
        return AdornerSetOrderTokens.boundingBoxPriority;
      }
    }
  }
}
