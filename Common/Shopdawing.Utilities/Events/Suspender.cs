// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Utility.Events.Suspender
// Assembly: Microsoft.Expression.Utility, Version=5.0.30709.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: B77F0E80-A3D7-4861-BF76-6A6A586443F3
// Assembly location: C:\Users\M4600\Documents\Project\4.5\Microsoft.Expression.ProjectReferences\Microsoft.Expression.Utility.dll

using System;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.Expression.Utility.Events
{
  public class Suspender
  {
    private int suspendCount;
    private Action suspensionLiftedAction;
    private Action suspensionActivatedAction;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public IDisposable Suspend
    {
      get
      {
        return (IDisposable) new Suspender.SuspendDisposer(this);
      }
    }

    public bool Suspended
    {
      get
      {
        return this.suspendCount > 0;
      }
    }

    public Suspender(Action suspensionLiftedAction = null, Action suspensionActivatedAction = null)
    {
      this.suspensionLiftedAction = suspensionLiftedAction;
      this.suspensionActivatedAction = suspensionActivatedAction;
    }

    private class SuspendDisposer : IDisposable
    {
      private Suspender suspender;

      public SuspendDisposer(Suspender suspender)
      {
        this.suspender = suspender;
        if (Interlocked.Increment(ref this.suspender.suspendCount) != 1)
          return;
        Action action = this.suspender.suspensionActivatedAction;
        if (action == null)
          return;
        action();
      }

      public void Dispose()
      {
        GC.SuppressFinalize((object) this);
        this.Dispose(true);
      }

      private void Dispose(bool disposing)
      {
        if (!disposing || Interlocked.Decrement(ref this.suspender.suspendCount) >= 1)
          return;
        Action action = this.suspender.suspensionLiftedAction;
        if (action == null)
          return;
        action();
      }
    }
  }
}
