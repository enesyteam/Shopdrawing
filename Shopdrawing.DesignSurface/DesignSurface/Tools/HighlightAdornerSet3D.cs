// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.HighlightAdornerSet3D
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.Tools
{
  internal class HighlightAdornerSet3D : AdornerSet
  {
    public HighlightAdornerSet3D(ToolBehaviorContext toolContext, Base3DElement adornedElement)
      : base(toolContext, (SceneElement) adornedElement)
    {
    }

    protected override void CreateAdorners()
    {
      this.AddAdorner((Adorner) new HighlightAdorner3D((AdornerSet) this));
    }
  }
}
