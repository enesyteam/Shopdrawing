// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.ModelVisual3DElement
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class ModelVisual3DElement : Visual3DElement, IChildContainer3D
  {
    public static readonly IPropertyId ChildrenProperty = (IPropertyId) PlatformTypes.ModelVisual3D.GetMember(MemberType.LocalProperty, "Children", MemberAccessTypes.Public);
    public static readonly IPropertyId ContentProperty = (IPropertyId) PlatformTypes.ModelVisual3D.GetMember(MemberType.LocalProperty, "Content", MemberAccessTypes.Public);
    public new static readonly IPropertyId TransformProperty = (IPropertyId) PlatformTypes.ModelVisual3D.GetMember(MemberType.LocalProperty, "Transform", MemberAccessTypes.Public);
    public static readonly ModelVisual3DElement.ConcreteModelVisual3DElementFactory Factory = new ModelVisual3DElement.ConcreteModelVisual3DElementFactory();

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
        Model3DElement model3Dcontent = this.Model3DContent;
        if (model3Dcontent != null)
          empty.Union(Base3DElement.TransformAxisAligned(model3Dcontent.Transform.Value, model3Dcontent.LocalSpaceBounds));
        return empty;
      }
    }

    public Rect3D DesignTimeBounds
    {
      get
      {
        Rect3D empty = Rect3D.Empty;
        foreach (ModelVisual3DElement modelVisual3Delement in (IEnumerable<Visual3DElement>) this.Children)
          empty.Union(modelVisual3Delement.DesignTimeBounds);
        if (this.Model3DContent != null)
          empty.Union(this.Model3DContent.DesignTimeBounds);
        return empty;
      }
    }

    public SceneNode Content
    {
      get
      {
        return this.GetLocalValueAsSceneNode(ModelVisual3DElement.ContentProperty);
      }
      set
      {
        this.SetValueAsSceneNode(ModelVisual3DElement.ContentProperty, value);
      }
    }

    public Model3DElement Model3DContent
    {
      get
      {
        return this.Content as Model3DElement;
      }
    }

    public ISceneNodeCollection<Visual3DElement> Children
    {
      get
      {
        return (ISceneNodeCollection<Visual3DElement>) new SceneNode.SceneNodeCollection<Visual3DElement>((SceneNode) this, ModelVisual3DElement.ChildrenProperty);
      }
    }

    public override Transform3D Transform
    {
      get
      {
        return (Transform3D) this.GetComputedValue(ModelVisual3DElement.TransformProperty);
      }
      set
      {
        this.SetValue(ModelVisual3DElement.TransformProperty, (object) value);
      }
    }

    public override IPropertyId TransformPropertyId
    {
      get
      {
        return ModelVisual3DElement.TransformProperty;
      }
    }

    public SceneElement AddChild(SceneViewModel sceneView, Base3DElement child)
    {
      Visual3DElement visual3Delement = BaseElement3DCoercionHelper.CoerceToVisual3D(sceneView, (SceneElement) child);
      if (visual3Delement != null)
        this.Children.Add(visual3Delement);
      return (SceneElement) visual3Delement;
    }

    public static bool GetIsGroup(SceneElement element)
    {
      ModelVisual3DElement modelVisual3Delement = element as ModelVisual3DElement;
      if (modelVisual3Delement != null)
      {
        if (modelVisual3Delement.ParentElement is Viewport3DElement)
        {
          if (modelVisual3Delement.Children.Count > 0)
            return modelVisual3Delement.Content == null;
          return false;
        }
        if (modelVisual3Delement.ParentElement is ModelVisual3DElement && modelVisual3Delement.Children.Count > 0)
        {
          if (modelVisual3Delement.Content != null)
            return modelVisual3Delement.Model3DContent != null;
          return true;
        }
      }
      return false;
    }

    public class ConcreteModelVisual3DElementFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new ModelVisual3DElement();
      }

      public ModelVisual3DElement Instantiate(SceneViewModel viewModel)
      {
        return (ModelVisual3DElement) this.Instantiate(viewModel, PlatformTypes.ModelVisual3D);
      }
    }
  }
}
