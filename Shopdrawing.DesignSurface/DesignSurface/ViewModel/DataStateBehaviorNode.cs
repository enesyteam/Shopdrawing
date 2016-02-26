// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.DataStateBehaviorNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class DataStateBehaviorNode : BehaviorNode
  {
    public static readonly IPropertyId TrueStateProperty = (IPropertyId) ProjectNeutralTypes.DataStateBehavior.GetMember(MemberType.LocalProperty, "TrueState", MemberAccessTypes.Public);
    public static readonly IPropertyId FalseStateProperty = (IPropertyId) ProjectNeutralTypes.DataStateBehavior.GetMember(MemberType.LocalProperty, "FalseState", MemberAccessTypes.Public);
    public static readonly DataStateBehaviorNode.ConcreteDataStateBehaviorNodeFactory Factory = new DataStateBehaviorNode.ConcreteDataStateBehaviorNodeFactory();

    public class ConcreteDataStateBehaviorNodeFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new DataStateBehaviorNode();
      }

      public DataStateBehaviorNode Instantiate(SceneViewModel viewModel)
      {
        return (DataStateBehaviorNode) this.Instantiate(viewModel, ProjectNeutralTypes.DataStateBehavior);
      }
    }
  }
}
