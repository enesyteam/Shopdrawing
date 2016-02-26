// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.SnapLineRenderer
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.Tools
{
  internal sealed class SnapLineRenderer
  {
    private ToolBehaviorContext toolContext;
    private SnapLineAdornerSet snapLineAdornerSet;

    public bool IsAttached
    {
      get
      {
        return this.snapLineAdornerSet != null;
      }
    }

    public void Attach(ToolBehaviorContext toolContext, SceneElement containerElement)
    {
      this.CheckAttached(false);
      this.toolContext = toolContext;
      this.snapLineAdornerSet = new SnapLineAdornerSet(this.toolContext, containerElement);
      if (this.toolContext.View == null)
        return;
      this.toolContext.View.AdornerLayer.Add((IAdornerSet) this.snapLineAdornerSet);
    }

    public void Detach()
    {
      this.CheckAttached(true);
      if (this.toolContext.View != null)
        this.toolContext.View.AdornerLayer.Remove((IAdornerSet) this.snapLineAdornerSet);
      this.toolContext = (ToolBehaviorContext) null;
      this.snapLineAdornerSet = (SnapLineAdornerSet) null;
    }

    public void ReplaceSnapLines(SceneElement targetElement, Rect targetBounds, List<SnapLine> snapLines)
    {
      this.CheckAttached(true);
      this.snapLineAdornerSet.ReplaceSnapLines(targetElement, targetBounds, snapLines);
    }

    public void UpdateTargetBounds(Rect targetBounds)
    {
      if (!this.IsAttached)
        return;
      this.snapLineAdornerSet.UpdateTargetBounds(targetBounds);
    }

    private void CheckAttached(bool shouldBeAttached)
    {
      if (this.IsAttached != shouldBeAttached)
        throw new InvalidOperationException(ExceptionStringTable.SnapLineRendererIsInWrongAttachedDetachedStateForOperation);
    }
  }
}
