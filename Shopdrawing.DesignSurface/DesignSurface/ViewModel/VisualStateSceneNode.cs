// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.VisualStateSceneNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class VisualStateSceneNode : SceneNode
  {
    public static readonly IPropertyId StoryboardProperty = (IPropertyId) ProjectNeutralTypes.VisualState.GetMember(MemberType.LocalProperty, "Storyboard", MemberAccessTypes.Public);
    public static readonly IPropertyId VisualStateNameProperty = (IPropertyId) ProjectNeutralTypes.VisualState.GetMember(MemberType.LocalProperty, "Name", MemberAccessTypes.Public);
    public static readonly VisualStateSceneNode.ConcreteVisualStateSceneNodeFactory Factory = new VisualStateSceneNode.ConcreteVisualStateSceneNodeFactory();

    public override bool ShouldSerialize
    {
      get
      {
        return base.ShouldSerialize;
      }
      set
      {
        this.SetShouldSerializeCore(value);
        if (!value || this.StateGroup == null)
          return;
        if (!this.StateGroup.ShouldSerialize)
          this.StateGroup.ShouldSerialize = true;
        foreach (VisualStateSceneNode visualStateSceneNode in (IEnumerable<VisualStateSceneNode>) this.StateGroup.States)
        {
          if (visualStateSceneNode != this && !visualStateSceneNode.ShouldSerialize)
            visualStateSceneNode.SetShouldSerializeCore(true);
        }
      }
    }

    public StoryboardTimelineSceneNode Storyboard
    {
      get
      {
        StoryboardTimelineSceneNode timelineSceneNode = this.GetLocalValueAsSceneNode(VisualStateSceneNode.StoryboardProperty) as StoryboardTimelineSceneNode;
        if (timelineSceneNode == null || !timelineSceneNode.CanAccessProperties)
          return (StoryboardTimelineSceneNode) null;
        return timelineSceneNode;
      }
      set
      {
        this.SetValueAsSceneNode(VisualStateSceneNode.StoryboardProperty, (SceneNode) value);
        if (this.ShouldSerialize || this.Storyboard == null || (!this.Storyboard.ShouldSerialize || this.ViewModel == null))
          return;
        this.ShouldSerialize = true;
      }
    }

    public VisualStateGroupSceneNode StateGroup
    {
      get
      {
        return this.Parent as VisualStateGroupSceneNode;
      }
    }

    public string Description { get; set; }

    public void UpdateTransitionsForStateValueChange(TimelineSceneNode.PropertyNodePair targetElementAndProperty, object newValue)
    {
      VisualStateGroupSceneNode stateGroup = this.StateGroup;
      if (stateGroup != null)
      {
        foreach (VisualStateTransitionSceneNode transitionSceneNode in (IEnumerable<VisualStateTransitionSceneNode>) stateGroup.Transitions)
        {
          if (transitionSceneNode.ToStateName == this.Name)
            transitionSceneNode.UpdateToValuesForStateValueChange(targetElementAndProperty, newValue);
        }
      }
      this.UpdateAutoTransitions();
    }

    public void UpdateAutoTransitions()
    {
      VisualStateGroupSceneNode stateGroup = this.StateGroup;
      if (stateGroup == null || !stateGroup.IsSketchFlowAnimation)
        return;
      foreach (VisualStateTransitionSceneNode transitionSceneNode in (IEnumerable<VisualStateTransitionSceneNode>) stateGroup.Transitions)
      {
        if (transitionSceneNode.ToStateName != null && transitionSceneNode.ToStateName == this.Name)
          transitionSceneNode.UpdateAutoTransitionsForState(this);
      }
    }

    public void SetShouldSerializeCore(bool value)
    {
      base.ShouldSerialize = value;
    }

    public class ConcreteVisualStateSceneNodeFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new VisualStateSceneNode();
      }

      internal VisualStateSceneNode Instantiate(SceneViewModel sceneViewModel)
      {
        return (VisualStateSceneNode) this.Instantiate(sceneViewModel, ProjectNeutralTypes.VisualState);
      }
    }
  }
}
