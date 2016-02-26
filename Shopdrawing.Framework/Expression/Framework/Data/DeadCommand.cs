// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Data.DeadCommand
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Windows.Input;

namespace Microsoft.Expression.Framework.Data
{
  public class DeadCommand : ICommand
  {
    private static DeadCommand instance = new DeadCommand();

    public static ICommand Instance
    {
      get
      {
        return (ICommand) DeadCommand.instance;
      }
    }

    public event EventHandler CanExecuteChanged;

    private DeadCommand()
    {
    }

    public void Execute(object parameter)
    {
    }

    public bool CanExecute(object parameter)
    {
      return false;
    }

    protected void OnCanExecuteChanged()
    {
      if (this.CanExecuteChanged == null)
        return;
      this.CanExecuteChanged((object) this, EventArgs.Empty);
    }
  }
}
