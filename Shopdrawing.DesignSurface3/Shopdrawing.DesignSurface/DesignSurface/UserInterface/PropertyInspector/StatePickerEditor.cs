// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.StatePickerEditor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class StatePickerEditor : Grid, INotifyPropertyChanged, IComponentConnector
  {
    private List<StateInfo> states = new List<StateInfo>();
    private DesignerContext designerContext;
    private IProjectContext activeProjectContext;
    private SceneViewModel viewModel;
    private ICollectionView statesView;
    private SceneNodeProperty editingProperty;
    private SceneNodeProperty targetNameProperty;
    private SceneNodeProperty targetObjectProperty;
    private ReferenceStep stateGroupNameProperty;
    private ReferenceStep statesProperty;
    private ReferenceStep stateNameProperty;
    internal StatePickerEditor StatePickerEditorControl;
    private bool _contentLoaded;

    private SceneNodeProperty EditingProperty
    {
      get
      {
        return this.editingProperty;
      }
      set
      {
        if (this.viewModel != null)
        {
          this.viewModel.LateSceneUpdatePhase -= new SceneUpdatePhaseEventHandler(this.ViewModel_LateSceneUpdatePhase);
          this.viewModel = (SceneViewModel) null;
        }
        if (this.editingProperty != null)
        {
          this.editingProperty.PropertyReferenceChanged -= new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.stateNameProperty_PropertyReferenceChanged);
          if (this.targetNameProperty != null)
          {
            this.targetNameProperty.PropertyReferenceChanged -= new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.OnTargetPropertyPropertyReferenceChanged);
            this.targetNameProperty.OnRemoveFromCategory();
            this.targetNameProperty = (SceneNodeProperty) null;
          }
          if (this.targetObjectProperty != null)
          {
            this.targetObjectProperty.PropertyReferenceChanged -= new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.OnTargetPropertyPropertyReferenceChanged);
            this.targetObjectProperty.OnRemoveFromCategory();
            this.targetObjectProperty = (SceneNodeProperty) null;
          }
        }
        this.editingProperty = value;
        if (this.editingProperty == null)
          return;
        this.EditingProperty.PropertyReferenceChanged += new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.stateNameProperty_PropertyReferenceChanged);
        SceneNodeObjectSet sceneNodeObjectSet = this.EditingProperty.SceneNodeObjectSet;
        if (sceneNodeObjectSet.ProjectContext != null)
        {
          ReferenceStep singleStep1 = (ReferenceStep) sceneNodeObjectSet.ProjectContext.ResolveProperty(BehaviorTargetedTriggerActionNode.BehaviorTargetNameProperty);
          if (singleStep1 != null)
          {
            this.targetNameProperty = (SceneNodeProperty) sceneNodeObjectSet.CreateProperty(new PropertyReference(singleStep1), (AttributeCollection) null);
            this.targetNameProperty.PropertyReferenceChanged += new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.OnTargetPropertyPropertyReferenceChanged);
          }
          ReferenceStep singleStep2 = (ReferenceStep) sceneNodeObjectSet.ProjectContext.ResolveProperty(BehaviorTargetedTriggerActionNode.BehaviorTargetObjectProperty);
          if (singleStep2 != null)
          {
            this.targetObjectProperty = (SceneNodeProperty) sceneNodeObjectSet.CreateProperty(new PropertyReference(singleStep2), (AttributeCollection) null);
            this.targetObjectProperty.PropertyReferenceChanged += new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.OnTargetPropertyPropertyReferenceChanged);
          }
          if (this.editingProperty.SceneNodeObjectSet.ViewModel == null)
            return;
          this.viewModel = this.editingProperty.SceneNodeObjectSet.ViewModel;
          this.viewModel.LateSceneUpdatePhase += new SceneUpdatePhaseEventHandler(this.ViewModel_LateSceneUpdatePhase);
        }
        else
          this.editingProperty = (SceneNodeProperty) null;
      }
    }

    public ICollectionView States
    {
      get
      {
        return this.statesView;
      }
    }

    public StateInfo CurrentState
    {
      get
      {
        StateInfo stateInfo = (StateInfo) null;
        if (this.EditingProperty != null && this.EditingProperty.PropertyValue != null && this.EditingProperty.PropertyValue.Value != null)
        {
          string stateNameToFind = this.EditingProperty.PropertyValue.Value as string;
          stateInfo = this.states.Find((Predicate<StateInfo>) (item => item.StateName.Equals(stateNameToFind, StringComparison.Ordinal)));
        }
        return stateInfo;
      }
      set
      {
        if (this.EditingProperty.PropertyValue == null)
          return;
        this.EditingProperty.PropertyValue.Value = (object) value.StateName;
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
          this.states.Clear();
        if (this.activeProjectContext != null)
          this.activeProjectContext.TypesChanged -= new EventHandler<TypesChangedEventArgs>(this.ProjectContext_TypesChanged);
        this.activeProjectContext = value;
        if (this.activeProjectContext == null)
          return;
        this.activeProjectContext.TypesChanged += new EventHandler<TypesChangedEventArgs>(this.ProjectContext_TypesChanged);
        this.stateGroupNameProperty = this.activeProjectContext.ResolveProperty(VisualStateGroupSceneNode.VisualStateGroupNameProperty) as ReferenceStep;
        this.statesProperty = this.activeProjectContext.ResolveProperty(VisualStateGroupSceneNode.StatesProperty) as ReferenceStep;
        this.stateNameProperty = this.activeProjectContext.ResolveProperty(VisualStateSceneNode.VisualStateNameProperty) as ReferenceStep;
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public StatePickerEditor()
    {
      this.InitializeComponent();
      this.DataContextChanged += new DependencyPropertyChangedEventHandler(this.StatePickerEditor_DataContextChanged);
      this.Loaded += new RoutedEventHandler(this.StatePickerEditor_Loaded);
      this.Unloaded += new RoutedEventHandler(this.StatePickerEditor_Unloaded);
    }

    private void ViewModel_LateSceneUpdatePhase(object sender, SceneUpdatePhaseEventArgs args)
    {
      foreach (DocumentNodeChange documentNodeChange in args.DocumentChanges.CollapsedChangeList)
      {
        if (documentNodeChange.ParentNode != null && (documentNodeChange.ParentNode.SceneNode is VisualStateGroupSceneNode || documentNodeChange.ParentNode.SceneNode is VisualStateSceneNode))
        {
          this.Rebuild();
          break;
        }
      }
    }

    private void StatePickerEditor_Unloaded(object sender, RoutedEventArgs e)
    {
      this.EditingProperty = (SceneNodeProperty) null;
      this.ActiveProjectContext = (IProjectContext) null;
    }

    private void StatePickerEditor_Loaded(object sender, RoutedEventArgs e)
    {
      this.UpdateFromDataContext();
    }

    private void UpdateFromDataContext()
    {
      this.EditingProperty = (SceneNodeProperty) null;
      Microsoft.Windows.Design.PropertyEditing.PropertyValue propertyValue = this.DataContext as Microsoft.Windows.Design.PropertyEditing.PropertyValue;
      if (propertyValue == null)
        return;
      this.EditingProperty = propertyValue.ParentProperty as SceneNodeProperty;
      if (this.EditingProperty == null || this.EditingProperty.SceneNodeObjectSet.Document == null)
      {
        this.EditingProperty = (SceneNodeProperty) null;
      }
      else
      {
        if (this.designerContext == null)
          this.designerContext = this.EditingProperty.SceneNodeObjectSet.DesignerContext;
        this.ActiveProjectContext = this.EditingProperty.SceneNodeObjectSet.ProjectContext;
        this.Rebuild();
      }
    }

    private void OnTargetPropertyPropertyReferenceChanged(object sender, PropertyReferenceChangedEventArgs e)
    {
      this.Rebuild();
    }

    private void ProjectContext_TypesChanged(object sender, TypesChangedEventArgs e)
    {
      this.states.Clear();
      this.UpdateFromDataContext();
    }

    private void stateNameProperty_PropertyReferenceChanged(object sender, PropertyReferenceChangedEventArgs e)
    {
      this.UpdateFromDataContext();
    }

    private void StatePickerEditor_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      this.UpdateFromDataContext();
    }

    private void Rebuild()
    {
      if (this.EditingProperty == null)
        return;
      this.EditingProperty.Recache();
      this.states.Clear();
      if (this.EditingProperty.ObjectSet.IsHomogenous && this.EditingProperty.ObjectSet.Count > 0)
      {
        BehaviorTargetedTriggerActionNode triggerActionNode = this.EditingProperty.SceneNodeObjectSet.Objects[0] as BehaviorTargetedTriggerActionNode;
        if (triggerActionNode == null || triggerActionNode.TargetObject == null && string.IsNullOrEmpty(triggerActionNode.TargetName))
        {
          SceneNode editingContainer = this.viewModel.ActiveEditingContainer;
          SceneNode hostNode = VisualStateManagerSceneNode.GetHostNode(this.viewModel.ActiveEditingContainer);
          if (hostNode != null)
          {
            foreach (VisualStateGroupSceneNode stateGroupSceneNode in (IEnumerable<VisualStateGroupSceneNode>) VisualStateManagerSceneNode.GetStateGroups(hostNode))
            {
              foreach (SceneNode sceneNode in (IEnumerable<VisualStateSceneNode>) stateGroupSceneNode.States)
                this.states.Add(new StateInfo(sceneNode.Name, stateGroupSceneNode.Name));
            }
          }
          ControlTemplateElement controlTemplateElement = this.viewModel.ActiveEditingContainer as ControlTemplateElement;
          if (controlTemplateElement != null)
          {
            IType type = this.viewModel.ProjectContext.ResolveType(controlTemplateElement.ControlTemplateTargetTypeId);
            foreach (DefaultStateRecord defaultStateRecord in ProjectAttributeHelper.GetDefaultStateRecords(type, (ITypeResolver) (this.viewModel.ProjectContext as ProjectContext)))
              this.AddStateInfoIfNeeded(defaultStateRecord.StateName, defaultStateRecord.GroupName, type);
          }
        }
        else if (triggerActionNode.TargetNode != null && triggerActionNode.TargetNode.IsViewObjectValid)
        {
          IViewControl viewControl = triggerActionNode.TargetNode.ViewObject as IViewControl;
          if (viewControl != null)
          {
            object stateManagerHost = viewControl.VisualStateManagerHost;
            if (stateManagerHost != null)
            {
              foreach (object group in (IEnumerable) StatePickerEditor.GetVisualStateGroups(stateManagerHost, (ITypeResolver) triggerActionNode.ProjectContext))
              {
                string stateGroupName = this.GetStateGroupName(group);
                foreach (object state in (IEnumerable) this.GetStates(group))
                  this.states.Add(new StateInfo(this.GetStateName(state), stateGroupName));
              }
            }
            IType type = triggerActionNode.TargetNode.ProjectContext.GetType(triggerActionNode.TargetNode.TargetType);
            foreach (DefaultStateRecord defaultStateRecord in ProjectAttributeHelper.GetDefaultStateRecords(triggerActionNode.TargetNode.Type, (ITypeResolver) (this.viewModel.ProjectContext as ProjectContext)))
              this.AddStateInfoIfNeeded(defaultStateRecord.StateName, defaultStateRecord.GroupName, type);
          }
        }
      }
      this.states.RemoveAll((Predicate<StateInfo>) (stateInfo =>
      {
        if (!string.IsNullOrEmpty(stateInfo.GroupName) && !string.IsNullOrEmpty(stateInfo.StateName) && !stateInfo.GroupName.StartsWith(VisualStateManagerSceneNode.SketchFlowAnimationXamlDelimiter, StringComparison.Ordinal))
          return stateInfo.StateName.StartsWith("_BlendEditTimeState-", StringComparison.Ordinal);
        return true;
      }));
      this.statesView = CollectionViewSource.GetDefaultView((object) this.states);
      PropertyGroupDescription groupDescription = new PropertyGroupDescription();
      groupDescription.PropertyName = "GroupName";
      this.statesView.GroupDescriptions.Clear();
      this.statesView.GroupDescriptions.Add((GroupDescription) groupDescription);
      this.OnPropertyChanged("CurrentState");
      this.OnPropertyChanged("States");
    }

    private void AddStateInfoIfNeeded(string stateName, string groupName, IType targetNodeType)
    {
      if (this.states.Find((Predicate<StateInfo>) (stateInfo =>
      {
        if (stateInfo.GroupName == groupName)
          return stateInfo.StateName == stateName;
        return false;
      })) != null || targetNodeType == null || PlatformTypes.UserControl.IsAssignableFrom((ITypeId) targetNodeType) && groupName.Equals("ValidationStates"))
        return;
      this.states.Add(new StateInfo(stateName, groupName));
    }

    private string GetStateGroupName(object group)
    {
      if (this.stateGroupNameProperty != null)
        return this.stateGroupNameProperty.GetValue(group) as string;
      return (string) null;
    }

    private IList GetStates(object group)
    {
      if (this.statesProperty != null)
        return this.statesProperty.GetValue(group) as IList;
      return (IList) null;
    }

    private string GetStateName(object state)
    {
      if (this.stateNameProperty != null)
        return this.stateNameProperty.GetValue(state) as string;
      return (string) null;
    }

    private static IList GetVisualStateGroups(object hostElement, ITypeResolver typeResolver)
    {
      IType type1 = typeResolver.ResolveType(ProjectNeutralTypes.VisualStateManager);
      typeResolver.ResolveType(ProjectNeutralTypes.VisualStateGroup);
      IType type2 = typeResolver.GetType(hostElement.GetType());
      if (!PlatformTypes.FrameworkElement.IsAssignableFrom((ITypeId) type2))
        return (IList) new List<object>();
      return PlatformTypeHelper.GetMethod(type1.RuntimeType, "GetVisualStateGroups").Invoke((object) null, new object[1]
      {
        hostElement
      }) as IList;
    }

    private void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/userinterface/propertyinspector/complexvalueeditors/statepickereditor.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      if (connectionId == 1)
        this.StatePickerEditorControl = (StatePickerEditor) target;
      else
        this._contentLoaded = true;
    }
  }
}
