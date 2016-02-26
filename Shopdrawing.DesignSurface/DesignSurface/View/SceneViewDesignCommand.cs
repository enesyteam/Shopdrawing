// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.View.SceneViewDesignCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework.Commands;

namespace Microsoft.Expression.DesignSurface.View
{
  internal abstract class SceneViewDesignCommand : Command
  {
    private SceneView sceneView;

    protected SceneView SceneView
    {
      get
      {
        return this.sceneView;
      }
    }

    public override bool IsEnabled
    {
      get
      {
        if (base.IsEnabled)
          return SceneViewDesignCommand.DesignCommandEnabled(this.SceneView);
        return false;
      }
    }

    protected SceneViewDesignCommand(SceneView sceneView)
    {
      this.sceneView = sceneView;
    }

    public static bool DesignCommandEnabled(SceneView sceneView)
    {
      if (sceneView.IsDesignSurfaceEnabled)
        return sceneView.IsValid;
      return false;
    }
  }
}
