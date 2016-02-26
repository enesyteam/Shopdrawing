// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Path.RectangleGeometryAdornerSetBase
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.Tools.Transforms;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Path
{
  public abstract class RectangleGeometryAdornerSetBase : AdornerSet, ISubElementAdornerSet
  {
    public Rect TargetRect
    {
      get
      {
        RectangleGeometry rectangleGeometry = this.Element.GetLocalValueAsWpf(Base2DElement.ClipProperty) as RectangleGeometry;
        if (rectangleGeometry != null)
          return rectangleGeometry.Rect;
        return new Rect(0.0, 0.0, 0.0, 0.0);
      }
    }

    public Matrix TargetMatrix
    {
      get
      {
        return RectangleGeometryAdornerSetBase.GetRectangleClipGeometryTransform(this.Element);
      }
    }

    protected RectangleGeometryAdornerSetBase(ToolBehaviorContext toolContext, BaseFrameworkElement adornedElement, AdornerSetOrder order)
      : base(toolContext, (SceneElement) adornedElement, order)
    {
    }

    public override Matrix GetTransformMatrix(IViewObject targetViewObject)
    {
      Matrix transformMatrix = base.GetTransformMatrix(targetViewObject);
      transformMatrix.Prepend(RectangleGeometryAdornerSetBase.GetRectangleClipGeometryTransform(this.Element));
      return transformMatrix;
    }

    public override Matrix GetTransformMatrixToAdornerLayer()
    {
      Matrix matrixToAdornerLayer = base.GetTransformMatrixToAdornerLayer();
      matrixToAdornerLayer.Prepend(RectangleGeometryAdornerSetBase.GetRectangleClipGeometryTransform(this.Element));
      return matrixToAdornerLayer;
    }

    public static Matrix GetRectangleClipGeometryTransform(SceneElement element)
    {
      RectangleGeometry rectangleGeometry = element.GetLocalValueAsWpf(Base2DElement.ClipProperty) as RectangleGeometry;
      if (rectangleGeometry != null)
      {
        Transform transform = rectangleGeometry.Transform;
        if (transform != null && !transform.Value.IsIdentity)
          return transform.Value;
      }
      return Matrix.Identity;
    }
  }
}
