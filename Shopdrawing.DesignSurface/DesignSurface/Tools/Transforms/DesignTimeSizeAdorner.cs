// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.DesignTimeSizeAdorner
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  internal class DesignTimeSizeAdorner : AnchorPointAdorner, IClickable
  {
    private const double EdgeAdornerDepth = 4.0;
    private const double EdgeAdornerLength = 13.0;
    private const double CornerLength = 10.0;
    private const double OuterDistanceFromEdge = 15.0;
    private DesignTimeAdornerType type;
    private bool handlingDrag;

    private BaseFrameworkElement BaseElement
    {
      get
      {
        return this.Element as BaseFrameworkElement;
      }
    }

    private DesignTimeAdornerType Type
    {
      get
      {
        return this.type;
      }
    }

    public bool IsEnabled
    {
      get
      {
        bool flag = false;
        if (this.TestFlags(EdgeFlags.LeftOrRight))
          flag |= DesignTimeSizeBehavior.IsWidthEnabled(this.BaseElement);
        if (this.TestFlags(EdgeFlags.TopOrBottom))
          flag |= DesignTimeSizeBehavior.IsHeightEnabled(this.BaseElement);
        return flag;
      }
    }

    public override object ToolTip
    {
      get
      {
        if (this.IsEnabled)
        {
          switch (this.Type)
          {
            case DesignTimeAdornerType.Width:
              return (object) StringTable.DesignTimeWidthEnabledTooltip;
            case DesignTimeAdornerType.Height:
              return (object) StringTable.DesignTimeHeightEnabledTooltip;
            case DesignTimeAdornerType.Corner:
              return (object) StringTable.DesignTimeCornerEnabledTooltip;
          }
        }
        else
        {
          switch (this.Type)
          {
            case DesignTimeAdornerType.Width:
              return (object) StringTable.DesignTimeWidthDisabledTooltip;
            case DesignTimeAdornerType.Height:
              return (object) StringTable.DesignTimeHeightDisabledTooltip;
            case DesignTimeAdornerType.Corner:
              return (object) StringTable.DesignTimeCornerDisabledTooltip;
          }
        }
        return (object) null;
      }
    }

    public DesignTimeSizeAdorner(AdornerSet adornerSet, EdgeFlags edgeFlags)
      : base(adornerSet, edgeFlags)
    {
      if (this.EdgeFlags == EdgeFlags.Left || this.EdgeFlags == EdgeFlags.Right)
        this.type = DesignTimeAdornerType.Width;
      else if (this.EdgeFlags == EdgeFlags.Top || this.EdgeFlags == EdgeFlags.Bottom)
        this.type = DesignTimeAdornerType.Height;
      else
        this.type = DesignTimeAdornerType.Corner;
    }

    protected override void HandleAdornerLayerEvent(AdornerPropertyChangedEventArgs eventArgs)
    {
      if (!(eventArgs.PropertyName == "HandlingDrag"))
        return;
      this.handlingDrag = (bool) eventArgs.Value;
      this.InvalidateRender();
      this.Redraw();
    }

    public Point GetClickablePoint(Matrix matrix)
    {
      return this.GetOffsetAnchorPoint(matrix, 13.0);
    }

    public override void Draw(DrawingContext ctx, Matrix matrix)
    {
      Point anchorPoint = this.GetAnchorPoint(matrix);
      Brush brush = this.IsActive ? this.ActiveBrush : this.InactiveBrush;
      Rect elementBounds = this.ElementBounds;
      if (this.Type == DesignTimeAdornerType.Width || this.Type == DesignTimeAdornerType.Height)
      {
        if (!this.handlingDrag || this.IsActive || !this.IsEnabled)
        {
          Pen thinPen = this.ThinPen;
          this.DrawEdgeAdorner(ctx, anchorPoint, matrix, brush, thinPen, elementBounds);
        }
      }
      else
      {
        if (!this.TestFlags(EdgeFlags.BottomRight))
          throw new NotSupportedException();
        Pen pen = this.IsEnabled ? this.MediumPen : this.ThinPen;
        this.DrawCornerAdorner(ctx, anchorPoint, matrix, brush, pen);
      }
      if (!this.IsActive || !this.IsEnabled)
        return;
      Matrix transformMatrix = this.ElementSet.GetTransformMatrix(this.DesignerContext.ActiveView.ViewRoot);
      SizeAdorner.DrawDimensions(ctx, matrix, transformMatrix, this.ThinPen, elementBounds, this.EdgeFlags);
    }

    private void DrawEdgeAdorner(DrawingContext drawingContext, Point anchorPoint, Matrix matrix, Brush brush, Pen pen, Rect bounds)
    {
      if (SizeAdorner.SkipAdorner(this.EdgeFlags, matrix, bounds))
        return;
      Vector vector1 = new Vector(1.0, 0.0) * matrix;
      Vector vector2 = new Vector(0.0, 1.0) * matrix;
      vector1.Normalize();
      vector2.Normalize();
      double num1 = this.TestFlags(EdgeFlags.Right) ? -1.0 : 1.0;
      double num2 = this.TestFlags(EdgeFlags.Bottom) ? -1.0 : 1.0;
      switch (this.EdgeFlags & EdgeFlags.TopOrBottom)
      {
        case EdgeFlags.None:
          anchorPoint -= 0.5 * (13.0 - pen.Thickness) * vector2;
          break;
        case EdgeFlags.Top:
        case EdgeFlags.Bottom:
          anchorPoint -= (15.0 - 0.5 * pen.Thickness) * vector2 * num2;
          break;
      }
      switch (this.EdgeFlags & EdgeFlags.LeftOrRight)
      {
        case EdgeFlags.None:
          anchorPoint -= 0.5 * (13.0 - pen.Thickness) * vector1;
          break;
        case EdgeFlags.Left:
        case EdgeFlags.Right:
          anchorPoint -= (15.0 - 0.5 * pen.Thickness) * vector1 * num1;
          break;
      }
      bool flag = this.TestFlags(EdgeFlags.LeftOrRight);
      double num3 = (flag ? 4.0 : 13.0) - pen.Thickness;
      double num4 = (flag ? 13.0 : 4.0) - pen.Thickness;
      vector1 *= num3 * num1;
      Vector vector3 = vector2 * (num4 * num2);
      StreamGeometry streamGeometry = new StreamGeometry();
      StreamGeometryContext streamGeometryContext = streamGeometry.Open();
      streamGeometryContext.BeginFigure(anchorPoint, true, true);
      streamGeometryContext.LineTo(anchorPoint + vector3, true, false);
      streamGeometryContext.LineTo(anchorPoint + vector1 + vector3, true, false);
      streamGeometryContext.LineTo(anchorPoint + vector1, true, false);
      streamGeometryContext.Close();
      streamGeometry.Freeze();
      if (this.IsEnabled)
      {
          drawingContext.DrawGeometry(brush, pen, (System.Windows.Media.Geometry)streamGeometry);
      }
      else
      {
          drawingContext.DrawGeometry((Brush)Brushes.Transparent, (Pen)null, (System.Windows.Media.Geometry)streamGeometry);
        Vector vector4 = flag ? vector3 : vector1;
        drawingContext.DrawLine(pen, anchorPoint, anchorPoint + vector4);
      }
    }

    private void DrawCornerAdorner(DrawingContext drawingContext, Point anchorPoint, Matrix matrix, Brush brush, Pen pen)
    {
      Vector vector1 = new Vector(1.0, 0.0) * matrix;
      Vector vector2 = new Vector(0.0, 1.0) * matrix;
      vector1.Normalize();
      vector2.Normalize();
      anchorPoint += (15.0 - 0.5 * pen.Thickness) * vector2;
      anchorPoint += (5.0 + 0.5 * pen.Thickness) * vector1;
      double num = 10.0 - pen.Thickness;
      Vector vector3 = vector1 * num;
      Vector vector4 = vector2 * num;
      StreamGeometry streamGeometry = new StreamGeometry();
      StreamGeometryContext streamGeometryContext = streamGeometry.Open();
      streamGeometryContext.BeginFigure(anchorPoint, true, true);
      streamGeometryContext.LineTo(anchorPoint + vector3, true, false);
      streamGeometryContext.LineTo(anchorPoint + vector3 - vector4, true, false);
      streamGeometryContext.Close();
      streamGeometry.Freeze();
      if (this.IsEnabled)
      {
        drawingContext.DrawGeometry(brush, pen, (System.Windows.Media.Geometry) streamGeometry);
      }
      else
      {
          drawingContext.DrawGeometry((Brush)Brushes.Transparent, (Pen)null, (System.Windows.Media.Geometry)streamGeometry);
        drawingContext.DrawLine(pen, anchorPoint, anchorPoint + vector3);
        drawingContext.DrawLine(pen, anchorPoint + vector3, anchorPoint + vector3 - vector4);
      }
    }
  }
}
