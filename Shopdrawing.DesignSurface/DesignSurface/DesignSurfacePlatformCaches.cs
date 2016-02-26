// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.DesignSurfacePlatformCaches
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using System;

namespace Microsoft.Expression.DesignSurface
{
  internal static class DesignSurfacePlatformCaches
  {
    public static readonly string UserThemeAssetAggregatorProviderCache = "Blend.UserThemeAssetAggregatorProviderCache";
    public static readonly string ThemeContentProviderCache = "Blend.ThemeContentProviderCache";
    public static readonly string EasingFunctionsCache = "Blend.EasingFunctionsCache";
    public static readonly string LayoutDesignerFactory = "Blend.LayoutDesignerFactory";
    public static readonly string AssemblyAssetProviders = "Blend.AssemblyAssetProviders";
    public static readonly string AmbientPropertyCache = "Blend.AmbientPropertyCache";
    public static readonly string MoveStrategyFactoryCache = "Blend.MoveStrategyFactory";

    public static T GetOrCreateCache<T>(this IPlatformTypes platformTypes, string cacheKey) where T : class
    {
      T obj = (T) platformTypes.GetPlatformCache(cacheKey);
      if ((object) obj == null)
      {
        obj = (T) Activator.CreateInstance(typeof (T));
        platformTypes.SetPlatformCache(cacheKey, (object) obj);
      }
      return obj;
    }
  }
}
