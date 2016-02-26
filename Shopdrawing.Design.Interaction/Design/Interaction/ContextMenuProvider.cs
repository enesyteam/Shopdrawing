// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Interaction.ContextMenuProvider
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Features;
using MS.Internal.Features;
using System;
using System.Collections.ObjectModel;

namespace Microsoft.Windows.Design.Interaction
{
  [FeatureConnector(typeof (ContextMenuFeatureConnector))]
  public abstract class ContextMenuProvider : FeatureProvider
  {
    private ObservableCollection<MenuBase> _items;

    public ObservableCollection<MenuBase> Items
    {
      get
      {
        if (this._items == null)
          this._items = new ObservableCollection<MenuBase>();
        return this._items;
      }
    }

    public event EventHandler<MenuActionEventArgs> UpdateItemStatus;

    public void Update(EditingContext context)
    {
      if (this.UpdateItemStatus != null)
        this.UpdateItemStatus((object) this, new MenuActionEventArgs(context));
      this.AssignContext(context, this.Items);
    }

    private void AssignContext(EditingContext context, ObservableCollection<MenuBase> items)
    {
      foreach (MenuBase menuBase in (Collection<MenuBase>) items)
      {
        menuBase.Context = context;
        MenuGroup menuGroup = menuBase as MenuGroup;
        if (menuGroup != null)
          this.AssignContext(context, menuGroup.Items);
      }
    }
  }
}
