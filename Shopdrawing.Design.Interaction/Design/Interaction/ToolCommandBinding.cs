// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Interaction.ToolCommandBinding
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design;
using System;
using System.Windows.Input;

namespace Microsoft.Windows.Design.Interaction
{
  public class ToolCommandBinding
  {
    private ToolCommand _command;

    public ToolCommand Command
    {
      get
      {
        return this._command;
      }
      set
      {
        if (value == null)
          throw new ArgumentNullException("value");
        this._command = value;
      }
    }

    public event ExecutedToolEventHandler Execute;

    public event CanExecuteToolEventHandler CanExecute;

    public ToolCommandBinding()
    {
    }

    public ToolCommandBinding(ToolCommand command)
    {
      if (command == null)
        throw new ArgumentNullException("command");
      this.Command = command;
    }

    public ToolCommandBinding(ToolCommand command, ExecutedToolEventHandler executedToolEventHandler)
    {
      if (command == null)
        throw new ArgumentNullException("command");
      if (executedToolEventHandler == null)
        throw new ArgumentNullException("executedToolEventHandler");
      this.Execute += executedToolEventHandler;
      this.Command = command;
    }

    public ToolCommandBinding(ToolCommand command, ExecutedToolEventHandler executedToolEventHandler, CanExecuteToolEventHandler canExecuteToolEventHandler)
    {
      if (command == null)
        throw new ArgumentNullException("command");
      if (executedToolEventHandler == null)
        throw new ArgumentNullException("executedToolEventHandler");
      if (canExecuteToolEventHandler == null)
        throw new ArgumentNullException("canExecuteToolEventHandler");
      this.CanExecute += canExecuteToolEventHandler;
      this.Execute += executedToolEventHandler;
      this.Command = command;
    }

    internal bool OnCanExecute(EditingContext sender, CanExecuteToolEventArgs e)
    {
      if (this.CanExecute != null)
      {
        this.CanExecute((object) sender, e);
        if (!e.CanExecute)
          return false;
      }
      return e.CanExecute = this.Execute != null;
    }

    internal bool OnExecute(EditingContext sender, ExecutedToolEventArgs e)
    {
      CanExecuteToolEventArgs e1 = new CanExecuteToolEventArgs((ICommand) this._command, e.Parameter);
      if (this.Execute == null || !this.OnCanExecute(sender, e1))
        return false;
      this.Execute((object) sender, e);
      return true;
    }
  }
}
