// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Policies.ItemPolicy
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Model;
using MS.Internal.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Windows.Design.Policies
{
  public abstract class ItemPolicy
  {
    private static ModelItem[] _emptyItems = new ModelItem[0];
    private EditingContext _context;

    protected EditingContext Context
    {
      get
      {
        if (this._context == null)
          throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_ObjectNotActive, new object[1]
          {
            (object) this.GetType().Name
          }));
        return this._context;
      }
    }

    public virtual bool IsSurrogate
    {
      get
      {
        return false;
      }
    }

    public abstract IEnumerable<ModelItem> PolicyItems { get; }

    public event EventHandler<PolicyItemsChangedEventArgs> PolicyItemsChanged;

    public virtual IEnumerable<ModelItem> GetSurrogateItems(ModelItem item)
    {
      return (IEnumerable<ModelItem>) ItemPolicy._emptyItems;
    }

    internal void Activate(EditingContext context)
    {
      if (context == null)
        throw new ArgumentNullException("context");
      if (this._context != null)
        throw new InvalidOperationException(Resources.Error_ObjectAlreadyActive);
      this._context = context;
      this.OnActivated();
    }

    protected abstract void OnActivated();

    internal void Deactivate()
    {
      this.OnDeactivated();
    }

    protected virtual void OnDeactivated()
    {
    }

    protected virtual void OnPolicyItemsChanged(PolicyItemsChangedEventArgs e)
    {
      if (this.PolicyItemsChanged == null)
        return;
      this.PolicyItemsChanged((object) this, e);
    }
  }
}
