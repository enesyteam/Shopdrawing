// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.PropertyEditing.CategoryEntry
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.Windows.Design.PropertyEditing
{
  public abstract class CategoryEntry : INotifyPropertyChanged, IPropertyFilterTarget
  {
    private string _name;
    private bool _matchesFilter;

    public string CategoryName
    {
      get
      {
        return this._name;
      }
    }

    public abstract IEnumerable<PropertyEntry> Properties { get; }

    public abstract PropertyEntry this[string propertyName] { get; }

    public virtual bool MatchesFilter
    {
      get
      {
        return this._matchesFilter;
      }
      protected set
      {
        if (this._matchesFilter == value)
          return;
        this._matchesFilter = value;
        this.OnPropertyChanged("MatchesFilter");
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public event EventHandler<PropertyFilterAppliedEventArgs> FilterApplied;

    protected CategoryEntry(string name)
    {
      if (string.IsNullOrEmpty(name))
        throw new ArgumentNullException("name");
      this._name = name;
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
      if (propertyName == null)
        throw new ArgumentNullException("propertyName");
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }

    protected virtual void OnFilterApplied(PropertyFilter filter)
    {
      if (this.FilterApplied == null)
        return;
      this.FilterApplied(this, new PropertyFilterAppliedEventArgs(filter));
    }

    public virtual void ApplyFilter(PropertyFilter filter)
    {
      this.MatchesFilter = filter == null || filter.Match((IPropertyFilterTarget) this);
      this.OnFilterApplied(filter);
    }

    public abstract bool MatchesPredicate(PropertyFilterPredicate predicate);
  }
}
