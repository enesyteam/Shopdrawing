// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Path.DeletePointAction
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.Tools.Path
{
  internal sealed class DeletePointAction : DeleteAction
  {
    public override string ActionString
    {
      get
      {
        return StringTable.UndoUnitDeletePoint;
      }
    }

    public override Cursor HoverCursor
    {
      get
      {
        return ToolCursors.PenDeleteCursor;
      }
    }

    public override Cursor DragCursor
    {
      get
      {
        return ToolCursors.PenDeleteCursor;
      }
    }

    public DeletePointAction(PathEditorTarget pathEditorTarget, SceneViewModel viewModel)
      : base(pathEditorTarget, viewModel)
    {
    }

    protected override void DoPreDelete()
    {
      this.ViewModel.PathPartSelectionSet.RemoveSelection(this.ViewModel.PathPartSelectionSet.GetSelectionByElement(this.EditingElement, this.PathEditorTarget.PathEditMode));
    }

    protected override void DoDelete(PathEditContext pathEditContext)
    {
      this.BeginEditing().RemovePoint(pathEditContext.FigureIndex, pathEditContext.PartIndex);
    }
  }
}
