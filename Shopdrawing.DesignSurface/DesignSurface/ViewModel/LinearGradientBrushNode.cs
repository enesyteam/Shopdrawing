// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.LinearGradientBrushNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class LinearGradientBrushNode : GradientBrushNode
  {
    public static readonly IPropertyId EndPointProperty = (IPropertyId) PlatformTypes.LinearGradientBrush.GetMember(MemberType.LocalProperty, "EndPoint", MemberAccessTypes.Public);
    public static readonly IPropertyId StartPointProperty = (IPropertyId) PlatformTypes.LinearGradientBrush.GetMember(MemberType.LocalProperty, "StartPoint", MemberAccessTypes.Public);
    public static readonly LinearGradientBrushNode.ConcreteLinearGradientBrushNodeFactory Factory = new LinearGradientBrushNode.ConcreteLinearGradientBrushNodeFactory();

    public class ConcreteLinearGradientBrushNodeFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new LinearGradientBrushNode();
      }
    }
  }
}
