// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.BrushScaleAdornerSetCreator
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  internal sealed class BrushScaleAdornerSetCreator : IAdornerSetCreator
  {
    public IAdornerSet CreateAdornerSet(ToolBehaviorContext toolContext, SceneElement adornedElement)
    {
      return (IAdornerSet) new BrushScaleAdornerSetCreator.BrushScaleAdornerSet(toolContext, adornedElement);
    }

    private sealed class BrushScaleAdornerSet : BrushTransformAdornerSet
    {
      public override ToolBehavior Behavior
      {
        get
        {
          return (ToolBehavior) new BrushScaleBehavior(this.ToolContext);
        }
      }

      public BrushScaleAdornerSet(ToolBehaviorContext toolContext, SceneElement adornedElement)
        : base(toolContext, adornedElement)
      {
      }

      protected override void CreateAdorners()
      {
        this.AddAdorner((Adorner) new BrushScaleAdorner((BrushTransformAdornerSet) this, EdgeFlags.TopLeft));
        this.AddAdorner((Adorner) new BrushScaleAdorner((BrushTransformAdornerSet) this, EdgeFlags.Top));
        this.AddAdorner((Adorner) new BrushScaleAdorner((BrushTransformAdornerSet) this, EdgeFlags.TopRight));
        this.AddAdorner((Adorner) new BrushScaleAdorner((BrushTransformAdornerSet) this, EdgeFlags.Left));
        this.AddAdorner((Adorner) new BrushScaleAdorner((BrushTransformAdornerSet) this, EdgeFlags.Right));
        this.AddAdorner((Adorner) new BrushScaleAdorner((BrushTransformAdornerSet) this, EdgeFlags.BottomLeft));
        this.AddAdorner((Adorner) new BrushScaleAdorner((BrushTransformAdornerSet) this, EdgeFlags.Bottom));
        this.AddAdorner((Adorner) new BrushScaleAdorner((BrushTransformAdornerSet) this, EdgeFlags.BottomRight));
      }

      public override Cursor GetCursor(IAdorner adorner)
      {
        return ToolCursors.ResizeCursor.GetCursor(((BrushAnchorPointAdorner) adorner).NormalDirection);
      }
    }
  }
}
