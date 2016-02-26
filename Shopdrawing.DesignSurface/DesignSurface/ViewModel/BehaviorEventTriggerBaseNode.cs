// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.BehaviorEventTriggerBaseNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class BehaviorEventTriggerBaseNode : BehaviorTriggerBaseNode
  {
    public static readonly IPropertyId BehaviorSourceNameProperty = (IPropertyId) ProjectNeutralTypes.BehaviorEventTriggerBase.GetMember(MemberType.LocalProperty, "SourceName", MemberAccessTypes.Public);
    public static readonly IPropertyId BehaviorSourceObjectProperty = (IPropertyId) ProjectNeutralTypes.BehaviorEventTriggerBase.GetMember(MemberType.LocalProperty, "SourceObject", MemberAccessTypes.Public);
    public static readonly BehaviorEventTriggerBaseNode.ConcreteBehaviorEventTriggerBaseNodeFactory Factory = new BehaviorEventTriggerBaseNode.ConcreteBehaviorEventTriggerBaseNodeFactory();

    public string SourceName
    {
      get
      {
        if (this.IsAttached)
          return (string) this.GetComputedValue(BehaviorEventTriggerBaseNode.BehaviorSourceNameProperty);
        return (string) null;
      }
      set
      {
        DocumentNode documentNode = (DocumentNode) null;
        if (!string.IsNullOrEmpty(value))
          documentNode = this.CreateNode((object) value);
        this.DocumentCompositeNode.Properties[BehaviorEventTriggerBaseNode.BehaviorSourceNameProperty] = documentNode;
      }
    }

    public SceneNode SourceObject
    {
      get
      {
        object computedValue = this.GetComputedValue(BehaviorEventTriggerBaseNode.BehaviorSourceObjectProperty);
        if (computedValue == null)
          return (SceneNode) null;
        DocumentNode correspondingDocumentNode = this.ViewModel.DefaultView.GetCorrespondingDocumentNode(this.Platform.ViewObjectFactory.Instantiate(computedValue), true);
        if (correspondingDocumentNode != null)
          return this.ViewModel.GetSceneNode(correspondingDocumentNode);
        return this.ViewModel.CreateSceneNode(computedValue);
      }
    }

    public SceneNode SourceNode
    {
      get
      {
        return this.SourceObject ?? BehaviorHelper.FindNamedElement((SceneNode) this, this.SourceName);
      }
    }

    public class ConcreteBehaviorEventTriggerBaseNodeFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new BehaviorEventTriggerBaseNode();
      }

      public BehaviorEventTriggerBaseNode Instantiate(SceneViewModel viewModel)
      {
        return (BehaviorEventTriggerBaseNode) this.Instantiate(viewModel, ProjectNeutralTypes.BehaviorEventTriggerBase);
      }
    }
  }
}
