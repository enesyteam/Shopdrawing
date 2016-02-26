// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.AdornerElementSet
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Geometry.Core;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using System;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  public class AdornerElementSet
  {
    private int primaryElementIndex = -1;
    private bool boundsCached;
    private Rect cachedBounds;
    private bool? primaryNonAffine;
    private SceneElementCollection elements;
    private Transform cachedTransform;
    private Point renderTransformOrigin;
    private CanonicalDecomposition sharedTransformToRootVisual;
    private bool hasHomogenousRotation;
    private bool hasHomogenousSkew;
    private bool needsUpdate;

    public SceneElement PrimaryElement
    {
      get
      {
        return this.elements[this.primaryElementIndex];
      }
      set
      {
        int num = this.elements.IndexOf(value);
        if (num < 0)
          return;
        this.primaryElementIndex = num;
      }
    }

    public SceneElementCollection Elements
    {
      get
      {
        return this.elements;
      }
    }

    public bool AdornsMultipleElements
    {
      get
      {
        return this.elements.Count > 1;
      }
    }

    private DesignerContext DesignerContext
    {
      get
      {
        return this.elements[this.primaryElementIndex].DesignerContext;
      }
    }

    public SceneViewModel ViewModel
    {
      get
      {
        return this.DesignerContext.ActiveSceneViewModel;
      }
    }

    public bool IsAttached
    {
      get
      {
        foreach (SceneNode sceneNode in this.elements)
        {
          if (!sceneNode.IsAttached)
            return false;
        }
        return true;
      }
    }

    public Point RenderTransformOrigin
    {
      get
      {
        if (this.AdornsMultipleElements)
          return this.renderTransformOrigin;
        BaseFrameworkElement frameworkElement = this.PrimaryElement as BaseFrameworkElement;
        if (frameworkElement != null)
          return frameworkElement.RenderTransformOrigin;
        return new Point(0.5, 0.5);
      }
      set
      {
        if (!this.AdornsMultipleElements)
          this.PrimaryElement.SetValue(Base2DElement.RenderTransformOriginProperty, (object) value);
        else
          this.renderTransformOrigin = value;
      }
    }

    public bool IsPrimaryTransformNonAffine
    {
      get
      {
        if (!this.primaryNonAffine.HasValue)
          this.primaryNonAffine = new bool?(Adorner.NonAffineTransformInParentStack(this.PrimaryElement));
        return this.primaryNonAffine.Value;
      }
    }

    public bool HasNonAffineTransform
    {
      get
      {
        if (this.elements.Count == 1)
          return this.IsPrimaryTransformNonAffine;
        foreach (SceneElement element in this.elements)
        {
          if (Adorner.NonAffineTransformInParentStack(element))
            return true;
        }
        return false;
      }
    }

    public Point RenderTransformOriginInElementCoordinates
    {
      get
      {
        this.EnsureBounds();
        Rect rect = this.cachedBounds;
        Matrix inverseMatrix = ElementUtilities.GetInverseMatrix(this.GetTransformMatrixToAdornerLayer());
        rect.Transform(this.SharedTransformToRootVisual.Value * inverseMatrix);
        return new Point(rect.Left + rect.Width * this.RenderTransformOrigin.X, rect.Top + rect.Height * this.RenderTransformOrigin.Y);
      }
    }

    public bool HasHomogeneousSkew
    {
      get
      {
        return this.hasHomogenousSkew;
      }
    }

    public bool HasHomogeneousRotation
    {
      get
      {
        return this.hasHomogenousRotation;
      }
    }

    public Rect Bounds
    {
      get
      {
        this.EnsureBounds();
        Rect rect = this.cachedBounds;
        if (rect.Width < 0.0 || rect.Height < 0.0)
          return new Rect(0.0, 0.0, 0.0, 0.0);
        return this.cachedBounds;
      }
    }

    public Rect ElementBounds
    {
      get
      {
        return new Rect(0.0, 0.0, this.Bounds.Width, this.Bounds.Height);
      }
    }

    public bool NeedsUpdate
    {
      get
      {
        return this.needsUpdate;
      }
    }

    private CanonicalDecomposition SharedTransformToRootVisual
    {
      get
      {
        if (this.sharedTransformToRootVisual == (CanonicalDecomposition) null)
          this.sharedTransformToRootVisual = this.CalculateSharedTransform(true, true);
        return this.sharedTransformToRootVisual;
      }
    }

    private Visual RootVisual
    {
      get
      {
        return (Visual) this.DesignerContext.ActiveView.AdornerLayer;
      }
    }

    public AdornerElementSet(SceneElement element)
    {
      this.elements = new SceneElementCollection();
      this.elements.Add(element);
      this.primaryElementIndex = 0;
      this.renderTransformOrigin = new Point(0.5, 0.5);
    }

    public AdornerElementSet(SceneElementCollection elements)
    {
      this.elements = elements;
      this.primaryElementIndex = 0;
      this.renderTransformOrigin = new Point(0.5, 0.5);
    }

    public Point GetComputedCenter()
    {
      Rect elementBounds = this.ElementBounds;
      return !(elementBounds != Rect.Empty) ? new Point(0.0, 0.0) : new Point((elementBounds.Left + elementBounds.Right) / 2.0, (elementBounds.Top + elementBounds.Bottom) / 2.0);
    }

    public Matrix GetTransformMatrix(IViewObject targetVisual)
    {
      return this.GetTransformMatrix(targetVisual, true);
    }

    private Matrix GetTransformMatrix(IViewObject targetVisual, bool includeParentTransforms)
    {
      Matrix matrix = Matrix.Identity;
      if (!this.AdornsMultipleElements)
      {
        IViewObject viewTargetElement = this.PrimaryElement.ViewTargetElement;
        if (viewTargetElement != null && this.ViewModel.DefaultView.IsInArtboard(this.PrimaryElement))
          matrix = VectorUtilities.GetMatrixFromTransform(this.ViewModel.DefaultView.ComputeTransformToVisual(viewTargetElement, targetVisual));
      }
      else
      {
        if (this.cachedTransform == null)
        {
          CanonicalDecomposition transformToRootVisual = this.SharedTransformToRootVisual;
          this.EnsureBounds();
          Matrix identity = Matrix.Identity;
          identity.Translate(this.cachedBounds.X, this.cachedBounds.Y);
          this.cachedTransform = (Transform) new MatrixTransform(identity * transformToRootVisual.Value);
        }
        Matrix computedTransformToRoot = this.ViewModel.DefaultView.GetComputedTransformToRoot(targetVisual);
        Transform transform = (Transform) this.ViewModel.DefaultView.Artboard.CalculateTransformFromContentToArtboard().Inverse;
        computedTransformToRoot.Append(transform.Value);
        matrix = this.cachedTransform.Value * computedTransformToRoot;
      }
      return matrix;
    }

    public Matrix GetTransformMatrixToAdornerLayer()
    {
      return this.GetTransformMatrixToAdornerLayer(true);
    }

    public Matrix CalculatePrimaryElementTransformMatrixToAdornerLayer()
    {
      Matrix matrix = Matrix.Identity;
      if (this.PrimaryElement.ViewTargetElement != null && this.ViewModel.DefaultView.IsInArtboard(this.PrimaryElement))
        matrix = this.ViewModel.DefaultView.GetComputedTransformToRootVerified(this.PrimaryElement.Visual);
      Transform transform = this.ViewModel.DefaultView.Artboard.CalculateTransformFromContentToArtboard();
      matrix.Append(transform.Value);
      return matrix;
    }

    private Matrix GetTransformMatrixToAdornerLayer(bool includeParentTransforms)
    {
      Matrix identity1 = Matrix.Identity;
      if (!this.AdornsMultipleElements)
      {
        identity1.Append(this.CalculatePrimaryElementTransformMatrixToAdornerLayer());
      }
      else
      {
        if (this.cachedTransform == null)
        {
          CanonicalDecomposition canonicalDecomposition = includeParentTransforms ? this.SharedTransformToRootVisual : this.CalculateSharedTransform();
          this.EnsureBounds();
          Matrix identity2 = Matrix.Identity;
          identity2.Translate(this.cachedBounds.X, this.cachedBounds.Y);
          this.cachedTransform = (Transform) new MatrixTransform(identity2 * canonicalDecomposition.Value);
        }
        identity1 = this.cachedTransform.Value;
      }
      return identity1;
    }

    public void Invalidate()
    {
      this.needsUpdate = true;
    }

    public void Update()
    {
      this.cachedTransform = (Transform) null;
      this.boundsCached = false;
      this.cachedBounds = Rect.Empty;
      this.sharedTransformToRootVisual = (CanonicalDecomposition) null;
      this.needsUpdate = false;
      this.primaryNonAffine = new bool?();
    }

    public Matrix GetTransformToElementSpaceFromMemberElement(SceneElement element)
    {
      return VectorUtilities.GetMatrixFromTransform(element.ViewModel.DefaultView.ComputeTransformToVisual(element.Visual, this.RootVisual)) * ElementUtilities.GetInverseMatrix(this.GetTransformMatrixToAdornerLayer(false));
    }

    public CanonicalDecomposition CalculateSharedTransform()
    {
      return this.CalculateSharedTransform(false, false);
    }

    private void EnsureBounds()
    {
      if (this.boundsCached)
        return;
      Rect empty = Rect.Empty;
      Matrix inverseMatrix = ElementUtilities.GetInverseMatrix(this.SharedTransformToRootVisual.Value);
      foreach (SceneElement sceneElement in this.elements)
      {
        IViewObject viewTargetElement = sceneElement.ViewTargetElement;
        if (viewTargetElement != null)
        {
          Matrix matrixFromTransform = VectorUtilities.GetMatrixFromTransform(this.ViewModel.DefaultView.ComputeTransformToVisual(viewTargetElement, this.RootVisual));
          Rect actualBounds = this.ViewModel.DefaultView.GetActualBounds(viewTargetElement);
          actualBounds.Transform(matrixFromTransform * inverseMatrix);
          empty.Union(actualBounds);
        }
      }
      this.cachedBounds = empty;
      this.boundsCached = true;
    }

    private CanonicalDecomposition CalculateSharedTransform(bool scaleByZoom, bool addParentToRoot)
    {
      bool flag = false;
      CanonicalDecomposition canonicalDecomposition1 = new CanonicalDecomposition();
      this.hasHomogenousRotation = true;
      this.hasHomogenousSkew = true;
      foreach (SceneElement sceneElement in this.elements)
      {
        if (sceneElement is BaseFrameworkElement)
        {
          if (!sceneElement.IsAttached)
            return new CanonicalDecomposition();
          Transform identity = Transform.Identity;
          Transform transform = sceneElement.GetComputedValueAsWpf(sceneElement.Platform.Metadata.CommonProperties.RenderTransform) as Transform ?? Transform.Identity;
          CanonicalDecomposition canonicalDecomposition2 = new CanonicalDecomposition();
          CanonicalDecomposition canonicalDecomposition3 = !CanonicalTransform.IsCanonical(transform) ? new CanonicalDecomposition(transform.Value) : new CanonicalTransform(transform).Decomposition;
          if (!double.IsNaN(canonicalDecomposition3.ScaleX) && !double.IsNaN(canonicalDecomposition3.ScaleY))
          {
            if (!flag)
            {
              canonicalDecomposition1 = canonicalDecomposition3;
              canonicalDecomposition1.ScaleX = (double) Math.Sign(canonicalDecomposition1.ScaleX);
              canonicalDecomposition1.ScaleY = (double) Math.Sign(canonicalDecomposition1.ScaleY);
              flag = true;
            }
            else
            {
              if (Math.Sign(canonicalDecomposition3.ScaleX) != Math.Sign(canonicalDecomposition1.ScaleX))
                canonicalDecomposition1.ScaleX = 1.0;
              if (Math.Sign(canonicalDecomposition3.ScaleY) != Math.Sign(canonicalDecomposition1.ScaleY))
                canonicalDecomposition1.ScaleY = 1.0;
              if (!Tolerances.AreClose(canonicalDecomposition3.RotationAngle, canonicalDecomposition1.RotationAngle) || !Tolerances.AreClose(canonicalDecomposition3.SkewX, canonicalDecomposition1.SkewX) || !Tolerances.AreClose(canonicalDecomposition3.SkewY, canonicalDecomposition1.SkewY))
              {
                canonicalDecomposition1.RotationAngle = 0.0;
                canonicalDecomposition1.SkewX = 0.0;
                canonicalDecomposition1.SkewY = 0.0;
                this.hasHomogenousSkew = false;
                this.hasHomogenousRotation = false;
              }
            }
          }
        }
      }
      double zoom = this.DesignerContext.ActiveView.Artboard.Zoom;
      if (scaleByZoom)
      {
        canonicalDecomposition1.ScaleX = zoom * canonicalDecomposition1.ScaleX;
        canonicalDecomposition1.ScaleY = zoom * canonicalDecomposition1.ScaleY;
      }
      canonicalDecomposition1.Translation = new Vector(0.0, 0.0);
      if (addParentToRoot && this.PrimaryElement.VisualElementAncestor != null)
      {
        Matrix computedTransformToRoot = this.ViewModel.DefaultView.GetComputedTransformToRoot(this.PrimaryElement.VisualElementAncestor.Visual);
        Transform transform = this.ViewModel.DefaultView.Artboard.CalculateTransformFromContentToArtboard();
        computedTransformToRoot.Append(transform.Value);
        computedTransformToRoot.OffsetX = 0.0;
        computedTransformToRoot.OffsetY = 0.0;
        CanonicalDecomposition canonicalDecomposition2 = new CanonicalDecomposition(computedTransformToRoot);
        canonicalDecomposition1.Scale = new Vector(canonicalDecomposition1.ScaleX * (canonicalDecomposition2.ScaleX / zoom), canonicalDecomposition1.ScaleY * (canonicalDecomposition2.ScaleY / zoom));
        canonicalDecomposition1.Skew += canonicalDecomposition2.Skew;
        canonicalDecomposition1.RotationAngle *= (double) (Math.Sign(canonicalDecomposition2.ScaleX) * Math.Sign(canonicalDecomposition2.ScaleY));
        canonicalDecomposition1.RotationAngle += canonicalDecomposition2.RotationAngle;
        canonicalDecomposition1.Translation += canonicalDecomposition2.Translation;
      }
      return canonicalDecomposition1;
    }
  }
}
