// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.Classifiers.LineBasedStateManagement`1
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using Microsoft.Expression.Code;
using Microsoft.VisualStudio.ApplicationModel.Environments;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Microsoft.Expression.Code.Classifiers
{
  internal class LineBasedStateManagement<T> : IStateManagement<T> where T : struct, IComparable<T>
  {
    private T?[] _states;
    private ITextBuffer _textBuffer;
    private IClassificationScanner<T> _scanner;
    private int _previousLineCount;
    private IEnvironment environment;

    public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;

    public LineBasedStateManagement(ITextBuffer textBuffer, IClassificationScanner<T> scanner, IEnvironment environment)
    {
      if (textBuffer == null)
        throw new ArgumentNullException("textBuffer");
      if (scanner == null)
        throw new ArgumentNullException("scanner");
      this._scanner = scanner;
      this._textBuffer = textBuffer;
      this.environment = environment;
      this._previousLineCount = this._textBuffer.CurrentSnapshot.LineCount;
      this._states = new T?[this._previousLineCount * 3 / 2];
      this._textBuffer.Changed += new EventHandler<TextContentChangedEventArgs>(this.HandleBufferChanged);
    }

    public SnapshotSpan GetSpanToTokenize(SnapshotSpan requestedSpan)
    {
      ITextSnapshot snapshot = requestedSpan.Snapshot;
      int start = snapshot.GetLineFromPosition(requestedSpan.Span.Start).Start;
      int includingLineBreak = snapshot.GetLineFromPosition(this.GetSpanEnd(requestedSpan.Span)).EndIncludingLineBreak;
      return new SnapshotSpan(snapshot, new Span(start, includingLineBreak - start));
    }

    public T GetState(ITextSnapshot snapshot, int startPosition)
    {
      int numberFromPosition = snapshot.GetLineNumberFromPosition(startPosition);
      if (numberFromPosition == 0)
        return default (T);
      int val2 = numberFromPosition - 1;
      if (this._states[val2].HasValue)
        return this._states[val2].Value;
      int endLine = val2;
      do
        ;
      while (val2-- > 0 && !this._states[val2].HasValue);
      T state = val2 < 0 ? default (T) : this._states[val2].Value;
      this.CacheLineStates(snapshot, Math.Max(0, val2), endLine, state);
      return this._states[endLine].Value;
    }

    private T CacheLineStates(ITextSnapshot snapshot, int startLine, int endLine, T state)
    {
      for (int lineNumber = startLine; lineNumber <= endLine; ++lineNumber)
      {
        T currentState = state;
        ITextSnapshotLine lineFromLineNumber = snapshot.GetLineFromLineNumber(lineNumber);
        int start = lineFromLineNumber.Start;
        int endOfLastToken = lineFromLineNumber.LengthIncludingLineBreak;
        state = this.GetStateAtEndOfSpan(snapshot, new Span(start, endOfLastToken), currentState, out endOfLastToken);
        this._states[lineNumber] = new T?(state);
      }
      return state;
    }

    private T GetStateAtEndOfSpan(ITextSnapshot snapshot, Span span, T currentState, out int endOfLastToken)
    {
      T endState;
      IList<ClassificationSpan> classificationSpans = this._scanner.GetClassificationSpans(new SnapshotSpan(snapshot, span), currentState, out endState);
      endOfLastToken = span.End;
      if (classificationSpans.Count > 0)
        endOfLastToken = Math.Max(endOfLastToken, classificationSpans[classificationSpans.Count - 1].Span.End);
      return endState;
    }

    public void SetState(ITextSnapshot snapshot, int position, T state)
    {
      this._states[snapshot.GetLineNumberFromPosition(Math.Max(0, position - 1))] = new T?(state);
    }

    private void HandleBufferChanged(object sender, TextContentChangedEventArgs e)
    {
      this.UpdateStateArray(e);
      Span span1 = new Span();
      foreach (ITextChange textChange in (IEnumerable<ITextChange>) e.Changes)
      {
        Span span2 = this.DetermineInvalidatedSpan(e.After, new Span(textChange.NewPosition, textChange.NewLength));
        if (!span1.Contains(span2))
        {
          this.RaiseClassificationChangedEvent(new SnapshotSpan(e.After, span2));
          if (!span1.IsEmpty)
          {
            int start = Math.Min(span2.Start, span1.Start);
            int num = Math.Max(span2.End, span1.End);
            span1 = new Span(start, num - start);
          }
          else
            span1 = span2;
        }
      }
      this._previousLineCount = e.After.LineCount;
    }

    private void UpdateStateArray(TextContentChangedEventArgs e)
    {
      this.GrowArray(e.After.LineCount - e.Before.LineCount);
      if (e.Changes.Count == 1)
      {
        int numberFromPosition = e.Before.GetLineNumberFromPosition(e.Changes[0].OldPosition);
        this.ResizeStateArray(e.After.LineCount - e.Before.LineCount, numberFromPosition - 1);
      }
      else
        Array.Clear((Array) this._states, 0, this._states.Length);
    }

    private void ResizeStateArray(int diffLines, int unchangedLine)
    {
      if (diffLines > 0)
      {
        Array.ConstrainedCopy((Array) this._states, unchangedLine + 1, (Array) this._states, unchangedLine + 1 + diffLines, this._previousLineCount - unchangedLine - 1);
        for (int index = 0; index < diffLines; ++index)
          this._states[unchangedLine + 1 + index] = new T?();
      }
      else
      {
        if (diffLines >= 0)
          return;
        int num = Math.Abs(diffLines);
        Array.ConstrainedCopy((Array) this._states, unchangedLine + num + 1, (Array) this._states, unchangedLine + 1, this._previousLineCount - unchangedLine + diffLines - 1);
        for (int index = 0; index < num; ++index)
          this._states[this._previousLineCount + diffLines + index] = new T?();
      }
    }

    private void GrowArray(int diffLines)
    {
      if (this._previousLineCount + diffLines <= this._states.Length)
        return;
      Array.Resize<T?>(ref this._states, (this._previousLineCount + diffLines) * 3 / 2);
    }

    private Span DetermineInvalidatedSpan(ITextSnapshot snapshot, Span span)
    {
      ITextSnapshotLine lineFromPosition1 = snapshot.GetLineFromPosition(span.Start);
      if (this.environment != null)
      {
        IWpfTextViewHost wpfTextViewHost = (IWpfTextViewHost) this.environment.Get(EditingService.TextViewHostEnvironmentVariable);
        if (wpfTextViewHost != null && (wpfTextViewHost.HostControl.Visibility == Visibility.Collapsed || wpfTextViewHost.TextView.RenderedTextLines.Count > 0 && wpfTextViewHost.TextView.RenderedTextLines[wpfTextViewHost.TextView.RenderedTextLines.Count - 1].LineSpan.End < lineFromPosition1.Start))
        {
          for (int index = Math.Max(0, lineFromPosition1.LineNumber - 1); index < this._states.Length; ++index)
            this._states[index] = new T?();
          return new Span(lineFromPosition1.Start, snapshot.Length - lineFromPosition1.Start);
        }
      }
      ITextSnapshotLine lineFromPosition2 = snapshot.GetLineFromPosition(this.GetSpanEnd(span));
      T state = this.GetState(snapshot, lineFromPosition1.Start);
      T? nullable = lineFromPosition2.LineNumber < snapshot.LineCount ? this._states[lineFromPosition2.LineNumber] : new T?();
      T currentState = this.CacheLineStates(snapshot, lineFromPosition1.LineNumber, lineFromPosition2.LineNumber, state);
      int lineNumber = lineFromPosition2.LineNumber;
      int endOfLastToken = lineFromPosition2.EndIncludingLineBreak;
      ITextSnapshotLine lineFromLineNumber;
      for (; ++lineNumber < snapshot.LineCount && (!nullable.HasValue || currentState.CompareTo(nullable.Value) != 0); currentState = this.GetStateAtEndOfSpan(snapshot, (Span) new SnapshotSpan(snapshot, lineFromLineNumber.Start, lineFromLineNumber.LengthIncludingLineBreak), currentState, out endOfLastToken))
      {
        this._states[lineNumber - 1] = new T?(currentState);
        nullable = this._states[lineNumber];
        lineFromLineNumber = snapshot.GetLineFromLineNumber(lineNumber);
      }
      return new Span(lineFromPosition1.Start, endOfLastToken - lineFromPosition1.Start);
    }

    private int GetSpanEnd(Span span)
    {
      if (span.Length == 0)
        return span.End;
      return span.End - 1;
    }

    private void RaiseClassificationChangedEvent(SnapshotSpan span)
    {
      EventHandler<ClassificationChangedEventArgs> eventHandler = this.ClassificationChanged;
      if (eventHandler == null)
        return;
      eventHandler((object) this, new ClassificationChangedEventArgs(span));
    }
  }
}
