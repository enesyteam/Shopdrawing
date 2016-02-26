// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Assets.StyleAssetInfoModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.UserInterface.ResourcePane;

namespace Microsoft.Expression.DesignSurface.Tools.Assets
{
  public class StyleAssetInfoModel : AssetInfoModel
  {
    public string Description { get; private set; }

    public string TargetType { get; private set; }

    public string StyleName { get; private set; }

    internal static StyleAssetInfoModel Create(IType type, ResourceModel resourceModel)
    {
      StyleAssetInfoModel styleAssetInfoModel = new StyleAssetInfoModel();
      styleAssetInfoModel.TargetType = type.Name;
      styleAssetInfoModel.Description = AssetInfoModel.GetDescription(type);
      styleAssetInfoModel.SetFullPath(resourceModel.KeyNode.Context.DocumentUrl);
      styleAssetInfoModel.StyleName = resourceModel.Name ?? type.Name;
      return styleAssetInfoModel;
    }
  }
}
