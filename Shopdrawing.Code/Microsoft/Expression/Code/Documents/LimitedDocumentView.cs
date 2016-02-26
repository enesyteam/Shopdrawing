// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.Documents.LimitedDocumentView
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using Microsoft.Expression.Code;
using Microsoft.Expression.DesignModel.Code;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.UserInterface;
using System;
using System.Collections;
using System.Windows;
using System.Windows.Threading;

namespace Microsoft.Expression.Code.Documents
{
  internal sealed class LimitedDocumentView : DocumentView, IElementProvider
  {
    private CodeEditor codeEditor;

    public FrameworkElement Element
    {
      get
      {
        return this.codeEditor.Element;
      }
    }

    public override object ActiveEditor
    {
      get
      {
        return (object) this.codeEditor;
      }
    }

    private ITextEditor Editor
    {
      get
      {
        return (ITextEditor) this.codeEditor;
      }
    }

    internal LimitedDocumentView(IDocument document, EditingService editingService)
      : base(document)
    {
      LimitedDocument limitedDocument = (LimitedDocument) document;
      this.codeEditor = editingService.CreateCodeEditor(limitedDocument.TextBuffer);
      foreach (DictionaryEntry dictionaryEntry in this.codeEditor.GetEditCommands())
        this.AddCommand((string) dictionaryEntry.Key, (ICommand) dictionaryEntry.Value);
      foreach (DictionaryEntry dictionaryEntry in this.codeEditor.GetUndoCommands())
        this.AddCommand((string) dictionaryEntry.Key, (ICommand) dictionaryEntry.Value);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.codeEditor != null)
      {
        this.codeEditor.Dispose();
        this.codeEditor = (CodeEditor) null;
      }
      base.Dispose(disposing);
    }

    public override void ReturnFocus()
    {
      UIThreadDispatcher.Instance.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(this.SetFocus));
    }

    private void SetFocus()
    {
      if (this.Editor == null)
        return;
      this.Editor.Focus();
    }
  }
}
