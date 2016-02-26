// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.TimelineActionNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public abstract class TimelineActionNode : TriggerActionNode
  {
    public abstract StoryboardTimelineSceneNode TargetTimeline { get; set; }

    public abstract TimelineOperation TimelineOperation { get; }

    public static TimelineActionNode CreateActionForOperation(TimelineOperation timelineOperation, SceneViewModel sceneViewModel)
    {
      switch (timelineOperation)
      {
        case TimelineOperation.Begin:
          return (TimelineActionNode) BeginActionNode.Factory.Instantiate(sceneViewModel);
        case TimelineOperation.Pause:
          return (TimelineActionNode) PauseActionNode.Factory.Instantiate(sceneViewModel);
        case TimelineOperation.SkipToFill:
          return (TimelineActionNode) SkipToFillActionNode.Factory.Instantiate(sceneViewModel);
        case TimelineOperation.Stop:
          return (TimelineActionNode) StopActionNode.Factory.Instantiate(sceneViewModel);
        case TimelineOperation.Remove:
          return (TimelineActionNode) RemoveActionNode.Factory.Instantiate(sceneViewModel);
        case TimelineOperation.Resume:
          return (TimelineActionNode) ResumeActionNode.Factory.Instantiate(sceneViewModel);
        default:
          return (TimelineActionNode) null;
      }
    }
  }
}
