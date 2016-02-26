// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Timeline.DragDrop.DropBehaviorAction
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.Generic;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.Timeline.DragDrop
{
  public class DropBehaviorAction : DropAction<BehaviorBaseNode>
  {
    public DropBehaviorAction(BehaviorBaseNode behaviorNode, ISceneInsertionPoint insertionPoint)
      : base(behaviorNode, insertionPoint)
    {
    }

    protected override bool OnQueryCanDrop(TimelineDragDescriptor descriptor)
    {
      descriptor.DisableInBetween();
      if (!this.InsertionPoint.CanInsert((ITypeId) this.SourceData.Type))
        return false;
      if (descriptor.AllowCopy)
        descriptor.SetCopyInto(this.InsertionPoint);
      else
        descriptor.SetMoveInto(this.InsertionPoint);
      descriptor.TryReplace((object) this.SourceData, SmartInsertionPoint.From(this.InsertionPoint), this.DestinationCollection);
      return true;
    }

    protected override DragDropEffects OnHandleDrop(DragDropEffects dropEffects)
    {
      bool flag = (dropEffects & DragDropEffects.Copy) != DragDropEffects.None;
      using (SceneEditTransaction editTransaction = this.ViewModel.CreateEditTransaction(flag ? StringTable.UndoUnitCopy : StringTable.UndoUnitArrange))
      {
        this.ViewModel.ClearSelections();
        SceneNode sceneNode = !flag ? this.Move((SceneNode) this.SourceData) : this.Copy((SceneNode) this.SourceData);
        if (sceneNode != null)
        {
          this.ViewModel.SelectNodes((ICollection<SceneNode>) new SceneNode[1]
          {
            sceneNode
          });
          editTransaction.Commit();
        }
        else
        {
          editTransaction.Cancel();
          return DragDropEffects.None;
        }
      }
      return dropEffects;
    }

    private SceneNode Move(SceneNode node)
    {
      BehaviorTriggerActionNode actionNode = node as BehaviorTriggerActionNode;
      BehaviorNode behaviorNode = node as BehaviorNode;
      if (actionNode != null)
      {
        BehaviorTriggerBaseNode triggerNode = this.CopyTriggerAndActionNode(actionNode);
        this.DeleteBehaviorNode((BehaviorBaseNode) actionNode);
        return this.InsertTrigger(triggerNode);
      }
      if (behaviorNode == null)
        return (SceneNode) null;
      this.DeleteBehaviorNode((BehaviorBaseNode) behaviorNode);
      return this.InsertBehavior(behaviorNode);
    }

    private SceneNode Copy(SceneNode node)
    {
      BehaviorTriggerActionNode actionNode = node as BehaviorTriggerActionNode;
      BehaviorNode behaviorNode = node as BehaviorNode;
      if (actionNode != null)
        return this.InsertTrigger(this.CopyTriggerAndActionNode(actionNode));
      if (behaviorNode != null)
        return this.InsertBehavior((BehaviorNode) this.ViewModel.GetSceneNode(behaviorNode.DocumentNode.Clone(this.ViewModel.Document.DocumentContext)));
      return (SceneNode) null;
    }

    private void DeleteBehaviorNode(BehaviorBaseNode behaviorNode)
    {
      if (behaviorNode == null || !behaviorNode.IsAttached)
        return;
      BehaviorHelper.DeleteBehavior(behaviorNode);
    }

    private BehaviorTriggerBaseNode CopyTriggerAndActionNode(BehaviorTriggerActionNode actionNode)
    {
      SceneViewModel viewModel = this.InsertionPoint.SceneNode.ViewModel;
      DocumentNode node1 = actionNode.DocumentNode.Clone(viewModel.Document.DocumentContext);
      DocumentNode node2 = actionNode.Parent.DocumentNode.Clone(viewModel.Document.DocumentContext);
      BehaviorTriggerBaseNode behaviorTriggerBaseNode = (BehaviorTriggerBaseNode) viewModel.GetSceneNode(node2);
      BehaviorTriggerActionNode triggerActionNode = (BehaviorTriggerActionNode) viewModel.GetSceneNode(node1);
      behaviorTriggerBaseNode.Actions.Clear();
      behaviorTriggerBaseNode.Actions.Add((SceneNode) triggerActionNode);
      return behaviorTriggerBaseNode;
    }

    private SceneNode InsertTrigger(BehaviorTriggerBaseNode triggerNode)
    {
      BehaviorTriggerActionNode triggerActionNode1 = (BehaviorTriggerActionNode) triggerNode.Actions[0];
      DocumentCompositeNode documentCompositeNode = (DocumentCompositeNode) triggerNode.DocumentNode;
      IProjectContext projectContext = triggerNode.ProjectContext;
      SceneNode sceneNode = this.InsertionPoint.SceneNode;
      ISceneNodeCollection<SceneNode> collectionForProperty = sceneNode.GetCollectionForProperty(BehaviorHelper.BehaviorTriggersProperty);
      BehaviorTriggerBaseNode behaviorTriggerBaseNode = BehaviorHelper.FindMatchingTriggerNode((DocumentNode) documentCompositeNode, collectionForProperty);
      if (behaviorTriggerBaseNode != null)
      {
        BehaviorTriggerActionNode triggerActionNode2 = (BehaviorTriggerActionNode) this.ViewModel.GetSceneNode(triggerActionNode1.DocumentNode.Clone(this.ViewModel.Document.DocumentContext));
        behaviorTriggerBaseNode.Actions.Add((SceneNode) triggerActionNode2);
        triggerActionNode1 = triggerActionNode2;
      }
      else
      {
        behaviorTriggerBaseNode = triggerNode;
        collectionForProperty.Add((SceneNode) behaviorTriggerBaseNode);
      }
      if (projectContext.IsCapabilitySet(PlatformCapability.SupportsAttachingToRootElements) && sceneNode.ViewModel.ActiveEditingContainer.Equals((object) sceneNode))
      {
        IProperty property = projectContext.ResolveProperty(BehaviorTargetedTriggerActionNode.BehaviorTargetObjectProperty);
        if (property != null)
        {
          foreach (BehaviorTriggerActionNode triggerActionNode2 in (IEnumerable<SceneNode>) behaviorTriggerBaseNode.Actions)
          {
            if (ProjectNeutralTypes.BehaviorTargetedTriggerAction.IsAssignableFrom((ITypeId) triggerActionNode2.Type))
              BehaviorHelper.CreateAndSetElementNameBinding((IPropertyId) property, (SceneNode) triggerActionNode2, sceneNode);
          }
        }
      }
      BehaviorEventTriggerNode eventTriggerNode = behaviorTriggerBaseNode as BehaviorEventTriggerNode;
      if (eventTriggerNode != null)
        BehaviorEventTriggerNode.FixUpEventName(eventTriggerNode);
      return (SceneNode) triggerActionNode1;
    }

    private SceneNode InsertBehavior(BehaviorNode behaviorNode)
    {
      SmartInsertionPoint smartInsertionPoint = SmartInsertionPoint.From(this.InsertionPoint);
      if (smartInsertionPoint == null)
        return (SceneNode) null;
      smartInsertionPoint.Insert((SceneNode) behaviorNode);
      return (SceneNode) behaviorNode;
    }
  }
}
