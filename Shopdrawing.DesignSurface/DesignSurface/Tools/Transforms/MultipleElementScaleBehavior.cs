// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.MultipleElementScaleBehavior
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  internal class MultipleElementScaleBehavior : ScaleBehavior
  {
    private Dictionary<SceneElement, Matrix> startTransformsDictionary = new Dictionary<SceneElement, Matrix>();
    private Dictionary<SceneElement, Matrix> elementToElementsTransformDictionary = new Dictionary<SceneElement, Matrix>();
    private Dictionary<SceneElement, Rect> startBoundsDictionary = new Dictionary<SceneElement, Rect>();
    private Dictionary<SceneElement, Point> startCentersDictionary = new Dictionary<SceneElement, Point>();
    private Matrix startSharedTransform;

    protected Dictionary<SceneElement, Matrix> StartTransformsDictionary
    {
      get
      {
        return this.startTransformsDictionary;
      }
    }

    protected Dictionary<SceneElement, Matrix> ElementToElementsTransformDictionary
    {
      get
      {
        return this.elementToElementsTransformDictionary;
      }
    }

    protected Dictionary<SceneElement, Rect> StartBoundsDictionary
    {
      get
      {
        return this.startBoundsDictionary;
      }
    }

    protected Dictionary<SceneElement, Point> StartCentersDictionary
    {
      get
      {
        return this.startCentersDictionary;
      }
    }

    protected Matrix StartSharedTransform
    {
      get
      {
        return this.startSharedTransform;
      }
    }

    public MultipleElementScaleBehavior(ToolBehaviorContext toolContext)
      : base(toolContext)
    {
    }

    protected override void Initialize()
    {
      this.StartBounds = this.ActiveAdorner.ElementSet.ElementBounds;
      this.startSharedTransform = this.EditingElementSet.CalculateSharedTransform().Value;
      this.StartCenter = new Point(this.StartBounds.X + this.StartBounds.Width * this.EditingElementSet.RenderTransformOrigin.X, this.StartBounds.Y + this.StartBounds.Height * this.EditingElementSet.RenderTransformOrigin.Y);
      foreach (SceneElement element in this.EditingElementSet.Elements)
      {
        GeneralTransform generalTransform = element.GetComputedValueAsWpf(Base2DElement.RenderTransformProperty) as GeneralTransform;
        Matrix identity = Matrix.Identity;
        if (generalTransform != null && generalTransform is Transform)
          identity = ((Transform) generalTransform).Value;
        this.startTransformsDictionary[element] = identity;
        this.elementToElementsTransformDictionary[element] = this.EditingElementSet.GetTransformToElementSpaceFromMemberElement(element);
        this.startBoundsDictionary[element] = ((Base2DElement) element).GetComputedTightBounds();
        this.startCentersDictionary[element] = ((Base2DElement) element).RenderTransformOrigin;
      }
    }

    protected override void ApplyScale(Vector scale, Point center)
    {
      Rect elementBounds = this.EditingElementSet.ElementBounds;
      Matrix m = this.startSharedTransform;
      Matrix inverseMatrix = ElementUtilities.GetInverseMatrix(m);
      Matrix matrix1 = new Matrix();
      matrix1.Translate(-center.X, -center.Y);
      foreach (SceneElement element in this.EditingElementSet.Elements)
      {
        Matrix matrix2 = this.elementToElementsTransformDictionary[element] * matrix1;
        Point point1 = this.startCentersDictionary[element];
        Matrix matrix3 = this.startTransformsDictionary[element];
        Rect rect = this.startBoundsDictionary[element];
        Point point2 = new Point(rect.X + rect.Width * point1.X, rect.Y + rect.Height * point1.Y);
        Point point3 = matrix2.Transform(point2);
        Vector vector = new Point(scale.X * point3.X, scale.Y * point3.Y) - point3;
        Matrix matrix4 = matrix3 * inverseMatrix;
        matrix4.ScaleAt(scale.X, scale.Y, matrix4.OffsetX, matrix4.OffsetY);
        matrix4.Translate(vector.X, vector.Y);
        CanonicalDecomposition newTransform = new CanonicalDecomposition(matrix4 * m);
        newTransform.ScaleX = RoundingHelper.RoundScale(newTransform.ScaleX);
        newTransform.ScaleY = RoundingHelper.RoundScale(newTransform.ScaleY);
        newTransform.SkewX = RoundingHelper.RoundAngle(newTransform.SkewX);
        newTransform.SkewY = RoundingHelper.RoundAngle(newTransform.SkewY);
        newTransform.RotationAngle = RoundingHelper.RoundAngle(newTransform.RotationAngle);
        newTransform.TranslationX = RoundingHelper.RoundLength(newTransform.TranslationX);
        newTransform.TranslationY = RoundingHelper.RoundLength(newTransform.TranslationY);
        AdornedToolBehavior.UpdateElementTransform(element, newTransform, AdornedToolBehavior.TransformPropertyFlags.All);
      }
    }
  }
}
