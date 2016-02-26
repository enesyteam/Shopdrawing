// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Triggers.TriggerModel3
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.DesignSurface.ViewModel.SceneNodeSearch;
using Microsoft.Expression.Framework.Data;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.UserInterface.Triggers
{
  public abstract class TriggerModel3 : SubSubscription, INotifyPropertyChanged
  {
    private TriggerBaseNode trigger;
    private DelegateCommand deleteCommand;
    private SceneNodeSubscription<object, TriggerActionModel> actionSubscription;

    public TriggerBaseNode SceneNode
    {
      get
      {
        return this.trigger;
      }
      protected set
      {
        this.trigger = value;
      }
    }

    public abstract IList<object> Contents { get; }

    public ICommand DeleteCommand
    {
      get
      {
        return (ICommand) this.deleteCommand;
      }
    }

    public virtual bool IsActive
    {
      get
      {
        return this.SceneNode.ViewModel.ActiveVisualTrigger == this.SceneNode;
      }
    }

    public virtual bool IsRecording
    {
      get
      {
        if (this.SceneNode.ViewModel.AnimationEditor.IsRecording)
          return this.IsActive;
        return false;
      }
      set
      {
        this.SceneNode.ViewModel.AnimationEditor.IsRecording = value;
      }
    }

    public virtual bool HasEffect
    {
      get
      {
        return false;
      }
    }

    protected ITriggerContainer Container
    {
      get
      {
        return this.SceneNode.TriggerContainer;
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    internal TriggerModel3(TriggerBaseNode trigger)
    {
      this.trigger = trigger;
    }

    public virtual void Initialize()
    {
      this.deleteCommand = new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.Delete));
      this.actionSubscription = new SceneNodeSubscription<object, TriggerActionModel>();
      this.actionSubscription.Path = new SearchPath(new SearchStep[1]
      {
        new SearchStep(SearchAxis.DocumentDescendant, (ISearchPredicate) new TargetTypePredicate(PlatformTypes.TriggerAction))
      });
      this.actionSubscription.SetPathNodeInsertedHandler(new SceneNodeSubscription<object, TriggerActionModel>.PathNodeInsertedHandler(this.ActionSubscription_Inserted));
      this.actionSubscription.PathNodeRemoved += new SceneNodeSubscription<object, TriggerActionModel>.PathNodeRemovedListener(this.ActionSubscription_Removed);
      this.actionSubscription.PathNodeContentChanged += new SceneNodeSubscription<object, TriggerActionModel>.PathNodeContentChangedListener(this.ActionSubscription_ContentChanged);
      this.actionSubscription.SetBasisNodes(this.SceneNode.ViewModel, (IEnumerable<Microsoft.Expression.DesignSurface.ViewModel.SceneNode>) new List<Microsoft.Expression.DesignSurface.ViewModel.SceneNode>(1)
      {
        (Microsoft.Expression.DesignSurface.ViewModel.SceneNode) this.SceneNode
      });
    }

    public virtual void Detach()
    {
      if (this.actionSubscription == null)
        return;
      this.actionSubscription.PathNodeRemoved -= new SceneNodeSubscription<object, TriggerActionModel>.PathNodeRemovedListener(this.ActionSubscription_Removed);
      this.actionSubscription.PathNodeContentChanged -= new SceneNodeSubscription<object, TriggerActionModel>.PathNodeContentChangedListener(this.ActionSubscription_ContentChanged);
      this.actionSubscription.CurrentViewModel = (SceneViewModel) null;
      this.actionSubscription = (SceneNodeSubscription<object, TriggerActionModel>) null;
    }

    public abstract void Update();

    public abstract void Activate();

    public void RefreshIsActive()
    {
      this.OnPropertyChanged("IsActive");
      this.OnPropertyChanged("IsRecording");
    }

    public abstract TriggerActionModel AddAction(TriggerActionNode action);

    public abstract void RemoveAction(TriggerActionNode action);

    public abstract bool ContainsAction(TriggerActionNode action);

    protected virtual void Delete()
    {
      if (!this.SceneNode.IsAttached)
        return;
      if (!TriggersHelper.CanDeleteTrigger(this.SceneNode))
      {
        this.SceneNode.ViewModel.DesignerContext.MessageDisplayService.ShowError(StringTable.TriggerDeleteBeginAction);
      }
      else
      {
        using (SceneEditTransaction editTransaction = this.SceneNode.ViewModel.CreateEditTransaction(StringTable.TriggerChangeUndoUnit))
        {
          this.SceneNode.Remove();
          editTransaction.Commit();
        }
      }
    }

    internal static TriggerModel3 ConstructModel(TriggerBaseNode trigger, TriggerModelManager triggerManager)
    {
      EventTriggerNode trigger1 = trigger as EventTriggerNode;
      BaseTriggerNode trigger2 = trigger as BaseTriggerNode;
      TriggerModel3 triggerModel3 = (TriggerModel3) null;
      if (trigger1 != null)
        triggerModel3 = (TriggerModel3) new EventTriggerModel(trigger1);
      else if (trigger2 != null)
        triggerModel3 = (TriggerModel3) new PropertyTriggerModel(trigger2, triggerManager);
      if (triggerModel3 != null)
        triggerModel3.Initialize();
      return triggerModel3;
    }

    protected void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }

    protected override void Update(DocumentNodeChangeList changes, uint documentChangeStamp)
    {
      this.actionSubscription.Update(this.SceneNode.ViewModel, changes, documentChangeStamp);
      this.Update();
    }

    private TriggerActionModel ActionSubscription_Inserted(object sender, Microsoft.Expression.DesignSurface.ViewModel.SceneNode basisNode, object basisContent, Microsoft.Expression.DesignSurface.ViewModel.SceneNode newPathNode)
    {
      return this.AddAction((TriggerActionNode) newPathNode);
    }

    private void ActionSubscription_Removed(object sender, Microsoft.Expression.DesignSurface.ViewModel.SceneNode basisNode, object basisContent, Microsoft.Expression.DesignSurface.ViewModel.SceneNode oldPathNode, TriggerActionModel oldContent)
    {
      this.RemoveAction(oldContent.SceneNode);
    }

    private void ActionSubscription_ContentChanged(object sender, Microsoft.Expression.DesignSurface.ViewModel.SceneNode pathNode, TriggerActionModel content, DocumentNodeMarker damageMarker, DocumentNodeChange damage)
    {
      content.Update();
    }
  }
}
