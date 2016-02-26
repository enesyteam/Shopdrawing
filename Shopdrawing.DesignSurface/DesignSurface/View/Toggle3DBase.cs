// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.View.Toggle3DBase
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Commands;

namespace Microsoft.Expression.DesignSurface.View
{
  internal abstract class Toggle3DBase : Command
  {
    private SceneView sceneView;

    public SceneView SceneView
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
        bool flag = !this.sceneView.ViewModel.ElementSelectionSet.IsEmpty;
        if (flag)
        {
          foreach (SceneElement sceneElement in this.sceneView.ViewModel.ElementSelectionSet.Selection)
          {
            if (sceneElement is Viewport3DElement || sceneElement is Base3DElement)
              return true;
          }
          flag = false;
        }
        return flag;
      }
    }

    public Toggle3DBase(SceneView sceneView)
    {
      this.sceneView = sceneView;
    }

    public abstract override void Execute();
  }
}
