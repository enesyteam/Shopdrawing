// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Interaction.PlacementAdapter
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design.Model;
using System;
using System.Windows;

namespace Microsoft.Windows.Design.Interaction
{
  public abstract class PlacementAdapter : Adapter
  {
    public override Type AdapterType
    {
      get
      {
        return typeof (PlacementAdapter);
      }
    }

    public virtual void BeginPlacement(ModelItem item)
    {
    }

    public virtual void EndPlacement()
    {
    }

    public abstract bool CanSetPosition(PlacementIntent intent, RelativePosition position);

    public abstract RelativeValueCollection GetPlacement(ModelItem item, params RelativePosition[] positions);

    public abstract void SetPlacements(ModelItem item, PlacementIntent intent, params RelativeValue[] positions);

    public abstract void SetPlacements(ModelItem item, PlacementIntent intent, RelativeValueCollection placement);

    public virtual void SetNudgePlacements(ModelItem item, NudgeIntent intent, RelativeValueCollection placement)
    {
      if (item == null)
        throw new ArgumentNullException("item");
      if (placement == null)
        throw new ArgumentNullException("placement");
      if (!EnumValidator.IsValid(intent))
        throw new ArgumentOutOfRangeException("intent");
      this.SetPlacements(item, PlacementIntent.Move, placement);
    }

    public virtual void SetNudgePlacements(ModelItem item, NudgeIntent intent, RelativeValue[] positions)
    {
      if (item == null)
        throw new ArgumentNullException("item");
      if (positions == null)
        throw new ArgumentNullException("positions");
      if (!EnumValidator.IsValid(intent))
        throw new ArgumentOutOfRangeException("intent");
      this.SetPlacements(item, PlacementIntent.Move, positions);
    }

    public abstract Rect GetPlacementBoundary(ModelItem item);

    public abstract Rect GetPlacementBoundary(ModelItem item, PlacementIntent intent, params RelativeValue[] positions);
  }
}
