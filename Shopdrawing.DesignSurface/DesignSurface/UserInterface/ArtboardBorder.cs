// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.ArtboardBorder
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework;
using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  [ContentProperty("Child")]
  public class ArtboardBorder : FrameworkElement
  {
    public static readonly DependencyProperty BorderBrushProperty = Border.BorderBrushProperty.AddOwner(typeof (ArtboardBorder), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.AffectsRender));
    public static readonly DependencyProperty BorderThicknessProperty = Border.BorderThicknessProperty.AddOwner(typeof (ArtboardBorder), (PropertyMetadata) new FrameworkPropertyMetadata((object) new Thickness(), FrameworkPropertyMetadataOptions.AffectsRender));
    private UIElement child;
    private ViewExceptionCallback viewExceptionCallback;

    public Brush BorderBrush
    {
      get
      {
        return (Brush) this.GetValue(ArtboardBorder.BorderBrushProperty);
      }
      set
      {
        this.SetValue(ArtboardBorder.BorderBrushProperty, (object) value);
      }
    }

    public Thickness BorderThickness
    {
      get
      {
        return (Thickness) this.GetValue(ArtboardBorder.BorderThicknessProperty);
      }
      set
      {
        this.SetValue(ArtboardBorder.BorderThicknessProperty, (object) value);
      }
    }

    public virtual UIElement Child
    {
      get
      {
        return this.child;
      }
      set
      {
        if (this.child == value)
          return;
        this.RemoveVisualChild((Visual) this.child);
        this.RemoveLogicalChild((object) this.child);
        this.child = value;
        this.AddLogicalChild((object) value);
        this.AddVisualChild((Visual) value);
        this.InvalidateMeasure();
      }
    }

    protected override IEnumerator LogicalChildren
    {
      get
      {
        yield return (object) this.Child;
      }
    }

    protected override int VisualChildrenCount
    {
      get
      {
        return this.child == null ? 0 : 1;
      }
    }

    public ArtboardBorder(ViewExceptionCallback viewExceptionCallback)
    {
      this.viewExceptionCallback = viewExceptionCallback;
    }

    public void SafelyRemoveChild()
    {
      if (this.child == null)
        return;
      try
      {
        this.RemoveVisualChild((Visual) this.child);
      }
      catch
      {
      }
      try
      {
        this.RemoveLogicalChild((object) this.child);
      }
      catch
      {
      }
      this.child = (UIElement) null;
      this.InvalidateMeasure();
    }

    protected override Visual GetVisualChild(int index)
    {
      if (this.child == null || index != 0)
        throw new ArgumentOutOfRangeException("index");
      return (Visual) this.child;
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      UIElement child = this.Child;
      if (child == null)
        return new Size();
      try
      {
        child.Measure(availableSize);
      }
      catch (Exception ex)
      {
        this.viewExceptionCallback(ex);
        UIThreadDispatcher.Instance.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(((UIElement) this).InvalidateMeasure));
      }
      return child.DesiredSize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
      UIElement child = this.Child;
      if (child != null)
      {
        try
        {
          child.Arrange(new Rect(finalSize));
        }
        catch (Exception ex)
        {
          this.viewExceptionCallback(ex);
          UIThreadDispatcher.Instance.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(((UIElement) this).InvalidateArrange));
        }
      }
      return finalSize;
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
      double width = this.RenderSize.Width;
      double height = this.RenderSize.Height;
      double left = this.BorderThickness.Left;
      Brush borderBrush = this.BorderBrush;
      drawingContext.DrawRectangle(borderBrush, (Pen) null, new Rect(-left, -left, width + 2.0 * left, left));
      drawingContext.DrawRectangle(borderBrush, (Pen) null, new Rect(-left, height, width + 2.0 * left, left));
      drawingContext.DrawRectangle(borderBrush, (Pen) null, new Rect(-left, -left, left, height + 2.0 * left));
      drawingContext.DrawRectangle(borderBrush, (Pen) null, new Rect(width, -left, left, height + 2.0 * left));
    }
  }
}
