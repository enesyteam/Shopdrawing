// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Metadata.AttributeTable
// Assembly: Microsoft.Windows.Design.Extensibility, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4673B7C2-4EF5-4715-85F2-D8E573468337
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Extensibility\Microsoft.Windows.Design.Extensibility.dll

using MS.Internal;
using MS.Internal.Metadata;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Windows.Design.Metadata
{
  public sealed class AttributeTable
  {
    private MutableAttributeTable _attributes;

    public IEnumerable<Type> AttributedTypes
    {
      get
      {
        return this._attributes.AttributedTypes;
      }
    }

    internal MutableAttributeTable MutableTable
    {
      get
      {
        return this._attributes;
      }
    }

    internal AttributeTable(MutableAttributeTable attributes)
    {
      this._attributes = attributes;
    }

    public bool ContainsAttributes(Type type)
    {
      if (type == null)
        throw new ArgumentNullException("type");
      return this._attributes.ContainsAttributes(type);
    }

    public IEnumerable GetCustomAttributes(Assembly assembly)
    {
      if (assembly == null)
        throw new ArgumentNullException("assembly");
      return this._attributes.GetCustomAttributes(assembly);
    }

    public IEnumerable GetCustomAttributes(Type type)
    {
      if (type == null)
        throw new ArgumentNullException("type");
      return this._attributes.GetCustomAttributes(type);
    }

    public IEnumerable GetCustomAttributes(Type ownerType, string memberName)
    {
      if (ownerType == null)
        throw new ArgumentNullException("ownerType");
      if (memberName == null)
        throw new ArgumentNullException("memberName");
      return this._attributes.GetCustomAttributes(ownerType, (string) Identifier.For(memberName));
    }

    internal IEnumerable GetCustomAttributes(Type ownerType, Identifier memberIdentifier)
    {
      return this._attributes.GetCustomAttributes(ownerType, (string) memberIdentifier);
    }
  }
}
