// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Data.GapList`1
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Expression.Framework.Data
{
  public class GapList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
  {
    private T[] items;
    private int gap;
    private int length;

    public T this[int index]
    {
      get
      {
        return this.GetAt(index);
      }
      set
      {
        this.items[this.ToReal(index)] = value;
      }
    }

    public int Count
    {
      get
      {
        return this.length;
      }
    }

    public T First
    {
      get
      {
        return this.GetAt(0);
      }
    }

    public T Last
    {
      get
      {
        return this.GetAt(this.Count - 1);
      }
    }

    public bool IsReadOnly
    {
      get
      {
        return false;
      }
    }

    public int Gap
    {
      get
      {
        return this.gap;
      }
    }

    public T[] Items
    {
      get
      {
        return this.items;
      }
    }

    public int Capacity
    {
      get
      {
        return this.items.Length;
      }
    }

    public int GapLength
    {
      get
      {
        return this.items.Length - this.length;
      }
    }

    public int Part2
    {
      get
      {
        return this.items.Length - this.length + this.gap;
      }
    }

    public GapList()
      : this(4)
    {
    }

    public GapList(int capacity)
    {
      this.items = new T[capacity];
      this.gap = 0;
      this.length = 0;
    }

    public GapList(IEnumerable<T> items)
    {
      ICollection<T> collection = items as ICollection<T>;
      if (collection != null)
      {
        this.items = new T[collection.Count + collection.Count / 2];
        collection.CopyTo(this.items, 0);
        this.length = collection.Count;
        this.gap = collection.Count;
      }
      else
      {
        this.items = new T[4];
        this.gap = 0;
        this.length = 0;
        foreach (T obj in items)
          this.Add(obj);
      }
    }

    public int IndexOf(T item)
    {
      for (int v = 0; v < this.length; ++v)
      {
        if (object.Equals((object) this.GetAt(v), (object) item))
          return v;
      }
      return -1;
    }

    public void Insert(int index, T item)
    {
      this.Normalize(index);
      this.Add(item);
    }

    public virtual void RemoveAt(int index)
    {
      this.Normalize(index);
      this.Delete(1);
    }

    public virtual void Add(T item)
    {
      this.MakeSpace(1);
      this.items[this.gap++] = item;
      ++this.length;
      this.Sync(this.gap - 1, 1);
    }

    public void Clear()
    {
      int len1 = this.gap;
      int len2 = this.length - this.gap;
      if (len1 > 0)
        this.ClearRange(0, len1);
      if (len2 > 0)
        this.ClearRange(this.Part2, len2);
      this.length = 0;
      this.gap = 0;
    }

    public bool Contains(T item)
    {
      return this.IndexOf(item) >= 0;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
      Array.Copy((Array) this.items, 0, (Array) array, arrayIndex, this.gap);
      Array.Copy((Array) this.items, this.Part2, (Array) array, arrayIndex, this.items.Length - this.Part2);
    }

    public bool Remove(T item)
    {
      int index = this.IndexOf(item);
      if (index < 0)
        return false;
      this.RemoveAt(index);
      return true;
    }

    public void Sort(IComparer<T> comparer)
    {
      this.Normalize(this.Count);
      Array.Sort<T>(this.items, 0, this.length, comparer);
    }

    public void Sort()
    {
      this.Normalize(this.Count);
      Array.Sort<T>(this.items, 0, this.length);
    }

    public IEnumerator<T> GetEnumerator()
    {
      int i;
      for (i = 0; i < this.gap; ++i)
        yield return this.items[i];
      for (i = this.Part2; i < this.items.Length; ++i)
        yield return this.items[i];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      int i;
      for (i = 0; i < this.gap; ++i)
        yield return (object) this.items[i];
      for (i = this.Part2; i < this.items.Length; ++i)
        yield return (object) this.items[i];
    }

    public IEnumerable<T> EnumerateRange(int v, int n)
    {
      for (int i = v; i < v + n; ++i)
        yield return this.GetAt(i);
    }

    public virtual void RemoveRange(int v, int n)
    {
      if (this.gap < v)
      {
        this.Normalize(v);
        this.Delete(n);
      }
      else if (v + n <= this.gap)
      {
        this.Normalize(v + n);
        this.Delete(-n);
      }
      else
      {
        int num = this.gap - v;
        int n1 = n - num;
        this.Delete(-num);
        this.Delete(n1);
      }
    }

    public int ToReal(int v)
    {
      if (v >= this.gap)
        return this.items.Length - this.length + v;
      return v;
    }

    public int ToVirtual(int r)
    {
      if (r >= this.gap)
        return r + this.length - this.items.Length;
      return r;
    }

    public T GetAt(int v)
    {
      return this.GetAtReal(this.ToReal(v));
    }

    public T GetAtReal(int r)
    {
      return this.items[r];
    }

    public virtual void Sync(int r, int len)
    {
    }

    public int RunLength(int r)
    {
      if (r >= this.gap)
        return this.items.Length - r;
      return this.gap - r;
    }

    public int InverseRunLength(int r)
    {
      if (r > this.gap)
        return r - this.Part2;
      return r;
    }

    public bool CheckSpace(int n)
    {
      return this.length + n <= this.items.Length;
    }

    public virtual void Normalize(int v)
    {
      if (this.gap == v)
        return;
      int gapLength = this.GapLength;
      if (gapLength > 0)
      {
        if (this.gap > v)
        {
          int index = gapLength + v;
          int length = this.gap - v;
          if (length > 1)
          {
            GapList<T>.IntraArrayCopy(this.items, v, index, length);
          }
          else
          {
            this.items[index] = this.items[v];
            this.items[v] = default (T);
          }
          this.Sync(index, this.gap - v);
        }
        else
        {
          int length = v - this.gap;
          if (length > 1)
          {
            GapList<T>.IntraArrayCopy(this.items, this.gap + gapLength, this.gap, length);
          }
          else
          {
            this.items[this.gap] = this.items[this.gap + gapLength];
            this.items[this.gap + gapLength] = default (T);
          }
          this.Sync(this.gap, v - this.gap);
        }
      }
      this.gap = v;
    }

    private static void IntraArrayCopy(T[] items, int sourceIndex, int destinationIndex, int length)
    {
      if (sourceIndex < destinationIndex)
      {
        for (int index = length - 1; index >= 0; --index)
        {
          items[destinationIndex + index] = items[sourceIndex + index];
          items[sourceIndex + index] = default (T);
        }
      }
      else
      {
        for (int index = 0; index < length; ++index)
        {
          items[destinationIndex + index] = items[sourceIndex + index];
          items[sourceIndex + index] = default (T);
        }
      }
    }

    public void MakeSpace(int n)
    {
      if (this.CheckSpace(n))
        return;
      int num = this.items.Length;
      if (num < n)
        num = n + n / 2;
      int length = this.items.Length + num;
      T[] objArray = new T[length];
      Array.Copy((Array) this.items, (Array) objArray, this.gap);
      Array.Copy((Array) this.items, this.Part2, (Array) objArray, length - this.length + this.gap, this.length - this.gap);
      this.items = objArray;
      this.Sync(this.Part2, this.length - this.gap);
    }

    internal void Delete(int n)
    {
      if (n < 0)
      {
        if (this.gap + n < 0)
          n = -this.gap;
        this.gap += n;
        this.length += n;
        this.ClearRange(this.gap, -n);
      }
      else
      {
        if (n == 0)
          return;
        if (n > this.length - this.gap)
        {
          this.ClearRange(this.Part2, this.length - this.gap);
          this.length = this.gap;
        }
        else
        {
          this.ClearRange(this.Part2, n);
          this.length -= n;
        }
      }
    }

    public void Get(int v, int n, T[] destination, int destinationIndex)
    {
      int sourceIndex = this.ToReal(v);
      if (sourceIndex >= this.gap || sourceIndex + n < this.gap)
      {
        Array.Copy((Array) this.items, sourceIndex, (Array) destination, destinationIndex, n);
      }
      else
      {
        int length = this.gap - sourceIndex;
        Array.Copy((Array) this.items, sourceIndex, (Array) destination, destinationIndex, length);
        Array.Copy((Array) this.items, this.Part2, (Array) destination, destinationIndex + length, n - length);
      }
    }

    public void Get(int v, int n, GapList<T> destination)
    {
      destination.MakeSpace(n);
      this.Get(v, n, destination.items, destination.gap);
      destination.Sync(destination.gap, n);
      destination.gap += n;
      destination.length += n;
    }

    public void Move(int vTo, GapList<T> source, int vFrom, int n)
    {
      if (this == source && vTo == vFrom || n <= 0)
        return;
      this.Normalize(vTo);
      source.Get(vFrom, n, this);
      if (this == source && vTo < vFrom)
        vFrom += n;
      source.RemoveRange(vFrom, n);
    }

    protected virtual void ClearRange(int r, int len)
    {
      this.ClearRangeWorker(r, len);
    }

    private void ClearRangeWorker(int r, int len)
    {
      for (int index = r; index < r + len; ++index)
        this.items[index] = default (T);
    }

    public override string ToString()
    {
      return "items : Length=" + this.items.Length.ToString() + ", gap=" + this.gap.ToString() + ", length=" + this.length.ToString();
    }
  }
}
