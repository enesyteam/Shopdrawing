// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Path.JoinAction
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Geometry.Core;
using Microsoft.Expression.DesignSurface.SceneCommands;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.Tools.Path
{
  internal sealed class JoinAction : RedefineTangentAction
  {
    private PathEditorTarget oldPathEditorTarget;
    private PathEditContext oldPathEditContext;

    public override string ActionString
    {
      get
      {
        return StringTable.UndoUnitJoinPaths;
      }
    }

    public override Cursor HoverCursor
    {
      get
      {
        return ToolCursors.PenJoinCursor;
      }
    }

    public override bool SetPathActive
    {
      get
      {
        return true;
      }
    }

    public JoinAction(PathEditorTarget pathEditorTarget, SceneViewModel viewModel, PathEditorTarget oldPathEditorTarget, PathEditContext oldPathEditContext)
      : base(pathEditorTarget, viewModel)
    {
      this.oldPathEditorTarget = oldPathEditorTarget;
      this.oldPathEditContext = oldPathEditContext;
    }

    protected override void OnBegin(PathEditContext pathEditContext, MouseDevice mouseDevice)
    {
      BaseFrameworkElement frameworkElement = this.oldPathEditorTarget.EditingElement.GetCommonAncestor((SceneNode) this.EditingElement) as BaseFrameworkElement;
      Base2DElement editingElement = this.oldPathEditorTarget.EditingElement;
      Base2DElement base2Delement = (Base2DElement) this.EditingElement;
      if (frameworkElement != base2Delement && !base2Delement.GetComputedTransformToElement((SceneElement) frameworkElement).HasInverse)
      {
        this.End();
      }
      else
      {
        int figureIndex1 = pathEditContext.FigureIndex;
        int num1 = PathFigureUtilities.PointCount(this.oldPathEditContext.GetPathFigure(this.oldPathEditorTarget.PathGeometry));
        if (editingElement != base2Delement)
        {
          this.PathEditorTarget.EndEditing(false);
          figureIndex1 += this.oldPathEditorTarget.PathGeometry.Figures.Count;
          List<PathElement> otherElements = new List<PathElement>();
          PathElement mainElement = (PathElement) editingElement;
          PathElement pathElement = (PathElement) base2Delement;
          otherElements.Add(pathElement);
          PathCommandHelper.MakeCompoundPath(mainElement, otherElements, this.EditTransaction);
          this.UpdateEditTransaction();
          this.PathEditorTarget = this.oldPathEditorTarget;
        }
        this.PathEditorTarget.BeginEditing();
        PathGeometryEditor geometryEditor = this.PathEditorTarget.CreateGeometryEditor();
        if (pathEditContext.PartIndex != 0)
          geometryEditor.CreatePathFigureEditor(figureIndex1).Reverse();
        geometryEditor.JoinFigure(this.oldPathEditContext.FigureIndex, figureIndex1);
        int figureIndex2 = this.oldPathEditContext.FigureIndex;
        if (pathEditContext.FigureIndex < this.oldPathEditContext.FigureIndex && editingElement == base2Delement)
          --figureIndex2;
        this.UpdateEditTransaction();
        this.View.UpdateLayout();
        PathFigureEditor pathFigureEditor = geometryEditor.CreatePathFigureEditor(figureIndex2);
        int downstreamSegment1 = pathFigureEditor.GetLastIndexOfDownstreamSegment(num1 - 1);
        if (pathFigureEditor.GetPointKind(downstreamSegment1) == PathPointKind.Cubic)
        {
          Vector lastTangent = this.oldPathEditorTarget.LastTangent;
          pathFigureEditor.SetPoint(downstreamSegment1 - 2, pathFigureEditor.GetPoint(downstreamSegment1 - 2) + lastTangent);
        }
        this.PathEditorTarget.EndEditing(false);
        int downstreamSegment2 = pathFigureEditor.GetLastIndexOfDownstreamSegment(num1 - 1);
        this.PathEditContext = new PathEditContext(figureIndex2, downstreamSegment2);
        int num2 = PathFigureUtilities.PointCount(pathFigureEditor.PathFigure);
        if (pathFigureEditor.GetPointKind(num2 - 1) == PathPointKind.Cubic)
          this.LastTangent = pathFigureEditor.GetPoint(num2 - 1) - pathFigureEditor.GetPoint(num2 - 2);
        else
          this.LastTangent = new Vector(0.0, 0.0);
        this.UpdateEditTransaction();
        this.zeroTangents = false;
        this.PathEditorTarget.BeginEditing();
        base.OnBegin(this.PathEditContext, mouseDevice);
      }
    }
  }
}
