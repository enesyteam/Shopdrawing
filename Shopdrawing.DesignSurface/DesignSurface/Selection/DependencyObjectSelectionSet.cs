// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Selection.DependencyObjectSelectionSet
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Expression.DesignSurface.Selection
{
  public class DependencyObjectSelectionSet : SelectionSet<SceneNode, MarkerBasedSceneNodeCollection<SceneNode>>
  {
    public override bool IsExclusive
    {
      get
      {
        return true;
      }
    }

    public event EventHandler SelectionChangedOutsideUndo;

    public DependencyObjectSelectionSet(SceneViewModel viewModel)
      : base(viewModel, (ISelectionSetNamingHelper) new DependencyObjectSelectionSet.DepObjNamingHelper(), (SelectionSet<SceneNode, MarkerBasedSceneNodeCollection<SceneNode>>.IStorageProvider) new SceneNodeSelectionSetStorageProvider<SceneNode>(viewModel))
    {
    }

    protected override void SetSelectionWithUndo(string description, MarkerBasedSceneNodeCollection<SceneNode> newSelection, SceneNode newPrimarySelection)
    {
      SceneNode containerForSelection = this.ViewModel.GetEditingContainerForSelection<SceneNode>((ICollection<SceneNode>) newSelection);
      List<SceneNode> removedElements;
      newSelection = this.StorageProvider.CopyList((ICollection<SceneNode>) this.ViewModel.GetSelectionForEditingContainer<SceneNode>(containerForSelection, (ICollection<SceneNode>) newSelection, out removedElements));
      if (removedElements.Contains(newPrimarySelection))
        newPrimarySelection = newSelection.Count > 0 ? newSelection[newSelection.Count - 1] : (SceneNode) null;
      this.OnSelectionChanging((IList<SceneNode>) newSelection);
      if (this.EditTransactionFactory != null)
      {
        description = string.Format((IFormatProvider) CultureInfo.CurrentCulture, description, new object[1]
        {
          (object) this.NamingHelper.Name
        });
        using (SceneEditTransaction editTransaction = this.EditTransactionFactory.CreateEditTransaction(description, true))
        {
          this.SetSelectionWithUndo(description, newSelection, newPrimarySelection);
          if (containerForSelection != null)
            this.ViewModel.ActiveEditingContainerPath = containerForSelection.DocumentNodePath;
          editTransaction.Commit();
        }
      }
      else
        this.SetSelectionInternal(newSelection, newPrimarySelection);
      this.OnSelectionChangedOutsideUndo();
    }

    private void OnSelectionChangedOutsideUndo()
    {
      if (this.SelectionChangedOutsideUndo == null)
        return;
      this.SelectionChangedOutsideUndo((object) this, EventArgs.Empty);
    }

    private class DepObjNamingHelper : ISelectionSetNamingHelper
    {
      public string Name
      {
        get
        {
          return StringTable.UndoUnitSceneElementName;
        }
      }

      public string GetUndoString(object obj)
      {
        return ((SceneNode) obj).Name;
      }
    }
  }
}
