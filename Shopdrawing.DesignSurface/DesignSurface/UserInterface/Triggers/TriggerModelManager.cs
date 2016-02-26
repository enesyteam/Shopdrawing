// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Triggers.TriggerModelManager
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Selection;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.DesignSurface.ViewModel.SceneNodeSearch;
using Microsoft.Expression.Framework.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.UserInterface.Triggers
{
  public class TriggerModelManager : INotifyPropertyChanged
  {
    private ObservableCollection<TriggerModel3> triggerModels = new ObservableCollection<TriggerModel3>();
    private List<SubSubscription> delayedUpdates = new List<SubSubscription>();
    private List<Base3DElement> locked3DElements = new List<Base3DElement>();
    private DesignerContext designerContext;
    private DelegateCommand addTriggerCommand;
    private DelegateCommand addPropertyTriggerCommand;
    private DelegateCommand deleteTriggerCommand;
    private TriggerModel3 selectedItem;
    private NoneTriggerModel noneTrigger;
    private SceneNodeSubscription<object, TriggerModel3> triggerSubscription;
    private TriggerBaseNode triggerToBeSelected;
    private int selectionChangesSinceLock;

    private SceneViewModel ActiveSceneViewModel
    {
      get
      {
        SceneViewModel activeSceneViewModel = this.designerContext.ActiveSceneViewModel;
        if (activeSceneViewModel == null || !activeSceneViewModel.IsEditable || (!this.designerContext.ActiveView.IsDesignSurfaceEnabled || !JoltHelper.TriggersSupported(activeSceneViewModel.ProjectContext)))
          return (SceneViewModel) null;
        return activeSceneViewModel;
      }
    }

    public IList<TriggerModel3> Triggers
    {
      get
      {
        return (IList<TriggerModel3>) this.triggerModels;
      }
    }

    public ICommand AddTriggerCommand
    {
      get
      {
        return (ICommand) this.addTriggerCommand;
      }
    }

    public ICommand AddPropertyTriggerCommand
    {
      get
      {
        return (ICommand) this.addPropertyTriggerCommand;
      }
    }

    public ICommand DeleteTriggerCommand
    {
      get
      {
        return (ICommand) this.deleteTriggerCommand;
      }
    }

    public bool ShouldDisplayNoneTrigger
    {
      get
      {
        if (!(this.CurrentContainer is StyleNode))
          return this.CurrentContainer is FrameworkTemplateElement;
        return true;
      }
    }

    public TriggerModel3 SelectedItem
    {
      get
      {
        return this.selectedItem;
      }
      set
      {
        if (this.selectedItem == value)
          return;
        this.selectedItem = value;
        this.UpdateDeleteTrigger();
        if (this.selectedItem != null)
          this.selectedItem.Activate();
        this.OnPropertyChanged("SelectedItem");
      }
    }

    public TriggerBaseNode TriggerToBeSelected
    {
      get
      {
        return this.triggerToBeSelected;
      }
      set
      {
        this.triggerToBeSelected = value;
      }
    }

    private ITriggerContainer CurrentContainer
    {
      get
      {
        if (this.ActiveSceneViewModel == null)
          return (ITriggerContainer) null;
        ITriggerContainer triggerContainer = this.ActiveSceneViewModel.ActiveEditingContainer as ITriggerContainer;
        if (triggerContainer != null && triggerContainer.CanEditTriggers)
          return triggerContainer;
        return (ITriggerContainer) null;
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    internal TriggerModelManager(DesignerContext designerContext)
    {
      this.designerContext = designerContext;
    }

    public void Initialize()
    {
      this.designerContext.SelectionManager.EarlyActiveSceneUpdatePhase += new SceneUpdatePhaseEventHandler(this.SelectionManager_EarlyActiveSceneUpdatePhase);
      this.designerContext.SelectionManager.LateActiveSceneUpdatePhase += new SceneUpdatePhaseEventHandler(this.SelectionManager_LateActiveSceneUpdatePhase);
      this.addTriggerCommand = new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.CreateNewEventTrigger));
      this.addPropertyTriggerCommand = new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.CreateNewPropertyTrigger));
      this.deleteTriggerCommand = new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.DeleteTrigger));
      this.UpdateDeleteTrigger();
      this.triggerSubscription = new SceneNodeSubscription<object, TriggerModel3>();
      this.triggerSubscription.Path = new SearchPath(new SearchStep[1]
      {
        new SearchStep(SearchAxis.DocumentDescendant, (ISearchPredicate) new TargetTypePredicate(PlatformTypes.TriggerBase), (ISearchPredicate) new DelegatePredicate(new Predicate<SceneNode>(TriggerModelManager.IsNotStyleOrTemplate), SearchScope.NodeTreeSelf))
      });
      this.triggerSubscription.SetPathNodeInsertedHandler(new SceneNodeSubscription<object, TriggerModel3>.PathNodeInsertedHandler(this.TriggerSubscription_Inserted));
      this.triggerSubscription.PathNodeRemoved += new SceneNodeSubscription<object, TriggerModel3>.PathNodeRemovedListener(this.TriggerSubscription_Removed);
      this.triggerSubscription.PathNodeContentChanged += new SceneNodeSubscription<object, TriggerModel3>.PathNodeContentChangedListener(this.TriggerSubscription_ContentChanged);
      this.UpdateFromEditingContainerChange(true);
    }

    public static bool IsNotStyleOrTemplate(SceneNode sceneNode)
    {
      if (!PlatformTypes.FrameworkTemplate.IsAssignableFrom((ITypeId) sceneNode.Type) && !PlatformTypes.Style.IsAssignableFrom((ITypeId) sceneNode.Type))
        return !PlatformTypes.FrameworkElement.IsAssignableFrom((ITypeId) sceneNode.Type);
      return false;
    }

    public void CreateNewEventTrigger()
    {
      if (this.CurrentContainer == null)
        return;
      SceneViewModel activeSceneViewModel = this.ActiveSceneViewModel;
      SceneDocument document = activeSceneViewModel.Document;
      ITriggerContainer currentContainer = this.CurrentContainer;
      using (SceneEditTransaction editTransaction = document.CreateEditTransaction(StringTable.TriggerChangeUndoUnit))
      {
        IProjectContext projectContext = document.ProjectContext;
        IType type = projectContext.GetType(currentContainer.TargetElementType);
        if (type != null)
        {
          TriggerSourceInformation triggerSource = (TriggerSourceInformation) TriggersHelper.GetDefaultEvent((ITypeResolver) projectContext, type);
          if (triggerSource != (TriggerSourceInformation) null)
          {
            TriggerBaseNode trigger = TriggersHelper.CreateTrigger(triggerSource, activeSceneViewModel);
            if (trigger != null)
            {
              int index = currentContainer.VisualTriggers.Count;
              if (this.selectedItem != null)
                index = this.selectedItem != this.noneTrigger ? currentContainer.VisualTriggers.IndexOf(this.selectedItem.SceneNode) + 1 : 0;
              currentContainer.VisualTriggers.Insert(index, trigger);
              this.TriggerToBeSelected = trigger;
            }
          }
        }
        editTransaction.Commit();
      }
    }

    public void CreateNewPropertyTrigger()
    {
      if (this.CurrentContainer == null)
        return;
      ITriggerContainer currentContainer = this.CurrentContainer;
      SceneViewModel activeSceneViewModel = this.ActiveSceneViewModel;
      SceneDocument document = activeSceneViewModel.Document;
      StyleNode styleNode = currentContainer as StyleNode;
      FrameworkTemplateElement frameworkTemplateElement = currentContainer as FrameworkTemplateElement;
      using (SceneEditTransaction editTransaction = document.CreateEditTransaction(StringTable.TriggerChangeUndoUnit))
      {
        TriggerSourceInformation triggerSource = (TriggerSourceInformation) null;
        if (styleNode != null || frameworkTemplateElement != null)
          triggerSource = (TriggerSourceInformation) TriggersHelper.GetDefaultProperty(currentContainer.TargetElementType, document.DocumentContext);
        if (triggerSource != (TriggerSourceInformation) null)
        {
          TriggerBaseNode trigger = TriggersHelper.CreateTrigger(triggerSource, activeSceneViewModel);
          if (trigger != null)
          {
            int index = currentContainer.VisualTriggers.Count;
            if (this.selectedItem != null)
              index = this.selectedItem != this.noneTrigger ? currentContainer.VisualTriggers.IndexOf(this.selectedItem.SceneNode) + 1 : 0;
            currentContainer.VisualTriggers.Insert(index, trigger);
            activeSceneViewModel.SetActiveTrigger(trigger);
          }
        }
        editTransaction.Commit();
      }
    }

    public void DeleteTrigger()
    {
      TriggerModel3 selectedItem = this.SelectedItem;
      if (selectedItem == null || selectedItem.DeleteCommand == null)
        return;
      selectedItem.DeleteCommand.Execute((object) null);
    }

    private void UpdateDeleteTrigger()
    {
      TriggerModel3 selectedItem = this.SelectedItem;
      this.deleteTriggerCommand.IsEnabled = selectedItem != null && selectedItem.DeleteCommand != null && selectedItem.DeleteCommand.CanExecute((object) null);
    }

    private void SelectionManager_EarlyActiveSceneUpdatePhase(object sender, SceneUpdatePhaseEventArgs args)
    {
      if (!args.IsDirtyViewState(SceneViewModel.ViewStateBits.ActiveEditingContainer) && !args.IsDirtyViewState(SceneViewModel.ViewStateBits.IsEditable))
        return;
      this.UpdateNoneTrigger();
    }

    private void SelectionManager_LateActiveSceneUpdatePhase(object sender, SceneUpdatePhaseEventArgs args)
    {
      if (args.IsDirtyViewState(SceneViewModel.ViewStateBits.ActiveEditingContainer) || args.IsDirtyViewState(SceneViewModel.ViewStateBits.IsEditable))
        this.UpdateFromEditingContainerChange(false);
      this.triggerSubscription.Update(args.ViewModel, args.DocumentChanges, args.DocumentChangeStamp);
      foreach (SubSubscription subSubscription in this.delayedUpdates)
        subSubscription.ProcessChanges(args.DocumentChangeStamp);
      this.delayedUpdates.Clear();
      if (args.IsDirtyViewState(SceneViewModel.ViewStateBits.ActiveTrigger | SceneViewModel.ViewStateBits.RecordMode) && this.ActiveSceneViewModel != null)
      {
        this.UpdateActiveTrigger();
        if (this.SelectedItem == null || this.SelectedItem is NoneTriggerModel)
        {
          if (this.locked3DElements.Count != 0)
          {
            using (SceneEditTransaction editTransaction = this.ActiveSceneViewModel.CreateEditTransaction("Unlock3DElements", true))
            {
              SceneElementSelectionSet elementSelectionSet = this.ActiveSceneViewModel.ElementSelectionSet;
              if (elementSelectionSet != null && this.selectionChangesSinceLock <= 2)
              {
                foreach (Base3DElement base3Delement in this.locked3DElements)
                {
                  if (base3Delement.IsAttached)
                    elementSelectionSet.ExtendSelection((SceneElement) base3Delement);
                }
              }
              editTransaction.Commit();
            }
            this.locked3DElements.Clear();
          }
        }
        else if (this.locked3DElements.Count == 0)
        {
          this.selectionChangesSinceLock = 0;
          SceneElementSelectionSet elementSelectionSet = this.ActiveSceneViewModel.ElementSelectionSet;
          if (elementSelectionSet != null)
          {
            using (SceneEditTransaction editTransaction = this.ActiveSceneViewModel.CreateEditTransaction("Lock3DElements", true))
            {
              foreach (SceneElement sceneElement in elementSelectionSet.Selection)
              {
                Base3DElement base3Delement = sceneElement as Base3DElement;
                if (base3Delement != null)
                {
                  this.locked3DElements.Add(base3Delement);
                  elementSelectionSet.RemoveSelection((SceneElement) base3Delement);
                }
              }
              editTransaction.Commit();
            }
          }
        }
      }
      if (!args.IsDirtyViewState(SceneViewModel.ViewStateBits.ElementSelection))
        return;
      ++this.selectionChangesSinceLock;
    }

    private void UpdateNoneTrigger()
    {
      if (this.noneTrigger != null && this.CurrentContainer != this.noneTrigger.ActiveEditingContainer)
      {
        this.noneTrigger.Detach();
        this.triggerModels.Remove((TriggerModel3) this.noneTrigger);
        this.noneTrigger = (NoneTriggerModel) null;
      }
      if (!this.ShouldDisplayNoneTrigger || this.noneTrigger != null)
        return;
      this.noneTrigger = new NoneTriggerModel((SceneNode) this.CurrentContainer);
      this.noneTrigger.Initialize();
      this.triggerModels.Insert(0, (TriggerModel3) this.noneTrigger);
      this.SelectedItem = (TriggerModel3) this.noneTrigger;
    }

    private void UpdateFromEditingContainerChange(bool updateNoneTrigger)
    {
      if (updateNoneTrigger)
        this.UpdateNoneTrigger();
      List<SceneNode> list = new List<SceneNode>();
      if (this.ActiveSceneViewModel != null)
      {
        SceneNode editingContainer = this.ActiveSceneViewModel.ActiveEditingContainer;
        if (editingContainer != null)
          list.Add(editingContainer);
      }
      this.triggerSubscription.SetBasisNodes(this.ActiveSceneViewModel, (IEnumerable<SceneNode>) list);
      this.UpdateCommands();
    }

    private void UpdateActiveTrigger()
    {
      foreach (TriggerModel3 triggerModel3 in (Collection<TriggerModel3>) this.triggerModels)
      {
        triggerModel3.RefreshIsActive();
        if (triggerModel3.IsActive)
          this.SelectedItem = triggerModel3;
      }
    }

    private TriggerModel3 TriggerSubscription_Inserted(object sender, SceneNode basisNode, object basisContent, SceneNode newPathNode)
    {
      TriggerBaseNode trigger = (TriggerBaseNode) newPathNode;
      TriggerModel3 triggerModel3 = TriggerModel3.ConstructModel(trigger, this);
      if (triggerModel3 != null)
      {
        ITriggerContainer currentContainer = this.CurrentContainer;
        if (currentContainer == null)
          return (TriggerModel3) null;
        int triggerIndex = this.GetTriggerIndex(currentContainer, trigger);
        if (triggerIndex == -1)
          return (TriggerModel3) null;
        if (this.ShouldDisplayNoneTrigger)
          ++triggerIndex;
        this.triggerModels.Insert(triggerIndex, triggerModel3);
        if (trigger == this.TriggerToBeSelected)
        {
          this.TriggerToBeSelected = (TriggerBaseNode) null;
          this.SelectedItem = triggerModel3;
        }
      }
      return triggerModel3;
    }

    private int GetTriggerIndex(ITriggerContainer container, TriggerBaseNode trigger)
    {
      int num = 0;
      foreach (TriggerBaseNode triggerBaseNode in (IEnumerable<TriggerBaseNode>) container.VisualTriggers)
      {
        if (triggerBaseNode == trigger)
          return num;
        if (PlatformTypes.Trigger.IsAssignableFrom((ITypeId) triggerBaseNode.Type) || PlatformTypes.MultiTrigger.IsAssignableFrom((ITypeId) triggerBaseNode.Type) || PlatformTypes.EventTrigger.IsAssignableFrom((ITypeId) triggerBaseNode.Type))
          ++num;
      }
      return -1;
    }

    private void TriggerSubscription_Removed(object sender, SceneNode basisNode, object basisContent, SceneNode oldPathNode, TriggerModel3 oldContent)
    {
      if (oldContent == null)
        return;
      this.triggerModels.Remove(oldContent);
      oldContent.Detach();
    }

    private void TriggerSubscription_ContentChanged(object sender, SceneNode pathNode, TriggerModel3 content, DocumentNodeMarker damageMarker, DocumentNodeChange damage)
    {
      if (content == null)
        return;
      content.AddChange(damageMarker, damage, (IList<SubSubscription>) this.delayedUpdates);
    }

    private void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }

    private void UpdateCommands()
    {
      this.addTriggerCommand.IsEnabled = this.CurrentContainer != null;
      this.addPropertyTriggerCommand.IsEnabled = this.CurrentContainer is StyleNode || this.CurrentContainer is FrameworkTemplateElement;
    }
  }
}
