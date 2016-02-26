// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.PropertyEditing.PropertyValueCollection
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Microsoft.Windows.Design.PropertyEditing
{
  public abstract class PropertyValueCollection : IEnumerable<PropertyValue>, IEnumerable, INotifyCollectionChanged
  {
    private PropertyValue _parentValue;

    public PropertyValue ParentValue
    {
      get
      {
        return this._parentValue;
      }
    }

    public abstract PropertyValue this[int index] { get; }

    public abstract int Count { get; }

    public event NotifyCollectionChangedEventHandler CollectionChanged;

    protected PropertyValueCollection(PropertyValue parentValue)
    {
      if (parentValue == null)
        throw new ArgumentNullException("parentValue");
      this._parentValue = parentValue;
    }

    public abstract PropertyValue Add(object value);

    public abstract PropertyValue Insert(object value, int index);

    public abstract bool Remove(PropertyValue propertyValue);

    public abstract void RemoveAt(int index);

    public abstract void SetIndex(int currentIndex, int newIndex);

    public abstract IEnumerator<PropertyValue> GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this.GetEnumerator();
    }

    protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
      if (this.CollectionChanged == null)
        return;
      this.CollectionChanged((object) this, e ?? new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }
  }
}
