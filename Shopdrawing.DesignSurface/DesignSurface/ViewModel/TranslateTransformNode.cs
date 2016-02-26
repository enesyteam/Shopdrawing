// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.TranslateTransformNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class TranslateTransformNode : TransformNode
  {
    public static readonly IPropertyId XProperty = (IPropertyId) PlatformTypes.TranslateTransform.GetMember(MemberType.LocalProperty, "X", MemberAccessTypes.Public);
    public static readonly IPropertyId YProperty = (IPropertyId) PlatformTypes.TranslateTransform.GetMember(MemberType.LocalProperty, "Y", MemberAccessTypes.Public);
    public static readonly TranslateTransformNode.ConcreteTranslateTransformNodeFactory Factory = new TranslateTransformNode.ConcreteTranslateTransformNodeFactory();

    public double X
    {
      get
      {
        return (double) this.GetComputedValue(TranslateTransformNode.XProperty);
      }
      set
      {
        this.SetValue(TranslateTransformNode.XProperty, (object) value);
      }
    }

    public double Y
    {
      get
      {
        return (double) this.GetComputedValue(TranslateTransformNode.YProperty);
      }
      set
      {
        this.SetValue(TranslateTransformNode.YProperty, (object) value);
      }
    }

    public override bool CopyToCompositeTransform(CompositeTransformNode compositeTransform)
    {
      if (!this.Platform.Metadata.IsCapabilitySet(PlatformCapability.SupportsCompositeTransform))
        return false;
      SceneNode.CopyPropertyValue((SceneNode) this, TranslateTransformNode.XProperty, (SceneNode) compositeTransform, CompositeTransformNode.TranslateXProperty);
      SceneNode.CopyPropertyValue((SceneNode) this, TranslateTransformNode.YProperty, (SceneNode) compositeTransform, CompositeTransformNode.TranslateYProperty);
      return true;
    }

    public class ConcreteTranslateTransformNodeFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new TranslateTransformNode();
      }
    }
  }
}
