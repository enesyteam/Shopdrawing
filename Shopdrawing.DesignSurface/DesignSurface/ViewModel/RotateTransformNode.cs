// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.RotateTransformNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class RotateTransformNode : TransformNode
  {
    public static readonly IPropertyId AngleProperty = (IPropertyId) PlatformTypes.RotateTransform.GetMember(MemberType.LocalProperty, "Angle", MemberAccessTypes.Public);
    public static readonly IPropertyId CenterXProperty = (IPropertyId) PlatformTypes.RotateTransform.GetMember(MemberType.LocalProperty, "CenterX", MemberAccessTypes.Public);
    public static readonly IPropertyId CenterYProperty = (IPropertyId) PlatformTypes.RotateTransform.GetMember(MemberType.LocalProperty, "CenterY", MemberAccessTypes.Public);
    public static readonly RotateTransformNode.ConcreteRotateTransformNodeFactory Factory = new RotateTransformNode.ConcreteRotateTransformNodeFactory();

    public double Angle
    {
      get
      {
        return (double) this.GetComputedValue(RotateTransformNode.AngleProperty);
      }
      set
      {
        this.SetValue(RotateTransformNode.AngleProperty, (object) value);
      }
    }

    public double CenterX
    {
      get
      {
        return (double) this.GetComputedValue(RotateTransformNode.CenterXProperty);
      }
      set
      {
        this.SetValue(RotateTransformNode.CenterXProperty, (object) value);
      }
    }

    public double CenterY
    {
      get
      {
        return (double) this.GetComputedValue(RotateTransformNode.CenterYProperty);
      }
      set
      {
        this.SetValue(RotateTransformNode.CenterYProperty, (object) value);
      }
    }

    public override bool CopyToCompositeTransform(CompositeTransformNode compositeTransform)
    {
      if (!this.Platform.Metadata.IsCapabilitySet(PlatformCapability.SupportsCompositeTransform))
        return false;
      SceneNode.CopyPropertyValue((SceneNode) this, RotateTransformNode.CenterXProperty, (SceneNode) compositeTransform, CompositeTransformNode.CenterXProperty);
      SceneNode.CopyPropertyValue((SceneNode) this, RotateTransformNode.CenterYProperty, (SceneNode) compositeTransform, CompositeTransformNode.CenterYProperty);
      SceneNode.CopyPropertyValue((SceneNode) this, RotateTransformNode.AngleProperty, (SceneNode) compositeTransform, CompositeTransformNode.RotationProperty);
      return true;
    }

    public class ConcreteRotateTransformNodeFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new RotateTransformNode();
      }
    }
  }
}
