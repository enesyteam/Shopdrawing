// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Data.DelegateCommand
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Windows.Input;

namespace Microsoft.Expression.Framework.Data
{
  public sealed class DelegateCommand : ICommand
  {
    private bool isEnabled = true;
    private DelegateCommand.SimpleEventHandler handler;

    public bool IsEnabled
    {
      get
      {
        return this.isEnabled;
      }
      set
      {
        this.isEnabled = value;
        this.OnCanExecuteChanged();
      }
    }

    public event EventHandler CanExecuteChanged;

    public DelegateCommand(DelegateCommand.SimpleEventHandler handler)
    {
      this.handler = handler;
    }

    void ICommand.Execute(object arg)
    {
      this.handler();
    }

    bool ICommand.CanExecute(object arg)
    {
      return this.IsEnabled;
    }

    private void OnCanExecuteChanged()
    {
      if (this.CanExecuteChanged == null)
        return;
      this.CanExecuteChanged((object) this, EventArgs.Empty);
    }

    public delegate void SimpleEventHandler();
  }
}
