// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ListItemDragDropHandler
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.UserInterface.Timeline.UI;
using System;
using System.Windows;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface
{
  public class ListItemDragDropHandler : DragDropHandler
  {
    private HoverHelper hoverHelper;

    public FrameworkElement DropTarget { get; private set; }

    public DropHitTestResults HitTestResult { get; private set; }

    public DropHitTestResults LastHitTestResult { get; private set; }

    public ListItemDragDropHandler(FrameworkElement element)
    {
      this.DropTarget = element;
    }

    public static DropHitTestResults HitTest(FrameworkElement element, Point point)
    {
      DropHitTestResults dropHitTestResults = DropHitTestResults.None;
      if (element != null)
      {
        double actualWidth = element.ActualWidth;
        double actualHeight = element.ActualHeight;
        if (point.X >= 0.0 && point.X < actualWidth && (point.Y >= 0.0 && point.Y < actualHeight))
        {
          if (point.Y < actualHeight / 2.0)
            dropHitTestResults |= DropHitTestResults.UpperHalf;
          else
            dropHitTestResults |= DropHitTestResults.LowerHalf;
          if (point.Y > actualHeight / 4.0 && point.Y < actualHeight * 3.0 / 4.0)
            dropHitTestResults |= DropHitTestResults.CenterHalf;
        }
      }
      return dropHitTestResults;
    }

    public override void OnDragOver(DragEventArgs e)
    {
      base.OnDragOver(e);
      this.LastHitTestResult = this.HitTestResult;
      if (this.DropTarget != null)
      {
        this.HitTestResult = ListItemDragDropHandler.HitTest(this.DropTarget, e.GetPosition((IInputElement) this.DropTarget));
        if (this.hoverHelper == null)
          return;
        this.hoverHelper.HandleMouseMove(Mouse.GetPosition((IInputElement) this.DropTarget), (this.HitTestResult & DropHitTestResults.CenterHalf) != DropHitTestResults.None);
      }
      else
        this.HitTestResult = DropHitTestResults.None;
    }

    public override void OnDragEnter(DragEventArgs e)
    {
      base.OnDragEnter(e);
      this.LastHitTestResult = this.HitTestResult;
      if (this.DropTarget != null)
        this.HitTestResult = ListItemDragDropHandler.HitTest(this.DropTarget, e.GetPosition((IInputElement) this.DropTarget));
      else
        this.HitTestResult = DropHitTestResults.None;
    }

    public override void OnDragLeave(DragEventArgs e)
    {
      this.LastHitTestResult = this.HitTestResult = DropHitTestResults.None;
      base.OnDragLeave(e);
    }

    public override void OnDrop(DragEventArgs e)
    {
      this.LastHitTestResult = this.HitTestResult = DropHitTestResults.None;
      base.OnDrop(e);
    }

    protected virtual void OnHoverEnter()
    {
    }

    protected virtual void OnHoverLeave()
    {
    }

    protected override void StartListeners(IDataObject data)
    {
      if (this.DropTarget != null)
      {
        this.hoverHelper = new HoverHelper((UIElement) this.DropTarget);
        this.hoverHelper.HoverEnter += new EventHandler<EventArgs>(this.HoverEnter);
        this.hoverHelper.HoverLeave += new EventHandler<EventArgs>(this.HoverLeave);
        this.hoverHelper.HandleMouseEnter(Mouse.GetPosition((IInputElement) this.DropTarget));
      }
      base.StartListeners(data);
    }

    protected override void StopListeners(IDataObject data)
    {
      base.StopListeners(data);
      if (this.hoverHelper == null)
        return;
      this.hoverHelper.HandleMouseLeave();
      this.hoverHelper.HoverEnter -= new EventHandler<EventArgs>(this.HoverEnter);
      this.hoverHelper.HoverLeave -= new EventHandler<EventArgs>(this.HoverLeave);
      this.hoverHelper = (HoverHelper) null;
    }

    private void HoverEnter(object sender, EventArgs e)
    {
      this.OnHoverEnter();
    }

    private void HoverLeave(object sender, EventArgs e)
    {
      this.OnHoverLeave();
    }
  }
}
