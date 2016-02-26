// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.SkewAdornerSetCreator
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  internal sealed class SkewAdornerSetCreator : IAdornerSetCreator
  {
    public IAdornerSet CreateAdornerSet(ToolBehaviorContext toolContext, SceneElement adornedElement)
    {
      return (IAdornerSet) new SkewAdornerSetCreator.SkewAdornerSet(toolContext, (BaseFrameworkElement) adornedElement);
    }

    private sealed class SkewAdornerSet : AdornerSet
    {
      public override ToolBehavior Behavior
      {
        get
        {
          return (ToolBehavior) new SkewBehavior(this.ToolContext);
        }
      }

      public SkewAdornerSet(ToolBehaviorContext toolContext, BaseFrameworkElement adornedElement)
        : base(toolContext, (SceneElement) adornedElement)
      {
      }

      protected override void CreateAdorners()
      {
        this.AddAdorner((Adorner) new RotateAdorner((AdornerSet) this, EdgeFlags.Top));
        this.AddAdorner((Adorner) new RotateAdorner((AdornerSet) this, EdgeFlags.Right));
        this.AddAdorner((Adorner) new RotateAdorner((AdornerSet) this, EdgeFlags.Bottom));
        this.AddAdorner((Adorner) new RotateAdorner((AdornerSet) this, EdgeFlags.Left));
      }

      public override Cursor GetCursor(IAdorner adorner)
      {
        return ToolCursors.SkewCursor.GetCursor(((AnchorPointAdorner) adorner).EdgeDirection);
      }
    }
  }
}
