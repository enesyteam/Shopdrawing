// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.PathAnimationSceneNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class PathAnimationSceneNode : AnimationSceneNode
  {
    public static readonly IPropertyId PathGeometryProperty = (IPropertyId) PlatformTypes.DoubleAnimationUsingPath.GetMember(MemberType.LocalProperty, "PathGeometry", MemberAccessTypes.Public);
    public static readonly IPropertyId SourceProperty = (IPropertyId) PlatformTypes.DoubleAnimationUsingPath.GetMember(MemberType.LocalProperty, "Source", MemberAccessTypes.Public);
    public static readonly PathAnimationSceneNode.ConcretePathAnimationSceneNodeFactory Factory = new PathAnimationSceneNode.ConcretePathAnimationSceneNodeFactory();
    private static Dictionary<ITypeId, ITypeId> animatedTypes = new Dictionary<ITypeId, ITypeId>();
    private static Dictionary<ITypeId, ITypeId> animationsForTypes;

    public PathGeometry Path
    {
      get
      {
        return this.GetLocalValue(PathAnimationSceneNode.PathGeometryProperty) as PathGeometry;
      }
      set
      {
        this.SetValue(PathAnimationSceneNode.PathGeometryProperty, (object) value);
      }
    }

    public PathAnimationSource Source
    {
      get
      {
        PathAnimationSource? nullable = this.GetLocalValue(PathAnimationSceneNode.SourceProperty) as PathAnimationSource?;
        if (!nullable.HasValue)
          return (PathAnimationSource) this.GetDefaultValue(PathAnimationSceneNode.SourceProperty);
        return nullable.Value;
      }
      set
      {
        this.SetValue(PathAnimationSceneNode.SourceProperty, (object) value);
      }
    }

    public override bool IsDiscreteOnly
    {
      get
      {
        return false;
      }
    }

    public override double NaturalDuration
    {
      get
      {
        return this.Duration;
      }
    }

    public override ITypeId AnimatedType
    {
      get
      {
        ITypeId typeId;
        AnimationSceneNode.PlatformNeutralTypeDictionaryTryGetValue(PathAnimationSceneNode.animatedTypes, (ITypeId) this.Type, out typeId);
        return typeId;
      }
    }

    static PathAnimationSceneNode()
    {
      PathAnimationSceneNode.animatedTypes[PlatformTypes.DoubleAnimationUsingPath] = PlatformTypes.Double;
      PathAnimationSceneNode.animatedTypes[PlatformTypes.MatrixAnimationUsingPath] = PlatformTypes.Matrix;
      PathAnimationSceneNode.animatedTypes[PlatformTypes.PointAnimationUsingPath] = PlatformTypes.Point;
      PathAnimationSceneNode.animationsForTypes = new Dictionary<ITypeId, ITypeId>();
      foreach (ITypeId index in PathAnimationSceneNode.animatedTypes.Keys)
        PathAnimationSceneNode.animationsForTypes[PathAnimationSceneNode.animatedTypes[index]] = index;
    }

    public static ITypeId GetPathAnimationForType(ITypeId type)
    {
      ITypeId typeId;
      AnimationSceneNode.PlatformNeutralTypeDictionaryTryGetValue(PathAnimationSceneNode.animationsForTypes, type, out typeId);
      return typeId;
    }

    public static bool CanAnimateType(ITypeId type)
    {
      return PathAnimationSceneNode.animationsForTypes.ContainsKey(type);
    }

    public class ConcretePathAnimationSceneNodeFactory : AnimationSceneNode.ConcreteAnimationSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new PathAnimationSceneNode();
      }

      public PathAnimationSceneNode InstantiateWithTarget(SceneViewModel viewModel, SceneNode targetElement, PropertyReference targetProperty, IStoryboardContainer referenceStoryboardContainer, ITypeId type)
      {
        return (PathAnimationSceneNode) base.InstantiateWithTarget(viewModel, targetElement, targetProperty, referenceStoryboardContainer, type);
      }
    }
  }
}
