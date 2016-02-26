// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Utility.Collections.NotifyingCollectionBase`1
// Assembly: Microsoft.Expression.Utility, Version=5.0.30709.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: B77F0E80-A3D7-4861-BF76-6A6A586443F3
// Assembly location: C:\Users\M4600\Documents\Project\4.5\Microsoft.Expression.ProjectReferences\Microsoft.Expression.Utility.dll

using Microsoft.Expression.Utility.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.Expression.Utility.Collections
{
  public abstract class NotifyingCollectionBase<T> : ICollection<T>, INotifyingEnumerable<T>, IEnumerable<T>, IEnumerable
  {
    private ICollection<T> hostedCollection;
    private object notificationSender;

    protected ISuspendingEventManager<EnumerationChangedEventArgs<T>> EnumerationChangedEvent { private get; set; }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public IDisposable SuspendNotification
    {
      get
      {
        return this.EnumerationChangedEvent.SuspendEvent;
      }
    }

    public int Count
    {
      get
      {
        return this.hostedCollection.Count;
      }
    }

    public bool IsReadOnly
    {
      get
      {
        return this.hostedCollection.IsReadOnly;
      }
    }

    public event EventHandler<EnumerationChangedEventArgs<T>> EnumerationChanged;

    protected NotifyingCollectionBase(ICollection<T> hostedCollection, object notificationSender)
    {
      this.hostedCollection = hostedCollection;
      this.notificationSender = notificationSender;
    }

    protected void EventInvoker(EnumerationChangedEventArgs<T> eventArguments)
    {
      if (this.EnumerationChanged == null)
        return;
      this.EnumerationChanged(this.notificationSender, eventArguments);
    }

    public void SendChangedItemNotification(T item)
    {
      if (!this.Contains(item))
        return;
      this.EnumerationChangedEvent.InvokeEvent(new EnumerationChangedEventArgs<T>(item, EnumerationChangeType.ItemChanged, (string) null));
    }

    public void SendRenamedItemNotification(T item, string oldName)
    {
      if (!this.Contains(item))
        return;
      this.EnumerationChangedEvent.InvokeEvent(new EnumerationChangedEventArgs<T>(item, EnumerationChangeType.ItemRenamed, oldName));
    }

    public void Add(T item)
    {
      this.hostedCollection.Add(item);
      this.EnumerationChangedEvent.InvokeEvent(new EnumerationChangedEventArgs<T>(item, EnumerationChangeType.ItemAdded, (string) null));
    }

    public void Clear()
    {
      foreach (T obj in Enumerable.ToArray<T>((IEnumerable<T>) this.hostedCollection))
        this.Remove(obj);
    }

    public bool Contains(T item)
    {
      return this.hostedCollection.Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
      this.hostedCollection.CopyTo(array, arrayIndex);
    }

    public bool Remove(T item)
    {
      bool flag = this.hostedCollection.Remove(item);
      if (flag)
        this.EnumerationChangedEvent.InvokeEvent(new EnumerationChangedEventArgs<T>(item, EnumerationChangeType.ItemRemoved, (string) null));
      return flag;
    }

    public IEnumerator<T> GetEnumerator()
    {
      return this.hostedCollection.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this.hostedCollection.GetEnumerator();
    }
  }
}
