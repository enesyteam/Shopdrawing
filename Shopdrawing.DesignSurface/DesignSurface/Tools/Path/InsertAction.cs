// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Path.InsertAction
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Geometry.Core;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Path
{
  internal sealed class InsertAction : PenAction
  {
    public override string ActionString
    {
      get
      {
        return StringTable.UndoUnitInsertPoint;
      }
    }

    public override Cursor HoverCursor
    {
      get
      {
        return ToolCursors.PenInsertCursor;
      }
    }

    public override Cursor DragCursor
    {
      get
      {
        return ToolCursors.PenInsertCursor;
      }
    }

    public InsertAction(PathEditorTarget pathEditorTarget, SceneViewModel viewModel)
      : base(pathEditorTarget, viewModel)
    {
    }

    protected override void OnBegin(PathEditContext pathEditContext, MouseDevice mouseDevice)
    {
      PerformanceUtility.MeasurePerformanceUntilRender(PerformanceEvent.EditPath);
      Point viewRootCoordinates = this.GetPointInViewRootCoordinates(mouseDevice, false);
      PathFigureEditor pathFigureEditor = new PathFigureEditor(pathEditContext.GetPathFigure(this.Path), this.PathEditorTarget.PathDiffChangeList, pathEditContext.FigureIndex);
      Matrix elementTransformToRoot = this.EditingElementTransformToRoot;
      int partIndex = pathEditContext.PartIndex;
      Point closestPoint;
      double distanceSquared;
      double ofUpstreamSegment = pathFigureEditor.GetClosestPointOfUpstreamSegment(partIndex, viewRootCoordinates, elementTransformToRoot, Tolerances.CurveFlatteningTolerance, out closestPoint, out distanceSquared);
      this.InsertPoint(pathEditContext, ofUpstreamSegment);
      base.OnBegin(pathEditContext, mouseDevice);
      this.End();
    }

    public void InsertPoint(PathEditContext pathEditContext, double segmentParameter)
    {
      if (segmentParameter <= 0.0 || segmentParameter >= 1.0)
        return;
      new PathFigureEditor(pathEditContext.GetPathFigure(this.Path), this.PathEditorTarget.PathDiffChangeList, pathEditContext.FigureIndex).SubdivideSegment(pathEditContext.PartIndex, segmentParameter);
    }
  }
}
