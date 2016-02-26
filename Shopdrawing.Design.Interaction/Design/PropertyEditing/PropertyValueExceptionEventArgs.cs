// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.PropertyEditing.PropertyValueExceptionEventArgs
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using System;
using System.Reflection;

namespace Microsoft.Windows.Design.PropertyEditing
{
  public class PropertyValueExceptionEventArgs : EventArgs
  {
    private string _message;
    private PropertyValue _value;
    private PropertyValueExceptionSource _source;
    private Exception _exception;

    public string Message
    {
      get
      {
        return this._message;
      }
    }

    public PropertyValue PropertyValue
    {
      get
      {
        return this._value;
      }
    }

    public PropertyValueExceptionSource Source
    {
      get
      {
        return this._source;
      }
    }

    public Exception Exception
    {
      get
      {
        return this._exception;
      }
    }

    public PropertyValueExceptionEventArgs(string message, PropertyValue value, PropertyValueExceptionSource source, Exception exception)
    {
      if (message == null)
        throw new ArgumentNullException("message");
      if (value == null)
        throw new ArgumentNullException("value");
      if (!EnumValidator.IsValid(source))
        throw new ArgumentOutOfRangeException("source");
      if (exception == null)
        throw new ArgumentNullException("exception");
      if (exception.GetType() == typeof (TargetInvocationException))
        exception = exception.InnerException;
      this._message = message;
      this._value = value;
      this._source = source;
      this._exception = exception;
    }
  }
}
