// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.TransformGroupNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class TransformGroupNode : TransformNode
  {
    public static readonly IPropertyId ChildrenProperty = (IPropertyId) PlatformTypes.TransformGroup.GetMember(MemberType.LocalProperty, "Children", MemberAccessTypes.Public);
    public static readonly TransformGroupNode.ConcreteTransformGroupNodeFactory Factory = new TransformGroupNode.ConcreteTransformGroupNodeFactory();

    public ISceneNodeCollection<SceneNode> Children
    {
      get
      {
        return this.GetCollectionForProperty(TransformGroupNode.ChildrenProperty);
      }
    }

    public bool IsCanonical
    {
      get
      {
        ISceneNodeCollection<SceneNode> children = this.Children;
        if (this.Children == null || this.Children.Count != CanonicalTransformOrder.TransformCount)
          return false;
        ScaleTransformNode scaleTransformNode = children[CanonicalTransformOrder.ScaleIndex] as ScaleTransformNode;
        SkewTransformNode skewTransformNode = children[CanonicalTransformOrder.SkewIndex] as SkewTransformNode;
        RotateTransformNode rotateTransformNode = children[CanonicalTransformOrder.RotateIndex] as RotateTransformNode;
        TranslateTransformNode translateTransformNode = children[CanonicalTransformOrder.TranslateIndex] as TranslateTransformNode;
        if (scaleTransformNode != null && skewTransformNode != null && (rotateTransformNode != null && translateTransformNode != null) && (skewTransformNode.CenterX == rotateTransformNode.CenterX && skewTransformNode.CenterY == rotateTransformNode.CenterY && scaleTransformNode.CenterX == rotateTransformNode.CenterX))
          return scaleTransformNode.CenterY == rotateTransformNode.CenterY;
        return false;
      }
    }

    public override bool CopyToCompositeTransform(CompositeTransformNode compositeTransform)
    {
      if (!this.Platform.Metadata.IsCapabilitySet(PlatformCapability.SupportsCompositeTransform))
        return false;
      ISceneNodeCollection<SceneNode> children = this.Children;
      if (children == null || children.Count == 0)
        return false;
      if (children.Count == 1)
        return ((TransformNode) children[0]).CopyToCompositeTransform(compositeTransform);
      if (!this.IsCanonical)
        return false;
      foreach (TransformNode transformNode in (IEnumerable<SceneNode>) children)
      {
        if (transformNode == null)
          return false;
        transformNode.CopyToCompositeTransform(compositeTransform);
      }
      return true;
    }

    public class ConcreteTransformGroupNodeFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new TransformGroupNode();
      }
    }
  }
}
