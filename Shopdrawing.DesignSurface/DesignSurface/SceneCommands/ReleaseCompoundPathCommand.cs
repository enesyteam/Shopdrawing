// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.ReleaseCompoundPathCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Selection;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal sealed class ReleaseCompoundPathCommand : SceneCommandBase
  {
    public override bool IsEnabled
    {
      get
      {
        if (!base.IsEnabled)
          return false;
        bool flag = false;
        foreach (SceneElement sceneElement in this.SceneViewModel.ElementSelectionSet.Selection)
        {
          PathElement pathElement = sceneElement as PathElement;
          int figureCount = ReleaseCompoundPathCommand.GetFigureCount(pathElement);
          if (figureCount > 1)
          {
            ISceneNodeCollection<SceneNode> collectionContainer = pathElement.GetCollectionContainer();
            if (collectionContainer.FixedCapacity.HasValue && collectionContainer.FixedCapacity.Value < collectionContainer.Count - 1 + figureCount)
              return false;
            flag = true;
          }
        }
        return flag;
      }
    }

    public ReleaseCompoundPathCommand(SceneViewModel viewModel)
      : base(viewModel)
    {
    }

    private static int GetFigureCount(PathElement pathElement)
    {
      if (pathElement == null || !pathElement.IsViewObjectValid)
        return 0;
      IPlatformTypes metadata = pathElement.Platform.Metadata;
      if (!PlatformTypes.IsInstance(pathElement.GetLocalValue(PathElement.DataProperty), PlatformTypes.PathGeometry, metadata.DefaultTypeResolver))
        return pathElement.PathGeometry.Figures.Count;
      object obj = new PropertyReference(new List<ReferenceStep>()
      {
        metadata.ResolveProperty(PathElement.DataProperty) as ReferenceStep,
        metadata.ResolveProperty(PathElement.FiguresProperty) as ReferenceStep,
        metadata.ResolveProperty(PathElement.PathFigureCollectionCountProperty) as ReferenceStep
      }).GetValue(pathElement.ViewObject.PlatformSpecificObject);
      if (obj is int)
        return (int) obj;
      return 0;
    }

    public override void Execute()
    {
      using (SceneEditTransaction editTransaction = this.SceneViewModel.CreateEditTransaction(StringTable.UndoUnitReleaseCompoundPath))
      {
        SceneElementSelectionSet elementSelectionSet = this.SceneViewModel.ElementSelectionSet;
        ICollection<SceneElement> collection = (ICollection<SceneElement>) elementSelectionSet.Selection;
        elementSelectionSet.Clear();
        SceneElementCollection elementCollection = new SceneElementCollection();
        foreach (SceneElement sceneElement in (IEnumerable<SceneElement>) collection)
        {
          PathElement pathElement1 = sceneElement as PathElement;
          if (ReleaseCompoundPathCommand.GetFigureCount(pathElement1) > 1)
          {
            foreach (PathElement pathElement2 in PathCommandHelper.ReleaseCompoundPaths(pathElement1, editTransaction))
              elementCollection.Add((SceneElement) pathElement2);
          }
          else
            elementCollection.Add(sceneElement);
        }
        elementSelectionSet.SetSelection((ICollection<SceneElement>) elementCollection, (SceneElement) null);
        editTransaction.Commit();
      }
    }
  }
}
