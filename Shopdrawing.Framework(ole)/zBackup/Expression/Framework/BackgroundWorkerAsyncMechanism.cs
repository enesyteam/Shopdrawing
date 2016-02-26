// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.BackgroundWorkerAsyncMechanism
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.ComponentModel;

namespace Microsoft.Expression.Framework
{
  public sealed class BackgroundWorkerAsyncMechanism : IAsyncMechanism, IDisposable
  {
    private BackgroundWorker workerImpl;
    private BackgroundWorkMode workMode;
    private bool isPaused;

    public bool IsThreaded
    {
      get
      {
        return false;
      }
    }

    public event DoWorkEventHandler DoWork;

    public BackgroundWorkerAsyncMechanism(BackgroundWorkMode workMode)
    {
      this.workMode = workMode;
    }

    private void PlaysNicely_DoWork(object sender, DoWorkEventArgs e)
    {
      DoWorkEventArgs e1 = new DoWorkEventArgs(e.Argument);
      if (this.DoWork == null)
        return;
      this.DoWork(sender, e1);
      if (!object.Equals(e1.Result, (object) AsyncProcessResult.StillGoing))
        return;
      this.Begin();
    }

    private void ThreadHog_DoWork(object sender, DoWorkEventArgs e)
    {
      DoWorkEventArgs e1 = new DoWorkEventArgs(e.Argument);
      while (this.DoWork != null && !this.isPaused)
      {
        this.DoWork(sender, e1);
        if (!object.Equals(e1.Result, (object) AsyncProcessResult.StillGoing) || this.workerImpl.CancellationPending)
          break;
      }
    }

    public void Begin()
    {
      this.ResetWorker(true);
    }

    public void Kill()
    {
      this.ResetWorker(false);
    }

    public void Pause()
    {
      this.isPaused = true;
    }

    public void Resume()
    {
      this.isPaused = false;
    }

    private void ResetWorker(bool restart)
    {
      if (this.workerImpl != null)
      {
        this.workerImpl.CancelAsync();
        this.workerImpl.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(this.OnRunWorkerCompleted);
        if (this.workMode == BackgroundWorkMode.PlaysNicely)
          this.workerImpl.DoWork -= new DoWorkEventHandler(this.PlaysNicely_DoWork);
        else
          this.workerImpl.DoWork -= new DoWorkEventHandler(this.ThreadHog_DoWork);
        this.workerImpl.Dispose();
        this.workerImpl = (BackgroundWorker) null;
      }
      if (!restart)
        return;
      this.workerImpl = new BackgroundWorker();
      this.workerImpl.WorkerSupportsCancellation = true;
      this.workerImpl.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.OnRunWorkerCompleted);
      if (this.workMode == BackgroundWorkMode.PlaysNicely)
        this.workerImpl.DoWork += new DoWorkEventHandler(this.PlaysNicely_DoWork);
      else
        this.workerImpl.DoWork += new DoWorkEventHandler(this.ThreadHog_DoWork);
      this.workerImpl.RunWorkerAsync();
    }

    private void OnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      BackgroundWorker backgroundWorker = (BackgroundWorker) sender;
      backgroundWorker.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(this.OnRunWorkerCompleted);
      backgroundWorker.Dispose();
    }

    public void Dispose()
    {
      if (this.workerImpl != null)
      {
        this.workerImpl.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(this.OnRunWorkerCompleted);
        this.workerImpl.Dispose();
        this.workerImpl = (BackgroundWorker) null;
      }
      GC.SuppressFinalize((object) this);
    }
  }
}
