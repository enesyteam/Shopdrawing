// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.ActivateStateActionNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class ActivateStateActionNode : BehaviorTriggerActionNode, ISketchFlowBehaviorNode
  {
    public static readonly IPropertyId TargetScreenProperty = (IPropertyId) ProjectNeutralTypes.ActivateStateAction.GetMember(MemberType.LocalProperty, "TargetScreen", MemberAccessTypes.Public);
    public static readonly IPropertyId TargetStateProperty = (IPropertyId) ProjectNeutralTypes.ActivateStateAction.GetMember(MemberType.LocalProperty, "TargetState", MemberAccessTypes.Public);
    public static readonly ActivateStateActionNode.ConcreteActivateStateActionNodeFactory Factory = new ActivateStateActionNode.ConcreteActivateStateActionNodeFactory();

    public string TargetScreen
    {
      get
      {
        return (string) this.GetComputedValue(ActivateStateActionNode.TargetScreenProperty);
      }
      set
      {
        this.SetValue(ActivateStateActionNode.TargetScreenProperty, (object) value);
      }
    }

    public string TargetState
    {
      get
      {
        return (string) this.GetComputedValue(ActivateStateActionNode.TargetStateProperty);
      }
      set
      {
        this.SetValue(ActivateStateActionNode.TargetStateProperty, (object) value);
      }
    }

    public class ConcreteActivateStateActionNodeFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new ActivateStateActionNode();
      }

      public ActivateStateActionNode Instantiate(SceneViewModel viewModel)
      {
        return (ActivateStateActionNode) this.Instantiate(viewModel, ProjectNeutralTypes.ActivateStateAction);
      }
    }
  }
}
