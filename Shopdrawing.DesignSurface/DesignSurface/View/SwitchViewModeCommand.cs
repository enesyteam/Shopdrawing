// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.View.SwitchViewModeCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework.Commands;

namespace Microsoft.Expression.DesignSurface.View
{
  internal sealed class SwitchViewModeCommand : CheckCommandBase
  {
    private SceneView sceneView;
    private ViewMode viewMode;

    public SwitchViewModeCommand(SceneView sceneView, ViewMode viewMode)
    {
      this.sceneView = sceneView;
      this.viewMode = viewMode;
    }

    protected override bool IsChecked()
    {
      return this.sceneView.ViewMode == this.viewMode;
    }

    protected override void OnCheckedChanged(bool isChecked)
    {
      if (!isChecked)
        return;
      this.sceneView.ViewMode = this.viewMode;
    }
  }
}
