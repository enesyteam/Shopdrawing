// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Assets.StyleAssetInstantiator
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections;

namespace Microsoft.Expression.DesignSurface.Tools.Assets
{
  internal class StyleAssetInstantiator : UserThemeTypeInstantiator
  {
    private StyleAssetProvider provider;
    private StyleAsset styleAsset;

    public StyleAssetInstantiator(SceneView sceneView, StyleAssetProvider provider, StyleAsset styleAsset)
      : base(sceneView)
    {
      this.provider = provider;
      this.styleAsset = styleAsset;
    }

    protected override StyleAsset GetRelatedUserThemeAsset(SceneNode node, SceneNode rootNode)
    {
      if (node == rootNode)
        return this.styleAsset;
      return StyleAsset.Find((IEnumerable) this.provider.Assets, (ITypeId) node.Type) ?? base.GetRelatedUserThemeAsset(node, rootNode);
    }
  }
}
