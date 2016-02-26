// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Metadata.AttributeTableBuilder
// Assembly: Microsoft.Windows.Design.Extensibility, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4673B7C2-4EF5-4715-85F2-D8E573468337
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Extensibility\Microsoft.Windows.Design.Extensibility.dll

using MS.Internal.Metadata;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Windows.Design.Metadata
{
  public class AttributeTableBuilder
  {
    private MutableAttributeTable _table = new MutableAttributeTable();
    private bool _cloneOnUse;

    private MutableAttributeTable MutableTable
    {
      get
      {
        if (this._cloneOnUse)
        {
          MutableAttributeTable mutableAttributeTable = new MutableAttributeTable();
          mutableAttributeTable.AddTable(this._table);
          this._table = mutableAttributeTable;
          this._cloneOnUse = false;
        }
        return this._table;
      }
    }

    public void AddCallback(Type type, AttributeCallback callback)
    {
      if (type == null)
        throw new ArgumentNullException("type");
      if (callback == null)
        throw new ArgumentNullException("callback");
      this.MutableTable.AddCallback(type, callback);
    }

    public void AddCustomAttributes(Assembly assembly, params Attribute[] attributes)
    {
      if (assembly == null)
        throw new ArgumentNullException("assembly");
      if (attributes == null)
        throw new ArgumentNullException("attributes");
      this.MutableTable.AddCustomAttributes(assembly, (IEnumerable<object>) attributes);
    }

    public void AddCustomAttributes(Type type, params Attribute[] attributes)
    {
      if (type == null)
        throw new ArgumentNullException("type");
      if (attributes == null)
        throw new ArgumentNullException("attributes");
      this.MutableTable.AddCustomAttributes(type, (IEnumerable<object>) attributes);
    }

    public void AddCustomAttributes(Type ownerType, string memberName, params Attribute[] attributes)
    {
      if (ownerType == null)
        throw new ArgumentNullException("ownerType");
      if (memberName == null)
        throw new ArgumentNullException("memberName");
      if (attributes == null)
        throw new ArgumentNullException("attributes");
      this.MutableTable.AddCustomAttributes(ownerType, memberName, (IEnumerable<object>) attributes);
    }

    public void AddTable(AttributeTable table)
    {
      if (table == null)
        throw new ArgumentNullException("table");
      this.MutableTable.AddTable(table.MutableTable);
    }

    public AttributeTable CreateTable()
    {
      this._cloneOnUse = true;
      return new AttributeTable(this._table);
    }

    public void ValidateTable()
    {
      this.MutableTable.ValidateTable();
    }
  }
}
