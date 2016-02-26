// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.MultipleElementCenterEditUndoUnit
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.Framework.Commands.Undo;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  internal sealed class MultipleElementCenterEditUndoUnit : UndoUnit
  {
    private AdornerElementSet elementSet;
    private Point oldRenderTransformOrigin;
    private Point newRenderTransformOrigin;
    private bool isFirstRedo;

    public MultipleElementCenterEditUndoUnit(AdornerElementSet elementSet, Point oldRenderTransformOrigin, Point newRenderTransformOrigin)
    {
      this.elementSet = elementSet;
      this.oldRenderTransformOrigin = oldRenderTransformOrigin;
      this.newRenderTransformOrigin = newRenderTransformOrigin;
    }

    public override void Undo()
    {
      this.elementSet.RenderTransformOrigin = this.oldRenderTransformOrigin;
      base.Undo();
    }

    public override void Redo()
    {
      if (this.isFirstRedo)
        this.isFirstRedo = false;
      else
        this.elementSet.RenderTransformOrigin = this.newRenderTransformOrigin;
      base.Redo();
    }
  }
}
