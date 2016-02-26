// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Policies.SelectionParentPolicy
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
using System.Collections.Generic;

namespace Microsoft.Windows.Design.Policies
{
  public class SelectionParentPolicy : SelectionPolicy
  {
    private Dictionary<ModelItem, ModelItem> _lastParents;
    private Selection _lastSelection;

    protected override IEnumerable<ModelItem> GetPolicyItems(Selection selection)
    {
      if (selection == this._lastSelection)
        return (IEnumerable<ModelItem>) this._lastParents.Keys;
      Dictionary<ModelItem, ModelItem> dictionary = new Dictionary<ModelItem, ModelItem>();
      foreach (ModelItem modelItem in selection.SelectedObjects)
      {
        ModelItem parent = modelItem.Parent;
        if (parent != null && this.IsInPolicy(selection, modelItem, parent))
          dictionary[parent] = parent;
      }
      this._lastParents = dictionary;
      this._lastSelection = selection;
      return (IEnumerable<ModelItem>) dictionary.Keys;
    }

    protected virtual bool IsInPolicy(Selection selection, ModelItem item, ModelItem parent)
    {
      return true;
    }
  }
}
