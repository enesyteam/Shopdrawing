// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Selection.SelectionSet`2
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Commands.Undo;
using Microsoft.Expression.Framework.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;

namespace Microsoft.Expression.DesignSurface.Selection
{
  public class SelectionSet<T, ListOfT> : ISelectionSet, ISelectionSet<T> where ListOfT : IOrderedList<T>
  {
    private ISelectionSetNamingHelper namingHelper;
    private ISceneEditTransactionFactory editTransactionFactory;
    private SelectionSet<T, ListOfT>.IStorageProvider storageProvider;
    private ListOfT selection;
    private int indexOfPrimarySelection;
    private uint changeStamp;
    private SceneViewModel viewModel;

    public bool IsEmpty
    {
      get
      {
        return this.Count == 0;
      }
    }

    public int Count
    {
      get
      {
        return this.selection.Count;
      }
    }

    public uint ChangeStamp
    {
      get
      {
        return this.changeStamp;
      }
    }

    public T PrimarySelection
    {
      get
      {
        if (this.indexOfPrimarySelection >= 0 && this.indexOfPrimarySelection < this.Count)
          return this.selection[this.indexOfPrimarySelection];
        return default (T);
      }
    }

    public ReadOnlyCollection<T> Selection
    {
      get
      {
        return new ReadOnlyCollection<T>((IList<T>) this.selection);
      }
    }

    protected ListOfT InternalSelection
    {
      get
      {
        return this.selection;
      }
    }

    internal SceneViewModel ViewModel
    {
      get
      {
        return this.viewModel;
      }
    }

    public virtual bool IsExclusive
    {
      get
      {
        return true;
      }
    }

    protected SelectionSet<T, ListOfT>.IStorageProvider StorageProvider
    {
      get
      {
        return this.storageProvider;
      }
    }

    public ISceneEditTransactionFactory EditTransactionFactory
    {
      get
      {
        return this.editTransactionFactory;
      }
    }

    protected ISelectionSetNamingHelper NamingHelper
    {
      get
      {
        return this.namingHelper;
      }
    }

    public event EventHandler Changed;

    public event EventHandler<SelectionSetChangingEventArgs<T>> Changing;

    public SelectionSet(SceneViewModel model, ISelectionSetNamingHelper namingHelper, SelectionSet<T, ListOfT>.IStorageProvider storageProvider)
    {
      this.viewModel = model;
      this.editTransactionFactory = this.viewModel != null ? (ISceneEditTransactionFactory) this.viewModel.Document : (ISceneEditTransactionFactory) null;
      this.namingHelper = namingHelper;
      this.storageProvider = storageProvider;
      this.selection = storageProvider.NewList();
      this.indexOfPrimarySelection = -1;
      this.changeStamp = 0U;
    }

    public bool IsSelected(T item)
    {
      return this.selection.IndexOf(item) >= 0;
    }

    public void Clear()
    {
      if (this.IsEmpty)
        return;
      this.SetSelectionWithUndo(StringTable.UndoUnitClearSelection, this.storageProvider.NewList(), default (T));
    }

    public virtual void Canonicalize(SceneUpdateTypeFlags flags)
    {
      T primarySelection = this.PrimarySelection;
      ListOfT newSelection = this.storageProvider.CanonicalizeList(this.selection, flags);
      if ((object) newSelection == null)
        return;
      this.SetSelectionWithUndo(StringTable.UndoUnitRemoveDeletedNodesFromSelection, newSelection, primarySelection);
    }

    public void SetSelection(T selectionToSet)
    {
      if (this.Count == 1 && this.selection[0].Equals((object) selectionToSet))
        return;
      ListOfT newSelection = this.storageProvider.NewList();
      newSelection.Add(selectionToSet);
      this.SetSelectionWithUndo(StringTable.UndoUnitSetSelection, newSelection, selectionToSet);
    }

    public void SetSelection(ICollection<T> selectionToSet, T primarySelection)
    {
      bool flag = false;
      if (selectionToSet.Count != this.Count)
      {
        flag = true;
      }
      else
      {
        foreach (T obj in (IEnumerable<T>) selectionToSet)
        {
          if (!this.IsSelected(obj))
          {
            flag = true;
            break;
          }
        }
      }
      if (flag)
      {
        ListOfT newSelection = this.storageProvider.CopyList(selectionToSet);
        T newPrimarySelection = primarySelection;
        if (object.Equals((object) newPrimarySelection, (object) default (T)))
          newPrimarySelection = newSelection.Count > 0 ? newSelection[newSelection.Count - 1] : default (T);
        this.SetSelectionWithUndo(StringTable.UndoUnitSetSelection, newSelection, newPrimarySelection);
      }
      else
      {
        T newPrimarySelection = primarySelection;
        if (object.Equals((object) newPrimarySelection, (object) default (T)))
          newPrimarySelection = this.selection.Count > 0 ? this.selection[this.selection.Count - 1] : default (T);
        if (object.Equals((object) newPrimarySelection, (object) this.PrimarySelection))
          return;
        this.SetSelectionWithUndo(StringTable.UndoUnitSetSelection, this.selection, newPrimarySelection);
      }
    }

    public void ExtendSelection(T selectionToExtend)
    {
      int num = this.selection.BinarySearch(selectionToExtend);
      if (num >= 0)
        return;
      ListOfT newSelection = this.storageProvider.CopyList((ICollection<T>) this.selection);
      newSelection.Insert(~num, selectionToExtend);
      this.SetSelectionWithUndo(StringTable.UndoUnitExtendSelection, newSelection, selectionToExtend);
    }

    public void ExtendSelection(ICollection<T> selectionToExtend)
    {
      if (selectionToExtend == null || selectionToExtend.Count == 0)
        return;
      ListOfT newSelection = this.storageProvider.CopyList((ICollection<T>) this.selection);
      bool flag = false;
      T newPrimarySelection = default (T);
      foreach (T obj in (IEnumerable<T>) selectionToExtend)
      {
        int num = newSelection.BinarySearch(obj);
        if (num < 0)
        {
          newPrimarySelection = obj;
          newSelection.Insert(~num, obj);
          flag = true;
        }
      }
      if (!flag)
        return;
      this.SetSelectionWithUndo(StringTable.UndoUnitExtendSelection, newSelection, newPrimarySelection);
    }

    public void ToggleSelection(T selectionToToggle)
    {
      ListOfT newSelection = this.storageProvider.CopyList((ICollection<T>) this.selection);
      int index = newSelection.BinarySearch(selectionToToggle);
      T newPrimarySelection = this.PrimarySelection;
      if (index >= 0)
      {
        newSelection.RemoveAt(index);
        if (object.Equals((object) newPrimarySelection, (object) selectionToToggle))
          newPrimarySelection = default (T);
      }
      else
      {
        newSelection.Insert(~index, selectionToToggle);
        newPrimarySelection = selectionToToggle;
      }
      this.SetSelectionWithUndo(StringTable.UndoUnitToggleSelection, newSelection, newPrimarySelection);
    }

    public void ToggleSelection(ICollection<T> selectionToToggle)
    {
      if (selectionToToggle == null || selectionToToggle.Count == 0)
        return;
      ListOfT newSelection = this.storageProvider.CopyList((ICollection<T>) this.selection);
      T newPrimarySelection = this.PrimarySelection;
      foreach (T obj in (IEnumerable<T>) selectionToToggle)
      {
        int index = newSelection.BinarySearch(obj);
        if (index >= 0)
        {
          newSelection.RemoveAt(index);
          if (object.Equals((object) newPrimarySelection, (object) obj))
            newPrimarySelection = default (T);
        }
        else
        {
          newSelection.Insert(~index, obj);
          newPrimarySelection = obj;
        }
      }
      this.SetSelectionWithUndo(StringTable.UndoUnitToggleSelection, newSelection, newPrimarySelection);
    }

    public void RemoveSelection(T selectionToRemove)
    {
      int index = this.selection.BinarySearch(selectionToRemove);
      if (index < 0)
        return;
      ListOfT newSelection = this.storageProvider.CopyList((ICollection<T>) this.selection);
      T newPrimarySelection = this.PrimarySelection;
      newSelection.RemoveAt(index);
      if (object.Equals((object) newPrimarySelection, (object) selectionToRemove))
        newPrimarySelection = default (T);
      this.SetSelectionWithUndo(StringTable.UndoUnitRemoveSelection, newSelection, newPrimarySelection);
    }

    public void RemoveSelection(ICollection<T> selectionToRemove)
    {
      if (selectionToRemove == null || selectionToRemove.Count == 0)
        return;
      ListOfT newSelection = this.storageProvider.CopyList((ICollection<T>) this.selection);
      T newPrimarySelection = this.PrimarySelection;
      bool flag = false;
      foreach (T obj in (IEnumerable<T>) selectionToRemove)
      {
        if (!object.Equals((object) obj, (object) default (T)))
        {
          int index = newSelection.BinarySearch(obj);
          if (index >= 0)
          {
            newSelection.RemoveAt(index);
            if (object.Equals((object) newPrimarySelection, (object) obj))
              newPrimarySelection = default (T);
            flag = true;
          }
        }
      }
      if (!flag)
        return;
      this.SetSelectionWithUndo(StringTable.UndoUnitRemoveSelection, newSelection, newPrimarySelection);
    }

    internal void Rebroadcast()
    {
      this.OnSelectionChanged();
    }

    protected virtual void SetSelectionWithUndo(string description, ListOfT newSelection, T newPrimarySelection)
    {
      this.OnSelectionChanging((IList<T>) newSelection);
      int primarySelection = this.GetIndexOfPrimarySelection(newSelection, newPrimarySelection);
      if (this.editTransactionFactory != null)
      {
        description = string.Format((IFormatProvider) CultureInfo.CurrentCulture, description, new object[1]
        {
          (object) this.namingHelper.Name
        });
        using (SceneEditTransaction editTransaction = this.editTransactionFactory.CreateEditTransaction(description, true))
        {
          editTransaction.AddUndoUnit((IUndoUnit) new SelectionSet<T, ListOfT>.SelectionChangeUndoUnit(this, newSelection, primarySelection));
          editTransaction.Commit();
        }
      }
      else
        this.SetSelectionInternal(newSelection, primarySelection);
      if (this.ViewModel == null || !this.IsExclusive || newSelection.Count <= 0)
        return;
      this.ViewModel.EnforceExclusiveSelection((ISelectionSet) this);
    }

    protected virtual void SetSelectionInternal(ListOfT newSelection, T newPrimarySelection)
    {
      if ((object) newPrimarySelection != null)
        object.Equals((object) newPrimarySelection, (object) default (T));
      int primarySelection = this.GetIndexOfPrimarySelection(newSelection, newPrimarySelection);
      this.SetSelectionInternal(newSelection, primarySelection);
    }

    private void SetSelectionInternal(ListOfT newSelection, int newIndexOfPrimarySelection)
    {
      this.selection = newSelection;
      this.indexOfPrimarySelection = newIndexOfPrimarySelection;
      this.OnSelectionChanged();
    }

    private int GetIndexOfPrimarySelection(ListOfT newSelection, T newPrimarySelection)
    {
      int num = newSelection.IndexOf(newPrimarySelection);
      if (num < 0)
        num = newSelection.Count - 1;
      return num;
    }

    protected void OnSelectionChanged()
    {
      ++this.changeStamp;
      if (this.Changed == null)
        return;
      this.Changed((object) this, EventArgs.Empty);
    }

    protected void OnSelectionChanging(IList<T> newSelection)
    {
      if (this.Changing == null)
        return;
      this.Changing((object) this, new SelectionSetChangingEventArgs<T>(new ReadOnlyCollection<T>(newSelection)));
    }

    public interface IStorageProvider
    {
      ListOfT NewList();

      ListOfT CopyList(ICollection<T> set);

      ListOfT CanonicalizeList(ListOfT list, SceneUpdateTypeFlags flags);
    }

    private class SelectionChangeUndoUnit : UndoUnit
    {
      private SelectionSet<T, ListOfT> selectionSet;
      private ListOfT selection;
      private int indexOfPrimarySelection;

      public override bool IsHidden
      {
        get
        {
          return true;
        }
      }

      public override bool AllowsDeepMerge
      {
        get
        {
          return true;
        }
      }

      public SelectionChangeUndoUnit(SelectionSet<T, ListOfT> selectionSet, ListOfT selection, int indexOfPrimarySelection)
      {
        this.selectionSet = selectionSet;
        this.selection = selection;
        this.indexOfPrimarySelection = indexOfPrimarySelection;
      }

      public override void Undo()
      {
        base.Undo();
        this.Toggle();
      }

      public override void Redo()
      {
        base.Redo();
        this.Toggle();
      }

      public override UndoUnitMergeResult Merge(IUndoUnit otherUnit, out IUndoUnit mergedUnit)
      {
        SelectionSet<T, ListOfT>.SelectionChangeUndoUnit selectionChangeUndoUnit = otherUnit as SelectionSet<T, ListOfT>.SelectionChangeUndoUnit;
        if (selectionChangeUndoUnit != null && selectionChangeUndoUnit.selectionSet == this.selectionSet)
        {
          mergedUnit = (IUndoUnit) new SelectionSet<T, ListOfT>.SelectionChangeUndoUnit(this.selectionSet, this.selection, this.indexOfPrimarySelection);
          return UndoUnitMergeResult.MergedIntoOneUnit;
        }
        mergedUnit = (IUndoUnit) null;
        return UndoUnitMergeResult.CouldNotMerge;
      }

      private void Toggle()
      {
        ListOfT newSelection = this.selection;
        this.selection = this.selectionSet.selection;
        int newIndexOfPrimarySelection = this.indexOfPrimarySelection;
        this.indexOfPrimarySelection = this.selectionSet.indexOfPrimarySelection;
        this.selectionSet.SetSelectionInternal(newSelection, newIndexOfPrimarySelection);
      }

      public override string ToString()
      {
        StringWriter stringWriter = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture);
        stringWriter.Write("SelectionChange(");
        if ((object) this.selection == null)
        {
          stringWriter.Write("null");
        }
        else
        {
          stringWriter.Write("{");
          for (int index = 0; index < this.selection.Count; ++index)
          {
            if (index > 0)
              stringWriter.Write(", ");
            T obj = this.selection[index];
            if ((object) obj == null)
            {
              stringWriter.Write("null");
            }
            else
            {
              stringWriter.Write("\"");
              stringWriter.Write(this.selectionSet.namingHelper.GetUndoString((object) obj));
              stringWriter.Write("\"");
            }
          }
          stringWriter.Write("}");
        }
        stringWriter.Write(")");
        return stringWriter.ToString();
      }
    }
  }
}
