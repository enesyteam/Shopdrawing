// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.ControllableStoryboardActionNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public abstract class ControllableStoryboardActionNode : TimelineActionNode
  {
    public static readonly IPropertyId BeginStoryboardNameProperty = (IPropertyId) PlatformTypes.ControllableStoryboardAction.GetMember(MemberType.LocalProperty, "BeginStoryboardName", MemberAccessTypes.Public);

    public string BeginActionName
    {
      get
      {
        return DocumentPrimitiveNode.GetValueAsString(this.DocumentCompositeNode.Properties[ControllableStoryboardActionNode.BeginStoryboardNameProperty]);
      }
      set
      {
        this.DocumentCompositeNode.Properties[ControllableStoryboardActionNode.BeginStoryboardNameProperty] = (DocumentNode) this.DocumentContext.CreateNode(value);
      }
    }

    public override StoryboardTimelineSceneNode TargetTimeline
    {
      get
      {
        string str = (string) this.GetLocalValue(DesignTimeProperties.StoryboardNameProperty);
        if (!string.IsNullOrEmpty(str))
        {
          foreach (StoryboardTimelineSceneNode timelineSceneNode in this.ViewModel.AnimationEditor.EnumerateStoryboardsForContainer(this.StoryboardContainer))
          {
            if (timelineSceneNode.Name == str)
              return timelineSceneNode;
          }
        }
        return (StoryboardTimelineSceneNode) null;
      }
      set
      {
        this.SetLocalValue(DesignTimeProperties.StoryboardNameProperty, (object) value.Name);
      }
    }
  }
}
