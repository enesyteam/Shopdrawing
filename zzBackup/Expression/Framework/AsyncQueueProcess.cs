// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.AsyncQueueProcess
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System.Collections.Generic;

namespace Microsoft.Expression.Framework
{
  public class AsyncQueueProcess : AsyncProcess
  {
    private Queue<AsyncProcess> queue = new Queue<AsyncProcess>();
    private object currentProcessLock = new object();
    private int count;
    private int completedCount;
    private AsyncProcess currentProcess;

    public override string StatusText
    {
      get
      {
        if (this.currentProcess != null)
          return this.currentProcess.StatusText;
        return base.StatusText;
      }
    }

    public override int CompletedCount
    {
      get
      {
        return this.completedCount;
      }
    }

    public override int Count
    {
      get
      {
        return this.count;
      }
    }

    public int ScheduledCount
    {
      get
      {
        return this.queue.Count;
      }
    }

    public AsyncQueueProcess(IAsyncMechanism mechanism)
      : base(mechanism)
    {
      this.InternalReset();
    }

    public void Add(AsyncProcess job, bool beginProcessingAgain)
    {
      this.count += job.Count;
      this.completedCount += job.CompletedCount;
      bool flag;
      lock (this.queue)
      {
        flag = this.queue.Count == 0;
        this.queue.Enqueue(job);
      }
      if (!flag || !beginProcessingAgain || this.IsPaused)
        return;
      this.Begin();
    }

    protected override void InternalPause()
    {
      base.InternalPause();
      lock (this.currentProcessLock)
      {
        if (this.currentProcess == null)
          return;
        this.completedCount -= this.currentProcess.CompletedCount;
        this.currentProcess.Finish();
        this.completedCount += this.currentProcess.CompletedCount;
        this.currentProcess = (AsyncProcess) null;
      }
    }

    public void ForEach(ForEachAction action, object hint)
    {
      lock (this.queue)
      {
        foreach (AsyncProcess item_0 in this.queue)
          action(item_0, hint);
      }
    }

    public void Clear()
    {
      this.Kill();
      lock (this.queue)
        this.queue.Clear();
      this.count = 0;
      this.Reset();
    }

    public override void Kill()
    {
      lock (this.currentProcessLock)
      {
        if (this.currentProcess != null)
        {
          this.currentProcess.Kill();
          this.currentProcess = (AsyncProcess) null;
        }
      }
      base.Kill();
    }

    public override void Reset()
    {
      this.InternalReset();
    }

    protected override void InternalBegin()
    {
      lock (this.currentProcessLock)
        this.currentProcess = (AsyncProcess) null;
    }

    protected override void Work()
    {
      if (!this.IsAlive)
        return;
      lock (this.currentProcessLock)
      {
        if (this.currentProcess == null)
          return;
        AsyncProcess local_0 = this.currentProcess;
        this.completedCount -= local_0.CompletedCount;
        this.currentProcess.WorkWrapper();
        this.completedCount += local_0.CompletedCount;
      }
    }

    protected override bool MoveNext()
    {
      if (!this.IsAlive || this.IsPaused)
        return false;
      lock (this.currentProcessLock)
      {
        if (this.currentProcess == null || !this.currentProcess.MoveNextWrapper())
        {
          lock (this.queue)
          {
            while (this.queue.Count > 0)
            {
              this.currentProcess = this.queue.Dequeue();
              this.currentProcess.Begin();
              if (this.currentProcess != null)
              {
                if (this.currentProcess.MoveNextWrapper())
                  goto label_13;
              }
              else
                goto label_13;
            }
            this.currentProcess = (AsyncProcess) null;
            return false;
          }
        }
label_13:
        return true;
      }
    }

    private void InternalReset()
    {
      lock (this.currentProcessLock)
        this.currentProcess = (AsyncProcess) null;
      this.completedCount = 0;
      this.count = 0;
      lock (this.queue)
        this.queue.Clear();
    }
  }
}
