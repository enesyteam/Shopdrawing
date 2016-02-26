// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.StoryboardTimelineSceneNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Properties;
using System;
using System.Collections.Generic;
using System.Windows.Media.Animation;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class StoryboardTimelineSceneNode : TimelineSceneNode
  {
    public static readonly IPropertyId ChildrenProperty = (IPropertyId) PlatformTypes.Storyboard.GetMember(MemberType.LocalProperty, "Children", MemberAccessTypes.Public);
    public static readonly IPropertyId TargetNameProperty = (IPropertyId) PlatformTypes.Storyboard.GetMember(MemberType.AttachedProperty, "TargetName", MemberAccessTypes.Public);
    public static readonly IPropertyId TargetPropertyProperty = (IPropertyId) PlatformTypes.Storyboard.GetMember(MemberType.AttachedProperty, "TargetProperty", MemberAccessTypes.Public);
    public static readonly StoryboardTimelineSceneNode.ConcreteStoryboardTimelineSceneNodeFactory Factory = new StoryboardTimelineSceneNode.ConcreteStoryboardTimelineSceneNodeFactory();

    public double PlayDuration
    {
      get
      {
        double val1 = 0.0;
        foreach (TimelineSceneNode timelineSceneNode in (IEnumerable<TimelineSceneNode>) this.Children)
        {
          double val2 = timelineSceneNode.Begin + timelineSceneNode.ActiveDuration;
          val1 = Math.Max(val1, val2);
        }
        return val1;
      }
    }

    public new string Name
    {
      get
      {
        string str = (string) null;
        DictionaryEntryNode dictionaryEntryNode = this.Parent as DictionaryEntryNode;
        if (dictionaryEntryNode != null)
          str = dictionaryEntryNode.Key as string;
        if (str == null)
          str = base.Name;
        if (str == null && this.CanAccessProperties)
          str = this.GetLocalOrDefaultValue(DesignTimeProperties.StoryboardNameProperty) as string;
        return str;
      }
      set
      {
        DictionaryEntryNode dictionaryEntryNode = this.Parent as DictionaryEntryNode;
        if (dictionaryEntryNode != null)
        {
          if (this.ProjectContext.IsCapabilitySet(PlatformCapability.UseKeyByDefault))
            dictionaryEntryNode.Key = (object) value;
          else if (dictionaryEntryNode.Key != null && string.IsNullOrEmpty(base.Name))
            dictionaryEntryNode.Key = (object) value;
          else
            base.Name = value;
        }
        else
          base.Name = value;
        this.SetLocalValue(DesignTimeProperties.StoryboardNameProperty, (object) value);
      }
    }

    public Storyboard InstantiatedStoryboard
    {
      get
      {
        return (Storyboard) this.InstantiatedTimeline;
      }
    }

    public override double NaturalDuration
    {
      get
      {
        double val2 = 0.0;
        foreach (TimelineSceneNode timelineSceneNode in (IEnumerable<TimelineSceneNode>) this.Children)
        {
          if (timelineSceneNode.TargetProperty == null || !DesignTimeProperties.ExplicitAnimationProperty.Equals((object) timelineSceneNode.TargetProperty.LastStep))
            val2 = Math.Max(timelineSceneNode.ClipEnd, val2);
        }
        return val2;
      }
    }

    public bool IsInResourceDictionary
    {
      get
      {
        return this.Parent is DictionaryEntryNode;
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
        if (value)
        {
          if (this.ControllingTransition != null && !this.ControllingTransition.ShouldSerialize)
            this.ControllingTransition.ShouldSerialize = true;
          else if (this.ControllingState != null && !this.ControllingState.ShouldSerialize)
            this.ControllingState.ShouldSerialize = true;
        }
        base.ShouldSerialize = value;
      }
    }

    public IList<TimelineSceneNode> Children
    {
      get
      {
        if (this.CanAccessProperties)
          return (IList<TimelineSceneNode>) new SceneNode.SceneNodeCollection<TimelineSceneNode>((SceneNode) this, StoryboardTimelineSceneNode.ChildrenProperty);
        return (IList<TimelineSceneNode>) new SceneNode.EmptySceneNodeCollection<TimelineSceneNode>();
      }
    }

    public IEnumerable<TimelineActionNode> ControllingActions
    {
      get
      {
        if (this.StoryboardContainer != null)
        {
          foreach (TriggerBaseNode triggerBaseNode in (IEnumerable<TriggerBaseNode>) this.StoryboardContainer.VisualTriggers)
          {
            foreach (TriggerActionNode triggerActionNode in triggerBaseNode.GetActions())
            {
              TimelineActionNode timelineAction = triggerActionNode as TimelineActionNode;
              if (timelineAction != null && timelineAction.TargetTimeline == this)
                yield return timelineAction;
            }
          }
        }
      }
    }

    public AnimationSceneNode GetAnimation(SceneNode targetElement, IPropertyId targetProperty)
    {
      return this.GetAnimation(targetElement, new PropertyReference(this.ProjectContext.ResolveProperty(targetProperty) as ReferenceStep));
    }

    public AnimationSceneNode GetAnimation(SceneNode targetElement, PropertyReference targetProperty)
    {
      if (targetElement == null || targetProperty == null)
        return (AnimationSceneNode) null;
      StyleNode styleNode = targetElement as StyleNode;
      if (styleNode != null)
      {
        BaseFrameworkElement targetElement1 = styleNode.TargetElement;
      }
      PropertyReference propertyReference = targetProperty;
      if (propertyReference == null)
        return (AnimationSceneNode) null;
      string path = propertyReference.Path;
      foreach (TimelineSceneNode timelineSceneNode in (IEnumerable<TimelineSceneNode>) this.Children)
      {
        AnimationSceneNode animationSceneNode = timelineSceneNode as AnimationSceneNode;
        if (animationSceneNode != null)
        {
          TimelineSceneNode.PropertyNodePair elementAndProperty = animationSceneNode.TargetElementAndProperty;
          if (elementAndProperty.SceneNode == targetElement && elementAndProperty.PropertyReference != null && (elementAndProperty.PropertyReference.Path == path && !AnimationProxyManager.IsOptimizedAnimation((TimelineSceneNode) animationSceneNode)))
            return animationSceneNode;
        }
      }
      return (AnimationSceneNode) null;
    }

    public double[] GetCompoundKeyTimes(SceneNode targetElement)
    {
      List<double> list = new List<double>();
      foreach (TimelineSceneNode timelineSceneNode in (IEnumerable<TimelineSceneNode>) this.Children)
      {
        KeyFrameAnimationSceneNode animationSceneNode = timelineSceneNode as KeyFrameAnimationSceneNode;
        if (animationSceneNode != null && animationSceneNode.TargetElement == targetElement)
        {
          foreach (KeyFrameSceneNode keyFrameSceneNode in animationSceneNode.KeyFrames)
          {
            if (!list.Contains(keyFrameSceneNode.Time))
              list.Add(keyFrameSceneNode.Time);
          }
        }
      }
      return list.ToArray();
    }

    public void UpdateActionNames()
    {
      List<ControllableStoryboardActionNode> list1 = new List<ControllableStoryboardActionNode>();
      List<BeginActionNode> list2 = new List<BeginActionNode>();
      foreach (TimelineActionNode timelineActionNode in this.ControllingActions)
      {
        BeginActionNode beginActionNode = timelineActionNode as BeginActionNode;
        ControllableStoryboardActionNode storyboardActionNode = timelineActionNode as ControllableStoryboardActionNode;
        if (beginActionNode != null)
          list2.Add(beginActionNode);
        else
          list1.Add(storyboardActionNode);
      }
      foreach (BeginActionNode beginActionNode in list2)
      {
        if (beginActionNode.Name == null)
          beginActionNode.Name = StoryboardTimelineSceneNode.CreateBeginActionName((ITriggerContainer) this.StoryboardContainer, this.Name);
      }
      string str = string.Empty;
      if (list2.Count > 0)
        str = list2[0].Name;
      foreach (ControllableStoryboardActionNode storyboardActionNode in list1)
      {
        if (storyboardActionNode.BeginActionName != str)
          storyboardActionNode.BeginActionName = str;
      }
    }

    protected override void SetTimeRegionCore(TimeRegionChangeDetails details, double scaleFactor, double originalRegionBegin, double originalRegionEnd, double finalRegionBegin, double finalRegionEnd)
    {
    }

    private static string CreateBeginActionName(ITriggerContainer storyboardContainer, string timelineName)
    {
      string str = timelineName + StringTable.TriggerDefaultActionName;
      SceneNode sceneNode = (SceneNode) storyboardContainer;
      SceneNodeIDHelper sceneNodeIdHelper = new SceneNodeIDHelper(sceneNode.ViewModel, sceneNode);
      if (!sceneNodeIdHelper.IsValidElementID(sceneNode, str))
        str = sceneNodeIdHelper.GetValidElementID(sceneNode, str);
      return str;
    }

    protected override void OnChildAdding(SceneNode child)
    {
      base.OnChildAdding(child);
      if (this.ShouldSerialize)
        return;
      TimelineSceneNode timelineSceneNode = child as TimelineSceneNode;
      if (timelineSceneNode == null || !timelineSceneNode.ShouldSerialize)
        return;
      this.ShouldSerialize = true;
    }

    protected override void OnChildRemoving(SceneNode child)
    {
      base.OnChildRemoving(child);
      TimelineSceneNode timeline = child as TimelineSceneNode;
      if (this.ViewModel.AnimationProxyManager != null && AnimationProxyManager.IsAnimationProxy(timeline))
        this.ViewModel.AnimationProxyManager.UpdateOnDeletion(child as KeyFrameAnimationSceneNode);
      VisualStateSceneNode controllingState = this.ControllingState;
      if (controllingState == null)
        return;
      TimelineSceneNode.PropertyNodePair propertyNodePair = timeline != null ? timeline.TargetElementAndProperty : new TimelineSceneNode.PropertyNodePair((SceneNode) null, (PropertyReference) null);
      VisualStateGroupSceneNode stateGroup = controllingState.StateGroup;
      if (propertyNodePair.SceneNode == null || propertyNodePair.PropertyReference == null || (!timeline.ShouldSerialize || stateGroup == null))
        return;
      List<KeyValuePair<VisualStateSceneNode, bool>> list = new List<KeyValuePair<VisualStateSceneNode, bool>>(stateGroup.States.Count);
      list.Add(new KeyValuePair<VisualStateSceneNode, bool>(controllingState, true));
      foreach (VisualStateTransitionSceneNode transitionSceneNode in (IEnumerable<VisualStateTransitionSceneNode>) stateGroup.Transitions)
      {
        if (transitionSceneNode.Storyboard != null && (!string.IsNullOrEmpty(transitionSceneNode.FromStateName) || !string.IsNullOrEmpty(transitionSceneNode.ToStateName)))
        {
          VisualStateSceneNode fromState = transitionSceneNode.FromState;
          VisualStateSceneNode toState = transitionSceneNode.ToState;
          if (fromState == controllingState || toState == controllingState)
          {
            TimelineSceneNode timelineSceneNode = (TimelineSceneNode) transitionSceneNode.Storyboard.GetAnimation(propertyNodePair.SceneNode, propertyNodePair.PropertyReference);
            if (timelineSceneNode != null)
            {
              VisualStateSceneNode key = fromState == controllingState ? toState : fromState;
              bool? nullable = new bool?();
              if (key == null)
                nullable = new bool?(false);
              if (!nullable.HasValue)
              {
                foreach (KeyValuePair<VisualStateSceneNode, bool> keyValuePair in list)
                {
                  if (keyValuePair.Key == key)
                    nullable = new bool?(keyValuePair.Value);
                }
              }
              if (!nullable.HasValue)
              {
                nullable = new bool?(key.Storyboard != null && key.Storyboard.GetAnimation(propertyNodePair.SceneNode, propertyNodePair.PropertyReference) != null);
                list.Add(new KeyValuePair<VisualStateSceneNode, bool>(key, nullable.Value));
              }
              if (!nullable.Value)
                transitionSceneNode.Storyboard.Children.Remove(timelineSceneNode);
            }
          }
        }
      }
    }

    protected override void OnChildRemoved(SceneNode child)
    {
      base.OnChildRemoved(child);
      if (!string.IsNullOrEmpty(this.Name))
        return;
      bool flag = false;
      foreach (SceneNode sceneNode in (IEnumerable<TimelineSceneNode>) this.Children)
      {
        if (sceneNode.ShouldSerialize)
        {
          flag = true;
          break;
        }
      }
      this.ShouldSerialize = flag;
    }

    protected override void ModifyValue(PropertyReference propertyReference, object valueToSet, SceneNode.Modification modification, int index)
    {
      if (!this.ShouldSerialize && modification == SceneNode.Modification.SetValue)
        this.ShouldSerialize = true;
      base.ModifyValue(propertyReference, valueToSet, modification, index);
    }

    public class ConcreteStoryboardTimelineSceneNodeFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new StoryboardTimelineSceneNode();
      }

      public StoryboardTimelineSceneNode Instantiate(SceneViewModel viewModel)
      {
        return (StoryboardTimelineSceneNode) this.Instantiate(viewModel, PlatformTypes.Storyboard);
      }
    }
  }
}
