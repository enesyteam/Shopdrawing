// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Assets.TypeAssetInfoModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.Project;

namespace Microsoft.Expression.DesignSurface.Tools.Assets
{
  public class TypeAssetInfoModel : AssetInfoModel
  {
    public string Description { get; private set; }

    public string TypeName { get; private set; }

    public string Namespace { get; private set; }

    internal static TypeAssetInfoModel Create(IType type, IProject project, ExampleAssetInfo exampleInfo)
    {
      TypeAssetInfoModel typeAssetInfoModel = new TypeAssetInfoModel();
      typeAssetInfoModel.TypeName = type.Name;
      typeAssetInfoModel.Namespace = type.Namespace;
      typeAssetInfoModel.Description = exampleInfo == null || exampleInfo.Description == null ? AssetInfoModel.GetDescription(type) : exampleInfo.Description;
      typeAssetInfoModel.SetFullPath(AssetInfoModel.AssemblyLocation(type, project));
      return typeAssetInfoModel;
    }
  }
}
