// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.Controls.BeveledBorder
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Microsoft.VisualStudio.PlatformUI.Shell.Controls
{
  public class BeveledBorder : Decorator
  {
    public static readonly DependencyProperty BackgroundProperty = Border.BackgroundProperty.AddOwner(typeof (BeveledBorder), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));
    public static readonly DependencyProperty BorderBrushProperty = Border.BorderBrushProperty.AddOwner(typeof (BeveledBorder), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender, new PropertyChangedCallback(BeveledBorder.OnBorderBrushChanged)));
    public static readonly DependencyProperty BorderThicknessProperty = Border.BorderThicknessProperty.AddOwner(typeof (BeveledBorder), (PropertyMetadata) new FrameworkPropertyMetadata((object) new Thickness(), FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(BeveledBorder.OnBorderThicknessChanged)));
    public static readonly DependencyProperty PaddingProperty = Border.PaddingProperty.AddOwner(typeof (BeveledBorder), (PropertyMetadata) new FrameworkPropertyMetadata((object) new Thickness(), FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));
    public static readonly DependencyProperty BevelProperty = DependencyProperty.Register("Bevel", typeof (CornerRadius), typeof (BeveledBorder), (PropertyMetadata) new FrameworkPropertyMetadata((object) new CornerRadius(), FrameworkPropertyMetadataOptions.AffectsRender));
    private Rect borderRect;
    private Pen leftPen;
    private Pen rightPen;
    private Pen topPen;
    private Pen bottomPen;

    public CornerRadius Bevel
    {
      get
      {
        return (CornerRadius) this.GetValue(BeveledBorder.BevelProperty);
      }
      set
      {
        this.SetValue(BeveledBorder.BevelProperty, (object) value);
      }
    }

    public Thickness Padding
    {
      get
      {
        return (Thickness) this.GetValue(BeveledBorder.PaddingProperty);
      }
      set
      {
        this.SetValue(BeveledBorder.PaddingProperty, (object) value);
      }
    }

    public Thickness BorderThickness
    {
      get
      {
        return (Thickness) this.GetValue(BeveledBorder.BorderThicknessProperty);
      }
      set
      {
        this.SetValue(BeveledBorder.BorderThicknessProperty, (object) value);
      }
    }

    public Brush BorderBrush
    {
      get
      {
        return (Brush) this.GetValue(BeveledBorder.BorderBrushProperty);
      }
      set
      {
        this.SetValue(BeveledBorder.BorderBrushProperty, (object) value);
      }
    }

    public Brush Background
    {
      get
      {
        return (Brush) this.GetValue(BeveledBorder.BackgroundProperty);
      }
      set
      {
        this.SetValue(BeveledBorder.BackgroundProperty, (object) value);
      }
    }

    protected override Size MeasureOverride(Size constraint)
    {
      UIElement child = this.Child;
      Size size1 = new Size();
      Size size2 = BeveledBorder.HelperCollapseThickness(this.BorderThickness);
      Size size3 = BeveledBorder.HelperCollapseThickness(this.Padding);
      Size size4 = new Size(size2.Width + size3.Width, size2.Height + size3.Height);
      if (child == null)
        return size4;
      Size availableSize = new Size(Math.Max(0.0, constraint.Width - size4.Width), Math.Max(0.0, constraint.Height - size4.Height));
      child.Measure(availableSize);
      Size desiredSize = child.DesiredSize;
      size1.Width = desiredSize.Width + size4.Width;
      size1.Height = desiredSize.Height + size4.Height;
      return size1;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
      UIElement child = this.Child;
      if (child != null)
      {
        Rect finalRect = BeveledBorder.HelperDeflateRect(BeveledBorder.HelperDeflateRect(new Rect(finalSize), this.BorderThickness), this.Padding);
        child.Arrange(finalRect);
      }
      this.borderRect = new Rect(this.BorderThickness.Left / 2.0, this.BorderThickness.Top / 2.0, finalSize.Width, finalSize.Height);
      return finalSize;
    }

    private void CachePens()
    {
      Brush borderBrush = this.BorderBrush;
      Thickness borderThickness = this.BorderThickness;
      this.leftPen = BeveledBorder.GetPen(borderThickness.Left, borderBrush);
      this.rightPen = BeveledBorder.GetPen(borderThickness.Right, borderBrush);
      this.topPen = BeveledBorder.GetPen(borderThickness.Top, borderBrush);
      this.bottomPen = BeveledBorder.GetPen(borderThickness.Bottom, borderBrush);
    }

    private static Pen GetPen(double thickness, Brush brush)
    {
      Pen pen = new Pen();
      pen.Brush = brush;
      pen.Thickness = thickness;
      if (brush != null && brush.IsFrozen)
        pen.Freeze();
      return pen;
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
      List<Point> points = new List<Point>();
      List<Pen> pens = new List<Pen>();
      CornerRadius bevel = this.Bevel;
      this.AddTopLeftPoints(points, pens, bevel);
      this.AddTopRightPoints(points, pens, bevel);
      this.AddBottomRightPoints(points, pens, bevel);
      this.AddBottomLeftPoints(points, pens, bevel);
      Brush background;
      if ((background = this.Background) != null)
      {
        PathGeometry pathGeometry = new PathGeometry();
        PathFigure pathFigure = new PathFigure();
        pathFigure.StartPoint = points[0];
        for (int index = 1; index < points.Count; ++index)
          pathFigure.Segments.Add((PathSegment) new LineSegment(points[index], false));
        pathGeometry.Figures.Add(pathFigure);
        drawingContext.DrawGeometry(background, (Pen) null, (Geometry) pathGeometry);
      }
      Brush borderBrush;
      if ((borderBrush = this.BorderBrush) == null)
        return;
      int index1;
      for (index1 = 0; index1 < points.Count - 1; ++index1)
        drawingContext.DrawLine(pens[index1], points[index1], points[index1 + 1]);
      drawingContext.DrawLine(this.leftPen, points[index1], points[0]);
    }

    private void AddBottomLeftPoints(List<Point> points, List<Pen> pens, CornerRadius bevel)
    {
      if (bevel.BottomLeft > 0.0)
      {
        points.Add(new Point(this.borderRect.Left + bevel.BottomLeft, this.borderRect.Bottom));
        points.Add(new Point(this.borderRect.Left, this.borderRect.Bottom - bevel.BottomLeft));
        pens.Add(this.bottomPen);
        pens.Add(this.leftPen);
      }
      else
      {
        points.Add(this.borderRect.BottomLeft);
        pens.Add(this.bottomPen);
      }
    }

    private void AddBottomRightPoints(List<Point> points, List<Pen> pens, CornerRadius bevel)
    {
      if (bevel.BottomRight > 0.0)
      {
        points.Add(new Point(this.borderRect.Right, this.borderRect.Bottom - bevel.BottomRight));
        points.Add(new Point(this.borderRect.Right - bevel.BottomRight, this.borderRect.Bottom));
        pens.Add(this.rightPen);
        pens.Add(this.rightPen);
      }
      else
      {
        points.Add(this.borderRect.BottomRight);
        pens.Add(this.rightPen);
      }
    }

    private void AddTopRightPoints(List<Point> points, List<Pen> pens, CornerRadius bevel)
    {
      if (bevel.TopRight > 0.0)
      {
        points.Add(new Point(this.borderRect.Right - bevel.TopRight, this.borderRect.Top));
        points.Add(new Point(this.borderRect.Right, this.borderRect.Top + bevel.TopRight));
        pens.Add(this.topPen);
        pens.Add(this.rightPen);
      }
      else
      {
        points.Add(this.borderRect.TopRight);
        pens.Add(this.topPen);
      }
    }

    private void AddTopLeftPoints(List<Point> points, List<Pen> pens, CornerRadius bevel)
    {
      if (bevel.TopLeft > 0.0)
      {
        points.Add(new Point(this.borderRect.Left, this.borderRect.Top + bevel.TopLeft));
        points.Add(new Point(this.borderRect.Left + bevel.TopLeft, this.borderRect.Top));
        pens.Add(this.leftPen);
      }
      else
        points.Add(this.borderRect.TopLeft);
    }

    private static void OnBorderBrushChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      ((BeveledBorder) obj).CachePens();
    }

    private static void OnBorderThicknessChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      ((BeveledBorder) obj).CachePens();
    }

    private static Size HelperCollapseThickness(Thickness th)
    {
      return new Size(th.Left + th.Right, th.Top + th.Bottom);
    }

    private static Rect HelperDeflateRect(Rect rt, Thickness thick)
    {
      return new Rect(rt.Left + thick.Left, rt.Top + thick.Top, Math.Max(0.0, rt.Width - thick.Left - thick.Right), Math.Max(0.0, rt.Height - thick.Top - thick.Bottom));
    }
  }
}
