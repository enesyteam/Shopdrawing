// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.SnapLineAdornerSet
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.Generic;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.Tools
{
  internal sealed class SnapLineAdornerSet : AdornerSet
  {
    private SnapLineAdorner snapLineAdorner;

    public SnapLineAdornerSet(ToolBehaviorContext toolContext, SceneElement adornedElement)
      : base(toolContext, adornedElement)
    {
      this.snapLineAdorner = new SnapLineAdorner(this);
    }

    public void ReplaceSnapLines(SceneElement targetElement, Rect targetBounds, List<SnapLine> snapLines)
    {
      this.snapLineAdorner.ReplaceSnapLines(targetElement, targetBounds, snapLines);
    }

    public void UpdateTargetBounds(Rect targetBounds)
    {
      this.snapLineAdorner.UpdateTargetBounds(targetBounds);
    }

    protected override void CreateAdorners()
    {
      this.AddAdorner((Adorner) this.snapLineAdorner);
    }
  }
}
