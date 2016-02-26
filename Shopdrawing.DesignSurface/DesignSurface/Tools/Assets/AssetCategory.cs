// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Assets.AssetCategory
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework.Controls;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Expression.DesignSurface.Tools.Assets
{
  public sealed class AssetCategory : VirtualizingTreeItem<AssetCategory>
  {
    public AssetCategoryPath Path { get; private set; }

    public override string FullName
    {
      get
      {
        return this.Path.FullPath;
      }
    }

    public override string DisplayName
    {
      get
      {
        return this.Path.DisplayName;
      }
      set
      {
        throw new NotImplementedException();
      }
    }

    public AssetCategory this[AssetCategoryPath path]
    {
      get
      {
        return this.InternalFindOrCreate(path, false);
      }
    }

    public AssetCategory Self
    {
      get
      {
        return this;
      }
    }

    public int AssetCount { get; set; }

    private AssetCategory(AssetCategoryPath path)
    {
      this.Path = path;
    }

    public static AssetCategory CreateRootCategory()
    {
      return new AssetCategory(AssetCategoryPath.Root);
    }

    public AssetCategory CreateCategory(AssetCategoryPath path)
    {
      return this.InternalFindOrCreate(path, true);
    }

    internal void UpdateSelfBinding()
    {
      this.OnPropertyChanged("Self");
    }

    public bool Contains(Asset asset)
    {
      if (asset.Categories == null || asset.Categories.Count == 0)
        return false;
      if (this.Path.IsRoot)
        return true;
      if (!this.Path.AlwaysShow)
        return Enumerable.Any<AssetCategoryPath>((IEnumerable<AssetCategoryPath>) asset.Categories, (Func<AssetCategoryPath, bool>) (path => this.Path.Equals((object) path)));
      AssetCategoryPath parentPath = this.Path;
      if (parentPath.Equals((object) PresetAssetCategoryPath.ControlsAll))
        parentPath = (AssetCategoryPath) PresetAssetCategoryPath.ControlsRoot;
      return Enumerable.Any<AssetCategoryPath>((IEnumerable<AssetCategoryPath>) asset.Categories, (Func<AssetCategoryPath, bool>) (path =>
      {
        if (!path.AlwaysShow)
          return parentPath.Equals((object) path);
        return parentPath.Contains(path);
      }));
    }

    internal void NotifyAssetCountChanged()
    {
      this.OnPropertyChanged("AssetCount");
    }

    private AssetCategory InternalFindOrCreate(AssetCategoryPath path, bool create)
    {
      if (path == null)
        return (AssetCategory) null;
      if (this.Path.Equals((object) path))
        return this;
      if (!this.Path.Contains(path))
        return (AssetCategory) null;
      IComparer<AssetCategory> treeItemComparer = this.TreeItemComparer;
      AssetCategory assetCategory1 = this;
      for (int length = this.Path.Steps.Length; length < path.Steps.Length; ++length)
      {
        string relativePath = path.Steps[length];
        AssetCategory child = new AssetCategory(assetCategory1.Path.Append(relativePath, true));
        int index = assetCategory1.Children.BinarySearch(child, treeItemComparer);
        if (index < 0)
        {
          if (create)
          {
            assetCategory1.InternalInsertChild(child, ~index);
            assetCategory1 = child;
          }
          else
          {
            AssetCategory assetCategory2;
            assetCategory1 = assetCategory2 = (AssetCategory) null;
            break;
          }
        }
        else
          assetCategory1 = assetCategory1.Children[index];
      }
      return assetCategory1;
    }

    public override int CompareTo(AssetCategory treeItem)
    {
      return this.Path.CompareTo(treeItem.Path);
    }
  }
}
