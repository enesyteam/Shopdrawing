// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Selection.GridRowSelectionSet
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Expression.DesignSurface.Selection
{
  public class GridRowSelectionSet : SelectionSet<RowDefinitionNode, MarkerBasedSceneNodeCollection<RowDefinitionNode>>
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

    public GridRowSelectionSet(SceneViewModel viewModel)
      : base(viewModel, (ISelectionSetNamingHelper) new GridRowSelectionSet.GridRowNamingHelper(), (SelectionSet<RowDefinitionNode, MarkerBasedSceneNodeCollection<RowDefinitionNode>>.IStorageProvider) new SceneNodeSelectionSetStorageProvider<RowDefinitionNode>(viewModel))
    {
    }

    public void SetToSelection(RowDefinitionNode toSelectTo)
    {
      if (this.PrimarySelection == null)
      {
        this.SetSelection(toSelectTo);
      }
      else
      {
        GridElement gridElement = (GridElement) this.PrimarySelection.Parent;
        int val1 = gridElement.RowDefinitions.IndexOf(this.PrimarySelection);
        int val2 = gridElement.RowDefinitions.IndexOf(toSelectTo);
        int num1 = Math.Min(val1, val2);
        int num2 = Math.Max(val1, val2);
        List<RowDefinitionNode> list = new List<RowDefinitionNode>();
        for (int index = num1; index <= num2; ++index)
        {
          if (index != val1)
            list.Add(gridElement.RowDefinitions[index]);
        }
        list.Add(this.PrimarySelection);
        this.SetSelection((ICollection<RowDefinitionNode>) list, this.PrimarySelection);
      }
    }

    private class GridRowNamingHelper : ISelectionSetNamingHelper
    {
      public string Name
      {
        get
        {
          return StringTable.UndoUnitGridRowName;
        }
      }

      public string GetUndoString(object obj)
      {
        RowDefinitionNode rowDefinitionNode = obj as RowDefinitionNode;
        if (rowDefinitionNode == null)
          return "";
        GridElement gridElement = (GridElement) rowDefinitionNode.Parent;
        return string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0} {1}", new object[2]
        {
          (object) gridElement.Name,
          (object) gridElement.RowDefinitions.IndexOf(rowDefinitionNode)
        });
      }
    }
  }
}
