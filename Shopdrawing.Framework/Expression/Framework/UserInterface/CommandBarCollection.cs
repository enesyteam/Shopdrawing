// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.UserInterface.CommandBarCollection
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework.Commands;
using System;
using System.Collections;

namespace Microsoft.Expression.Framework.UserInterface
{
  internal class CommandBarCollection : ICommandBarCollection, ICollection, IEnumerable
  {
    private ArrayList commandBars = new ArrayList();
    private ICommandService commandService;
    private CommandBarService owner;

    public ICommandBar this[int index]
    {
      get
      {
        return (ICommandBar) this.commandBars[index];
      }
    }

    public ICommandBar this[string identifier]
    {
      get
      {
        return this.Search((ICollection) this, identifier);
      }
    }

    public int Count
    {
      get
      {
        return this.commandBars.Count;
      }
    }

    public bool IsSynchronized
    {
      get
      {
        return this.commandBars.IsSynchronized;
      }
    }

    public object SyncRoot
    {
      get
      {
        return this.commandBars.SyncRoot;
      }
    }

    public CommandBarCollection(CommandBarService owner, ICommandService commandService)
    {
      this.owner = owner;
      this.commandService = commandService;
    }

    public ICommandBar AddMenuBar(string identifier)
    {
      CommandBar commandBar = new CommandBar(this.commandService);
      commandBar.Name = identifier;
      commandBar.Text = identifier;
      commandBar.Identifier = identifier;
      this.commandBars.Add((object) commandBar);
      this.owner.Update();
      return (ICommandBar) commandBar;
    }

    public ICommandBar AddToolBar(string identifier)
    {
      throw new NotSupportedException();
    }

    public ICommandBar AddContextMenu(string identifier)
    {
      CommandBarContextMenu commandBarContextMenu = new CommandBarContextMenu(this.commandService);
      commandBarContextMenu.Name = identifier;
      commandBarContextMenu.Identifier = identifier;
      this.commandBars.Add((object) commandBarContextMenu);
      return (ICommandBar) commandBarContextMenu;
    }

    public void Remove(string identifier)
    {
      for (int index = this.commandBars.Count - 1; index >= 0; --index)
      {
        ICommandBar commandBar = (ICommandBar) this.commandBars[index];
        if (commandBar.Identifier == identifier)
        {
          this.commandBars.RemoveAt(index);
          if (!(commandBar is CommandBarContextMenu))
            this.owner.Update();
        }
      }
    }

    private ICommandBar Search(ICollection collection, string identifier)
    {
      foreach (object obj in (IEnumerable) collection)
      {
        ICommandBar commandBar = obj as ICommandBar;
        if (commandBar != null && commandBar.Identifier == identifier)
          return commandBar;
      }
      foreach (object obj in (IEnumerable) collection)
      {
        ICommandBar commandBar1 = obj as ICommandBar;
        if (commandBar1 != null)
        {
          ICommandBar commandBar2 = this.Search((ICollection) commandBar1.Items, identifier);
          if (commandBar2 != null)
            return commandBar2;
        }
      }
      return (ICommandBar) null;
    }

    public void CopyTo(Array array, int index)
    {
      this.commandBars.CopyTo(array, index);
    }

    public IEnumerator GetEnumerator()
    {
      return this.commandBars.GetEnumerator();
    }
  }
}
