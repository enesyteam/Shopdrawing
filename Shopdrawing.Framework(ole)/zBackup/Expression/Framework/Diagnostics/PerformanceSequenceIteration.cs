// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Diagnostics.PerformanceSequenceIteration
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

namespace Microsoft.Expression.Framework.Diagnostics
{
  internal class PerformanceSequenceIteration
  {
    private long startTime = -1L;
    private long endTime = -1L;

    public long StartTime
    {
      get
      {
        return this.startTime;
      }
    }

    public long Duration
    {
      get
      {
        return this.endTime - this.startTime;
      }
    }

    public PerformanceSequenceIteration(long startTime)
    {
      this.startTime = startTime;
      this.endTime = -1L;
    }

    public void AddEnd(long timeStamp)
    {
      this.endTime = timeStamp;
    }

    public bool IsOpen()
    {
      return this.endTime == -1L;
    }

    public bool IsValidSequence()
    {
      if (this.startTime >= 0L)
        return this.endTime >= this.startTime;
      return false;
    }
  }
}
