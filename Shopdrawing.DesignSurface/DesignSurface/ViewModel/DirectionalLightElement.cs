// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.DirectionalLightElement
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public sealed class DirectionalLightElement : LightElement
  {
    public static readonly IPropertyId DirectionProperty = (IPropertyId) PlatformTypes.DirectionalLight.GetMember(MemberType.LocalProperty, "Direction", MemberAccessTypes.Public);
    public static readonly DirectionalLightElement.ConcreteDirectionalLightElementFactory Factory = new DirectionalLightElement.ConcreteDirectionalLightElementFactory();

    public Vector3D Direction
    {
      get
      {
        return ((DirectionalLight) this.ViewObject.PlatformSpecificObject).Direction;
      }
    }

    public class ConcreteDirectionalLightElementFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new DirectionalLightElement();
      }
    }
  }
}
