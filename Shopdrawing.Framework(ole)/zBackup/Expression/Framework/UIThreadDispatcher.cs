// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.UIThreadDispatcher
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Threading;
using System.Windows.Threading;

namespace Microsoft.Expression.Framework
{
  public sealed class UIThreadDispatcher
  {
    private Dispatcher uiThreadDispatcher;
    private static UIThreadDispatcher dispatcher;

    public static UIThreadDispatcher Instance
    {
      get
      {
        return UIThreadDispatcher.dispatcher;
      }
      set
      {
        UIThreadDispatcher.dispatcher = value;
      }
    }

    public DispatcherHooks Hooks
    {
      get
      {
        if (this.uiThreadDispatcher != null)
          return this.uiThreadDispatcher.Hooks;
        return (DispatcherHooks) null;
      }
    }

    public bool IsUIThread
    {
      get
      {
        return this.uiThreadDispatcher == Dispatcher.CurrentDispatcher;
      }
    }

    public UIThreadDispatcher()
    {
      this.uiThreadDispatcher = Dispatcher.CurrentDispatcher;
    }

    public static void InitializeInstance()
    {
      UIThreadDispatcher.Instance = new UIThreadDispatcher();
    }

    public DispatcherOperation BeginInvoke(DispatcherPriority priority, Action action)
    {
      if (!this.uiThreadDispatcher.HasShutdownStarted)
        return this.uiThreadDispatcher.BeginInvoke(priority, (Delegate) action);
      return (DispatcherOperation) null;
    }

    public DispatcherOperation BeginInvoke<TResult>(DispatcherPriority priority, Func<TResult> func)
    {
      if (!this.uiThreadDispatcher.HasShutdownStarted)
        return this.uiThreadDispatcher.BeginInvoke(priority, (Delegate) func);
      return (DispatcherOperation) null;
    }

    public DispatcherOperation BeginInvoke<T>(DispatcherPriority priority, Action<T> action, T arg)
    {
      if (!this.uiThreadDispatcher.HasShutdownStarted)
        return this.uiThreadDispatcher.BeginInvoke(priority, (Delegate) action, (object) arg);
      return (DispatcherOperation) null;
    }

    public DispatcherOperation BeginInvoke<T, TResult>(DispatcherPriority priority, Func<T, TResult> func, T arg)
    {
      if (!this.uiThreadDispatcher.HasShutdownStarted)
        return this.uiThreadDispatcher.BeginInvoke(priority, (Delegate) func, (object) arg);
      return (DispatcherOperation) null;
    }

    public DispatcherOperation BeginInvoke<T1, T2>(DispatcherPriority priority, Action<T1, T2> action, T1 arg1, T2 arg2)
    {
      if (this.uiThreadDispatcher.HasShutdownStarted)
        return (DispatcherOperation) null;
      return this.uiThreadDispatcher.BeginInvoke(priority, (Delegate) action, (object) arg1, (object) arg2);
    }

    public void Invoke(DispatcherPriority priority, Action action)
    {
      if (this.uiThreadDispatcher.HasShutdownStarted)
        return;
      this.uiThreadDispatcher.Invoke(priority, (Delegate) action);
    }

    public void InvokeAfter(TimeSpan delay, DispatcherPriority priority, Action action)
    {
      Timer startTimer = (Timer) null;
      startTimer = new Timer((TimerCallback) (o =>
      {
        this.Invoke(priority, action);
        if (startTimer == null)
          return;
        startTimer.Dispose();
      }), (object) null, delay, new TimeSpan(0L));
    }

    public void Invoke<T>(DispatcherPriority priority, Action<T> action, T arg)
    {
      if (this.uiThreadDispatcher.HasShutdownStarted)
        return;
      this.uiThreadDispatcher.Invoke(priority, (Delegate) action, (object) arg);
    }

    public void Invoke<T1, T2>(DispatcherPriority priority, Action<T1, T2> action, T1 arg1, T2 arg2)
    {
      if (this.uiThreadDispatcher.HasShutdownStarted)
        return;
      this.uiThreadDispatcher.Invoke(priority, (Delegate) action, (object) arg1, new object[1]
      {
        (object) arg2
      });
    }

    public TResult Invoke<TResult>(DispatcherPriority priority, Func<TResult> func)
    {
      if (!this.uiThreadDispatcher.HasShutdownStarted)
        return (TResult) this.uiThreadDispatcher.Invoke(priority, (Delegate) func);
      return default (TResult);
    }

    public void DoEvents()
    {
      new DispatcherHelper().ClearFrames(this.uiThreadDispatcher);
    }
  }
}
