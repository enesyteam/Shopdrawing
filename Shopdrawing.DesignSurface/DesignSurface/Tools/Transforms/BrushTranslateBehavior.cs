// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.BrushTranslateBehavior
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  internal sealed class BrushTranslateBehavior : BrushTransformBehavior
  {
    private Vector lastMove;
    private Point dragStartPosition;
    private Point dragCurrentPosition;
    private bool isConstraining;

    public override string ActionString
    {
      get
      {
        return StringTable.UndoUnitMoveBrush;
      }
    }

    private BrushTranslateAdorner ActiveAdorner
    {
      get
      {
        return base.ActiveAdorner as BrushTranslateAdorner;
      }
    }

    public BrushTranslateBehavior(ToolBehaviorContext toolContext)
      : base(toolContext)
    {
    }

    protected override bool OnButtonDown(Point pointerPosition)
    {
      this.dragStartPosition = pointerPosition;
      this.lastMove = new Vector(0.0, 0.0);
      return true;
    }

    protected override bool OnClickEnd(Point pointerPosition, int clickCount)
    {
      if (clickCount == 1 && this.ActiveAdorner != null && this.Tool is GradientBrushTool)
      {
        PropertyReference propertyReference1 = this.GetBrushPropertyReference((SceneNode) this.EditingElement);
        GradientStopCollection gradientStopCollection = (GradientStopCollection) this.ActiveAdorner.GetBrushPropertyAsWpf(GradientBrushNode.GradientStopsProperty);
        Matrix matrix = this.ActiveAdorner.GetCompleteBrushTransformMatrix(true) * this.ActiveView.GetComputedTransformToRoot(this.EditingElement);
        Point startPoint;
        Point endPoint;
        if (gradientStopCollection == null || !this.ActiveAdorner.GetBrushEndpoints(out startPoint, out endPoint))
          return true;
        Vector perpendicular;
        double offset = RoundingHelper.RoundLength(GradientStopBehavior.VectorProjection(startPoint * matrix, endPoint * matrix, this.dragStartPosition, 0.0, out perpendicular));
        int index1 = 0;
        int index2 = 0;
        for (int index3 = 0; index3 < gradientStopCollection.Count; ++index3)
        {
          if (gradientStopCollection[index1].Offset > offset || gradientStopCollection[index3].Offset > gradientStopCollection[index1].Offset && gradientStopCollection[index3].Offset <= offset)
            index1 = index3;
          if (gradientStopCollection[index2].Offset < offset || gradientStopCollection[index3].Offset < gradientStopCollection[index2].Offset && gradientStopCollection[index3].Offset >= offset)
            index2 = index3;
        }
        GradientStop gradientStop;
        if (gradientStopCollection.Count == 0)
        {
          gradientStop = new GradientStop(Colors.White, offset);
        }
        else
        {
          double num = (offset - gradientStopCollection[index1].Offset) / (gradientStopCollection[index2].Offset - gradientStopCollection[index1].Offset);
          if (num > 1.0)
            num = 1.0;
          if (num < 0.0)
            num = 0.0;
          gradientStop = new GradientStop(Color.FromArgb((byte) ((double) gradientStopCollection[index1].Color.A * (1.0 - num) + (double) gradientStopCollection[index2].Color.A * num), (byte) ((double) gradientStopCollection[index1].Color.R * (1.0 - num) + (double) gradientStopCollection[index2].Color.R * num), (byte) ((double) gradientStopCollection[index1].Color.G * (1.0 - num) + (double) gradientStopCollection[index2].Color.G * num), (byte) ((double) gradientStopCollection[index1].Color.B * (1.0 - num) + (double) gradientStopCollection[index2].Color.B * num)), offset);
        }
        this.CopyPrimaryBrushToSelection();
        PropertyReference propertyReference2 = propertyReference1.Append(GradientBrushNode.GradientStopsProperty);
        PropertyReference propertyReference3 = propertyReference2.Append(GradientStopCollectionNode.CountProperty);
        foreach (SceneElement sceneElement in this.ActiveView.ElementSelectionSet.Selection)
        {
          PropertyReference propertyReference4 = this.ActiveView.DesignerContext.PropertyManager.FilterProperty((SceneNode) sceneElement, propertyReference3);
          PropertyReference propertyReference5 = this.ActiveView.DesignerContext.PropertyManager.FilterProperty((SceneNode) sceneElement, propertyReference2);
          if (propertyReference4 != null && propertyReference5 != null)
          {
            int index3 = (int) sceneElement.GetComputedValue(propertyReference4);
            sceneElement.InsertValueAsWpf(propertyReference5, index3, (object) gradientStop);
          }
        }
        this.SetSelectedStopIndex(gradientStopCollection.Count);
        this.CommitEditTransaction();
      }
      return base.OnClickEnd(pointerPosition, clickCount);
    }

    protected override bool OnDrag(Point dragStartPosition, Point dragCurrentPosition, bool scrollNow)
    {
      if (!this.HasMouseMovedAfterDown && this.ToolBehaviorContext.Tool is GradientBrushTool)
        this.Cursor = Cursors.SizeAll;
      this.dragCurrentPosition = dragCurrentPosition;
      this.isConstraining = this.IsShiftDown;
      this.UpdateTranslation(scrollNow);
      return true;
    }

    protected override bool OnDragEnd(Point dragStartPosition, Point dragEndPosition)
    {
      if (this.HasMouseMovedAfterDown)
        this.CommitEditTransaction();
      return base.OnDragEnd(dragStartPosition, dragEndPosition);
    }

    protected override bool OnKey(KeyEventArgs args)
    {
      bool isShiftDown = this.IsShiftDown;
      if (this.isConstraining != isShiftDown)
      {
        this.isConstraining = isShiftDown;
        if (this.HasMouseMovedAfterDown)
          this.UpdateTranslation(false);
      }
      return true;
    }

    private void UpdateTranslation(bool scrollNow)
    {
      SceneView activeView = this.ActiveView;
      SceneViewModel viewModel = activeView.ViewModel;
      Vector delta = this.dragCurrentPosition - this.dragStartPosition;
      if (this.isConstraining)
        delta = this.ConstrainDeltaToAxis(delta);
      if (delta == this.lastMove || this.IsAltDown)
        return;
      this.EnsureEditTransaction();
      Vector vector1 = delta - this.lastMove;
      SceneElement primarySelection = viewModel.ElementSelectionSet.PrimarySelection;
      if (primarySelection == null)
        return;
      PropertyReference propertyReference = this.GetBrushPropertyReference((SceneNode) primarySelection);
      if (propertyReference == null)
        return;
      object computedValue = primarySelection.GetComputedValue(propertyReference);
      if (computedValue != null && !PlatformTypes.IsInstance(computedValue, PlatformTypes.SolidColorBrush, (ITypeResolver) primarySelection.ProjectContext))
      {
        Vector vector2 = vector1 * activeView.GetComputedTransformFromRoot(primarySelection) * BrushAdorner.GetCompleteInverseBrushTransformMatrix((Base2DElement) primarySelection, computedValue, true);
        if (PlatformTypes.IsInstance(computedValue, PlatformTypes.LinearGradientBrush, (ITypeResolver) primarySelection.ProjectContext) && this.Tool is GradientBrushTool)
        {
          ReferenceStep referenceStep1 = (ReferenceStep) viewModel.ProjectContext.ResolveProperty(LinearGradientBrushNode.StartPointProperty);
          ReferenceStep referenceStep2 = (ReferenceStep) viewModel.ProjectContext.ResolveProperty(LinearGradientBrushNode.EndPointProperty);
          Point point1 = RoundingHelper.RoundPosition((Point) viewModel.DefaultView.ConvertToWpfValue(referenceStep1.GetCurrentValue(computedValue)) + vector2);
          Point point2 = RoundingHelper.RoundPosition((Point) viewModel.DefaultView.ConvertToWpfValue(referenceStep2.GetCurrentValue(computedValue)) + vector2);
          this.SetBrushValue(LinearGradientBrushNode.StartPointProperty, (object) point1);
          this.SetBrushValue(LinearGradientBrushNode.EndPointProperty, (object) point2);
        }
        else if (PlatformTypes.IsInstance(computedValue, PlatformTypes.RadialGradientBrush, (ITypeResolver) primarySelection.ProjectContext) && this.Tool is GradientBrushTool)
        {
          ReferenceStep referenceStep1 = (ReferenceStep) viewModel.ProjectContext.ResolveProperty(RadialGradientBrushNode.CenterProperty);
          ReferenceStep referenceStep2 = (ReferenceStep) viewModel.ProjectContext.ResolveProperty(RadialGradientBrushNode.GradientOriginProperty);
          Point point1 = RoundingHelper.RoundPosition((Point) viewModel.DefaultView.ConvertToWpfValue(referenceStep1.GetCurrentValue(computedValue)) + vector2);
          Point point2 = RoundingHelper.RoundPosition((Point) viewModel.DefaultView.ConvertToWpfValue(referenceStep2.GetCurrentValue(computedValue)) + vector2);
          this.SetBrushValue(RadialGradientBrushNode.CenterProperty, (object) point1);
          this.SetBrushValue(RadialGradientBrushNode.GradientOriginProperty, (object) point2);
        }
        else
          this.TranslateBrushPosition(vector1 * activeView.GetComputedTransformFromRoot(primarySelection) * BrushAdorner.GetCompleteInverseBrushTransformMatrix((Base2DElement) primarySelection, computedValue, false), primarySelection);
      }
      activeView.EnsureVisible(this.dragStartPosition + delta, scrollNow);
      this.lastMove = delta;
      this.UpdateEditTransaction();
    }

    private Vector ConstrainDeltaToAxis(Vector delta)
    {
      Vector vector = delta;
      if (Math.Abs(delta.X) > Math.Abs(delta.Y))
        vector.Y = 0.0;
      else
        vector.X = 0.0;
      return vector;
    }
  }
}
