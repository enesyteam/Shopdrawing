// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.PlaySketchFlowAnimationActionNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class PlaySketchFlowAnimationActionNode : BehaviorTriggerActionNode, ISketchFlowBehaviorNode
  {
    public static readonly IPropertyId TargetScreenProperty = (IPropertyId) ProjectNeutralTypes.PlaySketchFlowAnimationAction.GetMember(MemberType.LocalProperty, "TargetScreen", MemberAccessTypes.Public);
    public static readonly IPropertyId SketchFlowAnimationProperty = (IPropertyId) ProjectNeutralTypes.PlaySketchFlowAnimationAction.GetMember(MemberType.LocalProperty, "SketchFlowAnimation", MemberAccessTypes.Public);
    public static readonly PlaySketchFlowAnimationActionNode.ConcretePlaySketchFlowAnimationActionNodeFactory Factory = new PlaySketchFlowAnimationActionNode.ConcretePlaySketchFlowAnimationActionNodeFactory();

    public string TargetScreen
    {
      get
      {
        return (string) this.GetComputedValue(PlaySketchFlowAnimationActionNode.TargetScreenProperty);
      }
      set
      {
        this.SetValue(PlaySketchFlowAnimationActionNode.TargetScreenProperty, (object) value);
      }
    }

    public string SketchFlowAnimation
    {
      get
      {
        return (string) this.GetComputedValue(PlaySketchFlowAnimationActionNode.SketchFlowAnimationProperty);
      }
      set
      {
        this.SetValue(PlaySketchFlowAnimationActionNode.SketchFlowAnimationProperty, (object) value);
      }
    }

    public class ConcretePlaySketchFlowAnimationActionNodeFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new PlaySketchFlowAnimationActionNode();
      }

      public PlaySketchFlowAnimationActionNode Instantiate(SceneViewModel viewModel)
      {
        return (PlaySketchFlowAnimationActionNode) this.Instantiate(viewModel, ProjectNeutralTypes.PlaySketchFlowAnimationAction);
      }
    }
  }
}
