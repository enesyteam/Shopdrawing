// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Selection.ChildPropertySelectionSet
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.Selection
{
  public class ChildPropertySelectionSet : SelectionSet<SceneNode, MarkerBasedSceneNodeCollection<SceneNode>>
  {
    public override bool IsExclusive
    {
      get
      {
        return true;
      }
    }

    public ChildPropertySelectionSet(SceneViewModel viewModel)
      : base(viewModel, (ISelectionSetNamingHelper) new ChildPropertySelectionSet.PropertyNamingHelper(), (SelectionSet<SceneNode, MarkerBasedSceneNodeCollection<SceneNode>>.IStorageProvider) new SceneNodeSelectionSetStorageProvider<SceneNode>(viewModel))
    {
    }

    private class PropertyNamingHelper : ISelectionSetNamingHelper
    {
      public string Name
      {
        get
        {
          return StringTable.UndoUnitPropertyName;
        }
      }

      public string GetUndoString(object obj)
      {
        return "";
      }
    }
  }
}
