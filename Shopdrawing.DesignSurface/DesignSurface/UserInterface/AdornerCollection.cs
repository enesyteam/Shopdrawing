// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.AdornerCollection
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;
using System.Collections;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  public sealed class AdornerCollection : IAdornerCollection, ICollection, IEnumerable
  {
    private IList adornerList;

    public int Count
    {
      get
      {
        return this.adornerList.Count;
      }
    }

    public IAdorner this[int index]
    {
      get
      {
        return (IAdorner) this.adornerList[index];
      }
    }

    object ICollection.SyncRoot
    {
      get
      {
        return this.adornerList.SyncRoot;
      }
    }

    bool ICollection.IsSynchronized
    {
      get
      {
        return this.adornerList.IsSynchronized;
      }
    }

    public AdornerCollection(IList adornerList)
    {
      this.adornerList = adornerList;
    }

    void ICollection.CopyTo(Array array, int index)
    {
      this.adornerList.CopyTo(array, index);
    }

    public void CopyTo(IAdorner[] array, int index)
    {
      ((ICollection) this).CopyTo((Array) array, index);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return this.adornerList.GetEnumerator();
    }
  }
}
