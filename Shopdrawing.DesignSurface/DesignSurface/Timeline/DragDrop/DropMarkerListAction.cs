// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Timeline.DragDrop.DropMarkerListAction
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Designers;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.SceneCommands;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.Generic;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.Timeline.DragDrop
{
  public class DropMarkerListAction : DropAction<DocumentNodeMarkerSortedList>
  {
    private Dictionary<SceneNode, LayoutCacheRecord> layoutCache;
    private Rect boundsOfAllElements;

    public DropMarkerListAction(DocumentNodeMarkerSortedList markerList, ISceneInsertionPoint insertionPoint)
      : base(markerList, insertionPoint)
    {
    }

    protected override bool OnQueryCanDrop(TimelineDragDescriptor descriptor)
    {
      DocumentNodeMarkerSortedList sourceData = this.SourceData;
      if (sourceData.Count > 1 && !this.CanAddMultipleItems)
        return false;
      foreach (SceneNode node in SceneNode.FromMarkerList<SceneNode>((DocumentNodeMarkerSortedListBase) sourceData, this.ViewModel))
      {
        if (!this.CanDrop(node, descriptor))
          return false;
      }
      return descriptor.CanDrop;
    }

    protected override DragDropEffects OnHandleDrop(DragDropEffects dropEffects)
    {
      if (!this.TryValidateDropAction())
        return DragDropEffects.None;
      int index = SmartInsertionPoint.From(this.InsertionPoint).Index;
      if ((dropEffects & DragDropEffects.Copy) != DragDropEffects.None)
        this.Copy(index);
      else if ((dropEffects & DragDropEffects.Move) != DragDropEffects.None)
        this.Move(index);
      return DragDropEffects.None;
    }

    private bool CanDrop(SceneNode node, TimelineDragDescriptor descriptor)
    {
      SceneElement sceneElement;
      if (!descriptor.AllowCopy && !descriptor.AllowMove || ((sceneElement = node as SceneElement) == null || !this.InsertionPoint.CanInsert((ITypeId) sceneElement.Type)))
        return false;
      if (descriptor.AllowCopy)
      {
        descriptor.SetCopyInto(this.InsertionPoint);
      }
      else
      {
        if (sceneElement.IsAncestorOf(this.TargetNode))
          return false;
        descriptor.SetMoveInto(this.InsertionPoint);
      }
      descriptor.TryReplace((object) sceneElement, SmartInsertionPoint.From(this.InsertionPoint), this.DestinationCollection);
      return true;
    }

    private SceneNode Move(SceneNode node, ref int index)
    {
      SceneElement sceneElement = node as SceneElement;
      ISceneNodeCollection<SceneNode> destinationCollection = this.DestinationCollection;
      bool flag = destinationCollection.Contains((SceneNode) sceneElement);
      BaseFrameworkElement element = sceneElement as BaseFrameworkElement;
      ILayoutDesigner layoutDesigner1 = element == null ? (ILayoutDesigner) null : this.ViewModel.GetLayoutDesignerForChild((SceneElement) element, false);
      if (sceneElement.IsAttached)
      {
        if (flag)
        {
          if (destinationCollection.IndexOf((SceneNode) sceneElement) < index)
            --index;
          layoutDesigner1 = (ILayoutDesigner) null;
        }
        this.TargetNode.ViewModel.RemoveElement((SceneNode) sceneElement);
        this.ViewModel.Document.OnUpdatedEditTransaction();
      }
      SmartInsertionPoint.From(this.InsertionPoint, index).Insert((SceneNode) sceneElement);
      this.ViewModel.Document.OnUpdatedEditTransaction();
      ILayoutDesigner layoutDesigner2 = element == null ? (ILayoutDesigner) null : this.ViewModel.GetLayoutDesignerForChild((SceneElement) element, false);
      LayoutCacheRecord layoutCacheRecord;
      if (layoutDesigner1 != null && layoutDesigner2 != null && (element != null && this.layoutCache.TryGetValue((SceneNode) element, out layoutCacheRecord)))
        layoutDesigner2.SetLayoutFromCache(element, layoutCacheRecord, this.boundsOfAllElements);
      if (index != -1)
        ++index;
      return (SceneNode) sceneElement;
    }

    private void Move(int index)
    {
      this.CacheAllElements();
      using (SceneEditTransaction editTransaction = this.ViewModel.CreateEditTransaction(StringTable.UndoUnitArrange))
      {
        SceneElement primarySelection = this.ViewModel.ElementSelectionSet.PrimarySelection;
        List<SceneNode> list1 = SceneNode.FromMarkerList<SceneNode>((DocumentNodeMarkerSortedListBase) this.SourceData, this.ViewModel);
        list1.Sort((IComparer<SceneNode>) new ZOrderComparer<SceneNode>(this.ViewModel.RootNode, !this.ViewModel.TimelineItemManager.SortByZOrder, true));
        if (!this.ViewModel.TimelineItemManager.SortByZOrder)
          list1.Reverse();
        List<SceneElement> list2 = new List<SceneElement>();
        foreach (SceneNode node in list1)
        {
          SceneElement sceneElement = this.Move(node, ref index) as SceneElement;
          if (sceneElement != null)
            list2.Add(sceneElement);
        }
        this.ViewModel.ElementSelectionSet.SetSelection((ICollection<SceneElement>) list2, primarySelection);
        editTransaction.Commit();
      }
    }

    private void Copy(int index)
    {
      this.CacheAllElements();
      using (SceneEditTransaction editTransaction = this.ViewModel.CreateEditTransaction(StringTable.UndoUnitCopy))
      {
        List<SceneNode> list1 = SceneNode.FromMarkerList<SceneNode>((DocumentNodeMarkerSortedListBase) this.SourceData, this.ViewModel);
        list1.Sort((IComparer<SceneNode>) new ZOrderComparer<SceneNode>(this.ViewModel.RootNode, !this.ViewModel.TimelineItemManager.SortByZOrder, true));
        if (!this.ViewModel.TimelineItemManager.SortByZOrder)
          list1.Reverse();
        List<SceneElement> list2 = new List<SceneElement>();
        foreach (SceneNode node in list1)
        {
          SceneElement sceneElement = this.Copy(node, ref index) as SceneElement;
          if (sceneElement != null)
            list2.Add(sceneElement);
        }
        this.ViewModel.ElementSelectionSet.SetSelection((ICollection<SceneElement>) list2, (SceneElement) null);
        editTransaction.Commit();
      }
    }

    private SceneNode Copy(SceneNode node, ref int index)
    {
      SceneElement sceneElement1 = node as SceneElement;
      DocumentNode node1 = sceneElement1.DocumentNode.Clone(sceneElement1.DocumentContext);
      SceneElement sceneElement2 = (SceneElement) null;
      bool flag = false;
      if (node1 != null)
      {
        sceneElement2 = this.ViewModel.GetSceneNode(node1) as SceneElement;
        if (sceneElement2 != null)
        {
          if (sceneElement1.ParentElement != this.TargetNode)
          {
            DocumentCompositeNode documentCompositeNode = (DocumentCompositeNode) sceneElement2.DocumentNode;
            documentCompositeNode.ClearValue(DesignTimeProperties.ClassProperty);
            documentCompositeNode.ClearValue(DesignTimeProperties.SubclassProperty);
            documentCompositeNode.ClearValue(DesignTimeProperties.ClassModifierProperty);
            flag = true;
          }
          SmartInsertionPoint.From(this.InsertionPoint, index).Insert((SceneNode) sceneElement2);
          if (flag)
          {
            BaseFrameworkElement element = sceneElement2 as BaseFrameworkElement;
            ILayoutDesigner layoutDesigner = element == null ? (ILayoutDesigner) null : this.ViewModel.GetLayoutDesignerForChild((SceneElement) element, true);
            if (layoutDesigner != null)
            {
              this.ViewModel.Document.OnUpdatedEditTransaction();
              LayoutCacheRecord layoutCacheRecord;
              if (element.ViewObject != null && sceneElement1 is BaseFrameworkElement && this.layoutCache.TryGetValue((SceneNode) sceneElement1, out layoutCacheRecord))
                layoutDesigner.SetLayoutFromCache(element, layoutCacheRecord, this.boundsOfAllElements);
            }
          }
        }
      }
      if (index != -1)
        ++index;
      return (SceneNode) sceneElement2;
    }

    private void CacheAllElements()
    {
      this.boundsOfAllElements = Rect.Empty;
      this.layoutCache = new Dictionary<SceneNode, LayoutCacheRecord>();
      foreach (SceneNode sceneNode in SceneNode.FromMarkerList<SceneNode>((DocumentNodeMarkerSortedListBase) this.SourceData, this.ViewModel))
      {
        BaseFrameworkElement element = sceneNode as BaseFrameworkElement;
        if (element != null && element.ViewObject != null)
        {
          LayoutCacheRecord layoutCacheRecord = this.ViewModel.GetLayoutDesignerForChild((SceneElement) element, true).CacheLayout(element);
          this.layoutCache.Add((SceneNode) element, layoutCacheRecord);
          this.boundsOfAllElements.Union(layoutCacheRecord.Rect);
        }
      }
    }
  }
}
