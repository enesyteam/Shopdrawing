// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Metadata.AttributeTableValidationException
// Assembly: Microsoft.Windows.Design.Extensibility, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4673B7C2-4EF5-4715-85F2-D8E573468337
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Extensibility\Microsoft.Windows.Design.Extensibility.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Windows.Design.Metadata
{
  [Serializable]
  public class AttributeTableValidationException : Exception
  {
    private string[] _validationErrors;

    public IEnumerable<string> ValidationErrors
    {
      get
      {
        return (IEnumerable<string>) this._validationErrors;
      }
    }

    public AttributeTableValidationException()
    {
    }

    public AttributeTableValidationException(string message)
      : base(message)
    {
    }

    public AttributeTableValidationException(string message, Exception inner)
      : base(message, inner)
    {
    }

    public AttributeTableValidationException(string message, IEnumerable<string> validationErrors)
      : base(message)
    {
      this._validationErrors = AttributeTableValidationException.CreateArray(validationErrors);
    }

    public AttributeTableValidationException(string message, Exception inner, IEnumerable<string> validationErrors)
      : base(message, inner)
    {
      this._validationErrors = AttributeTableValidationException.CreateArray(validationErrors);
    }

    protected AttributeTableValidationException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      if (info == null)
        throw new ArgumentNullException("info");
      this._validationErrors = (string[]) info.GetValue("ValidationErrors", typeof (string[]));
    }

    private static string[] CreateArray(IEnumerable<string> validationErrors)
    {
      string[] strArray;
      if (validationErrors != null)
      {
        int length = 0;
        IEnumerator<string> enumerator = validationErrors.GetEnumerator();
        while (enumerator.MoveNext())
          ++length;
        enumerator.Reset();
        strArray = new string[length];
        int num = 0;
        while (enumerator.MoveNext())
          strArray[num++] = enumerator.Current;
      }
      else
        strArray = new string[0];
      return strArray;
    }

    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      if (info == null)
        throw new ArgumentNullException("info");
      base.GetObjectData(info, context);
      info.AddValue("ValidationErrors", (object) this._validationErrors);
    }
  }
}
