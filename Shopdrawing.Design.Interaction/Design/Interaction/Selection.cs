// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Interaction.Selection
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Windows.Design.Interaction
{
  public class Selection : ContextItem
  {
    private ICollection<ModelItem> _selectedObjects;
    private Selection _viewSelection;

    public Selection ViewSelection
    {
      get
      {
        if (this._viewSelection == null)
        {
          bool flag = false;
          if (this._selectedObjects.Count > 0)
          {
            foreach (ModelItem modelItem in (IEnumerable<ModelItem>) this._selectedObjects)
            {
              if (modelItem.View == (ViewItem) null)
              {
                flag = true;
                break;
              }
            }
          }
          if (flag)
          {
            HashSet<ModelItem> hashSet = new HashSet<ModelItem>();
            List<ModelItem> list = new List<ModelItem>(this._selectedObjects.Count);
            foreach (ModelItem modelItem1 in (IEnumerable<ModelItem>) this._selectedObjects)
            {
              ModelItem modelItem2 = modelItem1;
              while (modelItem2 != null && modelItem2.View == (ViewItem) null)
                modelItem2 = modelItem2.Parent;
              if (modelItem2 != null && !hashSet.Contains(modelItem2))
              {
                hashSet.Add(modelItem2);
                list.Add(modelItem2);
              }
            }
            this._viewSelection = new Selection((IEnumerable<ModelItem>) list);
          }
          else
            this._viewSelection = this;
        }
        return this._viewSelection;
      }
    }

    public ModelItem PrimarySelection
    {
      get
      {
        using (IEnumerator<ModelItem> enumerator = this._selectedObjects.GetEnumerator())
        {
          if (enumerator.MoveNext())
            return enumerator.Current;
        }
        return (ModelItem) null;
      }
    }

    public IEnumerable<ModelItem> SelectedObjects
    {
      get
      {
        return (IEnumerable<ModelItem>) this._selectedObjects;
      }
    }

    public int SelectionCount
    {
      get
      {
        return this._selectedObjects.Count;
      }
    }

    public override sealed Type ItemType
    {
      get
      {
        return typeof (Selection);
      }
    }

    public Selection()
    {
      this._selectedObjects = (ICollection<ModelItem>) new ModelItem[0];
    }

    public Selection(IEnumerable<ModelItem> selectedObjects)
    {
      if (selectedObjects == null)
        throw new ArgumentNullException("selectedObjects");
      List<ModelItem> list = new List<ModelItem>();
      list.AddRange(selectedObjects);
      this._selectedObjects = (ICollection<ModelItem>) list;
    }

    public Selection(IEnumerable<ModelItem> selectedObjects, Predicate<ModelItem> match)
    {
      if (selectedObjects == null)
        throw new ArgumentNullException("selectedObjects");
      if (match == null)
        throw new ArgumentNullException("match");
      List<ModelItem> list = new List<ModelItem>();
      foreach (ModelItem modelItem in selectedObjects)
      {
        if (match(modelItem))
          list.Add(modelItem);
      }
      this._selectedObjects = (ICollection<ModelItem>) list;
    }

    public Selection(IEnumerable selectedObjects)
    {
      if (selectedObjects == null)
        throw new ArgumentNullException("selectedObjects");
      List<ModelItem> list = new List<ModelItem>();
      foreach (object obj in selectedObjects)
      {
        ModelItem modelItem = obj as ModelItem;
        if (modelItem != null)
          list.Add(modelItem);
      }
      this._selectedObjects = (ICollection<ModelItem>) list;
    }

    public Selection(IEnumerable selectedObjects, Predicate<ModelItem> match)
    {
      if (selectedObjects == null)
        throw new ArgumentNullException("selectedObjects");
      if (match == null)
        throw new ArgumentNullException("match");
      List<ModelItem> list = new List<ModelItem>();
      foreach (object obj in selectedObjects)
      {
        ModelItem modelItem = obj as ModelItem;
        if (modelItem != null && match(modelItem))
          list.Add(modelItem);
      }
      this._selectedObjects = (ICollection<ModelItem>) list;
    }

    public Selection(params ModelItem[] selectedObjects)
      : this((IEnumerable<ModelItem>) selectedObjects)
    {
    }
  }
}
