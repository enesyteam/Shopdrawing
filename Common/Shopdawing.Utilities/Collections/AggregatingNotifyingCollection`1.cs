// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Utility.Collections.AggregatingNotifyingCollection`1
// Assembly: Microsoft.Expression.Utility, Version=5.0.30709.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: B77F0E80-A3D7-4861-BF76-6A6A586443F3
// Assembly location: C:\Users\M4600\Documents\Project\4.5\Microsoft.Expression.ProjectReferences\Microsoft.Expression.Utility.dll

using Microsoft.Expression.Utility.Events;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.Utility.Collections
{
  public sealed class AggregatingNotifyingCollection<T> : NotifyingCollectionBase<T>
  {
    private AggregatingNotifyingCollection(ICollection<T> hostedCollection, object notificationSender)
      : base(hostedCollection, notificationSender)
    {
    }

    public static AggregatingNotifyingCollection<T> Create(ICollection<T> hostedCollection, object notificationSender)
    {
      AggregatingNotifyingCollection<T> notifyingCollection = new AggregatingNotifyingCollection<T>(hostedCollection, notificationSender);
      notifyingCollection.EnumerationChangedEvent = (ISuspendingEventManager<EnumerationChangedEventArgs<T>>) new AggregatingSuspendingEventManager<EnumerationChangedEventArgs<T>>(new Action<EnumerationChangedEventArgs<T>>(((NotifyingCollectionBase<T>) notifyingCollection).EventInvoker), new Func<IEnumerable<EnumerationChangedEventArgs<T>>, EnumerationChangedEventArgs<T>>(EnumerationChangedEventArgs<T>.Aggregator));
      return notifyingCollection;
    }
  }
}
