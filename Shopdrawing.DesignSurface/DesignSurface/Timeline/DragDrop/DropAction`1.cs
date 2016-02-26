// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Timeline.DragDrop.DropAction`1
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.Timeline.DragDrop
{
  public abstract class DropAction<TSourceData> : IDropAction
  {
    public TSourceData SourceData { get; private set; }

    public ISceneInsertionPoint InsertionPoint { get; private set; }

    protected SceneNode TargetNode
    {
      get
      {
        return this.InsertionPoint.SceneNode;
      }
    }

    protected SceneElement TargetElement
    {
      get
      {
        return this.InsertionPoint.SceneElement;
      }
    }

    protected bool IsLockedOrAncestorLocked
    {
      get
      {
        for (SceneNode sceneNode = this.TargetNode; sceneNode != null; sceneNode = sceneNode.Parent)
        {
          SceneElement sceneElement = sceneNode as SceneElement;
          if (sceneElement != null)
            return sceneElement.IsLockedOrAncestorLocked;
        }
        return false;
      }
    }

    protected SceneViewModel ViewModel
    {
      get
      {
        return this.InsertionPoint.SceneNode.ViewModel;
      }
    }

    protected ISceneNodeCollection<SceneNode> DestinationCollection
    {
      get
      {
        return this.TargetNode.GetCollectionForProperty((IPropertyId) this.InsertionPoint.Property);
      }
    }

    protected bool CanAddMultipleItems
    {
      get
      {
        return this.TargetNode.IsCollectionProperty((IPropertyId) this.InsertionPoint.Property);
      }
    }

    protected DropAction(TSourceData sourceData, ISceneInsertionPoint insertionPoint)
    {
      this.CheckNullArgument((object) sourceData, "sourceData");
      this.CheckNullArgument((object) insertionPoint, "insertionPoint");
      this.SourceData = sourceData;
      this.InsertionPoint = insertionPoint;
    }

    public bool CanDrop(TimelineDragDescriptor descriptor)
    {
      this.CheckNullArgument((object) descriptor, "descriptor");
      if (this.IsLockedOrAncestorLocked)
        return false;
      return this.OnQueryCanDrop(descriptor);
    }

    public DragDropEffects HandleDrop(DragDropEffects dropEffects)
    {
      return this.OnHandleDrop(dropEffects);
    }

    protected virtual bool OnQueryCanDrop(TimelineDragDescriptor descriptor)
    {
      descriptor.DisableDrop();
      return false;
    }

    protected virtual DragDropEffects OnHandleDrop(DragDropEffects dropEffects)
    {
      return DragDropEffects.None;
    }

    protected void CheckNullArgument(object argument, string argumentName)
    {
      if (argument == null)
        throw new ArgumentNullException(argumentName);
    }

    protected bool TryValidateDropAction()
    {
      if (!this.ViewModel.TimelineItemManager.AnimationEditor.IsRecording)
        return true;
      this.ViewModel.TimelineItemManager.ViewModel.DesignerContext.MessageDisplayService.ShowError(StringTable.TimelineRearrangeInRecordingMessage);
      return false;
    }
  }
}
