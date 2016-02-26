// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Timeline.EffectTimelineItem
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Selection;
using Microsoft.Expression.DesignSurface.Timeline.DragDrop;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.UserInterface.Timeline
{
  public class EffectTimelineItem : ChildPropertyTimelineItem
  {
    public bool IsEffectDisabled
    {
      get
      {
        return this.TargetElementTimelineItem.Element.ViewModel.DefaultView != null && this.TargetElementTimelineItem.Element.ViewModel.DefaultView.InstanceBuilderContext.EffectManager.DisableEffects;
      }
    }

    public override DrawingBrush IconBrush
    {
      get
      {
        EffectNode effectNode = this.TargetElementTimelineItem.Element.GetLocalValueAsSceneNode(Base2DElement.EffectProperty, false) as EffectNode;
        return IconMapper.GetDrawingBrushForType(effectNode != null ? (ITypeId) effectNode.Type : PlatformTypes.Effect, true, 12, 12);
      }
    }

    public override bool IsShown
    {
      get
      {
        EffectNode effectNode = this.TargetElementTimelineItem.Element.GetLocalValueAsSceneNode(Base2DElement.EffectProperty, false) as EffectNode;
        if (effectNode != null)
          return !effectNode.IsDisabled;
        return false;
      }
      set
      {
        if (this.IsEffectDisabled)
          return;
        EffectNode effectNode = this.TargetElementTimelineItem.Element.GetLocalValueAsSceneNode(Base2DElement.EffectProperty, false) as EffectNode;
        if (effectNode == null)
          return;
        using (SceneEditTransaction editTransaction = this.TimelineItemManager.ViewModel.CreateEditTransaction(value ? StringTable.UndoUnitShow : StringTable.UndoUnitHide))
        {
          effectNode.IsDisabled = !value;
          editTransaction.Commit();
        }
        this.OnPropertyChanged("IsShown");
      }
    }

    public override bool CanLock
    {
      get
      {
        return true;
      }
    }

    public virtual bool IsAncestorHidden
    {
      get
      {
        if (this.IsEffectDisabled)
          return true;
        for (TimelineItem parent = this.Parent; parent != null; parent = parent.Parent)
        {
          ElementTimelineItem elementTimelineItem = parent as ElementTimelineItem;
          if (elementTimelineItem != null && !elementTimelineItem.IsShown)
            return true;
        }
        return false;
      }
    }

    public override bool CanHide
    {
      get
      {
        return PlatformTypes.UIElement.IsAssignableFrom(this.TargetElementTimelineItem.ElementType);
      }
    }

    public override object DragData
    {
      get
      {
        DocumentNodeMarkerSortedList markerList = SceneNode.GetMarkerList<SceneNode>((IEnumerable<SceneNode>) this.TimelineItemManager.ViewModel.ChildPropertySelectionSet.Selection, true);
        if (markerList.Count > 0)
          return (object) markerList;
        return (object) null;
      }
    }

    public override bool ShouldBubbleKeyFrames
    {
      get
      {
        return false;
      }
    }

    public override bool ShouldBubbleTimes
    {
      get
      {
        return false;
      }
    }

    protected override bool IsActiveCore
    {
      get
      {
        return true;
      }
    }

    public override string DisplayName
    {
      get
      {
        SceneNode valueAsSceneNode = this.TargetElementTimelineItem.Element.GetLocalValueAsSceneNode(Base2DElement.EffectProperty, false);
        if (valueAsSceneNode != null)
          return valueAsSceneNode.Type.Name;
        return Base2DElement.EffectProperty.Name;
      }
    }

    public EffectTimelineItem(TimelineItemManager timelineItemManager, IProperty targetProperty, ElementTimelineItem elementTimelineItem, ChildPropertyTimelineItemType itemType)
      : base(timelineItemManager, targetProperty, elementTimelineItem, itemType, false)
    {
    }

    public void RefreshIsAncestorHidden()
    {
      this.OnPropertyChanged("IsAncestorHidden");
      this.OnPropertyChanged("IsEffectDisabled");
    }

    public override bool CanDrag()
    {
      if (this.TargetElementTimelineItem.Element.IsInDocument && this.TargetElementTimelineItem.Element.IsSelectable)
        return this.TimelineItemManager.ViewModel.XamlDocument.IsEditable;
      return false;
    }

    protected override ContextMenu BuildContextMenu()
    {
      return ContextMenuHelper.CreateContextMenu((ISelectionSet<SceneElement>) this.TimelineItemManager.ViewModel.ElementSelectionSet, this.TimelineItemManager.ViewModel, false);
    }

    public override int CompareTo(TimelineItem treeItem)
    {
      return treeItem.Equals((object) this) ? 0 : int.MinValue;
    }

    public override ISceneInsertionPoint GetInsertionPoint(object data, DragDropContext context)
    {
      InsertionPointCreatorList pointCreatorList = new InsertionPointCreatorList();
      pointCreatorList.Add((IInsertionPointCreator) new DataBindingInsertionPointCreator((TimelineItem) this, this.TargetElementTimelineItem.Element.GetLocalValueAsSceneNode((IPropertyId) this.TargetProperty), (IProperty) null, context));
      pointCreatorList.Add((IInsertionPointCreator) new ChildPropertyInsertionPointCreator(this.TargetElementTimelineItem.Element, this.TargetProperty));
      return pointCreatorList.Create(data);
    }
  }
}
