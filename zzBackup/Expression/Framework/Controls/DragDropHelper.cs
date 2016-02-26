// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Controls.DragDropHelper
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System.Windows;

namespace Microsoft.Expression.Framework.Controls
{
  public class DragDropHelper
  {
    public static readonly DependencyProperty DragDropHandlerProperty = DependencyProperty.RegisterAttached("DragDropHandler", typeof (IDragDropHandler), typeof (DragDropHelper), (PropertyMetadata) new FrameworkPropertyMetadata(new PropertyChangedCallback(DragDropHelper.OnDragDropCommandInvalidated)));
    private static readonly DependencyProperty DragDropHelperProperty = DependencyProperty.RegisterAttached("DragDropHelper", typeof (DragDropHelper), typeof (DragDropHelper));
    private IDragDropHandler handler;
    private DragBeginHelper dragBeginHelper;

    private DragDropHelper(UIElement target, IDragDropHandler handler)
    {
      this.handler = handler;
      target.AllowDrop = true;
      this.dragBeginHelper = new DragBeginHelper(target, new DragBeginHelper.DragStartedHandler(handler.OnDragBegin));
      target.Drop += new DragEventHandler(this.DragDropControl_Drop);
      target.GiveFeedback += new GiveFeedbackEventHandler(this.DragSourceControl_GiveFeedback);
      target.DragEnter += new DragEventHandler(this.DragDropControl_DragEnter);
      target.DragLeave += new DragEventHandler(this.DragDropControl_DragLeave);
      target.DragOver += new DragEventHandler(this.DragDropControl_DragOver);
      target.QueryContinueDrag += new QueryContinueDragEventHandler(this.DragDropControl_QueryContinueDrag);
    }

    public static void SetDragDropHandler(DependencyObject target, object value)
    {
      target.SetValue(DragDropHelper.DragDropHandlerProperty, value);
    }

    public static void OnDragDropCommandInvalidated(DependencyObject target, DependencyPropertyChangedEventArgs e)
    {
      UIElement target1 = target as UIElement;
      if (target1 == null)
        return;
      DragDropHelper dragDropHelper1 = (DragDropHelper) target1.GetValue(DragDropHelper.DragDropHelperProperty);
      if (dragDropHelper1 == null)
      {
        IDragDropHandler handler = (IDragDropHandler) target1.GetValue(DragDropHelper.DragDropHandlerProperty);
        if (handler == null)
          return;
        DragDropHelper dragDropHelper2 = new DragDropHelper(target1, handler);
        target1.SetValue(DragDropHelper.DragDropHelperProperty, (object) dragDropHelper2);
      }
      else
      {
        IDragDropHandler handler = (IDragDropHandler) target1.GetValue(DragDropHelper.DragDropHandlerProperty);
        if (handler == null)
          return;
        dragDropHelper1.UpdateHandler(handler);
      }
    }

    private void UpdateHandler(IDragDropHandler handler)
    {
      this.handler = handler;
      this.dragBeginHelper.UpdateDragStartedHandler(new DragBeginHelper.DragStartedHandler(handler.OnDragBegin));
    }

    private void DragSourceControl_GiveFeedback(object sender, GiveFeedbackEventArgs e)
    {
      if (!(sender is UIElement))
        return;
      this.handler.OnGiveFeedback(e);
    }

    private void DragDropControl_Drop(object sender, DragEventArgs e)
    {
      if (!(sender is UIElement))
        return;
      this.handler.OnDrop(e);
    }

    private void DragDropControl_DragOver(object sender, DragEventArgs e)
    {
      if (!(sender is UIElement))
        return;
      this.handler.OnDragOver(e);
    }

    private void DragDropControl_DragLeave(object sender, DragEventArgs e)
    {
      if (!(sender is UIElement))
        return;
      this.handler.OnDragLeave(e);
    }

    private void DragDropControl_DragEnter(object sender, DragEventArgs e)
    {
      if (!(sender is UIElement))
        return;
      this.handler.OnDragEnter(e);
    }

    private void DragDropControl_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
    {
      if (!(sender is UIElement))
        return;
      this.handler.OnQueryContinueDrag(e);
    }
  }
}
