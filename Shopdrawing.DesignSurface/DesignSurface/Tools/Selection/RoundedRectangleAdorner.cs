// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Selection.RoundedRectangleAdorner
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Geometry.Core;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Selection
{
  internal class RoundedRectangleAdorner : EventHandlingAdorner, IClickable
  {
    private bool shouldDraw = true;
    private static readonly double radius = 3.0;
    private static readonly double offsetDistance = 24.5;
    private static readonly double dashLength = 3.0;
    private static readonly DashStyle dashStyle = new DashStyle((IEnumerable<double>) new double[2]
    {
      RoundedRectangleAdorner.dashLength,
      RoundedRectangleAdorner.dashLength
    }, 0.0);
    private bool isX;

    public bool IsX
    {
      get
      {
        return this.isX;
      }
    }

    public double HalfStrokeThickness
    {
      get
      {
        double num = 0.0;
        if (this.Element.GetComputedValue(ShapeElement.StrokeProperty) is Brush)
          num = (double) this.Element.GetComputedValue(ShapeElement.StrokeThicknessProperty) / 2.0;
        return num;
      }
    }

    private double EffectiveRadiusX
    {
      get
      {
        return Math.Min(Math.Max(0.0, (double) this.Element.GetComputedValue(RectangleElement.RadiusXProperty)) + this.HalfStrokeThickness, this.Element.ViewModel.DefaultView.GetRenderSize(this.Element.Visual).Width / 2.0);
      }
    }

    private double EffectiveRadiusY
    {
      get
      {
        return Math.Min(Math.Max(0.0, (double) this.Element.GetComputedValue(RectangleElement.RadiusYProperty)) + this.HalfStrokeThickness, this.Element.ViewModel.DefaultView.GetRenderSize(this.Element.Visual).Height / 2.0);
      }
    }

    public Vector NormalDirection
    {
      get
      {
        Vector vector = new Vector(this.IsX ? 0.0 : 1.0, this.IsX ? 1.0 : 0.0) * this.AdornerSet.Matrix;
        vector.Normalize();
        return vector;
      }
    }

    public Vector EdgeDirection
    {
      get
      {
        Vector vector = new Vector(this.IsX ? 1.0 : 0.0, this.IsX ? 0.0 : 1.0) * this.AdornerSet.Matrix;
        vector.Normalize();
        return vector;
      }
    }

    static RoundedRectangleAdorner()
    {
      RoundedRectangleAdorner.dashStyle.Freeze();
    }

    public RoundedRectangleAdorner(AdornerSet adornerSet, bool isX)
      : base(adornerSet)
    {
      this.isX = isX;
    }

    protected override void HandleAdornerLayerEvent(AdornerPropertyChangedEventArgs eventArgs)
    {
      if (!(eventArgs.PropertyName == "RoundedRectangleAdornerIsVisible"))
        return;
      this.shouldDraw = (bool) eventArgs.Value;
      this.InvalidateRender();
    }

    public Point GetClickablePoint(Matrix matrix)
    {
      Point point = new Point(this.IsX ? this.EffectiveRadiusX : 0.0, this.IsX ? 0.0 : this.EffectiveRadiusY);
      Vector vector = -RoundedRectangleAdorner.offsetDistance * this.NormalDirection;
      return point * matrix + vector;
    }

    public override void Draw(DrawingContext context, Matrix matrix)
    {
      if (!this.shouldDraw || !PlatformTypes.Rectangle.IsAssignableFrom((ITypeId) this.Element.Type))
        return;
      Size ofTransformedRect = Adorner.GetSizeOfTransformedRect(this.ElementBounds, matrix);
      if (ofTransformedRect.Width < 5.0 || ofTransformedRect.Height < 5.0)
        return;
      Point clickablePoint = this.GetClickablePoint(matrix);
      Point point = new Point(this.EffectiveRadiusX, this.EffectiveRadiusY) * matrix;
      Pen pen = this.ThinPen;
      if (VectorUtilities.SquaredDistance(clickablePoint, point) < 10000000000.0)
      {
        pen = pen.Clone();
        pen.DashStyle = RoundedRectangleAdorner.dashStyle;
        pen.Freeze();
      }
      context.DrawLine(pen, point, clickablePoint);
      Vector vector1 = RoundedRectangleAdorner.radius * this.NormalDirection;
      Vector vector2 = new Vector(vector1.Y, -vector1.X);
      StreamGeometry streamGeometry = new StreamGeometry();
      StreamGeometryContext streamGeometryContext = streamGeometry.Open();
      streamGeometryContext.BeginFigure(clickablePoint + vector1, true, true);
      streamGeometryContext.PolyLineTo((IList<Point>) new Point[3]
      {
        clickablePoint + vector2,
        clickablePoint - vector1,
        clickablePoint - vector2
      }, 1 != 0, 0 != 0);
      streamGeometryContext.Close();
      streamGeometry.Freeze();
      Brush brush = this.IsActive ? this.ActiveBrush : this.InactiveBrush;
      context.DrawGeometry(brush, this.ThinPen, (System.Windows.Media.Geometry)streamGeometry);
    }
  }
}
