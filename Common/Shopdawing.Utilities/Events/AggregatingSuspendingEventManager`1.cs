// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Utility.Events.AggregatingSuspendingEventManager`1
// Assembly: Microsoft.Expression.Utility, Version=5.0.30709.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: B77F0E80-A3D7-4861-BF76-6A6A586443F3
// Assembly location: C:\Users\M4600\Documents\Project\4.5\Microsoft.Expression.ProjectReferences\Microsoft.Expression.Utility.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Expression.Utility.Events
{
  public class AggregatingSuspendingEventManager<TEventArgs> : SuspendingEventManager<TEventArgs> where TEventArgs : EventArgs
  {
    private Func<IEnumerable<TEventArgs>, TEventArgs> aggregator;

    public AggregatingSuspendingEventManager(Action<TEventArgs> eventInvoker, Func<IEnumerable<TEventArgs>, TEventArgs> aggregator)
      : base(eventInvoker)
    {
      this.aggregator = aggregator;
    }

    protected override IEnumerable<TEventArgs> ProcessDequeuedEvents(IEnumerable<TEventArgs> dequeuedEvents)
    {
      if (!Enumerable.Any<TEventArgs>(dequeuedEvents))
        return dequeuedEvents;
      return (IEnumerable<TEventArgs>) new TEventArgs[1]
      {
        this.aggregator(dequeuedEvents)
      };
    }
  }
}
