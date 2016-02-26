// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Policies.SelectionPolicy
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
using System.Collections.Generic;

namespace Microsoft.Windows.Design.Policies
{
  [RequiresContextItem(typeof (Selection))]
  [RequiresContextItem(typeof (Tool))]
  public class SelectionPolicy : ItemPolicy
  {
    private List<ModelItem> _currentItems = new List<ModelItem>();

    public override IEnumerable<ModelItem> PolicyItems
    {
      get
      {
        return (IEnumerable<ModelItem>) this._currentItems;
      }
    }

    protected virtual IEnumerable<ModelItem> GetPolicyItems(Selection selection)
    {
      if (selection != null)
      {
        foreach (ModelItem modelItem in selection.SelectedObjects)
        {
          if (this.IsInPolicy(selection, modelItem))
            yield return modelItem;
        }
      }
    }

    protected virtual bool IsInPolicy(Selection selection, ModelItem item)
    {
      return true;
    }

    protected override void OnActivated()
    {
      this.Context.Items.Subscribe<Selection>(new SubscribeContextCallback<Selection>(this.OnSelectionChanged));
    }

    protected override void OnDeactivated()
    {
      this.Context.Items.Unsubscribe<Selection>(new SubscribeContextCallback<Selection>(this.OnSelectionChanged));
    }

    private void OnSelectionChanged(Selection newSelection)
    {
      IEnumerable<ModelItem> enumerable = (IEnumerable<ModelItem>) this._currentItems;
      IEnumerable<ModelItem> policyItems = this.GetPolicyItems(newSelection);
      Dictionary<ModelItem, ModelItem> dictionary = new Dictionary<ModelItem, ModelItem>(this._currentItems.Count);
      List<ModelItem> list = new List<ModelItem>(newSelection.SelectionCount);
      foreach (ModelItem index in enumerable)
        dictionary[index] = index;
      this._currentItems.Clear();
      foreach (ModelItem key in policyItems)
      {
        this._currentItems.Add(key);
        if (dictionary.ContainsKey(key))
          dictionary.Remove(key);
        else
          list.Add(key);
      }
      if (dictionary.Count == 0 && list.Count == 0)
        return;
      this.OnPolicyItemsChanged(new PolicyItemsChangedEventArgs((ItemPolicy) this, (IEnumerable<ModelItem>) list, (IEnumerable<ModelItem>) dictionary.Values));
    }
  }
}
