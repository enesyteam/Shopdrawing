// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Timeline.DragDrop.SmartInsertionPoint
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Designers;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.Timeline.DragDrop
{
  public class SmartInsertionPoint : ISceneInsertionPoint
  {
    private ISceneInsertionPoint reference;
    private int index;

    public SceneElement SceneElement
    {
      get
      {
        return this.reference.SceneElement;
      }
    }

    public SceneNode SceneNode
    {
      get
      {
        return this.reference.SceneNode;
      }
    }

    public IProperty Property
    {
      get
      {
        return this.reference.Property;
      }
    }

    public int Index
    {
      get
      {
        return this.index;
      }
    }

    private bool CanAddMultipleItems
    {
      get
      {
        return this.SceneNode.IsCollectionProperty((IPropertyId) this.Property);
      }
    }

    private ISceneNodeCollection<SceneNode> DestinationCollection
    {
      get
      {
        return this.SceneNode.GetCollectionForProperty((IPropertyId) this.Property);
      }
    }

    private SmartInsertionPoint(int index, ISceneInsertionPoint reference)
    {
      if (reference == null)
        throw new ArgumentNullException("reference");
      this.reference = reference;
      this.index = index;
    }

    public static SmartInsertionPoint From(ISceneInsertionPoint reference)
    {
      return SmartInsertionPoint.From(reference, -1);
    }

    public static SmartInsertionPoint From(ISceneInsertionPoint reference, int index)
    {
      SmartInsertionPoint smartInsertionPoint = reference as SmartInsertionPoint;
      if (smartInsertionPoint != null)
      {
        if (index < 0 || smartInsertionPoint.index == index)
          return smartInsertionPoint;
        return new SmartInsertionPoint(index, smartInsertionPoint.reference);
      }
      if (reference != null)
        return new SmartInsertionPoint(index, reference);
      return (SmartInsertionPoint) null;
    }

    public void Insert(SceneNode nodeToInsert)
    {
      if (this.ShouldNestContents(nodeToInsert))
      {
        this.NestContents(nodeToInsert);
      }
      else
      {
        ISceneNodeCollection<SceneNode> destinationCollection = this.DestinationCollection;
        if (destinationCollection.FixedCapacity.HasValue && destinationCollection.Count >= destinationCollection.FixedCapacity.Value)
          PropertySceneInsertionPoint.Cleanup(destinationCollection[destinationCollection.Count - 1]);
        if (this.index >= 0 && this.index < destinationCollection.Count)
          destinationCollection.Insert(this.index, nodeToInsert);
        else
          destinationCollection.Add(nodeToInsert);
      }
    }

    public bool CanInsert(ITypeId typeToInsert)
    {
      return this.reference.CanInsert(typeToInsert);
    }

    public bool ShouldNestContents(SceneNode node)
    {
      SceneElement sceneElement = node as SceneElement;
      if (sceneElement == null || this.CanAddMultipleItems)
        return false;
      ISceneInsertionPoint defaultInsertionPoint = sceneElement.DefaultInsertionPoint;
      if (defaultInsertionPoint == null)
        return false;
      foreach (SceneNode sceneNode in (IEnumerable<SceneNode>) this.DestinationCollection)
      {
        if (!defaultInsertionPoint.CanInsert((ITypeId) sceneNode.Type))
          return false;
      }
      return sceneElement.IsCollectionProperty((IPropertyId) sceneElement.DefaultContentProperty) || sceneElement.DefaultContent.Count <= 0;
    }

    private void NestContents(SceneNode node)
    {
      SceneElement sceneElement1 = (SceneElement) node;
      ISceneNodeCollection<SceneNode> destinationCollection = this.DestinationCollection;
      int count = destinationCollection.Count;
      SceneElement[] sceneElementArray = new SceneElement[count];
      LayoutCacheRecord[] layoutCacheRecordArray = new LayoutCacheRecord[count];
      ILayoutDesigner designerForParent1 = this.SceneNode.ViewModel.GetLayoutDesignerForParent(this.SceneElement, true);
      Rect empty = Rect.Empty;
      for (int index = 0; index < count; ++index)
      {
        BaseFrameworkElement frameworkElement = destinationCollection[index] as BaseFrameworkElement;
        if (frameworkElement != null)
        {
          layoutCacheRecordArray[index] = designerForParent1.CacheLayout(frameworkElement);
          empty.Union(designerForParent1.GetChildRect(frameworkElement));
        }
      }
      for (int index = 0; index < count; ++index)
      {
        SceneNode child = destinationCollection[index];
        SceneElement sceneElement2 = child as SceneElement;
        if (sceneElement2 != null)
          sceneElementArray[index] = sceneElement2;
        this.SceneNode.ViewModel.RemoveElement(child);
      }
      destinationCollection.Add((SceneNode) sceneElement1);
      for (int index = 0; index < count; ++index)
      {
        if (sceneElementArray[index] != null)
          sceneElement1.DefaultContent.Add((SceneNode) sceneElementArray[index]);
      }
      this.SceneNode.ViewModel.Document.OnUpdatedEditTransaction();
      this.SceneNode.DesignerContext.ActiveView.UpdateLayout();
      ILayoutDesigner designerForParent2 = this.SceneNode.ViewModel.GetLayoutDesignerForParent(sceneElement1, true);
      for (int index = 0; index < count; ++index)
      {
        if (sceneElementArray[index] != null && layoutCacheRecordArray[index] != null)
          designerForParent2.SetLayoutFromCache((BaseFrameworkElement) sceneElementArray[index], layoutCacheRecordArray[index], empty);
      }
      if (sceneElementArray.Length != 1)
        return;
      VisualStateManagerSceneNode.MoveStates(sceneElementArray[0], sceneElement1);
    }
  }
}
