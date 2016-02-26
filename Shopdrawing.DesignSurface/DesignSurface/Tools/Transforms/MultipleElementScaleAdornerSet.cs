// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.MultipleElementScaleAdornerSet
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  internal class MultipleElementScaleAdornerSet : AdornerSet
  {
    public override ToolBehavior Behavior
    {
      get
      {
        return (ToolBehavior) new MultipleElementScaleBehavior(this.ToolContext);
      }
    }

    public MultipleElementScaleAdornerSet(ToolBehaviorContext toolContext, AdornerElementSet adornedElement)
      : base(toolContext, adornedElement)
    {
    }

    protected override void CreateAdorners()
    {
      this.AddAdorner((Adorner) new SizeAdorner((AdornerSet) this, EdgeFlags.TopLeft));
      this.AddAdorner((Adorner) new SizeAdorner((AdornerSet) this, EdgeFlags.Top));
      this.AddAdorner((Adorner) new SizeAdorner((AdornerSet) this, EdgeFlags.TopRight));
      this.AddAdorner((Adorner) new SizeAdorner((AdornerSet) this, EdgeFlags.Left));
      this.AddAdorner((Adorner) new SizeAdorner((AdornerSet) this, EdgeFlags.Right));
      this.AddAdorner((Adorner) new SizeAdorner((AdornerSet) this, EdgeFlags.BottomLeft));
      this.AddAdorner((Adorner) new SizeAdorner((AdornerSet) this, EdgeFlags.Bottom));
      this.AddAdorner((Adorner) new SizeAdorner((AdornerSet) this, EdgeFlags.BottomRight));
    }

    public override Cursor GetCursor(IAdorner adorner)
    {
      return ToolCursors.ResizeCursor.GetCursor(((AnchorPointAdorner) adorner).NormalDirection);
    }
  }
}
