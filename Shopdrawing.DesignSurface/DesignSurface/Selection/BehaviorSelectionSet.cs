// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Selection.BehaviorSelectionSet
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.Selection
{
  public class BehaviorSelectionSet : SelectionSet<BehaviorBaseNode, MarkerBasedSceneNodeCollection<BehaviorBaseNode>>
  {
    public BehaviorSelectionSet(SceneViewModel viewModel)
      : base(viewModel, (ISelectionSetNamingHelper) new BehaviorSelectionSet.BehaviorNamingHelper(), (SelectionSet<BehaviorBaseNode, MarkerBasedSceneNodeCollection<BehaviorBaseNode>>.IStorageProvider) new SceneNodeSelectionSetStorageProvider<BehaviorBaseNode>(viewModel))
    {
    }

    private class BehaviorNamingHelper : ISelectionSetNamingHelper
    {
      public string Name
      {
        get
        {
          return StringTable.UndoUnitBehaviorName;
        }
      }

      public string GetUndoString(object obj)
      {
        BehaviorBaseNode behaviorBaseNode = obj as BehaviorBaseNode;
        if (behaviorBaseNode != null)
          return behaviorBaseNode.Type.Name;
        return "";
      }
    }
  }
}
