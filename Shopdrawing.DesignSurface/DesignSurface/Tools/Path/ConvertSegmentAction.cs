// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Path.ConvertSegmentAction
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Geometry.Core;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Path
{
  internal sealed class ConvertSegmentAction : PenAction
  {
    private static double segmentTolerance = 0.01;
    private static double dampingThreshold = 0.25;
    private Point initialFirstCubicHandle = new Point(0.0, 0.0);
    private Point initialSecondCubicHandle = new Point(0.0, 0.0);
    private double damping = 1.0;
    private Point initialPointerPosition;
    private double pathSegmentParameter;
    private double firstHandleCoef;
    private double secondHandleCoef;
    private Matrix geometryToDocument;
    private bool hasDragged;

    public override string ActionString
    {
      get
      {
        return StringTable.UndoUnitConvertSegment;
      }
    }

    public override Cursor HoverCursor
    {
      get
      {
        return ToolCursors.ConvertSegmentCursor;
      }
    }

    public override Cursor DragCursor
    {
      get
      {
        return ToolCursors.ConvertSegmentCursor;
      }
    }

    public ConvertSegmentAction(PathEditorTarget pathEditorTarget, SceneViewModel viewModel)
      : base(pathEditorTarget, viewModel)
    {
    }

    protected override void OnBegin(PathEditContext pathEditContext, MouseDevice mouseDevice)
    {
      int figureIndex = pathEditContext.FigureIndex;
      PathFigure figure = this.Path.Figures[figureIndex];
      this.geometryToDocument = this.EditingElementTransformToRoot;
      int num1 = pathEditContext.PartIndex;
      PathFigureEditor pathFigureEditor = new PathFigureEditor(figure, this.PathEditorTarget.PathDiffChangeList, figureIndex);
      if (pathFigureEditor.GetPointKind(num1) != PathPointKind.Cubic)
      {
        PathGeometryEditor pathGeometryEditor = this.BeginEditing();
        pathFigureEditor = new PathFigureEditor(pathGeometryEditor.PathGeometry.Figures[figureIndex], this.PathEditorTarget.PathDiffChangeList, figureIndex);
        num1 = pathGeometryEditor.PromoteSegment(figureIndex, num1);
        this.PathEditContext = new PathEditContext(pathEditContext.FigureIndex, num1);
      }
      this.View.ViewModel.PathPartSelectionSet.SetSelection((PathPart) new PathSegment((SceneElement) this.PathEditorTarget.EditingElement, this.PathEditorTarget.PathEditMode, pathEditContext.FigureIndex, this.PathEditContext.PartIndex));
      int ofUpstreamSegment = pathFigureEditor.GetFirstIndexOfUpstreamSegment(num1);
      Point viewRootCoordinates = this.GetPointInViewRootCoordinates(mouseDevice, false);
      this.ComputeSegmentParameterAndClosestPoint(pathEditContext.GetPathFigure(this.Path), pathEditContext.PartIndex, viewRootCoordinates);
      this.pathSegmentParameter = Math.Max(this.pathSegmentParameter, ConvertSegmentAction.segmentTolerance);
      this.pathSegmentParameter = Math.Min(this.pathSegmentParameter, 1.0 - ConvertSegmentAction.segmentTolerance);
      double num2 = 3.0 * this.pathSegmentParameter * (1.0 - this.pathSegmentParameter);
      double num3 = num2 * this.pathSegmentParameter;
      double num4 = num2 * (1.0 - this.pathSegmentParameter);
      double num5 = (num4 * num4 + num3 * num3 + num4 * num3) * 2.0;
      this.firstHandleCoef = (2.0 * num4 + num3) / num5;
      this.secondHandleCoef = (2.0 * num3 + num4) / num5;
      this.initialFirstCubicHandle = pathFigureEditor.GetPoint(ofUpstreamSegment + 1);
      this.initialSecondCubicHandle = pathFigureEditor.GetPoint(ofUpstreamSegment + 2);
      if (Math.Min(this.pathSegmentParameter, 1.0 - this.pathSegmentParameter) < ConvertSegmentAction.dampingThreshold)
      {
        this.damping = Math.Min(this.pathSegmentParameter, 1.0 - this.pathSegmentParameter);
        this.damping = Math.Sqrt(this.damping);
      }
      else
        this.damping = 1.0;
      this.hasDragged = false;
      base.OnBegin(pathEditContext, mouseDevice);
    }

    protected override void OnEnd()
    {
      if (!this.hasDragged)
      {
        PathFigureEditor pathFigureEditor = new PathFigureEditor(this.PathEditContext.GetPathFigure(this.Path));
        int partIndex = this.PathEditContext.PartIndex;
        int ofUpstreamSegment = pathFigureEditor.GetFirstIndexOfUpstreamSegment(partIndex);
        if (!VectorUtilities.ArePathPointsVeryClose(pathFigureEditor.GetPoint(ofUpstreamSegment), pathFigureEditor.GetPoint(ofUpstreamSegment + 1)) || !VectorUtilities.ArePathPointsVeryClose(pathFigureEditor.GetPoint(partIndex), pathFigureEditor.GetPoint(ofUpstreamSegment + 2)))
        {
          PathGeometryEditor pathGeometryEditor = this.BeginEditing();
          pathGeometryEditor.SetPoint(this.PathEditContext.FigureIndex, ofUpstreamSegment + 2, pathFigureEditor.GetPoint(partIndex));
          pathGeometryEditor.SetPoint(this.PathEditContext.FigureIndex, ofUpstreamSegment + 1, pathFigureEditor.GetPoint(ofUpstreamSegment));
        }
      }
      base.OnEnd();
    }

    protected override void OnDrag(MouseDevice mouseDevice, double zoom)
    {
      if (this.IsActive)
      {
        PathGeometryEditor pathGeometryEditor = this.BeginEditing();
        PathFigureEditor pathFigureEditor = new PathFigureEditor(this.PathEditContext.GetPathFigure(this.Path));
        int figureIndex = this.PathEditContext.FigureIndex;
        int partIndex = this.PathEditContext.PartIndex;
        int ofUpstreamSegment = pathFigureEditor.GetFirstIndexOfUpstreamSegment(partIndex);
        if (!this.hasDragged)
        {
          Point point1 = pathFigureEditor.GetPoint(ofUpstreamSegment);
          Point point2 = pathFigureEditor.GetPoint(partIndex);
          if (VectorUtilities.ArePathPointsVeryClose(point2, pathFigureEditor.GetPoint(ofUpstreamSegment + 2)) && VectorUtilities.ArePathPointsVeryClose(point1, pathFigureEditor.GetPoint(ofUpstreamSegment + 1)))
          {
            pathGeometryEditor.SetPoint(figureIndex, ofUpstreamSegment + 1, VectorUtilities.WeightedAverage(point1, point2, 1.0 / 3.0));
            pathGeometryEditor.SetPoint(figureIndex, ofUpstreamSegment + 2, VectorUtilities.WeightedAverage(point1, point2, 2.0 / 3.0));
            this.initialFirstCubicHandle = pathFigureEditor.GetPoint(ofUpstreamSegment + 1);
            this.initialSecondCubicHandle = pathFigureEditor.GetPoint(ofUpstreamSegment + 2);
          }
          this.hasDragged = true;
        }
        Vector correspondingVector = ElementUtilities.GetCorrespondingVector((this.GetPointInViewRootCoordinates(mouseDevice, false) - this.initialPointerPosition) * this.damping, this.geometryToDocument, this.IsShiftDown ? this.AxisConstraint : (AxisConstraint) null);
        Vector vector1 = correspondingVector * this.firstHandleCoef;
        Vector vector2 = correspondingVector * this.secondHandleCoef;
        pathGeometryEditor.SetPoint(figureIndex, ofUpstreamSegment + 1, this.initialFirstCubicHandle + vector1);
        pathGeometryEditor.SetPoint(figureIndex, ofUpstreamSegment + 2, this.initialSecondCubicHandle + vector2);
      }
      base.OnDrag(mouseDevice, zoom);
    }

    private void ComputeSegmentParameterAndClosestPoint(PathFigure figure, int segmentLastPointIndex, Point position)
    {
      Point closestPoint;
      double distanceSquared;
      this.pathSegmentParameter = new PathFigureEditor(figure).GetClosestPointOfUpstreamSegment(segmentLastPointIndex, position, this.geometryToDocument, Tolerances.CurveFlatteningTolerance, out closestPoint, out distanceSquared);
      this.initialPointerPosition = this.geometryToDocument.Transform(closestPoint);
    }
  }
}
