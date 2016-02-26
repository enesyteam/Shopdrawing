// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.DragSourceHelper
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework.Data;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface
{
  public static class DragSourceHelper
  {
    public static DragDropEffects DoDragDrop(UIElement dragSource, object data, DragDropEffects allowedEffects)
    {
      DragSourceHelper.DragSource dragData = new DragSourceHelper.DragSource(data);
      using (new DragSourceHelper.DragSourceAttacher(dragSource, dragData))
      {
        try
        {
          return DragDrop.DoDragDrop((DependencyObject) dragSource, (object) dragData, allowedEffects);
        }
        catch (Exception ex)
        {
          return DragDropEffects.None;
        }
        finally
        {
          dragData.OnDragFinished((object) dragSource, EventArgs.Empty);
        }
      }
    }

    public static bool FirstDataOfType<T>(IDataObject data, ref T result)
    {
      using (IEnumerator<T> enumerator = DragSourceHelper.DataOfType<T>(data).GetEnumerator())
      {
        if (enumerator.MoveNext())
        {
          T current = enumerator.Current;
          result = current;
          return true;
        }
      }
      return false;
    }

    public static bool FirstDataOfType<T>(SafeDataObject data, ref T result)
    {
      using (IEnumerator<T> enumerator = DragSourceHelper.DataOfType<T>(data).GetEnumerator())
      {
        if (enumerator.MoveNext())
        {
          T current = enumerator.Current;
          result = current;
          return true;
        }
      }
      return false;
    }

    public static IEnumerable<T> DataOfType<T>(IDataObject data)
    {
      if (data != null)
      {
        foreach (T obj in DragSourceHelper.DataOfType<T>(new SafeDataObject(data)))
          yield return obj;
      }
    }

    public static IEnumerable<T> DataOfType<T>(SafeDataObject data)
    {
      if (data != null)
      {
        T result = default (T);
        foreach (string format in data.GetFormats())
        {
          if (DragSourceHelper.InternalDataOfType<T>(data.GetData(format), ref result))
            yield return result;
        }
      }
    }

    private static bool InternalDataOfType<T>(object data, ref T result)
    {
      DragSourceHelper.DragSource dragSource = data as DragSourceHelper.DragSource;
      if (dragSource != null && dragSource.Data is T)
      {
        result = (T) dragSource.Data;
        return true;
      }
      if (!(data is T))
        return false;
      result = (T) data;
      return true;
    }

    private sealed class DragSource : IDragSource
    {
      public object Data { get; private set; }

      public event EventHandler<DragCanceledEventArgs> DragCanceled;

      public event GiveFeedbackEventHandler GiveFeedback;

      public event EventHandler<EventArgs> DragFinished;

      public DragSource(object data)
      {
        this.Data = data;
      }

      public void OnDragCanceled(object sender, DragCanceledEventArgs e)
      {
        if (this.DragCanceled == null)
          return;
        this.DragCanceled(sender, e);
      }

      public void OnGiveFeedback(object sender, GiveFeedbackEventArgs e)
      {
        if (this.GiveFeedback == null)
          return;
        this.GiveFeedback(sender, e);
      }

      public void OnDragFinished(object sender, EventArgs e)
      {
        if (this.DragFinished == null)
          return;
        this.DragFinished(sender, e);
      }
    }

    private sealed class DragSourceAttacher : IDisposable
    {
      public UIElement Element { get; private set; }

      public DragSourceHelper.DragSource DragData { get; private set; }

      public Point DragStartPoint { get; private set; }

      public DragSourceAttacher(UIElement element, DragSourceHelper.DragSource dragData)
      {
        this.Element = element;
        this.DragData = dragData;
        this.DragStartPoint = element.PointToScreen(Mouse.GetPosition((IInputElement) element));
        element.QueryContinueDrag += new QueryContinueDragEventHandler(this.Element_QueryContinueDrag);
        element.GiveFeedback += new GiveFeedbackEventHandler(this.Element_GiveFeedback);
      }

      public void Dispose()
      {
        this.Element.QueryContinueDrag -= new QueryContinueDragEventHandler(this.Element_QueryContinueDrag);
        this.Element.GiveFeedback -= new GiveFeedbackEventHandler(this.Element_GiveFeedback);
      }

      private void Element_GiveFeedback(object sender, GiveFeedbackEventArgs e)
      {
        if (this.DragData == null)
          return;
        this.DragData.OnGiveFeedback(sender, e);
      }

      private void Element_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
      {
        if (!e.EscapePressed || this.DragData == null)
          return;
        this.DragData.OnDragCanceled(sender, new DragCanceledEventArgs()
        {
          DragStartPoint = this.DragStartPoint
        });
      }
    }
  }
}
