// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Metadata.AttributeCallbackBuilder
// Assembly: Microsoft.Windows.Design.Extensibility, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4673B7C2-4EF5-4715-85F2-D8E573468337
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Extensibility\Microsoft.Windows.Design.Extensibility.dll

using MS.Internal.Metadata;
using System;
using System.Collections.Generic;

namespace Microsoft.Windows.Design.Metadata
{
  public sealed class AttributeCallbackBuilder
  {
    private MutableAttributeTable _table;
    private Type _callbackType;

    public Type CallbackType
    {
      get
      {
        return this._callbackType;
      }
    }

    internal AttributeCallbackBuilder(MutableAttributeTable table, Type callbackType)
    {
      this._table = table;
      this._callbackType = callbackType;
    }

    public void AddCustomAttributes(params Attribute[] attributes)
    {
      if (attributes == null)
        throw new ArgumentNullException("attributes");
      this._table.AddCustomAttributes(this._callbackType, (IEnumerable<object>) attributes);
    }

    public void AddCustomAttributes(string memberName, params Attribute[] attributes)
    {
      if (memberName == null)
        throw new ArgumentNullException("memberName");
      if (attributes == null)
        throw new ArgumentNullException("attributes");
      this._table.AddCustomAttributes(this._callbackType, memberName, (IEnumerable<object>) attributes);
    }
  }
}
