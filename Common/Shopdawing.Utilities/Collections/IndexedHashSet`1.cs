// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Utility.Collections.IndexedHashSet`1
// Assembly: Microsoft.Expression.Utility, Version=5.0.30709.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: B77F0E80-A3D7-4861-BF76-6A6A586443F3
// Assembly location: C:\Users\M4600\Documents\Project\4.5\Microsoft.Expression.ProjectReferences\Microsoft.Expression.Utility.dll

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Microsoft.Expression.Utility.Collections
{
  public class IndexedHashSet<T> : IIndexedHashSet<T>, ICollection, ICollection<T>, IEnumerable<T>, IEnumerable where T : class
  {
    private const int DefaultCapacity = 72;
    private ConcurrentDictionary<int, T> dictionary;
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
        return (object) null;
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

    protected IndexedHashSet(IEqualityComparer<T> equalityComparer, int initialCapacity)
    {
      if (initialCapacity < 0)
        throw new ArgumentOutOfRangeException("initialCapacity", "Capacity must be positive.");
      this.equalityComparer = equalityComparer ?? (IEqualityComparer<T>) new IndexedHashSet<T>.DefaultEqualityComparer<T>();
      this.dictionary = new ConcurrentDictionary<int, T>(4 * Environment.ProcessorCount, initialCapacity);
    }

    public static IndexedHashSet<T> Create(int initialCapacity = 72, IEqualityComparer<T> equalityComparer = null)
    {
      return new IndexedHashSet<T>(equalityComparer, initialCapacity);
    }

    public void CopyTo(Array array, int index)
    {
      ((ICollection) this.dictionary.Values).CopyTo(array, index);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return this.dictionary.Values.GetEnumerator();
    }

    protected virtual bool AddIfNotPresent(T item)
    {
      return this.AddIfNotPresentInternal(item);
    }

    private bool AddIfNotPresentInternal(T item)
    {
      return this.dictionary.TryAdd(this.equalityComparer.GetHashCode(item), item);
    }

    public bool Add(T item)
    {
      return this.AddIfNotPresent(item);
    }

    void ICollection<T>.Add(T item)
    {
      this.AddIfNotPresent(item);
    }

    public virtual void Clear()
    {
      this.dictionary.Clear();
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

    public virtual bool Remove(T item)
    {
      return this.RemoveInternal(item);
    }

    private bool RemoveInternal(T item)
    {
      return this.dictionary.TryRemove(this.equalityComparer.GetHashCode(item), out item);
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
      return this.dictionary.Values.GetEnumerator();
    }

    public virtual bool UpdateKey(T item, Action<T> updateAction)
    {
      if (!this.RemoveInternal(item))
        return false;
      updateAction(item);
      return this.AddIfNotPresentInternal(item);
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
