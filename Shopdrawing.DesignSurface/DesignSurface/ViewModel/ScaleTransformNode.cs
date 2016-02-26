// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.ScaleTransformNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class ScaleTransformNode : TransformNode
  {
    public static readonly IPropertyId ScaleXProperty = (IPropertyId) PlatformTypes.ScaleTransform.GetMember(MemberType.LocalProperty, "ScaleX", MemberAccessTypes.Public);
    public static readonly IPropertyId ScaleYProperty = (IPropertyId) PlatformTypes.ScaleTransform.GetMember(MemberType.LocalProperty, "ScaleY", MemberAccessTypes.Public);
    public static readonly IPropertyId CenterXProperty = (IPropertyId) PlatformTypes.ScaleTransform.GetMember(MemberType.LocalProperty, "CenterX", MemberAccessTypes.Public);
    public static readonly IPropertyId CenterYProperty = (IPropertyId) PlatformTypes.ScaleTransform.GetMember(MemberType.LocalProperty, "CenterY", MemberAccessTypes.Public);
    public static readonly ScaleTransformNode.ConcreteScaleTransformNodeFactory Factory = new ScaleTransformNode.ConcreteScaleTransformNodeFactory();

    public double ScaleX
    {
      get
      {
        return (double) this.GetComputedValue(ScaleTransformNode.ScaleXProperty);
      }
      set
      {
        this.SetValue(ScaleTransformNode.ScaleXProperty, (object) value);
      }
    }

    public double ScaleY
    {
      get
      {
        return (double) this.GetComputedValue(ScaleTransformNode.ScaleYProperty);
      }
      set
      {
        this.SetValue(ScaleTransformNode.ScaleYProperty, (object) value);
      }
    }

    public double CenterX
    {
      get
      {
        return (double) this.GetComputedValue(ScaleTransformNode.CenterXProperty);
      }
      set
      {
        this.SetValue(ScaleTransformNode.CenterXProperty, (object) value);
      }
    }

    public double CenterY
    {
      get
      {
        return (double) this.GetComputedValue(ScaleTransformNode.CenterYProperty);
      }
      set
      {
        this.SetValue(ScaleTransformNode.CenterYProperty, (object) value);
      }
    }

    public override bool CopyToCompositeTransform(CompositeTransformNode compositeTransform)
    {
      if (!this.Platform.Metadata.IsCapabilitySet(PlatformCapability.SupportsCompositeTransform))
        return false;
      SceneNode.CopyPropertyValue((SceneNode) this, ScaleTransformNode.CenterXProperty, (SceneNode) compositeTransform, CompositeTransformNode.CenterXProperty);
      SceneNode.CopyPropertyValue((SceneNode) this, ScaleTransformNode.CenterYProperty, (SceneNode) compositeTransform, CompositeTransformNode.CenterYProperty);
      SceneNode.CopyPropertyValue((SceneNode) this, ScaleTransformNode.ScaleXProperty, (SceneNode) compositeTransform, CompositeTransformNode.ScaleXProperty);
      SceneNode.CopyPropertyValue((SceneNode) this, ScaleTransformNode.ScaleYProperty, (SceneNode) compositeTransform, CompositeTransformNode.ScaleYProperty);
      return true;
    }

    public class ConcreteScaleTransformNodeFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new ScaleTransformNode();
      }
    }
  }
}
