// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.BehaviorNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class BehaviorNode : BehaviorBaseNode
  {
    public static readonly BehaviorNode.ConcreteBehaviorNodeFactory Factory = new BehaviorNode.ConcreteBehaviorNodeFactory();

    public class ConcreteBehaviorNodeFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new BehaviorNode();
      }

      public BehaviorNode Instantiate(SceneViewModel viewModel)
      {
        return (BehaviorNode) this.Instantiate(viewModel, ProjectNeutralTypes.Behavior);
      }
    }
  }
}
