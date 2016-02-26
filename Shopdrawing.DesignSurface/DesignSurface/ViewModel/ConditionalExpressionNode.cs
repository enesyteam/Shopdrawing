// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.ConditionalExpressionNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.ComponentModel;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class ConditionalExpressionNode : SceneNode
  {
    public static readonly IPropertyId ConditionsProperty = (IPropertyId) ProjectNeutralTypes.ConditionalExpression.GetMember(MemberType.Property, "Conditions", MemberAccessTypes.Public);
    public static readonly IPropertyId ForwardChainingProperty = (IPropertyId) ProjectNeutralTypes.ConditionalExpression.GetMember(MemberType.Property, "ForwardChaining", MemberAccessTypes.Public);
    public static readonly ConditionalExpressionNode.ConcreteConditionalExpressionNodeFactory Factory = new ConditionalExpressionNode.ConcreteConditionalExpressionNodeFactory();

    public ISceneNodeCollection<SceneNode> Conditions
    {
      get
      {
        return this.GetCollectionForProperty(ConditionalExpressionNode.ConditionsProperty);
      }
    }

    public bool ForwardChainingToOr
    {
      get
      {
        IProperty property = this.ProjectContext.ResolveProperty(ConditionalExpressionNode.ForwardChainingProperty);
        object localValue = this.GetLocalValue((IPropertyId) property);
        if (localValue != null)
          return string.Compare(Enum.GetName(property.PropertyType.RuntimeType, localValue), "Or", StringComparison.Ordinal) == 0;
        return false;
      }
      set
      {
        if (value)
        {
          IType type = this.ProjectContext.ResolveType(ProjectNeutralTypes.ForwardChaining);
          if (this.Platform.Metadata.IsNullType((ITypeId) type))
            return;
          TypeConverter typeConverter = type.TypeConverter;
          if (typeConverter == null)
            return;
          object valueToSet = typeConverter.ConvertFrom((object) "Or");
          if (valueToSet == null)
            return;
          this.SetValue(ConditionalExpressionNode.ForwardChainingProperty, valueToSet);
        }
        else
          this.ClearValue(ConditionalExpressionNode.ForwardChainingProperty);
      }
    }

    public class ConcreteConditionalExpressionNodeFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new ConditionalExpressionNode();
      }
    }
  }
}
