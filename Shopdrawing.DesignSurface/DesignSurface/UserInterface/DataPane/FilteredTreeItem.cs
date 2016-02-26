// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.FilteredTreeItem
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework.Controls;
using System;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public abstract class FilteredTreeItem : VirtualizingTreeItem<FilteredTreeItem>
  {
    private Predicate<FilteredTreeItem> filter;

    public virtual bool MatchesFilter
    {
      get
      {
        if (this.Filter != null)
          return this.Filter(this);
        return true;
      }
    }

    public virtual Predicate<FilteredTreeItem> Filter
    {
      get
      {
        return this.filter;
      }
      set
      {
        this.filter = value;
        this.OnFilterChanged();
        this.OnPropertyChanged("IsVisible");
      }
    }

    public virtual void OnFilterChanged()
    {
    }
  }
}
