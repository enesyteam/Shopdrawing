// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Diagnostics.PerformanceSummary
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Microsoft.Expression.Framework.Diagnostics
{
  internal class PerformanceSummary
  {
    private List<PerformanceSequence> sequences = new List<PerformanceSequence>();

    public List<PerformanceSequence> Sequences
    {
      get
      {
        return this.sequences;
      }
    }

    public void AddSequence(PerformanceSequence sequence)
    {
      this.sequences.Add(sequence);
    }

    public override string ToString()
    {
      using (StringWriter stringWriter = new StringWriter((IFormatProvider) CultureInfo.CurrentCulture))
      {
        stringWriter.WriteLine("PerformanceSummary");
        foreach (PerformanceSequence performanceSequence in this.sequences)
        {
          stringWriter.Write(performanceSequence.Name);
          stringWriter.Write(" : # " + (object) performanceSequence.IterationCount);
          stringWriter.Write(" : Time " + PerformanceSummary.PerformanceConstants.TicksToSeconds((double) performanceSequence.TotalElapsedTime).ToString((IFormatProvider) CultureInfo.CurrentCulture));
          stringWriter.Write(" : Avg. Time " + PerformanceSummary.PerformanceConstants.TicksToSeconds(performanceSequence.AverageDuration).ToString((IFormatProvider) CultureInfo.CurrentCulture));
          stringWriter.WriteLine();
        }
        return stringWriter.ToString();
      }
    }

    private static class PerformanceConstants
    {
      private static readonly double TicksPerSecond = 1000.0;

      public static double TicksToSeconds(double ticks)
      {
        return ticks / PerformanceSummary.PerformanceConstants.TicksPerSecond;
      }
    }
  }
}
