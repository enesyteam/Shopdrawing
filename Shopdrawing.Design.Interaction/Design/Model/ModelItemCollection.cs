// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Model.ModelItemCollection
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design.Metadata;
using MS.Internal.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;

namespace Microsoft.Windows.Design.Model
{
  public abstract class ModelItemCollection : ModelItem, IList<ModelItem>, ICollection<ModelItem>, IEnumerable<ModelItem>, IList, ICollection, IEnumerable, INotifyCollectionChanged
  {
    public static readonly PropertyIdentifier ItemProperty = new PropertyIdentifier(typeof (ModelItemCollection), "Item");

    public abstract ModelItem this[int index] { get; set; }

    public abstract int Count { get; }

    protected virtual bool IsFixedSize
    {
      get
      {
        return this.IsReadOnly;
      }
    }

    public abstract bool IsReadOnly { get; }

    protected virtual bool IsSynchronized
    {
      get
      {
        return false;
      }
    }

    protected virtual object SyncRoot
    {
      get
      {
        return (object) this;
      }
    }

    bool IList.IsFixedSize
    {
      get
      {
        return this.IsFixedSize;
      }
    }

    bool IList.IsReadOnly
    {
      get
      {
        return this.IsReadOnly;
      }
    }

    object IList.this[int index]
    {
      get
      {
        return (object) this[index];
      }
      set
      {
        this[index] = ModelItemCollection.ConvertType(value);
      }
    }

    int ICollection.Count
    {
      get
      {
        return this.Count;
      }
    }

    bool ICollection.IsSynchronized
    {
      get
      {
        return this.IsSynchronized;
      }
    }

    object ICollection.SyncRoot
    {
      get
      {
        return this.SyncRoot;
      }
    }

    public abstract event NotifyCollectionChangedEventHandler CollectionChanged;

    public abstract void Add(ModelItem item);

    public abstract ModelItem Add(object value);

    public abstract void Clear();

    public abstract bool Contains(ModelItem item);

    public abstract bool Contains(object value);

    private static ModelItem ConvertType(object value)
    {
      try
      {
        return (ModelItem) value;
      }
      catch (InvalidCastException ex)
      {
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_ArgIncorrectType, new object[2]
        {
          (object) "value",
          (object) typeof (ModelItem).FullName
        }));
      }
    }

    public abstract void CopyTo(ModelItem[] array, int arrayIndex);

    public abstract IEnumerator<ModelItem> GetEnumerator();

    public abstract int IndexOf(ModelItem item);

    public abstract void Insert(int index, ModelItem item);

    public abstract ModelItem Insert(int index, object value);

    public abstract void Move(int fromIndex, int toIndex);

    public abstract bool Remove(ModelItem item);

    public abstract bool Remove(object value);

    public abstract void RemoveAt(int index);

    int IList.Add(object value)
    {
      this.Add(value);
      return this.Count - 1;
    }

    void IList.Clear()
    {
      this.Clear();
    }

    bool IList.Contains(object value)
    {
      return this.Contains(value);
    }

    int IList.IndexOf(object value)
    {
      return this.IndexOf(ModelItemCollection.ConvertType(value));
    }

    void IList.Insert(int index, object value)
    {
      this.Insert(index, value);
    }

    void IList.Remove(object value)
    {
      this.Remove(value);
    }

    void IList.RemoveAt(int index)
    {
      this.RemoveAt(index);
    }

    void ICollection.CopyTo(Array array, int index)
    {
      for (int index1 = 0; index1 < this.Count; ++index1)
        array.SetValue((object) this[index1], index1 + index);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      foreach (object obj in this)
        yield return obj;
    }
  }
}
