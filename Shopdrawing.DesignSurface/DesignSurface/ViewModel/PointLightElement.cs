// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.PointLightElement
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public sealed class PointLightElement : LightElement
  {
    public static readonly PointLightElement.ConcretePointLightElementFactory Factory = new PointLightElement.ConcretePointLightElementFactory();

    public Point3D Position
    {
      get
      {
        return ((PointLightBase) this.ViewObject.PlatformSpecificObject).Position;
      }
    }

    public class ConcretePointLightElementFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new PointLightElement();
      }
    }
  }
}
