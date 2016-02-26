// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.PropertyInspector.CategoryContainer
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Markup;

namespace Microsoft.Expression.Framework.PropertyInspector
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public class CategoryContainer : ContentControl, IComponentConnector
  {
    public static readonly DependencyProperty PopupHostProperty = DependencyProperty.RegisterAttached("PopupHost", typeof (Popup), typeof (CategoryContainer), (PropertyMetadata) new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(CategoryContainer.OnPopupHostChanged)));
    public static readonly DependencyProperty CategoryProperty = DependencyProperty.Register("Category", typeof (CategoryBase), typeof (CategoryContainer), new PropertyMetadata(null, new PropertyChangedCallback(CategoryContainer.OnCategoryPropertyChanged)));
    public static readonly DependencyProperty AdvancedSectionPinnedProperty = DependencyProperty.Register("AdvancedSectionPinned", typeof (bool), typeof (CategoryContainer), (PropertyMetadata) new FrameworkPropertyMetadata((object) false));
    public static readonly DependencyProperty BasicPropertyMatchesFilterProperty = DependencyProperty.Register("BasicPropertyMatchesFilter", typeof (bool), typeof (CategoryContainer), (PropertyMetadata) new FrameworkPropertyMetadata((object) true));
    public static readonly DependencyProperty AdvancedPropertyMatchesFilterProperty = DependencyProperty.Register("AdvancedPropertyMatchesFilter", typeof (bool), typeof (CategoryContainer), (PropertyMetadata) new FrameworkPropertyMetadata((object) true, new PropertyChangedCallback(CategoryContainer.OnAdvancedPropertyMatchesFilterChanged)));
    public static readonly DependencyProperty ShowAdvancedHeaderProperty = DependencyProperty.Register("ShowAdvancedHeader", typeof (bool), typeof (CategoryContainer), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, FrameworkPropertyMetadataOptions.None, (PropertyChangedCallback) null, new CoerceValueCallback(CategoryContainer.CoerceShowAdvancedHeader)));
    public static readonly DependencyProperty OwningCategoryContainerProperty = DependencyProperty.RegisterAttached("OwningCategoryContainer", typeof (CategoryContainer), typeof (CategoryContainer), (PropertyMetadata) new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));
    public static readonly DependencyProperty ScrollIntoViewWhenLoadedProperty = DependencyProperty.RegisterAttached("ScrollIntoViewWhenLoaded", typeof (bool), typeof (CategoryContainer), (PropertyMetadata) new FrameworkPropertyMetadata(new PropertyChangedCallback(CategoryContainer.OnScrollIntoViewWhenLoadedPropertyChanged)));
    private bool wasExpandedBeforeFilter = true;
    private ObservableCollection<CategoryEditor> basicCategoryEditors = new ObservableCollection<CategoryEditor>();
    private ObservableCollection<CategoryEditor> advancedCategoryEditors = new ObservableCollection<CategoryEditor>();
    private ObservableCollection<PropertyEntry> unconsumedBasicProperties = new ObservableCollection<PropertyEntry>();
    private ObservableCollection<PropertyEntry> unconsumedAdvancedProperties = new ObservableCollection<PropertyEntry>();
    private CategoryContainer.FilterState filterIsEmpty;
    private bool haveCachedExpanded;
    private bool wasAdvancedPinnedBeforeFilter;
    internal CategoryContainer UserControlSelf;
    internal Border PaletteBGrec;
    internal CategoryLayoutContainer _basicEditors;
    internal StandardCategoryLayout _basicProperties;
    internal Expander PopupHost;
    internal CategoryLayoutContainer _advancedEditors;
    internal StandardCategoryLayout _advancedProperties;
    private bool _contentLoaded;

    public ObservableCollection<CategoryEditor> BasicCategoryEditors
    {
      get
      {
        return this.basicCategoryEditors;
      }
    }

    public ObservableCollection<CategoryEditor> AdvancedCategoryEditors
    {
      get
      {
        return this.advancedCategoryEditors;
      }
    }

    public ObservableCollection<PropertyEntry> UnconsumedBasicProperties
    {
      get
      {
        return this.unconsumedBasicProperties;
      }
    }

    public ObservableCollection<PropertyEntry> UnconsumedAdvancedProperties
    {
      get
      {
        return this.unconsumedAdvancedProperties;
      }
    }

    public CategoryBase Category
    {
      get
      {
        return (CategoryBase) this.GetValue(CategoryContainer.CategoryProperty);
      }
      set
      {
        this.SetValue(CategoryContainer.CategoryProperty, value);
      }
    }

    public bool AdvancedSectionPinned
    {
      get
      {
        return (bool) this.GetValue(CategoryContainer.AdvancedSectionPinnedProperty);
      }
      set
      {
        this.SetValue(CategoryContainer.AdvancedSectionPinnedProperty, (object) (bool) (value ? 1 : 0));
      }
    }

    public bool BasicPropertyMatchesFilter
    {
      get
      {
        return (bool) this.GetValue(CategoryContainer.BasicPropertyMatchesFilterProperty);
      }
      set
      {
        this.SetValue(CategoryContainer.BasicPropertyMatchesFilterProperty, (object) (bool) (value ? 1 : 0));
      }
    }

    public bool AdvancedPropertyMatchesFilter
    {
      get
      {
        return (bool) this.GetValue(CategoryContainer.AdvancedPropertyMatchesFilterProperty);
      }
      set
      {
        this.SetValue(CategoryContainer.AdvancedPropertyMatchesFilterProperty, (object) (bool) (value ? 1 : 0));
      }
    }

    public bool ShowAdvancedHeader
    {
      get
      {
        return (bool) this.GetValue(CategoryContainer.ShowAdvancedHeaderProperty);
      }
      set
      {
        this.SetValue(CategoryContainer.ShowAdvancedHeaderProperty, (object) (bool) (value ? 1 : 0));
      }
    }

    public CategoryContainer()
      : this(true)
    {
    }

    public CategoryContainer(bool initializeComponent)
    {
      if (initializeComponent)
        this.InitializeComponent();
      CategoryContainer.SetOwningCategoryContainer((DependencyObject) this, this);
      this.SetBinding(CategoryContainer.BasicPropertyMatchesFilterProperty, (BindingBase) new Binding("Category.BasicPropertyMatchesFilter")
      {
        Source = this,
        Mode = BindingMode.OneWay
      });
      this.SetBinding(CategoryContainer.AdvancedPropertyMatchesFilterProperty, (BindingBase) new Binding("Category.AdvancedPropertyMatchesFilter")
      {
        Source = this,
        Mode = BindingMode.OneWay
      });
    }

    public static void SetOwningCategoryContainer(DependencyObject dependencyObject, CategoryContainer value)
    {
      if (dependencyObject == null)
        throw new ArgumentNullException("dependencyObject");
      dependencyObject.SetValue(CategoryContainer.OwningCategoryContainerProperty, value);
    }

    public static CategoryContainer GetOwningCategoryContainer(DependencyObject dependencyObject)
    {
      if (dependencyObject == null)
        throw new ArgumentNullException("dependencyObject");
      return (CategoryContainer) dependencyObject.GetValue(CategoryContainer.OwningCategoryContainerProperty);
    }

    public static Popup GetPopupHost(DependencyObject target)
    {
      return (Popup) target.GetValue(CategoryContainer.PopupHostProperty);
    }

    public static void SetPopupHost(DependencyObject target, Popup value)
    {
      target.SetValue(CategoryContainer.PopupHostProperty, value);
    }

    private static void OnPopupHostChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      CategoryContainer categoryContainer = d as CategoryContainer;
      if (categoryContainer == null)
        return;
      if (e.NewValue != null)
        categoryContainer.AdvancedSectionPinned = true;
      else
        categoryContainer.AdvancedSectionPinned = false;
    }

    private static void OnCategoryPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      CategoryContainer categoryContainer = (CategoryContainer) d;
      if (e.NewValue == null)
        return;
      CategoryBase categoryBase1 = (CategoryBase) e.NewValue;
      categoryContainer.SetValue(AutomationElement.IdProperty, (object) (categoryBase1.get_CategoryName() + "Category"));
      CategoryBase categoryBase2 = (CategoryBase) e.OldValue;
      if (categoryBase2 != null)
      {
        categoryBase2.remove_FilterApplied(new EventHandler<PropertyFilterAppliedEventArgs>(categoryContainer.OnFilterApplied));
        categoryBase1.CategoryEditors.CollectionChanged -= new NotifyCollectionChangedEventHandler(categoryContainer.CategoryEditors_CollectionChanged);
        categoryContainer.basicCategoryEditors.Clear();
        categoryContainer.advancedCategoryEditors.Clear();
        categoryBase1.BasicProperties.CollectionChanged -= new NotifyCollectionChangedEventHandler(categoryContainer.BasicProperties_CollectionChanged);
        categoryBase1.AdvancedProperties.CollectionChanged -= new NotifyCollectionChangedEventHandler(categoryContainer.AdvancedProperties_CollectionChanged);
        categoryContainer.unconsumedBasicProperties.Clear();
        categoryContainer.unconsumedAdvancedProperties.Clear();
      }
      if (categoryBase1 != null)
      {
        categoryBase1.add_FilterApplied(new EventHandler<PropertyFilterAppliedEventArgs>(categoryContainer.OnFilterApplied));
        categoryContainer.AddCategoryEditors((IList) categoryBase1.CategoryEditors);
        categoryBase1.CategoryEditors.CollectionChanged += new NotifyCollectionChangedEventHandler(categoryContainer.CategoryEditors_CollectionChanged);
        using (IEnumerator<PropertyEntry> enumerator = categoryBase1.BasicProperties.GetEnumerator())
        {
          while (((IEnumerator) enumerator).MoveNext())
          {
            PropertyEntry current = enumerator.Current;
            categoryContainer.AddProperty(current, categoryContainer.unconsumedBasicProperties, categoryContainer.Category.BasicProperties, categoryContainer.basicCategoryEditors);
          }
        }
        using (IEnumerator<PropertyEntry> enumerator = categoryBase1.AdvancedProperties.GetEnumerator())
        {
          while (((IEnumerator) enumerator).MoveNext())
          {
            PropertyEntry current = enumerator.Current;
            categoryContainer.AddProperty(current, categoryContainer.unconsumedAdvancedProperties, categoryContainer.Category.AdvancedProperties, categoryContainer.advancedCategoryEditors);
          }
        }
        categoryBase1.BasicProperties.CollectionChanged += new NotifyCollectionChangedEventHandler(categoryContainer.BasicProperties_CollectionChanged);
        categoryBase1.AdvancedProperties.CollectionChanged += new NotifyCollectionChangedEventHandler(categoryContainer.AdvancedProperties_CollectionChanged);
      }
      categoryContainer.CoerceValue(CategoryContainer.ShowAdvancedHeaderProperty);
    }

    private void AddProperty(PropertyEntry property, ObservableCollection<PropertyEntry> unconsumedProperties, ObservableCollection<PropertyEntry> referenceOrder, ObservableCollection<CategoryEditor> categoryEditors)
    {
      bool flag = false;
      using (IEnumerator<CategoryEditor> enumerator = categoryEditors.GetEnumerator())
      {
        while (((IEnumerator) enumerator).MoveNext())
        {
          if (enumerator.Current.ConsumesProperty(property))
            flag = true;
        }
      }
      if (flag)
        return;
      int index1 = 0;
      int index2;
      for (index2 = 0; referenceOrder[index1] != property && index2 < unconsumedProperties.Count; ++index1)
      {
        if (unconsumedProperties[index2] == referenceOrder[index1])
          ++index2;
      }
      unconsumedProperties.Insert(index2, property);
    }

    private void OnFilterApplied(object source, PropertyFilterAppliedEventArgs args)
    {
      if (args.get_Filter().get_IsEmpty() && this.filterIsEmpty != CategoryContainer.FilterState.Empty || !args.get_Filter().get_IsEmpty() && this.filterIsEmpty != CategoryContainer.FilterState.NotEmpty)
      {
        if (args.get_Filter().get_IsEmpty())
        {
          if (this.haveCachedExpanded)
          {
            this.Category.Expanded = this.wasExpandedBeforeFilter;
            this.AdvancedSectionPinned = this.wasAdvancedPinnedBeforeFilter;
          }
        }
        else
        {
          this.haveCachedExpanded = true;
          this.wasExpandedBeforeFilter = this.Category.Expanded;
          this.wasAdvancedPinnedBeforeFilter = this.AdvancedSectionPinned;
        }
      }
      if (!args.get_Filter().get_IsEmpty())
      {
        this.Category.Expanded = this.BasicPropertyMatchesFilter || this.AdvancedPropertyMatchesFilter;
        this.AdvancedSectionPinned = this.AdvancedPropertyMatchesFilter;
      }
      this.filterIsEmpty = args.get_Filter().get_IsEmpty() ? CategoryContainer.FilterState.Empty : CategoryContainer.FilterState.NotEmpty;
    }

    private void OnMinimizeButtonClick(object sender, RoutedEventArgs e)
    {
      Popup popupHost = CategoryContainer.GetPopupHost((DependencyObject) this);
      if (popupHost == null)
        return;
      popupHost.IsOpen = false;
    }

    private static void OnAdvancedPropertyMatchesFilterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      CategoryContainer categoryContainer = d as CategoryContainer;
      if (categoryContainer == null)
        return;
      categoryContainer.CoerceValue(CategoryContainer.ShowAdvancedHeaderProperty);
    }

    private static object CoerceShowAdvancedHeader(DependencyObject d, object value)
    {
      CategoryContainer categoryContainer = d as CategoryContainer;
      if (categoryContainer != null && (categoryContainer.unconsumedAdvancedProperties.Count <= 0 && categoryContainer.advancedCategoryEditors.Count == 0 || !categoryContainer.AdvancedPropertyMatchesFilter))
        return (object) false;
      return (object) true;
    }

    protected override void OnInitialized(EventArgs e)
    {
      base.OnInitialized(e);
      this.Loaded += new RoutedEventHandler(this.CategoryContainer_Loaded);
    }

    private void CategoryContainer_Loaded(object sender, RoutedEventArgs e)
    {
      IPropertyInspector propertyInspectorModel = PropertyInspectorHelper.GetOwningPropertyInspectorModel((DependencyObject) this);
      if (propertyInspectorModel == null || CategoryContainer.GetPopupHost((DependencyObject) this) != null)
        return;
      this.Category.Expanded = propertyInspectorModel.IsCategoryExpanded(this.Category.get_CategoryName());
    }

    public static bool GetScrollIntoViewWhenLoaded(DependencyObject target)
    {
      return (bool) target.GetValue(CategoryContainer.ScrollIntoViewWhenLoadedProperty);
    }

    public static void SetScrollIntoViewWhenLoaded(DependencyObject target, bool value)
    {
      target.SetValue(CategoryContainer.ScrollIntoViewWhenLoadedProperty, (object) (bool) (value ? 1 : 0));
    }

    private static void OnScrollIntoViewWhenLoadedPropertyChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
    {
      if (!(bool) target.GetValue(CategoryContainer.ScrollIntoViewWhenLoadedProperty))
        return;
      FrameworkElement frameworkElement = target as FrameworkElement;
      if (frameworkElement == null || !frameworkElement.IsLoaded)
        return;
      frameworkElement.BringIntoView();
    }

    private void AdvancedProperties_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      switch (e.Action)
      {
        case NotifyCollectionChangedAction.Add:
          IEnumerator enumerator1 = e.NewItems.GetEnumerator();
          try
          {
            while (enumerator1.MoveNext())
              this.AddProperty((PropertyEntry) enumerator1.Current, this.unconsumedAdvancedProperties, this.Category.AdvancedProperties, this.advancedCategoryEditors);
            break;
          }
          finally
          {
            IDisposable disposable = enumerator1 as IDisposable;
            if (disposable != null)
              disposable.Dispose();
          }
        case NotifyCollectionChangedAction.Remove:
          IEnumerator enumerator2 = e.OldItems.GetEnumerator();
          try
          {
            while (enumerator2.MoveNext())
              this.unconsumedAdvancedProperties.Remove((PropertyEntry) enumerator2.Current);
            break;
          }
          finally
          {
            IDisposable disposable = enumerator2 as IDisposable;
            if (disposable != null)
              disposable.Dispose();
          }
      }
      this.CoerceValue(CategoryContainer.ShowAdvancedHeaderProperty);
    }

    private void BasicProperties_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      switch (e.Action)
      {
        case NotifyCollectionChangedAction.Add:
          IEnumerator enumerator1 = e.NewItems.GetEnumerator();
          try
          {
            while (enumerator1.MoveNext())
              this.AddProperty((PropertyEntry) enumerator1.Current, this.unconsumedBasicProperties, this.Category.BasicProperties, this.basicCategoryEditors);
            break;
          }
          finally
          {
            IDisposable disposable = enumerator1 as IDisposable;
            if (disposable != null)
              disposable.Dispose();
          }
        case NotifyCollectionChangedAction.Remove:
          IEnumerator enumerator2 = e.OldItems.GetEnumerator();
          try
          {
            while (enumerator2.MoveNext())
              this.unconsumedBasicProperties.Remove((PropertyEntry) enumerator2.Current);
            break;
          }
          finally
          {
            IDisposable disposable = enumerator2 as IDisposable;
            if (disposable != null)
              disposable.Dispose();
          }
      }
    }

    private bool IsAdvanced(CategoryEditor editor)
    {
      AttributeCollection attributeCollection = (AttributeCollection) null;
      try
      {
        attributeCollection = TypeDescriptor.GetAttributes((object) editor);
      }
      catch (Exception ex)
      {
      }
      if (attributeCollection != null)
      {
        foreach (Attribute attribute in attributeCollection)
        {
          EditorBrowsableAttribute browsableAttribute = attribute as EditorBrowsableAttribute;
          if (browsableAttribute != null)
            return browsableAttribute.State == EditorBrowsableState.Advanced;
        }
      }
      return false;
    }

    private void AddCategoryEditors(IList editors)
    {
      foreach (CategoryEditor categoryEditor in (IEnumerable) editors)
      {
        if (this.IsAdvanced(categoryEditor))
        {
          this.advancedCategoryEditors.Add(categoryEditor);
          this.UpdateUnconsumedProperties(categoryEditor, this.unconsumedAdvancedProperties);
        }
        else
        {
          this.basicCategoryEditors.Add(categoryEditor);
          this.UpdateUnconsumedProperties(categoryEditor, this.unconsumedBasicProperties);
        }
      }
    }

    private void UpdateUnconsumedProperties(CategoryEditor newEditor, ObservableCollection<PropertyEntry> unconsumedProperties)
    {
      for (int index = unconsumedProperties.Count - 1; index >= 0; --index)
      {
        if (newEditor.ConsumesProperty(unconsumedProperties[index]))
          unconsumedProperties.RemoveAt(index);
      }
    }

    private void RemoveCategoryEditors(IList editors)
    {
      bool flag1 = false;
      bool flag2 = false;
      foreach (CategoryEditor editor in (IEnumerable) editors)
      {
        if (this.IsAdvanced(editor))
        {
          this.advancedCategoryEditors.Remove(editor);
          flag2 = true;
        }
        else
        {
          this.basicCategoryEditors.Remove(editor);
          flag1 = true;
        }
      }
      if (this.Category == null)
        return;
      if (flag1)
        this.RefreshConsumedProperties(this.unconsumedBasicProperties, this.Category.BasicProperties, this.basicCategoryEditors);
      if (!flag2)
        return;
      this.RefreshConsumedProperties(this.unconsumedAdvancedProperties, this.Category.AdvancedProperties, this.advancedCategoryEditors);
    }

    private void RefreshConsumedProperties(ObservableCollection<PropertyEntry> unconsumedProperties, ObservableCollection<PropertyEntry> allProperties, ObservableCollection<CategoryEditor> categoryEditors)
    {
      if (allProperties == null || unconsumedProperties == null || unconsumedProperties.Count == allProperties.Count)
        return;
      using (IEnumerator<PropertyEntry> enumerator = allProperties.GetEnumerator())
      {
        while (((IEnumerator) enumerator).MoveNext())
        {
          PropertyEntry current = enumerator.Current;
          if (!unconsumedProperties.Contains(current))
            this.AddProperty(current, unconsumedProperties, allProperties, categoryEditors);
        }
      }
    }

    private void CategoryEditors_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      switch (e.Action)
      {
        case NotifyCollectionChangedAction.Add:
          this.AddCategoryEditors(e.NewItems);
          break;
        case NotifyCollectionChangedAction.Remove:
          this.RemoveCategoryEditors(e.OldItems);
          break;
        case NotifyCollectionChangedAction.Replace:
          this.RemoveCategoryEditors(e.OldItems);
          this.AddCategoryEditors(e.NewItems);
          break;
        case NotifyCollectionChangedAction.Reset:
          this.basicCategoryEditors.Clear();
          this.advancedCategoryEditors.Clear();
          break;
      }
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent(this, new Uri("/Microsoft.Expression.DesignSurface;component/properties/propertyinspector/categorycontainer.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    internal Delegate _CreateDelegate(Type delegateType, string handler)
    {
      return Delegate.CreateDelegate(delegateType, this, handler);
    }

    [DebuggerNonUserCode]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.UserControlSelf = (CategoryContainer) target;
          break;
        case 2:
          this.PaletteBGrec = (Border) target;
          break;
        case 3:
          this._basicEditors = (CategoryLayoutContainer) target;
          break;
        case 4:
          this._basicProperties = (StandardCategoryLayout) target;
          break;
        case 5:
          this.PopupHost = (Expander) target;
          break;
        case 6:
          this._advancedEditors = (CategoryLayoutContainer) target;
          break;
        case 7:
          this._advancedProperties = (StandardCategoryLayout) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }

    private enum FilterState
    {
      Unknown,
      Empty,
      NotEmpty,
    }
  }
}
