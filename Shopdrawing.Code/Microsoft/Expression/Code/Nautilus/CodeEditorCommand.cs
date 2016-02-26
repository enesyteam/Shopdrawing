// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.Nautilus.CodeEditorCommand
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using Microsoft.Expression.DesignModel.Code;
using Microsoft.Expression.Framework.Commands;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.UI.Undo;

namespace Microsoft.Expression.Code.Nautilus
{
  internal abstract class CodeEditorCommand : Command
  {
    private Microsoft.Expression.Code.Documents.CodeEditor codeEditor;

    protected ITextView TextView
    {
      get
      {
        return this.codeEditor.TextView;
      }
    }

    protected IEditorOperations EditorCommands
    {
      get
      {
        return this.codeEditor.EditorCommands;
      }
    }

    protected ITextEditor CodeEditor
    {
      get
      {
        return (ITextEditor) this.codeEditor;
      }
    }

    protected UndoHistory UndoHistory
    {
      get
      {
        return this.codeEditor.UndoHistory;
      }
    }

    protected CodeEditorCommand(Microsoft.Expression.Code.Documents.CodeEditor codeEditor)
    {
      this.codeEditor = codeEditor;
    }

    public override void Execute()
    {
      this.codeEditor.DismissCompletionSession();
    }
  }
}
