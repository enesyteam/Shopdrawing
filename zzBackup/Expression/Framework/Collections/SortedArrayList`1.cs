// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Collections.SortedArrayList`1
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Expression.Framework.Collections
{
  internal sealed class SortedArrayList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
  {
    private List<T> innerList;
    private IComparer<T> comparer;

    public T this[int index]
    {
      get
      {
        return this.innerList[index];
      }
      set
      {
        throw new InvalidOperationException();
      }
    }

    public int Count
    {
      get
      {
        return this.innerList.Count;
      }
    }

    public bool IsReadOnly
    {
      get
      {
        return false;
      }
    }

    public SortedArrayList()
      : this((IComparer<T>) null)
    {
    }

    public SortedArrayList(IComparer<T> comparer)
    {
      this.comparer = comparer;
      this.innerList = new List<T>();
    }

    public void AddRange(IEnumerable<T> collection)
    {
      foreach (T obj in collection)
        this.Add(obj);
    }

    public int IndexOf(T item)
    {
      int num = this.innerList.BinarySearch(item, this.comparer);
      if (num >= 0)
        return num;
      return -1;
    }

    public void Insert(int index, T item)
    {
      throw new InvalidOperationException();
    }

    public void RemoveAt(int index)
    {
      this.innerList.RemoveAt(index);
    }

    public void Add(T item)
    {
      int index = this.innerList.BinarySearch(item, this.comparer);
      if (index < 0)
        index = ~index;
      this.innerList.Insert(index, item);
    }

    public void Clear()
    {
      this.innerList.Clear();
    }

    public bool Contains(T item)
    {
      return this.IndexOf(item) != -1;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
      this.innerList.CopyTo(array, arrayIndex);
    }

    public bool Remove(T item)
    {
      int index = this.IndexOf(item);
      if (index == -1)
        return false;
      this.innerList.RemoveAt(index);
      return true;
    }

    public IEnumerator<T> GetEnumerator()
    {
      return (IEnumerator<T>) this.innerList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this.innerList.GetEnumerator();
    }
  }
}
