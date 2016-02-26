// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.Nautilus.RedoCommand
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using Microsoft.Expression.Code.Documents;

namespace Microsoft.Expression.Code.Nautilus
{
  internal class RedoCommand : CodeEditorCommand
  {
    public override bool IsEnabled
    {
      get
      {
        return this.UndoHistory.CanRedo;
      }
    }

    public RedoCommand(CodeEditor codeEditor)
      : base(codeEditor)
    {
    }

    public override void Execute()
    {
      this.UndoHistory.Redo(1);
    }
  }
}
