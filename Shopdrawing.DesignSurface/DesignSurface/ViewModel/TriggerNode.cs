// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.TriggerNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public sealed class TriggerNode : BaseTriggerNode, ITriggerConditionNode
  {
    public static readonly IPropertyId SettersProperty = (IPropertyId) PlatformTypes.Trigger.GetMember(MemberType.LocalProperty, "Setters", MemberAccessTypes.Public);
    public static readonly IPropertyId PropertyProperty = (IPropertyId) PlatformTypes.Trigger.GetMember(MemberType.LocalProperty, "Property", MemberAccessTypes.Public);
    public static readonly IPropertyId ValueProperty = (IPropertyId) PlatformTypes.Trigger.GetMember(MemberType.LocalProperty, "Value", MemberAccessTypes.Public);
    public static readonly IPropertyId SourceNameProperty = (IPropertyId) PlatformTypes.Trigger.GetMember(MemberType.LocalProperty, "SourceName", MemberAccessTypes.Public);
    public static readonly TriggerNode.ConcreteTriggerNodeFactory Factory = new TriggerNode.ConcreteTriggerNodeFactory();
    private TriggerCondition condition;

    public override string PresentationName
    {
      get
      {
        return this.condition.PresentationName;
      }
    }

    public override ISceneNodeCollection<SceneNode> Setters
    {
      get
      {
        return this.GetCollectionForProperty(TriggerNode.SettersProperty);
      }
    }

    BaseTriggerNode ITriggerConditionNode.TriggerNode
    {
      get
      {
        return (BaseTriggerNode) this;
      }
    }

    DependencyProperty ITriggerConditionNode.PropertyKey
    {
      get
      {
        return this.condition.PropertyKey;
      }
      set
      {
        this.SetValue(TriggerNode.PropertyProperty, (object) value);
      }
    }

    object ITriggerConditionNode.Value
    {
      get
      {
        return this.condition.Value;
      }
      set
      {
        this.SetValue(TriggerNode.ValueProperty, value);
      }
    }

    string ITriggerConditionNode.SourceName
    {
      get
      {
        return this.condition.SourceName;
      }
      set
      {
        if (string.IsNullOrEmpty(value))
          this.ClearValue(TriggerNode.SourceNameProperty);
        else
          this.SetValue(TriggerNode.SourceNameProperty, (object) value);
      }
    }

    SceneNode ITriggerConditionNode.Source
    {
      get
      {
        return this.StoryboardContainer.ResolveTargetName(((ITriggerConditionNode) this).SourceName, (SceneNode) this);
      }
      set
      {
        if (value == null)
        {
          ((ITriggerConditionNode) this).SourceName = (string) null;
        }
        else
        {
          value.EnsureNamed();
          ((ITriggerConditionNode) this).SourceName = value.Name;
        }
      }
    }

    public class ConcreteTriggerNodeFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        TriggerNode triggerNode = new TriggerNode();
        triggerNode.condition = new TriggerCondition((SceneNode) triggerNode, TriggerNode.PropertyProperty, TriggerNode.ValueProperty, TriggerNode.SourceNameProperty);
        return (SceneNode) triggerNode;
      }

      public TriggerNode Instantiate(SceneViewModel viewModel)
      {
        return (TriggerNode) this.Instantiate(viewModel, PlatformTypes.Trigger);
      }
    }
  }
}
