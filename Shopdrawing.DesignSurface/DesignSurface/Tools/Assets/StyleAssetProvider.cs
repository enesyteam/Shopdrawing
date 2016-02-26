// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Assets.StyleAssetProvider
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.UserInterface.ResourcePane;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;

namespace Microsoft.Expression.DesignSurface.Tools.Assets
{
  internal class StyleAssetProvider : ResourceAssetProvider
  {
    public override bool IsResourceValid(ResourceModel resourceModel)
    {
      DocumentNode keyNode = resourceModel.KeyNode;
      DocumentCompositeNode valueNode = resourceModel.ValueNode as DocumentCompositeNode;
      bool flag = keyNode != null && valueNode != null && PlatformTypes.Style.IsAssignableFrom((ITypeId) valueNode.Type) && valueNode.Properties[StyleNode.TargetTypeProperty] != null;
      if (flag)
      {
        IType styleType = StyleAsset.GetStyleType(valueNode);
        if (styleType == null || styleType.RuntimeType == (Type) null || !TypeUtilities.HasDefaultConstructor(styleType.RuntimeType, false))
          flag = false;
      }
      return flag;
    }

    public override ResourceAsset CreateAsset(ResourceModel resourceModel)
    {
      return (ResourceAsset) new StyleAsset(this, resourceModel);
    }
  }
}
