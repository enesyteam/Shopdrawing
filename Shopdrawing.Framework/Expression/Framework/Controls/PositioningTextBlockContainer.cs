// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Controls.PositioningTextBlockContainer
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Microsoft.Expression.Framework.Controls
{
  public class PositioningTextBlockContainer : Decorator
  {
    private static readonly DependencyProperty OffsetProperty = DependencyProperty.Register("Offset", typeof (Vector), typeof (PositioningTextBlockContainer), (PropertyMetadata) new FrameworkPropertyMetadata((object) new Vector()));

    private Vector Offset
    {
      get
      {
        return (Vector) this.GetValue(PositioningTextBlockContainer.OffsetProperty);
      }
      set
      {
        this.SetValue(PositioningTextBlockContainer.OffsetProperty, (object) value);
      }
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
      UIElement child = this.Child;
      if (child != null)
      {
        Rect finalRect = new Rect(finalSize);
        finalRect.Offset(this.Offset);
        child.Arrange(finalRect);
      }
      return finalSize;
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
      TextBlock textBlock = this.Child as TextBlock;
      if (textBlock != null)
      {
        Point point = new Point(0.0, textBlock.BaselineOffset);
        Vector vector = this.PointFromScreen(this.PointToScreen(point)) - point;
        if (vector != this.Offset || this.ReadLocalValue(PositioningTextBlockContainer.OffsetProperty) == DependencyProperty.UnsetValue)
        {
          this.Offset = vector;
          UIThreadDispatcher.Instance.BeginInvoke(DispatcherPriority.Render, new Action(this.DelayedInvalidateArrange));
        }
      }
      base.OnRender(drawingContext);
    }

    private void DelayedInvalidateArrange()
    {
      this.InvalidateArrange();
      this.InvalidateVisual();
    }
  }
}
