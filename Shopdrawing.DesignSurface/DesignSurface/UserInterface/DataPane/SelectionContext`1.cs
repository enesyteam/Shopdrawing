// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.SelectionContext`1
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public abstract class SelectionContext<T> : NotifyingObject, ICollection<T>, IEnumerable<T>, IEnumerable where T : class, ISelectable
  {
    public abstract T PrimarySelection { get; set; }

    public abstract int Count { get; }

    public bool IsReadOnly
    {
      get
      {
        return false;
      }
    }

    public abstract void SetSelection(T item);

    public abstract void SetSelection(IEnumerable<T> items);

    public abstract void Add(T item);

    public abstract void Clear();

    public abstract bool Contains(T item);

    public abstract bool Remove(T item);

    public abstract IEnumerator<T> GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this.GetEnumerator();
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
      throw new NotSupportedException();
    }
  }
}
