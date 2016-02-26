// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Path.ExtendAction
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
  internal sealed class ExtendAction : TangentUpdateAction
  {
    public override string ActionString
    {
      get
      {
        return StringTable.UndoUnitExtendAdjustEndpoint;
      }
    }

    public override Cursor HoverCursor
    {
      get
      {
        return ToolCursors.PenExtendCursor;
      }
    }

    public override bool SetPathActive
    {
      get
      {
        return true;
      }
    }

    public ExtendAction(PathEditorTarget pathEditorTarget, SceneViewModel viewModel)
      : base(pathEditorTarget, viewModel)
    {
    }

    protected override void OnBegin(PathEditContext pathEditContext, MouseDevice mouseDevice)
    {
      this.View.ViewModel.PathPartSelectionSet.SetSelection((PathPart) new PathPoint((SceneElement) this.PathEditorTarget.EditingElement, this.PathEditorTarget.PathEditMode, pathEditContext.FigureIndex, pathEditContext.PartIndex));
      Matrix elementTransformToRoot = this.EditingElementTransformToRoot;
      Point point = PathFigureUtilities.GetPoint(pathEditContext.GetPathFigure(this.Path), pathEditContext.PartIndex);
      this.Initialize(elementTransformToRoot.Transform(point), false, elementTransformToRoot);
      if (pathEditContext.PartIndex == 0)
        this.BeginEditing().Reverse();
      base.OnBegin(pathEditContext, mouseDevice);
    }
  }
}
