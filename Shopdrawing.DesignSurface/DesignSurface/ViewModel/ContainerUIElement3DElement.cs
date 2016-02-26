// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.ContainerUIElement3DElement
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class ContainerUIElement3DElement : UIElement3DElement
  {
    public static readonly IPropertyId ChildrenProperty = (IPropertyId) PlatformTypes.ContainerUIElement3D.GetMember(MemberType.LocalProperty, "Children", MemberAccessTypes.Public);
    public static readonly ContainerUIElement3DElement.ConcreteContainerUIElement3DElementFactory Factory = new ContainerUIElement3DElement.ConcreteContainerUIElement3DElementFactory();

    public override bool IsContainer
    {
      get
      {
        return true;
      }
    }

    public override Rect3D LocalSpaceBounds
    {
      get
      {
        Rect3D empty = Rect3D.Empty;
        foreach (Visual3DElement visual3Delement in (IEnumerable<Visual3DElement>) this.Children)
          empty.Union(Base3DElement.TransformAxisAligned(visual3Delement.Transform.Value, visual3Delement.LocalSpaceBounds));
        return empty;
      }
    }

    public ISceneNodeCollection<Visual3DElement> Children
    {
      get
      {
        return (ISceneNodeCollection<Visual3DElement>) new SceneNode.SceneNodeCollection<Visual3DElement>((SceneNode) this, ContainerUIElement3DElement.ChildrenProperty);
      }
    }

    public class ConcreteContainerUIElement3DElementFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new ContainerUIElement3DElement();
      }

      public ContainerUIElement3DElement Instantiate(SceneViewModel viewModel)
      {
        return (ContainerUIElement3DElement) this.Instantiate(viewModel, PlatformTypes.ContainerUIElement3D);
      }
    }
  }
}
