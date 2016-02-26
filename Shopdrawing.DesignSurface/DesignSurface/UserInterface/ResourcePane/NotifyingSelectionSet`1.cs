// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.ResourcePane.NotifyingSelectionSet`1
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Selection;
using Microsoft.Expression.DesignSurface.UserInterface.DataPane;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Data;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.UserInterface.ResourcePane
{
  public class NotifyingSelectionSet<T> : SelectionSet<T, OrderedList<T>> where T : ISelectable
  {
    public NotifyingSelectionSet(System.Collections.Generic.Comparer<T> comparer)
      : base((SceneViewModel) null, (ISelectionSetNamingHelper) new NotifyingSelectionSet<T>.NullNamingHelper(), (SelectionSet<T, OrderedList<T>>.IStorageProvider) new BasicSelectionSetStorageProvider<T>((IComparer<T>) comparer))
    {
    }

    protected override void SetSelectionInternal(OrderedList<T> newSelection, T newPrimarySelection)
    {
      this.NotifyItemsChanged(this.InternalSelection, newSelection);
      this.SetSelectionInternal(newSelection, newPrimarySelection);
    }

    protected override void SetSelectionWithUndo(string description, OrderedList<T> newSelection, T newPrimarySelection)
    {
      this.NotifyItemsChanged(this.InternalSelection, newSelection);
      this.SetSelectionWithUndo(description, newSelection, newPrimarySelection);
    }

    private void NotifyItemsChanged(OrderedList<T> selection, OrderedList<T> newSelection)
    {
      foreach (T obj in (List<T>) selection)
      {
        if (!newSelection.Contains(obj))
          obj.IsSelected = false;
      }
      foreach (T obj in (List<T>) newSelection)
      {
        if (!selection.Contains(obj))
          obj.IsSelected = true;
      }
    }

    public IList<S> GetFilteredSelection<S>() where S : T
    {
      List<S> list = new List<S>();
      foreach (T obj in this.Selection)
      {
        if ((object) obj is S)
          list.Add((S) (object) obj);
      }
      return (IList<S>) list;
    }

    private class NullNamingHelper : ISelectionSetNamingHelper
    {
      public string Name
      {
        get
        {
          return string.Empty;
        }
      }

      public string GetUndoString(object obj)
      {
        return string.Empty;
      }
    }
  }
}
