// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Timeline.DragDrop.DefaultTimelineItemInsertionPointCreator
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.UserInterface.Timeline;
using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.Timeline.DragDrop
{
  internal class DefaultTimelineItemInsertionPointCreator : IInsertionPointCreator
  {
    public DragDropContext Context { get; private set; }

    public TimelineItem Item { get; private set; }

    public SceneNode Node { get; private set; }

    public SceneElement Element
    {
      get
      {
        return this.Node as SceneElement;
      }
    }

    public DefaultTimelineItemInsertionPointCreator(TimelineItem targetItem, SceneNode node, DragDropContext context)
    {
      this.Item = targetItem;
      this.Node = node;
      this.Context = context;
    }

    public virtual ISceneInsertionPoint Create(object data)
    {
      if (this.Context != null && this.Item != null)
      {
        SceneElement dropTargetInfo = this.GetDropTargetInfo(this.Context.AllowedEffects, this.Context.Descriptor);
        if (dropTargetInfo != null)
          return dropTargetInfo.InsertionPointForProperty(this.GetContentPropertyFromDragDescriptor(this.Context.Descriptor));
      }
      return (ISceneInsertionPoint) null;
    }

    protected SceneElement GetDropTargetInfo(TimelineDropEffects dropEffects, TimelineDragDescriptor descriptor)
    {
      SceneElement sceneElement1 = this.Element;
      if (sceneElement1 == null)
        return (SceneElement) null;
      descriptor.DropIndex = -1;
      int num = 0;
      SceneElement sceneElement2 = (SceneElement) null;
      bool flag = false;
      if ((dropEffects & TimelineDropEffects.Before) != TimelineDropEffects.None)
      {
        sceneElement1 = this.Element.VisualElementAncestor;
        sceneElement2 = this.Element.VisualElementAncestor;
        if (this.Item.IsExpanded && this.Item.HasActiveChild)
        {
          sceneElement1 = this.Element;
          sceneElement2 = this.Element;
          descriptor.RelativeDepth = 1;
        }
        num = 1;
        flag = true;
      }
      else if ((dropEffects & TimelineDropEffects.After) != TimelineDropEffects.None)
      {
        sceneElement1 = this.Element.VisualElementAncestor;
        sceneElement2 = this.Element.VisualElementAncestor;
        flag = true;
      }
      if (flag && sceneElement2 != null)
      {
        ISceneNodeCollection<SceneNode> collectionForProperty = sceneElement2.GetCollectionForProperty((IPropertyId) this.GetContentPropertyFromDragDescriptor(descriptor));
        if (collectionForProperty != null)
          descriptor.DropIndex = collectionForProperty.IndexOf((SceneNode) this.Element) + num;
      }
      if (!this.Element.ViewModel.ActiveEditingContainer.IsAncestorOf((SceneNode) sceneElement1))
        sceneElement1 = (SceneElement) null;
      return sceneElement1;
    }

    private IProperty GetContentPropertyFromDragDescriptor(TimelineDragDescriptor descriptor)
    {
      if (this.Context.Target != null)
      {
        if (descriptor.AllowBetween && this.Context.Target.SceneNode != null)
        {
          DocumentNode documentNode = this.Context.Target.SceneNode.DocumentNode;
          if (documentNode.Parent != null)
          {
            if (documentNode.SitePropertyKey != null)
              return documentNode.SitePropertyKey;
            return documentNode.Parent.SitePropertyKey;
          }
        }
        else
        {
          if (this.Context.Target is ElementTimelineItem && this.Context.Target.SceneNode != null)
            return this.Context.Target.SceneNode.InsertionTargetProperty;
          ChildPropertyTimelineItem propertyTimelineItem = this.Context.Target as ChildPropertyTimelineItem;
          if (propertyTimelineItem != null)
            return propertyTimelineItem.TargetProperty;
        }
      }
      return (IProperty) null;
    }
  }
}
