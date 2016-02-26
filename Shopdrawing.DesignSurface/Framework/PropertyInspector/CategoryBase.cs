// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.PropertyInspector.CategoryBase
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework.Data;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.Framework.PropertyInspector
{
  public abstract class CategoryBase : CategoryEntry, IEnumerable<PropertyEntry>, IEnumerable
  {
    private static CategoryBase.AlphabeticalCategoryEditorComparer alphabeticalCategoryEditorComparer = new CategoryBase.AlphabeticalCategoryEditorComparer();
    private bool basicPropertyMatchesFilter = true;
    private bool advancedPropertyMatchesFilter = true;
    private ObservableCollectionWorkaround<CategoryEditor> categoryEditors = new ObservableCollectionWorkaround<CategoryEditor>();
    private bool expanded;
    private CategoryEditor iconProvider;
    private ImageSource categoryIcon;

    public override IEnumerable<PropertyEntry> Properties
    {
      get
      {
        return (IEnumerable<PropertyEntry>) this;
      }
    }

    public abstract ObservableCollection<PropertyEntry> BasicProperties { get; }

    public abstract ObservableCollection<PropertyEntry> AdvancedProperties { get; }

    public override PropertyEntry this[string propertyName]
    {
      get
      {
        foreach (PropertyEntry propertyEntry in this.Properties)
        {
          if (propertyEntry.PropertyName == propertyName)
            return propertyEntry;
        }
        return (PropertyEntry) null;
      }
    }

    public bool BasicPropertyMatchesFilter
    {
      get
      {
        return this.basicPropertyMatchesFilter;
      }
      set
      {
        if (this.basicPropertyMatchesFilter == value)
          return;
        this.basicPropertyMatchesFilter = value;
        this.OnPropertyChanged("BasicPropertyMatchesFilter");
      }
    }

    public bool AdvancedPropertyMatchesFilter
    {
      get
      {
        return this.advancedPropertyMatchesFilter;
      }
      set
      {
        if (this.advancedPropertyMatchesFilter == value)
          return;
        this.advancedPropertyMatchesFilter = value;
        this.OnPropertyChanged("AdvancedPropertyMatchesFilter");
      }
    }

    public virtual IComparable SortOrdering
    {
      get
      {
        return (IComparable) this.CategoryName;
      }
    }

    public ObservableCollection<CategoryEditor> CategoryEditors
    {
      get
      {
        return (ObservableCollection<CategoryEditor>) this.categoryEditors;
      }
    }

    public ImageSource CategoryIcon
    {
      get
      {
        return this.categoryIcon;
      }
      protected set
      {
        this.categoryIcon = value;
        this.OnPropertyChanged("CategoryIcon");
      }
    }

    public bool Expanded
    {
      get
      {
        return this.expanded;
      }
      set
      {
        this.expanded = value;
        this.OnPropertyChanged("Expanded");
      }
    }

    protected CategoryBase(string name)
      : base(name)
    {
      this.categoryEditors.CollectionChanged += new NotifyCollectionChangedEventHandler(this.categoryEditors_CollectionChanged);
      this.InitializeIcons();
    }

    private void categoryEditors_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.InitializeIcons();
    }

    private void InitializeIcons()
    {
      if (this.iconProvider != null && this.categoryEditors.Contains(this.iconProvider))
        return;
      foreach (CategoryEditor categoryEditor in (Collection<CategoryEditor>) this.categoryEditors)
      {
        ImageSource imageSource;
        try
        {
          imageSource = categoryEditor.GetImage(new Size(24.0, 24.0)) as ImageSource;
        }
        catch (Exception ex)
        {
          this.ReportCategoryException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ExceptionStringTable.CategoryIconLoadFailed, new object[2]
          {
            (object) this.CategoryName,
            (object) ex.Message
          }));
          continue;
        }
        if (imageSource != null)
        {
          if (imageSource is ISupportInitialize)
          {
            try
            {
              double height = imageSource.Height;
            }
            catch (InvalidOperationException ex)
            {
              this.ReportCategoryException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ExceptionStringTable.CategoryIconLoadFailed, new object[2]
              {
                (object) this.CategoryName,
                (object) ex.Message
              }));
              continue;
            }
          }
          this.CategoryIcon = imageSource;
          this.iconProvider = categoryEditor;
          return;
        }
      }
      this.CategoryIcon = (ImageSource) null;
      this.iconProvider = (CategoryEditor) null;
    }

    public virtual void ReportCategoryException(string message)
    {
    }

    IEnumerator<PropertyEntry> IEnumerable<PropertyEntry>.GetEnumerator()
    {
      return (IEnumerator<PropertyEntry>) new CategoryBase.PropertyEnumerator(this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) new CategoryBase.PropertyEnumerator(this);
    }

    public void AddCategoryEditor(CategoryEditor categoryEditor)
    {
      int index = this.categoryEditors.BinarySearch(categoryEditor, (IComparer<CategoryEditor>) CategoryBase.alphabeticalCategoryEditorComparer);
      if (index < 0)
        index = ~index;
      this.categoryEditors.Insert(index, categoryEditor);
    }

    public void RemoveCategoryEditor(Type categoryEditor)
    {
      for (int index = 0; index < this.CategoryEditors.Count; ++index)
      {
        if (this.CategoryEditors[index].GetType() == categoryEditor)
        {
          this.CategoryEditors.RemoveAt(index);
          break;
        }
      }
    }

    public void OnItemsChanged()
    {
      this.OnPropertyChanged("Item[]");
    }

    public virtual bool? QueryExpandedState(bool inspectingUIElement)
    {
      if (!inspectingUIElement)
        return new bool?(true);
      return new bool?();
    }

    public override void ApplyFilter(PropertyFilter filter)
    {
      this.MatchesFilter = filter.Match((IPropertyFilterTarget) this);
      bool flag1 = false;
      bool flag2 = false;
      foreach (PropertyEntry property in (Collection<PropertyEntry>) this.BasicProperties)
      {
        if (this.DoesPropertyMatchFilter(filter, property))
          flag1 = true;
      }
      foreach (PropertyEntry property in (Collection<PropertyEntry>) this.AdvancedProperties)
      {
        if (this.DoesPropertyMatchFilter(filter, property))
          flag2 = true;
      }
      this.BasicPropertyMatchesFilter = flag1;
      this.AdvancedPropertyMatchesFilter = flag2;
      this.OnFilterApplied(filter);
    }

    public override bool MatchesPredicate(PropertyFilterPredicate predicate)
    {
      return predicate.Match(this.CategoryName);
    }

    protected virtual bool DoesPropertyMatchFilter(PropertyFilter filter, PropertyEntry property)
    {
      property.ApplyFilter(filter);
      return property.MatchesFilter;
    }

    private class AlphabeticalCategoryEditorComparer : Comparer<CategoryEditor>
    {
      public override int Compare(CategoryEditor x, CategoryEditor y)
      {
        return x.GetType().ToString().CompareTo(y.GetType().ToString());
      }
    }

    private struct PropertyEnumerator : IEnumerator<PropertyEntry>, IDisposable, IEnumerator
    {
      private CategoryBase category;
      private IEnumerator<PropertyEntry> current;
      private bool enumeratingBasic;

      public PropertyEntry Current
      {
        get
        {
          return this.current.Current;
        }
      }

      object IEnumerator.Current
      {
        get
        {
          return (object) this.Current;
        }
      }

      public PropertyEnumerator(CategoryBase category)
      {
        this.category = category;
        this.current = (IEnumerator<PropertyEntry>) null;
        this.enumeratingBasic = false;
        this.Reset();
      }

      public bool MoveNext()
      {
        if (this.current.MoveNext())
          return true;
        if (!this.enumeratingBasic)
          return false;
        this.enumeratingBasic = false;
        this.current = this.category.AdvancedProperties.GetEnumerator();
        return this.MoveNext();
      }

      public void Reset()
      {
        this.current = this.category.BasicProperties.GetEnumerator();
        this.enumeratingBasic = true;
      }

      void IDisposable.Dispose()
      {
      }
    }
  }
}
