// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.FromToAnimationSceneNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class FromToAnimationSceneNode : AnimationSceneNode
  {
    public static readonly FromToAnimationSceneNode.ConcreteFromToAnimationSceneNodeFactory Factory = new FromToAnimationSceneNode.ConcreteFromToAnimationSceneNodeFactory();
    private static Dictionary<ITypeId, ITypeId> animatedTypes = new Dictionary<ITypeId, ITypeId>()
    {
      {
        PlatformTypes.ByteAnimation,
        PlatformTypes.Byte
      },
      {
        PlatformTypes.ColorAnimation,
        PlatformTypes.Color
      },
      {
        PlatformTypes.DecimalAnimation,
        PlatformTypes.Decimal
      },
      {
        PlatformTypes.DoubleAnimation,
        PlatformTypes.Double
      },
      {
        PlatformTypes.Int16Animation,
        PlatformTypes.Int16
      },
      {
        PlatformTypes.Int32Animation,
        PlatformTypes.Int32
      },
      {
        PlatformTypes.Int64Animation,
        PlatformTypes.Int64
      },
      {
        PlatformTypes.Point3DAnimation,
        PlatformTypes.Point3D
      },
      {
        PlatformTypes.PointAnimation,
        PlatformTypes.Point
      },
      {
        PlatformTypes.QuaternionAnimation,
        PlatformTypes.Quaternion
      },
      {
        PlatformTypes.Rotation3DAnimation,
        PlatformTypes.Rotation3D
      },
      {
        PlatformTypes.RectAnimation,
        PlatformTypes.Rect
      },
      {
        PlatformTypes.SingleAnimation,
        PlatformTypes.Single
      },
      {
        PlatformTypes.SizeAnimation,
        PlatformTypes.Size
      },
      {
        PlatformTypes.ThicknessAnimation,
        PlatformTypes.Thickness
      },
      {
        PlatformTypes.Vector3DAnimation,
        PlatformTypes.Vector3D
      },
      {
        PlatformTypes.VectorAnimation,
        PlatformTypes.Vector
      }
    };
    private static Dictionary<ITypeId, ITypeId> animationsForTypes = new Dictionary<ITypeId, ITypeId>();

    public override ITypeId AnimatedType
    {
      get
      {
        ITypeId typeId = (ITypeId) null;
        AnimationSceneNode.PlatformNeutralTypeDictionaryTryGetValue(FromToAnimationSceneNode.animatedTypes, (ITypeId) this.Type, out typeId);
        return typeId;
      }
    }

    public object From
    {
      get
      {
        return this.GetLocalValue(this.FromProperty);
      }
      set
      {
        this.SetValue(this.FromProperty, value);
      }
    }

    public object To
    {
      get
      {
        return this.GetLocalValue(this.ToProperty);
      }
      set
      {
        this.SetValue(this.ToProperty, value);
      }
    }

    public IPropertyId FromProperty
    {
      get
      {
        return (IPropertyId) this.Type.GetMember(MemberType.LocalProperty, "From", MemberAccessTypes.Public);
      }
    }

    public IPropertyId ToProperty
    {
      get
      {
        return (IPropertyId) this.Type.GetMember(MemberType.LocalProperty, "To", MemberAccessTypes.Public);
      }
    }

    public IPropertyId EasingFunctionProperty
    {
      get
      {
        return (IPropertyId) this.Type.GetMember(MemberType.LocalProperty, "EasingFunction", MemberAccessTypes.Public);
      }
    }

    public bool IsOptimized
    {
      get
      {
        return (bool) this.GetLocalOrDefaultValue(DesignTimeProperties.IsOptimizedProperty);
      }
      set
      {
        if (value)
          this.SetLocalValue(DesignTimeProperties.IsOptimizedProperty, (object) true);
        else
          this.ClearLocalValue(DesignTimeProperties.IsOptimizedProperty);
      }
    }

    public override double NaturalDuration
    {
      get
      {
        return this.Duration;
      }
    }

    public IEasingFunctionDefinition EasingFunction
    {
      get
      {
        if (this.IsEasingFunctionAvailable)
          return this.Platform.ViewObjectFactory.Instantiate(this.GetLocalValue(this.EasingFunctionProperty)) as IEasingFunctionDefinition;
        return (IEasingFunctionDefinition) null;
      }
      set
      {
        if (!this.IsEasingFunctionAvailable)
          return;
        if (value != null)
          this.SetLocalValue(this.EasingFunctionProperty, value.PlatformSpecificObject);
        else
          this.ClearLocalValue(this.EasingFunctionProperty);
      }
    }

    private bool IsEasingFunctionAvailable
    {
      get
      {
        return this.ProjectContext.ResolveProperty(this.EasingFunctionProperty) != null;
      }
    }

    static FromToAnimationSceneNode()
    {
      foreach (ITypeId index in FromToAnimationSceneNode.animatedTypes.Keys)
        FromToAnimationSceneNode.animationsForTypes[FromToAnimationSceneNode.animatedTypes[index]] = index;
    }

    public static ITypeId GetFromToAnimationForType(ITypeId type, IProjectContext projectContext)
    {
      ITypeId type1 = (ITypeId) null;
      bool flag = AnimationSceneNode.PlatformNeutralTypeDictionaryTryGetValue(FromToAnimationSceneNode.animationsForTypes, type, out type1);
      if (flag)
        flag = projectContext.Platform.Metadata.IsSupported((ITypeResolver) projectContext, type1);
      if (!flag)
        type1 = (ITypeId) null;
      return type1;
    }

    protected override void ModifyValue(PropertyReference propertyReference, object valueToSet, SceneNode.Modification modification, int index)
    {
      if (!this.ShouldSerialize && modification == SceneNode.Modification.SetValue)
      {
        ReferenceStep lastStep = propertyReference.LastStep;
        if (modification == SceneNode.Modification.SetValue)
        {
          this.ShouldSerialize = true;
          StoryboardTimelineSceneNode timelineSceneNode = this.Parent as StoryboardTimelineSceneNode;
          if (timelineSceneNode != null)
            timelineSceneNode.ShouldSerialize = true;
        }
      }
      base.ModifyValue(propertyReference, valueToSet, modification, index);
    }

    public class ConcreteFromToAnimationSceneNodeFactory : AnimationSceneNode.ConcreteAnimationSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new FromToAnimationSceneNode();
      }

      public FromToAnimationSceneNode InstantiateWithTarget(SceneViewModel viewModel, SceneNode targetElement, PropertyReference targetProperty, IStoryboardContainer referenceStoryboardContainer, ITypeId type)
      {
        return (FromToAnimationSceneNode) base.InstantiateWithTarget(viewModel, targetElement, targetProperty, referenceStoryboardContainer, type);
      }
    }
  }
}
