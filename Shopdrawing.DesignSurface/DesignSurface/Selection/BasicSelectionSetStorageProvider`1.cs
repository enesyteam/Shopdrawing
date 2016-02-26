// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Selection.BasicSelectionSetStorageProvider`1
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Data;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.Selection
{
  public class BasicSelectionSetStorageProvider<T> : SelectionSet<T, OrderedList<T>>.IStorageProvider
  {
    private IComparer<T> comparer;

    public BasicSelectionSetStorageProvider(IComparer<T> comparer)
    {
      this.comparer = comparer;
    }

    public OrderedList<T> NewList()
    {
      return new OrderedList<T>(this.comparer);
    }

    public OrderedList<T> CopyList(ICollection<T> set)
    {
      return new OrderedList<T>(this.comparer, set);
    }

    public OrderedList<T> CanonicalizeList(OrderedList<T> list, SceneUpdateTypeFlags flags)
    {
      return (OrderedList<T>) null;
    }
  }
}
