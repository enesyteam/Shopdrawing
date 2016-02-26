// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Assets.CustomAssetCategoryPath
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Windows.Design;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Expression.DesignSurface.Tools.Assets
{
  [Serializable]
  public class CustomAssetCategoryPath
  {
    public string CategoryPath { get; private set; }

    public bool AlwaysShows { get; private set; }

    private CustomAssetCategoryPath(string categoryPath, bool alwaysShows)
    {
      this.CategoryPath = categoryPath;
      this.AlwaysShows = alwaysShows;
    }

    public static CustomAssetCategoryPath Convert(ToolboxCategoryAttribute attribute)
    {
      return new CustomAssetCategoryPath(AssetCategoryPath.AssetCategoryPathHelper.NormalizePath(attribute.CategoryPath), attribute.AlwaysShows);
    }

    public static IEnumerable<CustomAssetCategoryPath> Convert(IEnumerable<ToolboxCategoryAttribute> attributes)
    {
      return Enumerable.Select<ToolboxCategoryAttribute, CustomAssetCategoryPath>(attributes, (Func<ToolboxCategoryAttribute, CustomAssetCategoryPath>) (attribute => CustomAssetCategoryPath.Convert(attribute)));
    }
  }
}
