// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.SceneNodeCategory
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Data;
using Microsoft.Expression.Framework.PropertyInspector;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public class SceneNodeCategory : CategoryBase
  {
    private static SceneNodeCategory.PropertyComparer propertyComparer = new SceneNodeCategory.PropertyComparer();
    public static readonly DependencyProperty AttachedPropertyEntryProperty = DependencyProperty.RegisterAttached("AttachedPropertyEntry", typeof (PropertyEntry), typeof (SceneNodeCategory), (PropertyMetadata) new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(SceneNodeCategory.AttachedPropertyEntryChanged)));
    private List<PropertyEntry> attachedPropertyEntries = new List<PropertyEntry>();
    private CategoryLocalizationHelper.CategoryName canonicalName;
    private SceneNodeCategory.CategoryOrder order;
    private ObservableCollectionWorkaround<PropertyEntry> basicProperties;
    private ObservableCollectionWorkaround<PropertyEntry> advancedProperties;
    private ImageSource highlightedCategoryIcon;
    private IMessageLoggingService messageLogger;
    private bool isTargetingFrameworkElement;

    public ImageSource HighlightedCategoryIcon
    {
      get
      {
        return this.highlightedCategoryIcon;
      }
      private set
      {
        this.highlightedCategoryIcon = value;
        this.OnPropertyChanged("HighlightedCategoryIcon");
      }
    }

    public CategoryLocalizationHelper.CategoryName CanonicalName
    {
      get
      {
        return this.canonicalName;
      }
    }

    public override IComparable SortOrdering
    {
      get
      {
        return (IComparable) this.order;
      }
    }

    public virtual bool IsEmpty
    {
      get
      {
        if (this.basicProperties.Count == 0)
          return this.advancedProperties.Count == 0;
        return false;
      }
    }

    public bool IsTargetingFrameworkElement
    {
      get
      {
        return this.isTargetingFrameworkElement;
      }
      private set
      {
        if (this.isTargetingFrameworkElement == value)
          return;
        this.isTargetingFrameworkElement = value;
        this.OnPropertyChanged("IsTargetingFrameworkElement");
      }
    }

    public override ObservableCollection<PropertyEntry> BasicProperties
    {
      get
      {
        return (ObservableCollection<PropertyEntry>) this.basicProperties;
      }
    }

    public override ObservableCollection<PropertyEntry> AdvancedProperties
    {
      get
      {
        return (ObservableCollection<PropertyEntry>) this.advancedProperties;
      }
    }

    public virtual CategoryHelpContext CategoryHelpContext
    {
      get
      {
        return new CategoryHelpContext(this.get_CategoryName(), string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.DefaultCategoryHelpToolTipFormat, new object[1]
        {
          this.get_CategoryName()
        }));
      }
    }

    public SceneNodeCategory(CategoryLocalizationHelper.CategoryName canonicalName, string localizedName, IMessageLoggingService messageLogger)
      : base(localizedName)
    {
      this.canonicalName = canonicalName;
      this.messageLogger = messageLogger;
      this.CategoryEditors.CollectionChanged += new NotifyCollectionChangedEventHandler(this.CategoryEditors_CollectionChanged);
      this.basicProperties = new ObservableCollectionWorkaround<PropertyEntry>();
      this.advancedProperties = new ObservableCollectionWorkaround<PropertyEntry>();
      this.order = new SceneNodeCategory.CategoryOrder(this);
      this.InitializeIcons();
    }

    private void InitializeIcons()
    {
      Size size = new Size(24.0, 24.0);
      using (IEnumerator<CategoryEditor> enumerator = this.CategoryEditors.GetEnumerator())
      {
        while (((IEnumerator) enumerator).MoveNext())
        {
          SceneNodeCategoryEditor nodeCategoryEditor = enumerator.Current as SceneNodeCategoryEditor;
          if (nodeCategoryEditor != null)
          {
            this.HighlightedCategoryIcon = (ImageSource) nodeCategoryEditor.GetHighlightedImage(size);
            return;
          }
        }
      }
      if (this.CategoryIcon == null)
      {
        this.CategoryIcon = Microsoft.Expression.DesignSurface.FileTable.GetImageSource("Resources\\Icons\\Categories\\pane_thirdParty_24x24_off.png");
        this.HighlightedCategoryIcon = Microsoft.Expression.DesignSurface.FileTable.GetImageSource("Resources\\Icons\\Categories\\pane_thirdParty_24x24_on.png");
      }
      else
        this.HighlightedCategoryIcon = this.CategoryIcon;
    }

    private void CategoryEditors_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.InitializeIcons();
    }

    internal void AddProperty(SceneNodeProperty property)
    {
      if (((PropertyEntry) property).get_IsAdvanced())
      {
        int index = this.advancedProperties.BinarySearch((PropertyEntry) property, (IComparer<PropertyEntry>) SceneNodeCategory.propertyComparer);
        if (index < 0)
          index = ~index;
        this.advancedProperties.Insert(index, (PropertyEntry) property);
      }
      else
      {
        int index = this.basicProperties.BinarySearch((PropertyEntry) property, (IComparer<PropertyEntry>) SceneNodeCategory.propertyComparer);
        if (index < 0)
          index = ~index;
        this.basicProperties.Insert(index, (PropertyEntry) property);
      }
    }

    public void CullDisassociatedProperties()
    {
      for (int index = this.basicProperties.Count - 1; index >= 0; --index)
      {
        PropertyBase propertyBase = (PropertyBase) this.basicProperties[index];
        if (!propertyBase.Associated && propertyBase.RemoveFromCategoryWhenDisassociated)
        {
          propertyBase.OnRemoveFromCategory();
          this.basicProperties.RemoveAt(index);
        }
      }
      for (int index = this.advancedProperties.Count - 1; index >= 0; --index)
      {
        PropertyBase propertyBase = (PropertyBase) this.advancedProperties[index];
        if (!propertyBase.Associated && propertyBase.RemoveFromCategoryWhenDisassociated)
        {
          propertyBase.OnRemoveFromCategory();
          this.advancedProperties.RemoveAt(index);
        }
      }
    }

    public override void ReportCategoryException(string message)
    {
      if (this.messageLogger == null)
        return;
      this.messageLogger.WriteLine(message);
    }

    public void MarkAllPropertiesDisassociated()
    {
      for (int index = this.basicProperties.Count - 1; index >= 0; --index)
        ((PropertyBase) this.basicProperties[index]).Associated = false;
      for (int index = this.advancedProperties.Count - 1; index >= 0; --index)
        ((PropertyBase) this.advancedProperties[index]).Associated = false;
    }

    internal SceneNodeProperty FindProperty(PropertyReference propertyReference)
    {
      for (int index = 0; index < this.basicProperties.Count; ++index)
      {
        SceneNodeProperty sceneNodeProperty = (SceneNodeProperty) this.basicProperties[index];
        if (sceneNodeProperty.Reference.Path == propertyReference.Path)
          return sceneNodeProperty;
      }
      for (int index = 0; index < this.advancedProperties.Count; ++index)
      {
        SceneNodeProperty sceneNodeProperty = (SceneNodeProperty) this.advancedProperties[index];
        if (sceneNodeProperty.Reference.Path == propertyReference.Path)
          return sceneNodeProperty;
      }
      return (SceneNodeProperty) null;
    }

    public virtual void OnSelectionChanged(SceneNode[] selectedObjects)
    {
      bool flag = true;
      foreach (SceneNode sceneNode in selectedObjects)
      {
        if (!(sceneNode is BaseFrameworkElement))
        {
          StyleNode styleNode = sceneNode as StyleNode;
          if (styleNode == null || !PlatformTypes.FrameworkElement.IsAssignableFrom((ITypeId) styleNode.StyleTargetTypeId))
          {
            flag = false;
            break;
          }
        }
      }
      this.RefreshPropertyCategorization();
      this.IsTargetingFrameworkElement = flag;
    }

    private void RefreshPropertyCategorization()
    {
      for (int index = this.basicProperties.Count - 1; index >= 0; --index)
      {
        PropertyEntry propertyEntry = this.basicProperties[index];
        if (propertyEntry.get_IsAdvanced())
        {
          this.basicProperties.RemoveAt(index);
          this.advancedProperties.Add(propertyEntry);
        }
      }
      for (int index = this.advancedProperties.Count - 1; index >= 0; --index)
      {
        PropertyEntry propertyEntry = this.advancedProperties[index];
        if (!propertyEntry.get_IsAdvanced())
        {
          this.advancedProperties.RemoveAt(index);
          this.basicProperties.Add(propertyEntry);
        }
      }
    }

    public override void ApplyFilter(PropertyFilter filter)
    {
      base.ApplyFilter(filter);
      bool propertyMatchesFilter1 = this.BasicPropertyMatchesFilter;
      bool propertyMatchesFilter2 = this.AdvancedPropertyMatchesFilter;
      using (List<PropertyEntry>.Enumerator enumerator = this.attachedPropertyEntries.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          PropertyEntry current = enumerator.Current;
          if (current.get_IsAdvanced())
            propertyMatchesFilter2 |= this.DoesPropertyMatchFilter(filter, current);
          else
            propertyMatchesFilter1 |= this.DoesPropertyMatchFilter(filter, current);
        }
      }
      this.BasicPropertyMatchesFilter = propertyMatchesFilter1;
      this.AdvancedPropertyMatchesFilter = propertyMatchesFilter2;
      this.OnFilterApplied(filter);
    }

    public static PropertyEntry GetAttachedPropertyEntry(DependencyObject dependencyObject)
    {
      return (PropertyEntry) dependencyObject.GetValue(SceneNodeCategory.AttachedPropertyEntryProperty);
    }

    public static void SetAttachedPropertyEntry(DependencyObject dependencyObject, PropertyEntry value)
    {
      dependencyObject.SetValue(SceneNodeCategory.AttachedPropertyEntryProperty, value);
    }

    private static void AttachedPropertyEntryChanged(DependencyObject source, DependencyPropertyChangedEventArgs args)
    {
      SceneNodeCategory sceneNodeCategory = source.GetValue(FrameworkElement.DataContextProperty) as SceneNodeCategory;
      if (sceneNodeCategory != null)
      {
        if (args.OldValue != null)
          sceneNodeCategory.RemoveAttachedProperty((PropertyEntry) args.OldValue);
        if (args.NewValue == null)
          return;
        sceneNodeCategory.AddAttachedProperty((PropertyEntry) args.NewValue);
      }
      else
        ((FrameworkElement) source).DataContextChanged += new DependencyPropertyChangedEventHandler(SceneNodeCategory.PropertyEditor_DataContextChanged);
    }

    private static void PropertyEditor_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      FrameworkElement frameworkElement = (FrameworkElement) sender;
      SceneNodeCategory sceneNodeCategory1 = e.NewValue as SceneNodeCategory;
      if (sceneNodeCategory1 != null)
      {
        PropertyEntry property = frameworkElement.GetValue(SceneNodeCategory.AttachedPropertyEntryProperty) as PropertyEntry;
        if (property == null)
          return;
        sceneNodeCategory1.AddAttachedProperty(property);
        frameworkElement.DataContextChanged -= new DependencyPropertyChangedEventHandler(SceneNodeCategory.PropertyEditor_DataContextChanged);
      }
      else
      {
        SceneNodeCategory sceneNodeCategory2 = e.OldValue as SceneNodeCategory;
        if (sceneNodeCategory2 == null)
          return;
        PropertyEntry property = frameworkElement.GetValue(SceneNodeCategory.AttachedPropertyEntryProperty) as PropertyEntry;
        if (property == null)
          return;
        sceneNodeCategory2.RemoveAttachedProperty(property);
      }
    }

    private void AddAttachedProperty(PropertyEntry property)
    {
      if (this.BasicProperties.Contains(property) || this.AdvancedProperties.Contains(property) || this.attachedPropertyEntries.Contains(property))
        return;
      this.attachedPropertyEntries.Add(property);
    }

    private void RemoveAttachedProperty(PropertyEntry property)
    {
      this.attachedPropertyEntries.Remove(property);
    }

    private class CategoryOrder : IComparable
    {
      private SceneNodeCategory category;
      private int priority;

      public CategoryOrder(SceneNodeCategory category)
      {
        this.category = category;
        this.priority = (int) category.CanonicalName;
        if (this.priority != -1)
          return;
        this.priority = 100;
      }

      public int CompareTo(object rhs)
      {
        SceneNodeCategory.CategoryOrder categoryOrder = (SceneNodeCategory.CategoryOrder) rhs;
        int num = this.priority - categoryOrder.priority;
        if (num != 0)
          return num;
        return this.category.get_CategoryName().CompareTo(categoryOrder.category.get_CategoryName());
      }
    }

    private class PropertyComparer : System.Collections.Generic.Comparer<PropertyEntry>
    {
      public override int Compare(PropertyEntry x, PropertyEntry y)
      {
        SceneNodeProperty sceneNodeProperty1 = (SceneNodeProperty) x;
        SceneNodeProperty sceneNodeProperty2 = (SceneNodeProperty) y;
        PropertyOrder propertyOrder1 = (PropertyOrder) sceneNodeProperty1.PropertyOrder;
        PropertyOrder propertyOrder2 = (PropertyOrder) sceneNodeProperty2.PropertyOrder;
        int num = !OrderToken.op_Equality((OrderToken) propertyOrder1, (OrderToken) propertyOrder2) ? ((OrderToken) propertyOrder1).CompareTo((OrderToken) propertyOrder2) : 0;
        if (num != 0)
          return num;
        return x.get_PropertyName().CompareTo(y.get_PropertyName());
      }
    }
  }
}
