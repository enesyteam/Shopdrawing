// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.BeginActionNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class BeginActionNode : TimelineActionNode
  {
    public static readonly IPropertyId StoryboardProperty = (IPropertyId) PlatformTypes.BeginStoryboard.GetMember(MemberType.LocalProperty, "Storyboard", MemberAccessTypes.Public);
    public static readonly BeginActionNode.ConcreteBeginActionNodeFactory Factory = new BeginActionNode.ConcreteBeginActionNodeFactory();

    public override TimelineOperation TimelineOperation
    {
      get
      {
        return TimelineOperation.Begin;
      }
    }

    public override StoryboardTimelineSceneNode TargetTimeline
    {
      get
      {
        return this.GetLocalValueAsSceneNode(BeginActionNode.StoryboardProperty) as StoryboardTimelineSceneNode;
      }
      set
      {
        using (this.ViewModel.ForceBaseValue())
        {
          if (value.IsInResourceDictionary)
          {
            IDocumentContext documentContext = this.DocumentContext;
            DocumentNode keyNode = ResourceNodeHelper.GetResourceEntryKey(value.Parent.DocumentNode as DocumentCompositeNode).Clone(documentContext);
            this.SetValue(BeginActionNode.StoryboardProperty, (object) DocumentNodeUtilities.NewStaticResourceNode(documentContext, keyNode));
          }
          else
            this.SetValueAsSceneNode(BeginActionNode.StoryboardProperty, (SceneNode) value);
        }
      }
    }

    internal static string TargetNameFromDocumentNode(DocumentCompositeNode node)
    {
      DocumentCompositeNode node1 = node.Properties[BeginActionNode.StoryboardProperty] as DocumentCompositeNode;
      if (node1 != null && node1.Type.IsResource)
      {
        DocumentNode resourceKey = ResourceNodeHelper.GetResourceKey(node1);
        if (resourceKey != null)
          return DocumentPrimitiveNode.GetValueAsString(resourceKey);
      }
      return (string) null;
    }

    public class ConcreteBeginActionNodeFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new BeginActionNode();
      }

      public BeginActionNode Instantiate(SceneViewModel viewModel)
      {
        return (BeginActionNode) this.Instantiate(viewModel, PlatformTypes.BeginStoryboard);
      }
    }
  }
}
