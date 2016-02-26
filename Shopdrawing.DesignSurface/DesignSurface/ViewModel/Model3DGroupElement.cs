// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.Model3DGroupElement
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public sealed class Model3DGroupElement : Model3DElement, IChildContainer3D
  {
    public static readonly IPropertyId ChildrenProperty = (IPropertyId) PlatformTypes.Model3DGroup.GetMember(MemberType.LocalProperty, "Children", MemberAccessTypes.Public);
    public static readonly Model3DGroupElement.ConcreteModel3DGroupElementFactory Factory = new Model3DGroupElement.ConcreteModel3DGroupElementFactory();

    public override bool IsContainer
    {
      get
      {
        return true;
      }
    }

    public ISceneNodeCollection<Model3DElement> Children
    {
      get
      {
        return (ISceneNodeCollection<Model3DElement>) new SceneNode.SceneNodeCollection<Model3DElement>((SceneNode) this, Model3DGroupElement.ChildrenProperty);
      }
    }

    public override Rect3D LocalSpaceBounds
    {
      get
      {
        Rect3D empty = Rect3D.Empty;
        foreach (Model3DElement model3Delement in (IEnumerable<Model3DElement>) this.Children)
          empty.Union(Base3DElement.TransformAxisAligned(model3Delement.Transform.Value, model3Delement.LocalSpaceBounds));
        return empty;
      }
    }

    public SceneElement AddChild(SceneViewModel sceneView, Base3DElement child)
    {
      Model3DElement model3Delement = BaseElement3DCoercionHelper.CoerceToModel3D(sceneView, (SceneElement) child);
      if (model3Delement != null)
        this.Children.Add(model3Delement);
      return (SceneElement) model3Delement;
    }

    public static bool GetIsGroup(SceneNode node)
    {
      Model3DGroupElement model3DgroupElement = node as Model3DGroupElement;
      if (model3DgroupElement != null)
      {
        if (model3DgroupElement.ParentElement is ModelVisual3DElement)
          return model3DgroupElement.Children.Count == 1;
        if (model3DgroupElement.ParentElement is Model3DGroupElement)
          return model3DgroupElement.Children.Count > 0;
      }
      return false;
    }

    public class ConcreteModel3DGroupElementFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new Model3DGroupElement();
      }
    }
  }
}
