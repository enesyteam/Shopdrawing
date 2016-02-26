// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.LockObjectsCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Selection;
using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal class LockObjectsCommand : FormatSelectedObjectsCommand
  {
    private bool locked;

    public LockObjectsCommand(SceneViewModel viewModel, bool locked)
      : base(viewModel)
    {
      this.locked = locked;
    }

    public override void Execute()
    {
      SceneElementSelectionSet elementSelectionSet = this.SceneViewModel.ElementSelectionSet;
      using (SceneEditTransaction editTransaction = this.SceneViewModel.CreateEditTransaction(StringTable.UndoUnitLock))
      {
        foreach (SceneElement sceneElement in elementSelectionSet.Selection)
        {
          if (sceneElement.CanLock)
            sceneElement.IsLocked = this.locked;
        }
        editTransaction.Commit();
      }
    }

    protected override bool IsSelectedElementValid(SceneElement element)
    {
      if (element.IsLocked == !this.locked)
        return element.CanLock;
      return false;
    }
  }
}
