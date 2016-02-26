// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.Classifiers.StatefulClassifier`1
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using Microsoft.VisualStudio.ApplicationModel.Environments;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.Code.Classifiers
{
  internal class StatefulClassifier<T> : IClassifier where T : struct, IComparable<T>
  {
    private IClassificationScanner<T> scanner;
    private IStateManagement<T> stateManagement;

    public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;

    public StatefulClassifier(ITextBuffer textBuffer, IClassificationScanner<T> scanner, IEnvironment environment)
      : this(scanner, (IStateManagement<T>) new LineBasedStateManagement<T>(textBuffer, scanner, environment))
    {
    }

    public StatefulClassifier(IClassificationScanner<T> scanner, IStateManagement<T> stateManagement)
    {
      if (scanner == null)
        throw new ArgumentNullException("scanner");
      if (stateManagement == null)
        throw new ArgumentNullException("stateManagement");
      this.scanner = scanner;
      this.stateManagement = stateManagement;
      this.stateManagement.ClassificationChanged += (EventHandler<ClassificationChangedEventArgs>) ((sender, e) => this.RaiseClassificationChangedEvent(e));
    }

    public T GetState(ITextSnapshot snapshot, int startPosition)
    {
      return this.stateManagement.GetState(snapshot, startPosition);
    }

    public SnapshotSpan GetSpanToTokenize(SnapshotSpan span)
    {
      return this.stateManagement.GetSpanToTokenize(span);
    }

    public SnapshotSpan GetSpanToTokenize(SnapshotPoint point)
    {
      return this.stateManagement.GetSpanToTokenize(new SnapshotSpan(point.Snapshot, point.Position, 0));
    }

    public SnapshotSpan GetPreviousSpanToTokenize(ITextSnapshot snapshot, int startPosition)
    {
      if (startPosition == 0)
        return new SnapshotSpan();
      --startPosition;
      return this.stateManagement.GetSpanToTokenize(new SnapshotSpan(snapshot, new Span(startPosition, 0)));
    }

    public SnapshotSpan GetNextSpanToTokenize(ITextSnapshot snapshot, int endPosition)
    {
      if (endPosition == snapshot.Length)
        return new SnapshotSpan();
      ++endPosition;
      return this.stateManagement.GetSpanToTokenize(new SnapshotSpan(snapshot, new Span(endPosition, 0)));
    }

    public IEnumerable<ClassificationPosition> ScanForward(ClassificationPosition startPosition)
    {
      ClassificationPosition currentPosition = startPosition;
      while (!currentPosition.CurrentLine.IsEmpty)
      {
        for (; currentPosition.CurrentSpanIndex < currentPosition.CurrentSpanList.Count; ++currentPosition.CurrentSpanIndex)
          yield return currentPosition;
        currentPosition.CurrentLine = this.GetNextSpanToTokenize(currentPosition.Snapshot, currentPosition.CurrentLine.End);
        if (!currentPosition.CurrentLine.IsEmpty)
        {
          currentPosition.CurrentSpanList = this.GetClassificationSpans(currentPosition.CurrentLine);
          currentPosition.CurrentSpanIndex = 0;
        }
      }
    }

    public IEnumerable<ClassificationPosition> ScanBackward(ClassificationPosition startPosition)
    {
      ClassificationPosition currentPosition = startPosition;
      --currentPosition.CurrentSpanIndex;
      while (!currentPosition.CurrentLine.IsEmpty)
      {
        for (; currentPosition.CurrentSpanIndex >= 0; --currentPosition.CurrentSpanIndex)
          yield return currentPosition;
        currentPosition.CurrentLine = this.GetPreviousSpanToTokenize(currentPosition.Snapshot, currentPosition.CurrentLine.Start);
        if (!currentPosition.CurrentLine.IsEmpty)
        {
          currentPosition.CurrentSpanList = this.GetClassificationSpans(currentPosition.CurrentLine);
          currentPosition.CurrentSpanIndex = currentPosition.CurrentSpanList.Count - 1;
        }
      }
    }

    public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
    {
      SnapshotSpan spanToTokenize = this.stateManagement.GetSpanToTokenize(span);
      T endState = this.stateManagement.GetState(span.Snapshot, spanToTokenize.Span.Start);
      IList<ClassificationSpan> classificationSpans = this.scanner.GetClassificationSpans(spanToTokenize, endState, out endState);
      this.stateManagement.SetState(span.Snapshot, spanToTokenize.Span.End, endState);
      List<ClassificationSpan> list = new List<ClassificationSpan>();
      foreach (ClassificationSpan classificationSpan in (IEnumerable<ClassificationSpan>) classificationSpans)
      {
        Span span1 = (Span) classificationSpan.Span;
        if (span1.Start < span.Span.End && span1.End >= span.Span.Start)
        {
          int start = Math.Max(span1.Start, span.Span.Start);
          int num = Math.Min(span1.End, span.Span.End);
          list.Add(new ClassificationSpan(new SnapshotSpan(span.Snapshot, start, num - start), classificationSpan.ClassificationType));
        }
      }
      return (IList<ClassificationSpan>) list;
    }

    private void RaiseClassificationChangedEvent(ClassificationChangedEventArgs args)
    {
      EventHandler<ClassificationChangedEventArgs> eventHandler = this.ClassificationChanged;
      if (eventHandler == null)
        return;
      eventHandler((object) this, args);
    }
  }
}
