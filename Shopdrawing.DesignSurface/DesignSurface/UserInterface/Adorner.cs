// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Adorner
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  public abstract class Adorner : DrawingVisual, IAdorner
  {
    private bool needsRedraw = true;
    private AdornerSet adornerSet;
    private bool isActive;

    public AdornerSet AdornerSet
    {
      get
      {
        return this.adornerSet;
      }
    }

    IAdornerSet IAdorner.AdornerSet
    {
      get
      {
        return (IAdornerSet) this.AdornerSet;
      }
    }

    public bool IsActive
    {
      get
      {
        return this.isActive;
      }
      set
      {
        if (this.isActive == value)
          return;
        this.isActive = value;
        this.OnIsActiveChanged();
      }
    }

    public SceneElement Element
    {
      get
      {
        return this.ElementSet.PrimaryElement;
      }
    }

    public AdornerElementSet ElementSet
    {
      get
      {
        return this.adornerSet.ElementSet;
      }
    }

    public virtual bool SupportsProjectionTransforms
    {
      get
      {
        return false;
      }
    }

    public virtual object ToolTip
    {
      get
      {
        return (object) null;
      }
    }

    internal AdornerType AdornerType
    {
      get
      {
        return !this.ElementSet.AdornsMultipleElements ? AdornerType.Default : AdornerType.MultipleElement;
      }
    }

    internal DesignerContext DesignerContext
    {
      get
      {
        return this.Element.DesignerContext;
      }
    }

    protected Rect ElementBounds
    {
      get
      {
        if (!this.ElementSet.AdornsMultipleElements && this.Element.ViewTargetElement != null)
          return this.Element.ViewModel.DefaultView.GetActualBounds(this.Element.ViewTargetElement);
        return this.ElementSet.ElementBounds;
      }
    }

    protected Pen ThinPen
    {
      get
      {
        return FeedbackHelper.GetThinPen(this.AdornerType);
      }
    }

    protected Pen MediumPen
    {
      get
      {
        return FeedbackHelper.GetMediumPen(this.AdornerType);
      }
    }

    protected Pen ThickPen
    {
      get
      {
        return FeedbackHelper.GetThickPen();
      }
    }

    protected Brush InactiveBrush
    {
      get
      {
        return FeedbackHelper.GetInactiveBrush();
      }
    }

    protected Brush ActiveBrush
    {
      get
      {
        return FeedbackHelper.GetActiveBrush(this.AdornerType);
      }
    }

    protected Adorner(AdornerSet adornerSet)
    {
      this.adornerSet = adornerSet;
    }

    public void InvalidateRender()
    {
      this.needsRedraw = true;
    }

    public void Redraw()
    {
      if (!this.needsRedraw || this.Element.Visual == null)
        return;
      this.needsRedraw = false;
      DrawingContext drawingContext = this.RenderOpen();
      if (this.SupportsProjectionTransforms || !this.ElementSet.HasNonAffineTransform)
        this.Draw(drawingContext, this.AdornerSet.Matrix);
      drawingContext.Close();
    }

    public virtual void OnRemove()
    {
    }

    public virtual void OnMouseEnter()
    {
    }

    public virtual void OnMouseLeave()
    {
    }

    public Point TransformPoint(Point point)
    {
      SceneView activeView = this.DesignerContext.ActiveView;
      Transform transform = activeView.Artboard.CalculateTransformFromContentToArtboard();
      point = activeView.TransformPoint(this.Element.Visual, (IViewObject) activeView.HitTestRoot, point);
      return transform.Transform(point);
    }

    public static bool NonAffineTransformInParentStack(SceneElement element)
    {
      if (element.Visual == null)
        return false;
      return !element.DesignerContext.ActiveView.IsMatrixTransform(element.Visual, (IViewObject) element.DesignerContext.ActiveView.HitTestRoot);
    }

    public static Size GetSizeOfTransformedRect(Rect rect, Matrix matrix)
    {
      return new Size(matrix.Transform(rect.TopRight - rect.TopLeft).Length, matrix.Transform(rect.BottomLeft - rect.TopLeft).Length);
    }

    public static System.Windows.Media.Geometry GetTransformedRectangleGeometry(Rect rect, Matrix matrix, double thickness)
    {
      if (rect.IsEmpty)
          return (System.Windows.Media.Geometry)null;
      Vector vector1 = new Vector(1.0, 0.0) * matrix;
      Vector vector2 = new Vector(0.0, 1.0) * matrix;
      Vector v1 = new Vector(vector2.Y, -vector2.X);
      Vector v2 = new Vector(vector1.Y, -vector1.X);
      if (v1 * vector1 < 0.0)
        v1 *= -1.0;
      if (v2 * vector2 < 0.0)
        v2 *= -1.0;
      double length1 = v1.Length;
      if (length1 > 0.0)
        v1 /= length1;
      double length2 = v2.Length;
      if (length2 > 0.0)
        v2 /= length2;
      Vector vector3 = (Vector) (rect.TopLeft * matrix);
      Vector vector4 = (Vector) (rect.BottomRight * matrix);
      double num = thickness / 2.0;
      double c1_1 = v1 * vector3 - num;
      double c2_1 = v2 * vector3 - num;
      double c1_2 = v1 * vector4 + num;
      double c2_2 = v2 * vector4 + num;
      Point intersection1 = Adorner.GetIntersection(v1, c1_1, v2, c2_1);
      Point intersection2 = Adorner.GetIntersection(v1, c1_2, v2, c2_1);
      Point intersection3 = Adorner.GetIntersection(v1, c1_1, v2, c2_2);
      Point intersection4 = Adorner.GetIntersection(v1, c1_2, v2, c2_2);
      StreamGeometry streamGeometry = new StreamGeometry();
      StreamGeometryContext streamGeometryContext = streamGeometry.Open();
      streamGeometryContext.BeginFigure(intersection1, true, true);
      streamGeometryContext.PolyLineTo((IList<Point>) new Point[3]
      {
        intersection2,
        intersection4,
        intersection3
      }, 1 != 0, 0 != 0);
      streamGeometryContext.Close();
      streamGeometry.Freeze();
      return (System.Windows.Media.Geometry)streamGeometry;
    }

    public System.Windows.Media.Geometry GetTransformedGeometry(SceneView view, SceneElement element, System.Windows.Media.Geometry renderedGeometry, Matrix matrix)
    {
      if (Adorner.NonAffineTransformInParentStack(element))
      {
        PathGeometry pathGeometry = renderedGeometry.GetFlattenedPathGeometry().Clone();
        Transform transform = view.Artboard.CalculateTransformFromContentToArtboard();
        foreach (PathFigure pathFigure in pathGeometry.Figures)
        {
          Point startPoint = pathFigure.StartPoint;
          Point point1 = view.TransformPoint(element.Visual, (IViewObject) view.HitTestRoot, startPoint);
          pathFigure.StartPoint = transform.Transform(point1);
          foreach (PathSegment pathSegment in pathFigure.Segments)
          {
            LineSegment lineSegment;
            if ((lineSegment = pathSegment as LineSegment) != null)
            {
              Point point2 = lineSegment.Point;
              Point point3 = view.TransformPoint(element.Visual, (IViewObject) view.HitTestRoot, point2);
              lineSegment.Point = transform.Transform(point3);
            }
            else
            {
              PolyLineSegment polyLineSegment;
              if ((polyLineSegment = pathSegment as PolyLineSegment) != null)
              {
                for (int index = 0; index < polyLineSegment.Points.Count; ++index)
                {
                  Point point2 = polyLineSegment.Points[index];
                  Point point3 = view.TransformPoint(element.Visual, (IViewObject) view.HitTestRoot, point2);
                  polyLineSegment.Points[index] = transform.Transform(point3);
                }
              }
            }
          }
        }
        return (System.Windows.Media.Geometry)pathGeometry;
      }
      Transform transform1 = renderedGeometry.Transform;
      if (transform1 != null)
        matrix = transform1.Value * matrix;
      MatrixTransform matrixTransform = new MatrixTransform(matrix);
      matrixTransform.Freeze();
      if (renderedGeometry.IsFrozen)
      {
        GeometryCollection geometryCollection = new GeometryCollection(1);
        geometryCollection.Add(renderedGeometry);
        geometryCollection.Freeze();
        renderedGeometry = (System.Windows.Media.Geometry)new GeometryGroup()
        {
          Children = geometryCollection
        };
      }
      renderedGeometry.Transform = (Transform) matrixTransform;
      renderedGeometry.Freeze();
      return renderedGeometry;
    }

    public static System.Windows.Media.Geometry GetTransformedRectangleGeometry(SceneView view, SceneElement element, Rect rect, double thickness, bool applyArtboardTransform)
    {
      return Adorner.GetTransformedRectangleGeometry(view, element, new Matrix?(), rect, thickness, applyArtboardTransform);
    }

    public static System.Windows.Media.Geometry GetTransformedRectangleGeometry(SceneView view, SceneElement element, Matrix? additionalTransform, Rect rect, double thickness, bool applyArtboardTransform)
    {
      if (rect.IsEmpty || element.Visual == null)
          return (System.Windows.Media.Geometry)null;
      Point point1 = rect.TopLeft;
      Point point2 = rect.BottomRight;
      Point point3 = rect.TopRight;
      Point point4 = rect.BottomLeft;
      if (additionalTransform.HasValue)
      {
        point1 = additionalTransform.Value.Transform(point1);
        point2 = additionalTransform.Value.Transform(point2);
        point3 = additionalTransform.Value.Transform(point3);
        point4 = additionalTransform.Value.Transform(point4);
      }
      Point point5 = view.TransformPoint(element.Visual, (IViewObject) view.HitTestRoot, point1);
      Point point6 = view.TransformPoint(element.Visual, (IViewObject) view.HitTestRoot, point2);
      Point point7 = view.TransformPoint(element.Visual, (IViewObject) view.HitTestRoot, point3);
      Point point8 = view.TransformPoint(element.Visual, (IViewObject) view.HitTestRoot, point4);
      if (applyArtboardTransform)
      {
        Transform transform = view.Artboard.CalculateTransformFromContentToArtboard();
        point5 = transform.Transform(point5);
        point6 = transform.Transform(point6);
        point7 = transform.Transform(point7);
        point8 = transform.Transform(point8);
      }
      if (thickness > 0.0)
      {
        Vector vector1 = point5 - point6;
        double length1 = vector1.Length;
        if (length1 > 0.0)
        {
          vector1.Normalize();
          Point point9 = point6 + vector1 * (length1 + 1.414 * thickness / 2.0);
          point6 = point5 - vector1 * (length1 + 1.414 * thickness / 2.0);
          point5 = point9;
        }
        Vector vector2 = point7 - point8;
        double length2 = vector2.Length;
        if (length2 > 0.0)
        {
          vector2.Normalize();
          Point point9 = point8 + vector2 * (length2 + 1.414 * thickness / 2.0);
          point8 = point7 - vector2 * (length2 + 1.414 * thickness / 2.0);
          point7 = point9;
        }
      }
      StreamGeometry streamGeometry = new StreamGeometry();
      StreamGeometryContext streamGeometryContext = streamGeometry.Open();
      streamGeometryContext.BeginFigure(point5, true, true);
      streamGeometryContext.PolyLineTo((IList<Point>) new Point[3]
      {
        point7,
        point6,
        point8
      }, 1 != 0, 0 != 0);
      streamGeometryContext.Close();
      streamGeometry.Freeze();
      return (System.Windows.Media.Geometry)streamGeometry;
    }

    protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParams)
    {
      HitTestResult hitTestResult = (HitTestResult) null;
      if (this is IClickable && this.AdornerSet.Matrix.M12 < 1E+15 && this.AdornerSet.Matrix.M21 < 1E+15)
        hitTestResult = base.HitTestCore(hitTestParams);
      return hitTestResult;
    }

    protected virtual void OnIsActiveChanged()
    {
      this.InvalidateRender();
      this.Redraw();
    }

    public abstract void Draw(DrawingContext drawingContext, Matrix matrix);

    private static Point GetIntersection(Vector v1, double c1, Vector v2, double c2)
    {
      double num = Vector.Determinant(v1, v2);
      Vector vector1 = new Vector(v1.X, v2.X);
      Vector vector2 = new Vector(v1.Y, v2.Y);
      Vector vector = new Vector(c1, c2);
      return new Point(Vector.Determinant(vector, vector2) / num, Vector.Determinant(vector1, vector) / num);
    }
  }
}
