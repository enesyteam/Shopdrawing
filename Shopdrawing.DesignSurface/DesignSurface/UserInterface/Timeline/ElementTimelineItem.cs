// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Timeline.ElementTimelineItem
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
using Microsoft.Expression.DesignSurface.UserInterface.SkinEditing;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.Expression.Framework.UserInterface;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.UserInterface.Timeline
{
  public class ElementTimelineItem : CompoundKeyFrameTimelineItem
  {
    private PartStatus partStatus = PartStatus.Unused;
    private SceneElement element;
    private ICommand renameCommand;

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

    public SceneElement Element
    {
      get
      {
        return this.element;
      }
    }

    public ICommand RenameCommand
    {
      get
      {
        return this.renameCommand;
      }
      set
      {
        this.renameCommand = value;
      }
    }

    public override bool IsSelectable
    {
      get
      {
        if (this.Element.IsInDocument)
          return this.Element.IsSelectable;
        return false;
      }
    }

    public virtual bool IsShown
    {
      get
      {
        return !this.Element.IsHidden;
      }
      set
      {
        if (this.IsShown == value)
          return;
        using (SceneEditTransaction editTransaction = this.TimelineItemManager.ViewModel.CreateEditTransaction(value ? StringTable.UndoUnitShow : StringTable.UndoUnitHide))
        {
          this.Element.IsHidden = !value;
          editTransaction.Commit();
        }
        this.OnPropertyChanged("IsShown");
      }
    }

    public virtual bool IsAncestorHidden
    {
      get
      {
        for (TimelineItem parent = this.Parent; parent != null; parent = parent.Parent)
        {
          ElementTimelineItem elementTimelineItem = parent as ElementTimelineItem;
          if (elementTimelineItem != null && !elementTimelineItem.IsShown)
            return true;
        }
        return false;
      }
    }

    public virtual bool IsLocked
    {
      get
      {
        return this.Element.IsLocked;
      }
      set
      {
        if (this.IsLocked == value)
          return;
        using (SceneEditTransaction editTransaction = this.TimelineItemManager.ViewModel.CreateEditTransaction(value ? StringTable.UndoUnitLock : StringTable.UndoUnitUnlock))
        {
          this.Element.IsLocked = value;
          editTransaction.Commit();
        }
        this.OnPropertyChanged("IsLocked");
        this.OnPropertyChanged("IsSelectable");
      }
    }

    public virtual bool IsAncestorLocked
    {
      get
      {
        if (this.Element.IsInDocument)
          return !this.Element.IsSelectable;
        return false;
      }
    }

    public override bool IsExpanded
    {
      get
      {
        return base.IsExpanded;
      }
      set
      {
        base.IsExpanded = value;
        this.OnPropertyChanged("CurrentPartStatus");
      }
    }

    public PartStatus PartStatus
    {
      get
      {
        return this.partStatus;
      }
      set
      {
        this.partStatus = value;
        this.OnPropertyChanged("CurrentPartStatus");
      }
    }

    public virtual ElementPartStatus CurrentPartStatus
    {
      get
      {
        switch (this.PartStatus)
        {
          case PartStatus.Assigned:
            return ElementPartStatus.Assigned;
          case PartStatus.WrongType:
            return ElementPartStatus.WrongType;
          default:
            return !this.IsExpanded && this.HasActiveChild && this.DescendantAssignedToPart ? ElementPartStatus.DescendantAssignedOrWrongType : ElementPartStatus.NotAssigned;
        }
      }
    }

    public string WronglyAssignedPartCorrectType { get; set; }

    public string PartStatusToolTip { get; set; }

    public bool DescendantAssignedToPart { get; set; }

    protected override bool IsActiveCore
    {
      get
      {
        return true;
      }
    }

    protected override bool IsCompoundKeyFrameRoot
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
        if (activeInsertionPoint != null)
          return activeInsertionPoint.SceneElement == this.Element;
        return false;
      }
    }

    public bool IsLockedInsertionPoint
    {
      get
      {
        PropertySceneInsertionPoint sceneInsertionPoint = this.TimelineItemManager.ViewModel.LockedInsertionPoint as PropertySceneInsertionPoint;
        if (sceneInsertionPoint != null && sceneInsertionPoint.SceneElement == this.Element)
          return sceneInsertionPoint.Property == this.Element.DefaultContentProperty;
        return false;
      }
    }

    public override bool CanLock
    {
      get
      {
        if (this.TimelineItemManager.ViewModel.RootNode != this.Element)
          return this.TimelineItemManager.ScopedTimelineItem != this;
        return false;
      }
    }

    public override bool CanHide
    {
      get
      {
        if (!PlatformTypes.UIElement.IsAssignableFrom(this.ElementType) || this.TimelineItemManager.ViewModel.RootNode == this.Element)
          return false;
        return this.TimelineItemManager.ScopedTimelineItem != this;
      }
    }

    public override string DisplayName
    {
      get
      {
        return this.element.DisplayName;
      }
      set
      {
        if (string.IsNullOrEmpty(value) && this.Element.ViewModel.AnimationEditor.IsTargetedByName(this.element) || (this.element.Name == value || this.DisplayName == value))
          return;
        using (SceneEditTransaction editTransaction = this.element.ViewModel.CreateEditTransaction(StringTable.UndoUnitElementRename))
        {
          this.element.Name = value;
          editTransaction.Commit();
        }
      }
    }

    public override string DisplayNameNoTextContent
    {
      get
      {
        return this.Element.DisplayNameNoTextContent;
      }
    }

    public override string FullName
    {
      get
      {
        return this.element.UniqueID;
      }
    }

    public bool IsInStyle
    {
      get
      {
        return this.Element.StoryboardContainer is StyleNode;
      }
    }

    public ITypeId ElementType
    {
      get
      {
        return (ITypeId) this.Element.Type;
      }
    }

    public override DrawingBrush IconBrush
    {
      get
      {
        return IconMapper.GetDrawingBrushForType((ITypeId) this.Element.Type, false, 12, 12);
      }
    }

    public override bool HasAnimationVisual
    {
      get
      {
        if (this.IsExpanded)
          return this.HasAnimation;
        if (!this.HasAnimation)
          return this.DescendantOnlyHasAnimation;
        return true;
      }
    }

    public override SceneNode SceneNode
    {
      get
      {
        return (SceneNode) this.Element;
      }
    }

    public override object DragData
    {
      get
      {
        DocumentNodeMarkerSortedList markerList = SceneNode.GetMarkerList<SceneElement>((IEnumerable<SceneElement>) this.TimelineItemManager.ViewModel.ElementSelectionSet.Selection, true);
        if (markerList.Count > 0)
          return (object) markerList;
        return (object) null;
      }
    }

    public ElementTimelineItem(TimelineItemManager timelineItemManager, SceneElement element)
      : base(timelineItemManager)
    {
      this.InitializeElementTimelineItem(element);
    }

    private void InitializeElementTimelineItem(SceneElement element)
    {
      this.element = element;
      if (this.element == null)
        return;
      this.TimelineItemManager.RegisterTimelineItem((TimelineItem) this);
    }

    protected override void ValidateCore()
    {
      base.ValidateCore();
      this.DescendantAssignedToPart = false;
      foreach (TimelineItem timelineItem in (Collection<TimelineItem>) this.Children)
      {
        ElementTimelineItem elementTimelineItem = timelineItem as ElementTimelineItem;
        if (elementTimelineItem != null && (elementTimelineItem.CurrentPartStatus != ElementPartStatus.NotAssigned || elementTimelineItem.DescendantAssignedToPart))
        {
          this.DescendantAssignedToPart = true;
          break;
        }
      }
      this.OnPropertyChanged("CurrentPartStatus");
    }

    public override int CompareTo(TimelineItem timelineItem)
    {
      return DependencyObjectTimelineItem.CompareToCommon((TimelineItem) this, timelineItem, this.TimelineItemManager);
    }

    public void RefreshIsLocked()
    {
      this.OnPropertyChanged("IsLocked");
      this.RefreshIsAncestorLocked();
    }

    public void RefreshIsAncestorLocked()
    {
      this.OnPropertyChanged("IsAncestorLocked");
      this.OnPropertyChanged("IsSelectable");
      this.RefreshIsAncestorLocked((TimelineItem) this);
    }

    public void RefreshIsShown()
    {
      this.OnPropertyChanged("IsShown");
      this.RefreshIsAncestorHidden();
    }

    public void RefreshIsAncestorHidden()
    {
      this.OnPropertyChanged("IsAncestorHidden");
      this.RefreshIsAncestorHidden((TimelineItem) this);
    }

    public void RefreshInsertionPointProperties()
    {
      this.OnPropertyChanged("IsDynamicInsertionPoint");
      this.OnPropertyChanged("IsLockedInsertionPoint");
    }

    public override void OnZoomChanged()
    {
      base.OnZoomChanged();
      this.OnPropertyChanged("Begin");
      this.OnPropertyChanged("Duration");
    }

    public void RefreshCanLockAndHide()
    {
      this.OnPropertyChanged("CanLockAndHide");
      this.OnPropertyChanged("CanLock");
      this.OnPropertyChanged("CanHide");
    }

    public override ISceneInsertionPoint GetInsertionPoint(object data, DragDropContext context)
    {
      InsertionPointCreatorList pointCreatorList = new InsertionPointCreatorList();
      pointCreatorList.Add((IInsertionPointCreator) new EffectInsertionPointCreator(this.Element));
      pointCreatorList.Add((IInsertionPointCreator) new BehaviorInsertionPointCreator(this.Element));
      pointCreatorList.Add((IInsertionPointCreator) new DataBindingInsertionPointCreator((TimelineItem) this, (SceneNode) this.Element, (IProperty) null, context));
      pointCreatorList.Add((IInsertionPointCreator) new DefaultTimelineItemInsertionPointCreator((TimelineItem) this, (SceneNode) this.Element, context));
      return pointCreatorList.Create(data);
    }

    public override bool CanDrag()
    {
      if (!this.IsAncestorLocked)
        return this.TimelineItemManager.ViewModel.XamlDocument.IsEditable;
      return false;
    }

    protected override void OnRemoved()
    {
      base.OnRemoved();
    }

    public override void Select()
    {
      SelectionManagerPerformanceHelper.MeasurePerformanceUntilPipelinePostSceneUpdate(this.TimelineItemManager.ViewModel.DesignerContext.SelectionManager, PerformanceEvent.SelectElement);
      SceneElementSelectionSet elementSelectionSet = this.TimelineItemManager.ViewModel.ElementSelectionSet;
      if (!this.IsSelectable)
        elementSelectionSet.Clear();
      else
        elementSelectionSet.SetSelection(this.Element);
    }

    public override void ToggleSelect()
    {
      if (!this.IsSelectable)
        return;
      this.TimelineItemManager.ViewModel.ElementSelectionSet.ToggleSelection(this.Element);
    }

    public override void ExtendSelect()
    {
      if (!this.IsSelectable)
        return;
      this.TimelineItemManager.ExtendElementSelection(this.Element);
    }

    protected override ContextMenu BuildContextMenu()
    {
      if (this.IsLocked || this.IsAncestorLocked)
        return (ContextMenu) null;
      ContextMenu contextMenu = ContextMenuHelper.CreateContextMenu((ISelectionSet<SceneElement>) this.TimelineItemManager.ViewModel.ElementSelectionSet, this.TimelineItemManager.ViewModel, false);
      this.AddContextMenuItems((ItemsControl) contextMenu);
      return contextMenu;
    }

    protected virtual void AddContextMenuItems(ItemsControl contextMenu)
    {
      MenuItem menuItem = new MenuItem();
      menuItem.Command = this.renameCommand;
      if (menuItem.Command == null)
        menuItem.IsEnabled = false;
      menuItem.Header = (object) StringTable.TimelineRenameMenuItemTitle;
      menuItem.Name = "Edit_Rename";
      menuItem.SetValue(AutomationElement.IdProperty, (object) "Edit_Rename");
      int insertIndex = 0;
      bool flag = false;
      foreach (object obj in (IEnumerable) contextMenu.Items)
      {
        if (obj is ICommandBarSeparator)
        {
          contextMenu.Items.Insert(insertIndex, (object) menuItem);
          flag = true;
          break;
        }
        ++insertIndex;
      }
      if (flag)
        return;
      contextMenu.Items.Add((object) menuItem);
    }

    private void RefreshIsAncestorLocked(TimelineItem timelineItem)
    {
      for (int index = 0; index < timelineItem.Children.Count; ++index)
      {
        TimelineItem timelineItem1 = timelineItem.Children[index];
        Item3DTimelineItem item3DtimelineItem = timelineItem1 as Item3DTimelineItem;
        if (item3DtimelineItem != null)
        {
          item3DtimelineItem.RefreshIsLocked();
        }
        else
        {
          ElementTimelineItem elementTimelineItem = timelineItem1 as ElementTimelineItem;
          if (elementTimelineItem != null)
          {
            elementTimelineItem.RefreshIsAncestorLocked();
          }
          else
          {
            StyleTimelineItem styleTimelineItem = timelineItem1 as StyleTimelineItem;
            if (styleTimelineItem != null)
              styleTimelineItem.RefreshIsAncestorLocked();
            else
              this.RefreshIsAncestorLocked(timelineItem1);
          }
        }
      }
    }

    private void RefreshIsAncestorHidden(TimelineItem timelineItem)
    {
      for (int index = 0; index < timelineItem.Children.Count; ++index)
      {
        TimelineItem timelineItem1 = timelineItem.Children[index];
        Item3DTimelineItem item3DtimelineItem = timelineItem1 as Item3DTimelineItem;
        if (item3DtimelineItem != null)
        {
          item3DtimelineItem.RefreshIsShown();
        }
        else
        {
          ElementTimelineItem elementTimelineItem = timelineItem1 as ElementTimelineItem;
          EffectTimelineItem effectTimelineItem = timelineItem1 as EffectTimelineItem;
          if (elementTimelineItem != null)
            elementTimelineItem.RefreshIsAncestorHidden();
          else if (effectTimelineItem != null)
            effectTimelineItem.RefreshIsAncestorHidden();
          else
            this.RefreshIsAncestorHidden(timelineItem1);
        }
      }
    }
  }
}
