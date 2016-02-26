// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.BehaviorCommandCategory
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.Selection;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.PropertyInspector;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public sealed class BehaviorCommandCategory : SceneNodeCategory
  {
    private SelectionManager listeningToSelectionManager;
    private BehaviorNode behaviorNode;
    private CollectionViewSource triggerAdvancedPropertiesViewSource;
    private CollectionViewSource triggerBasicPropertiesViewSource;
    private SceneViewModel sceneViewModel;
    private SceneNodeProperty propertyEntry;
    private BehaviorCommandCategory.PropertyEntryCollectionWrapper properties;

    private ObservableCollection<PropertyEntry> TriggerNodes { get; set; }

    public ICollectionView TriggerNodeView { get; private set; }

    public ICollectionView TriggerBasicPropertiesView
    {
      get
      {
        return this.triggerBasicPropertiesViewSource.View;
      }
    }

    public ICollectionView TriggerAdvancedPropertiesView
    {
      get
      {
        return this.triggerAdvancedPropertiesViewSource.View;
      }
    }

    public BehaviorCommandCategory(string localizedName, IMessageLoggingService messageLogger)
      : base(CategoryLocalizationHelper.CategoryName.BehaviorCommand, localizedName, messageLogger)
    {
      this.TriggerNodes = new ObservableCollection<PropertyEntry>();
      this.TriggerNodeView = CollectionViewSource.GetDefaultView((object) this.TriggerNodes);
      this.TriggerNodeView.CurrentChanged += new EventHandler(this.OnCurrentTriggerChanged);
      this.triggerBasicPropertiesViewSource = new CollectionViewSource();
      this.triggerBasicPropertiesViewSource.Filter += new FilterEventHandler(this.TriggerBasicPropertiesFilter);
      this.triggerBasicPropertiesViewSource.SortDescriptions.Add(new SortDescription("PropertyOrder", ListSortDirection.Ascending));
      this.triggerBasicPropertiesViewSource.SortDescriptions.Add(new SortDescription("PropertyName", ListSortDirection.Ascending));
      this.triggerAdvancedPropertiesViewSource = new CollectionViewSource();
      this.triggerAdvancedPropertiesViewSource.Filter += new FilterEventHandler(this.TriggerAdvancedPropertiesFilter);
      this.triggerAdvancedPropertiesViewSource.SortDescriptions.Add(new SortDescription("PropertyOrder", ListSortDirection.Ascending));
      this.triggerAdvancedPropertiesViewSource.SortDescriptions.Add(new SortDescription("PropertyName", ListSortDirection.Ascending));
    }

    public override void OnSelectionChanged(SceneNode[] selectedObjects)
    {
      if (this.propertyEntry != null)
      {
        this.propertyEntry.PropertyReferenceChanged -= new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.OnTriggersCollectionChanged);
        this.propertyEntry.OnRemoveFromCategory();
        this.propertyEntry = (SceneNodeProperty) null;
      }
      this.ClearTriggerPropertiesList();
      this.UnhookSelectionManagerEarlySceneUpdate();
      BehaviorNode behaviorNode = Enumerable.FirstOrDefault<BehaviorNode>(Enumerable.OfType<BehaviorNode>((IEnumerable) selectedObjects));
      if (behaviorNode == null)
        return;
      this.behaviorNode = behaviorNode;
      this.sceneViewModel = this.behaviorNode.ViewModel;
      this.HookSelectionManagerEarlySceneUpdate();
      this.Rebuild();
      this.TriggerNodeView.MoveCurrentToFirst();
      if (this.propertyEntry != null)
        return;
      this.propertyEntry = (SceneNodeProperty) this.sceneViewModel.DesignerContext.PropertyInspectorModel.SceneNodeObjectSet.CreateProperty(new PropertyReference((ReferenceStep) this.sceneViewModel.ProjectContext.ResolveProperty(BehaviorHelper.BehaviorTriggersProperty)), (AttributeCollection) null);
      this.propertyEntry.PropertyReferenceChanged += new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.OnTriggersCollectionChanged);
    }

    public override void ApplyFilter(PropertyFilter filter)
    {
      base.ApplyFilter(filter);
      bool propertyMatchesFilter1 = this.BasicPropertyMatchesFilter;
      bool propertyMatchesFilter2 = this.AdvancedPropertyMatchesFilter;
      if (this.TriggerBasicPropertiesView != null)
      {
        foreach (PropertyEntry property in (IEnumerable) this.TriggerBasicPropertiesView)
          propertyMatchesFilter1 |= this.DoesPropertyMatchFilter(filter, property);
      }
      if (this.TriggerAdvancedPropertiesView != null)
      {
        foreach (PropertyEntry property in (IEnumerable) this.TriggerAdvancedPropertiesView)
          propertyMatchesFilter2 |= this.DoesPropertyMatchFilter(filter, property);
      }
      this.BasicPropertyMatchesFilter = propertyMatchesFilter1;
      this.AdvancedPropertyMatchesFilter = propertyMatchesFilter2;
      this.OnFilterApplied(filter);
    }

    internal void AddTriggerNode()
    {
      Type newClrObject = ClrNewObjectDialog.CreateNewClrObject(this.sceneViewModel, this.sceneViewModel.ProjectContext.ResolveType(ProjectNeutralTypes.BehaviorTriggerBase).RuntimeType, true);
      if (!(newClrObject != (Type) null))
        return;
      using (SceneEditTransaction editTransaction = this.sceneViewModel.CreateEditTransaction(StringTable.AddBehaviorCommandTriggerUndo))
      {
        BehaviorTriggerBaseNode behaviorTriggerBaseNode = (BehaviorTriggerBaseNode) this.sceneViewModel.CreateSceneNode(newClrObject);
        InvokeCommandActionNode commandActionNode = (InvokeCommandActionNode) this.sceneViewModel.CreateSceneNode(ProjectNeutralTypes.InvokeCommandAction);
        commandActionNode.CommandName = this.CategoryName;
        behaviorTriggerBaseNode.Actions.Add((SceneNode) commandActionNode);
        if (ProjectNeutralTypes.BehaviorEventTriggerBase.IsAssignableFrom((ITypeId) behaviorTriggerBaseNode.Type))
        {
          SceneNode sceneNode = (SceneNode) this.behaviorNode;
          while (sceneNode != null && !(sceneNode is SceneElement))
            sceneNode = sceneNode.Parent;
          if (sceneNode != null)
          {
            sceneNode.EnsureNamed();
            ((BehaviorEventTriggerBaseNode) behaviorTriggerBaseNode).SourceName = sceneNode.Name;
          }
        }
        this.behaviorNode.GetCollectionForProperty(BehaviorHelper.BehaviorTriggersProperty).Add((SceneNode) behaviorTriggerBaseNode);
        editTransaction.Commit();
      }
      this.Rebuild();
      this.TriggerNodeView.MoveCurrentToLast();
    }

    internal void DeleteTriggerNode(SceneNodeProperty triggerProperty)
    {
      ISceneNodeCollection<SceneNode> collectionForProperty = this.behaviorNode.GetCollectionForProperty(BehaviorHelper.BehaviorTriggersProperty);
      SceneNode valueAsSceneNode = this.behaviorNode.GetLocalValueAsSceneNode(triggerProperty.Reference);
      int position = this.TriggerNodeView.CurrentPosition;
      int num = this.TriggerNodes.IndexOf((PropertyEntry) triggerProperty);
      if (position > num)
        --position;
      else if (position == num)
        position = -1;
      using (SceneEditTransaction editTransaction = this.sceneViewModel.CreateEditTransaction(string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.DeleteBehaviorCommandTriggerUndo, new object[1]
      {
        (object) valueAsSceneNode.TargetType.Name
      })))
      {
        collectionForProperty.Remove(valueAsSceneNode);
        editTransaction.Commit();
      }
      this.Rebuild();
      this.TriggerNodeView.MoveCurrentToPosition(position);
    }

    internal void Teardown()
    {
      this.UnhookSelectionManagerEarlySceneUpdate();
      this.sceneViewModel = (SceneViewModel) null;
      this.TriggerNodeView.CurrentChanged -= new EventHandler(this.OnCurrentTriggerChanged);
      this.TriggerNodeView = (ICollectionView) null;
      this.ClearTriggerPropertiesList();
      this.TriggerNodes = (ObservableCollection<PropertyEntry>) null;
      if (this.propertyEntry == null)
        return;
      this.propertyEntry.PropertyReferenceChanged -= new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.OnTriggersCollectionChanged);
      this.propertyEntry.OnRemoveFromCategory();
      this.propertyEntry = (SceneNodeProperty) null;
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
      if ((args.DirtyViewState & SceneViewModel.ViewStateBits.ElementSelection) == SceneViewModel.ViewStateBits.None)
        return;
      if (this.propertyEntry != null)
      {
        this.propertyEntry.PropertyReferenceChanged -= new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.OnTriggersCollectionChanged);
        this.propertyEntry.OnRemoveFromCategory();
        this.propertyEntry = (SceneNodeProperty) null;
      }
      this.ClearTriggerPropertiesList();
    }

    private void ClearTriggerPropertiesList()
    {
      if (this.TriggerNodes == null)
        return;
      foreach (PropertyBase propertyBase in (Collection<PropertyEntry>) this.TriggerNodes)
        propertyBase.OnRemoveFromCategory();
      this.TriggerNodes.Clear();
    }

    private void UpdateProperties()
    {
      PropertyEntry triggerProperty = (PropertyEntry) this.TriggerNodeView.CurrentItem;
      if (this.triggerBasicPropertiesViewSource == null)
      {
        this.triggerBasicPropertiesViewSource = new CollectionViewSource();
        this.triggerBasicPropertiesViewSource.Filter += new FilterEventHandler(this.TriggerBasicPropertiesFilter);
        this.triggerBasicPropertiesViewSource.SortDescriptions.Add(new SortDescription("PropertyOrder", ListSortDirection.Ascending));
        this.triggerBasicPropertiesViewSource.SortDescriptions.Add(new SortDescription("PropertyName", ListSortDirection.Ascending));
      }
      if (this.triggerAdvancedPropertiesViewSource == null)
      {
        this.triggerAdvancedPropertiesViewSource = new CollectionViewSource();
        this.triggerAdvancedPropertiesViewSource.Filter += new FilterEventHandler(this.TriggerAdvancedPropertiesFilter);
        this.triggerAdvancedPropertiesViewSource.SortDescriptions.Add(new SortDescription("PropertyOrder", ListSortDirection.Ascending));
        this.triggerAdvancedPropertiesViewSource.SortDescriptions.Add(new SortDescription("PropertyName", ListSortDirection.Ascending));
      }
      if (this.properties != null)
      {
        this.properties.ClearProperties();
        this.properties = (BehaviorCommandCategory.PropertyEntryCollectionWrapper) null;
      }
      if (triggerProperty != null)
      {
        this.properties = this.GetTriggerProperties(triggerProperty);
        this.triggerBasicPropertiesViewSource.Source = (object) this.properties.Properties;
        this.triggerAdvancedPropertiesViewSource.Source = (object) this.properties.Properties;
      }
      else
      {
        this.triggerBasicPropertiesViewSource.Source = (object) null;
        this.triggerAdvancedPropertiesViewSource.Source = (object) null;
      }
      this.UpdateViews();
    }

    private BehaviorCommandCategory.PropertyEntryCollectionWrapper GetTriggerProperties(PropertyEntry triggerProperty)
    {
      BehaviorCommandCategory.PropertyEntryCollectionWrapper collectionWrapper = new BehaviorCommandCategory.PropertyEntryCollectionWrapper(triggerProperty.PropertyValue.SubProperties);
      SceneNodeProperty sceneNodeProperty = (SceneNodeProperty) triggerProperty;
      BehaviorTriggerBaseNode behaviorTriggerBaseNode = (BehaviorTriggerBaseNode) this.behaviorNode.GetLocalValueAsSceneNode(sceneNodeProperty.Reference);
      foreach (BehaviorTriggerActionNode triggerActionNode in (IEnumerable<SceneNode>) behaviorTriggerBaseNode.Actions)
      {
        if (ProjectNeutralTypes.InvokeCommandAction.IsAssignableFrom((ITypeId) triggerActionNode.Type) && ((InvokeCommandActionNode) triggerActionNode).CommandName.Equals(this.CategoryName, StringComparison.Ordinal))
        {
          PropertyReference propertyReference = sceneNodeProperty.Reference.Append(BehaviorTriggerBaseNode.BehaviorActionsProperty).Append((ReferenceStep) IndexedClrPropertyReferenceStep.GetReferenceStep((ITypeResolver) triggerActionNode.ProjectContext, ProjectNeutralTypes.BehaviorTriggerActionCollection, behaviorTriggerBaseNode.Actions.IndexOf((SceneNode) triggerActionNode))).Append(InvokeCommandActionNode.CommandParameterProperty);
          collectionWrapper.AddCustomProperty(sceneNodeProperty.SceneNodeObjectSet.CreateProperty(propertyReference, propertyReference.LastStep.Attributes));
          break;
        }
      }
      return collectionWrapper;
    }

    private SceneNodeProperty CreateProperty(PropertyReference propertyReference)
    {
      if (this.sceneViewModel != null)
        return (SceneNodeProperty) this.sceneViewModel.DesignerContext.PropertyInspectorModel.SceneNodeObjectSet.CreateProperty(propertyReference, (AttributeCollection) null);
      return (SceneNodeProperty) null;
    }

    private void OnTriggersCollectionChanged(object sender, PropertyReferenceChangedEventArgs e)
    {
      foreach (PropertyReferenceProperty referenceProperty in (Collection<PropertyEntry>) this.TriggerNodes)
      {
        if (referenceProperty.Reference.IsPrefixOf(e.PropertyReference))
          return;
      }
      this.Rebuild();
    }

    private void Rebuild()
    {
      if (this.TriggerNodeView == null)
        return;
      int currentPosition = this.TriggerNodeView.CurrentPosition;
      this.ClearTriggerPropertiesList();
      ISceneNodeCollection<SceneNode> collectionForProperty = this.behaviorNode.GetCollectionForProperty(BehaviorHelper.BehaviorTriggersProperty);
      foreach (SceneNode sceneNode in (IEnumerable<SceneNode>) collectionForProperty)
      {
        BehaviorTriggerBaseNode behaviorTriggerBaseNode = (BehaviorTriggerBaseNode) sceneNode;
        foreach (BehaviorTriggerActionNode triggerActionNode in (IEnumerable<SceneNode>) behaviorTriggerBaseNode.Actions)
        {
          InvokeCommandActionNode commandActionNode = triggerActionNode as InvokeCommandActionNode;
          if (commandActionNode != null && commandActionNode.IsAttached && string.Compare(commandActionNode.CommandName, this.CategoryName, StringComparison.OrdinalIgnoreCase) == 0)
          {
            PropertyReference propertyReference = new PropertyReference(new List<ReferenceStep>()
            {
              this.behaviorNode.GetPropertyForChild((SceneNode) behaviorTriggerBaseNode) as ReferenceStep,
              (ReferenceStep) IndexedClrPropertyReferenceStep.GetReferenceStep((ITypeResolver) sceneNode.ProjectContext, ProjectNeutralTypes.BehaviorTriggerCollection, collectionForProperty.IndexOf((SceneNode) behaviorTriggerBaseNode))
            });
            if (!Enumerable.Any<PropertyEntry>((IEnumerable<PropertyEntry>) this.TriggerNodes, (Func<PropertyEntry, bool>) (existingProperty => ((PropertyReferenceProperty) existingProperty).Reference.Equals((object) propertyReference))))
              this.TriggerNodes.Add((PropertyEntry) this.CreateProperty(propertyReference));
          }
        }
      }
      if (currentPosition >= this.TriggerNodes.Count)
        return;
      this.TriggerNodeView.MoveCurrentToPosition(currentPosition);
    }

    private void TriggerBasicPropertiesFilter(object sender, FilterEventArgs e)
    {
      PropertyEntry propertyEntry = (PropertyEntry) e.Item;
      e.Accepted = !propertyEntry.IsAdvanced;
    }

    private void TriggerAdvancedPropertiesFilter(object sender, FilterEventArgs e)
    {
      PropertyEntry propertyEntry = (PropertyEntry) e.Item;
      e.Accepted = propertyEntry.IsAdvanced;
    }

    private void OnCurrentTriggerChanged(object sender, EventArgs e)
    {
      this.UpdateProperties();
    }

    private void UpdateViews()
    {
      this.OnPropertyChanged("TriggerBasicPropertiesView");
      this.OnPropertyChanged("TriggerAdvancedPropertiesView");
      if (this.sceneViewModel == null || this.sceneViewModel.DesignerContext == null)
        return;
      this.ApplyFilter(new PropertyFilter(this.sceneViewModel.DesignerContext.PropertyInspectorModel.FilterString));
    }

    private sealed class PropertyEntryCollectionWrapper
    {
      private PropertyEntryCollection propertyEntryCollection;
      private Collection<PropertyReferenceProperty> customProperties;

      public IEnumerable<PropertyReferenceProperty> Properties
      {
        get
        {
          List<PropertyReferenceProperty> list = new List<PropertyReferenceProperty>();
          foreach (PropertyEntry propertyEntry in this.propertyEntryCollection)
          {
            PropertyReferenceProperty referenceProperty = propertyEntry as PropertyReferenceProperty;
            if (referenceProperty != null)
              list.Add(referenceProperty);
          }
          list.AddRange((IEnumerable<PropertyReferenceProperty>) this.customProperties);
          return (IEnumerable<PropertyReferenceProperty>) list;
        }
      }

      public PropertyEntryCollectionWrapper(PropertyEntryCollection propertyEntryCollection)
      {
        this.propertyEntryCollection = propertyEntryCollection;
        this.customProperties = new Collection<PropertyReferenceProperty>();
      }

      public void AddCustomProperty(PropertyReferenceProperty customProperty)
      {
        this.customProperties.Add(customProperty);
      }

      public void ClearProperties()
      {
        foreach (PropertyBase propertyBase in this.customProperties)
          propertyBase.OnRemoveFromCategory();
        this.customProperties.Clear();
      }
    }
  }
}
