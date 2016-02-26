// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.TriggerBaseNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class TriggerBaseNode : SceneNode
  {
    public static readonly IPropertyId EnterActionsProperty = (IPropertyId) PlatformTypes.TriggerBase.GetMember(MemberType.LocalProperty, "EnterActions", MemberAccessTypes.Public);
    public static readonly IPropertyId ExitActionsProperty = (IPropertyId) PlatformTypes.TriggerBase.GetMember(MemberType.LocalProperty, "ExitActions", MemberAccessTypes.Public);
    public static readonly TriggerBaseNode.ConcreteTriggerBaseNodeFactory Factory = new TriggerBaseNode.ConcreteTriggerBaseNodeFactory();

    public virtual string PresentationName
    {
      get
      {
        return string.Empty;
      }
    }

    public ISceneNodeCollection<TriggerActionNode> EnterActions
    {
      get
      {
        return (ISceneNodeCollection<TriggerActionNode>) new SceneNode.SceneNodeCollection<TriggerActionNode>((SceneNode) this, TriggerBaseNode.EnterActionsProperty);
      }
    }

    public ISceneNodeCollection<TriggerActionNode> ExitActions
    {
      get
      {
        return (ISceneNodeCollection<TriggerActionNode>) new SceneNode.SceneNodeCollection<TriggerActionNode>((SceneNode) this, TriggerBaseNode.ExitActionsProperty);
      }
    }

    public ITriggerContainer TriggerContainer
    {
      get
      {
        return (ITriggerContainer) this.StoryboardContainer;
      }
    }

    public override string ToString()
    {
      return this.PresentationName;
    }

    public IEnumerable<TriggerActionNode> GetActions()
    {
      DocumentCompositeNode compositeNode = this.DocumentNode as DocumentCompositeNode;
      if (compositeNode != null)
      {
        IPlatformMetadata metadata = compositeNode.TypeResolver.PlatformMetadata;
        if (compositeNode.Properties.Contains(metadata.ResolveProperty(TriggerBaseNode.EnterActionsProperty)))
        {
          foreach (TriggerActionNode triggerActionNode in (IEnumerable<TriggerActionNode>) this.EnterActions)
            yield return triggerActionNode;
        }
        if (compositeNode.Properties.Contains(metadata.ResolveProperty(TriggerBaseNode.ExitActionsProperty)))
        {
          foreach (TriggerActionNode triggerActionNode in (IEnumerable<TriggerActionNode>) this.ExitActions)
            yield return triggerActionNode;
        }
        if (compositeNode.Properties.Contains(metadata.ResolveProperty(EventTriggerNode.ActionsProperty)))
        {
          EventTriggerNode eventTrigger = this as EventTriggerNode;
          if (eventTrigger != null)
          {
            foreach (TriggerActionNode triggerActionNode in (IEnumerable<SceneNode>) eventTrigger.Actions)
              yield return triggerActionNode;
          }
        }
      }
    }

    public class ConcreteTriggerBaseNodeFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new TriggerBaseNode();
      }
    }
  }
}
