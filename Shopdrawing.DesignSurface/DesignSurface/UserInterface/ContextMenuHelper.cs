// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.ContextMenuHelper
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Geometry;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.SceneCommands;
using Microsoft.Expression.DesignSurface.Selection;
using Microsoft.Expression.DesignSurface.Tools.Text;
using Microsoft.Expression.DesignSurface.UserInterface.SkinEditing;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.Expression.Framework.UserInterface;
using Microsoft.Windows.Design.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  internal static class ContextMenuHelper
  {
    private static DependencyPropertyDescriptor contextMenuIsOpenDescriptor = DependencyPropertyDescriptor.FromProperty(ContextMenu.IsOpenProperty, typeof (ContextMenu));

    public static void InvokeContextMenu(UIElement scopeElement, ISelectionSet<SceneElement> selection, SceneViewModel sceneViewModel, Point contextMenuPosition, ContextMenuHelper.ContextMenuType menuType)
    {
      switch (menuType)
      {
        case ContextMenuHelper.ContextMenuType.PrimaryContextMenu:
          ContextMenuHelper.InvokeContextMenu(scopeElement, selection, sceneViewModel, contextMenuPosition);
          break;
        case ContextMenuHelper.ContextMenuType.SelectionContextMenu:
          ContextMenuHelper.InvokeSelectionContextMenu(scopeElement, sceneViewModel, contextMenuPosition);
          break;
      }
    }

    public static void ResetContextMenuPropertiesOnClosing(object sender, EventArgs e)
    {
      ContextMenu contextMenu = (ContextMenu) sender;
      if (contextMenu.IsOpen)
        return;
      contextMenu.SetCurrentValue(FrameworkElement.UseLayoutRoundingProperty, (object) false);
      contextMenu.SetCurrentValue(TextOptions.TextFormattingModeProperty, (object) TextFormattingMode.Ideal);
    }

    public static void TurnOffPopupAnimationOnOpened(object sender, RoutedEventArgs e)
    {
      Popup popup = ((FrameworkElement) sender).Parent as Popup;
      if (popup == null)
        return;
      popup.SetCurrentValue(Popup.PopupAnimationProperty, (object) PopupAnimation.None);
    }

    private static void InvokeSelectionContextMenu(UIElement scopeElement, SceneViewModel sceneViewModel, Point contextMenuPosition)
    {
      ContextMenu selectionContextMenu = ContextMenuHelper.CreateSelectionContextMenu(sceneViewModel);
      ContextMenuHelper.InvokeContextMenu(scopeElement, contextMenuPosition, selectionContextMenu);
    }

    private static void InvokeContextMenu(UIElement scopeElement, ISelectionSet<SceneElement> selection, SceneViewModel sceneViewModel, Point contextMenuPosition)
    {
      PerformanceUtility.StartPerformanceSequence(PerformanceEvent.ContextMenuRender, "SceneElement Context Menu");
      ContextMenu contextMenu = ContextMenuHelper.CreateContextMenu(selection, sceneViewModel, true);
      if (contextMenu != null)
      {
        ContextMenuHelper.contextMenuIsOpenDescriptor.AddValueChanged((object) contextMenu, new EventHandler(ContextMenuHelper.ResetContextMenuPropertiesOnClosing));
        contextMenu.Opened += new RoutedEventHandler(ContextMenuHelper.TurnOffPopupAnimationOnOpened);
        contextMenu.Opened += new RoutedEventHandler(ContextMenuHelper.ContextMenu_Opened);
        contextMenu.Closed += new RoutedEventHandler(ContextMenuHelper.ContextMenu_Closed);
      }
      ContextMenuHelper.InvokeContextMenu(scopeElement, contextMenuPosition, contextMenu);
    }

    private static void ContextMenu_Opened(object sender, RoutedEventArgs e)
    {
      PerformanceUtility.EndPerformanceSequence(PerformanceEvent.ContextMenuRender, "SceneElement Context Menu");
    }

    private static void ContextMenu_Closed(object sender, RoutedEventArgs e)
    {
      ContextMenu contextMenu = (ContextMenu) sender;
      contextMenu.Closed -= new RoutedEventHandler(ContextMenuHelper.ContextMenu_Closed);
      contextMenu.Opened -= new RoutedEventHandler(ContextMenuHelper.ContextMenu_Opened);
      contextMenu.Opened -= new RoutedEventHandler(ContextMenuHelper.TurnOffPopupAnimationOnOpened);
      ContextMenuHelper.contextMenuIsOpenDescriptor.RemoveValueChanged((object) contextMenu, new EventHandler(ContextMenuHelper.ResetContextMenuPropertiesOnClosing));
    }

    private static void InvokeContextMenu(UIElement scopeElement, Point contextMenuPosition, ContextMenu contextMenu)
    {
      if (contextMenu == null)
        return;
      contextMenu.Placement = PlacementMode.RelativePoint;
      contextMenu.PlacementTarget = scopeElement;
      contextMenu.PlacementRectangle = new Rect(contextMenuPosition.X, contextMenuPosition.Y, 0.0, 0.0);
      contextMenu.IsOpen = true;
    }

    private static ContextMenu CreateSelectionContextMenu(SceneViewModel viewModel)
    {
      viewModel.DesignerContext.CommandBarService.CommandBars.Remove("Designer_SceneSelectionContextMenu");
      ICommandBar commandBar = viewModel.DesignerContext.CommandBarService.CommandBars.AddContextMenu("Designer_SceneSelectionContextMenu");
      if (new ToggleLockInsertionPointCommand(viewModel).IsEnabled)
      {
        commandBar.Items.AddCheckBox("Edit_LockInsertionPoint", StringTable.SelectionContextMenuLockInsertionPoint);
        commandBar.Items.AddSeparator();
      }
      commandBar.Items.AddInPlaceDynamicMenuItemsFrom("Edit_SetCurrentSelection");
      ContextMenu contextMenu = (ContextMenu) commandBar;
      contextMenu.MinWidth = 150.0;
      return contextMenu;
    }

    public static ContextMenu CreateContextMenu(ISelectionSet<SceneElement> selection, SceneViewModel viewModel, bool isOnArtboard)
    {
      viewModel.DesignerContext.CommandBarService.CommandBars.Remove("Designer_SceneContextMenu");
      ICommandBar commandBar = viewModel.DesignerContext.CommandBarService.CommandBars.AddContextMenu("Designer_SceneContextMenu");
      commandBar.Items.AddButton("Edit_Cut", StringTable.ElementContextMenuCut);
      commandBar.Items.AddButton("Edit_Copy", StringTable.ElementContextMenuCopy);
      commandBar.Items.AddButton("Edit_Paste", StringTable.ElementContextMenuPaste);
      commandBar.Items.AddButton("Edit_Delete", StringTable.ElementContextMenuDelete);
      ContextMenuHelper.AddSeparatorHelper.AddSeparator(commandBar);
      ContextMenuHelper.AddExtensibleMenuItems(commandBar, viewModel, selection.PrimarySelection != null ? selection.PrimarySelection.TargetType : (Type) null);
      if (new CopyToResourceCommand(viewModel).IsEnabled)
        commandBar.Items.AddButton("Edit_MakeTileBrush_CopyToResource", StringTable.ElementContextMenuCopyToResource);
      if (new MoveToResourceCommand(viewModel).IsEnabled)
        commandBar.Items.AddButton("Edit_MakeTileBrush_MoveToResource", StringTable.ElementContextMenuMoveToResource);
      bool flag1 = false;
      bool flag2 = false;
      bool flag3 = true;
      foreach (SceneElement element in selection.Selection)
      {
        if (!flag1 && PathConversionHelper.CanConvert(element))
        {
          flag1 = true;
          flag2 = true;
        }
        if (!flag2 && element.IsSet(Base2DElement.ClipProperty) == PropertyState.Set)
          flag1 = true;
        if (flag3 && !PlatformTypes.UIElement.IsAssignableFrom((ITypeId) element.Type))
          flag3 = false;
      }
      if (flag3)
      {
        ICommandBarMenu commandBarMenu1 = commandBar.Items.AddMenu("Order", StringTable.ElementContextMenuOrder);
        commandBarMenu1.Items.AddButton("Edit_Order_BringToFront", StringTable.ElementContextMenuOrderBringToFront);
        commandBarMenu1.Items.AddButton("Edit_Order_BringForward", StringTable.ElementContextMenuOrderBringForward);
        commandBarMenu1.Items.AddButton("Edit_Order_SendBackward", StringTable.ElementContextMenuOrderSendBackward);
        commandBarMenu1.Items.AddButton("Edit_Order_SendToBack", StringTable.ElementContextMenuOrderSendToBack);
        ICommandBarMenu commandBarMenu2 = commandBar.Items.AddMenu("Align", StringTable.ElementContextMenuAlign);
        commandBarMenu2.Items.AddButton("Edit_Align_AlignLeft", StringTable.ElementContextMenuAlignLeft);
        commandBarMenu2.Items.AddButton("Edit_Align_AlignCenter", StringTable.ElementContextMenuAlignCenter);
        commandBarMenu2.Items.AddButton("Edit_Align_AlignRight", StringTable.ElementContextMenuAlignRight);
        commandBarMenu2.Items.AddSeparator();
        commandBarMenu2.Items.AddButton("Edit_Align_AlignTop", StringTable.ElementContextMenuAlignTop);
        commandBarMenu2.Items.AddButton("Edit_Align_AlignMiddle", StringTable.ElementContextMenuAlignMiddle);
        commandBarMenu2.Items.AddButton("Edit_Align_AlignBottom", StringTable.ElementContextMenuAlignBottom);
        ICommandBarMenu commandBarMenu3 = commandBar.Items.AddMenu("AutoSize", StringTable.ElementContextMenuAutoSize);
        commandBarMenu3.Items.AddCheckBox("Edit_AutoSize_Horizontal", StringTable.ElementContextMenuAutoSizeWidth);
        commandBarMenu3.Items.AddCheckBox("Edit_AutoSize_Vertical", StringTable.ElementContextMenuAutoSizeHeight);
        commandBarMenu3.Items.AddCheckBox("Edit_AutoSize_Both", StringTable.ElementContextMenuAutoSizeBoth);
        commandBarMenu3.Items.AddButton("Edit_AutoSize_Fill", StringTable.ElementContextMenuAutoSizeFill);
      }
      if (flag3)
      {
        ContextMenuHelper.AddSeparatorHelper.AddSeparator(commandBar);
        commandBar.Items.AddButton("Edit_Group", StringTable.ElementContextMenuGroup);
        commandBar.Items.AddDynamicMenu("Edit_GroupInto", StringTable.GroupIntoCommandName);
        commandBar.Items.AddButton("Edit_Ungroup", StringTable.ElementContextMenuUngroup);
      }
      if (ChangeLayoutTypeFlyoutCommand.ShouldShowChangeLayoutTypeMenu(selection))
        commandBar.Items.AddDynamicMenu("Edit_ChangeLayoutTypes", StringTable.ChangeLayoutTypeCommandName);
      if (new ToggleLockInsertionPointCommand(viewModel).IsEnabled)
        commandBar.Items.AddCheckBox("Edit_LockInsertionPoint", StringTable.SelectionContextMenuLockInsertionPoint);
      if (isOnArtboard)
        commandBar.Items.AddDynamicMenu("Edit_SetCurrentSelection", StringTable.ElementContextMenuSetCurrentSelection);
      ContextMenuHelper.AddSeparatorHelper.AddSeparator(commandBar);
      if (flag2)
      {
        ICommandBarMenu menu = commandBar.Items.AddMenu("Combine", StringTable.ElementContextMenuToolsMenuCombine);
        menu.Items.AddButton("Tools_Combine_Unite", StringTable.ElementContextMenuToolsMenuUnite);
        menu.Items.AddButton("Tools_Combine_Divide", StringTable.ElementContextMenuToolsMenuDivide);
        menu.Items.AddButton("Tools_Combine_Intersect", StringTable.ElementContextMenuToolsMenuIntersect);
        menu.Items.AddButton("Tools_Combine_Subtract", StringTable.ElementContextMenuToolsMenuSubtract);
        menu.Items.AddButton("Tools_Combine_ExcludeOverlap", StringTable.ElementContextMenuToolsMenuExcludeOverlap);
        if (ContextMenuHelper.IsSubmenuDisabled(menu))
          commandBar.Items.Remove((ICommandBarItem) menu);
      }
      if (flag1)
      {
        ICommandBarMenu menu = commandBar.Items.AddMenu("Path", StringTable.ElementContextMenuObjectMenuPath);
        menu.Items.AddButton("Edit_ConvertToPath", StringTable.ElementContextMenuObjectMenuConvertToPath);
        menu.Items.AddButton("Edit_ConvertToMotionPath", StringTable.ElementContextMenuObjectMenuConvertToMotionPath);
        menu.Items.AddButton("Edit_MakeLayoutPath", StringTable.ElementContextMenuObjectMenuMakeLayoutPath);
        menu.Items.AddButton("Edit_MakeClippingPath", StringTable.ElementContextMenuObjectMenuMakeClippingPath);
        menu.Items.AddButton("Edit_RemoveClippingPath", StringTable.ElementContextMenuObjectMenuRemoveClippingPath);
        menu.Items.AddButton("Edit_MakeCompoundPath", StringTable.ElementContextMenuObjectMenuMakeCompoundPath);
        menu.Items.AddButton("Edit_ReleaseCompoundPath", StringTable.ElementContextMenuObjectMenuReleaseCompoundPath);
        if (ContextMenuHelper.IsSubmenuDisabled(menu))
          commandBar.Items.Remove((ICommandBarItem) menu);
      }
      ContextMenuHelper.AddSeparatorHelper.AddSeparator(commandBar);
      if (selection.Count == 1)
      {
        SceneElement primarySelection = selection.PrimarySelection;
        int count = commandBar.Items.Count;
        primarySelection.AddCustomContextMenuCommands(commandBar);
        if (count == commandBar.Items.Count)
          commandBar.Items.RemoveAt(count - 1);
        if (primarySelection.DocumentNode != null)
        {
          if (TextEditProxyFactory.IsEditableElement(primarySelection))
            commandBar.Items.AddButton("Edit_EditText", StringTable.ElementContextMenuEditText);
          if (flag3)
          {
            ContextMenuHelper.AddSeparatorHelper.AddSeparator(commandBar);
            commandBar.Items.AddButton("Edit_MakeButton", StringTable.MakeControlCommandName);
            if (viewModel.PartsModel.IsEnabled && selection.Count == 1)
            {
              if (viewModel.PartsModel.GetPartStatus((SceneNode) selection.PrimarySelection) != PartStatus.Assigned)
              {
                Type targetElementType = ((FrameworkTemplateElement) viewModel.ActiveEditingContainer).TargetElementType;
                commandBar.Items.AddDynamicMenu("Edit_MakePart", string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.MakeIntoPartMenuEnabled, new object[1]
                {
                  (object) targetElementType.Name
                }));
              }
              else
                commandBar.Items.AddButton("Edit_MakePart", StringTable.MakeIntoPartMenuDisabled);
              commandBar.Items.AddButton("Edit_ClearPart", StringTable.ClearPartAssignmentCommandName);
            }
            commandBar.Items.AddButton("Edit_MakeUserControl", StringTable.MakeUserControlCommandName);
            commandBar.Items.AddButton("Edit_MakeCompositionScreen", StringTable.MakeCompositionScreenCommandName);
            if (primarySelection.DocumentNode.Type.XamlSourcePath != null)
              commandBar.Items.AddButton("Edit_EditControl", StringTable.ElementContextMenuEditControl);
          }
        }
        ITypeId type = (ITypeId) primarySelection.Type;
        StyleNode styleNode = primarySelection as StyleNode;
        if (styleNode != null)
          type = (ITypeId) styleNode.StyleTargetTypeId;
        if (PlatformTypes.Control.IsAssignableFrom(type) || PlatformTypes.Page.IsAssignableFrom(type))
        {
          ContextMenuHelper.AddSeparatorHelper.AddSeparator(commandBar);
          ICommandBarMenu commandBarMenu = commandBar.Items.AddMenu("Template", StringTable.ElementContextMenuEditTemplateMenuName);
          commandBarMenu.Items.AddButton("Edit_Template_EditExisting", StringTable.EditExistingTemplateCommandName);
          commandBarMenu.Items.AddButton("Edit_Template_EditCopy", StringTable.EditClonedStyleCommandName);
          commandBarMenu.Items.AddButton("Edit_Template_EditNew", StringTable.EditEmptyStyleCommandName);
          commandBarMenu.Items.AddSeparator();
          commandBarMenu.Items.AddDynamicMenu("Edit_Template_LocalResource", StringTable.ElementContextMenuEditLocalResource);
          commandBar.Items.AddDynamicMenu("Edit_EditTemplates", StringTable.ElementContextMenuEditTemplates);
        }
        else if (PlatformTypes.FrameworkElement.IsAssignableFrom(type))
        {
          ContextMenuHelper.AddSeparatorHelper.AddSeparator(commandBar);
          ICommandBarMenu commandBarMenu = commandBar.Items.AddMenu("Style", StringTable.ElementContextMenuEditStyleMenuName);
          commandBarMenu.Items.AddButton("Edit_Style_EditExisting", StringTable.ElementContextMenuEditExistingStyle);
          commandBarMenu.Items.AddButton("Edit_Style_EditCopy", StringTable.ElementContextMenuEditCopyStyle);
          commandBarMenu.Items.AddButton("Edit_Style_EditNew", StringTable.ElementContextMenuEditNewStyle);
          commandBarMenu.Items.AddSeparator();
          commandBarMenu.Items.AddDynamicMenu("Edit_Style_LocalResource", StringTable.ElementContextMenuEditLocalResource);
        }
      }
      else if (flag3)
      {
        ContextMenuHelper.AddSeparatorHelper.AddSeparator(commandBar);
        commandBar.Items.AddButton("Edit_MakeUserControl", StringTable.MakeUserControlCommandName);
        commandBar.Items.AddButton("Edit_MakeCompositionScreen", StringTable.MakeCompositionScreenCommandName);
      }
      if (viewModel.DefaultView.ViewMode == ViewMode.Design)
      {
        ContextMenuHelper.AddSeparatorHelper.AddSeparator(commandBar);
        commandBar.Items.AddButton("View_GoToXaml", StringTable.ViewXamlCommandName);
      }
      IPrototypingService prototypingService = viewModel.DesignerContext.PrototypingService;
      if (prototypingService != null)
        prototypingService.AddElementContextMenuItems(commandBar, (IEnumerable<SceneElement>) selection.Selection, viewModel, isOnArtboard);
      ContextMenu contextMenu = (ContextMenu) commandBar;
      contextMenu.MinWidth = 200.0;
      ContextMenuHelper.AddSeparatorHelper.RemoveLastSeparatorIfNeeded(commandBar);
      return contextMenu;
    }

    private static bool IsSubmenuDisabled(ICommandBarMenu menu)
    {
      foreach (ICommandBarItem commandBarItem in (IEnumerable) menu.Items)
      {
        MenuItem menuItem = commandBarItem as MenuItem;
        if (menuItem != null && menuItem.IsEnabled)
          return false;
      }
      return true;
    }

    private static void AddExtensibleMenuItems(ICommandBar contextMenu, SceneViewModel viewModel, Type selectionType)
    {
      if (!(selectionType != (Type) null))
        return;
      bool anythingAdded = false;
      bool lastWasSeperator = true;
      viewModel.ExtensibilityManager.UpdateSelection();
      foreach (ContextMenuProvider contextMenuProvider in viewModel.ExtensibilityManager.FeatureManager.CreateFeatureProviders(typeof (ContextMenuProvider), selectionType))
      {
        contextMenuProvider.Update(viewModel.ExtensibilityManager.EditingContext);
        foreach (Microsoft.Windows.Design.Interaction.MenuBase menuItem in (Collection<Microsoft.Windows.Design.Interaction.MenuBase>) contextMenuProvider.Items)
          ContextMenuHelper.BuildExtensibleMenuItem(contextMenu, menuItem, ref lastWasSeperator, ref anythingAdded);
      }
      if (!anythingAdded || lastWasSeperator)
        return;
      contextMenu.Items.AddSeparator();
    }

    private static void BuildExtensibleMenuItem(ICommandBar parentMenu, Microsoft.Windows.Design.Interaction.MenuBase menuItem, ref bool lastWasSeperator, ref bool anythingAdded)
    {
      MenuAction menuAction = menuItem as MenuAction;
      MenuGroup menuGroup = menuItem as MenuGroup;
      if (menuAction != null)
      {
        if (!menuAction.Visible)
          return;
        ICommandBarButton commandBarButton = parentMenu.Items.AddButton("ExtensibilityCommand");
        commandBarButton.Text = menuAction.DisplayName;
        MenuItem menuItem1 = (MenuItem) commandBarButton;
        menuItem1.Command = menuAction.Command;
        menuItem1.IsEnabled = menuAction.Enabled;
        menuItem1.IsCheckable = menuAction.Checkable;
        menuItem1.IsChecked = menuAction.Checked;
        lastWasSeperator = false;
        anythingAdded = true;
      }
      else
      {
        if (menuGroup == null)
          return;
        ICommandBar parentMenu1;
        if (menuGroup.HasDropDown)
        {
          parentMenu1 = (ICommandBar) parentMenu.Items.AddMenu("ExtensibilityMenu", menuGroup.DisplayName);
          anythingAdded = true;
          lastWasSeperator = false;
        }
        else
        {
          parentMenu1 = parentMenu;
          if (anythingAdded && !lastWasSeperator)
          {
            parentMenu.Items.AddSeparator();
            lastWasSeperator = true;
          }
        }
        foreach (Microsoft.Windows.Design.Interaction.MenuBase menuItem1 in (Collection<Microsoft.Windows.Design.Interaction.MenuBase>) menuGroup.Items)
          ContextMenuHelper.BuildExtensibleMenuItem(parentMenu1, menuItem1, ref lastWasSeperator, ref anythingAdded);
        if (menuGroup.HasDropDown)
          return;
        if (!lastWasSeperator && anythingAdded)
        {
          parentMenu.Items.AddSeparator();
          lastWasSeperator = true;
        }
        else
          lastWasSeperator = false;
      }
    }

    public enum ContextMenuType
    {
      PrimaryContextMenu,
      SelectionContextMenu,
    }

    private static class AddSeparatorHelper
    {
      public static void AddSeparator(ICommandBar contextMenu)
      {
        if (contextMenu.Items.Count == 0 || contextMenu.Items[contextMenu.Items.Count - 1] is ICommandBarSeparator)
          return;
        contextMenu.Items.AddSeparator();
      }

      public static void RemoveLastSeparatorIfNeeded(ICommandBar contextMenu)
      {
        while (contextMenu.Items.Count > 0 && contextMenu.Items[contextMenu.Items.Count - 1] is ICommandBarSeparator)
          contextMenu.Items.RemoveAt(contextMenu.Items.Count - 1);
      }
    }
  }
}
