// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.NavigationMenuActionNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class NavigationMenuActionNode : BehaviorTargetedTriggerActionNode
  {
    public static readonly IPropertyId ActiveStateProperty = (IPropertyId) ProjectNeutralTypes.NavigationMenuAction.GetMember(MemberType.LocalProperty, "ActiveState", MemberAccessTypes.Public);
    public static readonly IPropertyId InactiveStateProperty = (IPropertyId) ProjectNeutralTypes.NavigationMenuAction.GetMember(MemberType.LocalProperty, "InactiveState", MemberAccessTypes.Public);
    public static readonly IPropertyId TargetScreenProperty = (IPropertyId) ProjectNeutralTypes.NavigationMenuAction.GetMember(MemberType.LocalProperty, "TargetScreen", MemberAccessTypes.Public);
    public static readonly NavigationMenuActionNode.ConcreteNavigationMenuActionFactory Factory = new NavigationMenuActionNode.ConcreteNavigationMenuActionFactory();

    public string ActiveState
    {
      get
      {
        return (string) this.GetComputedValue(NavigationMenuActionNode.ActiveStateProperty);
      }
      set
      {
        this.SetValue(NavigationMenuActionNode.ActiveStateProperty, (object) value);
      }
    }

    public string InactiveState
    {
      get
      {
        return (string) this.GetComputedValue(NavigationMenuActionNode.InactiveStateProperty);
      }
      set
      {
        this.SetValue(NavigationMenuActionNode.InactiveStateProperty, (object) value);
      }
    }

    public string TargetScreen
    {
      get
      {
        return (string) this.GetComputedValue(NavigationMenuActionNode.TargetScreenProperty);
      }
      set
      {
        this.SetValue(NavigationMenuActionNode.TargetScreenProperty, (object) value);
      }
    }

    public class ConcreteNavigationMenuActionFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new NavigationMenuActionNode();
      }

      public NavigationMenuActionNode Instantiate(SceneViewModel viewModel)
      {
        return (NavigationMenuActionNode) this.Instantiate(viewModel, ProjectNeutralTypes.NavigationMenuAction);
      }
    }
  }
}
