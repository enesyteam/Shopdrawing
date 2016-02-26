// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.FeedbackHelper
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  internal static class FeedbackHelper
  {
    private static readonly Brush DefaultAdornerBrush = (Brush) Brushes.CornflowerBlue;
    private const double dashRepeatLength = 8.0;
    private static readonly Pen DefaultThinPen;
    private static readonly Pen DefaultMediumPen;
    private static readonly Pen DefaultThickPen;
    private static readonly Pen InactiveThinPen;
    private static readonly Pen MultipleElementThinPen;
    private static readonly Pen MultipleElementMediumPen;
    private static readonly Pen ClipPathThinPen;
    private static readonly Pen ClipPathThickPen;
    private static readonly Pen MotionPathThinPen;
    private static readonly Pen MotionPathSegmentThinPen;
    private static readonly Pen MotionPathThickPen;
    private static readonly Pen MotionPathSegmentThickPen;
    private static readonly Brush InactiveBrush;
    private static readonly Brush ClipPathBrush;
    private static readonly Brush WhiteBrush;
    private static readonly PenZoomCache DefaultThinPenZoomCache;
    private static readonly TileBrush horizontalDashBrush;
    private static readonly TileBrush verticalDashBrush;

    static FeedbackHelper()
    {
      FeedbackHelper.DefaultAdornerBrush.Freeze();
      FeedbackHelper.DefaultThinPen = new Pen((Brush) Brushes.CornflowerBlue, 1.0);
      FeedbackHelper.DefaultThinPen.Freeze();
      FeedbackHelper.DefaultMediumPen = new Pen((Brush) Brushes.CornflowerBlue, 2.0);
      FeedbackHelper.DefaultMediumPen.Freeze();
      FeedbackHelper.DefaultThickPen = new Pen((Brush) Brushes.CornflowerBlue, 3.0);
      FeedbackHelper.DefaultThickPen.Freeze();
      FeedbackHelper.InactiveBrush = (Brush) new SolidColorBrush(Color.FromArgb(byte.MaxValue, (byte) 150, (byte) 150, (byte) 150));
      FeedbackHelper.InactiveBrush.Freeze();
      FeedbackHelper.InactiveThinPen = new Pen(FeedbackHelper.InactiveBrush, 1.0);
      FeedbackHelper.InactiveThinPen.Freeze();
      FeedbackHelper.MultipleElementThinPen = new Pen((Brush) Brushes.CornflowerBlue, 1.0);
      FeedbackHelper.MultipleElementThinPen.Freeze();
      FeedbackHelper.MultipleElementMediumPen = new Pen((Brush) Brushes.CornflowerBlue, 2.0);
      FeedbackHelper.MultipleElementMediumPen.Freeze();
      FeedbackHelper.ClipPathBrush = (Brush) new SolidColorBrush(Color.FromArgb(byte.MaxValue, (byte) 173, (byte) 42, (byte) 140));
      FeedbackHelper.ClipPathBrush.Freeze();
      FeedbackHelper.ClipPathThinPen = new Pen(FeedbackHelper.ClipPathBrush, 1.0);
      FeedbackHelper.ClipPathThinPen.Freeze();
      FeedbackHelper.ClipPathThickPen = new Pen(FeedbackHelper.ClipPathBrush, 3.0);
      FeedbackHelper.ClipPathThickPen.Freeze();
      FeedbackHelper.MotionPathThinPen = new Pen((Brush) Brushes.CornflowerBlue, 1.0);
      FeedbackHelper.MotionPathThinPen.Freeze();
      FeedbackHelper.WhiteBrush = (Brush) new SolidColorBrush(Color.FromArgb(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue));
      FeedbackHelper.WhiteBrush.Freeze();
      FeedbackHelper.MotionPathSegmentThinPen = new Pen((Brush) Brushes.CornflowerBlue, 1.0);
      FeedbackHelper.MotionPathSegmentThinPen.DashStyle = new DashStyle((IEnumerable<double>) new double[2]
      {
        6.0,
        6.0
      }, 1.0);
      FeedbackHelper.MotionPathSegmentThinPen.Freeze();
      FeedbackHelper.MotionPathThickPen = new Pen((Brush) Brushes.CornflowerBlue, 3.0);
      FeedbackHelper.MotionPathThickPen.Freeze();
      FeedbackHelper.MotionPathSegmentThickPen = new Pen((Brush) Brushes.CornflowerBlue, 3.0);
      FeedbackHelper.MotionPathSegmentThickPen.DashStyle = new DashStyle((IEnumerable<double>) new double[2]
      {
        2.0,
        2.0
      }, 1.0);
      FeedbackHelper.MotionPathSegmentThickPen.Freeze();
      FeedbackHelper.DefaultThinPenZoomCache = new PenZoomCache(FeedbackHelper.GetThinPen(AdornerType.Default));
      DrawingGroup drawingGroup = new DrawingGroup();
      DrawingContext drawingContext = drawingGroup.Open();
      drawingContext.DrawRectangle((Brush) Brushes.White, (Pen) null, new Rect(-1.0, -1.0, 3.0, 3.0));
      drawingContext.DrawRectangle((Brush) Brushes.Black, (Pen) null, new Rect(0.25, -1.0, 0.5, 3.0));
      drawingContext.Close();
      drawingGroup.Freeze();
      DrawingBrush drawingBrush = new DrawingBrush((Drawing) drawingGroup);
      drawingBrush.ViewportUnits = BrushMappingMode.Absolute;
      drawingBrush.Viewport = new Rect(0.0, 0.0, 8.0, 8.0);
      drawingBrush.ViewboxUnits = BrushMappingMode.Absolute;
      drawingBrush.Viewbox = new Rect(0.0, 0.0, 1.0, 1.0);
      drawingBrush.Stretch = Stretch.Uniform;
      drawingBrush.TileMode = TileMode.Tile;
      FeedbackHelper.horizontalDashBrush = (TileBrush) drawingBrush;
      FeedbackHelper.verticalDashBrush = (TileBrush) drawingBrush.Clone();
      FeedbackHelper.verticalDashBrush.Transform = (Transform) new RotateTransform(90.0);
    }

    public static Pen GetThinPen(AdornerType adornerType)
    {
      switch (adornerType)
      {
        case AdornerType.Default:
          return FeedbackHelper.DefaultThinPen;
        case AdornerType.Inactive:
          return FeedbackHelper.InactiveThinPen;
        case AdornerType.MultipleElement:
          return FeedbackHelper.MultipleElementThinPen;
        case AdornerType.ClipPath:
        case AdornerType.ClipPathSegment:
          return FeedbackHelper.ClipPathThinPen;
        case AdornerType.MotionPath:
          return FeedbackHelper.MotionPathThinPen;
        case AdornerType.MotionPathSegment:
          return FeedbackHelper.MotionPathSegmentThinPen;
        default:
          return FeedbackHelper.DefaultThinPen;
      }
    }

    public static Pen GetMediumPen(AdornerType adornerType)
    {
      switch (adornerType)
      {
        case AdornerType.Default:
          return FeedbackHelper.DefaultMediumPen;
        case AdornerType.MultipleElement:
          return FeedbackHelper.MultipleElementMediumPen;
        default:
          return FeedbackHelper.DefaultMediumPen;
      }
    }

    public static Pen GetThickPen(AdornerType adornerType)
    {
      switch (adornerType)
      {
        case AdornerType.ClipPath:
        case AdornerType.ClipPathSegment:
          return FeedbackHelper.ClipPathThickPen;
        case AdornerType.MotionPath:
          return FeedbackHelper.MotionPathThickPen;
        case AdornerType.MotionPathSegment:
          return FeedbackHelper.MotionPathSegmentThickPen;
        default:
          return FeedbackHelper.DefaultThickPen;
      }
    }

    public static Pen GetThickPen()
    {
      return FeedbackHelper.DefaultThickPen;
    }

    public static Pen GetThinPen(double zoom)
    {
      return FeedbackHelper.DefaultThinPenZoomCache.GetPen(zoom);
    }

    public static Brush GetInactiveBrush()
    {
      return FeedbackHelper.WhiteBrush;
    }

    public static Brush GetActiveBrush()
    {
      return FeedbackHelper.GetActiveBrush(AdornerType.Default);
    }

    public static Brush GetActiveBrush(AdornerType adornerType)
    {
      if (adornerType == AdornerType.ClipPath)
        return FeedbackHelper.ClipPathBrush;
      return FeedbackHelper.DefaultAdornerBrush;
    }

    public static void DrawDashedRectangle(DrawingContext drawingContext, double zoom, Point firstPoint, Point secondPoint)
    {
      if (drawingContext == null)
        throw new ArgumentNullException("drawingContext");
      if (zoom <= 0.0)
        throw new ArgumentOutOfRangeException("zoom");
      Point point1 = new Point(Math.Min(firstPoint.X, secondPoint.X), Math.Min(firstPoint.Y, secondPoint.Y));
      Point point2 = new Point(Math.Max(firstPoint.X, secondPoint.X), Math.Max(firstPoint.Y, secondPoint.Y));
      Point point3 = new Point(point2.X, point1.Y);
      Point point4 = new Point(point1.X, point2.Y);
      double num = 1.5 / zoom;
      Vector vector1 = new Vector(num, num);
      Vector vector2 = vector1 / 2.0;
      FeedbackHelper.horizontalDashBrush.Viewport = new Rect(0.0, 0.0, 8.0 / zoom, 8.0 / zoom);
      drawingContext.DrawRectangle((Brush) FeedbackHelper.horizontalDashBrush, (Pen) null, new Rect(point1 - vector2, point3 + vector2));
      drawingContext.DrawRectangle((Brush) FeedbackHelper.horizontalDashBrush, (Pen) null, new Rect(point4 - vector2, point2 + vector2));
      FeedbackHelper.verticalDashBrush.Viewport = FeedbackHelper.horizontalDashBrush.Viewport;
      drawingContext.DrawRectangle((Brush) FeedbackHelper.verticalDashBrush, (Pen) null, new Rect(point1 - vector2, point4 + vector2));
      drawingContext.DrawRectangle((Brush) FeedbackHelper.verticalDashBrush, (Pen) null, new Rect(point3 - vector2, point2 + vector2));
      drawingContext.DrawRectangle((Brush) Brushes.Black, (Pen) null, new Rect(point1 - vector2, vector1));
      drawingContext.DrawRectangle((Brush) Brushes.Black, (Pen) null, new Rect(point3 - vector2, vector1));
      drawingContext.DrawRectangle((Brush) Brushes.Black, (Pen) null, new Rect(point4 - vector2, vector1));
      drawingContext.DrawRectangle((Brush) Brushes.Black, (Pen) null, new Rect(point2 - vector2, vector1));
    }
  }
}
