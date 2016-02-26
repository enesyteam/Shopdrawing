// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.RootElementSizeAdornerSet
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  internal class RootElementSizeAdornerSet : SizeAdornerSet
  {
    public override ToolBehavior Behavior
    {
      get
      {
        return (ToolBehavior) new RootElementSizeBehavior(this.ToolContext);
      }
    }

    public RootElementSizeAdornerSet(ToolBehaviorContext toolContext, BaseFrameworkElement adornedElement)
      : base(toolContext, adornedElement)
    {
    }

    protected override void CreateAdorners()
    {
      this.AddAdorner((Adorner) new SizeAdorner((AdornerSet) this, EdgeFlags.Right));
      this.AddAdorner((Adorner) new SizeAdorner((AdornerSet) this, EdgeFlags.Bottom));
      this.AddAdorner((Adorner) new SizeAdorner((AdornerSet) this, EdgeFlags.BottomRight));
    }
  }
}
