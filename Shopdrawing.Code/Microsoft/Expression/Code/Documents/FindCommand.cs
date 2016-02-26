// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.Documents.FindCommand
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using Microsoft.Expression.Code.UserInterface;
using Microsoft.Expression.DesignModel.Code;
using Microsoft.Expression.Framework;

namespace Microsoft.Expression.Code.Documents
{
  internal class FindCommand : FindReplaceCommand
  {
    public FindCommand(ITextEditor textEditor, IMessageDisplayService messageManager)
      : base(textEditor, messageManager)
    {
    }

    public override void Execute()
    {
      ReplaceDialog.CloseIfOpen();
      FindInFilesDialog.CloseIfOpen();
      FindDialog findDialog = FindDialog.GetFindDialog(this.FindReplaceModel, this.TextEditor, this.MessageDisplayService);
      if (!this.TextEditor.IsSelectionEmpty)
      {
        string text = this.TextEditor.GetText(this.TextEditor.SelectionStart, this.TextEditor.SelectionLength);
        if (!string.IsNullOrEmpty(text) && text.IndexOf('\n') == -1)
          findDialog.FindText = text;
      }
      findDialog.Show();
    }
  }
}
