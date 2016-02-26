// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.BrushTranslateAdornerSetCreator
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  internal sealed class BrushTranslateAdornerSetCreator : IAdornerSetCreator
  {
    public IAdornerSet CreateAdornerSet(ToolBehaviorContext toolContext, SceneElement adornedElement)
    {
      return (IAdornerSet) new BrushTranslateAdornerSetCreator.BrushTranslateAdornerSet(toolContext, adornedElement);
    }

    private sealed class BrushTranslateAdornerSet : BrushTransformAdornerSet
    {
      public override ToolBehavior Behavior
      {
        get
        {
          return (ToolBehavior) new BrushTranslateBehavior(this.ToolContext);
        }
      }

      public BrushTranslateAdornerSet(ToolBehaviorContext toolContext, SceneElement adornedElement)
        : base(toolContext, adornedElement)
      {
      }

      protected override void CreateAdorners()
      {
        BrushTranslateAdorner translateAdorner = new BrushTranslateAdorner((BrushTransformAdornerSet) this);
        if (this.Behavior.Tool is GradientBrushTool && PlatformTypes.IsInstance(translateAdorner.PlatformBrush, PlatformTypes.RadialGradientBrush, (ITypeResolver) this.Element.ProjectContext))
          this.AddAdorner((Adorner) new RadialGradientBrushTranslateAdorner((BrushTransformAdornerSet) this));
        this.AddAdorner((Adorner) translateAdorner);
      }

      public override Cursor GetCursor(IAdorner adorner)
      {
        if (this.ToolContext.Tool is GradientBrushTool && adorner is BrushTranslateAdorner)
          return ToolCursors.AddArrowCursor;
        return Cursors.SizeAll;
      }
    }
  }
}
