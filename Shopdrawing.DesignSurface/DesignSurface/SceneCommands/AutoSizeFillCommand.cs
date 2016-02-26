// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.AutoSizeFillCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal sealed class AutoSizeFillCommand : AutoSizeBaseCommand
  {
    public AutoSizeFillCommand(SceneViewModel viewModel)
      : base(viewModel)
    {
    }

    public override void Execute()
    {
      ICollection<BaseFrameworkElement> selection = this.Selection;
      if (selection == null)
        return;
      using (SceneEditTransaction editTransaction = this.SceneViewModel.CreateEditTransaction(StringTable.UndoUnitAutoSizeFill))
      {
        foreach (BaseFrameworkElement element in (IEnumerable<BaseFrameworkElement>) selection)
          this.SceneViewModel.GetLayoutDesignerForChild((SceneElement) element, true).FillChild(element);
        editTransaction.Commit();
      }
    }
  }
}
