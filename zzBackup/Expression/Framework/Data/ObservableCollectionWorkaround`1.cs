// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Data.ObservableCollectionWorkaround`1
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Microsoft.Expression.Framework.Data
{
  public class ObservableCollectionWorkaround<T> : ObservableCollection<T>
  {
    public ObservableCollectionWorkaround()
    {
    }

    public ObservableCollectionWorkaround(List<T> list)
      : base(list)
    {
    }

    public ObservableCollectionWorkaround(IEnumerable<T> list)
      : base(list)
    {
    }

    public ObservableCollectionWorkaround(ICollection collection)
    {
      foreach (T obj in (IEnumerable) collection)
        this.Add(obj);
    }

    public ObservableCollectionWorkaround<T> Clone()
    {
      return new ObservableCollectionWorkaround<T>((IEnumerable<T>) this);
    }

    public int BinarySearch(T value, IComparer<T> comparer)
    {
      return ((List<T>) this.Items).BinarySearch(value, comparer);
    }

    public void Sort()
    {
      ((List<T>) this.Items).Sort();
      this.OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
      this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset, (IList) null, -1));
    }

    public void Sort(IComparer<T> comparer)
    {
      ((List<T>) this.Items).Sort(comparer);
      this.OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
      this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset, (IList) null, -1));
    }

    public void Sort(Comparison<T> comparison)
    {
      ((List<T>) this.Items).Sort(comparison);
      this.OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
      this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset, (IList) null, -1));
    }

    public void Reverse()
    {
      ((List<T>) this.Items).Reverse();
    }
  }
}
