// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Layout.DockPanelLayoutUtilities
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Expression.DesignSurface.Tools.Layout
{
  internal sealed class DockPanelLayoutUtilities
  {
    private DockPanelLayoutUtilities()
    {
    }

    public static int GetFillRegionInsertionIndex(DockPanelElement dockPanel)
    {
      int num = -1;
      if (num == -1)
        num = dockPanel.Children.Count;
      return num;
    }

    public static int GetChildInsertionIndex(DockPanelElement dockPanel, BaseFrameworkElement dockPanelChild, Dock adornerDock)
    {
      int num = dockPanel.Children.IndexOf((SceneNode) dockPanelChild);
      switch (DockPanel.GetDock((UIElement) (dockPanelChild.ViewObject.PlatformSpecificObject as FrameworkElement)))
      {
        case Dock.Left:
        case Dock.Top:
          if (adornerDock == Dock.Left || adornerDock == Dock.Top)
            return num;
          if (adornerDock == Dock.Right || adornerDock == Dock.Bottom)
            return num + 1;
          throw new ArgumentException(ExceptionStringTable.IllegalFillAdornerOnDockChild);
        case Dock.Right:
        case Dock.Bottom:
          if (adornerDock == Dock.Left || adornerDock == Dock.Top)
            return num + 1;
          if (adornerDock == Dock.Right || adornerDock == Dock.Bottom)
            return num;
          throw new ArgumentException(ExceptionStringTable.IllegalFillAdornerOnDockChild);
        default:
          throw new ArgumentException(ExceptionStringTable.IllegalFillAdornerOnDockChild);
      }
    }
  }
}
