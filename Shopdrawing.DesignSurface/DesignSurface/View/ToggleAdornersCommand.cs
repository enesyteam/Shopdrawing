// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.View.ToggleAdornersCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.Framework.Commands;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.View
{
  internal sealed class ToggleAdornersCommand : CheckCommandBase
  {
    private SceneView sceneView;

    public ToggleAdornersCommand(SceneView sceneView)
    {
      this.sceneView = sceneView;
    }

    protected override bool IsChecked()
    {
      AdornerLayer adornerLayer = this.sceneView.AdornerLayer;
      if (adornerLayer != null)
        return adornerLayer.IsVisible;
      return false;
    }

    protected override void OnCheckedChanged(bool isChecked)
    {
      AdornerLayer adornerLayer = this.sceneView.AdornerLayer;
      if (adornerLayer == null)
        return;
      adornerLayer.Visibility = isChecked ? Visibility.Visible : Visibility.Collapsed;
    }
  }
}
