// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.View.BlendDocumentService
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Framework.Documents;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.View
{
  public class BlendDocumentService : DocumentService
  {
    public BlendDocumentService(ICommandService commandService, IMessageDisplayService messageDisplayService)
      : base(commandService, messageDisplayService)
    {
      string commandName = "Application_FileClose";
      this.RemoveCommand(commandName);
      this.AddCommand(commandName, (ICommand) new BlendDocumentService.CloseFileByClosingViewCommand(this, messageDisplayService));
    }

    protected override bool RecycleView(IView view)
    {
      SceneView sceneView = view as SceneView;
      if (sceneView == null)
        return false;
      return sceneView.DesignerContext.ViewRootResolver.RecycleView(sceneView);
    }

    private sealed class CloseFileByClosingViewCommand : Command
    {
      private BlendDocumentService documentService;
      private IMessageDisplayService messageDisplayService;

      public override bool IsEnabled
      {
        get
        {
          return this.documentService.ActiveDocument != null;
        }
      }

      public CloseFileByClosingViewCommand(BlendDocumentService documentService, IMessageDisplayService messageDisplayService)
      {
        this.documentService = documentService;
        this.messageDisplayService = messageDisplayService;
      }

      public override void Execute()
      {
        IDocument activeDocument = this.documentService.ActiveDocument;
        if (activeDocument == null || !DocumentUtilities.PromptUserAndSaveDocument(activeDocument, true, this.messageDisplayService))
          return;
        List<IView> list = new List<IView>((IEnumerable<IView>) this.documentService.GetDocumentViews(activeDocument));
        if (list != null && list.Count > 0)
        {
          foreach (IView view in list)
            this.documentService.CloseView(view);
        }
        else
          this.documentService.CloseDocument(activeDocument);
      }
    }
  }
}
