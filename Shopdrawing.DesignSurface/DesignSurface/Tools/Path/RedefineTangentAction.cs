// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Path.RedefineTangentAction
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Geometry.Core;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Path
{
  public class RedefineTangentAction : PenAction
  {
    protected bool zeroTangents = true;
    protected Matrix geometryToDocument;
    protected bool hasMoved;

    public override string ActionString
    {
      get
      {
        return StringTable.UndoUnitRedefineTangents;
      }
    }

    public override Cursor HoverCursor
    {
      get
      {
        return ToolCursors.PenAdjustCursor;
      }
    }

    public override Cursor DragCursor
    {
      get
      {
        return ToolCursors.PenTangentCursor;
      }
    }

    public RedefineTangentAction(PathEditorTarget pathEditorTarget, SceneViewModel viewModel)
      : base(pathEditorTarget, viewModel)
    {
    }

    protected override void OnBegin(PathEditContext pathEditContext, MouseDevice mouseDevice)
    {
      this.View.ViewModel.PathPartSelectionSet.SetSelection((PathPart) new PathPoint((SceneElement) this.PathEditorTarget.EditingElement, this.PathEditorTarget.PathEditMode, pathEditContext.FigureIndex, pathEditContext.PartIndex));
      this.geometryToDocument = this.EditingElementTransformToRoot;
      int figureIndex = pathEditContext.FigureIndex;
      int num = pathEditContext.PartIndex;
      PathFigureEditor pathFigureEditor = new PathFigureEditor(pathEditContext.GetPathFigure(this.Path));
      Point point = pathFigureEditor.GetPoint(num);
      int downstreamSegment = pathFigureEditor.GetLastIndexOfDownstreamSegment(num);
      if (pathFigureEditor.GetPointKind(num) != PathPointKind.Cubic && pathFigureEditor.GetPointKind(num) != PathPointKind.Start || pathFigureEditor.GetPointKind(downstreamSegment) != PathPointKind.Cubic)
      {
        PathGeometryEditor pathGeometryEditor = this.BeginEditing();
        num = this.PromoteAdjacentSegments(pathEditContext);
        pathFigureEditor = new PathFigureEditor(pathGeometryEditor.PathGeometry.Figures[figureIndex]);
        this.PathEditContext = new PathEditContext(figureIndex, num);
        this.View.ViewModel.PathPartSelectionSet.SetSelection((PathPart) new PathPoint((SceneElement) this.PathEditorTarget.EditingElement, this.PathEditorTarget.PathEditMode, this.PathEditContext.FigureIndex, this.PathEditContext.PartIndex));
      }
      if (this.zeroTangents)
      {
        if ((num > 0 || PathFigureUtilities.IsClosed(pathFigureEditor.PathFigure)) && !VectorUtilities.ArePathPointsVeryClose(pathFigureEditor.GetPoint(num), pathFigureEditor.GetPoint(num - 1)))
          this.BeginEditing().SetPoint(figureIndex, num - 1, point);
        if (pathFigureEditor.GetLastIndexOfDownstreamSegment(num) != num && !VectorUtilities.ArePathPointsVeryClose(pathFigureEditor.GetPoint(num), pathFigureEditor.GetPoint(num + 1)))
          this.BeginEditing().SetPoint(figureIndex, num + 1, point);
        if (num == PathFigureUtilities.PointCount(pathFigureEditor.PathFigure) - 1 && PathFigureUtilities.IsOpen(pathFigureEditor.PathFigure))
          this.LastTangent = new Vector(0.0, 0.0);
      }
      base.OnBegin(pathEditContext, mouseDevice);
    }

    protected override void OnDrag(MouseDevice mouseDevice, double zoom)
    {
      Point point = PathFigureUtilities.GetPoint(this.PathEditContext.GetPathFigure(this.Path), this.PathEditContext.PartIndex);
      Point point1 = this.geometryToDocument.Transform(point);
      Point viewRootCoordinates = this.GetPointInViewRootCoordinates(mouseDevice, true);
      Vector vector = viewRootCoordinates - point1;
      Vector correspondingVector = ElementUtilities.GetCorrespondingVector(vector, this.geometryToDocument, this.IsShiftDown ? this.AxisConstraint : (AxisConstraint) null);
      if (this.hasMoved || Tolerances.HaveMoved(point1, viewRootCoordinates, zoom))
      {
        this.hasMoved = true;
        PathGeometry path = this.Path;
        int figureIndex = this.PathEditContext.FigureIndex;
        int partIndex = this.PathEditContext.PartIndex;
        PathGeometryEditor pathGeometryEditor = this.BeginEditing();
        PathFigureEditor pathFigureEditor = new PathFigureEditor(path.Figures[figureIndex]);
        if (partIndex > 0 || PathFigureUtilities.IsClosed(pathFigureEditor.PathFigure))
          pathGeometryEditor.SetPoint(figureIndex, partIndex - 1, point - correspondingVector);
        if (pathFigureEditor.GetLastIndexOfDownstreamSegment(partIndex) != partIndex)
          pathGeometryEditor.SetPoint(figureIndex, partIndex + 1, point + correspondingVector);
        if (partIndex == PathFigureUtilities.PointCount(pathFigureEditor.PathFigure) - 1 && PathFigureUtilities.IsOpen(pathFigureEditor.PathFigure))
          this.LastTangent = vector;
        this.PathEditorTarget.AddCriticalEdit();
      }
      base.OnDrag(mouseDevice, zoom);
    }

    public int PromoteAdjacentSegments(PathEditContext pathEditContext)
    {
      int partIndex = pathEditContext.PartIndex;
      int index = partIndex;
      PathFigureEditor figureEditor = this.PathEditorTarget.CreateFigureEditor(pathEditContext.FigureIndex);
      bool flag = false;
      if (figureEditor.GetPointKind(partIndex) != PathPointKind.Cubic && figureEditor.GetPointKind(partIndex) != PathPointKind.Start)
      {
        index = this.PromoteSegment(pathEditContext.FigureIndex, partIndex);
        if (index != partIndex)
          flag = true;
      }
      int num = figureEditor.GetLastIndexOfDownstreamSegment(index) % PathFigureUtilities.PointCount(figureEditor.PathFigure);
      if (index != num && figureEditor.GetPointKind(num) != PathPointKind.Cubic && this.PromoteSegment(pathEditContext.FigureIndex, num) != num)
        flag = true;
      if (flag)
      {
        this.PathEditorTarget.EndEditing(false);
        this.PathEditorTarget.BeginEditing();
      }
      return index;
    }

    public int PromoteSegment(int figureIndex, int pointIndex)
    {
      int num = pointIndex;
      PathFigureEditor figureEditor = this.PathEditorTarget.CreateFigureEditor(figureIndex);
      if (figureEditor.GetPointKind(pointIndex) != PathPointKind.Cubic)
        num = figureEditor.PromoteSegment(pointIndex);
      return num;
    }
  }
}
