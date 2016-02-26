// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.InvokeCommandActionNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class InvokeCommandActionNode : BehaviorTriggerActionNode
  {
    public static readonly IPropertyId CommandNameProperty = (IPropertyId) ProjectNeutralTypes.InvokeCommandAction.GetMember(MemberType.LocalProperty, "CommandName", MemberAccessTypes.Public);
    public static readonly IPropertyId CommandParameterProperty = (IPropertyId) ProjectNeutralTypes.InvokeCommandAction.GetMember(MemberType.LocalProperty, "CommandParameter", MemberAccessTypes.Public);
    public static readonly InvokeCommandActionNode.ConcreteInvokeCommandActionNodeFactory Factory = new InvokeCommandActionNode.ConcreteInvokeCommandActionNodeFactory();

    public string CommandName
    {
      get
      {
        return (string) this.GetComputedValue(InvokeCommandActionNode.CommandNameProperty);
      }
      set
      {
        this.SetValue(InvokeCommandActionNode.CommandNameProperty, (object) value);
      }
    }

    public class ConcreteInvokeCommandActionNodeFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new InvokeCommandActionNode();
      }

      public InvokeCommandActionNode Instantiate(SceneViewModel viewModel)
      {
        return (InvokeCommandActionNode) this.Instantiate(viewModel, ProjectNeutralTypes.BehaviorEventTriggerBase);
      }
    }
  }
}
