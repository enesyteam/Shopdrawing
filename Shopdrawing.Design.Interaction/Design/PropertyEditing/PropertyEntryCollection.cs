// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.PropertyEditing.PropertyEntryCollection
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Windows.Design.PropertyEditing
{
  public abstract class PropertyEntryCollection : IEnumerable<PropertyEntry>, IEnumerable
  {
    private PropertyValue _parentValue;

    public PropertyValue ParentValue
    {
      get
      {
        return this._parentValue;
      }
    }

    public abstract PropertyEntry this[string propertyName] { get; }

    public abstract int Count { get; }

    protected PropertyEntryCollection(PropertyValue parentValue)
    {
      if (parentValue == null)
        throw new ArgumentNullException("parentValue");
      this._parentValue = parentValue;
    }

    public abstract IEnumerator<PropertyEntry> GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this.GetEnumerator();
    }
  }
}
