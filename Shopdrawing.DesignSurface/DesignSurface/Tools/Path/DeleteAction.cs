// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Path.DeleteAction
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Geometry.Core;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.Tools.Path
{
  internal abstract class DeleteAction : PenAction
  {
    public DeleteAction(PathEditorTarget pathEditorTarget, SceneViewModel viewModel)
      : base(pathEditorTarget, viewModel)
    {
    }

    protected abstract void DoPreDelete();

    protected abstract void DoDelete(PathEditContext pathEditContext);

    protected override void OnBegin(PathEditContext pathEditContext, MouseDevice mouseDevice)
    {
      this.DoPreDelete();
      if (!PathFigureUtilities.IsIsolatedPoint(pathEditContext.GetPathFigure(this.Path)))
      {
        this.DoDelete(pathEditContext);
        if (this.PathEditorTarget.PathEditMode == PathEditMode.MotionPath)
          this.DeleteIsolatedPoint(pathEditContext);
      }
      else
        this.DeleteIsolatedPoint(pathEditContext);
      base.OnBegin(pathEditContext, mouseDevice);
      this.End();
    }

    private void DeleteIsolatedPoint(PathEditContext pathEditContext)
    {
      if (!PathFigureUtilities.IsIsolatedPoint(pathEditContext.GetPathFigure(this.Path)))
        return;
      if (this.Path.Figures.Count > 1)
        this.BeginEditing().RemoveFigure(pathEditContext.FigureIndex);
      else
        this.RemovePath();
    }
  }
}
