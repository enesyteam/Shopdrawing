// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Controls.DragBeginHelper
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.Framework.Controls
{
  public class DragBeginHelper
  {
    private Point dragStart;
    private DragBeginHelper.DragStartedHandler dragStartedHandler;

    private static UIElement ElementOwningButtonDown { get; set; }

    public DragBeginHelper(UIElement target, DragBeginHelper.DragStartedHandler dragStartedHandler)
    {
      this.dragStartedHandler = dragStartedHandler;
      target.AllowDrop = true;
      target.AddHandler(Mouse.MouseDownEvent, (Delegate) new RoutedEventHandler(this.DragSourceControl_MouseDown), true);
      target.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.DragSourceControl_MouseLeftButtonUp);
      target.AddHandler(Mouse.MouseMoveEvent, (Delegate) new MouseEventHandler(this.DragSourceControl_MouseMove), true);
      target.MouseEnter += new MouseEventHandler(this.DragSourceControl_MouseEnter);
      target.MouseLeave += new MouseEventHandler(this.target_MouseLeave);
    }

    public void UpdateDragStartedHandler(DragBeginHelper.DragStartedHandler dragStartedHandler)
    {
      this.dragStartedHandler = dragStartedHandler;
    }

    private void DragSourceControl_MouseEnter(object sender, MouseEventArgs e)
    {
      if (DragBeginHelper.ElementOwningButtonDown == sender)
        return;
      DragBeginHelper.ElementOwningButtonDown = (UIElement) null;
    }

    private void DragSourceControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      DragBeginHelper.ElementOwningButtonDown = (UIElement) null;
    }

    private void target_MouseLeave(object sender, MouseEventArgs e)
    {
      UIElement sourceElement = sender as UIElement;
      if (sourceElement != null && e.OriginalSource is Visual && (DragBeginHelper.ElementOwningButtonDown == sourceElement && PresentationSource.FromVisual((Visual) sourceElement) != null) && PresentationSource.FromVisual((Visual) sourceElement) == PresentationSource.FromVisual((Visual) e.OriginalSource))
        this.TryBeginDragDrop(sourceElement.PointToScreen(e.GetPosition((IInputElement) sourceElement)), sourceElement);
      DragBeginHelper.ElementOwningButtonDown = (UIElement) null;
    }

    private void TryBeginDragDrop(Point currentPoint, UIElement sourceElement)
    {
      if (Math.Abs(this.dragStart.X - currentPoint.X) <= SystemParameters.MinimumHorizontalDragDistance && Math.Abs(this.dragStart.Y - currentPoint.Y) <= SystemParameters.MinimumVerticalDragDistance)
        return;
      DragBeginHelper.ElementOwningButtonDown = (UIElement) null;
      this.dragStartedHandler(new DragBeginEventArgs(sourceElement));
    }

    private void DragSourceControl_MouseMove(object sender, MouseEventArgs e)
    {
      UIElement sourceElement = sender as UIElement;
      if (sourceElement == null || !(e.OriginalSource is Visual) || (DragBeginHelper.ElementOwningButtonDown != sourceElement || PresentationSource.FromVisual((Visual) sourceElement) == null) || PresentationSource.FromVisual((Visual) sourceElement) != PresentationSource.FromVisual((Visual) e.OriginalSource))
        return;
      this.TryBeginDragDrop(sourceElement.PointToScreen(e.GetPosition((IInputElement) sourceElement)), sourceElement);
    }

    private void DragSourceControl_MouseDown(object sender, RoutedEventArgs e)
    {
      UIElement uiElement = sender as UIElement;
      if (uiElement == null || !(e.OriginalSource is Visual) || (DragBeginHelper.ElementOwningButtonDown != null || Mouse.LeftButton != MouseButtonState.Pressed) || (PresentationSource.FromVisual((Visual) uiElement) == null || PresentationSource.FromVisual((Visual) uiElement) != PresentationSource.FromVisual((Visual) e.OriginalSource)))
        return;
      DragBeginHelper.ElementOwningButtonDown = uiElement;
      this.dragStart = uiElement.PointToScreen(Mouse.GetPosition((IInputElement) uiElement));
    }

    public delegate void DragStartedHandler(DragBeginEventArgs e);
  }
}
