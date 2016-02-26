// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.Nautilus.CodeEditorStandardCommands
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using Microsoft.Expression.Code.Documents;
using Microsoft.Expression.DesignModel.Code;
using Microsoft.Expression.Framework;
using System.Collections;

namespace Microsoft.Expression.Code.Nautilus
{
  internal static class CodeEditorStandardCommands
  {
    public static IDictionary GetEditCommands(CodeEditor codeEditor, IMessageDisplayService messageDisplayService)
    {
      Hashtable hashtable = new Hashtable();
      hashtable.Add((object) "Edit_Paste", (object) new PasteCommand(codeEditor));
      hashtable.Add((object) "Edit_Copy", (object) new CopyCommand(codeEditor));
      hashtable.Add((object) "Edit_Cut", (object) new CutCommand(codeEditor));
      hashtable.Add((object) "Edit_Delete", (object) new DeleteCommand(codeEditor));
      hashtable.Add((object) "Edit_SelectAll", (object) new SelectAllCommand(codeEditor));
      hashtable.Add((object) "Edit_SelectNone", (object) new SelectNoneCommand(codeEditor));
      hashtable.Add((object) "Edit_GoToLine", (object) new GoToLineCommand(codeEditor));
      if (messageDisplayService != null)
      {
        hashtable.Add((object) "Edit_Find", (object) new FindCommand((ITextEditor) codeEditor, messageDisplayService));
        hashtable.Add((object) "Edit_FindNext", (object) new FindNextCommand((ITextEditor) codeEditor, messageDisplayService));
        hashtable.Add((object) "Edit_Replace", (object) new ReplaceCommand((ITextEditor) codeEditor, messageDisplayService));
      }
      return (IDictionary) hashtable;
    }

    public static IDictionary GetUndoCommands(CodeEditor codeEditor)
    {
      return (IDictionary) new Hashtable()
      {
        {
          (object) "Edit_Undo",
          (object) new UndoCommand(codeEditor)
        },
        {
          (object) "Edit_Redo",
          (object) new RedoCommand(codeEditor)
        }
      };
    }
  }
}
