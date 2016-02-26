// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.OwnershipCollection`1
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Microsoft.VisualStudio.PlatformUI.Shell
{
  internal abstract class OwnershipCollection<T> : ObservableCollection<T>, IObservableCollection<T>, IList<T>, ICollection<T>, IEnumerable<T>, IList, ICollection, IEnumerable, INotifyPropertyChanged, INotifyCollectionChanged
  {
    protected abstract void TakeOwnership(T item);

    protected abstract void LoseOwnership(T item);

    protected override void ClearItems()
    {
      List<T> list = new List<T>((IEnumerable<T>) this);
      base.ClearItems();
      foreach (T obj in list)
        this.LoseOwnership(obj);
    }

    protected override void InsertItem(int index, T item)
    {
      if ((object) item == null)
        throw new InvalidOperationException("Collection item cannot be null");
      base.InsertItem(index, item);
      this.TakeOwnership(item);
    }

    protected override void SetItem(int index, T item)
    {
      if ((object) item == null)
        throw new InvalidOperationException("Collection item cannot be null");
      T obj = this[index];
      base.SetItem(index, item);
      this.LoseOwnership(obj);
      this.TakeOwnership(item);
    }

    protected override void RemoveItem(int index)
    {
      T obj = this[index];
      base.RemoveItem(index);
      this.LoseOwnership(obj);
    }
  }
}
