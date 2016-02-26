// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Tool
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Designers;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.Selection;
using Microsoft.Expression.DesignSurface.Tools.Path;
using Microsoft.Expression.DesignSurface.Tools.Selection;
using Microsoft.Expression.DesignSurface.Tools.Transforms;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.Expression.Framework.Documents;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools
{
  public abstract class Tool
  {
    private List<IAdornerSet> adornerSetList = new List<IAdornerSet>();
    private Dictionary<Viewport3DElement, bool> hasCreatedViewport3DAdorners = new Dictionary<Viewport3DElement, bool>();
    private ToolContext toolContext;
    private bool needToRebuildAdornerSets;
    private SceneElement multiselectionAdornerElement;
    private bool showDimensions;

    public abstract string Identifier { get; }

    public abstract string Caption { get; }

    public virtual string Description
    {
      get
      {
        return (string) null;
      }
    }

    public abstract Key Key { get; }

    public abstract ToolCategory Category { get; }

    public virtual string IconBrushKey
    {
      get
      {
        return (string) null;
      }
    }

    public virtual DrawingBrush NormalIconBrush
    {
      get
      {
        if (this.IconBrushKey != null)
          return this.GetToolBrush(this.IconBrushKey, false);
        return (DrawingBrush) null;
      }
    }

    public virtual DrawingBrush HoverIconBrush
    {
      get
      {
        if (this.IconBrushKey != null)
          return this.GetToolBrush(this.IconBrushKey, true);
        return (DrawingBrush) null;
      }
    }

    internal ToolContext ToolContext
    {
      get
      {
        return this.toolContext;
      }
    }

    public Tool AdornerOwnerTool
    {
      get
      {
        return this.ToolContext.ToolManager.OverrideTool ?? this;
      }
    }

    public SceneDocument ActiveDocument
    {
      get
      {
        SceneView activeView = this.ActiveView;
        if (activeView == null)
          return (SceneDocument) null;
        return activeView.Document;
      }
    }

    public SceneView ActiveView
    {
      get
      {
        return this.toolContext.ActiveView;
      }
    }

    public SceneViewModel ActiveSceneViewModel
    {
      get
      {
        SceneView activeView = this.ActiveView;
        if (activeView == null)
          return (SceneViewModel) null;
        return activeView.ViewModel;
      }
    }

    public bool ShowDimensions
    {
      get
      {
        return this.showDimensions;
      }
      set
      {
        this.showDimensions = value;
      }
    }

    internal virtual bool ShowExtensibleAdorners
    {
      get
      {
        return false;
      }
    }

    internal ElementToPathEditorTargetMap PathEditorTargetMap
    {
      get
      {
        return this.ToolContext.PathEditorTargetMap;
      }
    }

    private IList<AdornerElementSet> AdornerElementList
    {
      get
      {
        if (this.ActiveDocument == null)
          return (IList<AdornerElementSet>) null;
        List<AdornerElementSet> list1 = new List<AdornerElementSet>();
        ISceneInsertionPoint sceneInsertionPoint = this.ActiveSceneViewModel.ActiveSceneInsertionPoint;
        if (sceneInsertionPoint != null)
        {
          SceneElement sceneElement = sceneInsertionPoint.SceneElement;
          if (sceneElement is BaseFrameworkElement || sceneElement is Base3DElement || (sceneElement is StyleNode || sceneElement is FrameworkTemplateElement))
          {
            this.TryAdornElement(sceneElement, (IList<AdornerElementSet>) list1);
            if (!(sceneElement is Viewport3DElement) && !(sceneElement is Base3DElement))
            {
              foreach (SceneNode sceneNode in sceneElement.GetAllContent())
                this.TryAdornElement(sceneNode as SceneElement, (IList<AdornerElementSet>) list1);
            }
          }
        }
        ICollection<SceneElement> collection = (ICollection<SceneElement>) this.ActiveSceneViewModel.ElementSelectionSet.Selection;
        List<SceneElement> list2 = new List<SceneElement>();
        foreach (SceneElement element in (IEnumerable<SceneElement>) collection)
        {
          Base3DElement base3Delement = element as Base3DElement;
          if (base3Delement == null || (SceneElement) base3Delement.Viewport != null && base3Delement.IsSelectable)
          {
            if (element is Base3DElement)
              this.TryAdornElement(element, (IList<AdornerElementSet>) list1);
            else if (element.IsAttached)
              list2.Add(element);
          }
        }
        GridRowSelectionSet gridRowSelectionSet = this.ActiveSceneViewModel.DesignerContext.SelectionManager.GridRowSelectionSet;
        if (gridRowSelectionSet != null)
        {
          RowDefinitionNode primarySelection = gridRowSelectionSet.PrimarySelection;
          if (primarySelection != null)
            this.TryAdornElement(primarySelection.Parent as SceneElement, (IList<AdornerElementSet>) list1);
        }
        GridColumnSelectionSet columnSelectionSet = this.ActiveSceneViewModel.DesignerContext.SelectionManager.GridColumnSelectionSet;
        if (columnSelectionSet != null)
        {
          ColumnDefinitionNode primarySelection = columnSelectionSet.PrimarySelection;
          if (primarySelection != null)
            this.TryAdornElement(primarySelection.Parent as SceneElement, (IList<AdornerElementSet>) list1);
        }
        SceneElement primarySelection1 = this.ActiveSceneViewModel.ElementSelectionSet.PrimarySelection;
        BaseFrameworkElement panelClosestToRoot = this.ActiveSceneViewModel.FindPanelClosestToRoot();
        SceneNode editingContainer = this.ActiveSceneViewModel.ActiveEditingContainer;
        if (primarySelection1 != null && primarySelection1 != panelClosestToRoot && primarySelection1 != editingContainer)
        {
          SceneElement parentElement = primarySelection1.ParentElement;
          if (parentElement != null)
            this.TryAdornElement(parentElement, (IList<AdornerElementSet>) list1);
        }
        this.multiselectionAdornerElement = (SceneElement) null;
        if (list2.Count > 1)
        {
          SceneElementCollection elements = new SceneElementCollection();
          elements.AddRange((IEnumerable<SceneElement>) this.ActiveSceneViewModel.ElementSelectionSet.Selection);
          AdornerElementSet adornerElementSet = new AdornerElementSet(elements);
          list1.Add(adornerElementSet);
          this.multiselectionAdornerElement = this.ActiveSceneViewModel.ElementSelectionSet.PrimarySelection;
          adornerElementSet.PrimaryElement = this.multiselectionAdornerElement;
        }
        else if (list2.Count == 1)
        {
          this.multiselectionAdornerElement = this.ActiveSceneViewModel.ElementSelectionSet.PrimarySelection;
          if (!list2.Contains(this.multiselectionAdornerElement))
            this.multiselectionAdornerElement = list2[0];
          this.TryAdornElement(this.multiselectionAdornerElement, (IList<AdornerElementSet>) list1);
        }
        foreach (SceneElement element in (IEnumerable<SceneElement>) this.ActiveView.GetInvalidElements())
        {
          if (element is BaseFrameworkElement && element.IsAttached)
          {
            bool flag = false;
            foreach (AdornerElementSet adornerElementSet in list1)
            {
              if (adornerElementSet.Elements.Contains(element))
              {
                flag = true;
                break;
              }
            }
            if (!flag)
              list1.Add(this.ActiveView.AdornerLayer.CreateOrGetAdornerElementSetForElement(element));
          }
        }
        foreach (SceneElement element in (IEnumerable<SceneElement>) this.ActiveView.GetUserControlInstances())
        {
          if (element.IsAttached)
            this.TryAdornElement(element, (IList<AdornerElementSet>) list1);
        }
        foreach (SceneElement rootElement in this.ActiveView.AdornerLayer.ContainersWithProxiedLights)
        {
          foreach (SceneElement element in SceneElementHelper.GetElementTree(rootElement))
          {
            if (element is LightElement)
              this.TryAdornElement(element, (IList<AdornerElementSet>) list1);
          }
        }
        return (IList<AdornerElementSet>) list1;
      }
    }

    protected virtual bool UseDefaultEditingAdorners
    {
      get
      {
        return true;
      }
    }

    protected virtual bool AdornMultipleElementsAsOne
    {
      get
      {
        return true;
      }
    }

    protected virtual ViewState RequiredSelectionViewState
    {
      get
      {
        return ViewState.None;
      }
    }

    protected virtual ViewState RequiredActiveElementViewState
    {
      get
      {
        return ViewState.ElementValid | ViewState.AncestorValid;
      }
    }

    protected bool IsActiveViewValid
    {
      get
      {
        if (this.ActiveDocument != null && this.ActiveSceneViewModel != null)
          return this.ActiveSceneViewModel.DefaultView.IsDesignSurfaceVisible;
        return false;
      }
    }

    public virtual bool IsEnabled
    {
      get
      {
        if (!this.IsActiveViewValid)
          return false;
        ISceneInsertionPoint sceneInsertionPoint = this.ActiveSceneViewModel.ActiveSceneInsertionPoint;
        if (sceneInsertionPoint == null)
          return false;
        ViewState selectionViewState = this.RequiredSelectionViewState;
        if (selectionViewState != ViewState.None && (selectionViewState & this.ActiveSceneViewModel.DefaultView.GetViewStateForSelection()) != selectionViewState)
          return false;
        SceneElement sceneElement = sceneInsertionPoint.SceneElement;
        ViewState elementViewState = this.RequiredActiveElementViewState;
        if (elementViewState != ViewState.None && (elementViewState & this.ActiveSceneViewModel.DefaultView.GetViewState((SceneNode) sceneElement)) != elementViewState)
          return false;
        SceneElement primarySelection = this.ActiveSceneViewModel.ElementSelectionSet.PrimarySelection;
        if (!(sceneElement is Base3DElement) && !(sceneElement is Viewport3DElement))
          return !(primarySelection is Base3DElement);
        return false;
      }
    }

    public virtual bool IsVisible
    {
      get
      {
        return true;
      }
    }

    public virtual bool KeepTextSelectionSet
    {
      get
      {
        return false;
      }
    }

    internal Tool(ToolContext toolContext)
    {
      this.toolContext = toolContext;
    }

    protected bool CanAdornElement(SceneElement element)
    {
      if (element != null && element.IsInstantiatedElementVisible && !element.IsHiddenOrCollapsedOrAncestorHiddenOrCollapsed)
      {
        if (element.ViewTargetElement is IViewVisual)
          return this.ActiveView.IsInArtboard(element);
        Base3DElement base3Delement = element as Base3DElement;
        if (base3Delement != null)
          return this.CanAdornElement((SceneElement) base3Delement.Viewport);
      }
      return false;
    }

    protected void TryAdornElement(SceneElement element, IList<AdornerElementSet> adornedElementsSet)
    {
      if (!this.CanAdornElement(element))
        return;
      bool flag = false;
      foreach (AdornerElementSet adornerElementSet in (IEnumerable<AdornerElementSet>) adornedElementsSet)
      {
        if (adornerElementSet.Elements.Contains(element))
        {
          flag = true;
          break;
        }
      }
      if (flag)
        return;
      adornedElementsSet.Add(this.ActiveView.AdornerLayer.CreateOrGetAdornerElementSetForElement(element));
    }

    internal ToolBehaviorContext GetActiveViewContext()
    {
      return new ToolBehaviorContext(this.ToolContext, this, this.ActiveView);
    }

    public virtual ContextMenu CreateContextMenu()
    {
      return (ContextMenu) null;
    }

    public void DoubleClick()
    {
      this.OnDoubleClick();
    }

    public void Activate()
    {
      if (this.ActiveView == null || !this.ActiveView.IsDesignSurfaceVisible)
        return;
      ToolBehavior toolBehavior = this.CreateToolBehavior();
      this.ActiveSceneViewModel.EarlySceneUpdatePhase += new SceneUpdatePhaseEventHandler(this.ActiveSceneViewModel_EarlySceneUpdatePhase);
      this.RebuildAdornerSets();
      this.ActiveView.EventRouter.PushBehavior(toolBehavior);
      this.OnActivate();
    }

    public void Deactivate()
    {
      if (this.ActiveView == null || !this.ActiveView.IsDesignSurfaceVisible)
        return;
      this.ToolContext.ToolManager.OverrideTool = (Tool) null;
      this.OnDeactivate();
      this.ActiveView.EventRouter.PopAllBehaviors();
      if (this.ActiveDocument == null)
        return;
      this.ActiveSceneViewModel.EarlySceneUpdatePhase -= new SceneUpdatePhaseEventHandler(this.ActiveSceneViewModel_EarlySceneUpdatePhase);
      this.ClearAdornerSets();
    }

    public void RebuildAdornerSets()
    {
      using (this.ActiveView != null ? this.ActiveView.ViewModel.ScopeViewObjectCache() : (IDisposable) null)
      {
        if (this.ActiveView != null)
          this.ActiveView.AdornerLayer.ClearAdornerElementSets();
        this.AdornerOwnerTool.AdornElements(this.AdornerOwnerTool.AdornerElementList);
        this.needToRebuildAdornerSets = false;
      }
    }

    public void ClearAdornerSets()
    {
      this.AdornerOwnerTool.AdornElements((IList<AdornerElementSet>) new List<AdornerElementSet>());
    }

    public DrawingBrush GetToolBrush(string name, bool hover)
    {
      return (DrawingBrush) Application.Current.MainWindow.TryFindResource((object) ("tool_" + name + (hover ? "_on" : "_off") + "_24x24"));
    }

    public virtual PathEditorTarget GetPathEditorTarget(Base2DElement element, PathEditMode pathEditMode)
    {
      return this.ToolContext.PathEditorTargetMap.GetPathEditorTarget(element, pathEditMode);
    }

    public virtual IEnumerable<PathEditorTarget> GetAllPathEditorTargets(SceneElement element)
    {
      return this.ToolContext.PathEditorTargetMap.GetAllPathEditorTargets(element);
    }

    public virtual SelectionAdornerUsages GetSelectionAdornerUsages(SceneElement element)
    {
      SelectionAdornerUsages selectionAdornerUsages = SelectionAdornerUsages.None;
      if (this.ShowDimensions)
        selectionAdornerUsages = selectionAdornerUsages | SelectionAdornerUsages.ShowDimension | SelectionAdornerUsages.ShowBoundingBox;
      return !(element is ShapeElement) ? selectionAdornerUsages | SelectionAdornerUsages.ShowBoundingBox : selectionAdornerUsages | SelectionAdornerUsages.ShowGeometry;
    }

    protected virtual void AddEditingAdornerSetCreatorsForSelectedElement(IList<IAdornerSetCreator> adornerSetCreators, SceneElement element)
    {
    }

    protected virtual void AddEditingAdornerSetCreatorsForSelectedElements(IList<IAdornerSetCreator> adornerSetCreators, AdornerElementSet adornerElementSet)
    {
    }

    protected abstract ToolBehavior CreateToolBehavior();

    protected virtual void OnDoubleClick()
    {
    }

    protected virtual void OnActivate()
    {
    }

    protected virtual void OnDeactivate()
    {
      this.PathEditorTargetMap.Clear();
    }

    protected virtual void OnEarlySceneUpdatePhase(SceneUpdatePhaseEventArgs args)
    {
      foreach (PathEditorTarget pathEditorTarget in this.PathEditorTargetMap.PathEditorTargets)
        pathEditorTarget.UpdateFromDamage(args);
    }

    protected virtual bool DoesAdornerElementListNeedUpdating(IPropertyId changedProperty)
    {
      return changedProperty.Equals((object) Base2DElement.VisibilityProperty) || DesignTimeProperties.IsHiddenProperty.Equals((object) changedProperty) || (changedProperty.Equals((object) PopupElement.IsOpenProperty) || DesignTimeProperties.IsPopupOpenProperty.Equals((object) changedProperty));
    }

    protected bool CreateDefaultElement(ITypeId type)
    {
      SceneView activeView = this.ToolContext.ActiveView;
      if (!activeView.ViewModel.ActiveSceneInsertionPoint.CanInsert(type))
        return false;
      new UserThemeTypeInstantiator(activeView).CreateInstance(type, activeView.ViewModel.ActiveSceneInsertionPoint, Rect.Empty, (OnCreateInstanceAction) null);
      return true;
    }

    protected DrawingBrush GetIconBrushResource(string name)
    {
      return Application.Current.Resources[(object) name] as DrawingBrush;
    }

    protected DrawingBrush CreateDrawingBrushFromType(ITypeId type, bool isSelected)
    {
      ITypeResolver typeResolver = this.ActiveDocument != null ? (ITypeResolver) this.ActiveDocument.ProjectContext : this.toolContext.DesignerDefaultPlatform.Metadata.DefaultTypeResolver;
      type = (ITypeId) typeResolver.ResolveType(type);
      if (!typeResolver.PlatformMetadata.IsSupported(typeResolver, type))
        return IconMapper.GetDrawingBrushForType(PlatformTypes.FrameworkElement, isSelected, 24, 24);
      return IconMapper.GetDrawingBrushForType(type, isSelected, 24, 24);
    }

    private void ActiveSceneViewModel_EarlySceneUpdatePhase(object sender, SceneUpdatePhaseEventArgs args)
    {
      this.OnEarlySceneUpdatePhase(args);
      this.needToRebuildAdornerSets = false;
      if (args.IsDirtyViewState(SceneViewModel.ViewStateBits.ElementSelection | SceneViewModel.ViewStateBits.GridColumnSelection | SceneViewModel.ViewStateBits.GridRowSelection | SceneViewModel.ViewStateBits.ActiveSceneInsertionPoint | SceneViewModel.ViewStateBits.LockedInsertionPoint | SceneViewModel.ViewStateBits.ActiveStateContext) && this.ToolContext.ToolManager.ActiveTool == this)
        this.needToRebuildAdornerSets = true;
      if (args.DocumentChanges.Count > 0 && this.ToolContext.ToolManager.ActiveTool == this)
      {
        foreach (PropertySceneChange propertySceneChange in SceneChange.ChangesOfType<PropertySceneChange>(args.DocumentChanges, this.ActiveSceneViewModel.RootNode))
        {
          DesignTimePropertySceneChange args1 = propertySceneChange as DesignTimePropertySceneChange;
          if (args1 != null)
            this.OnDesignTimePropertyChanged(args1);
          else
            this.OnPropertyReferenceChanged((PropertyReferenceSceneChange) propertySceneChange);
        }
      }
      if (!this.needToRebuildAdornerSets)
        return;
      this.RebuildAdornerSets();
    }

    private void OnPropertyReferenceChanged(PropertyReferenceSceneChange args)
    {
      PropertyReference propertyChanged = args.PropertyChanged;
      if (propertyChanged == null || propertyChanged.Count <= 0 || !this.DoesAdornerElementListNeedUpdating((IPropertyId) propertyChanged[0]))
        return;
      this.needToRebuildAdornerSets = true;
    }

    private void OnDesignTimePropertyChanged(DesignTimePropertySceneChange args)
    {
      if (!this.DoesAdornerElementListNeedUpdating(args.DesignTimePropertyKey))
        return;
      this.needToRebuildAdornerSets = true;
    }

    private List<IAdornerSetCreator> GetAdornerSetCreatorsForElement(ToolBehaviorContext behaviorContext, AdornerElementSet adornerElementSet)
    {
      List<IAdornerSetCreator> list = new List<IAdornerSetCreator>();
      IAdornerSetCreator adornerSetCreator = (IAdornerSetCreator) new BoundingBoxAdornerSetCreator();
      list.Add(adornerSetCreator);
      if (this.UseDefaultEditingAdorners)
        this.AddDefaultEditingAdornerSetCreatorsForSelectedElements((IList<IAdornerSetCreator>) list, adornerElementSet);
      else
        this.AddEditingAdornerSetCreatorsForSelectedElements((IList<IAdornerSetCreator>) list, adornerElementSet);
      return list;
    }

    private List<IAdornerSetCreator> GetAdornerSetCreatorsForElement(ToolBehaviorContext behaviorContext, SceneElement element)
    {
      List<IAdornerSetCreator> list = new List<IAdornerSetCreator>();
      SceneView view = behaviorContext.View;
      SceneViewModel viewModel = view.ViewModel;
      ISceneInsertionPoint lockedInsertionPoint = view.ViewModel.LockedInsertionPoint;
      ISceneInsertionPoint sceneInsertionPoint = view.ViewModel.ActiveSceneInsertionPoint;
      SceneElement primarySelection = this.ActiveSceneViewModel.ElementSelectionSet.PrimarySelection;
      bool flag1 = this.ActiveSceneViewModel.ElementSelectionSet.IsSelected(element);
      if (this.ActiveView.GetException(element.DocumentNodePath) != null)
        list.Add((IAdornerSetCreator) new InvalidElementAdornerSetCreator());
      if (flag1 && (bool) element.GetLocalOrDefaultValue(DesignTimeProperties.IsPrototypingCompositionProperty))
        list.Add((IAdornerSetCreator) new PrototypingCompositionBadgeAdornerSetCreator());
      if (element.ViewTargetElement != null)
      {
        BaseFrameworkElement frameworkElement = element as BaseFrameworkElement;
        if (frameworkElement != null || element is StyleNode || element is FrameworkTemplateElement)
        {
          bool isParentOfEntireSelection = false;
          if (primarySelection != null)
          {
            SceneElement parentContainer = primarySelection.VisualElementAncestor;
            if (parentContainer == element)
              isParentOfEntireSelection = Enumerable.All<SceneElement>((IEnumerable<SceneElement>) this.ActiveSceneViewModel.ElementSelectionSet.Selection, (Func<SceneElement, bool>) (elem => elem.VisualElementAncestor == parentContainer));
          }
          if (element != viewModel.RootNode)
          {
            if (!flag1 && this.toolContext.ToolManager.ShowBoundaries && (element.ParentElement != null && sceneInsertionPoint != null) && element.ParentElement == sceneInsertionPoint.SceneElement)
              list.Add((IAdornerSetCreator) new BoundingBoxAdornerSetCreator());
            if (!flag1 && primarySelection != null && (primarySelection.ParentElement == element && isParentOfEntireSelection))
              list.Add((IAdornerSetCreator) new DimmedBoundingBoxAdornerSetCreator());
            string xamlSourcePath = element.Type.XamlSourcePath;
            if (xamlSourcePath != null)
            {
              bool flag2 = true;
              DocumentReference documentReference = DocumentReference.Create(xamlSourcePath);
              IProjectDocument document = element.ProjectContext.GetDocument(DocumentReferenceLocator.GetDocumentLocator(documentReference));
              if (document == null || document.Document == null || !((IDocument) document.Document).IsDirty)
              {
                if (element.ProjectContext.ProjectAssembly == null)
                {
                  flag2 = true;
                }
                else
                {
                  IAssembly projectAssembly = element.ProjectContext.ProjectAssembly;
                  if (projectAssembly.IsLoaded)
                  {
                    FileInfo fileInfo = new FileInfo(projectAssembly.Location);
                    if (fileInfo.Exists)
                      flag2 = new FileInfo(xamlSourcePath).LastWriteTimeUtc > fileInfo.LastWriteTimeUtc;
                  }
                }
              }
              if (flag2)
                list.Add((IAdornerSetCreator) new StaleTypeAdornerSetCreator());
            }
          }
          if (lockedInsertionPoint != null && element == lockedInsertionPoint.SceneElement && this.ToolContext.ToolManager.ShowActiveContainer)
            list.Add((IAdornerSetCreator) new LockedInsertionPointAdornerSetCreator());
          if (!typeof (Popup).IsAssignableFrom(element.TargetType))
          {
            if (this.ShowGridRowColumnAdornersForElement(element, isParentOfEntireSelection))
            {
              list.Add((IAdornerSetCreator) new GridRowColumnAdornerSetCreator());
              if (!this.ActiveSceneViewModel.ElementSelectionSet.IsSelected(element))
                list.Add((IAdornerSetCreator) new BoundingBoxAdornerSetCreator());
            }
            else if (this.ShowDimmedGridRowColumnAdornersForElement(element))
              list.Add((IAdornerSetCreator) new DimmedGridRowColumnAdornerSetCreator());
            if (element == this.multiselectionAdornerElement || flag1)
              list.Add((IAdornerSetCreator) new HighlightAdornerSetCreator());
            if (element == this.ActiveSceneViewModel.ElementSelectionSet.PrimarySelection || !this.AdornMultipleElementsAsOne && flag1)
            {
              if (this.ActiveSceneViewModel.RootNode != element && frameworkElement != null)
                list.Add((IAdornerSetCreator) new MotionPathAdornerSetCreator());
              if (this.UseDefaultEditingAdorners && (this.ActiveView.EventRouter.ActiveBehavior == null || this.ActiveView.EventRouter.ActiveBehavior.UseDefaultEditingAdorners))
                this.AddDefaultEditingAdornerSetCreatorsForSelectedElement((IList<IAdornerSetCreator>) list, element);
              else
                this.AddEditingAdornerSetCreatorsForSelectedElement((IList<IAdornerSetCreator>) list, element);
            }
          }
        }
        else
        {
          Base3DElement base3Delement;
          if ((base3Delement = element as Base3DElement) != null && !(element is CameraElement))
          {
            if (element is LightElement && element.ViewModel.DefaultView.AdornerLayer.IsProxied(element))
              Tool.Add3DLightAdorners((IList<IAdornerSetCreator>) list, element, true);
            else if (lockedInsertionPoint != null && element == lockedInsertionPoint.SceneElement && this.ToolContext.ToolManager.ShowActiveContainer)
            {
              list.Add((IAdornerSetCreator) new SceneInsertionPointAdornerSetCreator3D());
            }
            else
            {
              Viewport3DElement viewport = base3Delement.Viewport;
              bool flag2;
              if (!this.hasCreatedViewport3DAdorners.TryGetValue(viewport, out flag2))
              {
                list.Add((IAdornerSetCreator) new HighlightAdornerSetCreator3D());
                this.hasCreatedViewport3DAdorners.Add(viewport, true);
              }
            }
            if (element == primarySelection)
            {
              if (this.UseDefaultEditingAdorners && (this.ActiveView.EventRouter.ActiveBehavior == null || this.ActiveView.EventRouter.ActiveBehavior.UseDefaultEditingAdorners))
                this.AddDefaultEditingAdornerSetCreatorsForSelectedElement((IList<IAdornerSetCreator>) list, element);
              else
                this.AddEditingAdornerSetCreatorsForSelectedElement((IList<IAdornerSetCreator>) list, element);
            }
          }
        }
      }
      return list;
    }

    public static void Add3DLightAdorners(IList<IAdornerSetCreator> adornerSetCreators, SceneElement element, bool onlyProxy)
    {
      if (element is AmbientLightElement)
        adornerSetCreators.Add((IAdornerSetCreator) new AmbientLightProxy3DAdornerSetCreator());
      else if (element is DirectionalLightElement)
      {
        adornerSetCreators.Add((IAdornerSetCreator) new DirectionalLightProxy3DAdornerSetCreator());
        adornerSetCreators.Add((IAdornerSetCreator) new DirectionalLightAdornerSetCreator());
      }
      else if (element is PointLightElement)
      {
        adornerSetCreators.Add((IAdornerSetCreator) new PointLightProxy3DAdornerSetCreator());
      }
      else
      {
        if (!(element is SpotLightElement))
          return;
        adornerSetCreators.Add((IAdornerSetCreator) new SpotLightProxy3DAdornerSetCreator());
        if (onlyProxy)
          return;
        adornerSetCreators.Add((IAdornerSetCreator) new SpotLightAdornerSetCreator3D());
        adornerSetCreators.Add((IAdornerSetCreator) new SpotLightConeAdornerSetCreator3D());
      }
    }

    private void AddDefaultEditingAdornerSetCreatorsForSelectedElements(IList<IAdornerSetCreator> adornerSetCreators, AdornerElementSet adornerElementSet)
    {
      if (!adornerElementSet.AdornsMultipleElements)
        return;
      bool flag = false;
      AnimationEditor animationEditor = this.ActiveSceneViewModel.AnimationEditor;
      foreach (SceneElement sceneElement in adornerElementSet.Elements)
      {
        if (!(sceneElement is BaseFrameworkElement))
          return;
        if (!flag && animationEditor.ActiveStoryboardTimeline != null && CanonicalTransform.IsCanonical((Transform) sceneElement.GetComputedValueAsWpf(Base2DElement.RenderTransformProperty)))
        {
          PropertyReference propertyReference = this.ActiveView.ConvertFromWpfPropertyReference(sceneElement.Platform.Metadata.CommonProperties.RenderTransformTranslationX);
          PropertyReference animationProperty = animationEditor.GetAnimationProperty((SceneNode) sceneElement, propertyReference);
          if (animationEditor.ActiveStoryboardTimeline.GetAnimation((SceneNode) sceneElement, animationProperty) is PathAnimationSceneNode)
            flag = true;
        }
      }
      SceneElement visualElementAncestor = adornerElementSet.Elements[0].VisualElementAncestor;
      foreach (SceneElement sceneElement in adornerElementSet.Elements)
      {
        if (sceneElement.VisualElementAncestor != visualElementAncestor)
          return;
      }
      if ((this.GetSelectionAdornerUsages((SceneElement) null) & SelectionAdornerUsages.UseFullAdorners) == SelectionAdornerUsages.None)
        return;
      if (this.ActiveSceneViewModel.UsingEffectDesigner)
      {
        if (!flag)
        {
          IAdornerSetCreator adornerSetCreator = (IAdornerSetCreator) new ScaleAdornerSetCreator();
          adornerSetCreators.Add(adornerSetCreator);
        }
      }
      else
      {
        if (!flag)
        {
          IAdornerSetCreator adornerSetCreator = (IAdornerSetCreator) new RotateAdornerSetCreator();
          adornerSetCreators.Add(adornerSetCreator);
        }
        IAdornerSetCreator adornerSetCreator1 = (IAdornerSetCreator) new SizeAdornerSetCreator();
        adornerSetCreators.Add(adornerSetCreator1);
      }
      IAdornerSetCreator adornerSetCreator2 = (IAdornerSetCreator) new CenterPointAdornerSetCreator();
      adornerSetCreators.Add(adornerSetCreator2);
    }

    private void AddDefaultEditingAdornerSetCreatorsForSelectedElement(IList<IAdornerSetCreator> adornerSetCreators, SceneElement element)
    {
      BaseFrameworkElement frameworkElement = element as BaseFrameworkElement;
      bool flag1 = false;
      if (frameworkElement != null && !this.ActiveSceneViewModel.DefaultView.IsRootElementSizeFixed)
      {
        if (element == this.ActiveSceneViewModel.RootNode)
        {
          if (this.ActiveSceneViewModel.ProjectContext.IsCapabilitySet(PlatformCapability.AllowRootElementResizing))
            flag1 = true;
        }
        else
        {
          FrameworkTemplateElement frameworkTemplateElement = this.ActiveSceneViewModel.ActiveEditingContainer as FrameworkTemplateElement;
          if (frameworkTemplateElement != null && element.Parent == frameworkTemplateElement && this.ActiveSceneViewModel.ViewRoot != this.ActiveSceneViewModel.RootNode)
          {
            if (frameworkTemplateElement == this.ActiveSceneViewModel.ViewRoot)
              flag1 = true;
            else if (frameworkTemplateElement.Parent != null)
            {
              SetterSceneNode setterSceneNode = this.ActiveSceneViewModel.GetSceneNode(frameworkTemplateElement.DocumentNodePath.GetParent().Node) as SetterSceneNode;
              if (setterSceneNode != null && (setterSceneNode.Property.Equals((object) ControlElement.TemplateProperty) || setterSceneNode.Property.Equals((object) PageElement.TemplateProperty)) && (frameworkTemplateElement.Parent == this.ActiveSceneViewModel.ViewRoot || frameworkTemplateElement.Parent is TriggerNode && frameworkTemplateElement.Parent.Parent == this.ActiveSceneViewModel.ViewRoot))
                flag1 = true;
            }
          }
        }
      }
      if (frameworkElement != null)
      {
        if (this.ActiveSceneViewModel.RootNode == element)
        {
          if (this.ActiveSceneViewModel.ProjectContext.IsCapabilitySet(PlatformCapability.AllowRootElementResizing))
            adornerSetCreators.Add((IAdornerSetCreator) new RootElementSizeAdornerSetCreator());
        }
        else if (!element.IsPlaceholder)
        {
          if (PlatformTypes.Rectangle.IsAssignableFrom((ITypeId) element.Type) && (this.GetSelectionAdornerUsages((SceneElement) null) & SelectionAdornerUsages.UseFullAdorners) != SelectionAdornerUsages.None)
            adornerSetCreators.Add((IAdornerSetCreator) new RoundedRectangleAdornerSetCreator());
          int index = 0;
          while (index < adornerSetCreators.Count && (!(adornerSetCreators[index] is HighlightAdornerSetCreator) && !(adornerSetCreators[index] is GridRowColumnAdornerSetCreator)))
            ++index;
          if ((this.GetSelectionAdornerUsages((SceneElement) null) & SelectionAdornerUsages.UseFullAdorners) != SelectionAdornerUsages.None)
          {
            adornerSetCreators.Insert(index, (IAdornerSetCreator) new RotateAdornerSetCreator());
            adornerSetCreators.Insert(index, (IAdornerSetCreator) new SkewAdornerSetCreator());
            if (this.toolContext.ToolManager.ShowAlignmentAdorners && this.ShowMarginAdornersForElement(element))
              adornerSetCreators.Add((IAdornerSetCreator) new ElementLayoutAdornerSetCreator());
            if (this.ActiveSceneViewModel.UsingEffectDesigner)
              adornerSetCreators.Add((IAdornerSetCreator) new ScaleAdornerSetCreator());
            else
              adornerSetCreators.Add((IAdornerSetCreator) new SizeAdornerSetCreator());
            adornerSetCreators.Add((IAdornerSetCreator) new CenterPointAdornerSetCreator());
          }
        }
      }
      else if (element is Base3DElement)
      {
        bool flag2 = this.ActiveSceneViewModel.ElementSelectionSet.Count == 1;
        if (flag2)
          adornerSetCreators.Add((IAdornerSetCreator) new TranslateAdornerSetCreator3D());
        if (element is AmbientLightElement)
          adornerSetCreators.Add((IAdornerSetCreator) new AmbientLightProxy3DAdornerSetCreator());
        else if (element is PointLightElement)
        {
          adornerSetCreators.Add((IAdornerSetCreator) new PointLightProxy3DAdornerSetCreator());
        }
        else
        {
          if (flag2)
            adornerSetCreators.Add((IAdornerSetCreator) new RotateAdornerSetCreator3D());
          if (element is DirectionalLightElement)
          {
            adornerSetCreators.Add((IAdornerSetCreator) new DirectionalLightProxy3DAdornerSetCreator());
            adornerSetCreators.Add((IAdornerSetCreator) new DirectionalLightAdornerSetCreator());
          }
          else if (element is SpotLightElement)
          {
            adornerSetCreators.Add((IAdornerSetCreator) new SpotLightProxy3DAdornerSetCreator());
            adornerSetCreators.Add((IAdornerSetCreator) new SpotLightAdornerSetCreator3D());
            adornerSetCreators.Add((IAdornerSetCreator) new SpotLightConeAdornerSetCreator3D());
          }
          else if (flag2)
            adornerSetCreators.Add((IAdornerSetCreator) new ScaleAdornerSetCreator3D());
        }
      }
      if (!flag1)
        return;
      adornerSetCreators.Add((IAdornerSetCreator) new DesignTimeSizeAdornerSetCreator());
    }

    private bool ShowDimmedGridRowColumnAdornersForElement(SceneElement element)
    {
      if (PlatformTypes.Grid.IsAssignableFrom((ITypeId) element.Type) && this.ActiveView.GetException(element.DocumentNodePath) == null)
      {
        SceneElement primarySelection = this.ActiveSceneViewModel.ElementSelectionSet.PrimarySelection;
        if (this.ActiveSceneViewModel.ElementSelectionSet.Count == 1 && PlatformTypes.Grid.IsAssignableFrom((ITypeId) primarySelection.Type) && primarySelection.Parent == element)
          return true;
      }
      return false;
    }

    private bool ShowGridRowColumnAdornersForElement(SceneElement element, bool isParentOfEntireSelection)
    {
      if (PlatformTypes.Grid.IsAssignableFrom((ITypeId) element.Type) && this.ActiveView.GetException(element.DocumentNodePath) == null)
      {
        SceneElementSelectionSet elementSelectionSet = this.ActiveSceneViewModel.ElementSelectionSet;
        if (elementSelectionSet.IsEmpty)
        {
          ISceneInsertionPoint sceneInsertionPoint = this.ActiveSceneViewModel.ActiveSceneInsertionPoint;
          if (sceneInsertionPoint != null && element == sceneInsertionPoint.SceneElement)
            return true;
          GridRowSelectionSet gridRowSelectionSet = this.ActiveSceneViewModel.DesignerContext.SelectionManager.GridRowSelectionSet;
          if (gridRowSelectionSet != null)
          {
            RowDefinitionNode primarySelection = gridRowSelectionSet.PrimarySelection;
            if (primarySelection != null && primarySelection.Parent == element)
              return true;
          }
          GridColumnSelectionSet columnSelectionSet = this.ActiveSceneViewModel.DesignerContext.SelectionManager.GridColumnSelectionSet;
          if (columnSelectionSet != null)
          {
            ColumnDefinitionNode primarySelection = columnSelectionSet.PrimarySelection;
            if (primarySelection != null && primarySelection.Parent == element)
              return true;
          }
        }
        else if (elementSelectionSet.Count == 1)
        {
          SceneElement primarySelection = elementSelectionSet.PrimarySelection;
          if (primarySelection == element || !PlatformTypes.Grid.IsAssignableFrom((ITypeId) primarySelection.Type) && primarySelection.Parent == element)
            return true;
        }
        else if (elementSelectionSet.Count > 1 && isParentOfEntireSelection)
          return true;
      }
      return false;
    }

    private bool ShowMarginAdornersForElement(SceneElement element)
    {
      BaseFrameworkElement child = element as BaseFrameworkElement;
      if (this.ActiveSceneViewModel.DesignerContext.SelectionManager.ElementSelectionSet.Count != 1 || child == null || element.Parent == null)
        return false;
      ILayoutDesigner designerForChild = this.ActiveSceneViewModel.GetLayoutDesignerForChild(element, true);
      return ((designerForChild.GetWidthConstraintMode(child) & LayoutConstraintMode.CanvasLike) == LayoutConstraintMode.NonOverlappingGridlike || (designerForChild.GetHeightConstraintMode(child) & LayoutConstraintMode.CanvasLike) == LayoutConstraintMode.NonOverlappingGridlike) && (!this.ActiveSceneViewModel.AnimationEditor.IsRecording || this.ActiveSceneViewModel.AnimationEditor.CanAnimateLayout);
    }

    private void AdornElements(IList<AdornerElementSet> elementSets)
    {
      SceneView activeView = this.ActiveView;
      if (activeView == null)
        return;
      PerformanceUtility.StartPerformanceSequence(PerformanceEvent.AdornElements);
      for (int index = this.adornerSetList.Count - 1; index >= 0; --index)
        activeView.AdornerLayer.Remove(this.adornerSetList[index]);
      foreach (IAdornerSet adornerSet in this.adornerSetList)
      {
        IDisposable disposable = adornerSet as IDisposable;
        if (disposable != null)
          disposable.Dispose();
      }
      this.adornerSetList.Clear();
      this.hasCreatedViewport3DAdorners.Clear();
      ToolBehaviorContext behaviorContext = new ToolBehaviorContext(this.ToolContext, this, activeView);
      foreach (AdornerElementSet adornerElementSet in (IEnumerable<AdornerElementSet>) elementSets)
      {
        if (!adornerElementSet.AdornsMultipleElements)
        {
          SceneElement primaryElement = adornerElementSet.PrimaryElement;
          Base3DElement base3Delement = primaryElement as Base3DElement;
          if (base3Delement == null || base3Delement.Viewport != null && base3Delement.Viewport.Camera != null)
            this.adornerSetList.AddRange((IEnumerable<IAdornerSet>) this.CreateAdornerSets(behaviorContext, primaryElement));
        }
        else
          this.adornerSetList.AddRange((IEnumerable<IAdornerSet>) this.CreateAdornerSets(behaviorContext, adornerElementSet));
      }
      this.adornerSetList.Sort((IComparer<IAdornerSet>) new AdornerSetComparer((IList<IAdornerSet>) this.adornerSetList));
      foreach (IAdornerSet adornerSet in this.adornerSetList)
        activeView.AdornerLayer.Add(adornerSet);
      activeView.AdornerLayer.UpdateAdorners();
      if (this.ActiveView.EventRouter.ActiveBehavior != null)
        this.ActiveView.EventRouter.ActiveBehavior.UpdateCursor();
      PerformanceUtility.EndPerformanceSequence(PerformanceEvent.AdornElements);
    }

    private List<IAdornerSet> CreateAdornerSets(ToolBehaviorContext behaviorContext, SceneElement element)
    {
      List<IAdornerSet> list = new List<IAdornerSet>();
      foreach (IAdornerSetCreator adornerSetCreator in (IEnumerable<IAdornerSetCreator>) this.GetAdornerSetCreatorsForElement(behaviorContext, element))
      {
        IAdornerSet adornerSet = adornerSetCreator.CreateAdornerSet(behaviorContext, element);
        if (adornerSet != null)
          list.Add(adornerSet);
      }
      return list;
    }

    private List<IAdornerSet> CreateAdornerSets(ToolBehaviorContext behaviorContext, AdornerElementSet adornerElementSet)
    {
      List<IAdornerSet> list = new List<IAdornerSet>();
      foreach (IAdornerSetCreator adornerSetCreator1 in (IEnumerable<IAdornerSetCreator>) this.GetAdornerSetCreatorsForElement(behaviorContext, adornerElementSet))
      {
        IMultipleElementAdornerSetCreator adornerSetCreator2 = adornerSetCreator1 as IMultipleElementAdornerSetCreator;
        if (adornerSetCreator2 != null)
        {
          IAdornerSet adornerSet = adornerSetCreator2.CreateAdornerSet(behaviorContext, adornerElementSet);
          if (adornerSet != null)
            list.Add(adornerSet);
        }
      }
      return list;
    }
  }
}
