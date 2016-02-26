// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Designers.BaseFlowMoveStrategy
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.Designers
{
  internal abstract class BaseFlowMoveStrategy : InplaceMoveStrategy
  {
    private BaseFlowInsertionPoint insertionPoint;
    private AdornerSet adornerSet;

    protected BaseFlowInsertionPoint InsertionPoint
    {
      get
      {
        return this.insertionPoint;
      }
      set
      {
        this.insertionPoint = value;
      }
    }

    protected AdornerSet AdornerSet
    {
      get
      {
        return this.adornerSet;
      }
      set
      {
        this.adornerSet = value;
      }
    }

    protected BaseFlowMoveStrategy(MoveStrategyContext context, BaseFlowInsertionPoint insertionPoint)
      : base(context)
    {
      this.insertionPoint = insertionPoint;
    }

    protected override void OnBeginDrag()
    {
      this.PrepareDraggedElementsForDragging();
    }

    protected override void OnContinueDrag(BaseFrameworkElement hitElement)
    {
      this.MoveDraggedElementsWithTempTransform();
      this.UpdateInsertionPoint();
    }

    protected override bool OnEndDrag(bool commit)
    {
      this.ClearAdorner();
      this.RestoreDraggedElements();
      if (commit)
        return this.ReparentDraggedElements();
      this.insertionPoint.Clear();
      return true;
    }

    private bool ReparentDraggedElements()
    {
      if (!this.InsertionPoint.IsEmpty)
      {
        IPropertyId pointChildProperty = this.GetInsertionPointChildProperty();
        if (pointChildProperty != null)
        {
          ISceneNodeCollection<SceneNode> collectionForProperty = this.InsertionPoint.Element.GetCollectionForProperty(pointChildProperty);
          if (collectionForProperty != null)
          {
            this.AdjustIndexBeforeRemovingFromSceneView();
            HashSet<BaseFrameworkElement> hashSet = new HashSet<BaseFrameworkElement>();
            foreach (BaseFrameworkElement frameworkElement in this.DraggedElements)
            {
              hashSet.Add(frameworkElement);
              frameworkElement.Remove();
            }
            this.Context.Transaction.UpdateEditTransaction();
            this.AdjustIndexAfterRemovingFromSceneView();
            foreach (BaseFrameworkElement frameworkElement in this.DraggedElements)
            {
              this.InsertionPoint.Insert(collectionForProperty, (SceneElement) frameworkElement);
              ++this.InsertionPoint.Index;
            }
            this.Context.Transaction.UpdateEditTransaction();
            this.AdjustLayoutAfterReparenting((ICollection<BaseFrameworkElement>) hashSet);
            return true;
          }
        }
      }
      return false;
    }

    protected abstract IPropertyId GetInsertionPointChildProperty();

    protected abstract void UpdateInsertionPoint();

    protected void ClearAdorner()
    {
      if (this.adornerSet == null)
        return;
      this.ActiveView.AdornerLayer.Remove((IAdornerSet) this.adornerSet);
      this.adornerSet = (AdornerSet) null;
    }
  }
}
