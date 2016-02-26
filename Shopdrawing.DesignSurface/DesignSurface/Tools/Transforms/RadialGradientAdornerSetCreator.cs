// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.RadialGradientAdornerSetCreator
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  internal sealed class RadialGradientAdornerSetCreator : IAdornerSetCreator
  {
    public IAdornerSet CreateAdornerSet(ToolBehaviorContext toolContext, SceneElement adornedElement)
    {
      return (IAdornerSet) new RadialGradientAdornerSetCreator.RadialGradientAdornerSet(toolContext, adornedElement);
    }

    private sealed class RadialGradientAdornerSet : BrushTransformAdornerSet
    {
      public override ToolBehavior Behavior
      {
        get
        {
          return (ToolBehavior) new RadialGradientBehavior(this.ToolContext);
        }
      }

      public RadialGradientAdornerSet(ToolBehaviorContext toolContext, SceneElement adornedElement)
        : base(toolContext, adornedElement)
      {
      }

      protected override void CreateAdorners()
      {
        this.AddAdorner((Adorner) new RadialGradientAdorner((BrushTransformAdornerSet) this, RadialGradientAdornerKind.GradientOriginPoint));
        this.AddAdorner((Adorner) new RadialGradientAdorner((BrushTransformAdornerSet) this, RadialGradientAdornerKind.RadiusPoint));
      }

      public override Cursor GetCursor(IAdorner adorner)
      {
        return Cursors.Hand;
      }
    }
  }
}
