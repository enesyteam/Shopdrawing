// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.UserInterface.CommandBarItemCollection
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Framework.Diagnostics;
using System;
using System.Collections;
using System.Windows.Controls;
using System.Windows.Input;

namespace Microsoft.Expression.Framework.UserInterface
{
  internal class CommandBarItemCollection : ICommandBarItemCollection, ICollection, IEnumerable
  {
    private ICommandService commandService;
    private CommandInvocationSource invocationSource;
    private ItemCollection items;
    private bool displayShortcut;

    public ICommandBarItem this[int index]
    {
      get
      {
        return (ICommandBarItem) this.items[index];
      }
    }

    public int Count
    {
      get
      {
        return this.items.Count;
      }
    }

    public bool IsSynchronized
    {
      get
      {
        return false;
      }
    }

    public object SyncRoot
    {
      get
      {
        return (object) this;
      }
    }

    public CommandBarItemCollection(ICommandService commandService, CommandInvocationSource invocationSource, ItemCollection items)
      : this(commandService, invocationSource, items, true)
    {
    }

    public CommandBarItemCollection(ICommandService commandService, CommandInvocationSource invocationSource, ItemCollection items, bool displayShortcut)
    {
      this.commandService = commandService;
      this.invocationSource = invocationSource;
      this.items = items;
      this.displayShortcut = displayShortcut;
    }

    public void Add(ICommandBarItem item)
    {
      this.items.Add((object) item);
    }

    public ICommandBarButton AddButton(string commandName)
    {
      CommandBarButton button = this.CreateButton(commandName);
      this.Add((ICommandBarItem) button);
      return (ICommandBarButton) button;
    }

    public ICommandBarButton AddButton(string commandName, string text)
    {
      ICommandBarButton commandBarButton = this.AddButton(commandName);
      commandBarButton.Text = text;
      return commandBarButton;
    }

    public ICommandBarButton AddButton(string commandName, string text, params KeyBinding[] keyBindings)
    {
      ICommandBarButton commandBarButton = this.AddButton(commandName, text);
      if (keyBindings != null)
        this.commandService.SetCommandPropertyDefault(commandName, "Shortcuts", (object) keyBindings);
      return commandBarButton;
    }

    public ICommandBarCheckBox AddCheckBox(string commandName)
    {
      CommandBarCheckBox checkBox = this.CreateCheckBox(commandName);
      this.Add((ICommandBarItem) checkBox);
      return (ICommandBarCheckBox) checkBox;
    }

    public ICommandBarCheckBox AddCheckBox(string commandName, string text)
    {
      ICommandBarCheckBox commandBarCheckBox = this.AddCheckBox(commandName);
      commandBarCheckBox.Text = text;
      return commandBarCheckBox;
    }

    public ICommandBarCheckBox AddCheckBox(string commandName, string text, params KeyBinding[] keyBindings)
    {
      ICommandBarCheckBox commandBarCheckBox = this.AddCheckBox(commandName, text);
      if (keyBindings != null)
        this.commandService.SetCommandPropertyDefault(commandName, "Shortcuts", (object) keyBindings);
      return commandBarCheckBox;
    }

    public ICommandBarMenu AddMenu(string identifier, string text, bool isCollapsible)
    {
      CommandBarMenu menu = this.CreateMenu(identifier, text, isCollapsible);
      this.Add((ICommandBarItem) menu);
      return (ICommandBarMenu) menu;
    }

    public ICommandBarMenu AddMenu(string identifier, string text)
    {
      return this.AddMenu(identifier, text, false);
    }

    public ICommandBarDynamicMenu AddDynamicMenu(string commandName)
    {
      CommandBarDynamicMenu dynamicMenu = this.CreateDynamicMenu(commandName);
      this.Add((ICommandBarItem) dynamicMenu);
      return (ICommandBarDynamicMenu) dynamicMenu;
    }

    public ICommandBarDynamicMenu AddDynamicMenu(string commandName, string text)
    {
      ICommandBarDynamicMenu commandBarDynamicMenu = this.AddDynamicMenu(commandName);
      commandBarDynamicMenu.Text = text;
      return commandBarDynamicMenu;
    }

    public ICommandBarDynamicMenu AddDynamicMenu(string commandName, string text, params KeyBinding[] keyBindings)
    {
      ICommandBarDynamicMenu commandBarDynamicMenu = this.AddDynamicMenu(commandName, text);
      if (keyBindings != null)
        this.commandService.SetCommandPropertyDefault(commandName, "Shortcuts", (object) keyBindings);
      return commandBarDynamicMenu;
    }

    public void AddInPlaceDynamicMenuItemsFrom(string commandName)
    {
      if (!(bool) this.commandService.GetCommandProperty(commandName, "IsEnabled"))
        return;
      IEnumerable enumerable = (IEnumerable) this.commandService.GetCommandProperty(commandName, "Items");
      if (enumerable == null)
        return;
      foreach (object newItem in enumerable)
      {
        if (newItem is MenuItem)
          this.items.Add(newItem);
      }
    }

    public ICommandBarSeparator AddSeparator()
    {
      ICommandBarSeparator commandBarSeparator = (ICommandBarSeparator) new CommandBarSeparator();
      this.Add((ICommandBarItem) commandBarSeparator);
      return commandBarSeparator;
    }

    public void Clear()
    {
      this.items.Clear();
    }

    private CommandBarButton CreateButton(string commandName)
    {
      CommandBarButton commandBarButton = new CommandBarButton(this.commandService, commandName, this.invocationSource);
      commandBarButton.DisplayShortcut = this.displayShortcut;
      commandBarButton.Name = commandName;
      commandBarButton.SetValue(AutomationElement.IdProperty, (object) commandName);
      return commandBarButton;
    }

    private CommandBarCheckBox CreateCheckBox(string commandName)
    {
      CommandBarCheckBox commandBarCheckBox = new CommandBarCheckBox(this.commandService, commandName);
      commandBarCheckBox.DisplayShortcut = this.displayShortcut;
      commandBarCheckBox.Name = commandName;
      return commandBarCheckBox;
    }

    private CommandBarMenu CreateMenu(string identifier, string text, bool isCollapsible)
    {
      CommandBarMenu commandBarMenu = new CommandBarMenu(this.commandService, this.displayShortcut, isCollapsible);
      commandBarMenu.Identifier = identifier;
      commandBarMenu.Name = identifier;
      commandBarMenu.Header = (object) text;
      commandBarMenu.SetValue(AutomationElement.IdProperty, (object) identifier);
      return commandBarMenu;
    }

    private CommandBarDynamicMenu CreateDynamicMenu(string commandName)
    {
      CommandBarDynamicMenu commandBarDynamicMenu = new CommandBarDynamicMenu(this.commandService, commandName);
      commandBarDynamicMenu.Name = commandName;
      commandBarDynamicMenu.DisplayShortcut = this.displayShortcut;
      commandBarDynamicMenu.SetValue(AutomationElement.IdProperty, (object) commandName);
      return commandBarDynamicMenu;
    }

    public void Insert(int index, ICommandBarItem item)
    {
      this.items.Insert(index, (object) item);
    }

    public ICommandBarButton InsertButton(int index, string commandName)
    {
      CommandBarButton button = this.CreateButton(commandName);
      this.Insert(index, (ICommandBarItem) button);
      return (ICommandBarButton) button;
    }

    public ICommandBarButton InsertButton(int index, string commandName, string text)
    {
      ICommandBarButton commandBarButton = this.InsertButton(index, commandName);
      commandBarButton.Text = text;
      return commandBarButton;
    }

    public ICommandBarButton InsertButton(int index, string commandName, string text, params KeyBinding[] keyBindings)
    {
      ICommandBarButton commandBarButton = this.InsertButton(index, commandName, text);
      if (keyBindings != null)
        this.commandService.SetCommandPropertyDefault(commandName, "Shortcuts", (object) keyBindings);
      return commandBarButton;
    }

    public ICommandBarCheckBox InsertCheckBox(int index, string commandName)
    {
      CommandBarCheckBox checkBox = this.CreateCheckBox(commandName);
      this.Insert(index, (ICommandBarItem) checkBox);
      return (ICommandBarCheckBox) checkBox;
    }

    public ICommandBarCheckBox InsertCheckBox(int index, string commandName, string text)
    {
      ICommandBarCheckBox commandBarCheckBox = this.InsertCheckBox(index, commandName);
      commandBarCheckBox.Text = text;
      return commandBarCheckBox;
    }

    public ICommandBarCheckBox InsertCheckBox(int index, string commandName, string text, params KeyBinding[] keyBindings)
    {
      ICommandBarCheckBox commandBarCheckBox = this.InsertCheckBox(index, commandName, text);
      if (keyBindings != null)
        this.commandService.SetCommandPropertyDefault(commandName, "Shortcuts", (object) keyBindings);
      return commandBarCheckBox;
    }

    public ICommandBarMenu InsertMenu(int index, string identifier, string text, bool isCollapsible)
    {
      CommandBarMenu menu = this.CreateMenu(identifier, text, isCollapsible);
      this.Insert(index, (ICommandBarItem) menu);
      return (ICommandBarMenu) menu;
    }

    public ICommandBarMenu InsertMenu(int index, string identifier, string text)
    {
      return this.InsertMenu(index, identifier, text, false);
    }

    public ICommandBarDynamicMenu InsertDynamicMenu(int index, string commandName)
    {
      CommandBarDynamicMenu dynamicMenu = this.CreateDynamicMenu(commandName);
      this.Insert(index, (ICommandBarItem) dynamicMenu);
      return (ICommandBarDynamicMenu) dynamicMenu;
    }

    public ICommandBarDynamicMenu InsertDynamicMenu(int index, string commandName, string text)
    {
      ICommandBarDynamicMenu commandBarDynamicMenu = this.InsertDynamicMenu(index, commandName);
      commandBarDynamicMenu.Text = text;
      return commandBarDynamicMenu;
    }

    public ICommandBarDynamicMenu InsertDynamicMenu(int index, string commandName, string text, params KeyBinding[] keyBindings)
    {
      ICommandBarDynamicMenu commandBarDynamicMenu = this.InsertDynamicMenu(index, commandName, text);
      if (keyBindings != null)
        this.commandService.SetCommandPropertyDefault(commandName, "Shortcuts", (object) keyBindings);
      return commandBarDynamicMenu;
    }

    public ICommandBarSeparator InsertSeparator(int index)
    {
      ICommandBarSeparator commandBarSeparator = (ICommandBarSeparator) new CommandBarSeparator();
      this.Insert(index, (ICommandBarItem) commandBarSeparator);
      return commandBarSeparator;
    }

    public void Remove(ICommandBarItem item)
    {
      int index = this.items.IndexOf((object) item);
      if (index == -1)
        return;
      this.RemoveAt(index);
    }

    public void RemoveAt(int index)
    {
      this.items.RemoveAt(index);
    }

    public bool Contains(ICommandBarItem item)
    {
      return this.items.Contains((object) item);
    }

    public int IndexOf(ICommandBarItem item)
    {
      return this.items.IndexOf((object) item);
    }

    public IEnumerator GetEnumerator()
    {
      object[] objArray = new object[this.items.Count];
      this.items.CopyTo((Array) objArray, 0);
      return objArray.GetEnumerator();
    }

    public void CopyTo(Array array, int index)
    {
      this.items.CopyTo(array, index);
    }

    public void CopyTo(ICommandBarItem[] array, int index)
    {
      this.items.CopyTo((Array) array, index);
    }
  }
}
