// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.BehaviorTargetedTriggerActionNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
    public class BehaviorTargetedTriggerActionNode : BehaviorTriggerActionNode
    {
        public static readonly IPropertyId BehaviorTargetNameProperty = (IPropertyId)ProjectNeutralTypes.BehaviorTargetedTriggerAction.GetMember(MemberType.LocalProperty, "TargetName", MemberAccessTypes.Public);
        public static readonly IPropertyId BehaviorTargetObjectProperty = (IPropertyId)ProjectNeutralTypes.BehaviorTargetedTriggerAction.GetMember(MemberType.LocalProperty, "TargetObject", MemberAccessTypes.Public);
        public static readonly BehaviorTargetedTriggerActionNode.ConcreteBehaviorTargetedTriggerActionNodeFactory Factory = new BehaviorTargetedTriggerActionNode.ConcreteBehaviorTargetedTriggerActionNodeFactory();

        public string TargetName
        {
            get
            {
                return (string)this.GetComputedValue(BehaviorTargetedTriggerActionNode.BehaviorTargetNameProperty);
            }
            set
            {
                DocumentNode documentNode = (DocumentNode)null;
                if (!string.IsNullOrEmpty(value))
                    documentNode = this.CreateNode((object)value);
                this.DocumentCompositeNode.Properties[BehaviorTargetedTriggerActionNode.BehaviorTargetNameProperty] = documentNode;
            }
        }

        public SceneNode TargetObject
        {
            get
            {
                object computedValue = this.GetComputedValue(BehaviorTargetedTriggerActionNode.BehaviorTargetObjectProperty);
                if (computedValue == null)
                    return (SceneNode)null;
                DocumentNode correspondingDocumentNode = this.ViewModel.DefaultView.GetCorrespondingDocumentNode(this.Platform.ViewObjectFactory.Instantiate(computedValue), true);
                if (correspondingDocumentNode != null)
                    return this.ViewModel.GetSceneNode(correspondingDocumentNode);
                return this.ViewModel.CreateSceneNode(computedValue);
            }
        }

        public SceneNode TargetNode
        {
            get
            {
                return this.TargetObject ?? BehaviorHelper.FindNamedElement((SceneNode)this, this.TargetName);
            }
        }

        public class ConcreteBehaviorTargetedTriggerActionNodeFactory : SceneNode.ConcreteSceneNodeFactory
        {
            protected override SceneNode Instantiate()
            {
                return (SceneNode)new BehaviorTargetedTriggerActionNode();
            }

            public BehaviorTargetedTriggerActionNode Instantiate(SceneViewModel viewModel)
            {
                return (BehaviorTargetedTriggerActionNode)this.Instantiate(viewModel, ProjectNeutralTypes.BehaviorTargetedTriggerAction);
            }
        }
    }
}
