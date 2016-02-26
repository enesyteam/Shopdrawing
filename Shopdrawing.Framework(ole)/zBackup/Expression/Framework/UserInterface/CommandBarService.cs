// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.UserInterface.CommandBarService
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework.Commands;
using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Expression.Framework.UserInterface
{
  public class CommandBarService : DockPanel, ICommandBarService
  {
    private ICommandService commandService;
    private CommandBarCollection commandBars;

    public ICommandBarCollection CommandBars
    {
      get
      {
        return (ICommandBarCollection) this.commandBars;
      }
    }

    public bool HasOpenSubmenu
    {
      get
      {
        foreach (ItemsControl itemsControl in this.commandBars)
        {
          if (this.ContainsOpenSubmenu(itemsControl.Items))
            return true;
        }
        return false;
      }
    }

    public CommandBarService(ICommandService commandService)
    {
      this.commandService = commandService;
      this.commandBars = new CommandBarCollection(this, this.commandService);
    }

    internal void Update()
    {
      this.Children.Clear();
      foreach (ICommandBar commandBar1 in this.commandBars)
      {
        CommandBar commandBar2 = commandBar1 as CommandBar;
        if (commandBar2 != null)
          this.Children.Add((UIElement) commandBar2);
      }
    }

    private bool ContainsOpenSubmenu(ItemCollection items)
    {
      foreach (object obj in (IEnumerable) items)
      {
        MenuItem menuItem = obj as MenuItem;
        if (menuItem != null && (menuItem.IsSubmenuOpen || this.ContainsOpenSubmenu(menuItem.Items)))
          return true;
      }
      return false;
    }
  }
}
