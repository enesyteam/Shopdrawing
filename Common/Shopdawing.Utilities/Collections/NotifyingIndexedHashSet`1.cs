// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Utility.Collections.NotifyingIndexedHashSet`1
// Assembly: Microsoft.Expression.Utility, Version=5.0.30709.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: B77F0E80-A3D7-4861-BF76-6A6A586443F3
// Assembly location: C:\Users\M4600\Documents\Project\4.5\Microsoft.Expression.ProjectReferences\Microsoft.Expression.Utility.dll

using Microsoft.Expression.Utility.Events;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Expression.Utility.Collections
{
  public class NotifyingIndexedHashSet<T> : IndexedHashSet<T>, INotifyingEnumerable<T>, IEnumerable<T>, IEnumerable where T : class
  {
    private AggregatingSuspendingEventManager<EnumerationChangedEventArgs<T>> EnumerationChangedEvent;

    public IDisposable SuspendNotification
    {
      get
      {
        return this.EnumerationChangedEvent.SuspendEvent;
      }
    }

    public event EventHandler<EnumerationChangedEventArgs<T>> EnumerationChanged;

    protected NotifyingIndexedHashSet(object notificationSender, IEqualityComparer<T> equalityComparer, int initialCapacity, Action<EnumerationChangedEventArgs<T>> eventClearedCallback)
      : base(equalityComparer, initialCapacity)
    {
      NotifyingIndexedHashSet<T> notifyingIndexedHashSet = this;
      object sender = notificationSender ?? (object) this;
      this.EnumerationChangedEvent = new AggregatingSuspendingEventManager<EnumerationChangedEventArgs<T>>((Action<EnumerationChangedEventArgs<T>>) (eventArguments =>
      {
        if (closure_0.EnumerationChanged != null)
          closure_0.EnumerationChanged(sender, eventArguments);
        if (eventClearedCallback == null)
          return;
        eventClearedCallback(eventArguments);
      }), new Func<IEnumerable<EnumerationChangedEventArgs<T>>, EnumerationChangedEventArgs<T>>(EnumerationChangedEventArgs<T>.Aggregator));
    }

    public static NotifyingIndexedHashSet<T> Create(object notificationSender = null, int initialCapacity = 72, IEqualityComparer<T> equalityComparer = null, Action<EnumerationChangedEventArgs<T>> eventClearedCallback = null)
    {
      return new NotifyingIndexedHashSet<T>(notificationSender, equalityComparer, initialCapacity, eventClearedCallback);
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

    protected override bool AddIfNotPresent(T item)
    {
      if (!base.AddIfNotPresent(item))
        return false;
      this.EnumerationChangedEvent.InvokeEvent(new EnumerationChangedEventArgs<T>(item, EnumerationChangeType.ItemAdded, (string) null));
      return true;
    }

    public override bool Remove(T item)
    {
      if (!base.Remove(item))
        return false;
      this.EnumerationChangedEvent.InvokeEvent(new EnumerationChangedEventArgs<T>(item, EnumerationChangeType.ItemRemoved, (string) null));
      return true;
    }
  }
}
