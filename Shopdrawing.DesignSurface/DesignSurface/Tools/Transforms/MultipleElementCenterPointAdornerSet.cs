// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.MultipleElementCenterPointAdornerSet
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  internal sealed class MultipleElementCenterPointAdornerSet : AdornerSet
  {
    public override ToolBehavior Behavior
    {
      get
      {
        return (ToolBehavior) new MultipleElementCenterEditBehavior(this.ToolContext);
      }
    }

    public MultipleElementCenterPointAdornerSet(ToolBehaviorContext toolContext, AdornerElementSet adornedElementSet)
      : base(toolContext, adornedElementSet)
    {
    }

    protected override void CreateAdorners()
    {
      this.AddAdorner((Adorner) new CenterPointAdorner((AdornerSet) this));
    }

    public override Cursor GetCursor(IAdorner adorner)
    {
      return ToolCursors.CenterPointCursor;
    }
  }
}
