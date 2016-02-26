// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Selection.SubpartSelectionSet`2
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Expression.DesignSurface.Selection
{
  public class SubpartSelectionSet<T, ListOfT> : SelectionSet<T, ListOfT> where T : ISceneElementSubpart where ListOfT : IOrderedList<T>
  {
    private SceneElementSelectionSet sceneElementSelectionSet;

    public override bool IsExclusive
    {
      get
      {
        return false;
      }
    }

    public IEnumerable<SceneElement> ElementSelection
    {
      get
      {
        return (IEnumerable<SceneElement>) this.sceneElementSelectionSet.Selection;
      }
    }

    public SubpartSelectionSet(SceneViewModel viewModel, ISelectionSetNamingHelper namingHelper, SceneElementSelectionSet sceneElementSelectionSet, SelectionSet<T, ListOfT>.IStorageProvider storageProvider)
      : base(viewModel, namingHelper, storageProvider)
    {
      this.sceneElementSelectionSet = sceneElementSelectionSet;
      this.sceneElementSelectionSet.Changing += new EventHandler<SelectionSetChangingEventArgs<SceneElement>>(this.SceneElementSelectionSet_Changing);
    }

    public void Clear(bool elementSelection)
    {
      this.Clear();
      if (!elementSelection)
        return;
      this.SetElementSelection();
    }

    public void SetSelection(T selectionToSet, bool elementSelection)
    {
      this.SetSelection(selectionToSet);
      if (!elementSelection)
        return;
      this.SetElementSelection();
    }

    public void SetSelection(ICollection<T> selectionToSet, bool elementSelection)
    {
      this.SetSelection(selectionToSet, default (T));
      if (!elementSelection)
        return;
      this.SetElementSelection();
    }

    public void ExtendSelection(T selectionToExtend, bool elementSelection)
    {
      this.ExtendSelection(selectionToExtend);
      if (!elementSelection)
        return;
      this.ExtendElementSelection();
    }

    public void ExtendSelection(ICollection<T> selectionToExtend, bool elementSelection)
    {
      this.ExtendSelection(selectionToExtend);
      if (!elementSelection)
        return;
      this.ExtendElementSelection();
    }

    public void ToggleSelection(T selectionToToggle, bool elementSelection)
    {
      this.ToggleSelection(selectionToToggle);
      if (!elementSelection)
        return;
      this.ExtendElementSelection();
    }

    public void ToggleSelection(ICollection<T> selectionToToggle, bool elementSelection)
    {
      this.ToggleSelection(selectionToToggle);
      if (!elementSelection)
        return;
      this.ExtendElementSelection();
    }

    public void RemoveSelection(T selectionToRemove, bool elementSelection)
    {
      this.RemoveSelection(selectionToRemove);
    }

    public void RemoveSelection(ICollection<T> selectionToRemove, bool elementSelection)
    {
      this.RemoveSelection(selectionToRemove);
    }

    private void ExtendElementSelection()
    {
      List<SceneElement> list = new List<SceneElement>();
      foreach (T obj in this.Selection)
      {
        ISceneElementSubpart sceneElementSubpart = (ISceneElementSubpart) obj;
        if (!this.sceneElementSelectionSet.IsSelected(sceneElementSubpart.SceneElement) && !list.Contains(sceneElementSubpart.SceneElement))
          list.Add(sceneElementSubpart.SceneElement);
      }
      if (list.Count <= 0)
        return;
      this.sceneElementSelectionSet.ExtendSelection((ICollection<SceneElement>) list);
    }

    private void SetElementSelection()
    {
      List<SceneElement> list = new List<SceneElement>();
      foreach (T obj in this.Selection)
      {
        ISceneElementSubpart sceneElementSubpart = (ISceneElementSubpart) obj;
        if (!list.Contains(sceneElementSubpart.SceneElement) && sceneElementSubpart.SceneElement != null)
          list.Add(sceneElementSubpart.SceneElement);
      }
      if (list.Count > 0)
        this.sceneElementSelectionSet.SetSelection((ICollection<SceneElement>) list, (SceneElement) null);
      else
        this.sceneElementSelectionSet.Clear();
    }

    private void SceneElementSelectionSet_Changing(object sender, SelectionSetChangingEventArgs<SceneElement> args)
    {
      ReadOnlyCollection<SceneElement> newSelection = args.NewSelection;
      List<T> list = (List<T>) null;
      if (newSelection.Count == 0 && this.InternalSelection.Count != 0)
      {
        list = new List<T>((IEnumerable<T>) this.InternalSelection);
      }
      else
      {
        using (IEnumerator<T> enumerator = this.InternalSelection.GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            T current = enumerator.Current;
            if (!newSelection.Contains(current.SceneElement))
            {
              if (list == null)
                list = new List<T>();
              list.Add(current);
            }
          }
        }
      }
      if (list == null)
        return;
      this.RemoveSelection((ICollection<T>) list);
    }

    protected class SceneElementSubpartComparer : IComparer<T>
    {
      public int Compare(T x, T y)
      {
        return x.CompareTo((object) y);
      }
    }
  }
}
