// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Model.ModelItemDictionary
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
  public abstract class ModelItemDictionary : ModelItem, IDictionary<ModelItem, ModelItem>, ICollection<KeyValuePair<ModelItem, ModelItem>>, IEnumerable<KeyValuePair<ModelItem, ModelItem>>, IDictionary, ICollection, IEnumerable, INotifyCollectionChanged
  {
    public static readonly PropertyIdentifier KeyProperty = new PropertyIdentifier(typeof (ModelItemDictionary), "Key");

    public abstract ModelItem this[ModelItem key] { get; set; }

    public abstract ModelItem this[object key] { get; set; }

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

    public abstract ICollection<ModelItem> Keys { get; }

    protected virtual object SyncRoot
    {
      get
      {
        return (object) this;
      }
    }

    public abstract ICollection<ModelItem> Values { get; }

    bool IDictionary.IsFixedSize
    {
      get
      {
        return this.IsFixedSize;
      }
    }

    bool IDictionary.IsReadOnly
    {
      get
      {
        return this.IsReadOnly;
      }
    }

    ICollection IDictionary.Keys
    {
      get
      {
        object[] objArray = new object[this.Count];
        int num = 0;
        foreach (KeyValuePair<ModelItem, ModelItem> keyValuePair in this)
          objArray[num++] = (object) keyValuePair.Key;
        return (ICollection) objArray;
      }
    }

    ICollection IDictionary.Values
    {
      get
      {
        object[] objArray = new object[this.Count];
        int num = 0;
        foreach (KeyValuePair<ModelItem, ModelItem> keyValuePair in this)
          objArray[num++] = (object) keyValuePair.Value;
        return (ICollection) objArray;
      }
    }

    object IDictionary.this[object key]
    {
      get
      {
        return (object) this[ModelItemDictionary.ConvertType(key)];
      }
      set
      {
        this[ModelItemDictionary.ConvertType(key)] = ModelItemDictionary.ConvertType(value);
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

    public abstract void Add(ModelItem key, ModelItem value);

    public abstract ModelItem Add(object key, object value);

    public abstract void Clear();

    protected virtual void CopyTo(KeyValuePair<ModelItem, ModelItem>[] array, int arrayIndex)
    {
      foreach (KeyValuePair<ModelItem, ModelItem> keyValuePair in this)
        array[arrayIndex++] = keyValuePair;
    }

    protected virtual bool Contains(KeyValuePair<ModelItem, ModelItem> item)
    {
      ModelItem modelItem;
      if (this.TryGetValue(item.Key, out modelItem))
        return modelItem == item.Value;
      return false;
    }

    public abstract bool ContainsKey(ModelItem key);

    public abstract bool ContainsKey(object key);

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

    public abstract IEnumerator<KeyValuePair<ModelItem, ModelItem>> GetEnumerator();

    public abstract bool Remove(ModelItem key);

    public abstract bool Remove(object key);

    public abstract bool TryGetValue(ModelItem key, out ModelItem value);

    public abstract bool TryGetValue(object key, out ModelItem value);

    void IDictionary.Add(object key, object value)
    {
      this.Add(key, value);
    }

    void IDictionary.Clear()
    {
      this.Clear();
    }

    bool IDictionary.Contains(object key)
    {
      return this.ContainsKey(key);
    }

    IDictionaryEnumerator IDictionary.GetEnumerator()
    {
      return (IDictionaryEnumerator) new ModelItemDictionary.DictionaryEnumerator(this.GetEnumerator());
    }

    void IDictionary.Remove(object key)
    {
      this.Remove(key);
    }

    void ICollection.CopyTo(Array array, int index)
    {
      if (this.Count <= 0)
        return;
      int length = array.GetLength(0);
      if (index >= length)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_InvalidArrayIndex, new object[1]
        {
          (object) index
        }));
      KeyValuePair<ModelItem, ModelItem>[] array1 = new KeyValuePair<ModelItem, ModelItem>[length];
      this.CopyTo(array1, index);
      for (; index < array1.Length; ++index)
        array.SetValue((object) array1[index], index);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      foreach (KeyValuePair<ModelItem, ModelItem> keyValuePair in this)
        yield return (object) keyValuePair;
    }

    void ICollection<KeyValuePair<ModelItem, ModelItem>>.Add(KeyValuePair<ModelItem, ModelItem> item)
    {
      this.Add(item.Key, item.Value);
    }

    bool ICollection<KeyValuePair<ModelItem, ModelItem>>.Contains(KeyValuePair<ModelItem, ModelItem> item)
    {
      return this.Contains(item);
    }

    void ICollection<KeyValuePair<ModelItem, ModelItem>>.CopyTo(KeyValuePair<ModelItem, ModelItem>[] array, int arrayIndex)
    {
      if (arrayIndex >= array.Length)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_InvalidArrayIndex, new object[1]
        {
          (object) arrayIndex
        }));
      this.CopyTo(array, arrayIndex);
    }

    bool ICollection<KeyValuePair<ModelItem, ModelItem>>.Remove(KeyValuePair<ModelItem, ModelItem> item)
    {
      ModelItem modelItem;
      if (this.TryGetValue(item.Key, out modelItem) && modelItem == item.Value)
        return this.Remove(item.Key);
      return false;
    }

    private struct DictionaryEnumerator : IDictionaryEnumerator, IEnumerator
    {
      private IEnumerator<KeyValuePair<ModelItem, ModelItem>> _real;

      public DictionaryEntry Entry
      {
        get
        {
          return new DictionaryEntry((object) this._real.Current.Key, (object) this._real.Current.Value);
        }
      }

      public object Key
      {
        get
        {
          return (object) this._real.Current.Key;
        }
      }

      public object Value
      {
        get
        {
          return (object) this._real.Current.Value;
        }
      }

      public object Current
      {
        get
        {
          return (object) this.Entry;
        }
      }

      internal DictionaryEnumerator(IEnumerator<KeyValuePair<ModelItem, ModelItem>> real)
      {
        this._real = real;
      }

      public bool MoveNext()
      {
        return this._real.MoveNext();
      }

      public void Reset()
      {
        this._real.Reset();
      }
    }
  }
}
