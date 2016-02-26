// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Interaction.EnumValidator
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

namespace Microsoft.Windows.Design.Interaction
{
  internal static class EnumValidator
  {
    public static bool IsValid(AdornerPlacementDimension value)
    {
      if (value >= AdornerPlacementDimension.Left)
        return value <= AdornerPlacementDimension.Height;
      return false;
    }

    public static bool IsValid(AdornerStretch value)
    {
      if (value >= AdornerStretch.None)
        return value <= AdornerStretch.Stretch;
      return false;
    }

    public static bool IsValid(NudgeIntent value)
    {
      if (value >= NudgeIntent.Left)
        return value <= NudgeIntent.Down;
      return false;
    }

    public static bool IsValid(ToolAction value)
    {
      if (value >= ToolAction.None)
        return value <= ToolAction.DragOutside;
      return false;
    }
  }
}
