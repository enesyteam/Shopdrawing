// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Selection.AnimationSelectionSet
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Globalization;

namespace Microsoft.Expression.DesignSurface.Selection
{
  public class AnimationSelectionSet : SelectionSet<AnimationSceneNode, MarkerBasedSceneNodeCollection<AnimationSceneNode>>
  {
    public AnimationSelectionSet(SceneViewModel viewModel)
      : base(viewModel, (ISelectionSetNamingHelper) new AnimationSelectionSet.TimelineNamingHelper(), (SelectionSet<AnimationSceneNode, MarkerBasedSceneNodeCollection<AnimationSceneNode>>.IStorageProvider) new SceneNodeSelectionSetStorageProvider<AnimationSceneNode>(viewModel))
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
        if (!(obj is TimelineSceneNode))
          return "";
        return string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}", new object[1]
        {
          (object) ((SceneNode) obj).TargetType.ToString()
        });
      }
    }
  }
}
