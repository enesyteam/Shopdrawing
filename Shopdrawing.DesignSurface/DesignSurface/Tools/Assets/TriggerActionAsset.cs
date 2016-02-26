// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Assets.TriggerActionAsset
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.Tools.Assets
{
  internal class TriggerActionAsset : TypeAsset
  {
    public TriggerActionAsset(IType type, string displayName, ExampleAssetInfo exampleInfo, string onDemandAssemblyFileName, AssemblyNameAndLocation[] resolvableAssemblyReferences)
      : base(type, displayName, exampleInfo, onDemandAssemblyFileName, resolvableAssemblyReferences)
    {
    }

    protected override IEnumerable<ISceneInsertionPoint> InternalFindInsertionPoints(SceneViewModel viewModel)
    {
      return Enumerable.Select<SceneElement, ISceneInsertionPoint>((IEnumerable<SceneElement>) viewModel.ElementSelectionSet.Selection, (Func<SceneElement, ISceneInsertionPoint>) (element => new BehaviorInsertionPointCreator(element).Create((object) this)));
    }

    protected override bool InternalCanCreateInstance(ISceneInsertionPoint insertionPoint)
    {
      if (!insertionPoint.Property.Equals((object) BehaviorHelper.BehaviorTriggersProperty))
        return false;
      return insertionPoint.CanInsert((ITypeId) this.Type);
    }

    protected override SceneNode InternalCreateInstance(ISceneInsertionPoint insertionPoint, Rect rect, OnCreateInstanceAction action)
    {
      if (!this.EnsureTypeReferenced(ProjectContext.GetProjectContext(insertionPoint.SceneNode.ProjectContext)))
        return (SceneNode) null;
      return TriggerActionAsset.CreateTrigger(insertionPoint.SceneNode, this.Type);
    }

    public static SceneNode CreateTrigger(SceneNode targetNode, IType triggerType)
    {
      ProjectContext projectContext = ProjectContext.GetProjectContext(targetNode.ProjectContext);
      BehaviorHelper.EnsureSystemWindowsInteractivityReferenced((ITypeResolver) projectContext);
      targetNode.DesignerContext.ViewUpdateManager.RebuildPostponedViews();
      using (SceneEditTransaction editTransaction = targetNode.ViewModel.CreateEditTransaction(StringTable.CreateTriggerActionUndoString))
      {
        SceneNode sceneNode1 = targetNode;
        ISceneNodeCollection<SceneNode> collectionForProperty = sceneNode1.GetCollectionForProperty(BehaviorHelper.BehaviorTriggersProperty);
        BehaviorTriggerBaseNode behaviorTriggerBaseNode = (BehaviorTriggerBaseNode) null;
        SceneViewModel viewModel = targetNode.ViewModel;
        object triggerAttribute = BehaviorHelper.CreateTriggerFromDefaultTriggerAttribute((IEnumerable) TypeUtilities.GetAttributes(triggerType.RuntimeType), targetNode.TargetType);
        if (triggerAttribute != null)
          behaviorTriggerBaseNode = (BehaviorTriggerBaseNode) viewModel.CreateSceneNode(triggerAttribute);
        if (behaviorTriggerBaseNode == null)
        {
          BehaviorEventTriggerNode eventTriggerNode = (BehaviorEventTriggerNode) sceneNode1.ViewModel.CreateSceneNode(ProjectNeutralTypes.BehaviorEventTrigger);
          string result;
          if (!PlatformNeutralAttributeHelper.TryGetAttributeValue<string>(targetNode.TargetType, PlatformTypes.DefaultEventAttribute, "Name", out result))
            result = "Loaded";
          eventTriggerNode.EventName = result;
          behaviorTriggerBaseNode = (BehaviorTriggerBaseNode) eventTriggerNode;
        }
        bool flag = false;
        viewModel.BehaviorSelectionSet.Clear();
        foreach (SceneNode sceneNode2 in (IEnumerable<SceneNode>) collectionForProperty)
        {
          if (BehaviorHelper.CompareTriggerNodes(behaviorTriggerBaseNode.DocumentNode as DocumentCompositeNode, sceneNode2.DocumentNode as DocumentCompositeNode))
          {
            behaviorTriggerBaseNode = (BehaviorTriggerBaseNode) sceneNode2;
            flag = true;
            break;
          }
        }
        if (!flag)
          collectionForProperty.Add((SceneNode) behaviorTriggerBaseNode);
        BehaviorTriggerActionNode triggerActionNode = (BehaviorTriggerActionNode) sceneNode1.ViewModel.CreateSceneNode((ITypeId) triggerType);
        if (projectContext.IsCapabilitySet(PlatformCapability.SupportsAttachingToRootElements) && ProjectNeutralTypes.BehaviorTargetedTriggerAction.IsAssignableFrom((ITypeId) triggerActionNode.Type) && targetNode.ViewModel.ActiveEditingContainer.Equals((object) targetNode))
        {
          IProperty property = projectContext.ResolveProperty(BehaviorTargetedTriggerActionNode.BehaviorTargetObjectProperty);
          if (property != null)
            BehaviorHelper.CreateAndSetElementNameBinding((IPropertyId) property, (SceneNode) triggerActionNode, targetNode);
        }
        behaviorTriggerBaseNode.Actions.Add((SceneNode) triggerActionNode);
        viewModel.BehaviorSelectionSet.SetSelection((BehaviorBaseNode) triggerActionNode);
        editTransaction.Commit();
        return (SceneNode) triggerActionNode;
      }
    }
  }
}
