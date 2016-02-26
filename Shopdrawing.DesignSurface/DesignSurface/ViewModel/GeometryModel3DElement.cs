// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.GeometryModel3DElement
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class GeometryModel3DElement : Model3DElement
  {
    public static readonly IPropertyId MaterialProperty = (IPropertyId) PlatformTypes.GeometryModel3D.GetMember(MemberType.LocalProperty, "Material", MemberAccessTypes.Public);
    public static readonly GeometryModel3DElement.ConcreteGeometryModel3DElementFactory Factory = new GeometryModel3DElement.ConcreteGeometryModel3DElementFactory();

    public MaterialNode Material
    {
      get
      {
        return (MaterialNode) this.GetLocalValueAsSceneNode(GeometryModel3DElement.MaterialProperty);
      }
      set
      {
        this.SetValueAsSceneNode(GeometryModel3DElement.MaterialProperty, (SceneNode) value);
      }
    }

    public override Rect3D LocalSpaceBounds
    {
      get
      {
        Rect3D rect3D = Rect3D.Empty;
        if (this.ViewObject != null)
        {
          GeometryModel3D geometryModel3D = this.ViewObject.PlatformSpecificObject as GeometryModel3D;
          if (geometryModel3D != null && geometryModel3D.Geometry != null)
            rect3D = geometryModel3D.Geometry.Bounds;
        }
        return rect3D;
      }
    }

    public class ConcreteGeometryModel3DElementFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new GeometryModel3DElement();
      }
    }
  }
}
