// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.UserInterface.CommandBarHelper
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System.Collections;
using System.Windows;

namespace Microsoft.Expression.Framework.UserInterface
{
  public static class CommandBarHelper
  {
    public static void CollapseUnnecessarySeparators(ICommandBarItemCollection items)
    {
      object obj1 = (object) null;
      foreach (object obj2 in (IEnumerable) items)
      {
        CommandBarSeparator commandBarSeparator = obj2 as CommandBarSeparator;
        if (commandBarSeparator != null)
        {
          commandBarSeparator.Visibility = Visibility.Visible;
          if (obj1 is CommandBarSeparator || obj1 == null)
            commandBarSeparator.Visibility = Visibility.Collapsed;
        }
        UIElement uiElement = obj2 as UIElement;
        if (uiElement != null && uiElement.Visibility == Visibility.Visible)
          obj1 = (object) uiElement;
      }
      CommandBarSeparator commandBarSeparator1 = obj1 as CommandBarSeparator;
      if (commandBarSeparator1 == null)
        return;
      commandBarSeparator1.Visibility = Visibility.Collapsed;
    }
  }
}
