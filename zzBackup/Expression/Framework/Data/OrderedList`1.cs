// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Data.OrderedList`1
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Expression.Framework.Data
{
  public class OrderedList<T> : List<T>, IOrderedList<T>, IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
  {
    private IComparer<T> comparer;

    public OrderedList(IComparer<T> comparer)
    {
      this.comparer = comparer;
    }

    public OrderedList(IComparer<T> comparer, ICollection<T> copyFrom)
      : this(comparer)
    {
      this.AddRange((IEnumerable<T>) copyFrom);
      this.Sort(this.comparer);
    }

    int IList<T>.IndexOf(T item)
    {
      int num = this.BinarySearch(item, this.comparer);
      if (num >= 0)
        return num;
      return -1;
    }

    void IList<T>.Insert(int index, T item)
    {
      this.Insert(index, item);
    }

    void ICollection<T>.Add(T item)
    {
      int num = this.BinarySearch(item, this.comparer);
      if (num >= 0)
        throw new ArgumentException(ExceptionStringTable.ItemIsAlreadyPresentInList);
      this.Insert(~num, item);
    }

    bool ICollection<T>.Contains(T item)
    {
      return this.BinarySearch(item, this.comparer) >= 0;
    }

    bool ICollection<T>.Remove(T item)
    {
      int index = this.BinarySearch(item, this.comparer);
      if (index < 0)
        return false;
      this.RemoveAt(index);
      return true;
    }

    int IOrderedList<T>.BinarySearch(T item)
    {
      return this.BinarySearch(item, this.comparer);
    }
  }
}
