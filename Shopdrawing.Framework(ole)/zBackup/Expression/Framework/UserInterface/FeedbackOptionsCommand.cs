// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.UserInterface.FeedbackOptionsCommand
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Extensibility;
using Microsoft.Expression.Framework.Commands;

namespace Microsoft.Expression.Framework.UserInterface
{
  public class FeedbackOptionsCommand : Command
  {
    private IServices services;

    public FeedbackOptionsCommand(IServices services)
    {
      this.services = services;
    }

    public override void Execute()
    {
      FeedbackDialog feedbackDialog = new FeedbackDialog(this.services);
      bool? nullable = feedbackDialog.ShowDialog();
      if (!nullable.HasValue || !nullable.Value)
        return;
      feedbackDialog.Model.CommitChanges();
    }
  }
}
