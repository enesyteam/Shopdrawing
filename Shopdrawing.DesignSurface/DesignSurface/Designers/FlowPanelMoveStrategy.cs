// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Designers.FlowPanelMoveStrategy
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface.Tools.Layout;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.Designers
{
  internal class FlowPanelMoveStrategy : BaseFlowMoveStrategy
  {
    public FlowPanelMoveStrategy(MoveStrategyContext context)
      : base(context, (BaseFlowInsertionPoint) new FlowPanelInsertionPoint())
    {
    }

    protected override void AdjustIndexBeforeRemovingFromSceneView()
    {
      int offsetBeforeRemove = this.ComputeIndexOffsetBeforeRemove(this.InsertionPoint.Index);
      if (offsetBeforeRemove == 0)
        return;
      this.InsertionPoint.Index += offsetBeforeRemove;
    }

    protected override IPropertyId GetInsertionPointChildProperty()
    {
      IPropertyId propertyId = (IPropertyId) null;
      if (this.InsertionPoint.Element is PanelElement)
        propertyId = PanelElement.ChildrenProperty;
      else if (PlatformTypes.ItemsControl.IsAssignableFrom((ITypeId) this.InsertionPoint.Element.Type))
        propertyId = ItemsControlElement.ItemsProperty;
      return propertyId;
    }

    protected override void UpdateInsertionPoint()
    {
      IViewPanel viewPanel = MoveStrategy.GetContainerHost((SceneElement) this.LayoutContainer) as IViewPanel;
      this.ClearAdorner();
      this.InsertionPoint.Element = this.LayoutContainer;
      bool isCursorAtEnd = true;
      this.InsertionPoint.Index = FlowPanelLayoutUtilities.GetInsertionIndex((SceneElement) this.LayoutContainer, this.ActiveView.TransformPoint((IViewObject) this.ActiveView.HitTestRoot, this.LayoutContainer.Visual, this.DragCurrentPosition), out isCursorAtEnd);
      this.InsertionPoint.IsCursorAtEnd = isCursorAtEnd;
      this.AdornerSet = (AdornerSet) new FlowPanelInsertionPointAdornerSet(this.ToolContext, this.LayoutContainer, (FlowPanelInsertionPoint) this.InsertionPoint);
      this.ActiveView.AdornerLayer.Add((IAdornerSet) this.AdornerSet);
    }
  }
}
