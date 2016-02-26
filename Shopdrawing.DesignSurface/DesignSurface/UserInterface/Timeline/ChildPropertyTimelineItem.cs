// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Timeline.ChildPropertyTimelineItem
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Selection;
using Microsoft.Expression.DesignSurface.Timeline.DragDrop;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Data;
using Microsoft.Expression.Framework.Diagnostics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.UserInterface.Timeline
{
  public class ChildPropertyTimelineItem : CompoundKeyFrameTimelineItem
  {
    private IProperty targetProperty;
    private ElementTimelineItem elementTimelineItem;
    private ChildPropertyTimelineItemType itemType;
    private bool enableSelection;
    private bool isAlternateContent;
    private int stackOrder;
    private DelegateCommand lockInsertionPointCommand;

    public bool IsAlternateContentProperty
    {
      get
      {
        if (!this.isAlternateContent)
          return this.itemType == ChildPropertyTimelineItemType.Default;
        return true;
      }
    }

    public bool ExpandParentOnInsertion { get; set; }

    public ElementTimelineItem TargetElementTimelineItem
    {
      get
      {
        return this.elementTimelineItem;
      }
    }

    public int StackOrder
    {
      get
      {
        return this.stackOrder;
      }
      set
      {
        this.stackOrder = value;
      }
    }

    public bool EnableSelection
    {
      get
      {
        return this.enableSelection;
      }
      set
      {
        this.enableSelection = value;
      }
    }

    public override string DisplayName
    {
      get
      {
        return this.targetProperty.Name;
      }
      set
      {
      }
    }

    public override string FullName
    {
      get
      {
        return this.elementTimelineItem.FullName + "." + this.targetProperty.Name;
      }
    }

    public ChildPropertyTimelineItemType ItemType
    {
      get
      {
        return this.itemType;
      }
    }

    public IProperty TargetProperty
    {
      get
      {
        return this.targetProperty;
      }
    }

    public SceneNode TargetPropertyValue
    {
      get
      {
        return this.elementTimelineItem.Element.GetLocalValueAsSceneNode((IPropertyId) this.targetProperty);
      }
    }

    protected override bool IsActiveCore
    {
      get
      {
        return true;
      }
    }

    public bool IsDynamicInsertionPoint
    {
      get
      {
        if (this.IsLockedInsertionPoint)
          return false;
        PropertySceneInsertionPoint activeInsertionPoint = this.TimelineItemManager.ActiveInsertionPoint;
        if (activeInsertionPoint != null && activeInsertionPoint.SceneElement == this.elementTimelineItem.Element)
          return activeInsertionPoint.Property == this.TargetProperty;
        return false;
      }
    }

    public bool IsLockedInsertionPoint
    {
      get
      {
        PropertySceneInsertionPoint sceneInsertionPoint = this.TimelineItemManager.ViewModel.LockedInsertionPoint as PropertySceneInsertionPoint;
        if (sceneInsertionPoint != null && sceneInsertionPoint.SceneElement == this.elementTimelineItem.Element)
          return sceneInsertionPoint.Property == this.TargetProperty;
        return false;
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

    public double ActiveDuration
    {
      get
      {
        return this.Duration;
      }
    }

    public override bool IsSelectable
    {
      get
      {
        if (!this.enableSelection || this.Parent == null || !this.Parent.IsSelectable)
          return false;
        return this.TimelinePane.HasUnlockedActiveStoryboardContainer;
      }
    }

    public virtual bool IsShown
    {
      get
      {
        return !(bool) this.elementTimelineItem.Element.GetLocalValueAsSceneNode((IPropertyId) this.targetProperty).GetLocalOrDefaultValue(DesignTimeProperties.IsHiddenProperty);
      }
      set
      {
        if (this.IsShown == value)
          return;
        SceneNode valueAsSceneNode = this.elementTimelineItem.Element.GetLocalValueAsSceneNode((IPropertyId) this.targetProperty);
        if (valueAsSceneNode == null)
          return;
        using (SceneEditTransaction editTransaction = this.TimelineItemManager.ViewModel.CreateEditTransaction(value ? StringTable.UndoUnitShow : StringTable.UndoUnitHide))
        {
          if (!value)
            valueAsSceneNode.SetLocalValue(DesignTimeProperties.IsHiddenProperty, (object) true);
          else
            valueAsSceneNode.ClearLocalValue(DesignTimeProperties.IsHiddenProperty);
          editTransaction.Commit();
        }
        this.OnPropertyChanged("IsShown");
      }
    }

    public ChildPropertyTimelineItem(TimelineItemManager timelineItemManager, IProperty targetProperty, ElementTimelineItem elementTimelineItem, ChildPropertyTimelineItemType itemType, bool isAlternateContent)
      : base(timelineItemManager)
    {
      this.targetProperty = targetProperty;
      this.itemType = itemType;
      this.stackOrder = -1;
      this.isAlternateContent = isAlternateContent;
      this.elementTimelineItem = elementTimelineItem;
      this.elementTimelineItem.PropertyChanged += new PropertyChangedEventHandler(this.ElementTimelineItem_PropertyChanged);
      if (this.itemType != ChildPropertyTimelineItemType.Default)
        return;
      this.lockInsertionPointCommand = new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.LockInsertionPoint_Execute));
    }

    public override void Select()
    {
      if (!this.EnableSelection)
        return;
      ChildPropertySelectionSet propertySelectionSet = this.TimelineItemManager.ViewModel.ChildPropertySelectionSet;
      if (!this.IsSelectable)
      {
        propertySelectionSet.Clear();
      }
      else
      {
        using (this.TimelineItemManager.ViewModel.ForceBaseValue())
        {
          this.elementTimelineItem.Element.GetLocalValueAsSceneNode((IPropertyId) this.targetProperty);
          propertySelectionSet.SetSelection(this.TargetPropertyValue);
        }
      }
    }

    public override void ToggleSelect()
    {
      if (!this.IsSelectable)
        return;
      using (this.TimelineItemManager.ViewModel.ForceBaseValue())
        this.TimelineItemManager.ViewModel.ChildPropertySelectionSet.ToggleSelection(this.TargetPropertyValue);
    }

    public override void ExtendSelect()
    {
      if (!this.EnableSelection || (!this.IsSelectable || this.TimelineItemManager.ViewModel.ChildPropertySelectionSet.PrimarySelection == null))
        return;
      using (this.TimelineItemManager.ViewModel.ForceBaseValue())
        this.TimelineItemManager.ExtendChildPropertySelection(this.TargetPropertyValue, (IPropertyId) this.targetProperty);
    }

    public override int CompareTo(TimelineItem item)
    {
      ChildPropertyTimelineItem propertyTimelineItem = item as ChildPropertyTimelineItem;
      if (propertyTimelineItem == null)
        return 0;
      int num = this.stackOrder.CompareTo(propertyTimelineItem.stackOrder);
      if (num == 0)
        return string.Compare(this.DisplayName, propertyTimelineItem.DisplayName, StringComparison.CurrentCulture);
      return num;
    }

    public void ResetAllProperties()
    {
      this.OnPropertyChanged((string) null);
      SceneNode valueAsSceneNode = this.elementTimelineItem.Element.GetLocalValueAsSceneNode((IPropertyId) this.TargetProperty);
      ICollection<SceneNode> collection = (ICollection<SceneNode>) this.TimelineItemManager.ViewModel.ChildPropertySelectionSet.Selection;
      if (valueAsSceneNode != null && (collection != null || collection.Contains(valueAsSceneNode)))
        return;
      this.IsSelected = false;
    }

    public void RefreshInsertionPointProperties()
    {
      this.OnPropertyChanged("IsDynamicInsertionPoint");
      this.OnPropertyChanged("IsLockedInsertionPoint");
    }

    public override ISceneInsertionPoint GetInsertionPoint(object data, DragDropContext context)
    {
      InsertionPointCreatorList pointCreatorList = new InsertionPointCreatorList();
      pointCreatorList.Add((IInsertionPointCreator) new DataBindingInsertionPointCreator((TimelineItem) this, (SceneNode) this.elementTimelineItem.Element, this.targetProperty, context));
      pointCreatorList.Add((IInsertionPointCreator) new ChildPropertyInsertionPointCreator(this.elementTimelineItem.Element, this.targetProperty));
      return pointCreatorList.Create(data);
    }

    protected override void ChildPropertyChanged(string propertyName)
    {
      base.ChildPropertyChanged(propertyName);
      if (!(propertyName == "IsActive"))
        return;
      this.IsActive = this.IsActiveCore;
    }

    protected override void ChildRemoved(TimelineItem child)
    {
      base.ChildRemoved(child);
      this.IsActive = this.IsActiveCore;
    }

    protected override void KeyFramesRebuilt()
    {
      base.KeyFramesRebuilt();
      this.OnPropertyChanged("ActiveDuration");
    }

    private void LockInsertionPoint_Execute()
    {
      using (SceneEditTransaction editTransaction = this.TimelineItemManager.ViewModel.CreateEditTransaction(StringTable.UndoUnitLockInsertionPoint))
      {
        if (this.IsInsertionPointLocked())
          this.TimelineItemManager.ViewModel.LockedInsertionPoint = (ISceneInsertionPoint) null;
        else
          this.TimelineItemManager.ViewModel.LockedInsertionPoint = (ISceneInsertionPoint) new PropertySceneInsertionPoint(this.elementTimelineItem.Element, this.TargetProperty);
        editTransaction.Commit();
      }
    }

    private bool IsInsertionPointLocked()
    {
      if (this.TimelineItemManager.ViewModel.LockedInsertionPoint != null && this.TimelineItemManager.ViewModel.LockedInsertionPoint.SceneElement == this.elementTimelineItem.Element)
        return this.TimelineItemManager.ViewModel.LockedInsertionPoint.Property == this.TargetProperty;
      return false;
    }

    protected override ContextMenu BuildContextMenu()
    {
      if (this.ItemType != ChildPropertyTimelineItemType.Default)
        return (ContextMenu) null;
      ContextMenu contextMenu = new ContextMenu();
      MenuItem menuItem = new MenuItem();
      menuItem.Command = (ICommand) this.lockInsertionPointCommand;
      menuItem.Header = (object) StringTable.SelectionContextMenuLockInsertionPoint;
      menuItem.IsChecked = this.IsInsertionPointLocked();
      menuItem.SetValue(AutomationElement.IdProperty, (object) "Edit_LockInsertionPoint");
      contextMenu.Items.Add((object) menuItem);
      return contextMenu;
    }

    private void ElementTimelineItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == "FullName"))
        return;
      this.OnPropertyChanged("FullName");
    }
  }
}
