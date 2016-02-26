// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.EnumeratingAsyncProcess`1
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Expression.Framework
{
  public class EnumeratingAsyncProcess<T> : AsyncProcess
  {
    private IEnumerator<T> enumerator;
    private Action<T> action;
    private int count;
    private int completedCount;

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

    public EnumeratingAsyncProcess(IAsyncMechanism mechanism, IEnumerable<T> toProcess, Action<T> processor)
      : base(mechanism)
    {
      this.enumerator = toProcess.GetEnumerator();
      this.action = processor;
      this.count = Enumerable.Count<T>(toProcess);
      this.completedCount = 0;
    }

    protected override void Work()
    {
      this.action(this.enumerator.Current);
      ++this.completedCount;
    }

    protected override bool MoveNext()
    {
      return this.enumerator.MoveNext();
    }
  }
}
