// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Data.ArgumentDelegateCommand
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Windows.Input;

namespace Microsoft.Expression.Framework.Data
{
  public sealed class ArgumentDelegateCommand : ICommand
  {
    private ArgumentDelegateCommand.ArgumentEventHandler handler;

    public event EventHandler CanExecuteChanged;

    public ArgumentDelegateCommand(ArgumentDelegateCommand.ArgumentEventHandler handler)
    {
      this.handler = handler;
    }

    public void OnCanExecuteChanged()
    {
      if (this.CanExecuteChanged == null)
        return;
      this.CanExecuteChanged((object) this, EventArgs.Empty);
    }

    void ICommand.Execute(object parameter)
    {
      this.handler(parameter);
    }

    bool ICommand.CanExecute(object parameter)
    {
      return true;
    }

    public delegate void ArgumentEventHandler(object parameter);
  }
}
