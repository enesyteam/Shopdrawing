// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.NavigateUpCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal sealed class NavigateUpCommand : SceneCommandBase
  {
    public override bool IsEnabled
    {
      get
      {
        if (base.IsEnabled)
          return this.SceneViewModel.ElementSelectionSet.Count > 0;
        return false;
      }
    }

    public NavigateUpCommand(SceneViewModel viewModel)
      : base(viewModel)
    {
    }

    public override object GetProperty(string propertyName)
    {
      if (!(propertyName == "IsVisible"))
        return base.GetProperty(propertyName);
      return (object) false;
    }

    public override void Execute()
    {
      this.SceneViewModel.TimelineItemManager.NavigateUp();
    }
  }
}
