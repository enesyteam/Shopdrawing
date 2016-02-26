// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.BrushAdorner
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  internal abstract class BrushAdorner : Adorner, IClickable
  {
    protected const double ArrowHeadSize = 8.0;
    protected const double ArrowTailSize = 12.0;
    public const double ArrowHeadOffset = 7.0;

    private Base2DElement BaseElement
    {
      get
      {
        return (Base2DElement) this.Element;
      }
    }

    public BrushTransformAdornerSet AdornerSet
    {
      get
      {
        return (BrushTransformAdornerSet) base.AdornerSet;
      }
    }

    public object PlatformBrush
    {
      get
      {
        PropertyReference propertyReference = this.AdornerSet.BrushPropertyReference;
        if (propertyReference != null)
          return this.Element.GetComputedValue(propertyReference);
        return (object) null;
      }
    }

    public bool ShouldDraw
    {
      get
      {
        Rect elementBounds = this.ElementBounds;
        if ((PlatformTypes.IsInstance(this.PlatformBrush, PlatformTypes.GradientBrush, (ITypeResolver) this.Element.ProjectContext) || PlatformTypes.IsInstance(this.PlatformBrush, PlatformTypes.TileBrush, (ITypeResolver) this.Element.ProjectContext)) && elementBounds.Width > 0.0)
          return elementBounds.Height > 0.0;
        return false;
      }
    }

    public bool IsStretchedToFitBoundingBox
    {
      get
      {
        bool flag = false;
        object platformBrush = this.PlatformBrush;
        if (platformBrush != null)
        {
          ITypeId type = (ITypeId) this.Element.ProjectContext.GetType(platformBrush.GetType());
          if (PlatformTypes.GradientBrush.IsAssignableFrom(type))
            flag = (BrushMappingMode) this.GetBrushPropertyAsWpf(GradientBrushNode.MappingModeProperty) == BrushMappingMode.RelativeToBoundingBox;
          else if (PlatformTypes.TileBrush.IsAssignableFrom(type))
            flag = true;
        }
        return flag;
      }
    }

    public Rect BrushBounds
    {
      get
      {
        Rect rect = new Rect(0.0, 0.0, 1.0, 1.0);
        object platformBrush = this.PlatformBrush;
        if (platformBrush != null)
        {
          ITypeId type = (ITypeId) this.Element.ProjectContext.GetType(platformBrush.GetType());
          if (PlatformTypes.LinearGradientBrush.IsAssignableFrom(type))
          {
            rect = new Rect((Point) this.GetBrushPropertyAsWpf(LinearGradientBrushNode.StartPointProperty), (Point) this.GetBrushPropertyAsWpf(LinearGradientBrushNode.EndPointProperty));
            double num = Math.Max(rect.Width, rect.Height);
            rect.Offset((rect.Width - num) / 2.0, (rect.Height - num) / 2.0);
            rect.Width = num;
            rect.Height = num;
          }
          else if (PlatformTypes.RadialGradientBrush.IsAssignableFrom(type))
          {
            Point point = (Point) this.GetBrushPropertyAsWpf(RadialGradientBrushNode.CenterProperty);
            Vector vector = new Vector((double) this.GetBrushPropertyAsWpf(RadialGradientBrushNode.RadiusXProperty), (double) this.GetBrushPropertyAsWpf(RadialGradientBrushNode.RadiusYProperty));
            rect = new Rect(point - vector, point + vector);
          }
          else if (PlatformTypes.TileBrush.IsAssignableFrom(type))
          {
            ReferenceStep referenceStep = (ReferenceStep) this.Element.Platform.Metadata.ResolveProperty(TileBrushNode.ViewportProperty);
            if (referenceStep != null)
            {
              rect = (Rect) this.Element.ViewModel.DefaultView.ConvertToWpfValue(referenceStep.GetCurrentValue(this.PlatformBrush));
              if ((BrushMappingMode) this.GetBrushPropertyAsWpf(TileBrushNode.ViewportUnitsProperty) == BrushMappingMode.Absolute)
              {
                Rect elementBounds = this.ElementBounds;
                rect.Offset(-elementBounds.X, -elementBounds.Y);
                rect.Scale(elementBounds.Width == 0.0 ? 1.0 : 1.0 / elementBounds.Width, elementBounds.Height == 0.0 ? 1.0 : 1.0 / elementBounds.Height);
              }
            }
          }
        }
        return rect;
      }
    }

    public Point BrushCenter
    {
      get
      {
        Rect brushBounds = this.BrushBounds;
        return new Point((brushBounds.Left + brushBounds.Right) / 2.0, (brushBounds.Top + brushBounds.Bottom) / 2.0);
      }
    }

    public Point RelativeBrushCenter
    {
      get
      {
        Point brushCenter = this.BrushCenter;
        object platformBrush = this.PlatformBrush;
        if (platformBrush != null)
        {
          ITypeId type = (ITypeId) this.Element.ProjectContext.GetType(platformBrush.GetType());
          if (PlatformTypes.GradientBrush.IsAssignableFrom(type) && (BrushMappingMode) this.GetBrushPropertyAsWpf(GradientBrushNode.MappingModeProperty) == BrushMappingMode.Absolute)
          {
            Rect elementBounds = this.ElementBounds;
            brushCenter.Offset(-elementBounds.X, -elementBounds.Y);
            brushCenter.X /= elementBounds.Width != 0.0 ? elementBounds.Width : 1.0;
            brushCenter.Y /= elementBounds.Height != 0.0 ? elementBounds.Height : 1.0;
          }
        }
        return brushCenter;
      }
    }

    public Transform BrushTransform
    {
      get
      {
        Transform transform = Transform.Identity;
        if (this.PlatformBrush != null)
          transform = (Transform) this.GetBrushPropertyAsWpf(BrushNode.RelativeTransformProperty);
        return transform ?? Transform.Identity;
      }
    }

    public BrushAdorner(BrushTransformAdornerSet adornerSet)
      : base((Microsoft.Expression.DesignSurface.UserInterface.AdornerSet) adornerSet)
    {
    }

    public object GetBrushPropertyAsWpf(IPropertyId key)
    {
      return BrushAdorner.GetBrushPropertyAsWpf(this.BaseElement, this.PlatformBrush, key);
    }

    public static object GetBrushPropertyAsWpf(Base2DElement element, object brush, IPropertyId key)
    {
      ReferenceStep referenceStep = (ReferenceStep) element.Platform.Metadata.ResolveProperty(key);
      return element.ViewModel.DefaultView.ConvertToWpfValue(referenceStep.GetCurrentValue(brush));
    }

    public static bool IsAdorningFillProperty(SceneElement element)
    {
      return new PropertyReference((ReferenceStep) element.ProjectContext.ResolveProperty(ShapeElement.FillProperty)).Equals((object) BrushTool.GetBrushPropertyReference((SceneNode) element));
    }

    public static double GetStrokeWidth(SceneElement element)
    {
      if (element.GetComputedValue(ShapeElement.StrokeProperty) != null)
      {
        double d = (double) element.GetComputedValue(ShapeElement.StrokeThicknessProperty);
        if (!double.IsNaN(d) && !double.IsInfinity(d))
          return d;
      }
      return 0.0;
    }

    public static Matrix GetStrokeTransform(SceneElement element)
    {
      Matrix matrix = new Matrix();
      ShapeElement shapeElement = element as ShapeElement;
      if (shapeElement != null)
      {
        Rect computedTightBounds = shapeElement.GetComputedTightBounds();
        double strokeWidth = BrushAdorner.GetStrokeWidth(element);
        double scaleX = computedTightBounds.Width == 0.0 ? 1.0 : 1.0 - strokeWidth / computedTightBounds.Width;
        double scaleY = computedTightBounds.Height == 0.0 ? 1.0 : 1.0 - strokeWidth / computedTightBounds.Height;
        matrix.Translate(-0.5, -0.5);
        matrix.Scale(scaleX, scaleY);
        matrix.Translate(0.5, 0.5);
      }
      return matrix;
    }

    public Point TransformPoint(Point point, bool includeTransformFromElementToBrushCoordinates)
    {
      Matrix brushTransformMatrix = this.GetCompleteBrushTransformMatrix(includeTransformFromElementToBrushCoordinates);
      point *= brushTransformMatrix;
      return point;
    }

    public Vector TransformVector(Vector vector, bool includeTransformFromElementToBrushCoordinates)
    {
      Matrix brushTransformMatrix = this.GetCompleteBrushTransformMatrix(includeTransformFromElementToBrushCoordinates);
      vector *= brushTransformMatrix;
      return vector;
    }

    public Point InverseTransformPoint(Point point, bool includeTransformFromElementToBrushCoordinates)
    {
      Matrix brushTransformMatrix = this.GetCompleteInverseBrushTransformMatrix(includeTransformFromElementToBrushCoordinates);
      point *= brushTransformMatrix;
      return point;
    }

    public Vector InverseTransformVector(Vector vector, bool includeTransformFromElementToBrushCoordinates)
    {
      Matrix brushTransformMatrix = this.GetCompleteInverseBrushTransformMatrix(includeTransformFromElementToBrushCoordinates);
      vector *= brushTransformMatrix;
      return vector;
    }

    protected void DrawArrowBackdrop(DrawingContext context, Matrix rotation, Point corner1, Point corner2)
    {
      Point startPoint = new Point(corner1.X, corner1.Y) * rotation;
      Point point1 = new Point(corner2.X, corner1.Y) * rotation;
      Point point2 = new Point(corner2.X, corner2.Y) * rotation;
      Point point3 = new Point(corner1.X, corner2.Y) * rotation;
      StreamGeometry streamGeometry = new StreamGeometry();
      StreamGeometryContext streamGeometryContext = streamGeometry.Open();
      streamGeometryContext.BeginFigure(startPoint, true, true);
      streamGeometryContext.PolyLineTo((IList<Point>) new Point[3]
      {
        point1,
        point2,
        point3
      }, 0 != 0, 0 != 0);
      streamGeometryContext.Close();
      streamGeometry.Freeze();
      context.DrawGeometry((Brush)Brushes.Transparent, (Pen)null, (System.Windows.Media.Geometry)streamGeometry);
    }

    public abstract Point GetClickablePoint(Matrix matrix);

    [Conditional("DEBUG")]
    protected void DrawClickablePoint(DrawingContext context, Matrix m)
    {
      Point clickablePoint = this.GetClickablePoint(m);
      Pen pen = new Pen((Brush) Brushes.Red, 1.0);
      pen.Freeze();
      context.DrawLine(pen, clickablePoint + new Vector(5.0, 0.0), clickablePoint + new Vector(-5.0, 0.0));
      context.DrawLine(pen, clickablePoint + new Vector(0.0, 5.0), clickablePoint + new Vector(0.0, -5.0));
    }

    protected void DrawArrowHead(DrawingContext context, Matrix matrix, Point startPoint, Point endPoint)
    {
      startPoint *= matrix;
      endPoint *= matrix;
      Matrix identity = Matrix.Identity;
      identity.RotateAt(Math.Atan2(endPoint.Y - startPoint.Y, endPoint.X - startPoint.X) * 180.0 / Math.PI, endPoint.X, endPoint.Y);
      Point startPoint1 = new Point(endPoint.X + 7.0, endPoint.Y + 4.0) * identity;
      Point point1 = new Point(endPoint.X + 7.0, endPoint.Y - 4.0) * identity;
      Point point2 = new Point(endPoint.X + 8.0 + 7.0, endPoint.Y) * identity;
      StreamGeometry streamGeometry = new StreamGeometry();
      StreamGeometryContext streamGeometryContext = streamGeometry.Open();
      streamGeometryContext.BeginFigure(startPoint1, true, true);
      streamGeometryContext.LineTo(point1, true, false);
      streamGeometryContext.LineTo(point2, true, false);
      streamGeometryContext.Close();
      streamGeometry.Freeze();
      this.DrawArrowBackdrop(context, identity, new Point(endPoint.X, endPoint.Y - 4.0), new Point(endPoint.X + 8.0 + 7.0, endPoint.Y + 4.0));
      Brush brush = this.IsActive ? this.ActiveBrush : this.InactiveBrush;
      context.DrawGeometry(brush, this.ThinPen, (System.Windows.Media.Geometry)streamGeometry);
    }

    protected void DrawArrowTail(DrawingContext context, Matrix matrix, Point startPoint, Point endPoint)
    {
      startPoint *= matrix;
      endPoint *= matrix;
      Matrix identity = Matrix.Identity;
      identity.RotateAt(Math.Atan2(endPoint.Y - startPoint.Y, endPoint.X - startPoint.X) * 180.0 / Math.PI, startPoint.X, startPoint.Y);
      Point startPoint1 = new Point(startPoint.X - 8.0 - 7.0, startPoint.Y) * identity;
      Point point1 = new Point(startPoint.X - 12.0 - 7.0, startPoint.Y - 4.0) * identity;
      Point point2 = new Point(startPoint.X - 4.0 - 7.0, startPoint.Y - 4.0) * identity;
      Point point3 = new Point(startPoint.X - 7.0, startPoint.Y) * identity;
      Point point4 = new Point(startPoint.X - 4.0 - 7.0, startPoint.Y + 4.0) * identity;
      Point point5 = new Point(startPoint.X - 12.0 - 7.0, startPoint.Y + 4.0) * identity;
      StreamGeometry streamGeometry = new StreamGeometry();
      StreamGeometryContext streamGeometryContext = streamGeometry.Open();
      streamGeometryContext.BeginFigure(startPoint1, true, true);
      streamGeometryContext.PolyLineTo((IList<Point>) new Point[5]
      {
        point1,
        point2,
        point3,
        point4,
        point5
      }, 1 != 0, 0 != 0);
      streamGeometryContext.Close();
      streamGeometry.Freeze();
      this.DrawArrowBackdrop(context, identity, new Point(startPoint.X, startPoint.Y - 6.0), new Point(startPoint.X - 12.0 - 7.0, startPoint.Y + 6.0));
      Brush brush = this.IsActive ? this.ActiveBrush : this.InactiveBrush;
      context.DrawGeometry(brush, this.ThinPen, (System.Windows.Media.Geometry)streamGeometry);
    }

    public static Matrix GetBrushMatrix(Base2DElement element, object brush, IPropertyId matrixProperty)
    {
      Transform transform = BrushAdorner.GetBrushPropertyAsWpf(element, brush, matrixProperty) as Transform;
      if (transform != null)
        return transform.Value;
      return Matrix.Identity;
    }

    public Matrix GetCompleteInverseBrushTransformMatrix(bool includeTransformFromElementToBrushCoordinates)
    {
      return BrushAdorner.GetCompleteInverseBrushTransformMatrix(this.BaseElement, this.PlatformBrush, includeTransformFromElementToBrushCoordinates);
    }

    public static Matrix GetCompleteInverseBrushTransformMatrix(Base2DElement element, object brush, bool includeTransformFromElementToBrushCoordinates)
    {
      return ElementUtilities.GetInverseMatrix(BrushAdorner.GetCompleteBrushTransformMatrix(element, brush, includeTransformFromElementToBrushCoordinates));
    }

    public Matrix GetCompleteBrushTransformMatrix(bool includeTransformFromElementToBrushCoordinates)
    {
      return BrushAdorner.GetCompleteBrushTransformMatrix(this.BaseElement, this.PlatformBrush, includeTransformFromElementToBrushCoordinates);
    }

    public static Matrix GetCompleteBrushTransformMatrix(Base2DElement element, object brush, bool includeTransformFromElementToBrushCoordinates)
    {
      Matrix matrix1 = Matrix.Identity;
      if (brush != null && element != null)
      {
        Rect computedTightBounds = element.GetComputedTightBounds();
        Matrix matrix2 = BrushAdorner.GetBrushMatrix(element, brush, BrushNode.RelativeTransformProperty);
        if (PlatformTypes.IsInstance(brush, PlatformTypes.GradientBrush, (ITypeResolver) element.ProjectContext))
        {
          if ((BrushMappingMode) BrushAdorner.GetBrushPropertyAsWpf(element, brush, GradientBrushNode.MappingModeProperty) == BrushMappingMode.Absolute)
          {
            if (includeTransformFromElementToBrushCoordinates)
            {
              Matrix identity = Matrix.Identity;
              identity.Translate(-computedTightBounds.X, -computedTightBounds.Y);
              double scaleX = computedTightBounds.Width == 0.0 ? 1.0 : 1.0 / computedTightBounds.Width;
              double scaleY = computedTightBounds.Height == 0.0 ? 1.0 : 1.0 / computedTightBounds.Height;
              identity.Scale(scaleX, scaleY);
              matrix2 = identity * matrix2;
            }
          }
          else if (BrushAdorner.IsAdorningFillProperty((SceneElement) element))
            matrix2 *= BrushAdorner.GetStrokeTransform((SceneElement) element);
        }
        if (PlatformTypes.IsInstance(brush, PlatformTypes.TileBrush, (ITypeResolver) element.ProjectContext) && BrushAdorner.IsAdorningFillProperty((SceneElement) element))
          matrix2 *= BrushAdorner.GetStrokeTransform((SceneElement) element);
        Matrix matrix3 = new Matrix(computedTightBounds.Width, 0.0, 0.0, computedTightBounds.Height, computedTightBounds.X, computedTightBounds.Y);
        matrix1 = matrix2 * matrix3 * BrushAdorner.GetBrushMatrix(element, brush, BrushNode.TransformProperty);
      }
      return matrix1;
    }

    public bool GetBrushEndpoints(out Point startPoint, out Point endPoint)
    {
      startPoint = new Point();
      endPoint = new Point();
      if (PlatformTypes.IsInstance(this.PlatformBrush, PlatformTypes.LinearGradientBrush, (ITypeResolver) this.Element.ProjectContext))
      {
        startPoint = (Point) this.GetBrushPropertyAsWpf(LinearGradientBrushNode.StartPointProperty);
        endPoint = (Point) this.GetBrushPropertyAsWpf(LinearGradientBrushNode.EndPointProperty);
        return true;
      }
      if (!PlatformTypes.IsInstance(this.PlatformBrush, PlatformTypes.RadialGradientBrush, (ITypeResolver) this.Element.ProjectContext))
        return false;
      startPoint = (Point) this.GetBrushPropertyAsWpf(RadialGradientBrushNode.GradientOriginProperty);
      endPoint = this.GetRadialGradientEndPoint();
      return true;
    }

    protected Point GetRadialGradientEndPoint()
    {
      Point point = new Point();
      if (PlatformTypes.IsInstance(this.PlatformBrush, PlatformTypes.RadialGradientBrush, (ITypeResolver) this.Element.ProjectContext))
      {
        Vector direction = (Point) this.GetBrushPropertyAsWpf(RadialGradientBrushNode.CenterProperty) - (Point) this.GetBrushPropertyAsWpf(RadialGradientBrushNode.GradientOriginProperty);
        if (direction.X == 0.0 && direction.Y == 0.0)
          direction = new Vector(1.0, -1.0);
        point = (Point) this.GetBrushPropertyAsWpf(RadialGradientBrushNode.CenterProperty) + BrushAdorner.IntersectRayAndEllipse(direction, (double) this.GetBrushPropertyAsWpf(RadialGradientBrushNode.RadiusXProperty), (double) this.GetBrushPropertyAsWpf(RadialGradientBrushNode.RadiusYProperty));
      }
      return point;
    }

    protected void GetBrushOffsetEndpoints(out Point startPoint, out Point endPoint, double startOffset, double endOffset, Matrix matrix)
    {
      this.GetBrushEndpoints(out startPoint, out endPoint);
      Matrix matrix1 = this.GetCompleteBrushTransformMatrix(true) * matrix;
      startPoint *= matrix1;
      endPoint *= matrix1;
      Vector vector = startPoint - endPoint;
      if (Tolerances.NearZero(vector.LengthSquared))
        vector = new Vector(1.0, 0.0);
      else
        vector.Normalize();
      startPoint += startOffset * vector;
      endPoint -= endOffset * vector;
    }

    protected Point GetOffsetStartPoint(Matrix matrix, double offset)
    {
      Point startPoint = new Point();
      Point endPoint = new Point();
      this.GetBrushOffsetEndpoints(out startPoint, out endPoint, offset, 0.0, matrix);
      return startPoint;
    }

    protected Point GetOffsetEndPoint(Matrix matrix, double offset)
    {
      Point startPoint = new Point();
      Point endPoint = new Point();
      this.GetBrushOffsetEndpoints(out startPoint, out endPoint, 0.0, offset, matrix);
      return endPoint;
    }

    private static Vector IntersectRayAndEllipse(Vector direction, double radiusX, double radiusY)
    {
      if (direction.X == 0.0)
        return new Vector(0.0, (double) Math.Sign(direction.Y) * radiusY);
      if (radiusY == 0.0)
        return new Vector((double) Math.Sign(direction.X) * radiusX, 0.0);
      double num1 = direction.Y / direction.X;
      double num2 = num1 * radiusX / radiusY;
      double x = (double) Math.Sign(direction.X) * radiusX * Math.Sqrt(1.0 / (1.0 + num2 * num2));
      double y = num1 * x;
      return new Vector(x, y);
    }
  }
}
