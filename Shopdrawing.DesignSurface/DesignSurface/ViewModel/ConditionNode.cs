// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.ConditionNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public sealed class ConditionNode : SceneNode, ITriggerConditionNode
  {
    public static readonly IPropertyId PropertyProperty = (IPropertyId) PlatformTypes.Condition.GetMember(MemberType.LocalProperty, "Property", MemberAccessTypes.Public);
    public static readonly IPropertyId ValueProperty = (IPropertyId) PlatformTypes.Condition.GetMember(MemberType.LocalProperty, "Value", MemberAccessTypes.Public);
    public static readonly IPropertyId SourceNameProperty = (IPropertyId) PlatformTypes.Condition.GetMember(MemberType.LocalProperty, "SourceName", MemberAccessTypes.Public);
    public static readonly IPropertyId BindingProperty = (IPropertyId) PlatformTypes.Condition.GetMember(MemberType.LocalProperty, "Binding", MemberAccessTypes.Public);
    public static readonly ConditionNode.ConcreteConditionNodeFactory Factory = new ConditionNode.ConcreteConditionNodeFactory();
    private TriggerCondition condition;

    public string PresentationName
    {
      get
      {
        return this.condition.PresentationName;
      }
    }

    BaseTriggerNode ITriggerConditionNode.TriggerNode
    {
      get
      {
        return (BaseTriggerNode) this.Parent;
      }
    }

    public DependencyProperty PropertyKey
    {
      get
      {
        return this.condition.PropertyKey;
      }
      set
      {
        this.SetValue(ConditionNode.PropertyProperty, (object) value);
      }
    }

    public object Value
    {
      get
      {
        return this.condition.Value;
      }
      set
      {
        this.SetValue(ConditionNode.ValueProperty, value);
      }
    }

    public string SourceName
    {
      get
      {
        return this.condition.SourceName;
      }
      set
      {
        if (string.IsNullOrEmpty(value))
          this.ClearValue(ConditionNode.SourceNameProperty);
        else
          this.SetValue(ConditionNode.SourceNameProperty, (object) value);
      }
    }

    public SceneNode Source
    {
      get
      {
        return this.StoryboardContainer.ResolveTargetName(this.SourceName, (SceneNode) this);
      }
      set
      {
        if (value == null)
        {
          this.SourceName = (string) null;
        }
        else
        {
          value.EnsureNamed();
          this.SourceName = value.Name;
        }
      }
    }

    public override string ToString()
    {
      return this.PresentationName;
    }

    public class ConcreteConditionNodeFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        ConditionNode conditionNode = new ConditionNode();
        conditionNode.condition = new TriggerCondition((SceneNode) conditionNode, ConditionNode.PropertyProperty, ConditionNode.ValueProperty, ConditionNode.SourceNameProperty);
        return (SceneNode) conditionNode;
      }

      public ConditionNode Instantiate(SceneViewModel viewModel)
      {
        return (ConditionNode) this.Instantiate(viewModel, PlatformTypes.Condition);
      }
    }
  }
}
