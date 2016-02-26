// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Selection.SceneNodeSelectionSetStorageProvider`1
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.Selection
{
  internal class SceneNodeSelectionSetStorageProvider<T> : SelectionSet<T, MarkerBasedSceneNodeCollection<T>>.IStorageProvider where T : SceneNode
  {
    private SceneViewModel viewModel;

    public SceneNodeSelectionSetStorageProvider(SceneViewModel viewModel)
    {
      this.viewModel = viewModel;
    }

    public MarkerBasedSceneNodeCollection<T> NewList()
    {
      return new MarkerBasedSceneNodeCollection<T>(this.viewModel);
    }

    public MarkerBasedSceneNodeCollection<T> CopyList(ICollection<T> set)
    {
      return new MarkerBasedSceneNodeCollection<T>(this.viewModel, set);
    }

    public MarkerBasedSceneNodeCollection<T> CanonicalizeList(MarkerBasedSceneNodeCollection<T> list, SceneUpdateTypeFlags flags)
    {
      if (flags != SceneUpdateTypeFlags.UndoRedo)
        return list.FilterOutDeletedMarkers();
      list.RestoreDeletedMarkers();
      return (MarkerBasedSceneNodeCollection<T>) null;
    }
  }
}
