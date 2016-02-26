// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.ValueEditors.ExpressionPropertyValueEditorCommand
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Windows.Input;

namespace Microsoft.Expression.Framework.ValueEditors
{
  public sealed class ExpressionPropertyValueEditorCommand : ICommand
  {
    private ExpressionPropertyValueEditorCommandHandler handler;

    public event EventHandler CanExecuteChanged;

    public ExpressionPropertyValueEditorCommand(ExpressionPropertyValueEditorCommandHandler handler)
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
      this.handler(parameter as ExpressionValueEditorCommandArgs);
    }

    bool ICommand.CanExecute(object parameter)
    {
      return true;
    }
  }
}
