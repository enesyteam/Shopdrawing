// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.MultipleSelectionContext`1
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class MultipleSelectionContext<T> : SelectionContext<T> where T : class, ISelectable
  {
    private List<T> selection;

    public override T PrimarySelection
    {
      get
      {
        if (this.selection.Count <= 0)
          return default (T);
        return this.selection[this.selection.Count - 1];
      }
      set
      {
        if ((object) value == null)
          throw new InvalidOperationException();
        int index = this.selection.IndexOf(value);
        if (index == -1)
        {
          this.Add(value);
        }
        else
        {
          this.selection[index] = this.selection[this.selection.Count - 1];
          this.selection[this.selection.Count - 1] = value;
          this.OnPropertyChanged("PrimarySelection");
        }
      }
    }

    public override int Count
    {
      get
      {
        return this.selection.Count;
      }
    }

    public MultipleSelectionContext()
    {
      this.selection = new List<T>();
    }

    public override void SetSelection(T item)
    {
      T primarySelection = this.PrimarySelection;
      List<T> list = this.selection;
      this.selection = new List<T>();
      this.selection.Add(item);
      foreach (T obj in list)
      {
        if ((object) obj != (object) item)
          obj.IsSelected = false;
      }
      if (!list.Contains(item))
        item.IsSelected = true;
      if ((object) primarySelection == (object) this.PrimarySelection)
        return;
      this.OnPropertyChanged("PrimarySelection");
    }

    public override void SetSelection(IEnumerable<T> items)
    {
      T primarySelection = this.PrimarySelection;
      List<T> list = this.selection;
      this.selection = new List<T>(items);
      Dictionary<T, bool> dictionary = new Dictionary<T, bool>();
      foreach (T key in list)
        dictionary.Add(key, true);
      foreach (T key in this.selection)
      {
        if (dictionary.ContainsKey(key))
          dictionary.Remove(key);
        else
          key.IsSelected = true;
      }
      foreach (T obj in dictionary.Keys)
        obj.IsSelected = false;
      if ((object) primarySelection == (object) this.PrimarySelection)
        return;
      this.OnPropertyChanged("PrimarySelection");
    }

    public override void Add(T item)
    {
      if ((object) item == null)
        return;
      this.selection.Add(item);
      item.IsSelected = true;
      this.OnPropertyChanged("PrimarySelection");
    }

    public override void Clear()
    {
      List<T> list = this.selection;
      this.selection = new List<T>();
      foreach (T obj in list)
        obj.IsSelected = false;
      this.OnPropertyChanged("PrimarySelection");
    }

    public override bool Contains(T item)
    {
      return this.selection.Contains(item);
    }

    public override bool Remove(T item)
    {
      bool flag = (object) item == (object) this.PrimarySelection;
      if (!this.selection.Remove(item))
        return false;
      item.IsSelected = false;
      if (flag)
        this.OnPropertyChanged("PrimarySelection");
      return true;
    }

    public override IEnumerator<T> GetEnumerator()
    {
      return (IEnumerator<T>) this.selection.GetEnumerator();
    }
  }
}
