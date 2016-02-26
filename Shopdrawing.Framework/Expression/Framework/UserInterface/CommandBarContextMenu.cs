// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.UserInterface.CommandBarContextMenu
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework.Commands;
using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Expression.Framework.UserInterface
{
  internal sealed class CommandBarContextMenu : ContextMenu, ICommandBar
  {
    private ICommandService commandService;
    private string identifier;
    private CommandBarItemCollection items;

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

    public ICommandBarItemCollection Items
    {
      get
      {
        return (ICommandBarItemCollection) this.items;
      }
    }

    public CommandBarContextMenu(ICommandService commandService)
    {
      if (commandService == null)
        throw new ArgumentNullException("commandService");
      this.commandService = commandService;
      this.items = new CommandBarItemCollection(this.commandService, CommandInvocationSource.ContextMenu, base.Items, false);
    }

    protected override void OnOpened(RoutedEventArgs e)
    {
      foreach (object obj in (IEnumerable) this.Items)
      {
        CommandBarButtonBase commandBarButtonBase = obj as CommandBarButtonBase;
        if (commandBarButtonBase != null)
          commandBarButtonBase.Update();
      }
      CommandBarHelper.CollapseUnnecessarySeparators(this.Items);
      base.OnOpened(e);
    }
  }
}
