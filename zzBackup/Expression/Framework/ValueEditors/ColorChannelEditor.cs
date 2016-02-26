// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.ValueEditors.ColorChannelEditor
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.Framework.ValueEditors
{
  public class ColorChannelEditor : NumberEditor
  {
    public static readonly DependencyProperty RangeBrushProperty = DependencyProperty.Register("RangeBrush", typeof (Brush), typeof (ColorChannelEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.AffectsRender));
    public static readonly DependencyProperty RangeMarginProperty = DependencyProperty.Register("RangeMargin", typeof (Thickness), typeof (ColorChannelEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) new Thickness(), FrameworkPropertyMetadataOptions.AffectsRender));
    public static readonly DependencyProperty RangeCornerRadiusProperty = DependencyProperty.Register("RangeCornerRadius", typeof (CornerRadius), typeof (ColorChannelEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) new CornerRadius(), FrameworkPropertyMetadataOptions.AffectsRender));
    public static readonly DependencyProperty RangeHeightProperty = DependencyProperty.Register("RangeHeight", typeof (double), typeof (ColorChannelEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) 0.0, FrameworkPropertyMetadataOptions.AffectsRender));
    private SuperRoundedRectRenderer rangeRenderer;

    public Brush RangeBrush
    {
      get
      {
        return (Brush) this.GetValue(ColorChannelEditor.RangeBrushProperty);
      }
      set
      {
        this.SetValue(ColorChannelEditor.RangeBrushProperty, (object) value);
      }
    }

    public Thickness RangeMargin
    {
      get
      {
        return (Thickness) this.GetValue(ColorChannelEditor.RangeMarginProperty);
      }
      set
      {
        this.SetValue(ColorChannelEditor.RangeMarginProperty, (object) value);
      }
    }

    public CornerRadius RangeCornerRadius
    {
      get
      {
        return (CornerRadius) this.GetValue(ColorChannelEditor.RangeCornerRadiusProperty);
      }
      set
      {
        this.SetValue(ColorChannelEditor.RangeCornerRadiusProperty, (object) value);
      }
    }

    public double RangeHeight
    {
      get
      {
        return (double) this.GetValue(ColorChannelEditor.RangeHeightProperty);
      }
      set
      {
        this.SetValue(ColorChannelEditor.RangeHeightProperty, (object) value);
      }
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
      this.InvalidateRangeRenderer();
      return base.ArrangeOverride(finalSize);
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
      base.OnRender(drawingContext);
      if (this.rangeRenderer == null)
        this.rangeRenderer = new SuperRoundedRectRenderer();
      Thickness rangeMargin = this.RangeMargin;
      Rect rect = RenderUtils.CalculateInnerRect(new Rect(0.0, 0.0, this.ActualWidth, this.ActualHeight), this.BorderWidth);
      Rect renderRect = new Rect(new Point(rect.Left + rangeMargin.Left, rect.Top + rect.Height - rangeMargin.Bottom - this.RangeHeight), new Point(rect.Left + rect.Width - rangeMargin.Right, rect.Top + rect.Height - rangeMargin.Bottom));
      this.rangeRenderer.Render(drawingContext, renderRect, this.RangeBrush, this.RangeCornerRadius);
    }

    private void InvalidateRangeRenderer()
    {
      if (this.rangeRenderer == null)
        return;
      this.rangeRenderer.InvalidateGeometry();
    }
  }
}
