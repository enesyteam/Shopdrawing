// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.DragDropHandler
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework.Controls;
using System;
using System.Windows;

namespace Microsoft.Expression.DesignSurface
{
  public class DragDropHandler : IDragDropHandler
  {
    public Action<UIElement> DragAction { get; set; }

    public virtual void OnDragBegin(DragBeginEventArgs e)
    {
      if (this.DragAction == null)
        return;
      this.DragAction(e.DragSource);
      e.Handled = true;
    }

    public virtual void OnGiveFeedback(GiveFeedbackEventArgs e)
    {
    }

    public virtual void OnQueryContinueDrag(QueryContinueDragEventArgs e)
    {
    }

    public virtual void OnDragOver(DragEventArgs e)
    {
    }

    public virtual void OnDragEnter(DragEventArgs e)
    {
      this.StartListeners(e.Data);
    }

    public virtual void OnDragLeave(DragEventArgs e)
    {
      this.StopListeners(e.Data);
    }

    public virtual void OnDrop(DragEventArgs e)
    {
      this.StopListeners(e.Data);
    }

    protected virtual void OnDragSourceGiveFeedback(UIElement dragSource, GiveFeedbackEventArgs e)
    {
    }

    protected virtual void OnDragSourceDragCanceled(UIElement dragSource, DragCanceledEventArgs e)
    {
    }

    protected virtual void OnDragSourceDragFinished(UIElement dragSource, EventArgs e)
    {
    }

    protected virtual void StartListeners(IDataObject data)
    {
      foreach (IDragSource dragSource in DragSourceHelper.DataOfType<IDragSource>(data))
      {
        dragSource.DragCanceled += new EventHandler<DragCanceledEventArgs>(this.DragSourceDragCanceled);
        dragSource.GiveFeedback += new GiveFeedbackEventHandler(this.DragSourceGiveFeedback);
        dragSource.DragFinished += new EventHandler<EventArgs>(this.DragSourceDragFinished);
      }
    }

    protected virtual void StopListeners(IDataObject data)
    {
      foreach (IDragSource dragSource in DragSourceHelper.DataOfType<IDragSource>(data))
      {
        dragSource.DragCanceled -= new EventHandler<DragCanceledEventArgs>(this.DragSourceDragCanceled);
        dragSource.GiveFeedback -= new GiveFeedbackEventHandler(this.DragSourceGiveFeedback);
        dragSource.DragFinished -= new EventHandler<EventArgs>(this.DragSourceDragFinished);
      }
    }

    private void DragSourceGiveFeedback(object sender, GiveFeedbackEventArgs e)
    {
      this.OnDragSourceGiveFeedback(sender as UIElement, e);
    }

    private void DragSourceDragCanceled(object sender, DragCanceledEventArgs e)
    {
      this.OnDragSourceDragCanceled(sender as UIElement, e);
    }

    private void DragSourceDragFinished(object sender, EventArgs e)
    {
      this.OnDragSourceDragFinished(sender as UIElement, e);
    }
  }
}
