// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.Documents.FindInFilesCommand
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using Microsoft.Expression.Code.UserInterface;
using Microsoft.Expression.Extensibility;
using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Project;

namespace Microsoft.Expression.Code.Documents
{
  public class FindInFilesCommand : Command
  {
    private IServices services;

    public override bool IsEnabled
    {
      get
      {
        IProjectManager service = this.services.GetService<IProjectManager>();
        if (base.IsEnabled)
          return service.CurrentSolution != null;
        return false;
      }
    }

    public FindInFilesCommand(IServices services)
    {
      this.services = services;
    }

    public override void Execute()
    {
      FindDialog.CloseIfOpen();
      ReplaceDialog.CloseIfOpen();
      FindInFilesDialog.GetFindInFilesDialog(this.services).Show();
    }
  }
}
