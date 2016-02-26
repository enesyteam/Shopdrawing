// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Triggers.EventTriggerModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.UserInterface.Triggers
{
  public class EventTriggerModel : TriggerModel3
  {
    private EventTriggerNode eventTrigger;
    private ActionCollection actions;
    private DelegateCommand createActionCommand;

    public ActionCollection Actions
    {
      get
      {
        return this.actions;
      }
    }

    public override bool HasEffect
    {
      get
      {
        return this.actions.Count > 0;
      }
    }

    public IEnumerable AvailableTriggerSources
    {
      get
      {
        ObservableCollectionWorkaround<RoutedEventInformation> collectionWorkaround = new ObservableCollectionWorkaround<RoutedEventInformation>();
        IProjectContext projectContext = this.SceneNode.ProjectContext;
        IType type = projectContext.GetType(this.TargetElementType);
        if (type != null)
        {
          foreach (RoutedEventInformation eventInformation in EventInformation.GetEventsForType((ITypeResolver) projectContext, type, MemberType.RoutedEvent))
            collectionWorkaround.Add(eventInformation);
        }
        collectionWorkaround.Sort();
        ICollectionView defaultView = CollectionViewSource.GetDefaultView((object) collectionWorkaround);
        defaultView.GroupDescriptions.Add((GroupDescription) new PropertyGroupDescription()
        {
          PropertyName = "GroupBy"
        });
        return (IEnumerable) defaultView;
      }
    }

    private Type TargetElementType
    {
      get
      {
        ITriggerContainer triggerContainer = this.EventSource as ITriggerContainer;
        if (triggerContainer != null)
          return triggerContainer.TargetElementType;
        return typeof (object);
      }
    }

    public RoutedEventInformation EventTrigger
    {
      get
      {
        RoutedEvent routedEvent = this.EventTriggerNode.RoutedEvent;
        if (routedEvent != null)
        {
          IEvent @event = PlatformTypeHelper.GetEvent((ITypeResolver) this.EventTriggerNode.ProjectContext, routedEvent);
          if (@event != null)
            return new RoutedEventInformation(@event, (IType) null);
        }
        return (RoutedEventInformation) null;
      }
      set
      {
        if (!((TriggerSourceInformation) value != (TriggerSourceInformation) null) || value.RoutedEvent == this.EventTriggerNode.RoutedEvent)
          return;
        using (SceneEditTransaction editTransaction = this.SceneNode.ViewModel.CreateEditTransaction(StringTable.TriggerChangeUndoUnit))
        {
          this.eventTrigger.RoutedEvent = value.RoutedEvent;
          editTransaction.Commit();
        }
      }
    }

    public string EventSourceDisplayName
    {
      get
      {
        return SceneNodeToStringConverter.ConvertToString(this.EventSource);
      }
    }

    public SceneNode EventSource
    {
      get
      {
        return this.EventTriggerNode.Source;
      }
      set
      {
        if (value == null || value == this.EventSource)
          return;
        using (SceneEditTransaction editTransaction = this.SceneNode.ViewModel.CreateEditTransaction(StringTable.TriggerChangeUndoUnit))
        {
          this.eventTrigger.Source = value;
          RoutedEvent routedEvent = (RoutedEvent) null;
          IProjectContext projectContext = this.SceneNode.ProjectContext;
          IType type = value.Type;
          RoutedEventInformation eventInformation = EventTriggerModel.FromName((ITypeResolver) projectContext, this.EventTrigger.EventName, type);
          if ((TriggerSourceInformation) eventInformation != (TriggerSourceInformation) null)
          {
            routedEvent = eventInformation.RoutedEvent;
          }
          else
          {
            RoutedEventInformation defaultEvent = TriggersHelper.GetDefaultEvent((ITypeResolver) projectContext, type);
            if ((TriggerSourceInformation) defaultEvent != (TriggerSourceInformation) null)
              routedEvent = defaultEvent.RoutedEvent;
          }
          this.eventTrigger.RoutedEvent = routedEvent;
          editTransaction.Commit();
        }
      }
    }

    public IEnumerable AvailableEventSources
    {
      get
      {
        yield return (object) this.EventSource;
        foreach (SceneNode sceneNode in this.SceneNode.ViewModel.ElementSelectionSet.Selection)
        {
          if (sceneNode != this.EventSource)
            yield return (object) sceneNode;
        }
      }
    }

    public override IList<object> Contents
    {
      get
      {
        IList<object> list = (IList<object>) new List<object>();
        list.Add((object) this.Actions);
        return list;
      }
    }

    public ICommand CreateActionCommand
    {
      get
      {
        return (ICommand) this.createActionCommand;
      }
    }

    protected EventTriggerNode EventTriggerNode
    {
      get
      {
        return (EventTriggerNode) this.SceneNode;
      }
    }

    public EventTriggerModel(EventTriggerNode trigger)
      : base((TriggerBaseNode) trigger)
    {
      this.eventTrigger = trigger;
      this.createActionCommand = new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.CreateAction));
      this.actions = new ActionCollection("Actions", (ICommand) this.createActionCommand);
    }

    public override void Initialize()
    {
      base.Initialize();
    }

    public void CreateAction()
    {
      TriggerActionNode defaultAction = TriggersHelper.CreateDefaultAction(this.SceneNode.ViewModel, TriggersHelper.GetDefaultStoryboardName(this.SceneNode));
      if (defaultAction == null)
        return;
      using (SceneEditTransaction editTransaction = this.SceneNode.ViewModel.CreateEditTransaction(StringTable.TriggerChangeUndoUnit))
      {
        TriggersHelper.DefaultAddAction(this.SceneNode, defaultAction);
        editTransaction.Commit();
      }
    }

    public override TriggerActionModel AddAction(TriggerActionNode action)
    {
      TriggerActionModel triggerActionModel = TriggerActionModel.ConstructModel(action);
      if (triggerActionModel != null)
      {
        int index = this.eventTrigger.Actions.IndexOf((SceneNode) action);
        if (index != -1)
          this.actions.Insert(index, triggerActionModel);
      }
      this.OnPropertyChanged("HasEffect");
      return triggerActionModel;
    }

    public override void RemoveAction(TriggerActionNode action)
    {
      foreach (TriggerActionModel triggerActionModel in (Collection<TriggerActionModel>) this.actions)
      {
        if (triggerActionModel.SceneNode == action)
        {
          this.actions.Remove(triggerActionModel);
          break;
        }
      }
      this.OnPropertyChanged("HasEffect");
    }

    public override bool ContainsAction(TriggerActionNode action)
    {
      foreach (TriggerActionModel triggerActionModel in (Collection<TriggerActionModel>) this.Actions)
      {
        if (triggerActionModel.SceneNode == action)
          return true;
      }
      return false;
    }

    public override void Update()
    {
      this.OnPropertyChanged("AvailableTriggerSources");
      this.OnPropertyChanged("AvailableEventSources");
      this.OnPropertyChanged("EventSource");
      this.OnPropertyChanged("EventSourceDisplayName");
      this.OnPropertyChanged("EventTrigger");
    }

    public override void Activate()
    {
      this.eventTrigger.ViewModel.SetActiveTrigger((TriggerBaseNode) null);
    }

    private static RoutedEventInformation FromName(ITypeResolver typeResolver, string eventName, IType typeId)
    {
      foreach (RoutedEventInformation eventInformation in EventInformation.GetEventsForType(typeResolver, typeId, MemberType.RoutedEvent))
      {
        if (eventInformation.EventName == eventName)
          return eventInformation;
      }
      return (RoutedEventInformation) null;
    }
  }
}
