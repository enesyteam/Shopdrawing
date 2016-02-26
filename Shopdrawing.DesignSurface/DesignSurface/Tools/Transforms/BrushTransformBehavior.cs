// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.BrushTransformBehavior
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  internal abstract class BrushTransformBehavior : AdornedToolBehavior
  {
    protected BrushAdorner ActiveAdorner
    {
      get
      {
        return (BrushAdorner) base.ActiveAdorner;
      }
    }

    protected BrushTransformBehavior(ToolBehaviorContext toolContext)
      : base(toolContext)
    {
    }

    internal PropertyReference GetBrushPropertyReference(SceneNode sceneNode)
    {
      return BrushTool.GetBrushPropertyReference(sceneNode);
    }

    protected void CopyPrimaryBrushToSelection()
    {
      this.EnsureEditTransaction();
      SceneElement primarySelection = this.ActiveView.ViewModel.ElementSelectionSet.PrimarySelection;
      if (primarySelection == null)
        return;
      PropertyReference propertyReference1 = this.GetBrushPropertyReference((SceneNode) primarySelection);
      if (propertyReference1 == null)
        return;
      object computedValue = primarySelection.GetComputedValue(propertyReference1);
      foreach (SceneElement sceneElement in this.ActiveView.ViewModel.ElementSelectionSet.Selection)
      {
        if (sceneElement != primarySelection)
        {
          PropertyReference propertyReference2 = this.ActiveView.DesignerContext.PropertyManager.FilterProperty((SceneNode) sceneElement, propertyReference1);
          if (propertyReference2 != null)
            sceneElement.SetValue(propertyReference2, computedValue);
        }
      }
      this.UpdateEditTransaction();
    }

    protected object GetBrushValue(IPropertyId property)
    {
      SceneElement primarySelection = this.ActiveView.ElementSelectionSet.PrimarySelection;
      if (primarySelection == null)
        return (object) null;
      PropertyReference propertyReference1 = this.GetBrushPropertyReference((SceneNode) primarySelection);
      if (propertyReference1 == null)
        return (object) null;
      PropertyReference propertyReference2 = propertyReference1.Append(property);
      return primarySelection.GetComputedValueAsWpf(propertyReference2);
    }

    private void SetBrushTransformValue(PropertyReference propertyReference, double valueToSet)
    {
      SceneElement primarySelection = this.ActiveView.ElementSelectionSet.PrimarySelection;
      if (primarySelection == null)
        return;
      PropertyReference propertyReference1 = this.GetBrushPropertyReference((SceneNode) primarySelection);
      if (propertyReference1 == null)
        return;
      this.SetBrushValue(propertyReference1.Append(propertyReference), (object) valueToSet);
    }

    protected void SetBrushValue(IPropertyId property, object valueToSet)
    {
      SceneElement primarySelection = this.ActiveView.ElementSelectionSet.PrimarySelection;
      if (primarySelection == null)
        return;
      PropertyReference propertyReference = this.GetBrushPropertyReference((SceneNode) primarySelection);
      if (propertyReference == null)
        return;
      this.SetBrushValue(propertyReference.Append(property), valueToSet);
    }

    public void SetBrushValue(PropertyReference propertyReference, object valueToSet)
    {
      this.EnsureEditTransaction();
      foreach (SceneElement sceneElement in this.ActiveView.ElementSelectionSet.Selection)
      {
        PropertyReference propertyReference1 = this.GetBrushPropertyReference((SceneNode) sceneElement);
        if (propertyReference1 != null && sceneElement.GetComputedValue(propertyReference1) != null)
        {
          PropertyReference propertyReference2 = this.ActiveView.DesignerContext.PropertyManager.FilterProperty((SceneNode) sceneElement, propertyReference);
          if (propertyReference2 != null)
            sceneElement.SetValueAsWpf(propertyReference2, valueToSet);
        }
      }
    }

    protected void ClearBrushTransform()
    {
      SceneElement primarySelection = this.ActiveView.ElementSelectionSet.PrimarySelection;
      if (primarySelection == null)
        return;
      PropertyReference propertyReference1 = this.GetBrushPropertyReference((SceneNode) primarySelection);
      if (propertyReference1 == null)
        return;
      this.EnsureEditTransaction();
      PropertyReference propertyReference2 = propertyReference1.Append(BrushNode.RelativeTransformProperty);
      foreach (SceneNode sceneNode in this.ActiveView.ElementSelectionSet.Selection)
        sceneNode.ClearValue(propertyReference2);
    }

    protected void SetBrushTransform(CanonicalTransform transform)
    {
      this.EnsureEditTransaction();
      transform.CenterX = RoundingHelper.RoundLength(transform.CenterX);
      transform.CenterY = RoundingHelper.RoundLength(transform.CenterY);
      transform.ScaleX = RoundingHelper.RoundScale(transform.ScaleX);
      transform.ScaleY = RoundingHelper.RoundScale(transform.ScaleY);
      transform.SkewX = RoundingHelper.RoundAngle(transform.SkewX);
      transform.SkewY = RoundingHelper.RoundAngle(transform.SkewY);
      transform.RotationAngle = RoundingHelper.RoundAngle(transform.RotationAngle);
      transform.TranslationX = RoundingHelper.RoundLength(transform.TranslationX);
      transform.TranslationY = RoundingHelper.RoundLength(transform.TranslationY);
      IProjectContext projectContext = this.ActiveDocument.ProjectContext;
      CanonicalTransform canonicalTransform = new CanonicalTransform((Transform) this.GetBrushValue(BrushNode.RelativeTransformProperty));
      if (JoltHelper.CompareDoubles(projectContext, canonicalTransform.CenterX, transform.CenterX) != 0 || JoltHelper.CompareDoubles(projectContext, canonicalTransform.CenterY, transform.CenterY) != 0)
      {
        this.SetBrushValue(BrushNode.RelativeTransformProperty, transform.GetPlatformTransform(this.RootNode.Platform.GeometryHelper));
      }
      else
      {
        if (JoltHelper.CompareDoubles(projectContext, canonicalTransform.ScaleX, transform.ScaleX) != 0)
          this.SetBrushTransformValue(this.EditingElement.Platform.Metadata.CommonProperties.BrushScaleXReference, transform.ScaleX);
        if (JoltHelper.CompareDoubles(projectContext, canonicalTransform.ScaleY, transform.ScaleY) != 0)
          this.SetBrushTransformValue(this.EditingElement.Platform.Metadata.CommonProperties.BrushScaleYReference, transform.ScaleY);
        if (JoltHelper.CompareDoubles(projectContext, canonicalTransform.SkewX, transform.SkewX) != 0)
          this.SetBrushTransformValue(this.EditingElement.Platform.Metadata.CommonProperties.BrushSkewXReference, transform.SkewX);
        if (JoltHelper.CompareDoubles(projectContext, canonicalTransform.SkewY, transform.SkewY) != 0)
          this.SetBrushTransformValue(this.EditingElement.Platform.Metadata.CommonProperties.BrushSkewYReference, transform.SkewY);
        if (JoltHelper.CompareDoubles(projectContext, canonicalTransform.RotationAngle, transform.RotationAngle) != 0)
          this.SetBrushTransformValue(this.EditingElement.Platform.Metadata.CommonProperties.BrushRotationAngleReference, transform.RotationAngle);
        if (JoltHelper.CompareDoubles(projectContext, canonicalTransform.TranslationX, transform.TranslationX) != 0)
          this.SetBrushTransformValue(this.EditingElement.Platform.Metadata.CommonProperties.BrushTranslationXReference, transform.TranslationX);
        if (JoltHelper.CompareDoubles(projectContext, canonicalTransform.TranslationY, transform.TranslationY) != 0)
          this.SetBrushTransformValue(this.EditingElement.Platform.Metadata.CommonProperties.BrushTranslationYReference, transform.TranslationY);
      }
      this.UpdateEditTransaction();
    }

    public void TranslateBrushPosition(Vector elementDelta, SceneElement element)
    {
      PropertyReference propertyReference = this.GetBrushPropertyReference((SceneNode) element);
      if (propertyReference == null)
        return;
      object computedValue = element.GetComputedValue(propertyReference);
      if (computedValue == null || PlatformTypes.IsInstance(computedValue, PlatformTypes.SolidColorBrush, (ITypeResolver) element.ProjectContext))
        return;
      ReferenceStep referenceStep = (ReferenceStep) element.Platform.Metadata.ResolveProperty(BrushNode.RelativeTransformProperty);
      object obj = element.ViewModel.DefaultView.ConvertToWpfValue(referenceStep.GetCurrentValue(computedValue));
      CanonicalTransform canonicalTransform = !(obj is Transform) ? new CanonicalTransform(Matrix.Identity) : new CanonicalTransform((Transform) obj);
      elementDelta *= canonicalTransform.TransformGroup.Value;
      double valueToSet1 = RoundingHelper.RoundLength(canonicalTransform.TranslationX + elementDelta.X);
      double valueToSet2 = RoundingHelper.RoundLength(canonicalTransform.TranslationY + elementDelta.Y);
      this.SetBrushTransformValue(element.Platform.Metadata.CommonProperties.BrushTranslationXReference, valueToSet1);
      this.SetBrushTransformValue(element.Platform.Metadata.CommonProperties.BrushTranslationYReference, valueToSet2);
    }

    public static void OffsetDragPoint(ref Point positionInBrush, Point endPoint, double dragOffset, double smallestGradientLength)
    {
      Vector vector1 = positionInBrush - endPoint;
      double length = vector1.Length;
      if (length == 0.0)
      {
        positionInBrush = endPoint;
      }
      else
      {
        Vector vector2 = vector1 / length;
        double num = Math.Max(smallestGradientLength, length - dragOffset);
        positionInBrush = endPoint + vector2 * num;
      }
    }

    public void SetSelectedStopIndex(int index)
    {
      if (this.EditingElement.DesignerContext.GradientToolSelectionService.Index == index)
        return;
      this.EditingElement.DesignerContext.GradientToolSelectionService.Index = index;
      this.ActiveAdorner.AdornerSet.InvalidateRender();
      this.ActiveAdorner.AdornerSet.Update();
    }
  }
}
