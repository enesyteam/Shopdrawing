// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.SpotLightConeAdornerSet3D
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Tools.Transforms;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.Tools
{
  internal class SpotLightConeAdornerSet3D : AdornerSet3D
  {
    public SpotLightConeAdornerSet3D(ToolBehaviorContext toolContext, Base3DElement adornedElement)
      : base(toolContext, adornedElement)
    {
      this.CreateAdorners();
    }

    protected override void CreateAdorners()
    {
      this.AddAdorner((Adorner3D) new SpotLightConeAdorner3D((AdornerSet3D) this, SpotLightAdornerBehavior3D.TypeOfConeAngle.InnerConeAngle));
      this.AddAdorner((Adorner3D) new SpotLightConeAdorner3D((AdornerSet3D) this, SpotLightAdornerBehavior3D.TypeOfConeAngle.OuterConeAngle));
    }
  }
}
