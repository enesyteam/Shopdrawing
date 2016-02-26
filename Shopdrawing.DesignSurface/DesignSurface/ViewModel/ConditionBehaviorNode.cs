// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.ConditionBehaviorNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class ConditionBehaviorNode : BehaviorNode
  {
    public static readonly IPropertyId ConditionProperty = (IPropertyId) ProjectNeutralTypes.ConditionBehavior.GetMember(MemberType.Property, "Condition", MemberAccessTypes.Public);
    public static readonly ConditionBehaviorNode.ConcreteConditionBehaviorNodeFactory Factory = new ConditionBehaviorNode.ConcreteConditionBehaviorNodeFactory();

    public ConditionalExpressionNode ConditionAsConditionalExpressionNode
    {
      get
      {
        SceneNode valueAsSceneNode = this.GetLocalValueAsSceneNode(ConditionBehaviorNode.ConditionProperty);
        if (valueAsSceneNode != null && ProjectNeutralTypes.ConditionalExpression.IsAssignableFrom((ITypeId) valueAsSceneNode.Type))
          return (ConditionalExpressionNode) valueAsSceneNode;
        return (ConditionalExpressionNode) null;
      }
      set
      {
        this.SetValueAsSceneNode(ConditionBehaviorNode.ConditionProperty, (SceneNode) value);
      }
    }

    public class ConcreteConditionBehaviorNodeFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new ConditionBehaviorNode();
      }
    }
  }
}
