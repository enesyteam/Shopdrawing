// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Timeline.MediaTimelineItem
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Data;
using System.Windows.Controls;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.UserInterface.Timeline
{
  public class MediaTimelineItem : ScheduledTimelineItem
  {
    private SceneElement mediaElement;

    public override string DisplayName
    {
      get
      {
        return StringTable.MediaTimelineItemDisplayName;
      }
      set
      {
      }
    }

    public SceneElement MediaElement
    {
      get
      {
        return this.mediaElement;
      }
    }

    public override ICommand EditRepeatCountCommand
    {
      get
      {
        return (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(((ScheduledTimelineItem) this).EditRepeatCount));
      }
    }

    public MediaTimelineItem(TimelineItemManager timelineItemManager, SceneElement mediaElement, StoryboardTimelineSceneNode parentTimeline, MediaTimelineSceneNode mediaTimelineSceneNode)
      : base(timelineItemManager, parentTimeline, (TimelineSceneNode) mediaTimelineSceneNode)
    {
      this.mediaElement = mediaElement;
    }

    public override int CompareTo(TimelineItem timelineItem)
    {
      return 0;
    }

    public static MediaTimelineItem FindItem(TimelineItem parentTimelineItem, StoryboardTimelineSceneNode parentTimeline, SceneElement mediaElement)
    {
      for (int index = 0; index < parentTimelineItem.Children.Count; ++index)
      {
        MediaTimelineItem mediaTimelineItem = parentTimelineItem.Children[index] as MediaTimelineItem;
        if (mediaTimelineItem != null && mediaTimelineItem.ParentTimeline == parentTimeline && mediaTimelineItem.MediaElement == mediaElement)
          return mediaTimelineItem;
      }
      return (MediaTimelineItem) null;
    }

    protected override ContextMenu BuildContextMenu()
    {
      ContextMenu contextMenu = new ContextMenu();
      MenuItem menuItem = new MenuItem();
      menuItem.Command = this.EditRepeatCountCommand;
      menuItem.Header = (object) StringTable.EditRepeatCountCommandName;
      menuItem.Name = "EditRepeatCount";
      contextMenu.Items.Add((object) menuItem);
      return contextMenu;
    }
  }
}
