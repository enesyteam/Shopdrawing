// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.PropertyInspector.CollectionDialogEditor
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
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Microsoft.Expression.Framework.PropertyInspector
{
  public class CollectionDialogEditor : UserControl, INotifyPropertyChanged
  {
    public static readonly DependencyProperty PropertyValueProperty = DependencyProperty.Register("PropertyValue", typeof (PropertyValue), typeof (CollectionDialogEditor), new PropertyMetadata(new PropertyChangedCallback(CollectionDialogEditor.PropertyValueChanged)));
    private int indexToSelect = -1;
    private CollectionDialogEditorModel model;
    private ICollectionView childrenView;
    private ICollectionView quickTypes;
    private ObservableCollection<NewItemFactoryTypeModel> quickTypeCollection;
    private DelegateCommand addItemCommand;
    private DelegateCommand removeItemCommand;
    private DelegateCommand moveUpCommand;
    private DelegateCommand moveDownCommand;
    private object primitiveEditorContent;
    private Visibility primitiveEditorVisibility;
    private DataTemplate primitiveEditorTemplate;

    public PropertyValue PropertyValue
    {
      get
      {
        return (PropertyValue) this.GetValue(CollectionDialogEditor.PropertyValueProperty);
      }
      set
      {
        this.SetValue(CollectionDialogEditor.PropertyValueProperty, value);
      }
    }

    public IList<CategoryBase> Categories
    {
      get
      {
        return this.model.Categories;
      }
    }

    public ICollectionView ChildrenView
    {
      get
      {
        return this.childrenView;
      }
    }

    public ICollectionView QuickTypes
    {
      get
      {
        return this.quickTypes;
      }
    }

    public Visibility PrimitiveEditorVisibility
    {
      get
      {
        return this.primitiveEditorVisibility;
      }
    }

    public object PrimitiveEditorContent
    {
      get
      {
        return this.primitiveEditorContent;
      }
    }

    public DataTemplate PrimitiveEditorTemplate
    {
      get
      {
        return this.primitiveEditorTemplate;
      }
    }

    public ICommand AddItemCommand
    {
      get
      {
        return (ICommand) this.addItemCommand;
      }
    }

    public ICommand RemoveItemCommand
    {
      get
      {
        return (ICommand) this.removeItemCommand;
      }
    }

    public ICommand MoveUpCommand
    {
      get
      {
        return (ICommand) this.moveUpCommand;
      }
    }

    public ICommand MoveDownCommand
    {
      get
      {
        return (ICommand) this.moveDownCommand;
      }
    }

    private bool IsMoveUpEnabled
    {
      get
      {
        if (this.childrenView.CurrentPosition > 0)
          return this.model.CanMoveItem;
        return false;
      }
    }

    private bool IsMoveDownEnabled
    {
      get
      {
        if (this.model.CanMoveItem && this.childrenView.CurrentPosition != -1)
          return this.childrenView.CurrentPosition < this.PropertyValue.get_Collection().get_Count() - 1;
        return false;
      }
    }

    private bool IsRemoveEnabled
    {
      get
      {
        if (this.childrenView.CurrentItem is PropertyValue)
          return this.model.CanRemoveItem;
        return false;
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public CollectionDialogEditor(CollectionDialogEditorModel model)
    {
      this.model = model;
      this.model.Host = this;
      PropertyInspectorHelper.SetOwningPropertyInspectorElement((DependencyObject) this, (UIElement) this);
      PropertyInspectorHelper.SetOwningPropertyInspectorModel((DependencyObject) this, (IPropertyInspector) model);
      this.quickTypeCollection = new ObservableCollection<NewItemFactoryTypeModel>();
      this.quickTypes = CollectionViewSource.GetDefaultView(this.quickTypeCollection);
      this.addItemCommand = new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.AddNewItem));
      this.removeItemCommand = new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.RemoveItem));
      this.removeItemCommand.IsEnabled = false;
      this.moveUpCommand = new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.MoveUp));
      this.moveUpCommand.IsEnabled = false;
      this.moveDownCommand = new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.MoveDown));
      this.moveDownCommand.IsEnabled = false;
      this.SetValue(DesignerProperties.IsInDesignModeProperty, (object) false);
      this.Unloaded += new RoutedEventHandler(this.CollectionDialogEditor_Unloaded);
    }

    private static void PropertyValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      CollectionDialogEditor collectionDialogEditor = (CollectionDialogEditor) sender;
      collectionDialogEditor.model.PropertyValueChanged();
      collectionDialogEditor.Rebuild();
    }

    private void RemoveEventHandlers()
    {
      if (this.childrenView != null)
      {
        this.childrenView.CurrentChanged -= new EventHandler(this.OnChildrenViewCurrentChanged);
        this.childrenView.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.OnCollectionChanged);
      }
      if (this.quickTypes == null)
        return;
      this.quickTypes.CurrentChanged -= new EventHandler(this.OnQuickTypesCurrentChanged);
    }

    private void Rebuild()
    {
      this.RemoveEventHandlers();
      if (this.PropertyValue != null)
      {
        this.childrenView = CollectionViewSource.GetDefaultView(this.PropertyValue.get_Collection());
        this.childrenView.CurrentChanged += new EventHandler(this.OnChildrenViewCurrentChanged);
        this.childrenView.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnCollectionChanged);
        this.childrenView.MoveCurrentTo(null);
        this.UpdateSelectedItem();
        using (IEnumerator<KeyValuePair<Type, IList<NewItemFactory>>> enumerator1 = ((IEnumerable<KeyValuePair<Type, IList<NewItemFactory>>>) ExtensibilityMetadataHelper.GetNewItemFactoriesFromAttributes(this.model.GetNewItemTypesAttributes(), this.model.MessageLoggingService)).GetEnumerator())
        {
          while (((IEnumerator) enumerator1).MoveNext())
          {
            KeyValuePair<Type, IList<NewItemFactory>> current1 = enumerator1.Current;
            using (IEnumerator<NewItemFactory> enumerator2 = ((IEnumerable<NewItemFactory>) current1.Value).GetEnumerator())
            {
              while (((IEnumerator) enumerator2).MoveNext())
              {
                NewItemFactory current2 = enumerator2.Current;
                this.quickTypeCollection.Add(new NewItemFactoryTypeModel(current1.Key, current2, this.model.MessageLoggingService));
              }
            }
          }
        }
        this.quickTypes.CurrentChanged += new EventHandler(this.OnQuickTypesCurrentChanged);
      }
      else
        this.childrenView = (ICollectionView) null;
      this.OnPropertyChanged("ChildrenView");
    }

    private void CollectionDialogEditor_Unloaded(object sender, RoutedEventArgs e)
    {
      this.RemoveEventHandlers();
    }

    private void OnQuickTypesCurrentChanged(object sender, EventArgs e)
    {
      if (this.quickTypes.CurrentPosition < 0)
        return;
      this.AddItem();
    }

    private void AddNewItem()
    {
      this.quickTypes.MoveCurrentTo((object) -1);
      this.AddItem();
    }

    private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.UpdateSelectedItem();
    }

    private void UpdateSelectedItem()
    {
      int count = this.PropertyValue.get_Collection().get_Count();
      if (count <= 0)
        return;
      if (this.indexToSelect >= count)
        this.childrenView.MoveCurrentToLast();
      else if (this.indexToSelect == -1)
        this.childrenView.MoveCurrentToFirst();
      else
        this.childrenView.MoveCurrentToPosition(this.indexToSelect);
    }

    private void OnChildrenViewCurrentChanged(object sender, EventArgs e)
    {
      this.model.Categories.Clear();
      PropertyValue activePropertyValue = this.childrenView.CurrentItem as PropertyValue;
      this.removeItemCommand.IsEnabled = this.IsRemoveEnabled;
      this.moveUpCommand.IsEnabled = this.IsMoveUpEnabled;
      this.moveDownCommand.IsEnabled = this.IsMoveDownEnabled;
      this.primitiveEditorContent = null;
      this.primitiveEditorVisibility = Visibility.Collapsed;
      if (activePropertyValue != null)
      {
        Type selectedType = activePropertyValue.get_ParentProperty().get_PropertyType();
        object obj = activePropertyValue.get_Value();
        if (obj != null)
          selectedType = obj.GetType();
        if (selectedType.IsPrimitive || selectedType == typeof (string) || activePropertyValue.get_SubProperties().get_Count() == 0)
        {
          this.primitiveEditorVisibility = Visibility.Visible;
          PropertyValueEditor propertyValueEditor = activePropertyValue.get_ParentProperty().get_PropertyValueEditor();
          this.primitiveEditorContent = (object) activePropertyValue;
          this.primitiveEditorTemplate = propertyValueEditor == null ? this.TryFindResource((object) "PropertyContainerDefaultInlineTemplate") as DataTemplate : propertyValueEditor.get_InlineEditorTemplate();
        }
        else
        {
          using (IEnumerator<PropertyEntry> enumerator = activePropertyValue.get_SubProperties().GetEnumerator())
          {
            while (((IEnumerator) enumerator).MoveNext())
            {
              PropertyEntry current = enumerator.Current;
              CategoryBase orCreateCategory = this.model.FindOrCreateCategory(current.get_CategoryName(), selectedType);
              if (current.get_IsAdvanced())
                orCreateCategory.AdvancedProperties.Add(current);
              else
                orCreateCategory.BasicProperties.Add(current);
            }
          }
        }
      }
      this.OnPropertyChanged("PrimitiveEditorVisibility");
      this.OnPropertyChanged("PrimitiveEditorContent");
      this.OnPropertyChanged("PrimitiveEditorTemplate");
      this.model.OnRebuildComplete(activePropertyValue);
      this.indexToSelect = this.childrenView.CurrentPosition;
    }

    private void AddItem()
    {
      if (this.PropertyValue == null)
        return;
      int currentPosition = this.ChildrenView.CurrentPosition;
      int num = currentPosition != -1 ? currentPosition + 1 : this.PropertyValue.get_Collection().get_Count();
      NewItemFactoryTypeModel factoryTypeModel = this.quickTypes.CurrentItem as NewItemFactoryTypeModel;
      object obj = null;
      if (factoryTypeModel != null)
      {
        try
        {
          obj = factoryTypeModel.CreateInstance();
        }
        catch (Exception ex)
        {
          this.model.MessageLoggingService.WriteLine(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ExceptionStringTable.CollectionDialogEditorCollectionItemFactoryInstantiateFailed, new object[2]
          {
            (object) ((object) factoryTypeModel.ItemFactory).GetType().Name,
            (object) ExtensibilityMetadataHelper.GetExceptionMessage(ex)
          }));
        }
      }
      else
        obj = this.model.CreateType((Type) null);
      if (obj == null)
        return;
      this.indexToSelect = num;
      this.PropertyValue.get_Collection().Insert(obj, num);
    }

    private void RemoveItem()
    {
      if (this.PropertyValue == null)
        return;
      int currentPosition = this.ChildrenView.CurrentPosition;
      if (currentPosition == -1)
        return;
      this.indexToSelect = currentPosition - 1;
      this.PropertyValue.get_Collection().RemoveAt(currentPosition);
    }

    private void MoveUp()
    {
      if (this.PropertyValue == null)
        return;
      int currentPosition = this.ChildrenView.CurrentPosition;
      if (currentPosition <= 0)
        return;
      this.indexToSelect = currentPosition - 1;
      this.PropertyValue.get_Collection().SetIndex(currentPosition, currentPosition - 1);
    }

    private void MoveDown()
    {
      if (this.PropertyValue == null)
        return;
      int currentPosition = this.ChildrenView.CurrentPosition;
      if (currentPosition == -1 || currentPosition + 1 >= this.PropertyValue.get_Collection().get_Count())
        return;
      this.indexToSelect = currentPosition + 1;
      this.PropertyValue.get_Collection().SetIndex(currentPosition, currentPosition + 1);
    }

    protected void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
