// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.Model3DElement
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public abstract class Model3DElement : Base3DElement
  {
    public static readonly IPropertyId TransformProperty = (IPropertyId) PlatformTypes.Model3D.GetMember(MemberType.LocalProperty, "Transform", MemberAccessTypes.Public);

    public override Rect3D LocalSpaceBounds
    {
      get
      {
        return Rect3D.Empty;
      }
    }

    public override Transform3D Transform
    {
      get
      {
        return (Transform3D) this.GetComputedValue(Model3DElement.TransformProperty);
      }
      set
      {
        this.SetValue(Model3DElement.TransformProperty, (object) value);
      }
    }

    public override IPropertyId TransformPropertyId
    {
      get
      {
        return Model3DElement.TransformProperty;
      }
    }

    public Rect3D DesignTimeBounds
    {
      get
      {
        return (Rect3D) this.GetLocalOrDefaultValue(DesignTimeProperties.BoundsProperty);
      }
      set
      {
        this.SetLocalValue(DesignTimeProperties.BoundsProperty, (object) value);
      }
    }

    protected override object GetComputedValueInternal(PropertyReference propertyReference)
    {
      SceneElement sceneElement = (SceneElement) this;
      Visual3DElement visual3Delement;
      do
      {
        sceneElement = sceneElement.ParentElement;
        visual3Delement = sceneElement as Visual3DElement;
      }
      while (visual3Delement == null && sceneElement != null);
      PropertyReference propertyReference1 = Model3DElement.PropertyReferenceFromVisual3D((Base3DElement) this, propertyReference);
      if (propertyReference1 != null && visual3Delement != null && this.IsViewObjectValid)
        return visual3Delement.GetComputedValue(propertyReference1);
      return base.GetComputedValueInternal(propertyReference);
    }

    public static PropertyReference PropertyReferenceFromVisual3D(Base3DElement item, PropertyReference propertyReference)
    {
      for (Base3DElement base3Delement = item.ParentElement as Base3DElement; base3Delement != null; base3Delement = item.ParentElement as Base3DElement)
      {
        if (base3Delement is ModelVisual3DElement)
        {
          if (item is Model3DElement)
          {
            propertyReference = new PropertyReference((ReferenceStep) item.ProjectContext.ResolveProperty(ModelVisual3DElement.ContentProperty)).Append(propertyReference);
            break;
          }
          break;
        }
        if (base3Delement is ModelUIElement3DElement)
        {
          if (item is Model3DElement)
          {
            propertyReference = new PropertyReference((ReferenceStep) item.ProjectContext.ResolveProperty(ModelUIElement3DElement.ModelProperty)).Append(propertyReference);
            break;
          }
          break;
        }
        int siteChildIndex = item.DocumentNode.SiteChildIndex;
        PropertyReference propertyReference1 = new PropertyReference((ReferenceStep) IndexedClrPropertyReferenceStep.GetReferenceStep((ITypeResolver) item.ProjectContext, typeof (Model3DCollection), siteChildIndex));
        propertyReference = new PropertyReference((ReferenceStep) item.ProjectContext.ResolveProperty(Model3DGroupElement.ChildrenProperty)).Append(propertyReference1).Append(propertyReference);
        item = base3Delement;
      }
      return propertyReference;
    }
  }
}
