// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Selection.AnnotationSelectionSet
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.Selection
{
  public class AnnotationSelectionSet : SelectionSet<AnnotationSceneNode, MarkerBasedSceneNodeCollection<AnnotationSceneNode>>
  {
    public override bool IsExclusive
    {
      get
      {
        return true;
      }
    }

    public AnnotationSelectionSet(SceneViewModel viewModel)
      : base(viewModel, (ISelectionSetNamingHelper) new AnnotationSelectionSet.AnnotationNamingHelper(), (SelectionSet<AnnotationSceneNode, MarkerBasedSceneNodeCollection<AnnotationSceneNode>>.IStorageProvider) new SceneNodeSelectionSetStorageProvider<AnnotationSceneNode>(viewModel))
    {
    }

    private class AnnotationNamingHelper : ISelectionSetNamingHelper
    {
      public string Name
      {
        get
        {
          return StringTable.UndoUnitAnnotationName;
        }
      }

      public string GetUndoString(object obj)
      {
        AnnotationSceneNode annotationSceneNode = obj as AnnotationSceneNode;
        if (annotationSceneNode != null)
          return annotationSceneNode.Type.Name;
        return "";
      }
    }
  }
}
