// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.Actipro.GoToLineCommand
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using Microsoft.Expression.Code.UserInterface;
using System;
using System.Globalization;

namespace Microsoft.Expression.Code.Actipro
{
  internal class GoToLineCommand : ActiproCommand
  {
    public GoToLineCommand(ActiproEditor editor)
      : base(editor)
    {
    }

    public override void Execute()
    {
      int result = this.Editor.Caret.EditPosition.Line + 1;
      GoToLineDialog goToLineDialog = new GoToLineDialog();
      goToLineDialog.LineNumber = result.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      bool? nullable = goToLineDialog.ShowDialog();
      if (nullable.HasValue && nullable.Value && (int.TryParse(goToLineDialog.LineNumber, out result) && result > 0))
      {
        int documentLineIndex = result - 1;
        if (documentLineIndex >= this.Editor.Document.Lines.Count)
          documentLineIndex = this.Editor.Document.Lines.Count - 1;
        this.View.GoToLine(documentLineIndex);
        this.View.Selection.Collapse();
      }
      base.Execute();
    }
  }
}
