// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Utility.Collections.SortedArrayList`1
// Assembly: Microsoft.Expression.Utility, Version=5.0.30709.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: B77F0E80-A3D7-4861-BF76-6A6A586443F3
// Assembly location: C:\Users\M4600\Documents\Project\4.5\Microsoft.Expression.ProjectReferences\Microsoft.Expression.Utility.dll

using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Expression.Utility.Collections
{
  public sealed class SortedArrayList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
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
