// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Selection.StoryboardSelectionSet
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Globalization;

namespace Microsoft.Expression.DesignSurface.Selection
{
  public class StoryboardSelectionSet : SelectionSet<StoryboardTimelineSceneNode, MarkerBasedSceneNodeCollection<StoryboardTimelineSceneNode>>
  {
    public StoryboardSelectionSet(SceneViewModel viewModel)
      : base(viewModel, (ISelectionSetNamingHelper) new StoryboardSelectionSet.TimelineNamingHelper(), (SelectionSet<StoryboardTimelineSceneNode, MarkerBasedSceneNodeCollection<StoryboardTimelineSceneNode>>.IStorageProvider) new SceneNodeSelectionSetStorageProvider<StoryboardTimelineSceneNode>(viewModel))
    {
    }

    private class TimelineNamingHelper : ISelectionSetNamingHelper
    {
      public string Name
      {
        get
        {
          return StringTable.UndoUnitTimelineName;
        }
      }

      public string GetUndoString(object obj)
      {
        StoryboardTimelineSceneNode timelineSceneNode = obj as StoryboardTimelineSceneNode;
        if (timelineSceneNode == null)
          return "";
        return string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}", new object[1]
        {
          (object) timelineSceneNode.TargetType.ToString()
        });
      }
    }
  }
}
