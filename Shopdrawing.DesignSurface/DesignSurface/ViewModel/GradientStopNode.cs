// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.GradientStopNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class GradientStopNode : SceneNode
  {
    public static readonly IPropertyId ColorProperty = (IPropertyId) PlatformTypes.GradientStop.GetMember(MemberType.LocalProperty, "Color", MemberAccessTypes.Public);
    public static readonly IPropertyId OffsetProperty = (IPropertyId) PlatformTypes.GradientStop.GetMember(MemberType.LocalProperty, "Offset", MemberAccessTypes.Public);
    public static readonly GradientStopNode.ConcreteGradientStopNodeFactory Factory = new GradientStopNode.ConcreteGradientStopNodeFactory();

    public class ConcreteGradientStopNodeFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new GradientStopNode();
      }
    }
  }
}
