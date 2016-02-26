// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Timeline.BehaviorTimelineItem
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.Selection;
using Microsoft.Expression.DesignSurface.Timeline.DragDrop;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.Expression.Framework.UserInterface;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.UserInterface.Timeline
{
  public class BehaviorTimelineItem : CompoundKeyFrameTimelineItem
  {
    private BehaviorBaseNode behaviorNode;

    public BehaviorBaseNode ActionNode
    {
      get
      {
        return this.behaviorNode;
      }
    }

    public ICommand RenameCommand { get; set; }

    public override DrawingBrush IconBrush
    {
      get
      {
        return IconMapper.GetDrawingBrushForType((ITypeId) this.behaviorNode.Type, false, 12, 12);
      }
    }

    public override object DragData
    {
      get
      {
        DocumentNodeMarkerSortedList markerList = SceneNode.GetMarkerList<BehaviorBaseNode>((IEnumerable<BehaviorBaseNode>) this.TimelineItemManager.ViewModel.BehaviorSelectionSet.Selection, true);
        if (markerList.Count > 0)
          return (object) markerList;
        return (object) null;
      }
    }

    public override bool ShouldBubbleKeyFrames
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

    public override string DisplayName
    {
      get
      {
        if (this.ActionNode.IsSet(this.ActionNode.NameProperty) == PropertyState.Set)
          return this.ActionNode.Name;
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, StringTable.ElementTimelineItemBracketedName, new object[1]
        {
          (object) this.ActionNode.Type.Name
        });
      }
      set
      {
        if (string.IsNullOrEmpty(value) || !string.IsNullOrEmpty(this.ActionNode.Name) && this.ActionNode.Name.Equals(value))
          return;
        using (SceneEditTransaction editTransaction = this.ActionNode.ViewModel.CreateEditTransaction(StringTable.UndoUnitElementRename))
        {
          this.ActionNode.Name = value;
          editTransaction.Commit();
        }
      }
    }

    public override string FullName
    {
      get
      {
        string uniqueId = this.ActionNode.UniqueID;
        if (!string.IsNullOrEmpty(uniqueId))
          return uniqueId;
        return this.DisplayName;
      }
    }

    public override bool IsSelectable
    {
      get
      {
        if (this.Parent != null && this.Parent.IsSelectable)
          return this.TimelinePane.HasUnlockedActiveStoryboardContainer;
        return false;
      }
    }

    public BehaviorTimelineItem(TimelineItemManager timelineItemManager, BehaviorBaseNode behaviorNode)
      : base(timelineItemManager)
    {
      this.behaviorNode = behaviorNode;
      this.TimelineItemManager.RegisterTimelineItem((SceneNode) behaviorNode, (TimelineItem) this);
    }

    public override bool CanDrag()
    {
      if (this.ActionNode.IsInDocument && this.IsSelectable)
        return this.ActionNode.ViewModel.XamlDocument.IsEditable;
      return false;
    }

    public override ISceneInsertionPoint GetInsertionPoint(object data, DragDropContext context)
    {
      return new DataBindingInsertionPointCreator((TimelineItem) this, (SceneNode) this.behaviorNode, (IProperty) null, context).Create(data);
    }

    protected override ContextMenu BuildContextMenu()
    {
      SceneViewModel viewModel = this.ActionNode.ViewModel;
      ICommandBar commandBar = viewModel.DesignerContext.CommandBarService.CommandBars.AddContextMenu("Designer_SceneContextMenu");
      commandBar.Items.AddButton("Edit_Cut", StringTable.ElementContextMenuCut);
      commandBar.Items.AddButton("Edit_Copy", StringTable.ElementContextMenuCopy);
      commandBar.Items.AddButton("Edit_Paste", StringTable.ElementContextMenuPaste);
      commandBar.Items.AddButton("Edit_Delete", StringTable.ElementContextMenuDelete);
      MenuItem menuItem = new MenuItem();
      menuItem.Command = this.RenameCommand;
      if (menuItem.Command == null)
        menuItem.IsEnabled = false;
      menuItem.Header = (object) StringTable.TimelineRenameMenuItemTitle;
      menuItem.Name = "Edit_Rename";
      menuItem.SetValue(AutomationElement.IdProperty, (object) "Edit_Rename");
      ((ItemsControl) commandBar).Items.Add((object) menuItem);
      if (viewModel.DefaultView.ViewMode == ViewMode.Design)
      {
        commandBar.Items.AddSeparator();
        commandBar.Items.AddButton("View_GoToXaml", StringTable.ViewXamlCommandName);
      }
      return (ContextMenu) commandBar;
    }

    public override void Select()
    {
      if (!this.IsSelectable)
        return;
      base.Select();
      this.TimelineItemManager.ViewModel.BehaviorSelectionSet.SetSelection(this.ActionNode);
    }

    public override void ToggleSelect()
    {
      base.ToggleSelect();
      if (!this.IsSelectable)
        return;
      BehaviorSelectionSet behaviorSelectionSet = this.TimelineItemManager.ViewModel.BehaviorSelectionSet;
      if (behaviorSelectionSet.Count != 0 && !behaviorSelectionSet.Selection.Contains(this.ActionNode))
        return;
      behaviorSelectionSet.ToggleSelection(this.ActionNode);
    }

    public override int CompareTo(TimelineItem timelineItem)
    {
      BehaviorTimelineItem behaviorTimelineItem = timelineItem as BehaviorTimelineItem;
      ElementTimelineItem elementTimelineItem = this.Parent as ElementTimelineItem;
      if (timelineItem is EffectTimelineItem)
        return 1;
      if (behaviorTimelineItem == null || elementTimelineItem == null)
        return 0;
      DocumentNode documentNode1 = this.ActionNode.DocumentNode;
      DocumentNode documentNode2 = behaviorTimelineItem.ActionNode.DocumentNode;
      bool flag1 = documentNode1.Parent != null;
      bool flag2 = documentNode2.Parent != null;
      if (flag1 && !flag2)
        return 1;
      if (!flag1 && flag2)
        return -1;
      if (!flag1 && !flag2)
        return 0;
      if (documentNode1.Parent == documentNode2.Parent)
        return documentNode1.Parent.Children.IndexOf(documentNode1) - documentNode1.Parent.Children.IndexOf(documentNode2);
      DocumentNode documentNode3 = (DocumentNode) documentNode1.Parent.Parent;
      DocumentNode documentNode4 = (DocumentNode) documentNode2.Parent.Parent;
      if (documentNode3.Parent == documentNode4.Parent)
        return documentNode3.Parent.Children.IndexOf(documentNode3) - documentNode4.Parent.Children.IndexOf(documentNode4);
      int num1 = 0;
      for (SceneNode sceneNode = (SceneNode) this.ActionNode; sceneNode != null && !(sceneNode is SceneElement); sceneNode = sceneNode.Parent)
        ++num1;
      SceneNode sceneNode1 = (SceneNode) behaviorTimelineItem.ActionNode;
      int num2 = 0;
      for (; sceneNode1 != null && !(sceneNode1 is SceneElement); sceneNode1 = sceneNode1.Parent)
        ++num2;
      return num1 - num2;
    }
  }
}
