// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.SkinEditing.TransitionModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Data;
using Microsoft.Expression.Project;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.UserInterface.SkinEditing
{
  public class TransitionModel : ModelBase, ITransitionEffectSite, INotifyPropertyChanged
  {
    private VisualStateTransitionSceneNode sceneNode;
    private StateGroupModel groupModel;
    private bool isSelected;
    private double editDuration;
    private SceneNodeProperty easingFunctionProperty;
    private bool? generatedDurationApplies;
    private DelegateCommand deleteCommand;
    private DelegateCommand selectCommand;
    private DelegateCommand commitEditDurationCommand;
    private DelegateCommand beginEditDurationCommand;
    private EasingEditorPopup popup;

    public VisualStateTransitionSceneNode TransitionSceneNode
    {
      get
      {
        return this.sceneNode;
      }
      set
      {
        this.Duration = 0.0;
        this.NotifyPropertyChanged("GeneratedEasingFunction");
        this.NotifyPropertyChanged("Duration");
        this.NotifyPropertyChanged("TransitionEffectName");
        this.sceneNode = value;
      }
    }

    public VisualStateGroupSceneNode GroupNode
    {
      get
      {
        return this.groupModel.SceneNode;
      }
    }

    public StateGroupModel GroupModel
    {
      get
      {
        return this.groupModel;
      }
      set
      {
        this.groupModel = value;
      }
    }

    public bool IsSelected
    {
      get
      {
        return this.isSelected;
      }
      set
      {
        if (this.isSelected == value)
          return;
        this.isSelected = value;
        this.NotifyPropertyChanged("IsSelected");
        this.UpdateSelectedWithin();
      }
    }

    public StateModel StateModel
    {
      get
      {
        return this.ParentModel as StateModel;
      }
    }

    public string FromStateName
    {
      get
      {
        if (this.sceneNode == null)
          return "";
        return this.sceneNode.FromStateName;
      }
    }

    public string ToStateName
    {
      get
      {
        if (this.sceneNode == null)
          return "";
        return this.sceneNode.ToStateName;
      }
    }

    public bool IsDefaultTransition
    {
      get
      {
        if (string.IsNullOrEmpty(this.FromStateName))
          return string.IsNullOrEmpty(this.ToStateName);
        return false;
      }
    }

    public double Duration
    {
      get
      {
        return this.editDuration;
      }
      set
      {
        if (value == this.editDuration)
          return;
        this.editDuration = value;
        this.NotifyPropertyChanged("Duration");
      }
    }

    private double InternalDuration
    {
      get
      {
        double generatedDuration = this.GeneratedDuration;
        if (this.sceneNode == null)
          return 0.0;
        double d;
        if (this.sceneNode.Storyboard == null)
        {
          d = generatedDuration;
        }
        else
        {
          d = this.sceneNode.Storyboard.NaturalDuration;
          if (this.GeneratedDurationApplies && d < generatedDuration)
            d = generatedDuration;
        }
        return JoltHelper.RoundDouble(this.sceneNode.ProjectContext, d);
      }
      set
      {
        if (!this.ModelManager.IsEditTransactionOpen)
          return;
        if (this.sceneNode.Storyboard == null || !this.sceneNode.Storyboard.ShouldSerialize)
        {
          this.sceneNode.GeneratedDuration = new System.Windows.Duration(TimeSpan.FromSeconds(value));
          this.UpdateStoryboardIfNeeded();
        }
        else if (value != 0.0)
        {
          this.sceneNode.Storyboard.SetTimeRegion(0.0, this.sceneNode.Storyboard.PlayDuration, 0.0, value);
          if (this.GeneratedDuration > value)
          {
            this.GeneratedDuration = value;
            this.UpdateStoryboardIfNeeded();
          }
        }
        else
        {
          this.GeneratedDuration = 0.0;
          this.TransitionSceneNode.ClearValue(VisualStateTransitionSceneNode.StoryboardProperty);
          if (this.sceneNode.ViewModel.TransitionEditTarget == this.sceneNode)
            this.sceneNode.ViewModel.ActivateTransition(this.sceneNode);
        }
        this.NotifyPropertyChanged("Duration");
      }
    }

    public SceneViewModel ActiveSceneViewModel
    {
      get
      {
        return this.GroupModel.ActiveSceneViewModel;
      }
    }

    private StateModelManager ModelManager
    {
      get
      {
        return this.GroupModel.ModelManager;
      }
    }

    public ICommand BeginEditDurationCommand
    {
      get
      {
        if (this.beginEditDurationCommand == null)
          this.beginEditDurationCommand = new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.BeginEditDuration));
        return (ICommand) this.beginEditDurationCommand;
      }
    }

    public ICommand CommitEditDurationCommand
    {
      get
      {
        if (this.commitEditDurationCommand == null)
          this.commitEditDurationCommand = new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.CommitEditDuration));
        return (ICommand) this.commitEditDurationCommand;
      }
    }

    public ICommand CancelEditDurationCommand
    {
      get
      {
        return this.ModelManager.CancelOpenEditTransactionCommand;
      }
    }

    public ICommand ShowEasingFunctionEditorCommand
    {
      get
      {
        return (ICommand) new ArgumentDelegateCommand((ArgumentDelegateCommand.ArgumentEventHandler) (obj => this.ClickEasingFunctionPreviewIcon(obj as UIElement)));
      }
    }

    public object TransitionEffect
    {
      get
      {
        if (this.sceneNode != null)
        {
          IProperty property = this.sceneNode.ProjectContext.ResolveProperty(VisualStateManagerSceneNode.TransitionEffectProperty);
          if (property != null)
            return this.sceneNode.GetComputedValue((IPropertyId) property);
        }
        return (object) null;
      }
    }

    public ICommand DeleteCommand
    {
      get
      {
        if (this.deleteCommand == null)
          this.deleteCommand = new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.Delete));
        return (ICommand) this.deleteCommand;
      }
    }

    public ICommand SelectCommand
    {
      get
      {
        if (this.selectCommand == null)
          this.selectCommand = new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.Select));
        return (ICommand) this.selectCommand;
      }
    }

    private bool GeneratedDurationApplies
    {
      get
      {
        if (!this.generatedDurationApplies.HasValue)
        {
          if (this.TransitionSceneNode.FromStateName == null || this.TransitionSceneNode.ToStateName == null || (this.sceneNode.Storyboard == null || !this.sceneNode.Storyboard.ShouldSerialize))
          {
            this.generatedDurationApplies = new bool?(true);
          }
          else
          {
            Dictionary<TimelineSceneNode.PropertyNodePair, TransitionAnimationData> transitionTable = new Dictionary<TimelineSceneNode.PropertyNodePair, TransitionAnimationData>();
            this.sceneNode.UpdateTransitionStoryboard(false, transitionTable);
            this.generatedDurationApplies = new bool?(false);
            foreach (TransitionAnimationData transitionAnimationData in transitionTable.Values)
            {
              if (transitionAnimationData.TransitionAnimation == null || !transitionAnimationData.TransitionAnimation.ShouldSerialize)
              {
                this.generatedDurationApplies = new bool?(true);
                break;
              }
            }
          }
        }
        return this.generatedDurationApplies.Value;
      }
    }

    private double GeneratedDuration
    {
      get
      {
        if (this.sceneNode == null)
          return 0.0;
        System.Windows.Duration generatedDuration = this.sceneNode.GeneratedDuration;
        if (generatedDuration.HasTimeSpan)
          return generatedDuration.TimeSpan.TotalSeconds;
        return 0.0;
      }
      set
      {
        this.sceneNode.GeneratedDuration = new System.Windows.Duration(TimeSpan.FromSeconds(value));
      }
    }

    public bool IsGeneratedEasingFunctionAvailable
    {
      get
      {
        if (this.ActiveSceneViewModel != null)
          return this.ActiveSceneViewModel.ProjectContext.ResolveProperty(VisualStateTransitionSceneNode.GeneratedEasingFunctionProperty) != null;
        return false;
      }
    }

    public bool IsTransitionEffectAvailable
    {
      get
      {
        if (this.ActiveSceneViewModel != null && this.ActiveSceneViewModel.ProjectContext.IsCapabilitySet(PlatformCapability.SupportsTransitionEffect))
          return BlendSdkHelper.IsSdkInstalled(this.ActiveSceneViewModel.ProjectContext.TargetFramework);
        return false;
      }
    }

    public IEasingFunctionDefinition GeneratedEasingFunction
    {
      get
      {
        if (this.sceneNode != null && this.ActiveSceneViewModel != null)
          return this.ActiveSceneViewModel.ProjectContext.Platform.ViewObjectFactory.Instantiate(this.sceneNode.GetComputedValue(VisualStateTransitionSceneNode.GeneratedEasingFunctionProperty)) as IEasingFunctionDefinition;
        return (IEasingFunctionDefinition) null;
      }
    }

    protected override bool GetDirectChildrenOrSelfSelected
    {
      get
      {
        return this.IsSelected;
      }
    }

    public string AutomationId
    {
      get
      {
        return "Transition:" + this.ToStateName;
      }
    }

    public TransitionModel(VisualStateTransitionSceneNode sceneNode, StateGroupModel groupModel)
    {
      this.sceneNode = sceneNode;
      this.groupModel = groupModel;
      this.UpdateDuration();
    }

    public void OnContentChanged(IPropertyId propertyKey)
    {
      if (VisualStateTransitionSceneNode.FromStateNameProperty.Equals((object) propertyKey))
      {
        this.NotifyPropertyChanged("FromStateName");
        this.NotifyPropertyChanged("IsDefaultTransition");
      }
      else if (VisualStateTransitionSceneNode.ToStateNameProperty.Equals((object) propertyKey))
      {
        this.NotifyPropertyChanged("ToStateName");
        this.NotifyPropertyChanged("IsDefaultTransition");
        this.NotifyPropertyChanged("AutomationId");
      }
      else if (VisualStateTransitionSceneNode.GeneratedEasingFunctionProperty.Equals((object) propertyKey))
      {
        this.NotifyPropertyChanged("GeneratedEasingFunction");
      }
      else
      {
        if (!VisualStateManagerSceneNode.TransitionEffectProperty.Equals((object) propertyKey))
          return;
        this.NotifyPropertyChanged("TransitionEffect");
      }
    }

    public void UpdateDuration()
    {
      this.generatedDurationApplies = new bool?();
      this.Duration = this.InternalDuration;
    }

    private void BeginEditDuration()
    {
      this.EnsureTransitionSceneNode();
      this.ModelManager.BeginEditTransitionDurationCommand.Execute((object) null);
    }

    private void Delete()
    {
      using (SceneEditTransaction editTransaction = this.sceneNode.ViewModel.CreateEditTransaction(StringTable.DeleteTransitionUndoUnit))
      {
        this.sceneNode.StateGroup.DeleteTransition(this.sceneNode);
        editTransaction.Commit();
      }
    }

    private void ClickEasingFunctionPreviewIcon(UIElement placementTarget)
    {
      this.EnsureTransitionSceneNode();
      if (this.ActiveSceneViewModel == null)
        return;
      this.ActiveSceneViewModel.TransitionSelectionSet.SetSelection(this.sceneNode);
      VisualStateTransitionEditor transitionEditor = new VisualStateTransitionEditor(this.sceneNode);
      PropertyReference propertyReference = new PropertyReference((ReferenceStep) this.sceneNode.ProjectContext.ResolveProperty(VisualStateTransitionSceneNode.GeneratedEasingFunctionProperty));
      IType type = this.sceneNode.ProjectContext.ResolveType(ProjectNeutralTypes.IEasingFunction);
      if (!this.sceneNode.ProjectContext.PlatformMetadata.IsNullType((ITypeId) type))
      {
        this.easingFunctionProperty = transitionEditor.VisualTransitionObjectSet.CreateSceneNodeProperty(propertyReference, TypeUtilities.GetAttributes(type.RuntimeType));
        this.easingFunctionProperty.PropertyReferenceChanged += new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.easingFunctionProperty_PropertyReferenceChanged);
      }
      this.popup = new EasingEditorPopup(this.easingFunctionProperty, this.TransitionSceneNode.ViewModel.DefaultView);
      this.TransitionSceneNode.ViewModel.Closing += new EventHandler(this.ViewModel_Closing);
      this.popup.SynchronousClosed += (EventHandler) ((sender, e) =>
      {
        if (this.TransitionSceneNode.IsAttached && this.TransitionSceneNode.ViewModel != null)
          this.TransitionSceneNode.ViewModel.TransitionSelectionSet.Clear();
        if (this.easingFunctionProperty != null)
        {
          this.easingFunctionProperty.OnRemoveFromCategory();
          this.easingFunctionProperty.PropertyReferenceChanged -= new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.easingFunctionProperty_PropertyReferenceChanged);
          this.easingFunctionProperty = (SceneNodeProperty) null;
        }
        this.TransitionSceneNode.ViewModel.Closing -= new EventHandler(this.ViewModel_Closing);
        this.popup = (EasingEditorPopup) null;
      });
      this.popup.Placement = PlacementMode.Bottom;
      this.popup.PlacementTarget = placementTarget;
      this.popup.IsOpen = true;
    }

    private void ViewModel_Closing(object sender, EventArgs e)
    {
      if (this.popup == null)
        return;
      this.popup.IsOpen = false;
      this.popup = (EasingEditorPopup) null;
    }

    private void easingFunctionProperty_PropertyReferenceChanged(object sender, PropertyReferenceChangedEventArgs e)
    {
      this.NotifyPropertyChanged("GeneratedEasingFunction");
    }

    public void EnsureTransitionSceneNode()
    {
      if (this.sceneNode != null || this.ActiveSceneViewModel == null)
        return;
      using (SceneEditTransaction editTransaction = this.ActiveSceneViewModel.CreateEditTransaction(StringTable.AddDefaultTransitionUndoUnit, true))
      {
        this.TransitionSceneNode = VisualStateTransitionSceneNode.Factory.Instantiate(this.ActiveSceneViewModel);
        this.TransitionSceneNode.GeneratedDuration = new System.Windows.Duration(TimeSpan.Zero);
        this.TransitionSceneNode.ShouldSerialize = false;
        this.GroupNode.Transitions.Add(this.sceneNode);
        editTransaction.Commit();
      }
    }

    private void Select()
    {
      this.EnsureTransitionSceneNode();
      if (this.GroupModel.UseFluidLayout)
        return;
      this.GroupModel.SelectTransition(this);
    }

    private void CommitEditDuration()
    {
      this.InternalDuration = this.editDuration;
      this.ModelManager.CommitOpenEditTransactionCommand.Execute((object) null);
    }

    private void UpdateStoryboardIfNeeded()
    {
      if (this.sceneNode != this.sceneNode.ViewModel.TransitionEditTarget)
        return;
      this.sceneNode.UpdateTransitionStoryboard(true, (Dictionary<TimelineSceneNode.PropertyNodePair, TransitionAnimationData>) null);
    }
  }
}
