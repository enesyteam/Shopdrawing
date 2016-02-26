// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Assets.AssetAggregator
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Expression.DesignSurface.Tools.Assets
{
  internal class AssetAggregator : AssetProvider
  {
    private List<AssetProvider> assetProviders = new List<AssetProvider>();
    private Microsoft.Expression.Framework.IReadOnlyCollection<AssetProvider> readonlyAssetProviders;

    public Microsoft.Expression.Framework.IReadOnlyCollection<AssetProvider> AssetProviders
    {
      get
      {
        return this.readonlyAssetProviders ?? (this.readonlyAssetProviders = (Microsoft.Expression.Framework.IReadOnlyCollection<AssetProvider>) new ReadOnlyList<AssetProvider>((IList<AssetProvider>) this.assetProviders));
      }
    }

    protected override void InternalDispose(bool disposing)
    {
      base.InternalDispose(disposing);
      if (!disposing)
        return;
      this.ClearProviders();
    }

    public void ClearProviders()
    {
      foreach (AssetProvider assetProvider in this.assetProviders)
      {
        assetProvider.AssetsChanged -= new EventHandler(this.OnProviderAssetsChanged);
        assetProvider.NeedsUpdateChanged -= new EventHandler(this.Provider_NeedsUpdateChanged);
        assetProvider.Dispose();
      }
      this.assetProviders.Clear();
      if (this.Assets.Count <= 0)
        return;
      this.Assets.Clear();
      this.NeedsUpdate = false;
      this.NotifyAssetsChanged();
    }

    public void AddProvider(AssetProvider provider)
    {
      provider.AssetsChanged += new EventHandler(this.OnProviderAssetsChanged);
      provider.NeedsUpdateChanged += new EventHandler(this.Provider_NeedsUpdateChanged);
      this.assetProviders.Add(provider);
      this.NeedsUpdate = true;
    }

    public void RemoveProvider(AssetProvider provider)
    {
      provider.AssetsChanged -= new EventHandler(this.OnProviderAssetsChanged);
      provider.NeedsUpdateChanged -= new EventHandler(this.Provider_NeedsUpdateChanged);
      provider.Dispose();
      this.assetProviders.Remove(provider);
      this.NeedsUpdate = true;
    }

    protected override bool UpdateAssets()
    {
      if (!this.NeedsUpdate)
        return false;
      this.NeedsUpdate = false;
      ObservableCollectionWorkaround<Asset> collectionWorkaround = this.Assets.Clone();
      List<Asset> list = new List<Asset>(collectionWorkaround.Count);
      this.Assets.Clear();
      foreach (AssetProvider assetProvider in this.assetProviders)
      {
        foreach (Asset asset in (Collection<Asset>) assetProvider.Assets)
          list.Add(asset);
      }
      collectionWorkaround.Sort(Asset.DefaultComparer);
      list.Sort(Asset.DefaultComparer);
      int index1 = 0;
      int index2 = 0;
      while (index1 < collectionWorkaround.Count && index2 < list.Count)
      {
        int num = collectionWorkaround[index1].CompareTo(list[index2]);
        if (num == 0)
        {
          this.Assets.Add(list[index2]);
          ++index1;
          ++index2;
        }
        else if (num < 0)
          collectionWorkaround[index1++].IsValid = false;
        else
          this.Assets.Add(list[index2++]);
      }
      while (index1 < collectionWorkaround.Count)
        collectionWorkaround[index1++].IsValid = false;
      while (index2 < list.Count)
        this.Assets.Add(list[index2++]);
      foreach (Asset asset in (Collection<Asset>) this.Assets)
        asset.IsValid = true;
      this.NotifyAssetsChanged();
      return true;
    }

    private void Provider_NeedsUpdateChanged(object sender, EventArgs e)
    {
      AssetAggregator assetAggregator = this;
      int num = assetAggregator.NeedsUpdate | ((AssetProvider) sender).NeedsUpdate ? true : false;
      assetAggregator.NeedsUpdate = num != 0;
    }

    private void OnProviderAssetsChanged(object sender, EventArgs e)
    {
      this.NeedsUpdate = true;
    }
  }
}
