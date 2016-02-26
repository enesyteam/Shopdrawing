// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.ElementLayoutAdornerSet
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Designers;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  internal class ElementLayoutAdornerSet : AdornerSet
  {
    public override ToolBehavior Behavior
    {
      get
      {
        return (ToolBehavior) new ElementLayoutBehavior(this.ToolContext);
      }
    }

    public ElementLayoutAdornerSet(ToolBehaviorContext toolContext, BaseFrameworkElement adornedElement)
      : base(toolContext, (SceneElement) adornedElement)
    {
    }

    protected override void CreateAdorners()
    {
      BaseFrameworkElement child = this.Element as BaseFrameworkElement;
      BaseFrameworkElement frameworkElement = this.Element.ParentElement as BaseFrameworkElement;
      if (child == null || frameworkElement == null || !frameworkElement.IsAttached)
        return;
      ILayoutDesigner designerForChild = this.ViewModel.GetLayoutDesignerForChild(this.Element, true);
      if ((designerForChild.GetWidthConstraintMode(child) & LayoutConstraintMode.CanvasLike) == LayoutConstraintMode.NonOverlappingGridlike)
      {
        this.AddAdorner((Adorner) new ElementLayoutLineAdorner((AdornerSet) this, ElementLayoutAdornerType.Left));
        this.AddAdorner((Adorner) new ElementLayoutLineAdorner((AdornerSet) this, ElementLayoutAdornerType.Right));
        this.AddAdorner((Adorner) new ElementLayoutLockAdorner((AdornerSet) this, ElementLayoutAdornerType.Left));
        this.AddAdorner((Adorner) new ElementLayoutLockAdorner((AdornerSet) this, ElementLayoutAdornerType.Right));
      }
      if ((designerForChild.GetHeightConstraintMode(child) & LayoutConstraintMode.CanvasLike) != LayoutConstraintMode.NonOverlappingGridlike)
        return;
      this.AddAdorner((Adorner) new ElementLayoutLineAdorner((AdornerSet) this, ElementLayoutAdornerType.Top));
      this.AddAdorner((Adorner) new ElementLayoutLineAdorner((AdornerSet) this, ElementLayoutAdornerType.Bottom));
      this.AddAdorner((Adorner) new ElementLayoutLockAdorner((AdornerSet) this, ElementLayoutAdornerType.Top));
      this.AddAdorner((Adorner) new ElementLayoutLockAdorner((AdornerSet) this, ElementLayoutAdornerType.Bottom));
    }

    public override Cursor GetCursor(IAdorner adorner)
    {
      return Cursors.Hand;
    }
  }
}
