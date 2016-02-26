// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Data.DataBindingProxyCollectionView`1
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace Microsoft.Expression.Framework.Data
{
  public sealed class DataBindingProxyCollectionView<T> : CollectionView
  {
    private int currentIndex = -1;
    private ObservableCollection<T> collection;
    private IDataBindingProxy<T> proxy;

    public override int Count
    {
      get
      {
        return this.collection.Count;
      }
    }

    public override object CurrentItem
    {
      get
      {
        if (this.collection != null && this.currentIndex >= 0 && this.currentIndex < this.collection.Count)
          return (object) this.collection[this.currentIndex];
        return (object) null;
      }
    }

    public override int CurrentPosition
    {
      get
      {
        return this.currentIndex;
      }
    }

    public override bool IsCurrentBeforeFirst
    {
      get
      {
        return this.currentIndex == 0;
      }
    }

    public override bool IsCurrentAfterLast
    {
      get
      {
        if (this.collection.Count > 0)
          return this.currentIndex == this.collection.Count - 1;
        return false;
      }
    }

    public T Current
    {
      get
      {
        int index = this.currentIndex;
        if (index < 0 || index >= this.collection.Count)
          return default (T);
        return this.collection[index];
      }
    }

    public override bool CanFilter
    {
      get
      {
        return false;
      }
    }

    public override bool CanSort
    {
      get
      {
        return false;
      }
    }

    public DataBindingProxyCollectionView(ObservableCollection<T> collection, IDataBindingProxy<T> proxy)
      : base((IEnumerable) collection)
    {
      this.collection = collection;
      this.proxy = proxy;
      this.proxy.PropertyChanged += new PropertyChangedEventHandler(this.Proxy_PropertyChanged);
      this.MoveCurrentTo((object) this.proxy.Value);
    }

    protected override IEnumerator GetEnumerator()
    {
      return (IEnumerator) new DataBindingProxyCollectionView<T>.IListEnumerator((IList<T>) this.collection);
    }

    public override bool Contains(object item)
    {
      return this.collection.Contains((T) item);
    }

    public override int IndexOf(object item)
    {
      return item != null ? this.collection.IndexOf((T) item) : -1;
    }

    public override bool PassesFilter(object item)
    {
      return true;
    }

    public override bool MoveCurrentToFirst()
    {
      if (this.collection.Count > 0)
      {
        this.currentIndex = 0;
        this.proxy.Value = this.collection[this.currentIndex];
      }
      this.OnCurrentChanged();
      return (object) this.Current != null;
    }

    public override bool MoveCurrentToLast()
    {
      if (this.collection.Count > 0)
      {
        this.currentIndex = this.collection.Count - 1;
        this.proxy.Value = this.collection[this.currentIndex];
      }
      this.OnCurrentChanged();
      return (object) this.Current != null;
    }

    public override bool MoveCurrentToNext()
    {
      if (this.currentIndex >= 0 && this.collection.Count > this.currentIndex + 1)
      {
        ++this.currentIndex;
        this.proxy.Value = this.collection[this.currentIndex];
      }
      this.OnCurrentChanged();
      return (object) this.Current != null;
    }

    public override bool MoveCurrentToPrevious()
    {
      if (this.currentIndex > 0)
      {
        --this.currentIndex;
        this.proxy.Value = this.collection[this.currentIndex];
      }
      this.OnCurrentChanged();
      return (object) this.Current != null;
    }

    public override bool MoveCurrentTo(object item)
    {
      if (item == MixedProperty.Mixed)
        this.currentIndex = -1;
      else if (item != null)
      {
        this.proxy.Value = (T) item;
        this.currentIndex = this.IndexOf(item);
      }
      else
        this.currentIndex = -1;
      this.OnCurrentChanged();
      return (object) this.Current != null;
    }

    public override bool MoveCurrentToPosition(int index)
    {
      this.currentIndex = index;
      this.OnCurrentChanged();
      return (object) this.Current != null;
    }

    private void Proxy_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      this.MoveCurrentTo((object) this.proxy.Value);
    }

    private class IListEnumerator : IEnumerator
    {
      private IList<T> collection;
      private int currentIndex;

      public object Current
      {
        get
        {
          return (object) this.collection[this.currentIndex];
        }
      }

      public IListEnumerator(IList<T> collection)
      {
        this.currentIndex = -1;
        this.collection = collection;
      }

      public bool MoveNext()
      {
        return ++this.currentIndex < this.collection.Count;
      }

      public void Reset()
      {
        this.currentIndex = -1;
      }
    }
  }
}
