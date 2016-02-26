// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.VisualStateGroupSceneNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class VisualStateGroupSceneNode : SceneNode
  {
    public static readonly IPropertyId StatesProperty = (IPropertyId) ProjectNeutralTypes.VisualStateGroup.GetMember(MemberType.LocalProperty, "States", MemberAccessTypes.Public);
    public static readonly IPropertyId TransitionsProperty = (IPropertyId) ProjectNeutralTypes.VisualStateGroup.GetMember(MemberType.LocalProperty, "Transitions", MemberAccessTypes.Public);
    public static readonly IPropertyId VisualStateGroupNameProperty = (IPropertyId) ProjectNeutralTypes.VisualStateGroup.GetMember(MemberType.LocalProperty, "Name", MemberAccessTypes.Public);
    public static readonly VisualStateGroupSceneNode.ConcreteVisualStateGroupSceneNodeFactory Factory = new VisualStateGroupSceneNode.ConcreteVisualStateGroupSceneNodeFactory();

    public IList<VisualStateSceneNode> States
    {
      get
      {
        return (IList<VisualStateSceneNode>) new SceneNode.SceneNodeCollection<VisualStateSceneNode>((SceneNode) this, VisualStateGroupSceneNode.StatesProperty);
      }
    }

    public IList<VisualStateTransitionSceneNode> Transitions
    {
      get
      {
        return (IList<VisualStateTransitionSceneNode>) new SceneNode.SceneNodeCollection<VisualStateTransitionSceneNode>((SceneNode) this, VisualStateGroupSceneNode.TransitionsProperty);
      }
    }

    public bool IsSketchFlowAnimation
    {
      get
      {
        return (bool) this.GetComputedValue(DesignTimeProperties.IsSketchFlowAnimationProperty);
      }
    }

    public Duration DefaultTransitionDuration
    {
      get
      {
        VisualStateTransitionSceneNode defaultTransition = this.FindDefaultTransition();
        if (defaultTransition == null)
          return new Duration(TimeSpan.Zero);
        return defaultTransition.GeneratedDuration;
      }
      set
      {
        if (!value.HasTimeSpan)
          return;
        VisualStateTransitionSceneNode transitionSceneNode = this.FindDefaultTransition();
        if (value.TimeSpan != TimeSpan.Zero)
        {
          if (transitionSceneNode == null)
          {
            transitionSceneNode = VisualStateTransitionSceneNode.Factory.Instantiate(this.ViewModel);
            this.Transitions.Add(transitionSceneNode);
          }
          transitionSceneNode.GeneratedDuration = value;
        }
        else
        {
          if (transitionSceneNode == null)
            return;
          this.Transitions.Remove(transitionSceneNode);
        }
      }
    }

    public VisualStateSceneNode AddState(SceneNode rootNode, string name)
    {
      VisualStateSceneNode visualStateSceneNode = VisualStateSceneNode.Factory.Instantiate(this.ViewModel);
      visualStateSceneNode.ShouldSerialize = this.ShouldSerialize;
      this.States.Add(visualStateSceneNode);
      string validElementId = new SceneNodeIDHelper(this.ViewModel, rootNode).GetValidElementID((SceneNode) visualStateSceneNode, name);
      visualStateSceneNode.Name = validElementId;
      return visualStateSceneNode;
    }

    public void DeleteState(VisualStateSceneNode node)
    {
      if (node.Name != null)
      {
        List<VisualStateTransitionSceneNode> list = new List<VisualStateTransitionSceneNode>();
        foreach (VisualStateTransitionSceneNode transitionSceneNode in (IEnumerable<VisualStateTransitionSceneNode>) this.Transitions)
        {
          if (node.Name.Equals(transitionSceneNode.FromStateName) || node.Name.Equals(transitionSceneNode.ToStateName))
            list.Add(transitionSceneNode);
        }
        foreach (VisualStateTransitionSceneNode transitionNode in list)
          this.DeleteTransition(transitionNode);
      }
      this.States.Remove(node);
      if (this.States.Count != 0)
        return;
      this.ClearLocalValue(VisualStateGroupSceneNode.StatesProperty);
    }

    public VisualStateTransitionSceneNode AddTransition(VisualStateSceneNode fromNode, VisualStateSceneNode toNode, Duration duration)
    {
      VisualStateTransitionSceneNode transitionSceneNode = VisualStateTransitionSceneNode.Factory.Instantiate(this.ViewModel);
      if (fromNode != null)
        transitionSceneNode.FromStateName = fromNode.Name;
      if (toNode != null)
        transitionSceneNode.ToStateName = toNode.Name;
      transitionSceneNode.GeneratedDuration = duration;
      this.Transitions.Add(transitionSceneNode);
      if (!this.ShouldSerialize)
        this.ShouldSerialize = true;
      return transitionSceneNode;
    }

    public VisualStateSceneNode FindStateByName(string name)
    {
      IList<VisualStateSceneNode> states = this.States;
      foreach (VisualStateSceneNode visualStateSceneNode in (IEnumerable<VisualStateSceneNode>) this.States)
      {
        if (visualStateSceneNode.Name == name)
          return visualStateSceneNode;
      }
      return (VisualStateSceneNode) null;
    }

    internal void DeleteTransition(VisualStateTransitionSceneNode transitionNode)
    {
      this.Transitions.Remove(transitionNode);
      if (this.Transitions.Count != 0)
        return;
      this.ClearLocalValue(VisualStateGroupSceneNode.TransitionsProperty);
    }

    private VisualStateTransitionSceneNode FindDefaultTransition()
    {
      foreach (VisualStateTransitionSceneNode transitionSceneNode in (IEnumerable<VisualStateTransitionSceneNode>) this.Transitions)
      {
        if (string.IsNullOrEmpty(transitionSceneNode.FromStateName) && string.IsNullOrEmpty(transitionSceneNode.ToStateName))
          return transitionSceneNode;
      }
      return (VisualStateTransitionSceneNode) null;
    }

    protected override void ModifyValue(PropertyReference propertyReference, object valueToSet, SceneNode.Modification modification, int index)
    {
      if (!this.ShouldSerialize && this.ViewModel != null && (modification == SceneNode.Modification.InsertValue || modification == SceneNode.Modification.SetValue))
        this.ShouldSerialize = true;
      base.ModifyValue(propertyReference, valueToSet, modification, index);
    }

    public class ConcreteVisualStateGroupSceneNodeFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new VisualStateGroupSceneNode();
      }

      public VisualStateGroupSceneNode Instantiate(SceneViewModel viewModel)
      {
        return (VisualStateGroupSceneNode) this.Instantiate(viewModel, ProjectNeutralTypes.VisualStateGroup);
      }
    }
  }
}
