// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.PropertyInspector.CategoryContainerCommands
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System.Windows.Input;

namespace Microsoft.Expression.Framework.PropertyInspector
{
  public static class CategoryContainerCommands
  {
    private static readonly RoutedCommand togglePinAdvancedProperties = new RoutedCommand("TogglePinAdvancedProperties", typeof (CategoryContainerCommands));
    private static readonly RoutedCommand updateCategoryExpansionState = new RoutedCommand("UpdateCategoryExpansionState", typeof (CategoryContainerCommands));

    public static RoutedCommand TogglePinAdvancedProperties
    {
      get
      {
        return CategoryContainerCommands.togglePinAdvancedProperties;
      }
    }

    public static RoutedCommand UpdateCategoryExpansionState
    {
      get
      {
        return CategoryContainerCommands.updateCategoryExpansionState;
      }
    }
  }
}
