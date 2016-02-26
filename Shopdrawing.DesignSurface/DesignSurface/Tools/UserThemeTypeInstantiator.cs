// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.UserThemeTypeInstantiator
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Tools.Assets;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.Tools
{
  internal class UserThemeTypeInstantiator : DefaultTypeInstantiator
  {
    protected override bool ShouldUseDefaultInitializer
    {
      get
      {
        return false;
      }
    }

    public UserThemeTypeInstantiator(SceneView sceneView)
      : base(sceneView)
    {
    }

    protected override StyleAsset GetRelatedUserThemeAsset(SceneNode node, SceneNode rootNode)
    {
      if (node != null && node.Type != null)
        return this.ViewModel.DesignerContext.AssetLibrary.FindActiveUserThemeAsset((ITypeId) node.Type);
      return (StyleAsset) null;
    }
  }
}
