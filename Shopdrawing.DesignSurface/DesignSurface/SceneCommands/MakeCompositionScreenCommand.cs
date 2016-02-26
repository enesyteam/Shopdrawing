// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.MakeCompositionScreenCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal sealed class MakeCompositionScreenCommand : MakeUserControlCommandBase
  {
    public override bool IsAvailable
    {
      get
      {
        if (base.IsAvailable)
          return this.IsPrototypingEnabled;
        return false;
      }
    }

    public override bool AddToApplicationFlow
    {
      get
      {
        return true;
      }
    }

    protected override string DialogTitle
    {
      get
      {
        return StringTable.MakeCompositionScreenDialogTitle;
      }
    }

    public MakeCompositionScreenCommand(SceneViewModel viewModel)
      : base(viewModel)
    {
    }

    protected override string GetRecommendedName(IEnumerable<SceneElement> elementsToCopy)
    {
      return base.GetRecommendedName(elementsToCopy) + StringTable.RecommendedCompositionScreenName;
    }
  }
}
