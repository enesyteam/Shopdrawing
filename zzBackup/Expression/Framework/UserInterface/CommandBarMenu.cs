// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.UserInterface.CommandBarMenu
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Framework.Diagnostics;
using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Expression.Framework.UserInterface
{
  internal class CommandBarMenu : MenuItem, ICommandBarMenu, ICommandBarItem, ICommandBar
  {
    private string identifier;
    private CommandBarItemCollection items;
    private bool isCollapsible;

    public string Identifier
    {
      get
      {
        return this.identifier;
      }
      set
      {
        this.identifier = value;
      }
    }

    public string Text
    {
      get
      {
        return this.Header as string;
      }
      set
      {
        this.Header = (object) value;
      }
    }

    public ICommandBarItemCollection Items
    {
      get
      {
        return (ICommandBarItemCollection) this.items;
      }
    }

    public CommandBarMenu(ICommandService commandService, bool displayShortcut, bool isCollapsible)
    {
      this.isCollapsible = isCollapsible;
      this.items = new CommandBarItemCollection(commandService, CommandInvocationSource.MenuItem, base.Items, displayShortcut);
    }

    protected override void OnSubmenuOpened(RoutedEventArgs e)
    {
      PerformanceUtility.MeasurePerformanceUntilRender(PerformanceEvent.OpenCommandBarMenu);
      this.UpdateState();
    }

    internal void UpdateState()
    {
      bool flag = true;
      foreach (object obj in (IEnumerable) this.Items)
      {
        CommandBarButtonBase commandBarButtonBase = obj as CommandBarButtonBase;
        if (commandBarButtonBase != null)
        {
          commandBarButtonBase.Update();
        }
        else
        {
          CommandBarMenu commandBarMenu = obj as CommandBarMenu;
          if (commandBarMenu != null)
            commandBarMenu.UpdateState();
        }
        MenuItem menuItem = obj as MenuItem;
        if (flag && menuItem != null && menuItem.Visibility == Visibility.Visible)
          flag = false;
      }
      CommandBarHelper.CollapseUnnecessarySeparators(this.Items);
      this.IsEnabled = !flag;
      if (!this.isCollapsible)
        return;
      if (flag)
        this.Visibility = Visibility.Collapsed;
      else
        this.Visibility = Visibility.Visible;
    }
  }
}
