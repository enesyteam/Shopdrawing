// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Designers.GenericMoveStrategy
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.Designers
{
  internal class GenericMoveStrategy : InplaceMoveStrategy
  {
    public GenericMoveStrategy(MoveStrategyContext context)
      : base(context)
    {
    }

    protected override void OnBeginDrag()
    {
      if (this.DraggedElements.Count > MoveStrategy.GetMaxChildrenForElement((SceneElement) this.LayoutContainer, (IProperty) null))
        throw new InvalidOperationException(ExceptionStringTable.BehaviorCannotHandleMultipleElements);
      this.PrepareDraggedElementsForDragging();
    }

    protected override void OnContinueDrag(BaseFrameworkElement hitElement)
    {
      this.MoveDraggedElementsWithTempTransform();
    }

    protected override bool OnEndDrag(bool commit)
    {
      this.RestoreDraggedElements();
      if (!commit)
        return false;
      HashSet<BaseFrameworkElement> hashSet = new HashSet<BaseFrameworkElement>();
      foreach (BaseFrameworkElement frameworkElement in this.DraggedElements)
      {
        if (frameworkElement.Parent != this.LayoutContainer)
        {
          frameworkElement.Remove();
          hashSet.Add(frameworkElement);
          new PropertySceneInsertionPoint((SceneElement) this.LayoutContainer, this.LayoutContainer.DefaultContentProperty).Insert((SceneNode) frameworkElement);
        }
      }
      this.Context.Transaction.UpdateEditTransaction();
      this.AdjustLayoutAfterReparenting((ICollection<BaseFrameworkElement>) hashSet);
      return true;
    }
  }
}
