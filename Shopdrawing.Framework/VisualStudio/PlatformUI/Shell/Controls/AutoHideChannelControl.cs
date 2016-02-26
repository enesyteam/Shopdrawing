// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.Controls.AutoHideChannelControl
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Microsoft.VisualStudio.PlatformUI.Shell.Controls
{
  [TemplatePart(Name = "PART_AutoHideSlideout", Type = typeof (Canvas))]
  public class AutoHideChannelControl : LayoutSynchronizedItemsControl
  {
    public static readonly RoutedEvent AutoHideChannelContextMenuEvent = EventManager.RegisterRoutedEvent("AutoHideChannelContextMenu", RoutingStrategy.Bubble, typeof (EventHandler<AutoHideChannelContextMenuEventArgs>), typeof (AutoHideChannelControl));
    public static readonly DependencyProperty OrientationProperty = DependencyProperty.RegisterAttached("Orientation", typeof (Orientation), typeof (AutoHideChannelControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) Orientation.Vertical, FrameworkPropertyMetadataOptions.Inherits));
    public static readonly DependencyProperty ChannelDockProperty = DependencyProperty.RegisterAttached("ChannelDock", typeof (Dock), typeof (AutoHideChannelControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) Dock.Left, FrameworkPropertyMetadataOptions.Inherits));
    public static readonly DependencyProperty AutoHideSlideoutProperty = DependencyProperty.Register("AutoHideSlideout", typeof (object), typeof (AutoHideChannelControl));

    public object AutoHideSlideout
    {
      get
      {
        return this.GetValue(AutoHideChannelControl.AutoHideSlideoutProperty);
      }
      set
      {
        this.SetValue(AutoHideChannelControl.AutoHideSlideoutProperty, value);
      }
    }

    static AutoHideChannelControl()
    {
      FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (AutoHideChannelControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (AutoHideChannelControl)));
    }

    public static Orientation GetOrientation(UIElement element)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      return (Orientation) element.GetValue(AutoHideChannelControl.OrientationProperty);
    }

    public static void SetOrientation(UIElement element, Orientation value)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      element.SetValue(AutoHideChannelControl.OrientationProperty, (object) value);
    }

    public static Dock GetChannelDock(UIElement element)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      return (Dock) element.GetValue(AutoHideChannelControl.OrientationProperty);
    }

    public static void SetChannelDock(UIElement element, Dock value)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      element.SetValue(AutoHideChannelControl.OrientationProperty, (object) value);
    }

    protected override void OnContextMenuOpening(ContextMenuEventArgs e)
    {
      UIElement uiElement = this.GetTemplateChild("PART_AutoHideSlideout") as UIElement;
      Visual visual = (Visual) e.Source;
      if (uiElement == null || uiElement.IsAncestorOf((DependencyObject) visual))
        return;
      e.Handled = true;
      Point channelPoint = ((Visual) e.OriginalSource).TransformToVisual((Visual) this).Transform(new Point(e.CursorLeft, e.CursorTop));
      this.RaiseEvent((RoutedEventArgs) new AutoHideChannelContextMenuEventArgs(AutoHideChannelControl.AutoHideChannelContextMenuEvent, channelPoint));
    }
  }
}
