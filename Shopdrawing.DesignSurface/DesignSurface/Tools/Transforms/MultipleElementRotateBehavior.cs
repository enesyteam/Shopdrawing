// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.MultipleElementRotateBehavior
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Commands.Undo;
using Microsoft.Expression.Framework.Diagnostics;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  internal sealed class MultipleElementRotateBehavior : RotateBehavior
  {
    private Dictionary<SceneElement, CanonicalTransform> initialTransforms = new Dictionary<SceneElement, CanonicalTransform>();
    private Dictionary<SceneElement, Matrix> elementsToElement = new Dictionary<SceneElement, Matrix>();
    private Rect initialBounds;
    private Point initialElementSetCenter;
    private Point intialRenderTransformOrigin;
    private Point initialArtboardOrigin;
    private CanonicalDecomposition initialSharedTransform;

    public MultipleElementRotateBehavior(ToolBehaviorContext toolContext)
      : base(toolContext)
    {
    }

    protected override void Initialize()
    {
      PerformanceUtility.MeasurePerformanceUntilRender(PerformanceEvent.MultipleElementRotate);
      this.FireNotifications(false);
      CanonicalDecomposition canonicalDecomposition = this.EditingElementSet.CalculateSharedTransform();
      this.UnsnappedAngle = canonicalDecomposition.RotationAngle;
      this.initialBounds = this.EditingElementSet.Bounds;
      this.intialRenderTransformOrigin = this.EditingElementSet.RenderTransformOrigin;
      this.initialSharedTransform = canonicalDecomposition;
      this.initialElementSetCenter = this.EditingElementSet.RenderTransformOriginInElementCoordinates;
      foreach (SceneElement element in this.EditingElementSet.Elements)
      {
        this.initialTransforms[element] = new CanonicalTransform((Transform) element.GetComputedValueAsWpf(element.Platform.Metadata.CommonProperties.RenderTransform));
        this.elementsToElement[element] = ElementUtilities.GetInverseMatrix(this.EditingElementSet.GetTransformToElementSpaceFromMemberElement(element));
      }
      this.initialArtboardOrigin = this.EditingElement.DesignerContext.ActiveView.Artboard.ArtboardBounds.TopLeft;
    }

    private void FireNotifications(bool shouldDraw)
    {
      this.ActiveView.AdornerLayer.FirePropertyChanged("RotateBoundingBoxAdornerIsVisible", (object) (bool) (!shouldDraw ? true : false));
      this.ActiveView.AdornerLayer.FirePropertyChanged("SizeAdornerVisibility", (object) (bool) (shouldDraw ? true : false));
      this.ActiveView.AdornerLayer.FirePropertyChanged("BoundingBoxAdornerIsVisible", (object) (bool) (shouldDraw ? true : false));
      this.ActiveView.AdornerLayer.FirePropertyChanged("RoundedRectangleAdornerIsVisible", (object) (bool) (shouldDraw ? true : false));
      this.ActiveView.AdornerLayer.FirePropertyChanged("SingleElementCenterPointAdornerIsVisible", (object) (bool) (shouldDraw ? true : false));
    }

    protected override void Finish()
    {
      this.EditingElement.ViewModel.Document.AddUndoUnit((IUndoUnit) new MultipleElementCenterEditUndoUnit(this.EditingElementSet, this.intialRenderTransformOrigin, this.EditingElementSet.RenderTransformOrigin));
      this.FireNotifications(true);
    }

    protected override void ApplyRotation(double angle)
    {
      Point point1 = this.initialElementSetCenter;
      angle -= this.initialSharedTransform.RotationAngle;
      this.ActiveView.AdornerLayer.FirePropertyChanged("RotateBoundingBoxAdornerRotationAngle", (object) angle);
      foreach (SceneElement element in this.EditingElementSet.Elements)
      {
        if (!(element.GetComputedValueAsWpf(Base2DElement.RenderTransformProperty) is Transform))
        {
          Transform identity = Transform.Identity;
        }
        Matrix matrix = this.elementsToElement[element];
        Point elementCoordinates = ((Base2DElement) element).RenderTransformOriginInElementCoordinates;
        Point point2 = point1 * matrix;
        point2 = new Point(point2.X - elementCoordinates.X, point2.Y - elementCoordinates.Y);
        Point fixedPoint = RoundingHelper.RoundPosition(point2);
        CanonicalTransform newTransform = (CanonicalTransform) this.initialTransforms[element].Clone();
        newTransform.ApplyRotation(angle, fixedPoint);
        AdornedToolBehavior.UpdateElementTransform(element, newTransform, AdornedToolBehavior.TransformPropertyFlags.Translation | AdornedToolBehavior.TransformPropertyFlags.RotatationAngle);
      }
      if (this.EditingElementSet.HasHomogeneousRotation)
        return;
      this.EditingElementSet.Update();
      this.UpdateEditTransaction();
      this.EditingElement.ViewModel.DefaultView.UpdateLayout();
      Vector vector1 = this.initialArtboardOrigin - this.EditingElement.DesignerContext.ActiveView.Artboard.ArtboardBounds.TopLeft;
      Rect bounds = this.EditingElementSet.Bounds;
      Rect rect = this.initialBounds;
      Point point3 = new Point(0.0, 0.0);
      Vector vector2 = this.initialBounds.TopLeft + new Point(this.intialRenderTransformOrigin.X * rect.Width, this.intialRenderTransformOrigin.Y * rect.Height) - point3 - bounds.TopLeft + vector1;
      vector2.X /= bounds.Width;
      vector2.Y /= bounds.Height;
      this.EditingElementSet.RenderTransformOrigin = RoundingHelper.RoundPosition(point3 + vector2);
    }
  }
}
