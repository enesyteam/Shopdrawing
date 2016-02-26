// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.TransitionEffectInfo
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Tools.Assets;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public class TransitionEffectInfo
  {
    private Asset asset;

    public Asset Asset
    {
      get
      {
        return this.asset;
      }
    }

    public string TransitionEffectName
    {
      get
      {
        if (this.asset != null)
          return this.asset.Name;
        return StringTable.TransitionEffectNone;
      }
    }

    public string TransitionEffectFullName
    {
      get
      {
        if (this.asset != null)
          return this.asset.TargetType.FullName;
        return StringTable.TransitionEffectNone;
      }
    }

    public string TransitionEffectDescription
    {
      get
      {
        if (this.asset != null)
          return ((TypeAssetInfoModel) this.asset.AssetInfo).Description;
        return StringTable.TransitionEffectNone;
      }
    }

    public bool IsLoadingPlaceholder { get; set; }

    public TransitionEffectInfo(Asset asset)
    {
      this.asset = asset;
    }

    public override string ToString()
    {
      return this.TransitionEffectName;
    }
  }
}
