// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.AsyncExecuteCommandProcess
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework;
using System;
using System.ComponentModel;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  public class AsyncExecuteCommandProcess : DelegateAsyncProcess
  {
    private const int totalCount = 100;
    private AsyncProgressInfo progressInfo;
    private Action<object, DoWorkEventArgs> outsideWorkerDelegate;

    public string Name { get; private set; }

    public double CompleteRatio { get; private set; }

    public bool Cancel { get; set; }

    public override int CompletedCount
    {
      get
      {
        return (int) (this.progressInfo.CompleteRatio * 100.0);
      }
    }

    public override int Count
    {
      get
      {
        return 100;
      }
    }

    internal AsyncExecuteCommandProcess(string processName, Action<object, DoWorkEventArgs> WorkerDelegate)
    {
      this.Name = processName;
      this.WorkerDelegate = new Action<object, DoWorkEventArgs>(this.InternalWorker);
      this.outsideWorkerDelegate = WorkerDelegate;
      this.progressInfo = new AsyncProgressInfo()
      {
        CompleteRatio = 0.0,
        Status = AsyncProcessResult.Done
      };
    }

    private void InternalWorker(object sender, DoWorkEventArgs args)
    {
      if (!this.Cancel)
      {
        DoWorkEventArgs doWorkEventArgs = new DoWorkEventArgs((object) this.progressInfo);
        this.outsideWorkerDelegate(sender, doWorkEventArgs);
        this.progressInfo = (AsyncProgressInfo) doWorkEventArgs.Result;
        args.Result = (object) this.progressInfo.Status;
      }
      else
        args.Result = (object) AsyncProcessResult.Aborted;
    }

    protected override bool MoveNext()
    {
      if (!this.Cancel)
        return base.MoveNext();
      return false;
    }
  }
}
