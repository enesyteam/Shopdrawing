// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.Documents.LimitedDocument
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using Microsoft.Expression.Code;
using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.Expression.Framework.Documents;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.UI.Undo;
using System;
using System.IO;
using System.Text;

namespace Microsoft.Expression.Code.Documents
{
  internal sealed class LimitedDocument : UndoDocument, ICommandTarget
  {
    private Encoding encoding;
    private string contents;
    private ITextBuffer textBuffer;
    private ITextBufferUndoManager undoManager;
    private UndoTransactionMarker topMarker;
    private EditingService editingService;

    public ITextBuffer TextBuffer
    {
      get
      {
        if (this.textBuffer == null)
        {
          this.undoManager = this.editingService.CreateTextBuffer(this.contents, "text.xml", out this.topMarker);
          this.textBuffer = this.undoManager.TextBuffer;
          this.undoManager.TextBufferUndoHistory.UndoTransactionCompleted += new EventHandler<UndoTransactionCompletedEventArgs>(this.TextBufferUndoHistory_UndoTransactionCompleted);
          this.undoManager.TextBufferUndoHistory.UndoRedoHappened += new EventHandler<UndoRedoEventArgs>(this.TextBufferUndoHistory_UndoRedoHappened);
        }
        return this.textBuffer;
      }
    }

    public override bool IsDirty
    {
      get
      {
        if (!base.IsDirty)
        {
          object obj;
          return !this.undoManager.TextBufferUndoHistory.TryFindMarkerOnTop(this.topMarker, out obj);
        }
        return true;
      }
    }

    internal LimitedDocument(DocumentReference documentReference, bool isReadOnly, Encoding encoding, string contents, EditingService editingService)
      : base(documentReference, isReadOnly)
    {
      this.encoding = encoding;
      this.contents = contents;
      this.editingService = editingService;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (this.textBuffer != null)
          this.textBuffer = (ITextBuffer) null;
        if (this.undoManager != null)
        {
          this.undoManager.TextBufferUndoHistory.UndoRedoHappened -= new EventHandler<UndoRedoEventArgs>(this.TextBufferUndoHistory_UndoRedoHappened);
          this.undoManager.TextBufferUndoHistory.UndoTransactionCompleted -= new EventHandler<UndoTransactionCompletedEventArgs>(this.TextBufferUndoHistory_UndoTransactionCompleted);
          this.undoManager = (ITextBufferUndoManager) null;
        }
        if (this.topMarker != null)
          this.topMarker = (UndoTransactionMarker) null;
      }
      base.Dispose(disposing);
    }

    public override IDocumentView CreateDefaultView()
    {
      PerformanceUtility.StartPerformanceSequence(PerformanceEvent.CreateCodeEditor);
      IDocumentView documentView = (IDocumentView) new LimitedDocumentView((IDocument) this, this.editingService);
      PerformanceUtility.EndPerformanceSequence(PerformanceEvent.CreateCodeEditor);
      return documentView;
    }

    protected override void SaveCore(Stream stream)
    {
      if (this.textBuffer != null)
      {
        using (StreamWriter streamWriter = new StreamWriter(stream, this.encoding))
          streamWriter.Write(this.TextBuffer.CurrentSnapshot.GetText(0, this.TextBuffer.CurrentSnapshot.Length));
        using (UndoTransaction transaction = this.undoManager.TextBufferUndoHistory.CreateTransaction("EmptyTransaction", false))
          transaction.Complete();
        this.undoManager.TextBufferUndoHistory.ReplaceMarkerOnTop(this.topMarker, (object) null);
        this.OnIsDirtyChanged(EventArgs.Empty);
      }
      else
      {
        using (StreamWriter streamWriter = new StreamWriter(stream, this.encoding))
          streamWriter.Write(this.contents);
      }
    }

    private void TextBufferUndoHistory_UndoRedoHappened(object sender, UndoRedoEventArgs e)
    {
      this.OnIsDirtyChanged(EventArgs.Empty);
    }

    private void TextBufferUndoHistory_UndoTransactionCompleted(object sender, UndoTransactionCompletedEventArgs e)
    {
      this.OnIsDirtyChanged(EventArgs.Empty);
    }
  }
}
