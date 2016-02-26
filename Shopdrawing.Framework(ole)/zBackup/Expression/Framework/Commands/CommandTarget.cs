// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Commands.CommandTarget
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Expression.Framework.Commands
{
  public class CommandTarget : ICommandTarget
  {
    private Dictionary<string, ICommand> commands = new Dictionary<string, ICommand>();

    public ICommandCollection Commands
    {
      get
      {
        CommandCollection commandCollection = new CommandCollection();
        foreach (string command in this.commands.Keys)
          commandCollection.Add(command);
        return (ICommandCollection) commandCollection;
      }
    }

    public event CommandChangedEventHandler CommandChanged;

    public void Execute(string commandName, CommandInvocationSource invocationSource)
    {
      this.commands[commandName].Execute();
    }

    public void SetCommandProperty(string commandName, string propertyName, object propertyValue)
    {
      this.commands[commandName].SetProperty(propertyName, propertyValue);
    }

    public object GetCommandProperty(string commandName, string propertyName)
    {
      return this.commands[commandName].GetProperty(propertyName);
    }

    public void AddCommand(string commandName, ICommand command)
    {
      this.commands.Add(commandName, command);
      CommandCollection commandCollection1 = new CommandCollection();
      CommandCollection commandCollection2 = new CommandCollection();
      commandCollection1.Add(commandName);
      this.OnCommandChanged(new CommandChangedEventArgs((ICommandCollection) commandCollection1, (ICommandCollection) commandCollection2));
    }

    public void RemoveCommand(string commandName)
    {
      ICommand command = this.commands[commandName];
      this.commands.Remove(commandName);
      this.OnCommandChanged(new CommandChangedEventArgs((ICommandCollection) new CommandCollection(), (ICommandCollection) new CommandCollection()
      {
        commandName
      }));
    }

    protected void ClearCommands()
    {
      foreach (string commandName in (IEnumerable) this.Commands)
        this.RemoveCommand(commandName);
    }

    protected void OnCommandChanged(CommandChangedEventArgs e)
    {
      if (this.CommandChanged == null)
        return;
      this.CommandChanged((object) this, e);
    }
  }
}
