// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.FormatSelectedObjectsCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Selection;
using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal abstract class FormatSelectedObjectsCommand : SceneCommandBase
  {
    public override bool IsEnabled
    {
      get
      {
        if (base.IsEnabled)
        {
          SceneElementSelectionSet elementSelectionSet = this.SceneViewModel.ElementSelectionSet;
          if (elementSelectionSet.Count > 0)
          {
            foreach (SceneElement element in elementSelectionSet.Selection)
            {
              if (this.IsSelectedElementValid(element))
                return true;
            }
          }
        }
        return false;
      }
    }

    protected FormatSelectedObjectsCommand(SceneViewModel viewModel)
      : base(viewModel)
    {
    }

    protected abstract bool IsSelectedElementValid(SceneElement element);
  }
}
