// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Interaction.CommandExceptionEventArgs
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using System;
using System.Windows.Input;

namespace Microsoft.Windows.Design.Interaction
{
  public class CommandExceptionEventArgs : EventArgs
  {
    private ICommand _command;
    private Exception _exception;

    public ICommand Command
    {
      get
      {
        return this._command;
      }
    }

    public Exception Exception
    {
      get
      {
        return this._exception;
      }
    }

    public CommandExceptionEventArgs(ICommand command, Exception exception)
    {
      if (command == null)
        throw new ArgumentNullException("command");
      if (exception == null)
        throw new ArgumentNullException("exception");
      this._command = command;
      this._exception = exception;
    }
  }
}
