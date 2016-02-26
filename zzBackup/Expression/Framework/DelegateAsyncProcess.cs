// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.DelegateAsyncProcess
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.ComponentModel;

namespace Microsoft.Expression.Framework
{
  public class DelegateAsyncProcess : AsyncProcess
  {
    private Action<object, DoWorkEventArgs> workerDelegate;
    private bool stillWorking;

    public override string StatusText
    {
      get
      {
        return this.DelegateStatusText ?? base.StatusText;
      }
    }

    public string DelegateStatusText { get; set; }

    public Action<object, DoWorkEventArgs> WorkerDelegate
    {
      get
      {
        return this.workerDelegate;
      }
      set
      {
        this.workerDelegate = value;
      }
    }

    public override int Count
    {
      get
      {
        return 1;
      }
    }

    public override int CompletedCount
    {
      get
      {
        return !this.stillWorking ? 1 : 0;
      }
    }

    public DelegateAsyncProcess()
      : this((Action<object, DoWorkEventArgs>) null, (IAsyncMechanism) null)
    {
    }

    public DelegateAsyncProcess(Action<object, DoWorkEventArgs> workerDelegate)
      : this(workerDelegate, (IAsyncMechanism) null)
    {
    }

    public DelegateAsyncProcess(Action<object, DoWorkEventArgs> workerDelegate, IAsyncMechanism mechanism)
      : base(mechanism)
    {
      this.workerDelegate = workerDelegate;
      this.stillWorking = true;
    }

    protected override void Work()
    {
      DoWorkEventArgs doWorkEventArgs = new DoWorkEventArgs((object) null);
      doWorkEventArgs.Result = (object) AsyncProcessResult.Done;
      this.workerDelegate((object) this, doWorkEventArgs);
      this.stillWorking = object.Equals(doWorkEventArgs.Result, (object) AsyncProcessResult.StillGoing);
      if (!object.Equals(doWorkEventArgs.Result, (object) AsyncProcessResult.Aborted))
        return;
      this.Kill();
    }

    protected override bool MoveNext()
    {
      if (this.workerDelegate != null)
        return this.stillWorking;
      return false;
    }
  }
}
