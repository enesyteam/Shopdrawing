// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.Viewport3DElement
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public sealed class Viewport3DElement : BaseFrameworkElement, IChildContainer3D
  {
    public static readonly IPropertyId CameraProperty = (IPropertyId) PlatformTypes.Viewport3D.GetMember(MemberType.LocalProperty, "Camera", MemberAccessTypes.Public);
    public static readonly IPropertyId ChildrenProperty = (IPropertyId) PlatformTypes.Viewport3D.GetMember(MemberType.LocalProperty, "Children", MemberAccessTypes.Public);
    public static readonly IPropertyId ClipToBoundsProperty = (IPropertyId) PlatformTypes.Viewport3D.GetMember(MemberType.LocalProperty, "ClipToBounds", MemberAccessTypes.Public);
    public static readonly Viewport3DElement.ConcreteViewport3DElementFactory Factory = new Viewport3DElement.ConcreteViewport3DElementFactory();

    public CameraElement Camera
    {
      get
      {
        return this.GetLocalValueAsSceneNode(Viewport3DElement.CameraProperty) as CameraElement;
      }
      set
      {
        this.SetValueAsSceneNode(Viewport3DElement.CameraProperty, (SceneNode) value);
      }
    }

    public ISceneNodeCollection<Visual3DElement> Children
    {
      get
      {
        return (ISceneNodeCollection<Visual3DElement>) new SceneNode.SceneNodeCollection<Visual3DElement>((SceneNode) this, Viewport3DElement.ChildrenProperty);
      }
    }

    public SceneElement AddChild(SceneViewModel sceneView, Base3DElement child)
    {
      Visual3DElement visual3Delement = BaseElement3DCoercionHelper.CoerceToVisual3D(sceneView, (SceneElement) child);
      if (visual3Delement != null)
        this.Children.Add(visual3Delement);
      return (SceneElement) visual3Delement;
    }

    public class ConcreteViewport3DElementFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new Viewport3DElement();
      }
    }
  }
}
