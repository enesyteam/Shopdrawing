// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Utility.Collections.ReadOnlyIndexedHashSet`1
// Assembly: Microsoft.Expression.Utility, Version=5.0.30709.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: B77F0E80-A3D7-4861-BF76-6A6A586443F3
// Assembly location: C:\Users\M4600\Documents\Project\4.5\Microsoft.Expression.ProjectReferences\Microsoft.Expression.Utility.dll

using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Expression.Utility.Collections
{
  public sealed class ReadOnlyIndexedHashSet<T> : IIndexedHashSet<T>, ICollection, ICollection<T>, IReadOnlyCollection<T>, IEnumerable<T>, IEnumerable where T : class
  {
    private IIndexedHashSet<T> hashSet;

    public T this[int hash]
    {
      get
      {
        return this.hashSet[hash];
      }
    }

    public int Count
    {
      get
      {
        return this.hashSet.Count;
      }
    }

    bool ICollection.IsSynchronized
    {
      get
      {
        return this.hashSet.IsSynchronized;
      }
    }

    object ICollection.SyncRoot
    {
      get
      {
        return this.hashSet.SyncRoot;
      }
    }

    public bool IsReadOnly
    {
      get
      {
        return true;
      }
    }

    private ReadOnlyIndexedHashSet()
    {
    }

    public ReadOnlyIndexedHashSet(IIndexedHashSet<T> hashSet)
    {
      if (hashSet == null)
        throw new ArgumentNullException("hashSet");
      this.hashSet = hashSet;
    }

    public void CopyTo(Array array, int index)
    {
      this.hashSet.CopyTo(array, index);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return this.hashSet.GetEnumerator();
    }

    public void Add(T item)
    {
      throw new InvalidOperationException("Read only collection.");
    }

    public void Clear()
    {
      throw new InvalidOperationException("Read only collection.");
    }

    public bool Contains(T item)
    {
      return this.hashSet.Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
      this.hashSet.CopyTo(array, arrayIndex);
    }

    public bool Remove(T item)
    {
      throw new InvalidOperationException("Read only collection.");
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
      return this.hashSet.GetEnumerator();
    }
  }
}
