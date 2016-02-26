// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.MakeClippingPathCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Geometry;
using Microsoft.Expression.DesignSurface.Geometry.Core;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Selection;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.UserInterface;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal sealed class MakeClippingPathCommand : SceneCommandBase
  {
    public override bool IsEnabled
    {
      get
      {
        if (!base.IsEnabled)
          return false;
        SceneElementSelectionSet elementSelectionSet = this.SceneViewModel.ElementSelectionSet;
        if (elementSelectionSet.Count < 1 || elementSelectionSet.Count > 2)
          return false;
        SceneElement clipper;
        SceneElement elementToBeClipped;
        this.GetClipperAndElementToBeClipped(this.SceneViewModel.ElementSelectionSet, out clipper, out elementToBeClipped);
        bool flag = clipper != null && (elementSelectionSet.Count == 1 || elementSelectionSet.Count == 2 && elementToBeClipped != null);
        if (flag && elementToBeClipped != null && clipper.IsAncestorOf((SceneNode) elementToBeClipped))
          flag = false;
        return flag;
      }
    }

    public MakeClippingPathCommand(SceneViewModel viewModel)
      : base(viewModel)
    {
    }

    public override void Execute()
    {
      using (this.SceneViewModel.DisableUpdateChildrenOnAddAndRemove())
      {
        SceneElement clipper;
        SceneElement elementToBeClipped;
        this.GetClipperAndElementToBeClipped(this.SceneViewModel.ElementSelectionSet, out clipper, out elementToBeClipped);
        if (elementToBeClipped == null)
        {
          SceneElement root = this.SceneViewModel.ActiveEditingContainer as SceneElement;
          if (root != null)
            elementToBeClipped = (SceneElement) PathTargetDialog.ChooseClippingPathTarget(root, clipper);
        }
        if (elementToBeClipped == null)
          return;
        using (SceneEditTransaction editTransaction = this.SceneViewModel.CreateEditTransaction(StringTable.UndoUnitMakeClippingPath, false))
        {
          using (this.SceneViewModel.ForceBaseValue())
          {
            this.SceneViewModel.ElementSelectionSet.Clear();
            Transform geometryTransform = this.GetGeometryTransform(clipper, elementToBeClipped);
            System.Windows.Media.Geometry targetGeometry = this.GetTransformedClippingGeometry(clipper, geometryTransform);
            if (!this.DesignerContext.ActiveDocument.ProjectContext.IsCapabilitySet(PlatformCapability.PrefersRectangularClippingPath))
              targetGeometry = this.ApplyCurrentClippingToGeometry(targetGeometry, elementToBeClipped);
            if (!this.DesignerContext.ActiveDocument.ProjectContext.IsCapabilitySet(PlatformCapability.IsWpf))
            {
              PathGeometry original = targetGeometry as PathGeometry;
              if (original != null)
              {
                PathGeometry geometry = PathGeometryUtilities.RemoveMapping(original, true);
                PathGeometryUtilities.EnsureOnlySingleSegmentsInGeometry(geometry);
                targetGeometry = (System.Windows.Media.Geometry)geometry;
              }
            }
            elementToBeClipped.SetValueAsWpf(Base2DElement.ClipProperty, (object) targetGeometry);
            ReferenceStep singleStep1 = (ReferenceStep) this.SceneViewModel.ProjectContext.ResolveProperty(PathElement.DataProperty);
            ReferenceStep singleStep2 = (ReferenceStep) this.SceneViewModel.ProjectContext.ResolveProperty(Base2DElement.ClipProperty);
            PathCommandHelper.MoveVertexAnimations(clipper, new PropertyReference(singleStep1), elementToBeClipped, new PropertyReference(singleStep2), geometryTransform);
            this.SceneViewModel.DeleteElementTree(clipper);
            this.SceneViewModel.ElementSelectionSet.SetSelection(elementToBeClipped);
          }
          editTransaction.Commit();
        }
      }
    }

    private Transform GetGeometryTransform(SceneElement clipper, SceneElement elementToBeClipped)
    {
      Transform identity = Transform.Identity;
      return elementToBeClipped == null ? clipper.GetComputedValueAsWpf(Base2DElement.RenderTransformProperty) as Transform : (Transform) new MatrixTransform(clipper.GetComputedTransformToElement(elementToBeClipped));
    }

    private System.Windows.Media.Geometry ApplyCurrentClippingToGeometry(System.Windows.Media.Geometry targetGeometry, SceneElement clippedElement)
    {
        System.Windows.Media.Geometry geometry1 = (System.Windows.Media.Geometry)(clippedElement.GetComputedValueAsWpf(Base2DElement.ClipProperty) as PathGeometry);
      if (geometry1 != null && !geometry1.IsEmpty())
          targetGeometry = (System.Windows.Media.Geometry)System.Windows.Media.Geometry.Combine(geometry1, targetGeometry, GeometryCombineMode.Intersect, (Transform)null);
      return targetGeometry;
    }

    private System.Windows.Media.Geometry GetTransformedClippingGeometry(SceneElement clipper, Transform transform)
    {
      RectangleElement rectangleElement = clipper as RectangleElement;
      if (rectangleElement == null || !this.DesignerContext.ActiveDocument.ProjectContext.IsCapabilitySet(PlatformCapability.PrefersRectangularClippingPath))
      {
          PathGeometry pathGeometry = PathGeometryUtilities.TransformGeometry(this.ApplyCurrentClippingToGeometry((System.Windows.Media.Geometry)PathConversionHelper.ConvertToPathGeometry(clipper), clipper), transform);
        if (pathGeometry.Bounds == Rect.Empty)
          pathGeometry = new PathGeometry();
        return (System.Windows.Media.Geometry)pathGeometry;
      }
      RectangleGeometry rectangleGeometry = rectangleElement.ViewModel.DefaultView.GetRenderedGeometryAsWpf((SceneElement) rectangleElement) as RectangleGeometry;
      if (rectangleGeometry == null)
          return (System.Windows.Media.Geometry)new RectangleGeometry();
      CanonicalDecomposition canonicalDecomposition = new CanonicalDecomposition(transform.Value);
      Matrix matrix = new Matrix();
      matrix.Scale(canonicalDecomposition.ScaleX, canonicalDecomposition.ScaleY);
      matrix.Translate(canonicalDecomposition.TranslationX, canonicalDecomposition.TranslationY);
      Rect rect = rectangleGeometry.Rect;
      rect.Transform(matrix);
      rectangleGeometry.Rect = RoundingHelper.RoundRect(rect);
      CanonicalTransform canonicalTransform = new CanonicalTransform();
      bool flag = false;
      canonicalTransform.CenterX = canonicalDecomposition.TranslationX;
      canonicalTransform.CenterY = canonicalDecomposition.TranslationY;
      if (canonicalDecomposition.RotationAngle != 0.0)
      {
        canonicalTransform.RotationAngle = canonicalDecomposition.RotationAngle;
        flag = true;
      }
      if (canonicalDecomposition.SkewX != 0.0 || canonicalDecomposition.SkewY != 0.0)
      {
        canonicalTransform.SkewX = RoundingHelper.RoundLength(canonicalDecomposition.SkewX);
        canonicalTransform.SkewY = RoundingHelper.RoundLength(canonicalDecomposition.SkewY);
        flag = true;
      }
      if (flag)
        rectangleGeometry.Transform = (Transform) canonicalTransform.TransformGroup;
      if ((double) rectangleElement.GetComputedValue(RectangleElement.RadiusXProperty) != 0.0 || (double) rectangleElement.GetComputedValue(RectangleElement.RadiusYProperty) != 0.0)
      {
        rectangleGeometry.ClearValue(RectangleGeometry.RadiusXProperty);
        rectangleGeometry.ClearValue(RectangleGeometry.RadiusYProperty);
        this.SceneView.ShowBubble(StringTable.ClippingRectanglePropertiesLostWarning, MessageBubbleType.Warning);
      }
      return (System.Windows.Media.Geometry)rectangleGeometry;
    }

    private void GetClipperAndElementToBeClipped(SceneElementSelectionSet selectionSet, out SceneElement clipper, out SceneElement elementToBeClipped)
    {
      clipper = (SceneElement) null;
      elementToBeClipped = (SceneElement) null;
      SceneElement primarySelection = selectionSet.PrimarySelection;
      if (selectionSet.Count == 1)
      {
        if (!this.CanBeClipper(primarySelection))
          return;
        clipper = primarySelection;
      }
      else
      {
        SceneElement element = selectionSet.Selection[0] == primarySelection ? selectionSet.Selection[1] : selectionSet.Selection[0];
        if (this.CanBeClipper(primarySelection) && this.CanBeClipped(element))
        {
          clipper = primarySelection;
          elementToBeClipped = element;
        }
        else
        {
          if (!this.CanBeClipper(element) || !this.CanBeClipped(primarySelection))
            return;
          clipper = element;
          elementToBeClipped = primarySelection;
        }
      }
    }

    private bool CanBeClipper(SceneElement element)
    {
      if (this.DesignerContext.ActiveDocument.ProjectContext.IsCapabilitySet(PlatformCapability.PrefersRectangularClippingPath))
        return element is RectangleElement;
      return PathConversionHelper.CanConvert(element);
    }

    private bool CanBeClipped(SceneElement element)
    {
      return PlatformTypes.UIElement.IsAssignableFrom((ITypeId) element.Type);
    }
  }
}
