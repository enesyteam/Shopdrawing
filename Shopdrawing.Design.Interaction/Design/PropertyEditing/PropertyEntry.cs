// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.PropertyEditing.PropertyEntry
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.Windows.Design.PropertyEditing
{
  public abstract class PropertyEntry : INotifyPropertyChanged, IPropertyFilterTarget
  {
    private bool _matchesFilter = true;
    private PropertyValue _parentValue;
    private PropertyValue _value;

    public abstract string PropertyName { get; }

    public virtual string DisplayName
    {
      get
      {
        return this.PropertyName;
      }
    }

    public abstract Type PropertyType { get; }

    public abstract PropertyIdentifier Identifier { get; }

    public abstract string CategoryName { get; }

    public abstract string Description { get; }

    protected virtual bool HasStandardValues
    {
      get
      {
        ICollection standardValues = this.StandardValues;
        if (standardValues != null)
          return standardValues.Count > 0;
        return false;
      }
    }

    internal bool HasStandardValuesInternal
    {
      get
      {
        return this.HasStandardValues;
      }
    }

    public abstract bool IsReadOnly { get; }

    public abstract bool IsAdvanced { get; }

    public abstract ICollection StandardValues { get; }

    public abstract PropertyValueEditor PropertyValueEditor { get; }

    public PropertyValue ParentValue
    {
      get
      {
        return this._parentValue;
      }
    }

    public virtual PropertyValue PropertyValue
    {
      get
      {
        if (this._value == null)
          this._value = this.CreatePropertyValueInstance();
        return this._value;
      }
    }

    public abstract EditingContext Context { get; }

    public abstract IEnumerable<ModelProperty> ModelProperties { get; }

    public bool MatchesFilter
    {
      get
      {
        return this._matchesFilter;
      }
      protected set
      {
        if (value == this._matchesFilter)
          return;
        this._matchesFilter = value;
        this.OnPropertyChanged("MatchesFilter");
      }
    }

    public event EventHandler<PropertyFilterAppliedEventArgs> FilterApplied;

    public event PropertyChangedEventHandler PropertyChanged;

    protected PropertyEntry()
      : this((PropertyValue) null)
    {
    }

    protected PropertyEntry(PropertyValue parentValue)
    {
      this._parentValue = parentValue;
    }

    protected abstract PropertyValue CreatePropertyValueInstance();

    public virtual bool MatchesPredicate(PropertyFilterPredicate predicate)
    {
      if (predicate == null)
        return false;
      if (!predicate.Match(this.DisplayName))
        return predicate.Match(this.PropertyType.Name);
      return true;
    }

    public virtual void ApplyFilter(PropertyFilter filter)
    {
      this.MatchesFilter = filter == null || filter.Match((IPropertyFilterTarget) this);
      this.OnFilterApplied(filter);
    }

    protected virtual void OnFilterApplied(PropertyFilter filter)
    {
      if (this.FilterApplied == null)
        return;
      this.FilterApplied((object) this, new PropertyFilterAppliedEventArgs(filter));
    }

    protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
    {
      if (e == null)
        throw new ArgumentNullException("e");
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, e);
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
      if (propertyName == null)
        throw new ArgumentNullException("propertyName");
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
