// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Selection.SceneElementSelectionSet
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
  public class SceneElementSelectionSet : SelectionSet<SceneElement, MarkerBasedSceneNodeCollection<SceneElement>>
  {
    public event EventHandler SelectionChangedOutsideUndo;

    public SceneElementSelectionSet(SceneViewModel viewModel)
      : base(viewModel, (ISelectionSetNamingHelper) new SceneElementSelectionSet.SceneElementNamingHelper(), (SelectionSet<SceneElement, MarkerBasedSceneNodeCollection<SceneElement>>.IStorageProvider) new SceneNodeSelectionSetStorageProvider<SceneElement>(viewModel))
    {
    }

    public ICollection<T> GetFilteredSelection<T>() where T : SceneElement
    {
      List<T> list = new List<T>();
      foreach (SceneElement sceneElement in this.Selection)
      {
        T obj = sceneElement as T;
        if ((object) obj != null)
          list.Add(obj);
      }
      return (ICollection<T>) list;
    }

    protected override void SetSelectionWithUndo(string description, MarkerBasedSceneNodeCollection<SceneElement> newSelection, SceneElement newPrimarySelection)
    {
      SceneNode containerForSelection = this.ViewModel.GetEditingContainerForSelection<SceneElement>((ICollection<SceneElement>) newSelection);
      List<SceneElement> removedElements;
      newSelection = this.StorageProvider.CopyList((ICollection<SceneElement>) this.ViewModel.GetSelectionForEditingContainer<SceneElement>(containerForSelection, (ICollection<SceneElement>) newSelection, out removedElements));
      if (removedElements.Contains(newPrimarySelection))
        newPrimarySelection = newSelection.Count > 0 ? newSelection[newSelection.Count - 1] : (SceneElement) null;
      this.OnSelectionChanging((IList<SceneElement>) newSelection);
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

    private class SceneElementNamingHelper : ISelectionSetNamingHelper
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
