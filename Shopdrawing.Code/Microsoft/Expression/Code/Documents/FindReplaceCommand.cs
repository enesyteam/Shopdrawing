// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.Documents.FindReplaceCommand
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using Microsoft.Expression.Code.UserInterface;
using Microsoft.Expression.DesignModel.Code;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Commands;
using System.Windows;

namespace Microsoft.Expression.Code.Documents
{
  internal abstract class FindReplaceCommand : Command
  {
    private static FindReplaceModel findReplaceModel = new FindReplaceModel();
    private ITextEditor textEditor;
    private IMessageDisplayService messageManager;

    public override bool IsEnabled
    {
      get
      {
        if (base.IsEnabled)
          return this.TextEditor != null;
        return false;
      }
    }

    protected ITextEditor TextEditor
    {
      get
      {
        return this.textEditor;
      }
    }

    protected FindReplaceModel FindReplaceModel
    {
      get
      {
        return FindReplaceCommand.findReplaceModel;
      }
    }

    protected IMessageDisplayService MessageDisplayService
    {
      get
      {
        return this.messageManager;
      }
    }

    public FindReplaceCommand(ITextEditor textEditor, IMessageDisplayService messageManager)
    {
      this.textEditor = textEditor;
      this.messageManager = messageManager;
    }

    protected void ShowMessage(string message, MessageBoxButton button, MessageBoxImage image)
    {
      int num = (int) this.messageManager.ShowMessage(new MessageBoxArgs()
      {
        Message = message,
        Button = button,
        Image = image
      });
    }
  }
}
