// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.ThicknessNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class ThicknessNode : SceneNode
  {
    public static readonly IPropertyId BottomProperty = (IPropertyId) PlatformTypes.Thickness.GetMember(MemberType.LocalProperty, "Bottom", MemberAccessTypes.Public);
    public static readonly IPropertyId LeftProperty = (IPropertyId) PlatformTypes.Thickness.GetMember(MemberType.LocalProperty, "Left", MemberAccessTypes.Public);
    public static readonly IPropertyId RightProperty = (IPropertyId) PlatformTypes.Thickness.GetMember(MemberType.LocalProperty, "Right", MemberAccessTypes.Public);
    public static readonly IPropertyId TopProperty = (IPropertyId) PlatformTypes.Thickness.GetMember(MemberType.LocalProperty, "Top", MemberAccessTypes.Public);
    public static readonly ThicknessNode.ConcreteThicknessNodeFactory Factory = new ThicknessNode.ConcreteThicknessNodeFactory();

    public class ConcreteThicknessNodeFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new ThicknessNode();
      }
    }
  }
}
