// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.EmptyList`1
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Expression.Framework
{
  public sealed class EmptyList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable, IEnumerator<T>, IDisposable, IEnumerator
  {
    public T this[int index]
    {
      get
      {
        throw new InvalidOperationException();
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
      throw new InvalidOperationException();
    }

    public void RemoveAt(int index)
    {
      throw new InvalidOperationException();
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
