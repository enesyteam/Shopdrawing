// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.SketchFlowPickerEditor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.DesignSurface.ViewModel.SceneNodeSearch;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  internal abstract class SketchFlowPickerEditor : Grid, INotifyPropertyChanged, IComponentConnector
  {
    private IProjectContext activeProjectContext;
    private DesignerContext designerContext;
    protected SceneNodeProperty editingProperty;
    protected SceneNodeProperty targetScreenProperty;
    protected SceneViewModel viewModel;
    private readonly List<SketchFlowPickerDisplayItem> items;
    internal SketchFlowPickerEditor SketchFlowPickerEditorControl;
    private bool _contentLoaded;

    public abstract string AutomationID { get; }

    protected abstract IPropertyId TargetScreenProperty { get; }

    public ICollectionView ItemsView { get; private set; }

    public SketchFlowPickerDisplayItem CurrentItem
    {
      get
      {
        if (this.EditingProperty == null || this.EditingProperty.PropertyValue == null || this.EditingProperty.PropertyValue.Value == null)
          return (SketchFlowPickerDisplayItem) null;
        string xamlName = this.EditingProperty.PropertyValue.Value as string;
        return Enumerable.FirstOrDefault<SketchFlowPickerDisplayItem>((IEnumerable<SketchFlowPickerDisplayItem>) this.items, (Func<SketchFlowPickerDisplayItem, bool>) (item => item.XamlName == xamlName));
      }
      set
      {
        if (this.EditingProperty == null || this.EditingProperty.PropertyValue == null)
          return;
        SketchFlowPickerDisplayItem pickerDisplayItem = value == null ? (SketchFlowPickerDisplayItem) null : Enumerable.FirstOrDefault<SketchFlowPickerDisplayItem>((IEnumerable<SketchFlowPickerDisplayItem>) this.items, (Func<SketchFlowPickerDisplayItem, bool>) (item => item.XamlName == value.XamlName));
        this.editingProperty.PropertyValue.Value = pickerDisplayItem == null ? (object) (string) null : (object) pickerDisplayItem.XamlName;
      }
    }

    private IProjectContext ActiveProjectContext
    {
      get
      {
        return this.activeProjectContext;
      }
      set
      {
        if (this.activeProjectContext != value)
          this.items.Clear();
        if (this.activeProjectContext != null)
          this.activeProjectContext.TypesChanged -= new EventHandler<TypesChangedEventArgs>(this.ProjectContext_TypesChanged);
        this.activeProjectContext = value;
        if (this.activeProjectContext == null)
          return;
        this.activeProjectContext.TypesChanged += new EventHandler<TypesChangedEventArgs>(this.ProjectContext_TypesChanged);
      }
    }

    private SceneNodeProperty EditingProperty
    {
      get
      {
        return this.editingProperty;
      }
      set
      {
        this.DetachEditingProperty();
        SceneNodeProperty editingProperty = value;
        if (editingProperty == null || editingProperty.SceneNodeObjectSet.ViewModel == null)
          return;
        this.AttachEditingProperty(editingProperty);
      }
    }

    protected ITypeResolver TypeResolver
    {
      get
      {
        return (ITypeResolver) this.viewModel.ProjectContext;
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public SketchFlowPickerEditor()
    {
      this.items = new List<SketchFlowPickerDisplayItem>();
      this.InitializeComponent();
      this.Loaded += new RoutedEventHandler(this.Self_Loaded);
      this.Unloaded += new RoutedEventHandler(this.Self_Unloaded);
      this.DataContextChanged += new DependencyPropertyChangedEventHandler(this.Self_DataContextChanged);
    }

    protected abstract void Subscribe(SceneNodeProperty editingProperty);

    protected abstract void Unsubscribe();

    protected abstract IEnumerable<SketchFlowPickerDisplayItem> FindItemsToDisplay(SceneNode container);

    protected virtual void AddGroupDescriptions()
    {
    }

    private void AttachEditingProperty(SceneNodeProperty editingProperty)
    {
      this.Subscribe(editingProperty);
    }

    private void DetachEditingProperty()
    {
      this.Unsubscribe();
    }

    private void Rebuild()
    {
      if (this.designerContext == null || this.designerContext.PrototypingService == null || (this.designerContext.PrototypingService.ActiveScreen == null || this.EditingProperty == null) || (this.EditingProperty.SceneNodeObjectSet == null || this.EditingProperty.SceneNodeObjectSet.Count <= 0 || (!(this.EditingProperty.SceneNodeObjectSet.Objects[0] is ISketchFlowBehaviorNode) || !this.EditingProperty.SceneNodeObjectSet.IsHomogenous)))
        return;
      ISketchFlowBehaviorNode action = (ISketchFlowBehaviorNode) this.EditingProperty.SceneNodeObjectSet.Objects[0];
      SceneNode container = action.ViewModel.ActiveEditingContainer;
      if (!string.IsNullOrEmpty(action.TargetScreen))
      {
        SceneNode sceneNode = Enumerable.FirstOrDefault<SceneNode>(new SearchPath(new SearchStep[1]
        {
          new SearchStep(SearchAxis.DocumentSelfAndDescendant, (ISearchPredicate) new DelegatePredicate((Predicate<SceneNode>) (node =>
          {
            if (node.Type == null || !(node.Type.FullName == action.TargetScreen))
              return node.TrueTargetType.FullName == action.TargetScreen;
            return true;
          }), SearchScope.NodeTreeSelf))
        }).Query(this.viewModel.ActiveEditingContainer));
        if (sceneNode != null)
          container = sceneNode;
      }
      IEnumerable<SketchFlowPickerDisplayItem> collection = container == null ? Enumerable.Empty<SketchFlowPickerDisplayItem>() : this.FindItemsToDisplay(container);
      this.items.Clear();
      this.items.AddRange(collection);
      this.ItemsView = CollectionViewSource.GetDefaultView((object) this.items);
      this.ClearGroupDescriptions();
      this.AddGroupDescriptions();
      this.NotifyPropertyChanged("CurrentItem");
      this.NotifyPropertyChanged("ItemsView");
    }

    private void ClearGroupDescriptions()
    {
      this.ItemsView.GroupDescriptions.Clear();
    }

    private void UpdateFromDataContext()
    {
      this.EditingProperty = (SceneNodeProperty) null;
      Microsoft.Windows.Design.PropertyEditing.PropertyValue propertyValue = this.DataContext as Microsoft.Windows.Design.PropertyEditing.PropertyValue;
      if (propertyValue == null)
        return;
      this.EditingProperty = propertyValue.ParentProperty as SceneNodeProperty;
      if (this.EditingProperty == null || this.EditingProperty.SceneNodeObjectSet == null)
      {
        this.EditingProperty = (SceneNodeProperty) null;
      }
      else
      {
        if (this.designerContext == null)
          this.designerContext = this.EditingProperty.SceneNodeObjectSet.DesignerContext;
        this.ActiveProjectContext = this.EditingProperty.SceneNodeObjectSet.ProjectContext;
        this.EditingProperty.Recache();
        this.Rebuild();
      }
    }

    private MethodInfo ResolveTypeIdMethod(ITypeId typeId, string methodName)
    {
      IType type = this.TypeResolver.ResolveType(typeId);
      if (type == null || this.TypeResolver.PlatformMetadata.IsNullType((ITypeId) type))
        return (MethodInfo) null;
      MethodInfo method = PlatformTypeHelper.GetMethod(type.RuntimeType, methodName);
      int num = method == (MethodInfo) null ? true : false;
      return method;
    }

    private ReferenceStep ResolveTypeIdProperty(ITypeId typeId, string propertyName)
    {
      IType type = this.TypeResolver.ResolveType(typeId);
      if (type == null || this.TypeResolver.PlatformMetadata.IsNullType((ITypeId) type))
        return (ReferenceStep) null;
      return PlatformTypeHelper.GetProperty(this.TypeResolver, (ITypeId) type, MemberType.Property, propertyName);
    }

    protected IList GetVisualStateGroups(object host)
    {
      MethodInfo methodInfo = this.ResolveTypeIdMethod(ProjectNeutralTypes.VisualStateManager, "GetVisualStateGroups");
      if (host == null || !(methodInfo != (MethodInfo) null))
        return (IList) new List<object>();
      return methodInfo.Invoke((object) null, new object[1]
      {
        host
      }) as IList;
    }

    protected string GetVisualStateGroupName(object group)
    {
      ReferenceStep referenceStep = this.ResolveTypeIdProperty(ProjectNeutralTypes.VisualStateGroup, "Name");
      if (group != null && referenceStep != null)
        return referenceStep.GetValue(group) as string;
      return (string) null;
    }

    protected IList GetVisualStateGroupStates(object group)
    {
      ReferenceStep referenceStep = this.ResolveTypeIdProperty(ProjectNeutralTypes.VisualStateGroup, "States");
      if (group != null && referenceStep != null)
        return referenceStep.GetValue(group) as IList;
      return (IList) new List<object>();
    }

    protected string GetVisualStateName(object state)
    {
      ReferenceStep referenceStep = this.ResolveTypeIdProperty(ProjectNeutralTypes.VisualState, "Name");
      if (state != null && referenceStep != null)
        return referenceStep.GetValue(state) as string;
      return (string) null;
    }

    private void Self_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      this.UpdateFromDataContext();
    }

    private void Self_Loaded(object sender, RoutedEventArgs e)
    {
      this.Rebuild();
    }

    private void Self_Unloaded(object sender, RoutedEventArgs e)
    {
      this.EditingProperty = (SceneNodeProperty) null;
      this.ActiveProjectContext = (IProjectContext) null;
    }

    private void ProjectContext_TypesChanged(object sender, TypesChangedEventArgs e)
    {
      this.items.Clear();
      this.Rebuild();
    }

    protected void EditingProperty_PropertyReferenceChanged(object sender, PropertyReferenceChangedEventArgs e)
    {
      this.Rebuild();
    }

    protected void TargetScreenProperty_PropertyReferenceChanged(object sender, PropertyReferenceChangedEventArgs e)
    {
      this.Rebuild();
    }

    protected void Subscription_PathNodeContentChanged(object sender, SceneNode pathNode, object content, DocumentNodeMarker damageMarker, DocumentNodeChange damage)
    {
      this.Rebuild();
    }

    protected void Subscription_PathNodesChanged(object sender, SceneNode basisNode, object basisContent, SceneNode pathNode, object content)
    {
      this.Rebuild();
    }

    private void NotifyPropertyChanged(string propertyName)
    {
      PropertyChangedEventHandler changedEventHandler = this.PropertyChanged;
      if (changedEventHandler == null)
        return;
      changedEventHandler((object) this, new PropertyChangedEventArgs(propertyName));
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/userinterface/propertyinspector/complexvalueeditors/sketchflowpickereditor.xaml", UriKind.Relative));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DebuggerNonUserCode]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      if (connectionId == 1)
        this.SketchFlowPickerEditorControl = (SketchFlowPickerEditor) target;
      else
        this._contentLoaded = true;
    }
  }
}
