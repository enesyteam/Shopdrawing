// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.VisualStateTransitionSceneNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface.Properties;
using System.Collections.Generic;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class VisualStateTransitionSceneNode : SceneNode
  {
    public static readonly IPropertyId FromStateNameProperty = (IPropertyId) ProjectNeutralTypes.VisualTransition.GetMember(MemberType.LocalProperty, "From", MemberAccessTypes.Public);
    public static readonly IPropertyId ToStateNameProperty = (IPropertyId) ProjectNeutralTypes.VisualTransition.GetMember(MemberType.LocalProperty, "To", MemberAccessTypes.Public);
    public static readonly IPropertyId GeneratedDurationProperty = (IPropertyId) ProjectNeutralTypes.VisualTransition.GetMember(MemberType.LocalProperty, "GeneratedDuration", MemberAccessTypes.Public);
    public static readonly IPropertyId GeneratedEasingFunctionProperty = (IPropertyId) ProjectNeutralTypes.VisualTransition.GetMember(MemberType.LocalProperty, "GeneratedEasingFunction", MemberAccessTypes.Public);
    public static readonly IPropertyId StoryboardProperty = (IPropertyId) ProjectNeutralTypes.VisualTransition.GetMember(MemberType.LocalProperty, "Storyboard", MemberAccessTypes.Public);
    public static readonly VisualStateTransitionSceneNode.ConcreteVisualStateTransitionSceneNodeFactory Factory = new VisualStateTransitionSceneNode.ConcreteVisualStateTransitionSceneNodeFactory();

    public VisualStateGroupSceneNode StateGroup
    {
      get
      {
        return this.Parent as VisualStateGroupSceneNode;
      }
    }

    public string FromStateName
    {
      get
      {
        return this.GetLocalValue(VisualStateTransitionSceneNode.FromStateNameProperty) as string;
      }
      set
      {
        this.SetValue(VisualStateTransitionSceneNode.FromStateNameProperty, (object) value);
      }
    }

    public string ToStateName
    {
      get
      {
        return this.GetLocalValue(VisualStateTransitionSceneNode.ToStateNameProperty) as string;
      }
      set
      {
        this.SetValue(VisualStateTransitionSceneNode.ToStateNameProperty, (object) value);
      }
    }

    public VisualStateSceneNode FromState
    {
      get
      {
        if (this.StateGroup == null)
          return (VisualStateSceneNode) null;
        return this.StateGroup.FindStateByName(this.FromStateName);
      }
    }

    public VisualStateSceneNode ToState
    {
      get
      {
        if (this.StateGroup == null)
          return (VisualStateSceneNode) null;
        return this.StateGroup.FindStateByName(this.ToStateName);
      }
    }

    public Duration GeneratedDuration
    {
      get
      {
        Duration? nullable = this.GetLocalOrDefaultValueAsWpf(VisualStateTransitionSceneNode.GeneratedDurationProperty) as Duration?;
        if (!nullable.HasValue)
          return new Duration();
        return nullable.Value;
      }
      set
      {
        this.SetValueAsWpf(VisualStateTransitionSceneNode.GeneratedDurationProperty, (object) value);
      }
    }

    public StoryboardTimelineSceneNode Storyboard
    {
      get
      {
        StoryboardTimelineSceneNode timelineSceneNode = this.GetLocalValueAsSceneNode(VisualStateTransitionSceneNode.StoryboardProperty) as StoryboardTimelineSceneNode;
        if (timelineSceneNode == null || !timelineSceneNode.CanAccessProperties)
          return (StoryboardTimelineSceneNode) null;
        return timelineSceneNode;
      }
      set
      {
        this.SetValueAsSceneNode(VisualStateTransitionSceneNode.StoryboardProperty, (SceneNode) value);
      }
    }

    public override bool ShouldSerialize
    {
      get
      {
        return base.ShouldSerialize;
      }
      set
      {
        base.ShouldSerialize = value;
        if (!value || this.StateGroup == null || this.StateGroup.ShouldSerialize)
          return;
        this.StateGroup.ShouldSerialize = true;
      }
    }

    public IEasingFunctionDefinition GeneratedEasingFunction
    {
      get
      {
        if (this.ProjectContext.ResolveProperty(VisualStateTransitionSceneNode.GeneratedEasingFunctionProperty) != null)
          return this.Platform.ViewObjectFactory.Instantiate(this.GetLocalValue(VisualStateTransitionSceneNode.GeneratedEasingFunctionProperty)) as IEasingFunctionDefinition;
        return (IEasingFunctionDefinition) null;
      }
    }

    public void UpdateTransitionStoryboard(bool updateDocument, Dictionary<TimelineSceneNode.PropertyNodePair, TransitionAnimationData> transitionTable)
    {
      StoryboardTimelineSceneNode transitionStoryboard = this.Storyboard;
      bool flag1 = false;
      if (transitionStoryboard == null && updateDocument)
      {
        transitionStoryboard = StoryboardTimelineSceneNode.Factory.Instantiate(this.ViewModel);
        this.Storyboard = transitionStoryboard;
        flag1 = false;
      }
      VisualStateSceneNode fromState = this.FromState;
      VisualStateSceneNode toState = this.ToState;
      Dictionary<TimelineSceneNode.PropertyNodePair, TransitionAnimationData> animations = transitionTable;
      if (animations == null)
        animations = new Dictionary<TimelineSceneNode.PropertyNodePair, TransitionAnimationData>();
      else
        animations.Clear();
      if (transitionStoryboard != null)
      {
        foreach (TimelineSceneNode timelineSceneNode in (IEnumerable<TimelineSceneNode>) transitionStoryboard.Children)
        {
          AnimationSceneNode animation = timelineSceneNode as AnimationSceneNode;
          if (animation != null)
          {
            animations[animation.TargetElementAndProperty] = new TransitionAnimationData(animation, !animation.ShouldSerialize);
            if (animation.ShouldSerialize)
              flag1 = true;
          }
        }
      }
      bool flag2 = flag1 | this.UpdateTransitionTimelineForState(fromState, true, updateDocument, animations, transitionStoryboard) | this.UpdateTransitionTimelineForState(toState, false, updateDocument, animations, transitionStoryboard);
      if (!updateDocument)
        return;
      transitionStoryboard.ShouldSerialize = flag2;
    }

    private bool UpdateTransitionTimelineForState(VisualStateSceneNode state, bool stateIsFromState, bool updateDocument, Dictionary<TimelineSceneNode.PropertyNodePair, TransitionAnimationData> animations, StoryboardTimelineSceneNode transitionStoryboard)
    {
      bool flag1 = false;
      bool flag2 = false;
      IProperty property = this.ProjectContext.ResolveProperty(VisualStateManagerSceneNode.TransitionEffectProperty);
      if (property != null && this.GetLocalValueAsDocumentNode((IPropertyId) property) != null)
        flag2 = true;
      if (state != null && state.Storyboard != null)
      {
        foreach (TimelineSceneNode timelineSceneNode in (IEnumerable<TimelineSceneNode>) state.Storyboard.Children)
        {
          TransitionAnimationData transitionAnimationData = (TransitionAnimationData) null;
          AnimationSceneNode stateAnimation = timelineSceneNode as AnimationSceneNode;
          if (stateAnimation != null && stateAnimation.ShouldSerialize)
          {
            if (!animations.TryGetValue(stateAnimation.TargetElementAndProperty, out transitionAnimationData))
            {
              ITypeId animatedType = stateAnimation.AnimatedType;
              bool isConforming = VisualStateManagerSceneNode.IsSupportedTransitionAnimationType(animatedType);
              if (updateDocument)
              {
                TimelineSceneNode.PropertyNodePair elementAndProperty = stateAnimation.TargetElementAndProperty;
                if (isConforming && !flag2 && (elementAndProperty.SceneNode != null && elementAndProperty.PropertyReference != null))
                {
                  FromToAnimationSceneNode animationSceneNode1 = FromToAnimationSceneNode.Factory.InstantiateWithTarget(stateAnimation.ViewModel, elementAndProperty.SceneNode, elementAndProperty.PropertyReference, stateAnimation.StoryboardContainer, FromToAnimationSceneNode.GetFromToAnimationForType(animatedType, stateAnimation.ProjectContext));
                  animationSceneNode1.EasingFunction = this.GeneratedEasingFunction;
                  AnimationSceneNode animationSceneNode2 = (AnimationSceneNode) animationSceneNode1;
                  transitionStoryboard.Children.Add((TimelineSceneNode) animationSceneNode2);
                  animationSceneNode1.ShouldSerialize = false;
                  transitionAnimationData = new TransitionAnimationData((AnimationSceneNode) animationSceneNode1, true);
                  animations[stateAnimation.TargetElementAndProperty] = transitionAnimationData;
                }
                else
                {
                  transitionAnimationData = new TransitionAnimationData((AnimationSceneNode) null, false);
                  animations[stateAnimation.TargetElementAndProperty] = transitionAnimationData;
                }
              }
              else
              {
                transitionAnimationData = new TransitionAnimationData((AnimationSceneNode) null, isConforming);
                animations[stateAnimation.TargetElementAndProperty] = transitionAnimationData;
              }
            }
            else if (flag2 && transitionAnimationData.TransitionAnimation != null && !transitionAnimationData.TransitionAnimation.ShouldSerialize)
            {
              if (updateDocument)
                transitionStoryboard.Children.Remove((TimelineSceneNode) transitionAnimationData.TransitionAnimation);
              transitionAnimationData.TransitionAnimation = (AnimationSceneNode) null;
            }
            AnimationSceneNode transitionAnimation = transitionAnimationData.TransitionAnimation;
            if (transitionAnimation != null)
            {
              FromToAnimationSceneNode animationSceneNode = transitionAnimation as FromToAnimationSceneNode;
              if (animationSceneNode != null)
              {
                object objB = !stateIsFromState ? VisualStateTransitionSceneNode.GetTransitionValue(stateAnimation, stateIsFromState) : (object) null;
                if (!transitionAnimation.ShouldSerialize)
                {
                  if (updateDocument)
                  {
                    if (!stateIsFromState)
                      animationSceneNode.To = objB;
                    Duration generatedDuration = this.GeneratedDuration;
                    if (generatedDuration.HasTimeSpan)
                      animationSceneNode.Duration = generatedDuration.TimeSpan.TotalSeconds;
                    else
                      animationSceneNode.Duration = 0.0;
                    animationSceneNode.EasingFunction = this.GeneratedEasingFunction;
                    animationSceneNode.ShouldSerialize = false;
                  }
                }
                else
                {
                  transitionAnimationData.IsConforming = !stateIsFromState ? animationSceneNode.From == null && object.Equals(animationSceneNode.To, objB) : animationSceneNode.To == null;
                  flag1 = true;
                }
              }
              else
                flag1 = true;
            }
          }
        }
      }
      return flag1;
    }

    public StoryboardTimelineSceneNode BuildHandoffStoryboardNode()
    {
      if (this.ProjectContext.IsCapabilitySet(PlatformCapability.IsWpf))
        return (StoryboardTimelineSceneNode) null;
      VisualStateSceneNode fromState = this.FromState;
      StoryboardTimelineSceneNode timelineSceneNode1 = StoryboardTimelineSceneNode.Factory.Instantiate(this.ViewModel);
      Dictionary<TimelineSceneNode.PropertyNodePair, AnimationSceneNode> dictionary = new Dictionary<TimelineSceneNode.PropertyNodePair, AnimationSceneNode>();
      if (fromState != null && fromState.Storyboard != null)
      {
        foreach (TimelineSceneNode timelineSceneNode2 in (IEnumerable<TimelineSceneNode>) fromState.Storyboard.Children)
        {
          AnimationSceneNode animationSceneNode1 = timelineSceneNode2 as AnimationSceneNode;
          if (animationSceneNode1 != null)
          {
            TimelineSceneNode.PropertyNodePair elementAndProperty = animationSceneNode1.TargetElementAndProperty;
            if (this.ShouldGenerateHandoffAnimationFor(animationSceneNode1, elementAndProperty))
            {
              ITypeId animationForType = FromToAnimationSceneNode.GetFromToAnimationForType(animationSceneNode1.AnimatedType, animationSceneNode1.ProjectContext);
              AnimationSceneNode animationSceneNode2 = (AnimationSceneNode) null;
              if (animationForType != null)
              {
                FromToAnimationSceneNode animationSceneNode3 = FromToAnimationSceneNode.Factory.InstantiateWithTarget(animationSceneNode1.ViewModel, animationSceneNode1.TargetElement, animationSceneNode1.TargetProperty, animationSceneNode1.StoryboardContainer, animationForType);
                object transitionValue = VisualStateTransitionSceneNode.GetTransitionValue(animationSceneNode1, true);
                animationSceneNode3.To = transitionValue;
                animationSceneNode3.Duration = 0.0;
                animationSceneNode2 = (AnimationSceneNode) animationSceneNode3;
              }
              else
              {
                object transitionValue = VisualStateTransitionSceneNode.GetTransitionValue(animationSceneNode1, true);
                if (transitionValue != null && elementAndProperty.SceneNode != null && elementAndProperty.PropertyReference != null)
                {
                  KeyFrameAnimationSceneNode animationSceneNode3 = KeyFrameAnimationSceneNode.Factory.InstantiateWithTarget(animationSceneNode1.ViewModel, elementAndProperty.SceneNode, elementAndProperty.PropertyReference, animationSceneNode1.StoryboardContainer, PlatformTypes.ObjectAnimationUsingKeyFrames);
                  animationSceneNode3.AddKeyFrame(0.0, transitionValue);
                  animationSceneNode2 = (AnimationSceneNode) animationSceneNode3;
                }
              }
              if (animationSceneNode2 != null)
              {
                timelineSceneNode1.Children.Add((TimelineSceneNode) animationSceneNode2);
                dictionary[elementAndProperty] = animationSceneNode2;
              }
            }
          }
        }
      }
      foreach (TimelineSceneNode timelineSceneNode2 in (IEnumerable<TimelineSceneNode>) this.Storyboard.Children)
      {
        AnimationSceneNode animation = timelineSceneNode2 as AnimationSceneNode;
        if (animation != null)
        {
          TimelineSceneNode.PropertyNodePair elementAndProperty = timelineSceneNode2.TargetElementAndProperty;
          if (this.ShouldGenerateHandoffAnimationFor(animation, elementAndProperty) && !dictionary.ContainsKey(elementAndProperty))
          {
            ITypeId animationForType = FromToAnimationSceneNode.GetFromToAnimationForType(animation.AnimatedType, animation.ProjectContext);
            if (elementAndProperty.SceneNode != null && elementAndProperty.PropertyReference != null)
            {
              if (animationForType != null)
              {
                FromToAnimationSceneNode animationSceneNode = FromToAnimationSceneNode.Factory.InstantiateWithTarget(animation.ViewModel, elementAndProperty.SceneNode, elementAndProperty.PropertyReference, animation.StoryboardContainer, animationForType);
                timelineSceneNode1.Children.Add((TimelineSceneNode) animationSceneNode);
                animationSceneNode.Duration = 0.0;
              }
              else
              {
                object computedValue = elementAndProperty.SceneNode.GetComputedValue(elementAndProperty.PropertyReference);
                if (computedValue != null)
                {
                  KeyFrameAnimationSceneNode animationSceneNode = KeyFrameAnimationSceneNode.Factory.InstantiateWithTarget(animation.ViewModel, elementAndProperty.SceneNode, elementAndProperty.PropertyReference, animation.StoryboardContainer, PlatformTypes.ObjectAnimationUsingKeyFrames);
                  animationSceneNode.AddKeyFrame(0.0, computedValue);
                  timelineSceneNode1.Children.Add((TimelineSceneNode) animationSceneNode);
                }
              }
            }
          }
        }
      }
      return timelineSceneNode1;
    }

    private bool ShouldGenerateHandoffAnimationFor(AnimationSceneNode animation, TimelineSceneNode.PropertyNodePair targetElementAndProperty)
    {
      if ((animation.TargetName == null || targetElementAndProperty.SceneNode != null && targetElementAndProperty.SceneNode.IsInDocument) && (targetElementAndProperty.PropertyReference != null && !DesignTimeProperties.ExplicitAnimationProperty.Equals((object) targetElementAndProperty.PropertyReference[0])))
        return !AnimationProxyManager.IsAnimationProxy((TimelineSceneNode) animation);
      return false;
    }

    public static object GetTransitionValue(AnimationSceneNode stateAnimation, bool animationIsFromFromState)
    {
      KeyFrameAnimationSceneNode animationSceneNode1 = stateAnimation as KeyFrameAnimationSceneNode;
      FromToAnimationSceneNode animationSceneNode2 = stateAnimation as FromToAnimationSceneNode;
      object obj = (object) null;
      if (animationSceneNode1 != null)
      {
        int keyFrameCount = animationSceneNode1.KeyFrameCount;
        if (keyFrameCount > 0)
          obj = !animationIsFromFromState ? animationSceneNode1.GetKeyFrameAtIndex(0).Value : animationSceneNode1.GetKeyFrameAtIndex(keyFrameCount - 1).Value;
      }
      if (animationSceneNode2 != null)
        obj = !animationIsFromFromState ? animationSceneNode2.From ?? animationSceneNode2.To : animationSceneNode2.To;
      return obj;
    }

    public void UpdateToValuesForStateValueChange(TimelineSceneNode.PropertyNodePair targetElementAndProperty, object newValue)
    {
      StoryboardTimelineSceneNode storyboard = this.Storyboard;
      if (storyboard == null)
        return;
      foreach (TimelineSceneNode timelineSceneNode in (IEnumerable<TimelineSceneNode>) storyboard.Children)
      {
        AnimationSceneNode animationSceneNode1 = timelineSceneNode as AnimationSceneNode;
        if (animationSceneNode1 != null && animationSceneNode1.TargetElementAndProperty.Equals((object) targetElementAndProperty))
        {
          FromToAnimationSceneNode animationSceneNode2 = animationSceneNode1 as FromToAnimationSceneNode;
          if (animationSceneNode2 != null)
          {
            bool shouldSerialize = animationSceneNode2.ShouldSerialize;
            animationSceneNode2.To = newValue;
            animationSceneNode2.ShouldSerialize = shouldSerialize;
          }
        }
      }
    }

    public void UpdateAutoTransitionsForState(VisualStateSceneNode state)
    {
      if (!state.StateGroup.IsSketchFlowAnimation || !(this.FromStateName != VisualStateManagerSceneNode.SketchFlowAnimationHoldTimeStateName))
        return;
      StoryboardTimelineSceneNode storyboard = state.Storyboard;
      Dictionary<TimelineSceneNode.PropertyNodePair, TimelineSceneNode> dictionary1 = new Dictionary<TimelineSceneNode.PropertyNodePair, TimelineSceneNode>();
      Dictionary<TimelineSceneNode.PropertyNodePair, KeyFrameAnimationSceneNode> dictionary2 = new Dictionary<TimelineSceneNode.PropertyNodePair, KeyFrameAnimationSceneNode>();
      StoryboardTimelineSceneNode timelineSceneNode1 = this.Storyboard;
      if (timelineSceneNode1 != null)
      {
        foreach (TimelineSceneNode timelineSceneNode2 in (IEnumerable<TimelineSceneNode>) timelineSceneNode1.Children)
        {
          KeyFrameAnimationSceneNode animationSceneNode = timelineSceneNode2 as KeyFrameAnimationSceneNode;
          if (animationSceneNode != null)
            dictionary2[timelineSceneNode2.TargetElementAndProperty] = animationSceneNode;
        }
      }
      if (storyboard != null)
      {
        foreach (TimelineSceneNode timelineSceneNode2 in (IEnumerable<TimelineSceneNode>) storyboard.Children)
        {
          dictionary1[timelineSceneNode2.TargetElementAndProperty] = timelineSceneNode2;
          KeyFrameAnimationSceneNode animationSceneNode1 = timelineSceneNode2 as KeyFrameAnimationSceneNode;
          if (animationSceneNode1 != null && animationSceneNode1.NoAutoTransitionsProvided)
          {
            if (dictionary2.ContainsKey(timelineSceneNode2.TargetElementAndProperty))
            {
              KeyFrameAnimationSceneNode animationSceneNode2 = dictionary2[timelineSceneNode2.TargetElementAndProperty];
              if (animationSceneNode1.KeyFrameCount > 0 && (bool) animationSceneNode2.GetLocalOrDefaultValue(DesignTimeProperties.AutoTransitionProperty))
              {
                if (animationSceneNode2.KeyFrameCount > 0)
                  animationSceneNode2.GetKeyFrameAtIndex(animationSceneNode2.KeyFrameCount - 1).Value = animationSceneNode1.GetKeyFrameAtIndex(0).Value;
                else
                  animationSceneNode2.AddKeyFrame(0.0, animationSceneNode1.GetKeyFrameAtIndex(0).Value);
              }
            }
            else
            {
              if (timelineSceneNode1 == null)
              {
                timelineSceneNode1 = StoryboardTimelineSceneNode.Factory.Instantiate(this.ViewModel);
                this.Storyboard = timelineSceneNode1;
              }
              KeyFrameAnimationSceneNode animationSceneNode2 = KeyFrameAnimationSceneNode.Factory.InstantiateWithTarget(this.ViewModel, animationSceneNode1.TargetElementAndProperty.SceneNode, animationSceneNode1.TargetElementAndProperty.PropertyReference, this.StoryboardContainer, KeyFrameAnimationSceneNode.GetKeyFrameAnimationForType(animationSceneNode1.AnimatedType, this.ProjectContext));
              animationSceneNode2.ShouldSerialize = true;
              animationSceneNode2.SetValue(DesignTimeProperties.AutoTransitionProperty, (object) true);
              if (animationSceneNode1.KeyFrameCount > 0)
                animationSceneNode2.AddKeyFrame(0.0, animationSceneNode1.GetKeyFrameAtIndex(0).Value);
              timelineSceneNode1.Children.Add((TimelineSceneNode) animationSceneNode2);
            }
          }
        }
      }
      if (timelineSceneNode1 == null)
        return;
      foreach (TimelineSceneNode timelineSceneNode2 in (IEnumerable<TimelineSceneNode>) timelineSceneNode1.Children)
      {
        if ((bool) timelineSceneNode2.GetLocalOrDefaultValue(DesignTimeProperties.AutoTransitionProperty) && !dictionary1.ContainsKey(timelineSceneNode2.TargetElementAndProperty))
          timelineSceneNode2.Remove();
      }
      if (timelineSceneNode1.Children.Count != 0)
        return;
      timelineSceneNode1.Remove();
    }

    protected override void ModifyValue(PropertyReference propertyReference, object valueToSet, SceneNode.Modification modification, int index)
    {
      if (!this.ShouldSerialize && this.ViewModel != null && (modification == SceneNode.Modification.InsertValue || modification == SceneNode.Modification.SetValue))
        this.ShouldSerialize = true;
      if (this.StateGroup != null && !this.StateGroup.ShouldSerialize)
        this.StateGroup.ShouldSerialize = true;
      base.ModifyValue(propertyReference, valueToSet, modification, index);
      if (this.ViewModel == null || !propertyReference.FirstStep.Equals((object) VisualStateTransitionSceneNode.GeneratedEasingFunctionProperty) && !propertyReference.FirstStep.Equals((object) VisualStateTransitionSceneNode.GeneratedDurationProperty) && !propertyReference.FirstStep.Equals((object) VisualStateManagerSceneNode.TransitionEffectProperty))
        return;
      this.UpdateTransitionStoryboard(true, (Dictionary<TimelineSceneNode.PropertyNodePair, TransitionAnimationData>) null);
    }

    public class ConcreteVisualStateTransitionSceneNodeFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new VisualStateTransitionSceneNode();
      }

      public VisualStateTransitionSceneNode Instantiate(SceneViewModel viewModel)
      {
        return (VisualStateTransitionSceneNode) this.Instantiate(viewModel, ProjectNeutralTypes.VisualTransition);
      }
    }
  }
}
