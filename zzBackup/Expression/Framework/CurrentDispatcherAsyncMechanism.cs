// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.CurrentDispatcherAsyncMechanism
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.ComponentModel;
using System.Windows.Threading;

namespace Microsoft.Expression.Framework
{
  public class CurrentDispatcherAsyncMechanism : IAsyncMechanism
  {
    private DispatcherPriority priority;

    public bool IsThreaded
    {
      get
      {
        return false;
      }
    }

    public event DoWorkEventHandler DoWork;

    public CurrentDispatcherAsyncMechanism(DispatcherPriority priority)
    {
      this.priority = priority;
    }

    private object OnDoWork(object argument)
    {
      DoWorkEventArgs e = new DoWorkEventArgs(argument);
      if (this.DoWork != null)
      {
        this.DoWork((object) this, e);
        if (object.Equals(e.Result, (object) AsyncProcessResult.StillGoing))
          Dispatcher.CurrentDispatcher.BeginInvoke(this.priority, (Delegate) new DispatcherOperationCallback(this.OnDoWork), (object) null);
      }
      return (object) null;
    }

    public void Begin()
    {
      Dispatcher.CurrentDispatcher.BeginInvoke(this.priority, (Delegate) new DispatcherOperationCallback(this.OnDoWork), (object) null);
    }

    public void Kill()
    {
    }

    public void Pause()
    {
      throw new InvalidOperationException("Pause/Resume not yet supported on CurrentDispatcherAsyncMechanism");
    }

    public void Resume()
    {
      throw new InvalidOperationException("Pause/Resume not yet supported on CurrentDispatcherAsyncMechanism");
    }
  }
}
