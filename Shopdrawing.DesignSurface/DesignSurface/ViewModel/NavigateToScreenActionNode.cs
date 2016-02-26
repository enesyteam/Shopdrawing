// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.NavigateToScreenActionNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class NavigateToScreenActionNode : BehaviorTriggerActionNode
  {
    public static readonly IPropertyId TargetScreenProperty = (IPropertyId) ProjectNeutralTypes.NavigateToScreenAction.GetMember(MemberType.LocalProperty, "TargetScreen", MemberAccessTypes.Public);
    public static readonly NavigateToScreenActionNode.ConcreteNavigateToScreenActionNodeFactory Factory = new NavigateToScreenActionNode.ConcreteNavigateToScreenActionNodeFactory();

    public string TargetScreen
    {
      get
      {
        return (string) this.GetComputedValue(NavigateToScreenActionNode.TargetScreenProperty);
      }
      set
      {
        this.SetValue(NavigateToScreenActionNode.TargetScreenProperty, (object) value);
      }
    }

    public class ConcreteNavigateToScreenActionNodeFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new NavigateToScreenActionNode();
      }

      public NavigateToScreenActionNode Instantiate(SceneViewModel viewModel)
      {
        return (NavigateToScreenActionNode) this.Instantiate(viewModel, ProjectNeutralTypes.NavigateToScreenAction);
      }
    }
  }
}
