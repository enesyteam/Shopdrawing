// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.View.SwitchSplitViewOrientationCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework.Commands;

namespace Microsoft.Expression.DesignSurface.View
{
  internal sealed class SwitchSplitViewOrientationCommand : CheckCommandBase
  {
    private SceneView sceneView;
    private ViewOptionsModel viewOptionsModel;
    private bool isVerticalSplit;

    public override bool IsEnabled
    {
      get
      {
        return this.sceneView.ViewMode == ViewMode.Split;
      }
    }

    public SwitchSplitViewOrientationCommand(SceneView sceneView, ViewOptionsModel viewOptionsModel, bool isVerticalSplit)
    {
      this.sceneView = sceneView;
      this.viewOptionsModel = viewOptionsModel;
      this.isVerticalSplit = isVerticalSplit;
    }

    protected override bool IsChecked()
    {
      return this.viewOptionsModel.IsVerticalSplit == this.isVerticalSplit;
    }

    protected override void OnCheckedChanged(bool isChecked)
    {
      if (!isChecked)
        return;
      this.viewOptionsModel.IsVerticalSplit = this.isVerticalSplit;
    }
  }
}
