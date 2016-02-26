// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.SketchFlowAnimationTriggerNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public sealed class SketchFlowAnimationTriggerNode : BehaviorTriggerBaseNode, ISketchFlowBehaviorNode
  {
    public static readonly IPropertyId SketchFlowAnimationProperty = (IPropertyId) ProjectNeutralTypes.SketchFlowAnimationTrigger.GetMember(MemberType.LocalProperty, "SketchFlowAnimation", MemberAccessTypes.Public);
    public static readonly SketchFlowAnimationTriggerNode.ConcreteSketchFlowAnimationTriggerNodeFactory Factory = new SketchFlowAnimationTriggerNode.ConcreteSketchFlowAnimationTriggerNodeFactory();

    public string SketchFlowAnimation
    {
      get
      {
        return (string) this.GetComputedValue(SketchFlowAnimationTriggerNode.SketchFlowAnimationProperty);
      }
      set
      {
        this.SetValue(SketchFlowAnimationTriggerNode.SketchFlowAnimationProperty, (object) value);
      }
    }

    string ISketchFlowBehaviorNode.TargetScreen
    {
      get
      {
        return (string) null;
      }
    }

    public class ConcreteSketchFlowAnimationTriggerNodeFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new SketchFlowAnimationTriggerNode();
      }

      public SketchFlowAnimationTriggerNode Instantiate(SceneViewModel viewModel)
      {
        return (SketchFlowAnimationTriggerNode) this.Instantiate(viewModel, ProjectNeutralTypes.SketchFlowAnimationTrigger);
      }
    }
  }
}
