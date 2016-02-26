// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.ShowObjectsCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Selection;
using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal class ShowObjectsCommand : FormatSelectedObjectsCommand
  {
    private bool show;

    public ShowObjectsCommand(SceneViewModel viewModel, bool show)
      : base(viewModel)
    {
      this.show = show;
    }

    public override void Execute()
    {
      SceneElementSelectionSet elementSelectionSet = this.SceneViewModel.ElementSelectionSet;
      using (SceneEditTransaction editTransaction = this.SceneViewModel.CreateEditTransaction(this.show ? StringTable.UndoUnitShow : StringTable.UndoUnitHide))
      {
        foreach (SceneElement sceneElement in elementSelectionSet.Selection)
        {
          if (sceneElement.CanHide)
            sceneElement.IsHidden = !this.show;
        }
        editTransaction.Commit();
      }
    }

    protected override bool IsSelectedElementValid(SceneElement element)
    {
      if (element.CanHide)
        return element.IsHidden == this.show;
      return false;
    }
  }
}
