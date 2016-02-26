// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.ValueEditors.ExpressionValueEditorCommandArgs
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System.Windows;

namespace Microsoft.Expression.Framework.ValueEditors
{
  public sealed class ExpressionValueEditorCommandArgs
  {
    private IInputElement inputElement;
    private object parameter;

    public IInputElement InputElement
    {
      get
      {
        return this.inputElement;
      }
    }

    public object Parameter
    {
      get
      {
        return this.parameter;
      }
    }

    public ExpressionValueEditorCommandArgs(IInputElement inputElement, object parameter)
    {
      this.inputElement = inputElement;
      this.parameter = parameter;
    }
  }
}
