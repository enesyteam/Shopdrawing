// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.BehaviorHelper
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector;
using Microsoft.Expression.Project;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public static class BehaviorHelper
  {
    public static readonly IPropertyId BehaviorTriggersProperty = (IPropertyId) ProjectNeutralTypes.Interaction.GetMember(MemberType.AttachedProperty, "Triggers", MemberAccessTypes.Public);
    public static readonly IPropertyId BehaviorsProperty = (IPropertyId) ProjectNeutralTypes.Interaction.GetMember(MemberType.AttachedProperty, "Behaviors", MemberAccessTypes.Public);

    public static bool IsPropertyBehaviorCommand(PropertyReferenceProperty property)
    {
      return BehaviorHelper.IsPropertyBehaviorCommand(property.DeclaringTypeId, property.PropertyTypeId);
    }

    public static bool IsPropertyBehaviorCommand(IProperty property)
    {
      return BehaviorHelper.IsPropertyBehaviorCommand(property.DeclaringType, property.PropertyType);
    }

    private static bool IsPropertyBehaviorCommand(IType declaringType, IType propertyType)
    {
      if (ProjectNeutralTypes.Behavior.IsAssignableFrom((ITypeId) declaringType))
        return PlatformTypes.ICommand.IsAssignableFrom((ITypeId) propertyType);
      return false;
    }

    internal static bool EnsureBlendSDKLibraryAssemblyReferenced(SceneViewModel sceneViewModel, string assemblyName)
    {
      if (sceneViewModel == null || string.IsNullOrEmpty(assemblyName))
        return false;
      if (sceneViewModel.ProjectContext.GetAssembly(assemblyName) != null)
        return true;
      if (!BlendSdkHelper.IsSdkInstalled(sceneViewModel.ProjectContext.TargetFramework) || !sceneViewModel.ProjectContext.EnsureAssemblyReferenced(assemblyName))
        return false;
      sceneViewModel.DesignerContext.ViewUpdateManager.RebuildPostponedViews();
      return true;
    }

    internal static void EnsureSystemWindowsInteractivityReferenced(ITypeResolver typeResolver)
    {
      IType type = typeResolver.ResolveType(BehaviorHelper.BehaviorsProperty.DeclaringTypeId);
      ProjectXamlContext projectXamlContext = typeResolver as ProjectXamlContext;
      Uri result;
      if (typeResolver.PlatformMetadata.IsNullType((ITypeId) type) || projectXamlContext == null || !Uri.TryCreate(type.RuntimeType.Assembly.CodeBase, UriKind.Absolute, out result))
        return;
      projectXamlContext.EnsureAssemblyReferenced(result.LocalPath);
    }

    internal static object CreateTriggerFromDefaultTriggerAttribute(IEnumerable attributes, Type targetType)
    {
      List<Type> results1;
      List<Type> results2;
      List<object[]> results3;
      if (PlatformNeutralAttributeHelper.TryGetAttributeValues<Type>(attributes, ProjectNeutralTypes.DefaultTriggerAttribute, "TargetType", out results1) && PlatformNeutralAttributeHelper.TryGetAttributeValues<Type>(attributes, ProjectNeutralTypes.DefaultTriggerAttribute, "TriggerType", out results2) && PlatformNeutralAttributeHelper.TryGetAttributeValues<object[]>(attributes, ProjectNeutralTypes.DefaultTriggerAttribute, "Parameters", out results3))
      {
        int index1 = -1;
        for (int index2 = 0; index2 < results1.Count; ++index2)
        {
          Type c = results1[index2];
          if (c.IsAssignableFrom(targetType) && (index1 < 0 || results1[index1].IsAssignableFrom(c) && !c.IsAssignableFrom(results1[index1])))
            index1 = index2;
        }
        if (index1 >= 0)
        {
          try
          {
            return Activator.CreateInstance(results2[index1], results3[index1]);
          }
          catch
          {
          }
        }
      }
      return (object) null;
    }

    public static PropertyValueEditor GetCustomPropertyValueEditor(PropertyReferenceProperty property)
    {
      PropertyValueEditor propertyValueEditor = (PropertyValueEditor) null;
      object result;
      if (PlatformNeutralAttributeHelper.TryGetAttributeValue<object>((IEnumerable) property.Attributes, ProjectNeutralTypes.CustomPropertyValueEditorAttribute, "CustomPropertyValueEditor", out result))
      {
        switch (Enum.GetName(property.PropertyTypeId.PlatformMetadata.ResolveType(ProjectNeutralTypes.CustomPropertyValueEditor).RuntimeType, result))
        {
          case "Element":
            propertyValueEditor = (PropertyValueEditor) new ElementPickerPropertyValueEditor();
            break;
          case "Storyboard":
            propertyValueEditor = (PropertyValueEditor) new StoryboardPickerPropertyValueEditor();
            break;
          case "StateName":
            propertyValueEditor = (PropertyValueEditor) new StatePickerPropertyValueEditor();
            break;
          case "ElementBinding":
            propertyValueEditor = (PropertyValueEditor) new ElementBindingPickerPropertyValueEditor();
            break;
          case "PropertyBinding":
            propertyValueEditor = (PropertyValueEditor) new PropertyBindingPickerPropertyValueEditor();
            break;
        }
      }
      return propertyValueEditor;
    }

    public static SceneNode FindNamedElement(SceneNode behaviorNode, string elementName)
    {
      if (behaviorNode != null)
      {
        DocumentNode namedElement = BehaviorHelper.FindNamedElement(behaviorNode.DocumentNode, elementName);
        if (namedElement != null)
          return behaviorNode.ViewModel.GetSceneNode(namedElement);
      }
      return (SceneNode) null;
    }

    public static DocumentNode FindNamedElement(DocumentNode behaviorNode, string elementName)
    {
      if (!string.IsNullOrEmpty(elementName))
      {
        DocumentNodeNameScope containingNameScope = behaviorNode.FindContainingNameScope();
        if (containingNameScope == null)
          return (DocumentNode) null;
        DocumentNode node = containingNameScope.FindNode(elementName);
        if (node != null)
          return node;
      }
      else
      {
        if (ProjectNeutralTypes.BehaviorTriggerAction.IsAssignableFrom((ITypeId) behaviorNode.Type))
          return BehaviorHelper.ValidateNodeTypeAndGetParent(BehaviorHelper.ValidateNodeTypeAndGetParent(BehaviorHelper.ValidateNodeTypeAndGetParent((DocumentNode) behaviorNode.Parent, ProjectNeutralTypes.BehaviorTriggerActionCollection), ProjectNeutralTypes.BehaviorTriggerBase), ProjectNeutralTypes.BehaviorTriggerCollection);
        if (ProjectNeutralTypes.BehaviorTriggerBase.IsAssignableFrom((ITypeId) behaviorNode.Type))
          return BehaviorHelper.ValidateNodeTypeAndGetParent((DocumentNode) behaviorNode.Parent, ProjectNeutralTypes.BehaviorTriggerCollection);
      }
      return (DocumentNode) null;
    }

    public static void DeleteBehavior(BehaviorBaseNode node)
    {
      Stack<ReferenceStep> input = new Stack<ReferenceStep>();
      SceneNode parent1 = node.Parent;
      SceneNode child1 = (SceneNode) node;
      while (true)
      {
        ReferenceStep referenceStep = (ReferenceStep) parent1.GetPropertyForChild(child1);
        if (parent1.IsCollectionProperty((IPropertyId) referenceStep))
        {
          int index = parent1.GetCollectionForProperty((IPropertyId) referenceStep).IndexOf(child1);
          input.Push((ReferenceStep) IndexedClrPropertyReferenceStep.GetReferenceStep((ITypeResolver) node.ProjectContext, (ITypeId) referenceStep.PropertyType, index));
        }
        input.Push(referenceStep);
        if (!(parent1 is SceneElement))
        {
          child1 = parent1;
          parent1 = parent1.Parent;
        }
        else
          break;
      }
      SceneElement sceneElement = (SceneElement) parent1;
      SceneNode parent2 = node.Parent;
      PropertyReference propertyReference = new PropertyReference(input);
      node.ViewModel.AnimationEditor.DeleteAllAnimations((SceneNode) sceneElement, propertyReference.ToString());
      SceneNode child2 = (SceneNode) node;
      int num = propertyReference.Count - 1;
      ISceneNodeCollection<SceneNode> collectionForChild;
      int endIndex;
      while (true)
      {
        SceneNode parent3 = child2.Parent;
        if (sceneElement != child2 && parent3 != null)
        {
          ReferenceStep referenceStep = (ReferenceStep) parent3.GetPropertyForChild(child2);
          collectionForChild = parent3.GetCollectionForChild(child2);
          DocumentNodeHelper.PreserveFormatting(child2.DocumentNode);
          endIndex = propertyReference.ReferenceSteps.IndexOf(referenceStep);
          if (collectionForChild.Count == 1)
          {
            child2.Remove();
            child2 = parent3;
          }
          else
            goto label_10;
        }
        else
          break;
      }
      sceneElement.ClearValue((IPropertyId) propertyReference.FirstStep);
      goto label_11;
label_10:
      int index1 = collectionForChild.IndexOf(child2);
      child2.ViewModel.AnimationEditor.ValidateAnimations((SceneNode) sceneElement, propertyReference.Subreference(0, endIndex), index1, false);
      child2.Remove();
label_11:
      node.ViewModel.Document.OnUpdatedEditTransaction();
    }

    public static bool IsSceneNodeValidHost(SceneNode candidateNode, IType constrainedIType)
    {
      if (ProjectNeutralTypes.BehaviorEventTriggerBase.IsAssignableFrom((ITypeId) constrainedIType) || ProjectNeutralTypes.BehaviorTargetedTriggerAction.IsAssignableFrom((ITypeId) constrainedIType))
      {
        if (!candidateNode.Platform.Metadata.IsCapabilitySet(PlatformCapability.SupportsAttachingToRootElements) && candidateNode.ViewModel.ActiveEditingContainer.Equals((object) candidateNode))
          return false;
        Type result;
        if (PlatformNeutralAttributeHelper.TryGetAttributeValue<Type>((IEnumerable) TypeUtilities.GetAttributes(constrainedIType.RuntimeType), ProjectNeutralTypes.TypeConstraintAttribute, "Constraint", out result))
          return candidateNode.ProjectContext.GetType(result).IsAssignableFrom((ITypeId) candidateNode.Type);
        return true;
      }
      Type runtimeType1 = candidateNode.ProjectContext.ResolveType(ProjectNeutralTypes.Behavior).RuntimeType;
      Type runtimeType2 = candidateNode.ProjectContext.ResolveType(ProjectNeutralTypes.BehaviorTriggerBase).RuntimeType;
      Type runtimeType3 = candidateNode.ProjectContext.ResolveType(ProjectNeutralTypes.BehaviorTriggerAction).RuntimeType;
      Type type = constrainedIType.RuntimeType;
      while (type != (Type) null && type.BaseType != (Type) null && (!type.BaseType.Equals(runtimeType1) && !type.BaseType.Equals(runtimeType2)) && !type.BaseType.Equals(runtimeType3))
        type = type.BaseType;
      Type[] genericArguments = type.GetGenericArguments();
      return candidateNode.ProjectContext.GetType(genericArguments[0]).IsAssignableFrom((ITypeId) candidateNode.Type);
    }

    public static BehaviorTriggerBaseNode FindMatchingTriggerNode(DocumentNode candidate, ISceneNodeCollection<SceneNode> triggerNodes)
    {
      DocumentCompositeNode triggerNode1 = candidate as DocumentCompositeNode;
      if (triggerNode1 != null)
      {
        foreach (BehaviorTriggerBaseNode behaviorTriggerBaseNode in (IEnumerable<SceneNode>) triggerNodes)
        {
          if (behaviorTriggerBaseNode.Type.Equals((object) triggerNode1.Type))
          {
            DocumentCompositeNode triggerNode2 = (DocumentCompositeNode) behaviorTriggerBaseNode.DocumentNode;
            if (!object.ReferenceEquals((object) triggerNode2, (object) candidate) && BehaviorHelper.CompareTriggerNodes(triggerNode1, triggerNode2))
              return behaviorTriggerBaseNode;
          }
        }
      }
      return (BehaviorTriggerBaseNode) null;
    }

    public static bool CompareTriggerNodes(DocumentCompositeNode triggerNode1, DocumentCompositeNode triggerNode2)
    {
      IProperty propertyKey = triggerNode1.TypeResolver.ResolveProperty(BehaviorTriggerBaseNode.BehaviorActionsProperty);
      bool flag = false;
      int num1 = 0;
      if (triggerNode1.Properties.Count <= triggerNode2.Properties.Count)
      {
        flag = true;
        foreach (KeyValuePair<IProperty, DocumentNode> keyValuePair in (IEnumerable<KeyValuePair<IProperty, DocumentNode>>) triggerNode2.Properties)
        {
          IProperty key = keyValuePair.Key;
          DocumentNode documentNode = triggerNode1.Properties[(IPropertyId) key];
          if (!key.Equals((object) propertyKey))
          {
            if (documentNode == null || !documentNode.Equals(keyValuePair.Value))
            {
              flag = false;
              break;
            }
            ++num1;
          }
        }
      }
      if (!flag)
        return false;
      int num2 = triggerNode1.Properties.Contains(propertyKey) ? triggerNode1.Properties.Count - 1 : triggerNode1.Properties.Count;
      return num1 == num2;
    }

    public static void CreateAndSetElementNameBinding(IPropertyId property, SceneNode node, SceneNode nodeToTarget)
    {
      BehaviorHelper.CreateAndSetElementNameBinding(property.Name, nodeToTarget, (Action<BindingSceneNode>) (bindingNode => node.SetValueAsSceneNode(property, (SceneNode) bindingNode)));
    }

    public static void CreateAndSetElementNameBinding(SceneNodeProperty sceneNodeProperty, SceneNode nodeToTarget)
    {
      BehaviorHelper.CreateAndSetElementNameBinding(sceneNodeProperty.PropertyName, nodeToTarget, (Action<BindingSceneNode>) (bindingNode => sceneNodeProperty.SetValue((object) bindingNode.DocumentNode)));
    }

    private static void CreateAndSetElementNameBinding(string propertyName, SceneNode node, Action<BindingSceneNode> setValue)
    {
      string description = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.PropertyChangeUndoDescription, new object[1]
      {
        (object) propertyName
      });
      SceneViewModel viewModel = node.ViewModel;
      using (SceneEditTransaction editTransaction = viewModel.CreateEditTransaction(description))
      {
        node.EnsureNamed();
        editTransaction.Update();
        BindingSceneNode bindingSceneNode = viewModel.CreateSceneNode(PlatformTypes.Binding) as BindingSceneNode;
        bindingSceneNode.ElementName = node.Name;
        setValue(bindingSceneNode);
        editTransaction.Commit();
      }
    }

    internal static DocumentNode ValidateNodeTypeAndGetParent(DocumentNode childNode, ITypeId type)
    {
      if (childNode != null && type.IsAssignableFrom((ITypeId) childNode.Type))
        return (DocumentNode) childNode.Parent;
      return (DocumentNode) null;
    }

    internal static BehaviorTriggerBaseNode CloneTrigger(BehaviorTriggerBaseNode trigger, SceneViewModel viewModel)
    {
      DocumentCompositeNode documentCompositeNode = (DocumentCompositeNode) trigger.DocumentNode.Clone(trigger.DocumentContext);
      documentCompositeNode.Properties[BehaviorTriggerBaseNode.BehaviorActionsProperty] = (DocumentNode) null;
      return (BehaviorTriggerBaseNode) viewModel.GetSceneNode((DocumentNode) documentCompositeNode);
    }

    internal static void ReparentAction(ISceneNodeCollection<SceneNode> triggersCollection, BehaviorTriggerBaseNode oldTrigger, BehaviorTriggerBaseNode newTrigger, BehaviorTriggerActionNode action)
    {
      SceneViewModel viewModel = oldTrigger.ViewModel;
      viewModel.BehaviorSelectionSet.Clear();
      int num = triggersCollection.IndexOf((SceneNode) oldTrigger);
      DocumentNodeHelper.PreserveFormatting(action.DocumentNode);
      oldTrigger.Actions.Remove((SceneNode) action);
      viewModel.Document.OnUpdatedEditTransaction();
      if (oldTrigger.Actions.Count == 0)
        triggersCollection.RemoveAt(num--);
      if (!triggersCollection.Contains((SceneNode) newTrigger))
        triggersCollection.Insert(num + 1, (SceneNode) newTrigger);
      newTrigger.Actions.Add((SceneNode) action);
      viewModel.BehaviorSelectionSet.SetSelection((BehaviorBaseNode) action);
    }
  }
}
