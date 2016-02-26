// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.DimmedGridRowColumnAdornerSet
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  internal class DimmedGridRowColumnAdornerSet : GridRowColumnAdornerSetBase
  {
    public DimmedGridRowColumnAdornerSet(ToolBehaviorContext toolContext, BaseFrameworkElement adornedElement)
      : base(toolContext, adornedElement)
    {
    }

    protected override void CreateAdorners()
    {
      this.CacheRowColumnCounts();
      for (int index = 1; index < this.Columns; ++index)
        this.AddAdorner((Adorner) new LayoutLineAdorner((AdornerSet) this, true, index, true));
      for (int index = 1; index < this.Rows; ++index)
        this.AddAdorner((Adorner) new LayoutLineAdorner((AdornerSet) this, false, index, true));
    }
  }
}
