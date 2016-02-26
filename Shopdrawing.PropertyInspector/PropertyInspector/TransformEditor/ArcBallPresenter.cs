// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.TransformEditor.ArcBallPresenter
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.Framework;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.TransformEditor
{
  public sealed class ArcBallPresenter : Control
  {
    private static readonly double tangentSize = 4.0 * (Math.Sqrt(2.0) - 1.0) / 3.0;
    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof (Rotation3D), typeof (ArcBallPresenter), (PropertyMetadata) new FrameworkPropertyMetadata((object) Rotation3D.Identity, FrameworkPropertyMetadataOptions.AffectsRender));
    public static readonly DependencyProperty StrokeBrushProperty = DependencyProperty.Register("StrokeBrush", typeof (Brush), typeof (ArcBallPresenter), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(ArcBallPresenter.StrokeBrushChanged)));
    private Geometry XYFrontGeometry;
    private Geometry XZFrontGeometry;
    private Geometry YZFrontGeometry;
    private Geometry XYBackGeometry;
    private Geometry YZBackGeometry;
    private Geometry XZBackGeometry;
    private Pen frontPen;
    private Pen backPen;

    public Rotation3D Orientation
    {
      get
      {
        return (Rotation3D) this.GetValue(ArcBallPresenter.OrientationProperty);
      }
      set
      {
        this.SetValue(ArcBallPresenter.OrientationProperty, (object) value);
      }
    }

    public Brush StrokeBrush
    {
      get
      {
        return (Brush) this.GetValue(ArcBallPresenter.StrokeBrushProperty);
      }
      set
      {
        this.SetValue(ArcBallPresenter.StrokeBrushProperty, (object) value);
      }
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
      base.OnRender(drawingContext);
      this.UpdateGeometries(Helper3D.QuaternionFromRotation3D((Rotation3D) this.GetValue(ArcBallPresenter.OrientationProperty)));
      drawingContext.DrawRectangle((Brush) Brushes.Transparent, (Pen) null, new Rect(0.0, 0.0, this.ActualWidth, this.ActualHeight));
      drawingContext.DrawGeometry((Brush) null, this.backPen, this.XYBackGeometry);
      drawingContext.DrawGeometry((Brush) null, this.backPen, this.XZBackGeometry);
      drawingContext.DrawGeometry((Brush) null, this.backPen, this.YZBackGeometry);
      drawingContext.DrawGeometry((Brush) null, this.frontPen, this.XYFrontGeometry);
      drawingContext.DrawGeometry((Brush) null, this.frontPen, this.XZFrontGeometry);
      drawingContext.DrawGeometry((Brush) null, this.frontPen, this.YZFrontGeometry);
    }

    private static void StrokeBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ArcBallPresenter arcBallPresenter = d as ArcBallPresenter;
      if (arcBallPresenter == null)
        return;
      Brush brush1 = arcBallPresenter.StrokeBrush.Clone();
      Brush brush2 = arcBallPresenter.StrokeBrush.Clone();
      brush1.Opacity = 0.5;
      arcBallPresenter.frontPen = new Pen(brush1, 1.0);
      arcBallPresenter.backPen = new Pen(brush2, 0.5);
      arcBallPresenter.frontPen.Freeze();
      arcBallPresenter.backPen.Freeze();
    }

    private void UpdateGeometries(Quaternion orientation)
    {
      Matrix3D matrix3D = new RotateTransform3D((Rotation3D) new AxisAngleRotation3D(orientation.Axis, orientation.Angle), new Point3D()).Value;
      Vector3D vector3D1 = matrix3D.Transform(new Vector3D(1.0, 0.0, 0.0));
      Vector3D vector3D2 = matrix3D.Transform(new Vector3D(0.0, 1.0, 0.0));
      Vector3D vector3D3 = matrix3D.Transform(new Vector3D(0.0, 0.0, 1.0));
      PathGeometry frontGeometry;
      PathGeometry backGeometry;
      this.GetArcGeometry(vector3D1, vector3D2, vector3D3, out frontGeometry, out backGeometry);
      this.XYFrontGeometry = (Geometry) frontGeometry;
      this.XYBackGeometry = (Geometry) backGeometry;
      this.GetArcGeometry(vector3D1, vector3D3, -vector3D2, out frontGeometry, out backGeometry);
      this.XZFrontGeometry = (Geometry) frontGeometry;
      this.XZBackGeometry = (Geometry) backGeometry;
      this.GetArcGeometry(vector3D2, vector3D3, vector3D1, out frontGeometry, out backGeometry);
      this.YZFrontGeometry = (Geometry) frontGeometry;
      this.YZBackGeometry = (Geometry) backGeometry;
    }

    private void GetArcGeometry(Vector3D axis1, Vector3D axis2, Vector3D normal, out PathGeometry frontGeometry, out PathGeometry backGeometry)
    {
      Vector3D vector3D = Vector3D.CrossProduct(normal, new Vector3D(0.0, 0.0, 1.0));
      if (Tolerances.NearZero(vector3D.Length))
      {
        Point point = new Point(this.ActualWidth / 2.0, this.ActualHeight / 2.0);
        double num = this.ActualWidth / 2.0;
        Vector vector1 = new Vector(1.0, 0.0);
        Vector vector2 = new Vector(0.0, -1.0);
        Point top = point + vector2 * num;
        Point bottom = point - vector2 * num;
        Point left = point - vector1 * num;
        Point right = point + vector1 * num;
        double horizontalScale = num * ArcBallPresenter.tangentSize;
        double verticalScale = num * ArcBallPresenter.tangentSize;
        frontGeometry = this.GetEllipseGeometry(left, top, right, bottom, horizontalScale, verticalScale);
        backGeometry = new PathGeometry();
      }
      else
      {
        vector3D.Normalize();
        axis1 = vector3D;
        axis2 = Vector3D.CrossProduct(normal, axis1);
        if (axis2.Z < 0.0)
          axis2 = -axis2;
        Vector vector1 = new Vector(axis1.X, -axis1.Y);
        Vector vector2 = new Vector(axis2.X, -axis2.Y);
        Point center = new Point(this.ActualWidth / 2.0, this.ActualHeight / 2.0);
        double num = this.ActualWidth / 2.0;
        Point top1 = center + vector2 * num;
        Point top2 = center - vector2 * num;
        Point point1 = center + vector1 * num;
        Point point2 = center - vector1 * num;
        double horizontalScale = vector1.Length * num * ArcBallPresenter.tangentSize;
        double verticalScale = vector2.Length * num * ArcBallPresenter.tangentSize;
        frontGeometry = this.GetSemiEllipseGeometry(point1, top1, point2, center, horizontalScale, verticalScale);
        backGeometry = this.GetSemiEllipseGeometry(point2, top2, point1, center, horizontalScale, verticalScale);
      }
    }

    private PathGeometry GetSemiEllipseGeometry(Point left, Point top, Point right, Point center, double horizontalScale, double verticalScale)
    {
      Vector vector1 = right - center;
      if (!Tolerances.NearZero(vector1.Length))
      {
        vector1.Normalize();
        vector1 *= horizontalScale;
      }
      Vector vector2 = top - center;
      if (!Tolerances.NearZero(vector2.Length))
      {
        vector2.Normalize();
        vector2 *= verticalScale;
      }
      PathFigure pathFigure = new PathFigure();
      pathFigure.StartPoint = left;
      pathFigure.Segments.Add((PathSegment) new BezierSegment(left + vector2, top - vector1, top, true));
      pathFigure.Segments.Add((PathSegment) new BezierSegment(top + vector1, right + vector2, right, true));
      pathFigure.Freeze();
      PathGeometry pathGeometry = new PathGeometry((IEnumerable<PathFigure>) new PathFigure[1]
      {
        pathFigure
      });
      pathGeometry.Freeze();
      return pathGeometry;
    }

    private PathGeometry GetEllipseGeometry(Point left, Point top, Point right, Point bottom, double horizontalScale, double verticalScale)
    {
      Vector vector1 = right - left;
      if (!Tolerances.NearZero(vector1.Length))
      {
        vector1.Normalize();
        vector1 *= horizontalScale;
      }
      Vector vector2 = top - bottom;
      if (!Tolerances.NearZero(vector2.Length))
      {
        vector2.Normalize();
        vector2 *= verticalScale;
      }
      PathFigure pathFigure = new PathFigure();
      pathFigure.StartPoint = left;
      pathFigure.Segments.Add((PathSegment) new BezierSegment(left + vector2, top - vector1, top, true));
      pathFigure.Segments.Add((PathSegment) new BezierSegment(top + vector1, right + vector2, right, true));
      pathFigure.Segments.Add((PathSegment) new BezierSegment(right - vector2, bottom + vector1, bottom, true));
      pathFigure.Segments.Add((PathSegment) new BezierSegment(bottom - vector1, left - vector2, left, true));
      pathFigure.IsClosed = true;
      pathFigure.Freeze();
      PathGeometry pathGeometry = new PathGeometry((IEnumerable<PathFigure>) new PathFigure[1]
      {
        pathFigure
      });
      pathGeometry.Freeze();
      return pathGeometry;
    }
  }
}
