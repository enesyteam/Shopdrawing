// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Utility.Events.SuspendingEventManager`1
// Assembly: Microsoft.Expression.Utility, Version=5.0.30709.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: B77F0E80-A3D7-4861-BF76-6A6A586443F3
// Assembly location: C:\Users\M4600\Documents\Project\4.5\Microsoft.Expression.ProjectReferences\Microsoft.Expression.Utility.dll

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.Expression.Utility.Events
{
  public class SuspendingEventManager<TEventArgs> : ISuspendingEventManager<TEventArgs> where TEventArgs : EventArgs
  {
    private ConcurrentQueue<TEventArgs> eventQueue = new ConcurrentQueue<TEventArgs>();
    private Action<TEventArgs> eventInvoker;
    private Suspender suspender;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public IDisposable SuspendEvent
    {
      get
      {
        return this.suspender.Suspend;
      }
    }

    public SuspendingEventManager(Action<TEventArgs> eventInvoker)
    {
      this.eventInvoker = eventInvoker;
      this.suspender = new Suspender(new Action(this.ForwardEvents), (Action) null);
    }

    public void InvokeEvent(TEventArgs eventArgs)
    {
      this.eventQueue.Enqueue(eventArgs);
      if (this.suspender.Suspended)
        return;
      this.ForwardEvents();
    }

    private IEnumerable<TEventArgs> DequeueEvents()
    {
      TEventArgs eventArguments;
      while (this.eventQueue.TryDequeue(out eventArguments))
        yield return eventArguments;
    }

    private void ForwardEvents()
    {
      foreach (TEventArgs eventArgs in this.ProcessDequeuedEvents((IEnumerable<TEventArgs>) Enumerable.ToArray<TEventArgs>(this.DequeueEvents())))
        this.eventInvoker(eventArgs);
    }

    protected virtual IEnumerable<TEventArgs> ProcessDequeuedEvents(IEnumerable<TEventArgs> dequeuedEvents)
    {
      return dequeuedEvents;
    }
  }
}
