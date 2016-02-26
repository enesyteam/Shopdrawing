// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.DocumentModel.EmptyList`1
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignModel.DocumentModel
{
  internal sealed class EmptyList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable, IEnumerator<T>, IDisposable, IEnumerator
  {
    public T this[int index]
    {
      get
      {
        throw new ArgumentOutOfRangeException();
      }
      set
      {
        throw new ArgumentOutOfRangeException();
      }
    }

    public int Count
    {
      get
      {
        return 0;
      }
    }

    public bool IsReadOnly
    {
      get
      {
        return true;
      }
    }

    public T Current
    {
      get
      {
        throw new InvalidOperationException();
      }
    }

    object IEnumerator.Current
    {
      get
      {
        throw new InvalidOperationException();
      }
    }

    public int IndexOf(T item)
    {
      return -1;
    }

    public void Insert(int index, T item)
    {
      throw new ArgumentOutOfRangeException();
    }

    public void RemoveAt(int index)
    {
      throw new ArgumentOutOfRangeException();
    }

    public void Add(T item)
    {
      throw new NotSupportedException();
    }

    public void Clear()
    {
    }

    public bool Contains(T item)
    {
      return false;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
    }

    public bool Remove(T item)
    {
      return false;
    }

    public IEnumerator<T> GetEnumerator()
    {
      return (IEnumerator<T>) this;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this;
    }

    public void Dispose()
    {
    }

    public bool MoveNext()
    {
      return false;
    }

    public void Reset()
    {
    }
  }
}
