// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Selection.TransitionSelectionSet
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.Selection
{
  public class TransitionSelectionSet : SelectionSet<VisualStateTransitionSceneNode, MarkerBasedSceneNodeCollection<VisualStateTransitionSceneNode>>
  {
    public override bool IsExclusive
    {
      get
      {
        return false;
      }
    }

    public TransitionSelectionSet(SceneViewModel viewModel)
      : base(viewModel, (ISelectionSetNamingHelper) new TransitionSelectionSet.VisualStateTransitionNamingHelper(), (SelectionSet<VisualStateTransitionSceneNode, MarkerBasedSceneNodeCollection<VisualStateTransitionSceneNode>>.IStorageProvider) new SceneNodeSelectionSetStorageProvider<VisualStateTransitionSceneNode>(viewModel))
    {
    }

    private class VisualStateTransitionNamingHelper : ISelectionSetNamingHelper
    {
      public string Name
      {
        get
        {
          return StringTable.UndoUnitVisualStateTransitionName;
        }
      }

      public string GetUndoString(object obj)
      {
        return StringTable.UndoUnitVisualStateTransitionName;
      }
    }
  }
}
