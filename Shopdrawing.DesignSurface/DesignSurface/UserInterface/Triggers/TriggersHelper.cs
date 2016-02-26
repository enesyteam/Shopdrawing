// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Triggers.TriggersHelper
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.UserInterface.Triggers
{
  public static class TriggersHelper
  {
    public static void DefaultAddAction(TriggerBaseNode trigger, TriggerActionNode action)
    {
      TriggerNode triggerNode = trigger as TriggerNode;
      MultiTriggerNode multiTriggerNode = trigger as MultiTriggerNode;
      EventTriggerNode eventTriggerNode = trigger as EventTriggerNode;
      if (triggerNode != null)
        triggerNode.EnterActions.Add(action);
      else if (multiTriggerNode != null)
      {
        multiTriggerNode.EnterActions.Add(action);
      }
      else
      {
        if (eventTriggerNode == null)
          return;
        eventTriggerNode.Actions.Add((SceneNode) action);
      }
    }

    public static ConditionNode CreateDefaultCondition(Type targetType, SceneViewModel viewModel, IDocumentContext documentContext)
    {
      PropertyInformation.GetPropertiesForType(targetType, documentContext.TypeResolver).GetEnumerator();
      PropertyInformation defaultProperty = TriggersHelper.GetDefaultProperty(targetType, documentContext);
      if (!((TriggerSourceInformation) defaultProperty != (TriggerSourceInformation) null))
        return (ConditionNode) null;
      ConditionNode conditionNode = ConditionNode.Factory.Instantiate(viewModel);
      conditionNode.PropertyKey = defaultProperty.DependencyProperty;
      conditionNode.Value = defaultProperty.DependencyProperty.DefaultMetadata.DefaultValue;
      return conditionNode;
    }

    public static TriggerActionNode CreateDefaultAction(SceneViewModel viewModel, string defaultStoryboardName)
    {
      StoryboardTimelineSceneNode timelineSceneNode = viewModel.AnimationEditor.GetFirstActiveStoryboard(false);
      if (timelineSceneNode == null)
      {
        MessageBoxArgs args = new MessageBoxArgs()
        {
          Message = StringTable.MissingDefaultStoryboardMessage,
          Button = MessageBoxButton.OKCancel,
          Image = MessageBoxImage.Asterisk
        };
        if (viewModel.DesignerContext.MessageDisplayService.ShowMessage(args) == MessageBoxResult.OK)
          timelineSceneNode = viewModel.AnimationEditor.CreateNewTimeline(viewModel.ActiveStoryboardContainer, defaultStoryboardName);
      }
      if (timelineSceneNode == null)
        return (TriggerActionNode) null;
      TimelineActionNode timelineActionNode = (TimelineActionNode) BeginActionNode.Factory.Instantiate(viewModel);
      timelineActionNode.TargetTimeline = timelineSceneNode;
      return (TriggerActionNode) timelineActionNode;
    }

    public static string GetDefaultStoryboardName(TriggerBaseNode trigger)
    {
      EventTriggerNode eventTriggerNode = trigger as EventTriggerNode;
      string suggestion = "Storyboard";
      if (eventTriggerNode != null)
        suggestion = "On" + eventTriggerNode.RoutedEvent.Name;
      return trigger.ViewModel.AnimationEditor.GenerateNewTimelineName(trigger.StoryboardContainer, suggestion);
    }

    public static void CreateDefaultTrigger(StoryboardTimelineSceneNode storyboard)
    {
      ITriggerContainer triggerContainer = (ITriggerContainer) storyboard.StoryboardContainer ?? storyboard.ViewModel.RootNode as ITriggerContainer;
      if (triggerContainer == null || !triggerContainer.CanEditTriggers)
        return;
      SceneViewModel viewModel = storyboard.ViewModel;
      IProjectContext projectContext = viewModel.ProjectContext;
      IType type = projectContext.GetType(triggerContainer.TargetElementType);
      if (type == null)
        return;
      TriggerSourceInformation triggerSource = (TriggerSourceInformation) TriggersHelper.GetDefaultEvent((ITypeResolver) projectContext, type);
      if (!(triggerSource != (TriggerSourceInformation) null))
        return;
      TriggerBaseNode orCreateTrigger = TriggersHelper.FindOrCreateTrigger(triggerSource, triggerContainer, viewModel);
      if (orCreateTrigger == null)
        return;
      TimelineActionNode timelineActionNode = (TimelineActionNode) BeginActionNode.Factory.Instantiate(viewModel);
      timelineActionNode.TargetTimeline = storyboard;
      TriggersHelper.DefaultAddAction(orCreateTrigger, (TriggerActionNode) timelineActionNode);
    }

    public static PropertyInformation GetDefaultProperty(Type targetType, IDocumentContext documentContext)
    {
      IEnumerator<PropertyInformation> enumerator = PropertyInformation.GetPropertiesForType(targetType, documentContext.TypeResolver).GetEnumerator();
      while (enumerator.MoveNext())
      {
        if (documentContext.CreateNode(typeof (DependencyProperty), (object) enumerator.Current.DependencyProperty) != null)
          return enumerator.Current;
      }
      return (PropertyInformation) null;
    }

    public static RoutedEventInformation GetDefaultEvent(ITypeResolver typeResolver, IType typeId)
    {
      RoutedEventInformation eventInformation1 = (RoutedEventInformation) null;
      foreach (RoutedEventInformation eventInformation2 in EventInformation.GetEventsForType(typeResolver, typeId, MemberType.RoutedEvent))
      {
        if (eventInformation2.RoutedEvent != null && eventInformation2.RoutedEvent == FrameworkElement.LoadedEvent)
          return eventInformation2;
        if ((TriggerSourceInformation) eventInformation1 == (TriggerSourceInformation) null)
          eventInformation1 = eventInformation2;
      }
      return eventInformation1;
    }

    public static MultiTriggerNode ConvertToMultiTrigger(TriggerNode triggerNode)
    {
      MultiTriggerNode multiTriggerNode = MultiTriggerNode.Factory.Instantiate(triggerNode.ViewModel);
      ConditionNode conditionNode = ConditionNode.Factory.Instantiate(triggerNode.ViewModel);
      conditionNode.Value = ((ITriggerConditionNode) triggerNode).Value;
      conditionNode.PropertyKey = ((ITriggerConditionNode) triggerNode).PropertyKey;
      if (((ITriggerConditionNode) triggerNode).SourceName != null)
        conditionNode.SourceName = ((ITriggerConditionNode) triggerNode).SourceName;
      multiTriggerNode.Conditions.Add(conditionNode);
      List<SceneNode> list1 = new List<SceneNode>((IEnumerable<SceneNode>) triggerNode.Setters);
      while (triggerNode.Setters.Count > 0)
        triggerNode.Setters.RemoveAt(0);
      List<TriggerActionNode> list2 = new List<TriggerActionNode>((IEnumerable<TriggerActionNode>) triggerNode.EnterActions);
      while (triggerNode.EnterActions.Count > 0)
        triggerNode.EnterActions.RemoveAt(0);
      List<TriggerActionNode> list3 = new List<TriggerActionNode>((IEnumerable<TriggerActionNode>) triggerNode.ExitActions);
      while (triggerNode.ExitActions.Count > 0)
        triggerNode.ExitActions.RemoveAt(0);
      TriggersHelper.ReplaceSceneNode((SceneNode) triggerNode, (SceneNode) multiTriggerNode);
      foreach (SceneNode sceneNode in list1)
        multiTriggerNode.Setters.Add(sceneNode);
      foreach (TriggerActionNode triggerActionNode in list2)
        multiTriggerNode.EnterActions.Add(triggerActionNode);
      foreach (TriggerActionNode triggerActionNode in list3)
        multiTriggerNode.ExitActions.Add(triggerActionNode);
      SceneViewModel viewModel = triggerNode.ViewModel;
      if (viewModel.ActiveVisualTrigger == triggerNode)
        viewModel.SetActiveTrigger((TriggerBaseNode) multiTriggerNode);
      return multiTriggerNode;
    }

    public static TriggerNode ConvertToTrigger(MultiTriggerNode multiTrigger)
    {
      TriggerNode triggerNode = TriggerNode.Factory.Instantiate(multiTrigger.ViewModel);
      if (multiTrigger.Conditions.Count > 0)
      {
        ConditionNode conditionNode = multiTrigger.Conditions[0];
        ((ITriggerConditionNode) triggerNode).Value = conditionNode.Value;
        ((ITriggerConditionNode) triggerNode).PropertyKey = conditionNode.PropertyKey;
        if (conditionNode.SourceName != null)
          ((ITriggerConditionNode) triggerNode).SourceName = conditionNode.SourceName;
      }
      List<SceneNode> list1 = new List<SceneNode>((IEnumerable<SceneNode>) multiTrigger.Setters);
      while (multiTrigger.Setters.Count > 0)
        multiTrigger.Setters.RemoveAt(0);
      List<TriggerActionNode> list2 = new List<TriggerActionNode>((IEnumerable<TriggerActionNode>) multiTrigger.EnterActions);
      while (multiTrigger.EnterActions.Count > 0)
        multiTrigger.EnterActions.RemoveAt(0);
      List<TriggerActionNode> list3 = new List<TriggerActionNode>((IEnumerable<TriggerActionNode>) multiTrigger.ExitActions);
      while (multiTrigger.ExitActions.Count > 0)
        multiTrigger.ExitActions.RemoveAt(0);
      TriggersHelper.ReplaceSceneNode((SceneNode) multiTrigger, (SceneNode) triggerNode);
      foreach (SceneNode sceneNode in list1)
        triggerNode.Setters.Add(sceneNode);
      foreach (TriggerActionNode triggerActionNode in list2)
        triggerNode.EnterActions.Add(triggerActionNode);
      foreach (TriggerActionNode triggerActionNode in list3)
        triggerNode.ExitActions.Add(triggerActionNode);
      SceneViewModel viewModel = multiTrigger.ViewModel;
      if (viewModel.ActiveVisualTrigger == multiTrigger)
        viewModel.SetActiveTrigger((TriggerBaseNode) triggerNode);
      return triggerNode;
    }

    public static void ReplaceSceneNode(SceneNode oldValue, SceneNode newValue)
    {
      ISceneNodeCollection<SceneNode> collectionForChild = oldValue.Parent.GetCollectionForChild(oldValue);
      if (collectionForChild == null)
        return;
      int index = collectionForChild.IndexOf(oldValue);
      if (index < 0)
        return;
      collectionForChild[index] = newValue;
    }

    public static TriggerBaseNode CreateTrigger(TriggerSourceInformation triggerSource, SceneViewModel viewModel)
    {
      RoutedEventInformation eventInformation = triggerSource as RoutedEventInformation;
      PropertyInformation propertyInformation = triggerSource as PropertyInformation;
      if ((TriggerSourceInformation) eventInformation != (TriggerSourceInformation) null)
      {
        EventTriggerNode eventTriggerNode = EventTriggerNode.Factory.Instantiate(viewModel);
        eventTriggerNode.RoutedEvent = eventInformation.RoutedEvent;
        return (TriggerBaseNode) eventTriggerNode;
      }
      if (!((TriggerSourceInformation) propertyInformation != (TriggerSourceInformation) null))
        return (TriggerBaseNode) null;
      TriggerNode triggerNode = (TriggerNode) null;
      DependencyProperty dependencyProperty = propertyInformation.DependencyProperty;
      if (dependencyProperty != null)
      {
        triggerNode = TriggerNode.Factory.Instantiate(viewModel);
        ITriggerConditionNode triggerConditionNode = (ITriggerConditionNode) triggerNode;
        triggerConditionNode.PropertyKey = dependencyProperty;
        triggerConditionNode.Value = dependencyProperty.DefaultMetadata.DefaultValue;
      }
      return (TriggerBaseNode) triggerNode;
    }

    public static TriggerBaseNode FindOrCreateTrigger(TriggerSourceInformation triggerSource, ITriggerContainer triggerContainer, SceneViewModel viewModel)
    {
      RoutedEventInformation eventInformation = triggerSource as RoutedEventInformation;
      PropertyInformation propertyInformation = triggerSource as PropertyInformation;
      foreach (TriggerBaseNode triggerBaseNode in (IEnumerable<TriggerBaseNode>) triggerContainer.VisualTriggers)
      {
        EventTriggerNode eventTriggerNode = triggerBaseNode as EventTriggerNode;
        TriggerNode triggerNode = triggerBaseNode as TriggerNode;
        if (eventTriggerNode != null && (TriggerSourceInformation) eventInformation != (TriggerSourceInformation) null && eventTriggerNode.RoutedEvent == eventInformation.RoutedEvent)
          return (TriggerBaseNode) eventTriggerNode;
        if (triggerNode != null && (TriggerSourceInformation) propertyInformation != (TriggerSourceInformation) null && ((ITriggerConditionNode) triggerNode).PropertyKey == propertyInformation.DependencyProperty)
          return (TriggerBaseNode) triggerNode;
      }
      TriggerBaseNode trigger = TriggersHelper.CreateTrigger(triggerSource, viewModel);
      if (trigger != null)
        triggerContainer.VisualTriggers.Add(trigger);
      return trigger;
    }

    public static bool CanDeleteTrigger(TriggerBaseNode triggerNode)
    {
      List<SceneNode> list = new List<SceneNode>();
      foreach (SceneNode sceneNode in (IEnumerable<TriggerActionNode>) triggerNode.EnterActions)
        list.Add(sceneNode);
      foreach (SceneNode sceneNode in (IEnumerable<TriggerActionNode>) triggerNode.ExitActions)
        list.Add(sceneNode);
      EventTriggerNode eventTriggerNode = triggerNode as EventTriggerNode;
      if (eventTriggerNode != null)
        list.AddRange((IEnumerable<SceneNode>) eventTriggerNode.Actions);
      foreach (SceneNode sceneNode in list)
      {
        BeginActionNode beginActionNode = sceneNode as BeginActionNode;
        if (beginActionNode != null && beginActionNode.TargetTimeline != null)
        {
          bool flag1 = false;
          bool flag2 = false;
          foreach (TimelineActionNode timelineActionNode in beginActionNode.TargetTimeline.ControllingActions)
          {
            if (!list.Contains((SceneNode) timelineActionNode))
            {
              if (timelineActionNode is BeginActionNode && timelineActionNode != beginActionNode)
                flag2 = true;
              else if (timelineActionNode is ControllableStoryboardActionNode)
                flag1 = true;
            }
          }
          if (!flag2 && flag1)
            return false;
        }
      }
      return true;
    }

    public static bool CanDeleteAction(TimelineActionNode action)
    {
      BeginActionNode beginActionNode = action as BeginActionNode;
      if (beginActionNode == null || beginActionNode.TargetTimeline == null)
        return true;
      bool flag1 = false;
      bool flag2 = false;
      foreach (TimelineActionNode timelineActionNode in action.TargetTimeline.ControllingActions)
      {
        if (timelineActionNode is BeginActionNode && timelineActionNode != beginActionNode)
          flag2 = true;
        else if (timelineActionNode is ControllableStoryboardActionNode)
          flag1 = true;
      }
      if (!flag2)
        return !flag1;
      return true;
    }

    public static bool NeedsBeginAction(StoryboardTimelineSceneNode storyboard)
    {
      bool flag = false;
      foreach (TimelineActionNode timelineActionNode in storyboard.ControllingActions)
      {
        if (timelineActionNode is BeginActionNode)
          return false;
        if (timelineActionNode is ControllableStoryboardActionNode)
          flag = true;
      }
      return flag;
    }
  }
}
