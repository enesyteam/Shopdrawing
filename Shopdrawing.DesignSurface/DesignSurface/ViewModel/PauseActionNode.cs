// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.PauseActionNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class PauseActionNode : ControllableStoryboardActionNode
  {
    public static readonly PauseActionNode.ConcretePauseActionNodeFactory Factory = new PauseActionNode.ConcretePauseActionNodeFactory();

    public override TimelineOperation TimelineOperation
    {
      get
      {
        return TimelineOperation.Pause;
      }
    }

    public class ConcretePauseActionNodeFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new PauseActionNode();
      }

      public PauseActionNode Instantiate(SceneViewModel viewModel)
      {
        return (PauseActionNode) this.Instantiate(viewModel, PlatformTypes.PauseStoryboard);
      }
    }
  }
}
