// Decompiled with JetBrains decompiler
// Type: MS.Internal.ObservableCollectionAggregate`1
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace MS.Internal
{
  internal class ObservableCollectionAggregate<T> : INotifyCollectionChanged, ICollection<T>, IEnumerable<T>, IEnumerable
  {
    private List<ICollection<T>> _collections = new List<ICollection<T>>();

    public int Count
    {
      get
      {
        int num = 0;
        foreach (ICollection<T> collection in this._collections)
          num += collection.Count;
        return num;
      }
    }

    public bool IsReadOnly
    {
      get
      {
        return true;
      }
    }

    public event NotifyCollectionChangedEventHandler CollectionChanged;

    public void AddCollection(ICollection<T> collection)
    {
      if (collection == null)
        throw new ArgumentNullException("collection");
      this._collections.Add(collection);
      this.ConsumeCollection(collection);
    }

    public void Reset()
    {
      foreach (ICollection<T> collection in this._collections)
      {
        INotifyCollectionChanged collectionChanged = collection as INotifyCollectionChanged;
        if (collectionChanged != null)
          collectionChanged.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.OnIndividualCollectionChanged);
      }
      this._collections.Clear();
    }

    private void ConsumeCollection(ICollection<T> collection)
    {
      INotifyCollectionChanged collectionChanged = collection as INotifyCollectionChanged;
      if (collectionChanged != null)
        collectionChanged.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnIndividualCollectionChanged);
      this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    private void OnIndividualCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      int num = 0;
      if (e.Action != NotifyCollectionChangedAction.Reset)
      {
        foreach (ICollection<T> collection in this._collections)
        {
          if (!object.Equals((object) collection, sender))
            num += collection.Count;
          else
            break;
        }
      }
      NotifyCollectionChangedEventArgs e1;
      switch (e.Action)
      {
        case NotifyCollectionChangedAction.Add:
          e1 = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, e.NewItems, e.NewStartingIndex + num);
          break;
        case NotifyCollectionChangedAction.Remove:
          e1 = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, e.OldItems, e.OldStartingIndex + num);
          break;
        case NotifyCollectionChangedAction.Replace:
          e1 = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, e.NewItems, e.OldItems, e.NewStartingIndex + num);
          break;
        case NotifyCollectionChangedAction.Move:
          e1 = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, e.OldItems, e.NewStartingIndex + num, e.OldStartingIndex + num);
          break;
        default:
          e1 = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
          break;
      }
      this.OnCollectionChanged(e1);
    }

    private void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
      if (this.CollectionChanged == null)
        return;
      this.CollectionChanged((object) this, e);
    }

    public void Add(T item)
    {
      throw new InvalidOperationException();
    }

    public bool Contains(T item)
    {
      foreach (ICollection<T> collection in this._collections)
      {
        if (collection.Contains(item))
          return true;
      }
      return false;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
      foreach (ICollection<T> collection in this._collections)
      {
        collection.CopyTo(array, arrayIndex);
        arrayIndex += collection.Count;
      }
    }

    public bool Remove(T item)
    {
      throw new InvalidOperationException();
    }

    public void Clear()
    {
      throw new InvalidOperationException();
    }

    public IEnumerator<T> GetEnumerator()
    {
      foreach (ICollection<T> collection in this._collections)
      {
        foreach (T obj in (IEnumerable<T>) collection)
          yield return obj;
      }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this.GetEnumerator();
    }
  }
}
