// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.MakeUserControlCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal sealed class MakeUserControlCommand : MakeUserControlCommandBase
  {
    protected override string DialogTitle
    {
      get
      {
        return StringTable.MakeUserControlDialogTitle;
      }
    }

    public MakeUserControlCommand(SceneViewModel viewModel)
      : base(viewModel)
    {
    }

    protected override string GetRecommendedName(IEnumerable<SceneElement> elementsToCopy)
    {
      string recommendedName = base.GetRecommendedName(elementsToCopy);
      if (!string.IsNullOrEmpty(recommendedName))
        recommendedName += "Control";
      return recommendedName;
    }
  }
}
