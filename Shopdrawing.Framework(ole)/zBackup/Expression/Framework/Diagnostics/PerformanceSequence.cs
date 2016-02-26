// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Diagnostics.PerformanceSequence
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System.Collections.Generic;

namespace Microsoft.Expression.Framework.Diagnostics
{
  internal class PerformanceSequence
  {
    private long minimumDuration = long.MaxValue;
    private long maximumDuration = -1L;
    private Stack<PerformanceSequenceIteration> openIterations = new Stack<PerformanceSequenceIteration>();
    private List<PerformanceSequenceIteration> iterations = new List<PerformanceSequenceIteration>();
    private string name;
    private long totalElapsedTime;

    public string Name
    {
      get
      {
        return this.name;
      }
    }

    public List<PerformanceSequenceIteration> Iterations
    {
      get
      {
        return this.iterations;
      }
    }

    public int IterationCount
    {
      get
      {
        return this.iterations.Count;
      }
    }

    public long TotalElapsedTime
    {
      get
      {
        return this.totalElapsedTime;
      }
    }

    public double AverageDuration
    {
      get
      {
        return (double) this.TotalElapsedTime / (double) this.IterationCount;
      }
    }

    public long MaximumDuration
    {
      get
      {
        return this.maximumDuration;
      }
    }

    public long MinimumDuration
    {
      get
      {
        return this.minimumDuration;
      }
    }

    public PerformanceSequence(string name)
    {
      this.name = name;
    }

    public void AddStart(long timeStamp)
    {
      this.openIterations.Push(new PerformanceSequenceIteration(timeStamp));
    }

    public void AddEnd(long timeStamp)
    {
      if (this.openIterations.Count < 1)
        return;
      PerformanceSequenceIteration sequenceIteration = this.openIterations.Pop();
      sequenceIteration.AddEnd(timeStamp);
      this.iterations.Add(sequenceIteration);
      if (sequenceIteration.Duration > this.maximumDuration)
        this.maximumDuration = sequenceIteration.Duration;
      if (sequenceIteration.Duration < this.minimumDuration)
        this.minimumDuration = sequenceIteration.Duration;
      this.totalElapsedTime += sequenceIteration.Duration;
    }
  }
}
