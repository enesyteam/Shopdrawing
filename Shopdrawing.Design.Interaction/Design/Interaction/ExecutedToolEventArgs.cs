// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Interaction.ExecutedToolEventArgs
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using System;
using System.Windows.Input;

namespace Microsoft.Windows.Design.Interaction
{
  public sealed class ExecutedToolEventArgs : EventArgs
  {
    private ICommand _command;
    private object _parameter;

    public ICommand Command
    {
      get
      {
        return this._command;
      }
    }

    public object Parameter
    {
      get
      {
        return this._parameter;
      }
    }

    internal ExecutedToolEventArgs(ICommand command, object parameter)
    {
      if (command == null)
        throw new ArgumentNullException("command");
      this._command = command;
      this._parameter = parameter;
    }
  }
}
