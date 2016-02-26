// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Timeline.DragDrop.TimelineDragDescriptor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Tools.Assets;
using Microsoft.Expression.DesignSurface.UserInterface.Timeline;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.Timeline.DragDrop
{
  public sealed class TimelineDragDescriptor
  {
    private DragDropContext Context { get; set; }

    public TimelineItem TargetItem
    {
      get
      {
        return this.Context.Target;
      }
    }

    public IDataObject SourceObject
    {
      get
      {
        return this.Context.Data;
      }
    }

    public TimelineDropEffects ResultDropEffects { get; private set; }

    public DragDropEffects ResultEffects
    {
      get
      {
        return TimelineDropEffectsHelper.DragDropEffect(this.ResultDropEffects);
      }
    }

    public TimelineItem TargetParent { get; private set; }

    public SceneNode ReplacedChild { get; private set; }

    public bool HideSplitter { get; private set; }

    public bool IsReplacingChild { get; private set; }

    public bool IsCreating { get; private set; }

    public bool IsNestingContents { get; private set; }

    public bool IsDataBinding { get; private set; }

    public bool DropInDefaultContent { get; private set; }

    public int RelativeDepth { get; set; }

    public int DropIndex { get; set; }

    public object UserData { get; set; }

    public bool AllowBetween
    {
      get
      {
        return this.IsAnyFlagSet(TimelineDropEffects.Before | TimelineDropEffects.After);
      }
    }

    public bool AllowMove
    {
      get
      {
        return this.IsAnyFlagSet(TimelineDropEffects.Move);
      }
    }

    public bool AllowCopy
    {
      get
      {
        return this.IsAnyFlagSet(TimelineDropEffects.Copy);
      }
    }

    public bool CanDrop
    {
      get
      {
        return this.ResultDropEffects != TimelineDropEffects.None;
      }
    }

    public TimelineDragDescriptor(DragDropContext context)
    {
      if (context == null)
        throw new ArgumentNullException("context");
      this.DropIndex = -1;
      this.Context = context;
      this.ResultDropEffects = this.Context.AllowedEffects;
    }

    public static TimelineDragDescriptor CreateProxy(DragDropEffects allowedEffects)
    {
      return new DragDropContext((IDataObject) null, (TimelineItem) null, TimelineDropEffectsHelper.FromDragDropEffects(allowedEffects)).Descriptor;
    }

    public TimelineDragDescriptor Clone()
    {
      return this.MemberwiseClone() as TimelineDragDescriptor;
    }

    public void DisableDrop()
    {
      this.ResultDropEffects = TimelineDropEffects.None;
    }

    public void DisableMove()
    {
      this.SetFlag(TimelineDropEffects.Move, false);
    }

    public void DisableCopy()
    {
      this.SetFlag(TimelineDropEffects.Copy, false);
    }

    public void DisableInBetween()
    {
      this.SetFlag(TimelineDropEffects.Before | TimelineDropEffects.After, false);
    }

    public void SetMoveInto(ISceneInsertionPoint insertionPoint)
    {
      this.ResultDropEffects |= TimelineDropEffects.Move;
      this.TargetParent = this.FindParent(insertionPoint.SceneNode);
      this.DropInDefaultContent = insertionPoint.SceneNode.DefaultContentProperty == insertionPoint.Property;
    }

    public void SetCopyInto(ISceneInsertionPoint insertionPoint)
    {
      this.ResultDropEffects |= TimelineDropEffects.Copy;
      this.TargetParent = this.FindParent(insertionPoint.SceneNode);
      this.DropInDefaultContent = insertionPoint.SceneNode.DefaultContentProperty == insertionPoint.Property;
    }

    public void SetCreateIn(ISceneInsertionPoint insertionPoint)
    {
      this.IsCreating = true;
      this.TargetParent = this.FindParent(insertionPoint.SceneNode);
      this.DropInDefaultContent = insertionPoint.SceneNode.DefaultContentProperty == insertionPoint.Property;
    }

    public void SetDataBindingTo(ISceneInsertionPoint insertionPoint)
    {
      this.IsDataBinding = true;
      this.TargetParent = this.FindParent(insertionPoint.SceneNode);
      this.DropInDefaultContent = insertionPoint.SceneNode.DefaultContentProperty == insertionPoint.Property;
    }

    public bool TryReplace(object source, SmartInsertionPoint insertionPoint, ISceneNodeCollection<SceneNode> destinationCollection)
    {
      if (insertionPoint != null && insertionPoint.SceneNode != null && (insertionPoint.SceneNode.ViewModel != null && destinationCollection != null) && destinationCollection.Count > 0 && destinationCollection.FixedCapacity.HasValue)
      {
        int? fixedCapacity = destinationCollection.FixedCapacity;
        int count = destinationCollection.Count;
        if ((fixedCapacity.GetValueOrDefault() != count ? 0 : (fixedCapacity.HasValue ? true : false)) != 0)
        {
          this.IsReplacingChild = true;
          this.HideSplitter = true;
          this.ReplacedChild = destinationCollection[destinationCollection.Count - 1];
          SceneNode node = source as SceneNode;
          Asset asset = source as Asset;
          if (asset != null)
            node = asset.CreatePrototypeInstance((ISceneInsertionPoint) insertionPoint);
          if (node != null && insertionPoint.ShouldNestContents(node))
            this.IsNestingContents = true;
          return true;
        }
      }
      return false;
    }

    private TimelineItem FindParent(SceneNode parent)
    {
      if (this.TargetItem != null && this.TargetItem.TimelineItemManager != null)
        return this.TargetItem.TimelineItemManager.FindTimelineItem(parent);
      return (TimelineItem) null;
    }

    private bool IsAnyFlagSet(TimelineDropEffects effects)
    {
      return (this.ResultDropEffects & effects) != TimelineDropEffects.None;
    }

    private void SetFlag(TimelineDropEffects effects, bool value)
    {
      if (value)
        this.ResultDropEffects |= effects;
      else
        this.ResultDropEffects &= ~effects;
    }
  }
}
