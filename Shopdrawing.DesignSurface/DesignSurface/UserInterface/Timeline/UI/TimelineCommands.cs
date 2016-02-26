// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Timeline.UI.TimelineCommands
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.UserInterface.Timeline.UI
{
  public static class TimelineCommands
  {
    private static RoutedCommand applyKeyFrameEditsCommand = new RoutedCommand(StringTable.ApplyKeyFrameEditsCommandName, typeof (TimelineCommands));
    private static RoutedCommand editSelectedKeyFramesCommand = new RoutedCommand(StringTable.EditSelectedKeyFramesCommandName, typeof (TimelineCommands));
    private static RoutedCommand addExplicitKeyFrameCommand = new RoutedCommand(StringTable.AddExplicitKeyFrameCommandName, typeof (TimelineCommands));
    private static RoutedCommand seekBackCommand = new RoutedCommand(StringTable.SeekBackCommandName, typeof (TimelineCommands));
    private static RoutedCommand seekForwardCommand = new RoutedCommand(StringTable.SeekForwardCommandName, typeof (TimelineCommands));
    private static RoutedCommand seekBeginCommand = new RoutedCommand(StringTable.SeekBeginCommandName, typeof (TimelineCommands));
    private static RoutedCommand seekEndCommand = new RoutedCommand(StringTable.SeekEndCommandName, typeof (TimelineCommands));
    private static RoutedCommand createNewTimelineCommand = new RoutedCommand(StringTable.CreateNewTimelineCommandName, typeof (TimelineCommands));
    private static RoutedCommand deleteTimelineCommand = new RoutedCommand(StringTable.DeleteTimelineCommandName, typeof (TimelineCommands));
    private static RoutedCommand showSnapResolutionCommand = new RoutedCommand(StringTable.ShowSnappingResolutionCommand, typeof (TimelineCommands));
    private static RoutedCommand zoomInCommand = new RoutedCommand(StringTable.ZoomInTimelineCommandName, typeof (TimelineCommands));
    private static RoutedCommand zoomOutCommand = new RoutedCommand(StringTable.ZoomOutTimelineCommandName, typeof (TimelineCommands));
    private static RoutedCommand toggleLockAllCommand = new RoutedCommand(StringTable.ToggleLockAllCommandName, typeof (TimelineCommands));
    private static RoutedCommand toggleShowAllCommand = new RoutedCommand(StringTable.ToggleShowAllCommandName, typeof (TimelineCommands));
    private static RoutedCommand scopeToRootCommand = new RoutedCommand(StringTable.ScopeToRootCommandName, typeof (TimelineCommands));

    public static RoutedCommand ApplyKeyFrameEditsCommand
    {
      get
      {
        return TimelineCommands.applyKeyFrameEditsCommand;
      }
    }

    public static RoutedCommand EditSelectedKeyFramesCommand
    {
      get
      {
        return TimelineCommands.editSelectedKeyFramesCommand;
      }
    }

    public static RoutedCommand AddExplicitKeyFrameCommand
    {
      get
      {
        return TimelineCommands.addExplicitKeyFrameCommand;
      }
    }

    public static RoutedCommand SeekBackCommand
    {
      get
      {
        return TimelineCommands.seekBackCommand;
      }
    }

    public static RoutedCommand SeekForwardCommand
    {
      get
      {
        return TimelineCommands.seekForwardCommand;
      }
    }

    public static RoutedCommand SeekBeginCommand
    {
      get
      {
        return TimelineCommands.seekBeginCommand;
      }
    }

    public static RoutedCommand SeekEndCommand
    {
      get
      {
        return TimelineCommands.seekEndCommand;
      }
    }

    public static RoutedCommand CreateNewTimelineCommand
    {
      get
      {
        return TimelineCommands.createNewTimelineCommand;
      }
    }

    public static RoutedCommand DeleteTimelineCommand
    {
      get
      {
        return TimelineCommands.deleteTimelineCommand;
      }
    }

    public static RoutedCommand ShowSnapResolutionCommand
    {
      get
      {
        return TimelineCommands.showSnapResolutionCommand;
      }
    }

    public static RoutedCommand ZoomInCommand
    {
      get
      {
        return TimelineCommands.zoomInCommand;
      }
    }

    public static RoutedCommand ZoomOutCommand
    {
      get
      {
        return TimelineCommands.zoomOutCommand;
      }
    }

    public static RoutedCommand ToggleLockAllCommand
    {
      get
      {
        return TimelineCommands.toggleLockAllCommand;
      }
    }

    public static RoutedCommand ToggleShowAllCommand
    {
      get
      {
        return TimelineCommands.toggleShowAllCommand;
      }
    }

    public static RoutedCommand ScopeToRootCommand
    {
      get
      {
        return TimelineCommands.scopeToRootCommand;
      }
    }
  }
}
