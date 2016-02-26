// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.Documents.EditableTextBuffer
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using Microsoft.Expression.DesignModel.Code;
using Microsoft.Expression.DesignModel.Text;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.UI.Undo;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Microsoft.Expression.Code.Documents
{
  internal sealed class EditableTextBuffer : Microsoft.Expression.DesignModel.Text.ITextBuffer, IReadableSelectableTextBuffer, IReadableTextBuffer, ITextBufferUndo, IDisposable
  {
    private ITextBufferUndoManager undoManager;

    public Microsoft.VisualStudio.Text.ITextBuffer TextBuffer
    {
      get
      {
        return this.undoManager.TextBuffer;
      }
    }

    private UndoHistory UndoHistory
    {
      get
      {
        return this.undoManager.TextBufferUndoHistory;
      }
    }

    public int Length
    {
      get
      {
        if (this.undoManager != null)
          return this.TextBuffer.CurrentSnapshot.Length;
        return 0;
      }
    }

    public ITextRange Range
    {
      get
      {
        return (ITextRange) new EditableTextBuffer.TextRange(this, 0, this.Length);
      }
    }

    public event EventHandler<TextChangedEventArgs> TextChanged;

    public event EventHandler<TextUndoCompletedEventArgs> UndoUnitAdded;

    private event EventHandler<TextContentChangedEventArgs> NautilusBufferChanged;

    public EditableTextBuffer(ITextBufferUndoManager undoManager)
    {
      this.undoManager = undoManager;
      this.TextBuffer.Changed += new EventHandler<TextContentChangedEventArgs>(this.TextBuffer_Changed);
      this.UndoHistory.UndoTransactionCompleted += new EventHandler<UndoTransactionCompletedEventArgs>(this.UndoHistory_UndoTransactionCompleted);
    }

    public void Dispose()
    {
      if (this.undoManager == null)
        return;
      this.TextBuffer.Changed -= new EventHandler<TextContentChangedEventArgs>(this.TextBuffer_Changed);
      this.UndoHistory.UndoTransactionCompleted -= new EventHandler<UndoTransactionCompletedEventArgs>(this.UndoHistory_UndoTransactionCompleted);
      this.undoManager.UnregisterUndoHistory();
      this.undoManager = (ITextBufferUndoManager) null;
    }

    public string GetText(int offset, int length)
    {
      return this.TextBuffer.CurrentSnapshot.GetText(offset, length);
    }

    public IReadableSelectableTextBuffer Freeze()
    {
      return (IReadableSelectableTextBuffer) new EditableTextBuffer.FrozenTextBuffer(this.TextBuffer.CurrentSnapshot);
    }

    public string GetText()
    {
      return this.GetText(0, this.Length);
    }

    public ITextLocation GetLocation(int offset)
    {
      return (ITextLocation) new EditableTextBuffer.TextLocation(this.TextBuffer.CurrentSnapshot, offset);
    }

    public void SetText(int offset, int length, string text)
    {
      this.TextBuffer.Replace(new Span(offset, length), text);
    }

    public void SetText(string text)
    {
      this.SetText(0, this.Length, text);
    }

    public void Undo()
    {
      this.UndoHistory.Undo(1);
    }

    public void Redo()
    {
      this.UndoHistory.Redo(1);
    }

    public ITextChangesTracker RecordChanges(ShouldTrackChange shouldTrackChange)
    {
      return (ITextChangesTracker) new EditableTextBuffer.TextChanges(this, this.TextBuffer, shouldTrackChange);
    }

    public ITextRange FreezeRange(ITextRange textRange)
    {
      EditableTextBuffer.TextRange textRange1 = textRange as EditableTextBuffer.TextRange;
      if (textRange1 != null)
        return (ITextRange) new EditableTextBuffer.FrozenTextRange(this.TextBuffer.CurrentSnapshot, textRange1.Span);
      int offset = textRange.Offset;
      return (ITextRange) new Microsoft.Expression.DesignModel.Code.TextRange(offset, offset + textRange.Length);
    }

    public ITextRange CreateRange(int offset, int length)
    {
      return (ITextRange) new EditableTextBuffer.TextRange(this, offset, length);
    }

    public TextReader GetTextReader()
    {
      return (TextReader) new EditableTextBuffer.TextBufferReader(this.TextBuffer);
    }

    public TextWriter GetTextWriter()
    {
      return (TextWriter) new EditableTextBuffer.TextBufferWriter(this.TextBuffer);
    }

    private void TextBuffer_Changed(object sender, TextContentChangedEventArgs e)
    {
      if (this.NautilusBufferChanged != null)
        this.NautilusBufferChanged(sender, e);
      if (this.TextChanged == null)
        return;
      this.TextChanged((object) this, new TextChangedEventArgs((ITextRange) new EditableTextBuffer.TextRange(this, 0, 0)));
    }

    private void UndoHistory_UndoTransactionCompleted(object sender, UndoTransactionCompletedEventArgs e)
    {
      this.OnUndoUnitAdded(new TextUndoCompletedEventArgs(e.Result == UndoTransactionCompletionResult.TransactionMerged ? TextUndoCompletionResult.Merged : TextUndoCompletionResult.Added, (ITextUndoTransaction) new EditableTextBuffer.TextUndoTransaction(e.Transaction)));
    }

    private void OnUndoUnitAdded(TextUndoCompletedEventArgs e)
    {
      if (this.UndoUnitAdded == null)
        return;
      this.UndoUnitAdded((object) this, e);
    }

    private sealed class TextUndoTransaction : ITextUndoTransaction
    {
      private UndoTransaction undoTransaction;

      public bool IsHidden
      {
        get
        {
          return this.undoTransaction.IsHidden;
        }
      }

      public TextUndoTransaction(UndoTransaction undoTransaction)
      {
        this.undoTransaction = undoTransaction;
      }

      public void Undo()
      {
        this.undoTransaction.History.Undo(1);
      }

      public void Redo()
      {
        this.undoTransaction.History.Redo(1);
      }

      public void DisallowMerge()
      {
        this.undoTransaction.MergePolicy = (IMergeUndoTransactionPolicy) new EditableTextBuffer.TextUndoTransaction.UnmergablePolicy();
      }

      private class UnmergablePolicy : IMergeUndoTransactionPolicy
      {
        public bool CanMerge(UndoTransaction newerTransaction, UndoTransaction olderTransaction)
        {
          return false;
        }

        public void CompleteTransactionMerge(UndoTransaction newerTransaction, UndoTransaction olderTransaction, UndoTransaction mergedTransaction)
        {
        }

        public bool TestCompatiblePolicy(IMergeUndoTransactionPolicy other)
        {
          return false;
        }
      }
    }

    private sealed class TextChanges : ITextChangesTracker, IDisposable
    {
      private EditableTextBuffer hostBuffer;
      private Microsoft.VisualStudio.Text.ITextBuffer textBuffer;
      private ITextSnapshot originalSnapshot;
      private List<ITrackingSpan> changes;
      private bool changedAll;
      private bool containsRangeDeletions;
      private ShouldTrackChange shouldTrackChange;

      public bool HasChanged
      {
        get
        {
          if (!this.changedAll)
            return this.changes.Count > 0;
          return true;
        }
      }

      public bool ContainsRangeDeletions
      {
        get
        {
          return this.containsRangeDeletions;
        }
      }

      public TextChanges(EditableTextBuffer hostBuffer, Microsoft.VisualStudio.Text.ITextBuffer textBuffer, ShouldTrackChange shouldTrackChange)
      {
        this.hostBuffer = hostBuffer;
        this.hostBuffer.NautilusBufferChanged += new EventHandler<TextContentChangedEventArgs>(this.TextBuffer_Changed);
        this.textBuffer = textBuffer;
        this.originalSnapshot = this.textBuffer.CurrentSnapshot;
        this.changes = new List<ITrackingSpan>();
        this.shouldTrackChange = shouldTrackChange;
      }

      public ITextRange GetChanges()
      {
        return this.GetChanges(this.textBuffer.CurrentSnapshot);
      }

      private ITextRange GetChanges(ITextSnapshot snapshot)
      {
        if (this.changedAll)
          return (ITextRange) new Microsoft.Expression.DesignModel.Code.TextRange(0, snapshot.Length);
        ITextRange range = Microsoft.Expression.DesignModel.Code.TextRange.Null;
        foreach (ITrackingSpan trackingSpan in this.changes)
        {
          ITextRange other = (ITextRange) new Microsoft.Expression.DesignModel.Code.TextRange((int) trackingSpan.GetStartPoint(snapshot), (int) trackingSpan.GetEndPoint(snapshot));
          range = Microsoft.Expression.DesignModel.Code.TextRange.Union(range, other);
        }
        return range;
      }

      public int GetOffsetInOriginalReference(int line, int column)
      {
        int lineNumber = line - 1;
        if (lineNumber < 0)
          return 0;
        if (lineNumber >= this.originalSnapshot.LineCount)
          return Math.Max(0, this.textBuffer.CurrentSnapshot.Length - 1);
        int position = this.originalSnapshot.GetLineFromLineNumber(lineNumber).Start + column;
        if (position >= this.originalSnapshot.Length)
          return Math.Max(0, this.textBuffer.CurrentSnapshot.Length - 1);
        return this.originalSnapshot.CreateTrackingPoint(position, PointTrackingMode.Positive).GetPosition(this.textBuffer.CurrentSnapshot);
      }

      public void Dispose()
      {
        this.hostBuffer.NautilusBufferChanged -= new EventHandler<TextContentChangedEventArgs>(this.TextBuffer_Changed);
        this.hostBuffer = (EditableTextBuffer) null;
        this.shouldTrackChange = (ShouldTrackChange) null;
        this.originalSnapshot = (ITextSnapshot) null;
        this.textBuffer = (Microsoft.VisualStudio.Text.ITextBuffer) null;
        this.changes = (List<ITrackingSpan>) null;
      }

      private void TextBuffer_Changed(object sender, TextContentChangedEventArgs e)
      {
        if (this.shouldTrackChange != null && !this.shouldTrackChange())
          return;
        if (e.Changes.Count > 1)
        {
          this.changedAll = true;
        }
        else
        {
          if (this.changedAll)
            return;
          ITextSnapshot after = e.After;
          foreach (ITextChange textChange in (IEnumerable<ITextChange>) e.Changes)
          {
            this.changes.Add(after.CreateTrackingSpan(textChange.NewPosition, textChange.NewLength, SpanTrackingMode.EdgeExclusive));
            if (textChange.NewLength < textChange.OldLength)
              this.containsRangeDeletions = true;
          }
        }
      }
    }

    private sealed class TextBufferReader : TextReader
    {
      private readonly ITextSnapshot snapshot;
      private int offset;

      public TextBufferReader(Microsoft.VisualStudio.Text.ITextBuffer textBuffer)
      {
        this.snapshot = textBuffer.CurrentSnapshot;
      }

      public override int Read(char[] buffer, int index, int count)
      {
        this.offset += index;
        if (this.offset + count > this.snapshot.Length)
          count = this.snapshot.Length - this.offset;
        if (count <= 0)
          return 0;
        this.snapshot.CopyTo(this.offset, buffer, 0, count);
        this.offset += count;
        return count;
      }

      public override int ReadBlock(char[] buffer, int index, int count)
      {
        return this.Read(buffer, index, count);
      }
    }

    private sealed class TextBufferWriter : TextWriter
    {
      private Microsoft.VisualStudio.Text.ITextBuffer textBuffer;

      public override Encoding Encoding
      {
        get
        {
          return Encoding.Unicode;
        }
      }

      public TextBufferWriter(Microsoft.VisualStudio.Text.ITextBuffer textBuffer)
        : base((IFormatProvider) CultureInfo.CurrentUICulture)
      {
        this.textBuffer = textBuffer;
      }

      public override void Write(char[] buffer, int index, int count)
      {
        ITextEdit edit = this.textBuffer.CreateEdit();
        edit.Insert(this.textBuffer.CurrentSnapshot.Length, buffer, index, count);
        edit.Apply();
      }
    }

    private sealed class TextRange : ITextRange
    {
      private EditableTextBuffer textBuffer;
      private ITrackingSpan span;

      public ITrackingSpan Span
      {
        get
        {
          return this.span;
        }
      }

      public int Offset
      {
        get
        {
          return this.span.GetStartPoint(this.textBuffer.TextBuffer.CurrentSnapshot).Position;
        }
      }

      public int Length
      {
        get
        {
          return this.span.GetSpan(this.textBuffer.TextBuffer.CurrentSnapshot).Length;
        }
      }

      public TextRange(EditableTextBuffer textBuffer, int offset, int length)
      {
        this.textBuffer = textBuffer;
        this.span = this.textBuffer.TextBuffer.CurrentSnapshot.CreateTrackingSpan(offset, length, SpanTrackingMode.EdgeExclusive, TrackingFidelityModes.UndoRedo);
      }

      public override string ToString()
      {
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[{0}, {1}]", new object[2]
        {
          (object) this.Offset,
          (object) this.Length
        });
      }
    }

    internal sealed class TextLocation : ITextLocation
    {
      private ITextSnapshot snapshot;
      private int line;
      private int column;

      public int Line
      {
        get
        {
          this.EnsureInitialized();
          return this.line;
        }
      }

      public int Column
      {
        get
        {
          this.EnsureInitialized();
          return this.column;
        }
      }

      public TextLocation(ITextSnapshot snapshot, int offset)
      {
        this.snapshot = snapshot;
        this.line = offset;
      }

      public override bool Equals(object obj)
      {
        if (this == obj)
          return true;
        ITextLocation textLocation = obj as ITextLocation;
        if (textLocation != null && this.Line == textLocation.Line)
          return this.Column == textLocation.Column;
        return false;
      }

      public override int GetHashCode()
      {
        return this.Line.GetHashCode() ^ this.Column.GetHashCode();
      }

      private void EnsureInitialized()
      {
        if (this.snapshot == null)
          return;
        int position = this.line;
        ITextSnapshotLine lineFromPosition = this.snapshot.GetLineFromPosition(position);
        this.snapshot = (ITextSnapshot) null;
        this.line = lineFromPosition.LineNumber + 1;
        this.column = position - lineFromPosition.Start;
      }

      public override string ToString()
      {
        return "[" + (object) this.Line + ", " + (string) (object) this.Column + "]";
      }
    }

    private sealed class FrozenTextBuffer : IReadableSelectableTextBuffer, IReadableTextBuffer
    {
      private ITextSnapshot snapshot;

      public int Length
      {
        get
        {
          return this.snapshot.Length;
        }
      }

      public FrozenTextBuffer(ITextSnapshot snapshot)
      {
        this.snapshot = snapshot;
      }

      public ITextRange FreezeRange(ITextRange textRange)
      {
        EditableTextBuffer.TextRange textRange1 = textRange as EditableTextBuffer.TextRange;
        if (textRange1 != null)
          return (ITextRange) new EditableTextBuffer.FrozenTextRange(this.snapshot, textRange1.Span);
        return (ITextRange) (textRange as EditableTextBuffer.FrozenTextRange) ?? this.CreateRange(textRange.Offset, textRange.Length);
      }

      public ITextRange CreateRange(int offset, int length)
      {
        return (ITextRange) new EditableTextBuffer.FrozenTextRange(this.snapshot, this.snapshot.CreateTrackingSpan(offset, offset + length, SpanTrackingMode.EdgeExclusive, TrackingFidelityModes.UndoRedo));
      }

      public ITextLocation GetLocation(int offset)
      {
        throw new NotImplementedException();
      }

      public IReadableSelectableTextBuffer Freeze()
      {
        return (IReadableSelectableTextBuffer) this;
      }

      public string GetText(int offset, int length)
      {
        return this.snapshot.GetText(offset, length);
      }
    }

    private sealed class FrozenTextRange : ITextRange, IMappingTextRange
    {
      private ITextRange cachedSpan = Microsoft.Expression.DesignModel.Code.TextRange.Null;
      private ITextSnapshot snapshot;
      private ITrackingSpan span;

      public int Offset
      {
        get
        {
          return this.CacheSpan().Offset;
        }
      }

      public int Length
      {
        get
        {
          return this.CacheSpan().Length;
        }
      }

      public FrozenTextRange(ITextSnapshot snapshot, ITrackingSpan span)
      {
        this.snapshot = snapshot;
        this.span = span;
      }

      public ITextRange GetRangeForBuffer(Microsoft.Expression.DesignModel.Text.ITextBuffer textBuffer)
      {
        EditableTextBuffer editableTextBuffer = textBuffer as EditableTextBuffer;
        if (editableTextBuffer == null || editableTextBuffer.TextBuffer != this.span.TextBuffer)
          return Microsoft.Expression.DesignModel.Code.TextRange.Null;
        this.CacheSpan();
        Span span = this.span.GetSpan(editableTextBuffer.TextBuffer.CurrentSnapshot).Span;
        return (ITextRange) new Microsoft.Expression.DesignModel.Code.TextRange(span.Start, span.End);
      }

      private ITextRange CacheSpan()
      {
        if (Microsoft.Expression.DesignModel.Code.TextRange.IsNull(this.cachedSpan))
        {
          Span span = this.span.GetSpan(this.snapshot).Span;
          this.cachedSpan = (ITextRange) new Microsoft.Expression.DesignModel.Code.TextRange(span.Start, span.End);
        }
        return this.cachedSpan;
      }
    }
  }
}
