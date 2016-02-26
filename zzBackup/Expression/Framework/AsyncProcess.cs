// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.AsyncProcess
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.ComponentModel;

namespace Microsoft.Expression.Framework
{
  public abstract class AsyncProcess
  {
    private bool isAlive;
    private IAsyncMechanism asyncMechanism;

    public bool IsPaused { get; private set; }

    public IAsyncMechanism AsyncMechanism
    {
      get
      {
        return this.asyncMechanism;
      }
    }

    public abstract int CompletedCount { get; }

    public abstract int Count { get; }

    public bool IsAlive
    {
      get
      {
        return this.isAlive;
      }
      set
      {
        this.isAlive = value;
      }
    }

    public virtual string StatusText
    {
      get
      {
        return StringTable.AsyncProcessStatusText;
      }
    }

    public event EventHandler Begun;

    public event EventHandler Progress;

    public event EventHandler Complete;

    public event EventHandler Killed;

    protected AsyncProcess(IAsyncMechanism asyncMechanism)
    {
      this.asyncMechanism = asyncMechanism;
    }

    protected abstract void Work();

    protected abstract bool MoveNext();

    public virtual void Kill()
    {
      this.isAlive = false;
      if (this.asyncMechanism != null)
      {
        this.asyncMechanism.Kill();
        this.asyncMechanism.DoWork -= new DoWorkEventHandler(this.DoWork);
      }
      this.OnKilled();
    }

    public void Begin()
    {
      this.isAlive = true;
      this.InternalBegin();
      if (this.asyncMechanism != null)
      {
        this.asyncMechanism.DoWork += new DoWorkEventHandler(this.DoWork);
        this.asyncMechanism.Begin();
      }
      this.OnBegun();
    }

    public void Pause()
    {
      this.IsPaused = true;
      this.InternalPause();
      if (this.asyncMechanism == null)
        return;
      this.asyncMechanism.Pause();
      this.asyncMechanism.DoWork -= new DoWorkEventHandler(this.DoWork);
    }

    public void Resume()
    {
      this.IsPaused = false;
      if (this.asyncMechanism != null)
        this.asyncMechanism.Resume();
      this.Begin();
    }

    protected virtual void InternalBegin()
    {
      this.Reset();
    }

    protected virtual void InternalPause()
    {
    }

    public void Finish()
    {
      if (!this.isAlive)
        return;
      if (this.asyncMechanism != null)
      {
        this.asyncMechanism.Kill();
        this.asyncMechanism.DoWork -= new DoWorkEventHandler(this.DoWork);
      }
      while (this.MoveNextWrapper())
        this.WorkWrapper();
    }

    internal void WorkWrapper()
    {
      this.Work();
      this.OnProgress();
    }

    internal bool MoveNextWrapper()
    {
      if (!this.isAlive)
        return false;
      bool flag = this.MoveNext();
      if (!flag)
      {
        this.isAlive = false;
        this.OnComplete();
      }
      return flag;
    }

    public virtual void Reset()
    {
    }

    private void DoWork(object sender, DoWorkEventArgs e)
    {
      if (this.isAlive && this.MoveNextWrapper())
      {
        this.WorkWrapper();
        e.Result = (object) AsyncProcessResult.StillGoing;
      }
      else
      {
        e.Result = (object) AsyncProcessResult.Done;
        if (this.asyncMechanism == null)
          return;
        this.asyncMechanism.DoWork -= new DoWorkEventHandler(this.DoWork);
      }
    }

    protected void OnBegun()
    {
      if (this.Begun == null)
        return;
      this.Begun((object) this, EventArgs.Empty);
    }

    protected void OnProgress()
    {
      if (this.Progress == null)
        return;
      this.Progress((object) this, EventArgs.Empty);
    }

    protected void OnComplete()
    {
      if (this.Complete == null)
        return;
      this.Complete((object) this, EventArgs.Empty);
    }

    protected void OnKilled()
    {
      if (this.Killed == null)
        return;
      this.Killed((object) this, EventArgs.Empty);
    }
  }
}
