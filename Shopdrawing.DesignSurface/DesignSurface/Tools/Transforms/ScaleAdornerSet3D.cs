// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.ScaleAdornerSet3D
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  internal class ScaleAdornerSet3D : AdornerSet3D
  {
    public override ToolBehavior Behavior
    {
      get
      {
        return (ToolBehavior) new ScaleBehavior3D(this.ToolContext);
      }
    }

    public ScaleAdornerSet3D(ToolBehaviorContext toolContext, Base3DElement adornedElement)
      : base(toolContext, adornedElement)
    {
      this.DoScaleToScreen = true;
      this.DoRemoveObjectScale = true;
      this.DoCenterOnSpecifiedCenter = true;
      this.Placement = AdornerSet3D.Location.OrthographicLayer;
      this.CreateAdorners();
    }

    protected override void CreateAdorners()
    {
      this.AddAdorner((Adorner3D) new ScaleAdorner3D((AdornerSet3D) this, Adorner3D.TransformVia.XAxis));
      this.AddAdorner((Adorner3D) new ScaleAdorner3D((AdornerSet3D) this, Adorner3D.TransformVia.YAxis));
      this.AddAdorner((Adorner3D) new ScaleAdorner3D((AdornerSet3D) this, Adorner3D.TransformVia.ZAxis));
    }

    public override Cursor GetCursor(IAdorner adorner)
    {
      return ToolCursors.CrosshairCursor;
    }
  }
}
