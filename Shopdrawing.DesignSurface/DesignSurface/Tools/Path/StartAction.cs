// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Path.StartAction
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface.Geometry.Core;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Path
{
  internal sealed class StartAction : TangentUpdateAction
  {
    private PenCreateBehavior penCreateBehavior;

    public override string ActionString
    {
      get
      {
        return StringTable.UndoUnitCreatePath;
      }
    }

    public override Cursor HoverCursor
    {
      get
      {
        return ToolCursors.PenStartCursor;
      }
    }

    public override bool SetPathActive
    {
      get
      {
        return true;
      }
    }

    public StartAction(PenCreateBehavior penCreateBehavior, PathEditorTarget pathEditorTarget, SceneViewModel viewModel)
      : base(pathEditorTarget, viewModel)
    {
      this.penCreateBehavior = penCreateBehavior;
    }

    protected override void OnBegin(PathEditContext pathEditContext, MouseDevice mouseDevice)
    {
      this.EnsureEditTransaction();
      PathElement pathElement = this.penCreateBehavior.CreatePathElement();
      this.UpdateEditTransaction();
      IViewObject element = (IViewObject) null;
      if (pathElement.IsViewObjectValid)
        element = ((IViewVisual) pathElement.ViewObject).VisualParent;
      if (element != null)
      {
        this.PathEditorTarget = this.penCreateBehavior.Tool.PathEditorTargetMap.GetPathEditorTarget((Base2DElement) pathElement, PathEditMode.ScenePath);
        this.PathEditContext = new PathEditContext(0, 0);
        Point viewRootCoordinates = this.GetPointInViewRootCoordinates(mouseDevice, true);
        Matrix computedTransformToRoot = this.View.GetComputedTransformToRoot((SceneElement) pathElement);
        this.Initialize(viewRootCoordinates, false, computedTransformToRoot);
        this.PathEditorTarget.BeginEditing();
        new PathGeometryEditor(this.Path, this.PathEditorTarget.PathDiffChangeList).StartFigure(this.View.GetComputedTransformFromRoot(element).Transform(viewRootCoordinates));
        this.PathEditorTarget.EndEditing(true);
        this.UpdateEditTransaction();
        base.OnBegin(pathEditContext, mouseDevice);
      }
      else
        this.End();
    }
  }
}
