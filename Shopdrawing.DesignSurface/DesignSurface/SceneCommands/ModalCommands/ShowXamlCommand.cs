// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.ModalCommands.ShowXamlCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.SceneCommands;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.SceneCommands.ModalCommands
{
  internal sealed class ShowXamlCommand : ModalCommandBase
  {
    private DocumentNode target;

    public override bool IsEnabled
    {
      get
      {
        return true;
      }
    }

    public override string Description
    {
      get
      {
        return StringTable.ShowXamlContextualDescription;
      }
    }

    public ShowXamlCommand(SceneViewModel viewModel, DocumentNode target)
      : base(viewModel)
    {
      this.target = target;
    }

    public override void Execute()
    {
      if (this.target.DocumentRoot == null)
        return;
      GoToXamlCommand.GoToXaml((SceneXamlDocument) this.target.DocumentRoot, new List<DocumentNode>()
      {
        this.target
      });
    }
  }
}
