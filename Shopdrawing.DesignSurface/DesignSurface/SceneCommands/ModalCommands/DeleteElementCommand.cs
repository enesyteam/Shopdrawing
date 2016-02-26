// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.ModalCommands.DeleteElementCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.SceneCommands.ModalCommands
{
  internal sealed class DeleteElementCommand : ModalCommandBase
  {
    private SceneElement target;

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
        return StringTable.DeleteElementContextualDescription;
      }
    }

    public DeleteElementCommand(SceneElement target, SceneViewModel viewModel)
      : base(viewModel)
    {
      this.target = target;
    }

    public override void Execute()
    {
      using (SceneEditTransaction editTransaction = this.SceneViewModel.CreateEditTransaction(StringTable.UndoUnitDelete))
      {
        this.SceneViewModel.DeleteElementTree(this.target);
        editTransaction.Commit();
      }
    }
  }
}
