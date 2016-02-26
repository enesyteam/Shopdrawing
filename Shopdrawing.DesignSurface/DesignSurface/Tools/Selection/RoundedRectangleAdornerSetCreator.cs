// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Selection.RoundedRectangleAdornerSetCreator
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.Tools.Selection
{
  internal sealed class RoundedRectangleAdornerSetCreator : IAdornerSetCreator
  {
    public IAdornerSet CreateAdornerSet(ToolBehaviorContext toolContext, SceneElement adornedElement)
    {
      return (IAdornerSet) new RoundedRectangleAdornerSetCreator.RoundedRectangleAdornerSet(toolContext, (BaseFrameworkElement) adornedElement);
    }

    private class RoundedRectangleAdornerSet : AdornerSet
    {
      public override ToolBehavior Behavior
      {
        get
        {
          return (ToolBehavior) new RoundedRectangleBehavior(this.ToolContext);
        }
      }

      public RoundedRectangleAdornerSet(ToolBehaviorContext toolContext, BaseFrameworkElement adornedElement)
        : base(toolContext, (SceneElement) adornedElement)
      {
      }

      protected override void CreateAdorners()
      {
        this.AddAdorner((Adorner) new RoundedRectangleAdorner((AdornerSet) this, true));
        this.AddAdorner((Adorner) new RoundedRectangleAdorner((AdornerSet) this, false));
      }

      public override Cursor GetCursor(IAdorner adorner)
      {
        return ToolCursors.RoundedRectangleCursor;
      }
    }
  }
}
