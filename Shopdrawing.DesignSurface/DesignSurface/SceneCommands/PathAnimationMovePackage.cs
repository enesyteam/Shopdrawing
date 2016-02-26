// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.PathAnimationMovePackage
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.Generic;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  public class PathAnimationMovePackage
  {
    private List<KeyValuePair<StoryboardTimelineSceneNode, TimelineSceneNode>> toRemove = new List<KeyValuePair<StoryboardTimelineSceneNode, TimelineSceneNode>>();
    private List<KeyValuePair<StoryboardTimelineSceneNode, TimelineSceneNode>> toAdd = new List<KeyValuePair<StoryboardTimelineSceneNode, TimelineSceneNode>>();

    public void Remove(StoryboardTimelineSceneNode parent, TimelineSceneNode timeline)
    {
      this.toRemove.Add(new KeyValuePair<StoryboardTimelineSceneNode, TimelineSceneNode>(parent, timeline));
    }

    public void Add(StoryboardTimelineSceneNode parent, TimelineSceneNode timeline)
    {
      this.toAdd.Add(new KeyValuePair<StoryboardTimelineSceneNode, TimelineSceneNode>(parent, timeline));
    }

    public void AddAndRemove()
    {
      this.Remove();
      this.Add();
    }

    public void Remove()
    {
      foreach (KeyValuePair<StoryboardTimelineSceneNode, TimelineSceneNode> keyValuePair in this.toRemove)
        keyValuePair.Key.Children.Remove(keyValuePair.Value);
      this.toRemove.Clear();
    }

    public void Add()
    {
      foreach (KeyValuePair<StoryboardTimelineSceneNode, TimelineSceneNode> keyValuePair in this.toAdd)
        keyValuePair.Key.Children.Add(keyValuePair.Value);
      this.toAdd.Clear();
    }

    public void ApplyTransformToNewAnimations(Transform transform)
    {
      foreach (KeyValuePair<StoryboardTimelineSceneNode, TimelineSceneNode> keyValuePair in this.toAdd)
        PathCommandHelper.ApplyTransformToAnimation(keyValuePair.Value, transform);
    }
  }
}
