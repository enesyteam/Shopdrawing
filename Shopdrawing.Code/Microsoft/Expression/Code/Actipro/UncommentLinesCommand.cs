// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.Actipro.UncommentLinesCommand
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using Microsoft.Expression.Code.Documents;
using Microsoft.Expression.Framework.Documents;

namespace Microsoft.Expression.Code.Actipro
{
  internal class UncommentLinesCommand : ActiproCommand
  {
    private IViewService viewService;

    public override bool IsAvailable
    {
      get
      {
        if (this.viewService.ActiveView != null)
          return this.viewService.ActiveView is CodeView;
        return false;
      }
    }

    public UncommentLinesCommand(ActiproEditor editor, IViewService viewService)
      : base(editor)
    {
      this.viewService = viewService;
    }

    public override void Execute()
    {
      this.View.UncommentLines();
      base.Execute();
    }
  }
}
