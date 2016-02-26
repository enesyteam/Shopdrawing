// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Triggers.PropertyTriggerModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.DesignSurface.ViewModel.SceneNodeSearch;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Threading;

namespace Microsoft.Expression.DesignSurface.UserInterface.Triggers
{
  public class PropertyTriggerModel : TriggerModel3
  {
    private ConditionModelCollection conditions = new ConditionModelCollection();
    private SetterCollection setters = new SetterCollection();
    private BaseTriggerNode baseTriggerNode;
    private DelegateCommand addConditionCommand;
    private DelegateCommand createEnterActionCommand;
    private DelegateCommand createExitActionCommand;
    private ActionCollection enterActions;
    private ActionCollection exitActions;
    private SceneNodeSubscription<object, ConditionModel> conditionSubscription;
    private SceneNodeSubscription<object, SetterModel> setterSubscription;
    private TriggerModelManager triggerManager;
    private bool asyncConditionSortPending;

    public BaseTriggerNode TriggerNode
    {
      get
      {
        return this.baseTriggerNode;
      }
    }

    public ActionCollection EnterActions
    {
      get
      {
        return this.enterActions;
      }
    }

    public ActionCollection ExitActions
    {
      get
      {
        return this.exitActions;
      }
    }

    public ICommand AddConditionCommand
    {
      get
      {
        return (ICommand) this.addConditionCommand;
      }
    }

    public ICommand CreateEnterActionCommand
    {
      get
      {
        return (ICommand) this.createEnterActionCommand;
      }
    }

    public ICommand CreateExitActionCommand
    {
      get
      {
        return (ICommand) this.createExitActionCommand;
      }
    }

    public override IList<object> Contents
    {
      get
      {
        IList<object> list = (IList<object>) new List<object>();
        list.Add((object) this.Conditions);
        list.Add((object) this.EnterActions);
        list.Add((object) this.Setters);
        list.Add((object) this.ExitActions);
        return list;
      }
    }

    public ConditionModelCollection Conditions
    {
      get
      {
        return this.conditions;
      }
    }

    public SetterCollection Setters
    {
      get
      {
        return this.setters;
      }
    }

    public override bool HasEffect
    {
      get
      {
        if (this.enterActions.Count <= 0 && this.exitActions.Count <= 0)
          return this.setters.Count > 0;
        return true;
      }
    }

    protected Microsoft.Expression.DesignSurface.ViewModel.TriggerNode PropertyTrigger
    {
      get
      {
        return (Microsoft.Expression.DesignSurface.ViewModel.TriggerNode) this.SceneNode;
      }
    }

    public PropertyTriggerModel(BaseTriggerNode trigger, TriggerModelManager triggerManager)
      : base((TriggerBaseNode) trigger)
    {
      this.baseTriggerNode = trigger;
      this.triggerManager = triggerManager;
      this.createEnterActionCommand = new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.CreateEnterAction));
      this.createExitActionCommand = new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.CreateExitAction));
      this.enterActions = new ActionCollection("Enter Actions", (ICommand) this.createEnterActionCommand);
      this.exitActions = new ActionCollection("Exit Actions", (ICommand) this.createExitActionCommand);
      this.addConditionCommand = new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.AddCondition));
    }

    public override void Initialize()
    {
      base.Initialize();
      this.conditionSubscription = new SceneNodeSubscription<object, ConditionModel>();
      this.conditionSubscription.Path = new SearchPath(new SearchStep[1]
      {
        new SearchStep(SearchAxis.DocumentDescendant, (ISearchPredicate) new SceneNodeTypePredicate(typeof (ITriggerConditionNode)))
      });
      this.conditionSubscription.SetPathNodeInsertedHandler(new SceneNodeSubscription<object, ConditionModel>.PathNodeInsertedHandler(this.ConditionSubscription_Inserted));
      this.conditionSubscription.PathNodeRemoved += new SceneNodeSubscription<object, ConditionModel>.PathNodeRemovedListener(this.ConditionSubscription_Removed);
      this.conditionSubscription.PathNodeContentChanged += new SceneNodeSubscription<object, ConditionModel>.PathNodeContentChangedListener(this.ConditionSubscription_ContentChanged);
      this.setterSubscription = new SceneNodeSubscription<object, SetterModel>();
      this.setterSubscription.Path = new SearchPath(new SearchStep[1]
      {
        new SearchStep(SearchAxis.DocumentDescendant, (ISearchPredicate) new SceneNodeTypePredicate(typeof (SetterSceneNode)), (ISearchPredicate) new DelegatePredicate(new Predicate<SceneNode>(TriggerModelManager.IsNotStyleOrTemplate), SearchScope.NodeTreeSelf))
      });
      this.setterSubscription.SetPathNodeInsertedHandler(new SceneNodeSubscription<object, SetterModel>.PathNodeInsertedHandler(this.SetterSubscription_Inserted));
      this.setterSubscription.PathNodeRemoved += new SceneNodeSubscription<object, SetterModel>.PathNodeRemovedListener(this.SetterSubscription_Removed);
      this.setterSubscription.PathNodeContentChanged += new SceneNodeSubscription<object, SetterModel>.PathNodeContentChangedListener(this.SetterSubscription_ContentChanged);
      List<SceneNode> list = new List<SceneNode>(1);
      list.Add((SceneNode) this.SceneNode);
      this.conditionSubscription.SetBasisNodes(this.SceneNode.ViewModel, (IEnumerable<SceneNode>) list);
      this.setterSubscription.SetBasisNodes(this.SceneNode.ViewModel, (IEnumerable<SceneNode>) list);
      if (this.baseTriggerNode is Microsoft.Expression.DesignSurface.ViewModel.TriggerNode)
        this.AddCondition((ITriggerConditionNode) this.baseTriggerNode);
      this.SortAfterUpdate();
    }

    public override void Detach()
    {
      base.Detach();
      if (this.setterSubscription != null)
      {
        this.setterSubscription.PathNodeRemoved -= new SceneNodeSubscription<object, SetterModel>.PathNodeRemovedListener(this.SetterSubscription_Removed);
        this.setterSubscription.PathNodeContentChanged -= new SceneNodeSubscription<object, SetterModel>.PathNodeContentChangedListener(this.SetterSubscription_ContentChanged);
        this.setterSubscription.CurrentViewModel = (SceneViewModel) null;
        this.setterSubscription = (SceneNodeSubscription<object, SetterModel>) null;
      }
      if (this.conditionSubscription == null)
        return;
      this.conditionSubscription.PathNodeRemoved -= new SceneNodeSubscription<object, ConditionModel>.PathNodeRemovedListener(this.ConditionSubscription_Removed);
      this.conditionSubscription.PathNodeContentChanged -= new SceneNodeSubscription<object, ConditionModel>.PathNodeContentChangedListener(this.ConditionSubscription_ContentChanged);
      this.conditionSubscription.CurrentViewModel = (SceneViewModel) null;
      this.conditionSubscription = (SceneNodeSubscription<object, ConditionModel>) null;
    }

    public override TriggerActionModel AddAction(TriggerActionNode action)
    {
      int index1 = this.SceneNode.EnterActions.IndexOf(action);
      if (index1 != -1)
      {
        TriggerActionModel triggerActionModel = TriggerActionModel.ConstructModel(action);
        if (triggerActionModel != null)
        {
          this.EnterActions.Insert(index1, triggerActionModel);
          this.OnPropertyChanged("HasEffect");
        }
        return triggerActionModel;
      }
      int index2 = this.SceneNode.ExitActions.IndexOf(action);
      if (index2 == -1)
        return (TriggerActionModel) null;
      TriggerActionModel triggerActionModel1 = TriggerActionModel.ConstructModel(action);
      if (triggerActionModel1 != null)
      {
        this.ExitActions.Insert(index2, triggerActionModel1);
        this.OnPropertyChanged("HasEffect");
      }
      return triggerActionModel1;
    }

    public override void RemoveAction(TriggerActionNode action)
    {
      foreach (TriggerActionModel triggerActionModel in (Collection<TriggerActionModel>) this.EnterActions)
      {
        if (triggerActionModel.SceneNode == action)
        {
          this.EnterActions.Remove(triggerActionModel);
          this.OnPropertyChanged("HasEffect");
          return;
        }
      }
      foreach (TriggerActionModel triggerActionModel in (Collection<TriggerActionModel>) this.ExitActions)
      {
        if (triggerActionModel.SceneNode == action)
        {
          this.ExitActions.Remove(triggerActionModel);
          this.OnPropertyChanged("HasEffect");
          break;
        }
      }
    }

    public override bool ContainsAction(TriggerActionNode action)
    {
      foreach (TriggerActionModel triggerActionModel in (Collection<TriggerActionModel>) this.EnterActions)
      {
        if (triggerActionModel.SceneNode == action)
          return true;
      }
      foreach (TriggerActionModel triggerActionModel in (Collection<TriggerActionModel>) this.ExitActions)
      {
        if (triggerActionModel.SceneNode == action)
          return true;
      }
      return false;
    }

    public ConditionModel AddCondition(ITriggerConditionNode condition)
    {
      ConditionModel conditionModel = new ConditionModel(condition, this);
      conditionModel.Initialize();
      conditionModel.PropertyChanged += new PropertyChangedEventHandler(this.Condition_PropertyChanged);
      this.conditions.Add(conditionModel);
      this.AsyncSortConditions();
      return conditionModel;
    }

    private void Condition_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == "TriggerSource"))
        return;
      this.AsyncSortConditions();
    }

    private void AsyncSortConditions()
    {
      if (this.asyncConditionSortPending)
        return;
      this.asyncConditionSortPending = true;
      UIThreadDispatcher.Instance.BeginInvoke(DispatcherPriority.Normal, (Action) (() =>
      {
        this.asyncConditionSortPending = false;
        this.SortConditions();
      }));
    }

    private void SortConditions()
    {
      this.conditions.Sort(new Comparison<ConditionModel>(PropertyTriggerModel.CompareConditions));
      SceneNode sceneNode = (SceneNode) null;
      foreach (ConditionModel conditionModel in (Collection<ConditionModel>) this.conditions)
      {
        conditionModel.SetFirstSourceNameInGroup(conditionModel.TriggerSource != sceneNode);
        sceneNode = conditionModel.TriggerSource;
      }
    }

    public void RemoveCondition(ITriggerConditionNode condition)
    {
      foreach (ConditionModel conditionModel in (Collection<ConditionModel>) this.conditions)
      {
        if (conditionModel.Condition == condition)
        {
          conditionModel.PropertyChanged -= new PropertyChangedEventHandler(this.Condition_PropertyChanged);
          this.conditions.Remove(conditionModel);
          break;
        }
      }
    }

    public SetterModel AddSetter(SetterSceneNode setter)
    {
      SetterModel setterModel = new SetterModel(setter);
      setterModel.Initialize();
      this.setters.Add(setterModel);
      this.OnPropertyChanged("HasEffect");
      return setterModel;
    }

    public void RemoveSetter(SetterSceneNode setter)
    {
      foreach (SetterModel setterModel in (Collection<SetterModel>) this.setters)
      {
        if (setterModel.Setter == setter)
        {
          this.setters.Remove(setterModel);
          break;
        }
      }
      this.OnPropertyChanged("HasEffect");
    }

    public override void Activate()
    {
      this.baseTriggerNode.ViewModel.SetActiveTrigger((TriggerBaseNode) this.baseTriggerNode);
    }

    protected override void Delete()
    {
      if (this.SceneNode.ViewModel.ActiveVisualTrigger == this.SceneNode)
        this.SceneNode.ViewModel.SetActiveTrigger((TriggerBaseNode) null);
      base.Delete();
    }

    private static int CompareSetters(SetterModel a, SetterModel b)
    {
      if (a.Target == b.Target)
        return a.Property.CompareTo(b.Property);
      if (a.Target is FrameworkTemplateElement)
        return -1;
      if (b.Target is FrameworkTemplateElement)
        return 1;
      return a.TargetDisplayName.CompareTo(b.TargetDisplayName);
    }

    private static int CompareConditions(ConditionModel a, ConditionModel b)
    {
      if (a.TriggerSource == b.TriggerSource)
      {
        if ((TriggerSourceInformation) a.Property == (TriggerSourceInformation) null && (TriggerSourceInformation) b.Property == (TriggerSourceInformation) null)
          return 0;
        if ((TriggerSourceInformation) a.Property == (TriggerSourceInformation) null)
          return -1;
        if ((TriggerSourceInformation) b.Property == (TriggerSourceInformation) null)
          return 1;
        return a.Property.CompareTo((object) b.Property);
      }
      if (a.TriggerSource is FrameworkTemplateElement)
        return -1;
      if (b.TriggerSource is FrameworkTemplateElement)
        return 1;
      return a.TriggerSourceDisplayName.CompareTo(b.TriggerSourceDisplayName);
    }

    private void CreateEnterAction()
    {
      TriggerActionNode defaultAction = TriggersHelper.CreateDefaultAction(this.SceneNode.ViewModel, TriggersHelper.GetDefaultStoryboardName(this.SceneNode));
      if (defaultAction == null)
        return;
      using (SceneEditTransaction editTransaction = this.SceneNode.ViewModel.CreateEditTransaction(StringTable.TriggerChangeUndoUnit))
      {
        this.SceneNode.EnterActions.Add(defaultAction);
        editTransaction.Commit();
      }
    }

    private void CreateExitAction()
    {
      TriggerActionNode defaultAction = TriggersHelper.CreateDefaultAction(this.SceneNode.ViewModel, TriggersHelper.GetDefaultStoryboardName(this.SceneNode));
      if (defaultAction == null)
        return;
      using (SceneEditTransaction editTransaction = this.SceneNode.ViewModel.CreateEditTransaction(StringTable.TriggerChangeUndoUnit))
      {
        this.SceneNode.ExitActions.Add(defaultAction);
        editTransaction.Commit();
      }
    }

    private void AddCondition()
    {
      ConditionNode defaultCondition = TriggersHelper.CreateDefaultCondition(this.baseTriggerNode.TriggerContainer.TargetElementType, this.baseTriggerNode.ViewModel, this.baseTriggerNode.DocumentContext);
      if (defaultCondition == null)
        return;
      using (SceneEditTransaction editTransaction = this.baseTriggerNode.ViewModel.CreateEditTransaction(StringTable.TriggerChangeUndoUnit))
      {
        MultiTriggerNode multiTriggerNode1 = this.baseTriggerNode as MultiTriggerNode;
        if (multiTriggerNode1 != null)
        {
          multiTriggerNode1.Conditions.Add(defaultCondition);
        }
        else
        {
          MultiTriggerNode multiTriggerNode2 = TriggersHelper.ConvertToMultiTrigger((Microsoft.Expression.DesignSurface.ViewModel.TriggerNode) this.baseTriggerNode);
          multiTriggerNode2.Conditions.Add(defaultCondition);
          this.triggerManager.TriggerToBeSelected = (TriggerBaseNode) multiTriggerNode2;
        }
        editTransaction.Commit();
      }
    }

    protected override void Update(DocumentNodeChangeList changes, uint documentChangeStamp)
    {
      base.Update(changes, documentChangeStamp);
      this.conditionSubscription.Update(this.SceneNode.ViewModel, changes, documentChangeStamp);
      this.setterSubscription.Update(this.SceneNode.ViewModel, changes, documentChangeStamp);
      if (this.baseTriggerNode is Microsoft.Expression.DesignSurface.ViewModel.TriggerNode)
        this.conditions[0].Update();
      this.SortAfterUpdate();
    }

    protected void SortAfterUpdate()
    {
      this.setters.Sort(new Comparison<SetterModel>(PropertyTriggerModel.CompareSetters));
    }

    private ConditionModel ConditionSubscription_Inserted(object sender, SceneNode basisNode, object basisContent, SceneNode newPathNode)
    {
      return this.AddCondition((ITriggerConditionNode) newPathNode);
    }

    private void ConditionSubscription_Removed(object sender, SceneNode basisNode, object basisContent, SceneNode oldPathNode, ConditionModel oldContent)
    {
      this.RemoveCondition(oldContent.Condition);
    }

    private void ConditionSubscription_ContentChanged(object sender, SceneNode pathNode, ConditionModel content, DocumentNodeMarker damageMarker, DocumentNodeChange damage)
    {
      content.Update();
      this.OnPropertyChanged("TriggerNode");
    }

    private SetterModel SetterSubscription_Inserted(object sender, SceneNode basisNode, object basisContent, SceneNode newPathNode)
    {
      return this.AddSetter((SetterSceneNode) newPathNode);
    }

    private void SetterSubscription_Removed(object sender, SceneNode basisNode, object basisContent, SceneNode oldPathNode, SetterModel oldContent)
    {
      this.RemoveSetter(oldContent.Setter);
    }

    private void SetterSubscription_ContentChanged(object sender, SceneNode pathNode, SetterModel content, DocumentNodeMarker damageMarker, DocumentNodeChange damage)
    {
      content.Update();
    }

    public override void Update()
    {
      this.OnPropertyChanged("TriggerNode");
    }
  }
}
