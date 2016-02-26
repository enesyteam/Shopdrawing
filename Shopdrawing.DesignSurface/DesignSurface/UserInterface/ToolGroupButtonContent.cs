// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.ToolGroupButtonContent
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  public sealed class ToolGroupButtonContent : FrameworkElement
  {
    public static readonly DependencyProperty ExpansionTriangleBrushProperty = DependencyProperty.Register("ExpansionTriangleBrush", typeof (Brush), typeof (ToolGroupButtonContent), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.AffectsRender));
    public static readonly DependencyProperty ActiveToolProperty = DependencyProperty.Register("ActiveTool", typeof (ToolGroupItem), typeof (ToolGroupButtonContent), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.AffectsRender));
    public static readonly DependencyProperty ToolCountProperty = DependencyProperty.Register("ToolCount", typeof (int), typeof (ToolGroupButtonContent), (PropertyMetadata) new FrameworkPropertyMetadata((object) 0, FrameworkPropertyMetadataOptions.AffectsRender));
    public static readonly DependencyProperty IsCheckedProperty = ToggleButton.IsCheckedProperty.AddOwner(typeof (ToolGroupButtonContent));
    private const int iconWidth = 24;
    private const int iconHeight = 24;
    private const int contentWidth = 32;
    private const int contentHeight = 32;

    public Brush ExpansionTriangleBrush
    {
      get
      {
        return (Brush) this.GetValue(ToolGroupButtonContent.ExpansionTriangleBrushProperty);
      }
      set
      {
        this.SetValue(ToolGroupButtonContent.ExpansionTriangleBrushProperty, (object) value);
      }
    }

    public ToolGroupItem ActiveTool
    {
      get
      {
        return (ToolGroupItem) this.GetValue(ToolGroupButtonContent.ActiveToolProperty);
      }
      set
      {
        this.SetValue(ToolGroupButtonContent.ActiveToolProperty, (object) value);
      }
    }

    public int ToolCount
    {
      get
      {
        return (int) this.GetValue(ToolGroupButtonContent.ToolCountProperty);
      }
      set
      {
        this.SetValue(ToolGroupButtonContent.ToolCountProperty, (object) value);
      }
    }

    public bool? IsChecked
    {
      get
      {
        return (bool?) this.GetValue(ToolGroupButtonContent.IsCheckedProperty);
      }
      set
      {
        this.SetValue(ToolGroupButtonContent.IsCheckedProperty, (object) value);
      }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      return new Size(32.0, 32.0);
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
      base.OnRender(drawingContext);
      double actualWidth = this.ActualWidth;
      double actualHeight = this.ActualHeight;
      UIElement uiElement = this.Parent as UIElement;
      DrawingBrush drawingBrush;
      if (this.ActiveTool != null && uiElement != null)
      {
        drawingBrush = this.ActiveTool.NormalIconBrush;
        if (uiElement.IsMouseOver || this.IsChecked.HasValue && this.IsChecked.Value)
          drawingBrush = this.ActiveTool.HoverIconBrush;
      }
      else
        drawingBrush = IconMapper.GetDrawingBrushForType(PlatformTypes.Object, false, 24, 24);
      Rect rectangle = new Rect(new Point((double) (((int) actualWidth - 24) / 2), (double) (((int) actualHeight - 24) / 2)), new Size(24.0, 24.0));
      if (drawingBrush != null)
        drawingContext.DrawRectangle((Brush) drawingBrush, (Pen) null, rectangle);
      if (this.ToolCount <= 1)
        return;
      double x = actualWidth - 1.0;
      double y = actualHeight - 1.0;
      StreamGeometry streamGeometry = new StreamGeometry();
      StreamGeometryContext streamGeometryContext = streamGeometry.Open();
      streamGeometryContext.BeginFigure(new Point(x, y), true, true);
      streamGeometryContext.LineTo(new Point(x - 5.0, y), false, false);
      streamGeometryContext.LineTo(new Point(x, y - 5.0), false, false);
      streamGeometryContext.Close();
      streamGeometry.Freeze();
      drawingContext.DrawGeometry(this.ExpansionTriangleBrush, (Pen)null, (System.Windows.Media.Geometry)streamGeometry);
    }
  }
}
