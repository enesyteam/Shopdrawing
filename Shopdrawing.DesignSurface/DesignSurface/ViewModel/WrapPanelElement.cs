// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.WrapPanelElement
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public sealed class WrapPanelElement : PanelElement
  {
    public static readonly IPropertyId ItemHeightProperty = (IPropertyId) ProjectNeutralTypes.WrapPanel.GetMember(MemberType.LocalProperty, "ItemHeight", MemberAccessTypes.Public);
    public static readonly IPropertyId ItemWidthProperty = (IPropertyId) ProjectNeutralTypes.WrapPanel.GetMember(MemberType.LocalProperty, "ItemWidth", MemberAccessTypes.Public);
    public static readonly IPropertyId OrientationProperty = (IPropertyId) ProjectNeutralTypes.WrapPanel.GetMember(MemberType.LocalProperty, "Orientation", MemberAccessTypes.Public);
    public static readonly WrapPanelElement.ConcreteWrapPanelElementFactory Factory = new WrapPanelElement.ConcreteWrapPanelElementFactory();

    public class ConcreteWrapPanelElementFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new WrapPanelElement();
      }
    }
  }
}
