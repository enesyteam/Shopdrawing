// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Path.TangentDragAction
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Geometry.Core;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Path
{
  internal sealed class TangentDragAction : PenAction
  {
    private Matrix geometryToDocument;
    private Point startRootPoint;
    private bool adjustLastTangent;

    public override string ActionString
    {
      get
      {
        return StringTable.UndoUnitDragTangent;
      }
    }

    public override Cursor HoverCursor
    {
      get
      {
        return ToolCursors.ConvertTangentCursor;
      }
    }

    public override Cursor DragCursor
    {
      get
      {
        return ToolCursors.PenTangentCursor;
      }
    }

    public TangentDragAction(PathEditorTarget pathEditorTarget, SceneViewModel viewModel, bool adjustLastTangent)
      : base(pathEditorTarget, viewModel)
    {
      this.adjustLastTangent = adjustLastTangent;
    }

    protected override void OnBegin(PathEditContext pathEditContext, MouseDevice mouseDevice)
    {
      this.startRootPoint = PathFigureUtilities.GetPoint(this.PathEditorTarget.PathGeometry.Figures[pathEditContext.FigureIndex], pathEditContext.PartIndex, true);
      this.geometryToDocument = this.PathEditorTarget.GetTransformToAncestor((IViewObject) this.View.HitTestRoot);
      base.OnBegin(pathEditContext, mouseDevice);
    }

    protected override void OnDrag(MouseDevice mouseDevice, double zoom)
    {
      Point viewRootCoordinates = this.GetPointInViewRootCoordinates(mouseDevice, true);
      this.PathEditorTarget.BeginEditing();
      Vector correspondingVector = ElementUtilities.GetCorrespondingVector(viewRootCoordinates - this.startRootPoint * this.geometryToDocument, this.geometryToDocument, this.IsShiftDown ? this.AxisConstraint : (AxisConstraint) null);
      PathFigureEditor pathFigureEditor = new PathFigureEditor(this.PathEditContext.GetPathFigure(this.PathEditorTarget.PathGeometry));
      if (!this.adjustLastTangent)
        pathFigureEditor.SetPoint(this.PathEditContext.PartIndex, this.startRootPoint + correspondingVector);
      else
        this.LastTangent = correspondingVector;
      this.View.AdornerLayer.InvalidateAdornerVisuals(this.EditingElement);
      base.OnDrag(mouseDevice, zoom);
    }
  }
}
