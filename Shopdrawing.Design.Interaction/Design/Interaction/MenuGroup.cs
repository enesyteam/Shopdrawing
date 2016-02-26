// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Interaction.MenuGroup
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using System;
using System.Collections.ObjectModel;

namespace Microsoft.Windows.Design.Interaction
{
  public class MenuGroup : MenuBase
  {
    private ObservableCollection<MenuBase> _items;
    private bool _hasDropDown;

    public ObservableCollection<MenuBase> Items
    {
      get
      {
        if (this._items == null)
          this._items = new ObservableCollection<MenuBase>();
        return this._items;
      }
    }

    public bool HasDropDown
    {
      get
      {
        return this._hasDropDown;
      }
      set
      {
        if (this._hasDropDown == value)
          return;
        this._hasDropDown = value;
        this.OnPropertyChanged("HasDropDown");
      }
    }

    public MenuGroup(string groupName)
      : this(groupName, groupName)
    {
    }

    public MenuGroup(string groupName, string displayName)
    {
      if (string.IsNullOrEmpty(groupName))
        throw new ArgumentNullException("groupName");
      this.Name = groupName;
      this.DisplayName = displayName;
    }
  }
}
