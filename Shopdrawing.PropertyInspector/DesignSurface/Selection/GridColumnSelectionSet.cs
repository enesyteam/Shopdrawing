// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Selection.GridColumnSelectionSet
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Expression.DesignSurface.Selection
{
  public class GridColumnSelectionSet : SelectionSet<ColumnDefinitionNode, MarkerBasedSceneNodeCollection<ColumnDefinitionNode>>
  {
    private bool gridLineMode;

    public bool GridLineMode
    {
      get
      {
        return this.gridLineMode;
      }
      set
      {
        if (this.gridLineMode == value)
          return;
        this.Clear();
        this.gridLineMode = value;
      }
    }

    public GridColumnSelectionSet(SceneViewModel viewModel)
      : base(viewModel, (ISelectionSetNamingHelper) new GridColumnSelectionSet.GridColumnNamingHelper(), (SelectionSet<ColumnDefinitionNode, MarkerBasedSceneNodeCollection<ColumnDefinitionNode>>.IStorageProvider) new SceneNodeSelectionSetStorageProvider<ColumnDefinitionNode>(viewModel))
    {
    }

    public void SetToSelection(ColumnDefinitionNode toSelectTo)
    {
      if (this.PrimarySelection == null)
      {
        this.SetSelection(toSelectTo);
      }
      else
      {
        GridElement gridElement = (GridElement) this.PrimarySelection.Parent;
        int val1 = gridElement.ColumnDefinitions.IndexOf(this.PrimarySelection);
        int val2 = gridElement.ColumnDefinitions.IndexOf(toSelectTo);
        int num1 = Math.Min(val1, val2);
        int num2 = Math.Max(val1, val2);
        MarkerBasedSceneNodeCollection<ColumnDefinitionNode> sceneNodeCollection = this.StorageProvider.NewList();
        for (int index = num1; index <= num2; ++index)
        {
          if (index != val1)
            sceneNodeCollection.Add(gridElement.ColumnDefinitions[index]);
        }
        sceneNodeCollection.Add(this.PrimarySelection);
        this.SetSelection((ICollection<ColumnDefinitionNode>) sceneNodeCollection, this.PrimarySelection);
      }
    }

    private class GridColumnNamingHelper : ISelectionSetNamingHelper
    {
      public string Name
      {
        get
        {
          return StringTable.UndoUnitGridColumnName;
        }
      }

      public string GetUndoString(object obj)
      {
        ColumnDefinitionNode columnDefinitionNode = obj as ColumnDefinitionNode;
        if (columnDefinitionNode == null)
          return "";
        GridElement gridElement = (GridElement) columnDefinitionNode.Parent;
        return string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0} {1}", new object[2]
        {
          (object) gridElement.Name,
          (object) gridElement.ColumnDefinitions.IndexOf(columnDefinitionNode)
        });
      }
    }
  }
}
