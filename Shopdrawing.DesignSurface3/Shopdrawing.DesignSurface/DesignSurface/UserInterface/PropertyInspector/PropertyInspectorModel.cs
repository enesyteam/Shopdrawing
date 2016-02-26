// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.PropertyInspectorModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.Tools.Path;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.DesignSurface.ViewModel.Text;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Data;
using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.PropertyInspector;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Threading;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public sealed class PropertyInspectorModel : IPropertyInspector, INotifyPropertyChanged, IDisposable
  {
    private CategoryEditorSet categoryEditors = new CategoryEditorSet();
    private bool showPropertiesNotEvents = true;
    private bool showPropertiesEventsToggle = true;
    private bool hasAnyFilterMatches = true;
    private Dictionary<string, bool> categoryExpandedCache = new Dictionary<string, bool>();
    private bool inspectingUIElement = true;
    private SceneNodeCategoryCollection categories;
    private SelectedElementsObjectSet sceneNodeObjectSet;
    private string filterString;
    private DesignerContext designerContext;
    private SceneNode[] selectedObjects;
    private IList<TargetedReferenceStep> referenceStepCollection;
    private EventsModel events;
    private PropertyEditingHelper transactionHelper;
    private IType commonType;
    private bool hasSelection;
    private SceneNodePropertyLookup sceneNodePropertyLookup;
    private uint updatedPropertiesResourcesChangeStamp;
    private DelegateCommand clearFilterStringCommand;
    private int userInterfaceCount;
    private SceneNodeProperty selectionNameProperty;
    private SceneDocument activeDocument;
    private IProjectContext activeProjectContext;
    private bool isInvalidElementSelected;
    private IPlatform activePlatform;

    public ICommand HelpCommand { get; private set; }

    public ICommand SearchOnlineCommand { get; private set; }

    public SceneNodeCategoryCollection Categories
    {
      get
      {
        return this.categories;
      }
    }

    public SelectedElementsObjectSet SceneNodeObjectSet
    {
      get
      {
        return this.sceneNodeObjectSet;
      }
    }

    public string FilterString
    {
      get
      {
        return this.filterString;
      }
      set
      {
        if (!(this.filterString != value))
          return;
        this.filterString = value;
        this.UpdateFilter();
        this.OnPropertyChanged("FilterString");
      }
    }

    public bool HasSelection
    {
      get
      {
        return this.hasSelection;
      }
      private set
      {
        if (this.hasSelection == value)
          return;
        this.hasSelection = value;
        this.OnPropertyChanged("HasSelection");
      }
    }

    public bool IsViewRepresentationValid
    {
      get
      {
        foreach (SceneNode sceneNode in this.SceneNodeObjectSet.Objects)
        {
          if (!sceneNode.IsViewObjectValid)
            return false;
        }
        return true;
      }
    }

    public bool HasAnyFilterMatches
    {
      get
      {
        return this.hasAnyFilterMatches;
      }
      private set
      {
        if (this.hasAnyFilterMatches == value)
          return;
        this.hasAnyFilterMatches = value;
        this.OnPropertyChanged("HasAnyFilterMatches");
      }
    }

    public bool ShowPropertiesNotEvents
    {
      get
      {
        return this.showPropertiesNotEvents;
      }
      set
      {
        if (this.showPropertiesNotEvents == value)
          return;
        this.showPropertiesNotEvents = value;
        this.events.HiddenMode = value;
        this.OnPropertyChanged("ShowPropertiesNotEvents");
      }
    }

    public bool ShowPropertiesEventsToggle
    {
      get
      {
        return this.showPropertiesEventsToggle;
      }
      set
      {
        if (this.showPropertiesEventsToggle == value)
          return;
        this.showPropertiesEventsToggle = value;
        this.OnPropertyChanged("ShowPropertiesEventsToggle");
      }
    }

    public bool IsInvalidElementSelected
    {
      get
      {
        return this.isInvalidElementSelected;
      }
      set
      {
        if (this.isInvalidElementSelected == value)
          return;
        this.isInvalidElementSelected = value;
        this.OnPropertyChanged("IsInvalidElementSelected");
      }
    }

    public ICommand ClearFilterStringCommand
    {
      get
      {
        if (this.clearFilterStringCommand == null)
          this.clearFilterStringCommand = new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.OnClearFilterStringCommand));
        return (ICommand) this.clearFilterStringCommand;
      }
    }

    public DrawingBrush SelectionIconBrush
    {
      get
      {
        Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.SceneNodeObjectSet sceneNodeObjectSet = (Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.SceneNodeObjectSet) this.SceneNodeObjectSet;
        if (sceneNodeObjectSet.Count == 0)
          return (DrawingBrush) null;
        if (sceneNodeObjectSet.IsHomogenous)
        {
          IType type = this.activeDocument.ProjectContext.GetType(sceneNodeObjectSet.ObjectType);
          if (type != null)
            return IconMapper.GetDrawingBrushForType((ITypeId) type, true, 24, 24);
        }
        return IconMapper.GetDrawingBrushForType(PlatformTypes.Object, true, 24, 24);
      }
    }

    public string SelectionName
    {
      get
      {
        Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.SceneNodeObjectSet sceneNodeObjectSet = (Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.SceneNodeObjectSet) this.SceneNodeObjectSet;
        if (sceneNodeObjectSet.Count == 0)
          return (string) null;
        if (sceneNodeObjectSet.Count == 1)
        {
          SceneNode sceneNode = this.SceneNodeObjectSet.Objects[0];
          StoryboardTimelineSceneNode timelineSceneNode;
          TextRangeElement textRangeElement;
          return ((timelineSceneNode = sceneNode as StoryboardTimelineSceneNode) == null ? ((textRangeElement = sceneNode as TextRangeElement) == null ? sceneNode.Name : textRangeElement.TextEditProxy.TextSource.Name) : timelineSceneNode.Name) ?? StringTable.EmptyNameStandIn;
        }
        return string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.MultiElementSelectionNameFormat, new object[1]
        {
          (object) sceneNodeObjectSet.Count
        });
      }
      set
      {
        Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.SceneNodeObjectSet sceneNodeObjectSet = (Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.SceneNodeObjectSet) this.SceneNodeObjectSet;
        if (sceneNodeObjectSet.Count != 1)
          return;
        SceneNode sceneNode = sceneNodeObjectSet.Objects[0];
        StoryboardTimelineSceneNode storyboard;
        if ((storyboard = sceneNode as StoryboardTimelineSceneNode) != null)
        {
          storyboard.ViewModel.AnimationEditor.RenameStoryboardWithValidation(storyboard, value);
        }
        else
        {
          TextRangeElement textRangeElement = sceneNode as TextRangeElement;
          if (textRangeElement != null)
            sceneNode = (SceneNode) textRangeElement.TextEditProxy.TextSource;
          using (SceneEditTransaction editTransaction = sceneNode.ViewModel.CreateEditTransaction(StringTable.UndoUnitElementRename))
          {
            sceneNode.Name = value;
            editTransaction.Commit();
          }
        }
      }
    }

    public string SelectionTypeName
    {
      get
      {
        return this.GetSelectionTypeName(false);
      }
    }

    public string SelectionTypeNameFull
    {
      get
      {
        return this.GetSelectionTypeName(true);
      }
    }

    public bool IsInfoBarNameReadOnly
    {
      get
      {
        SelectedElementsObjectSet sceneNodeObjectSet = this.SceneNodeObjectSet;
        if (sceneNodeObjectSet != null && sceneNodeObjectSet.Count == 1 && !PlatformTypes.Style.IsAssignableFrom((ITypeId) sceneNodeObjectSet.RepresentativeSceneNode.Type))
          return PlatformTypes.FrameworkTemplate.IsAssignableFrom((ITypeId) sceneNodeObjectSet.RepresentativeSceneNode.Type);
        return true;
      }
    }

    public EventsModel Events
    {
      get
      {
        return this.events;
      }
    }

    internal IList<TargetedReferenceStep> ReferenceStepCollection
    {
      get
      {
        return this.referenceStepCollection;
      }
    }

    public SceneNodePropertyLookup SceneNodePropertyLookup
    {
      get
      {
        return this.sceneNodePropertyLookup;
      }
    }

    internal PropertyEditingHelper TransactionHelper
    {
      get
      {
        return this.transactionHelper;
      }
    }

    private bool IsCreatingShapeElement
    {
      get
      {
        if (this.designerContext.ActiveView != null && this.designerContext.ActiveView.EventRouter.ActiveBehavior is ShapeCreateBehavior && this.activeDocument != null)
          return this.activeDocument.HasOpenTransaction;
        return false;
      }
    }

    internal DesignerContext DesignerContext
    {
      get
      {
        return this.designerContext;
      }
    }

    private bool NeedPropertyUpdateBasedOnResourceChanges
    {
      get
      {
        return (int) this.updatedPropertiesResourcesChangeStamp != (int) this.designerContext.ResourceManager.ResourceChangeStamp;
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    internal PropertyInspectorModel(DesignerContext designerContext)
    {
      this.designerContext = designerContext;
      this.sceneNodeObjectSet = new SelectedElementsObjectSet(designerContext, (IPropertyInspector) this);
      this.categories = new SceneNodeCategoryCollection();
      this.events = new EventsModel(this.designerContext);
      this.transactionHelper = (PropertyEditingHelper) new DesignerContextPropertyEditingHelper(designerContext);
      this.sceneNodePropertyLookup = new SceneNodePropertyLookup((Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.SceneNodeObjectSet) this.sceneNodeObjectSet, (PropertyReference) null);
      this.designerContext.ViewService.ActiveViewChanged += new ViewChangedEventHandler(this.ViewService_ActiveViewChanged);
      this.designerContext.PropertyManager.MultiplePropertyReferencesChanged += new EventHandler<MultiplePropertyReferencesChangedEventArgs>(this.PropertyManager_MultiplePropertyReferencesChanged);
      this.designerContext.SelectionManager.LateActiveSceneUpdatePhase += new SceneUpdatePhaseEventHandler(this.SelectionManager_LateActiveSceneUpdatePhase);
      if (this.designerContext.WindowService != null)
        this.designerContext.WindowService.ThemeChanged += new EventHandler(this.WindowManager_ThemeChanged);
      this.HelpCommand = (ICommand) new PropertyInspectorContextualHelpCommand(this.DesignerContext);
      this.SearchOnlineCommand = (ICommand) new PropertyInspectorSearchOnlineHelpCommand(this.DesignerContext);
      CollectionViewSource.GetDefaultView((object) this.Categories).SortDescriptions.Add(new SortDescription("SortOrdering", ListSortDirection.Ascending));
      PropertyMerger.PropertiesUpdated += new EventHandler(this.OnPropertyMergerPropertiesUpdated);
    }

    public void Dispose()
    {
      this.designerContext.ViewService.ActiveViewChanged -= new ViewChangedEventHandler(this.ViewService_ActiveViewChanged);
      this.designerContext.PropertyManager.MultiplePropertyReferencesChanged -= new EventHandler<MultiplePropertyReferencesChangedEventArgs>(this.PropertyManager_MultiplePropertyReferencesChanged);
      this.designerContext.SelectionManager.LateActiveSceneUpdatePhase -= new SceneUpdatePhaseEventHandler(this.SelectionManager_LateActiveSceneUpdatePhase);
      ((PropertyInspectorHelpCommand) this.HelpCommand).Unhook();
      ((PropertyInspectorHelpCommand) this.SearchOnlineCommand).Unhook();
      if (this.activeDocument != null)
        this.activeDocument.EditTransactionCompleted -= new EventHandler(this.SceneDocument_EditTransactionCompleted);
      if (this.activeProjectContext != null)
        this.activeProjectContext.TypesChanged -= new EventHandler<TypesChangedEventArgs>(this.ProjectContext_TypesChanged);
      if (this.designerContext.WindowService != null)
        this.designerContext.WindowService.ThemeChanged -= new EventHandler(this.WindowManager_ThemeChanged);
      PropertyMerger.PropertiesUpdated -= new EventHandler(this.OnPropertyMergerPropertiesUpdated);
    }

    private void UpdateSelectionNameProperty()
    {
      if (this.selectionNameProperty != null)
      {
        this.selectionNameProperty.PropertyReferenceChanged -= new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.SelectionNameChanged);
        this.selectionNameProperty.OnRemoveFromCategory();
        this.selectionNameProperty = (SceneNodeProperty) null;
      }
      if (this.selectionNameProperty != null || this.sceneNodeObjectSet.Count != 1)
        return;
      this.selectionNameProperty = this.sceneNodeObjectSet.CreateSceneNodeProperty(new PropertyReference((ReferenceStep) this.SceneNodeObjectSet.Objects[0].NameProperty), (AttributeCollection) null);
      this.selectionNameProperty.PropertyReferenceChanged += new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.SelectionNameChanged);
    }

    private void SelectionNameChanged(object obj, PropertyReferenceChangedEventArgs args)
    {
      this.OnPropertyChanged("SelectionName");
    }

    public void AddUserInterface()
    {
      ++this.userInterfaceCount;
      if (this.userInterfaceCount != 1)
        return;
      this.UpdateSelection(false, false);
    }

    public void RemoveUserInterface()
    {
      if (this.userInterfaceCount <= 0)
        return;
      --this.userInterfaceCount;
      if (this.userInterfaceCount != 0)
        return;
      this.UpdateSelection(false, false);
    }

    private void SceneDocument_EditTransactionCompleted(object sender, EventArgs args)
    {
      if (!this.SceneNodeObjectSet.PropertyUpdatesLocked)
        return;
      this.SceneNodeObjectSet.PropertyUpdatesLocked = false;
      if (this.designerContext.ActiveSceneViewModel == null)
        return;
      this.designerContext.ActiveSceneViewModel.RefreshSelection();
    }

    private void SelectionManager_LateActiveSceneUpdatePhase(object sender, SceneUpdatePhaseEventArgs args)
    {
      this.designerContext.ResourceManager.OnSceneUpdate(args);
      if (this.NeedPropertyUpdateBasedOnResourceChanges)
        this.UpdateAllPropertyValues();
      if (!args.IsDirtyViewState(SceneViewModel.ViewStateBits.KeyFrameSelection) && !args.IsDirtyViewState(SceneViewModel.ViewStateBits.AnimationSelection) && !args.IsDirtyViewState(SceneViewModel.ViewStateBits.StoryboardSelection))
        return;
      if (this.IsCreatingShapeElement)
      {
        this.SceneNodeObjectSet.PropertyUpdatesLocked = true;
      }
      else
      {
        this.UpdateSelection(false, false);
        this.OnPropertyChanged("IsViewRepresentationValid");
      }
    }

    private void PropertyManager_MultiplePropertyReferencesChanged(object sender, MultiplePropertyReferencesChangedEventArgs args)
    {
      bool shouldCloseOpenTransactions = args.IsDirtyViewState(SceneViewModel.ViewStateBits.ElementSelection | SceneViewModel.ViewStateBits.KeyFrameSelection | SceneViewModel.ViewStateBits.AnimationSelection | SceneViewModel.ViewStateBits.StoryboardSelection | SceneViewModel.ViewStateBits.GridColumnSelection | SceneViewModel.ViewStateBits.GridRowSelection | SceneViewModel.ViewStateBits.ChildPropertySelection | SceneViewModel.ViewStateBits.BehaviorSelection);
      bool flag = args.IsDirtyViewState(SceneViewModel.ViewStateBits.IsEditable | SceneViewModel.ViewStateBits.ActiveTrigger | SceneViewModel.ViewStateBits.RecordMode);
      if (!shouldCloseOpenTransactions && !flag)
        return;
      if (this.IsCreatingShapeElement)
        this.SceneNodeObjectSet.PropertyUpdatesLocked = true;
      else
        this.UpdateSelection(args.IsForceUpdate, shouldCloseOpenTransactions);
    }

    private void ViewService_ActiveViewChanged(object sender, ViewChangedEventArgs e)
    {
      if (this.activeDocument != null)
        this.activeDocument.EditTransactionCompleted -= new EventHandler(this.SceneDocument_EditTransactionCompleted);
      if (this.activeProjectContext != null)
        this.activeProjectContext.TypesChanged -= new EventHandler<TypesChangedEventArgs>(this.ProjectContext_TypesChanged);
      this.activeDocument = this.designerContext.ActiveDocument;
      this.activeProjectContext = this.activeDocument != null ? this.activeDocument.ProjectContext : (IProjectContext) null;
      if (this.activeDocument != null)
        this.activeDocument.EditTransactionCompleted += new EventHandler(this.SceneDocument_EditTransactionCompleted);
      if (this.activeProjectContext != null)
        this.activeProjectContext.TypesChanged += new EventHandler<TypesChangedEventArgs>(this.ProjectContext_TypesChanged);
      this.sceneNodeObjectSet.SetActiveViewModel(this.designerContext.ActiveSceneViewModel);
      if (this.activeDocument == null)
        return;
      this.ShowPropertiesEventsToggle = true;
      IPlatform platform = this.activePlatform;
      this.activePlatform = this.activeDocument.ProjectContext.Platform;
      if (platform == this.activePlatform)
        return;
      this.UpdateSelection(true, true);
    }

    private void ProjectContext_TypesChanged(object sender, TypesChangedEventArgs e)
    {
      this.UpdateSelection(true, false);
    }

    private void UpdateAllPropertyValues()
    {
      this.UpdatePropertiesResourcesChangeStamp();
    }

    private void UpdateSelection(bool forceUpdate, bool shouldCloseOpenTransactions)
    {
      if (this.designerContext.SelectionManager == null)
        return;
      SceneNode[] selectedNodes = this.designerContext.SelectionManager.SelectedNodes;
      if (selectedNodes != null && selectedNodes.Length > 0)
      {
        PerformanceUtility.StartPerformanceSequence(PerformanceEvent.PropertyInspectorUpdateSelection);
        PerformanceUtility.MeasurePerformanceUntilRender(PerformanceEvent.PropertyInspectorSelectionToRender);
        PerformanceUtility.EndPerformanceSequenceAfterRender(PerformanceEvent.PropertyInspectorFromCreate);
      }
      SceneView activeView = this.designerContext.ActiveView;
      if (selectedNodes != null && activeView != null)
      {
        bool flag = true;
        if (activeView.IsEditable)
        {
          foreach (SceneNode sceneElement in selectedNodes)
          {
            if (sceneElement.ViewModel.IsDisposed)
            {
              flag = false;
              break;
            }
            ViewState viewState = ViewState.AncestorValid;
            if ((activeView.GetViewState(sceneElement) & viewState) != viewState)
            {
              flag = false;
              break;
            }
          }
          this.IsInvalidElementSelected = !flag;
        }
        else
        {
          this.IsInvalidElementSelected = false;
          flag = false;
        }
        this.selectedObjects = flag ? selectedNodes : new SceneNode[0];
      }
      else
      {
        this.selectedObjects = new SceneNode[0];
        this.IsInvalidElementSelected = false;
      }
      this.UpdateOnSelectionChanged(forceUpdate, shouldCloseOpenTransactions);
      if (selectedNodes == null || selectedNodes.Length <= 0)
        return;
      PerformanceUtility.EndPerformanceSequence(PerformanceEvent.PropertyInspectorUpdateSelection);
    }

    public bool IsCategoryExpanded(string categoryName)
    {
      int categoryForName = this.Categories.FindCategoryForName(categoryName);
      if (categoryForName >= 0)
      {
        bool? nullable = this.Categories[categoryForName].QueryExpandedState(this.inspectingUIElement);
        if (nullable.HasValue)
          return nullable.Value;
      }
      bool flag1;
      if (this.categoryExpandedCache.TryGetValue(categoryName, out flag1))
        return flag1;
      int num;
      switch (CategoryLocalizationHelper.GetCanonicalCategoryName(categoryName))
      {
        case CategoryLocalizationHelper.CategoryName.Brushes:
        case CategoryLocalizationHelper.CategoryName.Layout:
        case CategoryLocalizationHelper.CategoryName.Appearance:
        case CategoryLocalizationHelper.CategoryName.CommonProperties:
        case CategoryLocalizationHelper.CategoryName.Text:
        case CategoryLocalizationHelper.CategoryName.Easing:
        case CategoryLocalizationHelper.CategoryName.Triggers:
        case CategoryLocalizationHelper.CategoryName.Media:
        case CategoryLocalizationHelper.CategoryName.LayoutPaths:
          num = 1;
          break;
        default:
          num = this.categories.Count <= 4 ? 1 : 0;
          break;
      }
      bool flag2 = num != 0;
      this.categoryExpandedCache[categoryName] = flag2;
      return flag2;
    }

    public void UpdateTransaction()
    {
      this.transactionHelper.UpdateTransaction();
    }

    public void UpdateCategoryExpansion(CategoryContainer categoryContainer)
    {
      this.UpdateCategoryExpansion(categoryContainer.Category);
    }

    private void UpdateCategoryExpansion(CategoryBase categoryBase)
    {
      if (!this.inspectingUIElement || categoryBase.Expanded == this.IsCategoryExpanded(categoryBase.CategoryName))
        return;
      this.categoryExpandedCache[categoryBase.CategoryName] = categoryBase.Expanded;
    }

    private void UpdatePropertiesResourcesChangeStamp()
    {
      this.updatedPropertiesResourcesChangeStamp = this.designerContext.ResourceManager.ResourceChangeStamp;
    }

    private string GetSelectionTypeName(bool fullName)
    {
      SelectedElementsObjectSet sceneNodeObjectSet = this.SceneNodeObjectSet;
      if (sceneNodeObjectSet.Count == 0)
        return (string) null;
      if (sceneNodeObjectSet.IsHomogenous)
      {
        if (fullName)
          return sceneNodeObjectSet.ObjectType.FullName;
        return sceneNodeObjectSet.ObjectType.Name;
      }
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.MultiElementSelectionTypeNameFormat, new object[1]
      {
        (object) sceneNodeObjectSet.TypeCount
      });
    }

    private void UpdateFilter()
    {
      PropertyFilter filter = new PropertyFilter(this.FilterString);
      SceneNodeCategoryCollection categories = this.Categories;
      bool flag = false;
      for (int index = 0; index < categories.Count; ++index)
      {
        SceneNodeCategory sceneNodeCategory = categories[index];
        sceneNodeCategory.ApplyFilter(filter);
        flag = ((flag ? 1 : 0) | (sceneNodeCategory.MatchesFilter || sceneNodeCategory.BasicPropertyMatchesFilter ? 1 : (sceneNodeCategory.AdvancedPropertyMatchesFilter ? 1 : 0))) != 0;
      }
      this.HasAnyFilterMatches = flag;
    }

    private void OnClearFilterStringCommand()
    {
      this.FilterString = (string) null;
      IView activeView = this.designerContext.ViewService.ActiveView;
      if (activeView == null)
        return;
      activeView.ReturnFocus();
    }

    public void UpdateOnSelectionChanged(bool forceUpdate, bool shouldCloseOpenTransactions)
    {
      if (this.userInterfaceCount > 0)
      {
        this.referenceStepCollection = PropertyMerger.GetMergedProperties((IEnumerable<SceneNode>) this.selectedObjects);
      }
      else
      {
        this.referenceStepCollection = (IList<TargetedReferenceStep>) new List<TargetedReferenceStep>();
        forceUpdate = true;
      }
      if (this.referenceStepCollection.Count != 0 || this.DesignerContext.ProjectManager.ActiveBuildTarget == null || forceUpdate)
      {
        this.HasSelection = this.referenceStepCollection.Count != 0;
        try
        {
          this.UpdateCategories();
        }
        finally
        {
          this.UpdatePropertiesResourcesChangeStamp();
        }
        this.SceneNodeObjectSet.UpdateOnSelectionChanged();
        this.UpdateFilter();
      }
      else
        this.HasSelection = false;
      if (shouldCloseOpenTransactions)
        this.HandleDanglingTransactions();
      this.UpdateSelectionNameProperty();
      this.OnPropertyChanged("SelectionIconBrush");
      this.OnPropertyChanged("SelectionName");
      this.OnPropertyChanged("SelectionTypeName");
      this.OnPropertyChanged("SelectionTypeNameFull");
      this.OnPropertyChanged("IsInfoBarNameReadOnly");
    }

    public void HandleDanglingTransactions()
    {
      if (!this.transactionHelper.CommitOutstandingTransactions(0))
        return;
      this.ResetModel();
    }

    private void WindowManager_ThemeChanged(object sender, EventArgs args)
    {
      if (this.designerContext.ActiveSceneViewModel == null || this.referenceStepCollection == null)
        return;
      this.ResetModel();
    }

    private void OnPropertyMergerPropertiesUpdated(object sender, EventArgs e)
    {
      UIThreadDispatcher.Instance.BeginInvoke(DispatcherPriority.Send, (Action) (() =>
      {
        if (this.DesignerContext.ProjectManager == null || this.DesignerContext.ProjectManager.ActiveBuildTarget == null)
          return;
        this.UpdateOnSelectionChanged(false, false);
      }));
    }

    private void ResetModel()
    {
      IList<TargetedReferenceStep> list = this.referenceStepCollection;
      this.referenceStepCollection = (IList<TargetedReferenceStep>) new List<TargetedReferenceStep>();
      this.UpdateCategories();
      this.referenceStepCollection = list;
      this.UpdateCategories();
    }

    private Type GetProxyPropertyTypeFromReferenceStep(ReferenceStep referenceStep)
    {
      SceneNode representativeSceneNode = this.sceneNodeObjectSet.RepresentativeSceneNode;
      if (this.ShouldForwardAttributes(representativeSceneNode, referenceStep.Name))
        return this.GetValueTypeForProxyNode(representativeSceneNode);
      return (Type) null;
    }

    private Type GetValueTypeForProxyNode(SceneNode node)
    {
      TimelineSceneNode.PropertyNodePair elementAndProperty = this.GetTargetElementAndProperty(node);
      if (elementAndProperty.PropertyReference != null)
        return elementAndProperty.PropertyReference.ValueType;
      return (Type) null;
    }

    private bool ShouldForwardAttributes(SceneNode sceneNode, string propertyName)
    {
      if (propertyName == "Value" && sceneNode is KeyFrameSceneNode)
        return true;
      if (propertyName == "From" || propertyName == "To")
        return sceneNode is AnimationSceneNode;
      return false;
    }

    private TimelineSceneNode.PropertyNodePair GetTargetElementAndProperty(SceneNode sceneNode)
    {
      AnimationSceneNode animationSceneNode = sceneNode as AnimationSceneNode;
      if (animationSceneNode == null)
      {
        KeyFrameSceneNode keyFrameSceneNode = sceneNode as KeyFrameSceneNode;
        animationSceneNode = keyFrameSceneNode != null ? (AnimationSceneNode) keyFrameSceneNode.KeyFrameAnimation : (AnimationSceneNode) null;
      }
      if (animationSceneNode != null)
        return animationSceneNode.TargetElementAndProperty;
      return new TimelineSceneNode.PropertyNodePair();
    }

    private AttributeCollection GetAttributesFromReferenceStep(TargetedReferenceStep referenceStep)
    {
      AttributeCollection attributeCollection = referenceStep.Attributes;
      bool flag = true;
      if (this.ShouldForwardAttributes(this.SceneNodeObjectSet.RepresentativeSceneNode, referenceStep.ReferenceStep.Name))
      {
        Type type = (Type) null;
        List<Attribute> sourceAttributes = (List<Attribute>) null;
        foreach (object obj in this.SceneNodeObjectSet.Objects)
        {
          PropertyReference propertyReference = this.GetTargetElementAndProperty(obj as SceneNode).PropertyReference;
          if (propertyReference == null)
          {
            attributeCollection = (AttributeCollection) null;
            break;
          }
          if (type == (Type) null)
            type = propertyReference.ValueType;
          else if (!type.IsAssignableFrom(propertyReference.ValueType))
          {
            attributeCollection = (AttributeCollection) null;
            break;
          }
          if (sourceAttributes == null)
          {
            sourceAttributes = this.FilterAttributes(propertyReference.LastStep.Attributes);
            attributeCollection = new AttributeCollection(sourceAttributes.ToArray());
          }
          else
          {
            List<Attribute> destinationAttributes = this.FilterAttributes(propertyReference.LastStep.Attributes);
            if (!this.CompareForwardedPropertyAttributes(sourceAttributes, destinationAttributes))
              flag = false;
          }
          if (!flag)
            break;
        }
      }
      if (!flag)
        attributeCollection = (AttributeCollection) null;
      return attributeCollection;
    }

    private bool CompareForwardedPropertyAttributes(List<Attribute> sourceAttributes, List<Attribute> destinationAttributes)
    {
      bool flag = true;
      foreach (Attribute attribute in destinationAttributes)
      {
        if ((typeof (NumberFormatAttribute).IsAssignableFrom(attribute.GetType()) || typeof (NumberIncrementsAttribute).IsAssignableFrom(attribute.GetType()) || typeof (NumberRangesAttribute).IsAssignableFrom(attribute.GetType())) && !sourceAttributes.Contains(attribute))
        {
          flag = false;
          break;
        }
      }
      return flag;
    }

    private List<Attribute> FilterAttributes(AttributeCollection attributes)
    {
      List<Attribute> list = new List<Attribute>();
      for (int index = 0; index < attributes.Count; ++index)
      {
        Attribute attribute = attributes[index];
        Type type = attributes[index].GetType();
        if (!typeof (CategoryAttribute).IsAssignableFrom(type) && !typeof (PropertyOrderAttribute).IsAssignableFrom(type) && (!typeof (EditorBrowsableAttribute).IsAssignableFrom(type) || ((EditorBrowsableAttribute) attribute).State != EditorBrowsableState.Advanced))
          list.Add(attributes[index]);
      }
      return list;
    }

    private void InsertCategory(int index, SceneNodeCategory sceneNodeCategory)
    {
      sceneNodeCategory.PropertyChanged += new PropertyChangedEventHandler(this.CategoryEntry_PropertyChanged);
      this.Categories.Insert(index, sceneNodeCategory);
    }

    private void RemoveCategoryAt(int index)
    {
      this.Categories[index].PropertyChanged -= new PropertyChangedEventHandler(this.CategoryEntry_PropertyChanged);
      this.Categories.RemoveAt(index);
    }

    private void CategoryEntry_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      SceneNodeCategory sceneNodeCategory = sender as SceneNodeCategory;
      if (!(e.PropertyName == "Expanded"))
        return;
      this.UpdateCategoryExpansion((CategoryBase) sceneNodeCategory);
    }

    private void UpdateCategories()
    {
      this.commonType = this.SceneNodeObjectSet.ObjectTypeId;
      foreach (SceneNodeCategory sceneNodeCategory in (Collection<SceneNodeCategory>) this.Categories)
        sceneNodeCategory.MarkAllPropertiesDisassociated();
      foreach (TargetedReferenceStep targetedReferenceStep in (IEnumerable<TargetedReferenceStep>) this.referenceStepCollection)
      {
        if (PropertyInspectorModel.IsPropertyBrowsable(this.selectedObjects, targetedReferenceStep))
        {
          string str = this.GetCategoryName(targetedReferenceStep);
          CategoryLocalizationHelper.CategoryName canonicalCategoryName = CategoryLocalizationHelper.GetCanonicalCategoryName(str);
          if (canonicalCategoryName != CategoryLocalizationHelper.CategoryName.Unknown)
            str = CategoryLocalizationHelper.GetLocalizedCategoryName(canonicalCategoryName);
          if (!(PlatformTypeHelper.GetPropertyType((IProperty) targetedReferenceStep.ReferenceStep) == (Type) null) && PropertyInspectorModel.IsAttachedPropertyBrowsable(this.selectedObjects, this.commonType, targetedReferenceStep, (ITypeResolver) this.activeProjectContext) && (!PlatformTypes.RowDefinition.IsAssignableFrom((ITypeId) this.commonType) && !PlatformTypes.ColumnDefinition.IsAssignableFrom((ITypeId) this.commonType) || canonicalCategoryName == CategoryLocalizationHelper.CategoryName.Layout))
          {
            PropertyReference propertyReference = new PropertyReference(targetedReferenceStep.ReferenceStep);
            this.sceneNodePropertyLookup.UpdateSubProperties(propertyReference);
            int categoryForName = this.Categories.FindCategoryForName(str);
            if (categoryForName >= 0)
            {
              SceneNodeCategory category = this.Categories[categoryForName];
              if (!this.UpdateCategoryProperty(category, targetedReferenceStep, propertyReference))
              {
                SceneNodeProperty property = this.CreateProperty(targetedReferenceStep, propertyReference);
                if (property != null)
                  this.TryAddPropertyToCategory(property, category);
              }
            }
            else
            {
              SceneNodeProperty property = this.CreateProperty(targetedReferenceStep, propertyReference);
              if (property != null)
              {
                SceneNodeCategory sceneNodeCategory = CategoryFactory.GetCustomCategorySelector((PropertyReferenceProperty) property).CreateSceneNodeCategory(canonicalCategoryName, str, this.DesignerContext.MessageLoggingService);
                this.InsertCategory(~categoryForName, sceneNodeCategory);
                this.TryAddPropertyToCategory(property, sceneNodeCategory);
              }
            }
          }
        }
      }
      if (this.sceneNodeObjectSet.Count > 0 && this.sceneNodeObjectSet.RepresentativeSceneNode is KeyFrameSceneNode)
      {
        string localizedCategoryName = CategoryLocalizationHelper.GetLocalizedCategoryName(CategoryLocalizationHelper.CategoryName.Easing);
        int categoryForName = this.Categories.FindCategoryForName(localizedCategoryName);
        if (categoryForName < 0)
        {
          SceneNodeCategory sceneNodeCategory = (SceneNodeCategory) new EasingCategoryCollection(localizedCategoryName, this.DesignerContext.MessageLoggingService);
          this.InsertCategory(~categoryForName, sceneNodeCategory);
        }
      }
      if (ProjectNeutralTypes.BehaviorTriggerAction.IsAssignableFrom((ITypeId) this.sceneNodeObjectSet.ObjectTypeId))
      {
        string localizedCategoryName1 = CategoryLocalizationHelper.GetLocalizedCategoryName(CategoryLocalizationHelper.CategoryName.Triggers);
        int categoryForName1 = this.Categories.FindCategoryForName(localizedCategoryName1);
        if (categoryForName1 < 0 && this.sceneNodeObjectSet.Count > 0)
        {
          BehaviorTriggerActionNode triggerActionNode = (BehaviorTriggerActionNode) this.sceneNodeObjectSet.Objects[0];
          if (triggerActionNode.Parent != null && triggerActionNode.Parent is BehaviorTriggerBaseNode)
          {
            SceneNodeCategory sceneNodeCategory1 = (SceneNodeCategory) new TriggerCategory(localizedCategoryName1, this.DesignerContext.MessageLoggingService);
            this.InsertCategory(~categoryForName1, sceneNodeCategory1);
            if (this.activeProjectContext != null && this.activeProjectContext.PlatformMetadata.IsCapabilitySet(PlatformCapability.SupportsConditionBehavior))
            {
              string localizedCategoryName2 = CategoryLocalizationHelper.GetLocalizedCategoryName(CategoryLocalizationHelper.CategoryName.Conditions);
              int categoryForName2 = this.Categories.FindCategoryForName(localizedCategoryName2);
              if (categoryForName2 < 0)
              {
                SceneNodeCategory sceneNodeCategory2 = (SceneNodeCategory) new ConditionalExpressionBehaviorCategory(localizedCategoryName2, this.DesignerContext.MessageLoggingService);
                this.InsertCategory(~categoryForName2, sceneNodeCategory2);
              }
            }
          }
        }
      }
      this.sceneNodePropertyLookup.ClearProperties();
      this.sceneNodePropertyLookup.OnPropertiesUpdated();
      for (int index = this.Categories.Count - 1; index >= 0; --index)
      {
        SceneNodeCategory sceneNodeCategory = this.Categories[index];
        sceneNodeCategory.CullDisassociatedProperties();
        sceneNodeCategory.OnSelectionChanged(this.selectedObjects);
        if (sceneNodeCategory.IsEmpty)
          this.RemoveCategoryAt(index);
        else
          sceneNodeCategory.OnItemsChanged();
      }
      CategoryEditorSet categoryEditorSet = new CategoryEditorSet();
      foreach (CategoryEntry categoryEntry in (Collection<SceneNodeCategory>) this.Categories)
      {
        foreach (SceneNodeProperty sceneNodeProperty in categoryEntry.Properties)
        {
          if (sceneNodeProperty.Reference.LastStep.IsAttachable)
            PropertyInspectorModel.GetCategoryEditors(sceneNodeProperty.Reference.LastStep.DeclaringType, categoryEditorSet, this.DesignerContext.MessageLoggingService);
        }
      }
      PropertyInspectorModel.GetCategoryEditors(this.commonType, categoryEditorSet, this.DesignerContext.MessageLoggingService);
      foreach (SceneNodeCategory category in (Collection<SceneNodeCategory>) this.categories)
        categoryEditorSet = categoryEditorSet.Union(CategoryEditorInstanceFactory.GetEditors((ITypeId) this.commonType, category));
      this.UpdateCategoryEditors(categoryEditorSet);
      this.UpdateCategoryEditorOverrideExpandedState();
    }

    private bool UpdateCategoryProperty(SceneNodeCategory category, TargetedReferenceStep targetedReferenceStep, PropertyReference propertyReference)
    {
      SceneNodeProperty property = category.FindProperty(propertyReference);
      if (property == null)
        return false;
      Type proxyType = (Type) null;
      if (property.Reference.PlatformMetadata != propertyReference.PlatformMetadata)
      {
        if (category.CanonicalName == CategoryLocalizationHelper.CategoryName.Brushes || category.CanonicalName == CategoryLocalizationHelper.CategoryName.Transform)
          return false;
        proxyType = this.GetProxyPropertyTypeFromReferenceStep(targetedReferenceStep.ReferenceStep);
      }
      else if (property.HasProxyPropertyType)
        proxyType = property.ProxyPropertyType;
      if (proxyType != (Type) null)
      {
        SceneNode representativeSceneNode = this.SceneNodeObjectSet.RepresentativeSceneNode;
        if (representativeSceneNode == null || !proxyType.IsAssignableFrom(this.GetValueTypeForProxyNode(representativeSceneNode)))
          return false;
      }
      AttributeCollection fromReferenceStep = this.GetAttributesFromReferenceStep(targetedReferenceStep);
      property.UpdateAndRefresh(propertyReference, fromReferenceStep, proxyType);
      return true;
    }

    private void UpdateCategoryEditorOverrideExpandedState()
    {
      SceneNode sceneNode;
      this.inspectingUIElement = Enumerable.Any<SceneNode>((IEnumerable<SceneNode>) this.selectedObjects, (Func<SceneNode, bool>) (sceneObject =>
      {
        if ((sceneNode = sceneObject) != null)
          return PlatformTypes.UIElement.IsAssignableFrom((ITypeId) sceneNode.Type);
        return false;
      }));
      foreach (SceneNodeCategory sceneNodeCategory in (Collection<SceneNodeCategory>) this.Categories)
      {
        int num = (int) CategoryLocalizationHelper.GetCanonicalCategoryName(sceneNodeCategory.CategoryName);
        bool? nullable = sceneNodeCategory.QueryExpandedState(this.inspectingUIElement);
        if (nullable.HasValue)
        {
          sceneNodeCategory.Expanded = nullable.Value;
        }
        else
        {
          bool flag;
          if (!this.categoryExpandedCache.TryGetValue(sceneNodeCategory.CategoryName, out flag))
            flag = this.IsCategoryExpanded(sceneNodeCategory.CategoryName);
          sceneNodeCategory.Expanded = flag;
        }
      }
    }

    private SceneNodeProperty CreateProperty(TargetedReferenceStep referenceStep, PropertyReference propertyReference)
    {
      AttributeCollection fromReferenceStep1 = this.GetAttributesFromReferenceStep(referenceStep);
      Type fromReferenceStep2 = this.GetProxyPropertyTypeFromReferenceStep(referenceStep.ReferenceStep);
      SceneNodeProperty property = (SceneNodeProperty) null;
      if (fromReferenceStep2 == (Type) null)
        property = (SceneNodeProperty) this.SceneNodeObjectSet.CreateProperty(propertyReference, fromReferenceStep1);
      else if (fromReferenceStep1 != null)
        property = !(this.SceneNodeObjectSet.RepresentativeSceneNode is KeyFrameSceneNode) ? (SceneNodeProperty) this.SceneNodeObjectSet.CreateProperty(propertyReference, fromReferenceStep1, fromReferenceStep2) : (SceneNodeProperty) this.SceneNodeObjectSet.CreateKeyframeProperty(propertyReference, fromReferenceStep1, fromReferenceStep2);
      if (property != null)
        this.sceneNodePropertyLookup.AddProperty(propertyReference, property);
      return property;
    }

    private string GetCategoryName(TargetedReferenceStep referenceStep)
    {
      if (BehaviorHelper.IsPropertyBehaviorCommand((IProperty) referenceStep.ReferenceStep))
        return referenceStep.ReferenceStep.Name;
      string result;
      if (PlatformNeutralAttributeHelper.TryGetAttributeValue<string>((IEnumerable) referenceStep.Attributes, PlatformTypes.CategoryAttribute, "Category", out result))
        return result;
      return referenceStep.ReferenceStep.Category;
    }

    private void TryAddPropertyToCategory(SceneNodeProperty newProperty, SceneNodeCategory category)
    {
      if (newProperty == null || newProperty.Reference == null)
        return;
      category.AddProperty(newProperty);
    }

    private void UpdateCategoryEditors(CategoryEditorSet newCategoryEditors)
    {
      CategoryEditorSet categoryEditorSet = this.categoryEditors.Complement(newCategoryEditors);
      foreach (SceneNodeCategory sceneNodeCategory in (Collection<SceneNodeCategory>) this.Categories)
      {
        foreach (CategoryEditor categoryEditor in categoryEditorSet)
          sceneNodeCategory.RemoveCategoryEditor(categoryEditor.GetType());
      }
      using (IEnumerator<CategoryEditor> enumerator = newCategoryEditors.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          CategoryEditor categoryEditor = enumerator.Current;
          if (categoryEditor != null)
          {
            int categoryForName = this.Categories.FindCategoryForName(categoryEditor.TargetCategory);
            if (categoryForName >= 0)
            {
              SceneNodeCategory sceneNodeCategory = this.Categories[categoryForName];
              if (!Enumerable.Any<CategoryEditor>((IEnumerable<CategoryEditor>) sceneNodeCategory.CategoryEditors, (Func<CategoryEditor, bool>) (currentCategoryEditor => currentCategoryEditor.GetType() == categoryEditor.GetType())))
                sceneNodeCategory.AddCategoryEditor(categoryEditor);
            }
          }
        }
      }
      this.categoryEditors = newCategoryEditors;
    }

    public static void GetCategoryEditors(IType type, CategoryEditorSet categoryEditorsList, IMessageLoggingService exceptionLogger)
    {
      foreach (Attribute attribute1 in TypeUtilities.GetAttributes(type != null ? type.RuntimeType : (Type) null))
      {
        EditorAttribute attribute2 = attribute1 as EditorAttribute;
        if (attribute2 != null)
        {
          Type categoryEditorType = ExtensibilityMetadataHelper.GetCategoryEditorType(attribute2, exceptionLogger);
          if (categoryEditorType != (Type) null)
            categoryEditorsList.TryAddCategoryEditorType(categoryEditorType);
        }
      }
    }

    public static bool IsPropertyBrowsable(SceneNode[] selection, TargetedReferenceStep targetedReferenceStep)
    {
      return PropertyInspectorModel.IsPropertyBrowsable(selection, targetedReferenceStep, false, false);
    }

    public static bool IsPropertyBrowsable(SceneNode[] selection, TargetedReferenceStep targetedReferenceStep, bool showReadOnly)
    {
      return PropertyInspectorModel.IsPropertyBrowsable(selection, targetedReferenceStep, showReadOnly, false);
    }

    public static bool IsPropertyBrowsable(SceneNode[] selection, TargetedReferenceStep targetedReferenceStep, bool showReadOnly, bool allowSubclassDefinition)
    {
      ReferenceStep referenceStep = targetedReferenceStep.ReferenceStep;
      SceneNode sceneNode = selection.Length > 0 ? selection[0] : (SceneNode) null;
      ITypeId typeId = (ITypeId) referenceStep.PropertyType;
      DependencyPropertyReferenceStep propertyReferenceStep = referenceStep as DependencyPropertyReferenceStep;
      if (selection.Length == 1 && (sceneNode is StyleNode && (referenceStep.Name == "Style" && PlatformTypes.Style.IsAssignableFrom(typeId) || referenceStep.Name == "Name" && PlatformTypes.String.IsAssignableFrom(typeId) || propertyReferenceStep == null || sceneNode.ProjectContext.IsCapabilitySet(PlatformCapability.WorkaroundSL24722) && referenceStep != null && (GridElement.ColumnDefinitionsProperty.Equals((object) referenceStep) || GridElement.RowDefinitionsProperty.Equals((object) referenceStep) || ItemsControlElement.ItemsProperty.Equals((object) referenceStep))) || !allowSubclassDefinition && sceneNode.IsSubclassDefinition && (referenceStep.DeclaringType.RuntimeType == sceneNode.TrueTargetType && !referenceStep.DeclaringType.RuntimeType.IsAssignableFrom(sceneNode.TargetType)) && !referenceStep.IsAttachable))
        return false;
      if (sceneNode != null)
      {
        if (referenceStep.Name == "Effect" && PlatformTypes.Effect.IsAssignableFrom(typeId) && !sceneNode.ProjectContext.IsCapabilitySet(PlatformCapability.SupportsUIElementEffectProperty))
          return false;
        if (sceneNode.ProjectContext.IsCapabilitySet(PlatformCapability.SupportsUIElementEffectProperty) && referenceStep.Name == "BitmapEffect" && PlatformTypes.BitmapEffect.IsAssignableFrom(typeId) || referenceStep.Name == "BitmapEffectInput" && PlatformTypes.BitmapEffectInput.IsAssignableFrom(typeId))
        {
          bool flag = false;
          for (int index = 0; index < selection.Length; ++index)
          {
            sceneNode = selection[index];
            if (sceneNode.IsSet(Base2DElement.BitmapEffectProperty) == PropertyState.Set || sceneNode.IsSet(Base2DElement.BitmapEffectInputProperty) == PropertyState.Set)
            {
              flag = true;
              break;
            }
          }
          if (!flag)
            return false;
        }
      }
      if (propertyReferenceStep != null && PlatformTypes.Brush.IsAssignableFrom((ITypeId) propertyReferenceStep.PropertyType) && propertyReferenceStep.GetDefaultValue(propertyReferenceStep.TargetType) == Effect.ImplicitInput)
        return false;
      if (referenceStep.Name == "IsItemsHost")
      {
        if (selection.Length != 1)
          return false;
        ControlTemplateElement controlTemplateElement = selection[0].StoryboardContainer as ControlTemplateElement;
        if (controlTemplateElement == null || controlTemplateElement.ControlTemplateTargetTypeId == null || !PlatformTypes.ItemsControl.IsAssignableFrom(controlTemplateElement.ControlTemplateTargetTypeId))
          return false;
      }
      if (referenceStep.Equals((object) Visual3DElement.TransformProperty) && Enumerable.All<SceneNode>((IEnumerable<SceneNode>) selection, (Func<SceneNode, bool>) (node => PlatformTypes.ModelVisual3D.IsAssignableFrom((ITypeId) node.Type))) || selection.Length > 0 && !sceneNode.ProjectContext.IsCapabilitySet(PlatformCapability.IsWpf) && (typeId != null && PlatformTypes.ResourceDictionary.IsAssignableFrom(typeId)))
        return false;
      AttributeCollection attributes = targetedReferenceStep.Attributes;
      bool result1;
      if (PlatformNeutralAttributeHelper.TryGetAttributeValue<bool>((IEnumerable) attributes, PlatformTypes.BrowsableAttribute, "Browsable", out result1))
        return result1;
      object result2;
      if (PlatformNeutralAttributeHelper.TryGetAttributeValue<object>((IEnumerable) attributes, PlatformTypes.EditorBrowsableAttribute, "State", out result2) && (int) result2 != 0 && (int) result2 != 2)
        return false;
      if ((referenceStep.WriteAccess & MemberAccessType.Public) != MemberAccessType.None || showReadOnly)
        return true;
      if (typeId == null)
        return false;
      if (BehaviorHelper.IsPropertyBehaviorCommand((IProperty) referenceStep))
        return true;
      if (!PlatformTypes.ICollection.IsAssignableFrom(typeId) || PlatformTypes.Array.IsAssignableFrom(typeId) || PlatformTypes.UIElementCollection.IsAssignableFrom(typeId))
        return false;
      Type runtimeType = sceneNode.ProjectContext.ResolveType(PlatformTypes.ReadOnlyCollection).RuntimeType;
      IType type = sceneNode.ProjectContext.ResolveType(typeId);
      return type == null || !PlatformTypes.IsGenericTypeDefinitionOf(runtimeType, type.RuntimeType);
    }

    internal static bool IsAttachedPropertyBrowsable(SceneNode[] selection, IType commonType, TargetedReferenceStep targetedReferenceStep, ITypeResolver typeResolver)
    {
      ReferenceStep referenceStep = targetedReferenceStep.ReferenceStep;
      if (!referenceStep.IsAttachable)
        return true;
      Type declaringType = PlatformTypeHelper.GetDeclaringType((IMember) referenceStep);
      if (declaringType == (Type) null)
        return false;
      bool flag1 = PlatformTypes.TextElement.IsAssignableFrom((ITypeId) commonType) || PlatformTypes.Inline.IsAssignableFrom((ITypeId) commonType);
      if (PlatformTypes.TextElement.Equals((object) referenceStep.DeclaringType))
      {
        switch (referenceStep.Name)
        {
          case "FontFamily":
          case "FontSize":
          case "FontStretch":
          case "FontStyle":
          case "FontWeight":
          case "Foreground":
            return flag1;
        }
      }
      else if (PlatformTypes.Block.Equals((object) referenceStep.DeclaringType))
      {
        if (flag1 && (referenceStep.Name == "LineHeight" || referenceStep.Name == "LineStackingStrategy" || referenceStep.Name == "TextAlignment"))
          return true;
      }
      else
      {
        if (PlatformTypes.TextBlock.Equals((object) referenceStep.DeclaringType))
          return PlatformTypes.TextBlock.IsAssignableFrom((ITypeId) commonType);
        if (PlatformTypes.ScrollViewer.Equals((object) referenceStep.DeclaringType) && (PlatformTypes.TextBoxBase.IsAssignableFrom((ITypeId) commonType) || PlatformTypes.RichTextBox.IsAssignableFrom((ITypeId) commonType) || PlatformTypes.TextBox.IsAssignableFrom((ITypeId) commonType)) && (referenceStep.Name == "VerticalScrollBarVisibility" || referenceStep.Name == "HorizontalScrollBarVisibility"))
          return false;
      }
      MemberAccessTypes memberAccessTypes = (MemberAccessTypes) PlatformTypeHelper.GetMemberAccess(declaringType);
      if ((memberAccessTypes & MemberAccessTypes.Public) == MemberAccessTypes.None && (typeResolver == null || (TypeHelper.GetAllowableMemberAccess(typeResolver, referenceStep.DeclaringType) & memberAccessTypes) == MemberAccessTypes.None))
        return false;
      Assembly assembly1 = declaringType.Assembly;
      if (PlatformTypes.DesignToolAssemblies.Contains(assembly1))
        return false;
      bool flag2 = false;
      foreach (IAssembly assembly2 in (IEnumerable<IAssembly>) SystemThemeAssemblies.ThemeAssemblyReferences)
      {
        if (assembly2.CompareTo(assembly1))
        {
          flag2 = true;
          break;
        }
      }
      if (flag2)
        return false;
      bool flag3 = false;
      AttributeCollection attributes = targetedReferenceStep.Attributes;
      bool result1;
      if (PlatformNeutralAttributeHelper.TryGetAttributeValue<bool>((IEnumerable) attributes, PlatformTypes.BrowsableAttribute, "Browsable", out result1) && result1)
        flag3 = true;
      object result2;
      if (PlatformNeutralAttributeHelper.TryGetAttributeValue<object>((IEnumerable) attributes, PlatformTypes.EditorBrowsableAttribute, "State", out result2) && ((int) result2 == 0 || (int) result2 == 2))
        flag3 = true;
      bool flag4 = false;
      bool flag5 = false;
      AttachedPropertyBrowsableForChildrenAttribute childrenAttribute = (AttachedPropertyBrowsableForChildrenAttribute) attributes[typeof (AttachedPropertyBrowsableForChildrenAttribute)];
      if (childrenAttribute != null)
      {
        flag5 = true;
        bool flag6 = true;
        foreach (SceneNode sceneNode in selection)
        {
          bool flag7;
          if (childrenAttribute.IncludeDescendants)
          {
            flag7 = sceneNode.GetAncestors(false).Exists((Predicate<SceneNode>) (ancestor => declaringType.IsAssignableFrom(ancestor.TrueTargetType)));
          }
          else
          {
            Type type = declaringType;
            if (referenceStep.Equals((object) CanvasElement.CanvasZIndexProperty))
              type = sceneNode.Platform.Metadata.ResolveType(PlatformTypes.Panel).RuntimeType;
            flag7 = sceneNode.Parent != null && type.IsAssignableFrom(sceneNode.Parent.TrueTargetType);
          }
          if (!flag7)
          {
            flag6 = false;
            break;
          }
        }
        if (flag6)
          flag4 = true;
      }
      if (!flag4)
      {
        IEnumerable<AttachedPropertyBrowsableForTypeAttribute> source = Enumerable.OfType<AttachedPropertyBrowsableForTypeAttribute>((IEnumerable) attributes);
        if (Enumerable.Any<AttachedPropertyBrowsableForTypeAttribute>(source))
        {
          flag5 = true;
          bool flag6 = true;
          foreach (object obj in selection)
          {
            SceneNode sceneNode = obj as SceneNode;
            if (sceneNode != null)
            {
              bool flag7 = false;
              foreach (AttachedPropertyBrowsableForTypeAttribute forTypeAttribute in source)
              {
                if (forTypeAttribute.TargetType.IsAssignableFrom(sceneNode.TrueTargetType))
                {
                  flag7 = true;
                  break;
                }
              }
              if (!flag7)
                flag6 = false;
            }
          }
          if (flag6)
            flag4 = true;
        }
      }
      if (!flag4)
      {
        AttachedPropertyBrowsableWhenAttributePresentAttribute presentAttribute = (AttachedPropertyBrowsableWhenAttributePresentAttribute) attributes[typeof (AttachedPropertyBrowsableWhenAttributePresentAttribute)];
        if (presentAttribute != null)
        {
          flag5 = true;
          bool flag6 = true;
          foreach (object obj in selection)
          {
            SceneNode sceneNode = obj as SceneNode;
            if (sceneNode != null)
            {
              Attribute attribute = TypeUtilities.GetAttributes(sceneNode.TrueTargetType)[presentAttribute.AttributeType];
              if (attribute == null || attribute.IsDefaultAttribute())
              {
                flag6 = false;
                break;
              }
            }
          }
          if (flag6)
            flag4 = true;
        }
      }
      if (flag5)
        return flag4;
      return flag3;
    }

    private void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
