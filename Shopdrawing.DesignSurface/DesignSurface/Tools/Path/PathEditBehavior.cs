// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Path.PathEditBehavior
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Geometry.Core;
using Microsoft.Expression.DesignSurface.Selection;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Path
{
  internal class PathEditBehavior : AdornedToolBehavior
  {
    private Dictionary<PathEditorTarget, bool> pathEditorTargets = new Dictionary<PathEditorTarget, bool>();
    private AxisConstraint axisConstraint = new AxisConstraint();
    private bool isConstrainingAxes;
    private bool isEnforcingSmoothness;
    private BaseFrameworkElement currentEditingElement;
    private PathPartAdorner pathPartAdorner;
    private double pathSegmentParameter;
    private Point startRootPoint;
    private Point previousRootPoint;
    private Point correspondingPoint;
    private PathEditorTarget pathEditorTarget;

    public override string ActionString
    {
      get
      {
        if (this.pathPartAdorner is PathTangentAdorner)
          return StringTable.UndoUnitAdjustTangent;
        return StringTable.UndoUnitModifyPath;
      }
    }

    public PathEditBehavior(ToolBehaviorContext toolContext, PathEditorTarget pathEditorTarget)
      : base(toolContext)
    {
      this.pathEditorTarget = pathEditorTarget;
    }

    protected override bool OnButtonDown(Point pointerPosition)
    {
      this.pathPartAdorner = (PathPartAdorner) this.ActiveAdorner;
      this.isConstrainingAxes = this.IsShiftDown;
      this.startRootPoint = pointerPosition;
      this.currentEditingElement = this.pathPartAdorner.Element;
      bool isShiftDown = this.IsShiftDown;
      int num = this.IsAltDown ? true : false;
      bool isControlDown = this.IsControlDown;
      PathPointAdorner pathPointAdorner = this.pathPartAdorner as PathPointAdorner;
      PathTangentAdorner pathTangentAdorner = this.pathPartAdorner as PathTangentAdorner;
      PathSegmentAdorner pathSegmentAdorner = this.pathPartAdorner as PathSegmentAdorner;
      this.pathEditorTargets.Clear();
      if (pathTangentAdorner != null)
      {
        pathTangentAdorner.PathAdornerSet.SetActive((PathPartAdorner) pathTangentAdorner, true);
        this.isEnforcingSmoothness = this.ShouldEnforceSmoothness(pathTangentAdorner);
      }
      else
      {
        PathPartSelectionSet partSelectionSet = this.ActiveSceneViewModel.PathPartSelectionSet;
        PathPart pathPart = (PathPart) null;
        if (pathPointAdorner != null)
          pathPart = (PathPart) (PathPoint) pathPointAdorner;
        else if (pathSegmentAdorner != null)
          pathPart = (PathPart) (PathSegment) pathSegmentAdorner;
        if (isShiftDown)
          partSelectionSet.ExtendSelection(pathPart);
        else if (isControlDown && this.ToolBehaviorContext.ActiveTool is ISelectionTool)
        {
          partSelectionSet.ToggleSelection(pathPart);
          if (!partSelectionSet.IsSelected(pathPart))
            this.currentEditingElement = (BaseFrameworkElement) null;
        }
        else if (!partSelectionSet.IsSelected(pathPart))
        {
          partSelectionSet.Clear();
          partSelectionSet.ExtendSelection(pathPart);
        }
        if (partSelectionSet.IsSelected(pathPart) && pathSegmentAdorner != null)
          this.ComputeSegmentParameter();
      }
      this.ToolBehaviorContext.SnappingEngine.Start(this.ToolBehaviorContext, (BaseFrameworkElement) null, (IList<BaseFrameworkElement>) null);
      return true;
    }

    protected override bool OnDrag(Point dragStartPosition, Point dragCurrentPosition, bool scrollNow)
    {
      PerformanceUtility.MeasurePerformanceUntilRender(PerformanceEvent.EditPath);
      if (this.currentEditingElement != null && this.pathPartAdorner != null)
      {
        this.MoveSelection();
        this.ActiveView.EnsureVisible(dragCurrentPosition, scrollNow);
        if (this.IsEditTransactionOpen)
          this.UpdateEditTransaction();
      }
      return true;
    }

    protected override bool OnDragEnd(Point dragStartPosition, Point dragEndPosition)
    {
      return this.AllDone();
    }

    protected override bool OnClickEnd(Point pointerPosition, int clickCount)
    {
      if (clickCount == 2)
      {
        List<PathPart> list = new List<PathPart>();
        foreach (PathPartAdorner pathPartAdorner in (IEnumerable) this.pathPartAdorner.PathAdornerSet.Adorners)
        {
          PathPart pathPart = pathPartAdorner.ToPathPart();
          if (pathPart != (PathPart) null)
            list.Add(pathPart);
        }
        this.ActiveSceneViewModel.PathPartSelectionSet.SetSelection((ICollection<PathPart>) list, false);
      }
      return this.AllDone();
    }

    private bool AllDone()
    {
      this.ToolBehaviorContext.SnappingEngine.Stop();
      if (this.pathPartAdorner is PathTangentAdorner)
        this.pathPartAdorner.PathAdornerSet.SetActive(this.pathPartAdorner, false);
      if (this.HasMouseMovedAfterDown)
      {
        this.pathEditorTarget.EndEditing(false);
        foreach (PathEditorTarget pathEditorTarget in this.pathEditorTargets.Keys)
          pathEditorTarget.EndEditing(false);
        this.CommitEditTransaction();
      }
      this.PopSelf();
      return true;
    }

    protected override bool OnKey(KeyEventArgs args)
    {
      bool isShiftDown = this.IsShiftDown;
      if (this.isConstrainingAxes != isShiftDown)
      {
        this.isConstrainingAxes = isShiftDown;
        this.MoveSelection();
      }
      return true;
    }

    private void ComputeSegmentParameter()
    {
      PathSegmentAdorner pathSegmentAdorner = (PathSegmentAdorner) this.pathPartAdorner;
      Point position = this.ActiveView.Artboard.CalculateTransformFromArtboardToContent().Value.Transform(this.MouseDevice.GetPosition((IInputElement) this.ActiveView.Artboard));
      PathFigureEditor pathFigureEditor = new PathFigureEditor(pathSegmentAdorner.PathGeometry.Figures[pathSegmentAdorner.FigureIndex]);
      Matrix transformMatrix = pathSegmentAdorner.PathAdornerSet.GetTransformMatrix((IViewObject) this.ActiveView.HitTestRoot);
      Point closestPoint;
      double distanceSquared;
      this.pathSegmentParameter = pathFigureEditor.GetClosestPointOfUpstreamSegment(pathSegmentAdorner.LastPointIndex, position, transformMatrix, Tolerances.CurveFlatteningTolerance, out closestPoint, out distanceSquared);
    }

    private bool ShouldEnforceSmoothness(PathTangentAdorner pathTangentAdorner)
    {
      int partIndex = pathTangentAdorner.PartIndex;
      PathEditContext pathEditContext = new PathEditContext(pathTangentAdorner.FigureIndex, partIndex);
      PathFigureEditor pathFigureEditor = new PathFigureEditor(pathEditContext.GetPathFigure(this.pathEditorTarget.PathGeometry));
      Point point1 = pathFigureEditor.GetPoint(pathEditContext.PartIndex);
      bool flag = false;
      if (pathFigureEditor.IsFirstCubicBezierHandle(partIndex))
      {
        if (pathFigureEditor.GetPointKind(partIndex - 1) == PathPointKind.Cubic)
        {
          Point point2 = pathFigureEditor.GetPoint(partIndex - 1);
          Point point3 = pathFigureEditor.GetPoint(partIndex - 2);
          flag = VectorUtilities.HaveOppositeDirections(point1 - point2, point3 - point2);
        }
      }
      else if (pathFigureEditor.IsIndexValid(partIndex + 4) && pathFigureEditor.GetPointKind(partIndex + 4) == PathPointKind.Cubic)
      {
        Point point2 = pathFigureEditor.GetPoint(partIndex + 1);
        Point point3 = pathFigureEditor.GetPoint(partIndex + 2);
        flag = VectorUtilities.HaveOppositeDirections(point1 - point2, point3 - point2);
      }
      return flag;
    }

    private Point GetSnappedPointInAdornerLayer()
    {
      Artboard artboard = this.ActiveView.Artboard;
      Point position = this.MouseDevice.GetPosition((IInputElement) artboard);
      Matrix matrix1 = artboard.CalculateTransformFromArtboardToContent().Value;
      Point point = this.ToolBehaviorContext.SnappingEngine.SnapPoint(matrix1.Transform(position));
      Matrix matrix2 = matrix1;
      matrix2.Invert();
      return matrix2.Transform(point);
    }

    private void MoveSelection()
    {
      Matrix matrixToAdornerLayer = this.pathPartAdorner.PathAdornerSet.GetTransformMatrixToAdornerLayer();
      Point pointInAdornerLayer = this.GetSnappedPointInAdornerLayer();
      PathTangentAdorner pathTangentAdorner = this.pathPartAdorner as PathTangentAdorner;
      PathEditContext pathEditContext = new PathEditContext(this.pathPartAdorner.FigureIndex, this.pathPartAdorner.PartIndex);
      if (pathTangentAdorner != null)
      {
        this.EnsureEditTransaction();
        this.pathEditorTarget.BeginEditing();
        if (!this.HasMouseMovedAfterDown)
        {
          this.startRootPoint = PathFigureUtilities.GetPoint(this.pathEditorTarget.PathGeometry.Figures[pathEditContext.FigureIndex], pathEditContext.PartIndex, false);
          this.correspondingPoint = PathFigureUtilities.GetPoint(this.pathEditorTarget.PathGeometry.Figures[pathEditContext.FigureIndex], pathEditContext.PartIndex, true);
        }
        Point point = this.IsShiftDown ? this.correspondingPoint : this.startRootPoint;
        Vector correspondingVector = ElementUtilities.GetCorrespondingVector(pointInAdornerLayer - point * matrixToAdornerLayer, matrixToAdornerLayer, this.IsShiftDown ? this.axisConstraint : (AxisConstraint) null);
        new PathFigureEditor(pathEditContext.GetPathFigure(this.pathEditorTarget.PathGeometry)).MoveTangent(pathEditContext.PartIndex, point + correspondingVector, this.isEnforcingSmoothness);
        this.ActiveView.AdornerLayer.InvalidateAdornerVisuals(this.EditingElement);
      }
      else
      {
        this.EnsureEditTransaction();
        PathPointAdorner pathPointAdorner = this.pathPartAdorner as PathPointAdorner;
        PathSegmentAdorner pathSegmentAdorner = this.pathPartAdorner as PathSegmentAdorner;
        this.pathPartAdorner.PathAdornerSet.PathEditorTarget.BeginEditing();
        if (!this.HasMouseMovedAfterDown)
        {
          Point point;
          if (pathPointAdorner != null)
          {
            point = PathFigureUtilities.GetPoint(pathEditContext.GetPathFigure(this.pathEditorTarget.PathGeometry), pathEditContext.PartIndex);
          }
          else
          {
            PathFigureEditor pathFigureEditor = new PathFigureEditor(pathEditContext.GetPathFigure(this.pathEditorTarget.PathGeometry));
            int segmentIndex;
            int segmentPointIndex;
            PathFigureUtilities.GetSegmentFromPointIndex(pathEditContext.GetPathFigure(this.pathEditorTarget.PathGeometry), pathEditContext.PartIndex, out segmentIndex, out segmentPointIndex);
            point = pathFigureEditor.Evaluate(segmentIndex, segmentPointIndex, this.pathSegmentParameter) * pathSegmentAdorner.PathGeometryTransformMatrix;
          }
          this.startRootPoint = point;
          this.previousRootPoint = this.startRootPoint;
        }
        Vector correspondingVector = ElementUtilities.GetCorrespondingVector(pointInAdornerLayer - this.startRootPoint * matrixToAdornerLayer, matrixToAdornerLayer, this.IsShiftDown ? this.axisConstraint : (AxisConstraint) null);
        double zoom = this.ActiveView.Artboard.Zoom;
        Vector rootToArtboardScale = this.ActiveView.Artboard.ViewRootToArtboardScale;
        correspondingVector.X /= zoom * rootToArtboardScale.X;
        correspondingVector.Y /= zoom * rootToArtboardScale.Y;
        Vector vector = this.startRootPoint - this.previousRootPoint + correspondingVector;
        this.previousRootPoint = this.startRootPoint + correspondingVector;
        Vector deltaOffset = matrixToAdornerLayer.Transform(vector);
        PathPartSelectionSet partSelectionSet = this.ActiveSceneViewModel.PathPartSelectionSet;
        if (partSelectionSet == null)
          return;
        foreach (BaseFrameworkElement sceneElement in (IEnumerable<SceneElement>) partSelectionSet.SelectedPaths)
        {
          this.MovePathParts(partSelectionSet, sceneElement, PathEditMode.ScenePath, deltaOffset);
          this.MovePathParts(partSelectionSet, sceneElement, PathEditMode.MotionPath, deltaOffset);
          this.MovePathParts(partSelectionSet, sceneElement, PathEditMode.ClippingPath, deltaOffset);
        }
      }
    }

    private void MovePathParts(PathPartSelectionSet pathPartSelectionSet, BaseFrameworkElement sceneElement, PathEditMode pathEditMode, Vector deltaOffset)
    {
      ICollection<PathPart> selectionByElement = pathPartSelectionSet.GetSelectionByElement((SceneElement) sceneElement, pathEditMode);
      Tool activeTool = this.ActiveSceneViewModel.DesignerContext.ToolManager.ActiveTool;
      if (selectionByElement.Count <= 0 || activeTool == null)
        return;
      PathEditorTarget pathEditorTarget = activeTool.GetPathEditorTarget((Base2DElement) sceneElement, pathEditMode);
      if (pathEditorTarget == null)
        return;
      this.EnsureEditTransaction();
      pathEditorTarget.BeginEditing();
      if (!this.pathEditorTargets.ContainsKey(pathEditorTarget))
        this.pathEditorTargets.Add(pathEditorTarget, true);
      Matrix transformToAncestor = pathEditorTarget.GetTransformToAncestor((IViewObject) this.ActiveView.HitTestRoot);
      Vector correspondingVector = ElementUtilities.GetCorrespondingVector(deltaOffset, transformToAncestor);
      PathEditBehavior.TranslateSelection(pathEditorTarget, selectionByElement, correspondingVector);
      this.ActiveView.AdornerLayer.InvalidateAdornerVisuals((SceneElement) pathEditorTarget.EditingElement);
    }

    internal static void TranslateSelection(PathEditorTarget pathEditorTarget, ICollection<PathPart> pathParts, Vector offset)
    {
      List<BitArray> list = new List<BitArray>(pathEditorTarget.PathGeometry.Figures.Count);
      for (int index = 0; index < pathEditorTarget.PathGeometry.Figures.Count; ++index)
        list.Add(new BitArray(PathFigureUtilities.PointCount(pathEditorTarget.PathGeometry.Figures[index]), false));
      foreach (PathPart pathPart in (IEnumerable<PathPart>) pathParts)
      {
        PathPoint pathPoint = pathPart as PathPoint;
        if ((PathPart) pathPoint != (PathPart) null)
        {
          if (pathPoint.PartIndex < list[pathPoint.FigureIndex].Count)
            list[pathPoint.FigureIndex][pathPoint.PartIndex] = true;
        }
        else
        {
          PathSegment pathSegment = pathPart as PathSegment;
          if ((PathPart) pathSegment != (PathPart) null)
          {
            list[pathSegment.FigureIndex][pathSegment.PartIndex] = true;
            PathFigure pathFigure = pathEditorTarget.PathGeometry.Figures[pathPart.FigureIndex];
            if (pathFigure.IsClosed && pathPart.PartIndex == 0)
            {
              int num1 = PathFigureUtilities.PointCount(pathFigure);
              int num2 = 1;
              if (PathFigureUtilities.IsCloseSegmentDegenerate(pathFigure))
                num2 = PathSegmentUtilities.GetPointCount(pathFigure.Segments[pathFigure.Segments.Count - 1]);
              list[pathSegment.FigureIndex][num1 - num2] = true;
            }
            else
            {
              int segmentIndex;
              int segmentPointIndex;
              PathFigureUtilities.GetSegmentFromPointIndex(pathFigure, pathPart.PartIndex, out segmentIndex, out segmentPointIndex);
              int pointCount = PathSegmentUtilities.GetPointCount(pathFigure.Segments[segmentIndex]);
              int index = pathSegment.PartIndex - pointCount;
              if (index >= 0)
                list[pathSegment.FigureIndex][index] = true;
            }
          }
        }
      }
      for (int figureIndex = 0; figureIndex < pathEditorTarget.PathGeometry.Figures.Count; ++figureIndex)
      {
        BitArray bitArray = list[figureIndex];
        PathFigureEditor pathFigureEditor = new PathFigureEditor(pathEditorTarget.PathGeometry.Figures[figureIndex], pathEditorTarget.PathDiffChangeList, figureIndex);
        int num = PathFigureUtilities.PointCount(pathFigureEditor.PathFigure);
        for (int index = 0; index < num; ++index)
        {
          if (bitArray[index])
            pathFigureEditor.MovePoint(index, offset + pathFigureEditor.GetPoint(index));
        }
      }
    }
  }
}
