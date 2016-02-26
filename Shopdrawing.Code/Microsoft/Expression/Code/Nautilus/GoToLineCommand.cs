// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.Nautilus.GoToLineCommand
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using Microsoft.Expression.Code.Documents;
using Microsoft.Expression.Code.UserInterface;
using System;
using System.Globalization;

namespace Microsoft.Expression.Code.Nautilus
{
  internal class GoToLineCommand : CodeEditorCommand
  {
    public GoToLineCommand(CodeEditor codeEditor)
      : base(codeEditor)
    {
    }

    public override void Execute()
    {
      base.Execute();
      int result = this.TextView.TextSnapshot.GetLineNumberFromPosition(this.TextView.Caret.Position.Index) + 1;
      GoToLineDialog goToLineDialog = new GoToLineDialog();
      goToLineDialog.LineNumber = result.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      bool? nullable = goToLineDialog.ShowDialog();
      if (!nullable.HasValue || !nullable.Value || (!int.TryParse(goToLineDialog.LineNumber, out result) || result <= 0))
        return;
      --result;
      if (result >= this.TextView.TextSnapshot.LineCount)
        result = this.TextView.TextSnapshot.LineCount - 1;
      this.CodeEditor.GotoLine(result);
      this.CodeEditor.ClearSelection();
      this.CodeEditor.Focus();
    }
  }
}
