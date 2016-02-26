// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.View.ToggleLights
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.View
{
  internal sealed class ToggleLights : Toggle3DBase
  {
    public override bool IsAvailable
    {
      get
      {
        return JoltHelper.TypeSupported((ITypeResolver) this.SceneView.ViewModel.ProjectContext, PlatformTypes.Viewport3D);
      }
    }

    public ToggleLights(SceneView sceneView)
      : base(sceneView)
    {
    }

    public override void Execute()
    {
      List<SceneElement> list = new List<SceneElement>();
      foreach (SceneElement sceneElement in this.SceneView.ViewModel.ElementSelectionSet.Selection)
      {
        Viewport3DElement viewport3Delement;
        if (sceneElement is Base3DElement)
          viewport3Delement = ((Base3DElement) sceneElement).Viewport;
        else if (sceneElement is Viewport3DElement)
          viewport3Delement = (Viewport3DElement) sceneElement;
        else
          continue;
        list.Add((SceneElement) viewport3Delement);
      }
      bool flag = false;
      List<Viewport3DElement> withProxiedLights = this.SceneView.AdornerLayer.ContainersWithProxiedLights;
      foreach (Viewport3DElement viewport3Delement in list)
      {
        if (!withProxiedLights.Contains(viewport3Delement))
        {
          this.SceneView.AdornerLayer.ContainersWithProxiedLights.Add(viewport3Delement);
          flag = true;
        }
      }
      if (!flag)
        this.SceneView.AdornerLayer.ContainersWithProxiedLights.Clear();
      this.SceneView.ViewModel.DesignerContext.ToolManager.ActiveTool.RebuildAdornerSets();
    }
  }
}
