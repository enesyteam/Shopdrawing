// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.TriggerObjectSet
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.UserInterface.ResourcePane;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.PropertyInspector;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public class TriggerObjectSet : SceneNodeObjectSetBase
  {
    private BehaviorTriggerActionNode actionNode;

    public override SceneNode[] Objects
    {
      get
      {
        if (this.DocumentContext == null || this.actionNode == null || (this.actionNode.ViewModel == null || !this.actionNode.ViewModel.IsEditable) || this.actionNode.Parent == null)
          return new SceneNode[0];
        return new SceneNode[1]
        {
          this.actionNode.Parent
        };
      }
    }

    public override SceneViewModel ViewModel
    {
      get
      {
        if (this.actionNode == null)
          return (SceneViewModel) null;
        return this.actionNode.ViewModel;
      }
    }

    public override ObservableCollection<LocalResourceModel> LocalResources
    {
      get
      {
        return (ObservableCollection<LocalResourceModel>) this.ProvideLocalResources(new List<ResourceContainer>(this.DesignerContext.ResourceManager.ActiveResourceContainers));
      }
    }

    private BehaviorTriggerBaseNode ParentTrigger
    {
      get
      {
        return (BehaviorTriggerBaseNode) this.actionNode.Parent;
      }
    }

    private SceneNode HostElement
    {
      get
      {
        return this.ParentTrigger.Parent;
      }
    }

    private ISceneNodeCollection<SceneNode> TriggersCollection
    {
      get
      {
        return this.HostElement.GetCollectionForProperty(BehaviorHelper.BehaviorTriggersProperty);
      }
    }

    public override bool IsHomogenous
    {
      get
      {
        return true;
      }
    }

    public override bool ShouldWalkParentsForGetValue
    {
      get
      {
        return false;
      }
    }

    public override bool ShouldAllowAnimation
    {
      get
      {
        return false;
      }
    }

    public override bool IsValidForUpdate
    {
      get
      {
        return true;
      }
    }

    public TriggerObjectSet(BehaviorTriggerActionNode actionNode)
      : base(actionNode.DesignerContext, (IPropertyInspector) actionNode.DesignerContext.PropertyInspectorModel)
    {
      this.actionNode = actionNode;
    }

    public override void RegisterPropertyChangedHandler(PropertyReference propertyReference, Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler handler)
    {
      if (this.DesignerContext.PropertyManager == null || PlatformTypes.IList.IsAssignableFrom((ITypeId) propertyReference.LastStep.PropertyType))
        return;
      this.DesignerContext.PropertyManager.RegisterPropertyReferenceChangedHandler(propertyReference, handler, true);
    }

    public override void UnregisterPropertyChangedHandler(PropertyReference propertyReference, Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler handler)
    {
      if (this.DesignerContext.PropertyManager == null)
        return;
      this.DesignerContext.PropertyManager.UnregisterPropertyReferenceChangedHandler(propertyReference, handler);
    }

    private BehaviorTriggerBaseNode GetUpdatedParentNode(PropertyReference propertyReference, object valueToSet, Modification modification)
    {
      BehaviorTriggerBaseNode behaviorTriggerBaseNode = this.FindExistingTriggerMatchingPropertyChange(propertyReference, valueToSet, modification);
      if (behaviorTriggerBaseNode == null)
      {
        if (this.ParentTrigger.Actions.Count <= 1)
          return (BehaviorTriggerBaseNode) null;
        behaviorTriggerBaseNode = BehaviorHelper.CloneTrigger(this.ParentTrigger, this.ViewModel);
      }
      return behaviorTriggerBaseNode;
    }

    internal override SceneEditTransaction PrepareTreeForModifyValue(PropertyReference propertyReference, object valueToSet, Modification modification, out bool treeModified)
    {
      BehaviorTriggerBaseNode updatedParentNode = this.GetUpdatedParentNode(propertyReference, valueToSet, modification);
      if (updatedParentNode == null)
      {
        treeModified = false;
        return (SceneEditTransaction) null;
      }
      SceneEditTransaction transaction = this.CreateTransaction(propertyReference.LastStep.Name);
      this.ReparentActionAndCopyBehaviors(updatedParentNode);
      treeModified = true;
      return transaction;
    }

    private SceneEditTransaction CreateTransaction(string propertyName)
    {
      return this.ViewModel.CreateEditTransaction(string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.PropertyChangeUndoDescription, new object[1]
      {
        (object) propertyName
      }));
    }

    protected override void ModifyValue(PropertyReferenceProperty property, object valueToSet, Modification modification, int index)
    {
      BehaviorTriggerBaseNode updatedParentNode = this.GetUpdatedParentNode(property.Reference, valueToSet, modification);
      if (updatedParentNode == null)
      {
        base.ModifyValue(property, valueToSet, modification, index);
      }
      else
      {
        using (SceneEditTransaction transaction = this.CreateTransaction(property.PropertyName))
        {
          this.ReparentActionAndCopyBehaviors(updatedParentNode);
          base.ModifyValue(property, valueToSet, modification, index);
          transaction.Commit();
        }
      }
    }

    internal void ReparentActionAndCopyBehaviors(BehaviorTriggerBaseNode newTriggerParent)
    {
      SceneNode valueAsSceneNode = this.ParentTrigger.GetLocalValueAsSceneNode(BehaviorHelper.BehaviorsProperty);
      if (valueAsSceneNode != null)
      {
        DocumentNode node = valueAsSceneNode.DocumentNode.Clone(valueAsSceneNode.DocumentContext);
        SceneNode sceneNode = newTriggerParent.ViewModel.GetSceneNode(node);
        newTriggerParent.SetValueAsSceneNode(BehaviorHelper.BehaviorsProperty, sceneNode);
      }
      BehaviorHelper.ReparentAction(this.TriggersCollection, this.ParentTrigger, newTriggerParent, this.actionNode);
    }

    private BehaviorTriggerBaseNode FindExistingTriggerMatchingPropertyChange(PropertyReference propertyReference, object newValue, Modification modification)
    {
      SceneNode sceneNode = this.ViewModel.GetSceneNode(this.ParentTrigger.DocumentNode.Clone(this.DocumentContext));
      switch (modification)
      {
        case Modification.SetValue:
          sceneNode.SetValue(propertyReference, newValue);
          break;
        case Modification.ClearValue:
          sceneNode.ClearValue(propertyReference);
          break;
      }
      return this.FindExistingTriggerMatchingDocumentNode(sceneNode.DocumentNode);
    }

    internal BehaviorTriggerBaseNode FindExistingTriggerMatchingDocumentNode(DocumentNode candidate)
    {
      return BehaviorHelper.FindMatchingTriggerNode(candidate, this.TriggersCollection);
    }
  }
}
