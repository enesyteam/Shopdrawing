// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.BehaviorEventTriggerNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.UserInterface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class BehaviorEventTriggerNode : BehaviorEventTriggerBaseNode
  {
    public static readonly IPropertyId EventNameProperty = (IPropertyId) ProjectNeutralTypes.BehaviorEventTrigger.GetMember(MemberType.LocalProperty, "EventName", MemberAccessTypes.Public);
    public static readonly BehaviorEventTriggerNode.ConcreteBehaviorEventTriggerNodeFactory Factory = new BehaviorEventTriggerNode.ConcreteBehaviorEventTriggerNodeFactory();

    public string EventName
    {
      get
      {
        return (string) this.GetComputedValue(BehaviorEventTriggerNode.EventNameProperty);
      }
      set
      {
        DocumentNode documentNode = (DocumentNode) null;
        if (!string.IsNullOrEmpty(value))
          documentNode = this.CreateNode((object) value);
        this.DocumentCompositeNode.Properties[BehaviorEventTriggerNode.EventNameProperty] = documentNode;
      }
    }

    protected override void ModifyValue(PropertyReference propertyReference, object valueToSet, SceneNode.Modification modification, int index)
    {
      base.ModifyValue(propertyReference, valueToSet, modification, index);
      if (!this.IsInDocument)
        return;
      SceneNode sourceNode = (SceneNode) null;
      bool flag = this.IsSet(BehaviorEventTriggerBaseNode.BehaviorSourceObjectProperty) == PropertyState.Set;
      int num = (int) this.IsSet(BehaviorEventTriggerBaseNode.BehaviorSourceNameProperty);
      if (propertyReference.LastStep.Equals((object) BehaviorEventTriggerBaseNode.BehaviorSourceObjectProperty))
      {
        this.ViewModel.Document.OnUpdatedEditTransaction();
        if (modification == SceneNode.Modification.SetValue)
          sourceNode = this.ViewModel.CreateSceneNode(this.GetComputedValue(propertyReference));
        else if (modification == SceneNode.Modification.ClearValue)
          sourceNode = BehaviorHelper.FindNamedElement((SceneNode) this, this.SourceName);
      }
      else if (!flag && propertyReference.LastStep.Equals((object) BehaviorEventTriggerBaseNode.BehaviorSourceNameProperty))
        sourceNode = BehaviorHelper.FindNamedElement((SceneNode) this, valueToSet as string);
      BehaviorEventTriggerNode.FixUpEventNameImpl(this, sourceNode);
    }

    internal static void FixUpEventName(BehaviorEventTriggerNode eventTriggerNode)
    {
      eventTriggerNode.ViewModel.Document.OnUpdatedEditTransaction();
      BehaviorEventTriggerNode.FixUpEventNameImpl(eventTriggerNode, eventTriggerNode.SourceNode);
    }

    private static void FixUpEventNameImpl(BehaviorEventTriggerNode eventTriggerNode, SceneNode sourceNode)
    {
      if (sourceNode == null)
        return;
      IEnumerable<EventInformation> eventsForType = EventInformation.GetEventsForType((ITypeResolver) eventTriggerNode.ProjectContext, sourceNode.Type, MemberType.LocalEvent);
      if (!Enumerable.Any<EventInformation>(eventsForType, (Func<EventInformation, bool>) (info => info.EventName.Equals(eventTriggerNode.EventName, StringComparison.Ordinal))))
      {
        bool flag = false;
        DefaultEventAttribute defaultEventAttribute = TypeUtilities.GetAttribute<DefaultEventAttribute>(sourceNode.TargetType);
        if (defaultEventAttribute != null && !string.IsNullOrEmpty(defaultEventAttribute.Name) && (TriggerSourceInformation) Enumerable.SingleOrDefault<EventInformation>(eventsForType, (Func<EventInformation, bool>) (eventInfo => eventInfo.EventName == defaultEventAttribute.Name)) != (TriggerSourceInformation) null)
        {
          eventTriggerNode.SetValue(BehaviorEventTriggerNode.EventNameProperty, (object) defaultEventAttribute.Name);
          flag = true;
        }
        if (!flag && Enumerable.Any<EventInformation>(eventsForType, (Func<EventInformation, bool>) (info => info.EventName.Equals("Loaded", StringComparison.Ordinal))))
          eventTriggerNode.SetValue(BehaviorEventTriggerNode.EventNameProperty, (object) "Loaded");
        else
          eventTriggerNode.ClearValue(BehaviorEventTriggerNode.EventNameProperty);
      }
      ISceneNodeCollection<SceneNode> collectionForProperty = eventTriggerNode.Parent.GetCollectionForProperty(BehaviorHelper.BehaviorTriggersProperty);
      BehaviorTriggerBaseNode matchingTriggerNode = BehaviorHelper.FindMatchingTriggerNode(eventTriggerNode.DocumentNode, collectionForProperty);
      if (matchingTriggerNode == null || matchingTriggerNode.Equals((object) eventTriggerNode))
        return;
      for (int index = eventTriggerNode.Actions.Count - 1; index >= 0; --index)
      {
        BehaviorTriggerActionNode action = (BehaviorTriggerActionNode) eventTriggerNode.Actions[index];
        BehaviorHelper.ReparentAction(collectionForProperty, (BehaviorTriggerBaseNode) eventTriggerNode, matchingTriggerNode, action);
      }
      collectionForProperty.Remove((SceneNode) eventTriggerNode);
    }

    public class ConcreteBehaviorEventTriggerNodeFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new BehaviorEventTriggerNode();
      }

      public BehaviorEventTriggerNode Instantiate(SceneViewModel viewModel)
      {
        return (BehaviorEventTriggerNode) this.Instantiate(viewModel, ProjectNeutralTypes.BehaviorEventTrigger);
      }
    }
  }
}
