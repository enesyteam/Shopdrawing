// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.ComparisonConditionNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class ComparisonConditionNode : SceneNode
  {
    public static readonly IPropertyId LeftOperandProperty = (IPropertyId) ProjectNeutralTypes.ComparisonCondition.GetMember(MemberType.Property, "LeftOperand", MemberAccessTypes.Public);
    public static readonly IPropertyId RightOperandProperty = (IPropertyId) ProjectNeutralTypes.ComparisonCondition.GetMember(MemberType.Property, "RightOperand", MemberAccessTypes.Public);
    public static readonly ComparisonConditionNode.ConcreteComparisonConditionNodeFactory Factory = new ComparisonConditionNode.ConcreteComparisonConditionNodeFactory();

    public class ConcreteComparisonConditionNodeFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new ComparisonConditionNode();
      }
    }
  }
}
