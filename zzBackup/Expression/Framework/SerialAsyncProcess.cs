// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.SerialAsyncProcess
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System.Collections.Generic;

namespace Microsoft.Expression.Framework
{
  public class SerialAsyncProcess : AsyncProcess
  {
    private List<AsyncProcess> processes;
    private int currentProcessIndex;

    public override int Count
    {
      get
      {
        int num = 0;
        for (int index = 0; index < this.processes.Count; ++index)
          num += this.processes[index].Count;
        return num;
      }
    }

    public override int CompletedCount
    {
      get
      {
        int num = 0;
        if (this.currentProcessIndex >= 0)
        {
          for (int index = 0; index < this.currentProcessIndex; ++index)
            num += this.processes[index].CompletedCount;
          if (this.currentProcessIndex < this.processes.Count)
            num += this.processes[this.currentProcessIndex].CompletedCount;
        }
        return num;
      }
    }

    public override string StatusText
    {
      get
      {
        if (this.IsAlive && this.currentProcessIndex >= 0 && this.currentProcessIndex < this.processes.Count)
          return this.processes[this.currentProcessIndex].StatusText;
        return string.Empty;
      }
    }

    public SerialAsyncProcess(IAsyncMechanism mechanism)
      : base(mechanism)
    {
      this.processes = new List<AsyncProcess>();
      this.currentProcessIndex = -1;
    }

    public SerialAsyncProcess(IAsyncMechanism mechanism, params AsyncProcess[] processes)
      : base(mechanism)
    {
      this.processes = new List<AsyncProcess>((IEnumerable<AsyncProcess>) processes);
      this.currentProcessIndex = -1;
    }

    public void Add(AsyncProcess newProcess)
    {
      this.processes.Add(newProcess);
    }

    public void Clear()
    {
      this.processes.Clear();
    }

    public override void Reset()
    {
      this.currentProcessIndex = -1;
      base.Reset();
    }

    protected override void Work()
    {
      if (this.currentProcessIndex >= this.processes.Count)
        return;
      this.processes[this.currentProcessIndex].WorkWrapper();
    }

    protected override bool MoveNext()
    {
      if (this.currentProcessIndex >= this.processes.Count)
        return false;
      if (this.currentProcessIndex >= 0 && this.processes[this.currentProcessIndex].MoveNextWrapper())
        return true;
      ++this.currentProcessIndex;
      if (this.currentProcessIndex >= this.processes.Count)
        return false;
      this.processes[this.currentProcessIndex].Begin();
      return this.processes[this.currentProcessIndex].MoveNextWrapper();
    }
  }
}
