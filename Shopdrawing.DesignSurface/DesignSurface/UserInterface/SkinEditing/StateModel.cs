// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.SkinEditing.StateModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.DocumentProcessors;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.SceneCommands;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Data;
using Microsoft.Expression.Framework.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.UserInterface.SkinEditing
{
  public class StateModel : StateModelBase
  {
    public static readonly DependencyProperty MonkeyTailBrushProperty = DependencyProperty.RegisterAttached("MonkeyTailBrush", typeof (Brush), typeof (StateModel), (PropertyMetadata) null);
    private ObservableCollection<TransitionModel> transitions = new ObservableCollection<TransitionModel>();
    private ObservableCollection<PotentialTransitionModel> potentialTransitions = new ObservableCollection<PotentialTransitionModel>();
    private VisualStateSceneNode sceneNode;
    private ICommand addTransitionCommand;
    private ICommand deleteCommand;
    private ContextMenu contextMenu;

    public override string Name
    {
      get
      {
        if (!string.IsNullOrEmpty(this.sceneNode.Name))
          return this.sceneNode.Name;
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, StringTable.ElementTimelineItemBracketedName, new object[1]
        {
          (object) this.sceneNode.Type.Name
        });
      }
      set
      {
        string name = this.sceneNode.Name;
        if (!(name != value) || string.IsNullOrEmpty(value))
          return;
        string validElementId = new SceneNodeIDHelper(this.ActiveSceneViewModel, this.ActiveSceneViewModel.ActiveEditingContainer).GetValidElementID((Microsoft.Expression.DesignSurface.ViewModel.SceneNode) this.sceneNode, value);
        using (SceneEditTransaction editTransaction = this.sceneNode.ViewModel.CreateEditTransaction(StringTable.RenameStateUndoUnit))
        {
          this.sceneNode.Name = validElementId;
          VisualStateGroupSceneNode stateGroup = this.sceneNode.StateGroup;
          if (stateGroup != null)
          {
            foreach (VisualStateTransitionSceneNode transitionSceneNode in (IEnumerable<VisualStateTransitionSceneNode>) stateGroup.Transitions)
            {
              if (transitionSceneNode.FromStateName == name)
                transitionSceneNode.FromStateName = validElementId;
              if (transitionSceneNode.ToStateName == name)
                transitionSceneNode.ToStateName = validElementId;
            }
          }
          StateNameRepairProcessor nameRepairProcessor = new StateNameRepairProcessor(this.sceneNode.DesignerContext, new StateNameChangeModel(name, validElementId, (Microsoft.Expression.DesignSurface.ViewModel.SceneNode) this.sceneNode));
          nameRepairProcessor.Begin();
          if (nameRepairProcessor.Cancelled)
            editTransaction.Cancel();
          else
            editTransaction.Commit();
        }
        this.NotifyPropertyChanged("Name");
      }
    }

    public string PinToolTip
    {
      get
      {
        if (!this.IsPinned)
          return StringTable.PinStateToolTip;
        return StringTable.UnpinStateToolTip;
      }
    }

    public bool IsRecording
    {
      get
      {
        SceneViewModel activeSceneViewModel = this.ActiveSceneViewModel;
        if (activeSceneViewModel != null)
          return activeSceneViewModel.AnimationEditor.IsRecording;
        return false;
      }
      set
      {
        SceneViewModel activeSceneViewModel = this.ActiveSceneViewModel;
        if (activeSceneViewModel == null)
          return;
        activeSceneViewModel.AnimationEditor.IsRecording = value;
      }
    }

    public StateGroupModel GroupModel
    {
      get
      {
        return this.ParentModel as StateGroupModel;
      }
    }

    public SceneViewModel ActiveSceneViewModel
    {
      get
      {
        return this.GroupModel.ActiveSceneViewModel;
      }
    }

    public override bool IsStructureEditable
    {
      get
      {
        if (this.GroupModel.IsStructureEditable)
          return this.IsUserDefined;
        return false;
      }
    }

    public bool IsEditingName { get; set; }

    public bool IsPinned
    {
      get
      {
        bool flag = false;
        if (this.GroupModel.PinnedState != null)
          flag = this.GroupModel.PinnedState == this;
        return flag;
      }
      set
      {
        this.GroupModel.TogglePinState(this);
      }
    }

    public ContextMenu ContextMenu
    {
      get
      {
        return this.contextMenu;
      }
    }

    public DelegateCommand ContextMenuOpeningCommand
    {
      get
      {
        return new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.UpdateContextMenu));
      }
    }

    public ICommand AddTransitionCommand
    {
      get
      {
        if (this.addTransitionCommand == null)
          this.addTransitionCommand = (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.BeginAddTransition));
        return this.addTransitionCommand;
      }
    }

    public ICommand DeleteCommand
    {
      get
      {
        if (this.deleteCommand == null)
          this.deleteCommand = (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.DeleteState));
        return this.deleteCommand;
      }
    }

    public ObservableCollection<PotentialTransitionModel> PotentialTransitions
    {
      get
      {
        return this.potentialTransitions;
      }
    }

    public bool HasPotentialTransitions
    {
      get
      {
        if (string.IsNullOrEmpty(this.SceneNode.Name))
          return false;
        int num = this.GroupModel.StateModels.Count + 1;
        foreach (StateModel stateModel in (Collection<StateModel>) this.GroupModel.States)
        {
          if (string.IsNullOrEmpty(stateModel.SceneNode.Name))
            --num;
        }
        return this.transitions.Count < num;
      }
    }

    public ObservableCollection<TransitionModel> Transitions
    {
      get
      {
        return this.transitions;
      }
    }

    public VisualStateSceneNode SceneNode
    {
      get
      {
        return this.sceneNode;
      }
    }

    public bool IsUserDefined
    {
      get
      {
        if (this.GroupModel != null && this.GroupModel.ModelManager != null)
          return !this.GroupModel.ModelManager.IsStateFromMetadata(this.Name, this.GroupModel.Name);
        return false;
      }
    }

    public VisualStateGroupSceneNode GroupNode
    {
      get
      {
        return this.sceneNode.StateGroup;
      }
    }

    protected override bool GetDirectChildrenOrSelfSelected
    {
      get
      {
        if (!this.IsSelected)
          return Enumerable.Any<TransitionModel>((IEnumerable<TransitionModel>) this.transitions, (Func<TransitionModel, bool>) (t => t.IsSelectedWithin));
        return true;
      }
    }

    public StateModel(VisualStateSceneNode node)
    {
      if (node == null)
        throw new ArgumentException("Cannot have a null node as an argument to StateModel constructor");
      this.sceneNode = node;
    }

    public static Brush GetMonkeyTailBrush(DependencyObject dependencyObject)
    {
      return (Brush) dependencyObject.GetValue(StateModel.MonkeyTailBrushProperty);
    }

    public static void SetMonkeyTailBrush(DependencyObject dependencyObject, Brush value)
    {
      dependencyObject.SetValue(StateModel.MonkeyTailBrushProperty, (object) value);
    }

    public void OnIsPinnedChanged()
    {
      this.NotifyPropertyChanged("IsPinned");
      this.NotifyPropertyChanged("PinToolTip");
    }

    public bool DoesTransitionMatch(TransitionModel model)
    {
      if (model.FromStateName != null)
        return model.FromStateName == this.Name;
      return this.Name == model.ToStateName;
    }

    public void AddTransitionModel(TransitionModel model)
    {
      model.ParentModel = (ModelBase) this;
      if (model.ToStateName == this.Name)
        this.transitions.Insert(0, model);
      else if (this.transitions.Count > 0 && string.IsNullOrEmpty(this.transitions[this.transitions.Count - 1].ToStateName))
        this.transitions.Insert(this.transitions.Count - 1, model);
      else
        this.transitions.Add(model);
      this.NotifyHasPotentialTransitionsChanged();
    }

    public void RemoveTransitionModel(TransitionModel model)
    {
      model.ParentModel = (ModelBase) null;
      this.transitions.Remove(model);
      this.NotifyHasPotentialTransitionsChanged();
    }

    protected override void Select()
    {
      this.GroupModel.SelectState(this);
    }

    protected override void Pin()
    {
      this.GroupModel.TogglePinState(this);
    }

    private void BeginAddTransition()
    {
      this.potentialTransitions.Clear();
      if (string.IsNullOrEmpty(this.SceneNode.Name))
        return;
      if (!this.HasTransition((string) null, this.Name))
        this.potentialTransitions.Add(new PotentialTransitionModel((StateModel) null, this));
      foreach (StateModel toModel in (Collection<StateModel>) this.GroupModel.States)
      {
        if (toModel != this && !string.IsNullOrEmpty(toModel.SceneNode.Name) && !this.HasTransition(this.Name, toModel.Name))
          this.potentialTransitions.Add(new PotentialTransitionModel(this, toModel));
      }
      if (this.HasTransition(this.Name, (string) null))
        return;
      this.potentialTransitions.Add(new PotentialTransitionModel(this, (StateModel) null));
    }

    private void DeleteState()
    {
      using (SceneEditTransaction editTransaction = this.sceneNode.ViewModel.CreateEditTransaction(StringTable.DeleteStateUndoUnit))
      {
        if (this.GroupModel.SelectedState == this)
          this.GroupModel.SelectState((StateModel) null);
        this.GroupModel.SceneNode.DeleteState(this.sceneNode);
        editTransaction.Commit();
      }
    }

    private bool HasTransition(string fromName, string toName)
    {
      foreach (TransitionModel transitionModel in (Collection<TransitionModel>) this.Transitions)
      {
        if (transitionModel.FromStateName == fromName && transitionModel.ToStateName == toName)
          return true;
      }
      return false;
    }

    internal void UpdateContextMenu()
    {
      this.contextMenu = new ContextMenu();
      CopyStatePropertiesCommand[] propertiesCommandArray = new CopyStatePropertiesCommand[2]
      {
        (CopyStatePropertiesCommand) new CopyAllStatePropertiesCommand(this.ActiveSceneViewModel, this.SceneNode),
        (CopyStatePropertiesCommand) new CopySelectedStatePropertiesCommand(this.ActiveSceneViewModel, this.SceneNode)
      };
      string[] strArray = new string[2]
      {
        "Edit_CopyAllStateProperties",
        "Edit_CopySelectedStateProperties"
      };
      List<MenuItem> list = new List<MenuItem>(2);
      for (int index = 0; index < 2; ++index)
      {
        CopyStatePropertiesCommand propertiesCommand = propertiesCommandArray[index];
        MenuItem menuItem = new MenuItem();
        if (propertiesCommand.IsAvailable)
        {
          menuItem.SetValue(AutomationElement.IdProperty, (object) strArray[index]);
          menuItem.IsEnabled = propertiesCommand.IsEnabled;
          menuItem.ItemsSource = propertiesCommand.Items;
          menuItem.Header = (object) propertiesCommand.CommandName;
          list.Add(menuItem);
        }
      }
      this.contextMenu.ItemsSource = (IEnumerable) list;
      this.NotifyPropertyChanged("ContextMenu");
    }

    internal void NotifyIsRecordingChanged()
    {
      this.NotifyPropertyChanged("IsRecording");
    }

    internal void NotifyIsStructureEditableChanged()
    {
      this.NotifyPropertyChanged("IsStructureEditable");
    }

    internal void NotifyHasPotentialTransitionsChanged()
    {
      this.NotifyPropertyChanged("HasPotentialTransitions");
    }

    internal void NotifySelectedStateChanged()
    {
      this.NotifyPropertyChanged("PinToolTip");
    }

    internal void OnContentChanged(IPropertyId propertyKey)
    {
      if (!this.sceneNode.NameProperty.Equals((object) propertyKey))
        return;
      this.NotifyPropertyChanged("Name");
      this.NotifyPropertyChanged("PinToolTip");
      this.GroupModel.OnStateNameChanged(this);
    }
  }
}
