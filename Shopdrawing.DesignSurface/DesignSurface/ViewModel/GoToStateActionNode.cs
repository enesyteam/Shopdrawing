// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.GoToStateActionNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class GoToStateActionNode : BehaviorTargetedTriggerActionNode
  {
    public static readonly IPropertyId StateNameProperty = (IPropertyId) ProjectNeutralTypes.GoToStateAction.GetMember(MemberType.LocalProperty, "StateName", MemberAccessTypes.Public);
    public static readonly GoToStateActionNode.ConcreteGoToStateActionNodeFactory Factory = new GoToStateActionNode.ConcreteGoToStateActionNodeFactory();

    public string StateName
    {
      get
      {
        return (string) this.GetComputedValue(GoToStateActionNode.StateNameProperty);
      }
      set
      {
        this.SetValue(GoToStateActionNode.StateNameProperty, (object) value);
      }
    }

    public static SceneNode FindTargetElement(BehaviorTargetedTriggerActionNode goToStateNode)
    {
      if (!string.IsNullOrEmpty(goToStateNode.TargetName))
        return BehaviorHelper.FindNamedElement((SceneNode) goToStateNode, goToStateNode.TargetName);
      return goToStateNode.ViewModel.ActiveEditingContainer;
    }

    public static DocumentNode FindTargetElement(DocumentNode node, bool resolveTargetName)
    {
      DocumentCompositeNode documentCompositeNode1 = node as DocumentCompositeNode;
      if (documentCompositeNode1 != null)
      {
        DocumentNode documentNode1 = documentCompositeNode1.Properties[BehaviorTargetedTriggerActionNode.BehaviorTargetNameProperty];
        string elementName = !resolveTargetName || documentNode1 == null || !DocumentNodeUtilities.IsResource(documentNode1) ? documentCompositeNode1.GetValueAsString(BehaviorTargetedTriggerActionNode.BehaviorTargetNameProperty) : DocumentPrimitiveNode.GetValueAsString(ExpressionEvaluator.EvaluateExpression(documentNode1));
        IProperty property1 = node.PlatformMetadata.ResolveProperty(BehaviorTargetedTriggerActionNode.BehaviorTargetObjectProperty);
        if (property1 != null)
        {
          DocumentNode documentNode2 = documentCompositeNode1.Properties[(IPropertyId) property1];
          if (documentNode2 != null)
          {
            if (DocumentNodeUtilities.IsMarkupExtension(documentNode2))
            {
              if (!DocumentNodeUtilities.IsBinding(documentNode2) || !GoToStateActionNode.CanResolveTargetFromBinding(documentNode2))
                return ExpressionEvaluator.EvaluateExpression(documentNode2);
              documentNode2 = GoToStateActionNode.ResolveTargetFromBinding(node, (DocumentCompositeNode) documentNode2);
            }
            return documentNode2;
          }
        }
        if (!string.IsNullOrEmpty(elementName))
          return BehaviorHelper.FindNamedElement(node, elementName);
        DocumentNode documentNode3;
        for (documentNode3 = (DocumentNode) documentCompositeNode1; documentNode3 != null; documentNode3 = (DocumentNode) documentNode3.Parent)
        {
          DocumentCompositeNode documentCompositeNode2 = documentNode3 as DocumentCompositeNode;
          IProperty property2 = node.TypeResolver.ResolveProperty(VisualStateManagerSceneNode.VisualStateGroupsProperty);
          if (property2 == null)
            return (DocumentNode) null;
          if (documentCompositeNode2 != null && documentCompositeNode2.Properties[(IPropertyId) property2] != null)
            break;
        }
        if (documentNode3 != null)
        {
          DocumentCompositeNode parent = documentNode3.Parent;
          if (parent != null && PlatformTypes.ICollection.IsAssignableFrom((ITypeId) parent.Type) && parent.Parent != null)
            return (DocumentNode) parent.Parent;
          return (DocumentNode) documentNode3.Parent;
        }
      }
      return (DocumentNode) null;
    }

    private static DocumentNode ResolveTargetFromBinding(DocumentNode actionNode, DocumentCompositeNode bindingNode)
    {
      string valueAsString = bindingNode.GetValueAsString(BindingSceneNode.ElementNameProperty);
      return BehaviorHelper.FindNamedElement(actionNode, valueAsString);
    }

    public static bool CanResolveTargetFromBinding(DocumentNode node)
    {
      if (node == null || !DocumentNodeUtilities.IsBinding(node))
        return false;
      DocumentCompositeNode documentCompositeNode = (DocumentCompositeNode) node;
      if (documentCompositeNode.Properties[BindingSceneNode.ElementNameProperty] != null)
        return documentCompositeNode.Properties[BindingSceneNode.PathProperty] == null;
      return false;
    }

    public class ConcreteGoToStateActionNodeFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new GoToStateActionNode();
      }

      public GoToStateActionNode Instantiate(SceneViewModel viewModel)
      {
        return (GoToStateActionNode) this.Instantiate(viewModel, ProjectNeutralTypes.GoToStateAction);
      }
    }
  }
}
