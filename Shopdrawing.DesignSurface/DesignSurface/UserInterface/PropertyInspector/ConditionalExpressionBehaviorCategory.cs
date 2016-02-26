// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.ConditionalExpressionBehaviorCategory
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.Selection;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.PropertyInspector;
using Microsoft.Windows.Design.PropertyEditing;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public sealed class ConditionalExpressionBehaviorCategory : SceneNodeCategory
  {
    private SelectionManager listeningToSelectionManager;
    private ConditionBehaviorNode conditionBehaviorNode;
    private ConditionalExpressionNode conditionalExpressionNode;
    private BehaviorTriggerBaseNode behaviorTriggerBaseNode;
    private ISceneNodeCollection<SceneNode> conditions;
    private SceneViewModel sceneViewModel;
    private TriggerObjectSet objectSet;
    private bool keepPanelOpened;

    private ObservableCollection<PropertyEntry> ConditionNodes { get; set; }

    public override bool IsEmpty
    {
      get
      {
        return this.objectSet == null;
      }
    }

    public ICollectionView ConditionsNodeView { get; private set; }

    public bool ForwardChainingSetToOrEnabled
    {
      get
      {
        return this.HasConditions;
      }
    }

    public bool ConditionUIAvailable { get; private set; }

    public bool ForwardChainingSetToOr
    {
      get
      {
        if (this.conditionalExpressionNode != null)
          return this.conditionalExpressionNode.ForwardChainingToOr;
        return false;
      }
      set
      {
        using (SceneEditTransaction editTransaction = this.sceneViewModel.CreateEditTransaction(StringTable.ChangeConditionEvaluationTypeUndo))
        {
          this.conditionalExpressionNode.ForwardChainingToOr = value;
          editTransaction.Commit();
        }
        this.OnPropertyChanged("ForwardChainingSetToOr");
        this.OnPropertyChanged("ForwardChainingSetToOrEnabled");
      }
    }

    public override CategoryHelpContext CategoryHelpContext
    {
      get
      {
        return new CategoryHelpContext((object) CategoryNames.CategoryConditions.ToString(), StringTable.ConditionalContextualHelpToolTip);
      }
    }

    public bool HasConditions
    {
      get
      {
        if (this.conditions != null)
          return this.conditions.Count > 0;
        return false;
      }
    }

    public ConditionalExpressionBehaviorCategory(string localizedName, IMessageLoggingService messageLogger)
      : base(CategoryLocalizationHelper.CategoryName.BehaviorCommand, localizedName, messageLogger)
    {
      this.ConditionNodes = new ObservableCollection<PropertyEntry>();
      this.ConditionsNodeView = CollectionViewSource.GetDefaultView((object) this.ConditionNodes);
    }

    public override bool? QueryExpandedState(bool inspectingUIElement)
    {
      if (inspectingUIElement)
        return new bool?();
      if ((this.ConditionNodes == null || this.ConditionNodes.Count <= 0) && !this.keepPanelOpened)
        return new bool?(false);
      this.keepPanelOpened = false;
      return new bool?(true);
    }

    public override void OnSelectionChanged(SceneNode[] selectedObjects)
    {
      this.CleanBehaviorTriggerReferences();
      this.UnhookSelectionManagerEarlySceneUpdate();
      BehaviorTriggerActionNode actionNode = Enumerable.FirstOrDefault<BehaviorTriggerActionNode>(Enumerable.OfType<BehaviorTriggerActionNode>((IEnumerable) selectedObjects));
      if (actionNode == null || actionNode.Parent == null || !(actionNode.Parent is BehaviorTriggerBaseNode))
        return;
      this.behaviorTriggerBaseNode = (BehaviorTriggerBaseNode) actionNode.Parent;
      this.objectSet = new TriggerObjectSet(actionNode);
      this.sceneViewModel = this.behaviorTriggerBaseNode.ViewModel;
      this.HookSelectionManagerEarlySceneUpdate();
      this.Rebuild();
    }

    public override void ApplyFilter(PropertyFilter filter)
    {
      base.ApplyFilter(filter);
      ConditionalExpressionBehaviorCategory behaviorCategory1 = this;
      int num1 = behaviorCategory1.BasicPropertyMatchesFilter | this.MatchesFilter ? true : false;
      behaviorCategory1.BasicPropertyMatchesFilter = num1 != 0;
      ConditionalExpressionBehaviorCategory behaviorCategory2 = this;
      int num2 = behaviorCategory2.AdvancedPropertyMatchesFilter | this.MatchesFilter ? true : false;
      behaviorCategory2.AdvancedPropertyMatchesFilter = num2 != 0;
      if (!this.MatchesFilter || filter.IsEmpty)
        return;
      this.Expanded = true;
    }

    public override bool MatchesPredicate(PropertyFilterPredicate predicate)
    {
      return this.HasConditions && (predicate.Match("LeftOperand") || predicate.Match("RightOperand") || predicate.Match("Operator")) || predicate.Match("Conditions");
    }

    internal BehaviorTriggerBaseNode CloneCurrentTrigger()
    {
      return BehaviorHelper.CloneTrigger(this.behaviorTriggerBaseNode, this.sceneViewModel);
    }

    internal void AddConditionNode()
    {
      if (!this.EnsureInteractionsAssemblyReferenced() || this.behaviorTriggerBaseNode == null)
        return;
      BehaviorTriggerBaseNode newTriggerParent = this.CloneCurrentTrigger();
      using (SceneEditTransaction editTransaction = this.sceneViewModel.CreateEditTransaction(StringTable.AddConditionUndo))
      {
        ISceneNodeCollection<SceneNode> collectionForProperty = newTriggerParent.GetCollectionForProperty(BehaviorHelper.BehaviorsProperty);
        this.conditionalExpressionNode = (ConditionalExpressionNode) null;
        this.conditionBehaviorNode = (ConditionBehaviorNode) null;
        foreach (SceneNode sceneNode in (IEnumerable<SceneNode>) collectionForProperty)
        {
          if (ProjectNeutralTypes.ConditionBehavior.IsAssignableFrom((ITypeId) sceneNode.TrueTargetTypeId))
          {
            ConditionBehaviorNode conditionBehaviorNode = (ConditionBehaviorNode) sceneNode;
            ConditionalExpressionNode conditionalExpressionNode = conditionBehaviorNode.ConditionAsConditionalExpressionNode;
            if (conditionalExpressionNode != null)
            {
              this.conditionBehaviorNode = conditionBehaviorNode;
              this.conditionalExpressionNode = conditionalExpressionNode;
              break;
            }
          }
        }
        if (this.conditionBehaviorNode == null)
        {
          this.conditionBehaviorNode = (ConditionBehaviorNode) this.sceneViewModel.CreateSceneNode(ProjectNeutralTypes.ConditionBehavior);
          collectionForProperty.Add((SceneNode) this.conditionBehaviorNode);
          this.conditionalExpressionNode = (ConditionalExpressionNode) this.sceneViewModel.CreateSceneNode(ProjectNeutralTypes.ConditionalExpression);
          this.conditionBehaviorNode.ConditionAsConditionalExpressionNode = this.conditionalExpressionNode;
        }
        this.conditions = this.conditionalExpressionNode.Conditions;
        if (this.conditions != null)
          this.conditions.Add(this.sceneViewModel.CreateSceneNode(ProjectNeutralTypes.ComparisonCondition));
        if (this.behaviorTriggerBaseNode.Actions.Count > 1)
        {
          this.objectSet.ReparentActionAndCopyBehaviors(newTriggerParent);
          this.behaviorTriggerBaseNode = newTriggerParent;
        }
        else
        {
          SceneNode valueAsSceneNode = newTriggerParent.GetLocalValueAsSceneNode(BehaviorHelper.BehaviorsProperty);
          valueAsSceneNode.Remove();
          this.behaviorTriggerBaseNode.SetValueAsSceneNode(BehaviorHelper.BehaviorsProperty, valueAsSceneNode);
        }
        editTransaction.Commit();
      }
      this.Rebuild();
      this.Expanded = true;
    }

    internal void DeleteConditionNode(SceneNodeProperty conditionProperty)
    {
      int index = this.ConditionNodes.IndexOf((PropertyEntry) conditionProperty);
      if (index == -1 || this.conditions == null)
        return;
      using (SceneEditTransaction editTransaction = this.sceneViewModel.CreateEditTransaction(StringTable.DeleteConditionUndo))
      {
        this.conditions[index].Remove();
        this.Canonicalize();
        editTransaction.Commit();
      }
      this.Rebuild();
      this.ConditionsNodeView.MoveCurrentToNext();
    }

    internal void Teardown()
    {
      this.UnhookSelectionManagerEarlySceneUpdate();
      this.sceneViewModel = (SceneViewModel) null;
      this.CleanBehaviorTriggerReferences();
    }

    private void HookSelectionManagerEarlySceneUpdate()
    {
      if (this.listeningToSelectionManager != null)
        return;
      this.listeningToSelectionManager = this.sceneViewModel.DesignerContext.SelectionManager;
      this.listeningToSelectionManager.EarlyActiveSceneUpdatePhase += new SceneUpdatePhaseEventHandler(this.OnSelectionManagerEarlySceneUpdatePhase);
    }

    private void UnhookSelectionManagerEarlySceneUpdate()
    {
      if (this.listeningToSelectionManager == null)
        return;
      this.listeningToSelectionManager.EarlyActiveSceneUpdatePhase -= new SceneUpdatePhaseEventHandler(this.OnSelectionManagerEarlySceneUpdatePhase);
      this.listeningToSelectionManager = (SelectionManager) null;
    }

    private void OnSelectionManagerEarlySceneUpdatePhase(object sender, SceneUpdatePhaseEventArgs args)
    {
      if ((args.DirtyViewState & SceneViewModel.ViewStateBits.ElementSelection) != SceneViewModel.ViewStateBits.None)
        this.ClearConditionList();
      bool flag = false;
      if (args.DocumentChanges != null && args.DocumentChanges.Count > 0)
      {
        foreach (DocumentNodeChange documentNodeChange in args.DocumentChanges.DistinctChanges)
        {
          if (ProjectNeutralTypes.ConditionCollection.IsAssignableFrom((ITypeId) documentNodeChange.ParentNode.Type))
          {
            flag = true;
            break;
          }
          if (documentNodeChange.NewChildNode != null && ProjectNeutralTypes.BehaviorCollection.IsAssignableFrom((ITypeId) documentNodeChange.NewChildNode.Type))
          {
            flag = true;
            break;
          }
          if (documentNodeChange.OldChildNode != null && ProjectNeutralTypes.BehaviorCollection.IsAssignableFrom((ITypeId) documentNodeChange.OldChildNode.Type))
          {
            flag = true;
            break;
          }
          if (documentNodeChange.PropertyKey != null && documentNodeChange.PropertyKey.Name == "ForwardChaining")
          {
            this.OnPropertyChanged("ForwardChainingSetToOr");
            this.OnPropertyChanged("ForwardChainingSetToOrEnabled");
          }
        }
      }
      if (!flag)
        return;
      this.Rebuild();
    }

    private void CleanBehaviorTriggerReferences()
    {
      this.CleanConditionBehaviorReferences();
      if (this.objectSet != null)
      {
        this.objectSet.Dispose();
        this.objectSet = (TriggerObjectSet) null;
      }
      this.behaviorTriggerBaseNode = (BehaviorTriggerBaseNode) null;
    }

    private void CleanConditionBehaviorReferences()
    {
      this.ClearConditionList();
      this.conditions = (ISceneNodeCollection<SceneNode>) null;
      this.conditionalExpressionNode = (ConditionalExpressionNode) null;
      this.conditionBehaviorNode = (ConditionBehaviorNode) null;
    }

    private void ClearConditionList()
    {
      if (this.ConditionNodes == null)
        return;
      foreach (PropertyBase propertyBase in (Collection<PropertyEntry>) this.ConditionNodes)
        propertyBase.OnRemoveFromCategory();
      this.ConditionNodes.Clear();
    }

    private void Rebuild()
    {
      if (this.behaviorTriggerBaseNode == null)
        return;
      this.CleanConditionBehaviorReferences();
      if (this.behaviorTriggerBaseNode.IsSet(BehaviorHelper.BehaviorsProperty) == PropertyState.Set)
      {
        foreach (SceneNode sceneNode in (IEnumerable<SceneNode>) this.behaviorTriggerBaseNode.GetCollectionForProperty(BehaviorHelper.BehaviorsProperty))
        {
          if (ProjectNeutralTypes.ConditionBehavior.IsAssignableFrom((ITypeId) sceneNode.Type))
          {
            this.conditionBehaviorNode = (ConditionBehaviorNode) sceneNode;
            ConditionalExpressionNode conditionalExpressionNode = this.conditionBehaviorNode.ConditionAsConditionalExpressionNode;
            if (conditionalExpressionNode != null)
            {
              this.conditionalExpressionNode = conditionalExpressionNode;
              this.conditions = this.conditionalExpressionNode.Conditions;
              break;
            }
          }
        }
      }
      this.ConditionUIAvailable = true;
      if (this.ConditionsNodeView != null && this.conditions != null)
      {
        foreach (SceneNode condition in (IEnumerable<SceneNode>) this.conditions)
        {
          PropertyEntry entryForCondition = this.CreatePropertyEntryForCondition(condition);
          this.ConditionNodes.Add(entryForCondition);
          if (entryForCondition.PropertyValue.SubProperties.Count == 0)
          {
            this.ConditionUIAvailable = false;
            this.CleanConditionBehaviorReferences();
            break;
          }
        }
      }
      this.ConditionsNodeView.MoveCurrentToLast();
      this.OnPropertyChanged("ConditionsNodeView");
      this.OnPropertyChanged("ForwardChainingSetToOr");
      this.OnPropertyChanged("ForwardChainingSetToOrEnabled");
      this.OnPropertyChanged("ConditionUIAvailable");
    }

    private PropertyEntry CreatePropertyEntryForCondition(SceneNode condition)
    {
      List<ReferenceStep> steps = new List<ReferenceStep>();
      IProperty property1 = condition.ProjectContext.ResolveProperty(BehaviorHelper.BehaviorsProperty);
      steps.Add(property1 as ReferenceStep);
      ISceneNodeCollection<SceneNode> collectionForProperty = this.behaviorTriggerBaseNode.GetCollectionForProperty(BehaviorHelper.BehaviorsProperty);
      steps.Add((ReferenceStep) IndexedClrPropertyReferenceStep.GetReferenceStep((ITypeResolver) condition.ProjectContext, ProjectNeutralTypes.BehaviorCollection, collectionForProperty.IndexOf((SceneNode) this.conditionBehaviorNode)));
      IProperty property2 = condition.ProjectContext.ResolveProperty(ConditionBehaviorNode.ConditionProperty);
      steps.Add(property2 as ReferenceStep);
      IProperty property3 = condition.ProjectContext.ResolveProperty(ConditionalExpressionNode.ConditionsProperty);
      steps.Add(property3 as ReferenceStep);
      steps.Add((ReferenceStep) IndexedClrPropertyReferenceStep.GetReferenceStep((ITypeResolver) condition.ProjectContext, ProjectNeutralTypes.ConditionCollection, this.conditions.IndexOf(condition)));
      return (PropertyEntry) this.objectSet.CreateProperty(new PropertyReference(steps), (AttributeCollection) null);
    }

    private void Canonicalize()
    {
      bool flag = false;
      if (this.conditions.Count == 0)
      {
        this.conditionBehaviorNode.Remove();
        flag = true;
      }
      if (this.behaviorTriggerBaseNode.GetCollectionForProperty(BehaviorHelper.BehaviorsProperty).Count == 0)
      {
        this.behaviorTriggerBaseNode.ClearLocalValue(BehaviorHelper.BehaviorsProperty);
        BehaviorTriggerBaseNode matchingDocumentNode = this.objectSet.FindExistingTriggerMatchingDocumentNode(this.behaviorTriggerBaseNode.DocumentNode);
        if (matchingDocumentNode != null)
        {
          this.objectSet.ReparentActionAndCopyBehaviors(matchingDocumentNode);
          this.behaviorTriggerBaseNode = matchingDocumentNode;
          this.keepPanelOpened = true;
        }
      }
      if (!flag)
        return;
      this.CleanConditionBehaviorReferences();
    }

    private bool EnsureInteractionsAssemblyReferenced()
    {
      if (!BehaviorHelper.EnsureBlendSDKLibraryAssemblyReferenced(this.sceneViewModel, "Microsoft.Expression.Interactions"))
      {
        int num = (int) this.sceneViewModel.DesignerContext.MessageDisplayService.ShowMessage(StringTable.CantAddConditionMessage, (string) null, MessageBoxButton.OK, MessageBoxImage.Exclamation);
        return false;
      }
      IType type = this.sceneViewModel.ProjectContext.ResolveType(ProjectNeutralTypes.ConditionBehavior);
      if (type != null && !this.sceneViewModel.ProjectContext.PlatformMetadata.IsNullType((ITypeId) type))
        return true;
      int num1 = (int) this.sceneViewModel.DesignerContext.MessageDisplayService.ShowMessage(StringTable.WrongInteractionReference, (string) null, MessageBoxButton.OK, MessageBoxImage.Exclamation);
      return false;
    }
  }
}
