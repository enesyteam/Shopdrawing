// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Path.MotionPathAdornerSetCreator
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Path
{
  internal sealed class MotionPathAdornerSetCreator : IAdornerSetCreator
  {
    public IAdornerSet CreateAdornerSet(ToolBehaviorContext toolContext, SceneElement adornedElement)
    {
      return (IAdornerSet) new MotionPathAdornerSetCreator.MotionPathAdornerSet(toolContext, (BaseFrameworkElement) adornedElement);
    }

    private sealed class MotionPathAdornerSet : AdornerSet
    {
      public MotionPathAdornerSet(ToolBehaviorContext toolContext, BaseFrameworkElement adornedElement)
        : base(toolContext, (SceneElement) adornedElement)
      {
      }

      public override Matrix GetTransformMatrix(IViewObject targetViewObject)
      {
        return MotionPathAdorner.ComputeBaseValueMatrix(this.Element, targetViewObject);
      }

      public override Matrix GetTransformMatrixToAdornerLayer()
      {
        return MotionPathAdorner.RefineTransformToAdornerLayer(this.Element, base.GetTransformMatrixToAdornerLayer());
      }

      protected override void CreateAdorners()
      {
        this.AddAdorner((Adorner) new MotionPathAdorner((AdornerSet) this));
      }
    }
  }
}
