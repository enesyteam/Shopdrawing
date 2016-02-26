// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.ConvertToPathCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Geometry;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal sealed class ConvertToPathCommand : SceneCommandBase
  {
    public override bool IsEnabled
    {
      get
      {
        if (!base.IsEnabled)
          return false;
        foreach (SceneElement element in this.SceneViewModel.ElementSelectionSet.Selection)
        {
          if (element.Parent == null || PlatformTypes.Path.Equals((object) element.Type) || !PathConversionHelper.CanConvert(element))
            return false;
        }
        return this.SceneViewModel.ElementSelectionSet.Count > 0;
      }
    }

    public ConvertToPathCommand(SceneViewModel viewModel)
      : base(viewModel)
    {
    }

    public override void Execute()
    {
      using (this.SceneViewModel.DisableUpdateChildrenOnAddAndRemove())
      {
        using (SceneEditTransaction editTransaction = this.SceneViewModel.CreateEditTransaction(StringTable.UndoUnitConvertToPath, false))
        {
          PathCommandHelper.ConvertSelectionToPath(this.SceneViewModel.ElementSelectionSet);
          editTransaction.Commit();
        }
      }
    }
  }
}
