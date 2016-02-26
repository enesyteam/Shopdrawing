// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.LinearGradientAdornerSetCreator
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Windows;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  internal sealed class LinearGradientAdornerSetCreator : IAdornerSetCreator
  {
    public IAdornerSet CreateAdornerSet(ToolBehaviorContext toolContext, SceneElement adornedElement)
    {
      return (IAdornerSet) new LinearGradientAdornerSetCreator.LinearGradientAdornerSet(toolContext, adornedElement);
    }

    private sealed class LinearGradientAdornerSet : BrushTransformAdornerSet
    {
      public override ToolBehavior Behavior
      {
        get
        {
          return (ToolBehavior) new LinearGradientBehavior(this.ToolContext);
        }
      }

      public LinearGradientAdornerSet(ToolBehaviorContext toolContext, SceneElement adornedElement)
        : base(toolContext, adornedElement)
      {
      }

      protected override void CreateAdorners()
      {
        this.AddAdorner((Adorner) new LinearGradientAdorner((BrushTransformAdornerSet) this, LinearGradientAdornerKind.StartRotation));
        this.AddAdorner((Adorner) new LinearGradientAdorner((BrushTransformAdornerSet) this, LinearGradientAdornerKind.EndRotation));
        this.AddAdorner((Adorner) new LinearGradientAdorner((BrushTransformAdornerSet) this, LinearGradientAdornerKind.StartPoint));
        this.AddAdorner((Adorner) new LinearGradientAdorner((BrushTransformAdornerSet) this, LinearGradientAdornerKind.EndPoint));
      }

      public override Cursor GetCursor(IAdorner adorner)
      {
        LinearGradientAdorner linearGradientAdorner = (LinearGradientAdorner) adorner;
        if (linearGradientAdorner.Kind == LinearGradientAdornerKind.StartPoint || linearGradientAdorner.Kind == LinearGradientAdornerKind.EndPoint)
          return Cursors.Hand;
        Point point = linearGradientAdorner.StartPoint * linearGradientAdorner.AdornerSet.Matrix;
        Vector direction = linearGradientAdorner.EndPoint * linearGradientAdorner.AdornerSet.Matrix - point;
        if (linearGradientAdorner.Kind == LinearGradientAdornerKind.StartRotation)
          direction = -direction;
        return ToolCursors.RotateCursor.GetCursor(direction);
      }
    }
  }
}
