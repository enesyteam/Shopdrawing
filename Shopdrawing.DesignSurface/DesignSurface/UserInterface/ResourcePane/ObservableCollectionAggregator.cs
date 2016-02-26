// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.ResourcePane.ObservableCollectionAggregator
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Microsoft.Expression.DesignSurface.UserInterface.ResourcePane
{
  public sealed class ObservableCollectionAggregator : IList, ICollection, IEnumerable, INotifyCollectionChanged
  {
    private List<IList> mergedCollections;

    bool IList.IsFixedSize
    {
      get
      {
        return false;
      }
    }

    bool IList.IsReadOnly
    {
      get
      {
        return true;
      }
    }

    object IList.this[int index]
    {
      get
      {
        int index1;
        for (index1 = 0; index1 < this.mergedCollections.Count && index >= this.mergedCollections[index1].Count; ++index1)
          index -= this.mergedCollections[index1].Count;
        return this.mergedCollections[index1][index];
      }
      set
      {
        throw new InvalidOperationException();
      }
    }

    int ICollection.Count
    {
      get
      {
        int num = 0;
        foreach (IList list in this.mergedCollections)
          num += list.Count;
        return num;
      }
    }

    bool ICollection.IsSynchronized
    {
      get
      {
        return true;
      }
    }

    object ICollection.SyncRoot
    {
      get
      {
        return (object) this;
      }
    }

    public event NotifyCollectionChangedEventHandler CollectionChanged;

    public ObservableCollectionAggregator()
    {
      this.mergedCollections = new List<IList>();
    }

    public void AddCollection(IList collection)
    {
      this.mergedCollections.Add(collection);
      INotifyCollectionChanged collectionChanged = collection as INotifyCollectionChanged;
      if (collectionChanged != null)
        collectionChanged.CollectionChanged += new NotifyCollectionChangedEventHandler(this.ChildCollection_Changed);
      this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    public bool RemoveCollection(IList collection)
    {
      if (!this.mergedCollections.Remove(collection))
        return false;
      INotifyCollectionChanged collectionChanged = collection as INotifyCollectionChanged;
      if (collectionChanged != null)
        collectionChanged.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.ChildCollection_Changed);
      this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
      return true;
    }

    private void ChildCollection_Changed(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (e.Action != NotifyCollectionChangedAction.Reset)
      {
        int num = 0;
        for (int index = 0; index < this.mergedCollections.Count; ++index)
        {
          if (this.mergedCollections[index] != sender)
          {
            num += this.mergedCollections[index].Count;
          }
          else
          {
            if (e.Action == NotifyCollectionChangedAction.Move)
            {
              this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(e.Action, e.OldItems, e.OldStartingIndex + num, e.NewStartingIndex + num));
              return;
            }
            if (e.Action == NotifyCollectionChangedAction.Replace)
            {
              this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(e.Action, e.NewItems, e.OldItems, e.NewStartingIndex + num));
              return;
            }
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
              this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(e.Action, e.NewItems, e.NewStartingIndex + num));
              return;
            }
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
              this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(e.Action, e.OldItems, e.OldStartingIndex + num));
              return;
            }
          }
        }
      }
      this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    private void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
    {
      if (this.CollectionChanged == null)
        return;
      this.CollectionChanged((object) this, args);
    }

    int IList.Add(object value)
    {
      throw new InvalidOperationException();
    }

    public void Clear()
    {
      foreach (IList list in this.mergedCollections)
      {
        INotifyCollectionChanged collectionChanged = list as INotifyCollectionChanged;
        if (collectionChanged != null)
          collectionChanged.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.ChildCollection_Changed);
      }
      this.mergedCollections.Clear();
      this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    public bool Contains(object value)
    {
      foreach (object obj in (IEnumerable) this)
      {
        if (obj == value)
          return true;
      }
      return false;
    }

    int IList.IndexOf(object value)
    {
      int num = 0;
      foreach (object obj in (IEnumerable) this)
      {
        if (obj == value)
          return num;
        ++num;
      }
      return -1;
    }

    void IList.Insert(int index, object value)
    {
      throw new InvalidOperationException();
    }

    void IList.Remove(object value)
    {
      throw new InvalidOperationException();
    }

    void IList.RemoveAt(int index)
    {
      throw new InvalidOperationException();
    }

    void ICollection.CopyTo(Array array, int index)
    {
      throw new InvalidOperationException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      foreach (IList list in this.mergedCollections)
      {
        foreach (object obj in (IEnumerable) list)
          yield return obj;
      }
    }
  }
}
