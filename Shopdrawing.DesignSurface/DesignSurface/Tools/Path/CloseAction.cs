// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Path.CloseAction
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Geometry.Core;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Path
{
  internal sealed class CloseAction : TangentUpdateAction
  {
    public override string ActionString
    {
      get
      {
        return StringTable.UndoUnitClosePath;
      }
    }

    public override Cursor HoverCursor
    {
      get
      {
        return ToolCursors.PenCloseCursor;
      }
    }

    public CloseAction(PathEditorTarget pathEditorTarget, SceneViewModel viewModel)
      : base(pathEditorTarget, viewModel)
    {
    }

    protected override void OnBegin(PathEditContext pathEditContext, MouseDevice mouseDevice)
    {
      PathGeometryEditor pathGeometryEditor = this.BeginEditing();
      Point point = PathFigureUtilities.FirstPoint(pathEditContext.GetPathFigure(this.Path));
      Matrix elementTransformToRoot = this.EditingElementTransformToRoot;
      if (VectorUtilities.IsZero(this.LastTangent))
      {
        pathGeometryEditor.CloseFigureWithLineSegment(pathEditContext.FigureIndex);
      }
      else
      {
        Point q = this.GetLastPoint(pathEditContext.FigureIndex) + this.LastTangent;
        pathGeometryEditor.CloseFigureWithCubicBezier(q, point, pathEditContext.FigureIndex);
      }
      this.Initialize(elementTransformToRoot.Transform(point), true, elementTransformToRoot);
      base.OnBegin(pathEditContext, mouseDevice);
    }
  }
}
