// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Path.TangentUpdateAction
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
  internal abstract class TangentUpdateAction : PenAction
  {
    private Point currentNodePoint;
    private Point currentTangentPoint;
    private bool areTangentsSymmetric;
    private Matrix geometryToDocument;

    public override string ActionString
    {
      get
      {
        return StringTable.UndoUnitUpdateTangents;
      }
    }

    public override Cursor DragCursor
    {
      get
      {
        return ToolCursors.PenTangentCursor;
      }
    }

    public TangentUpdateAction(PathEditorTarget pathEditorTarget, SceneViewModel viewModel)
      : base(pathEditorTarget, viewModel)
    {
    }

    protected void Initialize(Point currentNodePoint, bool areTangentsSymmetric, Matrix geometryToDocument)
    {
      this.currentNodePoint = currentNodePoint;
      this.areTangentsSymmetric = areTangentsSymmetric;
      this.geometryToDocument = geometryToDocument;
    }

    protected override void OnBegin(PathEditContext pathEditContext, MouseDevice mouseDevice)
    {
      if (!Tolerances.AreClose(this.LastTangent, new Vector(0.0, 0.0)))
        this.LastTangent = new Vector(0.0, 0.0);
      base.OnBegin(pathEditContext, mouseDevice);
    }

    protected override void OnDrag(MouseDevice mouseDevice, double zoom)
    {
      this.currentTangentPoint = this.GetPointInViewRootCoordinates(mouseDevice, true);
      Vector correspondingVector = ElementUtilities.GetCorrespondingVector(this.currentTangentPoint - this.currentNodePoint, this.geometryToDocument, this.IsShiftDown ? this.AxisConstraint : (AxisConstraint) null);
      Vector lastTangent = this.LastTangent;
      this.LastTangent = correspondingVector;
      PathFigure pathFigure = this.PathEditContext.GetPathFigure(this.Path);
      PathFigureEditor pathFigureEditor = pathFigure == null ? (PathFigureEditor) null : new PathFigureEditor(pathFigure);
      if (pathFigureEditor != null && !PathFigureUtilities.IsIsolatedPoint(pathFigureEditor.PathFigure))
      {
        PathGeometryEditor pathGeometryEditor = this.BeginEditing();
        bool flag = PathFigureUtilities.IsClosed(pathFigureEditor.PathFigure);
        int index = PathFigureUtilities.PointCount(pathFigureEditor.PathFigure) - (flag ? 0 : 1);
        Point point1 = pathFigureEditor.GetPoint(index - 1);
        Point point2 = pathFigureEditor.GetPoint(index);
        if (pathFigureEditor.GetPointKind(index) == PathPointKind.Line)
        {
          pathGeometryEditor.RemoveLastSegmentOfFigure(this.PathEditContext.FigureIndex);
          if (flag)
            pathGeometryEditor.CloseFigureWithCubicBezier(point1, point2, this.PathEditContext.FigureIndex);
          else
            pathGeometryEditor.AppendCubicBezier(point1, point2, point2, this.PathEditContext.FigureIndex);
          pathFigureEditor = new PathFigureEditor(this.PathEditContext.GetPathFigure(this.Path));
        }
        if (!this.IsAltDown)
        {
          if (!this.areTangentsSymmetric)
          {
            double length = this.LastTangent.Length;
            if (length > 0.0)
            {
              int num1 = PathFigureUtilities.PointCount(pathFigureEditor.PathFigure);
              double num2 = (pathFigureEditor.GetPoint(num1 - 1) - pathFigureEditor.GetPoint(num1 - 2)).Length / length;
              pathGeometryEditor.SetPoint(this.PathEditContext.FigureIndex, num1 - 2, point2 - num2 * this.LastTangent);
            }
          }
          else
          {
            int pointIndex = PathFigureUtilities.PointCount(pathFigureEditor.PathFigure) - (PathFigureUtilities.IsClosed(pathFigureEditor.PathFigure) ? 1 : 2);
            pathGeometryEditor.SetPoint(this.PathEditContext.FigureIndex, pointIndex, point2 - this.LastTangent);
          }
        }
        if (PathFigureUtilities.IsClosed(pathFigureEditor.PathFigure) && pathFigureEditor.GetPointKind(3) == PathPointKind.Cubic && this.areTangentsSymmetric)
          pathGeometryEditor.SetPoint(this.PathEditContext.FigureIndex, 1, point2 + this.LastTangent);
        this.PathEditorTarget.AddCriticalEdit();
      }
      base.OnDrag(mouseDevice, zoom);
    }

    protected override void OnEnd()
    {
      base.OnEnd();
    }
  }
}
