// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Assets.NonLocalStyleAssetInstantiator
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.Tools.Assets
{
  internal class NonLocalStyleAssetInstantiator : StyleAssetInstantiator
  {
    private ResourceDictionaryAssetProvider provider;

    public NonLocalStyleAssetInstantiator(SceneView sceneView, ResourceDictionaryAssetProvider provider, NonLocalStyleAsset styleAsset)
      : base(sceneView, (StyleAssetProvider) provider, (StyleAsset) styleAsset)
    {
      this.provider = provider;
    }

    internal override void ApplyBeforeInsertionDefaultsToElements(IList<SceneNode> nodes, SceneNode rootNode, DefaultTypeInstantiator.SceneElementNamingCallback callback)
    {
      if (this.provider.ContentProvider != null)
        this.provider.ContentProvider.EnsureLinked(this.ViewModel);
      base.ApplyBeforeInsertionDefaultsToElements(nodes, rootNode, callback);
    }
  }
}
