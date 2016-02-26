// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.ModalCommands.EditControlModalCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.SceneCommands;
using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.SceneCommands.ModalCommands
{
  internal sealed class EditControlModalCommand : ModalCommandBase
  {
    private SceneElement target;

    public override bool IsEnabled
    {
      get
      {
        if (this.target != this.SceneViewModel.RootNode)
          return this.XamlSourcePath != null;
        return false;
      }
    }

    public override string Description
    {
      get
      {
        return StringTable.ElementContextMenuEditControl;
      }
    }

    private string XamlSourcePath
    {
      get
      {
        return this.target.Type.XamlSourcePath;
      }
    }

    public EditControlModalCommand(SceneViewModel viewModel, SceneElement target)
      : base(viewModel)
    {
      this.target = target;
    }

    public override void Execute()
    {
      EditControlCommand.EditControl(this.target);
    }
  }
}
