// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.Documents.FindNextCommand
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using Microsoft.Expression.Code.UserInterface;
using Microsoft.Expression.DesignModel.Code;
using Microsoft.Expression.Framework;

namespace Microsoft.Expression.Code.Documents
{
  internal class FindNextCommand : FindReplaceCommand
  {
    public FindNextCommand(ITextEditor textEditor, IMessageDisplayService messageManager)
      : base(textEditor, messageManager)
    {
    }

    public override void Execute()
    {
      ReplaceDialog.CloseIfOpen();
      FindInFilesDialog.CloseIfOpen();
      FindDialog findDialog = FindDialog.GetFindDialog(this.FindReplaceModel, this.TextEditor, this.MessageDisplayService);
      if (string.IsNullOrEmpty(findDialog.FindText))
      {
        findDialog.SearchReverse = false;
        findDialog.Show();
      }
      else
      {
        if (!findDialog.FindNext())
          return;
        this.TextEditor.Focus();
      }
    }
  }
}
