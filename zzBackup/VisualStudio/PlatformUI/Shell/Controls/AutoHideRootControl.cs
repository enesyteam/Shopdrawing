// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.Controls.AutoHideRootControl
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.VisualStudio.PlatformUI;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.VisualStudio.PlatformUI.Shell.Controls
{
  public class AutoHideRootControl : LayoutSynchronizedItemsControl
  {
    private FrameworkElement dockRoot;

    public FrameworkElement DockRoot
    {
      get
      {
        return this.dockRoot;
      }
    }

    static AutoHideRootControl()
    {
      FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (AutoHideRootControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (AutoHideRootControl)));
    }

    protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
    {
      base.PrepareContainerForItemOverride(element, item);
      UIElement element1 = (UIElement) element;
      DependencyObject dependencyObject = (DependencyObject) item;
      if (item is Microsoft.VisualStudio.PlatformUI.Shell.DockRoot)
      {
        Panel.SetZIndex(element1, 0);
        Grid.SetColumn(element1, 1);
        Grid.SetRow(element1, 1);
        this.dockRoot = element1 as FrameworkElement;
      }
      else
      {
        Panel.SetZIndex(element1, 1);
        switch ((Dock) dependencyObject.GetValue(DockPanel.DockProperty))
        {
          case Dock.Left:
            Grid.SetColumn(element1, 0);
            Grid.SetRow(element1, 1);
            break;
          case Dock.Top:
            Grid.SetColumn(element1, 1);
            Grid.SetRow(element1, 0);
            break;
          case Dock.Right:
            Grid.SetColumn(element1, 2);
            Grid.SetRow(element1, 1);
            break;
          case Dock.Bottom:
            Grid.SetColumn(element1, 1);
            Grid.SetRow(element1, 2);
            break;
        }
      }
    }
  }
}
