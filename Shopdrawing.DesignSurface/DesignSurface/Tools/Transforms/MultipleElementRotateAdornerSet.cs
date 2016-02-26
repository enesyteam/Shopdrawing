// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.MultipleElementRotateAdornerSet
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  internal sealed class MultipleElementRotateAdornerSet : AdornerSet
  {
    public override ToolBehavior Behavior
    {
      get
      {
        return (ToolBehavior) new MultipleElementRotateBehavior(this.ToolContext);
      }
    }

    public MultipleElementRotateAdornerSet(ToolBehaviorContext toolContext, AdornerElementSet adornedElementSet)
      : base(toolContext, adornedElementSet)
    {
    }

    protected override void CreateAdorners()
    {
      this.AddAdorner((Adorner) new RotateAdorner((AdornerSet) this, EdgeFlags.TopLeft));
      this.AddAdorner((Adorner) new RotateAdorner((AdornerSet) this, EdgeFlags.TopRight));
      this.AddAdorner((Adorner) new RotateAdorner((AdornerSet) this, EdgeFlags.BottomLeft));
      this.AddAdorner((Adorner) new RotateAdorner((AdornerSet) this, EdgeFlags.BottomRight));
      this.AddAdorner((Adorner) new RotateBoundingBoxAdorner((AdornerSet) this));
    }

    public override Cursor GetCursor(IAdorner adorner)
    {
      return ToolCursors.RotateCursor.GetCursor(((AnchorPointAdorner) adorner).NormalDirection);
    }
  }
}
