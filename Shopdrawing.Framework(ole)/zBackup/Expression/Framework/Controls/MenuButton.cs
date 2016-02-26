// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Controls.MenuButton
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.Framework.Controls
{
  public class MenuButton : Button
  {
    public static readonly DependencyProperty DropDownMenuProperty = DependencyProperty.Register("DropDownMenu", typeof (ContextMenu), typeof (MenuButton));
    public static readonly DependencyProperty PreDropDownCommandProperty = DependencyProperty.Register("PreDropDownCommand", typeof (ICommand), typeof (MenuButton));
    public static readonly DependencyProperty DropDownArrowBrushProperty = DependencyProperty.Register("DropDownArrowBrush", typeof (Brush), typeof (MenuButton));
    private bool dropDownMenuOpen;
    private UIElement dropDownButton;

    public ContextMenu DropDownMenu
    {
      get
      {
        return (ContextMenu) this.GetValue(MenuButton.DropDownMenuProperty);
      }
      set
      {
        this.SetValue(MenuButton.DropDownMenuProperty, (object) value);
      }
    }

    public ICommand PreDropDownCommand
    {
      get
      {
        return (ICommand) this.GetValue(MenuButton.PreDropDownCommandProperty);
      }
      set
      {
        this.SetValue(MenuButton.PreDropDownCommandProperty, (object) value);
      }
    }

    public Brush DropDownArrowBrush
    {
      get
      {
        return (Brush) this.GetValue(MenuButton.DropDownArrowBrushProperty);
      }
      set
      {
        this.SetValue(MenuButton.DropDownArrowBrushProperty, (object) value);
      }
    }

    protected override void OnInitialized(EventArgs e)
    {
      base.OnInitialized(e);
      this.Loaded += new RoutedEventHandler(this.MenuButton_Loaded);
      this.Unloaded += new RoutedEventHandler(this.MenuButton_Unloaded);
    }

    private void MenuButton_Unloaded(object sender, RoutedEventArgs e)
    {
      if (this.DropDownMenu != null)
        this.DropDownMenu.Closed -= new RoutedEventHandler(this.DropDownMenu_Closed);
      this.dropDownButton.PreviewMouseLeftButtonDown -= new MouseButtonEventHandler(this.DropDownButton_PreviewMouseLeftButtonDown);
    }

    private void MenuButton_Loaded(object sender, RoutedEventArgs e)
    {
      if (this.DropDownMenu != null)
        this.DropDownMenu.Closed += new RoutedEventHandler(this.DropDownMenu_Closed);
      if (this.VisualChildrenCount > 0)
        this.dropDownButton = LogicalTreeHelper.FindLogicalNode(VisualTreeHelper.GetChild((DependencyObject) this, 0), "PART_DropDownButton") as UIElement;
      if (this.dropDownButton == null)
        this.dropDownButton = (UIElement) this;
      this.dropDownButton.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(this.DropDownButton_PreviewMouseLeftButtonDown);
    }

    private void DropDownMenu_Closed(object sender, RoutedEventArgs e)
    {
      this.dropDownMenuOpen = false;
    }

    private void DropDownButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      if (this.DropDownMenu == null)
        return;
      if (this.dropDownMenuOpen)
      {
        this.DropDownMenu.IsOpen = false;
        this.dropDownMenuOpen = false;
      }
      else
      {
        if (this.PreDropDownCommand != null && this.PreDropDownCommand.CanExecute(this.CommandParameter))
          this.PreDropDownCommand.Execute(this.CommandParameter);
        this.DropDownMenu.PlacementTarget = (UIElement) this;
        this.DropDownMenu.Placement = PlacementMode.Bottom;
        this.DropDownMenu.IsOpen = true;
        this.dropDownMenuOpen = true;
      }
      e.Handled = true;
    }
  }
}
