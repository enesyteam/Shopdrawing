// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Policies.PolicyItemsChangedEventArgs
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design.Model;
using System;
using System.Collections.Generic;

namespace Microsoft.Windows.Design.Policies
{
  public class PolicyItemsChangedEventArgs : EventArgs
  {
    private ItemPolicy _policy;
    private IEnumerable<ModelItem> _itemsAdded;
    private IEnumerable<ModelItem> _itemsRemoved;

    public IEnumerable<ModelItem> ItemsAdded
    {
      get
      {
        return this._itemsAdded;
      }
    }

    public IEnumerable<ModelItem> ItemsRemoved
    {
      get
      {
        return this._itemsRemoved;
      }
    }

    public ItemPolicy Policy
    {
      get
      {
        return this._policy;
      }
    }

    public PolicyItemsChangedEventArgs(ItemPolicy policy, IEnumerable<ModelItem> itemsAdded, IEnumerable<ModelItem> itemsRemoved)
    {
      if (policy == null)
        throw new ArgumentNullException("policy");
      this._policy = policy;
      this._itemsAdded = itemsAdded;
      this._itemsRemoved = itemsRemoved;
      if (this._itemsAdded == null)
        this._itemsAdded = (IEnumerable<ModelItem>) new ModelItem[0];
      if (this._itemsRemoved != null)
        return;
      this._itemsRemoved = (IEnumerable<ModelItem>) new ModelItem[0];
    }
  }
}
