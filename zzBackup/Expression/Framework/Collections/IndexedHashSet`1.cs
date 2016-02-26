// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Collections.IndexedHashSet`1
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Expression.Framework.Collections
{
  public sealed class IndexedHashSet<T> : IIndexedHashSet<T>, ICollection, ICollection<T>, IEnumerable<T>, IEnumerable where T : class
  {
    private Dictionary<int, T> dictionary;
    private IEqualityComparer<T> equalityComparer;

    public int Count
    {
      get
      {
        return this.dictionary.Count;
      }
    }

    bool ICollection.IsSynchronized
    {
      get
      {
        return false;
      }
    }

    object ICollection.SyncRoot
    {
      get
      {
        return ((ICollection) this.dictionary).SyncRoot;
      }
    }

    public T this[int hash]
    {
      get
      {
        T obj;
        this.dictionary.TryGetValue(hash, out obj);
        return obj;
      }
    }

    bool ICollection<T>.IsReadOnly
    {
      get
      {
        return false;
      }
    }

    public IndexedHashSet()
      : this((IEqualityComparer<T>) new IndexedHashSet<T>.DefaultEqualityComparer<T>())
    {
    }

    public IndexedHashSet(int capacity)
      : this((IEqualityComparer<T>) new IndexedHashSet<T>.DefaultEqualityComparer<T>(), capacity)
    {
    }

    public IndexedHashSet(IEqualityComparer<T> equalityComparer)
      : this(equalityComparer, 72)
    {
    }

    public IndexedHashSet(IEqualityComparer<T> equalityComparer, int capacity)
    {
      if (equalityComparer == null)
        throw new ArgumentNullException("equalityComparer");
      if (capacity < 0)
        throw new ArgumentOutOfRangeException("capacity", "Capacity must be positive.");
      this.equalityComparer = equalityComparer;
      this.dictionary = new Dictionary<int, T>(capacity);
    }

    void ICollection.CopyTo(Array array, int index)
    {
      ((ICollection) this.dictionary.Values).CopyTo(array, index);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return ((IEnumerable) this.dictionary.Values).GetEnumerator();
    }

    private bool AddIfNotPresent(T item)
    {
      if ((object) item == null)
        throw new ArgumentNullException("item");
      int hashCode = this.equalityComparer.GetHashCode(item);
      if (this.dictionary.ContainsKey(hashCode))
        return false;
      this.dictionary.Add(hashCode, item);
      return true;
    }

    public bool Add(T item)
    {
      return this.AddIfNotPresent(item);
    }

    void ICollection<T>.Add(T item)
    {
      this.AddIfNotPresent(item);
    }

    public void Clear()
    {
      this.dictionary.Clear();
    }

    public void Clear(int count)
    {
      Dictionary<int, T> dictionary = new Dictionary<int, T>(this.dictionary.Count);
      foreach (KeyValuePair<int, T> keyValuePair in Enumerable.Take<KeyValuePair<int, T>>((IEnumerable<KeyValuePair<int, T>>) this.dictionary, count))
        dictionary.Add(keyValuePair.Key, keyValuePair.Value);
      this.dictionary = dictionary;
    }

    public bool Contains(T item)
    {
      return this.ContainsHash(this.equalityComparer.GetHashCode(item));
    }

    public bool ContainsHash(int hash)
    {
      return this.dictionary.ContainsKey(hash);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
      this.dictionary.Values.CopyTo(array, arrayIndex);
    }

    public bool Remove(T item)
    {
      return this.dictionary.Remove(this.equalityComparer.GetHashCode(item));
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
      return ((IEnumerable<T>) this.dictionary.Values).GetEnumerator();
    }

    private class DefaultEqualityComparer<D> : IEqualityComparer<D>
    {
      public int GetHashCode(D obj)
      {
        if ((object) obj == null)
          throw new ArgumentNullException("obj");
        return obj.GetHashCode();
      }

      public bool Equals(D x, D y)
      {
        if (object.ReferenceEquals((object) x, (object) y))
          return true;
        if ((object) x != null && (object) y != null)
          return x.GetHashCode() == y.GetHashCode();
        return false;
      }
    }
  }
}
