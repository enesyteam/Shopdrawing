// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.ChangePropertyActionNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class ChangePropertyActionNode : BehaviorTargetedTriggerActionNode
  {
    public static readonly IPropertyId PropertyNameProperty = (IPropertyId) ProjectNeutralTypes.ChangePropertyAction.GetMember(MemberType.LocalProperty, "PropertyName", MemberAccessTypes.Public);
    public static readonly IPropertyId ValueProperty = (IPropertyId) ProjectNeutralTypes.ChangePropertyAction.GetMember(MemberType.LocalProperty, "Value", MemberAccessTypes.Public);
    public new static readonly SceneNode.ConcreteSceneNodeFactory Factory = (SceneNode.ConcreteSceneNodeFactory) new ChangePropertyActionNode.ConcreteChangePropertyActionNodeFactory();

    protected override void ModifyValue(PropertyReference propertyReference, object valueToSet, SceneNode.Modification modification, int index)
    {
      if (propertyReference.LastStep.Equals((object) ChangePropertyActionNode.PropertyNameProperty) && modification == SceneNode.Modification.ClearValue)
        this.ClearValue(ChangePropertyActionNode.ValueProperty);
      base.ModifyValue(propertyReference, valueToSet, modification, index);
    }

    private class ConcreteChangePropertyActionNodeFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new ChangePropertyActionNode();
      }
    }
  }
}
