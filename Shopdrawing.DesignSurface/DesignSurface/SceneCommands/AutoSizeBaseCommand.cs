// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.AutoSizeBaseCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal abstract class AutoSizeBaseCommand : SceneCommandBase
  {
    public override bool IsEnabled
    {
      get
      {
        if (base.IsEnabled && this.Selection != null)
          return this.Selection.Count > 0;
        return false;
      }
    }

    protected ICollection<BaseFrameworkElement> Selection
    {
      get
      {
        if (this.SceneViewModel == null || this.SceneViewModel.ActiveStoryboardTimeline != null)
          return (ICollection<BaseFrameworkElement>) null;
        return this.SceneViewModel.ElementSelectionSet.GetFilteredSelection<BaseFrameworkElement>();
      }
    }

    public AutoSizeBaseCommand(SceneViewModel viewModel)
      : base(viewModel)
    {
    }
  }
}
