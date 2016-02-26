// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.Classifiers.ClassificationPosition
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using System.Collections.Generic;

namespace Microsoft.Expression.Code.Classifiers
{
  public struct ClassificationPosition
  {
    public SnapshotSpan CurrentLine { get; set; }

    public IList<ClassificationSpan> CurrentSpanList { get; set; }

    public int CurrentSpanIndex { get; set; }

    public ClassificationSpan CurrentSpan
    {
      get
      {
        if (this.CurrentSpanIndex < 0)
          return new ClassificationSpan(new SnapshotSpan(this.CurrentLine.Snapshot, this.CurrentLine.Span.Start, 0), XamlAnalyzer.ClassUnknown);
        if (this.CurrentSpanIndex >= this.CurrentSpanList.Count)
          return new ClassificationSpan(new SnapshotSpan(this.CurrentLine.Snapshot, this.CurrentLine.Span.End, 0), XamlAnalyzer.ClassUnknown);
        return this.CurrentSpanList[this.CurrentSpanIndex];
      }
    }

    public ITextSnapshot Snapshot
    {
      get
      {
        return this.CurrentLine.Snapshot;
      }
    }
  }
}
