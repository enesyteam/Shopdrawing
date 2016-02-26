// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.GradientStopBehavior
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Geometry.Core;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector;
using Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.BrushEditor;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  internal sealed class GradientStopBehavior : BrushTransformBehavior
  {
    private Point dragCurrentPosition;
    private bool isDraggedOff;
    private bool isDragInterrupted;
    private Color saveDraggedOffColor;

    public override string ActionString
    {
      get
      {
        return StringTable.UndoUnitMoveBrush;
      }
    }

    private GradientStopAdorner ActiveAdorner
    {
      get
      {
        return (GradientStopAdorner) base.ActiveAdorner;
      }
    }

    public GradientStopBehavior(ToolBehaviorContext toolContext)
      : base(toolContext)
    {
    }

    protected override bool OnButtonDown(Point pointerPosition)
    {
      this.SetSelectedStopIndex(this.ActiveAdorner.Index);
      return true;
    }

    internal override bool ShouldMotionlessAutoScroll(Point mousePoint, Rect artboardBoundary)
    {
      return false;
    }

    protected override bool OnDrag(Point dragStartPosition, Point dragCurrentPosition, bool scrollNow)
    {
      if (!this.isDragInterrupted)
      {
        this.dragCurrentPosition = dragCurrentPosition;
        this.UpdateTranslation();
      }
      return true;
    }

    protected override bool OnDragEnd(Point dragStartPosition, Point dragEndPosition)
    {
      if (this.isDraggedOff)
      {
        PropertyReference propertyReference1 = this.GetBrushPropertyReference((SceneNode) this.EditingElement).Append(GradientBrushNode.GradientStopsProperty);
        PropertyReference propertyReference2 = propertyReference1.Append((ReferenceStep) IndexedClrPropertyReferenceStep.GetReferenceStep((IPlatformMetadata) this.ActiveView.ViewModel.ProjectContext.Platform.Metadata, PlatformTypes.GradientStopCollection, this.ActiveAdorner.Index));
        foreach (SceneElement sceneElement in this.ActiveView.ElementSelectionSet.Selection)
        {
          if (sceneElement.IsSet(propertyReference2) == PropertyState.Set)
            sceneElement.RemoveValueAt(propertyReference1, this.ActiveAdorner.Index);
        }
        this.isDraggedOff = false;
      }
      if (this.HasMouseMovedAfterDown)
        this.CommitEditTransaction();
      this.isDragInterrupted = false;
      return base.OnDragEnd(dragStartPosition, dragEndPosition);
    }

    protected override bool OnKey(KeyEventArgs args)
    {
      if (args.Key == Key.Escape)
      {
        this.CancelEditTransaction();
        this.isDraggedOff = false;
        this.isDragInterrupted = true;
        this.ActiveAdorner.AdornerSet.InvalidateStructure();
      }
      return true;
    }

    protected override bool OnClickEnd(Point pointerPosition, int clickCount)
    {
      if (clickCount > 1)
      {
        PropertyReferenceProperty colorProperty = this.EditingElement.ViewModel.DesignerContext.PropertyInspectorModel.SceneNodeObjectSet.CreateProperty(this.GetBrushPropertyReference((SceneNode) this.EditingElement).Append(GradientBrushNode.GradientStopsProperty).Append((ReferenceStep) IndexedClrPropertyReferenceStep.GetReferenceStep((IPlatformMetadata) this.ActiveView.ViewModel.ProjectContext.Platform.Metadata, PlatformTypes.GradientStopCollection, this.ActiveAdorner.Index)).Append(GradientStopNode.ColorProperty), (AttributeCollection) null);
        this.CopyPrimaryBrushToSelection();
        GradientStopColorPopup gradientStopColorPopup = new GradientStopColorPopup(colorProperty, this.EditingElement.DesignerContext);
        gradientStopColorPopup.SynchronousClosed += (EventHandler) ((sender, e) =>
        {
          colorProperty.OnRemoveFromCategory();
          this.CommitEditTransaction();
        });
        gradientStopColorPopup.IsOpen = true;
      }
      return base.OnClickEnd(pointerPosition, clickCount);
    }

    private void UpdateTranslation()
    {
      SceneViewModel viewModel = this.ActiveView.ViewModel;
      PropertyReference propertyReference1 = this.GetBrushPropertyReference((SceneNode) this.EditingElement);
      if (propertyReference1 == null)
        return;
      Matrix matrixToAdornerLayer = this.ActiveAdorner.AdornerSet.GetTransformMatrixToAdornerLayer();
      this.EnsureEditTransaction();
      Point startPoint;
      Point endPoint;
      if (PlatformTypes.IsInstance(this.ActiveAdorner.PlatformBrush, PlatformTypes.GradientBrush, (ITypeResolver) this.EditingElement.ProjectContext) && this.ActiveAdorner.GetBrushEndpoints(out startPoint, out endPoint))
      {
        if (!this.HasMouseMovedAfterDown)
          this.CopyPrimaryBrushToSelection();
        PropertyReference propertyReference2 = propertyReference1.Append(GradientBrushNode.GradientStopsProperty);
        PropertyReference propertyReference3 = propertyReference2.Append((ReferenceStep) IndexedClrPropertyReferenceStep.GetReferenceStep((IPlatformMetadata) viewModel.ProjectContext.Platform.Metadata, PlatformTypes.GradientStopCollection, this.ActiveAdorner.Index));
        PropertyReference propertyReference4 = propertyReference3.Append(GradientStopNode.OffsetProperty);
        PropertyReference propertyReference5 = propertyReference3.Append(GradientStopNode.ColorProperty);
        Matrix matrix = this.ActiveAdorner.GetCompleteBrushTransformMatrix(true) * this.ActiveView.GetComputedTransformToRoot(this.EditingElement);
        GradientStopCollection gradientStopCollection = (GradientStopCollection) this.EditingElement.GetComputedValueAsWpf(propertyReference2);
        GradientStop gradientStop = gradientStopCollection[this.ActiveAdorner.Index];
        Vector perpendicular;
        double num1 = GradientStopBehavior.VectorProjection(startPoint * matrix, endPoint * matrix, this.dragCurrentPosition, gradientStop.Offset, out perpendicular);
        double length = (perpendicular * matrixToAdornerLayer).Length;
        int index = this.ActiveAdorner.Index + 1;
        if (index >= gradientStopCollection.Count)
          index = this.ActiveAdorner.Index - 1;
        if (length > 30.0 && !this.isDraggedOff && gradientStopCollection.Count > 2)
        {
          this.isDraggedOff = true;
          this.ActiveAdorner.Hidden = true;
          this.saveDraggedOffColor = gradientStop.Color;
          this.SetBrushValue(propertyReference5, (object) gradientStopCollection[index].Color);
          this.SetBrushValue(propertyReference4, (object) gradientStopCollection[index].Offset);
          this.Cursor = ToolCursors.MinusArrowCursor;
        }
        if (length <= 30.0 && this.isDraggedOff)
        {
          this.isDraggedOff = false;
          this.ActiveAdorner.Hidden = false;
          this.SetBrushValue(propertyReference5, (object) this.saveDraggedOffColor);
          this.Cursor = this.ActiveAdorner.AdornerSet.GetCursor((IAdorner) this.ActiveAdorner);
        }
        if (!this.isDraggedOff)
        {
          double num2 = RoundingHelper.RoundLength(gradientStop.Offset + num1);
          if (num2 > 1.0)
            num2 = 1.0;
          else if (num2 < 0.0)
            num2 = 0.0;
          this.SetBrushValue(propertyReference4, (object) num2);
        }
        else
          this.SetBrushValue(propertyReference4, (object) gradientStopCollection[index].Offset);
      }
      if (!this.HasMouseMovedAfterDown)
        this.SetSelectedStopIndex(this.ActiveAdorner.Index);
      this.UpdateEditTransaction();
    }

    public static double VectorProjection(Point S, Point E, Point drag, double offset, out Vector perpendicular)
    {
      perpendicular = new Vector();
      Vector vector1 = E - S;
      Point point = offset * vector1 + S;
      Vector vector2 = drag - point;
      double num = 0.0;
      if (vector1.Length > FloatingPointArithmetic.SingleTolerance)
        num = vector2 * vector1 / (vector1.Length * vector1.Length);
      perpendicular = vector2 - vector1 * num;
      return num;
    }
  }
}
