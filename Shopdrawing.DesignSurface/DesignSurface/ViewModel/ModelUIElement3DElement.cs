// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.ModelUIElement3DElement
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class ModelUIElement3DElement : UIElement3DElement
  {
    public static readonly IPropertyId ModelProperty = (IPropertyId) PlatformTypes.ModelUIElement3D.GetMember(MemberType.LocalProperty, "Model", MemberAccessTypes.Public);
    public static readonly ModelUIElement3DElement.ConcreteModelUIElement3DElementFactory Factory = new ModelUIElement3DElement.ConcreteModelUIElement3DElementFactory();

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
        Rect3D rect3D = Rect3D.Empty;
        Model3DElement model3Dmodel = this.Model3DModel;
        if (model3Dmodel != null)
          rect3D = Base3DElement.TransformAxisAligned(model3Dmodel.Transform.Value, model3Dmodel.LocalSpaceBounds);
        return rect3D;
      }
    }

    public SceneNode Model
    {
      get
      {
        return this.GetLocalValueAsSceneNode(ModelUIElement3DElement.ModelProperty);
      }
      set
      {
        this.SetValueAsSceneNode(ModelUIElement3DElement.ModelProperty, value);
      }
    }

    public Model3DElement Model3DModel
    {
      get
      {
        return this.Model as Model3DElement;
      }
    }

    public class ConcreteModelUIElement3DElementFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new ModelUIElement3DElement();
      }

      public ModelUIElement3DElement Instantiate(SceneViewModel viewModel)
      {
        return (ModelUIElement3DElement) this.Instantiate(viewModel, PlatformTypes.ModelUIElement3D);
      }
    }
  }
}
