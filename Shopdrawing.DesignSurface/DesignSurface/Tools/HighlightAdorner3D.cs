// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.HighlightAdorner3D
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Tools.Transforms;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.Tools
{
  internal class HighlightAdorner3D : Adorner
  {
    private static Pen DefaultPen = FeedbackHelper.GetThinPen(AdornerType.Default).Clone();

    static HighlightAdorner3D()
    {
      HighlightAdorner3D.DefaultPen.StartLineCap = PenLineCap.Round;
      HighlightAdorner3D.DefaultPen.EndLineCap = PenLineCap.Round;
      HighlightAdorner3D.DefaultPen.LineJoin = PenLineJoin.Round;
      HighlightAdorner3D.DefaultPen.Freeze();
    }

    public HighlightAdorner3D(AdornerSet adornerSet)
      : base(adornerSet)
    {
    }

    public override void Draw(DrawingContext drawingContext, Matrix matrix)
    {
      object platformSpecificObject = this.AdornerSet.Element.ViewTargetElement.PlatformSpecificObject;
      foreach (SceneElement sceneElement in this.Element.ViewModel.ElementSelectionSet.Selection)
      {
        Base3DElement element = sceneElement as Base3DElement;
        if (element != null && element.IsViewObjectValid)
        {
          Viewport3DElement viewport = element.Viewport;
          if (viewport != null && viewport.ViewObject.PlatformSpecificObject == platformSpecificObject)
            HighlightAdorner3D.DrawCube(drawingContext, matrix, element, HighlightAdorner3D.DefaultPen);
        }
      }
    }

    public static void DrawCube(DrawingContext drawingContext, Matrix matrix, Base3DElement element, Pen pen)
    {
      Viewport3DElement viewport1 = element.Viewport;
      if (viewport1 == null)
        return;
      Viewport3D viewport2 = viewport1.ViewObject.PlatformSpecificObject as Viewport3D;
      Rect3D localSpaceBounds = element.LocalSpaceBounds;
      Matrix3D viewport3DtoElement = element.GetComputedTransformFromViewport3DToElement();
      Matrix3D matrix3D = Helper3D.CameraRotationTranslationMatrix(viewport2.Camera);
      Matrix3D cameraToObject = viewport3DtoElement * matrix3D;
      KeyValuePair<Point, bool> keyValuePair1 = HighlightAdorner3D.Calculate2DPoint(viewport2, cameraToObject, viewport3DtoElement, matrix, localSpaceBounds.Location);
      KeyValuePair<Point, bool> keyValuePair2 = HighlightAdorner3D.Calculate2DPoint(viewport2, cameraToObject, viewport3DtoElement, matrix, localSpaceBounds.Location + new Vector3D(localSpaceBounds.SizeX, 0.0, 0.0));
      KeyValuePair<Point, bool> keyValuePair3 = HighlightAdorner3D.Calculate2DPoint(viewport2, cameraToObject, viewport3DtoElement, matrix, localSpaceBounds.Location + new Vector3D(localSpaceBounds.SizeX, localSpaceBounds.SizeY, 0.0));
      KeyValuePair<Point, bool> keyValuePair4 = HighlightAdorner3D.Calculate2DPoint(viewport2, cameraToObject, viewport3DtoElement, matrix, localSpaceBounds.Location + new Vector3D(0.0, localSpaceBounds.SizeY, 0.0));
      KeyValuePair<Point, bool> keyValuePair5 = HighlightAdorner3D.Calculate2DPoint(viewport2, cameraToObject, viewport3DtoElement, matrix, localSpaceBounds.Location + new Vector3D(0.0, 0.0, localSpaceBounds.SizeZ));
      KeyValuePair<Point, bool> keyValuePair6 = HighlightAdorner3D.Calculate2DPoint(viewport2, cameraToObject, viewport3DtoElement, matrix, localSpaceBounds.Location + new Vector3D(localSpaceBounds.SizeX, 0.0, localSpaceBounds.SizeZ));
      KeyValuePair<Point, bool> keyValuePair7 = HighlightAdorner3D.Calculate2DPoint(viewport2, cameraToObject, viewport3DtoElement, matrix, localSpaceBounds.Location + new Vector3D(localSpaceBounds.SizeX, localSpaceBounds.SizeY, localSpaceBounds.SizeZ));
      KeyValuePair<Point, bool> keyValuePair8 = HighlightAdorner3D.Calculate2DPoint(viewport2, cameraToObject, viewport3DtoElement, matrix, localSpaceBounds.Location + new Vector3D(0.0, localSpaceBounds.SizeY, localSpaceBounds.SizeZ));
      StreamGeometry streamGeometry = new StreamGeometry();
      StreamGeometryContext context = streamGeometry.Open();
      HighlightAdorner3D.DrawLine(context, keyValuePair1, keyValuePair2);
      HighlightAdorner3D.DrawLine(context, keyValuePair2, keyValuePair3);
      HighlightAdorner3D.DrawLine(context, keyValuePair3, keyValuePair4);
      HighlightAdorner3D.DrawLine(context, keyValuePair4, keyValuePair1);
      HighlightAdorner3D.DrawLine(context, keyValuePair5, keyValuePair6);
      HighlightAdorner3D.DrawLine(context, keyValuePair6, keyValuePair7);
      HighlightAdorner3D.DrawLine(context, keyValuePair7, keyValuePair8);
      HighlightAdorner3D.DrawLine(context, keyValuePair8, keyValuePair5);
      HighlightAdorner3D.DrawLine(context, keyValuePair6, keyValuePair2);
      HighlightAdorner3D.DrawLine(context, keyValuePair7, keyValuePair3);
      HighlightAdorner3D.DrawLine(context, keyValuePair5, keyValuePair1);
      HighlightAdorner3D.DrawLine(context, keyValuePair8, keyValuePair4);
      context.Close();
      streamGeometry.Freeze();
      drawingContext.DrawGeometry((Brush)null, pen, (System.Windows.Media.Geometry)streamGeometry);
    }

    private static KeyValuePair<Point, bool> Calculate2DPoint(Viewport3D viewport, Matrix3D cameraToObject, Matrix3D worldToObject, Matrix matrix, Point3D point3D)
    {
      double num = 0.0;
      ProjectionCamera projectionCamera = viewport.Camera as ProjectionCamera;
      if (projectionCamera != null)
        num = projectionCamera.NearPlaneDistance;
      if (cameraToObject.Transform(point3D).Z > -num - 1E-06)
        return new KeyValuePair<Point, bool>(new Point(), false);
      return new KeyValuePair<Point, bool>(matrix.Transform(AdornedToolBehavior3D.Point3DInViewport3D(viewport, worldToObject, point3D)), true);
    }

    private static void DrawLine(StreamGeometryContext context, KeyValuePair<Point, bool> start, KeyValuePair<Point, bool> end)
    {
      if (!start.Value || !end.Value)
        return;
      context.BeginFigure(start.Key, false, false);
      context.LineTo(end.Key, true, false);
    }
  }
}
