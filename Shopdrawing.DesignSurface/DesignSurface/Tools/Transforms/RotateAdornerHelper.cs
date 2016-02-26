// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.RotateAdornerHelper
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  internal static class RotateAdornerHelper
  {
      private static Dictionary<EdgeFlags, System.Windows.Media.Geometry> sharedGeometries = new Dictionary<EdgeFlags, System.Windows.Media.Geometry>(16);
    private const double radius = 16.0;

    public static double Radius
    {
      get
      {
        return 16.0;
      }
    }

    public static void DrawAdorner(DrawingContext drawingContext, Point anchorPoint, EdgeFlags edgeFlags, Matrix matrix, Brush brush)
    {
      Vector transformedXAxis = new Vector(1.0, 0.0) * matrix;
      Vector transformedYAxis = new Vector(0.0, 1.0) * matrix;
      transformedXAxis.Normalize();
      transformedYAxis.Normalize();
      System.Windows.Media.Geometry geometry;
      Transform transform;
      if (Math.Abs(transformedXAxis * transformedYAxis) < 0.01)
      {
        if (!RotateAdornerHelper.sharedGeometries.TryGetValue(edgeFlags, out geometry))
        {
          geometry = RotateAdornerHelper.GetGeometry(edgeFlags, new Vector(1.0, 0.0), new Vector(0.0, 1.0), false);
          RotateAdornerHelper.sharedGeometries.Add(edgeFlags, geometry);
        }
        transform = (Transform) new MatrixTransform(transformedXAxis.X, transformedXAxis.Y, transformedYAxis.X, transformedYAxis.Y, anchorPoint.X, anchorPoint.Y);
      }
      else
      {
        bool isFlipped = matrix.Determinant < 0.0;
        geometry = RotateAdornerHelper.GetGeometry(edgeFlags, transformedXAxis, transformedYAxis, isFlipped);
        transform = (Transform) new TranslateTransform(anchorPoint.X, anchorPoint.Y);
      }
      transform.Freeze();
      drawingContext.PushTransform(transform);
      drawingContext.DrawGeometry(brush, (Pen) null, geometry);
      drawingContext.Pop();
    }

    public static void DrawAdorner(DrawingContext drawingContext, Point anchorPoint, EdgeFlags edgeFlags, Matrix matrix, Brush brush, Rect bounds)
    {
      Size ofTransformedRect = Adorner.GetSizeOfTransformedRect(bounds, matrix);
      if ((edgeFlags == EdgeFlags.Top || edgeFlags == EdgeFlags.Bottom) && (ofTransformedRect.Width < RotateAdornerHelper.Radius * 2.0 || ofTransformedRect.Height < SizeAdorner.Size) || (edgeFlags == EdgeFlags.Left || edgeFlags == EdgeFlags.Right) && (ofTransformedRect.Height < RotateAdornerHelper.Radius * 2.0 || ofTransformedRect.Width < SizeAdorner.Size) || (edgeFlags == EdgeFlags.TopLeft || edgeFlags == EdgeFlags.TopRight || (edgeFlags == EdgeFlags.BottomLeft || edgeFlags == EdgeFlags.BottomRight)) && (ofTransformedRect.Width < SizeAdorner.Size && ofTransformedRect.Height < SizeAdorner.Size))
        return;
      RotateAdornerHelper.DrawAdorner(drawingContext, anchorPoint, edgeFlags, matrix, brush);
    }

    private static System.Windows.Media.Geometry GetGeometry(EdgeFlags edgeFlags, Vector transformedXAxis, Vector transformedYAxis, bool isFlipped)
    {
      double num = 16.0;
      Vector vector1;
      Vector vector2;
      switch (edgeFlags)
      {
        case EdgeFlags.Left:
          vector1 = -transformedYAxis;
          vector2 = transformedYAxis;
          break;
        case EdgeFlags.Top:
          vector1 = transformedXAxis;
          vector2 = -transformedXAxis;
          break;
        case EdgeFlags.TopLeft:
          vector1 = transformedXAxis;
          vector2 = transformedYAxis;
          num -= SizeAdorner.Size / 2.0;
          break;
        case EdgeFlags.Right:
          vector1 = transformedYAxis;
          vector2 = -transformedYAxis;
          break;
        case EdgeFlags.TopRight:
          vector1 = transformedYAxis;
          vector2 = -transformedXAxis;
          num -= SizeAdorner.Size / 2.0;
          break;
        case EdgeFlags.Bottom:
          vector1 = -transformedXAxis;
          vector2 = transformedXAxis;
          break;
        case EdgeFlags.BottomLeft:
          vector1 = -transformedYAxis;
          vector2 = transformedXAxis;
          num -= SizeAdorner.Size / 2.0;
          break;
        case EdgeFlags.BottomRight:
          vector1 = -transformedXAxis;
          vector2 = -transformedYAxis;
          num -= SizeAdorner.Size / 2.0;
          break;
        default:
          throw new InvalidOperationException();
      }
      StreamGeometry streamGeometry = new StreamGeometry();
      StreamGeometryContext streamGeometryContext = streamGeometry.Open();
      streamGeometryContext.BeginFigure(new Point(), true, true);
      streamGeometryContext.LineTo((Point) (vector1 * num), true, false);
      streamGeometryContext.ArcTo((Point) (vector2 * num), new Size(16.0, 16.0), 0.0, true, isFlipped ? SweepDirection.Clockwise : SweepDirection.Counterclockwise, true, false);
      streamGeometryContext.Close();
      streamGeometry.Freeze();
      return (System.Windows.Media.Geometry)streamGeometry;
    }
  }
}
