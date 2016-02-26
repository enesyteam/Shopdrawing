// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Path.DeleteLastSegmentAction
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Geometry.Core;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Path
{
  internal sealed class DeleteLastSegmentAction : DeleteAction
  {
    public override string ActionString
    {
      get
      {
        return StringTable.UndoUnitDeleteSegment;
      }
    }

    public override Cursor HoverCursor
    {
      get
      {
        return (Cursor) null;
      }
    }

    public override Cursor DragCursor
    {
      get
      {
        return (Cursor) null;
      }
    }

    public DeleteLastSegmentAction(PathEditorTarget pathEditorTarget, SceneViewModel viewModel)
      : base(pathEditorTarget, viewModel)
    {
    }

    protected override void DoPreDelete()
    {
      this.LastTangent = new Vector(0.0, 0.0);
    }

    protected override void DoDelete(PathEditContext pathEditContext)
    {
      PathFigure pathFigure = pathEditContext.GetPathFigure(this.Path);
      PathFigureEditor pathFigureEditor = new PathFigureEditor(pathFigure);
      int index1 = PathFigureUtilities.PointCount(pathFigure) - (PathFigureUtilities.IsClosed(pathFigure) ? 0 : 1);
      if (pathFigureEditor.GetPointKind(index1) == PathPointKind.Cubic)
      {
        int index2 = index1 - 3;
        this.LastTangent = pathFigureEditor.GetPoint(index2 + 1) - pathFigureEditor.GetPoint(index2);
      }
      this.BeginEditing().RemoveLastSegmentOfFigure(pathEditContext.FigureIndex);
    }
  }
}
