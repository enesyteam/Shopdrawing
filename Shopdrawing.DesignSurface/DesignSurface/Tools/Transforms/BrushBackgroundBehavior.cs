// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.BrushBackgroundBehavior
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.BrushEditor;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  internal sealed class BrushBackgroundBehavior : BrushTransformBehavior
  {
    private Point dragStartPosition;
    private Point dragCurrentPosition;
    private bool isDragInterrupted;
    private bool hasInitializedBrush;

    public override string ActionString
    {
      get
      {
        return StringTable.UndoUnitMoveBrush;
      }
    }

    public BrushBackgroundBehavior(ToolBehaviorContext toolContext)
      : base(toolContext)
    {
    }

    protected override bool OnButtonDown(Point pointerPosition)
    {
      this.dragStartPosition = pointerPosition;
      return true;
    }

    protected override bool OnDrag(Point dragStartPosition, Point dragCurrentPosition, bool scrollNow)
    {
      if (!this.isDragInterrupted)
      {
        this.dragCurrentPosition = dragCurrentPosition;
        this.UpdateTranslation(scrollNow);
      }
      return true;
    }

    protected override bool OnDragEnd(Point dragStartPosition, Point dragEndPosition)
    {
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
        this.isDragInterrupted = true;
      }
      return true;
    }

    private void UpdateTranslation(bool scrollNow)
    {
      SceneView activeView = this.ActiveView;
      SceneViewModel viewModel = activeView.ViewModel;
      if (!this.HasMouseMovedAfterDown)
        this.hasInitializedBrush = false;
      SceneElement primarySelection = this.ActiveView.ElementSelectionSet.PrimarySelection;
      if (primarySelection == null)
        return;
      PropertyReference propertyReference = this.GetBrushPropertyReference((SceneNode) primarySelection);
      if (propertyReference == null || Adorner.NonAffineTransformInParentStack(primarySelection))
        return;
      this.EnsureEditTransaction();
      if (!this.hasInitializedBrush)
      {
        if (!PlatformTypes.IsInstance(this.ActiveView.ElementSelectionSet.PrimarySelection.GetComputedValue(propertyReference), PlatformTypes.GradientBrush, (ITypeResolver) primarySelection.ProjectContext))
        {
          object lastUsed = BrushCategory.GetLastUsed(this.ActiveView.DesignerContext, this.ActiveView.Document.DocumentContext, BrushCategory.Gradient);
          this.ActiveView.ElementSelectionSet.PrimarySelection.SetValueAsWpf(propertyReference, lastUsed);
          this.UpdateEditTransaction();
        }
        this.CopyPrimaryBrushToSelection();
        this.hasInitializedBrush = true;
      }
      object computedValue = primarySelection.GetComputedValue(propertyReference);
      Matrix matrix = activeView.GetComputedTransformFromRoot(primarySelection) * BrushAdorner.GetCompleteInverseBrushTransformMatrix((Base2DElement) primarySelection, computedValue, true);
      Point point1 = this.dragStartPosition * matrix;
      Point point2 = this.dragCurrentPosition * matrix;
      Point point3 = RoundingHelper.RoundPosition(point1);
      Point point4 = RoundingHelper.RoundPosition(point2);
      if (PlatformTypes.IsInstance(computedValue, PlatformTypes.LinearGradientBrush, (ITypeResolver) primarySelection.ProjectContext))
      {
        this.SetBrushValue(LinearGradientBrushNode.StartPointProperty, (object) point3);
        this.SetBrushValue(LinearGradientBrushNode.EndPointProperty, (object) point4);
      }
      else if (PlatformTypes.IsInstance(computedValue, PlatformTypes.RadialGradientBrush, (ITypeResolver) primarySelection.ProjectContext))
      {
        this.SetBrushValue(RadialGradientBrushNode.CenterProperty, (object) point3);
        this.SetBrushValue(RadialGradientBrushNode.GradientOriginProperty, (object) point3);
        if (this.IsShiftDown)
        {
          Vector vector = point4 - point3;
          this.SetBrushValue(RadialGradientBrushNode.RadiusXProperty, (object) vector.Length);
          this.SetBrushValue(RadialGradientBrushNode.RadiusYProperty, (object) vector.Length);
        }
        else
        {
          Point point5 = RoundingHelper.RoundPosition(new Point(Math.Abs(point3.X - point4.X), Math.Abs(point3.Y - point4.Y)));
          this.SetBrushValue(RadialGradientBrushNode.RadiusXProperty, (object) point5.X);
          this.SetBrushValue(RadialGradientBrushNode.RadiusYProperty, (object) point5.Y);
        }
      }
      activeView.EnsureVisible(this.dragStartPosition + this.dragCurrentPosition - this.dragStartPosition, scrollNow);
      this.UpdateEditTransaction();
    }
  }
}
