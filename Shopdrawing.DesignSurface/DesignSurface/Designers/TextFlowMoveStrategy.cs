// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Designers.TextFlowMoveStrategy
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects.TextObjects;
using Microsoft.Expression.DesignSurface.Tools.Layout;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Windows;
using System.Windows.Documents;

namespace Microsoft.Expression.DesignSurface.Designers
{
  internal class TextFlowMoveStrategy : BaseFlowMoveStrategy
  {
    private IViewTextPointer insertionPositionToAdjust;

    public TextFlowMoveStrategy(MoveStrategyContext context)
      : base(context, (BaseFlowInsertionPoint) new TextFlowInsertionPoint())
    {
    }

    protected override IPropertyId GetInsertionPointChildProperty()
    {
      IPropertyId propertyId = (IPropertyId) null;
      ITextFlowSceneNode textFlowSceneNode = this.InsertionPoint.Element as ITextFlowSceneNode;
      if (textFlowSceneNode != null)
        propertyId = textFlowSceneNode.TextChildProperty;
      return propertyId;
    }

    protected override void AdjustIndexBeforeRemovingFromSceneView()
    {
      ITextFlowSceneNode textFlowSceneNode = this.InsertionPoint.Element as ITextFlowSceneNode;
      if (textFlowSceneNode == null || textFlowSceneNode.ContentStart == null)
        return;
      this.insertionPositionToAdjust = textFlowSceneNode.ContentStart.GetPositionAtOffset(this.InsertionPoint.Index);
    }

    protected override void AdjustIndexAfterRemovingFromSceneView()
    {
      ITextFlowSceneNode textFlowSceneNode = this.InsertionPoint.Element as ITextFlowSceneNode;
      if (textFlowSceneNode != null && textFlowSceneNode.ContentStart != null && (this.insertionPositionToAdjust != null && this.insertionPositionToAdjust.IsInSameDocument(textFlowSceneNode.ContentStart)))
        this.InsertionPoint.Index = textFlowSceneNode.ContentStart.GetOffsetToPosition(this.insertionPositionToAdjust);
      this.insertionPositionToAdjust = (IViewTextPointer) null;
    }

    protected override void UpdateInsertionPoint()
    {
      this.ClearAdorner();
      this.InsertionPoint.Element = this.LayoutContainer;
      ITextFlowSceneNode textFlowSceneNode = this.InsertionPoint.Element as ITextFlowSceneNode;
      Point mousePosition = this.InsertionPoint.Element.ViewModel.DefaultView.GetMousePosition(this.Pointer, MoveStrategy.GetContainerHost((SceneElement) this.InsertionPoint.Element));
      IViewTextPointer positionFromPoint = textFlowSceneNode.GetPositionFromPoint(mousePosition);
      IViewTextPointer position = positionFromPoint != null ? positionFromPoint.GetInsertionPosition(LogicalDirection.Forward) : textFlowSceneNode.ContentEnd.GetNextInsertionPosition(LogicalDirection.Backward);
      this.InsertionPoint.IsCursorAtEnd = false;
      if (textFlowSceneNode.ContentStart == null)
      {
        this.InsertionPoint.IsCursorAtEnd = true;
      }
      else
      {
        this.InsertionPoint.Index = textFlowSceneNode.ContentStart.GetOffsetToPosition(position);
        this.AdornerSet = (AdornerSet) new TextFlowInsertionPointAdornerSet(this.ToolContext, this.LayoutContainer, (TextFlowInsertionPoint) this.InsertionPoint);
        this.ActiveView.AdornerLayer.Add((IAdornerSet) this.AdornerSet);
      }
    }
  }
}
