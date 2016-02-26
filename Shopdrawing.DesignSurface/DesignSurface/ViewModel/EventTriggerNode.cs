// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.EventTriggerNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public sealed class EventTriggerNode : TriggerBaseNode
  {
    public static readonly IPropertyId ActionsProperty = (IPropertyId) PlatformTypes.EventTrigger.GetMember(MemberType.LocalProperty, "Actions", MemberAccessTypes.Public);
    public static readonly IPropertyId RoutedEventProperty = (IPropertyId) PlatformTypes.EventTrigger.GetMember(MemberType.LocalProperty, "RoutedEvent", MemberAccessTypes.Public);
    public static readonly IPropertyId SourceNameProperty = (IPropertyId) PlatformTypes.EventTrigger.GetMember(MemberType.LocalProperty, "SourceName", MemberAccessTypes.Public);
    public static readonly EventTriggerNode.ConcreteEventTriggerNodeFactory Factory = new EventTriggerNode.ConcreteEventTriggerNodeFactory();

    public ISceneNodeCollection<SceneNode> Actions
    {
      get
      {
        return (ISceneNodeCollection<SceneNode>) new SceneNode.SceneNodeCollection<SceneNode>((SceneNode) this, EventTriggerNode.ActionsProperty);
      }
    }

    public RoutedEvent RoutedEvent
    {
      get
      {
        return (RoutedEvent) this.GetLocalValue(EventTriggerNode.RoutedEventProperty);
      }
      set
      {
        this.DocumentCompositeNode.Properties[EventTriggerNode.RoutedEventProperty] = (DocumentNode) DocumentNodeUtilities.NewRoutedEventNode(this.DocumentContext, value);
      }
    }

    public string SourceID
    {
      get
      {
        IProperty property = this.ProjectContext.ResolveProperty(EventTriggerNode.SourceNameProperty);
        if (property == null)
          return (string) null;
        return DocumentPrimitiveNode.GetValueAsString(this.DocumentCompositeNode.Properties[(IPropertyId) property]);
      }
      set
      {
        DocumentNode documentNode = (DocumentNode) null;
        if (value != null)
          documentNode = (DocumentNode) this.DocumentContext.CreateNode(value);
        this.DocumentCompositeNode.Properties[EventTriggerNode.SourceNameProperty] = documentNode;
      }
    }

    public SceneNode Source
    {
      get
      {
        return this.StoryboardContainer.ResolveTargetName(this.SourceID, (SceneNode) this);
      }
      set
      {
        if (value == null)
        {
          this.SourceID = (string) null;
        }
        else
        {
          value.EnsureNamed();
          this.SourceID = value.Name;
        }
      }
    }

    public override string PresentationName
    {
      get
      {
        return this.RoutedEvent.Name;
      }
    }

    private DocumentCompositeNode DocumentCompositeNode
    {
      get
      {
        return (DocumentCompositeNode) this.DocumentNode;
      }
    }

    public class ConcreteEventTriggerNodeFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new EventTriggerNode();
      }

      public EventTriggerNode Instantiate(SceneViewModel viewModel)
      {
        return (EventTriggerNode) this.Instantiate(viewModel, PlatformTypes.EventTrigger);
      }
    }
  }
}
