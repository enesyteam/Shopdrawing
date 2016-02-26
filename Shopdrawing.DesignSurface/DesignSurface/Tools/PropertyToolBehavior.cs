// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.PropertyToolBehavior
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.DesignSurface.ViewModel.Text;
using Microsoft.Expression.Framework;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools
{
  internal sealed class PropertyToolBehavior : ElementToolBehavior
  {
    internal static readonly string PaintBucketUndoString = StringTable.UndoUnitApplyPaintBucket;
    internal static readonly string EyedropperUndoString = StringTable.UndoUnitApplyEyedropper;
    private static readonly IPropertyId[] PropertyList = new IPropertyId[24]
    {
      ControlElement.ForegroundProperty,
      ControlElement.BackgroundProperty,
      ControlElement.BorderBrushProperty,
      ShapeElement.FillProperty,
      ShapeElement.StrokeProperty,
      Base2DElement.OpacityMaskProperty,
      Base2DElement.OpacityProperty,
      ShapeElement.StrokeThicknessProperty,
      ShapeElement.StrokeMiterLimitProperty,
      ShapeElement.StrokeStartLineCapProperty,
      ShapeElement.StrokeEndLineCapProperty,
      ShapeElement.StrokeLineJoinProperty,
      ShapeElement.StrokeDashCapProperty,
      ControlElement.FontFamilyProperty,
      ControlElement.FontSizeProperty,
      ControlElement.FontWeightProperty,
      ControlElement.FontStyleProperty,
      RichTextBoxRangeElement.TextDecorationsProperty,
      TextBoxElement.TextDecorationsProperty,
      BlockProperties.LineHeightProperty,
      ParagraphElement.TextIndentProperty,
      ParagraphElement.TextAlignmentProperty,
      RichTextBoxElement.TextAlignmentProperty,
      TextBoxElement.TextAlignmentProperty
    };
    private PropertyToolAction openUndoUnitAction;
    private readonly PropertyToolAction action;
    private string actionString;

    public override string ActionString
    {
      get
      {
        return this.actionString;
      }
    }

    protected override Cursor DefaultCursor
    {
      get
      {
        if (this.GetActionToApply() != PropertyToolAction.Eyedropper)
          return ToolCursors.PaintBucketCursor;
        return ToolCursors.EyedropperCursor;
      }
    }

    public PropertyToolBehavior(ToolBehaviorContext toolContext, PropertyToolAction action)
      : base(toolContext)
    {
      this.action = action;
    }

    protected override bool OnButtonDownOverNonAdorner(Point pointerPosition)
    {
      PropertyToolAction actionToApply = this.GetActionToApply();
      this.UpdateCursor();
      SceneElement elementAtPoint = this.ActiveView.GetElementAtPoint(pointerPosition, (HitTestModifier) null, new InvisibleObjectHitTestModifier(SceneView.SmartInvisiblePanelSelect), (ICollection<BaseFrameworkElement>) null);
      if (elementAtPoint == null || actionToApply == PropertyToolAction.PaintBucket && !elementAtPoint.IsSelectable || (elementAtPoint.ViewObject == null || !PlatformTypes.FrameworkElement.IsAssignableFrom((ITypeId) elementAtPoint.ViewObject.GetIType((ITypeResolver) this.ActiveSceneViewModel.ProjectContext))))
        return false;
      switch (actionToApply)
      {
        case PropertyToolAction.Eyedropper:
          this.actionString = PropertyToolBehavior.EyedropperUndoString;
          this.openUndoUnitAction = PropertyToolAction.Eyedropper;
          this.ApplyEyedropper(elementAtPoint);
          break;
        case PropertyToolAction.PaintBucket:
          this.actionString = PropertyToolBehavior.PaintBucketUndoString;
          this.openUndoUnitAction = PropertyToolAction.PaintBucket;
          this.ApplyPaintBucket(elementAtPoint);
          break;
        default:
          throw new ArgumentException(ExceptionStringTable.PropertyToolBehaviorUnrecognizedAction);
      }
      return true;
    }

    internal override bool ShouldMotionlessAutoScroll(Point mousePoint, Rect artboardBoundary)
    {
      return false;
    }

    protected override bool OnDrag(Point dragStartPosition, Point dragCurrentPosition, bool scrollNow)
    {
      PropertyToolAction actionToApply = this.GetActionToApply();
      this.UpdateCursor();
      SceneElement elementAtPoint = this.ActiveView.GetElementAtPoint(dragCurrentPosition, (HitTestModifier) null, new InvisibleObjectHitTestModifier(SceneView.SmartInvisiblePanelSelect), (ICollection<BaseFrameworkElement>) null);
      if (elementAtPoint == null || actionToApply == PropertyToolAction.PaintBucket && !elementAtPoint.IsSelectable || (elementAtPoint.ViewObject == null || !PlatformTypes.FrameworkElement.IsAssignableFrom((ITypeId) elementAtPoint.ViewObject.GetIType((ITypeResolver) this.ActiveSceneViewModel.ProjectContext))))
        return false;
      switch (actionToApply)
      {
        case PropertyToolAction.Eyedropper:
          if (this.openUndoUnitAction != PropertyToolAction.Eyedropper)
          {
            this.CommitEditTransaction();
            this.actionString = PropertyToolBehavior.EyedropperUndoString;
            this.EnsureEditTransaction();
            this.openUndoUnitAction = PropertyToolAction.Eyedropper;
          }
          this.ApplyEyedropper(elementAtPoint);
          break;
        case PropertyToolAction.PaintBucket:
          if (this.openUndoUnitAction != PropertyToolAction.PaintBucket)
          {
            this.CommitEditTransaction();
            this.actionString = PropertyToolBehavior.PaintBucketUndoString;
            this.EnsureEditTransaction();
            this.openUndoUnitAction = PropertyToolAction.PaintBucket;
          }
          this.ApplyPaintBucket(elementAtPoint);
          break;
        default:
          throw new ArgumentException(ExceptionStringTable.PropertyToolBehaviorUnrecognizedAction);
      }
      return true;
    }

    protected override bool OnDragEnd(Point dragStartPosition, Point dragEndPosition)
    {
      this.CommitEditTransaction();
      this.UpdateCursor();
      return true;
    }

    protected override bool OnClickEnd(Point pointerPosition, int clickCount)
    {
      this.CommitEditTransaction();
      return true;
    }

    protected override bool OnKey(KeyEventArgs args)
    {
      this.UpdateCursor();
      if (this.IsEditTransactionOpen)
        return false;
      return base.OnKey(args);
    }

    private PropertyToolAction GetActionToApply()
    {
      if (!this.IsAltDown)
        return this.action;
      return this.action == PropertyToolAction.Eyedropper ? PropertyToolAction.PaintBucket : PropertyToolAction.Eyedropper;
    }

    private void ApplyPaintBucket(SceneElement hitElement)
    {
      this.EnsureEditTransaction();
      PropertyManager propertyManager = (PropertyManager) this.ToolBehaviorContext.PropertyManager;
      IPlatform platform = this.ActiveDocument.ProjectContext.Platform;
      IPlatformMetadata platformMetadata = (IPlatformMetadata) platform.Metadata;
      foreach (IPropertyId propertyId in PropertyToolBehavior.PropertyList)
      {
        ReferenceStep singleStep = platformMetadata.ResolveProperty(propertyId) as ReferenceStep;
        if (singleStep != null && singleStep.PropertyType.PlatformMetadata == platform.Metadata)
        {
          PropertyReference propertyReference1 = new PropertyReference(singleStep);
          PropertyReference propertyReference2 = propertyManager.FilterProperty((SceneNode) hitElement, propertyReference1);
          if (propertyReference2 != null)
          {
            object obj = propertyManager.GetValue(propertyReference2);
            object computedValue = hitElement.GetComputedValue(propertyReference2);
            if (obj != MixedProperty.Mixed && !PropertyUtilities.Compare(computedValue, obj, hitElement.ViewModel.DefaultView))
              hitElement.SetValue(propertyReference2, obj);
          }
        }
      }
      this.UpdateEditTransaction();
    }

    private void ApplyEyedropper(SceneElement hitElement)
    {
      BaseTextElement textElement = hitElement as BaseTextElement;
      if (textElement != null)
        this.ApplyEyedropperTextElement(textElement);
      else
        this.ApplyEyedropperSceneElement(hitElement);
    }

    private void ApplyEyedropperTextElement(BaseTextElement textElement)
    {
      PropertyManager propertyManager = (PropertyManager) this.ToolBehaviorContext.PropertyManager;
      IPlatform platform = this.ActiveDocument.ProjectContext.Platform;
      IPlatformMetadata platformMetadata = (IPlatformMetadata) platform.Metadata;
      Artboard artboard = this.ActiveView.Artboard;
      Matrix matrix = artboard.CalculateTransformFromArtboardToContent().Value;
      Matrix transformFromRoot = this.ActiveView.GetComputedTransformFromRoot((SceneElement) textElement);
      Point position = this.MouseDevice.GetPosition((IInputElement) artboard);
      Point point1 = matrix.Transform(position);
      Point point2 = transformFromRoot.Transform(point1);
      this.EnsureEditTransaction();
      foreach (IPropertyId propertyId in PropertyToolBehavior.PropertyList)
      {
        ReferenceStep singleStep = platformMetadata.ResolveProperty(propertyId) as ReferenceStep;
        if (singleStep != null && singleStep.PropertyType.PlatformMetadata == platform.Metadata)
        {
          PropertyReference propertyReference1 = new PropertyReference(singleStep);
          PropertyReference propertyReference2 = propertyManager.FilterProperty((SceneNode) textElement, propertyReference1);
          if (propertyReference2 != null)
          {
            object textValueAtPoint = textElement.GetTextValueAtPoint(point2, true, propertyReference2);
            propertyManager.SetValue(propertyReference2, textValueAtPoint);
          }
        }
      }
      this.UpdateEditTransaction();
    }

    private void ApplyEyedropperSceneElement(SceneElement hitElement)
    {
      PropertyManager propertyManager = (PropertyManager) this.ToolBehaviorContext.PropertyManager;
      IPlatform platform = this.ActiveDocument.ProjectContext.Platform;
      IPlatformMetadata platformMetadata = (IPlatformMetadata) platform.Metadata;
      this.EnsureEditTransaction();
      foreach (IPropertyId propertyId in PropertyToolBehavior.PropertyList)
      {
        ReferenceStep singleStep = platformMetadata.ResolveProperty(propertyId) as ReferenceStep;
        if (singleStep != null && singleStep.PropertyType.PlatformMetadata == platform.Metadata)
        {
          PropertyReference propertyReference1 = new PropertyReference(singleStep);
          PropertyReference propertyReference2 = propertyManager.FilterProperty((SceneNode) hitElement, propertyReference1);
          if (propertyReference2 != null)
          {
            object second = propertyManager.GetValue(propertyReference2);
            object computedValue = hitElement.GetComputedValue(propertyReference2);
            if (computedValue != MixedProperty.Mixed && !PropertyUtilities.Compare(computedValue, second, hitElement.ViewModel.DefaultView))
              propertyManager.SetValue(propertyReference2, computedValue);
          }
        }
      }
      this.UpdateEditTransaction();
    }
  }
}
