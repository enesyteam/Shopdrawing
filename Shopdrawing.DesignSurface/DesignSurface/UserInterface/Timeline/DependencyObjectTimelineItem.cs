// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Timeline.DependencyObjectTimelineItem
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignSurface.SceneCommands;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Microsoft.Expression.DesignSurface.UserInterface.Timeline
{
  public class DependencyObjectTimelineItem : CompoundKeyFrameTimelineItem
  {
    private SceneNode sceneNode;

    public override string DisplayName
    {
      get
      {
        return this.sceneNode.TargetType.Name.ToString();
      }
      set
      {
      }
    }

    public override SceneNode SceneNode
    {
      get
      {
        return this.sceneNode;
      }
    }

    public override object DragData
    {
      get
      {
        DocumentNodeMarkerSortedList markerList = SceneNode.GetMarkerList<SceneNode>((IEnumerable<SceneNode>) this.TimelineItemManager.ViewModel.DependencyObjectSelectionSet.Selection, true);
        if (markerList.Count > 0)
          return (object) markerList;
        return (object) null;
      }
    }

    public virtual bool IsAncestorLocked
    {
      get
      {
        if (!this.SceneNode.IsInDocument)
          return false;
        TimelineItem parent = this.Parent;
        do
        {
          parent = parent.Parent;
        }
        while (parent != null && !(parent is ElementTimelineItem));
        if (parent == null || !(parent is ElementTimelineItem))
          return false;
        ElementTimelineItem elementTimelineItem = (ElementTimelineItem) parent;
        if (!elementTimelineItem.IsLocked)
          return elementTimelineItem.IsAncestorLocked;
        return true;
      }
    }

    public override bool ShouldBubbleKeyFrames
    {
      get
      {
        return true;
      }
    }

    public override bool ShouldBubbleTimes
    {
      get
      {
        return true;
      }
    }

    protected override bool IsActiveCore
    {
      get
      {
        return true;
      }
    }

    public DependencyObjectTimelineItem(TimelineItemManager timelineItemManager, SceneNode sceneNode)
      : base(timelineItemManager)
    {
      this.sceneNode = sceneNode;
      this.TimelineItemManager.RegisterTimelineItem((TimelineItem) this);
    }

    public override bool CanDrag()
    {
      return !this.IsAncestorLocked && this.TimelineItemManager.ViewModel.XamlDocument.IsEditable;
    }

    public override void Select()
    {
      this.TimelineItemManager.ViewModel.DependencyObjectSelectionSet.SetSelection(this.sceneNode);
    }

    public override void ToggleSelect()
    {
      this.TimelineItemManager.ViewModel.DependencyObjectSelectionSet.SetSelection(this.sceneNode);
    }

    protected override ContextMenu BuildContextMenu()
    {
      return base.BuildContextMenu();
    }

    public static int CompareToCommon(TimelineItem timelineItemA, TimelineItem timelineItemB, TimelineItemManager timelineItemManager)
    {
      int num1 = timelineItemManager.SortByZOrder ? 1 : -1;
      TimelineItem parent = timelineItemA.Parent;
      SceneNode sceneNode1 = timelineItemA.SceneNode;
      SceneNode sceneNode2 = timelineItemB.SceneNode;
      if (sceneNode1 == null || sceneNode2 == null)
        return 0;
      DocumentNode documentNode1 = sceneNode1.DocumentNode;
      DocumentNode documentNode2 = sceneNode2.DocumentNode;
      if (documentNode1.Parent == null)
        return documentNode2.Parent == null ? 0 : -1;
      if (documentNode1.IsChild && documentNode2.IsChild)
      {
        int num2 = documentNode1.Parent.Children.IndexOf(documentNode1);
        int num3 = documentNode1.Parent.Children.IndexOf(documentNode2);
        return num1 * (num3 - num2);
      }
      SceneNode sceneNode3 = timelineItemA.Parent.SceneNode;
      if (sceneNode3 == null)
        return 0;
      ZOrderComparer<SceneNode> zorderComparer = new ZOrderComparer<SceneNode>(sceneNode3);
      return -num1 * zorderComparer.Compare(sceneNode1, sceneNode2);
    }

    public override int CompareTo(TimelineItem treeItem)
    {
      return DependencyObjectTimelineItem.CompareToCommon((TimelineItem) this, treeItem, this.TimelineItemManager);
    }
  }
}
