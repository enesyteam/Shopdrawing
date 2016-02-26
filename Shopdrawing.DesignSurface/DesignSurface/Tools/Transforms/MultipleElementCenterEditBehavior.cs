// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.MultipleElementCenterEditBehavior
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Commands.Undo;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  internal sealed class MultipleElementCenterEditBehavior : CenterEditBehavior
  {
    private Point initialRenderTransformOrigin;

    public MultipleElementCenterEditBehavior(ToolBehaviorContext toolContext)
      : base(toolContext)
    {
    }

    protected override void UpdateCenterPoint(Point centerPoint)
    {
      Rect elementBounds = this.EditingElementSet.ElementBounds;
      Point renderTransformOrigin = this.EditingElementSet.RenderTransformOrigin;
      Point point2 = new Point((centerPoint.X - elementBounds.Left) / (elementBounds.Width == 0.0 ? 1.0 : elementBounds.Width), (centerPoint.Y - elementBounds.Top) / (elementBounds.Height == 0.0 ? 1.0 : elementBounds.Height));
      point2 = RoundingHelper.RoundPosition(point2);
      if (Point.Equals(renderTransformOrigin, point2))
        return;
      this.EditingElementSet.RenderTransformOrigin = point2;
    }

    protected override void Begin()
    {
      this.initialRenderTransformOrigin = this.EditingElementSet.RenderTransformOrigin;
    }

    protected override void Finish()
    {
      if (Tolerances.AreClose(this.initialRenderTransformOrigin, this.EditingElementSet.RenderTransformOrigin))
        return;
      this.EnsureEditTransaction();
      this.EditingElement.ViewModel.Document.AddUndoUnit((IUndoUnit) new MultipleElementCenterEditUndoUnit(this.EditingElementSet, this.initialRenderTransformOrigin, this.EditingElementSet.RenderTransformOrigin));
    }
  }
}
