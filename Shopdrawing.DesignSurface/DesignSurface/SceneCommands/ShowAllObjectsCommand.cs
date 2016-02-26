// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.ShowAllObjectsCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal class ShowAllObjectsCommand : FormatAllObjectsCommand
  {
    private bool show;

    public ShowAllObjectsCommand(SceneViewModel viewModel, bool show)
      : base(viewModel)
    {
      this.show = show;
    }

    public override void Execute()
    {
      SceneElement rootElement = this.SceneViewModel.ActiveEditingContainer as SceneElement;
      using (SceneEditTransaction editTransaction = this.SceneViewModel.CreateEditTransaction(this.show ? StringTable.UndoUnitShowAll : StringTable.UndoUnitHideAll))
      {
        foreach (SceneElement sceneElement in SceneElementHelper.GetElementTree(rootElement))
        {
          if (sceneElement.CanHide)
            sceneElement.IsHidden = !this.show;
        }
        editTransaction.Commit();
      }
    }
  }
}
