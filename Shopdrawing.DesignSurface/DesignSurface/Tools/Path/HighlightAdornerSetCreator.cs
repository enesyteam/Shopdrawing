// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Path.HighlightAdornerSetCreator
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.Tools.Transforms;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.Tools.Path
{
  internal class HighlightAdornerSetCreator : IMultipleElementAdornerSetCreator, IAdornerSetCreator
  {
    public IAdornerSet CreateAdornerSet(ToolBehaviorContext toolContext, AdornerElementSet adornedElementSet)
    {
      return (IAdornerSet) new HighlightAdornerSetCreator.HighlightAdornerSet(toolContext, adornedElementSet);
    }

    public IAdornerSet CreateAdornerSet(ToolBehaviorContext toolContext, SceneElement adornedElement)
    {
      return (IAdornerSet) new HighlightAdornerSetCreator.HighlightAdornerSet(toolContext, toolContext.View.Artboard.AdornerLayer.CreateOrGetAdornerElementSetForElement(adornedElement));
    }

    private class HighlightAdornerSet : AdornerSet
    {
      private SelectionAdornerUsages adornerUsage;

      public override ToolBehavior Behavior
      {
        get
        {
          if ((this.adornerUsage & SelectionAdornerUsages.UseBoundingBox) != SelectionAdornerUsages.None)
            return (ToolBehavior) new RelocateBehavior(this.ToolContext);
          return base.Behavior;
        }
      }

      public HighlightAdornerSet(ToolBehaviorContext toolContext, AdornerElementSet adornedElement)
        : base(toolContext, adornedElement)
      {
        this.adornerUsage = this.ToolContext.Tool.AdornerOwnerTool.GetSelectionAdornerUsages(this.Element);
      }

      protected override void CreateAdorners()
      {
        if ((this.adornerUsage & SelectionAdornerUsages.ShowBoundingBox) != SelectionAdornerUsages.None && this.Element is BaseFrameworkElement)
          this.AddAdorner((Adorner) new MoveAdorner((AdornerSet) this));
        if ((this.adornerUsage & SelectionAdornerUsages.ShowGeometry) != SelectionAdornerUsages.None && this.Element is ShapeElement)
          this.AddAdorner((Adorner) new PathAdorner((AdornerSet) this, PathEditMode.ScenePath));
        if ((this.adornerUsage & SelectionAdornerUsages.ShowClipping) != SelectionAdornerUsages.None && this.Element is BaseFrameworkElement)
          this.AddAdorner((Adorner) new PathAdorner((AdornerSet) this, PathEditMode.ClippingPath));
        if ((this.adornerUsage & SelectionAdornerUsages.ShowMotionPath) == SelectionAdornerUsages.None || !(this.Element is BaseFrameworkElement))
          return;
        this.AddAdorner((Adorner) new PathAdorner((AdornerSet) this, PathEditMode.MotionPath));
      }

      public override Cursor GetCursor(IAdorner adorner)
      {
        if ((this.adornerUsage & SelectionAdornerUsages.UseBoundingBox) == SelectionAdornerUsages.None)
          return (Cursor) null;
        if (!(this.ToolContext.Tool is SubselectionTool))
          return ToolCursors.RelocateCursor;
        return ToolCursors.SubselectMoveCursor;
      }
    }
  }
}
