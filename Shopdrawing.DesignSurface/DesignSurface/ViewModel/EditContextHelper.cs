// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.EditContextHelper
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public static class EditContextHelper
  {
    public static DocumentNodePath SelectedElementPathInEditContext(SceneViewModel viewModel, EditContext editContext, bool returnMultiSelectionAsNull)
    {
      SceneElement sceneElement = (SceneElement) null;
      if (!returnMultiSelectionAsNull || viewModel.ActiveEditContext != editContext || viewModel.ElementSelectionSet.Count == 1)
        sceneElement = viewModel.ElementSelectionSet.PrimarySelection;
      DocumentNodePath nodePath = (DocumentNodePath) null;
      if (viewModel.ActiveEditContext != editContext || sceneElement != null && !editContext.EditingContainerPath.Equals((object) sceneElement.DocumentNodePath.GetContainerNodePath()))
        nodePath = editContext.LastPrimarySelectedPath;
      if (nodePath == null && sceneElement != null)
        nodePath = sceneElement.DocumentNodePath;
      if (nodePath != null)
        return viewModel.GetAncestorInEditingContainer(nodePath, editContext.EditingContainerPath, (DocumentNodePath) null);
      return (DocumentNodePath) null;
    }
  }
}
