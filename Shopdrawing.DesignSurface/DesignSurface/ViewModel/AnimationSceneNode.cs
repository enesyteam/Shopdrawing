// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.AnimationSceneNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class AnimationSceneNode : TimelineSceneNode
  {
    public static readonly AnimationSceneNode.ConcreteAnimationSceneNodeFactory Factory = new AnimationSceneNode.ConcreteAnimationSceneNodeFactory();

    public virtual bool IsDiscreteOnly
    {
      get
      {
        return false;
      }
    }

    public virtual ITypeId AnimatedType
    {
      get
      {
        return (ITypeId) null;
      }
    }

    public bool IsInStateAnimation
    {
      get
      {
        return this.ControllingState != null;
      }
    }

    public bool IsInTransitionAnimation
    {
      get
      {
        return this.ControllingTransition != null;
      }
    }

    public virtual bool ChangeCanAffectTransition(SceneNode node, PropertyReference changedProperty)
    {
      return false;
    }

    public int CompareTo(object obj)
    {
      SceneNode y = obj as SceneNode;
      if (y != null)
        return SceneNode.MarkerCompare((SceneNode) this, y);
      return 1;
    }

    protected static bool PlatformNeutralTypeDictionaryTryGetValue(Dictionary<ITypeId, ITypeId> dictionary, ITypeId key, out ITypeId value)
    {
      foreach (ITypeId index in dictionary.Keys)
      {
        if (key.Equals((object) index))
        {
          value = dictionary[index];
          return true;
        }
      }
      value = (ITypeId) null;
      return false;
    }

    public class ConcreteAnimationSceneNodeFactory : TimelineSceneNode.ConcreteTimelineSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new AnimationSceneNode();
      }

      public AnimationSceneNode InstantiateWithTarget(SceneViewModel viewModel, SceneNode targetElement, PropertyReference targetProperty, IStoryboardContainer referenceStoryboardContainer, ITypeId type)
      {
        return (AnimationSceneNode) base.InstantiateWithTarget(viewModel, targetElement, targetProperty, referenceStoryboardContainer, type);
      }
    }
  }
}
