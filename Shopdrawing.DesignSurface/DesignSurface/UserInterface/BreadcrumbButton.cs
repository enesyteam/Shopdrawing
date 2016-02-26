// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.BreadcrumbButton
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.UserInterface;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  public sealed class BreadcrumbButton : Button
  {
    private DependencyPropertyDescriptor contextMenuIsOpenDescriptor = DependencyPropertyDescriptor.FromProperty(ContextMenu.IsOpenProperty, typeof (ContextMenu));
    private static readonly RoutedCommand clickBreadcrumbCommand = new RoutedCommand("ClickBreadcrumbCommand", typeof (BreadcrumbButton));
    private ContextMenu contextMenu;
    private bool isMenuOpen;

    public static RoutedCommand ClickBreadcrumbCommand
    {
      get
      {
        return BreadcrumbButton.clickBreadcrumbCommand;
      }
    }

    private Breadcrumb BreadcrumbModel
    {
      get
      {
        return this.DataContext as Breadcrumb;
      }
    }

    private SceneViewModel ViewModel
    {
      get
      {
        return this.BreadcrumbModel.LocalViewModel;
      }
    }

    public BreadcrumbButton()
    {
      this.CommandBindings.Add(new CommandBinding((ICommand) BreadcrumbButton.ClickBreadcrumbCommand, new ExecutedRoutedEventHandler(this.ClickBreadcrumbCommand_Execute)));
    }

    private void ClickBreadcrumbCommand_Execute(object target, ExecutedRoutedEventArgs args)
    {
      if (this.BreadcrumbModel == null)
        return;
      this.BreadcrumbModel.Activate();
    }

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
      if (this.BreadcrumbModel != null && this.BreadcrumbModel.IsMenuEnabled && e.ChangedButton == MouseButton.Left)
      {
        if (this.isMenuOpen)
          this.contextMenu.IsOpen = false;
        else
          this.ShowContextMenu();
        e.Handled = true;
      }
      base.OnMouseDown(e);
    }

    private void ContextMenuOpened(object sender, RoutedEventArgs e)
    {
      this.isMenuOpen = true;
    }

    private void ContextMenuClosed(object sender, RoutedEventArgs e)
    {
      this.isMenuOpen = false;
      this.contextMenu.Closed -= new RoutedEventHandler(this.ContextMenuClosed);
      this.contextMenu.Opened -= new RoutedEventHandler(this.ContextMenuOpened);
      this.contextMenu.Opened -= new RoutedEventHandler(ContextMenuHelper.TurnOffPopupAnimationOnOpened);
      this.contextMenuIsOpenDescriptor.RemoveValueChanged((object) this.contextMenu, new EventHandler(ContextMenuHelper.ResetContextMenuPropertiesOnClosing));
      this.contextMenu = (ContextMenu) null;
    }

    private void ShowContextMenu()
    {
      this.ViewModel.DesignerContext.CommandBarService.CommandBars.Remove("BreadcrumbBar_BreadcrumbContextMenu");
      ICommandBar commandBar = this.ViewModel.DesignerContext.CommandBarService.CommandBars.AddContextMenu("BreadcrumbBar_BreadcrumbContextMenu");
      ICommandBarMenu commandBarMenu = commandBar.Items.AddMenu("Template", StringTable.ElementContextMenuEditTemplateMenuName);
      commandBarMenu.Items.AddButton("Edit_Template_EditExisting", StringTable.EditExistingTemplateCommandName);
      commandBarMenu.Items.AddButton("Edit_Template_EditCopy", StringTable.EditClonedStyleCommandName);
      commandBarMenu.Items.AddButton("Edit_Template_EditNew", StringTable.EditEmptyStyleCommandName);
      commandBarMenu.Items.AddSeparator();
      commandBarMenu.Items.AddDynamicMenu("Edit_Template_LocalResource", StringTable.ElementContextMenuEditLocalResource);
      commandBar.Items.AddDynamicMenu("Edit_EditTemplates", StringTable.ElementContextMenuEditTemplates);
      this.contextMenu = (ContextMenu) commandBar;
      this.contextMenu.MinWidth = 200.0;
      this.contextMenu.PlacementTarget = (UIElement) this;
      this.contextMenu.Placement = PlacementMode.Bottom;
      this.contextMenu.HorizontalOffset = 4.0;
      this.contextMenu.VerticalOffset = -2.0;
      this.contextMenuIsOpenDescriptor.AddValueChanged((object) this.contextMenu, new EventHandler(ContextMenuHelper.ResetContextMenuPropertiesOnClosing));
      this.contextMenu.Opened += new RoutedEventHandler(ContextMenuHelper.TurnOffPopupAnimationOnOpened);
      this.contextMenu.Opened += new RoutedEventHandler(this.ContextMenuOpened);
      this.contextMenu.Closed += new RoutedEventHandler(this.ContextMenuClosed);
      this.contextMenu.IsOpen = true;
    }
  }
}
