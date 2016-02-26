// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.View.ApplyProjectionsCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.View
{
  internal sealed class ApplyProjectionsCommand : SceneViewDesignCheckCommand
  {
    public override bool IsAvailable
    {
      get
      {
        return this.SceneView.ProjectContext.IsCapabilitySet(PlatformCapability.SupportsUIElementProjection);
      }
    }

    public ApplyProjectionsCommand(SceneView sceneView)
      : base(sceneView)
    {
    }

    protected override bool IsChecked()
    {
      return !this.SceneView.InstanceBuilderContext.DisableProjectionTransforms;
    }

    protected override void OnCheckedChanged(bool isChecked)
    {
      this.SceneView.InstanceBuilderContext.DisableProjectionTransforms = !isChecked;
      this.SceneView.InvalidateAndUpdate();
      this.SceneView.InvalidateAndRebuildAdornerSets();
    }
  }
}
