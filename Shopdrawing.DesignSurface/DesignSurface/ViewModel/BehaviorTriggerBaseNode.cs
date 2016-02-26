// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.BehaviorTriggerBaseNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class BehaviorTriggerBaseNode : SceneNode
  {
    public static readonly IPropertyId BehaviorActionsProperty = (IPropertyId) ProjectNeutralTypes.BehaviorTriggerBase.GetMember(MemberType.LocalProperty, "Actions", MemberAccessTypes.Public);
    public static readonly BehaviorTriggerBaseNode.ConcreteBehaviorTriggerBaseNodeFactory Factory = new BehaviorTriggerBaseNode.ConcreteBehaviorTriggerBaseNodeFactory();

    public ISceneNodeCollection<SceneNode> Actions
    {
      get
      {
        return this.GetCollectionForProperty(BehaviorTriggerBaseNode.BehaviorActionsProperty);
      }
    }

    protected DocumentCompositeNode DocumentCompositeNode
    {
      get
      {
        return (DocumentCompositeNode) this.DocumentNode;
      }
    }

    protected override void OnChildAdding(SceneNode child)
    {
      if (this.IsAttached)
        this.ViewModel.OnNodeAdding((SceneNode) this, child);
      base.OnChildAdding(child);
    }

    public class ConcreteBehaviorTriggerBaseNodeFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new BehaviorTriggerBaseNode();
      }

      public BehaviorTriggerBaseNode Instantiate(SceneViewModel viewModel)
      {
        return (BehaviorTriggerBaseNode) this.Instantiate(viewModel, ProjectNeutralTypes.BehaviorTriggerBase);
      }
    }
  }
}
